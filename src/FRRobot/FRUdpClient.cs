using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using fairino;


namespace fairino
{
    public delegate void UdpFrameReceivedHandler(int comType, int count, int cmdID, int contentLen, string content);

    public class FRUdpClient
    {
        private Socket udpSocket;
        private IPEndPoint remoteEndPoint;
        private Thread recvThread;
        private volatile bool runFlag = true;
        private const int RECV_TIMEOUT = 2000;
        /* 固定本地源端口：机器人端 DTLS socket 为连接态，重启后需同源端口才能被接受 */
        private const int LOCAL_BIND_PORT = 20008;

        public UdpFrameReceivedHandler OnFrameReceived { get; set; }

        /// <summary>mTLS 链路（由 Robot 类在初始化时注入；null 或未启用 = 明文模式，与旧版本一致）</summary>
        public MtlsLink Mtls { get; set; }

        public int Connect(string ip, int port)
        {
            try
            {
                Console.WriteLine($"[FRUdpClient] 正在连接到 {ip}:{port}...");
                remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                // 固定本地源端口：机器人端 DTLS socket 是连接态，上位机重启后
                // 同源端口才能被接受（换随机端口会触发 QNX 的 ICMP 不可达拒绝）
                try
                {
                    udpSocket.Bind(new IPEndPoint(IPAddress.Any, LOCAL_BIND_PORT));
                    Console.WriteLine($"[FRUdpClient] bound to local port {LOCAL_BIND_PORT}");
                }
                catch (SocketException)
                {
                    udpSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                    Console.WriteLine("[FRUdpClient] fixed local port busy, fallback to ephemeral (reconnect may fail)");
                }
                //udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, RECV_TIMEOUT);
                //udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, RECV_TIMEOUT);

                Console.WriteLine($"[FRUdpClient] 套接字创建成功，本地是否绑定: {udpSocket.IsBound}");

                /* mTLS 模式：socket 锁定对端地址后发起 DTLS 握手（带重试自愈——
                 * 机器人端刚重启/旧会话未清理时，前几次握手可能失败） */
                if (Mtls != null && Mtls.Enabled)
                {
                    udpSocket.Connect(remoteEndPoint);
                    int tryCount = 0;
                    bool handshakeOk = false;
                    while (tryCount < 3 && !handshakeOk)
                    {
                        tryCount++;
                        try
                        {
                            /* 握手前清残留：上一轮失败握手（吊销拒绝等）留下的
                             * 旧 DTLS 包会污染新一轮握手 */
                            DrainUdpSocket();
                            /* 握手总超时 5s：机器人端未开启加密时不会回 ServerHello，
                             * BC 内部会无限重传卡死——超时后抛异常走重试 */
                            var hsTask = Task.Run(() => Mtls.StartDtls(udpSocket));
                            if (!hsTask.Wait(5000))
                            {
                                throw new TimeoutException("DTLS handshake timeout (robot may not have mTLS enabled)");
                            }
                            hsTask.GetAwaiter().GetResult();
                            Console.WriteLine("[FRUdpClient-DTLS] ############DTLS handshake OK#############");
                            handshakeOk = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[FRUdpClient-DTLS] DTLS handshake attempt {tryCount}/3 failed: {ex.GetType().Name}: {ex.Message}" +
                                              (ex.InnerException != null ? $" (inner: {ex.InnerException.Message})" : ""));
                            /* 超时后旧 socket 仍被后台握手线程占用，重试前重建 */
                            try { udpSocket.Close(); } catch { }
                            if (tryCount < 3)
                            {
                                Console.WriteLine("  retry in 2s ...");
                                Thread.Sleep(2000);
                                udpSocket = CreateUdpSocket(remoteEndPoint);
                            }
                        }
                    }
                    if (!handshakeOk)
                    {
                        /* 握手失败：保持加密模式不降级，直接报错。
                         * 后续发送失败会自动触发 RehandshakeDtls 重试自愈 */
                        Console.WriteLine("[FRUdpClient-DTLS] ############DTLS handshake failed after retries##########");
                        Console.WriteLine("[FRUdpClient-DTLS] 错误：DTLS 握手失败，保持加密模式不降级——" +
                                          "请检查①机器人端加密开关是否开启 ②证书是否被吊销/过期 ③两端证书是否同一套");
                        /* 重建 socket（bind 20008 + connect 锁定对端），供后续重试握手使用 */
                        try { udpSocket.Close(); } catch { }
                        udpSocket = CreateUdpSocket(remoteEndPoint);
                        return -1;
                    }
                }

                // 启动接收线程
                recvThread = new Thread(RobotUDPCmdRecvThread);
                recvThread.IsBackground = true;
                recvThread.Start();
                Console.WriteLine("[FRUdpClient] 接收线程已启动");

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FRUdpClient] Connect error: {ex.Message}");
                return -1;
            }
        }

        /* 创建并绑定本地源端口 + connect 锁定对端的 UDP socket（握手超时重建用） */
        private Socket CreateUdpSocket(IPEndPoint remote)
        {
            var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                s.Bind(new IPEndPoint(IPAddress.Any, LOCAL_BIND_PORT));
            }
            catch (SocketException)
            {
                s.Bind(new IPEndPoint(IPAddress.Any, 0));
                Console.WriteLine("[FRUdpClient] fixed local port busy, fallback to ephemeral (reconnect may fail)");
            }
            s.Connect(remote);
            return s;
        }

        public int Close()
        {
            Console.WriteLine("[FRUdpClient] 正在关闭连接...");
            runFlag = false;

            if (recvThread != null && recvThread.IsAlive)
                recvThread.Join(1000);

            /* 新增：优雅关闭 DTLS（发 close_notify，机器人可感知主动断开） */
            if (Mtls != null && Mtls.Enabled)
            {
                try { Mtls.Dispose(); } catch { }
                Mtls = null;
            }


            if (udpSocket != null)
            {
                udpSocket.Close();
                udpSocket = null;
                Console.WriteLine("[FRUdpClient] 套接字已关闭");
            }

            return 0;
        }

        private void RobotUDPCmdRecvThread()
        {
            byte[] buffer = new byte[2048];
            EndPoint senderRemote = new IPEndPoint(IPAddress.Any, 0);
            Console.WriteLine("[FRUdpClient] 接收线程开始运行");

            while (runFlag)
            {
                try
                {
                    byte[] payload = buffer;
                    int payloadLen;

                    if (Mtls != null && Mtls.Enabled)
                    {
                        /* DTLS 通道：Receive 出来的就是明文帧。
                         * 断线时 Receive 抛异常（ICMP 不可达），空闲时超时返回<=0——
                         * 两者无法直接区分，累计 30 秒无数据才判定会话失效并重握手 */
                        int received = 0;
                        try
                        {
                            received = Mtls.DtlsReceive(buffer, 0, buffer.Length, 2000);
                        }
                        catch
                        {
                            received = -1;
                        }
                        if (received <= 0)
                        {
                            udpIdleTimeout++;
                            if (udpIdleTimeout >= 15)
                            {
                                udpIdleTimeout = 0;
                                RehandshakeDtls();
                            }
                            continue;
                        }
                        udpIdleTimeout = 0;
                        payloadLen = received;
                    }
                    else
                    {
                        int received = udpSocket.ReceiveFrom(buffer, ref senderRemote);
                        if (received <= 0)
                            continue;
                        payloadLen = received;
                    }

                    string receivedStr = Encoding.UTF8.GetString(payload, 0, payloadLen);
                    //Console.WriteLine($"[FRUdpClient] 收到原始数据: {receivedStr} (长度: {received})");

                    List<string> frames = FrameHandle.SplitFrame(receivedStr);
                    //Console.WriteLine($"[FRUdpClient] 分割出 {frames.Count} 个帧");

                    foreach (string frameStr in frames)
                    {
                        FRAME frame = FrameHandle.UnpacketFrame(frameStr);
                        //Console.WriteLine($"[FRUdpClient] 解析帧: count={frame.count}, cmdID={frame.cmdID}, contentLen={frame.contentLen}, content={frame.content}");
                        //OnFrameReceived?.Invoke(0, frame.count, frame.cmdID, frame.contentLen, frame.content);
                        try
                        {
                            OnFrameReceived?.Invoke(0, frame.count, frame.cmdID, frame.contentLen, frame.content);
                        }
                        catch (Exception ex)
                        {
                            // 记录异常，但继续处理后续帧和其他订阅者
                            Console.WriteLine($"[FRUdpClient] 回调执行异常: {ex.Message}");
                            // 可选择记录更详细的日志
                        }
                    }
                }
                //catch (SocketException ex) 
                //{
                //    // 超时正常，继续循环
                //    Console.WriteLine($"[FRUdpClient] receive error: {ex.Message} {ex.ErrorCode}");
                //    continue;
                //}

                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    // 超时正常，继续循环
                    //Console.WriteLine($"[FRUdpClient] receive error: {ex.Message}");
                    continue;
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
                {
                    // 套接字被关闭导致的异常，正常退出循环
                    break;
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"[FRUdpClient] receive error: {ex.Message}");
                    continue;
                }

            }
            Console.WriteLine("[FRUdpClient] 接收线程结束");
        }

        private readonly object dtlsHsLock = new object();   /* 收发线程共用，防并发重握手 */
        private int udpIdleTimeout = 0;                     /* DTLS 连续无数据计数 */
        private DateTime lastUdpHsOkAt = DateTime.MinValue; /* 上次重握手成功时刻（去重） */

        /* 清空 socket 接收缓冲中的残留 DTLS 包。
         * 握手被拒（如吊销）时机器人发过的 ServerHello/证书/alert 可能残留在
         * 缓冲里，下一次握手的 epoch/记录序号从 0 开始与旧包重叠，BC 会把它
         * 们当新握手响应解析 → 状态错乱 → 机器人端 bad signature */
        private void DrainUdpSocket()
        {
            try
            {
                byte[] tmp = new byte[2048];
                while (udpSocket != null && udpSocket.Available > 0)
                    udpSocket.Receive(tmp);
            }
            catch { }
        }

        /* DTLS 重新握手：socket 保持不动（connect 锁定对端不变），只重建会话。
         * 机器人端收到新 ClientHello 会经 0x16 探测关旧会话后重新握手。 */
        private bool RehandshakeDtls()
        {
            lock (dtlsHsLock)
            {
                if (Mtls == null || !Mtls.Enabled || udpSocket == null)
                    return false;
                /* 3 秒内刚重握手成功过：另一触发路径（发送失败/30s无数据）的
                 * 重复调用，直接跳过——避免重连期间连续两次重握手 */
                if (Mtls.DtlsActive && (DateTime.Now - lastUdpHsOkAt).TotalSeconds < 3)
                    return true;
                //Console.WriteLine("[FRUdpClient-DTLS] UDP re-handshaking (socket kept, new DTLS session) ...");
                try
                {
                    Mtls.Dispose();           /* 关旧 transport（旧会话） */
                    DrainUdpSocket();         /* 清旧会话残留包，防污染新握手 */
                    Mtls.StartDtls(udpSocket);
                    lastUdpHsOkAt = DateTime.Now;
                    Console.WriteLine($"[FRUdpClient-DTLS] ############UDP re-handshake OK ({DateTime.Now:HH:mm:ss})#############");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[FRUdpClient-DTLS] UDP re-handshake failed: {ex.Message}, retry later");
                    return false;
                }
            }
        }

        public int SendFrame(string frame)
        {
            if (udpSocket == null)
            {
                Console.WriteLine("[FRUdpClient] 发送失败: udpSocket 为 null");
                return -1;
            }

            try
            {
                //Console.WriteLine($"[FRUdpClient] 准备发送帧: {frame}");
                byte[] data = Encoding.UTF8.GetBytes(frame);

                /* mTLS 模式：DTLS 通道发送（原帧直发，通道自动加密） */
                if (Mtls != null && Mtls.Enabled)
                {
                    try
                    {
                        Mtls.DtlsSend(data);
                        return 0;
                    }
                    catch
                    {
                        /* 发送失败 = 会话已断：重握手后重发一次 */
                        if (RehandshakeDtls())
                        {
                            try
                            {
                                Mtls.DtlsSend(data);
                                return 0;
                            }
                            catch (Exception ex2)
                            {
                                Console.WriteLine($"[FRUdpClient] UDP re-send failed: {ex2.Message}");
                            }
                        }
                        return -1;
                    }
                }
                //Console.WriteLine($"[FRUdpClient] 数据长度: {data.Length} 字节");

                int sent = udpSocket.SendTo(data, remoteEndPoint);
                //Console.WriteLine($"[FRUdpClient] 实际发送字节数: {sent}");

                return sent == data.Length ? 0 : -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FRUdpClient] send error: {ex.Message}");
                return -1;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

        public UdpFrameReceivedHandler OnFrameReceived { get; set; }

        public int Connect(string ip, int port)
        {
            try
            {
                Console.WriteLine($"[FRUdpClient] 正在连接到 {ip}:{port}...");
                remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                // 显式绑定到本地任意可用端口，确保套接字立即处于绑定状态
                udpSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                //udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, RECV_TIMEOUT);
                //udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, RECV_TIMEOUT);

                Console.WriteLine($"[FRUdpClient] 套接字创建成功，本地是否绑定: {udpSocket.IsBound}");

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

        public int Close()
        {
            Console.WriteLine("[FRUdpClient] 正在关闭连接...");
            runFlag = false;

            if (recvThread != null && recvThread.IsAlive)
                recvThread.Join(1000);

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
                    int received = udpSocket.ReceiveFrom(buffer, ref senderRemote);
                    if (received <= 0)
                        continue;

                    string receivedStr = Encoding.UTF8.GetString(buffer, 0, received);
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
                    //Console.WriteLine($"[FRUdpClient] receive error: {ex.Message}");
                    continue;
                }

            }
            Console.WriteLine("[FRUdpClient] 接收线程结束");
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

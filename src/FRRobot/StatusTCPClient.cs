using fairino;
using System.Net.Sockets;
using System.Net;
using System;
using System.Diagnostics;
using System.Threading;

internal class StatusTCPClient
{
    private string ip;
    private int port;

    public Socket mSocket;
    private NetworkStream mNetworkStream;
    private bool isConnected = false;
    private bool reconnEnable = true;  // 重连使能  
    private int reconnTimes = 100;     // 重连次数  
    private int curReconnTimes = 0;    // 当前重连次数  
    private int reconnPeriod = 200;    // 重连时间间隔（毫秒）  
    private int timeOut = 500;    // 发送、接收超时（毫秒） 
    private bool reconnState = false;  // 当前重连状态  
    private Log log;
    private readonly object socketLock = new object();
    // 添加同步锁对象

    public void Close()
    {
        lock (socketLock) // 使用锁确保线程安全
        {
            if (this.mSocket != null)
            {
                try
                {
                    this.mSocket.Shutdown(SocketShutdown.Both);
                    this.mSocket.Close();
                    this.mSocket = null;
                }
                catch (Exception)
                {
                }
            }
            isConnected = false;
            Thread.Sleep(50);
        }
    }

    public bool ReConnect()
    {
        //curReconnTimes = 0;
        //if (!reconnEnable) return false;
        //if (reconnState) return false; // 防止并发重连
        //reconnState = true;

        //if (!reconnEnable)
        //{
        //    reconnState = false;
        //    return false;
        //}

        // 防止并发重连
        lock (socketLock)
        {
            if (reconnState) return false;
            reconnState = true;
        }

        curReconnTimes = 0;
        if (!reconnEnable)
        {
            reconnState = false;
            return false;
        }

        while (curReconnTimes < reconnTimes)
        {
            Close(); // 确保在锁内关闭
            Thread.Sleep(100);
            lock (socketLock) // 确保 Connect 时不会有其他线程干扰
            {

                if (Connect())
                {
                    if (log != null)
                    {
                        log.LogInfo("SDK Disconnected from robot, try to reconnect robot success! ");
                    }
                    Console.WriteLine($"SDK Disconnected from robot, try to reconnect robot success!");
                    reconnState = false;
                    curReconnTimes = 0;
                    return true;
                }
                else
                {
                    curReconnTimes++;
                    if (log != null)
                    {
                        log.LogInfo($"SDK Disconnected from robot, Reconnect TCP attempt {curReconnTimes}/{reconnTimes} failed.");
                    }
                    Console.WriteLine($"SDK Disconnected from robot, Reconnect TCP attempt {curReconnTimes}/{reconnTimes} failed.");
                    //System.Threading.Thread.Sleep(reconnPeriod);
                }
            }
        }
        curReconnTimes = 0;
        reconnState = false;
        return false;
    }
    public StatusTCPClient(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
    }

    public int SetReconnectParam(bool enable, int times, int period)
    {
        try
        {
            reconnEnable = enable;
            reconnTimes = times;
            reconnPeriod = period;
        }
        catch (Exception)
        {
            return -1;
        }
        return 0;
    }

    public bool GetReconnState()
    {
        return reconnState;
    }



    public bool Connect()
    {
        lock (socketLock)
        {
            if (mSocket != null && mSocket.Connected)
            {
                Console.WriteLine("复用现有连接，未进行实际网络连接");
                isConnected = true;
                return true;
            }
            //try
            //{
            //    if (mSocket.Connected && !(mSocket.Poll(100000, SelectMode.SelectRead) && mSocket.Available == 0))
            //    {
            //        Console.WriteLine("复用现有连接，未进行实际网络连接");
            //        isConnected = true;
            //        return true;
            //    }
            //}
            //catch { }
            Close();

            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            try
            {
                IAsyncResult result = mSocket.BeginConnect(iPEndPoint, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(reconnPeriod, true);
                //mSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                if (!success)
                {
                    mSocket.Close();
                    mSocket = null;
                    throw new SocketException((int)SocketError.TimedOut);
                }

                mSocket.EndConnect(result);

                Console.WriteLine("连接成功");

                mSocket.ReceiveTimeout = timeOut;
                mSocket.SendTimeout = timeOut;

                isConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"连接失败：{ex.Message}");
                mSocket = null;
                isConnected = false;
                return false;
            }
        }
    }

    public int RecvPkg(byte[] recvBytes, int recvSize)
    {
        Array.Clear(recvBytes, 0, recvSize);
        int curRecvTotalSize = 0;
        byte[] allRecvBuf = new byte[4 * 1024];
        byte[] tmpRecvBuf = new byte[4 * 1024];

        while (recvSize - curRecvTotalSize > 0)
        {
            Array.Clear(tmpRecvBuf, 0, tmpRecvBuf.Length);

            try
            {
        
                DateTime scheduleStart = DateTime.Now;
                int tmpRecvSize = mSocket.Receive(tmpRecvBuf, recvSize - curRecvTotalSize, SocketFlags.None);
                TimeSpan scheduleDelay = DateTime.Now - scheduleStart;

                if (scheduleDelay.TotalMilliseconds > 10)
                {
                  //  Console.WriteLine($"警告：Receive调用调度延迟 {scheduleDelay.TotalMilliseconds:F1}ms");

                    //// 检查线程优先级
                    //Console.WriteLine($"当前线程优先级: {Thread.CurrentThread.Priority}");

                    //// 检查CPU使用率
                    //PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    //float cpuUsage = cpuCounter.NextValue();
                    //Thread.Sleep(100);
                    //cpuUsage = cpuCounter.NextValue();
                    //Console.WriteLine($"系统CPU使用率: {cpuUsage:F1}%");
                }

                if (tmpRecvSize == 0)
                {
                    // 连接被对方正常关闭
                    Console.WriteLine($"连接被远程主机关闭 (Receive返回0)");
                    Console.WriteLine($"Socket状态: Connected={mSocket.Connected}, Available={mSocket.Available}");

                    Close();
                    bool reconnectSuccess = ReConnect();
                    if (reconnectSuccess)
                    {
                        curRecvTotalSize = 0;
                        continue;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else if (tmpRecvSize < 0)
                {
                    // 接收错误
                    Console.WriteLine($"Receive返回负值: {tmpRecvSize}");
                    int lastError = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    Console.WriteLine($"Win32错误码: {lastError}");

                    if (lastError != 0)
                    {
                        try
                        {
                            Console.WriteLine($"错误描述: {new System.ComponentModel.Win32Exception(lastError).Message}");
                        }
                        catch { }
                    }

                    Close();
                    bool reconnectSuccess = ReConnect();
                    if (reconnectSuccess)
                    {
                        curRecvTotalSize = 0;
                        continue;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    // 正常接收数据
                    Array.Copy(tmpRecvBuf, 0, allRecvBuf, curRecvTotalSize, tmpRecvSize);
                    curRecvTotalSize += tmpRecvSize;
                    //Console.WriteLine($"成功接收 {tmpRecvSize} 字节，累计 {curRecvTotalSize}/{recvSize}");
                }
            }
            catch (SocketException ex)
            {
                // 捕获Socket异常并打印详细错误
                Console.WriteLine($"Socket异常: 错误码={ex.ErrorCode}, Socket错误={ex.SocketErrorCode}");
                Console.WriteLine($"错误消息: {ex.Message}");

                // 常见的错误处理
                if (ex.SocketErrorCode == SocketError.ConnectionReset)
                {
                    Console.WriteLine("连接被远程主机重置");
                }
                else if (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    Console.WriteLine("接收超时");
                }
                else if (ex.SocketErrorCode == SocketError.Shutdown)
                {
                    Console.WriteLine("连接已关闭");
                }

                Close();
                bool reconnectSuccess = ReConnect();
                if (reconnectSuccess)
                {
                    curRecvTotalSize = 0;
                    continue;
                }
                else
                {
                    return -1;
                }
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Socket已被释放");
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"其他异常: {ex.GetType().Name} - {ex.Message}");
                return -1;
            }

            if (recvSize == curRecvTotalSize)
            {
                // Console.WriteLine($"recv all bytes");
                if (allRecvBuf[0] == 0x5A && allRecvBuf[1] == 0x5A)
                {
                    // Console.WriteLine($"find head");
                    UInt16 len = 0;
                    len = (UInt16)(len | allRecvBuf[4]);
                    len = (UInt16)(len << 8);
                    len = (UInt16)(len | allRecvBuf[3]);
                    if (len + 7 > recvSize)  //考虑兼容，低SDK适配高版本机器人
                    {
                        if (len + 7 < 3 * recvSize)  //判断计算出来的数据长度是否在合理区间
                        {
                            recvSize = len + 7;
                            continue;
                        }
                        else   //如果长度差距过大，那可能出错了或SDK版本太老
                        {
                            return -3;
                        }
                    }
                    else if (len + 7 == recvSize)
                    {
                        // Console.WriteLine($"start check");
                        UInt16 checksum = 0;
                        UInt16 checkdata = 0;
                        checkdata = (UInt16)(checkdata | allRecvBuf[recvSize - 1]);
                        checkdata = (UInt16)(checkdata << 8);
                        checkdata = (UInt16)(checkdata | allRecvBuf[recvSize - 2]);

                        for (int j = 0; j < recvSize - 2; j++)
                        {
                            checksum += allRecvBuf[j];
                        }
                        // Console.WriteLine($"recv {checkdata}    compute {checksum}");
                        if (checksum == checkdata)
                        {
                            //Console.WriteLine($"check success");
                            Array.Copy(allRecvBuf, 0, recvBytes, 0, recvSize);  //一切正常
                            return 0;
                        }
                        else
                        {
                            return -2;////和校验失败
                        }
                    }
                    else
                    {
                        return -3;  //SDK 比机器人版本新，得更新机器人版本
                    }

                }
                else
                {
                    return -4;  //帧头校验失败
                }
            }
        }
        return 0;
    }


    public void SetLog(Log logger)
    {
        log = logger;
    }

    public bool IsConnected()
    {
        lock (socketLock)
        {
            if (mSocket == null)
                return false;

            return mSocket.Connected && isConnected;
        }
    }

    public void SetIpPort(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
    }

    public int Send(byte[] data)
    {
        lock (socketLock)
        {
            if (mSocket == null || !mSocket.Connected)
                return -1;
            try
            {
                mSocket.Send(data);
                return data.Length;
            }
            catch
            {
                return -1;
            }
        }
    }

    public int RecvCNDEPkg(byte[] recvBytes, int maxBufferSize)
    {
        byte[] headerBuf = new byte[8];
        int received = 0;
        while (received < 8)
        {
            int n;
            try
            {
                n = mSocket.Receive(headerBuf, received, 8 - received, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket异常: {ex.Message}");
                Close();   // 只关闭，不重连
                return -1;
            }
            if (n == 0)
            {
                Close();
                return -1;   // 连接关闭
            }
            received += n;
        }

        if (headerBuf[0] != 0x5A || headerBuf[1] != 0x5A)
            return -4;

        ushort dataLen = (ushort)(headerBuf[4] | (headerBuf[5] << 8));
        int totalLen = 8 + dataLen;
        if (totalLen > maxBufferSize)
            return -3;

        Array.Copy(headerBuf, 0, recvBytes, 0, 8);
        int receivedTotal = 8;

        while (receivedTotal < totalLen)
        {
            int n;
            try
            {
                n = mSocket.Receive(recvBytes, receivedTotal, totalLen - receivedTotal, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket异常: {ex.Message}");
                Close();
                return -1;
            }
            if (n == 0)
            {
                Close();
                return -1;
            }
            receivedTotal += n;
        }

        if (recvBytes[totalLen - 2] != 0xA5 || recvBytes[totalLen - 1] != 0xA5)
            return -4;

        return totalLen;
    }

}

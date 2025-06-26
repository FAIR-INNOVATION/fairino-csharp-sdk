using fairino;
using System.Net.Sockets;
using System.Net;
using System;

internal class StatusTCPClient
{
    private string ip;
    private int port;

    public Socket mSocket;
    private NetworkStream mNetworkStream;
    private bool isConnected = false;
    private bool reconnEnable = false;  // 重连使能  
    private int reconnTimes = 100;     // 重连次数  
    private int curReconnTimes = 0;    // 当前重连次数  
    private int reconnPeriod = 200;    // 重连时间间隔（毫秒）  
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
        }
    }

    public bool ReConnect()
    {
        curReconnTimes = 0;
        reconnState = true;

        if (!reconnEnable)
        {
            reconnState = false;
            return false;
        }

        while (curReconnTimes < reconnTimes)
        {
            Close(); // 确保在锁内关闭
            lock (socketLock) // 确保 Connect 时不会有其他线程干扰
            {
                if (Connect())
                {
                    if (log != null)
                    {
                        log.LogInfo("SDK Disconnected from robot, try to reconnect robot success! ");
                    }
                    reconnState = false;

                    return true;
                }
                else
                {
                    curReconnTimes++;
                    if (log != null)
                    {
                        log.LogInfo($"SDK Disconnected from robot, try to reconnect robot failed! {curReconnTimes} / {reconnTimes}");
                    }
                    System.Threading.Thread.Sleep(reconnPeriod);
                }
            }
        }
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
                isConnected = true;
                return true;
            }

            Close();

            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            try
            {
                IAsyncResult result = mSocket.BeginConnect(iPEndPoint, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(reconnPeriod, true);

                if (!success)
                {
                    mSocket.Close();
                    mSocket = null;
                    throw new SocketException((int)SocketError.TimedOut);
                }

                mSocket.EndConnect(result);

                Console.WriteLine("连接成功");

                mSocket.ReceiveTimeout = reconnPeriod;
                mSocket.SendTimeout = reconnPeriod;

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
        //Console.WriteLine($"11111111111111   {recvSize}");
        Array.Clear(recvBytes, 0, recvSize);
        int curRecvTotalSize = 0;
        byte[] allRecvBuf = new byte[4 * 1024];
        byte[] tmpRecvBuf = new byte[4 * 1024];
        while (recvSize - curRecvTotalSize > 0)  //还有数据未接收
        {
            Array.Clear(tmpRecvBuf, 0, tmpRecvBuf.Length);

            int tmpRecvSize = mSocket.Receive(tmpRecvBuf, recvSize - curRecvTotalSize, SocketFlags.None);
            // Console.WriteLine($"recv length {tmpRecvSize}...");
            if (tmpRecvSize <= 0)
            {
                Close();
                bool reconnectSuccess = ReConnect();
                if (reconnectSuccess)
                {
                    curRecvTotalSize = 0;
                    continue;
                }
                else
                {
                    return -1;   //接收数据异常
                }
            }
            else
            {
                Array.Copy(tmpRecvBuf, 0, allRecvBuf, curRecvTotalSize, tmpRecvSize);
                curRecvTotalSize += tmpRecvSize;
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
}

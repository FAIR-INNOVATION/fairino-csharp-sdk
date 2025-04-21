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

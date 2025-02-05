using fairino;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Fairino
{
    // FR机器人TCP通信类  
    internal class TCPClient
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

        public TCPClient(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        /// <summary>  
        /// 设置TCPClient重连参数  
        /// </summary>  
        /// <param name="enable">是否使能，true:使能，false:不使能</param>  
        /// <param name="times">重连次数</param>  
        /// <param name="period">重连时间间隔（毫秒）</param>  
        /// <returns>错误码（本例中始终返回0）</returns>  
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

        /// <summary>  
        /// 获取TCPClient重连状态  
        /// </summary>  
        /// <returns>重连状态，true：正在重连，false：未重连</returns>  
        public bool GetReconnState()
        {
            return reconnState;
        }

        /// <summary>  
        /// TCPClient重连  
        /// </summary>  
        /// <returns>重连状态，true：重连成功，false：重连失败</returns>  
        public bool ReConnect()
        {
            curReconnTimes = 0;
            reconnState = true;
            //if (reconnEnable==true)
            //{
            //    Console.WriteLine("reconnEnable" + reconnEnable);
            //}
            if (!reconnEnable)
            {
               
                reconnState = false;
                return false;
            }
     
            while (curReconnTimes < reconnTimes)
            {
                Close();
                if (Connect())
                {
                    if (log != null)
                    {
                        
                        log.LogInfo("SDK Disconnected from robot, try to reconnect robot success! ");
                    }
                    Console.WriteLine("SDK Disconnected from robot, try to reconnect robot success! ");
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
                    System.Threading.Thread.Sleep(reconnPeriod); // 等待重连间隔  
                }
            }
            reconnState = false;
            return false;
        }

        public bool Connect()
        {
            this.mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            try
            {
                this.mSocket.Connect(iPEndPoint);
                Console.WriteLine("连接成功");
                //mNetworkStream = new NetworkStream(mSocket);
                //mSocket.NoDelay = true;
                //mSocket.ReceiveTimeout = reconnPeriod;
                //mSocket.SendTimeout = reconnPeriod;
                this.isConnected = true;
                return this.isConnected;
            }
            catch (Exception)
            {
                this.isConnected = false;
                return this.isConnected;
               
            }
           
        }

        public void Close()
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

        public void SetLog(Log logger)
        {
            log = logger;
        }

        public bool IsConnected()
        {
            return isConnected;
        }
    }



}
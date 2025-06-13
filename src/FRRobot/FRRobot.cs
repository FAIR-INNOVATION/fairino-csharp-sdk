using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.RuntimeHelpers;
//using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Linq;
using System.Data;
using System.Text.RegularExpressions;
using static System.Windows.Forms.AxHost;
using System.Drawing.Drawing2D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Globalization;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Runtime.Remoting.Channels;
using System.Security.AccessControl;
using System.IO.Ports;
using Fairino;
using System.Collections;
using System.Threading.Tasks;

namespace fairino
{
    public class Robot
    {
        ICallSupervisor proxy = null;

        const string SDK_VERSION = "C#SDK V1.1.3";

        private string robot_ip = "192.168.58.2";//机器人ip
        private int g_sock_com_err = (int)RobotError.ERR_SUCCESS;
        const int ROBOT_REALTIME_PORT = 20004;
        const int ROBOT_CMD_PORT = 8080;
        const int BUFFER_SIZE = 1024 * 4;
        const int MAX_CHECK_CNT_COM = 25;
        const int MAX_UPLOAD_FILE_SIZE = 500 * 1024 * 1024;//最大上传文件为2Mb
        private const int DOWNLOAD_POINT_TABLE_PORT = 20011;//点位表下载
        //private Socket sock_cli_cmd;//实时指令发送接收通讯
        //private Socket sock_cli_state;//实时状态反馈通讯
        private byte robot_realstate_exit = 0;//实时状态反馈线程循环标志
        private byte robot_instcmd_send_exit = 0;//实时发送数据循环标志
        private byte robot_instcmd_recv_exit = 0;//实时接受指令循环标志
        private byte robot_task_exit = 0;//检测通讯状态线程循环标志
        private bool is_sendcmd = false;
        private string g_sendbuf = "";
        private string g_recvbuf = "";

        private bool reconnEnable = false;  // 重连使能  
        private int reconnTimes = 1000;     // 重连次数  
        private int curReconnTimes = 0;    // 当前重连次数  
        private int reconnPeriod = 200;    // 重连时间间隔（毫秒）  
        private bool reconnState = false;  // 当前重连状态  

        private ROBOT_STATE_PKG robot_state_pkg;//状态反馈结构体

        private byte s_last_frame_cnt = 0;//上一个指令计数

        private int PauseMotionCnt = 0;

        private int ResumeMotionCnt = 0;

        private Log log = null;
        private int? _conveyorCounter = null;//传送带跟踪检测
        private double fileUploadPercent = 0;
        private StatusTCPClient sock_cli_state; //实时状态反馈通讯
        private TCPClient sock_cli_cmd;//实时指令发送接收通讯

        private int ReceivePortTimeout = 200;//20004端口接受超时时间
        public Robot()
        {
            proxy = XmlRpcProxyGen.Create<ICallSupervisor>();
        }

        /// <summary>  
        /// 设置重连参数  
        /// </summary>  
        /// <param name="enable">是否使能，true:使能，false:不使能</param>  
        /// <param name="times">重连次数</param>  
        /// <param name="period">重连时间间隔（毫秒）</param>  
        public void SetReconnectParam(bool enable, int times, int period)
        {
            reconnEnable = enable;
            reconnTimes = times;
            reconnPeriod = period;
        }

        private void RobotStateRoutineThread()
        {
            byte[] recvbuf = new byte[BUFFER_SIZE];
            byte[] tmp_recvbuf = new byte[BUFFER_SIZE];
            byte[] state_pkg = new byte[BUFFER_SIZE];
            int i;
            byte find_head_flag = 0;
            UInt16 index = 0;
            UInt16 len = 0;
            UInt16 tmp_len = 0;
            int recvbyte = 0;

            long start = 0;

            sock_cli_state = new StatusTCPClient(robot_ip, ROBOT_REALTIME_PORT);
            sock_cli_state.SetReconnectParam(reconnEnable, reconnTimes, reconnPeriod);//断线重连参数
            sock_cli_state.SetLog(log);
            bool brtn = sock_cli_state.Connect();
            if (!brtn)
            {
                g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
                if (log != null)
                {
                    log.LogError("sdk connect robot runtime fail");
                }
                return;
            }
            try
            {
                while (robot_realstate_exit == 0)
                {
                    try
                    {
                        // 在访问 Socket 前检查连接状态
                        if (!sock_cli_state.IsConnected())
                        {
                            Console.WriteLine("Socket is not connected, attempting reconnect...");
                            if (!sock_cli_state.ReConnect())
                            {
                                g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
                                break; // 重连失败则退出循环
                            }
                        }

                        // 确保 Socket 不为 null
                        if (sock_cli_state.mSocket == null)
                        {
                            Console.WriteLine("Socket is null, reconnecting...");
                            sock_cli_state.ReConnect();
                            continue;
                        }
                        //DateTime dateTimeS = DateTime.Now;
                        //start = (long)(dateTimeS.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

                        // 设置 Receive 超时时间为 100ms
                        sock_cli_state.mSocket.ReceiveTimeout = ReceivePortTimeout;

                        // 启用 TCP KeepAlive
                        sock_cli_state.mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                        uint keepAliveTime = 500;    // 1秒后开始探测
                        uint keepAliveInterval = 500; // 每次探测间隔1秒
                        byte[] keepAliveConfig = new byte[12];
                        BitConverter.GetBytes((uint)1).CopyTo(keepAliveConfig, 0);
                        BitConverter.GetBytes(keepAliveTime).CopyTo(keepAliveConfig, 4);
                        BitConverter.GetBytes(keepAliveInterval).CopyTo(keepAliveConfig, 8);
                        sock_cli_state.mSocket.IOControl(IOControlCode.KeepAliveValues, keepAliveConfig, null);

                        // 接收数据
                        recvbyte = sock_cli_state.mSocket.Receive(recvbuf);

                        //DateTime dateTimeE = DateTime.Now;
                        //long end = (long)(dateTimeE.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                        //Console.WriteLine($"the recv state is {recvbyte}  recv time is {end - start}");

                        if (recvbyte < 0)
                        {
                            sock_cli_state.Close();
                            g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
                            if (log != null)
                            {
                                Task.Run(() => log.LogError("receive robot state byte -1")); // 异步日志
                            }
                            throw new IOException("Remote host closed the connection.");
                        }
                        else
                        {
                            /* 判断是否有上一次数据残留 */
                            if (tmp_len > 0)
                            {
                                if ((tmp_len + recvbyte) <= BUFFER_SIZE)
                                {
                                    tmp_recvbuf = tmp_recvbuf.Concat(recvbuf).ToArray();
                                    Array.Clear(recvbuf, 0, recvbuf.Length);
                                    recvbuf = tmp_recvbuf;
                                    recvbyte += tmp_len; // 加上之前遗留的数据
                                    tmp_len = 0;
                                    Array.Clear(tmp_recvbuf, 0, tmp_recvbuf.Length); // 每次拼接后都将之前的 temp 内容删除
                                }
                                else
                                {
                                    /* 清除上一次数据残留 */
                                    tmp_len = 0;
                                    Array.Clear(tmp_recvbuf, 0, tmp_recvbuf.Length);
                                }
                            }
                            else
                            {
                                tmp_len = 0;
                                Array.Clear(tmp_recvbuf, 0, tmp_recvbuf.Length);
                            }

                            for (i = 0; i < recvbyte; i++)
                            {
                                /* 找帧头1 */
                                if (recvbuf[i] == 0x5A && find_head_flag == 0)
                                {
                                    /* 判断帧头+CNT+LEN数据长度能否满足解析要求 */
                                    if ((i + 4) < recvbyte)
                                    {
                                        /* 找帧头2 */
                                        if (recvbuf[i + 1] == 0x5A)
                                        {
                                            find_head_flag = 1;
                                            state_pkg[0] = recvbuf[i];
                                            index++;
                                            len = (UInt16)(len | recvbuf[i + 4]);
                                            len = (UInt16)(len << 8);
                                            len = (UInt16)(len | recvbuf[i + 3]);
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        /* 剩余数据存入临时变量 */
                                        tmp_recvbuf = tmp_recvbuf.Concat(recvbuf.Skip(i).ToArray()).ToArray();
                                        tmp_len = (UInt16)(recvbyte - i);
                                        break;
                                    }
                                }
                                /* 写入数据 */
                                else if (find_head_flag == 1 && index < len + 5)
                                {
                                    state_pkg[index] = recvbuf[i];
                                    index++;
                                }
                                /* 数据校验 */
                                else if (find_head_flag == 1 && index >= len + 5)
                                {
                                    if ((i + 1) < recvbyte)
                                    {
                                        UInt16 checksum = 0;
                                        UInt16 checkdata = 0;

                                        checkdata = (UInt16)(checkdata | recvbuf[i + 1]);
                                        checkdata = (UInt16)(checkdata << 8);
                                        checkdata = (UInt16)(checkdata | recvbuf[i]);

                                        for (int j = 0; j < index; j++)
                                        {
                                            checksum += state_pkg[j];
                                        }

                                        if (checksum == checkdata)
                                        {
                                            int size = Marshal.SizeOf(robot_state_pkg);

                                            if (size > state_pkg.Length)
                                            {
                                                if (log != null)
                                                {
                                                    Task.Run(() => log.LogError("robot state pkg too small")); // 异步日志
                                                }
                                                return;
                                            }

                                            IntPtr structPtr = Marshal.AllocHGlobal(size);
                                            Marshal.Copy(state_pkg, 0, structPtr, size);
                                            robot_state_pkg = (ROBOT_STATE_PKG)Marshal.PtrToStructure(structPtr, typeof(ROBOT_STATE_PKG));

                                            Marshal.FreeHGlobal(structPtr);
                                            Array.Clear(state_pkg, 0, state_pkg.Length);
                                            find_head_flag = 0; // 只有校验通过才将相关标志位复位
                                            index = 0;
                                            len = 0;
                                            i++;
                                        }
                                        else
                                        {
                                            find_head_flag = 0;
                                            index = 0;
                                            len = 0;
                                            i++;
                                        }
                                    }
                                    else
                                    {
                                        /* 剩余数据存入临时变量 */
                                        tmp_recvbuf = tmp_recvbuf.Concat(recvbuf.Skip(i).ToArray()).ToArray();
                                        tmp_len = (UInt16)(recvbyte - i);
                                        break;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        Console.WriteLine("Receive timed out, reconnecting...");
                        if (!sock_cli_state.ReConnect())
                        {
                            Console.WriteLine($"断线重连失败");
                            g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
                            break; // 重连失败则设置错误码并退出
                        }
                        else
                        {
                            g_sock_com_err = (int)RobotError.ERR_SUCCESS;
                            Console.WriteLine("断线重连成功");
                        }
                    }
                    catch (ObjectDisposedException ex)
                    {
                        Console.WriteLine($"Socket was disposed: {ex.Message}");
                        if (!sock_cli_state.ReConnect())
                        {
                            g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
                            break;
                        }
                        else
                        {
                            g_sock_com_err = (int)RobotError.ERR_SUCCESS;
                            Console.WriteLine("断线重连成功");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected error: {ex.Message}");
                        if (!sock_cli_state.ReConnect())
                        {
                            g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
                            break;
                        }
                        else
                        {
                            g_sock_com_err = (int)RobotError.ERR_SUCCESS;
                            Console.WriteLine("断线重连成功");
                        }
                    }

                }
            }
            catch
            {
                if (log != null)
                {
                    log.LogError("get robot state pkg fail");
                }
            }
            if (sock_cli_state.mSocket != null)
            {
                sock_cli_state.Close();
                g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
            }
        }

        //20004端口接收超时时间
        public void SetReceivePortTimeout(int receivetime)
        {
            ReceivePortTimeout = receivetime;
        }

        //重连状态  false:未重连   true:重连
        public bool GetReconnectState()
        {
            return sock_cli_state.GetReconnState();
        }



        /**
        * @brief 即时指令发送处理线程 
        */
        private void RobotInstCmdSendRoutineThread()
        {
            int sendbyte = 0;

            /* 建立通讯 */
            sock_cli_cmd = new TCPClient(robot_ip, ROBOT_CMD_PORT);
            sock_cli_cmd.SetReconnectParam(reconnEnable, reconnTimes, reconnPeriod);//断线重连参数
            sock_cli_cmd.SetLog(log);
            bool brtn = sock_cli_cmd.Connect();
            if (!brtn)
            {
                g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
                if (log != null)
                {
                    log.LogError("send cmd connect fail");
                }
                return;
            }

            try
            {
                while (robot_instcmd_send_exit == 0)
                {
                    try
                    {
                        if (is_sendcmd && g_sendbuf.Length > 0)
                        {
                            byte[] sendCmdBytes = System.Text.Encoding.UTF8.GetBytes(g_sendbuf);
                            Console.WriteLine("now send bytes");
                            sendbyte = sock_cli_cmd.mSocket.Send(sendCmdBytes);
                            Console.WriteLine("sendbyte：" + sendbyte);
                            if (sendbyte < 0)
                            {
                                sock_cli_cmd.Close();
                                g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
                                if (log != null)
                                {
                                    log.LogError("send cmd fail");
                                }
                                return;
                            }
                            is_sendcmd = false;
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                    catch
                    {
                        if (sock_cli_cmd.ReConnect())
                        {
                            g_sock_com_err = (int)RobotError.ERR_SUCCESS;
                            //continue;
                        }
                        else
                        {
                            if (log != null)
                            {
                                log.LogError("SDK Disconnected from robot, try to reconnect robot failed!");
                                log.LogError("send cmd fail");
                            }
                            return;
                        }
                    }

                }
            }
            catch
            {
                g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
                if (log != null)
                {
                    log.LogError("send cmd exception");
                }
                return;
            }


            if (sock_cli_cmd.mSocket != null)
            {
                sock_cli_cmd.Close();
                g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
            }
        }
        private void RobotInstCmdRecvRoutineThread()
        {
            int recvbyte;

            try
            {
                while (robot_instcmd_recv_exit == 0)
                {
                    g_recvbuf = "";
                    byte[] recvBytes = new byte[BUFFER_SIZE];
                    try
                    {
                        //Console.WriteLine($"now recv cmd rtn");
                        recvbyte = sock_cli_cmd.mSocket.Receive(recvBytes);
                        if (recvbyte < 0)
                        {
                            sock_cli_cmd.Close();
                            g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
                            if (log != null)
                            {
                                log.LogError("recv cmd fail");
                            }
                            return;
                        }
                        g_recvbuf = System.Text.Encoding.UTF8.GetString(recvBytes);

                    }
                    catch (Exception)
                    {
                        if (sock_cli_cmd.mSocket != null)
                        {
                            sock_cli_cmd.mSocket.Close();

                        }
                        if (sock_cli_cmd.ReConnect())
                        {
                            continue;
                        }
                        else
                        {
                            if (log != null)
                            {
                                log.LogError("SDK Disconnected from robot, try to reconnect robot failed!");
                                log.LogError("send cmd fail");
                            }
                            return;
                        }
                    }
                }
            }
            catch
            {
                if (log != null)
                {
                    log.LogError("recv cmd exception");
                }
            }
            if (sock_cli_cmd.mSocket != null)
            {
                sock_cli_cmd.Close();
                g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
            }
            return;
        }

        private void RobotTaskRoutineThread()
        {
            byte s_isFirst = 0;
            byte s_check_cnt = 0;
            UInt32 s_last_time = 0;

            while (robot_task_exit == 0)
            {
                if (g_sock_com_err == (int)RobotError.ERR_SUCCESS && sock_cli_state.mSocket != null)//如果当前没出错
                {
                    //打时间戳
                    UInt32 curtime = (UInt32)DateTime.UtcNow.TimeOfDay.TotalMilliseconds;

                    if (s_isFirst == 0)
                    {
                        s_last_frame_cnt = robot_state_pkg.frame_cnt;
                        s_last_time = curtime;
                        s_isFirst = 1;
                    }
                    else
                    {
                        if (((robot_state_pkg.frame_cnt - s_last_frame_cnt) == 0) && ((curtime - s_last_time) < 10 * 30))//两次帧计数相同，正常帧计数是累加的
                        {
                            s_check_cnt++;
                            if (s_check_cnt >= MAX_CHECK_CNT_COM)
                            {
                                if (!reconnEnable)
                                {
                                    g_sock_com_err = (int)RobotError.ERR_SOCKET_COM_FAILED;
                                }

                                //if (log != null)
                                //{
                                //    log.LogError("robot task loss pkg");
                                //}
                                s_check_cnt = 0;
                            }
                        }
                        else
                        {
                            s_check_cnt = 0;
                        }

                        s_last_frame_cnt = robot_state_pkg.frame_cnt;
                        s_last_time = curtime;
                    }
                }

                Thread.Sleep(30);
            }

            s_isFirst = 0;

            return;
        }

        /**
        * @brief  与机器人控制器建立通讯
        * @param  [in] ip  控制器IP地址，出场默认为192.168.58.2
        * @return 错误码
        */
        public int RPC(string ip)
        {
            robot_instcmd_send_exit = 0;
            robot_instcmd_recv_exit = 0;
            robot_realstate_exit = 0;
            robot_task_exit = 0;
            string url = $"http://{ip}:20003/RPC2";
            proxy.Url = url;
            robot_ip = ip;
            robot_ip = ip;
            g_sock_com_err = (int)RobotError.ERR_SUCCESS;

            Thread stateThread = new Thread(RobotStateRoutineThread);
            stateThread.Start();
            Thread cmdsendThread = new Thread(RobotInstCmdSendRoutineThread);
            cmdsendThread.Start();
            Thread.Sleep(2000);
            if (IsSockComError())
            {
                log.LogInfo("RPC Fail.");

                Console.WriteLine("RPC Fail." + g_sock_com_err);
                return g_sock_com_err;
            }
            Console.WriteLine("RPC ");
            Thread cmdrecvThread = new Thread(RobotInstCmdRecvRoutineThread);
            cmdrecvThread.Start();
            Thread taskThread = new Thread(RobotTaskRoutineThread);
            taskThread.Start();
            if (log != null)
            {
                log.LogInfo($"RPC {ip}");
            }
            return g_sock_com_err;
        }

        /**
         * @brief  与机器人控制器关闭通讯
         * @return 错误码
         */
        public int CloseRPC()
        {
            robot_instcmd_send_exit = 1;
            robot_instcmd_recv_exit = 1;
            robot_realstate_exit = 1;
            robot_task_exit = 1;

            Thread.Sleep(100);
            if (sock_cli_cmd.mSocket != null)
            {
                sock_cli_cmd.mSocket.Close();

            }

            if (sock_cli_state.mSocket != null)
            {
                sock_cli_state.mSocket.Close();
            }

            g_sock_com_err = (int)RobotError.ERR_SUCCESS;

            if (log != null)
            {
                log.LogInfo("Close RPC");
            }

            if (log != null)
            {
                log.LogClose();
            }

            return 0;
        }


        /**
         * @brief  查询SDK版本号
         * @param  [out] version   SDK版本号
         * @return  错误码
         */
        public int GetSDKVersion(ref string version)
        {
            version = SDK_VERSION;
            if (log != null)
            {
                log.LogInfo($"GetSDKVersion(ref {version})");
            }
            return 0;
        }

        /**
         * @brief  获取控制器IP
         * @param  [out] ip  控制器IP
         * @return  错误码
         */
        public int GetControllerIP(ref string ip)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                string ipAddress = "";
                object[] result = proxy.GetControllerIP(ipAddress);
                int rtn = (int)result[0];
                ip = (string)result[1];

                if (log != null)
                {
                    log.LogInfo($"GetSDKVersion(ref {ip}) : {rtn}");
                }

                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        public int GetSafetyCode()
        {
            if (robot_state_pkg.safety_stop0_state == 1 || robot_state_pkg.safety_stop1_state == 1)
            {
                return 99;
            }

            return 0;
        }
        /**
         * @brief 控制机器人手自动模式切换
         * @param [in] mode 0-自动模式，1-手动模式
         * @return 错误码
         */
        public int Mode(int mode)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.Mode(mode);
                if (log != null)
                {
                    log.LogInfo($"Mode({mode}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  控制机器人进入或退出拖动示教模式
         * @param  [in] state 0-退出拖动示教模式，1-进入拖动示教模式
         * @return  错误码
         */
        public int DragTeachSwitch(byte state)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.DragTeachSwitch(state);
                if (log != null)
                {
                    log.LogInfo($"DragTeachSwitch({state}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  查询机器人是否处于拖动示教模式
         * @param  [out] state 0-非拖动示教模式，1-拖动示教模式
         * @return  错误码
         */
        public int IsInDragTeach(ref byte state)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                if (robot_state_pkg.robot_state == 4)
                {
                    state = 1;
                }
                else
                {
                    state = 0;
                }
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"IsInDragTeach(ref {state}) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief  控制机器人上使能或下使能，机器人上电后默认自动上使能
         * @param  [in] state  0-下使能，1-上使能
         * @return  错误码
         */
        public int RobotEnable(byte state)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.RobotEnable(state);
                if (log != null)
                {
                    log.LogInfo($"RobotEnable(ref {state}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (log != null)
                {
                    log.LogError($"RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
        * @brief 开始奇异位姿保护
        * @param [in] protectMode 奇异保护模式，0：关节模式；1-笛卡尔模式
        * @param [in] minShoulderPos 肩奇异调整范围(mm), 默认100
        * @param [in] minElbowPos 肘奇异调整范围(mm), 默认50
        * @param [in] minWristPos 腕奇异调整范围(°), 默认10
        * @return 错误码
        */
        public int SingularAvoidStart(int protectMode, double minShoulderPos, double minElbowPos, double minWristPos)
        {

            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SingularAvoidStart(protectMode, minShoulderPos, minElbowPos, minWristPos);

                if (log != null)
                {
                    log.LogInfo($"SingularAvoidStart({protectMode}, {minShoulderPos}, {minElbowPos}, {minWristPos}) : {rtn}");
                }

                return rtn;
            }
            catch
            {
                if (log != null)
                {
                    log.LogError($"RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
        * @brief 停止奇异位姿保护
        * @return 错误码
*/
        public int SingularAvoidEnd()
        {

            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SingularAvoidEnd();

                if (log != null)
                {
                    log.LogInfo($"SingularAvoidEnd  fail : {rtn}");
                }

                return rtn;
            }
            catch
            {
                if (log != null)
                {
                    log.LogError($"RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
         * @brief  jog点动
         * @param  [in]  refType 0-关节点动，2-基坐标系下点动，4-工具坐标系下点动，8-工件坐标系下点动
         * @param  [in]  nb 1-关节1(或x轴)，2-关节2(或y轴)，3-关节3(或z轴)，4-关节4(或绕x轴旋转)，5-关节5(或绕y轴旋转)，6-关节6(或绕z轴旋转)
         * @param  [in]  dir 0-负方向，1-正方向
         * @param  [in]  vel 速度百分比，[0~100]
         * @param  [in]  acc 加速度百分比， [0~100]
         * @param  [in]  max_dis 单次点动最大角度，单位[°]或距离，单位[mm]
         * @return  错误码
         */
        public int StartJOG(int refType, int nb, int dir, float vel, float acc, float max_dis)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.StartJOG(refType, nb, dir, vel, acc, max_dis);
                if (log != null)
                {
                    log.LogInfo($"StartJOG({refType}, {nb}, {dir}, {vel}, {acc}, {max_dis}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  jog点动减速停止
         * @param  [in]  stopType  1-关节点动停止，3-基坐标系下点动停止，5-工具坐标系下点动停止，9-工件坐标系下点动停止
         * @return  错误码
         */
        public int StopJOG(byte stopType)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.StopJOG(stopType);
                if (log != null)
                {
                    log.LogInfo($"StopJOG({stopType}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief jog点动立即停止
         * @return  错误码
         */
        public int ImmStopJOG()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.ImmStopJOG();
                if (log != null)
                {
                    log.LogInfo($"ImmStopJOG() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  关节空间运动
         * @param  [in] jopublic int_pos  目标关节位置,单位deg
         * @param  [in] desc_pos   目标笛卡尔位姿
         * @param  [in] tool  工具坐标号，范围[0~14]
         * @param  [in] user  工件坐标号，范围[0~14]
         * @param  [in] vel  速度百分比，范围[0~100]
         * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] ovl  速度缩放因子，范围[0~100]
         * @param  [in] epos  扩展轴位置，单位mm
         * @param  [in] blendT [-1.0]-运动到位(阻塞)，[0~500.0]-平滑时间(非阻塞)，单位ms
         * @param  [in] offset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
         * @param  [in] offset_pos  位姿偏移量
         * @return  错误码
         */
        public int MoveJ(JointPos joint_pos, DescPose desc_pos, int tool, int user, float vel, float acc, float ovl, ExaxisPos epos, float blendT, byte offset_flag, DescPose offset_pos)
        {
            int rtn;
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                double[] joint = joint_pos.jPos;
                double[] desc = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                double[] exteraxis = epos.ePos;
                double[] offect = new double[6] { offset_pos.tran.x, offset_pos.tran.y, offset_pos.tran.z, offset_pos.rpy.rx, offset_pos.rpy.ry, offset_pos.rpy.rz };
                rtn = proxy.MoveJ(joint, desc, tool, user, vel, acc, ovl, exteraxis, blendT, offset_flag, offect);
                if (log != null)
                {
                    log.LogInfo($"MoveJ({joint[0]},{joint[1]},{joint[2]},{joint[3]},{joint[4]},{joint[5]},{desc[0]},{desc[1]},{desc[2]},{desc[3]},{desc[4]},{desc[5]},{tool},{user},{vel},{acc},{ovl}," +
                        $"{epos.ePos[0]},{epos.ePos[1]},{epos.ePos[2]},{epos.ePos[3]},{blendT},{offset_flag},{offect[0]},{offect[1]},{offect[2]},{offect[3]},{offect[4]},{offect[5]}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }

            }
        }
        /**
       * @brief  笛卡尔空间直线运动
       * @param  [in] joint_pos  目标关节位置,单位deg
       * @param  [in] desc_pos   目标笛卡尔位姿
       * @param  [in] tool  工具坐标号，范围[0~14]
       * @param  [in] user  工件坐标号，范围[0~14]
       * @param  [in] vel  速度百分比，范围[0~100]
       * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
       * @param  [in] ovl  速度缩放因子，范围[0~100]
       * @param  [in] blendR [-1.0]-运动到位(阻塞)，[0~1000.0]-平滑半径(非阻塞)，单位mm
       * @param  [in] epos  扩展轴位置，单位mm
       * @param  [in] search  0-不焊丝寻位，1-焊丝寻位
       * @param  [in] offset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
       * @param  [in] offset_pos  位姿偏移量
       * @param  [in] overSpeedStrategy  超速处理策略，1-标准；2-超速时报错停止；3-自适应降速，默认为0
       * @param  [in] speedPercent  允许降速阈值百分比[0-100]，默认10%
       * @return  错误码
       */
        public int MoveL(JointPos joint_pos, DescPose desc_pos, int tool, int user, float vel, float acc, float ovl, float blendR, ExaxisPos epos, byte search, byte offset_flag, DescPose offset_pos, int overSpeedStrategy = 0, int speedPercent = 10)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = -1;
                if (overSpeedStrategy > 1)
                {
                    rtn = proxy.JointOverSpeedProtectStart(overSpeedStrategy, speedPercent);
                    if (log != null)
                    {
                        log.LogInfo($"JointOverSpeedProtectStart({overSpeedStrategy},{speedPercent}) : {rtn}");
                    }
                    if (rtn != 0)
                    {
                        return rtn;
                    }
                }

                double[] joint = joint_pos.jPos;
                double[] desc = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                double[] exteraxis = epos.ePos;
                double[] offect = new double[6] { offset_pos.tran.x, offset_pos.tran.y, offset_pos.tran.z, offset_pos.rpy.rx, offset_pos.rpy.ry, offset_pos.rpy.rz };
                rtn = proxy.MoveL(joint, desc, tool, user, vel, acc, ovl, blendR, 0,exteraxis, search, offset_flag, offect);
                if (log != null)
                {
                    log.LogInfo($"MoveL({joint[0]},{joint[1]},{joint[2]},{joint[3]},{joint[4]},{joint[5]},{desc[0]},{desc[1]},{desc[2]},{desc[3]},{desc[4]},{desc[5]},{tool},{user},{vel},{acc},{ovl},{blendR}" +
                        $"{epos.ePos[0]},{epos.ePos[1]},{epos.ePos[2]},{epos.ePos[3]},{search},{offset_flag},{offect[0]},{offect[1]},{offect[2]},{offect[3]},{offect[4]},{offect[5]}) : {rtn}");
                }

                if (overSpeedStrategy > 1)
                {
                    rtn = proxy.JointOverSpeedProtectEnd();
                    if (log != null)
                    {
                        log.LogInfo($"JointOverSpeedProtectEnd() : {rtn}");
                    }
                    if (rtn != 0)
                    {
                        return rtn;
                    }
                }
                if ((robot_state_pkg.main_code != 0 || robot_state_pkg.sub_code != 0) && rtn == 0)
                {
                    rtn = 14;
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
         * @brief  笛卡尔空间直线运动
         * @param  [in] joint_pos  目标关节位置,单位deg
         * @param  [in] desc_pos   目标笛卡尔位姿
         * @param  [in] tool  工具坐标号，范围[0~14]
         * @param  [in] user  工件坐标号，范围[0~14]
         * @param  [in] vel  速度百分比，范围[0~100]
         * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] ovl  速度缩放因子，范围[0~100]
         * @param  [in] blendR [-1.0]-运动到位(阻塞)，[0~1000.0]-平滑半径(非阻塞)，单位mm
         * @param  [in] blendMode 过渡方式；0-内切过渡；1-角点过渡
         * @param  [in] epos  扩展轴位置，单位mm
         * @param  [in] search  0-不焊丝寻位，1-焊丝寻位
         * @param  [in] offset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
         * @param  [in] offset_pos  位姿偏移量
         * @param  [in] overSpeedStrategy  超速处理策略，1-标准；2-超速时报错停止；3-自适应降速，默认为0
         * @param  [in] speedPercent  允许降速阈值百分比[0-100]，默认10%
         * @return  错误码
         */
        public int MoveL(JointPos joint_pos, DescPose desc_pos, int tool, int user, float vel, float acc, float ovl, float blendR, int blendMode, ExaxisPos epos, byte search, byte offset_flag, DescPose offset_pos, int overSpeedStrategy = 0, int speedPercent = 10)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = -1;
                if (overSpeedStrategy > 1)
                {
                    rtn = proxy.JointOverSpeedProtectStart(overSpeedStrategy, speedPercent);
                    if (log != null)
                    {
                        log.LogInfo($"JointOverSpeedProtectStart({overSpeedStrategy},{speedPercent}) : {rtn}");
                    }
                    if (rtn != 0)
                    {
                        return rtn;
                    }
                }

                double[] joint = joint_pos.jPos;
                double[] desc = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                double[] exteraxis = epos.ePos;
                double[] offect = new double[6] { offset_pos.tran.x, offset_pos.tran.y, offset_pos.tran.z, offset_pos.rpy.rx, offset_pos.rpy.ry, offset_pos.rpy.rz };
                rtn = proxy.MoveL(joint, desc, tool, user, vel, acc, ovl, blendR, blendMode, exteraxis, search, offset_flag, offect);
                if (log != null)
                {
                    log.LogInfo($"MoveL({joint[0]},{joint[1]},{joint[2]},{joint[3]},{joint[4]},{joint[5]},{desc[0]},{desc[1]},{desc[2]},{desc[3]},{desc[4]},{desc[5]},{tool},{user},{vel},{acc},{ovl},{blendR}" +
                        $"{epos.ePos[0]},{epos.ePos[1]},{epos.ePos[2]},{epos.ePos[3]},{search},{offset_flag},{offect[0]},{offect[1]},{offect[2]},{offect[3]},{offect[4]},{offect[5]}) : {rtn}");
                }

                if (overSpeedStrategy > 1)
                {
                    rtn = proxy.JointOverSpeedProtectEnd();
                    if (log != null)
                    {
                        log.LogInfo($"JointOverSpeedProtectEnd() : {rtn}");
                    }
                    if (rtn != 0)
                    {
                        return rtn;
                    }
                }
                if ((robot_state_pkg.main_code != 0 || robot_state_pkg.sub_code != 0) && rtn == 0)
                {
                    rtn = 14;
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  笛卡尔空间圆弧运动
         * @param  [in] joint_pos_p  路径点关节位置,单位deg
         * @param  [in] desc_pos_p   路径点笛卡尔位姿
         * @param  [in] ptool  工具坐标号，范围[0~14]
         * @param  [in] puser  工件坐标号，范围[0~14]
         * @param  [in] pvel  速度百分比，范围[0~100]
         * @param  [in] pacc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] epos_p  扩展轴位置，单位mm
         * @param  [in] poffset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
         * @param  [in] offset_pos_p  位姿偏移量
         * @param  [in] joint_pos_t  目标点关节位置,单位deg
         * @param  [in] desc_pos_t   目标点笛卡尔位姿
         * @param  [in] ttool  工具坐标号，范围[0~14]
         * @param  [in] tuser  工件坐标号，范围[0~14]
         * @param  [in] tvel  速度百分比，范围[0~100]
         * @param  [in] tacc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] epos_t  扩展轴位置，单位mm
         * @param  [in] toffset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
         * @param  [in] offset_pos_t  位姿偏移量	 
         * @param  [in] ovl  速度缩放因子，范围[0~100]	 
         * @param  [in] blendR [-1.0]-运动到位(阻塞)，[0~1000.0]-平滑半径(非阻塞)，单位mm	 
         * @return  错误码
         */
        public int MoveC(JointPos joint_pos_p, DescPose desc_pos_p, int ptool, int puser, float pvel, float pacc, ExaxisPos epos_p, byte poffset_flag, DescPose offset_pos_p, JointPos joint_pos_t, DescPose desc_pos_t, int ttool, int tuser, float tvel, float tacc, ExaxisPos epos_t, byte toffset_flag, DescPose offset_pos_t, float ovl, float blendR)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                double[] jointP = joint_pos_p.jPos;
                double[] descP = new double[6] { desc_pos_p.tran.x, desc_pos_p.tran.y, desc_pos_p.tran.z, desc_pos_p.rpy.rx, desc_pos_p.rpy.ry, desc_pos_p.rpy.rz };
                double[] exteraxisP = epos_p.ePos;
                double[] offectP = new double[6] { offset_pos_p.tran.x, offset_pos_p.tran.y, offset_pos_p.tran.z, offset_pos_p.rpy.rx, offset_pos_p.rpy.ry, offset_pos_p.rpy.rz };
                double[] controlP = new double[4] { ptool, puser, pvel, pacc };

                double[] jointT = joint_pos_t.jPos;
                double[] descT = new double[6] { desc_pos_t.tran.x, desc_pos_t.tran.y, desc_pos_t.tran.z, desc_pos_t.rpy.rx, desc_pos_t.rpy.ry, desc_pos_t.rpy.rz };
                double[] exteraxisT = epos_t.ePos;
                double[] offectT = new double[6] { offset_pos_t.tran.x, offset_pos_t.tran.y, offset_pos_t.tran.z, offset_pos_t.rpy.rx, offset_pos_t.rpy.ry, offset_pos_t.rpy.rz };
                double[] controlT = new double[4] { ttool, tuser, tvel, tacc };
                int rtn = proxy.MoveC(jointP, descP, controlP, exteraxisP, poffset_flag, offectP, jointT, descT, controlT, exteraxisT, toffset_flag, offectT, ovl, blendR);
                if (log != null)
                {
                    log.LogInfo($"MoveC({jointP[0]},{jointP[1]},{jointP[2]},{jointP[3]},{jointP[4]},{jointP[5]},{descP[0]},{descP[1]},{descP[2]},{descP[3]},{descP[4]},{descP[5]},{ptool},{puser},{pvel},{pacc}," +
                        $"{epos_p.ePos[0]},{epos_p.ePos[1]},{epos_p.ePos[2]},{epos_p.ePos[3]},{poffset_flag},{offectP[0]},{offectP[1]},{offectP[2]},{offectP[3]},{offectP[4]},{offectP[5]},) " +
                        $"{jointT[0]},{jointT[1]},{jointT[2]},{jointT[3]},{jointT[4]},{jointT[5]},{descT[0]},{descT[1]},{descT[2]},{descT[3]},{descT[4]},{descT[5]},{ttool},{tuser},{tvel},{tacc}," +
                        $"{epos_t.ePos[0]},{epos_t.ePos[1]},{epos_t.ePos[2]},{epos_t.ePos[3]},{toffset_flag},{offectT[0]},{offectT[1]},{offectT[2]},{offectT[3]},{offectT[4]},{offectT[5]},{ovl},{blendR} : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  笛卡尔空间整圆运动
         * @param  [in] joint_pos_p  路径点1关节位置,单位deg
         * @param  [in] desc_pos_p   路径点1笛卡尔位姿
         * @param  [in] ptool  工具坐标号，范围[0~14]
         * @param  [in] puser  工件坐标号，范围[0~14]
         * @param  [in] pvel  速度百分比，范围[0~100]
         * @param  [in] pacc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] epos_p  扩展轴位置，单位mm
         * @param  [in] joint_pos_t  路径点2关节位置,单位deg
         * @param  [in] desc_pos_t   路径点2笛卡尔位姿
         * @param  [in] ttool  工具坐标号，范围[0~14]
         * @param  [in] tuser  工件坐标号，范围[0~14]
         * @param  [in] tvel  速度百分比，范围[0~100]
         * @param  [in] tacc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] epos_t  扩展轴位置，单位mm
         * @param  [in] ovl  速度缩放因子，范围[0~100]	
         * @param  [in] offset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
         * @param  [in] offset_pos  位姿偏移量	 	 
         * @return  错误码
         */
        public int Circle(JointPos joint_pos_p, DescPose desc_pos_p, int ptool, int puser, float pvel, float pacc, ExaxisPos epos_p, JointPos joint_pos_t, DescPose desc_pos_t, int ttool, int tuser, float tvel, float tacc, ExaxisPos epos_t, float ovl, byte offset_flag, DescPose offset_pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                double[] jointP = joint_pos_p.jPos;
                double[] descP = new double[6] { desc_pos_p.tran.x, desc_pos_p.tran.y, desc_pos_p.tran.z, desc_pos_p.rpy.rx, desc_pos_p.rpy.ry, desc_pos_p.rpy.rz };
                double[] exteraxisP = epos_p.ePos;
                double[] offect = new double[6] { offset_pos.tran.x, offset_pos.tran.y, offset_pos.tran.z, offset_pos.rpy.rx, offset_pos.rpy.ry, offset_pos.rpy.rz };
                double[] controlP = new double[4] { ptool, puser, pvel, pacc };
                double[] jointT = joint_pos_t.jPos;
                double[] descT = new double[6] { desc_pos_t.tran.x, desc_pos_t.tran.y, desc_pos_t.tran.z, desc_pos_t.rpy.rx, desc_pos_t.rpy.ry, desc_pos_t.rpy.rz };
                double[] exteraxisT = epos_t.ePos;
                double[] controlT = new double[4] { ttool, tuser, tvel, tacc };
                int rtn = proxy.Circle(jointP, descP, controlP, exteraxisP, jointT, descT, controlT, exteraxisT, ovl, offset_flag, offect);
                if (log != null)
                {
                    log.LogInfo($"Circle({jointP[0]},{jointP[1]},{jointP[2]},{jointP[3]},{jointP[4]},{jointP[5]},{descP[0]},{descP[1]},{descP[2]},{descP[3]},{descP[4]},{descP[5]},{ptool},{puser},{pvel},{pacc}," +
                        $"{epos_p.ePos[0]},{epos_p.ePos[1]},{epos_p.ePos[2]},{epos_p.ePos[3]},) " +
                        $"{jointT[0]},{jointT[1]},{jointT[2]},{jointT[3]},{jointT[4]},{jointT[5]},{descT[0]},{descT[1]},{descT[2]},{descT[3]},{descT[4]},{descT[5]},{ttool},{tuser},{tvel},{tacc}," +
                        $"{epos_t.ePos[0]},{epos_t.ePos[1]},{epos_t.ePos[2]},{epos_t.ePos[3]},{ovl},{offset_flag},{offect[0]},{offect[1]},{offect[2]},{offect[3]},{offect[4]},{offect[5]} : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  笛卡尔空间螺旋线运动
         * @param  [in] joint_pos  目标关节位置,单位deg
         * @param  [in] desc_pos   目标笛卡尔位姿
         * @param  [in] tool  工具坐标号，范围[0~14]
         * @param  [in] user  工件坐标号，范围[0~14]
         * @param  [in] vel  速度百分比，范围[0~100]
         * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] epos  扩展轴位置，单位mm
         * @param  [in] ovl  速度缩放因子，范围[0~100]	 
         * @param  [in] offset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
         * @param  [in] offset_pos  位姿偏移量
         * @param  [in] spiral_param  螺旋参数
         * @return  错误码
         */
        public int NewSpiral(JointPos joint_pos, DescPose desc_pos, int tool, int user, float vel, float acc, ExaxisPos epos, float ovl, byte offset_flag, DescPose offset_pos, SpiralParam spiral_param)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                double[] jointPos = joint_pos.jPos;
                double[] descPos = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                double[] exteraxisPos = epos.ePos;
                double[] offectPos = new double[6] { offset_pos.tran.x, offset_pos.tran.y, offset_pos.tran.z, offset_pos.rpy.rx, offset_pos.rpy.ry, offset_pos.rpy.rz };
                double[] spiralParam = new double[6] { spiral_param.circle_num, spiral_param.circle_angle, spiral_param.rad_init, spiral_param.rad_add, spiral_param.rotaxis_add, spiral_param.rot_direction };

                int rtn = proxy.NewSpiral(jointPos, descPos, tool, user, vel, acc, exteraxisPos, ovl, offset_flag, offectPos, spiralParam);
                if (log != null)
                {
                    log.LogInfo($"NewSpiral({jointPos[0]},{jointPos[1]},{jointPos[2]},{jointPos[3]},{jointPos[4]},{jointPos[5]},{descPos[0]},{descPos[1]},{descPos[2]},{descPos[3]},{descPos[4]},{descPos[5]},{tool},{user},{vel},{acc}," +
                        $"{epos.ePos[0]},{epos.ePos[1]},{epos.ePos[2]},{epos.ePos[3]},{ovl},{offset_flag},) " +
                        $"{offectPos[0]},{offectPos[1]},{offectPos[2]},{offectPos[3]},{offectPos[4]},{offectPos[5]},{spiralParam[0]},{spiralParam[1]},{spiralParam[2]},{spiralParam[3]},{spiralParam[4]},{spiralParam[5]} : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /**
         * @brief 伺服运动开始，配合ServoJ、ServoCart指令使用
         * @return  错误码
         */
        public int ServoMoveStart()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.ServoMoveStart();
                if (log != null)
                {
                    log.LogInfo($"ServoMoveStart({rtn}");
                }
                return rtn;
            }
            catch
            {

                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 伺服运动结束，配合ServoJ、ServoCart指令使用
         * @return  错误码
         */
        public int ServoMoveEnd()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.ServoMoveEnd();
                if (log != null)
                {
                    log.LogInfo($"ServoMoveEnd({rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  关节空间伺服模式运动
         * @param  [in] joint_pos  目标关节位置,单位deg
         * @param  [in] axisPos  外部轴位置,单位mm
         * @param  [in] acc  加速度百分比，范围[0~100],暂不开放，默认为0
         * @param  [in] vel  速度百分比，范围[0~100]，暂不开放，默认为0
         * @param  [in] cmdT  指令下发周期，单位s，建议范围[0.001~0.0016]
         * @param  [in] filterT 滤波时间，单位s，暂不开放，默认为0
         * @param  [in] gain  目标位置的比例放大器，暂不开放，默认为0
         * @return  错误码
         */
        public int ServoJ(JointPos joint_pos, ExaxisPos axisPos, float acc, float vel, float cmdT, float filterT, float gain)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                double[] jointPos = joint_pos.jPos;
                double[] axis = axisPos.ePos;
                int rtn = proxy.ServoJ(jointPos, axis, acc, vel, cmdT, filterT, gain);
                if (log != null)
                {
                    log.LogInfo($"ServoJ({jointPos[0]},{jointPos[1]},{jointPos[2]},{jointPos[3]},{jointPos[4]},{jointPos[5]},  {axis[0]},{axis[1]},{axis[2]},{axis[3]},   {acc},{vel},{cmdT},{filterT},{gain}): {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  笛卡尔空间伺服模式运动
         * @param  [in]  mode  0-绝对运动(基坐标系)，1-增量运动(基坐标系)，2-增量运动(工具坐标系)
         * @param  [in]  desc_pos  目标笛卡尔位姿或位姿增量
         * @param  [in]  pos_gain  位姿增量比例系数，仅在增量运动下生效，范围[0~1]
         * @param  [in] acc  加速度百分比，范围[0~100],暂不开放，默认为0
         * @param  [in] vel  速度百分比，范围[0~100]，暂不开放，默认为0
         * @param  [in] cmdT  指令下发周期，单位s，建议范围[0.001~0.0016]
         * @param  [in] filterT 滤波时间，单位s，暂不开放，默认为0
         * @param  [in] gain  目标位置的比例放大器，暂不开放，默认为0
         * @return  错误码
         */
        public int ServoCart(int mode, DescPose desc_pose, double[] pos_gain, float acc, float vel, float cmdT, float filterT, float gain)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                double[] descPos = new double[6] { desc_pose.tran.x, desc_pose.tran.y, desc_pose.tran.z, desc_pose.rpy.rx, desc_pose.rpy.ry, desc_pose.rpy.rz };
                int rtn = proxy.ServoCart(mode, descPos, pos_gain, acc, vel, cmdT, filterT, gain);
                if (log != null)
                {
                    log.LogInfo($"ServoCart({mode},{descPos[0]},{descPos[1]},{descPos[2]},{descPos[3]},{descPos[4]},{descPos[5]},{pos_gain[0]},{pos_gain[1]},{pos_gain[2]},{pos_gain[3]},{pos_gain[4]},{pos_gain[5]},{acc},{vel},{cmdT},{filterT},{gain} : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  笛卡尔空间点到点运动
         * @param  [in]  desc_pos  目标笛卡尔位姿或位姿增量
         * @param  [in] tool  工具坐标号，范围[0~14]
         * @param  [in] user  工件坐标号，范围[0~14]
         * @param  [in] vel  速度百分比，范围[0~100]
         * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] ovl  速度缩放因子，范围[0~100]
         * @param  [in] blendT [-1.0]-运动到位(阻塞)，[0~500.0]-平滑时间(非阻塞)，单位ms	
         * @param  [in] config  关节空间配置，[-1]-参考当前关节位置解算，[0~7]-参考特定关节空间配置解算，默认为-1	 
         * @return  错误码
         */
        public int MoveCart(DescPose desc_pos, int tool, int user, float vel, float acc, float ovl, float blendT, int config)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                double[] descPos = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                int rtn = proxy.MoveCart(descPos, tool, user, vel, acc, ovl, blendT, config);
                if (log != null)
                {
                    log.LogInfo($"MoveCart({descPos[0]},{descPos[1]},{descPos[2]},{descPos[3]},{descPos[4]},{descPos[5]},{tool},{user},{vel},{acc},{ovl},{blendT},{config} : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  样条运动开始
         * @return  错误码
         */
        public int SplineStart()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.SplineStart();
                if (log != null)
                {
                    log.LogInfo($"SplineStart({rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  关节空间样条运动
         * @param  [in] joint_pos  目标关节位置,单位deg
         * @param  [in] desc_pos   目标笛卡尔位姿
         * @param  [in] tool  工具坐标号，范围[0~14]
         * @param  [in] user  工件坐标号，范围[0~14]
         * @param  [in] vel  速度百分比，范围[0~100]
         * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] ovl  速度缩放因子，范围[0~100]	
         * @return  错误码
         */
        public int SplinePTP(JointPos joint_pos, DescPose desc_pos, int tool, int user, float vel, float acc, float ovl)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                double[] jointPos = joint_pos.jPos;
                double[] descPos = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                int rtn = proxy.SplinePTP(jointPos, descPos, tool, user, vel, acc, ovl);
                if (log != null)
                {
                    log.LogInfo($"SplinePTP({jointPos[0]},{jointPos[1]},{jointPos[2]},{jointPos[3]},{jointPos[4]},{jointPos[5]},{descPos[0]},{descPos[1]},{descPos[2]},{descPos[3]},{descPos[4]},{descPos[5]},{tool},{user},{vel},{acc},{ovl} : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  样条运动结束
         * @return  错误码
         */
        public int SplineEnd()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SplineEnd();
                if (log != null)
                {
                    log.LogInfo($"SplineEnd() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 新样条运动开始
         * @param  [in] type   0-圆弧过渡，1-给定点位为路径点
         * @param  [in] averageTime  全局平均衔接时间(ms)(10 ~  )，默认2000
         * @return  错误码
         */
        public int NewSplineStart(int type, int averageTime = 2000)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.NewSplineStart(type, averageTime);
                if (log != null)
                {
                    log.LogInfo($"NewSplineStart() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 新样条指令点
         * @param  [in] joint_pos  目标关节位置,单位deg
         * @param  [in] desc_pos   目标笛卡尔位姿
         * @param  [in] tool  工具坐标号，范围[0~14]
         * @param  [in] user  工件坐标号，范围[0~14]
         * @param  [in] vel  速度百分比，范围[0~100]
         * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] ovl  速度缩放因子，范围[0~100]
         * @param  [in] blendR [-1.0]-运动到位(阻塞)，[0~1000.0]-平滑半径(非阻塞)，单位mm
         * @param  [in] lastFlag 是否为最后一个点，0-否，1-是
         * @return  错误码
         */
        public int NewSplinePoint(JointPos joint_pos, DescPose desc_pos, int tool, int user, float vel, float acc, float ovl, float blendR, int lastFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                double[] jointPos = joint_pos.jPos;
                double[] descPos = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                int rtn = proxy.NewSplinePoint(jointPos, descPos, tool, user, vel, acc, ovl, blendR, lastFlag);
                if (log != null)
                {
                    log.LogInfo($"NewSplinePoint({jointPos[0]},{jointPos[1]},{jointPos[2]},{jointPos[3]},{jointPos[4]},{jointPos[5]},{descPos[0]},{descPos[1]},{descPos[2]},{descPos[3]},{descPos[4]},{descPos[5]}," +
                        $"{tool},{user},{vel},{acc},{ovl},{blendR},{lastFlag} : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 新样条运动结束
         * @return  错误码
         */
        public int NewSplineEnd()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.NewSplineEnd();
                if (log != null)
                {
                    log.LogInfo($"NewSplineEnd() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 终止运动
         * @return  错误码
         */
        public int StopMotion()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.StopMotion();
                if (log != null)
                {
                    log.LogInfo($"StopMotion() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 暂停运动
         * @return  错误码
         */
        public int PauseMotion()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            while (is_sendcmd == true) //说明当前正在处理上一条指令
            {
                Thread.Sleep(10);
            }

            g_sendbuf = $"/f/bIII{PauseMotionCnt}III103III5IIIPAUSEIII/b/f";
            PauseMotionCnt++;
            is_sendcmd = true;
            if (log != null)
            {
                log.LogInfo($"PauseMotion() : {g_sock_com_err}");
            }
            return 0;
        }

        /**
         * @brief 恢复运动
         * @return  错误码
         */
        public int ResumeMotion()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            while (is_sendcmd == true) //说明当前正在处理上一条指令
            {
                Thread.Sleep(10);
            }

            g_sendbuf = $"/f/bIII{ResumeMotionCnt}III104III6IIIRESUMEIII/b/f";
            ResumeMotionCnt++;
            is_sendcmd = true;
            if (log != null)
            {
                log.LogInfo($"ResumeMotion() : {g_sock_com_err}");
            }
            return 0;
        }

        /**
         * @brief  点位整体偏移开始
         * @param  [in]  flag  0-基坐标系下/工件坐标系下偏移，2-工具坐标系下偏移
         * @param  [in] offset_pos  位姿偏移量
         * @return  错误码
         */
        public int PointsOffsetEnable(int flag, DescPose offset_pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                double[] offectPos = new double[6] { offset_pos.tran.x, offset_pos.tran.y, offset_pos.tran.z, offset_pos.rpy.rx, offset_pos.rpy.ry, offset_pos.rpy.rz };
                int rtn = proxy.PointsOffsetEnable(flag, offectPos);
                if (log != null)
                {
                    log.LogInfo($"PointsOffsetEnable({flag},{offectPos[0]},{offectPos[1]},{offectPos[2]},{offectPos[3]},{offectPos[4]},{offectPos[5]} : {rtn}");
                }
                return rtn;
            }
            catch
            {

                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  点位整体偏移结束
         * @return  错误码
         */
        public int PointsOffsetDisable()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.PointsOffsetDisable();
                if (log != null)
                {
                    log.LogInfo($"PointsOffsetDisable() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置控制箱数字量输出
         * @param  [in] id  io编号，范围[0~15]
         * @param  [in] status 0-关，1-开
         * @param  [in] smooth 0-不平滑， 1-平滑
         * @param  [in] block  0-阻塞，1-非阻塞
         * @return  错误码
         */
        public int SetDO(int id, byte status, byte smooth, byte block)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetDO(id, status, smooth, block);
                if (log != null)
                {
                    log.LogInfo($"SetDO({id},{status},{smooth},{block}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置工具数字量输出
         * @param  [in] id  io编号，范围[0~1]
         * @param  [in] status 0-关，1-开
         * @param  [in] smooth 0-不平滑， 1-平滑
         * @param  [in] block  0-阻塞，1-非阻塞
         * @return  错误码
         */
        public int SetToolDO(int id, byte status, byte smooth, byte block)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetToolDO(id, status, smooth, block);
                if (log != null)
                {
                    log.LogInfo($"SetToolDO({id},{status},{smooth},{block}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置控制箱模拟量输出
         * @param  [in] id  io编号，范围[0~1]
         * @param  [in] value 电流或电压值百分比，范围[0~100]对应电流值[0~20mA]或电压[0~10V]
         * @param  [in] block  0-阻塞，1-非阻塞
         * @return  错误码
         */
        public int SetAO(int id, float value, byte block)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetAO(id, value * 40.95, block);
                if (log != null)
                {
                    log.LogInfo($"SetAO({id},{value},{block}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置工具模拟量输出
         * @param  [in] id  io编号，范围[0]
         * @param  [in] value 电流或电压值百分比，范围[0~100]对应电流值[0~20mA]或电压[0~10V]
         * @param  [in] block  0-阻塞，1-非阻塞
         * @return  错误码
         */
        public int SetToolAO(int id, float value, byte block)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetToolAO(id, value * 40.95, block);
                if (log != null)
                {
                    log.LogInfo($"SetToolAO({id},{value},{block}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取控制箱数字量输入
         * @param  [in] id  io编号，范围[0~15]
         * @param  [in] block  0-阻塞，1-非阻塞
         * @param  [out] level  0-低电平，1-高电平
         * @return  错误码
         */
        public int GetDI(int id, byte block, ref byte level)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            int errcode = 0;
            try
            {
                if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
                {
                    if (id >= 0 && id < 8)
                    {
                        level = (byte)((robot_state_pkg.cl_dgt_input_l & (0x01 << id)) >> id);
                    }
                    else if (id >= 8 && id < 16)
                    {
                        id -= 8;
                        level = (byte)((robot_state_pkg.cl_dgt_input_h & (0x01 << id)) >> id);
                    }
                    else
                    {
                        level = 0;
                        errcode = -1;
                    }
                }
                else
                {
                    errcode = g_sock_com_err;
                }
                if (log != null)
                {
                    log.LogInfo($"GetDI({id},{block},ref {level}) : {errcode}");
                }
                return errcode;
            }
            catch
            {
                return -1;
            }
        }

        /**
         * @brief  获取工具数字量输入
         * @param  [in] id  io编号，范围[0~1]
         * @param  [in] block  0-阻塞，1-非阻塞
         * @param  [out] level  0-低电平，1-高电平
         * @return  错误码
         */
        public int GetToolDI(int id, byte block, ref byte level)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int errcode = 0;

                if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
                {
                    if (id >= 0 && id < 2)
                    {
                        id += 1;
                        level = (byte)((robot_state_pkg.tl_dgt_input_l & (0x01 << id)) >> id);
                    }
                    else
                    {
                        level = 0;
                        errcode = -1;
                    }
                }
                else
                {
                    errcode = g_sock_com_err;
                }

                if (log != null)
                {
                    log.LogInfo($"GetToolDI({id},{block},ref {level}) : {errcode}");
                }
                return errcode;
            }
            catch
            {
                return -1;
            }
        }

        /**
         * @brief 等待控制箱数字量输入
         * @param  [in] id  io编号，范围[0~15]
         * @param  [in]  status 0-关，1-开
         * @param  [in]  max_time  最大等待时间，单位ms
         * @param  [in]  opt  超时后策略，0-程序停止并提示超时，1-忽略超时提示程序继续执行，2-一直等待
         * @return  错误码
         */
        public int WaitDI(int id, byte status, int max_time, int opt)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WaitDI(id, status, max_time, opt);
                if (log != null)
                {
                    log.LogInfo($"WaitDI({id},{status},{max_time},{opt}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 等待控制箱多路数字量输入
         * @param  [in] mode 0-多路与，1-多路或
         * @param  [in] id  io编号，bit0~bit7对应DI0~DI7，bit8~bit15对应CI0~CI7
         * @param  [in]  status 0-关，1-开
         * @param  [in]  max_time  最大等待时间，单位ms
         * @param  [in]  opt  超时后策略，0-程序停止并提示超时，1-忽略超时提示程序继续执行，2-一直等待
         * @return  错误码
         */
        public int WaitMultiDI(int mode, int id, int status, int max_time, int opt)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WaitMultiDI(mode, id, status, max_time, opt);
                if (log != null)
                {
                    log.LogInfo($"WaitMultiDI({id},{status},{max_time},{opt}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief 等待工具数字量输入
         * @param  [in] id  io编号，范围[0~1]
         * @param  [in]  status 0-关，1-开
         * @param  [in]  max_time  最大等待时间，单位ms
         * @param  [in]  opt  超时后策略，0-程序停止并提示超时，1-忽略超时提示程序继续执行，2-一直等待
         * @return  错误码
         */
        public int WaitToolDI(int id, byte status, int max_time, int opt)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WaitToolDI(id + 1, status, max_time, opt);
                if (log != null)
                {
                    log.LogInfo($"WaitToolDI({id},{status},{max_time},{opt}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取控制箱模拟量输入
         * @param  [in] id  io编号，范围[0~1]
         * @param  [in] block  0-阻塞，1-非阻塞
         * @param  [out] persent  输入电流或电压值百分比，范围[0~100]对应电流值[0~20mS]或电压[0~10V]
         * @return  错误码
         */
        public int GetAI(int id, byte block, ref float persent)
        {
            try
            {
                int errcode = 0;

                if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
                {
                    if (id >= 0 && id < 2)
                    {
                        persent = (float)(robot_state_pkg.cl_analog_input[id] / 40.95);
                    }
                    else
                    {
                        persent = 0;
                        errcode = -1;
                    }
                }
                else
                {
                    errcode = g_sock_com_err;
                }
                if (log != null)
                {
                    log.LogInfo($"GetAI({id},{block},ref {persent}) : {errcode}");
                }
                return errcode;
            }
            catch
            {
                return -1;
            }
        }

        /**
         * @brief  获取工具模拟量输入
         * @param  [in] id  io编号，范围[0]
         * @param  [in] block  0-阻塞，1-非阻塞
         * @param  [out] persent  输入电流或电压值百分比，范围[0~100]对应电流值[0~20mS]或电压[0~10V]
         * @return  错误码
         */
        public int GetToolAI(int id, byte block, ref float persent)
        {
            try
            {
                int errcode = 0;

                if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
                {
                    persent = (float)(robot_state_pkg.tl_anglog_input / 40.95);
                }
                else
                {
                    errcode = g_sock_com_err;
                }
                if (log != null)
                {
                    log.LogInfo($"GetToolAI({id},{block},ref {persent}) : {errcode}");
                }
                return errcode;
            }
            catch
            {
                return -1;
            }

        }

        /**
         * @brief 获取机器人末端点记录按钮状态
         * @param [out] state 按钮状态，0-按下，1-松开
         * @return 错误码
         */
        public int GetAxlePointRecordBtnState(ref byte state)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                state = (byte)((robot_state_pkg.tl_dgt_input_l & 0x10) >> 4);
            }
            else
            {
                errcode = g_sock_com_err;
            }

            if (log != null)
            {
                log.LogInfo($"GetAxlePointRecordBtnState(ref {state}) : {errcode}");
            }

            return errcode;
        }

        /**
         * @brief 获取机器人末端DO输出状态
         * @param [out] do_state DO输出状态，do0~do1对应bit1~bit2,从bit0开始
         * @return 错误码
         */
        public int GetToolDO(ref byte do_state)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                do_state = robot_state_pkg.tl_dgt_output_l;
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetToolDO(ref {do_state}) : {errcode}");
            }

            return errcode;
        }

        /**
         * @brief 获取机器人控制器DO输出状态
         * @param [out] do_state_h DO输出状态，co0~co7对应bit0~bit7
         * @param [out] do_state_l DO输出状态，do0~do7对应bit0~bit7
         * @return 错误码
         */
        public int GetDO(ref int do_state_h, ref int do_state_l)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                do_state_h = robot_state_pkg.cl_dgt_output_h;
                do_state_l = robot_state_pkg.cl_dgt_output_l;
            }
            else
            {
                errcode = g_sock_com_err;
            }

            if (log != null)
            {
                log.LogInfo($"GetDO(ref {do_state_h}, ref {do_state_l}) : {errcode}");
            }

            return errcode;
        }

        /**
         * @brief 等待控制箱模拟量输入
         * @param  [in] id  io编号，范围[0~1]
         * @param  [in]  sign 0-大于，1-小于
         * @param  [in]  value 输入电流或电压值百分比，范围[0~100]对应电流值[0~20mS]或电压[0~10V]
         * @param  [in]  max_time  最大等待时间，单位ms
         * @param  [in]  opt  超时后策略，0-程序停止并提示超时，1-忽略超时提示程序继续执行，2-一直等待
         * @return  错误码
         */
        public int WaitAI(int id, int sign, float value, int max_time, int opt)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WaitAI(id, sign, value * 40.95, max_time, opt);
                if (log != null)
                {
                    log.LogInfo($"WaitAI({id},{sign},{id},{value},{max_time},{opt}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 等待工具模拟量输入
         * @param  [in] id  io编号，范围[0]
         * @param  [in]  sign 0-大于，1-小于
         * @param  [in]  value 输入电流或电压值百分比，范围[0~100]对应电流值[0~20mS]或电压[0~10V]
         * @param  [in]  max_time  最大等待时间，单位ms
         * @param  [in]  opt  超时后策略，0-程序停止并提示超时，1-忽略超时提示程序继续执行，2-一直等待
         * @return  错误码
         */
        public int WaitToolAI(int id, int sign, float value, int max_time, int opt)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WaitToolAI(id, sign, value * 40.95, max_time, opt);
                if (log != null)
                {
                    log.LogInfo($"WaitToolAI({id},{sign},{id},{value},{max_time},{opt}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置全局速度
         * @param  [in]  vel  速度百分比，范围[0~100]
         * @return  错误码
         */
        public int SetSpeed(int vel)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetSpeed(vel);
                if (log != null)
                {
                    log.LogInfo($"SetSpeed({vel}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  设置系统变量值
         * @param  [in]  id  变量编号，范围[1~20]
         * @param  [in]  value 变量值
         * @return  错误码
         */
        public int SetSysVarValue(int id, double value)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetSysVarValue(id, value);
                if (log != null)
                {
                    log.LogInfo($"SetSysVarValue({id},{value}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置工具参考点-六点法
         * @param [in] point_num 点编号,范围[1~6] 
         * @return 错误码
         */
        public int SetToolPoint(int point_num)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetToolPoint(point_num);
                if (log != null)
                {
                    log.LogInfo($"SetToolPoint({point_num}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  计算工具坐标系
         * @param [out] tcp_pose 工具坐标系
         * @return 错误码
         */
        public int ComputeTool(ref DescPose tcp_pose)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                object[] result = proxy.ComputeTool();
                if ((int)result[0] == 0)
                {
                    tcp_pose.tran.x = (double)result[1];
                    tcp_pose.tran.y = (double)result[2];
                    tcp_pose.tran.z = (double)result[3];
                    tcp_pose.rpy.rx = (double)result[4];
                    tcp_pose.rpy.ry = (double)result[5];
                    tcp_pose.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"ComputeTool(ref {tcp_pose.tran.x},ref {tcp_pose.tran.y},ref {tcp_pose.tran.z},ref {tcp_pose.rpy.rx},ref {tcp_pose.rpy.ry},ref {tcp_pose.rpy.rz}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置工具参考点-四点法
         * @param [in] point_num 点编号,范围[1~4] 
         * @return 错误码
         */
        public int SetTcp4RefPoint(int point_num)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetTcp4RefPoint(point_num);
                if (log != null)
                {
                    log.LogInfo($"SetTcp4RefPoint({point_num}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  计算工具坐标系
         * @param [out] tcp_pose 工具坐标系
         * @return 错误码
         */
        public int ComputeTcp4(ref DescPose tcp_pose)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                object[] result = proxy.ComputeTcp4();
                if ((int)result[0] == 0)
                {
                    tcp_pose.tran.x = (double)result[1];
                    tcp_pose.tran.y = (double)result[2];
                    tcp_pose.tran.z = (double)result[3];
                    tcp_pose.rpy.rx = (double)result[4];
                    tcp_pose.rpy.ry = (double)result[5];
                    tcp_pose.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"ComputeTcp4(ref {tcp_pose.tran.x},ref {tcp_pose.tran.y},ref {tcp_pose.tran.z},ref {tcp_pose.rpy.rx},ref {tcp_pose.rpy.ry},ref {tcp_pose.rpy.rz}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置工具坐标系
         * @param  [in] id 坐标系编号，范围[0~14]
         * @param  [in] coord  工具中心点相对于末端法兰中心位姿
         * @param  [in] type  0-工具坐标系，1-传感器坐标系
         * @param  [in] install 安装位置，0-机器人末端，1-机器人外部
         * param   [in] toolID 工具ID
         * @param  [in] loadNum 负载编号
         * @return  错误码
         */
        public int SetToolCoord(int id, DescPose coord, int type, int install, int toolID, int loadNum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] descCoord = new double[6] { coord.tran.x, coord.tran.y, coord.tran.z, coord.rpy.rx, coord.rpy.ry, coord.rpy.rz };
                int rtn = proxy.SetToolCoord(id, descCoord, type, install, toolID, loadNum);
                if (log != null)
                {
                    log.LogInfo($"SetToolCoord({id},{descCoord[0]},{descCoord[1]},{descCoord[2]},{descCoord[3]},{descCoord[4]},{descCoord[5]},{type},{install},{toolID},{loadNum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  设置工具坐标系列表
         * @param  [in] id 坐标系编号，范围[1~15]
         * @param  [in] coord  工具中心点相对于末端法兰中心位姿
         * @param  [in] type  0-工具坐标系，1-传感器坐标系
         * @param  [in] install 安装位置，0-机器人末端，1-机器人外部
         * @param  [in] loadNum 负载编号
         * @return  错误码
         */
        public int SetToolList(int id, DescPose coord, int type, int install, int loadNum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] descCoord = new double[6] { coord.tran.x, coord.tran.y, coord.tran.z, coord.rpy.rx, coord.rpy.ry, coord.rpy.rz };
                int rtn = proxy.SetToolList(id, descCoord, type, install, loadNum);
                if (log != null)
                {
                    log.LogInfo($"SetToolList({id},{descCoord[0]},{descCoord[1]},{descCoord[2]},{descCoord[3]},{descCoord[4]},{descCoord[5]},{type},{install},{loadNum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /**
         * @brief 设置外部工具参考点-三点法
         * @param [in] point_num 点编号,范围[1~3] 
         * @return 错误码
         */
        public int SetExTCPPoint(int point_num)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetExTCPPoint(point_num);
                if (log != null)
                {
                    log.LogInfo($"SetExTCPPoint({point_num}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  计算外部工具坐标系-三点法
         * @param [out] tcp_pose 外部工具坐标系
         * @return 错误码
         */
        public int ComputeExTCF(ref DescPose tcp_pose)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                object[] result = proxy.ComputeExTCF();
                if ((int)result[0] == 0)
                {
                    tcp_pose.tran.x = (double)result[1];
                    tcp_pose.tran.y = (double)result[2];
                    tcp_pose.tran.z = (double)result[3];
                    tcp_pose.rpy.rx = (double)result[4];
                    tcp_pose.rpy.ry = (double)result[5];
                    tcp_pose.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"ComputeExTCF(ref {tcp_pose.tran.x},ref {tcp_pose.tran.y},ref {tcp_pose.tran.z},ref {tcp_pose.rpy.rx},ref {tcp_pose.rpy.ry},ref {tcp_pose.rpy.rz}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置外部工具坐标系
         * @param  [in] id 坐标系编号，范围[0~14]
         * @param  [in] etcp  工具中心点相对末端法兰中心位姿
         * @param  [in] etool  待定
         * @return  错误码
         */
        public int SetExToolCoord(int id, DescPose etcp, DescPose etool)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] descEtcp = new double[6] { etcp.tran.x, etcp.tran.y, etcp.tran.z, etcp.rpy.rx, etcp.rpy.ry, etcp.rpy.rz };
                double[] descEtool = new double[6] { etool.tran.x, etool.tran.y, etool.tran.z, etool.rpy.rx, etool.rpy.ry, etool.rpy.rz };
                int rtn = proxy.SetExToolCoord(id, descEtcp, descEtool);
                if (log != null)
                {
                    log.LogInfo($"SetExToolCoord({id},{descEtcp[0]},{descEtcp[1]},{descEtcp[2]},{descEtcp[3]},{descEtcp[4]},{descEtcp[5]},{descEtool[0]},{descEtool[1]},{descEtool[2]},{descEtool[3]},{descEtool[4]},{descEtool[5]}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置外部工具坐标系列表
         * @param  [in] id 坐标系编号，范围[0~14]
         * @param  [in] etcp  工具中心点相对末端法兰中心位姿
         * @param  [in] etool  待定
         * @return  错误码
         */
        public int SetExToolList(int id, DescPose etcp, DescPose etool)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] descEtcp = new double[6] { etcp.tran.x, etcp.tran.y, etcp.tran.z, etcp.rpy.rx, etcp.rpy.ry, etcp.rpy.rz };
                double[] descEtool = new double[6] { etool.tran.x, etool.tran.y, etool.tran.z, etool.rpy.rx, etool.rpy.ry, etool.rpy.rz };
                int rtn = proxy.SetExToolList(id, descEtcp, descEtool);
                if (log != null)
                {
                    log.LogInfo($"SetExToolList({id},{descEtcp[0]},{descEtcp[1]},{descEtcp[2]},{descEtcp[3]},{descEtcp[4]},{descEtcp[5]},{descEtool[0]},{descEtool[1]},{descEtool[2]},{descEtool[3]},{descEtool[4]},{descEtool[5]}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置工件参考点-三点法
         * @param [in] point_num 点编号,范围[1~3] 
         * @return 错误码
         */
        public int SetWObjCoordPoint(int point_num)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetWObjCoordPoint(point_num);
                if (log != null)
                {
                    log.LogInfo($"SetWObjCoordPoint({point_num}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  计算工件坐标系
         * @param [in] method 计算方法 0：原点-x轴-z轴  1：原点-x轴-xy平面
         * @param [in] refFrame 参考坐标系
         * @param [out] wobj_pose 工件坐标系
         * @return 错误码
         */
        public int ComputeWObjCoord(int method, int refFrame, ref DescPose wobj_pose)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                object[] result = proxy.ComputeWObjCoord(method, refFrame);
                if ((int)result[0] == 0)
                {
                    wobj_pose.tran.x = (double)result[1];
                    wobj_pose.tran.y = (double)result[2];
                    wobj_pose.tran.z = (double)result[3];
                    wobj_pose.rpy.rx = (double)result[4];
                    wobj_pose.rpy.ry = (double)result[5];
                    wobj_pose.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"ComputeWObjCoord(ref {wobj_pose.tran.x},ref {wobj_pose.tran.y},ref {wobj_pose.tran.z},ref {wobj_pose.rpy.rx},ref {wobj_pose.rpy.ry},ref {wobj_pose.rpy.rz}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }


        /**
         * @brief  设置工件坐标系
         * @param  [in] id 坐标系编号，范围[1~15]
         * @param  [in] coord  工件坐标系相对于末端法兰中心位姿
         * @param  [in] refFrame 参考坐标系
         * @return  错误码
         */
        public int SetWObjCoord(int id, DescPose coord, int refFrame)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] descCoord = new double[6] { coord.tran.x, coord.tran.y, coord.tran.z, coord.rpy.rx, coord.rpy.ry, coord.rpy.rz };
                int rtn = proxy.SetWObjCoord(id, descCoord, refFrame);
                if (log != null)
                {
                    log.LogInfo($"SetWObjCoord({id},{descCoord[0]},{descCoord[1]},{descCoord[2]},{descCoord[3]},{descCoord[4]},{descCoord[5]}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置工件坐标系列表
         * @param  [in] id 坐标系编号，范围[0~14]
         * @param  [in] coord  工件坐标系相对于末端法兰中心位姿
         * @param  [in] refFrame 参考坐标系
         * @return  错误码
         */
        public int SetWObjList(int id, DescPose coord, int refFrame)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] descCoord = new double[6] { coord.tran.x, coord.tran.y, coord.tran.z, coord.rpy.rx, coord.rpy.ry, coord.rpy.rz };
                int rtn = proxy.SetWObjList(id, descCoord, refFrame);
                if (log != null)
                {
                    log.LogInfo($"SetWObjList({id},{descCoord[0]},{descCoord[1]},{descCoord[2]},{descCoord[3]},{descCoord[4]},{descCoord[5]}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置末端负载重量
         * @param  [in] loadNum 负载编号
         * @param  [in] weight  负载重量，单位kg
         * @return  错误码
         */
        public int SetLoadWeight(int loadNum, float weight)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetLoadWeight(loadNum, weight);
                if (log != null)
                {
                    log.LogInfo($"SetLoadWeight({weight}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置末端负载质心坐标
         * @param  [in] coord 质心坐标，单位mm
         * @return  错误码
         */
        public int SetLoadCoord(DescTran coord)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetLoadCoord(coord.x, coord.y, coord.z);
                if (log != null)
                {
                    log.LogInfo($"SetLoadCoord({coord.x},{coord.y},{coord.z}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置机器人安装方式
         * @param  [in] install  安装方式，0-正装，1-侧装，2-倒装
         * @return  错误码
         */
        public int SetRobotInstallPos(byte install)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetRobotInstallPos(install);
                if (log != null)
                {
                    log.LogInfo($"SetRobotInstallPos({install}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (log != null)
                {
                    log.LogError($"RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
         * @brief  设置机器人安装角度，自由安装
         * @param  [in] yangle  倾斜角
         * @param  [in] zangle  旋转角
         * @return  错误码
         */
        public int SetRobotInstallAngle(double yangle, double zangle)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetRobotInstallAngle(yangle, zangle);
                if (log != null)
                {
                    log.LogInfo($"SetRobotInstallAngle({yangle},{zangle}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  等待指定时间
         * @param  [in]  t_ms  单位ms
         * @return  错误码
         */
        public int WaitMs(int t_ms)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WaitMs(t_ms);
                if (log != null)
                {
                    log.LogInfo($"WaitMs({t_ms}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置碰撞等级
         * @param  [in]  mode  0-等级，1-百分比
         * @param  [in]  level 碰撞阈值，等级对应范围[],百分比对应范围[0~1]
         * @param  [in]  config 0-不更新配置文件，1-更新配置文件
         * @return  错误码
         */
        public int SetAnticollision(int mode, double[] level, int config)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] collisionLevel;
                if (mode == 1)
                {
                    collisionLevel = Array.ConvertAll<double, double>(level, t => Convert.ToDouble(t) * 10);
                }
                else
                {
                    collisionLevel = level;
                }
                int rtn = proxy.SetAnticollision(mode, collisionLevel, config);
                if (log != null)
                {
                    log.LogInfo($"SetAnticollision({mode},{collisionLevel[0]},{collisionLevel[1]},{collisionLevel[2]},{collisionLevel[3]},{collisionLevel[4]},{collisionLevel[5]},{config}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置碰撞后策略
         * @param  [in] strategy  0-报错停止，1-继续运行
         * @param  [in] safeTime  安全停止时间[1000 - 2000]ms
         * @param  [in] safeDistance  安全停止距离[1-150]mm
         * @param  [in] safeVel  tcp安全停止速度 [50-250]mm/s
         * @param  [in] safetyMargin  j1-j6安全系数[1-10]
         * @return  错误码
         */
        public int SetCollisionStrategy(int strategy, int safeTime, int safeDistance, int safeVel, int[] safetyMargin)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetCollisionStrategy(strategy, safeTime, safeDistance, safeVel, safetyMargin);
                if (log != null)
                {
                    log.LogInfo($"SetCollisionStrategy({strategy}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  设置正限位
         * @param  [in] limit 六个关节位置，单位deg
         * @return  错误码
         */
        public int SetLimitPositive(double[] limit)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetLimitPositive(limit);
                if (log != null)
                {
                    log.LogInfo($"SetLimitPositive({limit}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置负限位
         * @param  [in] limit 六个关节位置，单位deg
         * @return  错误码
         */
        public int SetLimitNegative(double[] limit)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetLimitNegative(limit);
                if (log != null)
                {
                    log.LogInfo($"SetLimitNegative({limit}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  错误状态清除
         * @return  错误码
         */
        public int ResetAllError()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ResetAllError();
                if (log != null)
                {
                    log.LogInfo($"ResetAllError() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  关节摩擦力补偿开关
         * @param  [in]  state  0-关，1-开
         * @return  错误码
         */
        public int FrictionCompensationOnOff(byte state)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.FrictionCompensationOnOff(state);
                if (log != null)
                {
                    log.LogInfo($"FrictionCompensationOnOff({state}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置关节摩擦力补偿系数-正装
         * @param  [in]  coeff 六个关节补偿系数，范围[0~1]
         * @return  错误码
         */
        public int SetFrictionValue_level(double[] coeff)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetFrictionValue_level(coeff);
                if (log != null)
                {
                    log.LogInfo($"SetFrictionValue_level({coeff}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置关节摩擦力补偿系数-侧装
         * @param  [in]  coeff 六个关节补偿系数，范围[0~1]
         * @return  错误码
         */
        public int SetFrictionValue_wall(double[] coeff)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetFrictionValue_wall(coeff);
                if (log != null)
                {
                    log.LogInfo($"SetFrictionValue_wall({coeff}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置关节摩擦力补偿系数-倒装
         * @param  [in]  coeff 六个关节补偿系数，范围[0~1]
         * @return  错误码
         */
        public int SetFrictionValue_ceiling(double[] coeff)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetFrictionValue_ceiling(coeff);
                if (log != null)
                {
                    log.LogInfo($"SetFrictionValue_ceiling({coeff}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置关节摩擦力补偿系数-自由安装
         * @param  [in]  coeff 六个关节补偿系数，范围[0~1]
         * @return  错误码
         */
        public int SetFrictionValue_freedom(double[] coeff)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetFrictionValue_freedom(coeff);
                if (log != null)
                {
                    log.LogInfo($"SetFrictionValue_freedom({coeff}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  获取机器人安装角度
         * @param  [out] yangle 倾斜角
         * @param  [out] zangle 旋转角
         * @return  错误码
         */
        public int GetRobotInstallAngle(ref double yangle, ref double zangle)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetRobotInstallAngle();
                if ((int)result[0] == 0)
                {
                    yangle = (double)result[1];
                    zangle = (double)result[2];
                }
                if (log != null)
                {
                    log.LogInfo($"GetRobotInstallAngle(ref {yangle}, ref {zangle}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (log != null)
                {
                    log.LogError($"RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
         * @brief  获取系统变量值
         * @param  [in] id 系统变量编号，范围[1~20]
         * @param  [out] value  系统变量值
         * @return  错误码
         */
        public int GetSysVarValue(int id, ref double value)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetSysVarValue(id);
                if ((int)result[0] == 0)
                {
                    value = (double)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetSysVarValue({id}, ref {value}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取当前关节位置(角度)
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] jPos 六个关节位置，单位deg
         * @return  错误码
         */
        public int GetActualJointPosDegree(byte flag, ref JointPos jPos)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                for (int i = 0; i < 6; i++)
                {
                    jPos.jPos[i] = robot_state_pkg.jt_cur_pos[i];
                }
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetActualJointPosDegree({flag}, ref {jPos.jPos[0]},ref {jPos.jPos[1]},ref {jPos.jPos[2]},ref {jPos.jPos[3]}) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief  获取当前关节位置(弧度)
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] jPos 六个关节位置，单位rad
         * @return  错误码
         */
        public int GetActualJointPosRadian(byte flag, ref JointPos jPos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetActualJointPosRadian(flag);
                if ((int)result[0] == 0)
                {
                    jPos.jPos[0] = (double)result[1];
                    jPos.jPos[1] = (double)result[2];
                    jPos.jPos[2] = (double)result[3];
                    jPos.jPos[3] = (double)result[4];
                    jPos.jPos[4] = (double)result[5];
                    jPos.jPos[5] = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"GetActualJointPosRadian({flag},ref {jPos.jPos[0]},ref {jPos.jPos[1]}, ref {jPos.jPos[2]}, ref {jPos.jPos[3]}, ref {jPos.jPos[4]}, ref {jPos.jPos[5]}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取关节反馈速度-deg/s
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] speed 六个关节速度
         * @return  错误码 
         */
        public int GetActualJointSpeedsDegree(byte flag, ref double[] speed)
        {
            int errcode = 0;
            int i;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                for (i = 0; i < 6; i++)
                {
                    speed[i] = (double)robot_state_pkg.actual_qd[i];
                }
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetActualJointSpeedsDegree({flag},ref {speed[0]},ref {speed[1]}, ref {speed[2]}, ref {speed[3]}, ref {speed[4]}, ref {speed[5]}) : {errcode}");
            }
            return errcode;
        }


        /**
         * @brief  获取关节反馈加速度-deg/s^2
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] acc 六个关节加速度
         * @return  错误码 
         */
        public int GetActualJointAccDegree(byte flag, ref double[] acc)
        {
            int errcode = 0;
            int i;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                for (i = 0; i < 6; i++)
                {
                    acc[i] = robot_state_pkg.actual_qdd[i];
                }
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetActualJointAccDegree({flag},ref {acc[0]},ref {acc[1]}, ref {acc[2]}, ref {acc[3]}, ref {acc[4]}, ref {acc[5]}) : {errcode}");
            }
            return errcode;

        }

        /**
         * @brief  获取TCP指令速度
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] tcp_speed 线性速度
         * @param  [out] ori_speed 姿态速度
         * @return  错误码 
         */
        public int GetTargetTCPCompositeSpeed(byte flag, ref double tcp_speed, ref double ori_speed)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                tcp_speed = (double)robot_state_pkg.target_TCP_CmpSpeed[0];
                ori_speed = (double)robot_state_pkg.target_TCP_CmpSpeed[1];
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetTargetTCPCompositeSpeed({flag},ref {tcp_speed},ref {ori_speed}) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief  获取TCP反馈速度
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] tcp_speed 线性速度
         * @param  [out] ori_speed 姿态速度
         * @return  错误码 
         */
        public int GetActualTCPCompositeSpeed(byte flag, ref double tcp_speed, ref double ori_speed)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                tcp_speed = (double)robot_state_pkg.actual_TCP_CmpSpeed[0];
                ori_speed = (double)robot_state_pkg.actual_TCP_CmpSpeed[1];
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetActualTCPCompositeSpeed({flag},ref {tcp_speed},ref {ori_speed}) : {errcode}");
            }
            return errcode;

        }

        /**
         * @brief  获取TCP指令速度
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] speed [x,y,z,rx,ry,rz]速度
         * @return  错误码 
         */
        public int GetTargetTCPSpeed(byte flag, ref double[] speed)
        {
            int errcode = 0;
            int i;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                for (i = 0; i < 6; i++)
                {
                    speed[i] = (double)robot_state_pkg.target_TCP_Speed[i];
                }
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetTargetTCPSpeed({flag},ref {speed[0]},ref {speed[1]}, ref {speed[2]}, ref {speed[3]}, ref {speed[4]}, ref {speed[5]}) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief  获取TCP反馈速度
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] speed [x,y,z,rx,ry,rz]速度
         * @return  错误码 
         */
        public int GetActualTCPSpeed(byte flag, ref double[] speed)
        {
            int errcode = 0;
            int i;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                for (i = 0; i < 6; i++)
                {
                    speed[i] = (double)robot_state_pkg.actual_TCP_Speed[i];
                }
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetActualTCPSpeed({flag},ref {speed[0]},ref {speed[1]}, ref {speed[2]}, ref {speed[3]}, ref {speed[4]}, ref {speed[5]}) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief  获取当前工具位姿
         * @param  [in] flag  0-阻塞，1-非阻塞
         * @param  [out] desc_pos  工具位姿
         * @return  错误码
         */
        public int GetActualTCPPose(byte flag, ref DescPose desc_pos)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                desc_pos.tran.x = robot_state_pkg.tl_cur_pos[0];
                desc_pos.tran.y = robot_state_pkg.tl_cur_pos[1];
                desc_pos.tran.z = robot_state_pkg.tl_cur_pos[2];
                desc_pos.rpy.rx = robot_state_pkg.tl_cur_pos[3];
                desc_pos.rpy.ry = robot_state_pkg.tl_cur_pos[4];
                desc_pos.rpy.rz = robot_state_pkg.tl_cur_pos[5];
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetActualTCPPose({flag}, ref {desc_pos.tran.x},ref {desc_pos.tran.y},ref {desc_pos.tran.z},ref {desc_pos.rpy.rx},ref {desc_pos.rpy.ry},ref {desc_pos.rpy.rz},) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief  获取当前工具坐标系编号
         * @param  [in] flag  0-阻塞，1-非阻塞
         * @param  [out] id  工具坐标系编号
         * @return  错误码
         */
        public int GetActualTCPNum(byte flag, ref int id)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                id = robot_state_pkg.tool;
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetActualTCPNum({flag}, ref {id}) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief  获取当前工件坐标系编号
         * @param  [in] flag  0-阻塞，1-非阻塞
         * @param  [out] id  工件坐标系编号
         * @return  错误码
         */
        public int GetActualWObjNum(byte flag, ref int id)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                id = robot_state_pkg.user;
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetActualWObjNum({flag}, ref {id}) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief  获取当前末端法兰位姿
         * @param  [in] flag  0-阻塞，1-非阻塞
         * @param  [out] desc_pos  法兰位姿
         * @return  错误码
         */
        public int GetActualToolFlangePose(byte flag, ref DescPose desc_pos)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                desc_pos.tran.x = robot_state_pkg.flange_cur_pos[0];
                desc_pos.tran.y = robot_state_pkg.flange_cur_pos[1];
                desc_pos.tran.z = robot_state_pkg.flange_cur_pos[2];
                desc_pos.rpy.rx = robot_state_pkg.flange_cur_pos[3];
                desc_pos.rpy.ry = robot_state_pkg.flange_cur_pos[4];
                desc_pos.rpy.rz = robot_state_pkg.flange_cur_pos[5];
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetActualToolFlangePose({flag}, ref {desc_pos.tran.x},ref {desc_pos.tran.y},ref {desc_pos.tran.z},ref {desc_pos.rpy.rx},ref {desc_pos.rpy.ry},ref {desc_pos.rpy.rz},) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief  逆运动学求解
         * @param  [in] type 0-绝对位姿(基坐标系)，1-增量位姿(基坐标系)，2-增量位姿(工具坐标系)
         * @param  [in] desc_pos 笛卡尔位姿
         * @param  [in] config 关节空间配置，[-1]-参考当前关节位置解算，[0~7]-依据特定关节空间配置求解
         * @param  [out] joint_pos 关节位置
         * @return  错误码
         */
        public int GetInverseKin(int type, DescPose desc_pos, int config, ref JointPos joint_pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] descPos = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                object[] result = proxy.GetInverseKin(type, descPos, config);
                if ((int)result[0] == 0)
                {
                    joint_pos.jPos[0] = (double)result[1];
                    joint_pos.jPos[1] = (double)result[2];
                    joint_pos.jPos[2] = (double)result[3];
                    joint_pos.jPos[3] = (double)result[4];
                    joint_pos.jPos[4] = (double)result[5];
                    joint_pos.jPos[5] = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"GetInverseKin({type},{descPos[0]},{descPos[1]},{descPos[2]},{descPos[3]},{descPos[4]},{descPos[5]},{config},ref {joint_pos.jPos[0]},ref {joint_pos.jPos[1]}, ref {joint_pos.jPos[2]}, ref {joint_pos.jPos[3]}, ref {joint_pos.jPos[4]}, ref {joint_pos.jPos[5]}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  逆运动学求解，参考指定关节位置求解
         * @param  [in] posMode 0绝对位姿， 1相对位姿-基坐标系   2相对位姿-工具坐标系
         * @param  [in] desc_pos 笛卡尔位姿
         * @param  [in] joint_pos_ref 参考关节位置
         * @param  [out] joint_pos 关节位置
         * @return  错误码
         */
        public int GetInverseKinRef(int posMode, DescPose desc_pos, JointPos joint_pos_ref, ref JointPos joint_pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] descPos = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                double[] jointPos = joint_pos_ref.jPos;
                object[] result = proxy.GetInverseKinRef(posMode, descPos, jointPos);
                if ((int)result[0] == 0)
                {
                    joint_pos.jPos[0] = (double)result[1];
                    joint_pos.jPos[1] = (double)result[2];
                    joint_pos.jPos[2] = (double)result[3];
                    joint_pos.jPos[3] = (double)result[4];
                    joint_pos.jPos[4] = (double)result[5];
                    joint_pos.jPos[5] = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"GetInverseKinRef({posMode},{descPos[0]},{descPos[1]},{descPos[2]},{descPos[3]},{descPos[4]},{descPos[5]},{jointPos[0]},{jointPos[1]},{jointPos[2]},{jointPos[3]},{jointPos[4]},{jointPos[5]},ref {joint_pos.jPos[0]},ref {joint_pos.jPos[1]}, ref {joint_pos.jPos[2]}, ref {joint_pos.jPos[3]}, ref {joint_pos.jPos[4]}, ref {joint_pos.jPos[5]}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  逆运动学求解，参考指定关节位置判断是否有解
         * @param  [in] posMode 0绝对位姿， 1相对位姿-基坐标系   2相对位姿-工具坐标系
         * @param  [in] desc_pos 笛卡尔位姿
         * @param  [in] joint_pos_参考关节位置
         * @param  [out] hasResult 0-无解，1-有解
         * @return  错误码
         */
        public int GetInverseKinHasSolution(int posMode, DescPose desc_pos, JointPos joint_pos_ref, ref bool hasResult)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] descPos = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                double[] jointPos = joint_pos_ref.jPos;
                object[] result = proxy.GetInverseKinHasSolution(posMode, descPos, jointPos);
                if ((int)result[0] == 0)
                {
                    hasResult = (bool)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetInverseKinHasSolution({posMode}, {desc_pos.tran.x},{desc_pos.tran.y},{desc_pos.tran.z},{desc_pos.rpy.rx},{desc_pos.rpy.ry},{desc_pos.rpy.rz},{jointPos[0]},{jointPos[1]},{jointPos[2]},{jointPos[3]},{jointPos[4]},{jointPos[5]},{hasResult}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {

                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  正运动学求解
         * @param  [in] joint_pos 关节位置
         * @param  [out] desc_pos 笛卡尔位姿
         * @return  错误码
         */
        public int GetForwardKin(JointPos joint_pos, ref DescPose desc_pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] jointPos = joint_pos.jPos;
                object[] result = proxy.GetForwardKin(jointPos);
                if ((int)result[0] == 0)
                {
                    desc_pos.tran.x = (double)result[1];
                    desc_pos.tran.y = (double)result[2];
                    desc_pos.tran.z = (double)result[3];
                    desc_pos.rpy.rx = (double)result[4];
                    desc_pos.rpy.ry = (double)result[5];
                    desc_pos.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"GetForwardKin({jointPos[0]},{jointPos[1]},{jointPos[2]},{jointPos[3]},{jointPos[4]},{jointPos[5]}, ref {desc_pos.tran.x},ref {desc_pos.tran.y},ref {desc_pos.tran.z},ref {desc_pos.rpy.rx},ref {desc_pos.rpy.ry},ref {desc_pos.rpy.rz},) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief 获取当前关节转矩
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] torques 关节转矩
         * @return  错误码
         */
        public int GetJointTorques(byte flag, double[] torques)
        {
            int errcode = 0;
            int i;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                for (i = 0; i < 6; i++)
                {
                    torques[i] = (float)robot_state_pkg.jt_cur_tor[i];
                }
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetJointTorques({flag}, {torques[0]},{torques[1]},{torques[2]},{torques[3]},{torques[4]},{torques[5]},{torques}) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief  获取当前负载的重量
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] weight 负载重量，单位kg
         * @return  错误码
         */
        public int GetTargetPayload(byte flag, ref double weight)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetTargetPayload(flag);
                if ((int)result[0] == 0)
                {
                    weight = (double)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetTargetPayload({flag}, {weight}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取当前负载的质心
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] cog 负载质心，单位mm
         * @return  错误码
         */
        public int GetTargetPayloadCog(byte flag, ref DescTran cog)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetTargetPayloadCog(flag);
                if ((int)result[0] == 0)
                {
                    cog.x = (double)result[1];
                    cog.y = (double)result[2];
                    cog.z = (double)result[3];
                }
                if (log != null)
                {
                    log.LogInfo($"GetTargetPayloadCog({flag},ref {cog.x},ref {cog.y},ref {cog.z}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取当前工具坐标系
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] desc_pos 工具坐标系位姿
         * @return  错误码
         */
        public int GetTCPOffset(byte flag, ref DescPose desc_pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetTCPOffset(flag);
                if ((int)result[0] == 0)
                {
                    desc_pos.tran.x = (double)result[1];
                    desc_pos.tran.y = (double)result[2];
                    desc_pos.tran.z = (double)result[3];
                    desc_pos.rpy.rx = (double)result[4];
                    desc_pos.rpy.ry = (double)result[5];
                    desc_pos.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"GetTCPOffset({flag},ref {desc_pos.tran.x},ref {desc_pos.tran.y},ref {desc_pos.tran.z},ref {desc_pos.rpy.rx},ref {desc_pos.rpy.ry},ref {desc_pos.rpy.rz},) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  获取当前工件坐标系
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] desc_pos 工件坐标系位姿
         * @return  错误码
         */
        public int GetWObjOffset(byte flag, ref DescPose desc_pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetWObjOffset(flag);
                if ((int)result[0] == 0)
                {
                    desc_pos.tran.x = (double)result[1];
                    desc_pos.tran.y = (double)result[2];
                    desc_pos.tran.z = (double)result[3];
                    desc_pos.rpy.rx = (double)result[4];
                    desc_pos.rpy.ry = (double)result[5];
                    desc_pos.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"GetWObjOffset({flag},ref {desc_pos.tran.x},ref {desc_pos.tran.y},ref {desc_pos.tran.z},ref {desc_pos.rpy.rx},ref {desc_pos.rpy.ry},ref {desc_pos.rpy.rz},) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  获取关节软限位角度
         * @param  [in] flag 0-阻塞，1-非阻塞	 
         * @param  [out] negative  负限位角度，单位deg
         * @param  [out] positive  正限位角度，单位deg
         * @return  错误码
         */
        public int GetJointSoftLimitDeg(byte flag, ref double[] negative, ref double[] positive)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetJointSoftLimitDeg(flag);
                if ((int)result[0] == 0)
                {
                    negative[0] = (double)result[1];
                    positive[0] = (double)result[2];
                    negative[1] = (double)result[3];
                    positive[1] = (double)result[4];
                    negative[2] = (double)result[5];
                    positive[2] = (double)result[6];
                    negative[3] = (double)result[7];
                    positive[3] = (double)result[8];
                    negative[4] = (double)result[9];
                    positive[4] = (double)result[10];
                    negative[5] = (double)result[11];
                    positive[5] = (double)result[12];
                }
                if (log != null)
                {
                    log.LogInfo($"GetJointSoftLimitDeg({flag},{negative[0]},{negative[1]},{negative[2]},{negative[3]},{negative[4]},{negative[5]},{positive[0]},{positive[1]},{positive[2]},{positive[3]},{positive[4]},{positive[5]})");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  获取系统时间
         * @param  [out] t_ms 单位ms
         * @return  错误码
         */
        public int GetSystemClock(ref double t_ms)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetSystemClock();
                if ((int)result[0] == 0)
                {
                    t_ms = (double)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetSystemClock(ref {t_ms}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  获取机器人当前关节配置
         * @param  [out]  config  关节空间配置，范围[0~7]
         * @return  错误码
         */
        public int GetRobotCurJointsConfig(ref int config)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetRobotCurJointsConfig();
                if ((int)result[0] == 0)
                {
                    config = (int)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetRobotCurJointsConfig(ref {config}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取机器人默认速度
         * @param  [out]  vel  速度，单位mm/s
         * @return  错误码
         */
        public int GetDefaultTransVel(ref double vel)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetDefaultTransVel();
                if ((int)result[0] == 0)
                {
                    vel = (double)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetDefaultTransVel(ref {vel}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  查询机器人运动是否完成
         * @param  [out]  state  0-未完成，1-完成
         * @return  错误码
         */
        public int GetRobotMotionDone(ref byte state)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                state = (byte)robot_state_pkg.motion_done;
            }
            else
            {
                errcode = g_sock_com_err;
            }

            if (log != null)
            {
                log.LogInfo($"GetRobotMotionDone(ref {state}) : {errcode}");
            }

            return errcode;
        }

        /**
         * @brief  查询机器人错误码
         * @param  [out]  maincode  主错误码
         * @param  [out]  subcode   子错误码
         * @return  错误码
         */
        public int GetRobotErrorCode(ref int maincode, ref int subcode)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                maincode = robot_state_pkg.main_code;
                subcode = robot_state_pkg.sub_code;
            }
            else
            {
                errcode = g_sock_com_err;
            }

            if (log != null)
            {
                log.LogInfo($"GetRobotErrorCode(ref {maincode}, ref {subcode}) : {errcode}");
            }

            return errcode;
        }

        private int GetError()
        {
            int err = 0;
            int ree = 0;
            object[] result = proxy.GetRobotErrorCode();
            if ((int)result[0] == 0)
            {
                err = (int)result[1];
                ree = (int)result[2];
            }
            if (log != null)
            {
                log.LogInfo($"GetError( {err}, {ree}");
            }
            return (int)result[0];
        }

        /**
         * @brief  查询机器人示教管理点位数据
         * @param  [in]   name  点位名
         * @param  [out]  data   点位数据double[20]{x,y,z,rx,ry,rz,j1,j2,j3,j4,j5,j6,tool,wobj,speed,acc,e1,e2,e3,e4}
         * @return  错误码
         */
        public int GetRobotTeachingPoint(string name, ref double[] data)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetRobotTeachingPoint(name);
                if ((int)result[0] == 0)
                {
                    string paramStr = (string)result[1];
                    string[] parS = paramStr.Split(',');
                    if (parS.Length != 20)
                    {
                        log.LogError("get Teaching Point size fail");
                        return -1;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        data[i] = double.Parse(parS[i]);
                    }
                    return (int)result[0];
                }
                if (log != null)
                {
                    log.LogInfo($"GetRobotTeachingPoint(ref {name},ref {data[0]},ref {data[1]},ref {data[2]},ref {data[3]},ref {data[4]},ref {data[5]},ref {data[6]},ref {data[7]},ref {data[8]},ref {data[9]},ref {data[10]},ref {data[11]},ref {data[12]},ref {data[13]},ref {data[14]},ref {data[15]},ref {data[16]},ref {data[17]},ref {data[18]},ref {data[19]}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (log != null)
                {
                    log.LogError($"RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }


        /**
         * @brief  查询机器人运动队列缓存长度
         * @param  [out]  len  缓存长度
         * @return  错误码
         */
        public int GetMotionQueueLength(ref int len)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                len = robot_state_pkg.mc_queue_len;
            }
            else
            {
                errcode = g_sock_com_err;
            }

            if (log != null)
            {
                log.LogInfo($"GetMotionQueueLength(ref {len}) : {errcode}");
            }

            return errcode;
        }


        /**
         * @brief  设置轨迹记录参数
         * @param  [in] type  记录数据类型，1-关节位置
         * @param  [in] name  轨迹文件名
         * @param  [in] period_ms  数据采样周期，固定值2ms或4ms或8ms
         * @param  [in] di_choose  DI选择,bit0~bit7对应控制箱DI0~DI7，bit8~bit9对应末端DI0~DI1，0-不选择，1-选择
         * @param  [in] do_choose  DO选择,bit0~bit7对应控制箱DO0~DO7，bit8~bit9对应末端DO0~DO1，0-不选择，1-选择
         * @return  错误码
         */
        public int SetTPDParam(int type, string name, int period_ms, UInt16 di_choose, UInt16 do_choose)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetTPDParam(type, name, period_ms, di_choose, do_choose);
                if (log != null)
                {
                    log.LogInfo($"SetTPDParam({type},{name},{period_ms},{di_choose},{do_choose}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  开始轨迹记录
         * @param  [in] type  记录数据类型，1-关节位置
         * @param  [in] name  轨迹文件名
         * @param  [in] period_ms  数据采样周期，固定值2ms或4ms或8ms
         * @param  [in] di_choose  DI选择,bit0~bit7对应控制箱DI0~DI7，bit8~bit9对应末端DI0~DI1，0-不选择，1-选择
         * @param  [in] do_choose  DO选择,bit0~bit7对应控制箱DO0~DO7，bit8~bit9对应末端DO0~DO1，0-不选择，1-选择
         * @return  错误码
         */
        public int SetTPDStart(int type, string name, int period_ms, UInt16 di_choose, UInt16 do_choose)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetTPDStart(type, name, period_ms, di_choose, do_choose);
                if (log != null)
                {
                    log.LogInfo($"SetTPDStart({type},{name},{period_ms},{di_choose},{do_choose}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  停止轨迹记录
         * @return  错误码
         */
        public int SetWebTPDStop()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetWebTPDStop();
                if (log != null)
                {
                    log.LogInfo($"SetWebTPDStop() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  删除轨迹记录
         * @param  [in] name  轨迹文件名
         * @return  错误码
         */
        public int SetTPDDelete(string name)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetTPDDelete(name);
                if (log != null)
                {
                    log.LogInfo($"SetTPDDelete({name}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  轨迹预加载
         * @param  [in] name  轨迹文件名
         * @return  错误码
         */
        public int LoadTPD(string name)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.LoadTPD(name);
                if (log != null)
                {
                    log.LogInfo($"LoadTPD({name}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  获取轨迹起始位姿
         * @param  [in] name 轨迹文件名,不需要文件后缀
         * @return  错误码
         */
        public int GetTPDStartPose(string name, ref DescPose desc_pose)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetTPDStartPose(name);
                if ((int)result[0] == 0)
                {
                    desc_pose.tran.x = (double)result[1];
                    desc_pose.tran.y = (double)result[2];
                    desc_pose.tran.z = (double)result[3];
                    desc_pose.rpy.rx = (double)result[4];
                    desc_pose.rpy.ry = (double)result[5];
                    desc_pose.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"GetTrajectoryStartPose(ref {desc_pose.tran.x},ref {desc_pose.tran.y},ref {desc_pose.tran.z},ref {desc_pose.rpy.rx},ref {desc_pose.rpy.ry},ref {desc_pose.rpy.rz}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  轨迹复现
         * @param  [in] name  轨迹文件名
         * @param  [in] blend 0-不平滑，1-平滑
         * @param  [in] ovl  速度缩放百分比，范围[0~100]
         * @return  错误码
         */
        public int MoveTPD(string name, byte blend, float ovl)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.MoveTPD(name, blend, ovl);
                if (log != null)
                {
                    log.LogInfo($"MoveTPD({name}, {blend},{ovl}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  轨迹预处理
         * @param  [in] name  轨迹文件名
         * @param  [in] ovl 速度缩放百分比，范围[0~100]
         * @param  [in] opt 1-控制点，默认为1
         * @return  错误码
         */
        public int LoadTrajectoryJ(string name, float ovl, int opt)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.LoadTrajectoryJ(name, ovl, opt);
                if (log != null)
                {
                    log.LogInfo($"LoadTrajectoryJ({name}, {ovl},{opt}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  轨迹复现
         * @return  错误码
         */
        public int MoveTrajectoryJ()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.MoveTrajectoryJ();
                if (log != null)
                {
                    log.LogInfo($"MoveTrajectoryJ() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取轨迹起始位姿
         * @param  [in] name 轨迹文件名
         * @return  错误码
         */
        public int GetTrajectoryStartPose(string name, ref DescPose desc_pose)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetTrajectoryStartPose(name);
                if ((int)result[0] == 0)
                {
                    desc_pose.tran.x = (double)result[1];
                    desc_pose.tran.y = (double)result[2];
                    desc_pose.tran.z = (double)result[3];
                    desc_pose.rpy.rx = (double)result[4];
                    desc_pose.rpy.ry = (double)result[5];
                    desc_pose.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"GetTrajectoryStartPose(ref {desc_pose.tran.x},ref {desc_pose.tran.y},ref {desc_pose.tran.z},ref {desc_pose.rpy.rx},ref {desc_pose.rpy.ry},ref {desc_pose.rpy.rz}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  获取轨迹点编号
         * @return  错误码
         */
        public int GetTrajectoryPointNum(ref int pnum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int errcode = 0;

                if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
                {
                    pnum = robot_state_pkg.trajectory_pnum;
                }
                else
                {
                    errcode = g_sock_com_err;
                }

                if (log != null)
                {
                    log.LogInfo($"GetTrajectoryPointNum(ref {pnum}) : {errcode}");
                }

                return errcode;
            }
            catch
            {

                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  设置轨迹运行中的速度
         * @param  [in] ovl 速度百分比
         * @return  错误码
         */
        public int SetTrajectoryJSpeed(double ovl)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetTrajectoryJSpeed(ovl);
                if (log != null)
                {
                    log.LogInfo($"SetTrajectoryJSpeed({ovl}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  设置轨迹运行中的力和扭矩
         * @param  [in] ft 三个方向的力和扭矩，单位N和Nm
         * @return  错误码
         */
        public int SetTrajectoryJForceTorque(ForceTorque ft)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] ftData = new double[6] { ft.fx, ft.fy, ft.fz, ft.tx, ft.ty, ft.tz };
                int rtn = proxy.SetTrajectoryJForceTorque(ftData);
                if (log != null)
                {
                    log.LogInfo($"SetTrajectoryJForceTorque({ftData[0]},{ftData[1]},{ftData[2]},{ftData[3]},{ftData[4]},{ftData[5]}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置轨迹运行中的沿x方向的力
         * @param  [in] fx 沿x方向的力，单位N
         * @return  错误码
         */
        public int SetTrajectoryJForceFx(double fx)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetTrajectoryJForceFx(fx);
                if (log != null)
                {
                    log.LogInfo($"SetTrajectoryJForceFx({fx}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  设置轨迹运行中的沿y方向的力
         * @param  [in] fy 沿y方向的力，单位N
         * @return  错误码
         */
        public int SetTrajectoryJForceFy(double fy)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetTrajectoryJForceFy(fy);
                if (log != null)
                {
                    log.LogInfo($"SetTrajectoryJForceFy({fy}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  设置轨迹运行中的沿z方向的力
         * @param  [in] fz 沿x方向的力，单位N
         * @return  错误码
         */
        public int SetTrajectoryJForceFz(double fz)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetTrajectoryJForceFz(fz);
                if (log != null)
                {
                    log.LogInfo($"SetTrajectoryJForceFz({fz}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  设置轨迹运行中的绕x轴的扭矩
         * @param  [in] tx 绕x轴的扭矩，单位Nm
         * @return  错误码
         */
        public int SetTrajectoryJTorqueTx(double tx)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetTrajectoryJTorqueTx(tx);
                if (log != null)
                {
                    log.LogInfo($"SetTrajectoryJTorqueTx({tx}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置轨迹运行中的绕x轴的扭矩
         * @param  [in] ty 绕y轴的扭矩，单位Nm
         * @return  错误码
         */
        public int SetTrajectoryJTorqueTy(double ty)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetTrajectoryJTorqueTy(ty);
                if (log != null)
                {
                    log.LogInfo($"SetTrajectoryJTorqueTy({ty}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  设置轨迹运行中的绕x轴的扭矩
         * @param  [in] tz 绕z轴的扭矩，单位Nm
         * @return  错误码
         */
        public int SetTrajectoryJTorqueTz(double tz)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetTrajectoryJTorqueTz(tz);
                if (log != null)
                {
                    log.LogInfo($"SetTrajectoryJTorqueTz({tz}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  设置开机自动加载默认的作业程序
         * @param  [in] flag  0-开机不自动加载默认程序，1-开机自动加载默认程序
         * @param  [in] program_name 作业程序名及路径，如"/fruser/movej.lua"，其中"/fruser/"为固定路径
         * @return  错误码
         */
        public int LoadDefaultProgConfig(byte flag, string program_name)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.LoadDefaultProgConfig(flag, program_name);
                if (log != null)
                {
                    log.LogInfo($"LoadDefaultProgConfig({flag},{program_name}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  加载指定的作业程序
         * @param  [in] program_name 作业程序名及路径，如"/fruser/movej.lua"，其中"/fruser/"为固定路径
         * @return  错误码
         */
        public int ProgramLoad(string program_name)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ProgramLoad(program_name);
                if (log != null)
                {
                    log.LogInfo($"ProgramLoad({program_name}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  获取已加载的作业程序名
         * @param  [out] program_name 作业程序名及路径，如"/fruser/movej.lua"，其中"/fruser/"为固定路径
         * @return  错误码
         */
        public int GetLoadedProgram(ref string program_name)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetLoadedProgram();
                if ((int)result[0] == 0)
                {
                    program_name = (string)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetLoadedProgram({program_name}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  获取当前机器人作业程序执行的行号
         * @param  [out] line  行号
         * @return  错误码
         */
        public int GetCurrentLine(ref int line)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetCurrentLine();
                if ((int)result[0] == 0)
                {
                    line = (int)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetCurrentLine({line}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }


        }

        /**
         * @brief  运行当前加载的作业程序
         * @return  错误码
         */
        public int ProgramRun()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.ProgramRun();
                if (log != null)
                {
                    log.LogInfo($"ProgramRun() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  暂停当前运行的作业程序
         * @return  错误码
         */
        public int ProgramPause()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.ProgramPause();
                if (log != null)
                {
                    log.LogInfo($"ProgramPause() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  恢复当前暂停的作业程序
         * @return  错误码
         */
        public int ProgramResume()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.ProgramResume();
                if (log != null)
                {
                    log.LogInfo($"ProgramResume() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  终止当前运行的作业程序
         * @return  错误码
         */
        public int ProgramStop()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ProgramStop();
                if (log != null)
                {
                    log.LogInfo($"ProgramStop() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  获取机器人作业程序执行状态
         * @param  [out]  state 1-程序停止或无程序运行，2-程序运行中，3-程序暂停
         * @return  错误码
         */
        public int GetProgramState(ref byte state)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                state = robot_state_pkg.robot_state;
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetProgramState(ref {state}) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief  配置夹爪
         * @param  [in] company  夹爪厂商，1-Robotiq，2-慧灵，3-天机，4-大寰，5-知行
         * @param  [in] device  设备号，Robotiq(0-2F-85系列)，慧灵(0-NK系列,1-Z-EFG-100)，天机(0-TEG-110)，大寰(0-PGI-140)，知行(0-CTPM2F20)
         * @param  [in] softvesion  软件版本号，暂不使用，默认为0
         * @param  [in] bus 设备挂在末端总线位置，暂不使用，默认为0
         * @return  错误码
         */
        public int SetGripperConfig(int company, int device, int softvesion, int bus)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetGripperConfig(company, device, softvesion, bus);
                if (log != null)
                {
                    log.LogInfo($"SetGripperConfig({company},{device},{softvesion},{bus}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         *@brief  获取夹爪配置
         *@param  [out] deviceID 夹爪编号
         *@param  [out] company  夹爪厂商，1-Robotiq，2-慧灵，3-天机，4-大寰，5-知行
         *@param  [out] device  设备号，Robotiq(0-2F-85系列)，慧灵(0-NK系列,1-Z-EFG-100)，天机(0-TEG-110)，大寰(0-PGI-140)，知行(0-CTPM2F20)
         *@param  [out] softvesion  软件版本号，暂不使用，默认为0
         *@return  错误码
         */
        public int GetGripperConfig(ref int deviceID, ref int company, ref int device, ref int softvesion)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetGripperConfig();
                if ((int)result[0] == 0)
                {
                    deviceID = (int)result[1] + 1;//目前不支持多夹爪，设置获取值为1
                    company = (int)result[2] + 1;
                    device = (int)result[3];
                    softvesion = (int)result[4];
                }
                if (log != null)
                {
                    log.LogInfo($"GetGripperConfig(ref {deviceID},ref {company},ref {device},ref {softvesion}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  激活夹爪
         * @param  [in] index  夹爪编号
         * @param  [in] act  0-复位，1-激活
         * @return  错误码
         */
        public int ActGripper(int index, byte act)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ActGripper(index, act);
                if (log != null)
                {
                    log.LogInfo($"ActGripper({index},{act}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  控制夹爪
         * @param  [in] index  夹爪编号
         * @param  [in] pos  位置百分比，范围[0~100]
         * @param  [in] vel  速度百分比，范围[0~100]
         * @param  [in] force  力矩百分比，范围[0~100]
         * @param  [in] max_time  最大等待时间，范围[0~30000]，单位ms
         * @param  [in] block  0-阻塞，1-非阻塞
         * @param  [in] type 夹爪类型，0-平行夹爪；1-旋转夹爪
         * @param  [in] rotNum 旋转圈数
         * @param  [in] rotVel 旋转速度百分比[0-100]
         * @param  [in] rotTorque 旋转力矩百分比[0-100]
         * @return  错误码
         */
        public int MoveGripper(int index, int pos, int vel, int force, int max_time, byte block, int type, double rotNum, int rotVel, int rotTorque)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.MoveGripper(index, pos, vel, force, max_time, block, type, rotNum, rotVel, rotTorque);
                if (log != null)
                {
                    log.LogInfo($"MoveGripper({index},{pos},{vel},{force},{max_time},{block}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  获取夹爪运动状态
         * @param  [out] fault  0-无错误，1-有错误
         * @param  [out] staus  0-运动未完成，1-运动完成
         * @return  错误码
         */
        public int GetGripperMotionDone(ref int fault, ref int status)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetGripperMotionDone();
                if ((int)result[0] == 0)
                {
                    fault = (int)result[1];
                    status = (int)result[2];
                }
                if (log != null)
                {
                    log.LogInfo($"GetGripperMotionDone(ref {fault},ref {status}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取夹爪激活状态
         * @param  [out] fault  0-无错误，1-有错误
         * @param  [out] status  bit0~bit15对应夹爪编号0~15，bit=0为未激活，bit=1为激活
         * @return  错误码
         */
        public int GetGripperActivateStatus(ref int fault, ref int status)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                fault = robot_state_pkg.gripper_fault;
                status = robot_state_pkg.gripper_active;
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetGripperActivateStatus(ref {fault},ref {status}) : {errcode}");
            }


            return errcode;
        }

        /**
         * @brief  获取夹爪位置
         * @param  [out] fault  0-无错误，1-有错误
         * @param  [out] position  位置百分比，范围0~100%
         * @return  错误码
         */
        public int GetGripperCurPosition(ref int fault, ref int position)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                fault = robot_state_pkg.gripper_fault;
                position = robot_state_pkg.gripper_position;
            }
            else
            {
                errcode = g_sock_com_err;
            }

            if (log != null)
            {
                log.LogInfo($"GetGripperCurPosition(ref {fault},ref {position}) : {errcode}");
            }

            return errcode;
        }

        /**
         * @brief  获取夹爪速度
         * @param  [out] fault  0-无错误，1-有错误
         * @param  [out] speed  速度百分比，范围0~100%
         * @return  错误码
         */
        public int GetGripperCurSpeed(ref int fault, ref int speed)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                fault = robot_state_pkg.gripper_fault;
                speed = robot_state_pkg.gripper_speed;
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetGripperCurSpeed(ref {fault},ref {speed}) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief  获取夹爪电流
         * @param  [out] fault  0-无错误，1-有错误
         * @param  [out] current  电流百分比，范围0~100%
         * @return  错误码
         */
        public int GetGripperCurCurrent(ref int fault, ref int current)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                fault = robot_state_pkg.gripper_fault;
                current = robot_state_pkg.gripper_current;
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetGripperCurCurrent(ref {fault},ref {current}) : {errcode}");
            }

            return errcode;
        }

        /**
         * @brief  获取夹爪电压
         * @param  [out] fault  0-无错误，1-有错误
         * @param  [out] voltage  电压,单位0.1V
         * @return  错误码
         */
        public int GetGripperVoltage(ref int fault, ref int voltage)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                fault = robot_state_pkg.gripper_fault;
                voltage = robot_state_pkg.gripper_voltage;
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetGripperVoltage(ref {fault},ref {voltage}) : {errcode}");
            }


            return errcode;
        }

        /**
         * @brief  获取夹爪温度
         * @param  [out] fault  0-无错误，1-有错误
         * @param  [out] temp  温度，单位℃
         * @return  错误码
         */
        public int GetGripperTemp(ref int fault, ref int temp)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                fault = robot_state_pkg.gripper_fault;
                temp = robot_state_pkg.gripper_tmp;
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetGripperTemp(ref {fault},ref {temp}) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief  计算预抓取点-视觉
         * @param  [in] desc_pos  抓取点笛卡尔位姿
         * @param  [in] zlength   z轴偏移量
         * @param  [in] zangle    绕z轴旋转偏移量
         * @param  [out] pre_pos  获取点
         * @return  错误码 
         */
        public int ComputePrePick(DescPose desc_pos, double zlength, double zangle, ref DescPose pre_pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                double[] descPos = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                object[] result = proxy.ComputePrePick(descPos, zlength, zangle);
                if ((int)result[0] == 0)
                {
                    pre_pos.tran.x = (double)result[1];
                    pre_pos.tran.y = (double)result[2];
                    pre_pos.tran.z = (double)result[3];
                    pre_pos.rpy.rx = (double)result[4];
                    pre_pos.rpy.ry = (double)result[5];
                    pre_pos.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"ComputePrePick({descPos[0]},{descPos[1]},{descPos[2]},{descPos[3]},{descPos[4]},{descPos[5]},{zlength},{zangle}, ref {pre_pos.tran.x},ref {pre_pos.tran.y},ref {pre_pos.tran.z},ref {pre_pos.rpy.rx},ref {pre_pos.rpy.ry},ref {pre_pos.rpy.rz},) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  计算撤退点-视觉
         * @param  [in] desc_pos  抓取点笛卡尔位姿
         * @param  [in] zlength   z轴偏移量
         * @param  [in] zangle    绕z轴旋转偏移量
         * @param  [out] post_pos 撤退点
         * @return  错误码 
         */
        public int ComputePostPick(DescPose desc_pos, double zlength, double zangle, ref DescPose post_pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                double[] descPos = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                object[] result = proxy.ComputePostPick(descPos, zlength, zangle);
                if ((int)result[0] == 0)
                {
                    post_pos.tran.x = (double)result[1];
                    post_pos.tran.y = (double)result[2];
                    post_pos.tran.z = (double)result[3];
                    post_pos.rpy.rx = (double)result[4];
                    post_pos.rpy.ry = (double)result[5];
                    post_pos.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"ComputePostPick({descPos[0]},{descPos[1]},{descPos[2]},{descPos[3]},{descPos[4]},{descPos[5]},{zlength},{zangle}, ref {post_pos.tran.x},ref {post_pos.tran.y},ref {post_pos.tran.z},ref {post_pos.rpy.rx},ref {post_pos.rpy.ry},ref {post_pos.rpy.rz}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief  配置力传感器
        * @param  [in] company  力传感器厂商，17-坤维科技，19-航天十一院，20-ATI传感器，21-中科米点，22-伟航敏芯，23-NBIT，24-鑫精诚(XJC)，26-NSR
        * @param  [in] device  设备号，坤维(0-KWR75B)，航天十一院(0-MCS6A-200-4)，ATI(0-AXIA80-M8)，中科米点(0-MST2010)，伟航敏芯(0-WHC6L-YB-10A)，NBIT(0-XLH93003ACS)，鑫精诚XJC(0-XJC-6F-D82)，NSR(0-NSR-FTSensorA)
        * @param  [in] softvesion  软件版本号，暂不使用，默认为0
        * @param  [in] bus 设备挂在末端总线位置，暂不使用，默认为0
        * @return  错误码
        */
        public int FT_SetConfig(int company, int device, int softvesion, int bus)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.FT_SetConfig(company, device, softvesion, bus);
                if (log != null)
                {
                    log.LogInfo($"FT_SetConfig({company},{device},{softvesion},{bus}) : {rtn}");
                }

                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  获取力传感器配置
         * @param  [out] deviceID 力传感器编号
         * @param  [out] company  力传感器厂商，待定
         * @param  [out] device  设备号，暂不使用，默认为0
         * @param  [out] softvesion  软件版本号，暂不使用，默认为0
         * 
         * @return  错误码
         */
        public int FT_GetConfig(ref int deviceID, ref int company, ref int device, ref int softvesion)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.FT_GetConfig();
                if ((int)result[0] == 0)
                {
                    deviceID = (int)result[1] + 1;//目前不支持多力传感器，正常设置的都是1
                    company = (int)result[2] + 1;
                    device = (int)result[3];
                    softvesion = (int)result[4];
                }
                if (log != null)
                {
                    log.LogInfo($"FT_GetConfig(ref {deviceID},ref {company},ref {device},ref {softvesion}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  力传感器激活
         * @param  [in] act  0-复位，1-激活
         * @return  错误码
         */
        public int FT_Activate(byte act)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.FT_Activate(act);
                if (log != null)
                {
                    log.LogInfo($"FT_Activate({act}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  力传感器校零
         * @param  [in] act  0-去除零点，1-零点矫正
         * @return  错误码
         */
        public int FT_SetZero(byte act)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.FT_SetZero(act);
                if (log != null)
                {
                    log.LogInfo($"FT_SetZero({act}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  设置力传感器参考坐标系
         * @param  [in]type  0-工具坐标系，1-基坐标系, 
         * @return  错误码
         */
        public int FT_SetRCS(byte type, DescPose coord)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] coords = new double[6] { coord.tran.x, coord.tran.y, coord.tran.z, coord.rpy.rx, coord.rpy.ry, coord.rpy.rz };
                int rtn = proxy.FT_SetRCS(type, coords);
                if (log != null)
                {
                    log.LogInfo($"FT_SetRCS({type},{coord.tran.x},{coord.tran.y},{coord.tran.z},{coord.rpy.rx},{coord.rpy.ry},{coord.rpy.rz}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /**
         * @brief  负载重量辨识记录
         * @param  [in] id  传感器坐标系编号，范围[1~14]
         * @return  错误码
         */
        public int FT_PdIdenRecord(int id)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.FT_PdIdenRecord(id);
                if (log != null)
                {
                    log.LogInfo($"FT_PdIdenRecord({id}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  负载重量辨识计算
         * @param  [out] weight  负载重量，单位kg
         * @return  错误码
         */
        public int FT_PdIdenCompute(ref double weight)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.FT_PdIdenCompute();
                if ((int)result[0] == 0)
                {
                    weight = (double)result[1];
                }

                if (log != null)
                {
                    log.LogInfo($"FT_PdIdenCompute(ref {weight}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  负载质心辨识记录
         * @param  [in] id  传感器坐标系编号，范围[1~14]
         * @param  [in] index 点编号，范围[1~3]
         * @return  错误码
         */
        public int FT_PdCogIdenRecord(int id, int index)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.FT_PdCogIdenRecord(id, index);
                if (log != null)
                {
                    log.LogInfo($"FT_PdCogIdenRecord({id},{index}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  负载质心辨识计算
         * @param  [out] cog  负载质心，单位mm
         * @return  错误码
         */
        public int FT_PdCogIdenCompute(ref DescTran cog)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.FT_PdCogIdenCompute();
                if ((int)result[0] == 0)
                {
                    cog.x = (double)result[1];
                    cog.y = (double)result[2];
                    cog.z = (double)result[3];
                }
                if (log != null)
                {
                    log.LogInfo($"FT_PdCogIdenCompute(ref {cog.x},ref {cog.y},ref {cog.z}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  获取参考坐标系下力/扭矩数据
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] ft  力/扭矩，fx,fy,fz,tx,ty,tz
         * @return  错误码
         */
        public int FT_GetForceTorqueRCS(byte flag, ref ForceTorque ft)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                ft.fx = robot_state_pkg.ft_sensor_data[0];
                ft.fy = robot_state_pkg.ft_sensor_data[1];
                ft.fz = robot_state_pkg.ft_sensor_data[2];
                ft.tx = robot_state_pkg.ft_sensor_data[3];
                ft.ty = robot_state_pkg.ft_sensor_data[4];
                ft.tz = robot_state_pkg.ft_sensor_data[5];
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"FT_GetForceTorqueRCS({flag},ref {ft.fx},ref {ft.fy},ref {ft.fz},ref {ft.tx},ref {ft.ty},ref {ft.tz}) : {errcode}");
            }

            return errcode;
        }

        /**
         * @brief  获取力传感器原始力/扭矩数据
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] ft  力/扭矩，fx,fy,fz,tx,ty,tz
         * @return  错误码
         */
        public int FT_GetForceTorqueOrigin(byte flag, ref ForceTorque ft)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                ft.fx = robot_state_pkg.ft_sensor_raw_data[0];
                ft.fy = robot_state_pkg.ft_sensor_raw_data[1];
                ft.fz = robot_state_pkg.ft_sensor_raw_data[2];
                ft.tx = robot_state_pkg.ft_sensor_raw_data[3];
                ft.ty = robot_state_pkg.ft_sensor_raw_data[4];
                ft.tz = robot_state_pkg.ft_sensor_raw_data[5];
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"FT_GetForceTorqueOrigin({flag},ref {ft.fx},ref {ft.fy},ref {ft.fz},ref {ft.tx},ref {ft.ty},ref {ft.tz}) : {errcode}");
            }

            return errcode;
        }

        /**
         * @brief  碰撞守护
         * @param  [in] flag 0-关闭碰撞守护，1-开启碰撞守护
         * @param  [in] sensor_id 力传感器编号
         * @param  [in] select  选择六个自由度是否检测碰撞，0-不检测，1-检测
         * @param  [in] ft  碰撞力/扭矩，fx,fy,fz,tx,ty,tz
         * @param  [in] max_threshold 最大阈值
         * @param  [in] min_threshold 最小阈值
         * @note   力/扭矩检测范围：(ft-min_threshold, ft+max_threshold)
         * @return  错误码
         */
        public int FT_Guard(int flag, int sensor_id, int[] select, ForceTorque ft, double[] max_threshold, double[] min_threshold)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] ftData = new double[6] { ft.fx, ft.fy, ft.fz, ft.tx, ft.ty, ft.tz };
                int rtn = proxy.FT_Guard(flag, sensor_id, select, ftData, max_threshold, min_threshold);
                if (log != null)
                {
                    log.LogInfo($"FT_Guard({flag},{sensor_id},{flag},{select[0]},{select[1]},{select[2]},{select[3]},{select[4]},{select[5]},{ft.fx},{ft.fy},{ft.fz},{ft.tx},{ft.ty},{ft.tz}," +
                        $"{max_threshold[0]},{max_threshold[1]},{max_threshold[2]},{max_threshold[3]},{max_threshold[4]},{max_threshold[5]},{min_threshold[0]},{min_threshold[1]},{min_threshold[2]},{min_threshold[3]},{min_threshold[4]},{min_threshold[5]}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  恒力控制
         * @param  [in] flag 0-关闭恒力控制，1-开启恒力控制
         * @param  [in] sensor_id 力传感器编号
         * @param  [in] select  选择六个自由度是否检测碰撞，0-不检测，1-检测
         * @param  [in] ft  碰撞力/扭矩，fx,fy,fz,tx,ty,tz
         * @param  [in] ft_pid 力pid参数，力矩pid参数
         * @param  [in] adj_sign 自适应启停控制，0-关闭，1-开启
         * @param  [in] ILC_sign ILC启停控制， 0-停止，1-训练，2-实操
         * @param  [in] max_dis 最大调整距离，单位mm
         * @param  [in] max_ang 最大调整角度，单位deg
         * @param  [in] filter_Sign 滤波开启标志 0-关；1-开，默认关闭
         * @param  [in] posAdapt_sign 姿态顺应开启标志 0-关；1-开，默认关闭
         * @param  [in] isNoBlock 阻塞标志，0-阻塞；1-非阻塞
         * @return  错误码
         */
        public int FT_Control(int flag, int sensor_id, int[] select, ForceTorque ft, double[] ft_pid, int adj_sign, int ILC_sign, double max_dis, double max_ang, int filter_Sign = 0, int posAdapt_sign = 0, int isNoBlock = 0)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] ftData = new double[6] { ft.fx, ft.fy, ft.fz, ft.tx, ft.ty, ft.tz };
                int rtn = proxy.FT_Control(flag, sensor_id, select, ftData, ft_pid, adj_sign, ILC_sign, max_dis, max_ang, filter_Sign, posAdapt_sign, isNoBlock);
                if (log != null)
                {
                    log.LogInfo($"FT_Control({flag},{sensor_id},{flag},{select[0]},{select[1]},{select[2]},{select[3]},{select[4]},{select[5]},{ft.fx},{ft.fy},{ft.fz},{ft.tx},{ft.ty},{ft.tz}," +
                        $"{ft_pid[0]},{ft_pid[1]},{ft_pid[2]},{ft_pid[3]},{ft_pid[4]},{ft_pid[5]},{adj_sign},{ILC_sign},{max_dis},{max_ang},{filter_Sign},{posAdapt_sign},{isNoBlock}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  柔顺控制开启
         * @param  [in] p 位置调节系数或柔顺系数
         * @param  [in] force 柔顺开启力阈值，单位N
         * @return  错误码
         */
        public int FT_ComplianceStart(float p, float force)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.FT_ComplianceStart(p, force);
                if (log != null)
                {
                    log.LogInfo($"FT_ComplianceStart({p},{force}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  柔顺控制关闭
         * @return  错误码
         */
        public int FT_ComplianceStop()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.FT_ComplianceStop();
                if (log != null)
                {
                    log.LogInfo($"FT_ComplianceStop() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 负载辨识初始化
         * @return 错误码
         */
        public int LoadIdentifyDynFilterInit()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.LoadIdentifyDynFilterInit();
                if (log != null)
                {
                    log.LogInfo($"LoadIdentifyDynFilterInit() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 负载辨识初始化
         * @return 错误码
         */
        public int LoadIdentifyDynVarInit()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.LoadIdentifyDynVarInit();
                if (log != null)
                {
                    log.LogInfo($"LoadIdentifyDynVarInit() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 负载辨识主程序
         * @param [in] joint_torque 关节扭矩
         * @param [in] joint_pos 关节位置
         * @param [in] t 采样周期
         * @return 错误码
         */
        public int LoadIdentifyMain(double[] joint_torque, double[] joint_pos, double t)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.LoadIdentifyMain(joint_torque, joint_pos, t);
                if (log != null)
                {
                    log.LogInfo($"LoadIdentifyMain({joint_torque[0]},{joint_torque[1]},{joint_torque[2]},{joint_torque[3]},{joint_torque[4]},{joint_torque[5]},{joint_pos[0]},{joint_pos[1]},{joint_pos[2]},{joint_pos[3]},{joint_pos[4]},{joint_pos[5]},{t}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /**
         * @brief 获取负载辨识结果
         * @param [in] gain  
         * @param [out] weight 负载重量
         * @param [out] cog 负载质心
         * @return 错误码
         */
        public int LoadIdentifyGetResult(double[] gain, ref double weight, ref DescTran cog)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.LoadIdentifyGetResult(gain);
                if ((int)result[0] == 0)
                {
                    weight = (double)result[1];
                    cog.x = (double)result[2];
                    cog.y = (double)result[3];
                    cog.z = (double)result[4];
                }
                if (log != null)
                {
                    log.LogInfo($"LoadIdentifyGetResult(ref {weight},ref {cog.x},ref {cog.y},ref {cog.z}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 传动带启动、停止
         * @param [in] status 状态，1-启动，0-停止 
         * @return 错误码
         */
        public int ConveyorStartEnd(byte status)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ConveyorStartEnd(status);
                if (log != null)
                {
                    log.LogInfo($"ConveyorStartEnd({status}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief 记录IO检测点
         * @return 错误码
         */
        public int ConveyorPointIORecord()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ConveyorPointIORecord();
                if (log != null)
                {
                    log.LogInfo($"ConveyorPointIORecord() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief 记录A点
         * @return 错误码
         */
        public int ConveyorPointARecord()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ConveyorPointARecord();
                if (log != null)
                {
                    log.LogInfo($"ConveyorPointARecord() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 记录参考点
         * @return 错误码
         */
        public int ConveyorRefPointRecord()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ConveyorRefPointRecord();
                if (log != null)
                {
                    log.LogInfo($"ConveyorRefPointRecord() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 记录B点
         * @return 错误码
         */
        public int ConveyorPointBRecord()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ConveyorPointBRecord();
                if (log != null)
                {
                    log.LogInfo($"ConveyorPointBRecord() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 传送带工件IO检测
         * @param [in] max_t 最大检测时间，单位ms
         * @return 错误码
         */
        public int ConveyorIODetect(int max_t)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ConveyorIODetect(max_t);
                if (log != null)
                {
                    log.LogInfo($"ConveyorIODetect({max_t}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 获取物体当前位置
         * @param [in] mode 1-跟踪抓取，2-跟踪运动，3-TPD跟踪
         * @return 错误码
         */
        public int ConveyorGetTrackData(int mode)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ConveyorGetTrackData(mode);
                if (log != null)
                {
                    log.LogInfo($"ConveyorGetTrackData({mode}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 传动带跟踪开始
         * @param [in] status 状态，1-启动，0-停止 
         * @return 错误码
         */
        public int ConveyorTrackStart(byte status)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ConveyorTrackStart(status);
                if (log != null)
                {
                    log.LogInfo($"ConveyorTrackStart({status}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 传动带跟踪停止
         * @return 错误码
         */
        public int ConveyorTrackEnd()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ConveyorTrackEnd();
                if (log != null)
                {
                    log.LogInfo($"ConveyorTrackEnd() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
        * @brief 传动带参数配置
        * @param [in] para[0] 编码器通道 1~2
        * @param [in] para[1] 编码器转一圈的脉冲数
        * @param [in] para[2] 编码器转一圈传送带行走距离
        * @param [in] para[3] 工件坐标系编号 针对跟踪运动功能选择工件坐标系编号，跟踪抓取、TPD跟踪设为0
        * @param [in] para[4] 是否配视觉  0 不配  1 配
        * @param [in] para[5] 速度比  针对传送带跟踪抓取选项（1-100）  其他选项默认为1 
        * @param [in] followType 跟踪运动类型，0-跟踪运动；1-追检运动
        * @param [in] startDis 追检抓取需要设置， 跟踪起始距离， -1：自动计算(工件到达机器人下方后自动追检)，单位mm， 默认值0
        * @param [in] endDis 追检抓取需要设置，跟踪终止距离， 单位mm， 默认值100
        * @return 错误码
        */

        public int ConveyorSetParam(int encChannel, int resolution, double lead, int wpAxis, int vision, double speedRadio, int followType=0, int startDis=0, int endDis=100)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                double[] param = new double[6] { encChannel, resolution, lead, wpAxis, vision, speedRadio };

                int rtn = proxy.ConveyorSetParam(param, followType, startDis, endDis);
                if (log != null)
                {
                    log.LogInfo($"ConveyorSetParam({encChannel},{resolution}),{lead},{wpAxis},{vision},{speedRadio},{followType},{startDis},{endDis} : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置传动带抓取点补偿
         * @param [in] cmp 补偿位置 double[3]{x, y, z}
         * @return 错误码
         */
        public int ConveyorCatchPointComp(double[] cmp)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ConveyorCatchPointComp(cmp);
                if (log != null)
                {
                    log.LogInfo($"ConveyorCatchPointComp({cmp} : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 直线运动
         * @param [in] status 状态，1-启动，0-停止 
         * * @param [in] name 运动点描述
         * @param [in] tool 工具坐标号，范围[0~14] 
         * @param [in] wobj 工件坐标号，范围[0~14] 
         * @param [in] vel 速度百分比，范围[0~100] 
         * @param [in] acc 加速度百分比，范围[0~100],暂不开放 
         * @param [in] ovl 速度缩放因子，范围[0~100] 
         * @param [in] blendR [-1.0]-运动到位(阻塞)，[0~1000.0]-平滑半径(非阻塞)，单位mm  
         * @param [in] flag 用于焊接模式下，是否寻位 
         * @param [in] type 用于焊接模式下，板材类型
         * @return 错误码
         */
        public int ConveyorTrackMoveL(string name, int tool, int wobj, float vel, float acc, float ovl, float blendR, int flag, int type)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ConveyorTrackMoveL(name, tool, wobj, vel, acc, ovl, blendR, 0, 0);
                if (log != null)
                {
                    log.LogInfo($"ConveyorTrackMoveL({name},{tool},{wobj},{vel},{acc},{ovl},{blendR} : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief 获取SSH公钥
         * @param [out] keygen 公钥
         * @return 错误码
         */
        public int GetSSHKeygen(ref string keygen)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetSSHKeygen();
                if ((int)result[0] == 0)
                {
                    keygen = (string)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetSSHKeygen(ref {keygen}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 计算指定路径下文件的MD5值
         * @param [in] file_path 文件路径包含文件名，默认Traj文件夹路径为:"/fruser/traj/",如"/fruser/traj/trajHelix_aima_1.txt"
         * @param [out] md5 文件MD5值
         * @return 错误码
         */
        public int ComputeFileMD5(string file_path, ref string md5)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.ComputeFileMD5(file_path);
                if ((int)result[0] == 0)
                {
                    md5 = (string)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"ComputeFileMD5({file_path}, ref {md5}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief 获取机器人急停状态
         * @param [out] state 急停状态，0-非急停，1-急停
         * @return 错误码  
         */
        public int GetRobotEmergencyStopState(ref byte state)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                state = robot_state_pkg.EmergencyStop;
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetRobotEmergencyStopState(ref {state}) : {errcode}");
            }
            return errcode;
        }

        /**
         * @brief 获取SDK与机器人的通讯状态
         * @param [out]  state 通讯状态，0-通讯正常，1-通讯异常
         */
        public int GetSDKComState(ref int state)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                state = 0;
            }
            else if (g_sock_com_err == (int)RobotError.ERR_SOCKET_COM_FAILED)
            {
                state = 1;
            }
            if (log != null)
            {
                log.LogInfo($"GetSDKComState(ref {state}) : {errcode}");
            }
            return errcode;
        }


        /**
         * @brief 获取安全停止信号
         * @param [out]  si0_state 安全停止信号SI0，0-无效，1-有效
         * @param [out]  si1_state 安全停止信号SI1，0-无效，1-有效
         */
        public int GetSafetyStopState(ref byte si0_state, ref byte si1_state)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                si0_state = robot_state_pkg.safety_stop0_state;
                si1_state = robot_state_pkg.safety_stop1_state;
            }
            else
            {
                errcode = g_sock_com_err;
            }
            if (log != null)
            {
                log.LogInfo($"GetSafetyStopState(ref {si0_state}, ref {si1_state}) : {errcode}");
            }
            return errcode;
        }

        /** 
        * @brief 获取机器人DH参数补偿值 
        * @param [out] dhCompensation 机器人DH参数补偿值(mm) [cmpstD1,cmpstA2,cmpstA3,cmpstD4,cmpstD5,cmpstD6]
        * @return 错误码 
        */
        public int GetDHCompensation(ref double[] dhCompensation)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetDHCompensation();
                if ((int)result[0] == 0)
                {
                    Console.WriteLine("wefaegag");
                    dhCompensation[0] = (double)result[1];
                    dhCompensation[1] = (double)result[2];
                    dhCompensation[2] = (double)result[3];
                    dhCompensation[3] = (double)result[4];
                    dhCompensation[4] = (double)result[5];
                    dhCompensation[5] = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"GetDHCompensation(ref {dhCompensation[0]},ref {dhCompensation[1]},ref {dhCompensation[2]},ref {dhCompensation[3]},ref {dhCompensation[4]},ref {dhCompensation[5]}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 下载点位表数据库 
         * @param [in] pointTableName 要下载的点位表名称    pointTable1.db
         * @param [in] saveFilePath 下载点位表的存储路径   C://test/
         * @return 错误码 
         */
        public int PointTableDownLoad(string pointTableName, string saveFilePath)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                //判断保存的文件路径是否存在
                if (!Directory.Exists(saveFilePath))
                {
                    return (int)RobotError.ERR_SAVE_FILE_PATH_NOT_FOUND;
                }
                int rtn = proxy.PointTableDownload(pointTableName);
                if (rtn == -1)
                {
                    return (int)RobotError.ERR_POINTTABLE_NOTFOUND;
                }
                else if (rtn != 0)
                {
                    return rtn;
                }

                IPAddress ipAddr = IPAddress.Parse(robot_ip);

                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 20011);

                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IAsyncResult connResult = client.BeginConnect(ipEndPoint, null, null);
                connResult.AsyncWaitHandle.WaitOne(2000, true);  //等待2秒

                client.ReceiveTimeout = 2000;
                client.SendTimeout = 2000;

                if (!connResult.IsCompleted)
                {
                    client.Close();
                    return (int)RobotError.ERR_OTHER;
                }
                byte[] totalbuffer = new byte[1024 * 1024 * 50];//50Mb
                int totalSize = 0;
                string recvMd5 = "";
                int recvSize = 0;
                bool findHeadFlag = false;

                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int num = client.Receive(buffer);
                    if (num < 1)
                    {
                        return (int)RobotError.ERR_OTHER;
                    }

                    buffer.CopyTo(totalbuffer, totalSize);
                    totalSize += num;

                    if (!findHeadFlag && totalSize > 4 && Encoding.UTF8.GetString(totalbuffer, 0, 4) == "/f/b")
                    {
                        findHeadFlag = true;
                    }

                    if (findHeadFlag && totalSize > 12 + 32)
                    {
                        recvSize = int.Parse(Encoding.UTF8.GetString(totalbuffer, 4, 8));
                        recvMd5 = Encoding.UTF8.GetString(totalbuffer, 12, 32);
                    }

                    if (findHeadFlag && totalSize == recvSize)
                    {
                        break;
                    }
                }
                if (totalSize == 0)
                {
                    return (int)RobotError.ERR_OTHER;
                }

                byte[] fileBuffer = new byte[1024 * 1024 * 50];//最大50M
                fileBuffer = subBytes(totalbuffer, 12 + 32, totalSize - 16 - 32);
                FileStream fsWriter = new FileStream(saveFilePath + pointTableName, FileMode.Create, FileAccess.Write, FileShare.None);

                fsWriter.Write(fileBuffer, 0, totalSize - 16 - 32);
                fsWriter.Flush();
                fsWriter.Close();

                string checkMd5 = getMD5ByMD5CryptoService(saveFilePath + pointTableName).ToLower();
                if (checkMd5 == recvMd5)
                {
                    client.Send(System.Text.Encoding.Default.GetBytes("SUCCESS"));
                    if (log != null)
                    {
                        log.LogInfo($"PointTableDownLoad({pointTableName}, {saveFilePath}) : {"success"}");
                    }
                    return 0;
                }
                else
                {
                    client.Send(System.Text.Encoding.Default.GetBytes("FAIL"));
                    System.IO.File.Delete(saveFilePath + pointTableName);
                    if (log != null)
                    {
                        log.LogInfo($"PointTableDownLoad({pointTableName}, {saveFilePath}) : {"FAIL"}");
                    }
                    return (int)RobotError.ERR_OTHER;
                }
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
        * @brief 上传点位表数据库 
        * @param [in] pointTableFilePath 上传点位表的全路径名   C://test/pointTable1.db
        * @return 错误码 
        */
        public int PointTableUpLoad(string pointTableFilePath)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                //判断上传文件是否存在
                FileInfo fileInfo = new FileInfo(pointTableFilePath);
                if (!fileInfo.Exists)
                {
                    return (int)RobotError.ERR_UPLOAD_FILE_NOT_FOUND;
                }

                int totalSize = GetFileSize(pointTableFilePath) + 16 + 32;
                if (totalSize > MAX_UPLOAD_FILE_SIZE)
                {
                    Console.WriteLine("Files larger than 2 MB are not supported!");
                    return -1;
                }

                string pointTableName = Path.GetFileName(pointTableFilePath);

                int rtn = proxy.PointTableUpload(pointTableName);
                if (rtn != 0)
                {
                    return rtn;
                }

                IPAddress ipAddr = IPAddress.Parse(robot_ip);
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 20010);

                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IAsyncResult connResult = client.BeginConnect(ipEndPoint, null, null);
                connResult.AsyncWaitHandle.WaitOne(2000, true);  //等待2秒

                if (!connResult.IsCompleted)
                {
                    client.Close();
                    return (int)RobotError.ERR_OTHER;
                }

                client.ReceiveTimeout = 2000;
                client.SendTimeout = 2000;

                string sendMd5 = getMD5ByMD5CryptoService(pointTableFilePath).ToLower();

                int num = client.Send(System.Text.Encoding.Default.GetBytes("/f/b" + totalSize.ToString("D8") + sendMd5));
                if (num < 1)
                {
                    return (int)RobotError.ERR_OTHER;
                }

                FileStream fs = new FileStream(pointTableFilePath, FileMode.Open, FileAccess.Read);
                int fileLength = (int)fs.Length;
                byte[] fileBytes = new byte[fileLength];
                int r = fs.Read(fileBytes, 0, fileLength);

                num = client.Send(fileBytes);
                if (num < 1)
                {
                    return (int)RobotError.ERR_OTHER;
                }

                num = client.Send(System.Text.Encoding.Default.GetBytes("/b/f"));
                if (num < 1)
                {
                    return (int)RobotError.ERR_OTHER;
                }

                byte[] resultBuf = new byte[1024];//最大50M
                num = client.Receive(resultBuf);
                if (num < 1)
                {
                    return (int)RobotError.ERR_OTHER;
                }
                if (Encoding.UTF8.GetString(resultBuf, 0, 7) == "SUCCESS")
                {
                    if (log != null)
                    {
                        log.LogInfo($"PointTableUpLoad({pointTableFilePath}) : {"success"}");
                    }
                    return (int)RobotError.ERR_SUCCESS;
                }
                else
                {
                    if (log != null)
                    {
                        log.LogInfo($"PointTableUpLoad({pointTableFilePath}) : {"fail"}");
                    }
                    return (int)RobotError.ERR_OTHER;
                }
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
        * @brief 切换点位表并应用
        * @param [in] pointTableName 要切换的点位表名称   "pointTable1.db"
        * @param [out] errorStr 切换点位表错误信息   
        * @return 错误码 
        */
        public int PointTableSwitch(string pointTableName, ref string errorStr)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.PointTableSwitch(pointTableName);//切换点位表
                if (rtn != 0)
                {
                    if (rtn == (int)RobotError.ERR_POINTTABLE_NOTFOUND)
                    {
                        errorStr = "PointTable not Found!";
                    }
                    else
                    {
                        errorStr = "not defined error";
                    }
                    if (log != null)
                    {
                        log.LogInfo($"PointTableSwitch({pointTableName}) : {errorStr}");
                    }

                }
                else
                {
                    Thread.Sleep(500);
                    errorStr = "success";
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /** 
        * @brief 点位表更新lua文件
        * @param [in] pointTableName 要切换的点位表名称   "pointTable1.db",当点位表为空，即""时，表示将lua程序更新为未应用点位表的初始程序
        * @param [in] luaFileName 要更新的lua文件名称   "testPointTable.lua"
        * @param [out] errorStr 切换点位表错误信息   
        * @return 错误码 
        */
        public int PointTableUpdateLua(string pointTableName, string luaFileName, ref string errorStr)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.PointTableSwitch(pointTableName);//切换点位表
                if (rtn != 0)
                {
                    if (rtn == (int)RobotError.ERR_POINTTABLE_NOTFOUND)
                    {
                        errorStr = "PointTable not Found!";
                    }
                    else
                    {
                        errorStr = "not defined error";
                    }
                    if (log != null)
                    {
                        log.LogInfo($"PointTableSwitch({pointTableName}) : {errorStr}");
                    }
                    return rtn;
                }

                Thread.Sleep(300);//切换点位表与更新lua程序之间在控制器里时异步的，为了保证切换后后端确实收到切换后的点位表名称，所以在更新前加点延时
                object[] result = proxy.PointTableUpdateLua(luaFileName);
                errorStr = (string)result[1];
                if (errorStr == "")
                {
                    errorStr = "fail to update lua,please inspect pointtable";
                }
                if (log != null)
                {
                    log.LogInfo($"PointTableUpdateLua({luaFileName}) : {errorStr}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
        * @brief 根据当前点位表更新当前lua程序 
        * @param [in] luaFileName 要更新的lua文件名 pointTable.lua
        * @param [out] errorStr 更新lua程序错误信息  成功：SUCCESS，其他：错误码
        * @return 错误码 
        */
        //public int PointTableUpdateLua(string luaFileName, ref string errorStr)
        //{
        //    if (IsSockComError())
        //    {
        //        return g_sock_com_err;
        //    }

        //    try
        //    {
        //        Thread.Sleep(300);//切换点位表与更新lua程序之间在控制器里时异步的，为了保证切换后后端确实收到切换后的点位表名称，所以在更新前加点延时
        //        object[] result = proxy.PointTableUpdateLua(luaFileName);
        //        errorStr = (string)result[1];
        //        if(errorStr == "")
        //        {
        //            errorStr = "fail to update lua,please inspect pointtable";
        //        }
        //        return (int)result[0];
        //    }
        //    catch
        //    {
        //        return (int)RobotError.ERR_RPC_ERROR;
        //    }
        //}




        private byte[] subBytes(byte[] src, int begin, int count)
        {
            byte[] bs = new byte[count];
            for (int i = begin; i < begin + count; i++)
            {
                bs[i - begin] = src[i];
            }
            return bs;
        }

        private string getMD5ByMD5CryptoService(string path)
        {
            if (!System.IO.File.Exists(path))
                throw new ArgumentException(string.Format("<{0}>, 不存在", path));
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
            byte[] buffer = md5Provider.ComputeHash(fs);
            string resule = BitConverter.ToString(buffer);
            resule = resule.Replace("-", "");
            md5Provider.Clear();
            fs.Close();
            return resule;
        }


        private int GetFileSize(string sFullName)
        {
            long lSize = 0;
            if (System.IO.File.Exists(sFullName))
                lSize = new FileInfo(sFullName).Length;
            return (int)lSize;
        }

        /** 
        * @brief 获取机器人软件版本 
        * @param [out] robotModel 机器人型号
        * @param [out] webVersion web版本
        * @param [out] controllerVersion 控制器版本
        * @return 错误码 
        */
        public int GetSoftwareVersion(ref string robotModel, ref string webVersion, ref string controllerVersion)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetSoftwareVersion();
                if ((int)result[0] == 0)
                {
                    robotModel = (string)result[1];
                    webVersion = (string)result[2];
                    controllerVersion = (string)result[3];
                }

                if (log != null)
                {
                    log.LogInfo($"GetSoftwareVersion(ref {robotModel},ref {webVersion},ref {controllerVersion}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 获取机器人硬件版本 
         * @param [out] ctrlBoxBoardVersion 控制箱载板硬件版本
         * @param [out] driver1Version 驱动器1硬件版本
         * @param [out] driver1Version 驱动器2硬件版本
         * @param [out] driver1Version 驱动器3硬件版本
         * @param [out] driver1Version 驱动器4硬件版本
         * @param [out] driver1Version 驱动器5硬件版本
         * @param [out] driver1Version 驱动器6硬件版本
         * @param [out] endBoardVersion 末端板硬件版本
         * @return 错误码 
         */
        public int GetHardwareVersion(ref string ctrlBoxBoardVersion, ref string driver1Version, ref string driver2Version, ref string driver3Version,
                    ref string driver4Version, ref string driver5Version, ref string driver6Version, ref string endBoardVersion)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetSlaveHardVersion();
                if ((int)result[0] == 0)
                {
                    ctrlBoxBoardVersion = (string)result[1];
                    driver1Version = (string)result[2];
                    driver2Version = (string)result[3];
                    driver3Version = (string)result[4];
                    driver4Version = (string)result[5];
                    driver5Version = (string)result[6];
                    driver6Version = (string)result[7];
                    endBoardVersion = (string)result[8];
                }
                if (log != null)
                {
                    log.LogInfo($"GetSlaveHardVersion(ref {ctrlBoxBoardVersion},ref {driver1Version},ref {driver2Version},ref {driver3Version},ref {driver4Version},ref {driver5Version},ref {driver6Version},ref {endBoardVersion}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 获取机器人固件版本 
         * @param [out] ctrlBoxBoardVersion 控制箱载板固件版本
         * @param [out] driver1Version 驱动器1固件版本
         * @param [out] driver1Version 驱动器2固件版本
         * @param [out] driver1Version 驱动器3固件版本
         * @param [out] driver1Version 驱动器4固件版本
         * @param [out] driver1Version 驱动器5固件版本
         * @param [out] driver1Version 驱动器6固件版本
         * @param [out] endBoardVersion 末端板固件版本
         * @return 错误码 
         */
        public int GetFirmwareVersion(ref string ctrlBoxBoardVersion, ref string driver1Version, ref string driver2Version, ref string driver3Version,
                    ref string driver4Version, ref string driver5Version, ref string driver6Version, ref string endBoardVersion)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetSlaveFirmVersion();
                if ((int)result[0] == 0)
                {
                    ctrlBoxBoardVersion = (string)result[1];
                    driver1Version = (string)result[2];
                    driver2Version = (string)result[3];
                    driver3Version = (string)result[4];
                    driver4Version = (string)result[5];
                    driver5Version = (string)result[6];
                    driver6Version = (string)result[7];
                    endBoardVersion = (string)result[8];
                }
                if (log != null)
                {
                    log.LogInfo($"GetSlaveFirmVersion(ref {ctrlBoxBoardVersion},ref {driver1Version},ref {driver2Version},ref {driver3Version},ref {driver4Version},ref {driver5Version},ref {driver6Version},ref {endBoardVersion}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 焊接开始 
         * @param [in] ioType io类型 0-控制器IO； 1-扩展IO
         * @param [in] arcNum 焊机配置文件编号
         * @param [in] timeout 起弧超时时间
         * @return 错误码 
         */
        public int ARCStart(int ioType, int arcNum, int timeout)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ARCStart(ioType, arcNum, timeout);
                if (log != null)
                {
                    log.LogInfo($"ARCStart({ioType},{arcNum},{timeout}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 焊接结束
         * @param [in] ioType io类型 0-控制器IO； 1-扩展IO
         * @param [in] arcNum 焊机配置文件编号
         * @param [in] timeout 熄弧超时时间
         * @return 错误码 
         */
        public int ARCEnd(int ioType, int arcNum, int timeout)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ARCEnd(ioType, arcNum, timeout);
                if (log != null)
                {
                    log.LogInfo($"ARCEnd({ioType},{arcNum},{timeout}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 设置焊接电流与输出模拟量对应关系
         * @param [in] currentMin 焊接电流-模拟量输出线性关系左侧点电流值(A)
         * @param [in] currentMax 焊接电流-模拟量输出线性关系右侧点电流值(A)
         * @param [in] outputVoltageMin 焊接电流-模拟量输出线性关系左侧点模拟量输出电压值(V)
         * @param [in] outputVoltageMax 焊接电流-模拟量输出线性关系右侧点模拟量输出电压值(V)
         * @param [in] AOIndex 焊接电流模拟量输出端口
         * @return 错误码 
         */
        public int WeldingSetCurrentRelation(double currentMin, double currentMax, double outputVoltageMin, double outputVoltageMax, int AOIndex = 0)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WeldingSetCurrentRelation(currentMin, currentMax, outputVoltageMin, outputVoltageMax, AOIndex);
                if (log != null)
                {
                    log.LogInfo($"WeldingSetCurrentRelation({currentMin},{currentMax},{outputVoltageMin},{outputVoltageMax}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 设置焊接电压与输出模拟量对应关系
         * @param [in] weldVoltageMin 焊接电压-模拟量输出线性关系左侧点焊接电压值(A)
         * @param [in] weldVoltageMax 焊接电压-模拟量输出线性关系右侧点焊接电压值(A)
         * @param [in] outputVoltageMin 焊接电压-模拟量输出线性关系左侧点模拟量输出电压值(V)
         * @param [in] outputVoltageMax 焊接电压-模拟量输出线性关系右侧点模拟量输出电压值(V)
         * @param [in] AOIndex 焊接电压模拟量输出端口
         * @return 错误码 
         */
        public int WeldingSetVoltageRelation(double weldVoltageMin, double weldVoltageMax, double outputVoltageMin, double outputVoltageMax, int AOIndex = 0)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WeldingSetVoltageRelation(weldVoltageMin, weldVoltageMax, outputVoltageMin, outputVoltageMax, AOIndex);
                if (log != null)
                {
                    log.LogInfo($"WeldingSetVoltageRelation({weldVoltageMin},{weldVoltageMax},{outputVoltageMin},{outputVoltageMax}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 获取焊接电流与输出模拟量对应关系
         * @param [out] currentMin 焊接电流-模拟量输出线性关系左侧点电流值(A)
         * @param [out] currentMax 焊接电流-模拟量输出线性关系右侧点电流值(A)
         * @param [out] outputVoltageMin 焊接电流-模拟量输出线性关系左侧点模拟量输出电压值(V)
         * @param [out] outputVoltageMax 焊接电流-模拟量输出线性关系右侧点模拟量输出电压值(V)
         * @param [out] AOIndex 焊接电流模拟量输出端口
         * @return 错误码 
         */
        public int WeldingGetCurrentRelation(ref double currentMin, ref double currentMax, ref double outputVoltageMin, ref double outputVoltageMax, ref int AOIndex)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.WeldingGetCurrentRelation();
                if ((int)result[0] == 0)
                {
                    currentMin = (double)result[1];
                    currentMax = (double)result[2];
                    outputVoltageMin = (double)result[3];
                    outputVoltageMax = (double)result[4];
                    AOIndex = (int)result[5];
                }
                if (log != null)
                {
                    log.LogInfo($"WeldingGetCurrentRelation(ref {currentMin},ref {currentMax},ref {outputVoltageMin},ref {outputVoltageMax},ref {AOIndex}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 获取焊接电压与输出模拟量对应关系
         * @param [out] weldVoltageMin 焊接电压-模拟量输出线性关系左侧点焊接电压值(A)
         * @param [out] weldVoltageMax 焊接电压-模拟量输出线性关系右侧点焊接电压值(A)
         * @param [out] outputVoltageMin 焊接电压-模拟量输出线性关系左侧点模拟量输出电压值(V)
         * @param [out] outputVoltageMax 焊接电压-模拟量输出线性关系右侧点模拟量输出电压值(V)
         * @param [out] AOIndex 焊接电压模拟量输出端口
         * @return 错误码 
         */
        public int WeldingGetVoltageRelation(ref double weldVoltageMin, ref double weldVoltageMax, ref double outputVoltageMin, ref double outputVoltageMax, ref int AOIndex)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.WeldingGetVoltageRelation();
                if ((int)result[0] == 0)
                {
                    weldVoltageMin = (double)result[1];
                    weldVoltageMax = (double)result[2];
                    outputVoltageMin = (double)result[3];
                    outputVoltageMax = (double)result[4];
                    AOIndex = (int)result[5];
                }
                if (log != null)
                {
                    log.LogInfo($"WeldingGetVoltageRelation(ref {weldVoltageMin},ref {weldVoltageMax},ref {outputVoltageMin},ref {outputVoltageMax},ref {AOIndex}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 设置焊接电流
         * @param [in] ioType 控制IO类型 0-控制箱IO；1-扩展IO
         * @param [in] current 焊接电流值(A)
         * @param [in] AOIndex 焊接电流控制箱模拟量输出端口(0-1)
         * @param [in] blend 是否平滑 0-不平滑；1-平滑
         * @return 错误码 
         */
        public int WeldingSetCurrent(int ioType, double current, int AOIndex, int blend = 0)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WeldingSetCurrent(ioType, current, AOIndex, blend = 0);
                if (log != null)
                {
                    log.LogInfo($"WeldingSetCurrent({ioType},{current},{AOIndex},{blend}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 设置焊接电压
         * @param [in] ioType 控制IO类型 0-控制箱IO；1-扩展IO
         * @param [in] voltage 焊接电压值(A)
         * @param [in] AOIndex 焊接电压控制箱模拟量输出端口(0-1)
         * @param [in] blend 是否平滑 0-不平滑；1-平滑
         * @return 错误码 
         */
        public int WeldingSetVoltage(int ioType, double voltage, int AOIndex, int blend = 0)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WeldingSetVoltage(ioType, voltage, AOIndex, blend);
                if (log != null)
                {
                    log.LogInfo($"WeldingSetVoltage({ioType},{voltage},{AOIndex},{blend}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
           * @brief 设置摆动参数
           * @param [in] weaveNum 摆焊参数配置编号
           * @param [in] weaveType 摆动类型 0-平面三角波摆动；1-垂直L型三角波摆动；2-顺时针圆形摆动；3-逆时针圆形摆动；4-平面正弦波摆动；5-垂直L型正弦波摆动；6-垂直三角波摆动；7-垂直正弦波摆动
           * @param [in] weaveFrequency 摆动频率(Hz)
           * @param [in] weaveIncStayTime 等待模式 0-周期不包含等待时间；1-周期包含等待时间
           * @param [in] weaveRange 摆动幅度(mm)
           * @param [in] weaveLeftRange 垂直三角摆动左弦长度(mm)
           * @param [in] weaveRightRange 垂直三角摆动右弦长度(mm)
           * @param [in] additionalStayTime 垂直三角摆动垂三角点停留时间(mm)
           * @param [in] weaveLeftStayTime 摆动左停留时间(ms)
           * @param [in] weaveRightStayTime 摆动右停留时间(ms)
           * @param [in] weaveCircleRadio 圆形摆动-回调比率(0-100%)
           * @param [in] weaveStationary 摆动位置等待，0-等待时间内位置继续移动；1-等待时间内位置静止
           * @param [in] weaveYawAngle 摆动方向方位角(绕摆动Z轴旋转)，单位°
           * @return 错误码 
           */
        public int WeaveSetPara(int weaveNum, int weaveType, double weaveFrequency, int weaveIncStayTime, double weaveRange, double weaveLeftRange, double weaveRightRange, int additionalStayTime, int weaveLeftStayTime, int weaveRightStayTime, int weaveCircleRadio, int weaveStationary, double weaveYawAngle, double weaveRotAngle=0)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WeaveSetPara(weaveNum, weaveType, weaveFrequency, weaveIncStayTime, weaveRange, weaveLeftRange, weaveRightRange, additionalStayTime, weaveLeftStayTime, weaveRightStayTime, weaveCircleRadio, weaveStationary, weaveYawAngle, weaveRotAngle);
                if (log != null)
                {
                    log.LogInfo($"WeaveSetPara({weaveNum},{weaveType},{weaveFrequency},{weaveIncStayTime},{weaveRange},{weaveLeftRange}, {weaveRightRange}, {additionalStayTime},{weaveLeftStayTime},{weaveRightStayTime},{weaveCircleRadio},{weaveStationary}, {weaveYawAngle},{weaveRotAngle})) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 即时设置摆动参数
         * @param [in] weaveNum 摆焊参数配置编号
         * @param [in] weaveType 摆动类型 0-平面三角波摆动；1-垂直L型三角波摆动；2-顺时针圆形摆动；3-逆时针圆形摆动；4-平面正弦波摆动；5-垂直L型正弦波摆动；6-垂直三角波摆动；7-垂直正弦波摆动
         * @param [in] weaveFrequency 摆动频率(Hz)
         * @param [in] weaveIncStayTime 等待模式 0-周期不包含等待时间；1-周期包含等待时间
         * @param [in] weaveRange 摆动幅度(mm)
         * @param [in] weaveLeftStayTime 摆动左停留时间(ms)
         * @param [in] weaveRightStayTime 摆动右停留时间(ms)
         * @param [in] weaveCircleRadio 圆形摆动-回调比率(0-100%)
         * @param [in] weaveStationary 摆动位置等待，0-等待时间内位置继续移动；1-等待时间内位置静止
         * @return 错误码 
         */
        public int WeaveOnlineSetPara(int weaveNum, int weaveType, double weaveFrequency, int weaveIncStayTime, double weaveRange, int weaveLeftStayTime, int weaveRightStayTime, int weaveCircleRadio, int weaveStationary)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WeaveOnlineSetPara(weaveNum, weaveType, weaveFrequency, weaveIncStayTime, weaveRange, weaveLeftStayTime, weaveRightStayTime, weaveCircleRadio, weaveStationary);
                if (log != null)
                {
                    log.LogInfo($"WeaveOnlineSetPara({weaveNum},{weaveType},{weaveFrequency},{weaveIncStayTime},{weaveRange},{weaveLeftStayTime},{weaveRightStayTime},{weaveCircleRadio},{weaveStationary}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 摆动开始
         * @param [in] weaveNum 摆焊参数配置编号
         * @return 错误码 
         */
        public int WeaveStart(int weaveNum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WeaveStart(weaveNum);
                if (log != null)
                {
                    log.LogInfo($"WeaveStart({weaveNum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 摆动结束
         * @param [in] weaveNum 摆焊参数配置编号
         * @return 错误码 
         */
        public int WeaveEnd(int weaveNum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.WeaveEnd(weaveNum);
                if (log != null)
                {
                    log.LogInfo($"WeaveEnd({weaveNum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 正向送丝
         * @param [in] ioType io类型  0-控制器IO；1-扩展IO
         * @param [in] wireFeed 送丝控制  0-停止送丝；1-送丝
         * @return 错误码 
         */
        public int SetForwardWireFeed(int ioType, int wireFeed)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetForwardWireFeed(ioType, wireFeed);
                if (log != null)
                {
                    log.LogInfo($"SetForwardWireFeed({ioType},{wireFeed}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 反向送丝
         * @param [in] ioType io类型  0-控制器IO；1-扩展IO
         * @param [in] wireFeed 送丝控制  0-停止送丝；1-送丝
         * @return 错误码 
         */
        public int SetReverseWireFeed(int ioType, int wireFeed)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetReverseWireFeed(ioType, wireFeed);
                if (log != null)
                {
                    log.LogInfo($"SetReverseWireFeed({ioType},{wireFeed}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 送气
         * @param [in] ioType io类型  0-控制器IO；1-扩展IO
         * @param [in] airControl 送气控制  0-停止送气；1-送气
         * @return 错误码 
         */
        public int SetAspirated(int ioType, int airControl)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetAspirated(ioType, airControl);
                if (log != null)
                {
                    log.LogInfo($"SetAspirated({ioType},{airControl}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
        * @brief 段焊开始
        * @param [in] startDesePos 起始点笛卡尔位置
        * @param [in] endDesePos 结束点笛卡尔位姿
        * @param [in] startJPos 起始点关节位姿
        * @param [in] endJPos 结束点关节位姿
        * @param [in] weldLength 焊接段长度(mm)
        * @param [in] noWeldLength 非焊接段长度(mm)
        * @param [in] weldIOType 焊接IO类型(0-控制箱IO；1-扩展IO)
        * @param [in] arcNum 焊机配置文件编号
        * @param [in] weldTimeout 起/收弧超时时间
        * @param [in] isWeave 是否摆动
        * @param [in] weaveNum 摆焊参数配置编号
        * @param [in] tool 工具号
        * @param [in] user 工件号
        * @param [in] vel  速度百分比，范围[0~100]
        * @param [in] acc  加速度百分比，范围[0~100],暂不开放
        * @param [in] ovl  速度缩放因子，范围[0~100]
        * @param [in] blendR [-1.0]-运动到位(阻塞)，[0~1000.0]-平滑半径(非阻塞)，单位mm	 
        * @param [in] epos  扩展轴位置，单位mm
        * @param [in] search  0-不焊丝寻位，1-焊丝寻位
        * @param [in] offset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
        * @param [in] offset_pos  位姿偏移量
        * @return 错误码 
        */
        public int SegmentWeldStart(DescPose startDesePos, DescPose endDesePos, JointPos startJPos, JointPos endJPos, double weldLength, double noWeldLength, int weldIOType,
            int arcNum, int weldTimeout, bool isWeave, int weaveNum, int tool, int user, float vel, float acc, float ovl, float blendR, ExaxisPos epos, byte search, byte offset_flag, DescPose offset_pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = 0;
                //获取起点到终点之间的距离和各方向角度余弦值
                object[] result = proxy.GetSegWeldDisDir(startDesePos.tran.x, startDesePos.tran.y, startDesePos.tran.z, endDesePos.tran.x, endDesePos.tran.y, endDesePos.tran.z);
                if ((int)result[0] != 0)
                {
                    return (int)result[0];
                }

                double distance = (double)result[1];
                double directionX = (double)result[2];
                double directionY = (double)result[3];
                double directionZ = (double)result[4];

                rtn = MoveJ(startJPos, startDesePos, tool, user, vel, acc, ovl, epos, -1, offset_flag, offset_pos);
                if (rtn != 0)
                {
                    return rtn;
                }
                int weldNum = 0;
                int noWeldNum = 0;
                int i = 0;
                while (i < (int)(distance / (weldLength + noWeldLength)) * 2 + 2)
                {
                    Console.WriteLine("efeses");
                    if (i % 2 == 0)
                    {
                        weldNum += 1;
                        if (weldNum * weldLength + noWeldNum * noWeldLength > distance)
                        {
                           
                            DescPose endOffPos = new DescPose(0, 0, 0, 0, 0, 0);
                            DescPose tmpWeldDesc = new DescPose(0, 0, 0, 0, 0, 0);
                            JointPos tmpJoint = new JointPos(0, 0, 0, 0, 0, 0);
                            int tmpTool = 0;
                            int tmpUser = 0;
                            //rtn = GetSegmentWeldPoint(startDesePos, endDesePos, distance, ref tmpWeldDesc, ref tmpJoint, ref tmpTool, ref tmpUser);
                            if (rtn != 0) //起弧前要先计算一下焊接点，
                            {
                                return rtn;
                            }
                            rtn = ARCStart(weldIOType, arcNum, weldTimeout);
                            if (rtn != 0)
                            {
                                return rtn;
                            }
                            if (isWeave)
                            {
                                rtn = WeaveStart(weaveNum);
                                if (rtn != 0)
                                {
                                    return rtn;
                                }
                            }
                            rtn = MoveL(endJPos, endDesePos, tmpTool, tmpUser, vel, acc, ovl, blendR, epos, search, 0, endOffPos);
                            if (rtn != 0)
                            {
                                ARCEnd(weldIOType, arcNum, weldTimeout);
                                if (isWeave)
                                {
                                    rtn = WeaveEnd(weaveNum);
                                    if (rtn != 0)
                                    {
                                        return rtn;
                                    }
                                }
                                return rtn;
                            }
                            rtn = ARCEnd(weldIOType, arcNum, weldTimeout);
                            if (rtn != 0)
                            {
                                return rtn;
                            }
                            if (isWeave)
                            {
                                rtn = WeaveEnd(weaveNum);
                                if (rtn != 0)
                                {
                                    return rtn;
                                }
                            }
                            break;
                        }
                        else
                        {
                            DescPose endOffPos = new DescPose(0, 0, 0, 0, 0, 0);
                            DescPose tmpWeldDesc = new DescPose(0, 0, 0, 0, 0, 0);
                            JointPos tmpJoint = new JointPos(0, 0, 0, 0, 0, 0);
                            int tmpTool = 0;
                            int tmpUser = 0;
                            rtn = GetSegmentWeldPoint(startDesePos, endDesePos, weldNum * weldLength + noWeldNum * noWeldLength, ref tmpWeldDesc, ref tmpJoint, ref tmpTool, ref tmpUser);
                            if (rtn != 0)
                            {
                                return rtn;
                            }
                            rtn = ARCStart(weldIOType, arcNum, weldTimeout);
                            if (rtn != 0)
                            {
                                return rtn;
                            }
                            if (isWeave)
                            {
                                rtn = WeaveStart(weaveNum);
                                if (rtn != 0)
                                {
                                    return rtn;
                                }
                            }
                            rtn = MoveL(tmpJoint, tmpWeldDesc, tmpTool, tmpUser, vel, acc, ovl, blendR, epos, search, 0, endOffPos);
                            if (rtn != 0)
                            {
                                ARCEnd(weldIOType, arcNum, weldTimeout);
                                if (isWeave)
                                {
                                    rtn = WeaveEnd(weaveNum);
                                    if (rtn != 0)
                                    {
                                        return rtn;
                                    }
                                }
                                return rtn;
                            }
                            rtn = ARCEnd(weldIOType, arcNum, weldTimeout);
                            if (rtn != 0)
                            {
                                return rtn;
                            }
                            if (isWeave)
                            {
                                rtn = WeaveEnd(weaveNum);
                                if (rtn != 0)
                                {
                                    return rtn;
                                }
                            }
                        }
                    }
                    else
                    {
                        noWeldNum += 1;
                        if (weldNum * weldLength + noWeldNum * noWeldLength > distance)
                        {
                            DescPose endOffPos = new DescPose(0, 0, 0, 0, 0, 0);
                            DescPose tmpWeldDesc = new DescPose(0, 0, 0, 0, 0, 0);
                            JointPos tmpJoint = new JointPos(0, 0, 0, 0, 0, 0);
                            int tmpTool = 0;
                            int tmpUser = 0;
                            rtn = GetSegmentWeldPoint(startDesePos, endDesePos, distance, ref tmpWeldDesc, ref tmpJoint, ref tmpTool, ref tmpUser);
                            rtn = MoveL(tmpJoint, tmpWeldDesc, tmpTool, tmpUser, vel, acc, ovl, blendR, epos, search, 0, endOffPos);
                            if (rtn != 0)
                            {
                                return rtn;
                            }
                            break;
                        }
                        else
                        {
                            DescPose endOffPos = new DescPose(0, 0, 0, 0, 0, 0);
                            DescPose tmpWeldDesc = new DescPose(0, 0, 0, 0, 0, 0);
                            JointPos tmpJoint = new JointPos(0, 0, 0, 0, 0, 0);
                            int tmpTool = 0;
                            int tmpUser = 0;
                            rtn = GetSegmentWeldPoint(startDesePos, endDesePos, weldNum * weldLength + noWeldNum * noWeldLength, ref tmpWeldDesc, ref tmpJoint, ref tmpTool, ref tmpUser);
                            if (rtn != 0)
                            {
                                return rtn;
                            }
                            rtn = MoveL(tmpJoint, tmpWeldDesc, tmpTool, tmpUser, vel, acc, ovl, blendR, epos, search, 0, endOffPos);
                            if (rtn != 0)
                            {
                                return rtn;
                            }
                        }
                    }
                    i++;
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /** 
         * @brief CI功能配置
         * @param [in] CIConfig CI配置 CIConfig[0] - CIConfig[7]表示CI0-CI7的功能配置 0-无配置；1-起弧成功；2-焊机准备；3-传送带检测；4-暂停；5-恢复；6-启动
         * 7-停止；8-暂停/恢复；9-启动/停止；10-脚踏拖动；11-移至作业原点；12-手自动切换；13-焊丝寻位成功；14-运动中断；15-启动主程序；16-启动倒带；17-启动确认；
         * 18-激光检测信号X；19-激光检测信号Y；20-外部急停输入信号1；21-外部急停输入信号2；22-一级缩减模式；23-二级缩减模式；24-三级缩减模式(停止)；25-恢复焊接；26-终止焊接
         * @return 错误码 
         */
        public int SetDIConfig(int[] CIConfig)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            string configStr = $"SetDIConfig({CIConfig[0]},{CIConfig[1]},{CIConfig[2]},{CIConfig[3]},{CIConfig[4]},{CIConfig[5]},{CIConfig[6]},{CIConfig[7]})";
            int configLength = configStr.Length;

            while (is_sendcmd == true) //说明当前正在处理上一条指令
            {
                Thread.Sleep(10);
            }

            g_sendbuf = $"/f/bIII{ResumeMotionCnt}III323III{configLength}III{configStr}III/b/f";
            if (log != null)
            {
                log.LogInfo($"SetDIConfig({configStr}) : {g_sock_com_err}");
            }
            ResumeMotionCnt++;
            is_sendcmd = true;
            return 0;
        }

        /** 
         * @brief CI输入有效电平配置
         * @param [in] CIConfig CI配置 CIConfig[0] - CIConfig[7]表示CI0-CI7的功能配置 0-高电平有效；1-低电平有效
         * @return 错误码 
         */
        public int SetDIConfigLevel(int[] CILevelConfig)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            string configStr = $"SetDIConfigLevel({CILevelConfig[0]},{CILevelConfig[1]},{CILevelConfig[2]},{CILevelConfig[3]},{CILevelConfig[4]},{CILevelConfig[5]},{CILevelConfig[6]},{CILevelConfig[7]})";
            int configLength = configStr.Length;

            while (is_sendcmd == true) //说明当前正在处理上一条指令
            {
                Thread.Sleep(10);
            }

            g_sendbuf = $"/f/bIII{ResumeMotionCnt}III335III{configLength}III{configStr}III/b/f";
            if (log != null)
            {
                log.LogInfo($"SetDIConfig({configStr}) : {g_sock_com_err}");
            }
            ResumeMotionCnt++;
            is_sendcmd = true;
            return 0;
        }

        private int SegmentWeldEnd(int ioType, int arcNum, int timeout)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = ARCEnd(ioType, arcNum, timeout);
                if (rtn != 0)
                {
                    return rtn;
                }
                rtn = StopMotion();
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
	     * @brief 初始化日志参数
	     * @param [in] logType：输出模式，DIRECT-直接输出；BUFFER-缓冲输出；ASYNC-异步输出
	     * @param [in] logLevel：日志过滤等级，ERROR-错误；WARNING-警告;INFO-信息；DEBUG-调试
	     * @param [in] filePath: 文件保存路径，如“D://Log/”
	     * @param [in] saveFileNum：保存文件个数，同时超出保存文件个数和保存文件天数的文件将被删除
	     * @param [in] saveDays: 保存文件天数，同时超出保存文件个数和保存文件天数的文件将被删除
	     * @return 错误码
	     */
        public int LoggerInit(FrLogType logType = FrLogType.DIRECT, FrLogLevel logLevel = FrLogLevel.INFO, string filePath = "", int saveFileNum = 10, int saveDays = 10)
        {
            if (log != null)
            {
                log.LogInfo($"log has already inited");
                return 0;
            }
            if (!Directory.Exists(filePath))
            {
                return -6;
            }

            if (saveDays < 1)
            {
                return 4;
            }

            log = new Log(logType, logLevel, filePath, saveFileNum, saveDays);
            if (log != null)
            {
                log.LogInfo($"LoggerInit({logType},{logLevel},{filePath},{saveFileNum},{saveDays}) : {0}");
            }
            return 0;
        }

        /**
         * @brief 设置日志过滤等级;
         * @param [in] logLevel: 日志过滤等级，ERROR-错误；WARNING-警告;INFO-信息；DEBUG-调试
         * @return 错误码
         */
        public int SetLoggerLevel(FrLogLevel logLevel)
        {
            if (log != null)
            {
                log.SetLogLevel(logLevel);
            }
            if (log != null)
            {
                log.LogInfo($"SetLogLevel({logLevel}) : {0}");
            }
            return 0;
        }


        private bool IsSockComError()
        {

            while (sock_cli_state.GetReconnState() && sock_cli_cmd.GetReconnState())
            {
                //如果正在重连，就等待重连结果
                Thread.Sleep(100);
            }
            if (g_sock_com_err != (int)RobotError.ERR_SUCCESS)
            {
                if (log != null)
                {
                    log.LogError($"sdk socket error {g_sock_com_err}");
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /** 
         * @brief 下载文件 
         * @param [in] fileType 文件类型    0-lua文件
         * @param [in] fileName 文件名称    “test.lua”
         * @param [in] saveFilePath 保存文件路径    “C：//test/”
         * @return 错误码 
         */
        private int FileDownLoad(int fileType, string fileName, string saveFilePath)
        {
            // 1. 检查Socket连接状态
            if (IsSockComError())
            {
                if (log != null) log.LogError("Socket通信异常");
                return g_sock_com_err;
            }
            try
            {
                // 2. 参数有效性检查
                if (string.IsNullOrEmpty(fileName))
                {
                    if (log != null) log.LogError("文件名不能为空");
                    return (int)RobotError.ERR_FILE_NAME;
                }

                if (string.IsNullOrEmpty(saveFilePath))
                {
                    if (log != null) log.LogError("保存路径不能为空");
                    return (int)RobotError.ERR_SAVE_FILE_PATH_NOT_FOUND;
                }

                // 3. 检查保存路径是否存在（跨平台实现）
                try
                {
                    if (!Directory.Exists(saveFilePath))
                    {
                        if (log != null) log.LogError($"路径不存在: {saveFilePath}");
                        return (int)RobotError.ERR_SAVE_FILE_PATH_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    if (log != null) log.LogError($"路径检查异常: {ex.Message}");
                    return (int)RobotError.ERR_SAVE_FILE_PATH_NOT_FOUND;
                }

                if (log != null) log.LogInfo($"路径验证通过: {saveFilePath}");

                // 4. 发起RPC调用

                int rtn = proxy.FileDownload(fileType, fileName);
                if (rtn != 0)
                {
                    if (rtn == -1)
                    {
                        if (log != null) log.LogError("文件不存在");
                        return (int)RobotError.ERR_POINTTABLE_NOTFOUND;
                    }
                    else
                    {
                        if (log != null) log.LogError($"文件下载请求失败，错误码: {rtn}");
                        return rtn;
                    }
                }

                // 5. 建立Socket连接
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.ReceiveTimeout = 5000; // 15秒超时
                client.SendTimeout = 5000;

                try
                {
                    // 使用20011作为端口号
                    IAsyncResult connectResult = client.BeginConnect(robot_ip, DOWNLOAD_POINT_TABLE_PORT, null, null);
                    if (!connectResult.AsyncWaitHandle.WaitOne(5000)) // 5秒连接超时
                    {
                        client.Close();
                        if (log != null) log.LogError("连接服务器超时");
                        return (int)RobotError.ERR_SOCKET_COM_FAILED;
                    }
                    client.EndConnect(connectResult);
                }
                catch (Exception ex)
                {
                    client.Close();
                    if (log != null) log.LogError($"连接服务器失败: {ex.Message}");
                    return (int)RobotError.ERR_RPC_ERROR;
                }

                // 6. 接收文件数据
                byte[] totalBuffer = new byte[1024 * 1024 * 50]; // 50MB缓冲区
                int totalBytes = 0;
                string expectedMd5 = "";
                int expectedSize = 0;
                bool headerFound = false;
                bool sizeMd5Found = false;
                bool downloadSuccess = false;
                DateTime startTime = DateTime.Now;

                try
                {
                    while (true)
                    {
                        // 超时检查
                        if ((DateTime.Now - startTime).TotalMilliseconds > 15000) // 15秒超时
                        {
                            if (log != null) log.LogError("下载超时");
                            break;
                        }

                        byte[] buffer = new byte[8192]; // 8KB缓冲区
                        int received = client.Receive(buffer);
                        if (received < 1)
                        {
                            if (log != null) log.LogError("接收数据异常");
                            break;
                        }

                        // 追加到总缓冲区
                        Buffer.BlockCopy(buffer, 0, totalBuffer, totalBytes, received);
                        totalBytes += received;

                        // 检查文件头 (4字节)
                        if (!headerFound && totalBytes >= 4)
                        {
                            string header = Encoding.ASCII.GetString(totalBuffer, 0, 4);
                            if (header != "/f/b")
                            {
                                if (log != null) log.LogError($"无效文件头: {header}");
                                break;
                            }
                            headerFound = true;
                            if (log != null) log.LogInfo("找到文件头");
                        }

                        // 提取长度和MD5 (4字节头 + 10字节长度 + 32字节MD5 = 46字节)
                        if (headerFound && !sizeMd5Found && totalBytes >= 46)
                        {
                            string sizeStr = Encoding.ASCII.GetString(totalBuffer, 4, 10);
                            if (!int.TryParse(sizeStr, out expectedSize))
                            {
                                if (log != null) log.LogError($"无效长度格式: {sizeStr}");
                                break;
                            }

                            expectedMd5 = Encoding.ASCII.GetString(totalBuffer, 14, 32);
                            sizeMd5Found = true;

                            if (log != null)
                                log.LogInfo($"预期大小: {expectedSize}, MD5: {expectedMd5}");
                        }

                        // 检查是否接收完成
                        if (headerFound && sizeMd5Found && totalBytes == expectedSize)
                        {
                            downloadSuccess = true;
                            break;
                        }
                    }

                    if (!downloadSuccess)
                    {
                        if (log != null) log.LogError("文件下载未完成");
                        return (int)RobotError.ERR_DOWN_LOAD_FILE_FAILED;
                    }

                    // 7. 验证文件尾 (最后4字节)
                    //string footer = Encoding.ASCII.GetString(totalBuffer, totalBytes - 4, 4);
                    //if (footer != "/b/f")
                    //{
                    //    if (log != null) log.LogError($"无效文件尾: {footer}");
                    //}

                    // 8. 提取文件内容 (跳过46字节头，去掉4字节尾)
                    int contentLength = totalBytes - 46 - 4;
                    byte[] fileContent = new byte[contentLength];
                    Buffer.BlockCopy(totalBuffer, 46, fileContent, 0, contentLength);

                    // 9. 计算并校验MD5
                    string computedMd5;
                    using (MD5 md5 = MD5.Create())
                    {
                        byte[] hash = md5.ComputeHash(fileContent);
                        computedMd5 = BitConverter.ToString(hash).Replace("-", "").ToLower();
                    }

                    if (computedMd5 != expectedMd5.ToLower())
                    {
                        if (log != null)
                            log.LogError($"MD5校验失败，预期: {expectedMd5}, 实际: {computedMd5}");
                        return (int)RobotError.ERR_DOWN_LOAD_FILE_CHECK_FAILED;
                    }

                    // 10. 保存文件
                    string fullPath = Path.Combine(saveFilePath, fileName);
                    try
                    {
                        System.IO.File.WriteAllBytes(fullPath, fileContent);
                        if (log != null) log.LogInfo($"文件保存成功: {fullPath}");
                    }
                    catch (Exception ex)
                    {
                        if (log != null) log.LogError($"文件保存失败: {ex.Message}");
                        return (int)RobotError.ERR_DOWN_LOAD_FILE_WRITE_FAILED;
                    }

                    // 11. 发送成功响应
                    client.Send(Encoding.ASCII.GetBytes("SUCCESS"));
                    return 0;
                }
                finally
                {
                    client.Close();
                }
            }
            catch (SocketException sockEx)
            {
                if (log != null) log.LogError($"Socket异常: {sockEx.Message}");
                return g_sock_com_err;
            }
            catch (Exception ex)
            {
                if (log != null) log.LogError($"系统异常: {ex.Message}");
                return g_sock_com_err;
            }
        }

        /** 
        * @brief 上传文件 
        * @param [in] fileType 文件类型    0-lua文件
        * @param [in] filePath 文件路径    “D://test.lua”
        * @return 错误码 
        */
        private int FileUpLoad(int fileType, string filePath)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                //判断上传文件是否存在
                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                {
                    if (log != null)
                    {
                        log.LogInfo("file not existed!");
                    }
                    return (int)RobotError.ERR_UPLOAD_FILE_NOT_FOUND;
                }

                int totalSize = GetFileSize(filePath) + 4 + 46;
                if (totalSize > MAX_UPLOAD_FILE_SIZE)
                {
                    if (log != null)
                    {
                        log.LogInfo("Files larger than 2 MB are not supported!");
                    }
                    return -1;
                }
                if (log != null)
                {
                    log.LogInfo($"all upload file size is {totalSize}");
                }
                string fileName = fileInfo.Name;
                int rtn = proxy.FileUpload(fileType, fileName);
                if (rtn != 0)
                {
                    return rtn;
                }

                IPAddress ipAddr = IPAddress.Parse(robot_ip);
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 20010);

                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Thread.Sleep(40);
                IAsyncResult connResult = client.BeginConnect(ipEndPoint, null, null);
                connResult.AsyncWaitHandle.WaitOne(2000, true);  //等待2秒

                if (!connResult.IsCompleted)
                {
                    client.Close();
                    return (int)RobotError.ERR_OTHER;
                }
                if (log != null)
                {
                    log.LogDebug("Upload file connected!");
                }

                client.ReceiveTimeout = 10000;
                client.SendTimeout = 2000;

                string sendMd5 = getMD5ByMD5CryptoService(filePath).ToLower();
                if (log != null)
                {
                    log.LogDebug($"send Md5 is {sendMd5}!");
                }

                int num = client.Send(System.Text.Encoding.Default.GetBytes("/f/b" + totalSize.ToString("D10") + sendMd5));
                if (num < 1)
                {
                    if (log != null)
                    {
                        log.LogDebug("send head failed!");
                    }
                    return (int)RobotError.ERR_OTHER;
                }

                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                long leftLength = fs.Length;//还没有读取的文件内容长度
                byte[] buffer = new byte[2 * 1024 * 1024]; //创建接收文件内容的字节数组
                int maxLength = buffer.Length;//每次读取的最大字节数
                int readLength = 0;//每次实际返回的字节数长度      
                int fileStart = 0;//文件开始读取的位置
                while (leftLength > 0)
                {
                    fileUploadPercent = (totalSize - leftLength) * 1.0 / totalSize * 100;
                    Console.WriteLine($"robot file upload percent {fileUploadPercent}");
                    fs.Position = fileStart;//设置文件流的读取位置
                    if (leftLength < maxLength)
                    {
                        readLength = fs.Read(buffer, 0, Convert.ToInt32(leftLength));
                    }
                    else
                    {
                        readLength = fs.Read(buffer, 0, maxLength);
                    }
                    if (readLength == 0)
                    {
                        break;
                    }
                    fileStart += readLength;
                    leftLength -= readLength;
                    if (log != null)
                    {
                        log.LogDebug($"left length {leftLength}!");
                    }
                    num = client.Send(buffer, readLength, SocketFlags.None);
                    if (log != null)
                    {
                        log.LogDebug($"send {num}!");
                    }
                    if (num < 1)
                    {
                        if (log != null)
                        {
                            log.LogError("send file failed!");
                        }
                        return (int)RobotError.ERR_OTHER;
                    }
                }

                num = client.Send(System.Text.Encoding.Default.GetBytes("/b/f"));
                if (num < 1)
                {
                    if (log != null)
                    {
                        log.LogDebug("send end failed!");
                    }
                    return (int)RobotError.ERR_OTHER;
                }

                if (log != null)
                {
                    log.LogDebug($"send file end success!");
                }

                byte[] resultBuf = new byte[1024];//最大50M
                num = client.Receive(resultBuf);
                if (num < 1)
                {
                    if (log != null)
                    {
                        log.LogDebug("get result failed!");
                    }
                    return (int)RobotError.ERR_OTHER;
                }
                if (log != null)
                {
                    log.LogDebug($"recv success!");
                }
                if (Encoding.UTF8.GetString(resultBuf, 0, 7) == "SUCCESS")
                {
                    if (log != null)
                    {
                        log.LogInfo($"fileUpLoad({filePath}) : {"success"}");
                    }
                    fileUploadPercent = 100;
                    return (int)RobotError.ERR_SUCCESS;
                }
                else
                {
                    if (log != null)
                    {
                        log.LogError("upload get result fail!");
                    }
                    if (log != null)
                    {
                        log.LogInfo($"fileUpLoad({filePath}) : {"fail"}");
                    }
                    return (int)RobotError.ERR_OTHER;
                }
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
        * @brief 删除文件 
        * @param [in] fileType 文件类型    0-lua文件
        * @param [in] fileName 文件名称    “test.lua”
        * @return 错误码 
        */
        private int FileDelete(int fileType, string fileName)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.FileDelete(fileType, fileName);
                if (log != null)
                {
                    log.LogInfo($"FileDelete({fileType},{fileName}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
        * @brief 上传Lua文件
        * @param [in] filePath 本地lua文件路径名 ".../test.lua"或".../test.tar.gz"
        * @param [out] errStr 错误信息
        * @return 错误码 
        */
        public int LuaUpload(string filePath, ref string errStr)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return (int)RobotError.ERR_UPLOAD_FILE_NOT_FOUND;
            }

            int rtn = FileUpLoad(0, filePath);
            if (rtn == 0)
            {
                object[] result = proxy.LuaUpLoadUpdate(fileInfo.Name);

                errStr = (string)result[1];
                if ((int)result[0] != 0)
                {
                    if (log != null)
                    {
                        log.LogError($"LuaUpLoadUpdate({errStr})");
                    }
                }

                if (log != null)
                {
                    log.LogInfo($"LuaUpLoadUpdate({filePath}, ref {errStr}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            else
            {
                errStr = "Lua Upload Fail";
                if (log != null)
                {
                    log.LogError($"LuaUpLoadUpdate({filePath}, ref {errStr}) : {rtn}");
                }
                return rtn;
            }
        }

        /** 
        * @brief 下载Lua文件
        * @param [in] fileName 要下载的lua文件名"test.lua"或"test.tar.gz"
        * @param [in] savePath 保存文件本地路径“D://Down/”
        * @return 错误码 
        */
        public int LuaDownLoad(string fileName, string savePath)
        {
            int rtn = 0;
            string[] nameStrs = fileName.Split('.');
            if (nameStrs.Length > 2 && nameStrs[1] == "tar" && nameStrs[2] == "gz")
            {

                rtn = proxy.LuaDownLoadPrepare(fileName);
                if (log != null)
                {
                    log.LogDebug($"LuaDownLoad({fileName} rtn {rtn})");
                }
                if (rtn != 0)
                {
                    return rtn;
                }
            }
            rtn = FileDownLoad(0, fileName, savePath);
            if (rtn == -1)
            {
                return (int)RobotError.ERR_LUAFILENITFOUND;
            }
            else
            {
                return rtn;
            }

        }

        /** 
        * @brief 删除Lua文件
        * @param [in] fileName 要删除的lua文件名"test.lua"
        * @return 错误码 
        */
        public int LuaDelete(string fileName)
        {
            int rtn = FileDelete(0, fileName);
            return rtn;
        }

        /** 
        * @brief 获取当前所有lua文件名称
        * @param [out] luaNames lua文件名列表
        * @return 错误码 
        */
        public int GetLuaList(ref List<string> luaNames)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int luaNum = 0;
                string luaNameStr = "";
                object[] result = proxy.GetLuaList();
                if ((int)result[0] == 0)
                {
                    luaNum = (int)result[1];
                    luaNameStr = (string)result[2];
                    string[] names = luaNameStr.Split(';');
                    for (int i = 0; i < luaNum; i++)
                    {
                        luaNames.Add(names[i]);
                    }

                }
                if (log != null)
                {
                    log.LogInfo($"GetLuaList(ref {luaNameStr}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /** 
        * @brief 设置485扩展轴参数
        * @param [in] servoId 伺服驱动器ID，范围[1-16],对应从站ID 
        * @param [in] servoCompany 伺服驱动器厂商，1-戴纳泰克
        * @param [in] servoModel 伺服驱动器型号，1-FD100-750C
        * @param [in] servoSoftVersion 伺服驱动器软件版本，1-V1.0
        * @param [in] servoResolution 编码器分辨率
        * @param [in] axisMechTransRatio 机械传动比
        * @return 错误码 
        */
        public int AuxServoSetParam(int servoId, int servoCompany, int servoModel, int servoSoftVersion, int servoResolution, double axisMechTransRatio)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.AuxServoSetParam(servoId, servoCompany, servoModel, servoSoftVersion, servoResolution, axisMechTransRatio);
                if (log != null)
                {
                    log.LogInfo($"AuxServoSetParam({servoId},{servoCompany},{servoModel},{servoSoftVersion},{servoResolution},{axisMechTransRatio}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 获取485扩展轴配置参数
         * @param [in] servoId 伺服驱动器ID，范围[1-16],对应从站ID 
         * @param [out] servoCompany 伺服驱动器厂商，1-戴纳泰克
         * @param [out] servoModel 伺服驱动器型号，1-FD100-750C
         * @param [out] servoSoftVersion 伺服驱动器软件版本，1-V1.0
         * @param [out] servoResolution 编码器分辨率
         * @param [out] axisMechTransRatio 机械传动比
         * @return 错误码 
         */
        public int AuxServoGetParam(int servoId, ref int servoCompany, ref int servoModel, ref int servoSoftVersion, ref int servoResolution, ref double axisMechTransRatio)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.AuxServoGetParam(servoId);
                if ((int)result[0] == 0)
                {
                    servoCompany = (int)result[1];
                    servoModel = (int)result[2];
                    servoSoftVersion = (int)result[3];
                    servoResolution = (int)result[4];
                    axisMechTransRatio = (double)result[5];
                }
                if (log != null)
                {
                    log.LogInfo($"AuxServoGetParam(ref {servoId},ref {servoCompany},ref {servoModel},ref {servoSoftVersion},ref {servoResolution},ref {axisMechTransRatio}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 设置485扩展轴使能/去使能
         * @param [in] servoId 伺服驱动器ID，范围[1-16],对应从站ID 
         * @param [in] status 使能状态，0-去使能， 1-使能
         * @return 错误码 
         */
        public int AuxServoEnable(int servoId, int status)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.AuxServoEnable(servoId, status);
                if (log != null)
                {
                    log.LogInfo($"AuxServoEnable({servoId},{status}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 设置485扩展轴控制模式
         * @param [in] servoId 伺服驱动器ID，范围[1-16],对应从站ID 
         * @param [in] mode 控制模式，0-位置模式，1-速度模式
         * @return 错误码 
         */
        public int AuxServoSetControlMode(int servoId, int mode)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.AuxServoSetControlMode(servoId, mode);
                if (log != null)
                {
                    log.LogInfo($"AuxServoSetControlMode({servoId},{mode}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
        * @brief 设置485扩展轴目标位置(位置模式)
        * @param [in] servoId 伺服驱动器ID，范围[1-16],对应从站ID 
        * @param [in] pos 目标位置，mm或°
        * @param [in] speed 目标速度，mm/s或°/s
        * @param [in] acc 加速度百分比[0-100]
        * @return 错误码 
        */
        public int AuxServoSetTargetPos(int servoId, double pos, double speed, double acc = 100)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.AuxServoSetTargetPos(servoId, pos, speed, acc);
                if (log != null)
                {
                    log.LogInfo($"AuxServoSetTargetPos({servoId},{pos},{speed},{acc}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
        * @brief 设置485扩展轴目标速度(速度模式)
        * @param [in] servoId 伺服驱动器ID，范围[1-16],对应从站ID 
        * @param [in] speed 目标速度，mm/s或°/s
        * @param [in] acc 加速度百分比[0-100]
        * @return 错误码 
        */
        public int AuxServoSetTargetSpeed(int servoId, double speed, double acc = 100)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.AuxServoSetTargetSpeed(servoId, speed, acc);
                if (log != null)
                {
                    log.LogInfo($"AuxServoSetTargetSpeed({servoId},{speed}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
        * @brief 设置485扩展轴目标转矩(力矩模式)
        * @param [in] servoId 伺服驱动器ID，范围[1-16],对应从站ID 
        * @param [in] torque 目标力矩，Nm
        * @return 错误码 
        */
        public int AuxServoSetTargetTorque(int servoId, double torque)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.AuxServoSetTargetTorque(servoId, torque);
                if (log != null)
                {
                    log.LogInfo($"AuxServoSetTargetTorque({servoId},{torque}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
        * @brief 设置485扩展轴回零
        * @param [in] servoId 伺服驱动器ID，范围[1-16],对应从站ID 
        * @param [in] mode 回零模式，1-当前位置回零；2-负限位回零；3-正限位回零
        * @param [in] searchVel 回零速度，mm/s或°/s
        * @param [in] latchVel 箍位速度，mm/s或°/s
        * @param [in] acc 加速度百分比[0-100]
        * @return 错误码 
        */
        public int AuxServoHoming(int servoId, int mode, double searchVel, double latchVel, double acc = 100)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.AuxServoHoming(servoId, mode, searchVel, latchVel, acc);
                if (log != null)
                {
                    log.LogInfo($"AuxServoHoming({servoId},{mode},{searchVel},{latchVel},{acc}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
        * @brief 清除485扩展轴错误信息
        * @param [in] servoId 伺服驱动器ID，范围[1-16],对应从站ID 
        * @return 错误码 
        */
        public int AuxServoClearError(int servoId)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.AuxServoClearError(servoId);
                if (log != null)
                {
                    log.LogInfo($"AuxServoClearError({servoId}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 获取485扩展轴伺服状态
         * @param [in] servoId 伺服驱动器ID，范围[1-16],对应从站ID 
         * @param [out] servoErrCode 伺服驱动器故障码
         * @param [out] servoState 伺服驱动器状态 bit0:0-未使能；1-使能;  bit1:0-未运动；1-正在运动;  bit4 0-未定位完成；1-定位完成；  bit5：0-未回零；1-回零完成
         * @param [out] servoPos 伺服当前位置 mm或°
         * @param [out] servoSpeed 伺服当前速度 mm/s或°/s
         * @param [out] servoTorque 伺服当前转矩Nm
         * @return 错误码 
         */
        public int AuxServoGetStatus(int servoId, ref int servoErrCode, ref int servoState, ref double servoPos, ref double servoSpeed, ref double servoTorque)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.AuxServoGetStatus(servoId);
                if ((int)result[0] == 0)
                {
                    servoErrCode = (int)result[1];
                    servoState = (int)result[2];
                    servoPos = (double)result[3];
                    servoSpeed = (double)result[4];
                    servoTorque = (double)result[5];
                }
                if (log != null)
                {
                    log.LogInfo($"AuxServoGetStatus({servoId},ref {servoErrCode},ref {servoState},ref {servoPos},ref {servoSpeed},ref {servoTorque}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 设置状态反馈中485扩展轴数据轴号
         * @param [in] servoId 伺服驱动器ID，范围[1-16],对应从站ID 
         * @return 错误码 
         */
        public int AuxServosetStatusID(int servoId)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.AuxServosetStatusID(servoId);
                if (log != null)
                {
                    log.LogInfo($"AuxServosetStatusID({servoId}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 获取机器人实时状态结构体
         * @param [out] pkg 机器人实时状态结构体 
         * @return 错误码 
         */
        public int GetRobotRealTimeState(ref ROBOT_STATE_PKG pkg)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                pkg = robot_state_pkg;
                return 0;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 获取机器人外设协议
         * @param [out] protocol 机器人外设协议号 4096-扩展轴控制卡；4097-ModbusSlave；4098-ModbusMaster
         * @return 错误码 
         */
        public int GetExDevProtocol(ref int protocol)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetExDevProtocol();
                if ((int)result[0] == 0)
                {
                    protocol = (int)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetExDevProtocol(ref {protocol}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
         * @brief 获取机器人外设协议
         * @param [in] protocol 机器人外设协议号 4096-扩展轴控制卡；4097-ModbusSlave；4098-ModbusMaster
         * @return 错误码 
         */
        public int SetExDevProtocol(int protocol)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetExDevProtocol(protocol);
                if (log != null)
                {
                    log.LogInfo($"SetExDevProtocol({protocol}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置机器人加速度
         * @param [in] acc 机器人加速度百分比
         * @return 错误码
         */
        public int SetOaccScale(double acc)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetOaccScale(acc);
                if (log != null)
                {
                    log.LogInfo($"SetOaccScale({acc}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
	     * @brief 控制箱AO飞拍开始
	     * @param [in] AONum 控制箱AO编号
	     * @param [in] maxTCPSpeed 最大TCP速度值[1-5000mm/s]，默认1000
	     * @param [in] maxAOPercent 最大TCP速度值对应的AO百分比，默认100%
	     * @param [in] zeroZoneCmp 死区补偿值AO百分比，整形，默认为20%，范围[0-100]
	     * @return 错误码
	     */

        public int MoveAOStart(int AONum, int maxTCPSpeed, int maxAOPercent, int zeroZoneCmp)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.MoveAOStart(AONum, maxTCPSpeed, maxAOPercent, zeroZoneCmp);
                if (log != null)
                {
                    log.LogInfo($"MoveAOStart({AONum}, {maxTCPSpeed}, {maxAOPercent}, {zeroZoneCmp}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 控制箱AO飞拍停止
         * @return 错误码
         */
        public int MoveAOStop()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.MoveAOStop();
                if (log != null)
                {
                    log.LogInfo($"MoveAOStop() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 末端AO飞拍开始
         * @param [in] AONum 末端AO编号
         * @param [in] maxTCPSpeed 最大TCP速度值[1-5000mm/s]，默认1000
         * @param [in] maxAOPercent 最大TCP速度值对应的AO百分比，默认100%
         * @param [in] zeroZoneCmp 死区补偿值AO百分比，整形，默认为20%，范围[0-100]
         * @return 错误码
         */
        public int MoveToolAOStart(int AONum, int maxTCPSpeed, int maxAOPercent, int zeroZoneCmp)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.MoveToolAOStart(AONum, maxTCPSpeed, maxAOPercent, zeroZoneCmp);
                if (log != null)
                {
                    log.LogInfo($"MoveToolAOStart({AONum}, {maxTCPSpeed}, {maxAOPercent}, {zeroZoneCmp}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 末端AO飞拍停止
         * @return 错误码
         */
        public int MoveToolAOStop()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.MoveToolAOStop();
                if (log != null)
                {
                    log.LogInfo($"MoveToolAOStop() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /**
        * @brief 可移动装置停止运动
        * @return 错误码
        */
        public int TractorStop()
        {
            int rtn = ProgramStop();
            return rtn;
        }

        /**
         * @brief UDP扩展轴通讯参数配置
         * @param [in] ip PLC IP地址
         * @param [in] port	端口号
         * @param [in] period	通讯周期(ms，默认为2，请勿修改此参数)
         * @param [in] lossPkgTime	丢包检测时间(ms)
         * @param [in] lossPkgNum	丢包次数
         * @param [in] disconnectTime	通讯断开确认时长
         * @param [in] reconnectEnable	通讯断开自动重连使能 0-不使能 1-使能
         * @param [in] reconnectPeriod	重连周期间隔(ms)
         * @param [in] reconnectNum	重连次数
         * @param [in] selfConnect 断电重启是否自动建立连接；0-不建立连接；1-建立连接
         * @return 错误码
         */
        public int ExtDevSetUDPComParam(string ip, int port, int period, int lossPkgTime, int lossPkgNum, int disconnectTime, int reconnectEnable, int reconnectPeriod, int reconnectNum, int selfConnect)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ExtDevSetUDPComParam(ip, port, period, lossPkgTime, lossPkgNum, disconnectTime, reconnectEnable, reconnectPeriod, reconnectNum, selfConnect);
                if (log != null)
                {
                    log.LogInfo($"ExtDevSetUDPComParam({ip}, {port}, {period}, {lossPkgTime}, {lossPkgNum}, {disconnectTime}, {reconnectEnable}, {reconnectPeriod}, {reconnectNum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 获取UDP扩展轴通讯参数
         * @param [out] ip PLC IP地址
         * @param [out] port	端口号
         * @param [out] period	通讯周期(ms，默认为2，请勿修改此参数)
         * @param [out] lossPkgTime	丢包检测时间(ms)
         * @param [out] lossPkgNum	丢包次数
         * @param [out] disconnectTime	通讯断开确认时长
         * @param [out] reconnectEnable	通讯断开自动重连使能 0-不使能 1-使能
         * @param [out] reconnectPeriod	重连周期间隔(ms)
         * @param [out] reconnectNum	重连次数
         * @return 错误码
         */
        public int ExtDevGetUDPComParam(ref string ip, ref int port, ref int period, ref int lossPkgTime, ref int lossPkgNum, ref int disconnectTime, ref int reconnectEnable, ref int reconnectPeriod, ref int reconnectNum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.ExtDevGetUDPComParam();
                if ((int)result[0] == 0)
                {
                    ip = (string)result[1];
                    port = (int)result[2];
                    period = (int)result[3];
                    lossPkgTime = (int)result[4];
                    lossPkgNum = (int)result[5];
                    disconnectTime = (int)result[6];
                    reconnectEnable = (int)result[7];
                    reconnectPeriod = (int)result[8];
                    reconnectNum = (int)result[9];

                }
                if (log != null)
                {
                    log.LogInfo($"ExtDevGetUDPComParam(ref {ip}, ref {port}, ref {period}, ref {lossPkgTime}, ref {lossPkgNum}, ref {disconnectTime}, ref {reconnectEnable}, ref {reconnectPeriod}, ref {reconnectNum}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 加载UDP通信
         * @return 错误码
         */
        public int ExtDevLoadUDPDriver()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ExtDevLoadUDPDriver();
                if (log != null)
                {
                    log.LogInfo($"ExtDevLoadUDPDriver() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 卸载UDP通信
         * @return 错误码
         */
        public int ExtDevUnloadUDPDriver()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ExtDevUnloadUDPDriver();
                if (log != null)
                {
                    log.LogInfo($"ExtDevUnloadUDPDriver() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief UDP扩展轴回零
         * @param [in] axisID 轴号[1-4]
         * @param [in] mode 回零方式 0-当前位置回零，1-负限位回零，2-正限位回零
         * @param [in] searchVel 寻零速度(mm/s)
         * @param [in] latchVel 寻零箍位速度(mm/s)
         * @return 错误码
         */
        public int ExtAxisSetHoming(int axisID, int mode, double searchVel, double latchVel)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ExtAxisSetHoming(axisID, mode, searchVel, latchVel);
                if (log != null)
                {
                    log.LogInfo($"ExtAxisSetHoming({axisID}, {mode}, {searchVel}, {latchVel}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (log != null)
                {
                    log.LogError($"RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
         * @brief UDP扩展轴点动开始
         * @param [in] axisID 轴号[1-4]
         * @param [in] direction 转动方向 0-反向；1-正向
         * @param [in] vel 速度(mm/s)
         * @param [in] acc (加速度 mm/s2)
         * @param [in] maxDistance 最大点动距离
         * @return 错误码
         */
        public int ExtAxisStartJog(int axisID, int direction, double vel, double acc, double maxDistance)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.ExtAxisStartJog(6, axisID, direction, vel, acc, maxDistance);
                if (log != null)
                {
                    log.LogInfo($"ExtAxisStartJog({axisID}, {direction}, {vel}, {acc}, {maxDistance}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief UDP扩展轴点动停止
         * @param [in] axisID 轴号[1-4]
         * @return 错误码
         */
        public int ExtAxisStopJog(int axisID)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            while (is_sendcmd == true) //说明当前正在处理上一条指令
            {
                Thread.Sleep(10);
            }

            g_sendbuf = $"/f/bIII19III240III14IIIStopExtAxisJogIII/b/f";
            is_sendcmd = true;
            if (log != null)
            {
                log.LogInfo($"StopExtAxisJog() : {g_sock_com_err}");
            }
            return 0;
        }

        /**
         * @brief UDP扩展轴使能
         * @param [in] axisID 轴号[1-4]
         * @param [in] status 0-去使能；1-使能
         * @return 错误码
         */
        public int ExtAxisServoOn(int axisID, int status)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ExtAxisServoOn(axisID, status);
                if (log != null)
                {
                    log.LogInfo($"ExtAxisServoOn({axisID}, {status}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
            * @brief 可移动装置使能
            * @param enable false-去使能；true-使能
            * @return 错误码
            */
        public int TractorEnable(bool enable)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int enable1;
                if (enable == false)
                {
                    enable1 = 0;
                }
                else
                {
                    enable1 = 1;
                }

                int rtn = proxy.TractorEnable(enable1);
                if (log != null)
                {
                    log.LogInfo($"execute TractorEnable({enable}) : {rtn}");
                }
                return rtn;

            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }
        /**
        * @brief 可移动装置回零
        * @return 错误码
        */
        public int TractorHoming()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.TractorHoming();

                if (log != null)
                {
                    log.LogInfo($"execute TractorHoming : {rtn}");
                }
                return rtn;

            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief 可移动装置直线运动
        * @param distance 直线运动距离（mm）
        * @param vel 直线运动速度百分比（0-100）
        * @return 错误码
        */
        public int TractorMoveL(double distance, double vel)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.TractorMoveL(distance, vel);

                if (log != null)
                {
                    log.LogInfo($"execute TractorMoveL({distance}, {vel}) : {rtn}");
                }

                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
        * @brief 可移动装置圆弧运动
        * @param radio 圆弧运动半径（mm）
        * @param angle 圆弧运动角度（°）
        * @param vel 直线运动速度百分比（0-100）
        * @return 错误码
        */
        public int TractorMoveC(double radio, double angle, double vel)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = proxy.TractorMoveC(radio, angle, vel);

                if (log != null)
                {
                    log.LogInfo($"execute TractorMoveC({radio}, {angle},{vel}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief 设置焊丝寻位扩展IO端口
        * @param searchDoneDINum 焊丝寻位成功DO端口(0-127)
        * @param searchStartDONum 焊丝寻位启停控制DO端口(0-127)
        * @return 错误码
        */
        public int SetWireSearchExtDIONum(int searchDoneDINum, int searchStartDONum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetWireSearchExtDIONum(searchDoneDINum, searchStartDONum);

                if (log != null)
                {
                    log.LogInfo($"execute SetWireSearchExtDIONum({searchDoneDINum}, {searchStartDONum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /**
        * @brief 设置焊机控制模式扩展DO端口
        * @param DONum 焊机控制模式DO端口(0-127)
        * @return 错误码
        */
        public int SetWeldMachineCtrlModeExtDoNum(int DONum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetWeldMachineCtrlModeExtDoNum(DONum);

                if (log != null)
                {
                    log.LogInfo($"execute SetWeldMachineCtrlModeExtDoNum({DONum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /**
        * @brief 设置焊机控制模式
        * @param mode 焊机控制模式;0-一元化
        * @return 错误码
        */
        public int SetWeldMachineCtrlMode(int mode)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetWeldMachineCtrlMode(mode);

                if (log != null)
                {
                    log.LogInfo($"execute SetWeldMachineCtrlMode({mode}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief UDP扩展轴运动
         * @param [in] pos 目标位置
         * @param [in] ovl 速度百分比
         * @return 错误码
         */
        public int ExtAxisMove(ExaxisPos pos, double ovl)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                //单独调用时，默认异步运动
                int rtn = proxy.ExtAxisMoveJ(0, pos.ePos[0], pos.ePos[1], pos.ePos[2], pos.ePos[3], ovl);
                if (log != null)
                {
                    log.LogInfo($"ExtAxisMove({pos.ePos[0]}, {pos.ePos[1]}, {pos.ePos[2]}, {pos.ePos[3]}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置扩展DO
         * @param [in] DONum DO编号
         * @param [in] bOpen 开关 true-开；false-关
         * @param [in] smooth 是否平滑
         * @param [in] block 是否阻塞
         * @return 错误码
         */
        public int SetAuxDO(int DONum, bool bOpen, bool smooth, bool block)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int openFlag = bOpen ? 1 : 0;
                int smoothFlag = smooth ? 1 : 0;
                int noBlockFlag = block ? 0 : 1;

                int rtn = proxy.SetAuxDO(DONum, openFlag, smoothFlag, noBlockFlag);
                if (log != null)
                {
                    log.LogInfo($"SetAuxDO({DONum}, {openFlag}, {smoothFlag}, {noBlockFlag}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置扩展AO
         * @param [in] AONum AO编号 
         * @param [in] value 模拟量值[0-4095]
         * @param [in] block 是否阻塞
         * @return 错误码
         */
        public int SetAuxAO(int AONum, double value, bool block)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int noBlockFlag = block ? 0 : 1;

                int rtn = proxy.SetAuxAO(AONum, value, noBlockFlag);
                if (log != null)
                {
                    log.LogInfo($"SetAuxAO({AONum}, {value}, {block}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置扩展DI输入滤波时间
         * @param [in] filterTime 滤波时间(ms)
         * @return 错误码
         */
        public int SetAuxDIFilterTime(int filterTime)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetAuxDIFilterTime(filterTime);
                if (log != null)
                {
                    log.LogInfo($"SetAuxDIFilterTime({filterTime}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置扩展AI输入滤波时间
         * @param [in] AONum AO编号
         * @param [in] filterTime 滤波时间(ms)
         * @return 错误码
         */
        public int SetAuxAIFilterTime(int AONum, int filterTime)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetAuxAIFilterTime(AONum, filterTime);
                if (log != null)
                {
                    log.LogInfo($"SetAuxAIFilterTime({AONum},{filterTime}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 等待扩展DI输入
         * @param [in] DINum DI编号
         * @param [in] bOpen 开关 0-关；1-开
         * @param [in] time 最大等待时间(ms)
         * @param [in] errorAlarm 是否继续运动
         * @return 错误码
         */
        public int WaitAuxDI(int DINum, bool bOpen, int time, bool errorAlarm)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int openFlag = bOpen ? 1 : 0;
                int errorAlarmFlag = errorAlarm ? 1 : 0;

                int rtn = proxy.WaitAuxDI(DINum, openFlag, time, errorAlarmFlag);
                if (log != null)
                {
                    log.LogInfo($"WaitAuxDI({DINum}, {bOpen}, {time}, {errorAlarm}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 等待扩展AI输入
         * @param [in] AINum AI编号
         * @param [in] sign 0-大于；1-小于
         * @param [in] value AI值
         * @param [in] time 最大等待时间(ms)
         * @param [in] errorAlarm 是否继续运动
         * @return 错误码
         */
        public int WaitAuxAI(int AINum, int sign, int value, int time, bool errorAlarm)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int errorAlarmFlag = errorAlarm ? 1 : 0;

                int rtn = proxy.WaitAuxAI(AINum, sign, value, time, errorAlarmFlag);
                if (log != null)
                {
                    log.LogInfo($"WaitAuxAI({AINum}, {sign}, {value}, {time}, {errorAlarm}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 获取扩展DI值
         * @param [in] DINum DI编号
         * @param [in] isNoBlock 是否阻塞
         * @param [out] isOpen 0-关；1-开
         * @return 错误码
         */
        public int GetAuxDI(int DINum, bool isNoBlock, ref bool isOpen)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int blockFlag = isNoBlock ? 0 : 1;

                object[] result = proxy.GetAuxDI(DINum, blockFlag);
                if ((int)result[0] == 0)
                {
                    int openFlag = (int)result[1];
                    isOpen = (openFlag == 1) ? true : false;
                }
                if (log != null)
                {
                    log.LogInfo($"GetAuxDI({DINum}, {isNoBlock}, ref {isOpen}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 获取扩展AI值
         * @param [in] AINum AI编号
         * @param [in] isNoBlock 是否阻塞
         * @param [in] value 输入值
         * @return 错误码
         */
        public int GetAuxAI(int AINum, bool isNoBlock, ref int value)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int blockFlag = isNoBlock ? 0 : 1;
                object[] result = proxy.GetAuxAI(AINum, blockFlag);
                if ((int)result[0] == 0)
                {
                    value = (int)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetAuxAI({AINum}, {isNoBlock}, ref {value}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief UDP扩展轴通信异常断开后恢复连接
         * @return 错误码
         */
        public int ExtDevUDPClientComReset()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ExtDevUDPClientComReset();
                if (log != null)
                {
                    log.LogInfo($"ExtDevUDPClientComReset() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief UDP扩展轴通信异常断开后关闭通讯
         * @return 错误码
         */
        public int ExtDevUDPClientComClose()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.ExtDevUDPClientComClose();
                if (log != null)
                {
                    log.LogInfo($"ExtDevUDPClientComClose() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /**
         * @brief UDP扩展轴参数配置
         * @param [in] axisID 轴号
         * @param [in] axisType 扩展轴类型 0-平移；1-旋转
         * @param [in] axisDirection 扩展轴方向 0-正向；1-方向 
         * @param [in] axisMax 扩展轴最大位置 mm
         * @param [in] axisMin 扩展轴最小位置 mm
         * @param [in] axisVel 速度mm/s
         * @param [in] axisAcc 加速度mm/s2
         * @param [in] axisLead 导程mm
         * @param [in] encResolution 编码器分辨率
         * @param [in] axisOffect焊缝起始点扩展轴偏移量
         * @param [in] axisCompany 驱动器厂家 1-禾川；2-汇川；3-松下
         * @param [in] axisModel 驱动器型号 1-禾川-SV-XD3EA040L-E，2-禾川-SV-X2EA150A-A，1-汇川-SV620PT5R4I，1-松下-MADLN15SG，2-松下-MSDLN25SG，3-松下-MCDLN35SG
         * @param [in] axisEncType 编码器类型  0-增量；1-绝对值
         * @return 错误码
         */
        public int ExtAxisParamConfig(int axisID, int axisType, int axisDirection, double axisMax, double axisMin, double axisVel, double axisAcc, double axisLead, int encResolution, double axisOffect, int axisCompany, int axisModel, int axisEncType)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.ExtAxisParamConfig(axisID, axisType, axisDirection, axisMax, axisMin, axisVel, axisAcc, axisLead, encResolution, axisOffect, axisCompany, axisModel, axisEncType);
                if (log != null)
                {
                    log.LogInfo($"ExtAxisParamConfig({axisType}, {axisDirection}, {axisMax}, {axisMin}, {axisVel}, {axisAcc}, {axisLead}, {encResolution}, {axisOffect}, {axisCompany}, {axisModel}, {axisEncType}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 获取扩展轴驱动器配置信息
         * @param [in] axisId 轴号[1-4]
         * @param [out] axisCompany 驱动器厂家 1-禾川；2-汇川；3-松下
         * @param [out] axisModel 驱动器型号 1-禾川-SV-XD3EA040L-E，2-禾川-SV-X2EA150A-A，1-汇川-SV620PT5R4I，1-松下-MADLN15SG，2-松下-MSDLN25SG，3-松下-MCDLN35SG
         * @param [out] axisEncType 编码器类型  0-增量；1-绝对值
         * @return 错误码
         */
        private int GetExAxisDriverConfig(int axisId, ref int axisCompany, ref int axisModel, ref int axisEncType)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetExAxisDriverConfig(axisId);
                if ((int)result[0] == 0)
                {
                    axisCompany = (int)result[1];
                    axisModel = (int)result[2];
                    axisEncType = (int)result[3];
                }
                if (log != null)
                {
                    log.LogInfo($"GetExAxisDriverConfig({axisId}, ref {axisCompany}, ref {axisModel}, ref {axisEncType}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置扩展轴安装位置
         * @param [in] installType 0-机器人安装在外部轴上，1-机器人安装在外部轴外
         * @return 错误码
         */
        public int SetRobotPosToAxis(int installType)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetRobotPosToAxis(installType);
                if (log != null)
                {
                    log.LogInfo($"SetRobotPosToAxis({installType}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置扩展轴系统DH参数配置
         * @param [in]  axisConfig 外部轴构型，0-单自由度直线滑轨，1-两自由度L型变位机，2-三自由度，3-四自由度，4-单自由度变位机
         * @param [in]  axisDHd1 外部轴DH参数d1 mm
         * @param [in]  axisDHd2 外部轴DH参数d2 mm
         * @param [in]  axisDHd3 外部轴DH参数d3 mm
         * @param [in]  axisDHd4 外部轴DH参数d4 mm
         * @param [in]  axisDHa1 外部轴DH参数11 mm
         * @param [in]  axisDHa2 外部轴DH参数a2 mm
         * @param [in]  axisDHa3 外部轴DH参数a3 mm
         * @param [in]  axisDHa4 外部轴DH参数a4 mm
         * @return 错误码
         */
        public int SetAxisDHParaConfig(int axisConfig, double axisDHd1, double axisDHd2, double axisDHd3, double axisDHd4, double axisDHa1, double axisDHa2, double axisDHa3, double axisDHa4)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetAxisDHParaConfig(axisConfig, axisDHd1, axisDHd2, axisDHd3, axisDHd4, axisDHa1, axisDHa2, axisDHa3, axisDHa4);
                if (log != null)
                {
                    log.LogInfo($"SetAxisDHParaConfig({axisConfig}, {axisDHd1}, {axisDHd2}, {axisDHd3}, {axisDHd4}, {axisDHa1}, {axisDHa2}, {axisDHa3}, {axisDHa4}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置扩展轴坐标系参考点-四点法
         * @param [in]  pointNum 点编号[1-4]
         * @return 错误码
         */
        public int ExtAxisSetRefPoint(int pointNum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.ExtAxisSetRefPoint(pointNum);
                if (log != null)
                {
                    log.LogInfo($"ExtAxisSetRefPoint({pointNum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 计算扩展轴坐标系-四点法
         * @param [out]  coord 坐标系值
         * @return 错误码
         */
        public int ExtAxisComputeECoordSys(ref DescPose coord)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.ExtAxisComputeECoordSys();
                if ((int)result[0] == 0)
                {
                    coord.tran.x = (double)result[1];
                    coord.tran.y = (double)result[1];
                    coord.tran.z = (double)result[1];
                    coord.rpy.rx = (double)result[1];
                    coord.rpy.ry = (double)result[1];
                    coord.rpy.rz = (double)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"ExtAxisComputeECoordSys({coord.tran.x}, ref {coord.tran.y}, ref {coord.tran.z}, ref {coord.rpy.rx}, ref {coord.rpy.ry}, ref {coord.rpy.rz}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 应用扩展轴坐标系
         * @param [in]  applyAxisId 扩展轴编号 bit0-bit3对应扩展轴编号1-4，如应用扩展轴1和3，则是 0b 0000 0101；也就是5
         * @param [in]  axisCoordNum 扩展轴坐标系编号
         * @param [in]  coord 坐标系值
         * @param [in]  calibFlag 标定标志 0-否，1-是
         * @return 错误码
         */
        public int ExtAxisActiveECoordSys(int applyAxisId, int axisCoordNum, DescPose coord, int calibFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.ExtAxisActiveECoordSys(applyAxisId, axisCoordNum, coord.tran.x, coord.tran.y, coord.tran.z, coord.rpy.rx, coord.rpy.ry, coord.rpy.rz, calibFlag);
                if (log != null)
                {
                    log.LogInfo($"ExtAxisActiveECoordSys({applyAxisId}, {axisCoordNum}, {coord.tran.x}, {coord.tran.y}, {coord.tran.z}, {coord.rpy.rx}, {coord.rpy.ry}, {coord.rpy.rz}, {calibFlag}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置标定参考点在变位机末端坐标系下位姿
         * @param [in] pos 位姿值
         * @return 错误码
         */
        public int SetRefPointInExAxisEnd(DescPose pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetRefPointInExAxisEnd(pos.tran.x, pos.tran.y, pos.tran.z, pos.rpy.rx, pos.rpy.ry, pos.rpy.rz);
                if (log != null)
                {
                    log.LogInfo($"SetRefPointInExAxisEnd({pos.tran.x}, {pos.tran.y}, {pos.tran.z}, {pos.rpy.rx}, {pos.rpy.ry}, {pos.rpy.rz}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 变位机坐标系参考点设置
         * @param [in]  pointNum 点编号[1-4]
         * @return 错误码
         */
        public int PositionorSetRefPoint(int pointNum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.PositionorSetRefPoint(pointNum);
                if (log != null)
                {
                    log.LogInfo($"PositionorSetRefPoint({pointNum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 变位机坐标系计算-四点法
         * @param [out]  coord 坐标系值
         * @return 错误码
         */
        public int PositionorComputeECoordSys(ref DescPose coord)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.PositionorComputeECoordSys();
                if ((int)result[0] == 0)
                {
                    coord.tran.x = (double)result[1];
                    coord.tran.y = (double)result[1];
                    coord.tran.z = (double)result[1];
                    coord.rpy.rx = (double)result[1];
                    coord.rpy.ry = (double)result[1];
                    coord.rpy.rz = (double)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"PositionorComputeECoordSys({coord.tran.x}, ref {coord.tran.y}, ref {coord.tran.z}, ref {coord.rpy.rx}, ref {coord.rpy.ry}, ref {coord.rpy.rz}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  UDP扩展轴与机器人关节运动同步运动
         * @param  [in] jopublic int_pos  目标关节位置,单位deg
         * @param  [in] desc_pos   目标笛卡尔位姿
         * @param  [in] tool  工具坐标号，范围[0~14]
         * @param  [in] user  工件坐标号，范围[0~14]
         * @param  [in] vel  速度百分比，范围[0~100]
         * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] ovl  速度缩放因子，范围[0~100]
         * @param  [in] epos  扩展轴位置，单位mm
         * @param  [in] blendT [-1.0]-运动到位(阻塞)，[0~500.0]-平滑时间(非阻塞)，单位ms
         * @param  [in] offset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
         * @param  [in] offset_pos  位姿偏移量
         * @return  错误码
         */
        public int ExtAxisSyncMoveJ(JointPos joint_pos, DescPose desc_pos, int tool, int user, float vel, float acc, float ovl, ExaxisPos epos, float blendT, byte offset_flag, DescPose offset_pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                double[] joint = joint_pos.jPos;
                double[] desc = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                double[] offect = new double[6] { offset_pos.tran.x, offset_pos.tran.y, offset_pos.tran.z, offset_pos.rpy.rx, offset_pos.rpy.ry, offset_pos.rpy.rz };
                int rtn = 0;
                rtn = proxy.ExtAxisMoveJ(1, epos.ePos[0], epos.ePos[1], epos.ePos[2], epos.ePos[3], ovl);
                if (rtn != 0)
                {
                    if (log != null)
                    {
                        log.LogInfo($"ExtAxisSyncMoveJ({joint[0]},{joint[1]},{joint[2]},{joint[3]},{joint[4]},{joint[5]},{desc[0]},{desc[1]},{desc[2]},{desc[3]},{desc[4]},{desc[5]},{tool},{user},{vel},{acc},{ovl}," +
                            $"{epos.ePos[0]},{epos.ePos[1]},{epos.ePos[2]},{epos.ePos[3]},{blendT},{offset_flag},{offect[0]},{offect[1]},{offect[2]},{offect[3]},{offect[4]},{offect[5]}) : {rtn}");
                    }
                    return rtn;
                }
                rtn = proxy.MoveJ(joint, desc, tool, user, vel, acc, ovl, epos.ePos, blendT, offset_flag, offect);
                if (log != null)
                {
                    log.LogInfo($"ExtAxisSyncMoveJ({joint[0]},{joint[1]},{joint[2]},{joint[3]},{joint[4]},{joint[5]},{desc[0]},{desc[1]},{desc[2]},{desc[3]},{desc[4]},{desc[5]},{tool},{user},{vel},{acc},{ovl}," +
                        $"{epos.ePos[0]},{epos.ePos[1]},{epos.ePos[2]},{epos.ePos[3]},{blendT},{offset_flag},{offect[0]},{offect[1]},{offect[2]},{offect[3]},{offect[4]},{offect[5]}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  UDP扩展轴与机器人直线运动同步运动
         * @param  [in] joint_pos  目标关节位置,单位deg
         * @param  [in] desc_pos   目标笛卡尔位姿
         * @param  [in] tool  工具坐标号，范围[0~14]
         * @param  [in] user  工件坐标号，范围[0~14]
         * @param  [in] vel  速度百分比，范围[0~100]
         * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] ovl  速度缩放因子，范围[0~100]
         * @param  [in] blendR [-1.0]-运动到位(阻塞)，[0~1000.0]-平滑半径(非阻塞)，单位mm	 
         * @param  [in] epos  扩展轴位置，单位mm
         * @param  [in] offset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
         * @param  [in] offset_pos  位姿偏移量
         * @return  错误码
         */
        public int ExtAxisSyncMoveL(JointPos joint_pos, DescPose desc_pos, int tool, int user, float vel, float acc, float ovl, float blendR, ExaxisPos epos, int offset_flag, DescPose offset_pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int search = 0;
                int rtn = -1;
                double[] joint = joint_pos.jPos;
                double[] desc = new double[6] { desc_pos.tran.x, desc_pos.tran.y, desc_pos.tran.z, desc_pos.rpy.rx, desc_pos.rpy.ry, desc_pos.rpy.rz };
                double[] offect = new double[6] { offset_pos.tran.x, offset_pos.tran.y, offset_pos.tran.z, offset_pos.rpy.rx, offset_pos.rpy.ry, offset_pos.rpy.rz };

                rtn = proxy.ExtAxisMoveJ(1, epos.ePos[0], epos.ePos[1], epos.ePos[2], epos.ePos[3], ovl);
                if (rtn != 0)
                {
                    if (log != null)
                    {
                        log.LogInfo($"ExtAxisMoveJ( {epos.ePos[0]},{epos.ePos[1]},{epos.ePos[2]},{epos.ePos[3]},{offset_flag},{offect[0]},{offect[1]},{offect[2]},{offect[3]},{offect[4]},{offect[5]}) : {rtn}");
                    }
                    return rtn;
                }
                rtn = proxy.MoveL(joint, desc, tool, user, vel, acc, ovl, blendR, epos.ePos, search, offset_flag, offect);
                if (log != null)
                {
                    log.LogInfo($"MoveL({joint[0]},{joint[1]},{joint[2]},{joint[3]},{joint[4]},{joint[5]},{desc[0]},{desc[1]},{desc[2]},{desc[3]},{desc[4]},{desc[5]},{tool},{user},{vel},{acc},{ovl},{blendR}" +
                        $"{epos.ePos[0]},{epos.ePos[1]},{epos.ePos[2]},{epos.ePos[3]},{offset_flag},{offect[0]},{offect[1]},{offect[2]},{offect[3]},{offect[4]},{offect[5]}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  UDP扩展轴与机器人圆弧运动同步运动
         * @param  [in] joint_pos_p  路径点关节位置,单位deg
         * @param  [in] desc_pos_p   路径点笛卡尔位姿
         * @param  [in] ptool  工具坐标号，范围[0~14]
         * @param  [in] puser  工件坐标号，范围[0~14]
         * @param  [in] pvel  速度百分比，范围[0~100]
         * @param  [in] pacc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] epos_p  中间点扩展轴位置，单位mm
         * @param  [in] poffset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
         * @param  [in] offset_pos_p  位姿偏移量
         * @param  [in] joint_pos_t  目标点关节位置,单位deg
         * @param  [in] desc_pos_t   目标点笛卡尔位姿
         * @param  [in] ttool  工具坐标号，范围[0~14]
         * @param  [in] tuser  工件坐标号，范围[0~14]
         * @param  [in] tvel  速度百分比，范围[0~100]
         * @param  [in] tacc  加速度百分比，范围[0~100],暂不开放
         * @param  [in] epos_t  扩展轴位置，单位mm
         * @param  [in] toffset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
         * @param  [in] offset_pos_t  位姿偏移量	 
         * @param  [in] ovl  速度缩放因子，范围[0~100]
         * @param  [in] blendR [-1.0]-运动到位(阻塞)，[0~1000.0]-平滑半径(非阻塞)，单位mm	 
         * @return  错误码
         */
        public int ExtAxisSyncMoveC(JointPos joint_pos_p, DescPose desc_pos_p, int ptool, int puser, float pvel, float pacc, ExaxisPos epos_p, int poffset_flag, DescPose offset_pos_p, JointPos joint_pos_t, DescPose desc_pos_t, int ttool, int tuser, float tvel, float tacc, ExaxisPos epos_t, int toffset_flag, DescPose offset_pos_t, float ovl, float blendR)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            try
            {
                int rtn = 0;
                double[] jointP = joint_pos_p.jPos;
                double[] descP = new double[6] { desc_pos_p.tran.x, desc_pos_p.tran.y, desc_pos_p.tran.z, desc_pos_p.rpy.rx, desc_pos_p.rpy.ry, desc_pos_p.rpy.rz };
                double[] offectP = new double[6] { offset_pos_p.tran.x, offset_pos_p.tran.y, offset_pos_p.tran.z, offset_pos_p.rpy.rx, offset_pos_p.rpy.ry, offset_pos_p.rpy.rz };
                double[] controlP = new double[4] { ptool, puser, pvel, pacc };

                double[] jointT = joint_pos_t.jPos;
                double[] descT = new double[6] { desc_pos_t.tran.x, desc_pos_t.tran.y, desc_pos_t.tran.z, desc_pos_t.rpy.rx, desc_pos_t.rpy.ry, desc_pos_t.rpy.rz };
                double[] offectT = new double[6] { offset_pos_t.tran.x, offset_pos_t.tran.y, offset_pos_t.tran.z, offset_pos_t.rpy.rx, offset_pos_t.rpy.ry, offset_pos_t.rpy.rz };
                double[] controlT = new double[4] { ttool, tuser, tvel, tacc };

                rtn = proxy.ExtAxisMoveJ(1, epos_t.ePos[0], epos_t.ePos[1], epos_t.ePos[2], epos_t.ePos[3], ovl);
                if (rtn != 0)
                {
                    if (log != null)
                    {
                        log.LogInfo($"ExtAxisMoveJ({epos_t.ePos[0]},{epos_t.ePos[1]},{epos_t.ePos[2]},{epos_t.ePos[3]},{blendR} : {rtn}");
                    }
                    return rtn;
                }

                rtn = proxy.MoveC(jointP, descP, controlP, epos_p.ePos, poffset_flag, offectP, jointT, descT, controlT, epos_t.ePos, toffset_flag, offectT, ovl, blendR);
                if (log != null)
                {
                    log.LogInfo($"MoveC({jointP[0]},{jointP[1]},{jointP[2]},{jointP[3]},{jointP[4]},{jointP[5]},{descP[0]},{descP[1]},{descP[2]},{descP[3]},{descP[4]},{descP[5]},{ptool},{puser},{pvel},{pacc}," +
                        $",{poffset_flag},{offectP[0]},{offectP[1]},{offectP[2]},{offectP[3]},{offectP[4]},{offectP[5]},) " +
                        $"{jointT[0]},{jointT[1]},{jointT[2]},{jointT[3]},{jointT[4]},{jointT[5]},{descT[0]},{descT[1]},{descT[2]},{descT[3]},{descT[4]},{descT[5]},{ttool},{tuser},{tvel},{tacc}," +
                        $"{toffset_flag},{offectT[0]},{offectT[1]},{offectT[2]},{offectT[3]},{offectT[4]},{offectT[5]},{ovl},{epos_t.ePos[0]},{epos_t.ePos[1]},{epos_t.ePos[2]},{epos_t.ePos[3]},{blendR} : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  焊丝寻位开始
         * @param  [in] refPos  1-基准点 2-接触点
         * @param  [in] searchVel   寻位速度 %
         * @param  [in] searchDis  寻位距离 mm
         * @param  [in] autoBackFlag 自动返回标志，0-不自动；-自动
         * @param  [in] autoBackVel  自动返回速度 %
         * @param  [in] autoBackDis  自动返回距离 mm
         * @param  [in] offectFlag  1-带偏移量寻位；2-示教点寻位
         * @return  错误码
         */
        public int WireSearchStart(int refPos, double searchVel, int searchDis, int autoBackFlag, double autoBackVel, int autoBackDis, int offectFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {

                return GetSafetyCode();
            }
            //try
            //{
            int rtn = proxy.WireSearchStart(refPos, searchVel, searchDis, autoBackFlag, autoBackVel, autoBackDis, offectFlag);
            if (log != null)
            {
                log.LogInfo($"WireSearchStart({refPos}, {searchVel}, {searchDis}, {autoBackFlag}, {autoBackVel}, {autoBackDis}, {offectFlag}) : {rtn}");
            }
            return rtn;
            //}
            //catch
            //{
            //    if (log != null)
            //    {
            //        log.LogError($"RPC exception");
            //    }
            //    return (int)RobotError.ERR_RPC_ERROR;
            //}
        }

        /**
         * @brief  焊丝寻位结束
         * @param  [in] refPos  1-基准点 2-接触点
         * @param  [in] searchVel   寻位速度 %
         * @param  [in] searchDis  寻位距离 mm
         * @param  [in] autoBackFlag 自动返回标志，0-不自动；-自动
         * @param  [in] autoBackVel  自动返回速度 %
         * @param  [in] autoBackDis  自动返回距离 mm
         * @param  [in] offectFlag  1-带偏移量寻位；2-示教点寻位
         * @return  错误码
         */
        public int WireSearchEnd(int refPos, double searchVel, int searchDis, int autoBackFlag, double autoBackVel, int autoBackDis, int offectFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.WireSearchEnd(refPos, searchVel, searchDis, autoBackFlag, autoBackVel, autoBackDis, offectFlag);
                if (log != null)
                {
                    log.LogInfo($"WireSearchEnd({refPos}, {searchVel}, {searchDis}, {autoBackFlag}, {autoBackVel}, {autoBackDis}, {offectFlag}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  计算焊丝寻位偏移量
         * @param  [in] seamType  焊缝类型
         * @param  [in] method   计算方法
         * @param  [in] varNameRef 基准点1-6，“#”表示无点变量
         * @param  [in] varNameRes 接触点1-6，“#”表示无点变量
         * @param  [out] offectFlag 0-偏移量直接叠加到指令点；1-偏移量需要对指令点进行坐标变换
         * @param  [out] offect 偏移位姿[x, y, z, a, b, c]
         * @return  错误码
         */
        public int GetWireSearchOffset(int seamType, int method, string[] varNameRef, string[] varNameRes, ref int offsetFlag, ref DescPose offset)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetWireSearchOffset(seamType, method, varNameRef[0], varNameRef[1], varNameRef[2], varNameRef[3], varNameRef[4], varNameRef[5], varNameRes[0], varNameRes[1], varNameRes[2], varNameRes[3], varNameRes[4], varNameRes[5]);
                if ((int)result[0] == 0)
                {
                    offsetFlag = (int)result[1];
                    offset.tran.x = (double)result[2];
                    offset.tran.y = (double)result[3];
                    offset.tran.z = (double)result[4];
                    offset.rpy.rx = (double)result[5];
                    offset.rpy.ry = (double)result[6];
                    offset.rpy.rz = (double)result[7];
                }
                if (log != null)
                {
                    log.LogInfo($"GetWireSearchOffect({seamType}, {method}, {varNameRef[0]}, {varNameRef[1]}, {varNameRef[2]}, {varNameRef[3]}, {varNameRef[4]}, {varNameRef[5]}, {varNameRes[0]}, {varNameRes[1]}, {varNameRes[2]}, {varNameRes[3]}, {varNameRes[4]}, {varNameRes[5]}, {offsetFlag}, {offset.tran.x}, ref {offset.tran.y}, ref {offset.tran.z}, ref {offset.rpy.rx}, ref {offset.rpy.ry}, ref {offset.rpy.rz}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  等待焊丝寻位完成
         * @return  错误码
         */
        public int WireSearchWait(string name)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.WireSearchWait(name);
                if (log != null)
                {
                    log.LogInfo($"WireSearchWait() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  焊丝寻位接触点写入数据库
         * @param  [in] varName  接触点名称 “RES0” ~ “RES99”
         * @param  [in] pos  接触点数据[x, y, x, a, b, c]
         * @return  错误码
         */
        public int SetPointToDatabase(string varName, DescPose pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                double[] tmpPos = new double[6] { pos.tran.x, pos.tran.y, pos.tran.z, pos.rpy.rx, pos.rpy.ry, pos.rpy.rz };
                int rtn = proxy.SetPointToDatabase(varName, tmpPos);
                if (log != null)
                {
                    log.LogInfo($"SetPointToDatabase({varName}, {pos.tran.x}, {pos.tran.y}, {pos.tran.z}, {pos.rpy.rx}, {pos.rpy.ry}, {pos.rpy.rz}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  电弧跟踪控制
         * @param  [in] flag 开关，0-关；1-开
         * @param  [in] dalayTime 滞后时间，单位ms
         * @param  [in] isLeftRight 左右偏差补偿
         * @param  [in] klr 左右调节系数(灵敏度)
         * @param  [in] tStartLr 左右开始补偿时间cyc
         * @param  [in] stepMaxLr 左右每次最大补偿量 mm
         * @param  [in] sumMaxLr 左右总计最大补偿量 mm
         * @param  [in] isUpLow 上下偏差补偿
         * @param  [in] kud 上下调节系数(灵敏度)
         * @param  [in] tStartUd 上下开始补偿时间cyc
         * @param  [in] stepMaxUd 上下每次最大补偿量 mm
         * @param  [in] sumMaxUd 上下总计最大补偿量
         * @param  [in] axisSelect 上下坐标系选择，0-摆动；1-工具；2-基座
         * @param  [in] referenceType 上下基准电流设定方式，0-反馈；1-常数
         * @param  [in] referSampleStartUd 上下基准电流采样开始计数(反馈)，cyc
         * @param  [in] referSampleCountUd 上下基准电流采样循环计数(反馈)，cyc
         * @param  [in] referenceCurrent 上下基准电流mA
         * @param  [in] offsetType 偏置跟踪类型，0-不偏置；1-采样；2-百分比  /version 3.7.9
         * @param  [in] offsetParameter 偏置参数；采样(偏置采样开始时间，默认采一周期)；百分比(偏置百分比(-100 ~ 100)) /version 3.7.9
         * @return  错误码
         */
        public int ArcWeldTraceControl(int flag, double delaytime, int isLeftRight, double klr, double tStartLr, double stepMaxLr, double sumMaxLr, int isUpLow, double kud, double tStartUd, double stepMaxUd, double sumMaxUd, int axisSelect, int referenceType, double referSampleStartUd, double referSampleCountUd, double referenceCurrent, int offsetType=0, int offsetParameter=0)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                double[] paramLR = new double[4] { klr, tStartLr, stepMaxLr, sumMaxLr };
                double[] paramUD = new double[4] { kud, tStartUd, stepMaxUd, sumMaxUd };
                int rtn = proxy.ArcWeldTraceControl(flag, delaytime, isLeftRight, paramLR, isUpLow, paramUD, axisSelect, referenceType, referSampleStartUd, referSampleCountUd, referenceCurrent, offsetType, offsetParameter);
                if (log != null)
                {
                    log.LogInfo($"ArcWeldTraceControl({flag}, {delaytime}, {isLeftRight}, {klr}, {tStartLr}, {stepMaxLr}, {sumMaxLr}, {isUpLow}, {kud}, {tStartUd}, {stepMaxUd}, {sumMaxUd}, {axisSelect}, {referenceType}, {referSampleStartUd}, {referSampleCountUd}, {referenceCurrent},{offsetType}, {offsetParameter}) : {rtn}");
                }
                return rtn;
            }
            catch { 
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  电弧跟踪AI通带选择
         * @param  [in] channel 电弧跟踪AI通带选择,[0-3]
         * @return  错误码
         */
        public int ArcWeldTraceExtAIChannelConfig(int channel)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.ArcWeldTraceExtAIChannelConfig(channel);
                if (log != null)
                {
                    log.LogInfo($"ArcWeldTraceExtAIChannelConfig({channel} ) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /**
        * @brief  力传感器辅助拖动
        * @param  [in] status 控制状态，0-关闭；1-开启
        * @param  [in] asaptiveFlag 自适应开启标志，0-关闭；1-开启
        * @param  [in] interfereDragFlag 干涉区拖动标志，0-关闭；1-开启
        * @param  [in] ingularityConstraintsFlag 奇异点策略，0-规避；1-穿越
        * @param  [in] M 惯性系数
        * @param  [in] B 阻尼系数
        * @param  [in] K 刚度系数
        * @param  [in] F 拖动六维力阈值
        * @param  [in] Fmax 最大拖动力限制 Nm
        * @param  [in] Vmax 最大关节速度限制 °/s
        * @return  错误码
        */
        public int EndForceDragControl(int status, int asaptiveFlag, int interfereDragFlag, int ingularityConstraintsFlag, double[] M, double[] B, double[] K, double[] F, double Fmax, double Vmax)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.EndForceDragControl(status, asaptiveFlag, interfereDragFlag, ingularityConstraintsFlag,M, B, K, F, Fmax, Vmax);
                if (log != null)
                {
                    log.LogInfo($"EndForceDragControl({status}, {asaptiveFlag}, {interfereDragFlag},{ingularityConstraintsFlag}, {M}, {B}, {K}, {F}, {Fmax}, {Vmax}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  报错清除后力传感器自动开启
         * @param  [in] status 控制状态，0-关闭；1-开启
         * @return  错误码
         */
        public int SetForceSensorDragAutoFlag(int status)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetForceSensorDragAutoFlag(status);
                if (log != null)
                {
                    log.LogInfo($"SetForceSensorDragAutoFlag({status}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置六维力和关节阻抗混合拖动开关及参数
         * @param  [in] status 控制状态，0-关闭；1-开启
         * @param  [in] impedanceFlag 阻抗开启标志，0-关闭；1-开启
         * @param  [in] lamdeDain 拖动增益
         * @param  [in] KGain 刚度增益
         * @param  [in] BGain 阻尼增益
         * @param  [in] dragMaxTcpVel 拖动末端最大线速度限制
         * @param  [in] dragMaxTcpOriVel 拖动末端最大角速度限制
         * @return  错误码
         */
        public int ForceAndJointImpedanceStartStop(int status, int impedanceFlag, double[] lamdeDain, double[] KGain, double[] BGain, double dragMaxTcpVel, double dragMaxTcpOriVel)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.ForceAndJointImpedanceStartStop(status, impedanceFlag, lamdeDain, KGain, BGain, dragMaxTcpVel, dragMaxTcpOriVel);
                if (log != null)
                {
                    log.LogInfo($"ForceAndJointImpedanceStartStop({status}, {impedanceFlag}, {lamdeDain}, {KGain}, {BGain}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取力传感器拖动开关状态
         * @param  [out] dragState 力传感器辅助拖动控制状态，0-关闭；1-开启
         * @param  [out] sixDimensionalDragState 六维力辅助拖动控制状态，0-关闭；1-开启
         * @return  错误码
         */
        public int GetForceAndTorqueDragState(ref int dragState, ref int sixDimensionalDragState)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetForceAndTorqueDragState();
                if ((int)result[0] == 0)
                {
                    dragState = (int)result[1];
                    sixDimensionalDragState = (int)result[2];
                }
                if (log != null)
                {
                    log.LogInfo($"GetForceAndTorqueDragState(ref {dragState}, ref {sixDimensionalDragState}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置力传感器下负载重量
         * @param  [in] weight 负载重量 kg
         * @return  错误码
         */
        public int SetForceSensorPayLoad(double weight)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetForceSensorPayload(weight);
                if (log != null)
                {
                    log.LogInfo($"SetForceSensorPayLoad({weight}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置力传感器下负载质心
         * @param  [in] x 负载质心x mm 
         * @param  [in] y 负载质心y mm
         * @param  [in] z 负载质心z mm
         * @return  错误码
         */
        public int SetForceSensorPayLoadCog(double x, double y, double z)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetForceSensorPayloadCog(x, y, z);
                if (log != null)
                {
                    log.LogInfo($"SetForceSensorPayLoadCog({x}, {y}, {z}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取力传感器下负载重量
         * @param  [in] weight 负载重量 kg
         * @return  错误码
         */
        public int GetForceSensorPayLoad(ref double weight)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetForceSensorPayload();
                if ((int)result[0] == 0)
                {
                    weight = (double)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetForceSensorPayLoad(ref {weight}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取力传感器下负载质心
         * @param  [out] x 负载质心x mm 
         * @param  [out] y 负载质心y mm
         * @param  [out] z 负载质心z mm
         * @return  错误码
         */
        public int GetForceSensorPayLoadCog(ref double x, ref double y, ref double z)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetForceSensorPayloadCog();
                if ((int)result[0] == 0)
                {
                    x = (double)result[1];
                    y = (double)result[2];
                    z = (double)result[3];
                }
                if (log != null)
                {
                    log.LogInfo($"GetForceSensorPayLoadCog(ref {x}, ref {y}, ref {z}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  力传感器自动校零
         * @param  [out] weight 传感器质量 kg 
         * @param  [out] pos 传感器质心 mm
         * @return  错误码
         */
        public int ForceSensorAutoComputeLoad(ref double weight, ref DescTran pos)
        {
            JointPos startJ = new JointPos(0, 0, 0, 0, 0, 0);
            DescPose startDesc = new DescPose(0, 0, 0, 0, 0, 0);
            GetActualJointPosDegree(1, ref startJ);
            GetActualTCPPose(1, ref startDesc);

            JointPos tmpJPos = new JointPos(0, 0, 0, 0, 0, 0);
            DescPose tmpDescPos = new DescPose(0, 0, 0, 0, 0, 0);
            DescPose offectPos = new DescPose(0, 0, 0, 0, 0, 0);
            ExaxisPos tmpExaxisPos = new ExaxisPos(0, 0, 0, 0);

            ForceSensorSetSaveDataFlag(1);

            GetActualJointPosDegree(1, ref tmpJPos);
            if (tmpJPos.jPos[2] < 0)
            {
                tmpJPos.jPos[3] += 90;
                GetForwardKin(tmpJPos, ref tmpDescPos);
            }
            else
            {
                tmpJPos.jPos[3] -= 90;
                GetForwardKin(tmpJPos, ref tmpDescPos);
            }
            MoveJ(tmpJPos, tmpDescPos, 0, 0, 100, 100, 100, tmpExaxisPos, -1, 0, offectPos);

            ForceSensorSetSaveDataFlag(2);

            GetActualJointPosDegree(1, ref tmpJPos);
            if (tmpJPos.jPos[5] < 0)
            {
                tmpJPos.jPos[5] += 90;
                GetForwardKin(tmpJPos, ref tmpDescPos);
            }
            else
            {
                tmpJPos.jPos[5] -= 90;
                GetForwardKin(tmpJPos, ref tmpDescPos);
            }
            MoveJ(tmpJPos, tmpDescPos, 0, 0, 100, 100, 100, tmpExaxisPos, -1, 0, offectPos);

            ForceSensorSetSaveDataFlag(3);

            ForceSensorComputeLoad(ref weight, ref pos);
            WaitMs(100);
            MoveJ(startJ, startDesc, 0, 0, 100, 100, 100, tmpExaxisPos, -1, 0, offectPos);
            return 0;
        }

        /**
         * @brief  传感器自动校零数据记录
         * @param  [in] recordCount 记录数据个数 1-3
         * @return  错误码
         */
        public int ForceSensorSetSaveDataFlag(int recordCount)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.ForceSensorSetSaveDataFlag(recordCount);
                if (log != null)
                {
                    log.LogInfo($"ForceSensorSetSaveDataFlag({recordCount}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  传感器自动校零计算
         * @param  [out] weight 传感器质量 kg
         * @param  [out] pos 传感器质心 [x, y, z]
         * @return  错误码
         */
        public int ForceSensorComputeLoad(ref double weight, ref DescTran pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.ForceSensorComputeLoad();
                if ((int)result[0] == 0)
                {
                    weight = (double)result[1];
                    pos.x = (double)result[2];
                    pos.y = (double)result[3];
                    pos.z = (double)result[4];
                }
                if (log != null)
                {
                    log.LogInfo($"ForceSensorComputeLoad(ref {weight}, ref {pos.x}, ref {pos.y}, ref {pos.z}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  段焊获取位置和姿态
         * @param  [in] startPos 起始点坐标
         * @param  [in] endPos 终止点坐标
         * @param  [in] startDistance 焊接点至起点的长度
         * @param  [out] weldPointDesc 焊接点的笛卡尔坐标信息
         * @param  [out] weldPointJoint 焊接点的关节坐标信息
         * @param  [out] tool 工具号
         * @param  [out] user 工件号
         * @return  错误码
         */
        public int GetSegmentWeldPoint(DescPose startPos, DescPose endPos, double startDistance, ref DescPose weldPointDesc, ref JointPos weldPointJoint, ref int tool, ref int user)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                //log.LogInfo($"segment input param {startPos.tran.x} {startPos.tran.y} {startPos.tran.z} {startPos.rpy.rx}");
                double[] tmpStartDesc = new double[6] { startPos.tran.x, startPos.tran.y, startPos.tran.z, startPos.rpy.rx, startPos.rpy.ry, startPos.rpy.rz };
                double[] tmpEndDesc = new double[6] { endPos.tran.x, endPos.tran.y, endPos.tran.z, endPos.rpy.rx, endPos.rpy.ry, endPos.rpy.rz };
                object[] result = proxy.GetSegmentWeldPoint(tmpStartDesc, tmpEndDesc, startDistance);
                if ((int)result[0] == 0)
                {
                    string paramStr = (string)result[1];
                    //Console.WriteLine(paramStr);
                    string[] parS = paramStr.Split(',');
                    if (parS.Length != 14)
                    {
                        log.LogError("get segment weld point fail");
                        return -1;
                    }
                    weldPointJoint.jPos[0] = double.Parse(parS[0]);
                    weldPointJoint.jPos[1] = double.Parse(parS[1]);
                    weldPointJoint.jPos[2] = double.Parse(parS[2]);
                    weldPointJoint.jPos[3] = double.Parse(parS[3]);
                    weldPointJoint.jPos[4] = double.Parse(parS[4]);
                    weldPointJoint.jPos[5] = double.Parse(parS[5]);

                    weldPointDesc.tran.x = double.Parse(parS[6]);
                    weldPointDesc.tran.y = double.Parse(parS[7]);
                    weldPointDesc.tran.z = double.Parse(parS[8]);
                    weldPointDesc.rpy.rx = double.Parse(parS[9]);
                    weldPointDesc.rpy.ry = double.Parse(parS[10]);
                    weldPointDesc.rpy.rz = double.Parse(parS[11]);

                    tool = (int)double.Parse(parS[12]);
                    user = (int)double.Parse(parS[13]);
                    return (int)result[0];
                }
                if (log != null)
                {
                    log.LogInfo($"GetSegmentWeldPoint({startPos}, ref {endPos}, {startDistance}, ref {weldPointDesc}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置焊接工艺曲线参数
         * @param  [in] id 焊接工艺编号(1-99)
         * @param  [in] startCurrent 起弧电流(A)
         * @param  [in] startVoltage 起弧电压(V)
         * @param  [in] startTime 起弧时间(ms)
         * @param  [in] weldCurrent 焊接电流(A)
         * @param  [in] weldVoltage 焊接电压(V)
         * @param  [in] endCurrent 收弧电流(A)
         * @param  [in] endVoltage 收弧电压(V)
         * @param  [in] endTime 收弧时间(ms)
         * @return  错误码
         */
        public int WeldingSetProcessParam(int id, double startCurrent, double startVoltage, double startTime, double weldCurrent, double weldVoltage, double endCurrent, double endVoltage, double endTime)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.WeldingSetProcessParam(id, startCurrent, startVoltage, startTime, weldCurrent, weldVoltage, endCurrent, endVoltage, endTime);
                if (log != null)
                {
                    log.LogInfo($"WeldingSetProcessParam({id}, {startCurrent}, {startVoltage}, {startTime}, {weldCurrent}, {weldVoltage}, {endCurrent}, {endVoltage}, {endTime}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取焊接工艺曲线参数
         * @param  [in] id 焊接工艺编号(1-99)
         * @param  [out] startCurrent 起弧电流(A)
         * @param  [out] startVoltage 起弧电压(V)
         * @param  [out] startTime 起弧时间(ms)
         * @param  [out] weldCurrent 焊接电流(A)
         * @param  [out] weldVoltage 焊接电压(V)
         * @param  [out] endCurrent 收弧电流(A)
         * @param  [out] endVoltage 收弧电压(V)
         * @param  [out] endTime 收弧时间(ms)
         * @return  错误码
         */
        public int WeldingGetProcessParam(int id, ref double startCurrent, ref double startVoltage, ref double startTime, ref double weldCurrent, ref double weldVoltage, ref double endCurrent, ref double endVoltage, ref double endTime)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.WeldingGetProcessParam(id);
                if ((int)result[0] == 0)
                {
                    startCurrent = (double)result[1];
                    startVoltage = (double)result[2];
                    startTime = (double)result[3];
                    weldCurrent = (double)result[4];
                    weldVoltage = (double)result[5];
                    endCurrent = (double)result[6];
                    endVoltage = (double)result[7];
                    endTime = (double)result[8];
                }
                if (log != null)
                {
                    log.LogInfo($"WeldingGetProcessParam({id}, ref {startCurrent}, ref {startVoltage}, ref {startTime}, ref {weldCurrent}, ref {weldVoltage}, ref {endCurrent}, ref {endVoltage}, ref {endTime}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  末端传感器配置
         * @param  [in] idCompany 厂商，18-JUNKONG；25-HUIDE
         * @param  [in] idDevice 类型，0-JUNKONG/RYR6T.V1.0
         * @param  [in] idSoftware 软件版本，0-J1.0/HuiDe1.0(暂未开放)
         * @param  [in] idBus 挂载位置，1-末端1号口；2-末端2号口...8-末端8号口(暂未开放)
         * @return  错误码
         */
        public int AxleSensorConfig(int idCompany, int idDevice, int idSoftware, int idBus)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.AxleSensorConfig(idCompany, idDevice, idSoftware, idBus);
                if (log != null)
                {
                    log.LogInfo($"AxleSensorConfig({idCompany}, {idDevice}, {idSoftware}, {idBus}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取末端传感器配置
         * @param  [out] idCompany 厂商，18-JUNKONG；25-HUIDE
         * @param  [out] idDevice 类型，0-JUNKONG/RYR6T.V1.0
         * @return  错误码
         */
        public int AxleSensorConfigGet(ref int idCompany, ref int idDevice)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.AxleSensorConfigGet();
                if ((int)result[0] == 0)
                {
                    idCompany = (int)result[1];
                    idDevice = (int)result[2];
                }
                if (log != null)
                {
                    log.LogInfo($"AxleSensorConfigGet(ref {idCompany}, ref {idDevice}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  末端传感器激活
         * @param  [in] actFlag 0-复位；1-激活
         * @return  错误码
         */
        public int AxleSensorActivate(int actFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.AxleSensorActivate(actFlag);
                if (log != null)
                {
                    log.LogInfo($"AxleSensorActivate({actFlag}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  末端传感器寄存器写入
         * @param  [in] devAddr  设备地址编号 0-255
         * @param  [in] regHAddr 寄存器地址高8位
         * @param  [in] regLAddr 寄存器地址低8位
         * @param  [in] regNum  寄存器个数 0-255
         * @param  [in] data1 写入寄存器数值1
         * @param  [in] data2 写入寄存器数值2
         * @param  [in] isNoBlock 0-阻塞；1-非阻塞
         * @return  错误码
         */
        public int AxleSensorRegWrite(int devAddr, int regHAddr, int regLAddr, int regNum, int data1, int data2, int isNoBlock)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.AxleSensorRegWrite(devAddr, regHAddr, regLAddr, regNum, data1, data2, isNoBlock);
                if (log != null)
                {
                    log.LogInfo($"AxleSensorRegWrite({devAddr}, {regHAddr}, {regLAddr}, {regNum}, {data1}, {data2}, {isNoBlock}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置控制箱DO停止/暂停后输出是否复位
         * @param  [in] resetFlag  0-不复位；1-复位
         * @return  错误码
         */
        public int SetOutputResetCtlBoxDO(int resetFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetOutputResetCtlBoxDO(resetFlag);
                if (log != null)
                {
                    log.LogInfo($"SetOutputResetCtlBoxDO({resetFlag}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置控制箱AO停止/暂停后输出是否复位
         * @param  [in] resetFlag  0-不复位；1-复位
         * @return  错误码
         */
        public int SetOutputResetCtlBoxAO(int resetFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetOutputResetCtlBoxAO(resetFlag);
                if (log != null)
                {
                    log.LogInfo($"SetOutputResetCtlBoxAO({resetFlag}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置末端工具DO停止/暂停后输出是否复位
         * @param  [in] resetFlag  0-不复位；1-复位
         * @return  错误码
         */
        public int SetOutputResetAxleDO(int resetFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetOutputResetAxleDO(resetFlag);
                if (log != null)
                {
                    log.LogInfo($"SetOutputResetAxleDO({resetFlag}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置末端工具AO停止/暂停后输出是否复位
         * @param  [in] resetFlag  0-不复位；1-复位
         * @return  错误码
         */
        public int SetOutputResetAxleAO(int resetFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetOutputResetAxleAO(resetFlag);
                if (log != null)
                {
                    log.LogInfo($"SetOutputResetAxleAO({resetFlag}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置扩展DO停止/暂停后输出是否复位
         * @param  [in] resetFlag  0-不复位；1-复位
         * @return  错误码
         */
        public int SetOutputResetExtDO(int resetFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetOutputResetExtDO(resetFlag);
                if (log != null)
                {
                    log.LogInfo($"SetOutputResetExtDO({resetFlag}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /**
         * @brief  设置扩展AO停止/暂停后输出是否复位
         * @param  [in] resetFlag  0-不复位；1-复位
         * @return  错误码
         */
        public int SetOutputResetExtAO(int resetFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetOutputResetExtAO(resetFlag);
                if (log != null)
                {
                    log.LogInfo($"SetOutputResetExtAO({resetFlag}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  设置SmartTool停止/暂停后输出是否复位
         * @param  [in] resetFlag  0-不复位；1-复位
         * @return  错误码
         */
        public int SetOutputResetSmartToolDO(int resetFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetOutputResetSmartToolDO(resetFlag);
                if (log != null)
                {
                    log.LogInfo($"SetOutputResetSmartToolDO({resetFlag}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  仿真摆动开始
         * @param  [in] weaveNum  摆动参数编号
         * @return  错误码
         */
        public int WeaveStartSim(int weaveNum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.WeaveStartSim(weaveNum);
                if (log != null)
                {
                    log.LogInfo($"WeaveStartSim({weaveNum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  仿真摆动结束
         * @param  [in] weaveNum  摆动参数编号
         * @return  错误码
         */
        public int WeaveEndSim(int weaveNum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.WeaveEndSim(weaveNum);
                if (log != null)
                {
                    log.LogInfo($"WeaveEndSim({weaveNum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  开始轨迹检测预警(不运动)
         * @param  [in] weaveNum   摆动参数编号
         * @return  错误码
         */
        public int WeaveInspectStart(int weaveNum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.WeaveInspectStart(weaveNum);
                if (log != null)
                {
                    log.LogInfo($"WeaveInspectStart({weaveNum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 结束轨迹检测预警(不运动)
         * @param  [in] weaveNum   摆动参数编号
         * @return  错误码
         */
        public int WeaveInspectEnd(int weaveNum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.WeaveInspectEnd(weaveNum);
                if (log != null)
                {
                    log.LogInfo($"WeaveInspectEnd({weaveNum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 扩展IO-配置焊机气体检测信号
         * @param  [in] DONum  气体检测信号扩展DO编号
         * @return  错误码
         */
        public int SetAirControlExtDoNum(int DONum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetAirControlExtDoNum(DONum);
                if (log != null)
                {
                    log.LogInfo($"SetAirControlExtDoNum({DONum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 扩展IO-配置焊机起弧信号
         * @param  [in] DONum  焊机起弧信号扩展DO编号
         * @return  错误码
         */
        public int SetArcStartExtDoNum(int DONum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetArcStartExtDoNum(DONum);
                if (log != null)
                {
                    log.LogInfo($"SetArcStartExtDoNum({DONum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 扩展IO-配置焊机反向送丝信号
         * @param  [in] DONum  反向送丝信号扩展DO编号
         * @return  错误码
         */
        public int SetWireReverseFeedExtDoNum(int DONum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetWireReverseFeedExtDoNum(DONum);
                if (log != null)
                {
                    log.LogInfo($"SetWireReverseFeedExtDoNum({DONum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 扩展IO-配置焊机正向送丝信号
         * @param  [in] DONum  正向送丝信号扩展DO编号
         * @return  错误码
         */
        public int SetWireForwardFeedExtDoNum(int DONum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetWireForwardFeedExtDoNum(DONum);
                if (log != null)
                {
                    log.LogInfo($"SetWireForwardFeedExtDoNum({DONum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 扩展IO-配置焊机起弧成功信号
         * @param  [in] DINum  起弧成功信号扩展DI编号
         * @return  错误码
         */
        public int SetArcDoneExtDiNum(int DINum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetArcDoneExtDiNum(DINum);
                if (log != null)
                {
                    log.LogInfo($"SetArcDoneExtDINum({DINum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 扩展IO-配置焊机准备信号
         * @param  [in] DINum  焊机准备信号扩展DI编号
         * @return  错误码
         */
        public int SetWeldReadyExtDiNum(int DINum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetWeldReadyExtDiNum(DINum);
                if (log != null)
                {
                    log.LogInfo($"SetWeldReadyExtDiNum({DINum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 扩展IO-配置焊接中断恢复信号
         * @param  [in] reWeldDINum  焊接中断后恢复焊接信号扩展DI编号
         * @param  [in] abortWeldDINum  焊接中断后退出焊接信号扩展DI编号
         * @return  错误码
         */
        public int SetExtDIWeldBreakOffRecover(int reWeldDINum, int abortWeldDINum)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetExtDIWeldBreakOffRecover(reWeldDINum, abortWeldDINum);
                if (log != null)
                {
                    log.LogInfo($"SetExtDIWeldBreakOffRecover({reWeldDINum},{abortWeldDINum}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置机器人碰撞检测方法
         * @param  [in] method 碰撞检测方法：0-电流模式；1-双编码器；2-电流和双编码器同时开启
         * @param [in] thresholdMode 碰撞等级阈值方式；0-碰撞等级固定阈值方式；1-自定义碰撞检测阈值
         * @return  错误码
         */
        public int SetCollisionDetectionMethod(int method,int thresholdMode=0)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetCollisionDetectionMethod(method, thresholdMode);
                if (log != null)
                {
                    log.LogInfo($"SetCollisionDetectionMethod({method}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置静态下碰撞检测开始关闭
         * @param  [in] status 0-关闭；1-开启
         * @return  错误码
         */
        public int SetStaticCollisionOnOff(int status)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = 0;
                proxy.SetStaticCollisionOnOff(status);
                if (log != null)
                {
                    log.LogInfo($"SetStaticCollisionOnOff({status}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 关节扭矩功率检测
         * @param  [in] status 0-关闭；1-开启
         * @param  [in] power 设定最大功率(W)
         * @return  错误码
         */
        public int SetPowerLimit(int status, double power)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetPowerLimit(status, power);
                if (log != null)
                {
                    log.LogInfo($"SetPowerLimit({status},{power}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 关节扭矩控制开始
         * @return  错误码
         */
        public int ServoJTStart()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.ServoJTStart();
                if (log != null)
                {
                    log.LogInfo($"ServoJTStart() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;

                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 关节扭矩控制
         * @param  [in] torque j1~j6关节扭矩，单位Nm
         * @param  [in] interval 指令周期，单位s，范围[0.001~0.008]
         * @return  错误码
         */
        public int ServoJT(double[] torque, double interval)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.ServoJT(torque, interval);
                if (log != null)
                {
                    log.LogInfo($"ServoJT() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 关节扭矩控制结束
         * @return  错误码
         */
        public int ServoJTEnd()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.ServoJTEnd();
                if (log != null)
                {
                    log.LogInfo($"ServoJTEnd() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置机器人 20004 端口反馈周期
         * @param [in] period 机器人 20004 端口反馈周期(ms)
         * @return  错误码
         */
        public int SetRobotRealtimeStateSamplePeriod(int period)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetRobotRealtimeStateSamplePeriod(period);
                if (log != null)
                {
                    log.LogInfo($"SetRobotRealtimeStateSamplePeriod({period}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief  获取机器人 20004 端口反馈周期
         * @param [out] period 机器人 20004 端口反馈周期(ms)
         * @return 错误码
         */
        public int GetRobotRealtimeStateSamplePeriod(ref int period)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetRobotRealtimeStateSamplePeriod();
                if ((int)result[0] == 0)
                {
                    period = (int)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetRobotRealtimeStateSamplePeriod(ref {period}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
     * @brief 获取机器人关节驱动器温度(℃)
     * @return 错误码
     */
        public int GetJointDriverTemperature(double[] temperature)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                GetRobotRealTimeState(ref pkg);
                for (int i = 0; i < 6; i++)
                {
                    temperature[i] = pkg.jointDriverTemperature[i];
                }
                return g_sock_com_err;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"get joint driver temperature failed");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 获取机器人关节驱动器扭矩(Nm)
         * @return 错误码
         */
        public int GetJointDriverTorque(double[] torque)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                GetRobotRealTimeState(ref pkg);
                for (int i = 0; i < 6; i++)
                {
                    torque[i] = pkg.jointDriverTorque[i];
                }
                return g_sock_com_err;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"get joint driver torque failed");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 电弧追踪 + 多层多道补偿开启
         * @return 错误码
         */
        public int ArcWeldTraceReplayStart()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.ArcWeldTraceReplayStart();
                if (log != null)
                {
                    log.LogInfo($"ArcWeldTraceReplayStart() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 电弧追踪 + 多层多道补偿关闭
         * @return 错误码
         */
        public int ArcWeldTraceReplayEnd()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.ArcWeldTraceReplayEnd();
                if (log != null)
                {
                    log.LogInfo($"ArcWeldTraceReplayEnd() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
        * @brief 偏移量坐标变化-多层多道焊
        * @param [in] pointO 基准点笛卡尔位姿
        * @param [in] pointX 基准点X向偏移方向点笛卡尔位姿
        * @param [in] pointZ 基准点Z向偏移方向点笛卡尔位姿
        * @param [in] dx x方向偏移量(mm)
        * @param [in] z方向偏移量(mm)
        * @param [in] 绕y轴偏移量(°)
        * @param [out] 计算结果偏移量
        * @return 错误码
        */
        public int MultilayerOffsetTrsfToBase(DescTran pointO, DescTran pointX, DescTran pointZ, double dx, double dy, double db, ref DescPose offset)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.MultilayerOffsetTrsfToBase(pointO.x, pointO.y, pointO.z, pointX.x, pointX.y, pointX.z, pointZ.x, pointZ.y, pointZ.z, dx, dy, db);
                if ((int)result[0] == 0)
                {
                    offset.tran.x = (double)result[1];
                    offset.tran.y = (double)result[2];
                    offset.tran.z = (double)result[3];
                    offset.rpy.rx = (double)result[4];
                    offset.rpy.ry = (double)result[5];
                    offset.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"MultilayerOffsetTrsfToBase({pointO.x}, {pointO.y}, {pointO.z}, {pointX.x}, {pointX.y}, {pointX.z}, {pointZ.x}, {pointZ.y}, {pointZ.z}, {dx}, {dy}, {db}): " + (int)result[0]);
                    log.LogInfo($"MultilayerOffsetTrsfToBase result offset ({offset.tran.x}, {offset.tran.y}, {offset.tran.z}, {offset.rpy.rx}, {offset.rpy.ry}, {offset.rpy.rz}): " + (int)result[0]);
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 指定姿态速度开启
         * @param [in] ratio 姿态速度百分比[0-300]
         * @return  错误码
         */
        public int AngularSpeedStart(int ratio)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.AngularSpeedStart(ratio);
                if (log != null)
                {
                    log.LogInfo($"AngularSpeedStart({ratio}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 指定姿态速度关闭
         * @return  错误码
         */
        public int AngularSpeedEnd()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.AngularSpeedEnd();
                if (log != null)
                {
                    log.LogInfo($"AngularSpeedEnd() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
        * @brief 机器人软件升级
        * @param [in] filePath 软件升级包全路径
        * @param [in] block 是否阻塞至升级完成 true:阻塞；false:非阻塞
        * @return  错误码
        */
        public int SoftwareUpgrade(string filePath, bool block)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            int errcode = FileUpLoad(1, filePath);
            if (0 == errcode)
            {
                int result = proxy.SoftwareUpgrade();

                if (result != 0)
                {
                    if (log != null)
                    {
                        log.LogError($"SoftwareUpgrade fail");
                    }
                    return result;
                }

                if (block)
                {
                    int upgradeState = -1;
                    Thread.Sleep(500);
                    GetSoftwareUpgradeState(ref upgradeState);
                    if (upgradeState == 0)
                    {
                        return -1;
                    }
                    while (upgradeState > 0 && upgradeState < 100)
                    {
                        Thread.Sleep(500);
                        GetSoftwareUpgradeState(ref upgradeState);
                    }

                    if (upgradeState == 100)
                    {
                        errcode = 0;
                    }
                    else
                    {
                        errcode = upgradeState;
                    }
                }
                return errcode;
            }
            else
            {
                return errcode;
            }
        }



        /**
        * @brief  获取机器人软件升级状态
        * @param [out] state 机器人软件包升级状态  0-空闲中或上传升级包中；1~100：升级完成百分比；-1:升级软件失败；-2：校验失败；-3：版本校验失败；-4：解压失败；-5：用户配置升级失败；-6：外设配置升级失败；-7：扩展轴配置升级失败；-8：机器人配置升级失败；-9：DH参数配置升级失败
        * @return  错误码
        */
        public int GetSoftwareUpgradeState(ref int state)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            state = robot_state_pkg.softwareUpgradeState;

            return g_sock_com_err;
        }

        /**
         * @brief 设置485扩展轴运动加减速度
         * @param [in] acc 485扩展轴运动加速度
         * @param [in] dec 485扩展轴运动减速度
         * @return  错误码
         */
        public int AuxServoSetAcc(double acc, double dec)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.AuxServoSetAcc(acc, dec);
                if (log != null)
                {
                    log.LogInfo($"AuxServoSetAcc({acc}, {dec}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置485扩展轴急停加减速度
         * @param [in] acc 485扩展轴急停加速度
         * @param [in] dec 485扩展轴急停减速度
         * @return  错误码
         */
        public int AuxServoSetEmergencyStopAcc(double acc, double dec)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.AuxServoSetEmergencyStopAcc(acc, dec);
                if (log != null)
                {
                    log.LogInfo($"AuxServoSetEmergencyStopAcc({acc}, {dec}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
	     * @brief 获取485扩展轴运动加减速度
	     * @param [out] acc 485扩展轴运动加速度
	     * @param [out] dec 485扩展轴运动减速度
	     * @return  错误码
	     */
        public int AuxServoGetAcc(ref double acc, ref double dec)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.AuxServoGetAcc();
                if ((int)result[0] == 0)
                {
                    acc = (double)result[1];
                    dec = (double)result[2];
                }
                if (log != null)
                {
                    log.LogInfo($"AuxServoGetAcc(ref {acc}, ref {dec}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
	     * @brief 获取485扩展轴急停加减速度
	     * @param [out] acc 485扩展轴急停加速度
	     * @param [out] dec 485扩展轴急停减速度
	     * @return  错误码
	     */
        public int AuxServoGetEmergencyStopAcc(ref double acc, ref double dec)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.AuxServoGetEmergencyStopAcc();
                if ((int)result[0] == 0)
                {
                    acc = (double)result[1];
                    dec = (double)result[2];
                }
                if (log != null)
                {
                    log.LogInfo($"AuxServoGetEmergencyStopAcc(ref {acc}, ref {dec}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
	     * @brief 获取末端通讯参数
	     * @param [out] baudRate 
	     * @param [out] dataBit 
	     * @param [out] stopBit
	     * @param [out] verify
	     * @param [out] timeout
	     * @param [out] timeoutTimes
	     * @param [out] period
	     * @return  错误码
	     */
        public int GetAxleCommunicationParam(ref AxleComParam getParam)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetAxleCommunicationParam();
                if ((int)result[0] == 0)
                {
                    getParam.baudRate = (int)result[1];
                    getParam.dataBit = (int)result[2];
                    getParam.stopBit = (int)result[3];
                    getParam.verify = (int)result[4];
                    getParam.timeout = (int)result[5];
                    getParam.timeoutTimes = (int)result[6];
                    getParam.period = (int)result[7];
                }
                if (log != null)
                {
                    log.LogInfo($"GetAxleCommunicationParam(ref {getParam.baudRate}, ref {getParam.dataBit}, ref {getParam.stopBit}, ref {getParam.verify}, ref {getParam.timeout}, ref {getParam.timeoutTimes}, ref {getParam.period}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置末端通讯参数
         * @param [in] baudRate 
         * @param [in] dataBit 
         * @param [in] stopBit
         * @param [in] verify
         * @param [in] timeout
         * @param [in] timeoutTimes
         * @param [in] period
         * @return  错误码
         */
        public int SetAxleCommunicationParam(AxleComParam param)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetAxleCommunicationParam(param.baudRate, param.dataBit, param.stopBit, param.verify, param.timeout, param.timeoutTimes, param.period);
                if (log != null)
                {
                    log.LogInfo($"SetAxleCommunicationParam({param.baudRate}, {param.dataBit}, {param.stopBit}, {param.verify}, {param.timeout}, {param.timeoutTimes}, {param.period}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置末端文件传输类型
         * @param [in] type 1-MCU升级文件；2-LUA文件
         * @return  错误码
         */
        public int SetAxleFileType(int type)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetAxleFileType(type);
                if (log != null)
                {
                    log.LogInfo($"SetAxleFileType({type}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置启用末端LUA执行
         * @param [in] enable 0-不启用；1-启用
         * @return  错误码
         */
        public int SetAxleLuaEnable(int enable)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetAxleLuaEnable(enable);
                if (log != null)
                {
                    log.LogInfo($"SetAxleLuaEnable({enable}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 末端LUA文件异常错误恢复
         * @param [in] status 0-不恢复；1-恢复
         * @return  错误码
         */
        public int SetRecoverAxleLuaErr(int status)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetRecoverAxleLuaErr(status);
                if (log != null)
                {
                    log.LogInfo($"SetRecoverAxleLuaErr({status}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 获取末端LUA执行使能状态
         * @param [out] status 0-未使能；1-已使能
         * @return  错误码
         */
        public int GetAxleLuaEnableStatus(ref int status)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetAxleLuaEnableStatus();
                if ((int)result[0] == 0)
                {
                    status = (int)result[1];
                }
                if (log != null)
                {
                    log.LogInfo($"GetAxleLuaEnableStatus(ref {status}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置末端LUA末端设备启用类型
         * @param [in] forceSensorEnable 力传感器启用状态，0-不启用；1-启用
         * @param [in] gripperEnable 夹爪启用状态，0-不启用；1-启用
         * @param [in] IOEnable IO设备启用状态，0-不启用；1-启用
         * @return  错误码
         */
        public int SetAxleLuaEnableDeviceType(int forceSensorEnable, int gripperEnable, int IOEnable)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetAxleLuaEnableDeviceType(forceSensorEnable, gripperEnable, IOEnable);
                if (log != null)
                {
                    log.LogInfo($"SetAxleLuaEnableDeviceType({forceSensorEnable},{gripperEnable}, {IOEnable}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 获取末端LUA末端设备启用类型
         * @param [out] forceSensorEnable 力传感器启用状态，0-不启用；1-启用
         * @param [out] gripperEnable 夹爪启用状态，0-不启用；1-启用
         * @param [out] IOEnable IO设备启用状态，0-不启用；1-启用
         * @return  错误码
         */
        public int GetAxleLuaEnableDeviceType(ref int forceSensorEnable, ref int gripperEnable, ref int IOEnable)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetAxleLuaEnableDeviceType();
                if ((int)result[0] == 0)
                {
                    forceSensorEnable = (int)result[1];
                    gripperEnable = (int)result[2];
                    IOEnable = (int)result[3];
                }
                if (log != null)
                {
                    log.LogInfo($"GetAxleLuaEnableDeviceType(ref {forceSensorEnable}, ref {gripperEnable}, ref {IOEnable}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 获取当前配置的末端设备
         * @param [out] forceSensorEnable 力传感器启用设备编号 0-未启用；1-启用
         * @param [out] gripperEnable 夹爪启用设备编号，0-不启用；1-启用
         * @param [out] IODeviceEnable IO设备启用设备编号，0-不启用；1-启用
         * @return  错误码
         */
        public int GetAxleLuaEnableDevice(ref int[] forceSensorEnable, ref int[] gripperEnable, ref int[] IODeviceEnable)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                string resultStr = "";
                object[] result = proxy.GetAxleLuaEnableDevice();
                if ((int)result[0] == 0)
                {
                    resultStr = (string)result[1];
                    string[] parS = resultStr.Split(',');
                    if (parS.Length != 24)
                    {
                        log.LogError("GetAxleLuaEnableDevice fail");
                        return -1;
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        forceSensorEnable[i] = int.Parse(parS[i]);
                        gripperEnable[i] = int.Parse(parS[i + 8]);
                        IODeviceEnable[i] = int.Parse(parS[i + 16]);
                    }
                    return (int)result[0];
                }
                if (log != null)
                {
                    log.LogInfo($"GetAxleLuaEnableDevice(ref {resultStr}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 设置启用夹爪动作控制功能
         * @param [in] id 夹爪设备编号
         * @param [in] func func[0]-夹爪使能；func[1]-夹爪初始化；2-位置设置；3-速度设置；4-力矩设置；6-读夹爪状态；7-读初始化状态；8-读故障码；9-读位置；10-读速度；11-读力矩
         * @return  错误码
         */
        public int SetAxleLuaGripperFunc(int id, int[] func)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetAxleLuaGripperFunc(id, func);
                if (log != null)
                {
                    log.LogInfo($"SetAxleLuaGripperFunc({id},{func[0]},{func[1]}, {func[2]}, {func[3]}, {func[4]}, {func[5]}, {func[6]}, {func[7]}, {func[8]}, {func[9]}, {func[10]}, {func[11]}, {func[12]}, {func[13]}, {func[14]}, {func[15]}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
          * @brief 获取启用夹爪动作控制功能
          * @param [in] id 夹爪设备编号
          * @param [out] func func[0]-夹爪使能；func[1]-夹爪初始化；2-位置设置；3-速度设置；4-力矩设置；6-读夹爪状态；7-读初始化状态；8-读故障码；9-读位置；10-读速度；11-读力矩
          * @return  错误码
          */
        public int GetAxleLuaGripperFunc(int id, ref int[] func)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                string resultStr = "";
                object[] result = proxy.GetAxleLuaGripperFunc(id);
                if ((int)result[0] == 0)
                {
                    resultStr = (string)result[1];
                    string[] parS = resultStr.Split(',');
                    if (parS.Length != 16)
                    {
                        log.LogError("GetAxleLuaGripperFunc fail");
                        return -1;
                    }
                    for (int i = 0; i < 16; i++)
                    {
                        func[i] = int.Parse(parS[i]);
                    }
                    if (log != null)
                    {
                        log.LogInfo($"GetAxleLuaGripperFunc({id},{func[0]},{func[1]}, {func[2]}, {func[3]}, {func[4]}, {func[5]}, {func[6]}, {func[7]}, {func[8]}, {func[9]}, {func[10]}, {func[11]}, {func[12]}, {func[13]}, {func[14]}, {func[15]}) : {(int)result[0]}");
                    }
                    return (int)result[0];
                }
                if (log != null)
                {
                    log.LogInfo($"GetAxleLuaGripperFunc(ref {resultStr}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
          * @brief 设置控制器外设协议LUA文件名
          * @param [in] id 协议编号
          * @param [in] name lua文件名称 “CTRL_LUA_test.lua”
          * @return  错误码
          */
        public int SetCtrlOpenLUAName(int id, string name)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetCtrlOpenLUAName(id, name);
                if (log != null)
                {
                    log.LogInfo($"SetCtrlOpenLUAName({id},{name}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
        * @brief 获取当前配置的控制器外设协议LUA文件名
        * @param [out] name 4个lua文件名称 “CTRL_LUA_test.lua”
        * @return  错误码
        */
        public int GetCtrlOpenLUAName(ref string[] name)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                string resultStr = "";
                object[] result = proxy.GetCtrlOpenLUAName();
                if ((int)result[0] == 0)
                {
                    resultStr = (string)result[1];
                    string[] parS = resultStr.Split(',');
                    if (parS.Length != 4)
                    {
                        log.LogError("GetCtrlOpenLUAName fail");
                        return -1;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        name[i] = parS[i];
                    }
                    return (int)result[0];
                }
                if (log != null)
                {
                    log.LogInfo($"GetCtrlOpenLUAName(ref {resultStr}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
        * @brief 加载控制器LUA协议
        * @param [in] id 控制器LUA协议编号
        * @return  错误码
        */
        public int LoadCtrlOpenLUA(int id)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.LoadCtrlOpenLUA(id);
                if (log != null)
                {
                    log.LogInfo($"LoadCtrlOpenLUA({id}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
        * @brief 卸载控制器LUA协议
        * @param [in] id 控制器LUA协议编号
        * @return  错误码
        */
        public int UnloadCtrlOpenLUA(int id)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.UnloadCtrlOpenLUA(id);
                if (log != null)
                {
                    log.LogInfo($"UnloadCtrlOpenLUA({id}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
        * @brief 设置控制器LUA协议错误码
        * @param [in] id 控制器LUA协议编号
        * @return  错误码
        */
        private int SetCtrlOpenLuaErrCode(int id, int code)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetCtrlOpenLuaErrCode(id, code);
                if (log != null)
                {
                    log.LogInfo($"SetCtrlOpenLuaErrCode({id},{code}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
        * @brief 机器人Ethercat从站文件写入
        * @param [in] type 从站文件类型，1-升级从站文件；2-升级从站配置文件
        * @param [in] slaveID 从站号
        * @param [in] fileName 上传文件名
        * @return  错误码
        */
        public int SlaveFileWrite(int type, int slaveID, string fileName)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SlaveFileWrite(type, slaveID, fileName);
                if (log != null)
                {
                    log.LogInfo($"SlaveFileWrite({type},{slaveID},{fileName}) : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
        * @brief 机器人Ethercat从站进入boot模式
        * @return  错误码
        */
        public int SetSysServoBootMode()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.SetSysServoBootMode();
                if (log != null)
                {
                    log.LogInfo($"SetSysServoBootMode() : {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /** 
        * @brief 上传末端Lua开放协议文件
        * @param filePath 本地lua文件路径名 ".../AXLE_LUA_End_DaHuan.lua"
        * @return 错误码 
        */
        public int AxleLuaUpload(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return (int)RobotError.ERR_UPLOAD_FILE_NOT_FOUND;
            }

            int rtn = FileUpLoad(10, filePath);
            if (rtn == 0)
            {
                String fileName = "/tmp/" + fileInfo.Name;
                rtn = SetAxleFileType(2);
                if (rtn != 0)
                {
                    return -1;
                }
                rtn = SetSysServoBootMode();
                if (rtn != 0)
                {
                    return -1;
                }
                rtn = SlaveFileWrite(1, 7, fileName);
                if (rtn != 0)
                {
                    return -1;
                }
                return 0;
            }
            else
            {
                if (log != null)
                {
                    log.LogError($"LuaUpLoadUpdate({filePath}) : {rtn}");
                }
                return rtn;
            }
        }
        /**
        * @brief  获取旋转夹爪的旋转圈数
        * @param  [out] fault  0-无错误，1-有错误
        * @param  [out] num  旋转圈数
        * @return  错误码
        */
        public int GetGripperRotNum(ref UInt16 fault, ref double num)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                fault = robot_state_pkg.gripper_fault;
                num = robot_state_pkg.gripperRotNum;
            }
            else
            {
                errcode = g_sock_com_err;
            }


            return errcode;
        }
        /**
        * @brief  获取旋转夹爪的旋转速度百分比
        * @param  [out] fault  0-无错误，1-有错误
        * @param  [out] speed  旋转速度百分比
        * @return  错误码
        */
        public int GetGripperRotSpeed(ref UInt16 fault, ref int speed)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                fault = robot_state_pkg.gripper_fault;
                speed = robot_state_pkg.gripperRotSpeed;
            }
            else
            {
                errcode = g_sock_com_err;
            }
            return errcode;
        }
        /**
        * @brief  获取旋转夹爪的旋转力矩百分比
        * @param  [out] fault  0-无错误，1-有错误
        * @param  [out] torque  旋转力矩百分比
        * @return  错误码
        */
        public int GetGripperRotTorque(ref UInt16 fault, ref int torque)
        {
            int errcode = 0;

            if (g_sock_com_err == (int)RobotError.ERR_SUCCESS)
            {
                fault = robot_state_pkg.gripper_fault;
                torque = robot_state_pkg.gripperRotTorque;
            }
            else
            {
                errcode = g_sock_com_err;
            }

            return errcode;
        }
        /**
        * @brief 开始Ptp运动FIR滤波
        * @param [in] maxAcc 最大加速度极值(deg/s2)
        * @param [in] maxJek 统一关节急动度极值(deg/s3)
        * @return 错误码
        */
        public int PtpFIRPlanningStart(double maxAcc, double maxJek=1000)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.PtpFIRPlanningStart(maxAcc, maxJek);
                if (log != null)
                {
                    log.LogInfo($"PtpFIRPlanningStart({maxAcc}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }
        /**
        * @brief 上传轨迹J文件
        * @param [in] filePath 上传轨迹文件的全路径名   C://test/testJ.txt
        * @return 错误码
        */
        public int TrajectoryJUpLoad(string filePath)
        {
            return FileUpLoad(20, filePath);
        }

        /**
         * @brief 删除轨迹J文件
         * @param [in] fileName 文件名称 testJ.txt
         * @return 错误码
         */

        public int TrajectoryJDelete(string fileName)
        {
            return FileDelete(20, fileName);
        }
        /**
        * @brief 关闭Ptp运动FIR滤波
        * @return 错误码
        */
        public int PtpFIRPlanningEnd()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.PtpFIRPlanningEnd();
                if (log != null)
                {
                    log.LogInfo($"PtpFIRPlanningEnd:({rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
        * @brief 开始LIN、ARC运动FIR滤波
        * @param [in] maxAccLin 线加速度极值(mm/s2)
        * @param [in] maxAccDeg 角加速度极值(deg/s2)
        * @param [in] maxJerkLin 线加加速度极值(mm/s3)
        * @param [in] maxJerkDeg 角加加速度极值(deg/s3)
        * @return 错误码
        */
        public int LinArcFIRPlanningStart(double maxAccLin, double maxAccDeg, double maxJerkLin, double maxJerkDeg)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.LinArcFIRPlanningStart(maxAccLin, maxAccDeg, maxJerkLin, maxJerkDeg);
                if (log != null)
                {
                    log.LogInfo($"LinArcFIRPlanningStart:({rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief 关闭LIN、ARC运动FIR滤波
        * @return 错误码
        */
        public int LinArcFIRPlanningEnd()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.LinArcFIRPlanningEnd();
                if (log != null)
                {
                    log.LogInfo($"LinArcFIRPlanningEnd:({rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
        * @brief 根据点位信息计算工具坐标系
        * @param [in] method 计算方法；0-四点法；1-六点法
        * @param [in] pos 关节位置组，四点法时数组长度为4个，六点法时数组长度为6个
        * @return 错误码
        */

        public int ComputeToolCoordWithPoints(int method, JointPos[] pos, ref DescPose coordRtn)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                double[] param0 = new double[6];
                double[] param1 = new double[6];
                double[] param2 = new double[6];
                double[] param3 = new double[6];
                double[] param4 = new double[6];
                double[] param5 = new double[6];

                // 填充前4行的数据
                for (int j = 0; j < 6; j++)
                {
                    param0[j] = pos[0].jPos[j];
                    param1[j] = pos[1].jPos[j];
                    param2[j] = pos[2].jPos[j];
                    param3[j] = pos[3].jPos[j];
                }

                // 根据 method 填充最后两行的数据
                if (method == 0)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        param4[j] = 0.0;
                        param5[j] = 0.0;
                    }
                }
                else if (method == 1)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        param4[j] = pos[4].jPos[j];
                        param5[j] = pos[5].jPos[j];
                    }
                }
                object[] result = proxy.ComputeToolCoordWithPoints(method, param0, param1, param2, param3, param4, param5);
                if ((int)result[0] == 0)
                {
                    coordRtn.tran.x = (double)result[1];
                    coordRtn.tran.y = (double)result[2];
                    coordRtn.tran.z = (double)result[3];
                    coordRtn.rpy.rx = (double)result[4];
                    coordRtn.rpy.ry = (double)result[5];
                    coordRtn.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"ComputeToolCoordWithPoints:({(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief 根据点位信息计算工件坐标系
        * @param [in] method 计算方法；0：原点-x轴-z轴  1：原点-x轴-xy平面
        * @param [in] pos 三个TCP位置组
        * @param [in] refFrame 参考坐标系
        * @return 错误码
        */
        public int ComputeWObjCoordWithPoints(int method, DescPose[] pos, int refFrame, ref DescPose coordRtn)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                double[] param0 = new double[6];
                double[] param1 = new double[6];
                double[] param2 = new double[6];
                param0[0] = pos[0].tran.x;
                param0[1] = pos[0].tran.y;
                param0[2] = pos[0].tran.z;
                param0[3] = pos[0].rpy.rx;
                param0[4] = pos[0].rpy.ry;
                param0[5] = pos[0].rpy.rz;

                // 填充 param1
                param1[0] = pos[1].tran.x;
                param1[1] = pos[1].tran.y;
                param1[2] = pos[1].tran.z;
                param1[3] = pos[1].rpy.rx;
                param1[4] = pos[1].rpy.ry;
                param1[5] = pos[1].rpy.rz;

                // 填充 param2
                param2[0] = pos[2].tran.x;
                param2[1] = pos[2].tran.y;
                param2[2] = pos[2].tran.z;
                param2[3] = pos[2].rpy.rx;
                param2[4] = pos[2].rpy.ry;
                param2[5] = pos[2].rpy.rz;
                object[] result = proxy.ComputeWObjCoordWithPoints(method, param0, param1, param2, refFrame);
                if ((int)result[0] == 0)
                {
                    coordRtn.tran.x = (double)result[1];
                    coordRtn.tran.y = (double)result[2];
                    coordRtn.tran.z = (double)result[3];
                    coordRtn.rpy.rx = (double)result[4];
                    coordRtn.rpy.ry = (double)result[5];
                    coordRtn.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"LinArcFIRPlanningEnd:({(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief 设置机器人焊接电弧意外中断检测参数
        * @param [in] checkEnable 是否使能检测；0-不使能；1-使能
        * @param [in] arcInterruptTimeLength 电弧中断确认时长(ms)
        * @return 错误码
        */
        public int WeldingSetCheckArcInterruptionParam(int checkEnable, int arcInterruptTimeLength)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.WeldingSetCheckArcInterruptionParam(checkEnable, arcInterruptTimeLength);
                if (log != null)
                {
                    log.LogInfo($"WeldingSetCheckArcInterruptionParam:({rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief 获取机器人焊接电弧意外中断检测参数
        * @param [out] checkEnable 是否使能检测；0-不使能；1-使能
        * @param [out] arcInterruptTimeLength 电弧中断确认时长(ms)
        * @return 错误码
        */
        public int WeldingGetCheckArcInterruptionParam(ref int checkEnable, ref int arcInterruptTimeLength)
        {

            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] rtn = proxy.WeldingGetCheckArcInterruptionParam();
                checkEnable = (int)rtn[1];
                arcInterruptTimeLength = (int)rtn[2];
                if (log != null)
                {
                    log.LogInfo($"WeldingGetCheckArcInterruptionParam:({rtn}");
                }
                return (int)rtn[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief 设置机器人焊接中断恢复参数
        * @param[in] enable 是否使能焊接中断恢复
        * @param[in] length 焊缝重叠距离(mm)
        * @param[in] velocity 机器人回到再起弧点速度百分比(0-100)
        * @param[in] moveType 机器人运动到再起弧点方式；0-LIN；1-PTP
        * @return 错误码
        */
        public int WeldingSetReWeldAfterBreakOffParam(int enable, double length, double velocity, int moveType)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int rtn = proxy.WeldingSetReWeldAfterBreakOffParam(enable, length, velocity, moveType);
                if (log != null)
                {
                    log.LogInfo($"WeldingSetReWeldAfterBreakOffParam:({rtn}");
                }
                return rtn;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief 获取机器人焊接中断恢复参数
        * @param [out] enable 是否使能焊接中断恢复
        * @param [out] length 焊缝重叠距离(mm)
        * @param [out] velocity 机器人回到再起弧点速度百分比(0-100)
        * @param [out] moveType 机器人运动到再起弧点方式；0-LIN；1-PTP
        * @return 错误码
        */
        public int WeldingGetReWeldAfterBreakOffParam(ref int enable, ref double length, ref double velocity, ref int moveType)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.WeldingGetReWeldAfterBreakOffParam();
                enable = (int)result[1];
                length = (double)result[2];
                velocity = (double)result[3];
                moveType = (int)result[4];
                if (log != null)
                {
                    log.LogInfo($"WeldingSetCheckArcInterruptionParam:({(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief 设置机器人焊接中断后恢复焊接
        * @return 错误码
        */
        public int WeldingStartReWeldAfterBreakOff()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int result = proxy.WeldingStartReWeldAfterBreakOff();
                if (log != null)
                {
                    log.LogInfo($"WeldingSetCheckArcInterruptionParam:({result}");
                }
                return result;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief 设置机器人焊接中断后退出焊接
        * @return 错误码
        */
        public int WeldingAbortWeldAfterBreakOff()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int result = proxy.WeldingAbortWeldAfterBreakOff();
                if (log != null)
                {
                    log.LogInfo($"WeldingSetCheckArcInterruptionParam:({result}");
                }
                return (int)result;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        public int LaserSensorRecord(int status, int delayMode, int delayTime, int delayDisExAxisNum, double delayDis, double sensitivePara, double speed)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int result = proxy.LaserSensorRecord(status, delayMode, delayTime, delayDisExAxisNum, delayDis, sensitivePara, speed);
                if (log != null)
                {
                    log.LogInfo($"WeldingSetCheckArcInterruptionParam:({result}");
                }
                return (int)result;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief  摆动渐变开始
        * @param [in] weaveChangeFlag 1-变摆动参数；2-变摆动参数+焊接速度
        * @param [in] weaveNum 摆动编号 
        * @param [in] velStart 焊接开始速度，(cm/min)
        * @param [in] velEnd 焊接结束速度，(cm/min)
        * @return  错误码
        */
        public int WeaveChangeStart(int weaveChangeFlag, int weaveNum, double velStart, double velEnd)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int result = proxy.WeaveChangeStart(weaveChangeFlag, weaveNum, velStart, velEnd);
                if (log != null)
                {
                    log.LogInfo($"WeldingSetCheckArcInterruptionParam:({result}");
                }
                return (int)result;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief  摆动渐变结束
        * @return  错误码
        * @version  3.7.9
        */
        public int WeaveChangeEnd()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int result = proxy.WeaveChangeEnd();
                if (log != null)
                {
                    log.LogInfo($"WeldingSetCheckArcInterruptionParam:({result}");
                }
                return (int)result;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief  轨迹预处理(轨迹前瞻)
        * @param  [in] name  轨迹文件名
        * @param  [in] mode 采样模式，0-不进行采样；1-等数据间隔采样；2-等误差限制采样
        * @param  [in] errorLim 误差限制，使用直线拟合生效
        * @param  [in] type 平滑方式，0-贝塞尔平滑
        * @param  [in] precision 平滑精度，使用贝塞尔平滑时生效
        * @param  [in] vamx 设定的最大速度，mm/s
        * @param  [in] amax 设定的最大加速度，mm/s2
        * @param  [in] jmax 设定的最大加加速度，mm/s3
        * @return  错误码     3.8.0
        */
        public int LoadTrajectoryLA(string name, int mode, double errorLim, int type, double precision, double vamx, double amax, double jmax)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int result = proxy.LoadTrajectoryLA(name, mode, errorLim, type, precision, vamx, amax, jmax);

                if (log != null)
                {
                    log.LogInfo($"LoadTrajectoryLA:({result}");
                }
                return (int)result;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }
        /**
        * @brief  轨迹复现(轨迹前瞻)
        * @return  错误码    3.8.0
        */
        public int MoveTrajectoryLA()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int result = proxy.MoveTrajectoryLA();

                if (log != null)
                {
                    log.LogInfo($"MoveTrajectoryLA:({result}");
                }
                return (int)result;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
        * @brief  自定义碰撞检测阈值功能开始，设置关节端和TCP端的碰撞检测阈值
        * @param  [in] flag 1-仅关节检测开启；2-仅TCP检测开启；3-关节和TCP检测同时开启
        * @param  [in] jointDetectionThreshould 关节碰撞检测阈值 j1-j6
        * @param  [in] tcpDetectionThreshould TCP碰撞检测阈值，xyzabc
        * @param  [in] block 0-非阻塞；1-阻塞
        * @return  错误码
        */
        public int CustomCollisionDetectionStart(int flag, double[] jointDetectionThreshould, double[] tcpDetectionThreshould, int block)
        {

            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }
            double[] param0 = new double[6];
            double[] param1 = new double[6];
            param0[0] = jointDetectionThreshould[0];
            param0[1] = jointDetectionThreshould[1];
            param0[2] = jointDetectionThreshould[2];
            param0[3] = jointDetectionThreshould[3];
            param0[4] = jointDetectionThreshould[4];
            param0[5] = jointDetectionThreshould[5];

            // 填充 param1
            param1[0] = tcpDetectionThreshould[0];
            param1[1] = tcpDetectionThreshould[1];
            param1[2] = tcpDetectionThreshould[2];
            param1[3] = tcpDetectionThreshould[3];
            param1[4] = tcpDetectionThreshould[4];
            param1[5] = tcpDetectionThreshould[5];

            try
            {
                int errcode = proxy.CustomCollisionDetectionStart(flag, param0, param1, block);

                if (log != null)
                {
                    log.LogInfo($"CustomCollisionDetectionEnd:({errcode}");
                }

                if ((robot_state_pkg.main_code != 0 || robot_state_pkg.sub_code != 0) && errcode == 0)
                {
                    errcode = 14;
                }

                return errcode;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }

        }

        /**
         * @brief  自定义碰撞检测阈值功能关闭
         * @return  错误码
         */
        public int CustomCollisionDetectionEnd()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }
            try
            {
                int result = proxy.CustomCollisionDetectionEnd();

                if (log != null)
                {
                    log.LogInfo($"CustomCollisionDetectionEnd:({result}");
                }
                return (int)result;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
      * @brief 加速度平滑开启
      * @param  [in] saveFlag 是否断电保存
      * @return  错误码
      */
        public int AccSmoothStart(bool saveFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }

            try
            {
                int param = saveFlag ? 1 : 0;
                int result = proxy.AccSmoothStart(param);

                if (log != null)
                {
                    log.LogInfo($"AccSmoothStart:({result}");
                }
                return (int)result;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }

        /**
         * @brief 加速度平滑关闭
         * @param  [in] saveFlag 是否断电保存
         * @return  错误码
         */
        public int AccSmoothEnd(bool saveFlag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }
            try
            {
                int param = saveFlag ? 1 : 0;
                int result = proxy.AccSmoothEnd(param);

                if (log != null)
                {
                    log.LogInfo($"AccSmoothEnd:({result}");
                }
                return (int)result;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_SUCCESS;
                }
            }
        }


        /**
        * @brief  控制器日志下载
        * @param [in] savePath 保存文件路径"D://zDown/"
        * @return  错误码
        */
        public int RbLogDownload(string savePath)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            int errcode = 0;
            try
            {
                errcode = proxy.RbLogDownloadPrepare();

                if (0 != errcode)
                {
                    log.LogInfo("RbLogDownloadPrepare fail.");

                    return errcode;
                }
                log.LogInfo("RbLogDownloadPrepare success.");

                string fileName = "rblog.tar.gz";
                errcode = FileDownLoad(1, fileName, savePath);
                return errcode;
            }
            catch
            {
                log.LogInfo("execute RbLogDownloadPrepare fail.");

                return (int)RobotError.ERR_SAVE_FILE_PATH_NOT_FOUND;
            }

        }

        /**
         * @brief 所有数据源下载
         * @param [in] savePath 保存文件路径"D://zDown/"
         * @return  错误码
         */
        public int AllDataSourceDownload(string savePath)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            int errcode = 0;
            try
            {
                errcode = proxy.AllDataSourceDownloadPrepare();

                if (0 != errcode)
                {
                    log.LogInfo("AllDataSourceDownloadPrepare fail.");

                    return errcode;
                }
                log.LogInfo("AllDataSourceDownloadPrepare success.");

                string fileName = "alldatasource.tar.gz";
                errcode = FileDownLoad(2, fileName, savePath);
                return errcode;
            }
            catch
            {

                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_RPC_ERROR;
                }
            }

        }

        /**
         * @brief 数据备份包下载
         * @param [in] savePath 保存文件路径"D://zDown/"
         * @return  错误码
         */
        public int DataPackageDownload(string savePath)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            int errcode = 0;
            try
            {
                errcode = proxy.DataPackageDownloadPrepare();

                if (0 != errcode)
                {
                    log.LogInfo("DataPackageDownloadPrepare fail.");

                    return errcode;
                }
                log.LogInfo("DataPackageDownloadPrepare success.");

                string fileName = "fr_user_data.tar.gz";
                errcode = FileDownLoad(3, fileName, savePath);
                return errcode;
            }
            catch
            {
                log.LogInfo("execute DataPackageDownloadPrepare fail.");

                return (int)RobotError.ERR_OTHER;
            }

        }

        /**
         * @brief 获取控制箱SN码
         * @param [out] SNCode 控制箱SN码
         * @return 错误码
         */
        public int GetRobotSN(ref string SNCode)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            int errcode = 0;
            try
            {
                object[] result = proxy.GetRobotSN();

                errcode = (int)result[0];

                if (0 == errcode)
                {
                    SNCode = (string)result[1];
                }
                else
                {
                    log.LogInfo($"GetRobotSN fail, errcode is: {errcode}");
                }
                return errcode;
            }
            catch
            {
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
         * @brief 关闭机器人操作系统
         * @return 错误码
         */
        public int ShutDownRobotOS()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                // 调用远程方法 "ShutDownRobotOS"
                int errcode = proxy.ShutDownRobotOS();

                // 返回错误码
                return errcode;
            }
            catch (Exception ex)
            {
                // 捕获异常，记录日志（可选）
                // 返回预定义的错误码
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
        * @brief 传送带通讯输入检测
        * @param [in] timeout 等待超时时间ms
        * @return 错误码
        */
        public int ConveryComDetect(int timeout)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }
            int errcode = 0;
            try
            {
                errcode = proxy.ConveryComDetect(timeout);


                return errcode;
            }
            catch
            {
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
         * @brief 传送带通讯输入检测触发
         * @return 错误码
         */
        public int ConveyorComDetectTrigger()
        {
            // 1. 检查Socket通信错误
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            int errcode = 0;

            // 2. 替换C++的静态计数器（C# 没有局部静态变量，改用类级别字段）
            if (_conveyorCounter == null)
            {
                _conveyorCounter = 0;
            }

            // 3. 直接赋值字符串（不再需要清空缓冲区）
            g_sendbuf = $"/f/bIII{_conveyorCounter}III1149III25IIIConveryComDetectTrigger()III/b/f";

            // 4. 计数器递增
            _conveyorCounter++;
            is_sendcmd = true;

            // 5. 记录日志
            log.LogInfo("ConveryComDetectTrigger().");

            // 6. 返回错误码（原C++代码缺少返回值，这里返回errcode）
            return errcode;
        }
        /**
      * @brief 电弧跟踪焊机电流反馈AI通道选择
      * @param [in]  channel 通道；0-扩展AI0；1-扩展AI1；2-扩展AI2；3-扩展AI3；4-控制箱AI0；5-控制箱AI1
      * @return 错误码
      */
        public int ArcWeldTraceAIChannelCurrent(int channel)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }
            int errcode = 0;
            try
            {
                errcode = proxy.ArcWeldTraceAIChannelCurrent(channel);


                return errcode;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_RPC_ERROR;
                }
            }
        }

        /**
         * @brief 电弧跟踪焊机电压反馈AI通道选择
         * @param [in]  channel 通道；0-扩展AI0；1-扩展AI1；2-扩展AI2；3-扩展AI3；4-控制箱AI0；5-控制箱AI1
         * @return 错误码
         */
        public int ArcWeldTraceAIChannelVoltage(int channel)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }
            int errcode = 0;
            try
            {
                errcode = proxy.ArcWeldTraceAIChannelVoltage(channel);


                return errcode;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_RPC_ERROR;
                }
            }
        }

        /**
         * @brief 电弧跟踪焊机电流反馈转换参数
         * @param [in] AILow AI通道下限，默认值0V，范围[0-10V]
         * @param [in] AIHigh AI通道上限，默认值10V，范围[0-10V]
         * @param [in] currentLow AI通道下限对应焊机电流值，默认值0V，范围[0-200V]
         * @param [in] currentHigh AI通道上限对应焊机电流值，默认值100V，范围[0-200V]
         * @return 错误码
         */
        public int ArcWeldTraceCurrentPara(float AILow, float AIHigh, float currentLow, float currentHigh)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }
            int errcode = 0;
            try
            {
                errcode = proxy.ArcWeldTraceCurrentPara(AILow, AIHigh, currentLow, currentHigh);


                return errcode;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    Console.WriteLine("wertyuio");
                    return (int)RobotError.ERR_RPC_ERROR;
                }
            }
        }

        /**
         * @brief 电弧跟踪焊机电压反馈转换参数
         * @param [in] AILow AI通道下限，默认值0V，范围[0-10V]
         * @param [in] AIHigh AI通道上限，默认值10V，范围[0-10V]
         * @param [in] voltageLow AI通道下限对应焊机电压值，默认值0V，范围[0-200V]
         * @param [in] voltageHigh AI通道上限对应焊机电压值，默认值100V，范围[0-200V]
         * @return 错误码
         */
        public int ArcWeldTraceVoltagePara(float AILow, float AIHigh, float voltageLow, float voltageHigh)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }
            int errcode = 0;
            try
            {
                errcode = proxy.ArcWeldTraceVoltagePara(AILow, AIHigh, voltageLow, voltageHigh);


                return errcode;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_RPC_ERROR;
                }
            }
        }
        /**
 * @brief 设置焊接电压渐变开始
 * @param [in] IOType 控制类型；0-控制箱IO；1-数字通信协议(UDP);2-数字通信协议(ModbusTCP)
 * @param [in] voltageStart 起始焊接电压(V)
 * @param [in] voltageEnd 终止焊接电压(V)
 * @param [in] AOIndex 控制箱AO端口号(0-1)
 * @param [in] blend 是否平滑 0-不平滑；1-平滑
 * @return 错误码
 */
        public int WeldingSetVoltageGradualChangeStart(int IOType, double voltageStart, double voltageEnd, int AOIndex, int blend)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }
            int errcode = 0;
            try
            {
                errcode = proxy.WeldingSetVoltageGradualChangeStart(IOType, voltageStart, voltageEnd, AOIndex, blend);


                return errcode;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_RPC_ERROR;
                }
            }

        }

        /**
         * @brief 设置焊接电压渐变结束
         * @return 错误码
         */
        public int WeldingSetVoltageGradualChangeEnd()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }
            int errcode = 0;
            try
            {
                errcode = proxy.WeldingSetVoltageGradualChangeEnd();


                return errcode;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_RPC_ERROR;
                }
            }
        }

        /**
         * @brief 设置焊接电流渐变开始
         * @param [in] IOType 控制类型；0-控制箱IO；1-数字通信协议(UDP);2-数字通信协议(ModbusTCP)
         * @param [in] voltageStart 起始焊接电流(A)
         * @param [in] voltageEnd 终止焊接电流(A)
         * @param [in] AOIndex 控制箱AO端口号(0-1)
         * @param [in] blend 是否平滑 0-不平滑；1-平滑
         * @return 错误码
         */
        public int WeldingSetCurrentGradualChangeStart(int IOType, double currentStart, double currentEnd, int AOIndex, int blend)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }
            int errcode = 0;
            try
            {
                errcode = proxy.WeldingSetCurrentGradualChangeStart(IOType, currentStart, currentEnd, AOIndex, blend);


                return errcode;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_RPC_ERROR;
                }
            }

        }

        /**
         * @brief 设置焊接电流渐变结束
         * @return 错误码
         */
        public int WeldingSetCurrentGradualChangeEnd()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }
            int errcode = 0;
            try
            {
                errcode = proxy.WeldingSetCurrentGradualChangeEnd();


                return errcode;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_RPC_ERROR;
                }
            }
        }

        /**
        * @brief 获取SmartTool按钮状态
        * @param [out] state SmartTool手柄按钮状态;(bit0:0-通信正常；1-通信掉线；bit1-撤销操作；bit2-清空程序；
        bit3-A键；bit4-B键；bit5-C键；bit6-D键；bit7-E键；bit8-IO键；bit9-手自动；bit10开始)
        * @return 错误码
        */
        public int GetSmarttoolBtnState(ref int state)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            state = robot_state_pkg.smartToolState;

            return 0;
        }

        /**
        * @brief 获取扩展轴坐标系
        * @param [out] coord 扩展轴坐标系
        * @return 错误码
        */
        public int ExtAxisGetCoord(ref DescPose coord)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                object[] result = proxy.ExtAxisGetCoord();
                if ((int)result[0] == 0)
                {
                    coord.tran.x = (double)result[1];
                    coord.tran.y = (double)result[2];
                    coord.tran.z = (double)result[3];
                    coord.rpy.rx = (double)result[4];
                    coord.rpy.ry = (double)result[5];
                    coord.rpy.rz = (double)result[6];
                }
                if (log != null)
                {
                    log.LogInfo($"ExtAxisGetCoord(ref {coord.tran.x},ref {coord.tran.y},ref {coord.tran.z},ref {coord.rpy.rx},ref {coord.rpy.ry},ref {coord.rpy.rz}) : {(int)result[0]}");
                }
                return (int)result[0];
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_RPC_ERROR;
                }
            }
        }
        /**
        * @brief 下发SCP指令
        * @param [in] mode 0-上传（上位机->控制器），1-下载（控制器->上位机）
        * @param [in] sshname 上位机用户名
        * @param [in] sship 上位机ip地址
        * @param [in] usr_file_url 上位机文件路径
        * @param [in] robot_file_url 机器人控制器文件路径
        * @return 错误码
        */
        public int SetSSHScpCmd(int mode, string sshname, string sship, string usr_file_url, string robot_file_url)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }
            try
            {
                int errcode = proxy.SetSSHScpCmd(mode, sshname, sship, usr_file_url, robot_file_url);


                return errcode;
            }
            catch
            {
                if (IsSockComError())
                {
                    if (log != null)
                    {
                        log.LogError($"RPC exception");
                    }
                    return g_sock_com_err;
                }
                else
                {
                    return (int)RobotError.ERR_RPC_ERROR;
                }
            }
        }



        //        /**
        //        * @brief  螺旋线探索
        //        * @param  [in] rcs 参考坐标系，0-工具坐标系，1-基坐标系
        //        * @param  [in] dr 每圈半径进给量
        //        * @param  [in] ft 力/扭矩阈值，fx,fy,fz,tx,ty,tz，范围[0~100]
        //        * @param  [in] max_t_ms 最大探索时间，单位ms
        //        * @param  [in] max_vel 最大线速度，单位mm/s
        //        * @return  错误码
        //        */
        //        public int FT_SpiralSearch(int rcs, float dr, float ft, float max_t_ms, float max_vel)
        //        {
        //            if (IsSockComError())
        //            {
        //                return g_sock_com_err;
        //            }
        //            int errcode = 0;
        //            try
        //            {
        //                errcode = proxy.FT_SpiralSearch(rcs,  dr,  ft,  max_t_ms,  max_vel);

        //                if (log != null)
        //                {
        //                    log.LogInfo($"FT_SpiralSearch(ref {rcs},ref {dr},ref {ft},ref {max_t_ms},ref {max_vel}: {errcode}");
        //                }
        //                return errcode;
        //            }
        //            catch
        //            {
        //                if (IsSockComError())
        //                {
        //                    if (log != null)
        //                    {
        //                        log.LogError($"RPC exception");
        //                    }
        //                    return g_sock_com_err;
        //                }
        //                else
        //                {
        //                    return (int)RobotError.ERR_RPC_ERROR;
        //                }
        //            }
        //        }
        //        /**
        //        * @brief  旋转插入
        //        * @param  [in] rcs 参考坐标系，0-工具坐标系，1-基坐标系
        //        * @param  [in] angVelRot 旋转角速度，单位deg/s
        //        * @param  [in] ft  力/扭矩阈值，fx,fy,fz,tx,ty,tz，范围[0~100]
        //        * @param  [in] max_angle 最大旋转角度，单位deg
        //        * @param  [in] orn 力/扭矩方向，1-沿z轴方向，2-绕z轴方向
        //        * @param  [in] max_angAcc 最大旋转加速度，单位deg/s^2，暂不使用，默认为0
        //        * @param  [in] rotorn  旋转方向，1-顺时针，2-逆时针
        //        * @return  错误码
        //*/
        //        public int FT_RotInsertion(int rcs, float angVelRot, float ft, float max_angle, uint8_t orn, float max_angAcc, uint8_t rotorn)
        //        {
        //            if (IsSockComError())
        //            {
        //                return g_sock_com_err;
        //            }
        //            int errcode = 0;
        //            try
        //            {
        //                errcode = proxy.FT_SpiralSearch(rcs, dr, ft, max_t_ms, max_vel);

        //                if (log != null)
        //                {
        //                    log.LogInfo($"FT_SpiralSearch(ref {rcs},ref {dr},ref {ft},ref {max_t_ms},ref {max_vel}: {errcode}");
        //                }
        //                return errcode;
        //            }
        //            catch
        //            {
        //                if (IsSockComError())
        //                {
        //                    if (log != null)
        //                    {
        //                        log.LogError($"RPC exception");
        //                    }
        //                    return g_sock_com_err;
        //                }
        //                else
        //                {
        //                    return (int)RobotError.ERR_RPC_ERROR;
        //                }
        //            }
        //        }
        //        /**
        //        * @brief  直线插入
        //        * @param  [in] rcs 参考坐标系，0-工具坐标系，1-基坐标系
        //        * @param  [in] ft  力/扭矩阈值，fx,fy,fz,tx,ty,tz，范围[0~100]
        //        * @param  [in] lin_v 直线速度，单位mm/s
        //        * @param  [in] lin_a 直线加速度，单位mm/s^2，暂不使用
        //        * @param  [in] max_dis 最大插入距离，单位mm
        //        * @param  [in] linorn  插入方向，0-负方向，1-正方向
        //        * @return  错误码
        //        */
        //        public int FT_LinInsertion(int rcs, float ft, float lin_v, float lin_a, float max_dis, uint8_t linorn)
        //        {
        //            if (IsSockComError())
        //            {
        //                return g_sock_com_err;
        //            }
        //            int errcode = 0;
        //            try
        //            {
        //                errcode = proxy.FT_SpiralSearch(rcs, dr, ft, max_t_ms, max_vel);

        //                if (log != null)
        //                {
        //                    log.LogInfo($"FT_SpiralSearch(ref {rcs},ref {dr},ref {ft},ref {max_t_ms},ref {max_vel}: {errcode}");
        //                }
        //                return errcode;
        //            }
        //            catch
        //            {
        //                if (IsSockComError())
        //                {
        //                    if (log != null)
        //                    {
        //                        log.LogError($"RPC exception");
        //                    }
        //                    return g_sock_com_err;
        //                }
        //                else
        //                {
        //                    return (int)RobotError.ERR_RPC_ERROR;
        //                }
        //            }
        //        }


        //        /**
        //        * @brief  表面定位
        //        * @param  [in] rcs 参考坐标系，0-工具坐标系，1-基坐标系
        //        * @param  [in] dir  移动方向，1-正方向，2-负方向
        //        * @param  [in] axis 移动轴，1-x轴，2-y轴，3-z轴
        //        * @param  [in] lin_v 探索直线速度，单位mm/s
        //        * @param  [in] lin_a 探索直线加速度，单位mm/s^2，暂不使用，默认为0
        //        * @param  [in] max_dis 最大探索距离，单位mm
        //        * @param  [in] ft  动作终止力/扭矩阈值，fx,fy,fz,tx,ty,tz
        //        * @return  错误码
        //*/
        //        public int FT_FindSurface(int rcs, uint8_t dir, uint8_t axis, float lin_v, float lin_a, float max_dis, float ft)
        //        {
        //            if (IsSockComError())
        //            {
        //                return g_sock_com_err;
        //            }
        //            int errcode = 0;
        //            try
        //            {
        //                errcode = proxy.FT_SpiralSearch(rcs, dr, ft, max_t_ms, max_vel);

        //                if (log != null)
        //                {
        //                    log.LogInfo($"FT_SpiralSearch(ref {rcs},ref {dr},ref {ft},ref {max_t_ms},ref {max_vel}: {errcode}");
        //                }
        //                return errcode;
        //            }
        //            catch
        //            {
        //                if (IsSockComError())
        //                {
        //                    if (log != null)
        //                    {
        //                        log.LogError($"RPC exception");
        //                    }
        //                    return g_sock_com_err;
        //                }
        //                else
        //                {
        //                    return (int)RobotError.ERR_RPC_ERROR;
        //                }
        //            }
        //        }


        //        /**
        //        * @brief  计算中间平面位置开始
        //        * @return  错误码
        //        */
        //        public int FT_CalCenterStart()
        //        {
        //            if (IsSockComError())
        //            {
        //                return g_sock_com_err;
        //            }
        //            int errcode = 0;
        //            try
        //            {
        //                errcode = proxy.FT_SpiralSearch(rcs, dr, ft, max_t_ms, max_vel);

        //                if (log != null)
        //                {
        //                    log.LogInfo($"FT_SpiralSearch(ref {rcs},ref {dr},ref {ft},ref {max_t_ms},ref {max_vel}: {errcode}");
        //                }
        //                return errcode;
        //            }
        //            catch
        //            {
        //                if (IsSockComError())
        //                {
        //                    if (log != null)
        //                    {
        //                        log.LogError($"RPC exception");
        //                    }
        //                    return g_sock_com_err;
        //                }
        //                else
        //                {
        //                    return (int)RobotError.ERR_RPC_ERROR;
        //                }
        //            }
        //        }
        //        /**
        //        * @brief  计算中间平面位置结束
        //        * @param  [out] pos 中间平面位姿
        //        * @return  错误码
        //        */
        //        public int FT_CalCenterEnd(DescPose* pos)
        //        {
        //            if (IsSockComError())
        //            {
        //                return g_sock_com_err;
        //            }
        //            int errcode = 0;
        //            try
        //            {
        //                errcode = proxy.FT_SpiralSearch(rcs, dr, ft, max_t_ms, max_vel);

        //                if (log != null)
        //                {
        //                    log.LogInfo($"FT_SpiralSearch(ref {rcs},ref {dr},ref {ft},ref {max_t_ms},ref {max_vel}: {errcode}");
        //                }
        //                return errcode;
        //            }
        //            catch
        //            {
        //                if (IsSockComError())
        //                {
        //                    if (log != null)
        //                    {
        //                        log.LogError($"RPC exception");
        //                    }
        //                    return g_sock_com_err;
        //                }
        //                else
        //                {
        //                    return (int)RobotError.ERR_RPC_ERROR;
        //                }
        //            }
        //        }
    }
}


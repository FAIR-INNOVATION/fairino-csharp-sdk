using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace TestPortReader
{
    #region 20004端口数据结构 (ROBOT_STATE_PKG)
    // 以下结构体与 fairino.RobotTypes 中的定义保持一致

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WELDING_BREAKOFF_STATE
    {
        public byte breakOffState;
        public byte weldArcState;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ROBOT_AUX_STATE
    {
        public byte servoId;
        public int servoErrCode;
        public int servoState;
        public double servoPos;
        public float servoVel;
        public float servoTorque;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EXT_AXIS_STATUS
    {
        public double pos;
        public double vel;
        public int errorCode;
        public byte ready;
        public byte inPos;
        public byte alarm;
        public byte flerr;
        public byte nlimit;
        public byte pLimit;
        public byte mdbsOffLine;
        public byte mdbsTimeout;
        public byte homingStatus;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ROBOT_TIME
    {
        public UInt16 year;
        public byte mouth;
        public byte day;
        public byte hour;
        public byte minute;
        public byte second;
        public UInt16 millisecond;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ROBOT_STATE_PKG
    {
        public UInt16 frame_head;
        public byte frame_cnt;
        public UInt16 data_len;
        public byte program_state;
        public byte robot_state;
        public int main_code;
        public int sub_code;
        public byte robot_mode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] jt_cur_pos;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] tl_cur_pos;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] flange_cur_pos;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] actual_qd;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] actual_qdd;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] target_TCP_CmpSpeed;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] target_TCP_Speed;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] actual_TCP_CmpSpeed;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] actual_TCP_Speed;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] jt_cur_tor;

        public int tool;
        public int user;
        public byte cl_dgt_output_h;
        public byte cl_dgt_output_l;
        public byte tl_dgt_output_l;
        public byte cl_dgt_input_h;
        public byte cl_dgt_input_l;
        public byte tl_dgt_input_l;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public UInt16[] cl_analog_input;

        public UInt16 tl_anglog_input;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] ft_sensor_raw_data;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] ft_sensor_data;

        public byte ft_sensor_active;
        public byte EmergencyStop;
        public int motion_done;
        public byte gripper_motiondone;
        public int mc_queue_len;
        public byte collisionState;
        public int trajectory_pnum;
        public byte safety_stop0_state;
        public byte safety_stop1_state;
        public byte gripper_fault_id;
        public UInt16 gripper_fault;
        public UInt16 gripper_active;
        public byte gripper_position;
        public byte gripper_speed;
        public byte gripper_current;
        public int gripper_tmp;
        public int gripper_voltage;

        public ROBOT_AUX_STATE auxState;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public EXT_AXIS_STATUS[] extAxisStatus;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public UInt16[] extDIState;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public UInt16[] extDOState;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt16[] extAIState;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt16[] extAOState;

        public int rbtEnableState;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] jointDriverTorque;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] jointDriverTemperature;

        public ROBOT_TIME robotTime;
        public int softwareUpgradeState;
        public UInt16 endLuaErrCode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public UInt16[] cl_analog_output;

        public UInt16 tl_analog_output;
        public float gripperRotNum;
        public byte gripperRotSpeed;
        public byte gripperRotTorque;

        public WELDING_BREAKOFF_STATE weldingBreakOffState;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] jt_tgt_tor;

        public int smartToolState;
        public float wideVoltageCtrlBoxTemp;
        public UInt16 wideVoltageCtrlBoxFanVel;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] toolCoord;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] wobjCoord;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] extoolCoord;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] exAxisCoord;

        public double load;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] loadCog;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] lastServoTarget;

        public int servoJCmdNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] targetJointPos;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] targetJointVel;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] targetJointAcc;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] targetJointCurrent;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] actualJointCurrent;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] actualTCPForce;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] targetTCPPos;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] collisionLevel;

        public double speedScaleManual;
        public double speedScaleAuto;
        public int luaLineNum;
        public byte abnomalStop;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] currentLuaFileName;

        public byte programTotalLine;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] safetyBoxSingal;

        public double weldVoltage;
        public double weldCurrent;
        public double weldTrackVel;
        public byte tpdException;
        public byte alarmRebootRobot;
        public byte modbusMasterConnect;
        public byte modbusSlaveConnect;
        public byte btnBoxStopSignal;
        public byte dragAlarm;
        public byte safetyDoorAlarm;
        public byte safetyPlaneAlarm;
        public byte motonAlarm;
        public byte interfaceAlarm;
        public int udpCmdState;
        public byte weldReadyState;
        public byte alarmCheckEmergStopBtn;
        public byte tsTmCmdComError;
        public byte tsTmStateComError;
        public int ctrlBoxError;
        public byte safetyDataState;
        public byte forceSensorErrState;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] ctrlOpenLuaErrCode;

        public byte strangePosFlag;
        public byte alarm;
        public byte driverAlarm;
        public byte aliveSlaveNumError;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] slaveComError;

        public byte cmdPointError;
        public byte IOError;
        public byte gripperError;
        public byte fileError;
        public byte paraError;
        public byte exaxisOutLimitError;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] driverComError;

        public byte driverError;
        public byte outSoftLimitError;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 130)]
        public byte[] axleGenComData;

        public byte socketConnTimeout;
        public byte socketReadTimeout;
        public byte tsWebStateComErr;
        public byte exaxisCoordID;

        public UInt16 check_sum;
    }
    #endregion

    #region 8083端口数据结构 (基于test.txt Table 2-2)
    // 根据test.txt中的表2-2定义，使用Pack=1保持与通信协议一致
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ROBOT_STATE_8083_PKG
    {
        public byte program_state;          // 1  程序运行状态
        public byte error_code;            // 2  故障码
        public byte robot_mode;            // 3  机器人模式

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] jt_cur_pos;         // 4-9  关节当前位置

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] tl_cur_pos;         // 10-15 工具当前位置

        public int toolNum;                 // 16 工具号

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] jt_cur_tor;         // 17-22 关节当前扭矩

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] program_name;         // 23 运行程序名

        public byte prog_total_line;        // 24 运行程序总行数
        public byte prog_cur_line;          // 25 运行程序当前行
        public byte cl_dgt_output_h;        // 26 控制箱数字量IO输出15-8
        public byte cl_dgt_output_l;        // 27 控制箱数字量IO输出7-0
        public byte tl_dgt_output_l;        // 28 工具数字量IO输出7-0
        public byte cl_dgt_input_h;         // 29 控制箱数字量IO输入15-8
        public byte cl_dgt_input_l;         // 30 控制箱数字量IO输入7-0
        public byte tl_dgt_input_l;         // 31 工具数字量IO输入7-0

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] FT_data;            // 32-37 力/扭矩传感器数据

        public byte FT_ActStatus;           // 38 力/扭矩传感器激活状态
        public byte EmergencyStop;          // 39 急停标志
        public int robot_motion_done;       // 40 机器人运动到位信号
        public byte gripper_motion_done;    // 41 夹爪运动到位信号
        public byte servo_id;               // 42 外部伺服驱动器id
        public int servo_errcode;           // 43 外部伺服驱动器故障码
        public int servo_state;             // 44 外部伺服驱动器状态
        public double servo_actual_pos;     // 45 外部伺服当前位置
        public float servo_actual_speed;    // 46 外部伺服当前速度
        public float servo_actual_torque;   // 47 外部伺服当前转矩
        public byte exaxis_out_slimit_error;// 48 外部轴超出软限位错误

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public EXT_AXIS_STATUS[] exaxis_status;// 49 外部轴(UDP)状态

        public byte exaxis_active_flag;     // 50 外部轴激活标志
        public byte exaxis_motion_status;   // 51 外部轴运动状态

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public UInt16[] cl_analog_input;    // 52 控制箱模拟量输入

        public UInt16 tl_analog_input;      // 53 末端模拟量输入

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public UInt16[] cl_analog_output;   // 54 控制箱模拟量输出

        public UInt16 tl_analog_output;     // 55 末端模拟量输出
        public byte gripper_fault_id;       // 56 错误夹爪号
        public UInt16 gripper_fault;        // 57 夹爪故障
        public UInt16 gripper_active;       // 58 夹爪激活状态
        public byte gripper_position;       // 59 夹爪位置
        public SByte gripper_speed;          // 60 夹爪速度
        public SByte gripper_current;        // 61 夹爪电流
        public int gripper_temp;            // 62 夹爪温度
        public int gripper_voltage;         // 63 夹爪电压
        public float gripper_rotNum;        // 64 旋转夹爪当前圈数
        public byte gripper_rotSpeed;       // 65 旋转夹爪当前速度
        public byte gripper_rotTorque;      // 66 旋转夹爪当前力矩
        public int main_errcode;            // 67 主故障码
        public int sub_errcode;             // 68 子故障码

        public UInt16 welding_state;        // 69 焊接状态
        public int smartToolState;          // 70 SmartTool按钮状态

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] toolCoord;          // 71 工具坐标系

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] wobjCoord;          // 72 工件坐标系

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] exToolCoord;        // 73 外部工具坐标系

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] exAxisCoord;        // 74 扩展轴坐标系

        public double load;                 // 75 负载质量

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] loadCog;            // 76 负载质心
    }
    #endregion

    class Program
    {
        private static readonly int PORT_20004 = 20004;
        private static readonly int PORT_8083 = 8083;
        private const int BUFFER_SIZE = 1024 * 64;

        // 默认机器人IP地址
        private static string robotIp = "192.168.58.2";

        static void Main(string[] args)
        {
            Console.WriteLine("============================================================");
            Console.WriteLine("  Fairino Robot Port Reader - 端口数据读取测试程序");
            Console.WriteLine("  读取20004端口和8083端口的main_code/sub_code");
            Console.WriteLine("============================================================");

            if (args.Length >= 1)
            {
                robotIp = args[0];
            }
            Console.WriteLine($"  机器人IP地址: {robotIp}");
            Console.WriteLine("============================================================");
            Console.WriteLine();

            // 启动两个线程分别读取两个端口
            Thread thread20004 = new Thread(() => ReadPort20004());
            Thread thread8083 = new Thread(() => ReadPort8083());

            thread20004.IsBackground = true;
            thread8083.IsBackground = true;

            thread20004.Start();
            // 稍等一会儿再启动8083线程，让20004先建立连接
            Thread.Sleep(500);
            thread8083.Start();

            // 主线程等待
            Console.WriteLine("按 Ctrl+C 或任意键退出程序...");
            Console.ReadKey();
        }

        /// <summary>
        /// 读取20004端口数据 (ROBOT_STATE_PKG)
        /// 与SDK中RecvPkg相同的逻辑：根据帧头动态确定帧大小
        /// </summary>
        static void ReadPort20004()
        {
            int pkgSize = Marshal.SizeOf(typeof(ROBOT_STATE_PKG));
            Console.WriteLine($"[20004] ROBOT_STATE_PKG 结构体大小: {pkgSize} bytes");
            Console.WriteLine($"[20004] main_code 在结构体中的偏移: {Marshal.OffsetOf(typeof(ROBOT_STATE_PKG), "main_code")}");
            Console.WriteLine($"[20004] sub_code 在结构体中的偏移: {Marshal.OffsetOf(typeof(ROBOT_STATE_PKG), "sub_code")}");

            while (true)
            {
                Socket socket = null;
                try
                {
                    socket = ConnectToPort(PORT_20004);
                    if (socket == null)
                    {
                        Thread.Sleep(3000);
                        continue;
                    }

                    Console.WriteLine($"[20004] 连接成功，开始接收数据...");

                    // 持续接收数据
                    while (true)
                    {
                        // 使用动态帧大小解析（与SDK中RecvPkg逻辑一致）
                        byte[] frameData = ReceiveFrameDynamic(socket, pkgSize);
                        if (frameData == null)
                        {
                            Console.WriteLine($"[20004] 接收帧失败，尝试重新连接...");
                            break;
                        }

                        int frameSize = frameData.Length;
                        // 将帧数据转换为结构体
                        IntPtr ptr = Marshal.AllocHGlobal(frameSize);
                        try
                        {
                            Marshal.Copy(frameData, 0, ptr, frameSize);
                            ROBOT_STATE_PKG pkg = (ROBOT_STATE_PKG)Marshal.PtrToStructure(ptr, typeof(ROBOT_STATE_PKG));

                            Console.WriteLine($"[20004] main_code = {pkg.main_code}, sub_code = {pkg.sub_code}  " +
                                $"(program_state={pkg.program_state}, robot_state={pkg.robot_state}, " +
                                $"robot_mode={pkg.robot_mode})");
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(ptr);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[20004] 异常: {ex.Message}");
                }
                finally
                {
                    CloseSocket(socket);
                }

                Console.WriteLine($"[20004] 3秒后重试连接...");
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// 读取8083端口数据 (Table 2-2 结构)
        /// 自动检测是否使用0x5A5A帧协议，如果不使用则按原始二进制数据读取
        /// </summary>
        static void ReadPort8083()
        {
            int pkgSize = Marshal.SizeOf(typeof(ROBOT_STATE_8083_PKG));
            Console.WriteLine($"[8083] ROBOT_STATE_8083_PKG 结构体大小: {pkgSize} bytes");
            Console.WriteLine($"[8083] main_errcode 偏移: {Marshal.OffsetOf(typeof(ROBOT_STATE_8083_PKG), "main_errcode")}");
            Console.WriteLine($"[8083] sub_errcode 偏移: {Marshal.OffsetOf(typeof(ROBOT_STATE_8083_PKG), "sub_errcode")}");

            bool useFrameProtocol = true;  // 默认尝试帧协议

            while (true)
            {
                Socket socket = null;
                try
                {
                    socket = ConnectToPort(PORT_8083);
                    if (socket == null)
                    {
                        Thread.Sleep(3000);
                        continue;
                    }

                    Console.WriteLine($"[8083] 连接成功，开始接收数据...");
                    useFrameProtocol = true;  // 每次重连后先尝试帧协议

                    while (true)
                    {
                        byte[] payload;

                        if (useFrameProtocol)
                        {
                            // 先尝试读取5字节，检测是否为0x5A5A帧头
                            byte[] header = new byte[5];
                            if (!ReceiveAll(socket, header, 0, 5))
                            {
                                Console.WriteLine($"[8083] 读取帧头失败，尝试重新连接...");
                                break;
                            }

                            if (header[0] == 0x5A && header[1] == 0x5A)
                            {
                                // 确认使用帧协议，读取剩余数据
                                UInt16 dataLen = (UInt16)(header[3] | (header[4] << 8));
                                int totalFrameSize = dataLen + 7;

                                if (totalFrameSize > BUFFER_SIZE)
                                {
                                    Console.WriteLine($"[8083] 帧大小({totalFrameSize})超过缓冲区");
                                    continue;
                                }

                                byte[] frameData = new byte[totalFrameSize];
                                Array.Copy(header, 0, frameData, 0, 5);

                                if (!ReceiveAll(socket, frameData, 5, totalFrameSize - 5))
                                {
                                    Console.WriteLine($"[8083] 读取帧数据失败，尝试重新连接...");
                                    break;
                                }

                                // 验证校验和
                                UInt16 checksum = 0;
                                for (int i = 0; i < totalFrameSize - 2; i++)
                                    checksum += frameData[i];
                                UInt16 receivedChecksum = (UInt16)(frameData[totalFrameSize - 2] |
                                    (frameData[totalFrameSize - 1] << 8));

                                if (checksum != receivedChecksum)
                                {
                                    Console.WriteLine($"[8083] 校验和不匹配: 计算={checksum}, 接收={receivedChecksum}");
                                    continue;
                                }

                                // 提取payload (跳过5字节帧头)
                                payload = new byte[dataLen];
                                Array.Copy(frameData, 5, payload, 0, dataLen);
                            }
                            else
                            {
                                // 不是0x5A5A帧协议，切换为原始数据模式
                                Console.WriteLine($"[8083] 未检测到0x5A5A帧头 (收到0x{header[0]:X2}{header[1]:X2})，切换为原始二进制读取模式");
                                useFrameProtocol = false;

                                // 这5字节是payload的开始，继续读取剩余struct字节
                                payload = new byte[pkgSize];
                                Array.Copy(header, 0, payload, 0, 5);

                                if (!ReceiveAll(socket, payload, 5, pkgSize - 5))
                                {
                                    Console.WriteLine($"[8083] 读取原始数据失败，尝试重新连接...");
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // 原始二进制模式：直接读取pkgSize字节
                            payload = new byte[pkgSize];
                            if (!ReceiveAll(socket, payload, 0, pkgSize))
                            {
                                Console.WriteLine($"[8083] 读取原始数据失败，尝试重新连接...");
                                break;
                            }
                        }

                        // 解析payload为结构体
                        if (payload.Length >= pkgSize)
                        {
                            IntPtr ptr = Marshal.AllocHGlobal(pkgSize);
                            try
                            {
                                Marshal.Copy(payload, 0, ptr, pkgSize);
                                ROBOT_STATE_8083_PKG pkg = (ROBOT_STATE_8083_PKG)Marshal.PtrToStructure(
                                    ptr, typeof(ROBOT_STATE_8083_PKG));

                                Console.WriteLine($"[8083] main_errcode = {pkg.main_errcode}, sub_errcode = {pkg.sub_errcode}  " +
                                    $"(program_state={pkg.program_state}, error_code={pkg.error_code}, " +
                                    $"robot_mode={pkg.robot_mode}, EmergencyStop={pkg.EmergencyStop})");
                            }
                            finally
                            {
                                Marshal.FreeHGlobal(ptr);
                            }
                        }
                        else
                        {
                            // payload太小，手动提取关键字段
                            int mainOffset = (int)Marshal.OffsetOf(typeof(ROBOT_STATE_8083_PKG), "main_errcode");
                            int subOffset = (int)Marshal.OffsetOf(typeof(ROBOT_STATE_8083_PKG), "sub_errcode");

                            if (payload.Length > subOffset + 4)
                            {
                                int mainErrcode = BitConverter.ToInt32(payload, mainOffset);
                                int subErrcode = BitConverter.ToInt32(payload, subOffset);
                                Console.WriteLine($"[8083] main_errcode = {mainErrcode}, sub_errcode = {subErrcode}  " +
                                    $"(program_state={payload[0]}, error_code={payload[1]}, robot_mode={payload[2]}) [手动提取]");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[8083] 异常: {ex.Message}");
                }
                finally
                {
                    CloseSocket(socket);
                }

                Console.WriteLine($"[8083] 3秒后重试连接...");
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// 使用0x5A5A帧协议动态接收完整一帧数据
        /// 先读5字节帧头确定数据长度，再读取剩余数据，最后验证校验和
        /// </summary>
        static byte[] ReceiveFrameDynamic(Socket socket, int expectedPkgSize)
        {
            // 先读取帧头 (5字节: frame_head[2] + frame_cnt[1] + data_len[2])
            byte[] header = new byte[5];
            if (!ReceiveAll(socket, header, 0, 5))
            {
                Console.WriteLine($"读取帧头失败");
                return null;
            }

            // 验证帧头
            if (header[0] != 0x5A || header[1] != 0x5A)
            {
                Console.WriteLine($"帧头验证失败: 0x{header[0]:X2}{header[1]:X2}");
                return null;
            }

            byte frameCnt = header[2];
            UInt16 dataLen = (UInt16)(header[3] | (header[4] << 8));

            // 总帧大小 = data_len + 7
            int totalFrameSize = dataLen + 7;

            if (totalFrameSize > BUFFER_SIZE)
            {
                Console.WriteLine($"帧大小({totalFrameSize})超过缓冲区");
                return null;
            }

            // 读取帧剩余部分 (payload + checksum)
            byte[] frameData = new byte[totalFrameSize];
            Array.Copy(header, 0, frameData, 0, 5);

            if (!ReceiveAll(socket, frameData, 5, totalFrameSize - 5))
            {
                Console.WriteLine($"读取帧剩余数据失败");
                return null;
            }

            // 验证校验和
            UInt16 checksum = 0;
            for (int i = 0; i < totalFrameSize - 2; i++)
            {
                checksum += frameData[i];
            }
            UInt16 receivedChecksum = (UInt16)(frameData[totalFrameSize - 2] | (frameData[totalFrameSize - 1] << 8));

            if (checksum != receivedChecksum)
            {
                Console.WriteLine($"校验和不匹配: 计算=0x{checksum:X4}, 接收=0x{receivedChecksum:X4}");
                return null;
            }

            return frameData;
        }

        /// <summary>
        /// 完整接收指定字节数
        /// </summary>
        static bool ReceiveAll(Socket socket, byte[] buffer, int offset, int size)
        {
            int received = 0;
            while (received < size)
            {
                int n = socket.Receive(buffer, offset + received, size - received, SocketFlags.None);
                if (n <= 0) return false;
                received += n;
            }
            return true;
        }

        /// <summary>
        /// 连接到指定端口
        /// </summary>
        static Socket ConnectToPort(int port)
        {
            try
            {
                Console.WriteLine($"正在连接 {robotIp}:{port} ...");
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.ReceiveTimeout = 5000;
                socket.SendTimeout = 500;

                IAsyncResult result = socket.BeginConnect(
                    new IPEndPoint(IPAddress.Parse(robotIp), port), null, null);
                bool success = result.AsyncWaitHandle.WaitOne(3000, true);

                if (!success)
                {
                    Console.WriteLine($"连接 {robotIp}:{port} 超时");
                    socket.Close();
                    return null;
                }

                socket.EndConnect(result);
                Console.WriteLine($"[{port}] 连接成功");
                return socket;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"连接 {robotIp}:{port} 失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 安全关闭socket
        /// </summary>
        static void CloseSocket(Socket socket)
        {
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch { }
            }
        }
    }
}

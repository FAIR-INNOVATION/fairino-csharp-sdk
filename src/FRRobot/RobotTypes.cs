using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
//using System.Threading.Tasks;

namespace fairino
{
    /**
    * @brief 关节位置数据类型
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct JointPos
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] jPos;   /* 六个关节位置，单位deg */

        public JointPos(double[] pos)
        {
            jPos = pos;
        }

        public JointPos(double j1, double j2, double j3, double j4, double j5, double j6)
        {
            jPos = new double[6] { j1, j2, j3, j4, j5, j6};
        }
    }

    /**
    * @brief 笛卡尔空间位置数据类型
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DescTran
    {
        public double x;    /* x轴坐标，单位mm  */
        public double y;    /* y轴坐标，单位mm  */
        public double z;    /* z轴坐标，单位mm  */
        public DescTran(double posX, double posY, double posZ)
        {
            x = posX; 
            y = posY; 
            z = posZ;
        }
    }

    /**
    * @brief 欧拉角姿态数据类型
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct Rpy
    {
        public double rx;   /* 绕固定轴X旋转角度，单位：deg  */
        public double ry;   /* 绕固定轴Y旋转角度，单位：deg  */
        public double rz;   /* 绕固定轴Z旋转角度，单位：deg  */
        public Rpy(double rotateX, double rotateY, double rotateZ)
        {
            rx = rotateX;
            ry = rotateY;
            rz = rotateZ;
        }
    }

    /**
    *@brief 笛卡尔空间位姿类型
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DescPose
    {
        public DescTran tran;      /* 笛卡尔空间位置  */
        public Rpy rpy;			/* 笛卡尔空间姿态  */
        public DescPose(DescTran descTran, Rpy rotateRpy)
        {
            tran = descTran;
            rpy = rotateRpy;
        }

        public DescPose(double tranX, double tranY, double tranZ, double rX, double ry, double rz)
        {
            tran.x = tranX;
            tran.y = tranY;
            tran.z = tranZ;
            rpy.rx = rX;
            rpy.ry = ry;
            rpy.rz = rz;
        }
    }

    /**
    * @brief 扩展轴位置数据类型
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct ExaxisPos
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public double[] ePos;   /* 四个扩展轴位置，单位mm */
        public ExaxisPos(double[] exaxisPos)
        { 
            ePos = exaxisPos;
        }

        public ExaxisPos(double x, double y, double z, double a)
        {
            ePos = new double[4] { x, y, z, a};
        }

    }

    /**
    * @brief 力传感器的受力分量和力矩分量
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct ForceTorque
    {
        public double fx;  /* 沿x轴受力分量，单位N  */
        public double fy;  /* 沿y轴受力分量，单位N  */
        public double fz;  /* 沿z轴受力分量，单位N  */
        public double tx;  /* 绕x轴力矩分量，单位Nm */
        public double ty;  /* 绕y轴力矩分量，单位Nm */
        public double tz;  /* 绕z轴力矩分量，单位Nm */
        public ForceTorque(double fX, double fY, double fZ, double tX, double tY, double tZ)
        {
            fx = fX; 
            fy = fY; 
            fz = fZ; 
            tx = tX; 
            ty = tY; 
            tz = tZ;
        }
    }

    /**
    * @brief  螺旋参数数据类型
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct SpiralParam
    {
        public int circle_num;           /* 螺旋圈数  */
        public float circle_angle;         /* 螺旋倾角  */
        public float rad_init;             /* 螺旋初始半径，单位mm  */
        public float rad_add;              /* 半径增量  */
        public float rotaxis_add;          /* 转轴方向增量  */
        public uint rot_direction;  /* 旋转方向，0-顺时针，1-逆时针  */
        public SpiralParam(int circleNum, float circleAngle, float radInit, float radAdd, float rotaxisAdd, uint rotDirection)
        {
            circle_num = circleNum; 
            circle_angle = circleAngle;
            rad_init = radInit;
            rad_add = radAdd;
            rotaxis_add = rotaxisAdd;
            rot_direction = rotDirection;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ROBOT_AUX_STATE
    {
        public byte servoId;           //伺服驱动器ID号
        public int servoErrCode;       //伺服驱动器故障码
        public int servoState;         //伺服驱动器状态
        public double servoPos;        //伺服当前位置
        public float servoVel;         //伺服当前速度
        public float servoTorque;      //伺服当前转矩    25
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EXT_AXIS_STATUS
    {
        public double pos;        //扩展轴位置
        public double vel;        //扩展轴速度
        public int errorCode;     //扩展轴故障码
        public byte ready;        //伺服准备好
        public byte inPos;        //伺服到位
        public byte alarm;        //伺服报警
        public byte flerr;        //跟随误差
        public byte nlimit;       //到负限位
        public byte pLimit;       //到正限位
        public byte mdbsOffLine;  //驱动器485总线掉线
        public byte mdbsTimeout;  //控制卡与控制箱485通信超时
        public byte homingStatus; //扩展轴回零状态
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

        public string ToString()
        {
            return $"{year}-{mouth}-{day}  {hour}:{minute}:{second}.{millisecond}";
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ROBOT_STATE_PKG
    {
        public UInt16 frame_head;           //帧头 0x5A5A
        public byte frame_cnt;              //帧计数
        public UInt16 data_len;             //数据长度  5
        public byte program_state;          //程序运行状态，1-停止；2-运行；3-暂停
        public byte robot_state;            //机器人运动状态，1-停止；2-运行；3-暂停；4-拖动  7
        public int main_code;               //主故障码
        public int sub_code;                //子故障码
        public byte robot_mode;             //机器人模式，0-自动模式；1-手动模式 16

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] jt_cur_pos;                             //关节当前位置
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] tl_cur_pos;                             //工具当前位姿
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] flange_cur_pos;                         //末端法兰当前位姿
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] actual_qd;                              //机器人当前关节速度
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] actual_qdd;                             //机器人当前关节加速度  16 + 8 * 6 * 5 = 256
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] target_TCP_CmpSpeed;                    //机器人TCP合成指令速度                         //256 + 8* 2 = 272
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] target_TCP_Speed;                       //机器人TCP指令速度                        //272 + 8 * 6 = 320 
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] actual_TCP_CmpSpeed;                    //机器人TCP合成实际速度                        //320 + 16 = 336
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] actual_TCP_Speed;                       //机器人TCP实际速度                      //336 + 8 * 6 = 384
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] jt_cur_tor;                             //当前扭矩         //384 + 8 * 6 = 432 
        public int tool;                        //工具号
        public int user;                        //工件号
        public byte cl_dgt_output_h;            //数字输出15-8
        public byte cl_dgt_output_l;            //数字输出7-0
        public byte tl_dgt_output_l;            //工具数字输出7-0(仅bit0-bit1有效)
        public byte cl_dgt_input_h;             //数字输入15-8
        public byte cl_dgt_input_l;             //数字输入7-0
        public byte tl_dgt_input_l;             //工具数字输入7-0(仅bit0-bit1有效)                    // + 14 = 446
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public UInt16[] cl_analog_input;        //控制箱模拟量输入
        public UInt16 tl_anglog_input;          //工具模拟量输入                              // + 6 = 452
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] ft_sensor_raw_data;     //力/扭矩传感器原始数据
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] ft_sensor_data;         //力/扭矩传感器数据                           // + 8 * 12 = 548
        public byte ft_sensor_active;           //力/扭矩传感器激活状态， 0-复位，1-激活
        public byte EmergencyStop;              //急停标志
        public int motion_done;                 //到位信号
        public byte gripper_motiondone;         //夹爪运动完成信号
        public int mc_queue_len;                //运动队列长度
        public byte collisionState;             //碰撞检测，1-碰撞；0-无碰撞
        public int trajectory_pnum;             //轨迹点编号
        public byte safety_stop0_state;  /* 安全停止信号SI0 */
        public byte safety_stop1_state;  /* 安全停止信号SI1 */
        public byte gripper_fault_id;    /* 错误夹爪号 */               // + 19 = 567
        public UInt16 gripper_fault;     /* 夹爪故障 */
        public UInt16 gripper_active;    /* 夹爪激活状态 */
        public byte gripper_position;    /* 夹爪位置 */
        public byte gripper_speed;       /* 夹爪速度 */
        public byte gripper_current;     /* 夹爪电流 */
        public int gripper_tmp;          /* 夹爪温度 */
        public int gripper_voltage;      /* 夹爪电压 */                 // + 15 = 582
        public ROBOT_AUX_STATE auxState; /* 485扩展轴状态 */            // + 25 = 607
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public EXT_AXIS_STATUS[] extAxisStatus;  /* UDP扩展轴状态 */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public UInt16[] extDIState;        //扩展DI输入
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public UInt16[] extDOState;        //扩展DO输出
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt16[] extAIState;        //扩展AI输入
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt16[] extAOState;        //扩展AO输出
        public int rbtEnableState;       //机器人使能状态--robot enable state
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] jointDriverTorque;               //关节驱动器扭矩
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] jointDriverTemperature;          //关节驱动器温度
        public ROBOT_TIME robotTime;     //机器人系统时间
        public int softwareUpgradeState; //软件升级状态  0-空闲中或上传升级包中；1~100：升级完成百分比；-1:升级软件失败；-2：校验失败；-3：版本校验失败；-4：解压失败；-5：用户配置升级失败；-6：外设配置升级失败；-7：扩展轴配置升级失败；-8：机器人配置升级失败；-9：DH参数配置升级失败
        public UInt16 endLuaErrCode;    //末端LUA运行状态 
        public UInt16 check_sum;         /* 和校验 */                  // + 2 = 609
    }

    enum RobotError
    {
        ERR_UPLOAD_FILE_NOT_FOUND = -7,   /* 上传文件不存在 */
        ERR_SAVE_FILE_PATH_NOT_FOUND = -6,/* 保存文件路径不存在 */
        ERR_NOT_FOUND_LUA_FILE = -5,      /* lua文件不存在 */
        ERR_RPC_ERROR = -4,
        ERR_SOCKET_COM_FAILED = -2,
        ERR_OTHER = -1,
        ERR_SUCCESS = 0,
        ERR_PARAM_NUM = 3,
        ERR_PARAM_VALUE = 4,
        ERR_TPD_START_TIMER = 5,                      //tpd定时器启动失败   The tpd timer fails to start
        ERR_TPD_CLOSE_TIMER = 6,                      //tpd定时器关闭失败   The tpd timer fails to close
        ERR_TPD_CREATE_FILE = 7,                      //tpd文件创建失败     Failed to create the tpd file
        ERR_TPD_FILE_OPEN_FAILED = 8,                 
        ERR_TPD_SEND_FILENAME = 9,                    //tpd文件名发送失败   Failed to send the tpd file name
        ERR_TPD_SEND_FILECONTENT = 10,                //tpd文件内容发送失败 Failed to send tpd file contents
        ERR_PROGRAM_CONTENT = 11,                     //程序异常，解析停止  Program exception, parsing stopped
        ERR_TPDFILECONTENT = 12,                      //tpd文件内容不一致   The contents of the tpd file are inconsistent
        ERR_TPD_FILE_NUM_POINTS = 13,                 //tpd文件点数异常     The number of tpd files is abnormal
        ERR_EXECUTION_FAILED = 14,
        ERR_TPD_POINT_NUM = 15,                      //tpd记录点数超限     The number of tpd records exceeds the limit
        ERR_PROTOCOL_LOADED = 16,                    //协议已加载          Protocol loaded
        ERR_PROTOCOL_UNLOAD = 17,                    //协议未加载          Protocol unload
        ERR_PROGRAM_IS_RUNNING = 18,                 
        ERR_POSSENSOR_IS_INSTALL = 19,               //位姿传感器通信异常   The communication of the pose sensor is abnormal
        ERR_WEAVE_NOT_SETTOOL = 20,                  //摆焊未设置工具       No tool is set for swing welding
        ERR_EXAXIS_IS_ACTIVATING = 21,               //外部轴未去除激活     External axis not removed active
        ERR_TOOL_IS_NOT_SET = 22,                    //三点法未设置工具     The three-point method does not set a tool
        ERR_POSSENSOR_DATA_GET_FAIL = 23,            //位姿传感器数据获取失败        Failed to obtain the pose sensor data
        ERR_EIGHTPOINT_DATA_POS = 24,                //八点法-前四点姿态变化太大     Eight point method. - The first four points change too much
        ERR_COMPUTE_FAILED = 25,
        ERR_EIGHTPOINT_TOOL = 26,                    //八点法-未切换到基座标系，未切换到exaxis0   Eight-point method - did not switch to the base system, did not switch to exaxis0
        ERR_EIGHTPOINT_RESULT = 27,                  //八点法-计算结果异常  Eight-point method - Calculation results are abnormal   
        ERR_INVERSE_KINEMATICS_COMPUTE_FAILED = 28,
        ERR_SERVOJ_JOINT_OVERRUN = 29,
        ERR_NON_RESSETTABLE_FAULT = 30,
        ERR_EMERGENCY_STOP = 31,                     //急停按钮松开，请断电重启控制箱    The emergency stop button is released, please power off and restart the control box
        ERR_MODIFY_TP_OVER = 32,                     //关节超限            Joint Over
        ERR_EXTAXIS_CONFIG_FAILURE = 33,//外部轴未处于零位，导程、分辨率设置失败
        ERR_WORKPIECE_NUM = 34,
        ERR_WORKPIECE_NUM_NOT_ZERO = 35,             //请切换至工件号0      Please switch to workpiece number 0
        ERR_FILENAME_TOO_LONG = 36,
        ERR_TOOL_NUM = 37,
        ERR_STRANGE_POSE = 38,                       //奇异位姿
        ERR_SOCKET_NAME = 39,                        //socket名称无效       Socket name invalid
        ERR_VEL_SCALE_OVER = 40,                     //速度百分比超限       Percentage of speed exceeds the limit
        ERR_EXTAXIS_NOT_HOMING = 41,//外部轴未回零
        ERR_LASER_OFFECT_POSE_CHANGE = 42,           //姿态变化过大         Excessive attitude change
        ERR_CONVEYOR_IO_NOT_CONFIG = 43,             //传送带检测开关DI未配置     The conveyor belt detection switch DI is not configured
        ERR_ROBOT_POSE_OVER = 44,                    //机器人姿态角超限      The robot attitude Angle exceeds the limit
        ERR_EXTAXIS_NOT_ACTIVING = 45,//外部轴未激活
        ERR_EXTAXIS_NOT_CALIB = 46,//同步功能需要标定外部轴
        ERR_EXTAXIS_SERVO_CONFIG_FAIL = 47,//外部驱动器信息配置失败
        ERR_EXTAXIS_SERVO_CONFIG_OVER = 48,//外部轴驱动器信息配置超时
        ERR_EXTAXIS_UNABLE_ENABLE = 49,              //外部轴错误无法使能    External axis error cannot be enabled
        ERR_EXTAXIS_SERVO_GET_FAIL = 50,             //外部轴驱动器信息获取失败   Failed to obtain external shaft drive information
        ERR_EXTAXIS_SERVO_GET_OVER = 51,             //外部轴驱动器信息获取超时   Obtaining external shaft drive information timed out
        ERR_EXTAXIS_NOT_STEP_OPERATE = 52,//同步功能不能使用单步操作
        ERR_FT_SENSOR_NOT_ACTIVATE = 59,             //力/扭矩传感器未激活   The force/torque sensor is not active
        ERR_FT_SENSOR_RCS = 60,                      //力/扭矩传感器参考坐标系未切换至工具    The force/torque sensor reference coordinate system is not switched to the tool
        ERR_FT_SENSOR_NOT_HONMING = 61,              //力/扭矩传感器未设置零点    The force/torque sensor is not homing
        ERR_FT_SENSOR_LOAD_NOT_ZERO = 62,            //力扭矩传感器负载未设置为零  Force torque sensor load is not set to zero
        ERR_SYSTEM_TIME_GET_FAILED = 63,             //系统时间获取失败           Failed to obtain the system time
        ERR_NOT_ADD_CMD_QUEUE = 64,
        ERR_CIRCLE_SPIRAL_MIDDLE_POINT1 = 66,
        ERR_CIRCLE_SPIRAL_MIDDLE_POINT2 = 67,
        ERR_CIRCLE_SPIRAL_MIDDLE_POINT3 = 68,
        ERR_MOVEC_MIDDLE_POINT = 69,
        ERR_MOVEC_TARGET_POINT = 70,
        ERR_GRIPPER_MOTION = 73,
        ERR_LINE_POINT = 74,
        ERR_CHANNEL_FAULT = 75,
        ERR_WAIT_TIMEOUT = 76,
        ERR_TPD_CMD_POINT = 82,
        ERR_TPD_CMD_TOOL = 83,
        ERR_WELDING_TRACKING_SEARCH = 83,           //焊缝寻位失败               Weld locating failed
        ERR_LINE_CMD = 84,                          //直线指令错误               Line command error
        ERR_EXAXIS_CFG_CHECK_FAILED = 90,           //外部轴配置文件检查失败      Failed to check the external axis configuration file
        ERR_EXDEV_CFG_VERSION = 91,                 //外设配置文件版本不匹配      The version of the peripheral configuration file does not match
        ERR_EXDEV_CFG_READ = 92,                    //外设配置文件读取失败        Failed to read the peripheral configuration file
        ERR_SPL_CMD_POINT_NUM = 93,                 //样条指令点数超限            The number of spline instructions exceeds the limit
        ERR_SPLINE_POINT = 94,
        ERR_SPLINE_PARAM = 95,                      //样条参数错误               The spline parameter is incorrect
        ERR_WIRE_SEARCH_FAILED = 96,                //焊丝寻位失败               Welding wire locating failed
        ERR_RECORD_DATA_EMPTY = 97,                 //记录数据为空               The recorded data is empty
        ERR_MAIN_PROG_NOT_CONFIG = 98,              //未配置主程序               The main program is not configured
        ERR_SAFETY_STOP = 99,                       //安全停止已触发             A safe stop has been triggered
        ERR_HOME_POINT_NOT_CONFIG = 100,            //未配置作业原点             The job origin is not configured
        ERR_ROBOT_NOT_ENABLE = 101,                 //机器人未使能               The robot is not enabled
        ERR_EXAXIS_NOT_ENABLE = 106,                //外部轴未使能               The external axis is not enabled
        ERR_SPIRAL_START_POINT = 108,
        ERR_DRAG_LOCK_INTERFERE = 110,              //请关闭进入干涉区拖动配置-阻抗回调     Please turn off Drag Configuration - Impedance callback into the interference zone
        ERR_INTERFERE_DRAG_LOCK = 111,              //请关闭拖动示教锁定自由度功能          Turn off the drag teaching lock freedom function
        ERR_TARGET_POSE_CANNOT_REACHED = 112,       //给定位姿无法到达
        ERR_DMP_CMD_POINT = 114,                    //DMP指令点错误              DMP command point error
        ERR_UNIFCIRCLE_CMD_POINT = 115,             //圆周运动指令点错误          Circular motion command point error
        ERR_EXAXIS_DRIVER_NOT_LOAD = 116,           //扩展轴外设通信驱动未加载    The extension shaft peripheral communication driver is not loaded
        ERR_FT_PAYLOAD = 118,                       //力传感器下负载重量错误      Load weight error under force sensor
        ERR_FT_PAYLOAD_COG = 119,                   //力传感器下负载质心错误      Load center of mass error under the force sensor
        ERR_TRAJ_CMD_POINT = 120,                   //轨迹指令点错误             Trace command point error
        ERR_TRAJ_POINT_EMPTY = 121,                 //轨迹点数为0                The number of trace points is 0
        ERR_JOINT_TORQUE_OVERRUN = 122,             //关节扭矩超限               Joint torque exceeds the limit
        ERR_DRAG_MODE_EXIT = 123,                   //请先退出拖动模式           Exit drag mode first
        ERR_AUX_SERVO_PARAM = 124,                  //扩展轴参数未配置           The extension axis parameters are not configured
        ERR_FILE_GET_LUA_FILE = 125,                //恢复焊接失败-解析原程序失败         Recovery weld failed - Failed to parse the original program
        ERR_GET_REWELD_PT_FAILED = 126,             //恢复焊接失败-无法获取再起弧点       Recovery weld failed - Unable to obtain rearc point
        ERR_CREATE_NEW_LUA_FAIL = 127,              //恢复焊接失败-无法生成恢复程序       Recovery weld failed - Unable to generate recovery program
        ERR_NOT_MANUAL_STOP = 128,                  //未处于手动模式/停止状态             Not in manual mode/stopped state
        ERR_DYNAMICS_MODE = 129,                    //请先切换至新动力学模式              Please switch to the new dynamics mode first
        ERR_POINTTABLE_NOTFOUND = 130,
        ERR_FT_DRAG_OR_RB_DRAG = 133,               //请先切入拖动模式/力传感器辅助拖动模式       Please first enter Drag mode/Force Sensor Assisted drag mode
        ERR_FT_SENSOR_NO_LOAD = 134,                //力/扭矩传感器无负载                Force/torque sensor no load
        ERR_FT_SENSOR_MATRIX_INV = 135,             //力/扭矩传感器下矩阵求逆异常         Inverse anomaly of matrix under force/torque sensor
        ERR_DETECT_DI_NOT_CONFIG = 138,             //检测开关DI未配置                   The detection switch DI is not configured
        ERR_NO_APPLY_EXAXIS_COORDSYS = 139,         //请先应用扩展轴坐标系                Please apply the extended axis coordinate system first
        ERR_EXTAXIS_NOT_ACTIVATED = 140,            //请先激活当前扩展轴                 Please activate the current extension axis first
        ERR_TEACHINGPOINTNOTFOUND = 143,            //示教点位信息不存在
        ERR_LUAFILENITFOUND = 144,                  //LUA文件不存在
        ERR_JOINTCONFIGCHANGE = 151,                //关节配置发生变化
        ERR_WEAVEPOINTDISTANCETOOSMALL = 152,       //摆焊指令点间距过小
        ERR_ARCLENGTHTOOSMALL = 153            //圆弧指令点间距太小



    }


    internal class RobotTypes
    {

    }
}

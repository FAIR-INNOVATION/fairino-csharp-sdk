using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using fairino;

internal class FRCNDEClient
{
    // 错误码常量（与C++保持一致）
    public const int ERR_SUCCESS = 0;
    public const int ERR_NEED_AT_LEAST_ONE_STATE = -19;
    public const int ERR_STATE_ALREADY_EXISTS = -17;
    public const int ERR_STATE_INVALID = -18;
    public const int ERR_TOO_MANY_STATES = -20;
    public const int ERR_PARAM_VALUE = 4;

    private StatusTCPClient _rtClient;
    private bool _robotStateRunFlag = false;
    private byte _sendCount = 0;
    private int _robotStatePeriod = 8;   // 默认500ms，与C++一致
    private readonly object _recvLock = new object();
    private ROBOT_STATE_PKG _robotStatePkg;
    private bool _isConnected = false;      // 是否已成功连接并启动
    private bool _startSent = false;        // 是否已发送 START 帧
    private Action<int> _errorCallback;   // 用于更新外部错误码

    // 配置的状态列表（顺序决定解析顺序）
    private List<RobotState> _configStates = new List<RobotState>();

    // 所有状态的元数据：字段名、偏移、结构体类型、CNDE类型
    private static Dictionary<RobotState, (string FieldName, int Offset, string StructType, string CndeType)> _allStates;

    private bool _reconnEnable = true;
    private int _reconnTimes = 1000;
    private int _reconnPeriod = 200;
    private Log log;

    public void SetLog(Log logger)
    {
        log = logger;
        if(logger == null)
        {
            Console.WriteLine("a null log");
        }
    }

    /// <summary>设置断线重连参数</summary>
    public void SetReconnectParam(bool enable, int reconnectTimes, int reconnectPeriod)
    {
        _reconnEnable = enable;
        _reconnTimes = reconnectTimes;
        _reconnPeriod = reconnectPeriod;
        // 将参数传递给底层的 TCP 客户端
        _rtClient.SetReconnectParam(enable, reconnectTimes, reconnectPeriod);
    }

    /// <summary>获取当前重连状态（是否正在重连）</summary>
    public bool GetReconnectState()
    {
        return _rtClient.GetReconnState();
    }

    public bool IsConnected()
    {
        return _rtClient != null && _rtClient.IsConnected();
    }

    private static readonly Dictionary<string, string> ProtocolToCSharpFieldMap = new Dictionary<string, string>
    {
        // 格式：{ "协议字段名（下划线）", "C# 结构体字段名（驼峰）" }
        { "frame_head", "frame_head" },          // 相同
        { "frame_cnt", "frame_cnt" },
        { "data_len", "data_len" },
        { "program_state", "program_state" },
        { "robot_state", "robot_state" },
        { "main_code", "main_code" },
        { "sub_code", "sub_code" },
        { "robot_mode", "robot_mode" },
        { "actual_joint_pos", "jt_cur_pos" },    // 协议名 ≠ C#字段名
        { "actual_TCP_pos", "tl_cur_pos" },
        { "actual_flange_pos", "flange_cur_pos" },
        { "actual_joint_vel", "actual_qd" },
        { "actual_joint_acc", "actual_qdd" },
        { "target_TCP_cmpvel", "target_TCP_CmpSpeed" },
        { "target_TCP_vel", "target_TCP_Speed" },
        { "actual_TCP_cmpvel", "actual_TCP_CmpSpeed" },
        { "actual_TCP_vel", "actual_TCP_Speed" },
        { "actual_joint_torque", "jt_cur_tor" },
        { "tool_id", "tool" },
        { "wobj_id", "user" },
        { "cfg_DO_box", "cl_dgt_output_h" },
        { "std_DO_box", "cl_dgt_output_l" },
        { "cfg_DO_tool", "tl_dgt_output_l" },
        { "cfg_DI_box", "cl_dgt_input_h" },
        { "std_DI_box", "cl_dgt_input_l" },
        { "cfg_DI_tool", "tl_dgt_input_l" },
        { "std_AI0_box,std_AI1_box", "cl_analog_input" },  // 复合字段特殊处理
        { "std_AI_tool", "tl_anglog_input" },
        { "ft_sensor_raw_data", "ft_sensor_raw_data" },
        { "ft_sensor_data", "ft_sensor_data" },
        { "ft_sensor_active", "ft_sensor_active" },
        { "emergency_stop", "EmergencyStop" },
        { "motion_done", "motion_done" },
        { "gripper_motion_done", "gripper_motiondone" },
        { "motion_queue_len", "mc_queue_len" },
        { "collision_state", "collisionState" },
        { "trajectory_pnum", "trajectory_pnum" },
        { "safety_stop0_state", "safety_stop0_state" },
        { "safety_stop1_state", "safety_stop1_state" },
        { "gripper_fault_id", "gripper_fault_id" },
        { "gripper_fault", "gripper_fault" },
        { "gripper_active", "gripper_active" },
        { "gripper_position", "gripper_position" },
        { "gripper_speed", "gripper_speed" },
        { "gripper_current", "gripper_current" },
        { "gripper_temp", "gripper_temp" },
        { "gripper_voltage", "gripper_voltage" },
        { "aux_axis_state", "auxState" },
        { "exaxis_status", "extAxisStatus" },
        { "ext_DI_state", "extDIState" },
        { "ext_DO_state", "extDOState" },
        { "ext_AI_state", "extAIState" },
        { "ext_AO_state", "extAOState" },
        { "rbt_enable_state", "rbtEnableState" },
        { "joint_driver_torque", "jointDriverTorque" },
        { "actual_joint_temp", "jointDriverTemperature" },
        { "robot_time", "robotTime" },
        { "software_upgrade_state", "softwareUpgradeState" },
        { "end_lua_err_code", "endLuaErrCode" },
        { "std_AO0_box,std_AO1_box", "cl_analog_output" },
        { "std_AO_tool", "tl_analog_output" },
        { "rotating_gripper_num", "gripperRotNum" },
        { "rotating_gripper_speed", "gripperRotSpeed" },
        { "rotating_gripper_tor", "gripperRotTorque" },
        { "weld_break_off_state,weld_arc_state", "weldingBreakOffState" },
        { "target_joint_torque", "jt_tgt_tor" },
        { "smarttool_state", "smartToolState" },
        { "wide_voltage_ctrl_box_temp", "wideVoltageCtrlBoxTemp" },
        { "wide_voltage_ctrl_box_fan_current", "wideVoltageCtrlBoxFanVel" },
        { "tool_coord", "toolCoord" },
        { "wobj_coord", "wobjCoord" },
        { "exTool_coord", "extoolCoord" },
        { "exAxis_coord", "exAxisCoord" },
        { "payload", "load" },
        { "pay_cog", "loadCog" },
        { "last_servoJ_target", "lastServoTarget" },
        { "servoJ_cmd_num", "servoJCmdNum" },
        { "target_joint_pos", "targetJointPos" },
        { "target_joint_vel", "targetJointVel" },
        { "target_joint_acc", "targetJointAcc" },
        { "target_joint_current", "targetJointCurrent" },
        { "actual_joint_current", "actualJointCurrent" },
        { "actual_TCP_force", "actualTCPForce" },
        { "target_TCP_pos", "targetTCPPos" },
        { "collision_level", "collisionLevel" },
        { "speed_scaling_man", "speedScaleManual" },
        { "speed_scaling_auto", "speedScaleAuto" },
        { "line_number", "luaLineNum" },
        { "abnormal_stop", "abnomalStop" },
        { "cur_lua_file_name", "currentLuaFileName" },
        { "prog_total_line", "programTotalLine" },
        { "safety_box_signal", "safetyBoxSingal" },
        { "welding_voltage", "weldVoltage" },
        { "welding_current", "weldCurrent" },
        { "welding_track_speed", "weldTrackVel" },
        { "tpd_exception", "tpdException" },
        { "alarm_reboot_robot", "alarmRebootRobot" },
        { "modbus_master_connect", "modbusMasterConnect" },
        { "modbus_slave_connect", "modbusSlaveConnect" },
        { "btn_box_stop_signal", "btnBoxStopSignal" },
        { "drag_alarm", "dragAlarm" },
        { "safety_door_alarm", "safetyDoorAlarm" },
        { "safety_plane_alarm", "safetyPlaneAlarm" },
        { "motion_alarm", "motonAlarm" },
        { "interfere_alarm", "interfaceAlarm" },
        { "udp_cmd_state", "udpCmdState" },
        { "weld_ready_state", "weldReadyState" },
        { "alarm_check_emerg_stop_btn", "alarmCheckEmergStopBtn" },
        { "ts_tm_cmd_com_error", "tsTmCmdComError" },
        { "ts_tm_state_com_error", "tsTmStateComError" },
        { "ctrl_box_error", "ctrlBoxError" },
        { "safety_data_state", "safetyDataState" },
        { "force_sensor_err_state", "forceSensorErrState" },
        { "ctrl_open_lua_errcode", "ctrlOpenLuaErrCode" },
        { "strange_pos_flag", "strangePosFlag" },
        { "alarm", "alarm" },
        { "dr_alarm", "driverAlarm" },
        { "alive_slave_num_error", "aliveSlaveNumError" },
        { "slave_com_error", "slaveComError" },
        { "cmd_point_error", "cmdPointError" },
        { "IO_error", "IOError" },
        { "gripper_error", "gripperError" },
        { "file_error", "fileError" },
        { "para_error", "paraError" },
        { "exaxis_out_slimit_error", "exaxisOutLimitError" },
        { "dr_com_err", "driverComError" },
        { "dr_err", "driverError" },
        { "out_sflimit_err", "outSoftLimitError" },
        { "axle_gen_com_data", "axleGenComData" },
        //{ "check_sum", "check_sum" }
        { "socket_conn_timeout", "socketConnTimeout" },
        { "socket_read_timeout", "socketReadTimeout" },
        { "ts_web_state_com_err", "tsWebStateComErr" },
    };

    private static readonly Dictionary<string, string> CSharpToProtocolFieldMap;

    static FRCNDEClient()
    {
        InitAllStates();
        // 构建反向映射：从 C# 字段名到协议名
        CSharpToProtocolFieldMap = ProtocolToCSharpFieldMap.ToDictionary(kv => kv.Value, kv => kv.Key);
    }

    private static void InitAllStates()
    {
        _allStates = new Dictionary<RobotState, (string, int, string, string)>();

        // 以下元数据完全对应C++ InitAllStates()函数中的内容
        // 注意：偏移量使用 Marshal.OffsetOf 动态获取，避免硬编码
        var pkgType = typeof(ROBOT_STATE_PKG);

        AddState(RobotState.FrameHead, "frame_head", "UINT16", "UINT16");
        AddState(RobotState.FrameCnt, "frame_cnt", "UINT8", "UINT8");
        AddState(RobotState.DataLen, "data_len", "UINT16", "UINT16");
        AddState(RobotState.ProgramState, "program_state", "UINT8", "UINT8");
        AddState(RobotState.RobotState, "robot_state", "UINT8", "UINT8");
        AddState(RobotState.MainCode, "main_code", "INT32", "INT32");
        AddState(RobotState.SubCode, "sub_code", "INT32", "INT32");
        AddState(RobotState.RobotMode, "robot_mode", "UINT8", "UINT8");
        AddState(RobotState.JointCurPos, "jt_cur_pos", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.ToolCurPos, "tl_cur_pos", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.FlangeCurPos, "flange_cur_pos", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.ActualJointVel, "actual_qd", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.ActualJointAcc, "actual_qdd", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.TargetTCPCmpSpeed, "target_TCP_CmpSpeed", "DOUBLE_2", "DOUBLE_2");
        AddState(RobotState.TargetTCPSpeed, "target_TCP_Speed", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.ActualTCPCmpSpeed, "actual_TCP_CmpSpeed", "DOUBLE_2", "DOUBLE_2");
        AddState(RobotState.ActualTCPSpeed, "actual_TCP_Speed", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.ActualJointTorque, "jt_cur_tor", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.Tool, "tool", "INT32", "INT32");
        AddState(RobotState.User, "user", "INT32", "INT32");
        AddState(RobotState.ClDgtOutputH, "cl_dgt_output_h", "UINT8", "UINT8");
        AddState(RobotState.ClDgtOutputL, "cl_dgt_output_l", "UINT8", "UINT8");
        AddState(RobotState.TlDgtOutputL, "tl_dgt_output_l", "UINT8", "UINT8");
        AddState(RobotState.ClDgtInputH, "cl_dgt_input_h", "UINT8", "UINT8");
        AddState(RobotState.ClDgtInputL, "cl_dgt_input_l", "UINT8", "UINT8");
        AddState(RobotState.TlDgtInputL, "tl_dgt_input_l", "UINT8", "UINT8");
        AddState(RobotState.ClAnalogInput, "cl_analog_input", "UINT16_2", "DOUBLE,DOUBLE");
        AddState(RobotState.TlAnglogInput, "tl_anglog_input", "UINT16", "DOUBLE");
        AddState(RobotState.FtSensorRawData, "ft_sensor_raw_data", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.FtSensorData, "ft_sensor_data", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.FtSensorActive, "ft_sensor_active", "UINT8", "UINT8");
        AddState(RobotState.EmergencyStop, "EmergencyStop", "UINT8", "UINT8");
        AddState(RobotState.MotionDone, "motion_done", "INT32", "INT32");
        AddState(RobotState.GripperMotiondone, "gripper_motiondone", "UINT8", "UINT8");
        AddState(RobotState.McQueueLen, "mc_queue_len", "INT32", "INT32");
        AddState(RobotState.CollisionState, "collisionState", "UINT8", "UINT8");
        AddState(RobotState.TrajectoryPnum, "trajectory_pnum", "INT32", "INT32");
        AddState(RobotState.SafetyStop0State, "safety_stop0_state", "UINT8", "UINT8");
        AddState(RobotState.SafetyStop1State, "safety_stop1_state", "UINT8", "UINT8");
        AddState(RobotState.GripperFaultId, "gripper_fault_id", "UINT8", "UINT8");
        AddState(RobotState.GripperFault, "gripper_fault", "UINT16", "INT32");
        AddState(RobotState.GripperActive, "gripper_active", "UINT16", "INT32");
        AddState(RobotState.GripperPosition, "gripper_position", "UINT8", "UINT8");
        AddState(RobotState.GripperSpeed, "gripper_speed", "INT8", "INT32");
        AddState(RobotState.GripperCurrent, "gripper_current", "INT8", "INT32");
        AddState(RobotState.GripperTemp, "gripper_temp", "INT32", "INT32");
        AddState(RobotState.GripperVoltage, "gripper_voltage", "INT32", "INT32");
        AddState(RobotState.AuxState, "auxState", "UINT8,INT32,INT32,DOUBLE,FLOAT,FLOAT", "UINT8,INT32,INT32,DOUBLE,FLOAT,FLOAT");
        AddState(RobotState.ExtAxisStatus, "extAxisStatus", "UINT8_116", "UINT8_116"); // 简化处理
        AddState(RobotState.ExtDIState, "extDIState", "UINT16_8", "UINT8_16");
        AddState(RobotState.ExtDOState, "extDOState", "UINT16_8", "UINT8_16");
        AddState(RobotState.ExtAIState, "extAIState", "UINT16_4", "INT32_4");
        AddState(RobotState.ExtAOState, "extAOState", "UINT16_4", "INT32_4");
        AddState(RobotState.RbtEnableState, "rbtEnableState", "INT32", "INT32");
        AddState(RobotState.JointDriverTorque, "jointDriverTorque", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.JointDriverTemperature, "jointDriverTemperature", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.RobotTime, "robotTime", "UINT16,UINT8,UINT8,UINT8,UINT8,UINT8,UINT16", "INT32_7");
        AddState(RobotState.SoftwareUpgradeState, "softwareUpgradeState", "INT32", "INT32");
        AddState(RobotState.EndLuaErrCode, "endLuaErrCode", "UINT16", "INT32");
        AddState(RobotState.ClAnalogOutput, "cl_analog_output", "UINT16_2", "DOUBLE,DOUBLE");
        AddState(RobotState.TlAnalogOutput, "tl_analog_output", "UINT16", "DOUBLE");
        AddState(RobotState.GripperRotNum, "gripperRotNum", "FLOAT", "DOUBLE");
        AddState(RobotState.GripperRotSpeed, "gripperRotSpeed", "UINT8", "UINT8");
        AddState(RobotState.GripperRotTorque, "gripperRotTorque", "UINT8", "UINT8");
        AddState(RobotState.WeldingBreakOffState, "weldingBreakOffState", "UINT8,UINT8", "UINT8,UINT8");
        AddState(RobotState.TargetJointTorque, "jt_tgt_tor", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.SmartToolState, "smartToolState", "INT32", "UINT32");
        AddState(RobotState.WideVoltageCtrlBoxTemp, "wideVoltageCtrlBoxTemp", "FLOAT", "DOUBLE");
        AddState(RobotState.WideVoltageCtrlBoxFanCurrent, "wideVoltageCtrlBoxFanVel", "UINT16", "INT32");
        AddState(RobotState.ToolCoord, "toolCoord", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.WobjCoord, "wobjCoord", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.ExtoolCoord, "extoolCoord", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.ExAxisCoord, "exAxisCoord", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.Load, "load", "DOUBLE", "DOUBLE");
        AddState(RobotState.LoadCog, "loadCog", "DOUBLE_3", "DOUBLE_3");
        AddState(RobotState.LastServoTarget, "lastServoTarget", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.ServoJCmdNum, "servoJCmdNum", "INT32", "INT32");
        AddState(RobotState.TargetJointPos, "target_joint_pos", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.TargetJointVel, "target_joint_vel", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.TargetJointAcc, "target_joint_acc", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.TargetJointCurrent, "target_joint_current", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.ActualJointCurrent, "actual_joint_current", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.ActualTCPForce, "actual_TCP_force", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.TargetTCPPos, "target_TCP_pos", "DOUBLE_6", "DOUBLE_6");
        AddState(RobotState.CollisionLevel, "collision_level", "UINT8_6", "UINT8_6");
        AddState(RobotState.SpeedScaleManual, "speed_scaling_man", "DOUBLE", "DOUBLE");
        AddState(RobotState.SpeedScaleAuto, "speed_scaling_auto", "DOUBLE", "DOUBLE");
        AddState(RobotState.LuaLineNum, "line_number", "INT32", "INT32");
        AddState(RobotState.AbnomalStop, "abnormal_stop", "UINT8", "UINT8");
        AddState(RobotState.CurrentLuaFileName, "cur_lua_file_name", "UINT8_256", "UINT8_256");
        AddState(RobotState.ProgramTotalLine, "prog_total_line", "UINT8", "UINT8");
        AddState(RobotState.SafetyBoxSingal, "safety_box_signal", "UINT8_6", "UINT8_6");
        AddState(RobotState.WeldVoltage, "welding_voltage", "DOUBLE", "DOUBLE");
        AddState(RobotState.WeldCurrent, "welding_current", "DOUBLE", "DOUBLE");
        AddState(RobotState.WeldTrackVel, "welding_track_speed", "DOUBLE", "DOUBLE");
        AddState(RobotState.TpdException, "tpd_exception", "UINT8", "UINT8");
        AddState(RobotState.AlarmRebootRobot, "alarm_reboot_robot", "UINT8", "UINT8");
        AddState(RobotState.ModbusMasterConnect, "modbus_master_connect", "UINT8", "UINT8");
        AddState(RobotState.ModbusSlaveConnect, "modbus_slave_connect", "UINT8", "UINT8");
        AddState(RobotState.BtnBoxStopSignal, "btn_box_stop_signal", "UINT8", "UINT8");
        AddState(RobotState.DragAlarm, "drag_alarm", "UINT8", "UINT8");
        AddState(RobotState.SafetyDoorAlarm, "safety_door_alarm", "UINT8", "UINT8");
        AddState(RobotState.SafetyPlaneAlarm, "safety_plane_alarm", "UINT8", "UINT8");
        AddState(RobotState.MotonAlarm, "motion_alarm", "UINT8", "UINT8");
        AddState(RobotState.InterfaceAlarm, "interfere_alarm", "UINT8", "UINT8");
        AddState(RobotState.UdpCmdState, "udp_cmd_state", "INT32", "INT32");
        AddState(RobotState.WeldReadyState, "weld_ready_state", "UINT8", "UINT8");
        AddState(RobotState.AlarmCheckEmergStopBtn, "alarm_check_emerg_stop_btn", "UINT8", "UINT8");
        AddState(RobotState.TsTmCmdComError, "ts_tm_cmd_com_error", "UINT8", "UINT8");
        AddState(RobotState.TsTmStateComError, "ts_tm_state_com_error", "UINT8", "UINT8");
        AddState(RobotState.CtrlBoxError, "ctrl_box_error", "INT32", "INT32");
        AddState(RobotState.SafetyDataState, "safety_data_state", "UINT8", "UINT8");
        AddState(RobotState.ForceSensorErrState, "force_sensor_err_state", "UINT8", "UINT8");
        AddState(RobotState.CtrlOpenLuaErrCode, "ctrl_open_lua_errcode", "UINT8_4", "UINT8_4");
        AddState(RobotState.StrangePosFlag, "strange_pos_flag", "UINT8", "UINT8");
        AddState(RobotState.Alarm, "alarm", "UINT8", "UINT8");
        AddState(RobotState.DriverAlarm, "dr_alarm", "UINT8", "UINT8");
        AddState(RobotState.AliveSlaveNumError, "alive_slave_num_error", "UINT8", "UINT8");
        AddState(RobotState.SlaveComError, "slave_com_error", "UINT8_8", "UINT8_8");
        AddState(RobotState.CmdPointError, "cmd_point_error", "UINT8", "UINT8");
        AddState(RobotState.IOError, "IO_error", "UINT8", "UINT8");
        AddState(RobotState.GripperError, "gripper_error", "UINT8", "UINT8");
        AddState(RobotState.FileError, "file_error", "UINT8", "UINT8");
        AddState(RobotState.ParaError, "para_error", "UINT8", "UINT8");
        AddState(RobotState.ExaxisOutLimitError, "exaxis_out_slimit_error", "UINT8", "UINT8");
        AddState(RobotState.DriverComError, "dr_com_err", "UINT8_6", "UINT8_6");
        AddState(RobotState.DriverError, "dr_err", "UINT8", "UINT8");
        AddState(RobotState.OutSoftLimitError, "out_sflimit_err", "UINT8", "UINT8");
        AddState(RobotState.AxleGenComData, "axle_gen_com_data", "UINT8_130", "UINT8_130");
        //AddState(RobotState.CheckSum, "check_sum", "UINT16", "UINT16");
        AddState(RobotState.SocketConnTimeout, "socket_conn_timeout", "UINT8", "UINT8");
        AddState(RobotState.SocketReadTimeout, "socket_read_timeout", "UINT8", "UINT8");
        AddState(RobotState.TsWebStateComErr, "ts_web_state_com_err", "UINT8", "UINT8");
    }

    private static void AddState(RobotState state, string protocolFieldName, string structType, string cndeType)
    {
        var pkgType = typeof(ROBOT_STATE_PKG);
        if (!ProtocolToCSharpFieldMap.TryGetValue(protocolFieldName, out string csharpFieldName))
        {
            csharpFieldName = protocolFieldName;
        }
        var field = pkgType.GetField(csharpFieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (field == null)
            throw new Exception($"字段 '{csharpFieldName}' (协议名 '{protocolFieldName}') 不存在");
        int offset = Marshal.OffsetOf(pkgType, csharpFieldName).ToInt32();
        // 存储 C# 字段名
        _allStates[state] = (csharpFieldName, offset, structType, cndeType);
    }

    public FRCNDEClient(ROBOT_STATE_PKG pkg, Action<int> errorCallback, string ip = "192.168.58.2", int port = 20005)
    {
        _robotStatePkg = pkg;
        _errorCallback = errorCallback;
        _rtClient = new StatusTCPClient(ip, port);
        // 默认配置（与C++一致）
        _configStates = new List<RobotState>
        {
            //RobotState.ProgramState
            RobotState.ProgramState, RobotState.RobotState, RobotState.MainCode,
            RobotState.SubCode, RobotState.RobotMode, RobotState.JointCurPos,
            RobotState.ToolCurPos, RobotState.FlangeCurPos, RobotState.ActualJointVel,
            RobotState.ActualJointAcc, RobotState.TargetTCPCmpSpeed, RobotState.TargetTCPSpeed,
            RobotState.ActualTCPCmpSpeed, RobotState.ActualTCPSpeed, RobotState.ActualJointTorque,
            RobotState.Tool, RobotState.User, RobotState.ClDgtOutputH, RobotState.ClDgtOutputL,
            RobotState.TlDgtOutputL, RobotState.ClDgtInputH, RobotState.ClDgtInputL,
            RobotState.TlDgtInputL, RobotState.ClAnalogInput, RobotState.TlAnglogInput,
            RobotState.FtSensorRawData, RobotState.FtSensorData, RobotState.FtSensorActive,
            RobotState.EmergencyStop, RobotState.MotionDone, RobotState.GripperMotiondone,
            RobotState.McQueueLen, RobotState.CollisionState, RobotState.TrajectoryPnum,
            RobotState.SafetyStop0State, RobotState.SafetyStop1State, RobotState.GripperFaultId,
            RobotState.GripperFault, RobotState.GripperActive, RobotState.GripperPosition,
            RobotState.GripperSpeed, RobotState.GripperCurrent, RobotState.GripperTemp,
            RobotState.GripperVoltage, RobotState.AuxState,RobotState.ExtAxisStatus,
            RobotState.ExtDIState, RobotState.ExtDOState, RobotState.ExtAIState,
            RobotState.ExtAOState, RobotState.RbtEnableState, RobotState.JointDriverTorque,
            RobotState.JointDriverTemperature, RobotState.RobotTime, RobotState.SoftwareUpgradeState,
            RobotState.EndLuaErrCode, RobotState.ClAnalogOutput, RobotState.TlAnalogOutput,
            RobotState.GripperRotNum, RobotState.GripperRotSpeed, RobotState.GripperRotTorque,
            RobotState.WeldingBreakOffState, RobotState.TargetJointTorque, RobotState.SmartToolState,
            RobotState.WideVoltageCtrlBoxTemp, RobotState.WideVoltageCtrlBoxFanCurrent,
            RobotState.ToolCoord, RobotState.WobjCoord, RobotState.ExtoolCoord, RobotState.ExAxisCoord,
            RobotState.Load, RobotState.LoadCog, RobotState.LastServoTarget
        };
    }

    private Thread _recvThread;

    // 连接管理
    public int Connect(string ip, int port)
    {
        if (_isConnected)
        {
            Console.WriteLine("[Connect] Already connected, skip.");
            return 0;
        }
        _rtClient.SetIpPort(ip, port);
        _rtClient.SetReconnectParam(_reconnEnable, _reconnTimes, _reconnPeriod);
        _rtClient.SetLog(log);
        if (!_rtClient.Connect()) return -1;
        _robotStateRunFlag = true;

        // 1. 发送配置帧
        int ret = SendCNDEOutputConfig();
        if (ret != 0)
        {
            Console.WriteLine($"SendCNDEOutputConfig 失败，重试...");
            // 短暂等待后重试
            Thread.Sleep(100);
            ret = SendCNDEOutputConfig();
            Console.WriteLine($"SendCNDEOutputConfig ret {ret}");
            if (ret != 0) return ret;
        }
        ClearAllFields();
        // 2. 发送开始帧
        ret = SetCNDEStart();
        if (ret != 0) return ret;

        // 3. 启动接收线程
        _recvThread = new Thread(RecvRobotStateThread);
        _recvThread.IsBackground = true;
        _recvThread.Start();
        _isConnected = true;
        _startSent = true;
        return 0;
    }

    public int Close()
    {
        _robotStateRunFlag = false;
        _rtClient.Close();
        _recvThread?.Join(2000);
        _isConnected = false;
        _startSent = false;
        return 0;
    }

    // 状态配置接口（完全对应C++）
    public int SetCNDEStateConfig(List<RobotState> states, int period)
    {
        if (states == null || states.Count == 0)
        {
            log?.LogInfo("CNDE SetCNDEStateConfig: 状态列表为空，返回 ERR_NEED_AT_LEAST_ONE_STATE");
            return ERR_NEED_AT_LEAST_ONE_STATE;
        }

        foreach (var state in states)
        {
            if (!_allStates.ContainsKey(state))
            {
                log?.LogInfo($"CNDE SetCNDEStateConfig: 无效状态枚举值 '{state}'，返回 ERR_STATE_INVALID");
                return ERR_STATE_INVALID;
            }
        }

        if (period < 8 || period > 1000)
        {
            log?.LogInfo($"CNDE SetCNDEStateConfig: 周期 {period} ms 超出允许范围 [8, 1000]，返回 ERR_PARAM_VALUE");
            return ERR_PARAM_VALUE;
        }

        _configStates.Clear();
        _configStates.AddRange(states);
        _robotStatePeriod = period;
        return ERR_SUCCESS;
    }

    public int AddCNDEState(RobotState state)
    {
        if (!_allStates.ContainsKey(state))
            return ERR_STATE_INVALID;
        if (_configStates.Contains(state))
            return ERR_STATE_ALREADY_EXISTS;
        _configStates.Add(state);
        return ERR_SUCCESS;
    }

    public int DeleteCNDEState(RobotState state)
    {
        if (!_configStates.Contains(state))
            return ERR_STATE_INVALID;
        if (_configStates.Count <= 1)
            return ERR_NEED_AT_LEAST_ONE_STATE;
        _configStates.Remove(state);
        return ERR_SUCCESS;
    }

    public int SetCNDEStatePeriod(int period)
    {
        if (period < 8 || period > 1000)
            return ERR_PARAM_VALUE;
        _robotStatePeriod = period;
        return ERR_SUCCESS;
    }

    public int GetCNDEStateConfig(out List<RobotState> states, out int period)
    {
        states = new List<RobotState>(_configStates);
        period = _robotStatePeriod;
        return ERR_SUCCESS;
    }

    // 启停控制
    public int SetCNDEStart()
    {
        if (_startSent)
        {
            Console.WriteLine("[SetCNDEStart] Already started, skip.");
            return 0;
        }
        lock (_recvLock)
        {
            Console.WriteLine("[SetCNDEStart] 开始发送 START 帧...");

            CNDE_PKG startPkg = new CNDE_PKG
            {
                Count = _sendCount++,
                Type = CNDEFrameType.START,
                Len = 0
            };
            byte[] frame = CNDEFrameHandle.CNDEPkgToFrame(startPkg);

            // 打印发送的 START 帧内容
            //Console.WriteLine($"[SetCNDEStart] 准备发送 {frame.Length} 字节:");
            StringBuilder hex = new StringBuilder();
            foreach (byte b in frame)
            {
                hex.Append(b.ToString("X2") + " ");
            }
            Console.WriteLine(hex.ToString());

            // 手动解析验证
            if (frame.Length >= 8)
            {
                ushort head = (ushort)(frame[0] | (frame[1] << 8));
                //Console.WriteLine($"[SetCNDEStart] 帧头: 0x{head:X4} (预期 0x5A5A)");
                //Console.WriteLine($"[SetCNDEStart] 帧计数: {frame[2]}");
                //Console.WriteLine($"[SetCNDEStart] 帧类型: {frame[3]}");
                ushort lenField = (ushort)(frame[4] | (frame[5] << 8));
                //Console.WriteLine($"[SetCNDEStart] 长度字段: {lenField} (预期 0)");
                ushort tail = (ushort)(frame[frame.Length - 2] | (frame[frame.Length - 1] << 8));
                //Console.WriteLine($"[SetCNDEStart] 帧尾: 0x{tail:X4} (预期 0xA5A5)");
            }

            int sendResult = _rtClient.Send(frame);
            if (sendResult != frame.Length)
            {
                //Console.WriteLine($"[SetCNDEStart] 发送失败，实际发送 {sendResult} 字节，预期 {frame.Length}");
                return -1;
            }
            //Console.WriteLine("[SetCNDEStart] 发送成功，等待机器人响应...");

            byte[] recvBuf = new byte[1024];
            DateTime startTime = DateTime.Now;
            int timeoutMs = 3000; // 3秒超时
            //Thread.Sleep(1000);
            while (_robotStateRunFlag && (DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                int len;
                try
                {
                    len = _rtClient.RecvCNDEPkg(recvBuf, recvBuf.Length);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"[SetCNDEStart] Recv 异常: {ex.Message}");
                    // 不直接返回，继续等待
                    Thread.Sleep(100);
                    continue;
                    //return -3;
                }
                if (len < 0)
                {
                    if (len == -2)
                    {
                        Console.WriteLine("[SetCNDEStart] 接收超时，继续等待...");
                        continue;
                    }    
                    Console.WriteLine($"[SetCNDEStart] 接收失败，错误码 {len}");
                    return -1;
                }
                if (len == 0)
                {
                    Console.WriteLine("[SetCNDEStart] 收到空数据（连接断开），重发 START 帧...");
                    _rtClient.Send(frame);
                    continue;
                }
                //Console.WriteLine($"[SetCNDEStart] 收到 {len} 字节数据");

                // 打印收到的原始数据
                StringBuilder recvHex = new StringBuilder();
                for (int i = 0; i < len; i++)
                    recvHex.Append(recvBuf[i].ToString("X2") + " ");
                //Console.WriteLine($"[SetCNDEStart] 接收数据: {recvHex.ToString()}");

                if (CNDEFrameHandle.FrameToCNDEPkg(recvBuf, out CNDE_PKG pkg) == 0)
                {
                    //Console.WriteLine($"[SetCNDEStart] 解析成功，帧类型: {pkg.Type}, 数据长度: {pkg.Data.Count}");

                    // 如果收到 OUTPUT_STATE 帧，说明机器人已经开始发送状态数据，认为启动成功
                    if (pkg.Type == CNDEFrameType.OUTPUT_STATE)
                    {
                        //Console.WriteLine("[SetCNDEStart] 收到 OUTPUT_STATE 帧，启动成功");
                        return 0;
                    }
                    // 如果收到 MESSAGE 帧
                    else if (pkg.Type == CNDEFrameType.MESSAGE)
                    {
                        if (pkg.Data.Count > 0 && pkg.Data[0] == 0x00)
                        {
                            Console.WriteLine("[SetCNDEStart] 机器人返回成功 (0x00)");
                            return 0;
                        }
                        else
                        {
                            Console.WriteLine($"[SetCNDEStart] 机器人返回失败，首字节 = {(pkg.Data.Count > 0 ? pkg.Data[0].ToString("X2") : "无数据")}");
                            return -2;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[SetCNDEStart] 机器人返回失败，首字节 = {(pkg.Data.Count > 0 ? pkg.Data[0].ToString("X2") : "无数据")}");
                        return -2;
                    }
                }
                else
                {
                    Console.WriteLine("[SetCNDEStart] 收到非 MESSAGE 帧，继续等待...");
                }
            }

            Console.WriteLine($"[SetCNDEStart] 等待响应超时 ({timeoutMs} ms)");
            return -3;
        }
    }


    public int SetCNDEStop()
    {
        lock (_recvLock)
        {
            CNDE_PKG stopPkg = new CNDE_PKG
            {
                Count = _sendCount++,
                Type = CNDEFrameType.STOP,
                Len = 0
            };
            byte[] frame = CNDEFrameHandle.CNDEPkgToFrame(stopPkg);
            if (_rtClient.Send(frame) != frame.Length) return -1;

            byte[] recvBuf = new byte[1024];
            while (_robotStateRunFlag)
            {
                int len = _rtClient.RecvCNDEPkg(recvBuf, recvBuf.Length);
                if (len < 0) return -1;
                if (len == 0) { _rtClient.Send(frame); continue; }
                if (CNDEFrameHandle.FrameToCNDEPkg(recvBuf, out CNDE_PKG pkg) == 0 && pkg.Type == CNDEFrameType.MESSAGE)
                {
                    if (pkg.Data.Count > 0 && pkg.Data[0] == 0x00)
                        return 0;
                    else
                        return -2;
                }
            }
            return 0;
        }
    }


    // 新增：发送输出配置帧（对应C++ SendCNDEOutputConfig）
    public int SendCNDEOutputConfig()
    {
        lock (_recvLock)
        {
            Console.WriteLine("[SendCNDEOutputConfig] 开始发送配置帧...");

            CNDE_PKG configPkg = new CNDE_PKG
            {
                Count = _sendCount++,
                Type = CNDEFrameType.OUTPUT_CONFIG,
                Len = 0
            };
            // 添加周期（2字节，小端）
            configPkg.Data.AddRange(BitConverter.GetBytes((ushort)_robotStatePeriod));
            Console.WriteLine($"[SendCNDEOutputConfig] 周期: {_robotStatePeriod} ms");

            // 构建字段名列表
            // 构建字段名列表（使用协议名）
            List<string> fieldNames = new List<string>();
            foreach (var state in _configStates)
            {
                if (_allStates.TryGetValue(state, out var meta))
                {
                    string csharpFieldName = meta.FieldName;  // 现在是 C# 字段名
                    if (CSharpToProtocolFieldMap.TryGetValue(csharpFieldName, out string protocolName))
                        fieldNames.Add(protocolName);
                    else
                        fieldNames.Add(csharpFieldName);  // 兼容未映射的字段
                }
            }
            string namesStr = string.Join(",", fieldNames);
            Console.WriteLine($"[SendCNDEOutputConfig] 请求的字段(协议名): {namesStr}");

            configPkg.Data.AddRange(System.Text.Encoding.ASCII.GetBytes(namesStr));
            configPkg.Len = (ushort)(2 + namesStr.Length); // 周期2字节 + 名称列表长度

            // 打印 CNDE_PKG 内部信息
            //Console.WriteLine($"[SendCNDEOutputConfig] CNDE_PKG 头: 0x{configPkg.Head:X4}, 计数: {configPkg.Count}, 类型: {configPkg.Type}, 长度字段: {configPkg.Len}, 数据长度: {configPkg.Data.Count}");

            byte[] frame = CNDEFrameHandle.CNDEPkgToFrame(configPkg);

            // 打印完整帧的十六进制
            //Console.WriteLine($"[SendCNDEOutputConfig] 准备发送 {frame.Length} 字节:");
            StringBuilder hex = new StringBuilder();
            foreach (byte b in frame)
            {
                hex.Append(b.ToString("X2") + " ");
            }
            Console.WriteLine(hex.ToString());

            // 手动解析帧头验证
            if (frame.Length >= 8)
            {
                ushort head = (ushort)(frame[0] | (frame[1] << 8));
                //Console.WriteLine($"[SendCNDEOutputConfig] 帧头: 0x{head:X4} (预期 0x5A5A)");
                //Console.WriteLine($"[SendCNDEOutputConfig] 帧计数: {frame[2]}");
                //Console.WriteLine($"[SendCNDEOutputConfig] 帧类型: {frame[3]}");
                ushort lenField = (ushort)(frame[4] | (frame[5] << 8));
                //Console.WriteLine($"[SendCNDEOutputConfig] 长度字段: {lenField} (预期 {configPkg.Len})");
                ushort tail = (ushort)(frame[frame.Length - 2] | (frame[frame.Length - 1] << 8));
                //Console.WriteLine($"[SendCNDEOutputConfig] 帧尾: 0x{tail:X4} (预期 0xA5A5)");
            }

            int sendResult = _rtClient.Send(frame);
            if (sendResult != frame.Length)
            {
                Console.WriteLine($"[SendCNDEOutputConfig] 发送失败，实际发送 {sendResult} 字节，预期 {frame.Length}");
                return -1;
            }
            Console.WriteLine("[SendCNDEOutputConfig] 发送成功，等待机器人响应...");

            byte[] recvBuf = new byte[1024];
            DateTime startTime = DateTime.Now;
            int timeoutMs = 3000;  // 3秒超时

            //Thread.Sleep(1000);
            while (_robotStateRunFlag && (DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                int len = _rtClient.RecvCNDEPkg(recvBuf, recvBuf.Length);
                if (len < 0)
                {
                    Console.WriteLine($"[SendCNDEOutputConfig] 接收失败，错误码 {len}");
                    return -1;
                }
                if (len == 0)
                {
                    Console.WriteLine("[SendCNDEOutputConfig] 收到空数据（连接断开），重发配置帧...");
                    _rtClient.Send(frame);
                    continue;
                }
                //Console.WriteLine($"[SendCNDEOutputConfig] 收到 {len} 字节数据");
                //Console.WriteLine($"[SendCNDEOutputConfig] 原始数据: {BitConverter.ToString(recvBuf, 0, len)}");
                int ret = CNDEFrameHandle.FrameToCNDEPkg(recvBuf, out CNDE_PKG pkg);
                //Console.WriteLine($"[SendCNDEOutputConfig] FrameToCNDEPkg ret: {ret}");
                if ( ret == 0 && pkg.Type == CNDEFrameType.MESSAGE)
                {

                    Console.WriteLine($"[SendCNDEOutputConfig] 收到 MESSAGE 帧，数据长度 {pkg.Data.Count}");
                    if (pkg.Data.Count > 0 && pkg.Data[0] == 0x00)
                    {
                        Console.WriteLine("[SendCNDEOutputConfig] 机器人返回成功 (0x00)");
                        return 0;
                    }
                    else
                    {
                        Console.WriteLine($"[SendCNDEOutputConfig] 机器人返回失败，首字节 = {(pkg.Data.Count > 0 ? pkg.Data[0].ToString("X2") : "无数据")}");
                        // 解析错误信息（机器人返回的字符串，以 ASCII 编码）
                        string errorMsg = "";
                        if (pkg.Data.Count > 0)
                        {
                            // 跳过首字节（错误码），剩余部分为文本
                            byte[] textBytes = pkg.Data.Skip(1).ToArray();
                            errorMsg = Encoding.ASCII.GetString(textBytes).TrimEnd('\0');
                        }
                        Console.WriteLine($"[SendCNDEOutputConfig] 机器人返回失败，错误码 = {(pkg.Data.Count > 0 ? pkg.Data[0].ToString("X2") : "无数据")}, 详情: {errorMsg}");

                        // 根据错误内容进行特定处理
                        if (errorMsg.Contains("NOT_FOUND") || errorMsg.Contains("unknown field"))
                        {
                            // 尝试找出哪个字段无效，并从配置中移除
                            // 注意：此处需要获取发送的字段名列表，可以从 namesStr 或重新构建
                            // 简化示例：记录错误并返回特定错误码
                            Console.WriteLine("[SendCNDEOutputConfig] 检测到无效字段，请检查配置的状态列表");
                            // 可以在这里触发重新配置（例如移除无效字段后重试），但注意防止无限递归
                            return (int)RobotError.ERR_STATE_INVALID;  // 自定义错误码：字段不存在
                        }
                       
                        return (int)RobotError.ERR_SOCKET_COM_FAILED;
                    }
                }
                else
                {
                    Console.WriteLine("[SendCNDEOutputConfig] 收到非 MESSAGE 帧，继续等待...");
                }
            }

            Console.WriteLine($"[SendCNDEOutputConfig] 等待响应超时 ({timeoutMs} ms)");
            return -3;
        }
    }


    //private void RecvRobotStateThread()
    //{
    //    byte[] pkgBuf = new byte[4096];
    //    DateTime lastReceiveTime = DateTime.Now;
    //    int frameCount = 0;

    //    while (_robotStateRunFlag)
    //    {
    //        try
    //        {
    //            int recvLen;
    //            lock (_recvLock)
    //            {
    //                recvLen = _rtClient.RecvCNDEPkg(pkgBuf, pkgBuf.Length);
    //            }

    //            if (recvLen <= 0)  // 连接断开或没有数据
    //            {
    //                if (!_robotStateRunFlag) break; // 已要求退出，直接结束线程
    //                Console.WriteLine("[RecvRobotStateThread] 连接断开，尝试重连...");
    //                if (_rtClient.ReConnect())
    //                {
    //                    _startSent = false;
    //                    // 重连成功后重新发送配置帧和启动帧
    //                    if (SendCNDEOutputConfig() == 0 && SetCNDEStart() == 0)//SendCNDEOutputConfig() == 0 &&  重连后只发start
    //                    {
    //                        Console.WriteLine("[RecvRobotStateThread] 重连并重新配置成功");
    //                        _errorCallback?.Invoke((int)RobotError.ERR_SUCCESS);
    //                    }
    //                    else
    //                    {
    //                        Console.WriteLine("[RecvRobotStateThread] 重连后配置失败");
    //                        _errorCallback?.Invoke((int)RobotError.ERR_SOCKET_COM_FAILED);
    //                        break;
    //                    }
    //                }
    //                else
    //                {
    //                    Console.WriteLine("[RecvRobotStateThread] 重连失败");
    //                    _errorCallback?.Invoke((int)RobotError.ERR_SOCKET_COM_FAILED);
    //                    break;
    //                }
    //            }
    //            else  // 收到数据
    //            {
    //                lock (_recvLock)
    //                {
    //                    StringBuilder hex = new StringBuilder();
    //                    for (int i = 0; i < recvLen; i++)
    //                        hex.Append(pkgBuf[i].ToString("X2") + " ");
    //                    //Console.WriteLine($"[RecvRobotStateThread] 原始数据: {hex.ToString()}");
    //                    // 创建有效数据副本
    //                    byte[] validData = new byte[recvLen];
    //                    Array.Copy(pkgBuf, validData, recvLen);

    //                    if (CNDEFrameHandle.FrameToCNDEPkg(validData, out CNDE_PKG pkg) == 0 && pkg.Type == CNDEFrameType.OUTPUT_STATE)
    //                    {
    //                        frameCount++;
    //                        var now = DateTime.Now;
    //                        var interval = (now - lastReceiveTime).TotalMilliseconds;
    //                        //Console.WriteLine($"[Recv] Frame #{frameCount} received, interval = {interval:F1} ms");
    //                        lastReceiveTime = now;

    //                        // 成功收到一帧数据，清除错误标志
    //                        _errorCallback?.Invoke((int)RobotError.ERR_SUCCESS);

    //                        ParseRobotState(pkg.Data);
    //                    }
    //                    else
    //                    {
    //                        Console.WriteLine("[RecvRobotStateThread] 帧解析失败或不是 OUTPUT_STATE 帧");
    //                        // 解析失败不一定是严重错误，不重置错误标志，也不断开
    //                    }
    //                    Array.Clear(pkgBuf, 0, pkgBuf.Length);
    //                }
    //            }
    //        }
    //        catch (SocketException ex)
    //        {
    //            Console.WriteLine($"[RecvRobotStateThread] Socket异常: {ex.Message}");
    //            // 尝试重连
    //            if (_rtClient.ReConnect())
    //            {
    //                _startSent = false;
    //                if (SendCNDEOutputConfig() == 0 && SetCNDEStart() == 0)
    //                {
    //                    Console.WriteLine("[RecvRobotStateThread] Socket异常后重连成功");
    //                    _errorCallback?.Invoke((int)RobotError.ERR_SUCCESS);
    //                }
    //                else
    //                {
    //                    Console.WriteLine("[RecvRobotStateThread] 重连后配置失败");
    //                    _errorCallback?.Invoke((int)RobotError.ERR_SOCKET_COM_FAILED);
    //                    break;
    //                }
    //            }
    //            else
    //            {
    //                Console.WriteLine("[RecvRobotStateThread] Socket异常后重连失败");
    //                _errorCallback?.Invoke((int)RobotError.ERR_SOCKET_COM_FAILED);
    //                break;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"[RecvRobotStateThread] 未知异常: {ex.Message}");
    //            // 尝试重连
    //            if (_rtClient.ReConnect())
    //            {
    //                _startSent = false;
    //                if (SendCNDEOutputConfig() == 0 && SetCNDEStart() == 0)
    //                {
    //                    Console.WriteLine("[RecvRobotStateThread] 异常后重连成功");
    //                    _errorCallback?.Invoke((int)RobotError.ERR_SUCCESS);
    //                }
    //                else
    //                {
    //                    Console.WriteLine("[RecvRobotStateThread] 重连后配置失败");
    //                    _errorCallback?.Invoke((int)RobotError.ERR_SOCKET_COM_FAILED);
    //                    break;
    //                }
    //            }
    //            else
    //            {
    //                Console.WriteLine("[RecvRobotStateThread] 异常后重连失败");
    //                _errorCallback?.Invoke((int)RobotError.ERR_SOCKET_COM_FAILED);
    //                break;
    //            }
    //        }
    //    }
    //    _rtClient.Close();
    //}

    private void RecvRobotStateThread()
    {
        byte[] pkgBuf = new byte[4096];
        DateTime lastReceiveTime = DateTime.Now;
        int frameCount = 0;

        while (_robotStateRunFlag)
        {
            try
            {
                int recvLen;
                lock (_recvLock)
                {
                    recvLen = _rtClient.RecvCNDEPkg(pkgBuf, pkgBuf.Length);
                }

                if (recvLen <= 0)  // 连接断开或没有数据
                {
                    if (!_robotStateRunFlag) break;

                    Console.WriteLine("[RecvRobotStateThread] 连接断开，尝试重连...");

                    // 重连+重新配置的重试循环
                    bool reconfigured = false;
                    const int maxReconfigRetries = 5;  // 最多重试5次

                    for (int retry = 0; retry < maxReconfigRetries && !reconfigured; retry++)
                    {
                        if (_rtClient.ReConnect())
                        {
                            _startSent = false;
                            // 重连成功后重新发送配置帧和启动帧
                            if ( SetCNDEStart() == 0) //SendCNDEOutputConfig() == 0 &&
                            {
                                reconfigured = true;
                                Console.WriteLine("[RecvRobotStateThread] 重连并重新配置成功");
                                _errorCallback?.Invoke((int)RobotError.ERR_SUCCESS);
                                break;
                            }
                            else
                            {
                                Console.WriteLine($"[RecvRobotStateThread] 重连后配置失败，重试 {retry + 1}/{maxReconfigRetries}");
                                // 配置失败，关闭当前连接（下次循环会重新 ReConnect）
                                _rtClient.Close();
                                Thread.Sleep(500);  // 等待后重试
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[RecvRobotStateThread] 重连失败，重试 {retry + 1}/{maxReconfigRetries}");
                            Thread.Sleep(500);
                        }
                    }

                    if (!reconfigured)
                    {
                        Console.WriteLine("[RecvRobotStateThread] 多次重连并重配置均失败，退出线程");
                        _errorCallback?.Invoke((int)RobotError.ERR_SOCKET_COM_FAILED);
                        break;
                    }
                    // 重配置成功，继续循环接收数据
                    continue;
                }
                else  // 收到数据
                {
                    lock (_recvLock)
                    {
                        // 创建有效数据副本
                        byte[] validData = new byte[recvLen];
                        Array.Copy(pkgBuf, validData, recvLen);

                        if (CNDEFrameHandle.FrameToCNDEPkg(validData, out CNDE_PKG pkg) == 0 && pkg.Type == CNDEFrameType.OUTPUT_STATE)
                        {
                            frameCount++;
                            lastReceiveTime = DateTime.Now;
                            _errorCallback?.Invoke((int)RobotError.ERR_SUCCESS);
                            ParseRobotState(pkg.Data);
                        }
                        else
                        {
                            Console.WriteLine("[RecvRobotStateThread] 帧解析失败或不是 OUTPUT_STATE 帧");
                        }
                        Array.Clear(pkgBuf, 0, pkgBuf.Length);
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[RecvRobotStateThread] Socket异常: {ex.Message}");
                // 尝试重连+重配置（复用上面的逻辑，或简单处理）
                bool reconfigured = false;
                const int maxRetries = 3;
                for (int i = 0; i < maxRetries && !reconfigured; i++)
                {
                    if (_rtClient.ReConnect())
                    {
                        _startSent = false;
                        if (SendCNDEOutputConfig() == 0 && SetCNDEStart() == 0)
                        {
                            reconfigured = true;
                            _errorCallback?.Invoke((int)RobotError.ERR_SUCCESS);
                            break;
                        }
                    }
                    Thread.Sleep(500);
                }
                if (!reconfigured)
                {
                    _errorCallback?.Invoke((int)RobotError.ERR_SOCKET_COM_FAILED);
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RecvRobotStateThread] 未知异常: {ex.Message}");
                // 同样尝试重连
                if (_rtClient.ReConnect())
                {
                    _startSent = false;
                    if (SendCNDEOutputConfig() == 0 && SetCNDEStart() == 0)
                    {
                        _errorCallback?.Invoke((int)RobotError.ERR_SUCCESS);
                        continue;
                    }
                }
                _errorCallback?.Invoke((int)RobotError.ERR_SOCKET_COM_FAILED);
                break;
            }
        }
        _rtClient.Close();
    }

    private void ParseRobotState(List<byte> data)
    {
        int offset = 0;
        //Console.WriteLine($"[ParseRobotState] Total data bytes = {data.Count}");
        foreach (var state in _configStates)
        {
            if (!_allStates.TryGetValue(state, out var meta))
                continue;

            int size = GetTypeSize(meta.CndeType);
            //Console.WriteLine($"[Parse] State {state}: expected size = {size}, current offset = {offset}");

            if (offset + size > data.Count)
            {
                Console.WriteLine($"[Parse] ERROR: offset+size exceeds data length!");
                break;
            }

            byte[] raw = data.GetRange(offset, size).ToArray();
            //Console.WriteLine($"[Parse] Raw data ({size} bytes): {BitConverter.ToString(raw)}");

            offset += size;
            SetStateValue(meta, raw);

            //if (state == RobotState.SpeedScaleManual)
            //{
            //    double val = BitConverter.ToDouble(raw, 0);
            //    Console.WriteLine($"SpeedScaleManual raw double: {val}");
            //}
        }
    }

    private void SetStateValue((string FieldName, int Offset, string StructType, string CndeType) meta, byte[] raw)
    {
        var field = typeof(ROBOT_STATE_PKG).GetField(meta.FieldName);
        if (field == null) return;

        object value = ConvertCndeToStruct(raw, meta.CndeType, meta.StructType, field.FieldType);
        if (value != null)
            field.SetValue(_robotStatePkg, value);
    }

    private object ConvertCndeToStruct(byte[] raw, string cndeType, string structType, Type targetType)
    {
        // 1. 优先处理扩展轴状态数组 (EXT_AXIS_STATUS[])
        if (targetType == typeof(EXT_AXIS_STATUS[]))
        {
            int elemSize = Marshal.SizeOf(typeof(EXT_AXIS_STATUS));
            if (elemSize == 0) return null;
            const int maxAxes = 4;  // 最多支持4个扩展轴
            int actualCount = raw.Length / elemSize;
            actualCount = Math.Min(actualCount, maxAxes);
            EXT_AXIS_STATUS[] arr = new EXT_AXIS_STATUS[maxAxes];
            for (int i = 0; i < actualCount; i++)
            {
                IntPtr ptr = Marshal.AllocHGlobal(elemSize);
                try
                {
                    Marshal.Copy(raw, i * elemSize, ptr, elemSize);
                    arr[i] = (EXT_AXIS_STATUS)Marshal.PtrToStructure(ptr, typeof(EXT_AXIS_STATUS));
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
            // 其余元素保持默认值 (0)
            return arr;
        }

        // ========== 新增：处理复合类型（如 "DOUBLE,DOUBLE" → UINT16_2） ==========
        if (cndeType.Contains(",") && targetType.IsArray && structType.StartsWith("UINT16_"))
        {
            // 解析数组长度，如 "UINT16_2" → 2
            int arrayLen = int.Parse(structType.Substring(7));
            ushort[] result = new ushort[arrayLen];

            string[] cndeParts = cndeType.Split(',');
            int offset = 0;
            for (int i = 0; i < arrayLen && i < cndeParts.Length; i++)
            {
                int partSize = GetTypeSize(cndeParts[i]);
                if (offset + partSize > raw.Length) break;

                byte[] partRaw = new byte[partSize];
                Array.Copy(raw, offset, partRaw, 0, partSize);

                // 根据 CNDE 类型转换为 double，再强转为 ushort
                if (cndeParts[i] == "DOUBLE")
                {
                    double val = BitConverter.ToDouble(partRaw, 0);  // 沿用原有字节序假设
                    result[i] = (ushort)val;
                }
                else if (cndeParts[i] == "UINT16")
                {
                    ushort val = BitConverter.ToUInt16(partRaw, 0);
                    result[i] = val;
                }
                // 可根据需要扩展其他类型，如 INT32 等
                offset += partSize;
            }
            return result;
        }
        // 基础类型
        if (cndeType == "UINT8" && structType == "UINT8") return raw[0];
        if (cndeType == "UINT8" && structType == "INT32") return (int)raw[0];
        if (cndeType == "INT8" && structType == "INT32") return (int)(sbyte)raw[0];
        if (cndeType == "UINT16" && structType == "UINT16") return BitConverter.ToUInt16(raw, 0);
        if (cndeType == "UINT16" && structType == "INT32") return (int)BitConverter.ToUInt16(raw, 0);
        if (cndeType == "INT32" && structType == "INT32") return BitConverter.ToInt32(raw, 0);
        if (cndeType == "INT32" && structType == "UINT8") return (byte)BitConverter.ToInt32(raw, 0);
        if (cndeType == "INT32" && structType == "UINT16") return (ushort)BitConverter.ToInt32(raw, 0);
        if (cndeType == "INT32" && structType == "UINT32") return (uint)BitConverter.ToInt32(raw, 0);
        if (cndeType == "UINT32" && structType == "INT32") return (int)BitConverter.ToUInt32(raw, 0);
        if (cndeType == "DOUBLE" && structType == "DOUBLE") return BitConverter.ToDouble(raw, 0);
        if (cndeType == "DOUBLE" && structType == "FLOAT") return (float)BitConverter.ToDouble(raw, 0);
        if (cndeType == "DOUBLE" && structType == "UINT16") return (ushort)BitConverter.ToDouble(raw, 0);
        if (cndeType == "FLOAT" && structType == "DOUBLE") return (double)BitConverter.ToSingle(raw, 0);
        if (cndeType == "FLOAT" && structType == "FLOAT") return BitConverter.ToSingle(raw, 0);

        // 数组类型
        if (cndeType.StartsWith("DOUBLE_") && structType.StartsWith("DOUBLE_"))
        {
            int count = int.Parse(cndeType.Substring(7));
            double[] arr = new double[count];
            for (int i = 0; i < count; i++)
                arr[i] = BitConverter.ToDouble(raw, i * 8);
            return arr;
        }
        if (cndeType.StartsWith("UINT16_") && structType.StartsWith("UINT16_"))
        {
            int count = int.Parse(cndeType.Substring(7));
            ushort[] arr = new ushort[count];
            for (int i = 0; i < count; i++)
                arr[i] = BitConverter.ToUInt16(raw, i * 2);
            return arr;
        }
        if (cndeType.StartsWith("UINT8_") && structType.StartsWith("UINT8_"))
        {
            int count = int.Parse(cndeType.Substring(6));
            byte[] arr = new byte[count];
            Array.Copy(raw, arr, count);
            return arr;
        }
        if (cndeType.StartsWith("INT32_") && structType.StartsWith("INT32_"))
        {
            int count = int.Parse(cndeType.Substring(6));
            int[] arr = new int[count];
            for (int i = 0; i < count; i++)
                arr[i] = BitConverter.ToInt32(raw, i * 4);
            return arr;
        }
        if (cndeType.StartsWith("INT32_") && structType.StartsWith("UINT16_"))
        {
            int count = int.Parse(cndeType.Substring(6));
            ushort[] arr = new ushort[count];
            for (int i = 0; i < count; i++)
                arr[i] = (ushort)BitConverter.ToInt32(raw, i * 4);
            return arr;
        }

        if (targetType == typeof(ROBOT_TIME))
        {
            // 机器人实际返回的是 7 个 int32，共 28 字节
            if (raw.Length < 28) return null;
            int yearInt = BitConverter.ToInt32(raw, 0);
            int monthInt = BitConverter.ToInt32(raw, 4);
            int dayInt = BitConverter.ToInt32(raw, 8);
            int hourInt = BitConverter.ToInt32(raw, 12);
            int minuteInt = BitConverter.ToInt32(raw, 16);
            int secondInt = BitConverter.ToInt32(raw, 20);
            int millisecondInt = BitConverter.ToInt32(raw, 24);
            return new ROBOT_TIME
            {
                year = (ushort)yearInt,
                mouth = (byte)monthInt,
                day = (byte)dayInt,
                hour = (byte)hourInt,
                minute = (byte)minuteInt,
                second = (byte)secondInt,
                millisecond = (ushort)millisecondInt
            };
        }

        // ROBOT_AUX_STATE
        if (targetType == typeof(ROBOT_AUX_STATE))
        {
            int off = 0;
            byte servoId = raw[off++];
            int servoErrCode = BitConverter.ToInt32(raw, off); off += 4;
            int servoState = BitConverter.ToInt32(raw, off); off += 4;
            double servoPos = BitConverter.ToDouble(raw, off); off += 8;
            float servoVel = BitConverter.ToSingle(raw, off); off += 4;
            float servoTorque = BitConverter.ToSingle(raw, off);
            return new ROBOT_AUX_STATE
            {
                servoId = servoId,
                servoErrCode = servoErrCode,
                servoState = servoState,
                servoPos = servoPos,
                servoVel = servoVel,
                servoTorque = servoTorque
            };
        }

        // WELDING_BREAKOFF_STATE
        if (targetType == typeof(WELDING_BREAKOFF_STATE))
        {
            return new WELDING_BREAKOFF_STATE
            {
                breakOffState = raw[0],
                weldArcState = raw[1]
            };
        }


        // EXT_AXIS_STATUS 等复杂类型可后续补充
        return null;
    }

    private int GetTypeSize(string typeStr)
    {
        if (string.IsNullOrEmpty(typeStr)) return 0;

        // 新增：处理复合类型，如 "DOUBLE,DOUBLE"
        if (typeStr.Contains(","))
        {
            int total = 0;
            foreach (var part in typeStr.Split(','))
                total += GetTypeSize(part.Trim());
            return total;
        }

        if (typeStr == "UINT8" || typeStr == "INT8") return 1;
        if (typeStr == "UINT16") return 2;
        if (typeStr == "INT32" || typeStr == "UINT32" || typeStr == "FLOAT") return 4;
        if (typeStr == "DOUBLE") return 8;
        if (typeStr.StartsWith("UINT8_")) return int.Parse(typeStr.Substring(6));
        if (typeStr.StartsWith("UINT16_")) return 2 * int.Parse(typeStr.Substring(7));
        if (typeStr.StartsWith("DOUBLE_")) return 8 * int.Parse(typeStr.Substring(7));
        if (typeStr.StartsWith("INT32_")) return 4 * int.Parse(typeStr.Substring(6));
        if (typeStr.Contains(",")) // 复合类型
        {
            int total = 0;
            foreach (var part in typeStr.Split(','))
                total += GetTypeSize(part.Trim());
            return total;
        }
        return 0;
    }

    private void ClearAllFields()
    {
        var type = typeof(ROBOT_STATE_PKG);
        var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var field in fields)
        {
            // 为每种类型设置默认值
            if (field.FieldType == typeof(double))
                field.SetValue(_robotStatePkg, 0.0);
            else if (field.FieldType == typeof(int))
                field.SetValue(_robotStatePkg, 0);
            else if (field.FieldType == typeof(byte))
                field.SetValue(_robotStatePkg, (byte)0);
            else if (field.FieldType == typeof(ushort))
                field.SetValue(_robotStatePkg, (ushort)0);
            else if (field.FieldType == typeof(float))
                field.SetValue(_robotStatePkg, 0.0f);
            else if (field.FieldType == typeof(sbyte))
                field.SetValue(_robotStatePkg, (sbyte)0);
            else if (field.FieldType == typeof(uint))
                field.SetValue(_robotStatePkg, 0U);
            else if (field.FieldType == typeof(ROBOT_TIME))
                field.SetValue(_robotStatePkg, new ROBOT_TIME());
            else if (field.FieldType == typeof(ROBOT_AUX_STATE))
                field.SetValue(_robotStatePkg, new ROBOT_AUX_STATE());
            else if (field.FieldType == typeof(WELDING_BREAKOFF_STATE))
                field.SetValue(_robotStatePkg, new WELDING_BREAKOFF_STATE());
            else if (field.FieldType.IsArray)
            {
                var array = field.GetValue(_robotStatePkg) as Array;
                if (array != null)
                {
                    // 清零数组元素
                    for (int i = 0; i < array.Length; i++)
                    {
                        var elem = array.GetValue(i);
                        if (elem != null)
                        {
                            if (elem.GetType().IsValueType)
                                array.SetValue(Activator.CreateInstance(elem.GetType()), i);
                            else if (elem is double) array.SetValue(0.0, i);
                            else if (elem is int) array.SetValue(0, i);
                            else if (elem is byte) array.SetValue((byte)0, i);
                            else if (elem is ushort) array.SetValue((ushort)0, i);
                            else if (elem is float) array.SetValue(0.0f, i);
                            else if (elem is sbyte) array.SetValue((sbyte)0, i);
                            else if (elem is uint) array.SetValue(0U, i);
                            else if (elem is EXT_AXIS_STATUS) array.SetValue(new EXT_AXIS_STATUS(), i);
                        }
                    }
                }
            }
        }
    }


}
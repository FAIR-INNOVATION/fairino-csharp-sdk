using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
//using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace fairino
{
    [XmlRpcUrl("http://192.168.58.2:20003/RPC2")]
    public interface ICallSupervisor : IXmlRpcProxy
    {
        /**
        * @brief  与机器人控制器建立通讯
        * @param  [in] ip  控制器IP地址，出场默认为192.168.58.2
        * @return 错误码
        */
        [XmlRpcMethod("RPC")]
        int RPC(string ip);

        /**
        * @brief  与机器人控制器关闭通讯
        * @return 错误码
        */
        [XmlRpcMethod("CloseRPC")]
        int CloseRPC();

        /**
         * @brief  获取控制器IP
         * @param  [out] ip  控制器IP
         * @return  错误码
         */
        [XmlRpcMethod("GetControllerIP")]
        object[] GetControllerIP(string ip);

        /**
        * @brief  控制机器人进入或退出拖动示教模式
        * @param  [in] state 0-退出拖动示教模式，1-进入拖动示教模式
        * @return  错误码
        */
        [XmlRpcMethod("DragTeachSwitch")]
        int DragTeachSwitch(int state);

        /**
        * @brief  控制机器人上使能或下使能，机器人上电后默认自动上使能
        * @param  [in] state  0-下使能，1-上使能
        * @return  错误码
        */
        [XmlRpcMethod("RobotEnable")]
        int RobotEnable(int state);


        /**
        * @brief 开始奇异位姿保护
        * @param [in] protectMode 奇异保护模式，0：关节模式；1-笛卡尔模式
        * @param [in] minShoulderPos 肩奇异调整范围(mm), 默认100
        * @param [in] minElbowPos 肘奇异调整范围(mm), 默认50
        * @param [in] minWristPos 腕奇异调整范围(°), 默认10
        * @return 错误码
        */
        [XmlRpcMethod("SingularAvoidStart")]
        int SingularAvoidStart(int protectMode, double minShoulderPos, double minElbowPos, double minWristPos);


        /**
        * @brief 停止奇异位姿保护
        * @return 错误码
        */
        [XmlRpcMethod("SingularAvoidEnd")]
        int SingularAvoidEnd();

        /**
        * @brief 控制机器人手自动模式切换
        * @param [in] mode 0-自动模式，1-手动模式
        * @return 错误码
        */
        [XmlRpcMethod("Mode")]
        int Mode(int mode);

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
        [XmlRpcMethod("StartJOG")]
        int StartJOG(int refType, int nb, int dir, double vel, double acc, double max_dis);

        /**
        * @brief  jog点动减速停止
        * @param  [in]  refType  1-关节点动停止，3-基坐标系下点动停止，5-工具坐标系下点动停止，9-工件坐标系下点动停止
        * @return  错误码
        */
        [XmlRpcMethod("StopJOG")]
        int StopJOG(int refType);

        /**
        * @brief jog点动立即停止
        * @return  错误码
        */
        [XmlRpcMethod("ImmStopJOG")]
        int ImmStopJOG();

        /**
        * @brief  关节空间运动
        * @param  [in] joint_pos  目标关节位置,单位deg
        * @param  [in] desc_pos   目标笛卡尔位姿
        * @param  [in] tool  工具坐标号，范围[1~15]
        * @param  [in] user  工件坐标号，范围[1~15]
        * @param  [in] vel  速度百分比，范围[0~100]
        * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
        * @param  [in] ovl  速度缩放因子，范围[0~100]
        * @param  [in] epos  扩展轴位置，单位mm
        * @param  [in] blendT [-1.0]-运动到位(阻塞)，[0~500.0]-平滑时间(非阻塞)，单位ms
        * @param  [in] offset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
        * @param  [in] offset_pos  位姿偏移量
        * @return  错误码
        */
        [XmlRpcMethod("MoveJ")]
        int MoveJ(double[] joint_pos, double[] desc_pos, int tool, int user, double vel, double acc, double ovl, double[] epos, double blendT, int offset_flag, double[] offset_pos);


        /**
        * @brief  笛卡尔空间直线运动
        * @param  [in] joint_pos  目标关节位置,单位deg
        * @param  [in] desc_pos   目标笛卡尔位姿
        * @param  [in] tool  工具坐标号，范围[1~15]
        * @param  [in] user  工件坐标号，范围[1~15]
        * @param  [in] vel  速度百分比，范围[0~100]
        * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
        * @param  [in] ovl  速度缩放因子，范围[0~100]
        * @param  [in] blendR [-1.0]-运动到位(阻塞)，[0~1000.0]-平滑半径(非阻塞)，单位mm
        * @param  [in] epos  扩展轴位置，单位mm
        * @param  [in] search  0-不焊丝寻位，1-焊丝寻位
        * @param  [in] offset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
        * @param  [in] offset_pos  位姿偏移量
        * @return  错误码
        */
        [XmlRpcMethod("MoveL")]
        int MoveL(double[] joint_pos, double[] desc_pos, int tool, int user, double vel, double acc, double ovl, double blendR, double[] epos, int search, int offset_flag, double[] offset_pos);

        /**
        * @brief  笛卡尔空间直线运动
        * @param  [in] joint_pos  目标关节位置,单位deg
        * @param  [in] desc_pos   目标笛卡尔位姿
        * @param  [in] tool  工具坐标号，范围[1~15]
        * @param  [in] user  工件坐标号，范围[1~15]
        * @param  [in] vel  速度百分比，范围[0~100]
        * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
        * @param  [in] ovl  速度缩放因子，范围[0~100]
        * @param  [in] blendR [-1.0]-运动到位(阻塞)，[0~1000.0]-平滑半径(非阻塞)，单位mm
        * @param  [in] blendMode 过渡方式；0-内切过渡；1-角点过渡
        * @param  [in] epos  扩展轴位置，单位mm
        * @param  [in] search  0-不焊丝寻位，1-焊丝寻位
        * @param  [in] offset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
        * @param  [in] offset_pos  位姿偏移量
        * @return  错误码
        */
        [XmlRpcMethod("MoveL")]
        int MoveL(double[] joint_pos, double[] desc_pos, int tool, int user, double vel, double acc, double ovl, double blendR, int blendMode, double[] epos, int search, int offset_flag, double[] offset_pos);

        /**
        * @brief  笛卡尔空间圆弧运动
        * @param  [in] joint_pos_p  路径点关节位置,单位deg
        * @param  [in] desc_pos_p   路径点笛卡尔位姿
        * @param  [in] controlP  路径点 工具坐标号，范围[1~15]工件坐标号，范围[1~15]速度百分比，范围[0~100]加速度百分比，范围[0~100],暂不开放
        * @param  [in] epos_p  扩展轴位置，单位mm
        * @param  [in] poffset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
        * @param  [in] offset_pos_p  位姿偏移量
        * @param  [in] joint_pos_t  目标点关节位置,单位deg
        * @param  [in] desc_pos_t   目标点笛卡尔位姿
        * @param  [in] controlT  路径点 工具坐标号，范围[1~15]工件坐标号，范围[1~15]速度百分比，范围[0~100]加速度百分比，范围[0~100],暂不开放
        * @param  [in] epos_t  扩展轴位置，单位mm
        * @param  [in] toffset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
        * @param  [in] offset_pos_t  位姿偏移量	 
        * @param  [in] ovl  速度缩放因子，范围[0~100]	 
        * @param  [in] blendR [-1.0]-运动到位(阻塞)，[0~1000.0]-平滑半径(非阻塞)，单位mm	 
        * @return  错误码
        */
        [XmlRpcMethod("MoveC")]
        int MoveC(double[] joint_pos_p, double[] desc_pos_p, double[] controlP, double[] epos_p, int poffset_flag, double[] offset_pos_p, double[] joint_pos_t, double[] desc_pos_t, double[] controlT, double[] epos_t, int toffset_flag, double[] offset_pos_t, double ovl, double blendR);

        /**
        * @brief  笛卡尔空间整圆运动
        * @param  [in] joint_pos_p  路径点1关节位置,单位deg
        * @param  [in] desc_pos_p   路径点1笛卡尔位姿
        * @param  [in] controlP  路径点 工具坐标号，范围[1~15]工件坐标号，范围[1~15]速度百分比，范围[0~100]加速度百分比，范围[0~100],暂不开放
        * @param  [in] epos_p  扩展轴位置，单位mm
        * @param  [in] joint_pos_t  路径点2关节位置,单位deg
        * @param  [in] desc_pos_t   路径点2笛卡尔位姿
        * @param  [in] controlT  路径点 工具坐标号，范围[1~15]工件坐标号，范围[1~15]速度百分比，范围[0~100]加速度百分比，范围[0~100],暂不开放
        * @param  [in] epos_t  扩展轴位置，单位mm
        * @param  [in] ovl  速度缩放因子，范围[0~100]	
        * @param  [in] offset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
        * @param  [in] offset_pos  位姿偏移量	 	 
        * @return  错误码
        */
        [XmlRpcMethod("Circle")]
        int Circle(double[] joint_pos_p, double[] desc_pos_p, double[] controlP, double[] epos_p, double[] joint_pos_t, double[] desc_pos_t, double[] controlT, double[] epos_t, double ovl, int offset_flag, double[] offset_pos);
        /**
        * @brief  笛卡尔空间螺旋线运动
        * @param  [in] joint_pos  目标关节位置,单位deg
        * @param  [in] desc_pos   目标笛卡尔位姿
        * @param  [in] tool  工具坐标号，范围[1~15]
        * @param  [in] user  工件坐标号，范围[1~15]
        * @param  [in] vel  速度百分比，范围[0~100]
        * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
        * @param  [in] epos  扩展轴位置，单位mm
        * @param  [in] ovl  速度缩放因子，范围[0~100]	 
        * @param  [in] offset_flag  0-不偏移，1-基坐标系/工件坐标系下偏移，2-工具坐标系下偏移
        * @param  [in] offset_pos  位姿偏移量
        * @param  [in] spiral_param  螺旋参数
        * @return  错误码
        */
        [XmlRpcMethod("NewSpiral")]
        int NewSpiral(double[] joint_pos, double[] desc_pos, int tool, int user, double vel, double acc, double[] epos, double ovl, int offset_flag, double[] offset_pos, double[] spiral_param);


        /**
        * @brief 伺服运动开始，配合ServoJ、ServoCart指令使用
        * @return  错误码
        */[XmlRpcMethod("ServoMoveStart")]
        int ServoMoveStart();

        /**
        * @brief 伺服运动结束，配合ServoJ、ServoCart指令使用
        * @return  错误码
        */[XmlRpcMethod("ServoMoveEnd")]
        int ServoMoveEnd();

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
        [XmlRpcMethod("ServoJ")]
        int ServoJ(double[] joint_pos, double[] axisPos, double acc, double vel, double cmdT, double filterT, double gain);

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
        */[XmlRpcMethod("ServoCart")]
        int ServoCart(int mode, double[] desc_pose, double[] pos_gain, double acc, double vel, double cmdT, double filterT, double gain);

        /**
        * @brief  笛卡尔空间点到点运动
        * @param  [in]  desc_pos  目标笛卡尔位姿或位姿增量
        * @param  [in] tool  工具坐标号，范围[1~15]
        * @param  [in] user  工件坐标号，范围[1~15]
        * @param  [in] vel  速度百分比，范围[0~100]
        * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
        * @param  [in] ovl  速度缩放因子，范围[0~100]
        * @param  [in] blendT [-1.0]-运动到位(阻塞)，[0~500.0]-平滑时间(非阻塞)，单位ms	
        * @param  [in] config  关节空间配置，[-1]-参考当前关节位置解算，[0~7]-参考特定关节空间配置解算，默认为-1	 
        * @return  错误码
        */[XmlRpcMethod("MoveCart")]
        int MoveCart(double[] desc_pos, int tool, int user, double vel, double acc, double ovl, double blendT, int config);

        /**
        * @brief  样条运动开始
        * @return  错误码
        */[XmlRpcMethod("SplineStart")]
        int SplineStart();

        /**
        * @brief  关节空间样条运动
        * @param  [in] joint_pos  目标关节位置,单位deg
        * @param  [in] desc_pos   目标笛卡尔位姿
        * @param  [in] tool  工具坐标号，范围[1~15]
        * @param  [in] user  工件坐标号，范围[1~15]
        * @param  [in] vel  速度百分比，范围[0~100]
        * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
        * @param  [in] ovl  速度缩放因子，范围[0~100]	
        * @return  错误码
        */[XmlRpcMethod("SplinePTP")]
        int SplinePTP(double[] joint_pos, double[] desc_pos, int tool, int user, double vel, double acc, double ovl);

        /**
        * @brief  样条运动结束
        * @return  错误码
        */[XmlRpcMethod("SplineEnd")]
        int SplineEnd();

        /**
        * @brief 新样条运动开始
        * @param  [in] type   0-圆弧过渡，1-给定点位为路径点
        * @return  错误码
        */[XmlRpcMethod("NewSplineStart")]
        int NewSplineStart(int type, int averageTime);

        /**
        * @brief 新样条指令点
        * @param  [in] joint_pos  目标关节位置,单位deg
        * @param  [in] desc_pos   目标笛卡尔位姿
        * @param  [in] tool  工具坐标号，范围[1~15]
        * @param  [in] user  工件坐标号，范围[1~15]
        * @param  [in] vel  速度百分比，范围[0~100]
        * @param  [in] acc  加速度百分比，范围[0~100],暂不开放
        * @param  [in] ovl  速度缩放因子，范围[0~100]
        * @param  [in] blendR [-1.0]-运动到位(阻塞)，[0~1000.0]-平滑半径(非阻塞)，单位mm
        * @return  错误码
        */[XmlRpcMethod("NewSplinePoint")]
        int NewSplinePoint(double[] joint_pos, double[] desc_pos, int tool, int user, double vel, double acc, double ovl, double blendR, int lastFlag);

        /**
        * @brief 新样条运动结束
        * @return  错误码
        */[XmlRpcMethod("NewSplineEnd")]
        int NewSplineEnd();

        /**
        * @brief 终止运动
        * @return  错误码
        */[XmlRpcMethod("StopMotion")]
        int StopMotion();

        /**
        * @brief  点位整体偏移开始
        * @param  [in]  flag  0-基坐标系下/工件坐标系下偏移，2-工具坐标系下偏移
        * @param  [in] offset_pos  位姿偏移量
        * @return  错误码
        */[XmlRpcMethod("PointsOffsetEnable")]
        int PointsOffsetEnable(int flag, double[] offset_pos);

        /**
        * @brief  点位整体偏移结束
        * @return  错误码
        */[XmlRpcMethod("PointsOffsetDisable")]
        int PointsOffsetDisable();

        /**
        * @brief  设置控制箱数字量输出
        * @param  [in] id  io编号，范围[0~15]
        * @param  [in] status 0-关，1-开
        * @param  [in] smooth 0-不平滑， 1-平滑
        * @param  [in] block  0-阻塞，1-非阻塞
        * @return  错误码
        */[XmlRpcMethod("SetDO")]
        int SetDO(int id, int status, int smooth, int block);

        /**
        * @brief  设置工具数字量输出
        * @param  [in] id  io编号，范围[0~1]
        * @param  [in] status 0-关，1-开
        * @param  [in] smooth 0-不平滑， 1-平滑
        * @param  [in] block  0-阻塞，1-非阻塞
        * @return  错误码
        */[XmlRpcMethod("SetToolDO")]
        int SetToolDO(int id, int status, int smooth, int block);

        /**
        * @brief  设置控制箱模拟量输出
        * @param  [in] id  io编号，范围[0~1]
        * @param  [in] value 电流或电压值百分比，范围[0~100]对应电流值[0~20mA]或电压[0~10V]
        * @param  [in] block  0-阻塞，1-非阻塞
        * @return  错误码
        */[XmlRpcMethod("SetAO")]
        int SetAO(int id, double value, int block);

        /**
        * @brief  设置工具模拟量输出
        * @param  [in] id  io编号，范围[0]
        * @param  [in] value 电流或电压值百分比，范围[0~100]对应电流值[0~20mA]或电压[0~10V]
        * @param  [in] block  0-阻塞，1-非阻塞
        * @return  错误码
        */[XmlRpcMethod("SetToolAO")]
        int SetToolAO(int id, double value, int block);

        /**
        * @brief 等待控制箱数字量输入
        * @param  [in] id  io编号，范围[0~15]
        * @param  [in]  status 0-关，1-开
        * @param  [in]  max_time  最大等待时间，单位ms
        * @param  [in]  opt  超时后策略，0-程序停止并提示超时，1-忽略超时提示程序继续执行，2-一直等待
        * @return  错误码
        */[XmlRpcMethod("WaitDI")]
        int WaitDI(int id, int status, int max_time, int opt);

        /**
        * @brief 等待控制箱多路数字量输入
        * @param  [in] mode 0-多路与，1-多路或
        * @param  [in] id  io编号，bit0~bit7对应DI0~DI7，bit8~bit15对应CI0~CI7
        * @param  [in]  status 0-关，1-开
        * @param  [in]  max_time  最大等待时间，单位ms
        * @param  [in]  opt  超时后策略，0-程序停止并提示超时，1-忽略超时提示程序继续执行，2-一直等待
        * @return  错误码
        */[XmlRpcMethod("WaitMultiDI")]
        int WaitMultiDI(int mode, int id, int status, int max_time, int opt);

        /**
        * @brief 等待工具数字量输入
        * @param  [in] id  io编号，范围[0~1]
        * @param  [in]  status 0-关，1-开
        * @param  [in]  max_time  最大等待时间，单位ms
        * @param  [in]  opt  超时后策略，0-程序停止并提示超时，1-忽略超时提示程序继续执行，2-一直等待
        * @return  错误码
        */[XmlRpcMethod("WaitToolDI")]
        int WaitToolDI(int id, int status, int max_time, int opt);

        /**
        * @brief 等待控制箱模拟量输入
        * @param  [in] id  io编号，范围[0~1]
        * @param  [in]  sign 0-大于，1-小于
        * @param  [in]  value 输入电流或电压值百分比，范围[0~100]对应电流值[0~20mS]或电压[0~10V]
        * @param  [in]  max_time  最大等待时间，单位ms
        * @param  [in]  opt  超时后策略，0-程序停止并提示超时，1-忽略超时提示程序继续执行，2-一直等待
        * @return  错误码
        */[XmlRpcMethod("WaitAI")]
        int WaitAI(int id, int sign, double value, int max_time, int opt);

        /**
        * @brief 等待工具模拟量输入
        * @param  [in] id  io编号，范围[0]
        * @param  [in]  sign 0-大于，1-小于
        * @param  [in]  value 输入电流或电压值百分比，范围[0~100]对应电流值[0~20mS]或电压[0~10V]
        * @param  [in]  max_time  最大等待时间，单位ms
        * @param  [in]  opt  超时后策略，0-程序停止并提示超时，1-忽略超时提示程序继续执行，2-一直等待
        * @return  错误码
        */[XmlRpcMethod("WaitToolAI")]
        int WaitToolAI(int id, int sign, double value, int max_time, int opt);

        /**
        * @brief  设置全局速度
        * @param  [in]  vel  速度百分比，范围[0~100]
        * @return  错误码
        */[XmlRpcMethod("SetSpeed")]
        int SetSpeed(int vel);

        /**
        * @brief  设置系统变量值
        * @param  [in]  id  变量编号，范围[1~20]
        * @param  [in]  value 变量值
        * @return  错误码
        */[XmlRpcMethod("SetSysVarValue")]
        int SetSysVarValue(int id, double value);

        /**
        * @brief 设置工具参考点-六点法
        * @param [in] point_num 点编号,范围[1~6] 
        * @return 错误码
        */[XmlRpcMethod("SetToolPoint")]
        int SetToolPoint(int point_num);

        /**
        * @brief  计算工具坐标系
        * @param [out] tcp_pose 工具坐标系
        * @return 错误码
        */[XmlRpcMethod("ComputeTool")]
        object[] ComputeTool();

        /**
        * @brief 设置工具参考点-四点法
        * @param [in] point_num 点编号,范围[1~4] 
        * @return 错误码
        */[XmlRpcMethod("SetTcp4RefPoint")]
        int SetTcp4RefPoint(int point_num);

        /**
        * @brief  计算工具坐标系
        * @param [out] tcp_pose 工具坐标系
        * @return 错误码
        */[XmlRpcMethod("ComputeTcp4")]
        object[] ComputeTcp4();


        /**
        * @brief  设置工具坐标系
        * @param  [in] id 坐标系编号，范围[1~15]
        * @param  [in] coord  工具中心点相对于末端法兰中心位姿
        * @param  [in] type  0-工具坐标系，1-传感器坐标系
        * @param  [in] install 安装位置，0-机器人末端，1-机器人外部
        * param   [in] toolID 工具ID
        * @param  [in] loadNum 负载编号
        * @return  错误码
        */
        [XmlRpcMethod("SetToolCoord")]
        int SetToolCoord(int id, double[] coord, int type, int install, int toolID, int loadNum);

        /**
        * @brief  设置工具坐标系列表
        * @param  [in] id 坐标系编号，范围[1~15]
        * @param  [in] coord  工具中心点相对于末端法兰中心位姿
        * @param  [in] type  0-工具坐标系，1-传感器坐标系
        * @param  [in] install 安装位置，0-机器人末端，1-机器人外部
        * @param  [in] loadNum 负载编号
        * @return  错误码
        */
        [XmlRpcMethod("SetToolList")]
        int SetToolList(int id, double[] coord, int type, int install,int loadNum);


        /**
        * @brief 设置外部工具参考点-六点法
        * @param [in] point_num 点编号,范围[1~4] 
        * @return 错误码
        */[XmlRpcMethod("SetExTCPPoint")]
        int SetExTCPPoint(int point_num);

        /**
        * @brief  计算外部工具坐标系
        * @param [out] tcp_pose 外部工具坐标系
        * @return 错误码
        */[XmlRpcMethod("ComputeExTCF")]
        object[] ComputeExTCF();

        /**
        * @brief  设置外部工具坐标系
        * @param  [in] id 坐标系编号，范围[1~15]
        * @param  [in] etcp  工具中心点相对末端法兰中心位姿
        * @param  [in] etool  待定
        * @return  错误码
        */[XmlRpcMethod("SetExToolCoord")]
        int SetExToolCoord(int id, double[] etcp, double[] etool);

        /**
        * @brief  设置外部工具坐标系列表
        * @param  [in] id 坐标系编号，范围[1~15]
        * @param  [in] etcp  工具中心点相对末端法兰中心位姿
        * @param  [in] etool  待定
        * @return  错误码
        */[XmlRpcMethod("SetExToolList")]
        int SetExToolList(int id, double[] etcp, double[] etool);

        /**
        * @brief 设置工件参考点-三点法
        * @param [in] point_num 点编号,范围[1~3] 
        * @return 错误码
        */[XmlRpcMethod("SetWObjCoordPoint")]
        int SetWObjCoordPoint(int point_num);

        /**
         * @brief  计算工件坐标系
         * @param [in] method 计算方法 0：原点-x轴-z轴  1：原点-x轴-xy平面
         * @param [in] refFrame 参考坐标系
         * @param [out] wobj_pose 工件坐标系
         * @return 错误码
        */[XmlRpcMethod("ComputeWObjCoord")]
        object[] ComputeWObjCoord(int method, int refFrame);


        /**
        * @brief  设置工件坐标系
        * @param  [in] id 坐标系编号，范围[1~15]
        * @param  [in] coord  工件坐标系相对于末端法兰中心位姿
        * @return  错误码
        */[XmlRpcMethod("SetWObjCoord")]
        int SetWObjCoord(int id, double[] coord, int refFrame);

        /**
        * @brief  设置工件坐标系列表
        * @param  [in] id 坐标系编号，范围[1~15]
        * @param  [in] coord  工件坐标系相对于末端法兰中心位姿
        * @return  错误码
        */[XmlRpcMethod("SetWObjList")]
        int SetWObjList(int id, double[] coord, int refFrame);

        /**
        * @brief  设置末端负载重量
        * @param  [in] loadNum 负载编号
        * @param  [in] weight  负载重量，单位kg
        * @return  错误码
        */
        [XmlRpcMethod("SetLoadWeight")]
        int SetLoadWeight(int loadNum, double weight);

        /**
        * @brief  设置末端负载质心坐标
        * @param  [in] coord 质心坐标，单位mm
        * @return  错误码
        */[XmlRpcMethod("SetLoadCoord")]
        int SetLoadCoord(double coordX, double coordY, double coordZ);

        /**
        * @brief  设置机器人安装方式
        * @param  [in] install  安装方式，0-正装，1-侧装，2-倒装
        * @return  错误码
        */[XmlRpcMethod("SetRobotInstallPos")]
        int SetRobotInstallPos(int install);

        /**
        * @brief  设置机器人安装角度，自由安装
        * @param  [in] yangle  倾斜角
        * @param  [in] zangle  旋转角
        * @return  错误码
        */[XmlRpcMethod("SetRobotInstallAngle")]
        int SetRobotInstallAngle(double yangle, double zangle);

        /**
        * @brief  等待指定时间
        * @param  [in]  t_ms  单位ms
        * @return  错误码
        */[XmlRpcMethod("WaitMs")]
        int WaitMs(int t_ms);

        /**
        * @brief 设置碰撞等级
        * @param  [in]  mode  0-等级，1-百分比
        * @param  [in]  level 碰撞阈值，等级对应范围[],百分比对应范围[0~1]
        * @param  [in]  config 0-不更新配置文件，1-更新配置文件
        * @return  错误码
        */[XmlRpcMethod("SetAnticollision")]
        int SetAnticollision(int mode, double[] level, int config);

        /**
        * @brief  设置碰撞后策略
        * @param  [in] strategy  0-报错停止，1-继续运行
        * @return  错误码	 
        */[XmlRpcMethod("SetCollisionStrategy")]
        int SetCollisionStrategy(int strategy, int safeTime, int safeDistance, int safeVel ,int[] safetyMargin);

        /**
        * @brief  设置正限位
        * @param  [in] limit 六个关节位置，单位deg
        * @return  错误码
        */[XmlRpcMethod("SetLimitPositive")]
        int SetLimitPositive(double[] limit);

        /**
        * @brief  设置负限位
        * @param  [in] limit 六个关节位置，单位deg
        * @return  错误码
        */[XmlRpcMethod("SetLimitNegative")]
        int SetLimitNegative(double[] limit);

        /**
        * @brief  错误状态清除
        * @return  错误码
        */[XmlRpcMethod("ResetAllError")]
        int ResetAllError();

        /**
        * @brief  关节摩擦力补偿开关
        * @param  [in]  state  0-关，1-开
        * @return  错误码
        */[XmlRpcMethod("FrictionCompensationOnOff")]
        int FrictionCompensationOnOff(int state);

        /**
        * @brief  设置关节摩擦力补偿系数-正装
        * @param  [in]  coeff 六个关节补偿系数，范围[0~1]
        * @return  错误码
        */[XmlRpcMethod("SetFrictionValue_level")]
        int SetFrictionValue_level(double[] coeff);

        /**
        * @brief  设置关节摩擦力补偿系数-侧装
        * @param  [in]  coeff 六个关节补偿系数，范围[0~1]
        * @return  错误码
        */[XmlRpcMethod("SetFrictionValue_wall")]
        int SetFrictionValue_wall(double[] coeff);

        /**
        * @brief  设置关节摩擦力补偿系数-倒装
        * @param  [in]  coeff 六个关节补偿系数，范围[0~1]
        * @return  错误码
        */[XmlRpcMethod("SetFrictionValue_ceiling")]
        int SetFrictionValue_ceiling(double[] coeff);

        /**
        * @brief  设置关节摩擦力补偿系数-自由安装
        * @param  [in]  coeff 六个关节补偿系数，范围[0~1]
        * @return  错误码
        */[XmlRpcMethod("SetFrictionValue_freedom")]
        int SetFrictionValue_freedom(double[] coeff);

        /**
        * @brief  获取机器人安装角度
        * @param  [out] yangle 倾斜角
        * @param  [out] zangle 旋转角
        * @return  错误码
        */[XmlRpcMethod("GetRobotInstallAngle")]
        object[] GetRobotInstallAngle();

        /**
        * @brief  获取系统变量值
        * @param  [in] id 系统变量编号，范围[1~20]
        * @param  [out] value  系统变量值
        * @return  错误码
        */[XmlRpcMethod("GetSysVarValue")]
        object[] GetSysVarValue(int id);

        /**
         * @brief  获取当前关节位置(弧度)
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] jPos 六个关节位置，单位rad
         * @return  错误码
         */
        [XmlRpcMethod("GetActualJointPosRadian")]
        object[] GetActualJointPosRadian(int flag);

        /**
         * @brief  逆运动学求解
         * @param  [in] posMode 0-绝对位姿(基坐标系)，1-增量位姿(基坐标系)，2-增量位姿(工具坐标系)
         * @param  [in] desc_pos 笛卡尔位姿
         * @param  [in] config 关节空间配置，[-1]-参考当前关节位置解算，[0~7]-依据特定关节空间配置求解
         * @param  [out] joint_pos 关节位置
         * @return  错误码
         */
        [XmlRpcMethod("GetInverseKin")]
        object[] GetInverseKin(int posMode, double[] desc_pos, int config);

        /**
         * @brief  逆运动学求解，参考指定关节位置求解
         * @param  [in] posMode 0绝对位姿， 1相对位姿-基坐标系   2相对位姿-工具坐标系
         * @param  [in] desc_pos 笛卡尔位姿
         * @param  [in] joint_pos_ref 参考关节位置
         * @param  [out] joint_pos 关节位置
         * @return  错误码
         */
        [XmlRpcMethod("GetInverseKinRef")]
        object[] GetInverseKinRef(int posMode, double[] desc_pos, double[] joint_pos_ref);

        /**
         * @brief  逆运动学求解，参考指定关节位置判断是否有解
         * @param  [in] desc_pos 笛卡尔位姿
         * @param  [in] joint_pos_ref 参考关节位置
         * @param  [out] result 0-无解，1-有解
         * @return  错误码
         */[XmlRpcMethod("GetInverseKinHasSolution")]
        object[] GetInverseKinHasSolution(int posMode, double[] desc_pos, double[] joint_pos_ref);

        /**
         * @brief  正运动学求解
         * @param  [in] joint_pos 关节位置
         * @param  [out] desc_pos 笛卡尔位姿
         * @return  错误码
         */[XmlRpcMethod("GetForwardKin")]
        object[] GetForwardKin(double[] joint_pos);

        /**
         * @brief  获取当前负载的重量
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] weight 负载重量，单位kg
         * @return  错误码
         */[XmlRpcMethod("GetTargetPayload")]
        object[] GetTargetPayload(int flag);

        /**
         * @brief  获取当前负载的质心
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] cog 负载质心，单位mm
         * @return  错误码
         */[XmlRpcMethod("GetTargetPayloadCog")]
        object[] GetTargetPayloadCog(int flag);

        /**
         * @brief  获取当前工具坐标系
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] desc_pos 工具坐标系位姿
         * @return  错误码
         */[XmlRpcMethod("GetTCPOffset")]
        object[] GetTCPOffset(int flag);

        /**
         * @brief  获取当前工件坐标系
         * @param  [in] flag 0-阻塞，1-非阻塞
         * @param  [out] desc_pos 工件坐标系位姿
         * @return  错误码
         */[XmlRpcMethod("GetWObjOffset")]
        object[] GetWObjOffset(int flag);

        /**
         * @brief  获取关节软限位角度
         * @param  [in] flag 0-阻塞，1-非阻塞	 
         * @param  [out] negative  负限位角度，单位deg
         * @param  [out] positive  正限位角度，单位deg
         * @return  错误码
         */[XmlRpcMethod("GetJointSoftLimitDeg")]
        object[] GetJointSoftLimitDeg(int flag);

        /**
         * @brief  获取系统时间
         * @param  [out] t_ms 单位ms
         * @return  错误码
         */[XmlRpcMethod("GetSystemClock")]
        object[] GetSystemClock();

        /**
         * @brief  获取机器人当前关节位置
         * @param  [out]  config  关节空间配置，范围[0~7]
         * @return  错误码
         */[XmlRpcMethod("GetRobotCurJointsConfig")]
        object[] GetRobotCurJointsConfig();

        /**
         * @brief  获取机器人当前速度
         * @param  [out]  vel  速度，单位mm/s
         * @return  错误码
         */[XmlRpcMethod("GetDefaultTransVel")]
        object[] GetDefaultTransVel();

        /**
         * @brief  查询机器人示教管理点位数据
         * @param  [in]  name  点位名
         * @param  [out]  data   点位数据
         * @return  错误码
         */[XmlRpcMethod("GetRobotTeachingPoint")]
        object[] GetRobotTeachingPoint(string name);

        /**
        * @brief  设置轨迹记录参数
        * @param  [in] type  记录数据类型，1-关节位置
        * @param  [in] name  轨迹文件名
        * @param  [in] period_ms  数据采样周期，固定值2ms或4ms或8ms
        * @param  [in] di_choose  DI选择,bit0~bit7对应控制箱DI0~DI7，bit8~bit9对应末端DI0~DI1，0-不选择，1-选择
        * @param  [in] do_choose  DO选择,bit0~bit7对应控制箱DO0~DO7，bit8~bit9对应末端DO0~DO1，0-不选择，1-选择
        * @return  错误码
        */[XmlRpcMethod("SetTPDParam")]
        int SetTPDParam(int type, string name, int period_ms, int di_choose, int do_choose);

        /**
        * @brief  开始轨迹记录
        * @param  [in] type  记录数据类型，1-关节位置
        * @param  [in] name  轨迹文件名
        * @param  [in] period_ms  数据采样周期，固定值2ms或4ms或8ms
        * @param  [in] di_choose  DI选择,bit0~bit7对应控制箱DI0~DI7，bit8~bit9对应末端DI0~DI1，0-不选择，1-选择
        * @param  [in] do_choose  DO选择,bit0~bit7对应控制箱DO0~DO7，bit8~bit9对应末端DO0~DO1，0-不选择，1-选择
        * @return  错误码
        */[XmlRpcMethod("SetTPDStart")]
        int SetTPDStart(int type, string name, int period_ms, int di_choose, int do_choose);

        /**
        * @brief  停止轨迹记录
        * @return  错误码
        */[XmlRpcMethod("SetWebTPDStop")]
        int SetWebTPDStop();

        /**
        * @brief  删除轨迹记录
        * @param  [in] name  轨迹文件名
        * @return  错误码
        */[XmlRpcMethod("SetTPDDelete")]
        int SetTPDDelete(string name);

        /**
        * @brief  轨迹预加载
        * @param  [in] name  轨迹文件名
        * @return  错误码
        */[XmlRpcMethod("LoadTPD")]
        int LoadTPD(string name);

        /**
        * @brief  获取轨迹起始位姿
        * @param  [in] name 轨迹文件名,不需要文件后缀
        * @return  错误码
        */[XmlRpcMethod("GetTPDStartPose")]
        object[] GetTPDStartPose(string name);

        /**
        * @brief  轨迹复现
        * @param  [in] name  轨迹文件名
        * @param  [in] blend 0-不平滑，1-平滑
        * @param  [in] ovl  速度缩放百分比，范围[0~100]
        * @return  错误码
        */[XmlRpcMethod("MoveTPD")]
        int MoveTPD(string name, int blend, double ovl);

        /**
        * @brief  轨迹预处理
        * @param  [in] name  轨迹文件名
        * @param  [in] ovl 速度缩放百分比，范围[0~100]
        * @param  [in] opt 1-控制点，默认为1
        * @return  错误码
        */[XmlRpcMethod("LoadTrajectoryJ")]
        int LoadTrajectoryJ(string name, double ovl, int opt);

        /**
        * @brief  轨迹复现
        * @return  错误码
        */[XmlRpcMethod("MoveTrajectoryJ")]
        int MoveTrajectoryJ();

        /**
        * @brief  获取轨迹起始位姿
        * @param  [in] name 轨迹文件名
        * @return  错误码
        */[XmlRpcMethod("GetTrajectoryStartPose")]
        object[] GetTrajectoryStartPose(string name);

        /**
        * @brief  设置轨迹运行中的速度
        * @param  [in] ovl 速度百分比
        * @return  错误码
        */[XmlRpcMethod("SetTrajectoryJSpeed")]
        int SetTrajectoryJSpeed(double ovl);

        /**
        * @brief  设置轨迹运行中的力和扭矩
        * @param  [in] ft 三个方向的力和扭矩，单位N和Nm
        * @return  错误码
        */[XmlRpcMethod("SetTrajectoryJForceTorque")]
        int SetTrajectoryJForceTorque(double[] ft);

        /**
        * @brief  设置轨迹运行中的沿x方向的力
        * @param  [in] fx 沿x方向的力，单位N
        * @return  错误码
        */[XmlRpcMethod("SetTrajectoryJForceFx")]
        int SetTrajectoryJForceFx(double fx);

        /**
        * @brief  设置轨迹运行中的沿y方向的力
        * @param  [in] fy 沿y方向的力，单位N
        * @return  错误码
        */[XmlRpcMethod("SetTrajectoryJForceFy")]
        int SetTrajectoryJForceFy(double fy);

        /**
        * @brief  设置轨迹运行中的沿z方向的力
        * @param  [in] fz 沿x方向的力，单位N
        * @return  错误码
        */[XmlRpcMethod("SetTrajectoryJForceFz")]
        int SetTrajectoryJForceFz(double fz);

        /**
        * @brief  设置轨迹运行中的绕x轴的扭矩
        * @param  [in] tx 绕x轴的扭矩，单位Nm
        * @return  错误码
        */[XmlRpcMethod("SetTrajectoryJTorqueTx")]
        int SetTrajectoryJTorqueTx(double tx);

        /**
        * @brief  设置轨迹运行中的绕x轴的扭矩
        * @param  [in] ty 绕y轴的扭矩，单位Nm
        * @return  错误码
        */[XmlRpcMethod("SetTrajectoryJTorqueTy")]
        int SetTrajectoryJTorqueTy(double ty);

        /**
        * @brief  设置轨迹运行中的绕x轴的扭矩
        * @param  [in] tz 绕z轴的扭矩，单位Nm
        * @return  错误码
        */[XmlRpcMethod("SetTrajectoryJTorqueTz")]
        int SetTrajectoryJTorqueTz(double tz);

        /**
        * @brief  设置开机自动加载默认的作业程序
        * @param  [in] flag  0-开机不自动加载默认程序，1-开机自动加载默认程序
        * @param  [in] program_name 作业程序名及路径，如"/fruser/movej.lua"，其中"/fruser/"为固定路径
        * @return  错误码
        */[XmlRpcMethod("LoadDefaultProgConfig")]
        int LoadDefaultProgConfig(int flag, string program_name);

        /**
        * @brief  加载指定的作业程序
        * @param  [in] program_name 作业程序名及路径，如"/fruser/movej.lua"，其中"/fruser/"为固定路径
        * @return  错误码
        */[XmlRpcMethod("ProgramLoad")]
        int ProgramLoad(string program_name);

        /**
         * @brief  获取已加载的作业程序名
         * @param  [out] program_name 作业程序名及路径，如"/fruser/movej.lua"，其中"/fruser/"为固定路径
         * @return  错误码
         */[XmlRpcMethod("GetLoadedProgram")]
        object[] GetLoadedProgram();

        /**
         * @brief  获取当前机器人作业程序执行的行号
         * @param  [out] line  行号
         * @return  错误码
         */[XmlRpcMethod("GetCurrentLine")]
        object[] GetCurrentLine();

        /**
        * @brief  运行当前加载的作业程序
        * @return  错误码
        */[XmlRpcMethod("ProgramRun")]
        int ProgramRun();

        /**
        * @brief  暂停当前运行的作业程序
        * @return  错误码
        */[XmlRpcMethod("ProgramPause")]
        int ProgramPause();

        /**
        * @brief  恢复当前暂停的作业程序
        * @return  错误码
        */[XmlRpcMethod("ProgramResume")]
        int ProgramResume();

        /**
        * @brief  终止当前运行的作业程序
        * @return  错误码
        */[XmlRpcMethod("ProgramStop")]
        int ProgramStop();

        /**
        * @brief  配置夹爪
        * @param  [in] company  夹爪厂商，1-Robotiq，2-慧灵，3-天机，4-大寰，5-知行
        * @param  [in] device  设备号，Robotiq(0-2F-85系列)，慧灵(0-NK系列,1-Z-EFG-100)，天机(0-TEG-110)，大寰(0-PGI-140)，知行(0-CTPM2F20)
        * @param  [in] softvesion  软件版本号，暂不使用，默认为0
        * @param  [in] bus 设备挂在末端总线位置，暂不使用，默认为0
        * @return  错误码
        */[XmlRpcMethod("SetGripperConfig")]
        int SetGripperConfig(int company, int device, int softvesion, int bus);

        /**
         *@brief  获取夹爪配置
         *@param  [out] company  夹爪厂商，1-Robotiq，2-慧灵，3-天机，4-大寰，5-知行
         *@param  [out] device  设备号，Robotiq(0-2F-85系列)，慧灵(0-NK系列,1-Z-EFG-100)，天机(0-TEG-110)，大寰(0-PGI-140)，知行(0-CTPM2F20)
         *@param  [out] softvesion  软件版本号，暂不使用，默认为0
         *@param  [out] bus 设备挂在末端总线位置，暂不使用，默认为0
         *@return  错误码
         */[XmlRpcMethod("GetGripperConfig")]
        object[] GetGripperConfig();

        /**
        * @brief  激活夹爪
        * @param  [in] index  夹爪编号
        * @param  [in] act  0-复位，1-激活
        * @return  错误码
        */[XmlRpcMethod("ActGripper")]
        int ActGripper(int index, int act);

        /**
        * @brief  控制夹爪
        * @param  [in] index  夹爪编号
        * @param  [in] pos  位置百分比，范围[0~100]
        * @param  [in] vel  速度百分比，范围[0~100]
        * @param  [in] force  力矩百分比，范围[0~100]
        * @param  [in] max_time  最大等待时间，范围[0~30000]，单位ms
        * @param  [in] block  0-阻塞，1-非阻塞
        * @return  错误码
        */[XmlRpcMethod("MoveGripper")]
        int MoveGripper(int index, int pos, int vel, int force, int max_time, int block, int type, double rotNum, int rotVel, int rotTorque);

        /**
         * @brief  获取夹爪运动状态
         * @param  [out] fault  0-无错误，1-有错误
         * @param  [out] staus  0-运动未完成，1-运动完成
         * @return  错误码
         */[XmlRpcMethod("GetGripperMotionDone")]
        object[] GetGripperMotionDone();

        /**
        * @brief  计算预抓取点-视觉
        * @param  [in] desc_pos  抓取点笛卡尔位姿
        * @param  [in] zlength   z轴偏移量
        * @param  [in] zangle    绕z轴旋转偏移量
        * @return  错误码 
        */[XmlRpcMethod("ComputePrePick")]
        object[] ComputePrePick(double[] desc_pos, double zlength, double zangle);

        /**
        * @brief  计算撤退点-视觉
        * @param  [in] desc_pos  抓取点笛卡尔位姿
        * @param  [in] zlength   z轴偏移量
        * @param  [in] zangle    绕z轴旋转偏移量
        * @return  错误码 
        */[XmlRpcMethod("ComputePostPick")]
        object[] ComputePostPick(double[] desc_pos, double zlength, double zangle);

        /**
	    * @brief  配置力传感器
	    * @param  [in] company  力传感器厂商，17-坤维科技，19-航天十一院，20-ATI传感器，21-中科米点，22-伟航敏芯，23-NBIT，24-鑫精诚(XJC)，26-NSR
	    * @param  [in] device  设备号，坤维(0-KWR75B)，航天十一院(0-MCS6A-200-4)，ATI(0-AXIA80-M8)，中科米点(0-MST2010)，伟航敏芯(0-WHC6L-YB-10A)，NBIT(0-XLH93003ACS)，鑫精诚XJC(0-XJC-6F-D82)，NSR(0-NSR-FTSensorA)
	    * @param  [in] softvesion  软件版本号，暂不使用，默认为0
	    * @param  [in] bus 设备挂在末端总线位置，暂不使用，默认为0
	    * @return  错误码
        */[XmlRpcMethod("FT_SetConfig")]
        int FT_SetConfig(int company, int device, int softvesion, int bus);

        /**
         * @brief  获取力传感器配置
         * @param  [out] company  力传感器厂商，待定
         * @param  [out] device  设备号，暂不使用，默认为0
         * @param  [out] softvesion  软件版本号，暂不使用，默认为0
         * @param  [out] bus 设备挂在末端总线位置，暂不使用，默认为0
         * @return  错误码
         */[XmlRpcMethod("FT_GetConfig")]
        object[] FT_GetConfig();

        /**
        * @brief  力传感器激活
        * @param  [in] act  0-复位，1-激活
        * @return  错误码
        */[XmlRpcMethod("FT_Activate")]
        int FT_Activate(int act);

        /**
        * @brief  力传感器校零
        * @param  [in] act  0-去除零点，1-零点矫正
        * @return  错误码
        */[XmlRpcMethod("FT_SetZero")]
        int FT_SetZero(int act);

        /**
        * @brief  设置力传感器参考坐标系
        * @param  [in] ref  0-工具坐标系，1-基坐标系
        * @return  错误码
        */[XmlRpcMethod("FT_SetRCS")]
        int FT_SetRCS(int refType, double[] coords);

        /**
        * @brief  负载重量辨识记录
        * @param  [in] id  传感器坐标系编号，范围[1~14]
        * @return  错误码
        */[XmlRpcMethod("FT_PdIdenRecord")]
        int FT_PdIdenRecord(int id);

        /**
         * @brief  负载重量辨识计算
         * @param  [out] weight  负载重量，单位kg
         * @return  错误码
         */[XmlRpcMethod("FT_PdIdenCompute")]
        object[] FT_PdIdenCompute();

        /**
        * @brief  负载质心辨识记录
        * @param  [in] id  传感器坐标系编号，范围[1~14]
        * @param  [in] index 点编号，范围[1~3]
        * @return  错误码
        */[XmlRpcMethod("FT_PdCogIdenRecord")]
        int FT_PdCogIdenRecord(int id, int index);

        /**
         * @brief  负载质心辨识计算
         * @param  [out] cog  负载质心，单位mm
         * @return  错误码
         */[XmlRpcMethod("FT_PdCogIdenCompute")]
        object[] FT_PdCogIdenCompute();

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
        */[XmlRpcMethod("FT_Guard")]
        int FT_Guard(int flag, int sensor_id, int[] select, double[] ft, double[] max_threshold, double[] min_threshold);

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
        * @return  错误码
        */[XmlRpcMethod("FT_Control")]
        int FT_Control(int flag, int sensor_id, int[] select, double[] ft, double[] ft_pid, int adj_sign, int ILC_sign, double max_dis, double max_ang, int filter_Sign, int posAdapt_sign, int isNoBlock);

        /**
        * @brief  柔顺控制开启
        * @param  [in] p 位置调节系数或柔顺系数
        * @param  [in] force 柔顺开启力阈值，单位N
        * @return  错误码
        */[XmlRpcMethod("FT_ComplianceStart")]
        int FT_ComplianceStart(double p, double force);

        /**
        * @brief  柔顺控制关闭
        * @return  错误码
        */[XmlRpcMethod("FT_ComplianceStop")]
        int FT_ComplianceStop();

        /**
        * @brief 负载辨识初始化
        * @return 错误码
        */
        [XmlRpcMethod("LoadIdentifyDynFilterInit")]
        int LoadIdentifyDynFilterInit();

        /**
        * @brief 负载辨识初始化
        * @return 错误码
        */
        [XmlRpcMethod("LoadIdentifyDynVarInit")]
        int LoadIdentifyDynVarInit();

        /**
        * @brief 负载辨识主程序
        * @param [in] joint_torque 关节扭矩
        * @param [in] joint_pos 关节位置
        * @param [in] t 采样周期
        * @return 错误码
        */
        [XmlRpcMethod("LoadIdentifyMain")]
        int LoadIdentifyMain(double[] joint_torque, double[] joint_pos, double t);


        /**
        * @brief 获取负载辨识结果
        * @param [in] gain  重力项系数double[6]，离心项系数double[6]
        * @param [out] weight 负载重量
        * @param [out] cog 负载质心
        * @return 错误码
        */
        [XmlRpcMethod("LoadIdentifyGetResult")]
        object[] LoadIdentifyGetResult(double[] gain);

        /**
        * @brief 传动带启动、停止
        * @param [in] status 状态，1-启动，0-停止 
        * @return 错误码
        */
        [XmlRpcMethod("ConveyorStartEnd")]
        int ConveyorStartEnd(int status);

        /**
        * @brief 记录IO检测点
        * @return 错误码
        */[XmlRpcMethod("ConveyorPointIORecord")]
        int ConveyorPointIORecord();

        /**
        * @brief 记录A点
        * @return 错误码
        */[XmlRpcMethod("ConveyorPointARecord")]
        int ConveyorPointARecord();

        /**
        * @brief 记录参考点
        * @return 错误码
        */[XmlRpcMethod("ConveyorRefPointRecord")]
        int ConveyorRefPointRecord();

        /**
        * @brief 记录B点
        * @return 错误码
        */[XmlRpcMethod("ConveyorPointBRecord")]
        int ConveyorPointBRecord();

        /**
        * @brief 传送带工件IO检测
        * @param [in] max_t 最大检测时间，单位ms
        * @return 错误码
        */[XmlRpcMethod("ConveyorIODetect")]
        int ConveyorIODetect(int max_t);

        /**
        * @brief 获取物体当前位置
        * @param [in] mode 
        * @return 错误码
        */[XmlRpcMethod("ConveyorGetTrackData")]
        int ConveyorGetTrackData(int mode);

        /**
        * @brief 传动带跟踪开始
        * @param [in] status 状态，1-启动，0-停止 
        * @return 错误码
        */[XmlRpcMethod("ConveyorTrackStart")]
        int ConveyorTrackStart(int status);

        /**
        * @brief 传动带跟踪停止
        * @return 错误码
        */[XmlRpcMethod("ConveyorTrackEnd")]
        int ConveyorTrackEnd();

        /**
        * @brief 传动带参数配置
        * @param [in] 
        * @return 错误码
        */[XmlRpcMethod("ConveyorSetParam")]
        int ConveyorSetParam(double[] param, int followType, int startDis, int endDis);

        /**
        * @brief 传动带抓取点补偿
        * @param [in] cmp 补偿位置 
        * @return 错误码
        */
        [XmlRpcMethod("ConveyorCatchPointComp")]
        int ConveyorCatchPointComp(double[] cmp);

        /**
        * @brief 直线运动
        * @param [in] status 状态，1-启动，0-停止 
        * @return 错误码
        */[XmlRpcMethod("ConveyorTrackMoveL")]
        int ConveyorTrackMoveL(string name, int tool, int wobj, double vel, double acc, double ovl, double blendR, int flag, int type);

        /**
         * @brief 获取SSH公钥
         * @param [out] keygen 公钥
         * @return 错误码
         */[XmlRpcMethod("GetSSHKeygen")]
        object[] GetSSHKeygen();

        /**
        * @brief 获取SSH公钥
        * @param [out] keygen 公钥
        * @return 错误码
        */
        [XmlRpcMethod("PtpFIRPlanningStart")]
        int PtpFIRPlanningStart(double maxAcc, double maxJek);

        /**
        * @brief 关闭Ptp运动FIR滤波
        * @return 错误码
        */
        [XmlRpcMethod("PtpFIRPlanningEnd")]
        int PtpFIRPlanningEnd();
        /**
        * @brief 开始LIN、ARC运动FIR滤波
        * @param [in] maxAccLin 线加速度极值(mm/s2)
        * @param [in] maxAccDeg 角加速度极值(deg/s2)
        * @param [in] maxJerkLin 线加加速度极值(mm/s3)
        * @param [in] maxJerkDeg 角加加速度极值(deg/s3)
        * @return 错误码
        */
        [XmlRpcMethod("LinArcFIRPlanningStart")]
        int LinArcFIRPlanningStart(double maxAccLin, double maxAccDeg, double maxJerkLin, double maxJerkDeg);

        /**
        * @brief 关闭LIN、ARC运动FIR滤波
        * @return 错误码
        */
        [XmlRpcMethod("LinArcFIRPlanningEnd")]
        int LinArcFIRPlanningEnd();
        /**
        * @brief 下发SCP指令
        * @param [in] mode 0-上传（上位机->控制器），1-下载（控制器->上位机）
        * @param [in] sshname 上位机用户名
        * @param [in] sship 上位机ip地址
        * @param [in] usr_file_url 上位机文件路径
        * @param [in] robot_file_url 机器人控制器文件路径
        * @return 错误码
        */[XmlRpcMethod("SetSSHScpCmd")]
        int SetSSHScpCmd(int mode, string sshname, string sship, string usr_file_url, string robot_file_url);

        /**
         * @brief 计算指定路径下文件的MD5值
         * @param [in] file_path 文件路径包含文件名，默认Traj文件夹路径为:"/fruser/traj/",如"/fruser/traj/trajHelix_aima_1.txt"
         * @param [out] md5 文件MD5值
         * @return 错误码
         */[XmlRpcMethod("ComputeFileMD5")]
        object[] ComputeFileMD5(string file_path);

        /** 获取DH参数补偿值 */
        [XmlRpcMethod("GetDHCompensation")]
        object[] GetDHCompensation();

        [XmlRpcMethod("PointTableDownload")]
        int PointTableDownload(string pointTableName);

        [XmlRpcMethod("PointTableUpload")]
        int PointTableUpload(string pointTableName);

        [XmlRpcMethod("PointTableSwitch")]
        int PointTableSwitch(string pointTableName);

        [XmlRpcMethod("PointTableUpdateLua")]
        object[] PointTableUpdateLua(string luaFileName);

        [XmlRpcMethod("GetSoftwareVersion")]
        object[] GetSoftwareVersion();

        [XmlRpcMethod("GetSlaveHardVersion")]
        object[] GetSlaveHardVersion();

        [XmlRpcMethod("GetSlaveFirmVersion")]
        object[] GetSlaveFirmVersion();

        [XmlRpcMethod("ARCStart")]
        int ARCStart(int ioType, int arcNum, int timeout);

        [XmlRpcMethod("ARCEnd")]
        int ARCEnd(int ioType, int arcNum, int timeout);

        [XmlRpcMethod("WeldingSetCurrentRelation")]
        int WeldingSetCurrentRelation(double currentMin, double currentMax, double outputVoltageMin, double outputVoltageMax, int AOIndex);

        [XmlRpcMethod("WeldingSetVoltageRelation")]
        int WeldingSetVoltageRelation(double weldVoltageMin, double weldVoltageMax, double outputVoltageMin, double outputVoltageMax, int AOIndex);

        [XmlRpcMethod("WeldingGetCurrentRelation")]
        object[] WeldingGetCurrentRelation();

        [XmlRpcMethod("WeldingGetVoltageRelation")]
        object[] WeldingGetVoltageRelation();

        [XmlRpcMethod("WeldingSetCurrent")]
        int WeldingSetCurrent(int iotype, double current, int AOIndex, int blend);

        [XmlRpcMethod("WeldingSetVoltage")]
        int WeldingSetVoltage(int ioType, double voltage, int AOIndex, int blend);

        [XmlRpcMethod("WeaveSetPara")]
        int WeaveSetPara(int weaveNum, int weaveType, double weaveFrequency, int weaveIncStayTime, double weaveRange, double weaveLeftRange, double weaveRightRange, int additionalStayTime, int weaveLeftStayTime, int weaveRightStayTime, int weaveCircleRadio, int weaveStationary, double weaveYawAngle, double weaveRotAngle);

        [XmlRpcMethod("WeaveOnlineSetPara")]
        int WeaveOnlineSetPara(int weaveNum, int weaveType, double weaveFrequency, int weaveIncStayTime, double weaveRange, int weaveLeftStayTime, int weaveRightStayTime, int weaveCircleRadio, int weaveStationary);

        [XmlRpcMethod("WeaveStart")]
        int WeaveStart(int weaveNum);

        [XmlRpcMethod("WeaveEnd")]
        int WeaveEnd(int weaveNum);

        [XmlRpcMethod("SetForwardWireFeed")]
        int SetForwardWireFeed(int ioType, int wireFeed);

        [XmlRpcMethod("SetReverseWireFeed")]
        int SetReverseWireFeed(int ioType, int wireFeed);

        [XmlRpcMethod("SetAspirated")]
        int SetAspirated(int ioType, int airControl);

        [XmlRpcMethod("GetSegWeldDisDir")]
        object[] GetSegWeldDisDir(double startPosX, double startPosY, double startPosZ, double endPosX, double endPosPosY, double endPosZ);

        [XmlRpcMethod("FileUpload")]
        int FileUpload(int fileType, string fileName);

        [XmlRpcMethod("FileDownload")]
        int FileDownload(int fileType, string fileName);

        [XmlRpcMethod("FileDelete")]
        int FileDelete(int fileType, string fileName);

        [XmlRpcMethod("GetLuaList")]
        object[] GetLuaList();

        [XmlRpcMethod("LuaUpLoadUpdate")]
        object[] LuaUpLoadUpdate(string fileName);

        [XmlRpcMethod("LuaDownLoadPrepare")]
        int LuaDownLoadPrepare(string fileName);

        [XmlRpcMethod("GetRobotErrorCode")]
        object[] GetRobotErrorCode();

        [XmlRpcMethod("AuxServoSetParam")]
        int AuxServoSetParam(int servoId, int servoCompany, int servoModel, int servoSoftVersion, int servoResolution, double axisMechTransRatio);

        [XmlRpcMethod("AuxServoGetParam")]
        object[] AuxServoGetParam(int servoId);

        [XmlRpcMethod("AuxServoEnable")]
        int AuxServoEnable(int servoId, int status);

        [XmlRpcMethod("AuxServoSetControlMode")]
        int AuxServoSetControlMode(int servoId, int mode);

        [XmlRpcMethod("AuxServoSetTargetPos")]
        int AuxServoSetTargetPos(int servoId, double pos, double speed, double acc);

        [XmlRpcMethod("AuxServoSetTargetSpeed")]
        int AuxServoSetTargetSpeed(int servoId, double speed, double acc);

        [XmlRpcMethod("AuxServoSetTargetTorque")]
        int AuxServoSetTargetTorque(int servoId, double torque);

        [XmlRpcMethod("AuxServoHoming")]
        int AuxServoHoming(int servoId, int mode, double searchVel, double latchVel, double acc);

        [XmlRpcMethod("AuxServoClearError")]
        int AuxServoClearError(int servoId);

        [XmlRpcMethod("AuxServoGetStatus")]
        object[] AuxServoGetStatus(int servoId);

        [XmlRpcMethod("AuxServoSetStatusID")]
        int AuxServosetStatusID(int servoId);

        [XmlRpcMethod("GetExDevProtocol")]
        object[] GetExDevProtocol();

        [XmlRpcMethod("SetExDevProtocol")]
        int SetExDevProtocol(int protocol);

        [XmlRpcMethod("SetOaccScale")]
        int SetOaccScale(double acc);

        [XmlRpcMethod("MoveAOStart")]
        int MoveAOStart(int AONum, int maxTCPSpeed, int maxAOPercent, int percent);

        [XmlRpcMethod("MoveAOStop")]
        int MoveAOStop();

        [XmlRpcMethod("MoveToolAOStart")]
        int MoveToolAOStart(int AONum, int maxTCPSpeed, int maxAOPercent, int percent);

        [XmlRpcMethod("MoveToolAOStop")]
        int MoveToolAOStop();

        [XmlRpcMethod("ExtDevSetUDPComParam")]
        int ExtDevSetUDPComParam(string ip, int port, int period, int lossPkgTime, int lossPkgNum, int disconnectTime, int reconnectEnable, int reconnectPeriod, int reconnectNum, int selfConnect);

        [XmlRpcMethod("ExtDevGetUDPComParam")]
        object[] ExtDevGetUDPComParam();

        [XmlRpcMethod("ExtDevLoadUDPDriver")]
        int ExtDevLoadUDPDriver();

        [XmlRpcMethod("ExtDevUnloadUDPDriver")]
        int ExtDevUnloadUDPDriver();

        [XmlRpcMethod("ExtAxisSetHoming")]
        int ExtAxisSetHoming(int axisID, int mode, double searchVel, double latchVel);

        [XmlRpcMethod("ExtAxisStartJog")]
        int ExtAxisStartJog(int motionCmd, int axisID, int direction, double vel, double acc, double maxDistance);

        [XmlRpcMethod("ExtAxisServoOn")]
        int ExtAxisServoOn(int axisID, int status);


        [XmlRpcMethod("TractorEnable")]
        int TractorEnable(int enable);

        [XmlRpcMethod("TractorHoming")]
        int TractorHoming();

        [XmlRpcMethod("TractorMoveL")]
        int TractorMoveL(double distance, double vel);

        [XmlRpcMethod("TractorMoveC")]
        int TractorMoveC(double radio, double angle, double vel);

        [XmlRpcMethod("SetWireSearchExtDIONum")]
        int SetWireSearchExtDIONum(int searchDoneDINum, int searchStartDONum);

        [XmlRpcMethod("SetWeldMachineCtrlModeExtDoNum")]
        int SetWeldMachineCtrlModeExtDoNum(int DONum);

        [XmlRpcMethod("SetWeldMachineCtrlMode")]
        int SetWeldMachineCtrlMode(int mode);

        [XmlRpcMethod("ExtAxisMoveJ")]
        int ExtAxisMoveJ(int syncFlag, double pos1, double pos2, double pos3, double pos4, double ovl);

        [XmlRpcMethod("SetAuxDO")]
        int SetAuxDO(int DONum, int bOpen, int smooth, int isNoblock);

        [XmlRpcMethod("SetAuxAO")]
        int SetAuxAO(int AONum, double value, int isNoblock);

        [XmlRpcMethod("SetAuxDIFilterTime")]
        int SetAuxDIFilterTime(int filterTime);

        [XmlRpcMethod("SetAuxAIFilterTime")]
        int SetAuxAIFilterTime(int AONum, int filterTime);

        [XmlRpcMethod("WaitAuxDI")]
        int WaitAuxDI(int DINum, int bOpen, int time, int errorAlarm);

        [XmlRpcMethod("WaitAuxAI")]
        int WaitAuxAI(int AINum, int sign, int value, int time, int errorAlarm);

        [XmlRpcMethod("GetAuxDI")]
        object[] GetAuxDI(int DINum, int isNoBlock);

        [XmlRpcMethod("GetAuxAI")]
        object[] GetAuxAI(int AINum, int isNoBlock);

        [XmlRpcMethod("ExtDevUDPClientComReset")]
        int ExtDevUDPClientComReset();

        [XmlRpcMethod("ExtDevUDPClientComClose")]
        int ExtDevUDPClientComClose();

        [XmlRpcMethod("ExtAxisParamConfig")]
        int ExtAxisParamConfig(int axisNum, int axisType, int axisDirection, double axisMax, double axisMin, double axisVel, double axisAcc, double axisLead, int encResolution, double axisOffect, int axisCompany, int axisModel, int axisEncType);

        [XmlRpcMethod("GetExAxisDriverConfig")]
        object[] GetExAxisDriverConfig(int axisId);

        [XmlRpcMethod("SetRobotPosToAxis")]
        int SetRobotPosToAxis(int installType);

        [XmlRpcMethod("SetAxisDHParaConfig")]
        int SetAxisDHParaConfig(int axisConfig, double axisDHd1, double axisDHd2, double axisDHd3, double axisDHd4, double axisDHa1, double axisDHa2, double axisDHa3, double axisDHa4);

        [XmlRpcMethod("ExtAxisSetRefPoint")]
        int ExtAxisSetRefPoint(int pointNum);

        [XmlRpcMethod("ExtAxisComputeECoordSys")]
        object[] ExtAxisComputeECoordSys();

        [XmlRpcMethod("ExtAxisActiveECoordSys")]
        int ExtAxisActiveECoordSys(int axisID, int toolNum, double x, double y, double z, double rx, double ry, double rz, int calibFlag);

        [XmlRpcMethod("SetRefPointInExAxisEnd")]
        int SetRefPointInExAxisEnd(double x, double y, double z, double rx, double ry, double rz);

        [XmlRpcMethod("PositionorSetRefPoint")]
        int PositionorSetRefPoint(int pointNum);

        [XmlRpcMethod("PositionorComputeECoordSys")]
        object[] PositionorComputeECoordSys();

        [XmlRpcMethod("JointOverSpeedProtectStart")]
        int JointOverSpeedProtectStart(int status, int speedPercent);

        [XmlRpcMethod("JointOverSpeedProtectEnd")]
        int JointOverSpeedProtectEnd();

        //V 3.7.4
        [XmlRpcMethod("WireSearchStart")]
        int WireSearchStart(int refPos, double searchVel, int searchDis, int autoBackFlag, double autoBackVel, int autoBackDis, int offectFlag);

        [XmlRpcMethod("WireSearchEnd")]
        int WireSearchEnd(int refPos, double searchVel, int searchDis, int autoBackFlag, double autoBackVel, int autoBackDis, int offectFlag);

        [XmlRpcMethod("GetWireSearchOffset")]
        object[] GetWireSearchOffset(int seamType, int mathod, string varNameRef1, string varNameRef2, string varNameRef3, string varNameRef4, string varNameRef5, string varNameRef6, string varNameRes1, string varNameRes2, string varNameRes3, string varNameRes4, string varNameRes5, string varNameRes6);

        [XmlRpcMethod("WireSearchWait")]
        int WireSearchWait(string name);

        [XmlRpcMethod("SetPointToDatabase")]
        int SetPointToDatabase(string varName, double[] pos);

        [XmlRpcMethod("ArcWeldTraceControl")]
        int ArcWeldTraceControl(int flag, double delaytime, int isLeftRight, double[] paramLR, int isUpLow, double[] paramUD, int axisSelect, int referenceType, double referSampleStartUd, double referSampleCountUd, double referenceCurrent, int offsetType, int offsetParameter);

        [XmlRpcMethod("ArcWeldTraceExtAIChannelConfig")]
        int ArcWeldTraceExtAIChannelConfig(int channel);

        [XmlRpcMethod("EndForceDragControl")]
        int EndForceDragControl(int status, int asaptiveFlag, int interfereDragFlag, int ingularityConstraintsFlag,double[] M, double[] B, double[] K, double[] F, double Fmax, double Vmax);

        [XmlRpcMethod("SetForceSensorDragAutoFlag")]
        int SetForceSensorDragAutoFlag(int status);

        [XmlRpcMethod("ForceAndJointImpedanceStartStop")]
        int ForceAndJointImpedanceStartStop(int status, int impedanceFlag, double[] lamdeDain, double[] KGain, double[] BGain, double dragMaxTcpVel, double dragMaxTcpOriVel);

        [XmlRpcMethod("GetForceAndTorqueDragState")]
        object[] GetForceAndTorqueDragState();

        [XmlRpcMethod("SetForceSensorPayload")]
        int SetForceSensorPayload(double weight);

        [XmlRpcMethod("SetForceSensorPayloadCog")]
        int SetForceSensorPayloadCog(double x, double y, double z);

        [XmlRpcMethod("GetForceSensorPayload")]
        object[] GetForceSensorPayload();

        [XmlRpcMethod("GetForceSensorPayloadCog")]
        object[] GetForceSensorPayloadCog();

        [XmlRpcMethod("ForceSensorSetSaveDataFlag")]
        int ForceSensorSetSaveDataFlag(int recordCount);

        [XmlRpcMethod("ForceSensorComputeLoad")]
        object[] ForceSensorComputeLoad();

        [XmlRpcMethod("GetSegmentWeldPoint")]
        object[] GetSegmentWeldPoint(double[] startPos, double[] endPos, double startDistance);

        [XmlRpcMethod("WeldingSetProcessParam")]
        int WeldingSetProcessParam(int id, double startCurrent, double startVoltage, double startTime, double weldCurrent, double weldVoltage, double endCurrent, double endVoltage, double endTime);

        [XmlRpcMethod("WeldingGetProcessParam")]
        object[] WeldingGetProcessParam(int id);

        [XmlRpcMethod("AxleSensorConfig")]
        int AxleSensorConfig(int idCompany, int idDevice, int idSoftware, int idBus);

        [XmlRpcMethod("AxleSensorConfigGet")]
        object[] AxleSensorConfigGet();

        [XmlRpcMethod("AxleSensorActivate")]
        int AxleSensorActivate(int actFlag);

        [XmlRpcMethod("AxleSensorRegWrite")]
        int AxleSensorRegWrite(int devAddr, int regHAddr, int regLAddr, int regNum, int data1, int data2, int isNoBlock);

        [XmlRpcMethod("SetOutputResetCtlBoxDO")]
        int SetOutputResetCtlBoxDO(int resetFlag);

        [XmlRpcMethod("SetOutputResetCtlBoxAO")]
        int SetOutputResetCtlBoxAO(int resetFlag);

        [XmlRpcMethod("SetOutputResetAxleDO")]
        int SetOutputResetAxleDO(int resetFlag);

        [XmlRpcMethod("SetOutputResetAxleAO")]
        int SetOutputResetAxleAO(int resetFlag);

        [XmlRpcMethod("SetOutputResetExtDO")]
        int SetOutputResetExtDO(int resetFlag);
        [XmlRpcMethod("SetOutputResetExtAO")]
        int SetOutputResetExtAO(int resetFlag);

        [XmlRpcMethod("SetOutputResetSmartToolDO")]
        int SetOutputResetSmartToolDO(int resetFlag);

        [XmlRpcMethod("WeaveStartSim")]
        int WeaveStartSim(int weaveNum);

        [XmlRpcMethod("WeaveEndSim")]
        int WeaveEndSim(int weaveNum);

        [XmlRpcMethod("WeaveInspectStart")]
        int WeaveInspectStart(int weaveNum);

        [XmlRpcMethod("WeaveInspectEnd")]
        int WeaveInspectEnd(int weaveNum);

        [XmlRpcMethod("SetAirControlExtDoNum")]
        int SetAirControlExtDoNum(int DONum);

        [XmlRpcMethod("SetArcStartExtDoNum")]
        int SetArcStartExtDoNum(int DONum);

        [XmlRpcMethod("SetWireReverseFeedExtDoNum")]
        int SetWireReverseFeedExtDoNum(int DONum);

        [XmlRpcMethod("SetWireForwardFeedExtDoNum")]
        int SetWireForwardFeedExtDoNum(int DONum);

        [XmlRpcMethod("SetArcDoneExtDiNum")]
        int SetArcDoneExtDiNum(int DINum);

        [XmlRpcMethod("SetWeldReadyExtDiNum")]
        int SetWeldReadyExtDiNum(int DINum);

        [XmlRpcMethod("SetExtDIWeldBreakOffRecover")]
        int SetExtDIWeldBreakOffRecover(int reWeldDINum, int abortWeldDINum);

        [XmlRpcMethod("SetCollisionDetectionMethod")]
        int SetCollisionDetectionMethod(int method, int thresholdMode);

        [XmlRpcMethod("SetStaticCollisionOnOff")]
        int SetStaticCollisionOnOff(int status);

        [XmlRpcMethod("SetPowerLimit")]
        int SetPowerLimit(int status, double power);

        [XmlRpcMethod("ServoJTStart")]
        int ServoJTStart();

        [XmlRpcMethod("ServoJT")]
        int ServoJT(double[] torque, double interval);

        [XmlRpcMethod("ServoJTEnd")]
        int ServoJTEnd();

        [XmlRpcMethod("SetRobotRealtimeStateSamplePeriod")]
        int SetRobotRealtimeStateSamplePeriod(int period);

        [XmlRpcMethod("GetRobotRealtimeStateSamplePeriod")]
        object[] GetRobotRealtimeStateSamplePeriod();

        [XmlRpcMethod("ArcWeldTraceReplayStart")]
        int ArcWeldTraceReplayStart();

        [XmlRpcMethod("ArcWeldTraceReplayEnd")]
        int ArcWeldTraceReplayEnd();

        [XmlRpcMethod("MultilayerOffsetTrsfToBase")]
        object[] MultilayerOffsetTrsfToBase(double pointOX, double pointOY, double pointOZ, double pointXX, double pointXY, double pointXZ, double pointZX, double pointZY, double pointZZ, double dx, double dy, double db);

        [XmlRpcMethod("AngularSpeedStart")]
        int AngularSpeedStart(int ratio);

        [XmlRpcMethod("AngularSpeedEnd")]
        int AngularSpeedEnd();

        [XmlRpcMethod("SoftwareUpgrade")]
        int SoftwareUpgrade();

        [XmlRpcMethod("AuxServoSetAcc")]
        int AuxServoSetAcc(double acc, double dec);

        [XmlRpcMethod("AuxServoSetEmergencyStopAcc")]
        int AuxServoSetEmergencyStopAcc(double acc, double dec);

        [XmlRpcMethod("AuxServoGetAcc")]
        object[] AuxServoGetAcc();

        [XmlRpcMethod("AuxServoGetEmergencyStopAcc")]
        object[] AuxServoGetEmergencyStopAcc();

        [XmlRpcMethod("GetAxleCommunicationParam")]
        object[] GetAxleCommunicationParam();

        [XmlRpcMethod("SetAxleCommunicationParam")]
        int SetAxleCommunicationParam(int baudRate, int dataBit, int stopBit, int verify, int timeout, int timeoutTimes, int period);

        [XmlRpcMethod("SetAxleFileType")]
        int SetAxleFileType(int type);

        [XmlRpcMethod("SetAxleLuaEnable")]
        int SetAxleLuaEnable(int enable);

        [XmlRpcMethod("SetRecoverAxleLuaErr")]
        int SetRecoverAxleLuaErr(int status);

        [XmlRpcMethod("GetAxleLuaEnableStatus")]
        object[] GetAxleLuaEnableStatus();

        [XmlRpcMethod("SetAxleLuaEnableDeviceType")]
        int SetAxleLuaEnableDeviceType(int forceSensorEnable, int gripperEnable, int IOEnable);

        [XmlRpcMethod("GetAxleLuaEnableDeviceType")]
        object[] GetAxleLuaEnableDeviceType();

        [XmlRpcMethod("GetAxleLuaEnableDevice")]
        object[] GetAxleLuaEnableDevice();

        [XmlRpcMethod("SetAxleLuaGripperFunc")]
        int SetAxleLuaGripperFunc(int id, int[] func);

        [XmlRpcMethod("GetAxleLuaGripperFunc")]
        object[] GetAxleLuaGripperFunc(int id);

        [XmlRpcMethod("SetCtrlOpenLUAName")]
        int SetCtrlOpenLUAName(int id, string name);

        [XmlRpcMethod("GetCtrlOpenLUAName")]
        object[] GetCtrlOpenLUAName();

        [XmlRpcMethod("LoadCtrlOpenLUA")]
        int LoadCtrlOpenLUA(int id);

        [XmlRpcMethod("UnloadCtrlOpenLUA")]
        int UnloadCtrlOpenLUA(int id);

        [XmlRpcMethod("SetCtrlOpenLuaErrCode")]
        int SetCtrlOpenLuaErrCode(int id, int code);

        [XmlRpcMethod("SlaveFileWrite")]
        int SlaveFileWrite(int type, int slaveID, string fileName);

        [XmlRpcMethod("SetSysServoBootMode")]
        int SetSysServoBootMode();

        [XmlRpcMethod("WeldingAbortWeldAfterBreakOff")]
        int WeldingAbortWeldAfterBreakOff();

        [XmlRpcMethod("WeldingStartReWeldAfterBreakOff")]
        int WeldingStartReWeldAfterBreakOff();

        [XmlRpcMethod("WeldingGetReWeldAfterBreakOffParam")]
        object[] WeldingGetReWeldAfterBreakOffParam();


        [XmlRpcMethod("WeldingSetReWeldAfterBreakOffParam")]
        int WeldingSetReWeldAfterBreakOffParam(int enable, double length, double velocity, int moveType);

        [XmlRpcMethod("WeldingGetCheckArcInterruptionParam")]
        object[] WeldingGetCheckArcInterruptionParam();

        [XmlRpcMethod("WeldingSetCheckArcInterruptionParam")]
        int WeldingSetCheckArcInterruptionParam(int checkEnable, int arcInterruptTimeLength);

        [XmlRpcMethod("ComputeWObjCoordWithPoints")]
        object[] ComputeWObjCoordWithPoints(int method, double[] param0, double[] param1, double[] param2, int refFrame);

        [XmlRpcMethod("ComputeToolCoordWithPoints")]
        object[] ComputeToolCoordWithPoints(int method, double[] param0, double[] param1, double[] param2, double[] param3, double[] param4,double[] param5);

        [XmlRpcMethod("LaserSensorRecord")]
        int LaserSensorRecord(int status, int delayMode, int delayTime, int delayDisExAxisNum, double delayDis, double sensitivePara, double speed);

        [XmlRpcMethod("WeaveChangeStart")]
        int WeaveChangeStart(int weaveChangeFlag, int weaveNum, double velStart, double velEnd);

        [XmlRpcMethod("WeaveChangeEnd")]
        int WeaveChangeEnd();

        [XmlRpcMethod("LoadTrajectoryLA")]
        int LoadTrajectoryLA(string name, int mode, double errorLim, int type, double precision, double vamx, double amax, double jmax);

        [XmlRpcMethod("MoveTrajectoryLA")]
        int MoveTrajectoryLA();

        [XmlRpcMethod("CustomCollisionDetectionStart")]
        int CustomCollisionDetectionStart(int flag, double[] jointDetectionThreshould, double[] tcpDetectionThreshould, int block);

        [XmlRpcMethod("CustomCollisionDetectionEnd")]
        int CustomCollisionDetectionEnd();



        [XmlRpcMethod("AccSmoothStart")]
        int AccSmoothStart(int saveFlag);

        [XmlRpcMethod("AccSmoothEnd")]
        int AccSmoothEnd(int saveFlag);

        [XmlRpcMethod("RbLogDownloadPrepare")]
        int RbLogDownloadPrepare();

        [XmlRpcMethod("AllDataSourceDownloadPrepare")]
        int AllDataSourceDownloadPrepare();

        [XmlRpcMethod("DataPackageDownloadPrepare")]
        int DataPackageDownloadPrepare();

        [XmlRpcMethod("GetRobotSN")]
        object[] GetRobotSN();

        [XmlRpcMethod("ShutDownRobotOS")]
        int ShutDownRobotOS();

        [XmlRpcMethod("ConveryComDetect")]
        int ConveryComDetect(int timeout);


        [XmlRpcMethod("ConveyorComDetectTrigger")]
        int ConveyorComDetectTrigger();

        [XmlRpcMethod("ArcWeldTraceAIChannelCurrent")]
        int ArcWeldTraceAIChannelCurrent(int channel);

        [XmlRpcMethod("ArcWeldTraceAIChannelVoltage")]
        int ArcWeldTraceAIChannelVoltage(int channel);

        [XmlRpcMethod("ArcWeldTraceCurrentPara")]
        int ArcWeldTraceCurrentPara(float AILow, float AIHigh, float currentLow, float currentHigh);

        [XmlRpcMethod("ArcWeldTraceVoltagePara")]
        int ArcWeldTraceVoltagePara(float AILow, float AIHigh, float voltageLow, float voltageHigh);

        [XmlRpcMethod("WeldingSetVoltageGradualChangeStart")]
        int WeldingSetVoltageGradualChangeStart(int IOType, double voltageStart, double voltageEnd, int AOIndex, int blend);

        [XmlRpcMethod("WeldingSetVoltageGradualChangeEnd")]
        int WeldingSetVoltageGradualChangeEnd();

        [XmlRpcMethod("WeldingSetCurrentGradualChangeStart")]
        int WeldingSetCurrentGradualChangeStart(int IOType, double currentStart, double currentEnd, int AOIndex, int blend);

        [XmlRpcMethod("WeldingSetCurrentGradualChangeEnd")]
        int WeldingSetCurrentGradualChangeEnd();

        [XmlRpcMethod("ExtAxisGetCoord")]
        object[] ExtAxisGetCoord();

        [XmlRpcMethod("FT_SpiralSearch")]
        int FT_SpiralSearch(int rcs, float dr, float ft, float max_t_ms, float max_vel);

    }
    internal class RPCHandle
    {
    }
}

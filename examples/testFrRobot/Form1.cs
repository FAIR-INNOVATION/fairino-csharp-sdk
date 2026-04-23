using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using fairino;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.AxHost;
using System.Xml.Linq;


namespace testFrRobot
{
    public partial class Form1 : Form
    {

        Robot robot;
        int rrpc ;


        public Form1()
        {
            InitializeComponent();
            robot = new Robot();//实例化机器人对象
            string path = "D://log/";
            robot.LoggerInit(FrLogType.BUFFER, FrLogLevel.INFO, path, 5, 5);
            robot.SetLoggerLevel(FrLogLevel.INFO);

            // 查看最终CNDE配置
            List<RobotState> finalStates;
            int finalPeriod;
            robot.GetRobotRealtimeStateConfig(out finalStates, out finalPeriod);
            Console.WriteLine($"最终配置状态数量: {finalStates.Count}");
            foreach (var s in finalStates) Console.WriteLine($"  {s}");
            Console.WriteLine($"最终周期: {finalPeriod} ms");


            robot.SetReconnectParam(true, 100, 1000);//断线重连参数
            rrpc = robot.RPC("192.168.58.2"); //与控制箱建立连接
                                              //20004端口接收超时时间
                                              //robot.SetReceivePortTimeout(40);


        }

        public void cndeconfigtest()
        {
            ////  
            // 2. 配置状态：JointCurPos, ToolCurPos，周期 20ms
            List<RobotState> states1 = new List<RobotState>
            {
                RobotState.JointCurPos,
                RobotState.ToolCurPos
            };
            int period1 = 20; // ms
            int ret = robot.SetRobotRealtimeStateConfig(states1, period1);
            Console.WriteLine($"初始配置结果: {ret}");

            // 8. 查看最终配置（仅用于验证）
            List<RobotState> finalStates;
            int finalPeriod;
            robot.GetRobotRealtimeStateConfig(out finalStates, out finalPeriod);
            Console.WriteLine($"最终配置状态数量: {finalStates.Count}");
            foreach (var s in finalStates) Console.WriteLine($"  {s}");
            Console.WriteLine($"最终周期: {finalPeriod} ms");

        }

        private void btnStandard_Click(object sender, EventArgs e)
        {

            string ip = "";
            string version = "";
            byte state = 0;

            robot.GetSDKVersion(ref version);
            Console.WriteLine($"SDK version : {version}");
            robot.GetControllerIP(ref ip);
            Console.WriteLine($"controller ip : {ip}");

            //robot.Mode(1);
            //Thread.Sleep(1000);
            //robot.DragTeachSwitch(1);
            //int rtn = robot.IsInDragTeach(ref state);
            //Console.WriteLine($"drag state : {state}");
            //Thread.Sleep(3000);
            //robot.DragTeachSwitch(0);
            //Thread.Sleep(1000);
            //robot.IsInDragTeach(ref state);
            //Console.WriteLine($"drag state : {state}");
            //Thread.Sleep(3000);
            //robot.RobotEnable(0);
            //Thread.Sleep(3000);
            //robot.RobotEnable(1);

            //robot.Mode(0);
            //Thread.Sleep(1000);
            //robot.Mode(1);
            //Thread.Sleep(2000);
           // robot.CloseRPC();
        }

        private void btnJOG_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                robot.StartJOG(0, i + 1, 0, 20.0f, 20.0f, 30.0f);
                Thread.Sleep(1000);
                robot.ImmStopJOG();
                Thread.Sleep(1000);
            }

            for (int i = 0; i < 6; i++)
            {
                robot.StartJOG(2, i + 1, 0, 20.0f, 20.0f, 30.0f);
                Thread.Sleep(1000);
                robot.ImmStopJOG();
                Thread.Sleep(1000);
            }

            for (int i = 0; i < 6; i++)
            {
                robot.StartJOG(4, i + 1, 0, 20.0f, 20.0f, 30.0f);
                Thread.Sleep(1000);
                robot.StopJOG(5);
                Thread.Sleep(1000);
            }

            for (int i = 0; i < 6; i++)
            {
                robot.StartJOG(8, i + 1, 0, 20.0f, 20.0f, 30.0f);
                Thread.Sleep(1000);
                robot.StopJOG(9);
                Thread.Sleep(1000);
            }

        }

        private void btnMovetest_Click(object sender, EventArgs e)
        {
            //JointPos j1= new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            //JointPos j2 = new JointPos(-45.615, -106.172, 124.296, -107.151, -91.282, 74.255);
            //JointPos j3 = new JointPos(-29.777, -84.536, 109.275, -114.075, -86.655, 74.257);
            //JointPos j4 = new JointPos(-31.154, -95.317, 94.276, -88.079, -89.740, 74.256);
            //DescPose desc_pos1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            //DescPose desc_pos2 = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);
            //DescPose desc_pos3 = new DescPose(-487.434, 154.362, 308.576, 176.600, 0.268, -14.061);
            //DescPose desc_pos4 = new DescPose(-443.165, 147.881, 480.951, 179.511, -0.775, -15.409);
            //DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            //ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);

            //int tool = 0;
            //int user = 0;
            //float vel = 100.0f;
            //float acc = 100.0f;
            //float ovl = 100.0f;
            //float blendT = 0.0f;
            //float blendR = 0.0f;
            //byte flag = 0;
            //byte search = 0;

            //robot.SetSpeed(20);
            //int rtn;
            //rtn = robot.MoveJ(j1, desc_pos1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            //Console.WriteLine($"MoveJ errcode:{rtn}" );

            //rtn = robot.MoveL(j2, desc_pos2, tool, user, vel, acc, ovl, blendR,epos, search, flag, offset_pos);
            //Console.WriteLine($"MoveL errcode:{rtn}");

            //rtn = robot.MoveC(j3, desc_pos3, tool, user, vel, acc, epos, flag, offset_pos, j4, desc_pos4, tool, user, vel, acc, epos, flag, offset_pos, ovl, blendR,0);
            //Console.WriteLine($"MoveC errcode:{rtn}");

            //rtn = robot.MoveJ(j2, desc_pos2, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            //Console.WriteLine("MoveJ errcode:%d\n", rtn);

            //rtn = robot.Circle(j3, desc_pos3, tool, user, vel, acc, epos, j1, desc_pos1, tool, user, vel, acc, epos, ovl, flag, offset_pos,0,0,0);
            //Console.WriteLine($"Circle errcode:{rtn}");

            //rtn = robot.MoveCart(desc_pos4, tool, user, vel, acc, ovl, blendT, -1);
            //Console.WriteLine($"MoveCart errcode:{rtn}");

        }

        private void btnDescSpiral_Click(object sender, EventArgs e)
        {
            int rtn;
            JointPos j = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            DescPose desc_pos = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose offset_pos1 = new DescPose(50, 0, 0, -30, 0, 0);
            DescPose offset_pos2 = new DescPose(50, 0, 0, -5, 0, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            SpiralParam sp;
            sp.circle_num = 5;
            sp.circle_angle = 5.0f;
            sp.rad_init = 50.0f;
            sp.rad_add = 10.0f;
            sp.rotaxis_add = 10.0f;
            sp.rot_direction = 0;

            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 100.0f;
            float blendT = 0.0f;
            byte flag = 2;

            robot.SetSpeed(20);

            rtn = robot.MoveJ(j, desc_pos, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos1);
            Console.WriteLine($"MoveJ errcode:{rtn}");

           // rtn = robot.NewSpiral(j, desc_pos, tool, user, vel, acc, epos, ovl, flag, offset_pos2, sp);
            Console.WriteLine($"NewSpiral errcode:{rtn}");
           
        }

        private void btnJointServoMove_Click(object sender, EventArgs e)
        {
            JointPos j = new JointPos(0, 0, 0, 0, 0, 0);

            float vel = 100.0f;
            float acc = 100.0f;
            float cmdT = 0.008f;
            float filterT = 0.0f;
            float gain = 0.0f;
            byte flag = 0;
            int count = 2000;
            float dt = 0.01f;

            int ret = robot.GetActualJointPosDegree(flag, ref j);
            ExaxisPos axis = new ExaxisPos(0, 0, 0, 0);

            if (ret == 0)
            {
                while (count > 0)
                {
                    robot.ServoJ(j, axis, acc, vel, cmdT, filterT, gain);
                    j.jPos[0] += dt;
                    count -= 1;
                    robot.WaitMs((int)(cmdT * 1000));
                }
            }
            else
            {
                Console.WriteLine($"GetActualJointPosDegree errcode:  {ret}");
            }

        }

        private void btnDescServoMove_Click(object sender, EventArgs e)
        {
            DescPose desc_pos_dt = new DescPose(0, 0, 0, 0, 0, 0);

            desc_pos_dt.tran.z = -0.5;
            double[] pos_gain = new double[6]{ 0.0, 0.0, 1.0, 0.0, 0.0, 0.0 };
            int mode = 2;
            float vel = 0.0f;
            float acc = 0.0f;
            float cmdT = 0.008f;
            float filterT = 0.0f;
            float gain = 0.0f;
            int count = 500;

            robot.SetSpeed(20);

            while (count > 0)
            {
                //robot.ServoCart(mode, desc_pos_dt, pos_gain, acc, vel, cmdT, filterT, gain);
                count -= 1;
                robot.WaitMs((int)(cmdT * 1000));
            }
        }

        private void btnDescPTPMove_Click(object sender, EventArgs e)
        {
            DescPose desc_pos1, desc_pos2, desc_pos3;

            desc_pos1 = new DescPose(-437.039, 411.064, 426.189, -177.886, 2.007, 31.155);
            desc_pos2 = new DescPose(-525.55, 562.3, 417.199, -178.325, 0.847, 31.109);
            desc_pos3 = new DescPose(-345.155, 535.733, 421.269, 179.475, 0.571, 18.332);

            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 100.0f;
            float blendT = -1.0f;
            float blendT1 = 0.0f;
            int config = -1;

            robot.SetSpeed(20);
            robot.MoveCart(desc_pos1, tool, user, vel, acc, ovl, blendT, config);
            robot.MoveCart(desc_pos2, tool, user, vel, acc, ovl, blendT, config);
            robot.MoveCart(desc_pos3, tool, user, vel, acc, ovl, blendT1, config);
        }

        private void btnSplineMove_Click(object sender, EventArgs e)
        {

            JointPos j1 = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            JointPos j2 = new JointPos(-45.615, -106.172, 124.296, -107.151, -91.282, 74.255);
            JointPos j3 = new JointPos(-61.954, -84.409, 108.153, -116.316, -91.283, 74.260);
            JointPos j4 = new JointPos(-89.575, -80.276, 102.713, -116.302, -91.284, 74.267);
            DescPose desc_pos1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose desc_pos2 = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);
            DescPose desc_pos3 = new DescPose(-327.622, 402.230, 320.402, -178.067, 2.127, -46.207);
            DescPose desc_pos4 = new DescPose(-104.066, 544.321, 327.023, -177.715, 3.371, -73.818);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);

            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 100.0f;
            float blendT = -1.0f;
            byte flag = 0;

            robot.SetSpeed(20);

            int err = -1;
            err = robot.MoveJ(j1, desc_pos1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            Console.WriteLine($"movej errcode:  {err}");
           
            robot.SplineStart();
            robot.SplinePTP(j1, desc_pos1, tool, user, vel, acc, ovl);
            robot.SplinePTP(j2, desc_pos2, tool, user, vel, acc, ovl);
            robot.SplinePTP(j3, desc_pos3, tool, user, vel, acc, ovl);
            robot.SplinePTP(j4, desc_pos4, tool, user, vel, acc, ovl);
            robot.SplineEnd();
        }

        private void btnPointOffect_Click(object sender, EventArgs e)
        {
            JointPos j1, j2;
            DescPose desc_pos1, desc_pos2, offset_pos, offset_pos1;
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);

            j1 = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            desc_pos1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);

            j2 = new JointPos(-45.615, -106.172, 124.296, -107.151, -91.282, 74.255);

            desc_pos2 = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);

            offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            offset_pos1 = new DescPose(50.0, 50.0, 50.0, 5.0, 5.0, 5.0);

            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 100.0f;
            float blendT = -1.0f;
            byte flag = 0;
            int type = 0;

            robot.SetSpeed(20);

            robot.MoveJ(j1, desc_pos1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            robot.MoveJ(j2, desc_pos2, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            Thread.Sleep(1000);
            robot.PointsOffsetEnable(type, offset_pos1);
            robot.MoveJ(j1, desc_pos1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            robot.MoveJ(j2, desc_pos2, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            robot.PointsOffsetDisable();
        }

        private void btnIOTest_Click(object sender, EventArgs e)
        {
            //int resetFlag = 0;
            //int rtn = robot.SetOutputResetCtlBoxDO(resetFlag);
            //robot.SetOutputResetCtlBoxAO(resetFlag);
            //robot.SetOutputResetAxleDO(resetFlag);
            //robot.SetOutputResetAxleAO(resetFlag);
            //robot.SetOutputResetExtDO(resetFlag);
            //robot.SetOutputResetExtAO(resetFlag);
            //robot.SetOutputResetSmartToolDO(resetFlag);

            ////robot.SetDO(1, 1, 0, 0);
            ////robot.SetDO(3, 1, 0, 0);
            ////robot.SetAO(0, 50, 0);
            ////robot.SetAO(1, 70, 0);
            ////robot.SetToolDO(1, 1, 0, 0);
            ////robot.SetToolAO(0, 40, 0);

            //robot.SetAuxDO(1, true, false, true);
            //robot.SetAuxDO(2, true, false, true);

            //robot.SetAuxAO(0, 1024, false);
            //robot.SetAuxAO(1, 2048, false);
            //return ;













            ////int rtn = robot.WaitMultiDI(0, 768, 768, 10000, 0);
            ////Console.WriteLine(rtn);

            ////int[] DIConfig = new int[8] { 1, 26, 3, 4, 0, 0, 0, 0};
            ////int[] DILevelConfig = new int[8] { 0, 1, 0, 1, 0, 1, 0, 1};
            ////robot.SetDIConfigLevel(DILevelConfig);
            ////robot.SetDIConfig(DIConfig);
            ////robot.WaitAI(0, 0, 0.53f, 1000, 2);
            ////float fff = 0.0f;
            ////for (int j = 0; j < 100; j++)
            ////{
            ////    robot.GetAI(0, 0, ref fff);
            ////    Console.WriteLine($"the ai is {fff}");
            ////    Thread.Sleep(100);
            ////}
            ////return;

            //byte status = 1;
            //byte smooth = 0;
            //byte block = 0;
            //byte di = 0, tool_di = 0;
            //float ai = 0.0f, tool_ai = 0.0f;
            //float value = 0.0f;
            //int doH = 0;
            //int doL = 0;
            //int i;
            //rtn = 0;
            //for (i = 0; i < 16; i++)//所有控制器IO输出置 1
            //{
            //    rtn = robot.SetDO(i, status, smooth, block);
            //    robot.WaitMs(500);

            //    robot.GetDO(ref doH, ref doL);
            //    Console.WriteLine($"setDO  {i}: {rtn}  getDO {doH}  {doL}");
            //}

            //status = 0;

            //for (i = 0; i < 16; i++)//所有控制器IO输出置 0
            //{
            //    robot.SetDO(i, status, smooth, block);
            //    robot.WaitMs(500);
            //    robot.GetDO(ref doH, ref doL);
            //    Console.WriteLine($"setDO  {i}: {rtn}  getDO {doH}  {doL}");
            //}

            //status = 1;

            //for (i = 0; i < 2; i++)//所有工具端IO输出置 1
            //{
            //    robot.SetToolDO(i, status, smooth, block);
            //    robot.WaitMs(500);
            //}

            //status = 0;

            //for (i = 0; i < 2; i++)//所有工具端IO输出置 0
            //{
            //    robot.SetToolDO(i, status, smooth, block);
            //    robot.WaitMs(500);
            //}

            //value = 50.0f;
            //robot.SetAO(0, value, block);//设置控制器0号模拟量输出50%
            //value = 100.0f;
            //robot.SetAO(1, value, block);//设置控制器1号模拟量输出100%
            //robot.WaitMs(300);
            //value = 0.0f;
            //robot.SetAO(0, value, block);//设置控制器0号模拟量输出0%
            //value = 0.0f;
            //robot.SetAO(1, value, block);//设置控制器1号模拟量输出0%

            //value = 100.0f;
            //robot.SetToolAO(0, value, block);//设置工具端0号模拟量输出100%
            //robot.WaitMs(1000);
            //value = 0.0f;
            //robot.SetToolAO(0, value, block);//设置工具端0号模拟量输出0%

            //robot.GetDI(0, block, ref di);//获取数字输入
            //Console.WriteLine($"di0 : {di}");
            //robot.WaitDI(0, 1, 0, 2);       //等待0号端口数字量输入1，一直等待
            //Console.WriteLine("wait di success");
            //robot.WaitMultiDI(0, 3, 0, 10000, 2);   //等待多路与， 0和1端口，输入置1，等待时间10000ms， 一直等待
            //Console.WriteLine("wait multi di success");
            //robot.GetToolDI(1, block, ref tool_di);//获取工具端数字量输入
            //Console.WriteLine($"tool_di1 : {tool_di}");
            //robot.WaitToolDI(1, 0, 0, 2);          //一直等待
            //Console.WriteLine("wait tool di success");
            //robot.GetAI(0, block, ref ai);
            //Console.WriteLine($"ai0 : {ai}");
            //robot.GetAI(1, block, ref ai);
            //Console.WriteLine($"ai1 : {ai}");
            //robot.WaitAI(0, 1, 50.0f, 0, 2);           //等待0号口， 小于 ， %50， 一直等待
            //Console.WriteLine("wait ai success");
            //robot.WaitToolAI(0, 1, 50, 0, 2);       //一直等待
            //Console.WriteLine("wait tool ai success");
            //robot.GetToolAI(0, block, ref tool_ai);
            //Console.WriteLine($"tool_ai0 : {tool_ai}");
        }

        private void btnCommonSets_Click(object sender, EventArgs e)
        {
            int i;
            double value = 0;
            int id;
            int type;
            int install;
            int toolID=0;
            int loadNum=0;
            int refFrame = 0;
            DescTran coord = new DescTran();
            DescPose t_coord, etcp, etool, w_coord;
            t_coord = new DescPose();
            etcp = new DescPose();
            w_coord = new DescPose();

            robot.SetSpeed(20);

            for (i = 1; i < 21; i++)
            {
                robot.SetSysVarValue(i, (float)(i + 0.5));
                robot.WaitMs(100);
            }

            for (i = 1; i < 21; i++)
            {
                robot.GetSysVarValue(i, ref value);
                Console.WriteLine($"sys value {i} : {value}");
            }

            int loadrtn = robot.SetLoadWeight(1,2.5f);
            Console.WriteLine($"load rtn: {loadrtn}");
            coord.x = 3.0;
            coord.y = 4.0;
            coord.z = 5.0;
            robot.SetLoadCoord(coord);
         
            id = 3;
            t_coord.tran.x = 1.0;
            t_coord.tran.y = 2.0;
            t_coord.tran.z = 300.0;
            t_coord.rpy.rx = 4.0;
            t_coord.rpy.ry = 5.0;
            t_coord.rpy.rz = 6.0;
            type = 0;
            install = 0;

            int rtn1 = -1;
            int rtn2 = -1;
            rtn1 = robot.SetToolCoord(id, t_coord, type, install, toolID, loadNum);
            rtn2 = robot.SetToolList(id, t_coord, type, install, loadNum);
            Console.WriteLine($"set tool coord result {rtn1}, set tool list rtn{rtn2}");
      
            etcp.tran.x = 1.0;
            etcp.tran.y = 2.0;
            etcp.tran.z = 3.0;
            etcp.rpy.rx = 4.0;
            etcp.rpy.ry = 5.0;
            etcp.rpy.rz = 6.0;
            etool.tran.x = 11.0;
            etool.tran.y = 22.0;
            etool.tran.z = 330.0;
            etool.rpy.rx = 44.0;
            etool.rpy.ry = 55.0;
            etool.rpy.rz = 66.0;
            id = 5;
            robot.SetExToolCoord(id, etcp, etool);
            robot.SetExToolList(id, etcp, etool);

            w_coord.tran.x = 110.0;
            w_coord.tran.y = 12.0;
            w_coord.tran.z = 13.0;
            w_coord.rpy.rx = 14.0;
            w_coord.rpy.ry = 15.0;
            w_coord.rpy.rz = 16.0;
            id = 12;
            robot.SetWObjCoord(id, w_coord, refFrame);
            //robot.SetWObjList(id, w_coord);

            double yangle = 0, zangle = 0;
            robot.SetRobotInstallPos(1);//侧装
            robot.SetRobotInstallAngle(15.0, 25.0);
            Thread.Sleep(1000);
            robot.GetRobotInstallAngle(ref yangle, ref zangle);
            Console.WriteLine($"yangle  {yangle}   zangle  {zangle}");
            robot.SetRobotInstallAngle(10.0, 10.0);
            Thread.Sleep(1000);
            robot.GetRobotInstallAngle(ref yangle, ref zangle);
            Console.WriteLine($"yangle  {yangle}   zangle  {zangle}");
        }

        private void btnRobotSafetySet_Click(object sender, EventArgs e)
        {
            robot.SetStaticCollisionOnOff(1);
            return;
            int mode = 0;
            int config = 1;
            double[] level1 = new double[6] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 };
            double[] level2 = new double[6] { 0.5, 0.2, 0.3, 0.4, 0.5, 0.12 };
            int[] safetyMargin = new int[6] { 1, 1, 1, 1, 1, 1 }; //j1 - j6安全系数[1 - 10]
            robot.SetAnticollision(mode, level1, config);
            mode = 1;
            robot.SetAnticollision(mode, level2, config);
            robot.SetCollisionStrategy(2,1000,20, 150,safetyMargin);

            double[] plimit = new double[6] { 170.0, 80.0, 150.0, 80.0, 170.0, 160.0 };
            int rtn = robot.SetLimitPositive(plimit);
            Console.WriteLine($"SetLimitPositive  rtn  {rtn}");
            double[] nlimit = new double[6] { -170.0, -260.0, -150.0, -260.0, -170.0, -160.0 };
            rtn = robot.SetLimitNegative(nlimit);
            Console.WriteLine($"SetLimitNegative  rtn  {rtn}");

            robot.ResetAllError();

            double[] lcoeff = new double[6] { 0.9, 0.9, 0.9, 0.9, 0.9, 0.9 };
            double[] wcoeff = new double[6] { 0.4, 0.4, 0.4, 0.4, 0.4, 0.4 };
            double[] ccoeff = new double[6] { 0.6, 0.6, 0.6, 0.6, 0.6, 0.6 };
            double[] fcoeff = new double[6] { 0.5, 0.5, 0.5, 0.5, 0.5, 0.5 };
            robot.FrictionCompensationOnOff(1);
            rtn = robot.SetFrictionValue_level(lcoeff);
            Console.WriteLine($"SetFrictionValue_level  rtn  {rtn}");
            rtn = robot.SetFrictionValue_wall(wcoeff);
            Console.WriteLine($"SetFrictionValue_wall  rtn  {rtn}");
            rtn = robot.SetFrictionValue_ceiling(ccoeff);
            Console.WriteLine($"SetFrictionValue_ceiling  rtn  {rtn}");
            rtn = robot.SetFrictionValue_freedom(fcoeff);
            Console.WriteLine($"SetFrictionValue_freedom  rtn  {rtn}");
        }

        private void btnRobotState_Click(object sender, EventArgs e)
        {
            robot.ProgramLoad("/fruser/test2.lua");
            robot.ProgramRun();
            while (true)
            {
                double[] temperature = new double[6];
                robot.GetJointDriverTemperature(temperature);
                double[] torque = new double[6];
                robot.GetJointDriverTorque(torque);
                Console.WriteLine($"torque is {torque[0] }  {torque[1]}  { torque[2]}  { torque[3]}  { torque[4]} { torque[5]}  temperature  {temperature[0]}  {temperature[1]}  { temperature[2]}  { temperature[3]}  { temperature[4]}  { temperature[5]}");
                
                Thread.Sleep(100);
            }

            //ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
            //robot.GetRobotRealTimeState(ref pkg);
            //Console.WriteLine($"torque is {pkg.jointDriverTorque[0]}  {pkg.jointDriverTorque[1]}  {pkg.jointDriverTorque[2]}  {pkg.jointDriverTorque[3]}  {pkg.jointDriverTorque[4]}  {pkg.jointDriverTorque[5]}   temperature  {pkg.jointDriverTemperature[0]}  {pkg.jointDriverTemperature[1]}  {pkg.jointDriverTemperature[2]}  {pkg.jointDriverTemperature[3]}  {pkg.jointDriverTemperature[4]}  {pkg.jointDriverTemperature[5]}  ");

            //double yangle = 0, zangle = 0;
            //byte flag = 0;
            //JointPos j_deg = new JointPos(0, 0, 0, 0, 0, 0);
            //JointPos j_rad = new JointPos(0, 0, 0, 0, 0, 0);
            //DescPose tcp, flange, tcp_offset, wobj_offset;
            //DescTran cog;
            //tcp = new DescPose();
            //flange = new DescPose();
            //tcp_offset = new DescPose();
            //wobj_offset = new DescPose();
            //cog = new DescTran();

            //int id = 0;
            //double[] torques = new double[6] { 0, 0, 0, 0, 0, 0};
            //double weight = 0.0;
            //double[] neg_deg = new double[6] { 0, 0, 0, 0, 0, 0 };
            //double[] pos_deg = new double[6] { 0, 0, 0, 0, 0, 0 };
            //double t_ms = 0;
            //int config = 0;
            //double vel = 0;

            //robot.GetRobotInstallAngle(ref yangle, ref zangle);
            //Console.WriteLine($"yangle:{yangle},zangle:{zangle}");

            //robot.GetActualJointPosDegree(flag, ref j_deg);
            //Console.WriteLine($"joint pos deg:{j_deg.jPos[0]},{j_deg.jPos[1]},{j_deg.jPos[2]},{j_deg.jPos[3]},{j_deg.jPos[4]},{j_deg.jPos[5]}");

            //robot.GetActualJointPosRadian(flag, ref j_rad);
            //Console.WriteLine($"joint pos rad:{j_rad.jPos[0]},{j_rad.jPos[1]},{j_rad.jPos[2]},{j_rad.jPos[3]},{j_rad.jPos[4]},{j_rad.jPos[5]}");

            //robot.GetActualTCPPose(flag, ref tcp);
            //Console.WriteLine($"tcp pose:{tcp.tran.x},{tcp.tran.y},{tcp.tran.z},{tcp.rpy.rx},{tcp.rpy.ry},{tcp.rpy.rz}");

            //robot.GetActualToolFlangePose(flag, ref flange);
            //Console.WriteLine($"flange pose:{flange.tran.x},{flange.tran.y},{flange.tran.z},{flange.rpy.rx},{flange.rpy.ry},{flange.rpy.rz}");

            //robot.GetActualTCPNum(flag, ref id);
            //Console.WriteLine($"tcp num : {id}");

            //robot.GetActualWObjNum(flag, ref id);
            //Console.WriteLine($"wobj num : {id}");

            //robot.GetJointTorques(flag, torques);
            //Console.WriteLine($"torques:{torques[0]},{torques[1]},{torques[2]},{torques[3]},{torques[4]},{torques[5]}");

            //robot.GetTargetPayload(flag, ref weight);
            //Console.WriteLine($"payload weight : {weight}");

            //robot.GetTargetPayloadCog(flag, ref cog);
            //Console.WriteLine($"payload cog:{cog.x},{cog.y},{cog.z}");

            //robot.GetTCPOffset(flag, ref tcp_offset);
            //Console.WriteLine($"tcp offset:{tcp_offset.tran.x},{tcp_offset.tran.y},{tcp_offset.tran.z},{tcp_offset.rpy.rx},{tcp_offset.rpy.ry},{tcp_offset.rpy.rz}");

            //robot.GetWObjOffset(flag, ref wobj_offset);
            //Console.WriteLine($"wobj offset:{wobj_offset.tran.x},{wobj_offset.tran.y},{wobj_offset.tran.z},{wobj_offset.rpy.rx},{wobj_offset.rpy.ry},{wobj_offset.rpy.rz}");

            //robot.GetJointSoftLimitDeg(flag, ref neg_deg, ref pos_deg);
            //Console.WriteLine($"neg limit deg:{neg_deg[0]},{neg_deg[1]},{neg_deg[2]},{neg_deg[3]},{neg_deg[4]},{neg_deg[5]}");
            //Console.WriteLine($"pos limit deg:{pos_deg[0]},{pos_deg[1]},{pos_deg[2]},{pos_deg[3]},{pos_deg[4]},{pos_deg[5]}");

            //robot.GetSystemClock(ref t_ms);
            //Console.WriteLine($"system clock : {t_ms}");

            //robot.GetRobotCurJointsConfig(ref config);
            //Console.WriteLine($"joint config : {config}");

            //robot.GetDefaultTransVel(ref vel);
            //Console.WriteLine($"trans vel : {vel}");
        }

        private void btnTCPRecord_Click(object sender, EventArgs e)
        {
            int type = 1;
            string name = "tpd2023";
            int period_ms = 2;
            UInt16 di_choose = 0;
            UInt16 do_choose = 0;

            robot.SetTPDParam(type, name, period_ms, di_choose, do_choose);

            robot.Mode(1);
            Thread.Sleep(1000);
            robot.DragTeachSwitch(1);
            robot.SetTPDStart(type, name, period_ms, di_choose, do_choose);
            Thread.Sleep(10000);
            robot.SetWebTPDStop();
            robot.DragTeachSwitch(0);

            //robot.SetTPDDelete(name);
        }

        private void btnTPDMove_Click(object sender, EventArgs e)
        {
            string name = "tpd2023";
            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 100.0f;
            float blendT = -1.0f;
            int config = -1;
            byte blend = 1;

            DescPose desc_pose = new DescPose();
            robot.GetTPDStartPose(name, ref desc_pose);
            Console.WriteLine($"GetTPDStartPose:{desc_pose.tran.x},{desc_pose.tran.y},{desc_pose.tran.z},{desc_pose.rpy.rx},{desc_pose.rpy.ry},{desc_pose.rpy.rz}");
            robot.SetTrajectoryJSpeed(100.0f);

            robot.LoadTPD(name);
            robot.MoveCart(desc_pose, tool, user, vel, acc, ovl, blendT, config);
            robot.MoveTPD(name, blend, 100.0f);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            robot.CloseRPC();
        }

        private void btnWebApp_Click(object sender, EventArgs e)
        {
            string program_name = "/fruser/test4.lua";
            string loaded_name = "";
            byte state = 0;
            int line = 0;

            robot.Mode(0);
            robot.ProgramLoad(program_name);
            robot.ProgramRun();
            Thread.Sleep(2000);
            robot.ProgramPause();
            robot.GetProgramState(ref state);
            Console.WriteLine($"program state : {state}");
            robot.GetCurrentLine(ref line);
            Console.WriteLine($"current line : {line}");
            robot.GetLoadedProgram(ref loaded_name);
            Console.WriteLine($"program name : {loaded_name}");
            Thread.Sleep(1000);
            robot.ProgramResume();
            Thread.Sleep(1000);
            robot.ProgramStop();
        }

        private void btnOperateGripper_Click(object sender, EventArgs e)
        {
            int company = 4;
            int device = 0;
            int softversion = 0;
            int bus = 1;
            int index = 1;
            byte act = 0;
            int max_time = 30000;
            byte block = 0;
            int status = -1, fault = -1;
            int rtn = -1;
            int deviceID = -1;
            int type = 0;
            double rotNum = 0;
            int rotVel = 50;
            int rotTorque = 50;
            robot.SetGripperConfig(company, device, softversion, bus);
            Thread.Sleep(1000);
            robot.GetGripperConfig(ref deviceID, ref company, ref device, ref softversion);
            Console.WriteLine($"gripper config :{deviceID} {company}, {device}, {softversion}");

            rtn = robot.ActGripper(index, act);
            Console.WriteLine($"ActGripper rtn : {rtn}");
            Thread.Sleep(1000);
            act = 1;
            rtn = robot.ActGripper(index, act);
            Console.WriteLine($"ActGripper rtn : {rtn}");
            Thread.Sleep(4000);

            rtn = robot.MoveGripper(index, 20, 50, 50, max_time, block, type, rotNum, rotVel,rotTorque);
            Console.WriteLine($"MoveGripper rtn : {rtn}");
            Thread.Sleep(2000);
            robot.MoveGripper(index, 10, 50, 0, max_time, block, type, rotNum, rotVel, rotTorque);

            Thread.Sleep(4000);
            rtn = robot.GetGripperMotionDone(ref fault, ref status);
            Console.WriteLine($"gripper motion done : {fault}, {status}   rtn : {rtn}");

            int current = -1;
            int tempture = -1;
            int voltage = -1;
            int position = -1;
            int activestatus = -2;
            int speed = -1;
            rtn = robot.GetGripperCurCurrent(ref fault, ref current);
            Console.WriteLine($"current { current}  rtn { rtn} fault { fault} ");
            rtn = robot.GetGripperCurPosition(ref fault, ref position);
            Console.WriteLine($"position {position}  rtn {rtn} fault {fault} ");

            rtn = robot.GetGripperActivateStatus(ref fault, ref activestatus);
            Console.WriteLine($"activestatus {activestatus}  rtn {rtn} fault {fault} ");

            rtn = robot.GetGripperCurSpeed(ref fault, ref speed);
            Console.WriteLine($"speed {speed}  rtn {rtn} fault {fault} ");

            rtn = robot.GetGripperVoltage(ref fault, ref voltage);
            Console.WriteLine($"voltage {voltage}  rtn {rtn} fault {fault} ");

            rtn = robot.GetGripperTemp(ref fault, ref tempture);
            Console.WriteLine($"voltage {tempture}  rtn {rtn} fault {fault} ");
        }

        private void btnFT_Click(object sender, EventArgs e)
        {
            int company = 17;
            int device = 0;
            int softversion = 0;
            int bus = 1;
            byte act = 0;
            int deviceID = -1;

            robot.FT_SetConfig(company, device, softversion, bus);
            Thread.Sleep(1000);
            company = 0;
            robot.FT_GetConfig(ref deviceID, ref company, ref device, ref softversion);
            Console.WriteLine($"FT config : {deviceID},{company}, {device}, {softversion}");
            Thread.Sleep(1000);

            robot.FT_Activate(act);
            Thread.Sleep(1000);
            act = 1;
            robot.FT_Activate(act);
            Thread.Sleep(1000);

            robot.SetLoadWeight(1,0.0f);
            Thread.Sleep(1000);
            DescTran coord = new DescTran(0, 0, 0);
          
            robot.SetLoadCoord(coord);
            Thread.Sleep(1000);
            robot.FT_SetZero(0);//0去除零点  1零点矫正
            Thread.Sleep(1000);

            ForceTorque ft = new ForceTorque(0, 0, 0, 0, 0, 0);
            int rtn = robot.FT_GetForceTorqueOrigin(1, ref ft);
            Console.WriteLine($"ft origin : {ft.fx}, {ft.fy}, { ft.fz}, { ft.tx}, { ft.ty}, { ft.tz}    rtn   {rtn}");
            rtn = robot.FT_SetZero(1);//零点矫正
            Console.WriteLine($"set zero rtn {rtn}");

            Thread.Sleep(2000);
            rtn = robot.FT_GetForceTorqueOrigin(1, ref ft);
            Console.WriteLine($"FT_GetForceTorqueOrigin : {ft.fx}, {ft.fy}, {ft.fz}, {ft.tx}, {ft.ty}, {ft.tz}  rtn  {rtn}");

            robot.FT_GetForceTorqueRCS(1, ref ft);
            Console.WriteLine($"FT_GetForceTorqueRCS rcs : {ft.fx}, {ft.fy}, {ft.fz}, {ft.tx}, {ft.ty}, {ft.tz}");
        }

        private void btnFTPdCog_Click(object sender, EventArgs e)
        {
            DescPose coord = new DescPose(0, 0, 1, 0, 0, 0);
            robot.FT_SetRCS(0, coord);
            return;

            double weight = 0.1;
            int rtn = -1;

            DescPose tcoord, desc_p1, desc_p2, desc_p3;
            tcoord = new DescPose(0, 0, 0, 0, 0, 0);
            desc_p1 = new DescPose(0, 0, 0, 0, 0, 0);
            desc_p2 = new DescPose(0, 0, 0, 0, 0, 0);
            desc_p3 = new DescPose(0, 0, 0, 0, 0, 0);

            //DescPose coord = new DescPose(0, 0, 1, 0, 0, 0);
            robot.FT_SetRCS(0, coord);
            Thread.Sleep(1000);

            tcoord.tran.z = 35.0;
            robot.SetToolCoord(10, tcoord, 1, 0,0,0);
            Thread.Sleep(1000);
            robot.FT_PdIdenRecord(10);
            Thread.Sleep(1000);
            robot.FT_PdIdenCompute(ref weight);
            Console.WriteLine($"payload weight : {weight}");

            desc_p1.tran.x = -47.805;
            desc_p1.tran.y = -362.266;
            desc_p1.tran.z = 317.754;
            desc_p1.rpy.rx = -179.496;
            desc_p1.rpy.ry = -0.255;
            desc_p1.rpy.rz = 34.948;

            desc_p2.tran.x = -77.805;
            desc_p2.tran.y = -312.266;
            desc_p2.tran.z = 317.754;
            desc_p2.rpy.rx = -179.496;
            desc_p2.rpy.ry = -0.255;
            desc_p2.rpy.rz = 34.948;

            desc_p3.tran.x = -167.805;
            desc_p3.tran.y = -312.266;
            desc_p3.tran.z = 387.754;
            desc_p3.rpy.rx = -179.496;
            desc_p3.rpy.ry = -0.255;
            desc_p3.rpy.rz = 34.948;

            rtn = robot.MoveCart(desc_p1, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            Console.WriteLine($"MoveCart rtn  {rtn}");
            Thread.Sleep(1000);
            robot.FT_PdCogIdenRecord(10, 1);
            robot.MoveCart(desc_p2, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            Thread.Sleep(1000);
            robot.FT_PdCogIdenRecord(10, 2);
            robot.MoveCart(desc_p3, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            Thread.Sleep(1000);
            robot.FT_PdCogIdenRecord(10, 3);
            Thread.Sleep(1000);
            DescTran cog = new DescTran(0, 0, 0);
            
            robot.FT_PdCogIdenCompute(ref cog);
            Console.WriteLine($"cog : {cog.x}, {cog.y}, {cog.z}");
        }

        private void btnFTGuard_Click(object sender, EventArgs e)
        {
            byte flag = 1;
            byte sensor_id = 1;
            int[] select = new int[6]{ 1, 0, 0, 0, 0, 0 };//只启用x轴碰撞守护
            double[] max_threshold = new double[6]{ 5.0f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f };
            double[] min_threshold = new double[6]{ 3.0f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f };

            ForceTorque ft = new ForceTorque(0, 0, 0, 0, 0, 0);
            DescPose desc_p1, desc_p2, desc_p3;
            desc_p1 = new DescPose(0, 0, 0, 0, 0, 0);
            desc_p2 = new DescPose(0, 0, 0, 0, 0, 0);
            desc_p3 = new DescPose(0, 0, 0, 0, 0, 0);

            desc_p1.tran.x = 1.299;
            desc_p1.tran.y = -719.159;
            desc_p1.tran.z = 141.314;
            desc_p1.rpy.rx = 177.999;
            desc_p1.rpy.ry = -0.715;
            desc_p1.rpy.rz = -161.937;

            desc_p2.tran.x = 245.047;
            desc_p2.tran.y = -675.509;
            desc_p2.tran.z = 139.538;
            desc_p2.rpy.rx = 177.987;
            desc_p2.rpy.ry = -0.129;
            desc_p2.rpy.rz = -142.238;

            desc_p3.tran.x = 157.233;
            desc_p3.tran.y = -550.088;
            desc_p3.tran.z = 112.485;
            desc_p3.rpy.rx = -176.579;
            desc_p3.rpy.ry = -2.819;
            desc_p3.rpy.rz = -148.415;
            robot.SetSpeed(5);

            int rtn =  robot.FT_Guard(flag, sensor_id, select, ft, max_threshold, min_threshold);
            Console.WriteLine($"FT_Guard start rtn {rtn}");
            robot.MoveCart(desc_p1, 1, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            robot.MoveCart(desc_p2, 1, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            robot.MoveCart(desc_p3, 1, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            flag = 0;
            rtn = robot.FT_Guard(flag, sensor_id, select, ft, max_threshold, min_threshold);
            Console.WriteLine($"FT_Guard end rtn {rtn}");
        }

        private void btnFTConttol_Click(object sender, EventArgs e)
        {
            byte flag = 1;
            byte sensor_id = 1;
            int[] select = new int[6]{ 0, 0, 1, 0, 0, 0 };
            double[] ft_pid = new double[6]{ 0.0005f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            byte adj_sign = 0;
            byte ILC_sign = 0;
            float max_dis = 100.0f;
            float max_ang = 0.0f;

            ForceTorque ft = new ForceTorque(0, 0, 0, 0 ,0 ,0);
            DescPose desc_p1, desc_p2, offset_pos;
            JointPos j1, j2;
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            desc_p1 = new DescPose(0, 0, 0, 0, 0, 0);
            desc_p2 = new DescPose(0, 0, 0, 0, 0, 0);
            offset_pos = new DescPose(0, 0, 0, 0, 0, 0);

            j2 = new JointPos(0, 0, 0, 0, 0, 0);
            j1 = new JointPos(0, 0, 0, 0, 0, 0);

            desc_p1.tran.x = 1.299;
            desc_p1.tran.y = -719.159;
            desc_p1.tran.z = 141.314;
            desc_p1.rpy.rx = 177.999;
            desc_p1.rpy.ry = -0.715;
            desc_p1.rpy.rz = -161.937;

            desc_p2.tran.x = 245.047;
            desc_p2.tran.y = -675.509;
            desc_p2.tran.z = 139.538;
            desc_p2.rpy.rx = 177.987;
            desc_p2.rpy.ry = -0.129;
            desc_p2.rpy.rz = -142.238;
            ft.fz = -10.0;

            robot.GetInverseKin(0, desc_p1, -1, ref j1);
            robot.GetInverseKin(0, desc_p2, -1, ref j2);

            robot.MoveJ(j1, desc_p1, 1, 0, 100.0f, 180.0f, 100.0f, epos, -1.0f, 0, offset_pos);
          //  int rtn = robot.FT_Control(flag, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
           // Console.WriteLine($"FT_Control start rtn {rtn}");

            robot.MoveL(j2, desc_p2, 1, 0, 100.0f, 180.0f, 20.0f, -1.0f, 0, epos, 0, 0, offset_pos);
            flag = 0;
        //  rtn = robot.FT_Control(flag, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
          //  Console.WriteLine($"FT_Control end rtn {rtn}");
        }

        private void btnComplience_Click(object sender, EventArgs e)
        {
    byte flag = 1;
    int sensor_id = 1;
    int[] select = new int[6]{ 1, 1, 1, 0, 0, 0 };
    double[] ft_pid = new double[6] { 0.0005f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    byte adj_sign = 0;
    byte ILC_sign = 0;
    float max_dis = 100.0f;
    float max_ang = 0.0f;

    ForceTorque ft = new ForceTorque(0, 0, 0, 0, 0, 0);
    DescPose desc_p1, desc_p2, offset_pos;
    JointPos j1, j2;

    ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
    desc_p1 = new DescPose(0, 0, 0, 0, 0, 0);
    desc_p2 = new DescPose(0, 0, 0, 0, 0, 0);
    offset_pos = new DescPose(0, 0, 0, 0, 0, 0);

    j2 = new JointPos(0, 0, 0, 0, 0, 0);
    j1 = new JointPos(0, 0, 0, 0, 0, 0);

    desc_p1.tran.x = 1.299;
    desc_p1.tran.y = -719.159;
    desc_p1.tran.z = 141.314;
    desc_p1.rpy.rx = 177.999;
    desc_p1.rpy.ry = -0.715;
    desc_p1.rpy.rz = -161.937;

    desc_p2.tran.x = 245.047;
    desc_p2.tran.y = -675.509;
    desc_p2.tran.z = 139.538;
    desc_p2.rpy.rx = 177.987;
    desc_p2.rpy.ry = -0.129;
    desc_p2.rpy.rz = -142.238;
    ft.fz = -10.0;

    robot.GetInverseKin(0, desc_p1, -1, ref j1);
    robot.GetInverseKin(0, desc_p2, -1, ref j2);

    ft.fx = -10.0;
    ft.fy = -10.0;
    ft.fz = -10.0;
  //  robot.FT_Control(flag, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
    float p = 0.00005f;
    float force = 30.0f;
    int rtn = robot.FT_ComplianceStart(p, force);
    Console.WriteLine($"FT_ComplianceStart rtn {rtn}");
    int count = 15;
    while (count > 0)
    {
        robot.MoveL(j1, desc_p1, 1, 0, 100.0f, 180.0f, 100.0f, -1.0f, 0, epos, 0, 1, offset_pos);
        robot.MoveL(j2, desc_p2, 1, 0, 100.0f, 180.0f, 100.0f, -1.0f, 0, epos, 0, 0, offset_pos);
        count -= 1;
    }
    rtn = robot.FT_ComplianceStop();
    Console.WriteLine($"FT_ComplianceStop rtn {rtn}");
    flag = 0;
    //robot.FT_Control(flag, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
        }

        private void btnServoStart_Click(object sender, EventArgs e)
        {
            robot.ServoMoveStart();
        }

        private void btnServoEnd_Click(object sender, EventArgs e)
        {
            robot.ServoMoveEnd();
        }

        private void btnNewSpline_Click(object sender, EventArgs e)
        {
            JointPos j1 = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            JointPos j2 = new JointPos(-45.615, -106.172, 124.296, -107.151, -91.282, 74.255);
            JointPos j3 = new JointPos(-61.954, -84.409, 108.153, -116.316, -91.283, 74.260);
            JointPos j4 = new JointPos(-89.575, -80.276, 102.713, -116.302, -91.284, 74.267);
            JointPos j5 = new JointPos(-95.228, -54.621, 73.691, -112.245, -91.280, 74.268);
            DescPose desc_pos1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose desc_pos2 = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);
            DescPose desc_pos3 = new DescPose(-327.622, 402.230, 320.402, -178.067, 2.127, -46.207);
            DescPose desc_pos4 = new DescPose(-104.066, 544.321, 327.023, -177.715, 3.371, -73.818);
            DescPose desc_pos5 = new DescPose(-33.421, 732.572, 275.103, -177.907, 2.709, -79.482);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);

            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 100.0f;
            float blendT = -1.0f;
            byte flag = 0;

            robot.SetSpeed(20);

            int err = -1;
            err = robot.MoveJ(j1, desc_pos1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            Console.WriteLine($"movej errcode:  {err}");

            robot.NewSplineStart(1, 2000);
            robot.NewSplinePoint(j1, desc_pos1, tool, user, vel, acc, ovl, -1, 0);
            robot.NewSplinePoint(j2, desc_pos2, tool, user, vel, acc, ovl, -1, 0);
            robot.NewSplinePoint(j3, desc_pos3, tool, user, vel, acc, ovl, -1, 0);
            robot.NewSplinePoint(j4, desc_pos4, tool, user, vel, acc, ovl, -1, 0);
            robot.NewSplinePoint(j5, desc_pos5, tool, user, vel, acc, ovl, -1, 0);
            robot.NewSplineEnd();
        }

        private void btnMotionPause_Click(object sender, EventArgs e)
        {
            int rtn;
            JointPos j1 = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            JointPos j5 = new JointPos(-95.228, -54.621, 73.691, -112.245, -91.280, 74.268);
            DescPose desc_pos1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose desc_pos5 = new DescPose(-33.421, 732.572, 275.103, -177.907, 2.709, -79.482);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);

            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 100.0f;
            float blendT = -1.0f;
            byte flag = 0;

            robot.SetSpeed(20);

            rtn = robot.MoveJ(j1, desc_pos1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            rtn = robot.MoveJ(j5, desc_pos5, tool, user, vel, acc, ovl, epos, 1, flag, offset_pos);
            Thread.Sleep(1000);
            robot.PauseMotion();

            Thread.Sleep(1000);
            robot.ResumeMotion();

            Thread.Sleep(1000);
            robot.StopMotion();

            Thread.Sleep(1000);

        }

        private void btnStopMove_Click(object sender, EventArgs e)
        {
            robot.StopMotion();
        }

        private void btnGetDO_Click(object sender, EventArgs e)
        {
            byte st = 0;
            int rtn = robot.GetAxlePointRecordBtnState(ref st);
            Console.WriteLine($"st is {st}  rtn =  {rtn}");

            robot.SetDO(1, 1, 0, 1);
            Thread.Sleep(1000);
            int cO = 0;
            int dO = 0;
            rtn = robot.GetDO(ref cO, ref dO);
            Console.WriteLine($"GetDO result {rtn}  DO {dO}");

            robot.SetDO(1, 0, 0, 1);
            Thread.Sleep(1000);
            robot.GetDO(ref cO, ref dO);
            Console.WriteLine($"GetDO result {rtn}  DO {dO}");

            byte toolDO = 255;
            robot.SetToolDO(1, 1, 0, 1);
            Thread.Sleep(1000);
            robot.GetToolDO(ref toolDO);
            Console.WriteLine($"GetToolDO result {rtn}  DO {toolDO}");

            robot.SetToolDO(1, 0, 0, 1);
            Thread.Sleep(1000);
            robot.GetToolDO(ref toolDO);
            Console.WriteLine($"GetToolDO result {rtn}  DO {toolDO}");
        }

        private void btnSix1_Click(object sender, EventArgs e)
        {
            int rtn = robot.SetToolPoint(1);
            Console.WriteLine($"SetToolPoint result {rtn}");
        }

        private void btnSix2_Click(object sender, EventArgs e)
        {
            robot.SetToolPoint(2);
        }

        private void btnSix3_Click(object sender, EventArgs e)
        {
            robot.SetToolPoint(3);
        }

        private void btnSix4_Click(object sender, EventArgs e)
        {
            robot.SetToolPoint(4);
        }

        private void btnSix5_Click(object sender, EventArgs e)
        {
            robot.SetToolPoint(5);
        }

        private void btnSix6_Click(object sender, EventArgs e)
        {
            DescPose dese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.SetToolPoint(6);
            Thread.Sleep(1000);
            int rtn = robot.ComputeTool(ref dese);
            Console.WriteLine($"ComputeTool : x  {dese.tran.x}  y  {dese.tran.y}  z   {dese.tran.z}   rtn  {rtn}");
        }

        private void btnFour1_Click(object sender, EventArgs e)
        {
            robot.SetTcp4RefPoint(1);
        }

        private void btnFour2_Click(object sender, EventArgs e)
        {
            robot.SetTcp4RefPoint(2);
        }

        private void btnFour3_Click(object sender, EventArgs e)
        {
            robot.SetTcp4RefPoint(3);
        }

        private void btnFour4_Click(object sender, EventArgs e)
        {
            DescPose dese = new DescPose(0, 0, 0, 0, 0, 0);
            robot.SetTcp4RefPoint(4);
            Thread.Sleep(1000);
            robot.ComputeTcp4(ref dese);
            Console.WriteLine($"ComputeTcp4 : x  {dese.tran.x}  y  {dese.tran.y}  z   {dese.tran.z}");
        }

        private void btnGetSpeeds_Click(object sender, EventArgs e)
        {
            double[] speeds = new double[6];
            double tcpSpeed = 0;
            double oriSpeed = 0;
            robot.GetActualJointSpeedsDegree(1, ref speeds);
            Console.WriteLine($"GetActualJointSpeedsDegree:{speeds[0]}, {speeds[1]}, {speeds[2]}, {speeds[3]}, {speeds[4]}, {speeds[5]}");
            robot.GetTargetTCPCompositeSpeed(1, ref tcpSpeed, ref oriSpeed);
            Console.WriteLine($"GetTargetTCPCompositeSpeed:{tcpSpeed}, {oriSpeed}");
            robot.GetActualTCPCompositeSpeed(1, ref tcpSpeed, ref oriSpeed);
            Console.WriteLine($"GetActualTCPCompositeSpeed:{tcpSpeed}, {oriSpeed}");
            robot.GetTargetTCPSpeed(1, ref speeds);
            Console.WriteLine($"GetTargetTCPSpeed:{speeds[0]}, {speeds[1]}, {speeds[2]}, {speeds[3]}, {speeds[4]}, {speeds[5]}");
            robot.GetActualTCPSpeed(1, ref speeds);
            Console.WriteLine($"GetActualTCPSpeed:{speeds[0]}, {speeds[1]}, {speeds[2]}, {speeds[3]}, {speeds[4]}, {speeds[5]}");
            speeds[0] = 0;
            speeds[1] = 0;
            robot.GetActualJointAccDegree(1, ref speeds);
            Console.WriteLine($"GetActualJointAccDegree:{speeds[0]}, {speeds[1]}, {speeds[2]}, {speeds[3]}, {speeds[4]}, {speeds[5]}");
        }

        private void btnInverse_Click(object sender, EventArgs e)
        {
            JointPos j1,j2;
            DescPose desc_pos1, desc_pos2, offect;
            
            desc_pos1 = new DescPose(-158.923, 386.866, 275.684, 179.175, 2.259, 47.955);
            desc_pos2 = new DescPose(0, 0, 0, 0, 0, 0);
            j2 = new JointPos(0, 0, 0, 0, 0, 0);


            j1 = new JointPos(134.076, -87.009, -150.729, -33.215, 92.199, -3.913);
            offect = new DescPose(0, 0, 0, 30, 0, 0);
            robot.GetInverseKin(1, offect, -1, ref j2);
            Console.WriteLine($"GetInverseKin:{j2.jPos[0]},{j2.jPos[1]},{j2.jPos[2]},{j2.jPos[3]},{j2.jPos[4]},{j2.jPos[5]}");

            j2.jPos[0] = 0;
            j2.jPos[1] = 0;
            robot.GetInverseKinRef(1, offect, j1, ref j2);
            Console.WriteLine($"GetInverseKinRef:{j2.jPos[0]},{j2.jPos[1]},{j2.jPos[2]},{j2.jPos[3]},{j2.jPos[4]},{j2.jPos[5]}");

            bool hasSolution = false;
            robot.GetInverseKinHasSolution(1, offect, j1, ref hasSolution);
            Console.WriteLine($"GetInverseKinHasSolution  {hasSolution}");
            
            //robot.GetForwardKin(j1, ref desc_pos2);
           // Console.WriteLine($"GetForwardKin : {desc_pos2.tran.x},{desc_pos2.tran.y},{desc_pos2.tran.z},{desc_pos2.rpy.rx},{desc_pos2.rpy.ry},{desc_pos2.rpy.rz}");
        }

        private void btnRobotState2_Click(object sender, EventArgs e)
        {
            byte robotMotionState = 255;
            robot.GetRobotMotionDone(ref robotMotionState);
            Console.WriteLine($"robotMotionState  {robotMotionState}");

            int mainErrCode = -1;
            int subErrCode = -1;
            robot.GetRobotErrorCode(ref mainErrCode, ref subErrCode);
            Console.WriteLine($"mainErrCode  {mainErrCode}  subErrCode  {subErrCode} ");


            string name = "P1";
            double[] point = new double[20] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            robot.GetRobotTeachingPoint(name, ref point);
            Console.WriteLine($"GetRobotTeachingPoint:{point[0]},{point[1]},{point[2]},{point[3]},{point[4]},{point[5]},{point[6]},{point[7]},{point[8]},{point[9]},{point[10]},{point[11]},{point[12]},{point[13]},{point[14]},{point[15]},{point[16]},{point[17]},{point[18]},{point[19]}");

            int length = -1;
            robot.GetMotionQueueLength(ref length);
            Console.WriteLine($"GetMotionQueueLength  {length}");
        }

        private void btnLoadDefaultLua_Click(object sender, EventArgs e)
        {
            int rtn = robot.LoadDefaultProgConfig(1, "/fruser/Text1.lua");
            if (rtn != 0) 
            {
                Console.WriteLine($"LoadDefaultProgConfig  fail");
            }
            else
            {
                Console.WriteLine($"LoadDefaultProgConfig  success");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnOutTool1_Click(object sender, EventArgs e)
        {
            robot.SetExTCPPoint(1);
            robot.SetWObjCoordPoint(1);
        }

        private void btnOutTool2_Click(object sender, EventArgs e)
        {
            robot.SetExTCPPoint(2);
            robot.SetWObjCoordPoint(2);
        }

        private void btnOutTool3_Click(object sender, EventArgs e)
        {
            int rtn = robot.SetExTCPPoint(3);
            Console.WriteLine($"SetExTCPPoint rtn :{rtn}");
            robot.SetWObjCoordPoint(3);
            DescPose desc_pos1 = new DescPose(0, 0, 0, 0, 0, 0);
        
            rtn = robot.ComputeWObjCoord(0,0, ref desc_pos1);
            Console.WriteLine($"ComputeWObjCoord rtn :{rtn}");
            Console.WriteLine($"ComputeWObjCoord:{desc_pos1.tran.x},{desc_pos1.tran.y},{desc_pos1.tran.z},{desc_pos1.rpy.rx},{desc_pos1.rpy.ry},{desc_pos1.rpy.rz}");

            Thread.Sleep(1000);
            robot.ComputeExTCF(ref desc_pos1);
            Console.WriteLine($"ComputeExTCF:{desc_pos1.tran.x},{desc_pos1.tran.y},{desc_pos1.tran.z},{desc_pos1.rpy.rx},{desc_pos1.rpy.ry},{desc_pos1.rpy.rz}");

        }

        private void btnDeleteTPD_Click(object sender, EventArgs e)
        {
            robot.SetTPDDelete("tpd2023");
        }

        private void btnDealTPD_Click(object sender, EventArgs e)
        {
            string name = "tpd2023";
            int rtn = -1;
            rtn = robot.LoadTrajectoryJ(name, 100, 1);
            Console.WriteLine($"LoadTrajectoryJ:{rtn}");
            rtn = robot.MoveTrajectoryJ();
            Console.WriteLine($"MoveTrajectoryJ:{rtn}");
        }

        private void btnTestOthers_Click(object sender, EventArgs e)
        {
            robot.SetRobotRealtimeStateSamplePeriod(50);
            int getPeriod = 0;
            robot.GetRobotRealtimeStateSamplePeriod(ref getPeriod);
            Console.WriteLine($"get robot realtin period is {getPeriod}");


            //int rtn = -1;
            //double[] dhCompensation = new double[6]{ 3,3,3,3,3,3};
            //rtn = robot.GetDHCompensation(ref dhCompensation);
            //Console.WriteLine($"GetDHCompensation:  rtn :{rtn}    {dhCompensation[0]}  {dhCompensation[1]}  {dhCompensation[2]}  {dhCompensation[3]}  {dhCompensation[4]}  {dhCompensation[5]}");
            //string ssh = "";
            //rtn = robot.GetSSHKeygen(ref ssh);
            //Console.WriteLine($"GetSSHKeygen:  ssh {ssh}  rtn  {rtn}");
            //string file_path = "/root/web/file/user/4.lua";
            //string md5 = "";
            //robot.ComputeFileMD5(file_path, ref md5);
            //Console.WriteLine($"the md5 is {md5}");

            //byte state = 255;
            //rtn = robot.GetRobotEmergencyStopState(ref state);
            //Console.WriteLine($"GetRobotEmergencyStopState:  rtn  {rtn}   state {state}");

            //int comState = -1;
            //rtn = robot.GetSDKComState(ref comState);
            //Console.WriteLine($"GetSDKComState:  rtn  {rtn}   state  {comState}");

            //byte si0_state = 255;
            //byte si1_state = 255;

            //rtn = robot.GetSafetyStopState(ref si0_state, ref si1_state);
            //Console.WriteLine($"GetSafetyStopState:  rtn  {rtn}   si0_state  {si0_state}   si1_state  {si1_state}");
        }

        private void btnStable_Click(object sender, EventArgs e)
        {
            DescPose desc_pos1, desc_pos2, desc_pos3;

            desc_pos1 = new DescPose(-356.088, -245.376, 419.719, 149.443, -45.358, -141.077);
            desc_pos2 = new DescPose(-347.749, 95.602, 377.933, 146.808, -44.266, -130.226);
            desc_pos3 = new DescPose(-342.339, -126.859, 242.898, 167.069, -19.688, -111.407);

            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 100.0f;
            float blendT = -1.0f;
            float blendT1 = 0.0f;
            int config = -1;
            double[] speeds = new double[6];
            robot.SetSpeed(3);
            while (true) 
            { 
                int rtn = robot.MoveCart(desc_pos1, tool, user, vel, acc, ovl, blendT, config);
                Console.WriteLine($"movechart rtn {rtn}");
                robot.MoveCart(desc_pos2, tool, user, vel, acc, ovl, blendT, config);
                robot.MoveCart(desc_pos3, tool, user, vel, acc, ovl, blendT1, config);
                int comerr = 0;
                robot.GetSDKComState(ref comerr);
                if(comerr == 1)
                {
                    Console.WriteLine("comerr--------------------");
                    return;
                }

                robot.GetActualJointAccDegree(1, ref speeds);
                Console.WriteLine($"GetActualJointAccDegree:{speeds[0]}, {speeds[1]}, {speeds[2]}, {speeds[3]}, {speeds[4]}, {speeds[5]}");
                Thread.Sleep(1000);
            }
            
        }

        private void btnComputePick_Click(object sender, EventArgs e)
        {
            DescPose desc_pos1, desc_pos2;
            desc_pos1 = new DescPose(-437.039, 411.064, 426.189, -177.886, 2.007, 31.155);
            desc_pos2 = new DescPose(0, 0, 0, 0, 0, 0);
            robot.ComputePrePick(desc_pos1, 10, 0, ref desc_pos2);
            Console.WriteLine($"ComputePrePick:{desc_pos2.tran.x},{desc_pos2.tran.y},{desc_pos2.tran.z},{desc_pos2.rpy.rx},{desc_pos2.rpy.ry},{desc_pos2.rpy.rz}");

            desc_pos2.tran.x = 0;
            robot.ComputePostPick(desc_pos1, 10, 0, ref desc_pos2);
            Console.WriteLine($"ComputePostPick:{desc_pos2.tran.x},{desc_pos2.tran.y},{desc_pos2.tran.z},{desc_pos2.rpy.rx},{desc_pos2.rpy.ry},{desc_pos2.rpy.rz}");
        }

        private void btnTrajectory_Click(object sender, EventArgs e)
        {
            string name = "/fruser/traj/trajHelix_aima_1.txt";
            int rtn = -1;

            rtn = robot.LoadTrajectoryJ(name, 100, 1);
            Console.WriteLine($"LoadTrajectoryJ:{rtn}");

            DescPose desc_pos2 = new DescPose(0, 0, 0, 0, 0, 0);
            rtn = robot.GetTrajectoryStartPose(name, ref desc_pos2);
            Console.WriteLine($"GetTrajectoryStartPose:{desc_pos2.tran.x},{desc_pos2.tran.y},{desc_pos2.tran.z},{desc_pos2.rpy.rx},{desc_pos2.rpy.ry},{desc_pos2.rpy.rz}");

            int tool = 1;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 100.0f;
            float blendT = -1.0f;
            int config = -1;
            robot.MoveCart(desc_pos2, tool, user, vel, acc, ovl, blendT, config);

            rtn = robot.SetTrajectoryJSpeed(20);
            Console.WriteLine($"SetTrajectoryJSpeed: rtn  {rtn}");

            rtn = robot.MoveTrajectoryJ();
            Console.WriteLine($"MoveTrajectoryJ:{rtn}");

            int pnum = -1;
            rtn = robot.GetTrajectoryPointNum(ref pnum);
            Console.WriteLine($"GetTrajectoryPointNum: rtn  {rtn}    num {pnum}");

            rtn = robot.SetTrajectoryJSpeed(100);
            Console.WriteLine($"SetTrajectoryJSpeed: rtn  {rtn}");

            ForceTorque ft = new ForceTorque(1, 1, 1, 1, 1, 1);
            rtn = robot.SetTrajectoryJForceTorque(ft);
            Console.WriteLine($"SetTrajectoryJForceTorque: rtn  {rtn}");

            rtn = robot.SetTrajectoryJForceFx(1.0);
            Console.WriteLine($"SetTrajectoryJForceFx: rtn  {rtn}");
            rtn = robot.SetTrajectoryJForceFy(1.0);
            Console.WriteLine($"SetTrajectoryJForceFx: rtn  {rtn}");
            rtn = robot.SetTrajectoryJForceFz(1.0);
            Console.WriteLine($"SetTrajectoryJForceFx: rtn  {rtn}");
            rtn = robot.SetTrajectoryJTorqueTx(1.0);
            Console.WriteLine($"SetTrajectoryJForceFx: rtn  {rtn}");
            rtn = robot.SetTrajectoryJTorqueTy(1.0);
            Console.WriteLine($"SetTrajectoryJForceFx: rtn  {rtn}");
            rtn = robot.SetTrajectoryJTorqueTz(1.0);
            Console.WriteLine($"SetTrajectoryJForceFx: rtn  {rtn}");
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            //传送带跟踪

            DescPose pos1 = new DescPose(-351.549, 87.914, 354.176, -179.679, -0.134, 2.468);
            DescPose pos2 = new DescPose(-351.203, -213.393, 351.054, -179.932, -0.508, 2.472);


            double[] cmp = { 0.0, 0.0, 0.0 };
            int rtn = robot.ConveyorCatchPointComp(cmp);//设置传动带抓取点补偿
            if (rtn != 0)
            {
                return;
            }
            Console.WriteLine("ConveyorCatchPointComp: rtn  " + rtn);

            rtn = robot.MoveCart(pos1, 1, 0, (float)30.0, (float)180.0, (float)100.0, (float)-1.0, -1);
            Console.WriteLine("MoveCart: rtn  " + rtn);

            rtn = robot.ConveyorIODetect(10000);//传送带工件IO检测
            Console.WriteLine("ConveyorIODetect: rtn   " + rtn);

            robot.ConveyorGetTrackData(1);//配置传送带跟踪抓取
            rtn = robot.ConveyorTrackStart(1);//跟踪开始
            Console.WriteLine("ConveyorTrackStart: rtn  " + rtn);

            //rtn = robot.ConveyorTrackMoveL("cvrCatchPoint", 1, 0, (float)100.0, (float)0.0, (float)100.0, (float)-1.0);
            Console.WriteLine("ConveyorTrackMoveL: rtn  " + rtn);

            rtn = robot.MoveGripper(1, 60, 60, 30, 30000, 0, 0, 0, 50, 50);
            Console.WriteLine("ConveyorTrackMoveL: rtn  " + rtn);
         

            //rtn = robot.ConveyorTrackMoveL("cvrRaisePoint", 1, 0, (float)100.0, (float)0.0, (float)100.0, (float)-1.0);
            Console.WriteLine("ConveyorTrackMoveL: rtn   " + rtn);

            rtn = robot.ConveyorTrackEnd();//传送带跟踪停止
            Console.WriteLine("ConveyorTrackEnd: rtn  " + rtn);

            rtn = robot.MoveCart(pos2, 1, 0, (float)30.0, (float)180.0, (float)100.0, (float)-1.0, -1);
            Console.WriteLine("MoveCart: rtn  " + rtn);

            rtn = robot.MoveGripper(1, 100, 60, 30, 30000, 0,0,0,50,50);
            Console.WriteLine("MoveGripper: rtn  " + rtn);
            //DescPose pos1 = new DescPose(-351.549, 87.914, 354.176, -179.679, -0.134, 2.468);
            //DescPose pos2 = new DescPose(-351.558, -247.286, 354.131, -179.679, -0.142, 2.474);


            //int rtn = -1;

            //double[] cmp = new double[3] { 0, 0, 0};
            //rtn = robot.ConveyorCatchPointComp(cmp);
            //if(rtn != 0)
            //{
            //    return;
            //}
            //Console.WriteLine($"ConveyorCatchPointComp: rtn  {rtn}");

            //rtn = robot.MoveCart(pos1, 1, 0, 30.0f, 100.0f, 100.0f, -1.0f, -1);
            //Console.WriteLine($"MoveCart: rtn  {rtn}");

            //rtn = robot.ConveyorIODetect(10000);
            //Console.WriteLine($"ConveyorIODetect: rtn  {rtn}");

            //robot.ConveyorGetTrackData(1);
            //rtn = robot.ConveyorTrackStart(1);
            //Console.WriteLine($"ConveyorTrackStart: rtn  {rtn}");

            //rtn = robot.ConveyorTrackMoveL("cvrCatchPoint", 1, 0, 30.0f, 0.0f, 100.0f, -1.0f);
            //Console.WriteLine($"ConveyorTrackMoveL: rtn  {rtn}");

            //rtn = robot.MoveGripper(1, 60, 60, 30, 30000, 0);
            //Console.WriteLine($"MoveGripper: rtn  {rtn}");

            //rtn = robot.ConveyorTrackMoveL("cvrRaisePoint", 1, 0, 30.0f, 0.0f, 100.0f, -1.0f);
            //Console.WriteLine($"ConveyorTrackMoveL: rtn  {rtn}");

            //rtn = robot.ConveyorTrackEnd();
            //Console.WriteLine($"ConveyorTrackEnd: rtn  {rtn}");

            //rtn = robot.MoveCart(pos2, 1, 0, 30.0f, 180.0f, 100.0f, -1.0f, -1);
            //Console.WriteLine($"MoveCart: rtn  {rtn}");

            //rtn = robot.MoveGripper(1, 100, 60, 30, 30000, 0);
            //Console.WriteLine($"MoveGripper: rtn  {rtn}");
        }

        private void btnIO_Click(object sender, EventArgs e)
        {
            int rtn = -1;
            rtn = robot.ConveyorPointIORecord();
            Console.WriteLine($"ConveyorPointIORecord: rtn  {rtn}");
        }

        private void btnStarta_Click(object sender, EventArgs e)
        {
            int rtn = -1;
            rtn = robot.ConveyorPointARecord();
            Console.WriteLine($"ConveyorPointARecord: rtn  {rtn}");
        }

        private void btnREF_Click(object sender, EventArgs e)
        {
            int rtn = -1;
            rtn = robot.ConveyorRefPointRecord();
            Console.WriteLine($"ConveyorRefPointRecord: rtn  {rtn}");
        }

        private void btnEndB_Click(object sender, EventArgs e)
        {
            int rtn = -1;
            rtn = robot.ConveyorPointBRecord();
            Console.WriteLine($"ConveyorPointBRecord: rtn  {rtn}");
        }

        private void btnSetConvey_Click(object sender, EventArgs e)
        {
            float[] param = new float[2];
            //int rtn = robot.ConveyorSetParam(1, 10000, 2.0, 1, 1, 20);
            //Console.WriteLine($"ConveyorSetParam: rtn  {rtn}");
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            txtLog.Text = "NULL";
            int rtn = robot.PointTableDownLoad(textBox1.Text, "D://zDOWN/");
            txtLog.Text = rtn.ToString();
        }

        private void txtSavePath_TextChanged(object sender, EventArgs e)
        {

        }

        private static byte[] subBytes(byte[] src, int begin, int count)
        {
            byte[] bs = new byte[count];
            for (int i = begin; i < begin + count; i++)
            {
                bs[i - begin] = src[i];
            }
            return bs;
        }

        private static string getMD5ByMD5CryptoService(string path)
        {
            if (!File.Exists(path))
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

        public static int GetFileSize(string sFullName)
        {
            long lSize = 0;
            if (File.Exists(sFullName))
                lSize = new FileInfo(sFullName).Length;
            return (int)lSize;
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            string txtLog = "D://zUP/test_point_A.db";
            int rtn = robot.PointTableUpLoad(txtLog);
            Console.WriteLine($"PointTableUpLoad rtn :{rtn} ");
        }

        private void btnUpdateLua_Click(object sender, EventArgs e)
        {
        }

        private void btnSwitchPointTable_Click(object sender, EventArgs e)
        {
            txtLog.Text = "NULL";
            int rtn = 0;
            string errorStr = "";
            rtn = robot.PointTableUpdateLua(txtUpdatePointTableName.Text, txtUopdateLuaName.Text, ref errorStr);
            Console.WriteLine($"PointTableSwitch rtn :{errorStr}    rtn is {rtn}");
            txtLog.Text = errorStr;
        }

        private void btnSwitchAndUpdate_Click(object sender, EventArgs e)
        {

        }

        private void btnRunPointTable_Click(object sender, EventArgs e)
        {
            string program_name = "/fruser/" + txtUopdateLuaName.Text;
            int rtn = 0;
            robot.Mode(0);
            rtn = robot.ProgramLoad(program_name);
            Console.WriteLine($"ProgramLoad rtn  is {rtn}");
            rtn = robot.ProgramRun();
            Console.WriteLine($"ProgramRun rtn  is {rtn}");
            txtLog.Text = "program run success";
        }

        private void btnpointTableStable_Click(object sender, EventArgs e)
        {
            int rtn = 0;
            string errorStr = "";
            rtn = robot.PointTableUpLoad("D://zUP/point_table_wand687.db");
            rtn = robot.PointTableSwitch("point_table_wand687.db", ref errorStr);
            Console.WriteLine(errorStr);
            rtn = robot.LuaUpload("D://zUP/wand687.lua", ref errorStr);
            Console.WriteLine(errorStr);
            rtn = robot.PointTableUpdateLua("point_table_wand687.db", "wand687.lua", ref errorStr);
            Console.WriteLine(errorStr);
            robot.Mode(0);
            rtn = robot.ProgramLoad("/fruser/wand687.lua");
            rtn = robot.ProgramRun();
            return;


            int count = 0;
            byte done = 0;
            string program_name = "/fruser/" + txtUopdateLuaName.Text;
            while (true)
            {
                //int rtn = 0;
                //string errorStr = "";
                rtn = robot.PointTableUpdateLua("point_table_小无人机.db", txtUopdateLuaName.Text, ref errorStr);
                rtn = robot.ProgramLoad(program_name);
                rtn = robot.ProgramRun();
                count++;
                txtLog.Text = "正在进行循环切换点位表测试，测试次数： " + count;

                while(done == 0)
                {
                    robot.GetRobotMotionDone(ref done);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(8000);




                rtn = robot.PointTableUpdateLua("point_table_大无人机.db", txtUopdateLuaName.Text, ref errorStr);
                rtn = robot.ProgramLoad(program_name);
                rtn = robot.ProgramRun();
                count++;
                txtLog.Text = "正在进行循环切换点位表测试，测试次数： " + count;
                done = 1;
                Thread.Sleep(2000);

                while (done == 0)
                {
                    robot.GetRobotMotionDone(ref done);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(8000);


                rtn = robot.PointTableUpdateLua("", txtUopdateLuaName.Text, ref errorStr);
                rtn = robot.ProgramLoad(program_name);
                rtn = robot.ProgramRun();
                count++;
                txtLog.Text = "正在进行循环切换点位表测试，测试次数： " + count;
                done = 1;
                Thread.Sleep(2000);

                while (done == 0)
                {
                    robot.GetRobotMotionDone(ref done);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(8000);

                done = 1;
            
            }
        }

        private void btnGetVersions_Click(object sender, EventArgs e)
        {
            string[] ver = new string[20];
            int rtn = 0;
            rtn = robot.GetSoftwareVersion(ref ver[0], ref ver[1], ref ver[2]);
            rtn = robot.GetHardwareVersion(ref ver[3], ref ver[4], ref ver[5], ref ver[6], ref ver[7], ref ver[8], ref ver[9], ref ver[10]);
            rtn = robot.GetFirmwareVersion(ref ver[11], ref ver[12], ref ver[13], ref ver[14], ref ver[15], ref ver[16], ref ver[17], ref ver[18]);
            Console.WriteLine($"robotmodel  is: {ver[0]}");
            Console.WriteLine($"webVersion  is: {ver[1]}");
            Console.WriteLine($"controllerVersion  is: {ver[2]}");
            Console.WriteLine($"Hard ctrlBox Version  is: {ver[3]}");
            Console.WriteLine($"Hard driver1 Version  is: {ver[4]}");
            Console.WriteLine($"Hard driver2 Version  is: {ver[5]}");
            Console.WriteLine($"Hard driver3 Version  is: {ver[6]}");
            Console.WriteLine($"Hard driver4 Version  is: {ver[7]}");
            Console.WriteLine($"Hard driver5 Version  is: {ver[8]}");
            Console.WriteLine($"Hard driver6 Version  is: {ver[9]}");
            Console.WriteLine($"Hard end Version  is: {ver[10]}");
            Console.WriteLine($"Firm ctrlBox Version  is: {ver[11]}");
            Console.WriteLine($"Firm driver1 Version  is: {ver[12]}");
            Console.WriteLine($"Firm driver2 Version  is: {ver[13]}");
            Console.WriteLine($"Firm driver3 Version  is: {ver[14]}");
            Console.WriteLine($"Firm driver4 Version  is: {ver[15]}");
            Console.WriteLine($"Firm driver5 Version  is: {ver[16]}");
            Console.WriteLine($"Firm driver6 Version  is: {ver[17]}");
            Console.WriteLine($"Firm end Version  is: {ver[18]}");

        }

        private void btnWeldTest_Click(object sender, EventArgs e)
        {
            FrmWeld frmWeld = new FrmWeld(robot);
            frmWeld.ShowDialog();
        }

        private void btnTestLog_Click(object sender, EventArgs e)
        {

            while (true)
            {
                robot.SetLoggerLevel(FrLogLevel.INFO);
                string version = "";
                robot.GetSDKVersion(ref version);
                byte flag = 0;
                double[] acc = new double[6];
                robot.GetActualJointAccDegree(flag, ref acc);

                JointPos pos = new JointPos(0, 0, 0, 0, 0, 0);
                robot.GetActualJointPosDegree(flag, ref pos);

                DescPose pose = new DescPose();
                robot.GetActualTCPPose(flag, ref pose);

                int line = 0;
                robot.GetCurrentLine(ref line);
                //robot.TestLog();
                Thread.Sleep(1);
            }
            
        }

        private void btnsetsysvalue_Click(object sender, EventArgs e)
        {
            int rtn = robot.SetSysVarValue(1, 0);
            Console.WriteLine(rtn);
        }

        private void btnUploadLua_Click(object sender, EventArgs e)
        {
            string errstr = "";
            robot.LuaUpload(txtLuaPath.Text, ref errstr);
            Console.WriteLine(errstr);

        }

        private void btnDownLoadLua_Click(object sender, EventArgs e)
        {

            int rtn = robot.LuaDownLoad("airlab.lua", "D://zDOWN/");
            Console.WriteLine(rtn);
        }

        private void btnDeleteLua_Click(object sender, EventArgs e)
        {
            robot.LuaDelete(txtDownLoadLuaName.Text);
        }

        private void btnGetLua_Click(object sender, EventArgs e)
        {
            List<string> lualist = new List<string>();
            robot.GetLuaList(ref lualist);
            int n = lualist.Count;
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine(lualist[i]);
            }
        }

        private void btnAuxServo_Click(object sender, EventArgs e)
        {
            Frm485Servo frm485Servo = new Frm485Servo(robot);
            frm485Servo.ShowDialog();
        }

        private void btnTestUDP_Click(object sender, EventArgs e)
        {
            FrmUDP frmUDP = new FrmUDP(robot);
            frmUDP.ShowDialog();
        }

        private void txtUopdateLuaName_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnMultyRobot_Click(object sender, EventArgs e)
        {
            Robot robot1 = new Robot();
            robot1.RPC("192.168.58.3");
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
            robot1.GetRobotRealTimeState(ref pkg);
            robot1.StartJOG(0, 1, 1, 10, 10, 50);
            Console.WriteLine("robot1 pos is " + pkg.jt_cur_pos[1]);
            robot.GetRobotRealTimeState(ref pkg);
            Console.WriteLine("robot pos is " + pkg.jt_cur_pos[1]);
            robot.StartJOG(0, 1, 0, 10, 10, 50);
        }

        private void btnLinOverVel_Click(object sender, EventArgs e)
        {
            DescPose startdescPose = new DescPose(370.948, 272.439, 283.719, -174.149, 2.018, -10.04);
            JointPos startjointPos = new JointPos(46.785, -83.236, -108.979, -73.979, 94.879, -32.909);
            JointPos startJP = startjointPos;
            robot.GetInverseKinRef(2, startdescPose, startjointPos, ref startJP);
            Console.WriteLine(startJP.jPos[0].ToString(), startJP.jPos[1].ToString());

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            DescPose enddescPose = new DescPose(55.555, -387.599, 228.309, -179.009, -2.179, -7.964);
            JointPos endjointPos = new JointPos(-67.523, -75.644, -122.689, -69.707, 91.378, -149.517);
            

            //robot.MoveL(startjointPos, startdescPose, 1, 1, 100, 100, 100, 0, exaxisPos, 0, 0, offdese);
            while(true)
            {
                robot.MoveL(startJP, startdescPose, 1, 0, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese, 1, 1);
                robot.GetForwardKin(endjointPos, ref enddescPose);

                robot.MoveL(endjointPos, enddescPose, 1, 0, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese, 1, 1);
            }
           
        }

        private void btnFTtest_Click(object sender, EventArgs e)
        {
            FrmFT frmFT = new FrmFT(robot);
            frmFT.ShowDialog();
        }

        private void btnUpgrade_Click(object sender, EventArgs e)
        {
            Console.WriteLine("robot software upgrade start");
            robot.SoftwareUpgrade("D://zUP/software/software.tar.gz", false);
            while (true)
            {
                int state = 0;
                robot.GetSoftwareUpgradeState(ref state);
                Console.WriteLine($"robot Upgrade state is {state}");
                if (state == 100)
                {
                    break;
                }

                Thread.Sleep(500);
            }

            Console.WriteLine("robot software upgrade end");
        }

        private void btnGetSysTime_Click(object sender, EventArgs e)
        {
            while (true)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                robot.GetRobotRealTimeState(ref pkg);
                Console.WriteLine($"cur robot time is {pkg.robotTime.ToString()}");
                Thread.Sleep(50);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Test testInterface = new Test(robot);
            testInterface.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            JointPos j1 = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            JointPos j2 = new JointPos(-45.615, -106.172, 124.296, -107.151, -91.282, 74.255);
            DescPose desc_pos1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose desc_pos2 = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);

            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 100.0f;
            float blendT = 0.0f;
            float blendR = 0.0f;
            byte flag = 0;
            byte search = 0;

            robot.SetSpeed(5);

            robot.MoveAOStart(0,100,100,20);
            robot.MoveJ(j1, desc_pos1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            robot.MoveJ(j2, desc_pos2, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            robot.MoveAOStop();

            robot.MoveToolAOStart(0, 100, 100, 20);
            robot.MoveJ(j1, desc_pos1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            robot.MoveJ(j2, desc_pos2, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            robot.MoveToolAOStop();
        }

        private void btnGetcnde_Click(object sender, EventArgs e)
        {
            Thread.Sleep(2000);
            //RunCNDETest();
            //TestAddDeleteSpeedScale();
            //TestRobotRealtimeStates();
            //TestRobotOperationalStates();
            //TestExtendedAxisAndIOStates();
            //TestGripperAndForceSensorStates();
            //TestRobotERRStatusStates();
            //TestErrorCodeInterfaces();
            //TestNormalFeedbackAndPeriod();
            //TestInvalidStateConfig();
            //TestSquareMotionWithMoveL();
            // 循环获取并打印实时状态
            while (true)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                // 获取最新的机器人实时状态（内部会更新pkg对象）
                robot.GetRobotRealTimeState(ref pkg);

                Console.WriteLine($"robot SocketConnTimeout: {pkg.socketConnTimeout}");
                Console.WriteLine($"robot SocketReadTimeout: {pkg.socketReadTimeout}");
                // 如需打印 TsWebStateComErr 可取消注释
                 Console.WriteLine($"robot TsWebStateComErr: {pkg.tsWebStateComErr}");

                Thread.Sleep(300); // 与C++示例一致，300ms打印一次
            }
        }


        private async void TestSquareMotionWithMoveL()
        {
            int ret = 0;
            // 1. 初始化机器人
            //robot = new Robot();
            //robot.LoggerInit(FrLogType.BUFFER, FrLogLevel.INFO, "D://log/", 5, 5);
            //robot.SetLoggerLevel(FrLogLevel.INFO);
            //robot.SetReconnectParam(true, 100, 1000);

            //// 2. 配置状态反馈：工具位姿、运动完成信号，周期 8ms
            //List<RobotState> states = new List<RobotState>
            //{
            //    RobotState.JointCurPos,
            //    RobotState.ToolCurPos,
            //    RobotState.MotionDone
            //};
            //int periodMs = 8;
            //int ret = robot.SetRobotRealtimeStateConfig(states, periodMs);
            //Console.WriteLine($"配置状态结果: {ret}");

            //// 3. 建立 RPC 连接
            //ret = robot.RPC("192.168.58.3");
            //if (ret != 0)
            //{
            //    Console.WriteLine($"RPC 连接失败: {ret}");
            //    return;
            //}
            //Console.WriteLine("RPC 连接成功，CNDE 已连接。开始正方形运动...");

            // 运动参数
            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 100.0f;
            float blendR = -1.0f;   // 运动到位（阻塞），但我们将手动等待 MotionDone
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            int search = 0;
            int offset_flag = 0;
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            float oacc = 100.0f;
            int velAccParamMode = 0;
            int overSpeedStrategy = 0;
            int speedPercent = 10;


            byte flag = 0;
  
            int blendMode = 0;
            int velAccMode = 0;

            double step = 200.0;  // 边长 100mm
            long cycles = 10000000000;      // 循环次数
            robot.Sleep(2000);
            for (long cycle = 1; cycle <= cycles; cycle++)
            {
                Console.WriteLine($"\n========== 第 {cycle} 次正方形运动 ==========");

                // 获取当前 TCP 位姿作为起点
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                ret = robot.GetRobotRealTimeState(ref pkg);
                if (pkg == null)
                {
                    Console.WriteLine("获取当前位姿失败，退出");
                    break;
                }
                if (pkg.tl_cur_pos != null && pkg.tl_cur_pos.Length >= 6)
                {
                    Console.WriteLine($"  TCP位姿(mm/°): X={pkg.tl_cur_pos[0]:F2}, Y={pkg.tl_cur_pos[1]:F2}, Z={pkg.tl_cur_pos[2]:F2}, RX={pkg.tl_cur_pos[3]:F2}, RY={pkg.tl_cur_pos[4]:F2}, RZ={pkg.tl_cur_pos[5]:F2}");
                }
                double startX = pkg.tl_cur_pos[0];
                double startY = pkg.tl_cur_pos[1];
                double startZ = pkg.tl_cur_pos[2];
                double startRX = pkg.tl_cur_pos[3];
                double startRY = pkg.tl_cur_pos[4];
                double startRZ = pkg.tl_cur_pos[5];

                // 定义四个目标点（相对起点）
                DescPose target1 = new DescPose(startX + step, startY, startZ, startRX, startRY, startRZ);
                DescPose target2 = new DescPose(startX + step, startY + step, startZ, startRX, startRY, startRZ);
                DescPose target3 = new DescPose(startX, startY + step, startZ, startRX, startRY, startRZ);
                DescPose target4 = new DescPose(startX, startY, startZ, startRX, startRY, startRZ);
                //DescPose desc_pos2 = new DescPose(-321.222f, 185.189f, 335.520f, -179.030f, -1.284f, -29.869f);
                JointPos jpos1 = new JointPos(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
                JointPos jposRef = new JointPos(pkg.jt_cur_pos[0], pkg.jt_cur_pos[1], pkg.jt_cur_pos[2], pkg.jt_cur_pos[3], pkg.jt_cur_pos[4], pkg.jt_cur_pos[5]);
                robot.GetInverseKinRef(0, target1, jposRef, ref jpos1);

                // 执行四条边
                await MoveLAndWait(target1, tool, user, vel, acc, ovl, blendR, epos, search, offset_flag, offset_pos, oacc, velAccParamMode, overSpeedStrategy, speedPercent);
                robot.Sleep(1000);
                await MoveLAndWait(target2, tool, user, vel, acc, ovl, blendR, epos, search, offset_flag, offset_pos, oacc, velAccParamMode, overSpeedStrategy, speedPercent);
                robot.Sleep(1000);
                await MoveLAndWait(target3, tool, user, vel, acc, ovl, blendR, epos, search, offset_flag, offset_pos, oacc, velAccParamMode, overSpeedStrategy, speedPercent);
                robot.Sleep(1000);
                await MoveLAndWait(target4, tool, user, vel, acc, ovl, blendR, epos, search, offset_flag, offset_pos, oacc, velAccParamMode, overSpeedStrategy, speedPercent);
                robot.Sleep(1000);
                Console.WriteLine($"第 {cycle} 次正方形运动完成");
            }

            Console.WriteLine("所有运动完成...");
            //robot.CloseRPC();
        }

        // 辅助方法：执行 MoveL 并等待 MotionDone
        private async Task MoveLAndWait( DescPose target, int tool, int user, float vel, float acc, float ovl, float blendR,
                                        ExaxisPos epos, int search, int offset_flag, DescPose offset_pos,
                                        float oacc, int velAccParamMode, int overSpeedStrategy, int speedPercent)
        {
            // 注意：这里使用 MoveL 重载，参数顺序参考您的示例
            //int rtn = robot.MoveL(target, 0, 0, 100, 100, 100, -1, 0, epos, 0, 0, offset_pos, 0, 0);
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
            JointPos jpos = new JointPos(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            int ret = robot.GetRobotRealTimeState(ref pkg);
            JointPos jposRef = new JointPos(pkg.jt_cur_pos[0], pkg.jt_cur_pos[1], pkg.jt_cur_pos[2], pkg.jt_cur_pos[3], pkg.jt_cur_pos[4], pkg.jt_cur_pos[5]);
            Console.WriteLine($"     {pkg.jt_cur_pos[0]}");
            robot.GetInverseKinRef(0, target, jposRef, ref jpos);
            int rtn = robot.MoveL(jpos, target, 0, 0, 100, 100, 100, -1, 0, epos, 0, 0, offset_pos, 100.0f, 0, 0);

            if (rtn != 0)
            {
                Console.WriteLine($"MoveL 指令失败，错误码: {rtn}");
                return;
            }

            // 循环等待 MotionDone 变为 1
            int timeoutMs = 10000;      // 10 秒超时
            int intervalMs = 8;         // 检查间隔 8ms
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                byte done = 0;
                ret = robot.GetRobotMotionDone(ref done);
                if (ret != 0)
                {
                    Console.WriteLine($"GetRobotMotionDone 失败，错误码: {ret}");
                    return;
                }
                if (done == 1)
                {
                    Console.WriteLine($"运动到位，目标位置: X={target.tran.x:F1}, Y={target.tran.y:F1}, Z={target.tran.z:F1}");
                    return;
                }
                await Task.Delay(intervalMs);
            }
            Console.WriteLine($"等待运动完成超时 (>{timeoutMs}ms)");
        }




        private async void TestErrorCodeInterfaces()
        {

            // 2. 配置状态反馈周期为 2000ms
            List<RobotState> states1 = new List<RobotState> { RobotState.JointCurPos, RobotState.ToolCurPos };
            int ret = robot.SetRobotRealtimeStateConfig(states1, 2000);
            Console.WriteLine($"SetRobotRealtimeStateConfig(周期2000ms) 返回: {ret}(预期 4)");

            // 3. 配置状态反馈周期为 7ms（允许范围 8~1000，应返回 ERR_PARAM_VALUE = 4）
            ret = robot.SetRobotRealtimeStateConfig(states1, 7);
            Console.WriteLine($"SetRobotRealtimeStateConfig(周期7ms) 返回: {ret} (预期 4)");

            // 4. 配置空字段组，周期 8ms
            List<RobotState> emptyStates = new List<RobotState>();
            ret = robot.SetRobotRealtimeStateConfig(emptyStates, 8);
            Console.WriteLine($"SetRobotRealtimeStateConfig(空字段,周期8ms) 返回: {ret} (预期  ERR_NEED_AT_LEAST_ONE_STATE -19)");

            // 5. 添加已存在的状态 JointCurPos
            ret = robot.AddRobotRealtimeState(RobotState.JointCurPos);
            Console.WriteLine($"AddRobotRealtimeState(已存在JointCurPos) 返回: {ret} (预期  ERR_STATE_ALREADY_EXISTS -17)");

            // 6. 删除不存在的状态 CtrlBoxError
            ret = robot.DeleteRobotRealtimeState(RobotState.CtrlBoxError);
            Console.WriteLine($"DeleteRobotRealtimeState(不存在的CtrlBoxError) 返回: {ret} (预期  ERR_STATE_INVALID -18)");

            // 7. 设置周期为 7ms（无效）
            ret = robot.SetRobotRealtimeStatePeriod(7);
            Console.WriteLine($"SetRobotRealtimeStatePeriod(7ms) 返回: {ret} (预期 4)");
            // 设置周期为 1001ms（无效）
            ret = robot.SetRobotRealtimeStatePeriod(1001);
            Console.WriteLine($"SetRobotRealtimeStatePeriod(1001ms) 返回: {ret} (预期 4)");

            // 8. 设置有效周期 10ms，并建立连接验证（可选）
            ret = robot.SetRobotRealtimeStatePeriod(10);
            Console.WriteLine($"SetRobotRealtimeStatePeriod(10ms) 返回: {ret} (预期 0)");
            robot.SetRobotRealtimeStateConfig(states1, 10);
            ret = robot.RPC("192.168.58.2");
            Console.WriteLine($"RPC 连接结果: {ret}");
            if (ret == 0)
            {
                Console.WriteLine("测试接口错误码完成，等待3秒后自动断开...");
                await Task.Delay(3000);
                robot.CloseRPC();
            }
        }
        private async void TestNormalFeedbackAndPeriod()
        {
            int ret = 0;
            // 设置需要反馈的状态：关节位置、TCP位姿、机器人时间、负载质量及质心、关节指令位置
            //List<RobotState> states = new List<RobotState>
            //{
            //    RobotState.JointCurPos,
            //    RobotState.ToolCurPos,
            //    RobotState.RobotTime,
            //    RobotState.Load,
            //    RobotState.LoadCog,
            //    RobotState.TargetJointPos,   // 关节指令位置
            //    RobotState.CollisionLevel
            //};
            //int periodMs = 8;   // 要求周期为8ms
            //int ret = robot.SetRobotRealtimeStatePeriod(periodMs);
            ret = robot.AddRobotRealtimeState(RobotState.CollisionLevel);
            //Console.WriteLine($"AddRobotRealtimeState(已存在JointCurPos) 返回: {ret} (预期  ERR_STATE_ALREADY_EXISTS -17)");
            //int periodMs = 8;
            //int ret = robot.SetRobotRealtimeStateConfig(states, periodMs);
            //Console.WriteLine($"配置状态结果: {ret}");

            //Console.WriteLine($"配置状态结果: {ret}");

            // 建立 RPC 连接
            ret = robot.RPC("192.168.58.2");
            if (ret != 0)
            {
                Console.WriteLine($"RPC 连接失败: {ret}");
                return;
            }
            Console.WriteLine("RPC 连接成功，开始接收数据，周期 8ms...");

            // 记录上一帧时间戳，用于计算间隔
            DateTime lastTimestamp = DateTime.Now;
            int frameCount = 0;

            // 循环接收 5 秒，每秒打印一次详细数据，同时记录每帧时间间隔
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds < 2500)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                ret = robot.GetRobotRealTimeState(ref pkg);
                DateTime now = DateTime.Now;
                double interval = (now - lastTimestamp).TotalMilliseconds;
                lastTimestamp = now;
                frameCount++;

                // 每收到一帧打印时间戳和间隔（用于验证周期）
                //Console.WriteLine($"[帧 {frameCount}] 时间戳: {now:HH:mm:ss.fff}, 间隔: {interval:F1} ms");
                printCNDE();
                //// 碰撞等级
                if (pkg.collisionLevel != null && pkg.collisionLevel.Length >= 6)
                    Console.WriteLine($"碰撞等级: J1={pkg.collisionLevel[0]}, J2={pkg.collisionLevel[1]}, J3={pkg.collisionLevel[2]}, J4={pkg.collisionLevel[3]}, J5={pkg.collisionLevel[4]}, J6={pkg.collisionLevel[5]}");

                // 每 1 秒打印一次详细数据（避免控制台刷屏）
                //if (frameCount % 1 == 0)  // 8ms周期，1秒约125帧
                //{
                //    Console.WriteLine($"\n--- 详细数据 ---");
                //    if (pkg.jt_cur_pos != null && pkg.jt_cur_pos.Length >= 6)
                //        Console.WriteLine($"  关节位置(°): J1={pkg.jt_cur_pos[0]:F2}, J2={pkg.jt_cur_pos[1]:F2}, J3={pkg.jt_cur_pos[2]:F2}, J4={pkg.jt_cur_pos[3]:F2}, J5={pkg.jt_cur_pos[4]:F2}, J6={pkg.jt_cur_pos[5]:F2}");
                //    if (pkg.tl_cur_pos != null && pkg.tl_cur_pos.Length >= 6)
                //        Console.WriteLine($"  TCP位姿(mm/°): X={pkg.tl_cur_pos[0]:F2}, Y={pkg.tl_cur_pos[1]:F2}, Z={pkg.tl_cur_pos[2]:F2}, RX={pkg.tl_cur_pos[3]:F2}, RY={pkg.tl_cur_pos[4]:F2}, RZ={pkg.tl_cur_pos[5]:F2}");
                //    Console.WriteLine($"  机器人时间: {pkg.robotTime.ToString()}");
                //    Console.WriteLine($"  负载质量: {pkg.load:F2} kg");
                //    if (pkg.loadCog != null && pkg.loadCog.Length >= 3)
                //        Console.WriteLine($"  负载质心(mm): X={pkg.loadCog[0]:F2}, Y={pkg.loadCog[1]:F2}, Z={pkg.loadCog[2]:F2}");
                //    if (pkg.targetJointPos != null && pkg.targetJointPos.Length >= 6)
                //        Console.WriteLine($"  关节指令位置(°): J1={pkg.targetJointPos[0]:F2}, J2={pkg.targetJointPos[1]:F2}, J3={pkg.targetJointPos[2]:F2}, J4={pkg.targetJointPos[3]:F2}, J5={pkg.targetJointPos[4]:F2}, J6={pkg.targetJointPos[5]:F2} (应为0)");
                //}

                // 等待约 8ms 再读下一帧（实际间隔由机器人决定，这里只是避免循环过紧）
                await Task.Delay(100);
            }

            Console.WriteLine("\n测试结束，断开连接...");
            robot.CloseRPC();
        }


        private async void TestInvalidStateConfig()
        {
            //robot = new Robot();
            //robot.LoggerInit(FrLogType.BUFFER, FrLogLevel.INFO, "D://log/", 5, 5);
            //robot.SetLoggerLevel(FrLogLevel.INFO);
            //robot.SetReconnectParam(true, 100, 1000);

            // 定义包含不存在状态 OtherState 的列表
            // 注意：RobotState.OtherState 在枚举中不存在，这里用强制转换一个不存在的值模拟
            // 实际使用时应使用一个肯定不存在的枚举值，比如 (RobotState)999
            List<RobotState> invalidStates = new List<RobotState>
            {
                RobotState.JointCurPos,
                (RobotState)999   // 不存在的状态
            };
            int periodMs = 20;
            int ret = robot.SetRobotRealtimeStateConfig(invalidStates, periodMs);
            Console.WriteLine($"配置不存在的状态返回: {ret} (预期 -18 或 ERR_STATE_INVALID=1003)");

            // 建立 RPC 连接，观察是否会返回错误
            ret = robot.RPC("192.168.58.2");
            if (ret != 0)
            {
                Console.WriteLine($"RPC 连接失败，错误码: {ret}");
                return;
            }

            // 尝试打印数据（实际上配置失败，机器人可能不会发送数据）
            for (int i = 0; i < 50000000; i++)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                ret = robot.GetRobotRealTimeState(ref pkg);
                if (pkg.jt_cur_pos != null && pkg.jt_cur_pos.Length >= 6)
                {
                    Console.WriteLine($"  关节位置(°): J1={pkg.jt_cur_pos[0]:F2}, J2={pkg.jt_cur_pos[1]:F2}, J3={pkg.jt_cur_pos[2]:F2}, J4={pkg.jt_cur_pos[3]:F2}, J5={pkg.jt_cur_pos[4]:F2}, J6={pkg.jt_cur_pos[5]:F2}");
                }
                await Task.Delay(10);
            }

            robot.CloseRPC();
            Console.WriteLine("测试结束。请检查日志文件 D://log/ 中是否有错误记录。");
        }



        private async void TestAddDeleteCNDE()
        {
            List<RobotState> finalStates;
            int finalPeriod;
            // 初始配置：不请求任何状态（默认配置）
            List<RobotState> emptyStates = new List<RobotState>();
            int ret = robot.SetRobotRealtimeStateConfig(emptyStates, 20);

            robot.SetRobotRealtimeStatePeriod(10);
            // 删除两个状态
            ret = robot.DeleteRobotRealtimeState(RobotState.JointCurPos);
            Console.WriteLine($"删除 JointCurPos 结果: {ret}");
            ret = robot.DeleteRobotRealtimeState(RobotState.ToolCurPos);
            Console.WriteLine($"删除 ToolCurPos 结果: {ret}");
            // 新增一个状态
            ret = robot.AddRobotRealtimeState(RobotState.CollisionLevel);
            Console.WriteLine($"新增 CollisionLevel 结果: {ret}");

            // 获取当前配置列表并重新发送
            List<RobotState> currentStates;
            int currentPeriod;
            robot.GetRobotRealtimeStateConfig(out currentStates, out currentPeriod);
            Console.WriteLine($"当前配置状态数: {currentStates.Count}");
            ret = robot.SetRobotRealtimeStateConfig(currentStates, currentPeriod);
            Console.WriteLine($"应用新配置结果: {ret}"); Console.WriteLine($"初始配置结果: {ret}");
            robot.GetRobotRealtimeStateConfig(out finalStates, out finalPeriod);
            Console.WriteLine($"配置状态数量: {finalStates.Count}");
            foreach (var s in finalStates) Console.WriteLine($"  {s}");
            Console.WriteLine($"周期: {finalPeriod} ms");

            Thread.Sleep(1000);
            //  建立 RPC 连接（内部自动连接 CNDE）
            robot.SetReconnectParam(true, 100, 1000);
            ret = robot.RPC("192.168.58.2");
            if (ret != 0)
            {
                Console.WriteLine($"RPC 连接失败: {ret}");
                return;
            }

            // 循环打印删除和新增的状态，删除的状态打印为0，新增的状态可正常获取实时值
            DateTime lastTime = DateTime.Now;
            int frameCount = 0;
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds < 10)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                robot.GetRobotRealTimeState(ref pkg);
                DateTime now = DateTime.Now;
                double interval = (now - lastTime).TotalMilliseconds;
                lastTime = now;
                frameCount++;

                if (pkg.jt_cur_pos != null && pkg.jt_cur_pos.Length >= 6)
                {
                    Console.WriteLine($"  关节位置(°): J1={pkg.jt_cur_pos[0]:F2}, J2={pkg.jt_cur_pos[1]:F2}, J3={pkg.jt_cur_pos[2]:F2}, J4={pkg.jt_cur_pos[3]:F2}, J5={pkg.jt_cur_pos[4]:F2}, J6={pkg.jt_cur_pos[5]:F2}");
                }
                if (pkg.tl_cur_pos != null && pkg.tl_cur_pos.Length >= 6)
                {
                    Console.WriteLine($"  TCP位姿(mm/°): X={pkg.tl_cur_pos[0]:F2}, Y={pkg.tl_cur_pos[1]:F2}, Z={pkg.tl_cur_pos[2]:F2}, RX={pkg.tl_cur_pos[3]:F2}, RY={pkg.tl_cur_pos[4]:F2}, RZ={pkg.tl_cur_pos[5]:F2}");
                }
                // 碰撞等级
                if (pkg.collisionLevel != null && pkg.collisionLevel.Length >= 6)
                    Console.WriteLine($"碰撞等级: J1={pkg.collisionLevel[0]}, J2={pkg.collisionLevel[1]}, J3={pkg.collisionLevel[2]}, J4={pkg.collisionLevel[3]}, J5={pkg.collisionLevel[4]}, J6={pkg.collisionLevel[5]}");

                await Task.Delay(50);
            }
            //断开连接
            robot.CloseRPC();
            Console.WriteLine("测试完成。");
        }

        /// <summary>
        /// 测试机器人本体实时状态反馈（使用正确的结构体变量名）
        /// 配置指定字段组，周期 8ms，建立 RPC 连接后循环打印状态值
        /// </summary>
        private async void TestRobotRealtimeStates()
        {
            // 1. 定义需要订阅的状态字段
            List<RobotState> requiredStates = new List<RobotState>
            {
                RobotState.JointCurPos,
                RobotState.ToolCurPos, 
                RobotState.JointDriverTemperature,
                RobotState.RobotTime,
            };

            // 2. 配置状态反馈（周期 8ms）
            int periodMs = 8;
            int ret = robot.SetRobotRealtimeStateConfig(requiredStates, periodMs);
            if (ret != 0)
            {
                Console.WriteLine($"配置状态失败，错误码: {ret}");
                return;
            }
            Console.WriteLine($"状态配置成功，共 {requiredStates.Count} 个字段，周期 {periodMs} ms");

            // 验证配置是否生效
            List<RobotState> actualStates;
            int actualPeriod;
            robot.GetRobotRealtimeStateConfig(out actualStates, out actualPeriod);
            Console.WriteLine($"实际生效的状态数: {actualStates.Count}, 周期: {actualPeriod} ms");
            Thread.Sleep(3000);
            // 3. 建立 RPC 连接（内部自动完成 CNDE 握手）
            robot.SetReconnectParam(true, 10, 1000);
            ret = robot.RPC("192.168.58.2");  // 请根据实际机器人 IP 修改
            if (ret != 0)
            {
                Console.WriteLine($"RPC 连接失败，错误码: {ret}");
                return;
            }
            // 4. 循环读取并打印状态数据
            DateTime startTime = DateTime.Now;
            const int durationSeconds = 500;

            while ((DateTime.Now - startTime).TotalSeconds < durationSeconds)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                ret = robot.GetRobotRealTimeState(ref pkg);
                Console.WriteLine($"GetRobotRealTimeState: {ret}");

                //关节位置（度）
                if (pkg.jt_cur_pos != null && pkg.jt_cur_pos.Length >= 6)
                    Console.WriteLine($"关节位置(°): J1={pkg.jt_cur_pos[0]:F2}, J2={pkg.jt_cur_pos[1]:F2}, J3={pkg.jt_cur_pos[2]:F2}, J4={pkg.jt_cur_pos[3]:F2}, J5={pkg.jt_cur_pos[4]:F2}, J6={pkg.jt_cur_pos[5]:F2}");

                //TCP 位姿（mm /°）
                if (pkg.tl_cur_pos != null && pkg.tl_cur_pos.Length >= 6)
                    Console.WriteLine($"TCP位姿(mm/°): X={pkg.tl_cur_pos[0]:F2}, Y={pkg.tl_cur_pos[1]:F2}, Z={pkg.tl_cur_pos[2]:F2}, RX={pkg.tl_cur_pos[3]:F2}, RY={pkg.tl_cur_pos[4]:F2}, RZ={pkg.tl_cur_pos[5]:F2}");
   
                // 关节温度
                if (pkg.jointDriverTemperature != null && pkg.jointDriverTemperature.Length >= 6)
                    Console.WriteLine($"关节温度(°C): J1={pkg.jointDriverTemperature[0]:F2}, J2={pkg.jointDriverTemperature[1]:F2}, J3={pkg.jointDriverTemperature[2]:F2}, J4={pkg.jointDriverTemperature[3]:F2}, J5={pkg.jointDriverTemperature[4]:F2}, J6={pkg.jointDriverTemperature[5]:F2}");

                // 机器人时间
                Console.WriteLine($"机器人时间: {pkg.robotTime.year}-{pkg.robotTime.mouth:D2}-{pkg.robotTime.day:D2} {pkg.robotTime.hour:D2}:{pkg.robotTime.minute:D2}:{pkg.robotTime.second:D2}.{pkg.robotTime.millisecond:D3}");

                await Task.Delay(100);
            }

            // 5. 断开连接
            robot.CloseRPC();
        }

        /// <summary>
        /// 测试机器人运行相关的状态反馈（手自动、DI、坐标系等）
        /// 配置指定字段组，周期 8ms，建立 RPC 连接后循环打印状态值
        /// </summary>
        private async void TestRobotOperationalStates()
        {
            // 1. 定义需要订阅的状态字段（与题目要求完全一致）
            List<RobotState> requiredStates = new List<RobotState>
            {
                //RobotState.Load,
                //RobotState.LoadCog,
                //RobotState.CollisionLevel
                //RobotState.ProgramState,
                //RobotState.RobotState,
                //RobotState.RobotMode,
                //RobotState.MotionDone,
                //RobotState.McQueueLen,
                //RobotState.TrajectoryPnum,
                //RobotState.Tool,
                //RobotState.User,
                //RobotState.ClDgtOutputH,
                //RobotState.ClDgtOutputL,
                //RobotState.TlDgtOutputL,
                //RobotState.ClDgtInputH,
                //RobotState.ClDgtInputL,
                //RobotState.TlDgtInputL,
                RobotState.ClAnalogInput,
                RobotState.TlAnglogInput,
                //RobotState.RbtEnableState,
                //RobotState.SoftwareUpgradeState,
                //RobotState.WeldingBreakOffState,
                RobotState.ClAnalogOutput,
                RobotState.TlAnalogOutput,
                //RobotState.ToolCoord,
                //RobotState.WobjCoord,
                //RobotState.ExtoolCoord,
                //RobotState.ExAxisCoord,
                //RobotState.Load,
                //RobotState.LoadCog,
                //RobotState.LastServoTarget,
                //RobotState.ServoJCmdNum,
                //RobotState.CollisionLevel,
                //RobotState.SpeedScaleManual,
                //RobotState.SpeedScaleAuto,
                //RobotState.LuaLineNum,
                //RobotState.AbnomalStop,
                //RobotState.CurrentLuaFileName,
                //RobotState.ProgramTotalLine,
                //RobotState.WeldVoltage,
                //RobotState.WeldCurrent,
                //RobotState.WeldTrackVel,
                //RobotState.UdpCmdState,
                //RobotState.WeldReadyState
            };

            // 2. 配置状态反馈（周期 8ms）
            int periodMs = 15;
            int ret = robot.SetRobotRealtimeStateConfig(requiredStates, periodMs);
            if (ret != 0)
            {
                Console.WriteLine($"配置状态失败，错误码: {ret}");
                return;
            }
            Console.WriteLine($"状态配置成功，共 {requiredStates.Count} 个字段，周期 {periodMs} ms");

            // 可选：验证配置是否生效
            List<RobotState> actualStates;
            int actualPeriod;
            robot.GetRobotRealtimeStateConfig(out actualStates, out actualPeriod);
            Console.WriteLine($"实际生效的状态数: {actualStates.Count}, 周期: {actualPeriod} ms");

            // 3. 建立 RPC 连接（内部自动完成 CNDE 握手）
            robot.SetReconnectParam(true, 100, 1000);
            ret = robot.RPC("192.168.58.3");  // 请根据实际机器人 IP 修改
            if (ret != 0)
            {
                Console.WriteLine($"RPC 连接失败，错误码: {ret}");
                return;
            }
            //Console.WriteLine("RPC 连接成功，开始获取机器人运行状态（将持续 30 秒）...");
            //Console.WriteLine("提示：此时可以进行以下操作，观察下方打印数据变化：");
            //Console.WriteLine("  - 切换手自动模式");
            //Console.WriteLine("  - 触发控制箱 DI 输入（如安全门、急停等）");
            //Console.WriteLine("  - 切换工具坐标系/工件坐标系");
            //Console.WriteLine("  - 加载不同程序等");

            // 4. 循环读取并打印状态数据
            DateTime startTime = DateTime.Now;
            int frameCount = 0;
            const int durationSeconds = 300;

            while ((DateTime.Now - startTime).TotalSeconds < durationSeconds)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                ret = robot.GetRobotRealTimeState(ref pkg);

                Console.WriteLine($"\n========== 帧 {++frameCount} @ {DateTime.Now:HH:mm:ss.fff} ==========");
                //Console.WriteLine($"负载质量: {pkg.load:F3} kg");
                //if (pkg.loadCog != null && pkg.loadCog.Length >= 3)
                //    Console.WriteLine($"负载质心: X={pkg.loadCog[0]:F3}, Y={pkg.loadCog[1]:F3}, Z={pkg.loadCog[2]:F3}");

                //// 碰撞等级
                //if (pkg.collisionLevel != null && pkg.collisionLevel.Length >= 6)
                //    Console.WriteLine($"碰撞等级: J1={pkg.collisionLevel[0]}, J2={pkg.collisionLevel[1]}, J3={pkg.collisionLevel[2]}, J4={pkg.collisionLevel[3]}, J5={pkg.collisionLevel[4]}, J6={pkg.collisionLevel[5]}");
                // 程序与机器人基础状态
                //Console.WriteLine($"程序状态: {pkg.program_state}  机器人状态: {pkg.robot_state}  机器人模式: {pkg.robot_mode}");
                //Console.WriteLine($"运动完成标志: {pkg.motion_done}  运动队列长度: {pkg.mc_queue_len}  轨迹点数: {pkg.trajectory_pnum}");
                //Console.WriteLine($"工具编号: {pkg.tool}  工件坐标系编号: {pkg.user}");
                //Console.WriteLine($"机器人使能状态: {pkg.rbtEnableState}  软件升级状态: {pkg.softwareUpgradeState}");

                //// IO 状态
                //Console.WriteLine($"控制箱数字输出高8位: 0x{pkg.cl_dgt_output_h:X2}  低8位: 0x{pkg.cl_dgt_output_l:X2}");
                //Console.WriteLine($"工具数字输出: 0x{pkg.tl_dgt_output_l:X2}");
                //Console.WriteLine($"控制箱数字输入高8位: 0x{pkg.cl_dgt_input_h:X2}  低8位: 0x{pkg.cl_dgt_input_l:X2}");
                //Console.WriteLine($"工具数字输入: 0x{pkg.tl_dgt_input_l:X2}");
                //if (pkg.cl_analog_input != null && pkg.cl_analog_input.Length >= 2)
                //    Console.WriteLine($"控制箱模拟输入: AI0={(double)(pkg.cl_analog_input[0] * 100 / 4096):F2}%, AI1={pkg.cl_analog_input[1]}");
                //Console.WriteLine($"工具模拟输入: {pkg.tl_anglog_input}");
                //if (pkg.cl_analog_output != null && pkg.cl_analog_output.Length >= 2)
                //    Console.WriteLine($"控制箱模拟输出: AO0={pkg.cl_analog_output[0]}, AO1={pkg.cl_analog_output[1]}");
                //Console.WriteLine($"工具模拟输出: {pkg.tl_analog_output}");
                if (pkg.cl_analog_input != null && pkg.cl_analog_input.Length >= 2)
                {
                    double ai0Percent = (double)pkg.cl_analog_input[0] * 100 / 4096;
                    double ai1Percent = (double)pkg.cl_analog_input[1] * 100 / 4096;
                    Console.WriteLine($"控制箱模拟输入: AI0={ai0Percent:F2}%, AI1={ai1Percent:F2}%");
                }
                else
                {
                    Console.WriteLine("控制箱模拟输入: 数据无效");
                }

                double toolAnalogInputPercent = (double)pkg.tl_anglog_input * 100 / 4096;
                Console.WriteLine($"工具模拟输入: {toolAnalogInputPercent:F2}%");

                if (pkg.cl_analog_output != null && pkg.cl_analog_output.Length >= 2)
                {
                    double ao0Percent = (double)pkg.cl_analog_output[0] * 100 / 4096;
                    double ao1Percent = (double)pkg.cl_analog_output[1] * 100 / 4096;
                    Console.WriteLine($"控制箱模拟输出: AO0={ao0Percent:F2}%, AO1={ao1Percent:F2}%");
                }
                else
                {
                    Console.WriteLine("控制箱模拟输出: 数据无效");
                }

                double toolAnalogOutputPercent = (double)pkg.tl_analog_output * 100 / 4096;
                Console.WriteLine($"工具模拟输出: {toolAnalogOutputPercent:F2}%");

                // 焊接相关

                //Console.WriteLine($"焊接断弧状态: {pkg.weldingBreakOffState.breakOffState}  焊接起弧状态: {pkg.weldingBreakOffState.weldArcState}");
                ////Console.WriteLine($"焊接电压: {pkg.weldVoltage:F2} V  焊接电流: {pkg.weldCurrent:F2} A  焊缝跟踪速度: {pkg.weldTrackVel:F2} mm/s");
                //Console.WriteLine($"焊接就绪状态: {pkg.weldReadyState}");

                // 坐标系数据
                //if (pkg.toolCoord != null && pkg.toolCoord.Length >= 6)
                //    Console.WriteLine($"工具坐标系: X={pkg.toolCoord[0]:F3}, Y={pkg.toolCoord[1]:F3}, Z={pkg.toolCoord[2]:F3}, RX={pkg.toolCoord[3]:F3}, RY={pkg.toolCoord[4]:F3}, RZ={pkg.toolCoord[5]:F3}");
                //if (pkg.wobjCoord != null && pkg.wobjCoord.Length >= 6)
                //    Console.WriteLine($"工件坐标系: X={pkg.wobjCoord[0]:F3}, Y={pkg.wobjCoord[1]:F3}, Z={pkg.wobjCoord[2]:F3}, RX={pkg.wobjCoord[3]:F3}, RY={pkg.wobjCoord[4]:F3}, RZ={pkg.wobjCoord[5]:F3}");
                //if (pkg.extoolCoord != null && pkg.extoolCoord.Length >= 6)
                //    Console.WriteLine($"扩展工具坐标系: X={pkg.extoolCoord[0]:F3}, Y={pkg.extoolCoord[1]:F3}, Z={pkg.extoolCoord[2]:F3}, RX={pkg.extoolCoord[3]:F3}, RY={pkg.extoolCoord[4]:F3}, RZ={pkg.extoolCoord[5]:F3}");
                //if (pkg.exAxisCoord != null && pkg.exAxisCoord.Length >= 6)
                //    Console.WriteLine($"扩展轴坐标系: J1={pkg.exAxisCoord[0]:F3}, J2={pkg.exAxisCoord[1]:F3}, J3={pkg.exAxisCoord[2]:F3}, J4={pkg.exAxisCoord[3]:F3}, J5={pkg.exAxisCoord[4]:F3}, J6={pkg.exAxisCoord[5]:F3}");

                //// 负载
                //Console.WriteLine($"负载质量: {pkg.load:F3} kg");
                //if (pkg.loadCog != null && pkg.loadCog.Length >= 3)
                //    Console.WriteLine($"负载质心: X={pkg.loadCog[0]:F3}, Y={pkg.loadCog[1]:F3}, Z={pkg.loadCog[2]:F3}");

                //// 伺服相关
                //if (pkg.lastServoTarget != null && pkg.lastServoTarget.Length >= 6)
                //    Console.WriteLine($"上次伺服J目标位置: [{string.Join(",", pkg.lastServoTarget.Select(v => v.ToString("F3")))}]");
                //Console.WriteLine($"伺服J命令数量: {pkg.servoJCmdNum}");

                //// 碰撞等级与速度倍率
                //if (pkg.collisionLevel != null && pkg.collisionLevel.Length >= 6)
                //    Console.WriteLine($"碰撞等级: J1={pkg.collisionLevel[0]}, J2={pkg.collisionLevel[1]}, J3={pkg.collisionLevel[2]}, J4={pkg.collisionLevel[3]}, J5={pkg.collisionLevel[4]}, J6={pkg.collisionLevel[5]}");
                //Console.WriteLine($"手动模式速度百分比: {pkg.speedScaleManual:F2}  自动模式速度百分比: {pkg.speedScaleAuto:F2}");

                //// 程序信息
                //Console.WriteLine($"Lua行号: {pkg.luaLineNum}  异常停止标志: {pkg.abnomalStop}");
                //string curLuaFileName = pkg.currentLuaFileName != null ? System.Text.Encoding.ASCII.GetString(pkg.currentLuaFileName).TrimEnd('\0') : "";
                //Console.WriteLine($"当前Lua文件名: {curLuaFileName}  程序总行数: {pkg.programTotalLine}");

                //// UDP 命令状态
                //Console.WriteLine($"UDP命令状态: {pkg.udpCmdState}");

                // 等待一段时间再读取下一帧（8ms周期下，每50ms打印一次即可，避免刷屏过快）
                await Task.Delay(100);
            }

            // 5. 断开连接
            robot.CloseRPC();
            Console.WriteLine("测试结束，RPC 连接已关闭。");
        }

        private async void RunCNDETest()
        {
            List<RobotState> finalStates;
            int finalPeriod;


            // 2. 配置状态：JointCurPos, ToolCurPos，周期 20ms
            List<RobotState> states1 = new List<RobotState>
            {
                RobotState.JointCurPos,
                RobotState.ToolCurPos
            };
            int period1 = 20;
            int ret = robot.SetRobotRealtimeStateConfig(states1, period1);
            Console.WriteLine($"初始配置结果: {ret}");
            robot.GetRobotRealtimeStateConfig(out finalStates, out finalPeriod);
            Console.WriteLine($"配置状态数量: {finalStates.Count}");
            foreach (var s in finalStates) Console.WriteLine($"  {s}");
            Console.WriteLine($"周期: {finalPeriod} ms");

            // 3. 建立 RPC 连接
            robot.SetReconnectParam(true, 100, 1000);
            ret = robot.RPC("192.168.58.3");
            if (ret != 0)
            {
                Console.WriteLine($"RPC 连接失败: {ret}");
                return;
            }
            Console.WriteLine("RPC 连接成功，CNDE 已连接，开始打印关节和 TCP 位姿...");

            // 4. 打印数据，持续 10 秒，记录时间戳
            DateTime lastTime = DateTime.Now;
            int frameCount = 0;
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds < 10)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                ret = robot.GetRobotRealTimeState(ref pkg);
                DateTime now = DateTime.Now;
                double interval = (now - lastTime).TotalMilliseconds;
                lastTime = now;
                frameCount++;

                //Console.WriteLine($"\n[帧 {frameCount}] 时间戳: {now:HH:mm:ss.fff}, 间隔: {interval:F1} ms");
                if (pkg.jt_cur_pos != null && pkg.jt_cur_pos.Length >= 6)
                {
                    Console.WriteLine($"  关节位置(°): J1={pkg.jt_cur_pos[0]:F2}, J2={pkg.jt_cur_pos[1]:F2}, J3={pkg.jt_cur_pos[2]:F2}, J4={pkg.jt_cur_pos[3]:F2}, J5={pkg.jt_cur_pos[4]:F2}, J6={pkg.jt_cur_pos[5]:F2}");
                }
                if (pkg.tl_cur_pos != null && pkg.tl_cur_pos.Length >= 6)
                {
                    Console.WriteLine($"  TCP位姿(mm/°): X={pkg.tl_cur_pos[0]:F2}, Y={pkg.tl_cur_pos[1]:F2}, Z={pkg.tl_cur_pos[2]:F2}, RX={pkg.tl_cur_pos[3]:F2}, RY={pkg.tl_cur_pos[4]:F2}, RZ={pkg.tl_cur_pos[5]:F2}");
                }
                await Task.Delay(20); // 保持与周期相近的打印频率
            }

            Console.WriteLine("\n第一步完成，等待 2 秒后修改配置...");
            await Task.Delay(2000);

            // 5. 断开 CNDE 连接（保持 RPC 不断）
            robot.DisconnectCNDE();
            Console.WriteLine("CNDE 已断开");

            // 6. 修改配置：RobotMode, RbtEnableState，周期 10ms
            List<RobotState> states2 = new List<RobotState>
            {
                RobotState.RobotMode,
                RobotState.RbtEnableState
            };
            int period2 = 10;
            ret = robot.SetRobotRealtimeStateConfig(states2, period2);
            Console.WriteLine($"新配置结果: {ret}");
            robot.GetRobotRealtimeStateConfig(out finalStates, out finalPeriod);
            Console.WriteLine($"配置状态数量: {finalStates.Count}");
            foreach (var s in finalStates) Console.WriteLine($"  {s}");
            Console.WriteLine($"周期: {finalPeriod} ms");

            // 重新连接 CNDE
            ret = robot.ConnectCNDE("192.168.58.3", 20005);
            robot.SetReconnectParam(true, 100, 1000);
            if (ret != 0)
            {
                Console.WriteLine($"CNDE 重连失败: {ret}");
                return;
            }
            Console.WriteLine("CNDE 重新连接成功，开始打印机器人模式和使能状态...");

            // 7. 打印新数据，持续 5 秒，记录时间戳
            lastTime = DateTime.Now;
            frameCount = 0;
            startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds < 20)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                ret = robot.GetRobotRealTimeState(ref pkg);
                DateTime now = DateTime.Now;
                double interval = (now - lastTime).TotalMilliseconds;
                lastTime = now;
                frameCount++;

                //Console.WriteLine($"\n[帧 {frameCount}] 时间戳: {now:HH:mm:ss.fff}, 间隔: {interval:F1} ms");
                Console.WriteLine($"  机器人模式: {(pkg.robot_mode == 0 ? "自动" : "手动")}");
                Console.WriteLine($"  机器人使能状态: {pkg.rbtEnableState}");
                await Task.Delay(10);
            }

            Console.WriteLine("\n测试完成。");
            robot.CloseRPC();
        }

        /// <summary>
        /// 测试机器人扩展轴及扩展IO状态反馈
        /// 配置 AuxState, ExtAxisStatus, ExtDIState, ExtDOState, ExtAIState, ExtAOState
        /// 周期 8ms，建立 RPC 连接后循环打印状态值
        /// </summary>
        private async void TestExtendedAxisAndIOStates()
        {
            // 1. 定义需要订阅的状态字段（扩展轴与扩展IO）
            List<RobotState> requiredStates = new List<RobotState>
            {
                RobotState.AuxState,        // 辅助轴状态（如伺服参数）
                RobotState.ExtAxisStatus,   // 扩展轴状态（29字节，包含多轴信息）
                RobotState.ExtDIState,      // 扩展数字输入（16字节，每bit代表一个DI）
                RobotState.ExtDOState,      // 扩展数字输出（16字节）
                RobotState.ExtAIState,      // 扩展模拟输入（4个INT32）
                RobotState.ExtAOState       // 扩展模拟输出（4个INT32）

            };

            // 2. 配置状态反馈（周期 8ms）
            int periodMs = 20;
            int ret = robot.SetRobotRealtimeStateConfig(requiredStates, periodMs);
            if (ret != 0)
            {
                Console.WriteLine($"配置状态失败，错误码: {ret}");
                return;
            }
            Console.WriteLine($"状态配置成功，共 {requiredStates.Count} 个字段，周期 {periodMs} ms");

            // 可选：验证配置是否生效
            List<RobotState> actualStates;
            int actualPeriod;
            robot.GetRobotRealtimeStateConfig(out actualStates, out actualPeriod);
            Console.WriteLine($"实际生效的状态数: {actualStates.Count}, 周期: {actualPeriod} ms");

            // 3. 建立 RPC 连接（内部自动完成 CNDE 握手）
            robot.SetReconnectParam(true, 100, 1000);
            ret = robot.RPC("192.168.58.2");  // 请根据实际机器人 IP 修改
            if (ret != 0)
            {
                Console.WriteLine($"RPC 连接失败，错误码: {ret}");
                return;
            }
            Console.WriteLine("RPC 连接成功，开始获取扩展轴及扩展IO状态（将持续 30 秒）...");
            Console.WriteLine("提示：此时可以进行以下操作，观察下方打印数据变化：");
            Console.WriteLine("  - 控制485扩展轴运动（如外部轴）");
            Console.WriteLine("  - 控制UDP扩展轴运动");
            Console.WriteLine("  - 触发扩展IO（数字/模拟输入输出）");

            // 4. 循环读取并打印状态数据
            DateTime startTime = DateTime.Now;
            int frameCount = 0;
            const int durationSeconds = 300;

            while ((DateTime.Now - startTime).TotalSeconds < durationSeconds)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                ret = robot.GetRobotRealTimeState(ref pkg);

                Console.WriteLine($"\n========== 帧 {++frameCount} @ {DateTime.Now:HH:mm:ss.fff} ==========");

                // ----- AuxState（辅助轴状态）-----
                //if (pkg.auxState != null)
                
                    Console.WriteLine($"辅助轴状态:");
                    Console.WriteLine($"  servoId: {pkg.auxState.servoId}");
                    Console.WriteLine($"  servoErrCode: {pkg.auxState.servoErrCode}");
                    Console.WriteLine($"  servoState: {pkg.auxState.servoState}");
                    Console.WriteLine($"  servoPos: {pkg.auxState.servoPos:F3} deg");
                    Console.WriteLine($"  servoVel: {pkg.auxState.servoVel:F3} deg/s");
                    Console.WriteLine($"  servoTorque: {pkg.auxState.servoTorque:F3} Nm");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("辅助轴状态: null");
                    //}

                    // ----- ExtAxisStatus（扩展轴状态）-----
                    // 注意：根据协议，extAxisStatus 可能是 byte[29] 或自定义结构体，这里假设为 byte 数组
                    if (pkg.extAxisStatus != null && pkg.extAxisStatus.Length > 0)
                    {
                        Console.Write($"扩展轴状态({pkg.extAxisStatus.Length} bytes): ");
                        // 打印前16字节（可根据需要调整）
                        for (int i = 0; i < Math.Min(16, pkg.extAxisStatus.Length); i++)
                            Console.Write($"{pkg.extAxisStatus[i]:X2} ");
                        Console.WriteLine();
                    // 如果需要解析具体轴位置/状态，需根据机器人协议定义进行转换
                    Console.WriteLine("extaxis udp states pos {0}; vel {1}; errorCode {2}; ready {3}; inPos {4}; alarm {5}; flerr {6}; nlimit {7}; pLimit {8}; homingStatus {9}",
                                        pkg.extAxisStatus[0].pos,
                                        pkg.extAxisStatus[0].vel,
                                        pkg.extAxisStatus[0].errorCode,
                                        pkg.extAxisStatus[0].ready,
                                        pkg.extAxisStatus[0].inPos,
                                        pkg.extAxisStatus[0].alarm,
                                        pkg.extAxisStatus[0].flerr,
                                        pkg.extAxisStatus[0].nlimit,
                                        pkg.extAxisStatus[0].pLimit,
                                        pkg.extAxisStatus[0].homingStatus);
                    }
                    else
                    {
                        Console.WriteLine("扩展轴状态: null");
                    }

                    // ----- ExtDIState（扩展数字输入）-----
                    // 通常为 16 字节，每个字节对应一组输入（8个DI）
                    if (pkg.extDIState != null && pkg.extDIState.Length > 0)
                    {
                        Console.Write($"扩展DI状态({pkg.extDIState.Length} bytes): ");
                        for (int i = 0; i < pkg.extDIState.Length; i++)
                            Console.Write($"{pkg.extDIState[i]:X2} ");
                        Console.WriteLine();
                        // 示例：解析第0字节的bit0-7
                        if (pkg.extDIState.Length >= 1)
                            Console.WriteLine($"  第1组DI (bit0~7): 0x{pkg.extDIState[0]:X2}");
                    }
                    else
                    {
                        Console.WriteLine("扩展DI状态: null");
                    }

                    // ----- ExtDOState（扩展数字输出）-----
                    if (pkg.extDOState != null && pkg.extDOState.Length > 0)
                    {
                        Console.Write($"扩展DO状态({pkg.extDOState.Length} bytes): ");
                        for (int i = 0; i < pkg.extDOState.Length; i++)
                            Console.Write($"{pkg.extDOState[i]:X2} ");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("扩展DO状态: null");
                    }

                    // ----- ExtAIState（扩展模拟输入）-----
                    // 通常为 4 个 INT32 值
                    if (pkg.extAIState != null && pkg.extAIState.Length >= 4)
                    {
                        Console.WriteLine($"扩展AI状态: AI0={pkg.extAIState[0]}, AI1={pkg.extAIState[1]}, AI2={pkg.extAIState[2]}, AI3={pkg.extAIState[3]}");
                    }
                    else
                    {
                        Console.WriteLine($"扩展AI状态: null 或长度不足 ({(pkg.extAIState == null ? "null" : pkg.extAIState.Length.ToString())})");
                    }

                    // ----- ExtAOState（扩展模拟输出）-----
                    if (pkg.extAOState != null && pkg.extAOState.Length >= 4)
                    {
                        Console.WriteLine($"扩展AO状态: AO0={pkg.extAOState[0]}, AO1={pkg.extAOState[1]}, AO2={pkg.extAOState[2]}, AO3={pkg.extAOState[3]}");
                    }
                    else
                    {
                        Console.WriteLine($"扩展AO状态: null 或长度不足 ({(pkg.extAOState == null ? "null" : pkg.extAOState.Length.ToString())})");
                    }

                    // 等待一段时间再读取下一帧（8ms周期下，每100ms打印一次即可，避免刷屏过快）
                    await Task.Delay(100);
             }

                // 5. 断开连接
                robot.CloseRPC();
                Console.WriteLine("测试结束，RPC 连接已关闭。");
        }

        /// <summary>
        /// 测试机器人力传感器和夹爪等外设状态反馈
        /// 配置 FtSensorRawData, FtSensorData, FtSensorActive, GripperMotiondone, GripperFaultId,
        /// GripperFault, GripperActive, GripperPosition, GripperSpeed, GripperCurrent, GripperTemp,
        /// GripperVoltage, GripperRotNum, GripperRotSpeed, GripperRotTorque, SmartToolState,
        /// ModbusMasterConnect, ModbusSlaveConnect, ForceSensorErrState
        /// 周期 8ms，建立 RPC 连接后循环打印状态值
        /// </summary>
        private async void TestGripperAndForceSensorStates()
        {
            // 1. 定义需要订阅的状态字段（力传感器与夹爪相关）
            List<RobotState> requiredStates = new List<RobotState>
            {
                RobotState.FtSensorRawData,
                RobotState.FtSensorData,
                RobotState.FtSensorActive,
                RobotState.GripperMotiondone,
                RobotState.GripperFaultId,
                RobotState.GripperFault,
                RobotState.GripperActive,
                RobotState.GripperPosition,
                RobotState.GripperSpeed,
                RobotState.GripperCurrent,
                RobotState.GripperTemp,
                RobotState.GripperVoltage,
                RobotState.GripperRotNum,
                RobotState.GripperRotSpeed,
                RobotState.GripperRotTorque,
                RobotState.SmartToolState,
                RobotState.ModbusMasterConnect,
                RobotState.ModbusSlaveConnect,
                RobotState.ForceSensorErrState,
                RobotState.AxleGenComData
            };

            // 2. 配置状态反馈（周期 8ms）
            int periodMs = 8;
            int ret = robot.SetRobotRealtimeStateConfig(requiredStates, periodMs);
            if (ret != 0)
            {
                Console.WriteLine($"配置状态失败，错误码: {ret}");
                return;
            }
            Console.WriteLine($"状态配置成功，共 {requiredStates.Count} 个字段，周期 {periodMs} ms");

            // 可选：验证配置是否生效
            List<RobotState> actualStates;
            int actualPeriod;
            robot.GetRobotRealtimeStateConfig(out actualStates, out actualPeriod);
            Console.WriteLine($"实际生效的状态数: {actualStates.Count}, 周期: {actualPeriod} ms");

            // 3. 建立 RPC 连接（内部自动完成 CNDE 握手）
            robot.SetReconnectParam(true, 100, 1000);
            ret = robot.RPC("192.168.58.2");  // 请根据实际机器人 IP 修改
            if (ret != 0)
            {
                Console.WriteLine($"RPC 连接失败，错误码: {ret}");
                return;
            }
            Console.WriteLine("RPC 连接成功，开始获取力传感器及夹爪状态（将持续 30 秒）...");
            Console.WriteLine("提示：此时可以进行以下操作，观察下方打印数据变化：");
            Console.WriteLine("  - 激活/运动夹爪（如夹爪开合）");
            Console.WriteLine("  - 激活力传感器（如施加外力）");
            Console.WriteLine("  - 检查夹爪故障、温度、电流等参数");
            int[] version = new int[5] { 0xAB, 0xBA, 0x11, 0x00, 0x76 };
            int[] state = new int[6] { 0xAB, 0xBA, 0x1B, 0x01, 0xAA, 0x2B };
            int[] cycleState = new int[6] { 0xAB, 0xBA, 0x12, 0x01, 0x00, 0x78 };

            int[] rcvdata = new int[16];
            // 4. 循环读取并打印状态数据
            DateTime startTime = DateTime.Now;
            int frameCount = 0;
            const int durationSeconds = 3000;

            while ((DateTime.Now - startTime).TotalSeconds < durationSeconds)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                ret = robot.GetRobotRealTimeState(ref pkg);

                Console.WriteLine($"\n========== 帧 {++frameCount} @ {DateTime.Now:HH:mm:ss.fff} ==========");

                ////读取版本号
                //ret = robot.SndRcvAxleGenComCmdData(5, version, 10, ref rcvdata);
                //Console.WriteLine($"SndRcvAxleGenComCmdData ret：{ret} hard version : {rcvdata[4]},hard code:{rcvdata[5]}, soft version:{rcvdata[6]} {rcvdata[7]}, soft code:{rcvdata[8]}");
                //if (ret != 0)
                //{
                //    break;
                //}
                //Thread.Sleep(1000);
                ////读取艾灸头在位状态
                //ret = robot.SndRcvAxleGenComCmdData(6, state, 6, ref rcvdata);
                //Console.WriteLine($"SndRcvAxleGenComCmdData ret：{ret} state : {rcvdata[4]}");
                //Thread.Sleep(1000);

                //int errorcode = pkg.axleGenComData[0];
                //int datalen = pkg.axleGenComData[1];
                //// 过滤异常包
                //if ((errorcode != 0) || (datalen == 0) ||
                //(pkg.axleGenComData[2] != 0xAB) ||
                //(pkg.axleGenComData[3] != 0xBA))
                //{
                //    Console.WriteLine($"################################rcv data is error, errorcode:{pkg.axleGenComData[0]} ");
                //    //Console.WriteLine($"################################data buff:{pkg.axleGenComData[2]} ");
                //    string[] hexStrings = pkg.axleGenComData.Select(n => n.ToString("X2")).ToArray();
                //    string result1 = string.Join("-", hexStrings);
                //    Console.WriteLine(result1);  // 
                //    break;
                //}
                //// 按照倍益康艾灸头协议进行组包
                //int curTem = pkg.axleGenComData[6];
                //int targetTem = pkg.axleGenComData[7];
                //int genData1 = pkg.axleGenComData[8] << 8 | pkg.axleGenComData[9];
                //int genData2 = pkg.axleGenComData[10] << 8 | pkg.axleGenComData[11];
                //int genData3 = pkg.axleGenComData[12] << 8 | pkg.axleGenComData[13];
                //int genData4 = pkg.axleGenComData[14] << 8 | pkg.axleGenComData[15];
                //int genData5 = pkg.axleGenComData[16] << 8 | pkg.axleGenComData[17];
                //int genData6 = pkg.axleGenComData[18] << 8 | pkg.axleGenComData[19];

                //Console.WriteLine($"the data is errorcode {errorcode};  datalen  {datalen}  curTem  {curTem}; targetTem  {targetTem}  genData1  {genData1}  genData2  {genData2}  genData3  {genData3}  genData4  {genData4}  genData5  {genData5}  genData6  {genData6}  ");


                //// ----- 力传感器相关 -----
                //// 原始力传感器数据 (Fx,Fy,Fz,Tx,Ty,Tz)
                if (pkg.ft_sensor_raw_data != null && pkg.ft_sensor_raw_data.Length >= 6)
                    Console.WriteLine($"力传感器原始数据(N/Nm): Fx={pkg.ft_sensor_raw_data[0]:F2}, Fy={pkg.ft_sensor_raw_data[1]:F2}, Fz={pkg.ft_sensor_raw_data[2]:F2}, Tx={pkg.ft_sensor_raw_data[3]:F2}, Ty={pkg.ft_sensor_raw_data[4]:F2}, Tz={pkg.ft_sensor_raw_data[5]:F2}");
                else
                    Console.WriteLine("力传感器原始数据: null");

                //// 处理后力传感器数据
                if (pkg.ft_sensor_data != null && pkg.ft_sensor_data.Length >= 6)
                    Console.WriteLine($"力传感器处理数据(N/Nm): Fx={pkg.ft_sensor_data[0]:F2}, Fy={pkg.ft_sensor_data[1]:F2}, Fz={pkg.ft_sensor_data[2]:F2}, Tx={pkg.ft_sensor_data[3]:F2}, Ty={pkg.ft_sensor_data[4]:F2}, Tz={pkg.ft_sensor_data[5]:F2}");
                else
                    Console.WriteLine("力传感器处理数据: null");

                Console.WriteLine($"力传感器激活状态: {pkg.ft_sensor_active}");
                Console.WriteLine($"力传感器错误状态: {pkg.forceSensorErrState}");

                //// ----- 夹爪相关（普通夹爪）-----
                //Console.WriteLine($"夹爪运动完成标志: {pkg.gripper_motiondone}");
                //Console.WriteLine($"夹爪故障ID: {pkg.gripper_fault_id}");
                //Console.WriteLine($"夹爪故障码: {pkg.gripper_fault}");
                //Console.WriteLine($"夹爪激活状态: {pkg.gripper_active}");
                //Console.WriteLine($"夹爪位置: {pkg.gripper_position} (0-255)");
                //Console.WriteLine($"夹爪速度: {pkg.gripper_speed}");
                //Console.WriteLine($"夹爪电流: {pkg.gripper_current} mA");
                //Console.WriteLine($"夹爪温度: {pkg.gripper_temp} °C");
                //Console.WriteLine($"夹爪电压: {pkg.gripper_voltage} mV");

                //// ----- 旋转夹爪相关 -----
                //Console.WriteLine($"旋转夹爪转动圈数: {pkg.gripperRotNum:F2}");
                //Console.WriteLine($"旋转夹爪速度: {pkg.gripperRotSpeed}");
                //Console.WriteLine($"旋转夹爪扭矩: {pkg.gripperRotTorque}");

                //// ----- 智能工具状态 -----
                Console.WriteLine($"smartToolState: {pkg.smartToolState}");

                //// ----- Modbus 通信状态 -----
                //Console.WriteLine($"Modbus主站连接: {pkg.modbusMasterConnect}");
                //Console.WriteLine($"Modbus从站连接: {pkg.modbusSlaveConnect}");

                // 等待一段时间再读取下一帧（8ms周期下，每100ms打印一次即可，避免刷屏过快）
                await Task.Delay(100);
            }

            // 5. 断开连接
            robot.CloseRPC();
            Console.WriteLine("测试结束，RPC 连接已关闭。");
        }

       private async void TestRobotERRStatusStates()
        {
            robot.SetReconnectParam(true, 100, 1000);
            // 2. 配置需要反馈的状态（题目指定的字段组）
            List<RobotState> states = new List<RobotState>
            {
                RobotState.EmergencyStop,
                RobotState.MainCode,
                RobotState.SubCode,
                RobotState.CollisionState,
                RobotState.EndLuaErrCode,
                RobotState.TpdException,
                RobotState.AlarmRebootRobot,
                RobotState.DragAlarm,
                RobotState.SafetyDoorAlarm,
                RobotState.SafetyPlaneAlarm,
                RobotState.MotonAlarm,
                RobotState.InterfaceAlarm,
                RobotState.AlarmCheckEmergStopBtn,
                RobotState.TsTmCmdComError,
                RobotState.TsTmStateComError,
                RobotState.CtrlBoxError,
                RobotState.SafetyDataState,
                RobotState.CtrlOpenLuaErrCode,
                RobotState.StrangePosFlag,
                RobotState.Alarm,
                RobotState.DriverAlarm,
                RobotState.AliveSlaveNumError,
                RobotState.SlaveComError,
                RobotState.CmdPointError,
                RobotState.IOError,
                RobotState.GripperError,
                RobotState.FileError,
                RobotState.ParaError,
                RobotState.ExaxisOutLimitError,
                RobotState.DriverComError,
                RobotState.DriverError,
                RobotState.OutSoftLimitError
            };
            int periodMs = 8;   // 8ms 周期
            int ret = robot.SetRobotRealtimeStateConfig(states, periodMs);
            Console.WriteLine($"配置状态结果: {ret}");

            // 3. 建立 RPC 连接（内部自动连接 CNDE）
            ret = robot.RPC("192.168.58.2");
            if (ret != 0)
            {
                Console.WriteLine($"RPC 连接失败: {ret}");
                return;
            }
            Console.WriteLine("RPC 连接成功，CNDE 已连接，开始打印机器人状态数据...");
            Console.WriteLine("提示：请进行碰撞触发、奇异位姿等操作，观察下方输出变化。");

            // 4. 循环打印 30 秒，每秒一次
            DateTime startTime = DateTime.Now;
            int count = 0;
            while ((DateTime.Now - startTime).TotalSeconds < 30000)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                ret = robot.GetRobotRealTimeState(ref pkg);
                Console.WriteLine($"\n--- 第 {++count} 秒 ---");

               // 打印每个字段
                Console.WriteLine($"  急停标志 (EmergencyStop): {pkg.EmergencyStop}");
                Console.WriteLine($"  主故障码 (main_code): {pkg.main_code}");
                Console.WriteLine($"  子故障码 (sub_code): {pkg.sub_code}");
                Console.WriteLine($"  碰撞检测 (collisionState): {pkg.collisionState}");
                Console.WriteLine($"  末端LUA错误码 (endLuaErrCode): {pkg.endLuaErrCode}");
                Console.WriteLine($"  TPD异常 (tpdException): {pkg.tpdException}");
                Console.WriteLine($"  报警重启机器人 (alarmRebootRobot): {pkg.alarmRebootRobot}");
                Console.WriteLine($"  拖动报警 (dragAlarm): {pkg.dragAlarm}");
                Console.WriteLine($"  安全门报警 (safetyDoorAlarm): {pkg.safetyDoorAlarm}");
                Console.WriteLine($"  安全平面报警 (safetyPlaneAlarm): {pkg.safetyPlaneAlarm}");
                Console.WriteLine($"  运动报警 (motonAlarm): {pkg.motonAlarm}");
                Console.WriteLine($"  干涉区报警 (interfaceAlarm): {pkg.interfaceAlarm}");
                Console.WriteLine($"  急停按钮检查报警 (alarmCheckEmergStopBtn): {pkg.alarmCheckEmergStopBtn}");
                Console.WriteLine($"  TS/TM命令通信错误 (tsTmCmdComError): {pkg.tsTmCmdComError}");
                Console.WriteLine($"  TS/TM状态通信错误 (tsTmStateComError): {pkg.tsTmStateComError}");
                Console.WriteLine($"  控制箱错误 (ctrlBoxError): {pkg.ctrlBoxError}");
                Console.WriteLine($"  安全数据状态 (safetyDataState): {pkg.safetyDataState}");
                Console.WriteLine($"  控制开放Lua错误码 (ctrlOpenLuaErrCode): {(pkg.ctrlOpenLuaErrCode != null ? string.Join(",", pkg.ctrlOpenLuaErrCode) : "")}");
                Console.WriteLine($"  奇异位姿标志 (strangePosFlag): {pkg.strangePosFlag}");
                Console.WriteLine($"  报警标志 (alarm): {pkg.alarm}");
                Console.WriteLine($"  驱动器报警轴号 (driverAlarm): {pkg.driverAlarm}");
                Console.WriteLine($"  活动从站数量错误 (aliveSlaveNumError): {pkg.aliveSlaveNumError}");
                Console.WriteLine($"  从站通信错误 (slaveComError): {(pkg.slaveComError != null ? string.Join(",", pkg.slaveComError) : "")}");
                Console.WriteLine($"  指令点错误 (cmdPointError): {pkg.cmdPointError}");
                Console.WriteLine($"  IO错误 (IOError): {pkg.IOError}");
                Console.WriteLine($"  夹爪错误 (gripperError): {pkg.gripperError}");
                Console.WriteLine($"  文件错误 (fileError): {pkg.fileError}");
                Console.WriteLine($"  参数错误 (paraError): {pkg.paraError}");
                Console.WriteLine($"  扩展轴超出软限位错误 (exaxisOutLimitError): {pkg.exaxisOutLimitError}");
                Console.WriteLine($"  驱动器通信错误 (driverComError): {(pkg.driverComError != null ? string.Join(",", pkg.driverComError) : "")}");
                Console.WriteLine($"  驱动器错误 (driverError): {pkg.driverError}");
                Console.WriteLine($"  超出软限位错误 (outSoftLimitError): {pkg.outSoftLimitError}");
                await Task.Delay(1000); // 每秒打印一次
            }
            // 断开连接
            robot.CloseRPC();
            Console.WriteLine("测试结束。");
        }
        
        public void printCNDE()
        {

            // 循环读取数据（10秒）
            //for (int i = 0; i < 10; i++)
            //{
                Thread.Sleep(1000);
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                int ret = robot.GetRobotRealTimeState(ref pkg);

                Console.WriteLine($"\n--- ---");

            // 基础状态
            Console.WriteLine($"  程序状态: {pkg.program_state}");
                Console.WriteLine($"  机器人状态: {pkg.robot_state}");
                //Console.WriteLine($"  主故障码: {pkg.main_code}");
                //Console.WriteLine($"  子故障码: {pkg.sub_code}");
                Console.WriteLine($"  机器人模式: {pkg.robot_mode}");
            //Console.WriteLine($"  急停: {pkg.EmergencyStop}");
            //Console.WriteLine($"  到位信号: {pkg.motion_done}");
            //Console.WriteLine($"  夹爪运动完成: {pkg.gripper_motiondone}");
            //Console.WriteLine($"  运动队列长度: {pkg.mc_queue_len}");
            //Console.WriteLine($"  碰撞检测: {pkg.collisionState}");
            //Console.WriteLine($"  轨迹点编号: {pkg.trajectory_pnum}");
            Console.WriteLine($"  安全停止0: {pkg.safety_stop0_state}");
                Console.WriteLine($"  安全停止1: {pkg.safety_stop1_state}");
                //Console.WriteLine($"  夹爪故障ID: {pkg.gripper_fault_id}");
                //Console.WriteLine($"  夹爪故障: {pkg.gripper_fault}");
                //Console.WriteLine($"  夹爪激活: {pkg.gripper_active}");
                //Console.WriteLine($"  夹爪位置: {pkg.gripper_position}");
                //Console.WriteLine($"  夹爪速度: {pkg.gripper_speed}");
                //Console.WriteLine($"  夹爪电流: {pkg.gripper_current}");
                //Console.WriteLine($"  夹爪温度: {pkg.gripper_temp}");
                //Console.WriteLine($"  夹爪电压: {pkg.gripper_voltage}");
                Console.WriteLine($"  机器人使能状态: {pkg.rbtEnableState}");
                Console.WriteLine($"  软件升级状态: {pkg.softwareUpgradeState}");
                Console.WriteLine($"  末端LUA错误码: {pkg.endLuaErrCode}");
                Console.WriteLine($"  SmartTool状态: {pkg.smartToolState}");
                //Console.WriteLine($"  宽电压控制箱温度: {pkg.wideVoltageCtrlBoxTemp}");
                //Console.WriteLine($"  宽电压控制箱风扇电流: {pkg.wideVoltageCtrlBoxFanVel}");
                Console.WriteLine($"  负载质量: {pkg.load}");

                // 关节数据
                if (pkg.jt_cur_pos != null && pkg.jt_cur_pos.Length >= 6)
                {
                    Console.WriteLine($"  关节位置(°): J1={pkg.jt_cur_pos[0]:F2}, J2={pkg.jt_cur_pos[1]:F2}, J3={pkg.jt_cur_pos[2]:F2}, J4={pkg.jt_cur_pos[3]:F2}, J5={pkg.jt_cur_pos[4]:F2}, J6={pkg.jt_cur_pos[5]:F2}");
                }
                if (pkg.jt_cur_tor != null && pkg.jt_cur_tor.Length >= 6)
                {
                    Console.WriteLine($"  关节扭矩(Nm): J1={pkg.jt_cur_tor[0]:F2}, J2={pkg.jt_cur_tor[1]:F2}, J3={pkg.jt_cur_tor[2]:F2}, J4={pkg.jt_cur_tor[3]:F2}, J5={pkg.jt_cur_tor[4]:F2}, J6={pkg.jt_cur_tor[5]:F2}");
                }
                if (pkg.actual_qd != null && pkg.actual_qd.Length >= 6)
                {
                    Console.WriteLine($"  关节速度(°/s): J1={pkg.actual_qd[0]:F2}, J2={pkg.actual_qd[1]:F2}, J3={pkg.actual_qd[2]:F2}, J4={pkg.actual_qd[3]:F2}, J5={pkg.actual_qd[4]:F2}, J6={pkg.actual_qd[5]:F2}");
                }
                if (pkg.actual_qdd != null && pkg.actual_qdd.Length >= 6)
                {
                    Console.WriteLine($"  关节加速度(°/s²): J1={pkg.actual_qdd[0]:F2}, J2={pkg.actual_qdd[1]:F2}, J3={pkg.actual_qdd[2]:F2}, J4={pkg.actual_qdd[3]:F2}, J5={pkg.actual_qdd[4]:F2}, J6={pkg.actual_qdd[5]:F2}");
                }
                if (pkg.jt_tgt_tor != null && pkg.jt_tgt_tor.Length >= 6)
                {
                    Console.WriteLine($"  目标关节扭矩(Nm): J1={pkg.jt_tgt_tor[0]:F2}, J2={pkg.jt_tgt_tor[1]:F2}, J3={pkg.jt_tgt_tor[2]:F2}, J4={pkg.jt_tgt_tor[3]:F2}, J5={pkg.jt_tgt_tor[4]:F2}, J6={pkg.jt_tgt_tor[5]:F2}");
                }
                //if (pkg.jointDriverTorque != null && pkg.jointDriverTorque.Length >= 6)
                //{
                //    Console.WriteLine($"  驱动器扭矩: J1={pkg.jointDriverTorque[0]:F2}, J2={pkg.jointDriverTorque[1]:F2}, J3={pkg.jointDriverTorque[2]:F2}, J4={pkg.jointDriverTorque[3]:F2}, J5={pkg.jointDriverTorque[4]:F2}, J6={pkg.jointDriverTorque[5]:F2}");
                //}
                //if (pkg.jointDriverTemperature != null && pkg.jointDriverTemperature.Length >= 6)
                //{
                //    Console.WriteLine($"  驱动器温度(℃): J1={pkg.jointDriverTemperature[0]:F2}, J2={pkg.jointDriverTemperature[1]:F2}, J3={pkg.jointDriverTemperature[2]:F2}, J4={pkg.jointDriverTemperature[3]:F2}, J5={pkg.jointDriverTemperature[4]:F2}, J6={pkg.jointDriverTemperature[5]:F2}");
                //}

                // TCP/工具位姿
                if (pkg.tl_cur_pos != null && pkg.tl_cur_pos.Length >= 6)
                {
                    Console.WriteLine($"  工具位姿: X={pkg.tl_cur_pos[0]:F2}mm, Y={pkg.tl_cur_pos[1]:F2}mm, Z={pkg.tl_cur_pos[2]:F2}mm, RX={pkg.tl_cur_pos[3]:F2}°, RY={pkg.tl_cur_pos[4]:F2}°, RZ={pkg.tl_cur_pos[5]:F2}°");
                }
                if (pkg.flange_cur_pos != null && pkg.flange_cur_pos.Length >= 6)
                {
                    Console.WriteLine($"  法兰位姿: X={pkg.flange_cur_pos[0]:F2}mm, Y={pkg.flange_cur_pos[1]:F2}mm, Z={pkg.flange_cur_pos[2]:F2}mm, RX={pkg.flange_cur_pos[3]:F2}°, RY={pkg.flange_cur_pos[4]:F2}°, RZ={pkg.flange_cur_pos[5]:F2}°");
                }
                if (pkg.target_TCP_Speed != null && pkg.target_TCP_Speed.Length >= 6)
                {
                    Console.WriteLine($"  目标TCP速度: Vx={pkg.target_TCP_Speed[0]:F2}mm/s, Vy={pkg.target_TCP_Speed[1]:F2}, Vz={pkg.target_TCP_Speed[2]:F2}, Rx={pkg.target_TCP_Speed[3]:F2}°/s, Ry={pkg.target_TCP_Speed[4]:F2}, Rz={pkg.target_TCP_Speed[5]:F2}");
                }
                if (pkg.actual_TCP_Speed != null && pkg.actual_TCP_Speed.Length >= 6)
                {
                    Console.WriteLine($"  实际TCP速度: Vx={pkg.actual_TCP_Speed[0]:F2}mm/s, Vy={pkg.actual_TCP_Speed[1]:F2}, Vz={pkg.actual_TCP_Speed[2]:F2}, Rx={pkg.actual_TCP_Speed[3]:F2}°/s, Ry={pkg.actual_TCP_Speed[4]:F2}, Rz={pkg.actual_TCP_Speed[5]:F2}");
                }
                if (pkg.target_TCP_CmpSpeed != null && pkg.target_TCP_CmpSpeed.Length >= 2)
                {
                    Console.WriteLine($"  目标TCP合成速度: 位置={pkg.target_TCP_CmpSpeed[0]:F2}mm/s, 姿态={pkg.target_TCP_CmpSpeed[1]:F2}°/s");
                }
                if (pkg.actual_TCP_CmpSpeed != null && pkg.actual_TCP_CmpSpeed.Length >= 2)
                {
                    Console.WriteLine($"  实际TCP合成速度: 位置={pkg.actual_TCP_CmpSpeed[0]:F2}mm/s, 姿态={pkg.actual_TCP_CmpSpeed[1]:F2}°/s");
                }

                // IO
                Console.WriteLine($"  控制箱DO高8位: 0x{pkg.cl_dgt_output_h:X2}, 低8位: 0x{pkg.cl_dgt_output_l:X2}");
                Console.WriteLine($"  工具DO: 0x{pkg.tl_dgt_output_l:X2}");
                Console.WriteLine($"  控制箱DI高8位: 0x{pkg.cl_dgt_input_h:X2}, 低8位: 0x{pkg.cl_dgt_input_l:X2}");
                Console.WriteLine($"  工具DI: 0x{pkg.tl_dgt_input_l:X2}");
                if (pkg.cl_analog_input != null && pkg.cl_analog_input.Length >= 2)
                {
                    Console.WriteLine($"  控制箱模拟输入: AI0={pkg.cl_analog_input[0]}, AI1={pkg.cl_analog_input[1]}");
                }
                Console.WriteLine($"  工具模拟输入: {pkg.tl_anglog_input}");
                if (pkg.cl_analog_output != null && pkg.cl_analog_output.Length >= 2)
                {
                    Console.WriteLine($"  控制箱模拟输出: AO0={pkg.cl_analog_output[0]}, AO1={pkg.cl_analog_output[1]}");
                }
                Console.WriteLine($"  工具模拟输出: {pkg.tl_analog_output}");

                // 力传感器
                //if (pkg.ft_sensor_data != null && pkg.ft_sensor_data.Length >= 6)
                //{
                //    Console.WriteLine($"  力传感器数据(N/Nm): Fx={pkg.ft_sensor_data[0]:F2}, Fy={pkg.ft_sensor_data[1]:F2}, Fz={pkg.ft_sensor_data[2]:F2}, Tx={pkg.ft_sensor_data[3]:F2}, Ty={pkg.ft_sensor_data[4]:F2}, Tz={pkg.ft_sensor_data[5]:F2}");
                //}
                //Console.WriteLine($"  力传感器激活: {pkg.ft_sensor_active}");

                // 扩展IO
                //if (pkg.extDIState != null && pkg.extDIState.Length >= 8)
                //{
                //    Console.WriteLine($"  扩展DI: {string.Join(",", pkg.extDIState)}");
                //}
                //if (pkg.extDOState != null && pkg.extDOState.Length >= 8)
                //{
                //    Console.WriteLine($"  扩展DO: {string.Join(",", pkg.extDOState)}");
                //}
                //if (pkg.extAIState != null && pkg.extAIState.Length >= 4)
                //{
                //    Console.WriteLine($"  扩展AI: {string.Join(",", pkg.extAIState)}");
                //}
                //if (pkg.extAOState != null && pkg.extAOState.Length >= 4)
                //{
                //    Console.WriteLine($"  扩展AO: {string.Join(",", pkg.extAOState)}");
                //}

                //// 旋转夹爪
                //Console.WriteLine($"  旋转夹爪圈数: {pkg.gripperRotNum}");
                //Console.WriteLine($"  旋转夹爪速度: {pkg.gripperRotSpeed}%");
                //Console.WriteLine($"  旋转夹爪力矩: {pkg.gripperRotTorque}%");

                //// 焊接
                //Console.WriteLine($"  焊接中断状态: {pkg.weldingBreakOffState.breakOffState}, 起弧状态: {pkg.weldingBreakOffState.weldArcState}");

                // 工具/工件坐标系
                if (pkg.toolCoord != null && pkg.toolCoord.Length >= 6)
                {
                    Console.WriteLine($"  工具坐标系: X={pkg.toolCoord[0]:F2}, Y={pkg.toolCoord[1]:F2}, Z={pkg.toolCoord[2]:F2}, RX={pkg.toolCoord[3]:F2}, RY={pkg.toolCoord[4]:F2}, RZ={pkg.toolCoord[5]:F2}");
                }
                if (pkg.wobjCoord != null && pkg.wobjCoord.Length >= 6)
                {
                    Console.WriteLine($"  工件坐标系: X={pkg.wobjCoord[0]:F2}, Y={pkg.wobjCoord[1]:F2}, Z={pkg.wobjCoord[2]:F2}, RX={pkg.wobjCoord[3]:F2}, RY={pkg.wobjCoord[4]:F2}, RZ={pkg.wobjCoord[5]:F2}");
                }
                if (pkg.extoolCoord != null && pkg.extoolCoord.Length >= 6)
                {
                    Console.WriteLine($"  外部工具坐标系: X={pkg.extoolCoord[0]:F2}, Y={pkg.extoolCoord[1]:F2}, Z={pkg.extoolCoord[2]:F2}, RX={pkg.extoolCoord[3]:F2}, RY={pkg.extoolCoord[4]:F2}, RZ={pkg.extoolCoord[5]:F2}");
                }
                if (pkg.exAxisCoord != null && pkg.exAxisCoord.Length >= 6)
                {
                    Console.WriteLine($"  扩展轴坐标系: X={pkg.exAxisCoord[0]:F2}, Y={pkg.exAxisCoord[1]:F2}, Z={pkg.exAxisCoord[2]:F2}, RX={pkg.exAxisCoord[3]:F2}, RY={pkg.exAxisCoord[4]:F2}, RZ={pkg.exAxisCoord[5]:F2}");
                }

                // 负载质心
                if (pkg.loadCog != null && pkg.loadCog.Length >= 3)
                {
                    Console.WriteLine($"  负载质心(mm): X={pkg.loadCog[0]:F2}, Y={pkg.loadCog[1]:F2}, Z={pkg.loadCog[2]:F2}");
                }

                // 最后一个ServoJ目标
                if (pkg.lastServoTarget != null && pkg.lastServoTarget.Length >= 6)
                {
                    Console.WriteLine($"  最后一个ServoJ目标(°): J1={pkg.lastServoTarget[0]:F2}, J2={pkg.lastServoTarget[1]:F2}, J3={pkg.lastServoTarget[2]:F2}, J4={pkg.lastServoTarget[3]:F2}, J5={pkg.lastServoTarget[4]:F2}, J6={pkg.lastServoTarget[5]:F2}");
                }

                // 系统时间
                Console.WriteLine($"  机器人时间: {pkg.robotTime.ToString()}");

                // ========== 新增状态（按映射表第二列变量名）==========
                //// 目标关节数据
                //if (pkg.targetJointPos != null && pkg.targetJointPos.Length >= 6)
                //    Console.WriteLine($"  目标关节位置(°): J1={pkg.targetJointPos[0]:F2}, J2={pkg.targetJointPos[1]:F2}, J3={pkg.targetJointPos[2]:F2}, J4={pkg.targetJointPos[3]:F2}, J5={pkg.targetJointPos[4]:F2}, J6={pkg.targetJointPos[5]:F2}");
                //if (pkg.targetJointVel != null && pkg.targetJointVel.Length >= 6)
                //    Console.WriteLine($"  目标关节速度(°/s): J1={pkg.targetJointVel[0]:F2}, J2={pkg.targetJointVel[1]:F2}, J3={pkg.targetJointVel[2]:F2}, J4={pkg.targetJointVel[3]:F2}, J5={pkg.targetJointVel[4]:F2}, J6={pkg.targetJointVel[5]:F2}");
                //if (pkg.targetJointAcc != null && pkg.targetJointAcc.Length >= 6)
                //    Console.WriteLine($"  目标关节加速度(°/s²): J1={pkg.targetJointAcc[0]:F2}, J2={pkg.targetJointAcc[1]:F2}, J3={pkg.targetJointAcc[2]:F2}, J4={pkg.targetJointAcc[3]:F2}, J5={pkg.targetJointAcc[4]:F2}, J6={pkg.targetJointAcc[5]:F2}");
                //if (pkg.targetJointCurrent != null && pkg.targetJointCurrent.Length >= 6)
                //    Console.WriteLine($"  目标关节电流(A): J1={pkg.targetJointCurrent[0]:F2}, J2={pkg.targetJointCurrent[1]:F2}, J3={pkg.targetJointCurrent[2]:F2}, J4={pkg.targetJointCurrent[3]:F2}, J5={pkg.targetJointCurrent[4]:F2}, J6={pkg.targetJointCurrent[5]:F2}");
                //if (pkg.actualJointCurrent != null && pkg.actualJointCurrent.Length >= 6)
                //    Console.WriteLine($"  实际关节电流(A): J1={pkg.actualJointCurrent[0]:F2}, J2={pkg.actualJointCurrent[1]:F2}, J3={pkg.actualJointCurrent[2]:F2}, J4={pkg.actualJointCurrent[3]:F2}, J5={pkg.actualJointCurrent[4]:F2}, J6={pkg.actualJointCurrent[5]:F2}");
                //if (pkg.actualTCPForce != null && pkg.actualTCPForce.Length >= 6)
                //    Console.WriteLine($"  实际TCP力(N/Nm): Fx={pkg.actualTCPForce[0]:F2}, Fy={pkg.actualTCPForce[1]:F2}, Fz={pkg.actualTCPForce[2]:F2}, Tx={pkg.actualTCPForce[3]:F2}, Ty={pkg.actualTCPForce[4]:F2}, Tz={pkg.actualTCPForce[5]:F2}");
                //if (pkg.targetTCPPos != null && pkg.targetTCPPos.Length >= 6)
                //    Console.WriteLine($"  目标TCP位置(mm): X={pkg.targetTCPPos[0]:F2}, Y={pkg.targetTCPPos[1]:F2}, Z={pkg.targetTCPPos[2]:F2}, RX={pkg.targetTCPPos[3]:F2}, RY={pkg.targetTCPPos[4]:F2}, RZ={pkg.targetTCPPos[5]:F2}");
                //if (pkg.collisionLevel != null && pkg.collisionLevel.Length >= 6)
                //    Console.WriteLine($"  碰撞等级: J1={pkg.collisionLevel[0]}, J2={pkg.collisionLevel[1]}, J3={pkg.collisionLevel[2]}, J4={pkg.collisionLevel[3]}, J5={pkg.collisionLevel[4]}, J6={pkg.collisionLevel[5]}");
                //Console.WriteLine($"  手动模式速度百分比: {pkg.speedScaleManual:F2}%");
                //Console.WriteLine($"  自动模式速度百分比: {pkg.speedScaleAuto:F2}%");
                //Console.WriteLine($"  程序行号: {pkg.luaLineNum}");
                //Console.WriteLine($"  异常停止标志: {pkg.abnomalStop}");
                //string curLuaFileName = pkg.currentLuaFileName != null ? System.Text.Encoding.ASCII.GetString(pkg.currentLuaFileName).TrimEnd('\0') : "";
                //Console.WriteLine($"  当前Lua文件名: {curLuaFileName}");
                //Console.WriteLine($"  程序总行数: {pkg.programTotalLine}");
                //if (pkg.safetyBoxSingal != null && pkg.safetyBoxSingal.Length >= 6)
                //    Console.WriteLine($"  安全盒信号: [{string.Join(",", pkg.safetyBoxSingal)}]");
                //Console.WriteLine($"  焊接电压: {pkg.weldVoltage:F2} V");
                //Console.WriteLine($"  焊接电流: {pkg.weldCurrent:F2} A");
                //Console.WriteLine($"  焊缝跟踪速度: {pkg.weldTrackVel:F2} mm/s");
                //Console.WriteLine($"  TPD异常: {pkg.tpdException}");
                //Console.WriteLine($"  报警重启机器人: {pkg.alarmRebootRobot}");
                //Console.WriteLine($"  Modbus主站连接: {pkg.modbusMasterConnect}");
                //Console.WriteLine($"  Modbus从站连接: {pkg.modbusSlaveConnect}");
                //Console.WriteLine($"  按钮盒停止信号: {pkg.btnBoxStopSignal}");
                //Console.WriteLine($"  拖动报警: {pkg.dragAlarm}");
                //Console.WriteLine($"  安全门报警: {pkg.safetyDoorAlarm}");
                //Console.WriteLine($"  安全平面报警: {pkg.safetyPlaneAlarm}");
                //Console.WriteLine($"  运动报警: {pkg.motonAlarm}");
                //Console.WriteLine($"  干涉区报警: {pkg.interfaceAlarm}");
                //Console.WriteLine($"  UDP命令状态: {pkg.udpCmdState}");
                //Console.WriteLine($"  焊接就绪状态: {pkg.weldReadyState}");
                //Console.WriteLine($"  急停按钮检查报警: {pkg.alarmCheckEmergStopBtn}");
                //Console.WriteLine($"  TS/TM命令通信错误: {pkg.tsTmCmdComError}");
                //Console.WriteLine($"  TS/TM状态通信错误: {pkg.tsTmStateComError}");
                //Console.WriteLine($"  控制箱错误: {pkg.ctrlBoxError}");
                //Console.WriteLine($"  安全数据状态: {pkg.safetyDataState}");
                //Console.WriteLine($"  力传感器错误状态: {pkg.forceSensorErrState}");
                //if (pkg.ctrlOpenLuaErrCode != null && pkg.ctrlOpenLuaErrCode.Length >= 4)
                //    Console.WriteLine($"  控制开放Lua错误码: [{string.Join(",", pkg.ctrlOpenLuaErrCode)}]");
                //Console.WriteLine($"  奇异位姿标志: {pkg.strangePosFlag}");
                //Console.WriteLine($"  报警标志: {pkg.alarm}");
                //Console.WriteLine($"  驱动器报警轴号: {pkg.driverAlarm}");
                //Console.WriteLine($"  活动从站数量错误: {pkg.aliveSlaveNumError}");
                //if (pkg.slaveComError != null && pkg.slaveComError.Length >= 8)
                //    Console.WriteLine($"  从站通信错误: [{string.Join(",", pkg.slaveComError)}]");
                //Console.WriteLine($"  指令点错误: {pkg.cmdPointError}");
                //Console.WriteLine($"  IO错误: {pkg.IOError}");
                //Console.WriteLine($"  夹爪错误: {pkg.gripperError}");
                //Console.WriteLine($"  文件错误: {pkg.fileError}");
                //Console.WriteLine($"  参数错误: {pkg.paraError}");
                //Console.WriteLine($"  扩展轴超出软限位错误: {pkg.exaxisOutLimitError}");
                //if (pkg.driverComError != null && pkg.driverComError.Length >= 6)
                //    Console.WriteLine($"  驱动器通信错误: [{string.Join(",", pkg.driverComError)}]");
                //Console.WriteLine($"  驱动器错误: {pkg.driverError}");
                //Console.WriteLine($"  超出软限位错误: {pkg.outSoftLimitError}");
                //if (pkg.axleGenComData != null && pkg.axleGenComData.Length > 0)
                //    Console.WriteLine($"  轴通用通信数据(前16字节): {BitConverter.ToString(pkg.axleGenComData, 0, Math.Min(16, pkg.axleGenComData.Length))}");
                //Console.WriteLine($"  和校验: {pkg.check_sum}");

            //}

            // 断开连接
            //robot.DisconnectCNDE();
            //Console.WriteLine("测试结束");
        }


    }
}



//ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
//robot.GetRobotRealTimeState(ref pkg);
//Console.WriteLine($"cur robot time is {pkg.robotTime.ToString()}");

//while(true)
//{
//    int state = 0;
//    robot.GetSoftwareUpgradeState(ref state);
//    Console.WriteLine($"robot Upgrade state is {state}");
//    if(state == 100)
//    {
//        break;
//    }
//    Thread.Sleep(500);
//}
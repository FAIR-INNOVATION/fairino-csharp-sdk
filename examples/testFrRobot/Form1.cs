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
            robot.LoggerInit(FrLogType.BUFFER, FrLogLevel.ERROR, path, 5, 5);
            robot.SetLoggerLevel(FrLogLevel.INFO);
            robot.SetReconnectParam(true, 100, 1000);//断线重连参数
            rrpc = robot.RPC("192.168.58.2"); //与控制箱建立连接
                                              //20004端口接收超时时间
                                              //robot.SetReceivePortTimeout(40);

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
            robot.CloseRPC();
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
            JointPos j1= new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            JointPos j2 = new JointPos(-45.615, -106.172, 124.296, -107.151, -91.282, 74.255);
            JointPos j3 = new JointPos(-29.777, -84.536, 109.275, -114.075, -86.655, 74.257);
            JointPos j4 = new JointPos(-31.154, -95.317, 94.276, -88.079, -89.740, 74.256);
            DescPose desc_pos1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose desc_pos2 = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);
            DescPose desc_pos3 = new DescPose(-487.434, 154.362, 308.576, 176.600, 0.268, -14.061);
            DescPose desc_pos4 = new DescPose(-443.165, 147.881, 480.951, 179.511, -0.775, -15.409);
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

            robot.SetSpeed(20);
            int rtn;
            rtn = robot.MoveJ(j1, desc_pos1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            Console.WriteLine($"MoveJ errcode:{rtn}" );

            rtn = robot.MoveL(j2, desc_pos2, tool, user, vel, acc, ovl, blendR,epos, search, flag, offset_pos);
            Console.WriteLine($"MoveL errcode:{rtn}");

            rtn = robot.MoveC(j3, desc_pos3, tool, user, vel, acc, epos, flag, offset_pos, j4, desc_pos4, tool, user, vel, acc, epos, flag, offset_pos, ovl, blendR);
            Console.WriteLine($"MoveC errcode:{rtn}");

            rtn = robot.MoveJ(j2, desc_pos2, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            Console.WriteLine("MoveJ errcode:%d\n", rtn);

            rtn = robot.Circle(j3, desc_pos3, tool, user, vel, acc, epos, j1, desc_pos1, tool, user, vel, acc, epos, ovl, flag, offset_pos);
            Console.WriteLine($"Circle errcode:{rtn}");

            rtn = robot.MoveCart(desc_pos4, tool, user, vel, acc, ovl, blendT, -1);
            Console.WriteLine($"MoveCart errcode:{rtn}");

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

            rtn = robot.NewSpiral(j, desc_pos, tool, user, vel, acc, epos, ovl, flag, offset_pos2, sp);
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
                robot.ServoCart(mode, desc_pos_dt, pos_gain, acc, vel, cmdT, filterT, gain);
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
            int resetFlag = 0;
            int rtn = robot.SetOutputResetCtlBoxDO(resetFlag);
            robot.SetOutputResetCtlBoxAO(resetFlag);
            robot.SetOutputResetAxleDO(resetFlag);
            robot.SetOutputResetAxleAO(resetFlag);
            robot.SetOutputResetExtDO(resetFlag);
            robot.SetOutputResetExtAO(resetFlag);
            robot.SetOutputResetSmartToolDO(resetFlag);

            //robot.SetDO(1, 1, 0, 0);
            //robot.SetDO(3, 1, 0, 0);
            //robot.SetAO(0, 50, 0);
            //robot.SetAO(1, 70, 0);
            //robot.SetToolDO(1, 1, 0, 0);
            //robot.SetToolAO(0, 40, 0);

            robot.SetAuxDO(1, true, false, true);
            robot.SetAuxDO(2, true, false, true);

            robot.SetAuxAO(0, 1024, false);
            robot.SetAuxAO(1, 2048, false);
            return ;













            //int rtn = robot.WaitMultiDI(0, 768, 768, 10000, 0);
            //Console.WriteLine(rtn);

            //int[] DIConfig = new int[8] { 1, 26, 3, 4, 0, 0, 0, 0};
            //int[] DILevelConfig = new int[8] { 0, 1, 0, 1, 0, 1, 0, 1};
            //robot.SetDIConfigLevel(DILevelConfig);
            //robot.SetDIConfig(DIConfig);
            //robot.WaitAI(0, 0, 0.53f, 1000, 2);
            //float fff = 0.0f;
            //for (int j = 0; j < 100; j++)
            //{
            //    robot.GetAI(0, 0, ref fff);
            //    Console.WriteLine($"the ai is {fff}");
            //    Thread.Sleep(100);
            //}
            //return;

            byte status = 1;
            byte smooth = 0;
            byte block = 0;
            byte di = 0, tool_di = 0;
            float ai = 0.0f, tool_ai = 0.0f;
            float value = 0.0f;
            int doH = 0;
            int doL = 0;
            int i;
            rtn = 0;
            for (i = 0; i < 16; i++)//所有控制器IO输出置 1
            {
                rtn = robot.SetDO(i, status, smooth, block);
                robot.WaitMs(500);

                robot.GetDO(ref doH, ref doL);
                Console.WriteLine($"setDO  {i}: {rtn}  getDO {doH}  {doL}");
            }

            status = 0;

            for (i = 0; i < 16; i++)//所有控制器IO输出置 0
            {
                robot.SetDO(i, status, smooth, block);
                robot.WaitMs(500);
                robot.GetDO(ref doH, ref doL);
                Console.WriteLine($"setDO  {i}: {rtn}  getDO {doH}  {doL}");
            }

            status = 1;

            for (i = 0; i < 2; i++)//所有工具端IO输出置 1
            {
                robot.SetToolDO(i, status, smooth, block);
                robot.WaitMs(500);
            }

            status = 0;

            for (i = 0; i < 2; i++)//所有工具端IO输出置 0
            {
                robot.SetToolDO(i, status, smooth, block);
                robot.WaitMs(500);
            }

            value = 50.0f;
            robot.SetAO(0, value, block);//设置控制器0号模拟量输出50%
            value = 100.0f;
            robot.SetAO(1, value, block);//设置控制器1号模拟量输出100%
            robot.WaitMs(300);
            value = 0.0f;
            robot.SetAO(0, value, block);//设置控制器0号模拟量输出0%
            value = 0.0f;
            robot.SetAO(1, value, block);//设置控制器1号模拟量输出0%

            value = 100.0f;
            robot.SetToolAO(0, value, block);//设置工具端0号模拟量输出100%
            robot.WaitMs(1000);
            value = 0.0f;
            robot.SetToolAO(0, value, block);//设置工具端0号模拟量输出0%

            robot.GetDI(0, block, ref di);//获取数字输入
            Console.WriteLine($"di0 : {di}");
            robot.WaitDI(0, 1, 0, 2);       //等待0号端口数字量输入1，一直等待
            Console.WriteLine("wait di success");
            robot.WaitMultiDI(0, 3, 0, 10000, 2);   //等待多路与， 0和1端口，输入置1，等待时间10000ms， 一直等待
            Console.WriteLine("wait multi di success");
            robot.GetToolDI(1, block, ref tool_di);//获取工具端数字量输入
            Console.WriteLine($"tool_di1 : {tool_di}");
            robot.WaitToolDI(1, 0, 0, 2);          //一直等待
            Console.WriteLine("wait tool di success");
            robot.GetAI(0, block, ref ai);
            Console.WriteLine($"ai0 : {ai}");
            robot.GetAI(1, block, ref ai);
            Console.WriteLine($"ai1 : {ai}");
            robot.WaitAI(0, 1, 50.0f, 0, 2);           //等待0号口， 小于 ， %50， 一直等待
            Console.WriteLine("wait ai success");
            robot.WaitToolAI(0, 1, 50, 0, 2);       //一直等待
            Console.WriteLine("wait tool ai success");
            robot.GetToolAI(0, block, ref tool_ai);
            Console.WriteLine($"tool_ai0 : {tool_ai}");
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
            int rtn = robot.FT_Control(flag, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
            Console.WriteLine($"FT_Control start rtn {rtn}");

            robot.MoveL(j2, desc_p2, 1, 0, 100.0f, 180.0f, 20.0f, -1.0f, 0, epos, 0, 0, offset_pos);
            flag = 0;
            rtn = robot.FT_Control(flag, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
            Console.WriteLine($"FT_Control end rtn {rtn}");
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
    robot.FT_Control(flag, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
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
    robot.FT_Control(flag, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
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
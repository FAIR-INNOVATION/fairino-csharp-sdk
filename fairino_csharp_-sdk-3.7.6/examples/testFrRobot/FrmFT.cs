using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using fairino;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace testFrRobot
{
    public partial class FrmFT : Form
    {
        Robot robot;
        public FrmFT(Robot ro)
        {
            InitializeComponent();
            robot = ro;
        }

        private void btnDragStart_Click(object sender, EventArgs e)
        {
            double[] M = new double[6] { 15.0, 15.0, 15.0, 0.5, 0.5, 0.1};
            double[] B = new double[6] { 150, 150, 150, 5.0, 5.0, 1.0 };
            double[] K = new double[6] { 0, 0, 0, 0, 0, 0 };
            double[] F = new double[6] { 10, 10, 10, 1, 1, 1 };
            robot.EndForceDragControl(1, 0, 0, M, B, K, F, 50, 180);
        }

        private void btnStopDrag_Click(object sender, EventArgs e)
        {
            double[] M = new double[6] { 15.0, 15.0, 15.0, 0.5, 0.5, 0.1 };
            double[] B = new double[6] { 150, 150, 150, 5.0, 5.0, 1.0 };
            double[] K = new double[6] { 0, 0, 0, 0, 0, 0 };
            double[] F = new double[6] { 10, 10, 10, 1, 1, 1 };
            robot.EndForceDragControl(0, 0, 0, M, B, K, F, 50, 100);
        }

        private void btnSixStart_Click(object sender, EventArgs e)
        {
            robot.DragTeachSwitch(1);
            double[] lamdeDain = new double[6] { 3.0, 2.0, 2.0, 2.0, 2.0, 3.0 };
            double[] KGain = new double[6] { 0, 0, 0, 0, 0, 0 };
            double[] BGain = new double[6] { 150, 150, 150, 5.0, 5.0, 1.0 };
            robot.ForceAndJointImpedanceStartStop(1, 0, lamdeDain, KGain, BGain, 1000, 180);
        }

        private void btnSixEnd_Click(object sender, EventArgs e)
        {
            robot.DragTeachSwitch(0);
            double[] lamdeDain = new double[6] { 3.0, 2.0, 2.0, 2.0, 2.0, 3.0 };
            double[] KGain = new double[6] { 0, 0, 0, 0, 0, 0 };
            double[] BGain = new double[6] { 150, 150, 150, 5.0, 5.0, 1.0 };
            robot.ForceAndJointImpedanceStartStop(0, 0, lamdeDain, KGain, BGain, 1000, 180);
        }

        private void btnGetDragState_Click(object sender, EventArgs e)
        {
            int draga = 0;
            int dragb = 0;
            robot.GetForceAndTorqueDragState(ref draga, ref dragb);
            Console.WriteLine($"robot drag state is {draga}  {dragb}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            robot.SetForceSensorPayLoad(0.824);
            robot.SetForceSensorPayLoadCog(0.778, 2.554, 48.765);
            double weight = 0;
            double x = 0, y = 0, z = 0;
            robot.GetForceSensorPayLoad(ref weight);
            robot.GetForceSensorPayLoadCog(ref x, ref y, ref z);
            Console.WriteLine($"the FT load is {weight} {x} {y} {z}");
        }

        private void btnFtAutoZero_Click(object sender, EventArgs e)
        {
            robot.SetForceSensorPayLoad(0);
            robot.SetForceSensorPayLoadCog(0, 0, 0);

            JointPos posqq = new JointPos(0, 0, 0, 0, 0, 0);
            robot.GetActualJointPosDegree(0, ref posqq);
            Console.WriteLine($"the joint 1 is {posqq.jPos[0]}");

            double weight = 0;
            DescTran pos = new DescTran(0, 0, 0);
            robot.ForceSensorAutoComputeLoad(ref weight, ref pos);
            Console.WriteLine($"the FT value is {weight} {pos.x} {pos.y} {pos.z}");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            robot.DragTeachSwitch(1);
            robot.SetPowerLimit(0, 2);
            double[] torques = { 0, 0, 0, 0, 0, 0 };
            robot.GetJointTorques(1, torques);

            int count = 100;
            robot.ServoJTStart(); //   #servoJT开始
            int error = 0;
            while (count > 0)
            {
                torques[0] = torques[0] + 0.1;//  #每次1轴增加0.1NM，运动100次
                error = robot.ServoJT(torques, 0.001);  //# 关节空间伺服模式运动
                count = count - 1;
                Thread.Sleep(1);
            }

            error = robot.ServoJTEnd();  //#伺服运动结束
        }

        private void btnStaticStart_Click(object sender, EventArgs e)
        {
            robot.SetCollisionDetectionMethod(1);
            //robot.SetStaticCollisionOnOff(1);
        }

        private void btnStaticEnd_Click(object sender, EventArgs e)
        {
            robot.SetStaticCollisionOnOff(0);
        }

        private void btnPowerStart_Click(object sender, EventArgs e)
        {
            robot.DragTeachSwitch(1);
            robot.SetPowerLimit(1, 2);
            double[] torques = { 0, 0, 0, 0, 0, 0 };
            robot.GetJointTorques(1, torques);

            int count = 100;
            robot.ServoJTStart(); //   #servoJT开始
            int error = 0;
            while (count > 0)
            {
                torques[0] = torques[0] + 0.1;//  #每次1轴增加0.1NM，运动100次
                error = robot.ServoJT(torques, 0.001);  //# 关节空间伺服模式运动
                count = count - 1;
                Thread.Sleep(1);
            }

            error = robot.ServoJTEnd();  //#伺服运动结束
        }

        private void FrmFT_Load(object sender, EventArgs e)
        {

        }

        private void btnAutoOnFT_Click(object sender, EventArgs e)
        {
            robot.SetForceSensorDragAutoFlag(1);
        }

        private void btnAutoCloseFT_Click(object sender, EventArgs e)
        {
            robot.SetForceSensorDragAutoFlag(0);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            robot.AxleSensorConfig(18, 0, 0, 1);
            int company = -1;
            int type = -1;
            robot.AxleSensorConfigGet(ref company, ref type);
            Console.WriteLine($"company is {company}, type is {type}");

            robot.AxleSensorActivate(1);

            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //robot.LuaDelete("/fruser/ytrtyuytrtyuj.lua");
            //robot.ProgramLoad("/fruser/ytrtyuytrtyuj.lua");
            //robot.ProgramRun();
            //return;
            string err = "";
            while(true)
            {
                int rtn = robot.LuaUpload("D://zUP/333.lua", ref err);
                Console.WriteLine($"errcode is {rtn} errstr is {err}");
                Thread.Sleep(3000);
            }
            
        }

        private void btn_RegWritre_Click(object sender, EventArgs e)
        {
            while(true)
            {
                robot.AxleSensorRegWrite(1, 4, 6, 1, 0, 0, 0);
            }
            
        }

        private void btnEndLuaDrag_Click(object sender, EventArgs e)
        {
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
            robot.SetAxleCommunicationParam(7, 8, 1, 0, 5, 3, 1);

            int baudRate = 0, dataBit = 0, stopBit = 0, verify = 0, timeout = 0, timeoutTimes = 0, period = 0;
            robot.GetAxleCommunicationParam(ref baudRate, ref dataBit, ref stopBit, ref verify, ref timeout, ref timeoutTimes, ref period);

            robot.SetAxleLuaEnable(1);
            int luaEnableStatus = 0;
            robot.GetAxleLuaEnableStatus(ref luaEnableStatus);
            robot.SetAxleLuaEnableDeviceType(1, 0, 0);
            int forceType = 0;
            int gripperType = 0;
            int ioType = 0;
            robot.GetAxleLuaEnableDeviceType(ref forceType, ref gripperType, ref ioType);

            int[] forceEnable = new int[16];
            int[] gripperEnable = new int[16];
            int[] ioEnable = new int[16];
            robot.GetAxleLuaEnableDevice(ref forceEnable, ref gripperEnable, ref ioEnable);

            Thread.Sleep(1000);
            double[] M = new double[6] { 15.0, 15.0, 15.0, 0.5, 0.5, 0.1 };
            double[] B = new double[6] { 150, 150, 150, 5.0, 5.0, 1.0 };
            double[] K = new double[6] { 0, 0, 0, 0, 0, 0 };
            double[] F = new double[6] { 10, 10, 10, 1, 1, 1 };
            robot.EndForceDragControl(1, 0, 0, M, B, K, F, 50, 180);

            Thread.Sleep(10 * 1000);

            robot.EndForceDragControl(0, 0, 0, M, B, K, F, 50, 100);
        }

        private void btnEndGripper_Click(object sender, EventArgs e)
        {
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
            robot.SetAxleCommunicationParam(7, 8, 1, 0, 5, 3, 1);

            int baudRate = 0, dataBit = 0, stopBit = 0, verify = 0, timeout = 0, timeoutTimes = 0, period = 0;
            robot.GetAxleCommunicationParam(ref baudRate, ref dataBit, ref stopBit, ref verify, ref timeout, ref timeoutTimes, ref period);

            robot.SetAxleLuaEnable(1);
            int luaEnableStatus = 0;
            robot.GetAxleLuaEnableStatus(ref luaEnableStatus);
            robot.SetAxleLuaEnableDeviceType(0, 1, 0);
            int forceType = 0;
            int gripperType = 0;
            int ioType = 0;
            robot.GetAxleLuaEnableDeviceType(ref forceType, ref gripperType, ref ioType);

            int[] forceEnable = new int[16];
            int[] gripperEnable = new int[16];
            int[] ioEnable = new int[16];
            robot.GetAxleLuaEnableDevice(ref forceEnable, ref gripperEnable, ref ioEnable);

            //int func[16] = {0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0};
            int[] func = new int[16]{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            robot.SetAxleLuaGripperFunc(1, func);
            int[] getFunc = new int[16];
             robot.GetAxleLuaGripperFunc(1, ref getFunc);

            robot.ActGripper(1, 0);
            Thread.Sleep(2000);
            robot.ActGripper(1, 1);
            Thread.Sleep(2000);
            robot.MoveGripper(1, 10, 10, 100, 50000, 0, 0, 0, 50, 50);
            while (true)
            {
                robot.GetRobotRealTimeState(ref pkg);
                Console.WriteLine($"pos is {pkg.gripper_position}");
                Thread.Sleep(100);
            }
        }

        private void btnUploadAxleLua_Click(object sender, EventArgs e)
        {
            //robot.AxleLuaUpload("D://zUP/AXLE_LUA_End_DaHuan_WeiHang_ERR1.lua");
            robot.AxleLuaUpload("D://zUP/AXLE_LUA_End_JunDuo_Xinjingcheng.lua");
            robot.SetAxleLuaEnable(1);
            while (true)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                robot.GetRobotRealTimeState(ref pkg);
                Console.WriteLine($"end lua err code is {pkg.endLuaErrCode}");
                Console.WriteLine($"gripper pos is {pkg.gripper_position}");
                Thread.Sleep(100);
            }
        }
    }
}

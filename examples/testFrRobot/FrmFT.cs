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

        private void btn_getGripper_Click(object sender, EventArgs e)
        {
            robot.SingularAvoidEnd();

            //****************************************TestSingularAvoidSArc*********************************

            //int rtn = 0;
            //DescPose startdescPose = new DescPose(299.993, -168.982, 299.998, 179.999, -0.002, -166.415);
            //JointPos startjointPos = new JointPos(-12.160, -71.236, -131.775, -66.992, 90.000, 64.255);

            //DescPose middescPose = new DescPose(249.985, -140.988, 299.929, 179.996, -0.013, -166.417);
            //JointPos midjointPos = new JointPos(-8.604, -60.474, -137.494, -72.046, 89.999, 67.813);

            //DescPose enddescPose = new DescPose(-249.991, -168.980, 299.981, 179.999, 0.004, -107.386);
            //JointPos endjointPos = new JointPos(-126.186, -63.401, -136.126, -70.477, 89.998, -108.800);

            //ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            //rtn = robot.MoveL(startjointPos, startdescPose, 0, 0, 30, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            //rtn = robot.SingularAvoidStart(2, 30, 5, 5);
            //rtn = robot.MoveC(midjointPos, middescPose, 0, 0, 30, 100, exaxisPos, 0, offdese, endjointPos, enddescPose, 0, 0, 30, 100, exaxisPos, 0, offdese, 100, -1);
            //rtn = robot.SingularAvoidEnd();
            //Console.WriteLine($"robot moving rtn is {rtn}");

            //****************************************TestSingularAvoidSLin*********************************
            //DescPose startdescPose = new DescPose(300.002, -102.991, 299.994, 180.000, -0.001, -166.416);
            //JointPos startjointPos = new JointPos(-0.189, -66.345, -134.615, -69.042, 90.000, 76.227);

            //DescPose enddescPose = new DescPose(-300.000, -103.001, 299.994, 179.998, 0.003, -107.384);
            //JointPos endjointPos = new JointPos(-142.292, -66.345, -134.615, -69.042, 89.997, -124.908);

            //ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            //robot.MoveL(startjointPos, startdescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            ////robot.SingularAvoidStart(2, 30, 10, 3);
            //robot.MoveL(endjointPos, enddescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            ////robot.SingularAvoidEnd();

            //****************************************TestSingularAvoidWArc*********************************
            //DescPose startdescPose = new DescPose(-352.575, -685.604, 479.380, -15.933, -54.906, 130.699);
            //JointPos startjointPos = new JointPos(49.630, -56.597, 60.017, -57.989, 42.725, 146.834);

            //DescPose middescPose = new DescPose(-437.302, -372.046, 366.764, -133.489, -62.309, -94.994);
            //JointPos midjointPos = new JointPos(21.202, -72.442, 84.164, -51.660, -29.880, 146.823);

            //DescPose enddescPose = new DescPose(-653.649, -235.926, 434.525, -176.386, -54.515, -66.734);
            //JointPos endjointPos = new JointPos(5.070, -58.920, 55.287, -57.937, -41.207, 146.834);

            //ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            //robot.MoveL(startjointPos, startdescPose, 0, 0, 30, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            //robot.SingularAvoidStart(2, 10, 5, 4);
            //robot.MoveC(midjointPos, middescPose, 0, 0, 30, 100, exaxisPos, 0, offdese, endjointPos, enddescPose, 0, 0, 30, 100, exaxisPos, 0, offdese, 100, -1);
            //robot.SingularAvoidEnd();

            //****************************************TestSingularAvoidWLin*********************************
            //robot.SingularAvoidEnd();

            //DescPose startdescPose = new DescPose(-352.574, -685.606, 479.415, -15.926, -54.905, 130.693);
            //JointPos startjointPos = new JointPos(49.630, -56.597, 60.013, -57.990, 42.725, 146.834);

            //DescPose enddescPose = new DescPose(-653.655, -235.943, 434.585, -176.403, -54.513, -66.719);
            //JointPos endjointPos = new JointPos(5.072, -58.920, 55.280, -57.939, -41.207, 146.834);

            //ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            //robot.MoveL(startjointPos, startdescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            //robot.SingularAvoidStart(2, 30, 10, 3);
            //robot.MoveL(endjointPos, enddescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            //robot.SingularAvoidEnd();



            //****************************************TestSingularAvoidEArc*********************************
            DescPose startdescPose = new DescPose(-57.170, -690.147, 370.969, 176.438, -8.320, 169.881);
            JointPos startjointPos = new JointPos(78.017, -62.036, 69.561, -94.199, -98.416, -1.360);

            DescPose middescPose = new DescPose(-71.044, -743.395, 375.996, -179.499, -5.398, 168.739);
            JointPos midjointPos = new JointPos(77.417, -55.000, 58.732, -94.360, -95.385, -1.376);

            DescPose enddescPose = new DescPose(-439.979, -512.743, 396.472, 178.112, 3.625, 146.576);
            JointPos endjointPos = new JointPos(40.243, -65.402, 70.802, -92.565, -87.055, -16.465);


            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.MoveL(startjointPos, startdescPose, 0, 0, 30, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            robot.SingularAvoidStart(2, 10, 5, 5);
            robot.MoveC(midjointPos, middescPose, 0, 0, 30, 100, exaxisPos, 0, offdese, endjointPos, enddescPose, 0, 0, 30, 100, exaxisPos, 0, offdese, 100, -1);
            robot.SingularAvoidEnd();

        }
        void FIRArc( bool enable)
        {
            DescPose startdescPose = new DescPose(-366.397, -572.427, 418.339, -178.972, 1.829, -142.970);
            JointPos startjointPos = new JointPos(43.651, -70.284, 91.057, -109.075, -88.768, -83.382);

            DescPose middescPose = new DescPose(-569.710, -132.595, 395.147, 178.418, -1.893, 171.051);
            JointPos midjointPos = new JointPos(-2.334, -79.300, 108.196, -120.594, -91.790, -83.386);

            DescPose enddescPose = new DescPose(-608.420, 610.692, 314.930, -176.438, -1.756, 117.333);
            JointPos endjointPos = new JointPos(-56.153, -46.964, 68.015, -113.200, -86.661, -83.479);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            if (enable)
            {
                robot.LinArcFIRPlanningStart(1000, 1000, 1000, 1000);
                robot.MoveL(startjointPos, startdescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
                robot.MoveC(midjointPos, middescPose, 0, 0, 100, 100, exaxisPos, 0, offdese, endjointPos, enddescPose, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1);
                robot.LinArcFIRPlanningEnd();
            }
            else
            {
                robot.MoveL(startjointPos, startdescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
                robot.MoveC(midjointPos, middescPose, 0, 0, 100, 100, exaxisPos, 0, offdese, endjointPos, enddescPose, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1);
            }
        }
        void FIRLin( bool enable)
        {
            DescPose startdescPose = new DescPose(-569.710, -132.595, 395.147, 178.418, -1.893, 171.051);
            JointPos startjointPos = new JointPos(-2.334, -79.300, 108.196, -120.594, -91.790, -83.386);

            DescPose enddescPose = new DescPose(-366.397, -572.427, 418.339, -178.972, 1.829, -142.970);
            JointPos endjointPos = new JointPos(43.651, -70.284, 91.057, -109.075, -88.768, -83.382);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            if (enable)
            {
                robot.LinArcFIRPlanningStart(5000, 5000, 5000, 5000);
                robot.MoveL(startjointPos, startdescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
                robot.MoveL(endjointPos, enddescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
                robot.LinArcFIRPlanningEnd();
            }
            else
            {
                robot.MoveL(startjointPos, startdescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
                robot.MoveL(endjointPos, enddescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            }
        }

        void FIRLinL( bool enable)
        {
            DescPose startdescPose = new DescPose(-608.420, 610.692, 314.930, -176.438, -1.756, 117.333);
            JointPos startjointPos = new JointPos(-56.153, -46.964, 68.015, -113.200, -86.661, -83.479);

            DescPose enddescPose = new DescPose(-366.397, -572.427, 418.339, -178.972, 1.829, -142.970);
            JointPos endjointPos = new JointPos(43.651, -70.284, 91.057, -109.075, -88.768, -83.382);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            if (enable)
            {
                robot.LinArcFIRPlanningStart(5000, 5000, 5000, 5000);
                robot.MoveL(startjointPos, startdescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
                robot.MoveL(endjointPos, enddescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
                robot.LinArcFIRPlanningEnd();
            }
            else
            {
                robot.MoveL(startjointPos, startdescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
                robot.MoveL(endjointPos, enddescPose, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            }
        }

        void FIRPTP( bool enable)
        {
            DescPose startdescPose = new DescPose(-569.710, -132.595, 395.147, 178.418, -1.893, 171.051);
            JointPos startjointPos = new JointPos(-2.334, -79.300, 108.196, -120.594, -91.790, -83.386);

            DescPose enddescPose = new DescPose(-366.397, -572.427, 418.339, -178.972, 1.829, -142.970);
            JointPos endjointPos = new JointPos(43.651, -70.284, 91.057, -109.075, -88.768, -83.382);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            if (enable)
            {
                robot.PtpFIRPlanningStart(1000);
                robot.MoveJ(startjointPos, startdescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
                robot.MoveJ(endjointPos, enddescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
                robot.PtpFIRPlanningEnd();
            }
            else
            {
                robot.MoveJ(startjointPos, startdescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
                robot.MoveJ(endjointPos, enddescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();


            FIRPTP(false);
            FIRPTP(true);
            //FIRLin(false);
            //FIRLin(true);

            //FIRLinL(false);
            //FIRLinL(true);

            //FIRArc(false);
            //FIRArc(true);
        }
        int UploadTrajectoryJ()
        {
            robot.TrajectoryJDelete("testA.txt");
            robot.TrajectoryJUpLoad("D://zUP/testA.txt");

            int retval = 0;
            string traj_file_name = "/fruser/traj/testA.txt";
            retval = robot.LoadTrajectoryJ(traj_file_name, 100, 1);
            Console.WriteLine($"LoadTrajectoryJ {traj_file_name}, retval is: {retval}");

            DescPose traj_start_pose = new DescPose(0, 0, 0, 0, 0, 0);

            retval = robot.GetTrajectoryStartPose(traj_file_name, ref traj_start_pose);
            retval = robot.GetTrajectoryStartPose(traj_file_name, ref traj_start_pose);
            Console.WriteLine($"GetTrajectoryStartPose is: {retval}");
            Console.WriteLine(string.Format("desc_pos:{0},{1},{2},{3},{4},{5}",
              traj_start_pose.tran.x,
              traj_start_pose.tran.y,
              traj_start_pose.tran.z,
              traj_start_pose.rpy.rx,
              traj_start_pose.rpy.ry,
              traj_start_pose.rpy.rz));

            robot.SetSpeed(20);
            robot.MoveCart(traj_start_pose, 1, 0, 100, 100, 100, -1, -1);

            Thread.Sleep(5000);

            int traj_num = 0;
            retval = robot.GetTrajectoryPointNum(ref traj_num);
            Console.WriteLine($"GetTrajectoryStartPose retval is: {retval}, traj num is:{traj_num}");

            retval = robot.MoveTrajectoryJ();
            Console.WriteLine($"MoveTrajectoryJ retval is: {retval}");
            return 0;
        }
        int UploadTrajectoryB()
        {
            robot.TrajectoryJDelete("testB.txt");
            robot.TrajectoryJUpLoad("D://zUP/testB.txt");

            int retval = 0;
            string traj_file_name = "/fruser/traj/testB.txt";
            retval = robot.LoadTrajectoryJ(traj_file_name, 100, 1);
            Console.WriteLine($"LoadTrajectoryJ {traj_file_name}, retval is: { retval}");

            DescPose traj_start_pose = new DescPose(0, 0, 0, 0, 0, 0);
            retval = robot.GetTrajectoryStartPose(traj_file_name, ref traj_start_pose);
            Console.WriteLine($"GetTrajectoryStartPose is: {retval}");
            Console.WriteLine(string.Format("desc_pos:{0},{1},{2},{3},{4},{5}",
              traj_start_pose.tran.x,
              traj_start_pose.tran.y,
              traj_start_pose.tran.z,
              traj_start_pose.rpy.rx,
              traj_start_pose.rpy.ry,
              traj_start_pose.rpy.rz));

            robot.SetSpeed(20);
            robot.MoveCart(traj_start_pose, 1, 0, 100, 100, 100, -1, -1);

            Thread.Sleep(5000);

            int traj_num = 0;
            retval = robot.GetTrajectoryPointNum(ref traj_num);
            Console.WriteLine($"GetTrajectoryStartPose retval is: {retval}, traj num is:{traj_num}");

            retval = robot.MoveTrajectoryJ();
            Console.WriteLine($"MoveTrajectoryJ retval is: {retval}");
            return 0;
        }
        int MoveRotGripper(int pos, double rotPos)
        {
            robot.ResetAllError();
            robot.ActGripper(1, 1);
            Thread.Sleep(1000);
            int rtn = robot.MoveGripper(1, pos, 50, 50, 5000, 1, 1, rotPos, 50, 100);
            Console.WriteLine($"move gripper rtn is {rtn}" );
            UInt16 fault = 0;
            double rotNum = 0.0;
            int rotSpeed = 0;
            int rotTorque = 0;
            robot.GetGripperRotNum(ref fault, ref rotNum);
            robot.GetGripperRotSpeed(ref fault, ref rotSpeed);
            robot.GetGripperRotTorque(ref fault, ref rotTorque);
            Console.WriteLine($"gripper rot num :{ rotNum}, gripper rotSpeed :{rotSpeed}, gripper rotTorque : { rotTorque}");
            return 0;
        }
        int SetAO(float value)
        {
            robot.SetAO(0, value, 0);
            robot.SetAO(1, value, 0);
            robot.SetToolAO(0, value, 0);
            while (true)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                robot.GetRobotRealTimeState(ref pkg);
                if (Math.Abs(pkg.cl_analog_output[0]/40.96 - value) < 0.5)
                {
                    break;
                }
                else
                {
                    Console.WriteLine($"cur AO value is {pkg.cl_analog_output[0]}");
                    Thread.Sleep(1);
                }
            }
            Console.WriteLine($"setAO Done  {value}");
            return 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
            
            int rtn;
            //SetAO((float)50.0);
            //MoveRotGripper(0, 1.2);

            //UploadTrajectoryJ();
            //UploadTrajectoryB();
            while (true)
            {
                MoveRotGripper(30, 0);
                MoveRotGripper(75, 0);
                UploadTrajectoryJ();
                //MoveRotGripper(90, 2);
                Thread.Sleep(5000);
                MoveRotGripper(30, 0);
                Thread.Sleep(5000);
                MoveRotGripper(75, 0);
                UploadTrajectoryB();
                //MoveRotGripper(90, 0);
                Thread.Sleep(5000);
                MoveRotGripper(30, 0);
                rtn = robot.GetRobotRealTimeState(ref pkg);
                //Console.WriteLine(string.Format("the robot AO0 {0}, AO1 {1}, tool AO0 {2}", pkg.cl_analog_output[0], pkg.cl_analog_output[1], pkg.tl_analog_output));
                Console.WriteLine($"gripper pos {pkg.gripper_position} - vel {pkg.gripper_speed} - torque {pkg.gripper_current} - rotPos {pkg.gripperRotNum:F2} - rotvel - {pkg.gripperRotSpeed:F2} rotTor - {pkg.gripperRotTorque:F2}");
            }

            //while (true)
            //{
            //    rtn = robot.GetRobotRealTimeState(ref pkg);
            //    //Console.WriteLine(string.Format("the robot AO0 {0}, AO1 {1}, tool AO0 {2}", pkg.cl_analog_output[0]/40.96, pkg.cl_analog_output[1]/40.96, pkg.tl_analog_output /40.96));
            //    Console.WriteLine($"gripper pos {pkg.gripper_position} - vel {pkg.gripper_speed} - torque {pkg.gripper_current} - rotPos {pkg.gripperRotNum} - rotvel - {pkg.gripperRotSpeed} rotTor - {pkg.gripperRotTorque}");
            //    Thread.Sleep(0);
            //}
            //robot.CloseRPC();



        }

        private void button6_Click(object sender, EventArgs e)
        {
            byte flag = 1;
            byte sensor_id = 2;
            int[] select = { 0, 0, 1, 0, 0, 0 };//只启用x轴碰撞守护
            double[] max_threshold = { 0.01, 0.01, 5.01, 0.01, 0.01, 0.01 };
            double[] min_threshold = { 0.01, 0.01, 5.01, 0.01, 0.01, 0.01 };

            ForceTorque ft = new ForceTorque(1.0, 0.0, 2.0, 0.0, 0.0, 0.0);
            DescPose desc_p1, desc_p2, desc_p3;

            desc_p1 = new DescPose(-280.5, -474.534, 320.677, 177.986, 1.498, -118.235);
            desc_p2 = new DescPose(-283.273, -468.668, 172.905, 177.986, 1.498, -118.235);

            int[] safetyMargin = { 1, 1, 1, 1, 1, 1 };
            robot.SetCollisionStrategy(5, 1000, 150,150,safetyMargin);
            int rtn = robot.FT_Guard(flag, sensor_id, select, ft, max_threshold, min_threshold);
            Console.WriteLine("FT_Guard start rtn " + rtn);
            robot.MoveCart(desc_p1, 0, 0, 20, 100.0f, 100.0f, -1.0f, -1);
            robot.MoveCart(desc_p2, 0, 0, 20, 100.0f, 100.0f, -1.0f, -1);
            flag = 0;
            rtn = robot.FT_Guard(flag, sensor_id, select, ft, max_threshold, min_threshold);
            Console.WriteLine("FT_Guard end rtn " + rtn);

        }

        private void button7_Click(object sender, EventArgs e)
        {
            int rtn = -1;
            rtn = robot.WeldingSetCheckArcInterruptionParam(1, 200);
            Console.WriteLine("WeldingSetCheckArcInterruptionParam  {0}", rtn);
            rtn = robot.WeldingSetReWeldAfterBreakOffParam(1, 5.7, 98.2, 0);
            Console.WriteLine("WeldingSetReWeldAfterBreakOffParam {0}", rtn);
            int enable = 0;
            double length = 0;
            double velocity = 0;
            int moveType = 0;
            int checkEnable = 0;
            int arcInterruptTimeLength = 0;
            rtn = robot.WeldingGetCheckArcInterruptionParam(ref checkEnable, ref arcInterruptTimeLength);
            Console.WriteLine($"WeldingGetCheckArcInterruptionParam  checkEnable {checkEnable} - arcInterruptTimeLength {arcInterruptTimeLength}");

            rtn = robot.WeldingGetReWeldAfterBreakOffParam(ref enable, ref length, ref velocity,ref moveType);
            Console.WriteLine("WeldingGetReWeldAfterBreakOffParam  enable = {0}, length = {1}, velocity = {2}, moveType = {3}", enable, length, velocity, moveType);

            robot.ProgramLoad("/fruser/test.lua");
            robot.ProgramRun();

            Thread.Sleep(5000);

            while (true)
            {
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG { };
                robot.GetRobotRealTimeState(ref pkg);
                Console.WriteLine("welding breakoff state is     {0}", pkg.weldingBreakOffState.breakOffState);
                if (pkg.weldingBreakOffState.breakOffState == 1)
                {
                    Console.WriteLine("welding breakoff ! \n");
                    Thread.Sleep(2000);
                    rtn = robot.WeldingStartReWeldAfterBreakOff();
                    Console.WriteLine("WeldingStartReWeldAfterBreakOff    %d\n", rtn);
                    break;
                }
                Thread.Sleep(100);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //for(int i = 0; i < 30; ++i)
            //{
            //    Console.WriteLine("工件坐标系测试"+i);
            //    Textwobj();
            //    Thread.Sleep(2000);
            //}

            //for (int i = 1; i < 31; ++i)
            //{

            //    Console.WriteLine("四点法工具坐标系标定" + i);
            //    TestTCP.PerformClick();
            //    Thread.Sleep(2000);
            //}
            //for (int i = 1; i < 31; ++i)
            //{
            //    Console.WriteLine("六点法工具坐标系标定" + i);
            //    TestTCP6.PerformClick();
            //    Thread.Sleep(2000);
            //}
            for (int i = 1; i < 31; ++i)
            {
                Console.WriteLine("焊接中断稳定性：" + i);
                button7.PerformClick();
                Thread.Sleep(1500);
            }
        }
        public void Textwobj()
        {
            DescPose p1Desc = new DescPose(-275.046, -293.122, 28.747, 174.533, -1.301, -112.101);
            JointPos p1Joint = new JointPos(35.207, -95.350, 133.703, -132.403, -93.897, -122.768);

            DescPose p2Desc = new DescPose(-280.339, -396.053, 29.762, 174.621, -3.448, -102.901);
            JointPos p2Joint = new JointPos(44.304, -85.020, 123.889, -134.679, -92.658, -122.768);

            DescPose p3Desc = new DescPose(-270.597, -290.603, 83.034, 179.314, 0.808, -114.171);
            JointPos p3Joint = new JointPos(32.975, -99.175, 125.966, -116.484, -91.014, -122.857);
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            DescPose[] posTCP = new DescPose[3] { p1Desc, p2Desc, p3Desc };
            DescPose coordRtn = new DescPose { };
            int rtn = robot.ComputeWObjCoordWithPoints(0, posTCP, 0, ref coordRtn);
            Console.WriteLine("ComputeToolCoordWithPoints  {0}  coord is {1} {2} {3} {4} {5} {6}", rtn, coordRtn.tran.x, coordRtn.tran.y, coordRtn.tran.z, coordRtn.rpy.rx, coordRtn.rpy.ry, coordRtn.rpy.rz);


            robot.MoveJ(p1Joint, p1Desc, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetWObjCoordPoint(1);
            robot.MoveJ(p2Joint, p2Desc, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetWObjCoordPoint(2);
            robot.MoveJ(p3Joint, p3Desc, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetWObjCoordPoint(3);
            robot.ComputeWObjCoord(0, 0, ref coordRtn);
            Console.WriteLine("ComputeTool {0}  coord is {1} {2} {3} {4} {5} {6}", rtn, coordRtn.tran.x, coordRtn.tran.y, coordRtn.tran.z, coordRtn.rpy.rx, coordRtn.rpy.ry, coordRtn.rpy.rz);
        }
        private void TestTCP_Click(object sender, EventArgs e)
        {
            DescPose p1Desc = new DescPose(-394.073, -276.405, 399.451, -133.692, 7.657, -139.047);
            JointPos p1Joint = new JointPos(15.234, -88.178, 96.583, -68.314, -52.303, -122.926);

            DescPose p2Desc = new DescPose( -187.141, -444.908, 432.425, 148.662, 15.483, -90.637);
            JointPos p2Joint = new JointPos(61.796, -91.959, 101.693, -102.417, -124.511, -122.767);

            DescPose p3Desc = new DescPose(-368.695, -485.023, 426.640, -162.588, 31.433, -97.036);
            JointPos p3Joint = new JointPos(43.896, -64.590, 60.087, -50.269, -94.663, -122.652);

            DescPose p4Desc = new DescPose(-291.069, -376.976, 467.560, -179.272, -2.326, -107.757);
            JointPos p4Joint = new JointPos(39.559, -94.731, 96.307, -93.141, -88.131, -122.673);

            DescPose p5Desc = new DescPose(-284.140, -488.041, 478.579, 179.785, -1.396, -98.030);
            JointPos p5Joint = new JointPos(49.283, -82.423, 81.993, -90.861, -89.427, -122.678);

            DescPose p6Desc = new DescPose(-296.307, -385.991, 484.492, -178.637, -0.057, -107.059);
            JointPos p6Joint = new JointPos(40.141, -92.742, 91.410, -87.978, -88.824, -122.808);

            ExaxisPos exaxisPos=new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            JointPos[] posJ = new JointPos[6]{ p1Joint, p2Joint, p3Joint, p4Joint, p5Joint, p6Joint };
            DescPose coordRtn = new DescPose(0, 0, 0, 0, 0, 0); 
            int rtn = robot.ComputeToolCoordWithPoints(0, posJ,ref coordRtn);
            Console.WriteLine("ComputeToolCoordWithPoints {0}  coord is {1} {2} {3} {4} {5} {6}", rtn, coordRtn.tran.x, coordRtn.tran.y, coordRtn.tran.z, coordRtn.rpy.rx, coordRtn.rpy.ry, coordRtn.rpy.rz);


            robot.MoveJ(p1Joint, p1Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetTcp4RefPoint(1);
            robot.MoveJ(p2Joint, p2Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetTcp4RefPoint(2);
            robot.MoveJ(p3Joint, p3Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetTcp4RefPoint(3);
            robot.MoveJ(p4Joint, p4Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetTcp4RefPoint(4);
            robot.ComputeTcp4(ref coordRtn);
            Console.WriteLine("ComputeTcp4 {0}  coord is {1} {2} {3} {4} {5} {6}", rtn, coordRtn.tran.x, coordRtn.tran.y, coordRtn.tran.z, coordRtn.rpy.rx, coordRtn.rpy.ry, coordRtn.rpy.rz);
            //robot.MoveJ(p5Joint, p5Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            //robot.MoveJ(p6Joint, p6Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
        }

        private void TestTCP6_Click(object sender, EventArgs e)
        {
            DescPose p1Desc = new DescPose(-394.073, -276.405, 399.451, -133.692, 7.657, -139.047);
            JointPos p1Joint = new JointPos(15.234, -88.178, 96.583, -68.314, -52.303, -122.926);

            DescPose p2Desc = new DescPose(-187.141, -444.908, 432.425, 148.662, 15.483, -90.637);
            JointPos p2Joint = new JointPos(61.796, -91.959, 101.693, -102.417, -124.511, -122.767);

            DescPose p3Desc = new DescPose(-368.695, -485.023, 426.640, -162.588, 31.433, -97.036);
            JointPos p3Joint = new JointPos(43.896, -64.590, 60.087, -50.269, -94.663, -122.652);

            DescPose p4Desc = new DescPose(-291.069, -376.976, 467.560, -179.272, -2.326, -107.757);
            JointPos p4Joint = new JointPos(39.559, -94.731, 96.307, -93.141, -88.131, -122.673);

            DescPose p5Desc = new DescPose(-284.140, -488.041, 478.579, 179.785, -1.396, -98.030);
            JointPos p5Joint = new JointPos(49.283, -82.423, 81.993, -90.861, -89.427, -122.678);

            DescPose p6Desc = new DescPose(-296.307, -385.991, 484.492, -178.637, -0.057, -107.059);
            JointPos p6Joint = new JointPos(40.141, -92.742, 91.410, -87.978, -88.824, -122.808);

            ExaxisPos exaxisPos=new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            JointPos[] posJ =new JointPos[6] { p1Joint, p2Joint, p3Joint, p4Joint, p5Joint, p6Joint };
            DescPose coordRtn  = new DescPose(0, 0, 0, 0, 0, 0);
            int rtn = robot.ComputeToolCoordWithPoints(1, posJ, ref coordRtn);
            Console.WriteLine("ComputeToolCoordWithPoints  {0}  coord is {1} {2} {3} {4} {5} {6}", rtn, coordRtn.tran.x, coordRtn.tran.y, coordRtn.tran.z, coordRtn.rpy.rx, coordRtn.rpy.ry, coordRtn.rpy.rz);

            robot.MoveJ(p1Joint, p1Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetToolPoint(1);
            robot.MoveJ(p2Joint, p2Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetToolPoint(2);
            robot.MoveJ(p3Joint, p3Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetToolPoint(3);
            robot.MoveJ(p4Joint, p4Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetToolPoint(4);
            robot.MoveJ(p5Joint, p5Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetToolPoint(5);
            robot.MoveJ(p6Joint, p6Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetToolPoint(6);
            robot.ComputeTool(ref coordRtn);
            Console.WriteLine("ComputeTool {0}  coord is {1} {2} {3} {4} {5} {6}", rtn, coordRtn.tran.x, coordRtn.tran.y, coordRtn.tran.z, coordRtn.rpy.rx, coordRtn.rpy.ry, coordRtn.rpy.rz);
        }

        private void ExtAxisLaserTracking_Click(object sender, EventArgs e)
        {
            DescPose p1Desc=new DescPose(381.070, -177.767, 227.851, 20.031, -2.455, -111.479);
            JointPos p1Joint=new JointPos(8.383, -44.801, -111.050, -97.707, 78.144, 27.709);

            DescPose p2Desc = new DescPose(381.077, -177.762, 217.865, 20.014, -0.131, -110.631);
            JointPos p2Joint = new JointPos(1.792, -44.574, -113.176, -93.687, 82.384, 21.154);

            DescPose p3Desc = new DescPose(381.070, -177.767, 227.851, 20.031, -2.455, -111.479);
            JointPos p3Joint = new JointPos(8.383, -44.801, -111.050, -97.707, 78.144, 27.709);

            ExaxisPos exaxisPos = new ExaxisPos(0.0, 0.0, 0.0, 0.0);
            DescPose offdese = new DescPose(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);

            ExaxisPos exaxisPosStart = new ExaxisPos(0.0, 0.0, 0.0, 0.0);
            robot.MoveJ(p1Joint, p1Desc, 8, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.ExtAxisMove(exaxisPosStart, 50.0);
            robot.MoveL(p2Joint, p2Desc, 8, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            robot.LaserSensorRecord(4, 1, 10, 2, 35, 0.1, 100);
            ExaxisPos  exaxisPosTarget = new ExaxisPos(0.000, 400.015, 0.000, 0.000);
            robot.ExtAxisMove(exaxisPosTarget, 10.0);
            robot.LaserSensorRecord(0, 1, 10, 2, 35, 0.1, 100);
            robot.MoveJ(p3Joint, p3Desc, 8, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.ExtAxisMove(exaxisPosStart, 50.0);
        }
    }
}

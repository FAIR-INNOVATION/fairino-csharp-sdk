using fairino;
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
using Microsoft.VisualBasic;
using System.IO.Ports;
using System.Runtime.InteropServices.ComTypes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Runtime;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
namespace testFrRobot
{
    public partial class Test : Form
    {
        Robot robot;

        ROBOT_STATE_PKG currentState = new ROBOT_STATE_PKG();
        public Test(Robot rob)
        {
            InitializeComponent();
            robot = rob;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            int rtn = 0;
            JointPos startjointPos = new JointPos(-11.904f, -99.669f, 117.473f, -108.616f, -91.726f, 74.256f);
            JointPos endjointPos = new JointPos(-45.615f, -106.172f, 124.296f, -107.151f, -91.282f, 74.255f);

            DescPose startdescPose = new DescPose(-419.524f, -13.000f, 351.569f, -178.118f, 0.314f, 3.833f);
            DescPose enddescPose = new DescPose(-321.222f, 185.189f, 335.520f, -179.030f, -1.284f, -29.869f);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            rtn = robot.AccSmoothStart(false);
            Console.WriteLine("AccSmoothStart rtn is " + rtn);
            robot.MoveJ(startjointPos, startdescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.MoveJ(endjointPos, enddescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            rtn = robot.AccSmoothEnd(false);
            Console.WriteLine("AccSmoothEnd rtn is " + rtn);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            DescPose dcs1 = new DescPose(32.316, -232.029, 1063.415, 90.159, 18.376, 36.575);
            DescPose dcs2 = new DescPose(105.25, -170.914, 1076.283, 87.032, 25.94, 54.644);
            DescPose dcs3 = new DescPose(79.164, 81.645, 1045.609, 133.691, -73.265, 162.726);
            DescPose dcs4 = new DescPose(298.779, -104.112, 298.242, 179.631, -0.628, -166.481);
            JointPos inverseRtn = new JointPos(0, 0, 0, 0, 0, 0);

            //robot.GetInverseKin(0, dcs1, -1, ref inverseRtn);
            //Console.WriteLine($"dcs1 getinverse rtn is {inverseRtn.jPos[0]} {inverseRtn.jPos[1]} {inverseRtn.jPos[2]} {inverseRtn.jPos[3]} {inverseRtn.jPos[4]} {inverseRtn.jPos[5]}");

            //robot.GetInverseKin(0, dcs2, -1, ref inverseRtn);
            //Console.WriteLine($"dcs2 getinverse rtn is {inverseRtn.jPos[0]} {inverseRtn.jPos[1]} {inverseRtn.jPos[2]} {inverseRtn.jPos[3]} {inverseRtn.jPos[4]} {inverseRtn.jPos[5]}");

            //robot.GetInverseKin(0, dcs3, -1, ref inverseRtn);
            //Console.WriteLine($"dcs3 getinverse rtn is {inverseRtn.jPos[0]} {inverseRtn.jPos[1]} {inverseRtn.jPos[2]} {inverseRtn.jPos[3]} {inverseRtn.jPos[4]} {inverseRtn.jPos[5]}");

            //robot.GetInverseKin(0, dcs4, -1, ref inverseRtn);
            //Console.WriteLine($"dcs4 getinverse rtn is {inverseRtn.jPos[0]} {inverseRtn.jPos[1]} {inverseRtn.jPos[2]} {inverseRtn.jPos[3]} {inverseRtn.jPos[4]} {inverseRtn.jPos[5]}");

            JointPos jpos1 = new JointPos(56.999, -59.002, 56.996, -96.552, 60.392, -90.005);
            DescPose forwordResult1 = new DescPose(0, 0, 0, 0, 0, 0);
            robot.GetForwardKin(jpos1, ref forwordResult1);
            Console.WriteLine($"jpos1 forwordResult rtn is {forwordResult1.tran.x} {forwordResult1.tran.y} {forwordResult1.tran.z} {forwordResult1.rpy.rx} {forwordResult1.rpy.ry} {forwordResult1.rpy.rz}");


        }

        private void button3_Click(object sender, EventArgs e)
        {

            // 禁用按钮防止重复点击
            button3.Enabled = false;

            // 在后台线程中执行耗时操作
            Thread conveyorThread = new Thread(ConveyorTest);
            conveyorThread.IsBackground = true;
            conveyorThread.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 获取用户输入
            string input = texBox.Text;
            Console.WriteLine($"please input a number to trigger:{input}");

            int rtn = robot.ConveyorComDetectTrigger();
            Console.WriteLine($"ConveyorComDetectTrigger 返回值: {rtn}");

        }

        private void ConveyorTest()
        {
            // 使用Invoke来更新UI线程上的控件
            this.Invoke((MethodInvoker)delegate {
                Console.WriteLine("开始传送带测试...");
            });

            int retval = 0;
            retval = 0;

            /* 传送带抓取流程 */
            DescPose startdescPose = new DescPose(-354.659, 63.299, 270.684, -178.845, -0.058, 0.034);
            JointPos startjointPos = new JointPos(-25.797, -110.917, 113.407, -92.941, -91.065, 64.164);


            //-25.797,-110.646,104.525,-84.330,-91.065,64.164,-354.659,63.297,330.679,-178.845,-0.058,0.034
            DescPose homePose = new DescPose(-354.659, 63.297, 330.679, -178.845, -0.058, 0.034);
            JointPos homejointPos = new JointPos(-25.797, -110.646, 104.525, -84.330, -91.065, 64.164);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            // 移动到起始位置
            robot.MoveL(startjointPos, startdescPose, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);

            // 传送带检测
            retval = robot.ConveyorComDetect(1000 * 10);
            Console.WriteLine($"ConveyorComDetect 返回值: {retval}");

            // 获取跟踪数据
            retval = robot.ConveyorGetTrackData(2);
            Console.WriteLine($"ConveyorGetTrackData 返回值: {retval}");

            // 开始跟踪
            retval = robot.ConveyorTrackStart(2);
            Console.WriteLine($"ConveyorTrackStart 返回值: {retval}");
            Thread.Sleep(2000);
            // 结束跟踪
            retval = robot.ConveyorTrackEnd();
            Console.WriteLine($"ConveyorTrackEnd 返回值: {retval}");

            // 返回安全位置
             //robot.MoveL(homejointPos, homePose, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);

            this.Invoke((MethodInvoker)delegate {
                Console.WriteLine("传送带测试完成!");
                button3.Enabled = true;
            });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // 关闭ROS系统（假设方法存在）
            // robot.ShutDownRobotOS();

            // 数据包下载循环
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("DataPackageDownload start");
                int rtn = robot.DataPackageDownload(@"D:\zDOWN\");
                Console.WriteLine($"DataPackageDownload rtn is {rtn}  times  {i}");
            }

            // 全数据源下载循环
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("AllDataSourceDownload start");
                int rtn = robot.AllDataSourceDownload(@"D:\zDOWN\");
                Console.WriteLine($"AllDataSourceDownload rtn is {rtn}  times  {i}");
            }

            // 日志下载循环
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("RbLogDownload start");
                int rtn = robot.RbLogDownload(@"D:\zDOWN\");
                Console.WriteLine($"RbLogDownload rtn is {rtn}  times  {i}");
            }

            // 获取机器人序列号循环
            for (int i = 0; i < 10; i++)
            {
                string SN = "";
                robot.GetRobotSN(ref SN); // 假设方法直接返回字符串
                Console.WriteLine($"robot SN is {SN}  times  {i}");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //string SN = "";
            //int rtn = robot.GetRobotSN(ref SN); // 假设方法直接返回字符串
            //Console.WriteLine($"robot SN is {SN}");

            Console.WriteLine("RbLogDownload start");
            int rtn = robot.RbLogDownload(@"D:\zDOWN\");
            Console.WriteLine($"RbLogDownload rtn is {rtn}");


            //Console.WriteLine("AllDataSourceDownload start");
            //int rtn = robot.AllDataSourceDownload(@"D:\zDOWN\");
            //Console.WriteLine($"AllDataSourceDownload rtn is {rtn}");

            //Console.WriteLine("DataPackageDownload start");
            //int rtn = robot.DataPackageDownload(@"D:\zDOWN\");
            //Console.WriteLine($"DataPackageDownload rtn is {rtn}");
            //// 关闭ROS系统（假设方法存在）
            //int rtn = robot.ShutDownRobotOS();
            //Console.WriteLine($"ShutDownRobotOS rtn is {rtn}");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DescPose startdescPose = new DescPose(146.273, -208.110, 270.102, 177.523, -3.782, -158.101);
            JointPos startjointPos = new JointPos(98.551, -128.309, 127.341, -87.490, -94.249, -13.208);
            DescPose enddescPose = new DescPose(146.272, -476.204, 270.102, 177.523, -3.781, -158.101);
            JointPos endjointPos = new JointPos(93.931, -89.722, 102.216, -101.300, -94.359, -17.840);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            robot.WeaveSetPara(0, 3, 2.000000, 0, 10.000000, 0.000000, 0.000000, 0, 0, 0, 0, 0, 0, 0);
            //robot.MoveL(startjointPos, startdescPose, 2, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            //robot.WeaveStart(0);
            //robot.MoveL(endjointPos, enddescPose, 2, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            //robot.WeaveEnd(0);

            //robot.WeaveSetPara(0, 3, 2.000000, 0, 10.000000, 0.000000, 0.000000, 0, 0, 0, 0, 0, 0, 30);
            //robot.MoveL(startjointPos, startdescPose, 2, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            //robot.WeaveStart(0);
            //robot.MoveL(endjointPos, enddescPose, 2, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            //robot.WeaveEnd(0);

        }

        private void button8_Click(object sender, EventArgs e)
        {

            DescPose startdescPose = new DescPose(441.901, 416.508, -51.979, -179.234, 0.718, -115.305);
            JointPos startjointPos = new JointPos(-146.22, -60.551, 104.859, -135.317, -90.289, 59.088);

            DescPose enddescPose = new DescPose(441.901, 615.317, -51.979, -179.234, 0.718, -115.305);
            JointPos endjointPos = new JointPos(-133.22, -44.193, 74.934, -121.661, -90.509, 72.087);

            DescPose safetydescPose = new DescPose(441.901, 416.508, -51.979, -179.234, 0.718, -115.305);
            JointPos safetyjointPos = new JointPos(-146.22, -60.551, 104.859, -135.317, -90.289, 59.088);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            robot.MoveJ(safetyjointPos, safetydescPose, 1, 0, 20, 100, 100, exaxisPos, -1, 0, offdese);

            robot.WeldingSetCurrentRelation(0, 495, 1, 10, 0);
            robot.WeldingSetVoltageRelation(10, 45, 1, 10, 1);
            robot.WeldingSetVoltage(0, 25, 1, 0);// ----设置电压
            robot.WeldingSetCurrent(0, 260, 0, 0);// ----设置电流

            int rtn = robot.ArcWeldTraceAIChannelCurrent(4);
            Console.WriteLine("ArcWeldTraceAIChannelCurrent rtn is " + rtn);
            rtn = robot.ArcWeldTraceAIChannelVoltage(5);
            Console.WriteLine("ArcWeldTraceAIChannelVoltage rtn is " + rtn);
            rtn = robot.ArcWeldTraceCurrentPara((double)0, (double)5, (double)0, (double)500);
            Console.WriteLine("ArcWeldTraceCurrentPara rtn is " + rtn);
            rtn = robot.ArcWeldTraceVoltagePara((double)1.018, (double)10, (double)0, (double)50);
            Console.WriteLine("ArcWeldTraceVoltagePara rtn is " + rtn);

            robot.MoveJ(startjointPos, startdescPose, 1, 0, 20, 100, 100, exaxisPos, -1, 0, offdese);
            robot.ArcWeldTraceControl(1, 0, 1, 0.08, 5, 5, 300, 1, 0.06, 4, 4, 300, 1, 0, 4, 1, 10, 0, 0);
            robot.ARCStart(0, 0, 10000);
            robot.WeaveStart(0);
             robot.MoveL(endjointPos, enddescPose, 1, 0, 100, 100, 2, -1, 0,exaxisPos, 0, 0, offdese);
            robot.ARCEnd(0, 0, 10000);
            robot.WeaveEnd(0);
            robot.ArcWeldTraceControl(0, 0, 1, 0.08, 5, 5, 300, 1, 0.06, 4, 4, 300, 1, 0, 4, 1, 10, 0, 0);
            robot.MoveJ(safetyjointPos, safetydescPose, 1, 0, 20, 100, 100, exaxisPos, -1, 0, offdese);

        }

        private void button9_Click(object sender, EventArgs e)
        {

            //DescPose startdescPose = new DescPose(-319.303, -240.689, 116.379, -175.879, -0.337, 148.239);
            //JointPos startjointPos = new JointPos(20.474, -103.554, 126.774, -116.682, -87.746, -37.709);

            //DescPose enddescPose = new DescPose(-454.166, -327.159, 62.217, 177.199, -2.276, 154.955);
            //JointPos endjointPos = new JointPos(27.176, -74.423, 104.557, -119.315, -93.514, -37.698);

            DescPose startdescPose = new DescPose(441.901, 416.508, -51.979, -179.234, 0.718, -115.305);
            JointPos startjointPos = new JointPos(-146.22, -60.551, 104.859, -135.317, -90.289, 59.088);

            DescPose enddescPose = new DescPose(441.901, 615.317, -51.979, -179.234, 0.718, -115.305);
            JointPos endjointPos = new JointPos(-133.22, -44.193, 74.934, -121.661, -90.509, 72.087);

            DescPose safedescPose = new DescPose(441.901, 416.508, -51.979, -179.234, 0.718, -115.305);
            JointPos safejointPos = new JointPos(-146.22, -60.551, 104.859, -135.317, -90.289, 59.088);

            //DescPose safedescPose = new DescPose(-375.533, -543.319, 19.798, 177.486, -2.489, 175.825);
            //JointPos safejointPos = new JointPos(48.074, -59.714, 89.955, -119.777, -93.508, -37.683);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.WeldingSetCurrentRelation(0, 495, 1, 10, 0);
            robot.WeldingSetVoltageRelation(10, 45, 1, 10, 1);

            robot.WeldingSetVoltage(0, 25, 1, 0);// ----设置电压
            robot.WeldingSetCurrent(0, 260, 0, 0);// ----设置电流

            robot.MoveJ(safejointPos, safedescPose, 1, 0, 5, 100, 100, exaxisPos, -1, 0, offdese);

            int rtn = robot.WeldingSetCurrentGradualChangeStart(0, 260, 220, 0, 0);
            Console.WriteLine($"WeldingSetCurrentGradualChangeStart rtn is {rtn}");
            rtn = robot.WeldingSetVoltageGradualChangeStart(0, 25, 22, 1, 0);
            Console.WriteLine($"WeldingSetVoltageGradualChangeStart rtn is {rtn}");

            rtn = robot.ArcWeldTraceControl(1, 0, 1, 0.08, 5, 5, 300, 1, 0.06, 4, 4, 300, 1, 0, 4, 1, 10, 0, 0);
            Console.WriteLine($"ArcWeldTraceControl rtn is {rtn}");

            robot.MoveJ(startjointPos, startdescPose, 1, 0, 5, 100, 100, exaxisPos, -1, 0, offdese);

            robot.ARCStart(0, 0, 10000);
            robot.WeaveStart(0);
            rtn = robot.WeaveChangeStart(2, 1, 24, 36);
            Console.WriteLine($"WeaveChangeStart rtn is {rtn}");
            robot.MoveL(endjointPos, enddescPose, 1, 0, 100, 100, 2, -1, 0, exaxisPos, 0, 0, offdese);
            robot.ARCEnd(0, 0, 10000);
            robot.WeaveChangeEnd();
            robot.WeaveEnd(0);
            robot.ArcWeldTraceControl(0, 0, 1, 0.08, 5, 5, 300, 1, 0.06, 4, 4, 300, 1, 0, 4, 1, 10, 0, 0);
            robot.WeldingSetCurrentGradualChangeEnd();
            robot.WeldingSetVoltageGradualChangeEnd();

        }

        private void button10_Click(object sender, EventArgs e)
        {

            DescPose startdescPose = new DescPose(-319.303, -240.689, 116.379, -175.879, -0.337, 148.239);
            JointPos startjointPos = new JointPos(20.474, -103.554, 126.774, -116.682, -87.746, -37.709);

            DescPose enddescPose = new DescPose(-454.166, -327.159, 62.217, 177.199, -2.276, 154.955);
            JointPos endjointPos = new JointPos(27.176, -74.423, 104.557, -119.315, -93.514, -37.698);

            DescPose safedescPose = new DescPose(-375.533, -543.319, 19.798, 177.486, -2.489, 175.825);
            JointPos safejointPos = new JointPos(48.074, -59.714, 89.955, -119.777, -93.508, -37.683);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            robot.MoveJ(startjointPos, startdescPose, 1, 0, 5, 100, 100, exaxisPos, -1, 0, offdese);

            robot.ARCStart(0, 0, 10000);
            robot.WeaveStart(0);
            int rtn = robot.WeaveChangeStart(2, 1, 24, 36);


            Console.WriteLine($"CustomCollisionDetectionStart rtn is {rtn}");
            //LoadTrajectoryLA

            //int[] safety = { 5, 5, 5, 5, 5, 5 };
            //robot.SetCollisionStrategy(3, 1000, 150, 250, safety);

            //double[] jointDetectionThreshold = { 0.3, 0.3, 0.3, 0.3, 0.3, 0.3 };
            //double[] tcpDetectionThreshold = { 80, 80, 80, 80, 80, 80 };
            //rtn = robot.CustomCollisionDetectionStart(3, jointDetectionThreshold, tcpDetectionThreshold, 0);
            //Console.WriteLine($"CustomCollisionDetectionStart rtn is {rtn}");
            //robot.AccSmoothStart(saveFlag);
        }

        private void button11_Click(object sender, EventArgs e)
        {

            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
            int state = 0;
            while (true)
            {
                int rtn = robot.GetSmarttoolBtnState(ref state);
                string binaryString = Convert.ToString(state, 2).PadLeft(32, '0'); // 转换为32位二进制字符串
                Console.WriteLine($"GetSmarttoolBtnState rtn (binary): {binaryString}");
                Thread.Sleep(100);
            }

        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            DescPose DP1 = new DescPose(-324.688, -512.411, 319.936, 177.834, -13.926, -123.378);
            JointPos JP1 = new JointPos(47.944, -74.115, 99.306, -129.280, -90.062, -98.421);
            robot.GetForwardKin(JP1, ref DP1);
            DescPose DP2 = new DescPose(-387.275, -328.129, 340.563, -159.121, 16.169, -174.292);
            JointPos JP2 = new JointPos(23.798, -86.390, 105.682, -100.633, -65.192, -70.820);
            robot.GetForwardKin(JP2, ref DP2);
            DescPose DP3 = new DescPose(-492.692, -49.563, 375.256, 161.781, -14.476, 159.830);
            JointPos JP3 = new JointPos(-1.812, -89.883, 108.067, -116.040, -111.809, -70.825);
            robot.GetForwardKin(JP3, ref DP3);
            DescPose DP4 = new DescPose(-432.689, -287.194, 305.739, -177.999, 1.920, -177.450);
            JointPos JP4 = new JointPos(21.721, -83.395, 108.235, -113.684, -87.480, -70.821);
            robot.GetForwardKin(JP4, ref DP4);
            DescPose DP5 = new DescPose(-232.690, -287.193, 305.746, -177.999, 1.919, -177.450);
            JointPos JP5 = new JointPos(34.158, -105.217, 128.305, -112.503, -87.290, -58.372);
            robot.GetForwardKin(JP5, ref DP5);
            DescPose DP6 = new DescPose(-232.695, -487.192, 305.744, -177.999, 1.919, -177.452);
            JointPos JP6 = new JointPos(53.031, -80.893, 105.748, -115.179, -87.247, -39.476);
            robot.GetForwardKin(JP6, ref DP6);
            JointPos JP7 = new JointPos(38.933, -66.532, 86.532, -109.644, -87.251, -53.590);
            DescPose DP7 = new DescPose(-432.695, -487.196, 305.749, -177.999, 1.918, -177.452);
            robot.GetForwardKin(JP7, ref DP7);
            JointPos JP8 = new JointPos(42.245, -82.011, 99.838, -116.087, -69.438, -70.824);
            DescPose DP8 = new DescPose(-315.138, -471.802, 373.506, -157.941, -1.233, -155.671);
            robot.GetForwardKin(JP8, ref DP8);
            DescPose DP9 = new DescPose(-513.450, -302.627, 402.163, 171.249, -16.204, -176.411);
            JointPos JP9 = new JointPos(22.919, -78.425, 92.035, -116.080, -103.583, -70.913);
            robot.GetForwardKin(JP9, ref DP9);
            DescPose DP10 = new DescPose(-428.141, -188.113, 351.314, 176.576, -19.670, 142.831);
            JointPos JP10 = new JointPos(14.849, -92.942, 114.901, -121.601, -107.553, -38.881);
            robot.GetForwardKin(JP10, ref DP10);
            DescPose DP11 = new DescPose(-587.412, -70.091, 370.337, 177.676, -23.575, 127.293);
            JointPos JP11 = new JointPos(0.209, -77.444, 96.217, -121.606, -110.075, -38.879);
            robot.GetForwardKin(JP11, ref DP11);
            JointPos JP12 = new JointPos(-21.947, -88.425, 108.395, -111.062, -77.881, -38.879);
            DescPose DP12 = new DescPose(-498.493, 67.966, 345.644, -171.472, 8.710, 107.699);
            robot.GetForwardKin(JP12, ref DP12);
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.MoveJ(JP1, DP1, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.MoveJ(JP2, DP2, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            robot.MoveJ(JP3, DP3, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            robot.MoveJ(JP4, DP4, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);

            robot.MoveL(JP5, DP5, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            robot.MoveL(JP6, DP6, 0, 0, 100, 100, 100, 20, 1, exaxisPos, 0, 0, offdese);
            robot.MoveL(JP7, DP7, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);

            robot.MoveJ(JP8, DP8, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            //robot.MoveC(JP9, DP9, 0, 0, 100, 100, exaxisPos, 0, offdese, JP10, DP10, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 30);
            //robot.MoveC(JP11, DP11, 0, 0, 100, 100, exaxisPos, 0, offdese, JP12, DP12, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1);

        }

        private void button14_Click(object sender, EventArgs e)
        {
            byte status = 1;
            byte smooth = 0;
            byte block = 0;
            //byte di = 0, tool_di = 0;
            //float ai = 0.0f, tool_ai = 0.0f;


            for (int i = 0; i < 16; i++)
            {
                robot.SetDO(i, status, smooth, block);
                Thread.Sleep(300);
            }

            status = 0;

            for (int i = 0; i < 16; i++)
            {
                robot.SetDO(i, status, smooth, block);
                Thread.Sleep(300);
            }

            status = 1;

            for (int i = 0; i < 2; i++)
            {
                robot.SetToolDO(i, status, smooth, block);
                Thread.Sleep(1000);
            }

            status = 0;

            for (int i = 0; i < 2; i++)
            {
                robot.SetToolDO(i, status, smooth, block);
                Thread.Sleep(1000);
            }

            for (int i = 0; i < 100; i++)
            {
                robot.SetAO(0, i, block);
                Thread.Sleep(30);
            }

            for (int i = 0; i < 100; i++)
            {
                robot.SetToolAO(0, i, block);
                Thread.Sleep(30);
            }

        }

        private void button15_Click(object sender, EventArgs e)
        {
            byte block = 0;
            byte di = 0, tool_di = 0;
            float ai = 0.0f, tool_ai = 0.0f;

            robot.GetDI(0, block, ref di);
            Console.WriteLine($"di0: {di}");

            tool_di = (byte)robot.GetToolDI(1, block, ref tool_di);
            Console.WriteLine($"tool_di1: {tool_di}");

            robot.GetAI(0, block, ref ai);
            Console.WriteLine($"ai0: {ai}");

            tool_ai = robot.GetToolAI(0, block, ref tool_ai);
            Console.WriteLine($"tool_ai0: {tool_ai}");

            byte _button_state = 0;
            robot.GetAxlePointRecordBtnState(ref _button_state);
            Console.WriteLine($"_button_state is: {_button_state}");

            byte tool_do_state = 0;
            robot.GetToolDO(ref tool_do_state);
            Console.WriteLine($"tool DO state is: {tool_do_state}");

            int do_state_h = 0;
            int do_state_l = 0;
            robot.GetDO(ref do_state_h, ref do_state_l);
            Console.WriteLine($"DO state high is: {do_state_h}\n DO state low is: {do_state_l}");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            //byte di = 0, tool_di = 0;
            //float ai = 0.0f, tool_ai = 0.0f;

            int rtn = robot.WaitDI(0, 1, 1000, 1);
            Console.WriteLine("WaitDI over; rtn is: " + rtn);

            robot.WaitMultiDI(1, 3, 3, 1000, 1);
            Console.WriteLine("WaitMultiDI over; rtn is: " + rtn);

            robot.WaitToolDI(1, 1, 1000, 1);
            Console.WriteLine("WaitToolDI over; rtn is: " + rtn);

            robot.WaitAI(0, 0, 50, 1000, 1);
            Console.WriteLine("WaitAI over; rtn is: " + rtn);

            robot.WaitToolAI(0, 0, 50, 1000, 1);
            Console.WriteLine("WaitToolAI over; rtn is: " + rtn);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < 16; i++)
            //{
            //    robot.SetDO(i, 1, 0, 0);
            //    Thread.Sleep(300);
            //}

            //int resetFlag = 1;
            //int rtn = robot.SetOutputResetCtlBoxDO(resetFlag);
            //robot.SetOutputResetCtlBoxAO(resetFlag);
            //robot.SetOutputResetAxleDO(resetFlag);
            //robot.SetOutputResetAxleAO(resetFlag);
            //robot.SetOutputResetExtDO(resetFlag);
            //robot.SetOutputResetExtAO(resetFlag);
            //robot.SetOutputResetSmartToolDO(resetFlag);

            //robot.ProgramLoad("/fruser/Text1.lua");
            //robot.ProgramRun();

        }

        private void button18_Click(object sender, EventArgs e)
        {
            DescPose p1Desc = new DescPose(186.331f, 487.913f, 209.850f, 149.030f, 0.688f, -114.347f);
            JointPos p1Joint = new JointPos(-127.876f, -75.341f, 115.417f, -122.741f, -59.820f, 74.300f);

            DescPose p2Desc = new DescPose(69.721f, 535.073f, 202.882f, -144.406f, -14.775f, -89.012f);
            JointPos p2Joint = new JointPos(-101.780f, -69.828f, 110.917f, -125.740f, -127.841f, 74.300f);

            DescPose p3Desc = new DescPose(146.861f, 578.426f, 205.598f, 175.997f, -36.178f, -93.437f);
            JointPos p3Joint = new JointPos(-112.851f, -60.191f, 86.566f, -80.676f, -97.463f, 74.300f);

            DescPose p4Desc = new DescPose(136.284f, 509.876f, 225.613f, 178.987f, 1.372f, -100.696f);
            JointPos p4Joint = new JointPos(-116.397f, -76.281f, 113.845f, -128.611f, -88.654f, 74.299f);

            DescPose p5Desc = new DescPose(138.395f, 505.972f, 298.016f, 179.134f, 2.147f, -101.110f);
            JointPos p5Joint = new JointPos(-116.814f, -82.333f, 109.162f, -118.662f, -88.585f, 74.302f);

            DescPose p6Desc = new DescPose(105.553f, 454.325f, 232.017f, -179.426f, 0.444f, -99.952f);
            JointPos p6Joint = new JointPos(-115.649f, -84.367f, 122.447f, -128.663f, -90.432f, 74.303f);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            JointPos[] posJ = new JointPos[] { p1Joint, p2Joint, p3Joint, p4Joint, p5Joint, p6Joint };
            DescPose coordRtn = new DescPose();
            int rtn = robot.ComputeToolCoordWithPoints(1, posJ, ref coordRtn);
            Console.WriteLine($"ComputeToolCoordWithPoints    {rtn}  coord is {coordRtn.tran.x} {coordRtn.tran.y} {coordRtn.tran.z} {coordRtn.rpy.rx} {coordRtn.rpy.ry} {coordRtn.rpy.rz}");

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
            rtn = robot.ComputeTool(ref coordRtn);
            Console.WriteLine($"6 Point ComputeTool        {rtn}  coord is {coordRtn.tran.x} {coordRtn.tran.y} {coordRtn.tran.z} {coordRtn.rpy.rx} {coordRtn.rpy.ry} {coordRtn.rpy.rz}");
            robot.SetToolList(1, coordRtn, 0, 0, 0);

            robot.MoveJ(p1Joint, p1Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetTcp4RefPoint(1);
            robot.MoveJ(p2Joint, p2Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetTcp4RefPoint(2);
            robot.MoveJ(p3Joint, p3Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetTcp4RefPoint(3);
            robot.MoveJ(p4Joint, p4Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetTcp4RefPoint(4);
            rtn = robot.ComputeTcp4(ref coordRtn);
            Console.WriteLine($"4 Point ComputeTool        {rtn}  coord is {coordRtn.tran.x} {coordRtn.tran.y} {coordRtn.tran.z} {coordRtn.rpy.rx} {coordRtn.rpy.ry} {coordRtn.rpy.rz}");

            robot.SetToolCoord(2, coordRtn, 0, 0, 1, 0);

            DescPose getCoord = new DescPose();
            rtn = robot.GetTCPOffset(0, ref getCoord);
            Console.WriteLine($"GetTCPOffset    {rtn}  coord is {coordRtn.tran.x} {coordRtn.tran.y} {coordRtn.tran.z} {coordRtn.rpy.rx} {coordRtn.rpy.ry} {coordRtn.rpy.rz}");
        }

        private void button19_Click(object sender, EventArgs e)
        {
            DescPose p1Desc = new DescPose(-89.606, 779.517, 193.516, 178.000, 0.476, -92.484);
            JointPos p1Joint = new JointPos(-108.145, -50.137, 85.818, -125.599, -87.946, 74.329);

            DescPose p2Desc = new DescPose(-24.656, 850.384, 191.361, 177.079, -2.058, -95.355);
            JointPos p2Joint = new JointPos(-111.024, -41.538, 69.222, -114.913, -87.743, 74.329);

            DescPose p3Desc = new DescPose(-99.813, 766.661, 241.878, -176.817, 1.917, -91.604);
            JointPos p3Joint = new JointPos(-107.266, -56.116, 85.971, -122.560, -92.548, 74.331);

            robot.GetForwardKin(p1Joint, ref p1Desc);
            robot.GetForwardKin(p2Joint, ref p2Desc);
            robot.GetForwardKin(p3Joint, ref p3Desc);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            DescPose[] posTCP = new DescPose[] { p1Desc, p2Desc, p3Desc };
            DescPose coordRtn = new DescPose();
            int rtn = robot.ComputeWObjCoordWithPoints(1, posTCP, 0, ref coordRtn);
            Console.WriteLine($"ComputeWObjCoordWithPoints    {rtn}  coord is {coordRtn.tran.x} {coordRtn.tran.y} {coordRtn.tran.z} {coordRtn.rpy.rx} {coordRtn.rpy.ry} {coordRtn.rpy.rz}");

            robot.MoveJ(p1Joint, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetWObjCoordPoint(1);
            robot.MoveJ(p2Joint, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetWObjCoordPoint(2);
            robot.MoveJ(p3Joint, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetWObjCoordPoint(3);
            rtn = robot.ComputeWObjCoord(1, 0, ref coordRtn);
            Console.WriteLine($"ComputeWObjCoord   {rtn}  coord is {coordRtn.tran.x} {coordRtn.tran.y} {coordRtn.tran.z} {coordRtn.rpy.rx} {coordRtn.rpy.ry} {coordRtn.rpy.rz}");

            robot.SetWObjCoord(1, coordRtn, 0);
            robot.SetWObjList(1, coordRtn, 0);

            DescPose getWobjDesc = new DescPose();
            rtn = robot.GetWObjOffset(0, ref getWobjDesc);
            Console.WriteLine($"GetWObjOffset                   {rtn}  coord is {coordRtn.tran.x} {coordRtn.tran.y} {coordRtn.tran.z} {coordRtn.rpy.rx} {coordRtn.rpy.ry} {coordRtn.rpy.rz}");

        }

        private void button20_Click(object sender, EventArgs e)
        {
            DescPose p1Desc = new DescPose(-89.606f, 779.517f, 193.516f, 178.000f, 0.476f, -92.484f);
            JointPos p1Joint = new JointPos(-108.145f, -50.137f, 85.818f, -125.599f, -87.946f, 74.329f);

            DescPose p2Desc = new DescPose(-24.656f, 850.384f, 191.361f, 177.079f, -2.058f, -95.355f);
            JointPos p2Joint = new JointPos(-111.024f, -41.538f, 69.222f, -114.913f, -87.743f, 74.329f);

            DescPose p3Desc = new DescPose(-99.813f, 766.661f, 241.878f, -176.817f, 1.917f, -91.604f);
            JointPos p3Joint = new JointPos(-107.266f, -56.116f, 85.971f, -122.560f, -92.548f, 74.331f);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            DescPose[] posTCP = new DescPose[] { p1Desc, p2Desc, p3Desc };
            DescPose coordRtn = new DescPose();

            robot.MoveJ(p1Joint, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetExTCPPoint(1);
            robot.MoveJ(p2Joint, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetExTCPPoint(2);
            robot.MoveJ(p3Joint, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetExTCPPoint(3);
            int rtn = robot.ComputeExTCF(ref coordRtn);
            Console.WriteLine($"ComputeExTCF                   {rtn}  coord is {coordRtn.tran.x} {coordRtn.tran.y} {coordRtn.tran.z} {coordRtn.rpy.rx} {coordRtn.rpy.ry} {coordRtn.rpy.rz}");

            robot.SetExToolCoord(1, coordRtn, offdese);
            robot.SetExToolList(1, coordRtn, offdese);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < 100; i++)
            {
                robot.SetSpeed(i);
                robot.SetOaccScale(i);
                Thread.Sleep(30);
            }

            double defaultVel = 0.0f;
            robot.GetDefaultTransVel(ref defaultVel);
            Console.WriteLine($"GetDefaultTransVel is {defaultVel}");

            for (int i = 1; i < 21; i++)
            {
                robot.SetSysVarValue(i, i + 0.5f);
                Thread.Sleep(100);
            }

            for (int i = 1; i < 21; i++)
            {
                double value = 0;
                robot.GetSysVarValue(i, ref value);
                Console.WriteLine($"sys value  {i} is :{value}");
                Thread.Sleep(100);
            }

            robot.SetLoadWeight(0, 2.5f);

            DescTran loadCoord = new DescTran();
            loadCoord.x = 3.0f;
            loadCoord.y = 4.0f;
            loadCoord.z = 5.0f;
            robot.SetLoadCoord(loadCoord);

            Thread.Sleep(1000);

            double getLoad = 0.0f;
            robot.GetTargetPayload(0, ref getLoad);

            DescTran getLoadTran = new DescTran();
            robot.GetTargetPayloadCog(0, ref getLoadTran);
            Console.WriteLine($"get load is {getLoad}; get load cog is {getLoadTran.x} {getLoadTran.y} {getLoadTran.z}");

            robot.SetRobotInstallPos(0);
            robot.SetRobotInstallAngle(15.0f, 25.0f);

            double anglex = 0.0f;
            double angley = 0.0f;
            robot.GetRobotInstallAngle(ref anglex, ref angley);
            Console.WriteLine($"GetRobotInstallAngle x:  {anglex};  y:  {angley}");
        }

        private void button22_Click(object sender, EventArgs e)
        {
            double[] lcoeff = { 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f };
            double[] wcoeff = { 0.4f, 0.4f, 0.4f, 0.4f, 0.4f, 0.4f };
            double[] ccoeff = { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f };
            double[] fcoeff = { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f };

            int rtn = robot.FrictionCompensationOnOff(1);
            Console.WriteLine($"FrictionCompensationOnOff rtn is{rtn}");

            rtn = robot.SetFrictionValue_level(lcoeff);
            Console.WriteLine($"SetFrictionValue_level rtn is {rtn}");

            rtn = robot.SetFrictionValue_wall(wcoeff);
            Console.WriteLine($"SetFrictionValue_wall rtn is{rtn}");

            rtn = robot.SetFrictionValue_ceiling(ccoeff);
            Console.WriteLine($"SetFrictionValue_ceiling rtn is {rtn}");

            rtn = robot.SetFrictionValue_freedom(fcoeff);
            Console.WriteLine($"SetFrictionValue_freedom rtn is {rtn}");
        }

        private void button23_Click(object sender, EventArgs e)
        {
            int maincode = 0, subcode = 0;
            robot.GetRobotErrorCode(ref maincode, ref subcode);
            Console.WriteLine($"robot maincode is{maincode};  subcode is {subcode}");

            robot.ResetAllError();

            Thread.Sleep(1000);

            robot.GetRobotErrorCode(ref maincode, ref subcode);
            Console.WriteLine($"robot maincode is{maincode};  subcode is{subcode}");
        }

        private void button24_Click(object sender, EventArgs e)
        {
            int mode = 0;
            int config = 1;
            double[] level1 = { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f };
            double[] level2 = { 0.50f, 0.20f, 0.30f, 0.40f, 0.50f, 0.60f };

            int rtn = robot.SetAnticollision(mode, level1, config);
            Console.WriteLine($"SetAnticollision mode 0 rtn is {rtn}");
            mode = 1;
            rtn = robot.SetAnticollision(mode, level2, config);
            Console.WriteLine($"SetAnticollision mode 1 rtn is {rtn}");

            JointPos p1Joint = new JointPos(-11.904f, -99.669f, 117.473f, -108.616f, -91.726f, 74.256f);
            JointPos p2Joint = new JointPos(-45.615f, -106.172f, 124.296f, -107.151f, -91.282f, 74.255f);

            DescPose p1Desc = new DescPose(-419.524f, -13.000f, 351.569f, -178.118f, 0.314f, 3.833f);
            DescPose p2Desc = new DescPose(-321.222f, 185.189f, 335.520f, -179.030f, -1.284f, -29.869f);

            ExaxisPos exaxisPos = new ExaxisPos(0.0f, 0.0f, 0.0f, 0.0f);
            DescPose offdese = new DescPose(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
            robot.MoveL(p2Joint, p2Desc, 0, 0, 100, 100, 100, 2, 0, exaxisPos, 0, 0, offdese);
            robot.ResetAllError();
            int[] safety = { 5, 5, 5, 5, 5, 5 };
            rtn = robot.SetCollisionStrategy(3, 1000, 150, 250, safety);
            Console.WriteLine($"SetCollisionStrategy rtn is {rtn}");

            double[] jointDetectionThreshould = { 0.1, 0.1, 0.1, 0.1, 0.1, 0.1 };
            double[] tcpDetectionThreshould = { 60, 60, 60, 60, 60, 60 };
            rtn = robot.CustomCollisionDetectionStart(3, jointDetectionThreshould, tcpDetectionThreshould, 0);
            Console.WriteLine($"CustomCollisionDetectionStart rtn is {rtn}");

            robot.MoveL(p1Joint, p1Desc, 0, 0, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            robot.MoveL(p2Joint, p2Desc, 0, 0, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            rtn = robot.CustomCollisionDetectionEnd();
            Console.WriteLine($"CustomCollisionDetectionEnd rtn is {rtn}");
        }

        private void button25_Click(object sender, EventArgs e)
        {
            double[] plimit = { 170.0f, 80.0f, 150.0f, 80.0f, 170.0f, 160.0f };
            robot.SetLimitPositive(plimit);
            double[] nlimit = { -170.0f, -260.0f, -150.0f, -260.0f, -170.0f, -160.0f };
            robot.SetLimitNegative(nlimit);

            double[] neg_deg = new double[6] { 0, 0, 0, 0, 0, 0 };
            double[] pos_deg = new double[6] { 0, 0, 0, 0, 0, 0 };
            robot.GetJointSoftLimitDeg(0, ref neg_deg, ref pos_deg);
            Console.WriteLine($"neg limit deg:{neg_deg[0]},{neg_deg[1]},{neg_deg[2]},{neg_deg[3]},{neg_deg[4]},{neg_deg[5]}");
            Console.WriteLine($"pos limit deg:{pos_deg[0]},{pos_deg[1]},{pos_deg[2]},{pos_deg[3]},{pos_deg[4]},{pos_deg[5]}");
        }

        private void button26_Click(object sender, EventArgs e)
        {
            int rtn = robot.SetCollisionDetectionMethod(0, 0);

            rtn = robot.SetStaticCollisionOnOff(1);
            Console.WriteLine($"SetStaticCollisionOnOff On rtn is {rtn}");
            Thread.Sleep(5000);
            rtn = robot.SetStaticCollisionOnOff(0);
            Console.WriteLine($"SetStaticCollisionOnOff Off rtn is {rtn}");
        }

        private void button27_Click(object sender, EventArgs e)
        {
            robot.DragTeachSwitch(1);
            robot.SetPowerLimit(1, 200);
            double[] torques = { 0, 0, 0, 0, 0, 0 };
            robot.GetJointTorques(1, torques);

            int count = 100;
            robot.ServoJTStart();
            int error = 0;
            while (count > 0)
            {
                error = robot.ServoJT(torques, 0.001f);
                count--;
                Thread.Sleep(1);
            }
            error = robot.ServoJTEnd();
            robot.DragTeachSwitch(0);
        }

        private void button28_Click(object sender, EventArgs e)
        {

            robot.DragTeachSwitch(1);
            double[] torques = { 0, 0, 0, 0, 0, 0 };
            robot.GetJointTorques(1, torques);

            int count = 100;
            robot.ServoJTStart(); //   #servoJT开始
            int error = 0;
            while (count > 0)
            {
                error = robot.ServoJT(torques, 0.001);
                count = count - 1;
                Thread.Sleep(1);
            }
            error = robot.ServoJTEnd();
            robot.DragTeachSwitch(0);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
            double yangle = 0, zangle = 0;
            robot.GetRobotInstallAngle(ref yangle, ref zangle);
            Console.WriteLine($"yangle:{yangle},zangle:{zangle}");

            JointPos j_deg = new JointPos(0, 0, 0, 0, 0, 0);
            robot.GetActualJointPosDegree(0, ref j_deg);
            Console.WriteLine($"joint pos deg:{j_deg.jPos[0]},{j_deg.jPos[1]},{j_deg.jPos[2]},{j_deg.jPos[3]},{j_deg.jPos[4]},{j_deg.jPos[5]}");

            double[] jointSpeed = new double[6];
            robot.GetActualJointSpeedsDegree(0, ref jointSpeed);
            Console.WriteLine($"joint speeds deg:{jointSpeed[0]},{jointSpeed[1]},{jointSpeed[2]},{jointSpeed[3]},{jointSpeed[4]},{jointSpeed[5]}");

            double[] jointAcc = new double[6];
            robot.GetActualJointAccDegree(0, ref jointAcc);
            Console.WriteLine($"joint acc deg:{jointAcc[0]},{jointAcc[1]},{jointAcc[2]},{jointAcc[3]},{jointAcc[4]},{jointAcc[5]}");

            double tcp_speed = 0, ori_speed = 0;
            robot.GetTargetTCPCompositeSpeed(0, ref tcp_speed, ref ori_speed);
            Console.WriteLine($"GetTargetTCPCompositeSpeed tcp {tcp_speed}; ori {ori_speed}");

            robot.GetActualTCPCompositeSpeed(0, ref tcp_speed, ref ori_speed);
            Console.WriteLine($"GetActualTCPCompositeSpeed tcp {tcp_speed}; ori {ori_speed}");

            double[] targetSpeed = new double[6];
            robot.GetTargetTCPSpeed(0, ref targetSpeed);
            Console.WriteLine($"GetTargetTCPSpeed {targetSpeed[0]},{targetSpeed[1]},{targetSpeed[2]},{targetSpeed[3]},{targetSpeed[4]},{targetSpeed[5]}");

            double[] actualSpeed = new double[6];
            robot.GetActualTCPSpeed(0, ref actualSpeed);
            Console.WriteLine($"GetTargetTCPSpeed {actualSpeed[0]},{actualSpeed[1]},{actualSpeed[2]},{actualSpeed[3]},{actualSpeed[4]},{actualSpeed[5]}");

            DescPose tcp = new DescPose(0, 0, 0, 0, 0, 0);
            robot.GetActualTCPPose(0, ref tcp);
            Console.WriteLine($"tcp pose:{tcp.tran.x},{tcp.tran.y},{tcp.tran.z},{tcp.rpy.rx},{tcp.rpy.ry},{tcp.rpy.rz}");

            DescPose flange = new DescPose(0, 0, 0, 0, 0, 0);
            robot.GetActualToolFlangePose(0, ref flange);
            Console.WriteLine($"flange pose:{flange.tran.x},{flange.tran.y},{flange.tran.z},{flange.rpy.rx},{flange.rpy.ry},{flange.rpy.rz}");

            int id = 0;
            robot.GetActualTCPNum(0, ref id);
            Console.WriteLine($"tcp num:{id}");

            robot.GetActualWObjNum(0, ref id);
            Console.WriteLine($"wobj num:{id}");

            double[] jtorque = new double[6];
            robot.GetJointTorques(0, jtorque);
            Console.WriteLine($"torques:{jtorque[0]},{jtorque[1]},{jtorque[2]},{jtorque[3]},{jtorque[4]},{jtorque[5]}");

            double t_ms = 0;
            robot.GetSystemClock(ref t_ms);
            Console.WriteLine($"system clock:{t_ms}");

            int config = 0;
            robot.GetRobotCurJointsConfig(ref config);
            Console.WriteLine($"joint config:{config}");

            byte motionDone = 0;
            robot.GetRobotMotionDone(ref motionDone);
            Console.WriteLine($"GetRobotMotionDone :{motionDone}");

            int len = 0;
            robot.GetMotionQueueLength(ref len);
            Console.WriteLine($"GetMotionQueueLength :{len}");

            byte emergState = 0;
            robot.GetRobotEmergencyStopState(ref emergState);
            Console.WriteLine($"GetRobotEmergencyStopState :{emergState}");

            int comstate = 0;
            robot.GetSDKComState(ref comstate);
            Console.WriteLine($"GetSDKComState :{comstate}");

            byte si0_state = 0, si1_state = 0;
            robot.GetSafetyStopState(ref si0_state, ref si1_state);
            Console.WriteLine($"GetSafetyStopState :{si0_state} {si1_state}");

            double[] temp = new double[6];
            robot.GetJointDriverTemperature(temp);
            Console.WriteLine($"Temperature:{temp[0]},{temp[1]},{temp[2]},{temp[3]},{temp[4]},{temp[5]}");

            double[] torque = new double[6];
            robot.GetJointDriverTorque(torque);
            Console.WriteLine($"torque:{torque[0]},{torque[1]},{torque[2]},{torque[3]},{torque[4]},{torque[5]}");

            robot.GetRobotRealTimeState(ref pkg);
        }



        private void button30_Click(object sender, EventArgs e)
        {
            JointPos j1 = new JointPos(-11.904f, -99.669f, 117.473f, -108.616f, -91.726f, 74.256f);
            DescPose desc_pos1 = new DescPose(-419.524f, -13.000f, 351.569f, -178.118f, 0.314f, 3.833f);

            JointPos inverseRtn = new JointPos(0, 0, 0, 0, 0, 0);

            robot.GetInverseKin(0, desc_pos1, -1, ref inverseRtn);
            Console.WriteLine($"dcs1 GetInverseKin rtn is {inverseRtn.jPos[0]} {inverseRtn.jPos[1]} {inverseRtn.jPos[2]} {inverseRtn.jPos[3]} {inverseRtn.jPos[4]} {inverseRtn.jPos[5]}");
            robot.GetInverseKinRef(0, desc_pos1, j1, ref inverseRtn);
            Console.WriteLine($"dcs1 GetInverseKinRef rtn is {inverseRtn.jPos[0]} {inverseRtn.jPos[1]} {inverseRtn.jPos[2]} {inverseRtn.jPos[3]} {inverseRtn.jPos[4]} {inverseRtn.jPos[5]}");

            bool hasResut = false;
            robot.GetInverseKinHasSolution(0, desc_pos1, j1, ref hasResut);
            Console.WriteLine($"dcs1 GetInverseKinRef result {hasResut}");

            DescPose forwordResult = new DescPose(0, 0, 0, 0, 0, 0);
            robot.GetForwardKin(j1, ref forwordResult);
            Console.WriteLine($"jpos1 forwordResult rtn is {forwordResult.tran.x} {forwordResult.tran.y} {forwordResult.tran.z} {forwordResult.rpy.rx} {forwordResult.rpy.ry} {forwordResult.rpy.rz}");
        }

        private void button31_Click(object sender, EventArgs e)
        {
            string name = "p1";
            double[] data = new double[20];
            int rtn = robot.GetRobotTeachingPoint(name, ref data);
            Console.WriteLine(" {0} name is: {1} \n", rtn, name);
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine("data is: {0} \n", data[i]);
            }

            int que_len = 0;
            rtn = robot.GetMotionQueueLength(ref que_len);
            Console.WriteLine("GetMotionQueueLength rtn is: {0}, queue length is: {1} \n", rtn, que_len);

            double[] dh = { 0, 0, 0, 0, 0, 0 };
            int retval = 0;
            retval = robot.GetDHCompensation(ref dh);
            Console.WriteLine($"retval is  {retval}");
            Console.WriteLine($"dh is {dh[0]}, {dh[1]}, {dh[2]}, {dh[3]}, {dh[4]}, {dh[5]}");
            string SN = "";
            robot.GetRobotSN(ref SN);
            Console.WriteLine($"robot SN is  {SN}");
        }

        private void button32_Click(object sender, EventArgs e)
        {
            int type = 1;
            string name = "tpd2025";
            int period_ms = 4;
            ushort di_choose = 0;
            ushort do_choose = 0;

            robot.SetTPDParam(type, name, period_ms, di_choose, do_choose);

            robot.Mode(1);
            Thread.Sleep(1000);
            robot.DragTeachSwitch(1);
            robot.SetTPDStart(type, name, period_ms, di_choose, do_choose);
            Thread.Sleep(10000);
            robot.SetWebTPDStop();
            robot.DragTeachSwitch(0);

            float ovl = 100.0f;
            byte blend = 0;

            DescPose start_pose = new DescPose();

            int rtn = robot.LoadTPD(name);
            Console.WriteLine("LoadTPD rtn is: {0}\n", rtn);

            robot.GetTPDStartPose(name, ref start_pose);
            Console.WriteLine("start pose, xyz is: {0} {1} {2}. rpy is: {3} {4} {5} \n",
                start_pose.tran.x, start_pose.tran.y, start_pose.tran.z,
                start_pose.rpy.rx, start_pose.rpy.ry, start_pose.rpy.rz);
            robot.MoveCart(start_pose, 0, 0, 100, 100, ovl, -1, -1);
            Thread.Sleep(1000);

            rtn = robot.MoveTPD(name, blend, ovl);
            Console.WriteLine("MoveTPD rtn is: {0}\n", rtn);
            Thread.Sleep(5000);

            robot.SetTPDDelete(name);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            int rtn = robot.TrajectoryJUpLoad("D://zUP/trajHelix_aima_1.txt");
            Console.WriteLine("Upload TrajectoryJ A {0}\n", rtn);

            string traj_file_name = "trajHelix_aima_1.txt";
            rtn = robot.LoadTrajectoryJ(traj_file_name, 100, 1);
            Console.WriteLine("LoadTrajectoryJ {0}, rtn is: {1}\n", traj_file_name, rtn);

            DescPose traj_start_pose = new DescPose();
            rtn = robot.GetTrajectoryStartPose(traj_file_name, ref traj_start_pose);
            Console.WriteLine("GetTrajectoryStartPose is: {0}\n", rtn);
            Console.WriteLine("desc_pos:{0},{1},{2},{3},{4},{5}\n",
                traj_start_pose.tran.x, traj_start_pose.tran.y, traj_start_pose.tran.z,
                traj_start_pose.rpy.rx, traj_start_pose.rpy.ry, traj_start_pose.rpy.rz);

            Thread.Sleep(1000);

            robot.SetSpeed(50);
            robot.MoveCart(traj_start_pose, 0, 0, 100, 100, 100, -1, -1);

            int traj_num = 0;
            rtn = robot.GetTrajectoryPointNum(ref traj_num);
            Console.WriteLine("GetTrajectoryStartPose rtn is: {0}, traj num is: {1}\n", rtn, traj_num);

            rtn = robot.SetTrajectoryJSpeed(50.0f);
            Console.WriteLine("SetTrajectoryJSpeed is: {0}\n", rtn);

            ForceTorque traj_force = new ForceTorque();
            traj_force.fx = 10;
            rtn = robot.SetTrajectoryJForceTorque(traj_force);
            Console.WriteLine("SetTrajectoryJForceTorque rtn is: {0}\n", rtn);

            rtn = robot.SetTrajectoryJForceFx(10.0f);
            Console.WriteLine("SetTrajectoryJForceFx rtn is: {0}\n", rtn);

            rtn = robot.SetTrajectoryJForceFy(0.0f);
            Console.WriteLine("SetTrajectoryJForceFy rtn is: {0}\n", rtn);

            rtn = robot.SetTrajectoryJForceFz(0.0f);
            Console.WriteLine("SetTrajectoryJForceFz rtn is: {0}\n", rtn);

            rtn = robot.SetTrajectoryJTorqueTx(10.0f);
            Console.WriteLine("SetTrajectoryJTorqueTx rtn is: {0}\n", rtn);

            rtn = robot.SetTrajectoryJTorqueTy(10.0f);
            Console.WriteLine("SetTrajectoryJTorqueTy rtn is: {0}\n", rtn);

            rtn = robot.SetTrajectoryJTorqueTz(10.0f);
            Console.WriteLine("SetTrajectoryJTorqueTz rtn is: {0}\n", rtn);

            rtn = robot.MoveTrajectoryJ();
            Console.WriteLine("MoveTrajectoryJ rtn is: {0}\n", rtn);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            string program_name = "Text1.lua";
            string loaded_name = "";
            byte state = 0;
            int line = 0;

            robot.Mode(0);
            robot.LoadDefaultProgConfig(0, program_name);
            robot.ProgramLoad(program_name);
            robot.ProgramRun();
            Thread.Sleep(1000);
            robot.ProgramPause();
            robot.GetProgramState(ref state);
            Console.WriteLine("program state:{0}\n", state);
            robot.GetCurrentLine(ref line);
            Console.WriteLine("current line:{0}\n", line);
            robot.GetLoadedProgram(ref loaded_name);
            Console.WriteLine("program name:{0}\n", loaded_name);
            Thread.Sleep(1000);
            robot.ProgramResume();
            Thread.Sleep(1000);
            robot.ProgramStop();
            Thread.Sleep(1000);
        }

        private void button35_Click(object sender, EventArgs e)
        {
            int rtn;
            List<string> luaNames = new List<string>();
            rtn = robot.GetLuaList(ref luaNames);
            Console.WriteLine("res is: {0}", rtn);
            Console.WriteLine("size is: {0}", luaNames.Count);
            foreach (var name in luaNames)
            {
                Console.WriteLine(name);
            }
            rtn = robot.LuaDownLoad("TT.lua", "D://zDOWN/");
            Console.WriteLine("LuaDownLoad rtn is {0}", rtn);
            string errStr = "";
            Thread.Sleep(2000);

            rtn = robot.LuaUpload("D://zUP/airlab.lua", ref errStr);
            Console.WriteLine("LuaUpload rtn is {0}", errStr);
            Thread.Sleep(2000);
            rtn = robot.LuaDelete("TT.lua");
            Console.WriteLine("LuaDelete rtn is {0}", rtn);
        }

        private void button36_Click(object sender, EventArgs e)
        {
            int company = 6;
            int device = 1;
            int softversion = 0;
            int bus = 1;
            int index = 1;
            byte act = 0;
            int max_time = 30000;
            byte block = 0;
            int status = 0;
            int fault = 0;
            int active_status = 0;
            int current_pos = 0;
            int current = 0;
            int voltage = 0;
            int temp = 0;
            int speed = 0;

            robot.SetGripperConfig(company, device, softversion, bus);
            Thread.Sleep(1000);
            robot.GetGripperConfig(ref company, ref device, ref softversion, ref bus);
            Console.WriteLine("gripper config:{0},{1},{2},{3}\n", company, device, softversion, bus);

            robot.ActGripper(index, act);
            Thread.Sleep(1000);
            act = 1;
            robot.ActGripper(index, act);
            Thread.Sleep(4000);

            robot.MoveGripper(index, 0, 100, 100, max_time, block, 0, 0, 0, 0);
            Thread.Sleep(4000);
            robot.MoveGripper(index, 90, 100, 100, max_time, block, 0, 0, 0, 0);

            robot.GetGripperMotionDone(ref fault, ref status);
            Console.WriteLine("motion status:{0},{1}\n", fault, status);

            robot.GetGripperActivateStatus(ref fault, ref active_status);
            Console.WriteLine("gripper active fault is: {0}, status is: {1}\n", fault, active_status);

            robot.GetGripperCurPosition(ref fault, ref current_pos);
            Console.WriteLine("fault is:{0}, current position is: {1}\n", fault, current_pos);

            robot.GetGripperCurCurrent(ref fault, ref current);
            Console.WriteLine("fault is:{0}, current current is: {1}\n", fault, current);

            robot.GetGripperVoltage(ref fault, ref voltage);
            Console.WriteLine("fault is:{0}, current voltage is: {1} \n", fault, voltage);

            robot.GetGripperTemp(ref fault, ref temp);
            Console.WriteLine("fault is:{0}, current temperature is: {1}\n", fault, temp);

            robot.GetGripperCurSpeed(ref fault, ref speed);
            Console.WriteLine("fault is:{0}, current speed is: {1}\n", fault, speed);

            //int retval = 0;
            //DescPose prepick_pose = new DescPose();
            //DescPose postpick_pose = new DescPose();

            //DescPose p1Desc = new DescPose(-419.524f, -13.000f, 351.569f, -178.118f, 0.314f, 3.833f);
            //DescPose p2Desc = new DescPose(-321.222f, 185.189f, 335.520f, -179.030f, -1.284f, -29.869f);

            //retval = robot.ComputePrePick(p1Desc, 10, 0, ref prepick_pose);
            //Console.WriteLine("ComputePrePick retval is: {0}\n", retval);
            //Console.WriteLine("xyz is: {0}, {1}, {2}; rpy is: {3}, {4}, {5}\n",
            //    prepick_pose.tran.x, prepick_pose.tran.y, prepick_pose.tran.z,
            //    prepick_pose.rpy.rx, prepick_pose.rpy.ry, prepick_pose.rpy.rz);

            //retval = robot.ComputePostPick(p2Desc, -10, 0, ref postpick_pose);
            //Console.WriteLine("ComputePostPick retval is: {0}\n", retval);
            //Console.WriteLine("xyz is: {0}, {1}, {2}; rpy is: {3}, {4}, {5}\n",
            //    postpick_pose.tran.x, postpick_pose.tran.y, postpick_pose.tran.z,
            //    postpick_pose.rpy.rx, postpick_pose.rpy.ry, postpick_pose.rpy.rz);

        }

        private void button37_Click(object sender, EventArgs e)
        {
            ushort fault = 0;
            double rotNum = 0.0;
            int rotSpeed = 0;
            int rotTorque = 0;
            robot.GetGripperRotNum(ref fault, ref rotNum);
            robot.GetGripperRotSpeed(ref fault, ref rotSpeed);
            robot.GetGripperRotTorque(ref fault, ref rotTorque);
            Console.WriteLine("gripper rot num : {0}, gripper rotSpeed : {1}, gripper rotTorque : {2}\n", rotNum, rotSpeed, rotTorque);

        }

        private void button38_Click(object sender, EventArgs e)
        {
            //传送带
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();


            int retval = 0;

            retval = robot.ConveyorStartEnd(1);
            Console.WriteLine("ConveyorStartEnd retval is: " + retval);

            retval = robot.ConveyorPointIORecord();
            Console.WriteLine("ConveyorPointIORecord retval is: " + retval);

            retval = robot.ConveyorPointARecord();
            Console.WriteLine("ConveyorPointARecord retval is: " + retval);

            retval = robot.ConveyorRefPointRecord();
            Console.WriteLine("ConveyorRefPointRecord retval is: " + retval);

            retval = robot.ConveyorPointBRecord();
            Console.WriteLine("ConveyorPointBRecord retval is: " + retval);

            retval = robot.ConveyorStartEnd(0);
            Console.WriteLine("ConveyorStartEnd retval is: " + retval);

            retval = 0;
            float[] param = { 1, 10000, 200, 0, 0, 20 };

            retval = robot.ConveyorSetParam(1, 10000, 200, 0, 0, 20);
            Console.WriteLine("ConveyorSetParam retval is: " + retval);

            double[] cmp = { 0.0, 0.0, 0.0 };
            retval = robot.ConveyorCatchPointComp(cmp);
            Console.WriteLine("ConveyorCatchPointComp retval is: " + retval);

            int index = 1;
            int max_time = 30000;
            byte block = 0;
            retval = 0;

            DescPose p1Desc = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose p2Desc = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);

            retval = robot.MoveCart(p1Desc, 1, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            Console.WriteLine("MoveCart retval is: " + retval);

            retval = robot.WaitMs(1);
            Console.WriteLine("WaitMs retval is: " + retval);

            //retval = robot.ConveyorIODetect(10000);
            //Console.WriteLine("ConveyorIODetect retval is: " + retval);

            //retval = robot.ConveyorGetTrackData(1);
            //Console.WriteLine("ConveyorGetTrackData retval is: " + retval);

            retval = robot.ConveyorTrackStart(1);
            Console.WriteLine("ConveyorTrackStart retval is: " + retval);

            retval = robot.ConveyorTrackMoveL("cvrCatchPoint", 1, 0, 100, 100, 100, -1.0f, 0, 0);
            Console.WriteLine("TrackMoveL retval is: " + retval);

            retval = robot.MoveGripper(index, 51, 40, 30, max_time, block, 0, 0, 0, 0);
            Console.WriteLine("MoveGripper retval is: " + retval);

            retval = robot.ConveyorTrackMoveL("cvrRaisePoint", 1, 0, 100, 100, 100, -1.0f, 0, 0);
            Console.WriteLine("TrackMoveL retval is: " + retval);

            retval = robot.ConveyorTrackEnd();
            Console.WriteLine("ConveyorTrackEnd retval is: " + retval);

            robot.MoveCart(p2Desc, 1, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);

            retval = robot.MoveGripper(index, 100, 40, 10, max_time, block, 0, 0, 0, 0);
            Console.WriteLine("MoveGripper retval is: " + retval);


        }

        private void button39_Click(object sender, EventArgs e)
        {
            robot.AxleSensorConfig(18, 0, 0, 1);
            int company = -1;
            int type = -1;
            robot.AxleSensorConfigGet(ref company, ref type);
            Console.WriteLine("company is " + company + ", type is " + type);

            int rtn = robot.AxleSensorActivate(1);
            Console.WriteLine("AxleSensorActivate rtn is " + rtn);

            Thread.Sleep(1000);

            rtn = robot.AxleSensorRegWrite(1, 4, 6, 1, 0, 0, 0);
            Console.WriteLine("AxleSensorRegWrite rtn is " + rtn);
        }

        private void button40_Click(object sender, EventArgs e)
        {
            int protocol = 4096;
            int rtn = robot.SetExDevProtocol(protocol);
            Console.WriteLine("SetExDevProtocol rtn " + rtn);
            rtn = robot.GetExDevProtocol(ref protocol);
            Console.WriteLine("GetExDevProtocol rtn " + rtn + " protocol is: " + protocol);
        }

        private void button41_Click(object sender, EventArgs e)
        {
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
            robot.AxleLuaUpload("D://zUP/AXLE_LUA_End_JunDuo_V0.4_20260602.lua");

            AxleComParam param = new AxleComParam(7, 8, 1, 0, 5, 3, 1);
            robot.SetAxleCommunicationParam(param);

            AxleComParam getParam = new AxleComParam();
            robot.GetAxleCommunicationParam(ref getParam);
            Console.WriteLine("GetAxleCommunicationParam param is {0} {1} {2} {3} {4} {5} {6}",
                getParam.baudRate, getParam.dataBit, getParam.stopBit, getParam.verify,
                getParam.timeout, getParam.timeoutTimes, getParam.period);

            robot.SetAxleLuaEnable(1);
            int luaEnableStatus = 0;
            robot.GetAxleLuaEnableStatus(ref luaEnableStatus);
            robot.SetAxleLuaEnableDeviceType(0, 1, 0, 0);

            int forceEnable = 0;
            int gripperEnable = 0;
            int ioEnable = 0;
            int dexhandEnable = 0;
            robot.GetAxleLuaEnableDeviceType(ref forceEnable, ref gripperEnable, ref ioEnable, ref dexhandEnable);
            Console.WriteLine("GetAxleLuaEnableDeviceType param is {0} {1} {2}", forceEnable, gripperEnable, ioEnable);

            int[] func = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            robot.SetAxleLuaGripperFunc(1, func);

            int[] getFunc = new int[32];
            robot.GetAxleLuaGripperFunc(1, ref getFunc);
            int[] getforceEnable = new int[16];
            int[] getgripperEnable = new int[16];
            int[] getioEnable = new int[16];
            int[] dexhandEnable1 = new int[16];
            robot.GetAxleLuaEnableDevice(ref getforceEnable, ref getgripperEnable, ref getioEnable,ref dexhandEnable1);
            Console.WriteLine("\ngetforceEnable status : ");
            foreach (int i in getforceEnable)
            {
                Console.Write(i + ",");
            }
            Console.WriteLine("\ngetgripperEnable status : ");
            foreach (int i in getgripperEnable)
            {
                Console.Write(i + ",");
            }
            Console.WriteLine("\ngetioEnable status : ");
            foreach (int i in getioEnable)
            {
                Console.Write(i + ",");
            }
            Console.WriteLine();
            robot.ActGripper(1, 0);
            Thread.Sleep(3000);
            robot.ActGripper(1, 1);
            Thread.Sleep(4000);
            robot.MoveGripper(1, 50, 10, 100, 50000, 0, 0, 0, 0, 0);
            while (true)
            {
                robot.GetRobotRealTimeState(ref pkg);
                Console.WriteLine("gripper pos is " + pkg.gripper_position);
                Thread.Sleep(100);
            }
        }

        private void button42_Click(object sender, EventArgs e)
        {
            robot.WeldingSetProcessParam(1, 177, 27, 1000, 178, 28, 176, 26, 1000);
            robot.WeldingSetProcessParam(2, 188, 28, 555, 199, 29, 133, 23, 333);

            double startCurrent = 0;
            double startVoltage = 0;
            double startTime = 0;
            double weldCurrent = 0;
            double weldVoltage = 0;
            double endCurrent = 0;
            double endVoltage = 0;
            double endTime = 0;

            robot.WeldingGetProcessParam(1, ref startCurrent, ref startVoltage, ref startTime, ref weldCurrent, ref weldVoltage, ref endCurrent, ref endVoltage, ref endTime);
            Console.WriteLine("the Num 1 process param is " + startCurrent + " " + startVoltage + " " + startTime + " " + weldCurrent + " " + weldVoltage + " " + endCurrent + " " + endVoltage + " " + endTime);
            robot.WeldingGetProcessParam(2, ref startCurrent, ref startVoltage, ref startTime, ref weldCurrent, ref weldVoltage, ref endCurrent, ref endVoltage, ref endTime);
            Console.WriteLine("the Num 2 process param is " + startCurrent + " " + startVoltage + " " + startTime + " " + weldCurrent + " " + weldVoltage + " " + endCurrent + " " + endVoltage + " " + endTime);

            int rtn = robot.WeldingSetCurrentRelation(0, 400, 0, 10, 0);
            Console.WriteLine("WeldingSetCurrentRelation rtn is: " + rtn);

            rtn = robot.WeldingSetVoltageRelation(0, 40, 0, 10, 1);
            Console.WriteLine("WeldingSetVoltageRelation rtn is: " + rtn);

            double current_min = 0;
            double current_max = 0;
            double vol_min = 0;
            double vol_max = 0;
            double output_vmin = 0;
            double output_vmax = 0;
            int curIndex = 0;
            int volIndex = 0;
            rtn = robot.WeldingGetCurrentRelation(ref current_min, ref current_max, ref output_vmin, ref output_vmax, ref curIndex);
            Console.WriteLine("WeldingGetCurrentRelation rtn is: " + rtn);
            Console.WriteLine("current min " + current_min + " current max " + current_max + " output vol min " + output_vmin + " output vol max " + output_vmax);

            rtn = robot.WeldingGetVoltageRelation(ref vol_min, ref vol_max, ref output_vmin, ref output_vmax, ref volIndex);
            Console.WriteLine("WeldingGetVoltageRelation rtn is: " + rtn);
            Console.WriteLine("vol min " + vol_min + " vol max " + vol_max + " output vol min " + output_vmin + " output vol max " + output_vmax);

            rtn = robot.WeldingSetCurrent(1, 100, 0, 0);
            Console.WriteLine("WeldingSetCurrent rtn is: " + rtn);

            System.Threading.Thread.Sleep(3000);

            rtn = robot.WeldingSetVoltage(1, 10, 0, 0);
            Console.WriteLine("WeldingSetVoltage rtn is: " + rtn);

            rtn = robot.WeaveSetPara(0, 0, 2.000000, 0, 10.000000, 0.000000, 0.000000, 0, 0, 0, 0, 0, 60.000000);
            Console.WriteLine("rtn is: " + rtn);

            robot.WeaveOnlineSetPara(0, 0, 1, 0, 20, 0, 0, 0, 0);

            rtn = robot.WeldingSetCheckArcInterruptionParam(1, 200);
            Console.WriteLine("WeldingSetCheckArcInterruptionParam    " + rtn);
            rtn = robot.WeldingSetReWeldAfterBreakOffParam(1, 5.7, 98.2, 0);
            Console.WriteLine("WeldingSetReWeldAfterBreakOffParam    " + rtn);
            int enable = 0;
            double length = 0;
            double velocity = 0;
            int moveType = 0;
            int checkEnable = 0;
            int arcInterruptTimeLength = 0;
            rtn = robot.WeldingGetCheckArcInterruptionParam(ref checkEnable, ref arcInterruptTimeLength);
            Console.WriteLine("WeldingGetCheckArcInterruptionParam  checkEnable  " + checkEnable + "   arcInterruptTimeLength  " + arcInterruptTimeLength);
            rtn = robot.WeldingGetReWeldAfterBreakOffParam(ref enable, ref length, ref velocity, ref moveType);
            Console.WriteLine("WeldingGetReWeldAfterBreakOffParam  enable = " + enable + ", length = " + length + ", velocity = " + velocity + ", moveType = " + moveType);

            robot.SetWeldMachineCtrlModeExtDoNum(17);
            for (int i = 0; i < 5; i++)
            {
                int getCtrlMode = -1;
                robot.SetWeldMachineCtrlMode(0);
                robot.GetWeldMachineCtrlMode(ref getCtrlMode);
                Console.WriteLine("GetWeldMachineCtrlMode {0}", getCtrlMode);
                Thread.Sleep(1000);
                robot.SetWeldMachineCtrlMode(1);
                robot.GetWeldMachineCtrlMode(ref getCtrlMode);
                Console.WriteLine("GetWeldMachineCtrlMode {0}", getCtrlMode);
                Thread.Sleep(1000);
            }
        }

        private void button43_Click(object sender, EventArgs e)
        {
            robot.WeldingSetCurrent(1, 230, 0, 0);
            robot.WeldingSetVoltage(1, 25, 0, 1);

            //DescPose p1Desc = new DescPose(228.879, -503.594, 453.984, -175.580, 8.293, 171.267);
            //JointPos p1Joint = new JointPos(102.700, -85.333, 90.518, -102.365, -83.932, 22.134);

            //DescPose p2Desc = new DescPose(-333.302, -435.580, 449.866, -174.997, 2.017, 109.815);
            //JointPos p2Joint = new JointPos(41.862, -85.333, 90.526, -100.587, -90.014, 22.135);

            DescPose p1Desc = new DescPose(441.901, 416.508, -51.979, -179.234, 0.718, -115.305);
            JointPos p1Joint = new JointPos(-146.22, -60.551, 104.859, -135.317, -90.289, 59.088);

            DescPose p2Desc = new DescPose(441.901, 615.317, -51.979, -179.234, 0.718, -115.305);
            JointPos p2Joint = new JointPos(-133.22, -44.193, 74.934, -121.661, -90.509, 72.087);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.MoveJ(p1Joint, p1Desc, 1, 0, 20, 100, 100, exaxisPos, -1, 0, offdese);
            robot.ARCStart(1, 0, 10000);
            robot.WeaveStart(0);
            robot.MoveL(p2Joint, p2Desc, 1, 0, 20, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            robot.ARCEnd(1, 0, 10000);
            robot.WeaveEnd(0);
        }

        private void button44_Click(object sender, EventArgs e)
        {
            robot.WeldingSetCurrent(0, 230, 0, 0);
            robot.WeldingSetVoltage(0, 24, 0, 1);

            DescPose p1Desc = new DescPose(441.901, 416.508, -51.979, -179.234, 0.718, -115.305);
            JointPos p1Joint = new JointPos(-146.22, -60.551, 104.859, -135.317, -90.289, 59.088);

            DescPose p2Desc = new DescPose(441.901, 615.317, -51.979, -179.234, 0.718, -115.305);
            JointPos p2Joint = new JointPos(-133.22, -44.193, 74.934, -121.661, -90.509, 72.087);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            int rtn = robot.SegmentWeldStart(p1Desc, p2Desc, p1Joint, p2Joint, 20, 20, 0, 0, 5000, false, 0, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            Console.WriteLine("SegmentWeldStart rtn is {0}", rtn);
        }

        private void button45_Click(object sender, EventArgs e)
        {
            //DescPose p1Desc = new DescPose(228.879, -503.594, 453.984, -175.580, 8.293, 171.267);
            //JointPos p1Joint = new JointPos(102.700, -85.333, 90.518, -102.365, -83.932, 22.134);

            //DescPose p2Desc = new DescPose(-333.302, -435.580, 449.866, -174.997, 2.017, 109.815);
            //JointPos p2Joint = new JointPos(41.862, -85.333, 90.526, -100.587, -90.014, 22.135);

            DescPose p1Desc = new DescPose(441.901, 416.508, -51.979, -179.234, 0.718, -115.305);
            JointPos p1Joint = new JointPos(-146.22, -60.551, 104.859, -135.317, -90.289, 59.088);

            DescPose p2Desc = new DescPose(441.901, 615.317, -51.979, -179.234, 0.718, -115.305);
            JointPos p2Joint = new JointPos(-133.22, -44.193, 74.934, -121.661, -90.509, 72.087);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.MoveJ(p1Joint, p1Desc, 1, 0, 20, 100, 100, exaxisPos, -1, 0, offdese);
            robot.WeaveStartSim(0);
            robot.MoveL(p2Joint, p2Desc, 1, 0, 20, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            robot.WeaveEndSim(0);
            robot.MoveJ(p1Joint, p1Desc, 1, 0, 20, 100, 100, exaxisPos, -1, 0, offdese);
            robot.WeaveInspectStart(0);
            robot.MoveL(p2Joint, p2Desc, 1, 0, 20, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            robot.WeaveInspectEnd(0);

            robot.WeldingSetVoltage(0, 19, 0, 0);
            robot.WeldingSetCurrent(0, 190, 0, 0);
            robot.MoveL(p1Joint, p1Desc, 1, 0, 100, 100, 50, -1, 0,exaxisPos, 0, 0, offdese);
            robot.ARCStart(0, 0, 10000);
            robot.ArcWeldTraceControl(1, 0, 1, 0.06, 5, 5, 60, 1, 0.06, 5, 5, 80, 0, 0, 4, 1, 10, 0, 0);
            robot.WeaveStart(0);
            robot.WeaveChangeStart(1, 1, 50, 30);
            robot.MoveL(p2Joint, p2Desc, 1, 0, 100, 100, 10, -1,0, exaxisPos, 0, 0, offdese);
            robot.WeaveChangeEnd();
            robot.WeaveEnd(0);
            robot.ArcWeldTraceControl(0, 0, 1, 0.06, 5, 5, 60, 1, 0.06, 5, 5, 80, 0, 0, 4, 1, 10, 0, 0);
            robot.ARCEnd(0, 0, 10000);
        }

        private void button46_Click(object sender, EventArgs e)
        {
            string file_path = "/usr/local/etc/controller/lua/airlab.lua";
            string md5 = "";

            string ssh_keygen = "";
            int retval = robot.GetSSHKeygen(ref ssh_keygen);
            Console.WriteLine("GetSSHKeygen retval is: {0}", retval);
            Console.WriteLine("ssh key is: {0}", ssh_keygen);

            string ssh_name = "fr";
            string ssh_ip = "192.168.58.45";
            string ssh_route = "/home/fr";
            string ssh_robot_url = "/root/robot/dhpara.config";
            retval = robot.SetSSHScpCmd(1, ssh_name, ssh_ip, ssh_route, ssh_robot_url);
            Console.WriteLine("SetSSHScpCmd retval is: {0}", retval);
            Console.WriteLine("robot url is: {0}", ssh_robot_url);

            robot.ComputeFileMD5(file_path, ref md5);
            Console.WriteLine("md5 is: {0}", md5);
        }

        private void button47_Click(object sender, EventArgs e)
        {
            robot.SetRobotRealtimeStateSamplePeriod(10);
            int getPeriod = 0;
            robot.GetRobotRealtimeStateSamplePeriod(ref getPeriod);
            Console.WriteLine("period is {0}", getPeriod);
            Thread.Sleep(1000);
        }

        private void button48_Click(object sender, EventArgs e)
        {
            int rtn = robot.SoftwareUpgrade("D://zUP/397/software.tar.gz", false);
            Console.WriteLine($"rtn is {rtn}");
            while (true)
            {
                int curState = -1;
                robot.GetSoftwareUpgradeState(ref curState);
                Console.WriteLine("upgrade state is {0}", curState);
                Thread.Sleep(300);
            }
        }

        private void button49_Click(object sender, EventArgs e)
        {
            string save_path = "D://zDOWN/";
            string point_table_name = "test_point_A.db";
            int rtn = robot.PointTableDownLoad(point_table_name, save_path);
            Console.WriteLine("download : {0} fail: {1}", point_table_name, rtn);

            string upload_path = "D://zUP/point_table_test_point_A.db";
            rtn = robot.PointTableUpLoad(upload_path);
            Console.WriteLine("retval is: {0}", rtn);

            string point_tablename = "test_point_A.db";
            string lua_name = "111.lua";

            string errorStr = "";
            rtn = robot.PointTableUpdateLua(point_tablename, lua_name, ref errorStr);
            Console.WriteLine("retval is: {0}", rtn);

        }

        private void button50_Click(object sender, EventArgs e)
        {
            int rtn = robot.RbLogDownload("D://zDOWN/");
            Console.WriteLine("RbLogDownload rtn is {0}", rtn);

            rtn = robot.AllDataSourceDownload("D://zDOWN/");
            Console.WriteLine("AllDataSourceDownload rtn is {0}", rtn);

            rtn = robot.DataPackageDownload("D://zDOWN/");
            Console.WriteLine("DataPackageDownload rtn is {0}", rtn);
        }

        private void button51_Click(object sender, EventArgs e)
        {
            robot.SetArcStartExtDoNum(10);
            robot.SetAirControlExtDoNum(20);
            robot.SetWireForwardFeedExtDoNum(30);
            robot.SetWireReverseFeedExtDoNum(40);

            robot.SetWeldReadyExtDiNum(50);
            robot.SetArcDoneExtDiNum(60);
            robot.SetExtDIWeldBreakOffRecover(70, 80);
            robot.SetWireSearchExtDIONum(0, 1);

            int[] DIConfig = new int[16];
            int[] DOConfig = new int[16];
            int rtn = robot.GetExtDIConfig(ref DIConfig);
            Console.WriteLine("GetExtDIConfig rtn={0}, welder ready={1}, arc done={2}, reweld start={3}, abort reweld={4}, wiresearch done={5}, laser state={6}, laser err={7}",
                rtn, DIConfig[0], DIConfig[1], DIConfig[2], DIConfig[3], DIConfig[4], DIConfig[5], DIConfig[6]);
            rtn = robot.GetExtDOConfig(ref DOConfig);
            Console.WriteLine("GetExtDOConfig rtn={0}, arc start={1}, air test={2}, wire forward={3}, wire inverse={4}, wiresearch={5}, weld mode={6}, laser enable={7}, laser on={8}, laser reset={9}",
                rtn, DOConfig[0], DOConfig[1], DOConfig[2], DOConfig[3], DOConfig[4], DOConfig[5], DOConfig[6], DOConfig[7], DOConfig[8]);
        }

        private void button52_Click(object sender, EventArgs e)
        {

            JointPos mulitilineorigin1_joint = new JointPos(-146.22, -60.551, 104.859, -135.317, -90.289, 59.088);
            DescPose mulitilineorigin1_desc = new DescPose(441.901, 416.508, -51.979, -179.234, 0.718, -115.305);

            DescTran mulitilineX1_desc = new DescTran();
            mulitilineX1_desc.x = -677.556;
            mulitilineX1_desc.y = 211.949;
            mulitilineX1_desc.z = -1.206;

            DescTran mulitilineZ1_desc = new DescTran();
            mulitilineZ1_desc.x = -677.564;
            mulitilineZ1_desc.y = 190.956;
            mulitilineZ1_desc.z = 19.817;

            JointPos mulitilinesafe_joint = new JointPos(-138.179, -55.975, 88.096, -123.081, -90.426, 67.129);
            DescPose mulitilinesafe_desc = new DescPose(439.754, 527.356, -4.026, -179.234, 0.719, -115.306);
            JointPos mulitilineorigin2_joint = new JointPos(-133.22, -44.193, 74.934, -121.661, -90.509, 72.087);
            DescPose mulitilineorigin2_desc = new DescPose(441.901, 615.317, -51.979, -179.234, 0.718, -115.305);

            DescTran mulitilineX2_desc = new DescTran();
            mulitilineX2_desc.x = -563.965;
            mulitilineX2_desc.y = 220.355;
            mulitilineX2_desc.z = -0.680;

            DescTran mulitilineZ2_desc = new DescTran();
            mulitilineZ2_desc.x = -563.968;
            mulitilineZ2_desc.y = 215.362;
            mulitilineZ2_desc.z = 4.331;

            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            DescPose offset = new DescPose(0, 0, 0, 0, 0, 0);

            Thread.Sleep(10);
            int error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
            Console.WriteLine("MoveJ return: {0}", error);

            error = robot.MoveL(mulitilineorigin1_joint, mulitilineorigin1_desc, 13, 0, 10, 100, 100, -1,0, epos, 0, 0, offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
            Console.WriteLine("MoveJ return: {0}", error);

            error = robot.MoveL(mulitilineorigin2_joint, mulitilineorigin2_desc, 13, 0, 10, 100, 100, -1, 0,epos, 0, 0, offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
            Console.WriteLine("MoveJ return: {0}", error);

            error = robot.MoveL(mulitilineorigin1_joint, mulitilineorigin1_desc, 13, 0, 10, 100, 100, -1, 0,epos, 0, 0, offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.ARCStart(0, 0, 3000);
            Console.WriteLine("ARCStart return: {0}", error);

            error = robot.WeaveStart(2);
            Console.WriteLine("WeaveStart return: {0}", error);

            error = robot.ArcWeldTraceControl(1, 0, 1, 0.06, 5, 5, 50, 1, 0.06, 5, 5, 55, 0, 0, 4, 1, 10);
            Console.WriteLine("ArcWeldTraceControl return: {0}", error);

            error = robot.MoveL(mulitilineorigin2_joint, mulitilineorigin2_desc, 13, 0, 1, 100, 100, -1,0, epos, 0, 0, offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.ArcWeldTraceControl(0, 0, 1, 0.06, 5, 5, 50, 1, 0.06, 5, 5, 55, 0, 0, 4, 1, 10);
            Console.WriteLine("ArcWeldTraceControl return: {0}", error);

            error = robot.WeaveEnd(2);
            Console.WriteLine("WeaveEnd return: {0}", error);

            error = robot.ARCEnd(0, 0, 10000);
            Console.WriteLine("ARCEnd return: {0}", error);

            error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
            Console.WriteLine("MoveJ return: {0}", error);

            error = robot.MultilayerOffsetTrsfToBase(mulitilineorigin1_desc.tran, mulitilineX1_desc, mulitilineZ1_desc, 10.0, 0.0, 0.0, ref offset);
            Console.WriteLine("MultilayerOffsetTrsfToBase return: {0}  offect is {1} {2} {3}", error, offset.tran.x, offset.tran.y, offset.tran.z);

            error = robot.MoveL(mulitilineorigin1_joint, mulitilineorigin1_desc, 13, 0, 10, 100, 100, -1, 0,epos, 0, 1, offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.ARCStart(0, 0, 3000);
            Console.WriteLine("ARCStart return: {0}", error);
            error = robot.WeaveStart(2);
            Console.WriteLine("WeaveStart return: {0}", error);

            error = robot.MultilayerOffsetTrsfToBase(mulitilineorigin2_desc.tran, mulitilineX2_desc, mulitilineZ2_desc, 10, 0, 0, ref offset);
            Console.WriteLine("MultilayerOffsetTrsfToBase return: {0}  offect is {1} {2} {3}", error, offset.tran.x, offset.tran.y, offset.tran.z);

            error = robot.ArcWeldTraceReplayStart();
            Console.WriteLine("ArcWeldTraceReplayStart return: {0}", error);

            error = robot.MoveL(mulitilineorigin2_joint, mulitilineorigin2_desc, 13, 0, 10, 100, 100, -1, 0,epos, 0, 1, offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.ArcWeldTraceReplayEnd();
            Console.WriteLine("ArcWeldTraceReplayEnd return: {0}", error);

            error = robot.WeaveEnd(2);
            Console.WriteLine("WeaveEnd return: {0}", error);

            error = robot.ARCEnd(0, 0, 10000);
            Console.WriteLine("ARCEnd return: {0}", error);

            error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
            Console.WriteLine("MoveJ return: {0}", error);

            error = robot.MultilayerOffsetTrsfToBase(mulitilineorigin1_desc.tran, mulitilineX1_desc, mulitilineZ1_desc, 0, 10, 0, ref offset);
            Console.WriteLine("MultilayerOffsetTrsfToBase return: {0}  offect is {1} {2} {3}", error, offset.tran.x, offset.tran.y, offset.tran.z);

            error = robot.MoveL(mulitilineorigin1_joint, mulitilineorigin1_desc, 13, 0, 10, 100, 100, -1,0, epos, 0, 1, offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.ARCStart(0, 0, 3000);
            Console.WriteLine("ARCStart return: {0}", error);
            error = robot.WeaveStart(2);
            Console.WriteLine("WeaveStart return: {0}", error);

            error = robot.MultilayerOffsetTrsfToBase(mulitilineorigin2_desc.tran, mulitilineX2_desc, mulitilineZ2_desc, 0, 10, 0, ref offset);
            Console.WriteLine("MultilayerOffsetTrsfToBase return: {0}  offect is {1} {2} {3}", error, offset.tran.x, offset.tran.y, offset.tran.z);

            error = robot.ArcWeldTraceReplayStart();
            Console.WriteLine("MoveJ return: {0}", error);

            error = robot.MoveL(mulitilineorigin2_joint, mulitilineorigin2_desc, 13, 0, 10, 100, 100, -1, 0,epos, 1, 1, offset, 1, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.ArcWeldTraceReplayEnd();
            Console.WriteLine("ArcWeldTraceReplayEnd return: {0}", error);

            error = robot.WeaveEnd(2);
            Console.WriteLine("WeaveEnd return: {0}", error);

            error = robot.ARCEnd(0, 0, 3000);
            Console.WriteLine("ARCEnd return: {0}", error);

            error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
            Console.WriteLine("MoveJ return: {0}", error);
        }

        private void button53_Click(object sender, EventArgs e)
        {
            DescPose toolCoord = new DescPose(0, 0, 200, 0, 0, 0);
            robot.SetToolCoord(1, toolCoord, 0, 0, 1, 0);
            DescPose wobjCoord = new DescPose(0, 0, 0, 0, 0, 0);
            robot.SetWObjCoord(1, wobjCoord, 0);

            int rtn0, rtn1, rtn2 = 0;
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            DescPose descStart = new DescPose(441.901, 416.508, -51.979, -179.234, 0.718, -115.305);
            JointPos jointStart = new JointPos(-146.22, -60.551, 104.859, -135.317, -90.289, 59.088);
            DescPose descEnd = new DescPose(441.901, 615.317, -51.979, -179.234, 0.718, -115.305);
            JointPos jointEnd = new JointPos(-133.22, -44.193, 74.934, -121.661, -90.509, 72.087);

            robot.MoveL(jointStart, descStart, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            robot.MoveL(jointEnd, descEnd, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);

            DescPose descREF0A = new DescPose(406.638, 347.992, -51.925, -179.229, 0.725, -115.305);
            JointPos jointREF0A = new JointPos(-150.307, -67.812, 117.086, -140.31, -90.216, 55);
            DescPose descREF0B = new DescPose(406.638, 403.815, -51.925, -179.229, 0.725, -115.305);
            JointPos jointREF0B = new JointPos(-145.285, -63.993, 110.76, -137.771, -90.305, 60.021);
            DescPose descREF1A = new DescPose(361.731, 357.024, -51.985, -179.235, 0.717, -115.304);
            JointPos jointREF1A = new JointPos(-146.785, -70.516, 121.407, -141.902, -90.278, 58.521);
            DescPose descREF1B = new DescPose(361.731, 399.681, -51.985, -179.235, 0.717, -115.304);
            JointPos jointREF1B = new JointPos(-142.858, -67.39, 116.395, -139.995, -90.347, 62.449);

            rtn0 = robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF0A, descREF0A, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);  //\u8d77\u70b9
            robot.MoveL(jointREF0B, descREF0B, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 1, 0, offdese);  //\u65b9\u5411\u70b9
            rtn1 = robot.WireSearchWait("REF0");
            rtn2 = robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            rtn0 = robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF1A, descREF1A, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);  //\u8d77\u70b9
            robot.MoveL(jointREF1B, descREF1B, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 1, 0, offdese);  //\u65b9\u5411\u70b9
            rtn1 = robot.WireSearchWait("REF1");
            rtn2 = robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            robot.Sleep(5000);

            rtn0 = robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF0A, descREF0A, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);  //\u8d77\u70b9
            robot.MoveL(jointREF0B, descREF0B, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 1, 0, offdese);  //\u65b9\u5411\u70b9
            rtn1 = robot.WireSearchWait("RES0");
            rtn2 = robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            rtn0 = robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF1A, descREF1A, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);  //\u8d77\u70b9
            robot.MoveL(jointREF1B, descREF1B, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 1, 0, offdese);  //\u65b9\u5411\u70b9
            rtn1 = robot.WireSearchWait("RES1");
            rtn2 = robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            string[] varNameRef = { "REF0", "REF1", "#", "#", "#", "#" };
            string[] varNameRes = { "RES0", "RES1", "#", "#", "#", "#" };
            int offectFlag = 0;
            DescPose offectPos = new DescPose(0, 0, 0, 0, 0, 0);
            rtn0 = robot.GetWireSearchOffset(0, 0, varNameRef, varNameRes, ref offectFlag, ref offectPos);
            Console.WriteLine("offset is {0} {1} {2}", offectPos.tran.x, offectPos.tran.y, offectPos.tran.z);
            robot.PointsOffsetEnable(0, offectPos);
            robot.MoveL(jointStart, descStart, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            robot.MoveL(jointEnd, descEnd, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 1, 0, offdese);
            robot.PointsOffsetDisable();
        }

        private void button54_Click(object sender, EventArgs e)
        {
            int company = 22;
            int device = 0;
            int softversion = 0;
            int bus = 1;

            robot.FT_SetConfig(company, device, softversion, bus);
            Thread.Sleep(1000);
            robot.FT_GetConfig(ref company, ref device, ref softversion, ref bus);
            Console.WriteLine($"FT config:{company},{device},{softversion},{bus}");
            Thread.Sleep(1000);

            robot.FT_Activate(0);
            Thread.Sleep(1000);
            robot.FT_Activate(1);
            Thread.Sleep(1000);

            Thread.Sleep(1000);
            robot.FT_SetZero(0);
            Thread.Sleep(1000);

            ForceTorque ft = new ForceTorque(0, 0, 0, 0, 0, 0);
            robot.FT_GetForceTorqueOrigin(0, ref ft);
            Console.WriteLine($"ft origin:{ft.fx},{ft.fy},{ft.fz},{ft.tx},{ft.ty},{ft.tz}");
            robot.FT_SetZero(1);
            Thread.Sleep(1000);

            DescPose ftCoord = new DescPose(0, 0, 0, 0, 0, 0);
            robot.FT_SetRCS(0, ftCoord);

            robot.SetForceSensorPayLoad(0.824);
            robot.SetForceSensorPayLoadCog(0.778, 2.554, 48.765);
            double weight = 0;
            double x = 0, y = 0, z = 0;
            robot.GetForceSensorPayLoad(ref weight);
            robot.GetForceSensorPayLoadCog(ref x, ref y, ref z);
            Console.WriteLine($"the FT load is {weight}, {x} {y} {z}");

            robot.SetForceSensorPayLoad(0);
            robot.SetForceSensorPayLoadCog(0, 0, 0);

            DescTran tran = new DescTran(0, 0, 0);
            robot.ForceSensorAutoComputeLoad(ref weight, ref tran);
            Console.WriteLine($"the result is weight {weight} pos is {tran.x} {tran.y} {tran.z}");

        }

        private void button55_Click(object sender, EventArgs e)
        {
            int company = 24, device = 0, softversion = 0, bus = 1;

            robot.FT_SetConfig(company, device, softversion, bus);
            Thread.Sleep(1000);
            robot.FT_GetConfig(ref company, ref device, ref softversion, ref bus);
            Console.WriteLine($"FT config: {company}, {device}, {softversion}, {bus}");
            Thread.Sleep(1000);

            robot.FT_Activate(0);
            Thread.Sleep(1000);
            robot.FT_Activate(1);
            Thread.Sleep(1000);

            Thread.Sleep(1000);
            robot.FT_SetZero(0);
            Thread.Sleep(1000);

            ForceTorque ft = new ForceTorque(0, 0, 0, 0, 0, 0);
            robot.FT_GetForceTorqueOrigin(0, ref ft);
            Console.WriteLine($"ft origin: {ft.fx}, {ft.fy}, {ft.fz}, {ft.tx}, {ft.ty}, {ft.tz}");
            robot.FT_SetZero(1);
            Thread.Sleep(1000);

            DescPose tcoord = new DescPose(0, 0, 35.0, 0, 0, 0);
            robot.SetToolCoord(10, tcoord, 1, 0, 0, 0);

            robot.FT_PdIdenRecord(10);
            Thread.Sleep(1000);

            double weight = 0.0f;
            robot.FT_PdIdenCompute(ref weight);
            Console.WriteLine($"payload weight: {weight}");

            DescPose desc_p1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose desc_p2 = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);
            DescPose desc_p3 = new DescPose(-327.622, 402.230, 320.402, -178.067, 2.127, -46.207);

            robot.MoveCart(desc_p1, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            Thread.Sleep(1000);
            robot.FT_PdCogIdenRecord(10, 1);
            robot.MoveCart(desc_p2, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            Thread.Sleep(1000);
            robot.FT_PdCogIdenRecord(10, 2);
            robot.MoveCart(desc_p3, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            Thread.Sleep(1000);
            robot.FT_PdCogIdenRecord(10, 3);

            DescTran cog = new DescTran(0, 0, 0);
            robot.FT_PdCogIdenCompute(ref cog);
            Console.WriteLine($"cog: {cog.x}, {cog.y}, {cog.z}");
        }

        private void button56_Click(object sender, EventArgs e)
        {
            int company = 24, device = 0, softversion = 0, bus = 1;

            robot.FT_SetConfig(company, device, softversion, bus);
            Thread.Sleep(1000);
            robot.FT_GetConfig(ref company, ref device, ref softversion, ref bus);
            Console.WriteLine($"FT config: {company}, {device}, {softversion}, {bus}");
            Thread.Sleep(1000);

            robot.FT_Activate(0);
            Thread.Sleep(1000);
            robot.FT_Activate(1);
            Thread.Sleep(1000);

            Thread.Sleep(1000);
            robot.FT_SetZero(0);
            Thread.Sleep(1000);

            byte sensor_id = 1;
            int[] select = { 1, 1, 1, 1, 1, 1 };
            double[] max_threshold = { 10.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f };
            double[] min_threshold = { 5.0f, 5.0f, 5.0f, 5.0f, 5.0f, 5.0f };

            ForceTorque ft = new ForceTorque();
            DescPose desc_p1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose desc_p2 = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);
            DescPose desc_p3 = new DescPose(-327.622, 402.230, 320.402, -178.067, 2.127, -46.207);

            robot.FT_Guard(1, sensor_id, select, ft, max_threshold, min_threshold);
            robot.MoveCart(desc_p1, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            robot.MoveCart(desc_p2, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            robot.MoveCart(desc_p3, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);

            robot.FT_Guard(0, sensor_id, select, ft, max_threshold, min_threshold);
        }

        private void button57_Click(object sender, EventArgs e)
        {
            int company = 24, device = 0, softversion = 0, bus = 1;
            robot.FT_SetConfig(company, device, softversion, bus);
            Thread.Sleep(1000);
            robot.FT_GetConfig(ref company, ref device, ref softversion, ref bus);
            Console.WriteLine($"FT config: {company}, {device}, {softversion}, {bus}");
            Thread.Sleep(1000);

            robot.FT_Activate(0);
            Thread.Sleep(1000);
            robot.FT_Activate(1);
            Thread.Sleep(1000);

            robot.FT_SetZero(0);
            Thread.Sleep(1000);

            int[] select = { 0, 0, 1, 0, 0, 0 };
            double[] ft_pid = { 0.0005f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            //byte adj_sign = 0, ILC_sign = 0;
            //float max_dis = 100.0f, max_ang = 0.0f;

            ForceTorque ft = new ForceTorque(0.0, 0.0, -10.0, 0.0, 0.0, 0.0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            JointPos j1 = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            JointPos j2 = new JointPos(-45.615, -106.172, 124.296, -107.151, -91.282, 74.255);
            DescPose desc_p1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose desc_p2 = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);

            int rtn = robot.MoveJ(j1, desc_p1, 0, 0, 100.0f, 180.0f, 100.0f, epos, -1.0f, 0, offset_pos);
           // robot.FT_Control(1, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
            rtn = robot.MoveJ(j2, desc_p2, 0, 0, 100.0f, 180.0f, 100.0f, epos, -1.0f, 0, offset_pos);
         //   robot.FT_Control(0, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
        }

        private void button58_Click(object sender, EventArgs e)
        {
            int company = 24, device = 0, softversion = 0, bus = 1;
            robot.FT_SetConfig(company, device, softversion, bus);
            Thread.Sleep(1000);
            //robot.FT_GetConfig(ref company, ref device, ref softversion, ref bus);
            //Console.WriteLine($"FT config: {company}, {device}, {softversion}, {bus}");
            //Thread.Sleep(1000);

            //robot.FT_Activate(0);
            //Thread.Sleep(1000);
            //robot.FT_Activate(1);
            //Thread.Sleep(1000);

            //robot.FT_SetZero(0);
            //Thread.Sleep(1000);

            //byte status = 1, sensor_num = 1;
            //double[] gain = { 0.0001f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            //byte adj_sign = 0, ILC_sign = 0;
            //float max_dis = 100.0f, max_ang = 5.0f;

            //ForceTorque ft = new ForceTorque();
            //int[] select1 = { 0, 0, 1, 1, 1, 0 };
            //ft.fz = -10.0;

            //robot.FT_Control(status, sensor_num, select1,  ft, gain, adj_sign, ILC_sign, max_dis, max_ang);
            //int rtn = robot.FT_SpiralSearch(0, 0.7f, 1.0f, 60000.0f, 3.0f);
            //Console.WriteLine($"FT_SpiralSearch rtn is {rtn}");
            //status = 0;
            //robot.FT_Control(status, sensor_num, select1,  ft, gain, adj_sign, ILC_sign, max_dis, max_ang);

            //int[] select2 = { 1, 1, 1, 0, 0, 0 };
            //gain[0] = 0.00005f;
            //ft.fz = -30.0;
            //status = 1;
            //robot.FT_Control(status, sensor_num, select2,  ft, gain, adj_sign, ILC_sign, max_dis, max_ang);
            //rtn = robot.FT_LinInsertion(0, 20.0f, 0.0f, 0.0f, 100.0f, 1);
            //Console.WriteLine($"FT_LinInsertion rtn is {rtn}");
            //status = 0;
            //robot.FT_Control(status, sensor_num, select2,  ft, gain, adj_sign, ILC_sign, max_dis, max_ang);

            //int[] select3 = { 0, 0, 1, 1, 1, 0 };
            //ft.fz = -10.0;
            //gain[0] = 0.0001f;
            //status = 1;
            //robot.FT_Control(status, sensor_num, select3,  ft, gain, adj_sign, ILC_sign, max_dis, max_ang);
            //rtn = robot.FT_RotInsertion(0, 2.0f, 1.0f, 45, 1, 0.0f, 1);
            //Console.WriteLine($"FT_RotInsertion rtn is {rtn}");
            //status = 0;
            //robot.FT_Control(status, sensor_num, select3,  ft, gain, adj_sign, ILC_sign, max_dis, max_ang);

            //int[] select4 = { 1, 1, 1, 0, 0, 0 };
            //ft.fz = -30.0;
            //status = 1;
            //robot.FT_Control(status, sensor_num, select4,  ft, gain, adj_sign, ILC_sign, max_dis, max_ang);
            //rtn = robot.FT_LinInsertion(0, 20.0f, 0.0f, 0.0f, 100.0f, 1);
            //Console.WriteLine($"FT_LinInsertion rtn is {rtn}");
            //status = 0;
            //robot.FT_Control(status, sensor_num, select4,  ft, gain, adj_sign, ILC_sign, max_dis, max_ang);
        }

        private void button59_Click(object sender, EventArgs e)
        {
            int company = 22;
            int device = 0;
            int softversion = 0;
            int bus = 1;

            robot.FT_SetConfig(company, device, softversion, bus);
            Thread.Sleep(1000);
            robot.FT_GetConfig(ref company, ref device, ref softversion, ref bus);
            Console.WriteLine("FT config:" + company + "," + device + "," + softversion + "," + bus);
            Thread.Sleep(1000);

            robot.FT_Activate(0);
            Thread.Sleep(1000);
            robot.FT_Activate(1);
            Thread.Sleep(1000);

            Thread.Sleep(1000);
            robot.FT_SetZero(0);
            Thread.Sleep(1000);

            int rcs = 0;
            byte dir = 1;
            byte axis = 1;
            float lin_v = 15.0f;
            float lin_a = 0.0f;
            float maxdis = 500.0f;
            float ft_goal = 2.0f;
            DescPose desc_pos = new DescPose(-419.524f, -13.000f, 351.569f, -178.118f, 0.314f, 3.833f);
            DescPose xcenter = new DescPose(0, 0, 0, 0, 0, 0);
            DescPose ycenter = new DescPose(0, 0, 0, 0, 0, 0);

            ForceTorque ft = new ForceTorque();

            ft.fx = -2.0f;

            robot.MoveCart(desc_pos, 1, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);

            robot.FT_CalCenterStart();
            robot.FT_FindSurface(rcs, dir, axis, lin_v, lin_a, maxdis, ft_goal);
            robot.MoveCart(desc_pos, 1, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            robot.WaitMs(1000);

            dir = 2;
            robot.FT_FindSurface(rcs, dir, axis, lin_v, lin_a, maxdis, ft_goal);
            robot.FT_CalCenterEnd(ref xcenter);
            Console.WriteLine("xcenter:" + xcenter.tran.x + "," + xcenter.tran.y + "," + xcenter.tran.z + "," + xcenter.rpy.rx + "," + xcenter.rpy.ry + "," + xcenter.rpy.rz);
            robot.MoveCart(xcenter, 1, 0, 60.0f, 50.0f, 50.0f, -1.0f, -1);

            robot.FT_CalCenterStart();
            dir = 1;
            axis = 2;
            lin_v = 6.0f;
            maxdis = 150.0f;
            robot.FT_FindSurface(rcs, dir, axis, lin_v, lin_a, maxdis, ft_goal);
            robot.MoveCart(desc_pos, 1, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            robot.WaitMs(1000);

            dir = 2;
            robot.FT_FindSurface(rcs, dir, axis, lin_v, lin_a, maxdis, ft_goal);
            robot.FT_CalCenterEnd(ref ycenter);
            Console.WriteLine("ycenter:" + ycenter.tran.x + "," + ycenter.tran.y + "," + ycenter.tran.z + "," + ycenter.rpy.rx + "," + ycenter.rpy.ry + "," + ycenter.rpy.rz);
            robot.MoveCart(ycenter, 1, 0, 60.0f, 50.0f, 50.0f, 0.0f, -1);

        }

        private void button61_Click(object sender, EventArgs e)
        {
            robot.SetForceSensorDragAutoFlag(1);
            double[] M = { 15.0, 15.0, 15.0, 0.5, 0.5, 0.1 };
            double[] B = { 150.0, 150.0, 150.0, 5.0, 5.0, 1.0 };
            double[] K = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] F = { 10.0, 10.0, 10.0, 1.0, 1.0, 1.0 };

            robot.EndForceDragControl(1, 0, 0, 0, M, B, K, F, 50, 100);
            robot.WaitMs(5000);

            int dragState = 0;
            int sixDimensionalDragState = 0;
            robot.GetForceAndTorqueDragState(ref dragState, ref sixDimensionalDragState);
            Console.WriteLine($"the drag state is {dragState} {sixDimensionalDragState}");

            robot.EndForceDragControl(0, 0, 0, 0, M, B, K, F, 50, 100);
        }

        private void button62_Click(object sender, EventArgs e)
        {
            robot.DragTeachSwitch(1);
            double[] lambdaGain = { 3.0, 2.0, 2.0, 2.0, 2.0, 3.0 };
            double[] kGain = { 0, 0, 0, 0, 0, 0 };
            double[] bGain = { 150, 150, 150, 5.0, 5.0, 1.0 };


            int rtn = robot.ForceAndJointImpedanceStartStop(1, 0, lambdaGain, kGain, bGain, 1000, 180);
            Console.WriteLine($"ForceAndJointImpedanceStartStop rtn is {rtn}");

            Thread.Sleep(5000); // 等待5秒

            robot.DragTeachSwitch(0);

            rtn = robot.ForceAndJointImpedanceStartStop(0, 0, lambdaGain, kGain, bGain, 1000, 180);
            Console.WriteLine($"ForceAndJointImpedanceStartStop rtn is {rtn}");

        }

        private void button60_Click(object sender, EventArgs e)
        {
            int company = 24, device = 0, softversion = 0, bus = 1;
            robot.FT_SetConfig(company, device, softversion, bus);
            Thread.Sleep(1000);
            robot.FT_GetConfig(ref company, ref device, ref softversion, ref bus);
            Console.WriteLine($"FT config: {company}, {device}, {softversion}, {bus}");
            Thread.Sleep(1000);

            robot.FT_Activate(0);
            Thread.Sleep(1000);
            robot.FT_Activate(1);
            Thread.Sleep(1000);

            robot.FT_SetZero(0);
            Thread.Sleep(1000);

            //byte flag = 0;
            int[] select = { 1, 1, 1, 0, 0, 0 };
            double[] ft_pid = { 0.0005f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            //byte adj_sign = 0, ILC_sign = 0;
            //float max_dis = 100.0f, max_ang = 0.0f;

            ForceTorque ft = new ForceTorque { fx = -10.0, fy = -10.0, fz = -10.0 };
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);

            JointPos j1 = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            JointPos j2 = new JointPos(-45.615, -106.172, 124.296, -107.151, -91.282, 74.255);
            DescPose desc_p1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose desc_p2 = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);

         //   robot.FT_Control(flag, (byte)sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
            float p = 0.00005f;
            float force = 30.0f;
            int rtn = robot.FT_ComplianceStart(p, force);
            Console.WriteLine($"FT_ComplianceStart rtn is {rtn}");

            //int count = 5;
            //while (count-- > 0)
            //{
            //    robot.MoveL(j1, desc_p1, 0, 0, 100.0f, 180.0f, 100.0f, -1.0f, epos, 0, 1, offset_pos);
            //    robot.MoveL(j2, desc_p2, 0, 0, 100.0f, 180.0f, 100.0f, -1.0f, epos, 0, 0, offset_pos);
            //}

            //robot.FT_ComplianceStop();
            //Console.WriteLine($"FT_ComplianceStop rtn is {rtn}");

            //flag = 0;
          //  robot.FT_Control(flag, (byte)sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
        }

        private void button63_Click(object sender, EventArgs e)
        {
            int que_len = 0;
            int rtn = robot.GetMotionQueueLength(ref que_len);
            Console.WriteLine($"GetMotionQueueLength rtn is:  {rtn}, queue length is:{que_len}");
            double[] dh = { 0, 0, 0, 0, 0, 0 };
            int retval = 0;
            retval = robot.GetDHCompensation(ref dh);
            Console.WriteLine($"retval is  {retval}");
            Console.WriteLine($"dh is {dh[0]}, {dh[1]}, {dh[2]}, {dh[3]}, {dh[4]}, {dh[5]}");
            string SN = "";
            robot.GetRobotSN(ref SN);
            Console.WriteLine($"robot SN is  {SN}");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button64_Click(object sender, EventArgs e)
        {
            int retval = robot.AuxServoSetParam(1, 1, 1, 1, 131072, 15.45);
            Console.WriteLine($"AuxServoSetParam is: {retval}");

            int servoCompany = 0;
            int servoModel = 0;
            int servoSoftVersion = 0;
            int servoResolution = 0;
            double axisMechTransRatio = 0;
            retval = robot.AuxServoGetParam(1, ref servoCompany, ref servoModel, ref servoSoftVersion, ref servoResolution, ref axisMechTransRatio);
            Console.WriteLine($"servoCompany {servoCompany}\n" +
                $"servoModel {servoModel}\n" +
                $"servoSoftVersion {servoSoftVersion}\n" +
                $"servoResolution {servoResolution}\n" +
                $"axisMechTransRatio {axisMechTransRatio}\n");

            retval = robot.AuxServoSetParam(1, 10, 11, 12, 13, 14);
            Console.WriteLine($"AuxServoSetParam is: {retval}");

            retval = robot.AuxServoGetParam(1, ref servoCompany, ref servoModel, ref servoSoftVersion, ref servoResolution, ref axisMechTransRatio);
            Console.WriteLine($"servoCompany {servoCompany}\n" +
                $"servoModel {servoModel}\n" +
                $"servoSoftVersion {servoSoftVersion}\n" +
                $"servoResolution {servoResolution}\n" +
                $"axisMechTransRatio {axisMechTransRatio}\n");

            retval = robot.AuxServoSetParam(1, 1, 1, 1, 131072, 36);
            Console.WriteLine($"AuxServoSetParam is: {retval}");
            Thread.Sleep(3000);

            robot.AuxServoSetAcc(3000, 3000);
            robot.AuxServoSetEmergencyStopAcc(5000, 5000);
            Thread.Sleep(1000);
            double emagacc = 0, acc = 0;
            double emagdec = 0, dec = 0;
            robot.AuxServoGetEmergencyStopAcc(ref emagacc, ref emagdec);
            Console.WriteLine($"emergency acc is {emagacc}  dec is {emagdec}");
            robot.AuxServoGetAcc(ref acc, ref dec);
            Console.WriteLine($"acc is {acc}  dec is {dec}");

            robot.AuxServoSetControlMode(1, 0);
            Thread.Sleep(2000);

            retval = robot.AuxServoEnable(1, 0);
            Console.WriteLine($"AuxServoEnable disenable {retval}");
            Thread.Sleep(1000);
            int servoErrCode = 0;
            int servoState = 0;
            double servoPos = 0;
            double servoSpeed = 0;
            double servoTorque = 0;
            retval = robot.AuxServoGetStatus(1, ref servoErrCode, ref servoState, ref servoPos, ref servoSpeed, ref servoTorque);
            Console.WriteLine($"AuxServoGetStatus servoState {servoState}");
            Thread.Sleep(1000);

            retval = robot.AuxServoEnable(1, 1);
            Console.WriteLine($"AuxServoEnable enable {retval}");
            Thread.Sleep(1000);
            retval = robot.AuxServoGetStatus(1, ref servoErrCode, ref servoState, ref servoPos, ref servoSpeed, ref servoTorque);
            Console.WriteLine($"AuxServoGetStatus servoState {servoState}");
            Thread.Sleep(1000);

            retval = robot.AuxServoHoming(1, 1, 5, 1);
            Console.WriteLine($"AuxServoHoming {retval}");
            Thread.Sleep(3000);

            retval = robot.AuxServoSetTargetPos(1, 200, 30);
            Console.WriteLine($"AuxServoSetTargetPos {retval}");
            Thread.Sleep(1000);
            retval = robot.AuxServoGetStatus(1, ref servoErrCode, ref servoState, ref servoPos, ref servoSpeed, ref servoTorque);
            Console.WriteLine($"AuxServoGetStatus servoSpeed {servoSpeed}");
            Thread.Sleep(8000);

            robot.AuxServoSetControlMode(1, 1);
            Thread.Sleep(2000);

            robot.AuxServoEnable(1, 0);
            Thread.Sleep(1000);
            robot.AuxServoEnable(1, 1);
            Thread.Sleep(1000);
            robot.AuxServoSetTargetSpeed(1, 100, 80);

            Thread.Sleep(5000);
            robot.AuxServoSetTargetSpeed(1, 0, 80);
        }

        private void button65_Click(object sender, EventArgs e)
        {
            int rtn = robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 200, 1, 100, 5, 1);
            Console.WriteLine("ExtDevSetUDPComParam rtn is " + rtn);
            string ip = ""; int port = 0; int period = 0; int lossPkgTime = 0; int lossPkgNum = 0; int disconnectTime = 0; int reconnectEnable = 0; int reconnectPeriod = 0; int reconnectNum = 0; int selfConnect = 0;
            rtn = robot.ExtDevGetUDPComParam(ref ip, ref port, ref period, ref lossPkgTime, ref lossPkgNum, ref disconnectTime, ref reconnectEnable, ref reconnectPeriod, ref reconnectNum, ref selfConnect);
            string param = "\nip " + ip + "\nport " + port.ToString() + "\nperiod  " + period.ToString() + "\nlossPkgTime " + lossPkgTime.ToString() + "\nlossPkgNum  " + lossPkgNum.ToString() + "\ndisConntime  " + disconnectTime.ToString() + "\nreconnecable  " + reconnectEnable.ToString() + "\nreconnperiod  " + reconnectPeriod.ToString() + "\nreconnnun  " + reconnectNum.ToString() + "\nselfConnect  " + selfConnect.ToString();
            Console.WriteLine("ExtDevGetUDPComParam rtn is " + rtn + param);

            robot.ExtDevLoadUDPDriver();

            rtn = robot.ExtAxisServoOn(1, 1);
            Console.WriteLine("ExtAxisServoOn axis id 1 rtn is " + rtn);
            rtn = robot.ExtAxisServoOn(2, 1);
            Console.WriteLine("ExtAxisServoOn axis id 2 rtn is " + rtn);
            Thread.Sleep(2000);

            rtn = robot.ExtAxisSetHoming(1, 0, 10, 2);
            Console.WriteLine("ExtAxisSetHoming 1 rtnn is  " + rtn);
            Thread.Sleep(2000);
            rtn = robot.ExtAxisSetHoming(2, 0, 10, 2);
            Console.WriteLine("ExtAxisSetHoming 2 rtnn is  " + rtn);

            Thread.Sleep(4000);

            rtn = robot.SetRobotPosToAxis(1);
            Console.WriteLine("SetRobotPosToAxis rtn is " + rtn);
            rtn = robot.SetAxisDHParaConfig(10, 20, 0, 0, 0, 0, 0, 0, 0);
            Console.WriteLine("SetAxisDHParaConfig rtn is " + rtn);


            int axisType = -1;
            int axisDirection = -1;
            double axisMax = -1;
            double axisMin = -1;
            double axisVel = -1;
            double axisAcc = -1;
            double axisLead = -1;
            int encResolution = -1;
            double axisOffect = -1;
            int axisCompany = -1;
            int axisModel = -1;
            int axisEncType = -1;

            rtn = robot.ExtAxisParamConfig(1, 1, 1, 1000, -1000, 1000, 1000, 1.905f, 262144, 200, 1, 0, 0);
            Console.WriteLine("ExtAxisParamConfig axis 1 rtn is " + rtn);
            rtn = robot.ExtAxisGetParamConfig(1, ref axisType, ref axisDirection, ref axisMax, ref axisMin, ref axisVel, ref axisAcc, ref axisLead, ref encResolution, ref axisOffect, ref axisCompany, ref axisModel, ref axisEncType);
            Console.WriteLine($"axis id 1 ExtAxisGetParamConfig : axisType {axisType}, axisDirection {axisDirection}, axisMax {axisMax}, axisMin {axisMin}, axisVel {axisVel}, axisAcc {axisAcc}, axisLead {axisLead}, encResolution {encResolution}, axisOffect {axisOffect}, axisCompany {axisCompany}, axisModel {axisModel}, axisEncType {axisEncType}\n");


            rtn = robot.ExtAxisParamConfig(2, 1, 1, 1000, -1000, 1000, 1000, 4.444f, 262144, 200, 1, 0, 0);
            Console.WriteLine("ExtAxisParamConfig axis 2 rtn is " + rtn);
            rtn = robot.ExtAxisGetParamConfig(2, ref axisType, ref axisDirection, ref axisMax, ref axisMin, ref axisVel, ref axisAcc, ref axisLead, ref encResolution, ref axisOffect, ref axisCompany, ref axisModel, ref axisEncType);
            Console.WriteLine($"axis id 2 ExtAxisGetParamConfig : axisType {axisType}, axisDirection {axisDirection}, axisMax {axisMax}, axisMin {axisMin}, axisVel {axisVel}, axisAcc {axisAcc}, axisLead {axisLead}, encResolution {encResolution}, axisOffect {axisOffect}, axisCompany {axisCompany}, axisModel {axisModel}, axisEncType {axisEncType}\n");


            Thread.Sleep(3000);
            robot.ExtAxisStartJog(1, 0, 10, 10, 30);
            Thread.Sleep(1000);
            robot.ExtAxisStopJog(1);

            Thread.Sleep(3000);
            robot.ExtAxisServoOn(1, 0);

            Thread.Sleep(3000);
            robot.ExtAxisStartJog(2, 0, 10, 10, 30);
            Thread.Sleep(1000);
            robot.ExtAxisStopJog(2);

            Thread.Sleep(3000);
            robot.ExtAxisServoOn(2, 0);
            Thread.Sleep(3000);
            robot.ExtDevUnloadUDPDriver();


            ExaxisPos axisPos = new ExaxisPos(20, 0, 0, 0);
            robot.ExtAxisMove(axisPos, 50);

            ExaxisPos axisPos1 = new ExaxisPos(35, 0, 0, 0);
            robot.ExtAxisMove(axisPos1, 50);

            ExaxisPos axisPos2 = new ExaxisPos(0, 0, 0, 0);
            robot.ExtAxisMove(axisPos2, 50);

        }

        private void button66_Click(object sender, EventArgs e)
        {
            int rtn = 0;
            //int rtn = robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 200, 1, 100, 5, 1);
            //Console.WriteLine("ExtDevSetUDPComParam rtn is " + rtn);
            //string ip = ""; int port = 0; int period = 0; int lossPkgTime = 0; int lossPkgNum = 0; int disconnectTime = 0; int reconnectEnable = 0; int reconnectPeriod = 0; int reconnectNum = 0; int selfConnect = 0;
            //rtn = robot.ExtDevGetUDPComParam(ref ip, ref port, ref period, ref lossPkgTime, ref lossPkgNum, ref disconnectTime, ref reconnectEnable, ref reconnectPeriod, ref reconnectNum, ref selfConnect);
            //string param = "\nip " + ip + "\nport " + port.ToString() + "\nperiod  " + period.ToString() + "\nlossPkgTime " + lossPkgTime.ToString() + "\nlossPkgNum  " + lossPkgNum.ToString() + "\ndisConntime  " + disconnectTime.ToString() + "\nreconnecable  " + reconnectEnable.ToString() + "\nreconnperiod  " + reconnectPeriod.ToString() + "\nreconnnun  " + reconnectNum.ToString() + "\nselfConnect" + selfConnect.ToString();
            //Console.WriteLine("ExtDevGetUDPComParam rtn is " + rtn + param);

            //robot.ExtDevLoadUDPDriver();

            //rtn = robot.ExtAxisServoOn(1, 1);
            //Console.WriteLine("ExtAxisServoOn axis id 1 rtn is " + rtn);
            //rtn = robot.ExtAxisServoOn(2, 1);
            //Console.WriteLine("ExtAxisServoOn axis id 2 rtn is " + rtn);
            //Thread.Sleep(2000);

            //robot.ExtAxisSetHoming(1, 0, 10, 2);
            //Thread.Sleep(2000);
            //rtn = robot.ExtAxisSetHoming(2, 0, 10, 2);
            //Console.WriteLine("ExtAxisSetHoming rtnn is  " + rtn);

            //Thread.Sleep(4000);

            rtn = robot.SetRobotPosToAxis(1);
            Console.WriteLine("SetRobotPosToAxis rtn is " + rtn);
            rtn = robot.SetAxisDHParaConfig(1, 128.5f, 206.4f, 0, 0, 0, 0, 0, 0);
            Console.WriteLine("SetAxisDHParaConfig rtn is " + rtn);
            //rtn = robot.ExtAxisParamConfig(1, 1, 1, 1000, -1000, 1000, 1000, 1.905f, 262144, 200, 1, 0, 0);
            //Console.WriteLine("ExtAxisParamConfig axis 1 rtn is " + rtn);
            //rtn = robot.ExtAxisParamConfig(2, 1, 1, 1000, -1000, 1000, 1000, 4.444f, 262144, 200, 1, 0, 0);
            //Console.WriteLine("ExtAxisParamConfig axis 1 rtn is " + rtn);

            DescPose toolCoord = new DescPose(0, 0, 300, 0, 0, 0);
            robot.SetToolCoord(1, toolCoord, 0, 0, 1, 0);

            JointPos jSafe = new JointPos(47.434, -74.061, -46.445, -140.394, 52.175, 108.040);
            JointPos j1 = new JointPos(46.778, -75.370, -45.376, -140.058, 51.582, 108.038);
            JointPos j2 = new JointPos(26.821, -79.971, -41.801, -124.459, 65.051, 108.036);
            JointPos j3 = new JointPos(26.709, -82.025, -39.224, -124.958, 64.560, 108.035);
            JointPos j4 = new JointPos(27.177, -82.909, -38.352, -124.937, 63.591, 108.035);

            //JointPos jSafe = new JointPos(115.193f, -96.149f, 92.489f, -87.068f, -89.15f, -83.488f);
            //JointPos j1 = new JointPos(117.559f, -92.624f, 100.329f, -96.909f, -94.057f, -83.488f);
            //JointPos j2 = new JointPos(112.239f, -90.096f, 99.282f, -95.909f, -89.824f, -83.488f);
            //JointPos j3 = new JointPos(110.839f, -83.473f, 93.166f, -89.22f, -90.499f, -83.487f);
            //JointPos j4 = new JointPos(107.935f, -83.572f, 95.424f, -92.873f, -87.933f, -83.488f);

            DescPose descSafe = new DescPose();
            DescPose desc1 = new DescPose();
            DescPose desc2 = new DescPose();
            DescPose desc3 = new DescPose();
            DescPose desc4 = new DescPose();
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.GetForwardKin(jSafe, ref descSafe);
            robot.MoveJ(jSafe, descSafe, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            Thread.Sleep(2000);

            ExaxisPos axisPos1 = new ExaxisPos(35, 22, 0, 0);
            robot.ExtAxisMove(axisPos1, 50);

            robot.GetForwardKin(j1, ref desc1);
            robot.MoveJ(j1, desc1, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            Thread.Sleep(2000);

            DescPose actualTCPPos = new DescPose();
            robot.GetActualTCPPose(0, ref actualTCPPos);
            robot.SetRefPointInExAxisEnd(actualTCPPos);
            rtn = robot.PositionorSetRefPoint(1);
            Console.WriteLine("PositionorSetRefPoint 1 rtn is " + rtn);
            Thread.Sleep(2000);

            //robot.MoveJ(jSafe, descSafe, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            //robot.ExtAxisStartJog(1, 0, 50, 50, 0);
            //Thread.Sleep(1000);
            robot.ExtAxisStartJog(2, 1, 50, 50, 5);
            Thread.Sleep(1000);
            robot.GetForwardKin(j2, ref desc2);
            rtn = robot.MoveJ(j2, desc2, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            rtn = robot.PositionorSetRefPoint(2);
            Console.WriteLine("PositionorSetRefPoint 2 rtn is " + rtn);
            Thread.Sleep(2000);

            //robot.MoveJ(jSafe, descSafe, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            //robot.ExtAxisStartJog(1, 0, 50, 50, 10);
            //Thread.Sleep(1000);
            robot.ExtAxisStartJog(2, 1, 50, 50, 5);
            Thread.Sleep(1000);
            robot.GetForwardKin(j3, ref desc3);
            robot.MoveJ(j3, desc3, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            rtn = robot.PositionorSetRefPoint(3);
            Console.WriteLine("PositionorSetRefPoint 3 rtn is " + rtn);
            Thread.Sleep(2000);

            //robot.MoveJ(jSafe, descSafe, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            //robot.ExtAxisStartJog(1, 0, 50, 50, 10);
            //Thread.Sleep(1000);
            robot.ExtAxisStartJog(2, 1, 50, 50, 5);
            Thread.Sleep(1000);
            robot.GetForwardKin(j4, ref desc4);
            robot.MoveJ(j4, desc4, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            rtn = robot.PositionorSetRefPoint(4);
            Console.WriteLine("PositionorSetRefPoint 4 rtn is " + rtn);
            Thread.Sleep(2000);

            DescPose axisCoord = new DescPose();
            robot.PositionorComputeECoordSys(ref axisCoord);
            //robot.MoveJ(jSafe, descSafe, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            Console.WriteLine("PositionorComputeECoordSys rtn is {0} {1} {2} {3} {4} {5}", axisCoord.tran.x, axisCoord.tran.y, axisCoord.tran.z, axisCoord.rpy.rx, axisCoord.rpy.ry, axisCoord.rpy.rz);
            rtn = robot.ExtAxisActiveECoordSys(3, 3, axisCoord, 1);
            Console.WriteLine("ExtAxisActiveECoordSys rtn is " + rtn);
        }

        private void button67_Click(object sender, EventArgs e)
        {
            int rtn;
            for (int i = 0; i < 128; i++)
            {
                robot.SetAuxDO(i, true, false, true);
                Thread.Sleep(100);
            }
            for (int i = 0; i < 128; i++)
            {
                robot.SetAuxDO(i, false, false, true);
                Thread.Sleep(100);
            }

            for (int i = 0; i < 409; i++)
            {
                robot.SetAuxAO(0, i * 10, true);
                robot.SetAuxAO(1, 4095 - i * 10, true);
                robot.SetAuxAO(2, i * 10, true);
                robot.SetAuxAO(3, 4095 - i * 10, true);
                Thread.Sleep(10);
            }

            robot.SetAuxDIFilterTime(10);
            robot.SetAuxAIFilterTime(0, 10);

            for (int i = 0; i < 20; i++)
            {
                bool curValue = false;
                rtn = robot.GetAuxDI(i, false, ref curValue);
                Console.WriteLine("DI" + i + "   " + curValue);
            }
            int curValueAI = -1;
            for (int i = 0; i < 4; i++)
            {
                rtn = robot.GetAuxAI(i, true, ref curValueAI);
            }

            robot.WaitAuxDI(1, false, 1000, false);
            robot.WaitAuxAI(1, 1, 132, 1000, false);
        }

        private void button68_Click(object sender, EventArgs e)
        {
            int rtn;
            robot.ExtDevSetUDPComParam("192.168.58.2", 2021, 2, 50, 5, 50, 1, 50, 10, 1);
            robot.ExtDevLoadUDPDriver();

            rtn = robot.ExtAxisServoOn(1, 1);
            rtn = robot.ExtAxisServoOn(2, 1);
            Thread.Sleep(2000);

            robot.ExtAxisSetHoming(1, 0, 10, 2);
            Thread.Sleep(2000);
            rtn = robot.ExtAxisSetHoming(2, 0, 10, 2);

            Thread.Sleep(4000);

            robot.ExtAxisParamConfig(1, 0, 0, 50000, -50000, 1000, 1000, 6.280f, 16384, 200, 0, 0, 0);
            robot.ExtAxisParamConfig(2, 0, 0, 50000, -50000, 1000, 1000, 6.280f, 16384, 200, 0, 0, 0);
            robot.SetAxisDHParaConfig(5, 0, 0, 0, 0, 0, 0, 0, 0);

            robot.TractorEnable(false);
            Thread.Sleep(2000);
            robot.TractorEnable(true);
            Thread.Sleep(2000);
            robot.TractorHoming();
            Thread.Sleep(2000);
            robot.TractorMoveL(100, 2);
            Thread.Sleep(5000);
            robot.TractorStop();
            robot.TractorMoveL(-100, 20);
            Thread.Sleep(5000);
            robot.TractorMoveC(300, 90, 20);
            Thread.Sleep(10000);
            robot.TractorMoveC(300, -90, 20);
            Thread.Sleep(1);
        }

        private void button69_Click(object sender, EventArgs e)
        {
            int rtn;
            JointPos startjointPos = new JointPos(-11.904f, -99.669f, 117.473f, -108.616f, -91.726f, 74.256f);
            JointPos midjointPos = new JointPos(-45.615f, -106.172f, 124.296f, -107.151f, -91.282f, 74.255f);
            JointPos endjointPos = new JointPos(-29.777f, -84.536f, 109.275f, -114.075f, -86.655f, 74.257f);

            DescPose startdescPose = new DescPose(-419.524f, -13.000f, 351.569f, -178.118f, 0.314f, 3.833f);
            DescPose middescPose = new DescPose(-321.222f, 185.189f, 335.520f, -179.030f, -1.284f, -29.869f);
            DescPose enddescPose = new DescPose(-487.434f, 154.362f, 308.576f, 176.600f, 0.268f, -14.061f);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            rtn = robot.PtpFIRPlanningStart(1000, 1000);
            Console.WriteLine("PtpFIRPlanningStart rtn is " + rtn);
            robot.MoveJ(startjointPos, startdescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.MoveJ(endjointPos, enddescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.PtpFIRPlanningEnd();
            Console.WriteLine("PtpFIRPlanningEnd rtn is " + rtn);

            robot.LinArcFIRPlanningStart(1000, 1000, 1000, 1000);
            Console.WriteLine("LinArcFIRPlanningStart rtn is " + rtn);
            robot.MoveL(startjointPos, startdescPose, 0, 0, 20, 100, 100, -1,0, exaxisPos, 0, 0, offdese, 1, 50);
            robot.MoveC(midjointPos, middescPose, 0, 0, 100, 100, exaxisPos, 0, offdese, endjointPos, enddescPose, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1, 100, 0);
            robot.LinArcFIRPlanningEnd();
            Console.WriteLine("LinArcFIRPlanningEnd rtn is " + rtn);

        }

        private void button70_Click(object sender, EventArgs e)
        {
            int rtn;
            JointPos startjointPos = new JointPos(-11.904f, -99.669f, 117.473f, -108.616f, -91.726f, 74.256f);
            JointPos endjointPos = new JointPos(-45.615f, -106.172f, 124.296f, -107.151f, -91.282f, 74.255f);

            DescPose startdescPose = new DescPose(-419.524f, -13.000f, 351.569f, -178.118f, 0.314f, 3.833f);
            DescPose enddescPose = new DescPose(-321.222f, 185.189f, 335.520f, -179.030f, -1.284f, -29.869f);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            rtn = robot.AccSmoothStart(false);
            Console.WriteLine("AccSmoothStart rtn is " + rtn);
            robot.MoveJ(startjointPos, startdescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.MoveJ(endjointPos, enddescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            rtn = robot.AccSmoothEnd(false);
            Console.WriteLine("AccSmoothEnd rtn is " + rtn);
        }

        private void button71_Click(object sender, EventArgs e)
        {
            int rtn;
            JointPos startjointPos = new JointPos(-11.904f, -99.669f, 117.473f, -108.616f, -91.726f, 74.256f);
            JointPos endjointPos = new JointPos(-45.615f, -106.172f, 124.296f, -107.151f, -91.282f, 74.255f);

            DescPose startdescPose = new DescPose(-419.524f, -13.000f, 351.569f, -178.118f, 0.314f, 3.833f);
            DescPose enddescPose = new DescPose(-321.222f, 185.189f, 335.520f, -179.030f, -1.284f, -29.869f);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            rtn = robot.AngularSpeedStart(50);
            Console.WriteLine("AngularSpeedStart rtn is " + rtn);
            robot.MoveJ(startjointPos, startdescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.MoveJ(endjointPos, enddescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            rtn = robot.AngularSpeedEnd();
            Console.WriteLine("AngularSpeedEnd rtn is " + rtn);
        }

        private void button72_Click(object sender, EventArgs e)
        {
            int rtn;
            JointPos startjointPos = new JointPos(-11.904f, -99.669f, 117.473f, -108.616f, -91.726f, 74.256f);
            JointPos endjointPos = new JointPos(-45.615f, -106.172f, 124.296f, -107.151f, -91.282f, 74.255f);

            DescPose startdescPose = new DescPose(-419.524f, -13.000f, 351.569f, -178.118f, 0.314f, 3.833f);
            DescPose enddescPose = new DescPose(-321.222f, 185.189f, 335.520f, -179.030f, -1.284f, -29.869f);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            rtn = robot.SingularAvoidStart(2, 10, 5, 5);
            Console.WriteLine("SingularAvoidStart rtn is " + rtn);
            robot.MoveJ(startjointPos, startdescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.MoveJ(endjointPos, enddescPose, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            rtn = robot.SingularAvoidEnd();
            Console.WriteLine("SingularAvoidEnd rtn is " + rtn);
        }

        private void button73_Click(object sender, EventArgs e)
        {
            int rtn;
            rtn = robot.TrajectoryJUpLoad("D://zUP/horse.txt");
            Console.WriteLine("Upload TrajectoryJ A " + rtn);

            string traj_file_name = "horse.txt";
            rtn = robot.LoadTrajectoryLA(traj_file_name, 1, 2, 0, 2, 100, 200, 1000, 1);
            Console.WriteLine("LoadTrajectoryLA " + traj_file_name + ", rtn is: " + rtn);

            DescPose traj_start_pose = new DescPose();
            rtn = robot.GetTrajectoryStartPose(traj_file_name, ref traj_start_pose);
            Console.WriteLine("GetTrajectoryStartPose is: " + rtn);
            Console.WriteLine("desc_pos:{0},{1},{2},{3},{4},{5}", traj_start_pose.tran.x, traj_start_pose.tran.y, traj_start_pose.tran.z, traj_start_pose.rpy.rx, traj_start_pose.rpy.ry, traj_start_pose.rpy.rz);

            Thread.Sleep(1000);

            robot.SetSpeed(50);
            robot.MoveCart(traj_start_pose, 0, 0, 100, 100, 100, -1, -1);

            rtn = robot.MoveTrajectoryLA();
            Console.WriteLine("MoveTrajectoryLA rtn is: " + rtn);
        }

        private void button74_Click(object sender, EventArgs e)
        {
            int retval = 0;

            retval = robot.LoadIdentifyDynFilterInit();
            Console.WriteLine("LoadIdentifyDynFilterInit retval is: " + retval);

            retval = robot.LoadIdentifyDynVarInit();
            Console.WriteLine("LoadIdentifyDynVarInit retval is: " + retval);

            JointPos posJ = new JointPos(0, 0, 0, 0, 0, 0);
            DescPose posDec = new DescPose(0, 0, 0, 0, 0, 0);
            double[] joint_toq = new double[6] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            robot.GetActualJointPosDegree(0, ref posJ);
            posJ.jPos[1] = posJ.jPos[1] + 10;
            robot.GetJointTorques(0, joint_toq);
            joint_toq[1] = joint_toq[1] + 2;

            double[] tmpTorque = new double[6] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            for (int i = 0; i < 6; i++)
            {
                tmpTorque[i] = joint_toq[i];
            }

            retval = robot.LoadIdentifyMain(tmpTorque, posJ.jPos, 1);
            Console.WriteLine("LoadIdentifyMain retval is: " + retval);

            double[] gain = new double[12] { 0, 0.05, 0, 0, 0, 0, 0, 0.02, 0, 0, 0, 0 };
            double weight = 0;
            DescTran load_pos = new DescTran(0, 0, 0);
            retval = robot.LoadIdentifyGetResult(gain, ref weight, ref load_pos);
            Console.WriteLine("LoadIdentifyGetResult retval is: {0}; weight is {1} cog is {2} {3} {4}", retval, weight, load_pos.x, load_pos.y, load_pos.z);
        }

        private void button75_Click(object sender, EventArgs e)
        {
            DescPose middescPoseCir1 = new DescPose(-435.414, -342.926, 309.205, -171.382, -4.513, 171.520);
            JointPos midjointPosCir1 = new JointPos(26.804, -79.866, 106.642, -125.433, -85.562, -54.721);
            DescPose enddescPoseCir1 = new DescPose(-524.862, -217.402, 308.459, -171.425, -4.810, 156.088);
            JointPos endjointPosCir1 = new JointPos(11.399, -78.055, 104.603, -125.421, -85.770, -54.721);

            DescPose middescPoseCir2 = new DescPose(-482.691, -587.899, 318.594, -171.001, -4.999, -172.996);
            JointPos midjointPosCir2 = new JointPos(42.314, -53.600, 67.296, -112.969, -85.533, -54.721);
            DescPose enddescPoseCir2 = new DescPose(-403.942, -489.061, 317.038, -163.189, -10.425, -175.627);
            JointPos endjointPosCir2 = new JointPos(39.959, -70.616, 96.679, -134.243, -82.276, -54.721);

            DescPose middescPoseMoveC = new DescPose(-435.414, -342.926, 309.205, -171.382, -4.513, 171.520);
            JointPos midjointPosMoveC = new JointPos(26.804, -79.866, 106.642, -125.433, -85.562, -54.721);
            DescPose enddescPoseMoveC = new DescPose(-524.862, -217.402, 308.459, -171.425, -4.810, 156.088);
            JointPos endjointPosmoveC = new JointPos(11.399, -78.055, 104.603, -125.421, -85.770, -54.721);

            DescPose middescPoseCir3 = new DescPose(-435.414, -342.926, 309.205, -171.382, -4.513, 171.520);
            JointPos midjointPosCir3 = new JointPos(26.804, -79.866, 106.642, -125.433, -85.562, -54.721);
            DescPose enddescPoseCir3 = new DescPose(-569.505, -405.378, 357.596, -172.862, -10.939, 171.108);
            JointPos endjointPosCir3 = new JointPos(27.138, -63.750, 78.586, -117.861, -90.588, -54.721);

            DescPose middescPoseCir4 = new DescPose(-482.691, -587.899, 318.594, -171.001, -4.999, -172.996);
            JointPos midjointPosCir4 = new JointPos(42.314, -53.600, 67.296, -112.969, -85.533, -54.721);
            DescPose enddescPoseCir4 = new DescPose(-569.505, -405.378, 357.596, -172.862, -10.939, 171.108);
            JointPos endjointPosCir4 = new JointPos(27.138, -63.750, 78.586, -117.861, -90.588, -54.721);

            DescPose startdescPose = new DescPose(-569.505, -405.378, 357.596, -172.862, -10.939, 171.108);
            JointPos startjointPos = new JointPos(27.138, -63.750, 78.586, -117.861, -90.588, -54.721);

            DescPose linedescPose = new DescPose(-403.942, -489.061, 317.038, -163.189, -10.425, -175.627);
            JointPos linejointPos = new JointPos(39.959, -70.616, 96.679, -134.243, -82.276, -54.721);


            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);


            //robot.MoveJ(startjointPos, startdescPose, 3, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            //rtn = robot.Circle(midjointPosCir1, middescPoseCir1, 3, 0, 100, 100, exaxisPos, endjointPosCir1, enddescPoseCir1, 3, 0, 100, 100, exaxisPos, 100, -1, offdese, 100, 20);
            //Console.WriteLine("Circle1" + rtn);



            //rtn = robot.Circle(midjointPosCir2, middescPoseCir2, 3, 0, 100, 100, exaxisPos, endjointPosCir2, enddescPoseCir2, 3, 0, 100, 100, exaxisPos, 100, -1, offdese, 100, 20);
            //Console.WriteLine("Circle2" + rtn);

            //robot.MoveC(midjointPosMoveC, middescPoseMoveC, 3, 0, 100, 100, exaxisPos, 0, offdese, endjointPosmoveC, enddescPoseMoveC, 3, 0, 100, 100, exaxisPos, 0, offdese, 100, 20);
            //rtn = robot.Circle(midjointPosCir3, middescPoseCir3, 3, 0, 100, 100, exaxisPos, endjointPosCir3, enddescPoseCir3, 3, 0, 100, 100, exaxisPos, 100, -1, offdese, 100, 20);
            //Console.WriteLine("Circle3" + rtn);
            //rtn = robot.MoveL(linejointPos, linedescPose, 3, 0, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            //Console.WriteLine("MoveL " + rtn);
            //rtn = robot.Circle(midjointPosCir4, middescPoseCir4, 3, 0, 100, 100, exaxisPos, endjointPosCir4, enddescPoseCir4, 3, 0, 100, 100, exaxisPos, 100, -1, offdese, 100, 20);
            //Console.WriteLine("Circle4" + rtn);
        }

        private void button76_Click(object sender, EventArgs e)
        {
            double[] M = { 15.0, 15.0, 15.0, 0.5, 0.5, 0.1 };
            double[] B = { 150.0, 150.0, 150.0, 5.0, 5.0, 1.0 };
            double[] K = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] F = { 10.0, 10.0, 10.0, 1.0, 1.0, 1.0 };
            int rtn = robot.EndForceDragControl(1, 0, 0, 0, 1, M, B, K, F, 50, 100);
            Console.WriteLine("force drag control start rtn is{rtn}");
            Thread.Sleep(5000);

            rtn = robot.EndForceDragControl(0, 0, 0, 0, 1, M, B, K, F, 50, 100);
            Console.WriteLine($"force drag control end rtn is{rtn}");

            rtn = robot.ResetAllError();
            Console.WriteLine($"ResetAllError rtn is{rtn}");

            robot.EndForceDragControl(1, 0, 0, 0, 1, M, B, K, F, 50, 100);
            Console.WriteLine($"force drag control start again rtn is{rtn}");
            Thread.Sleep(5000);

            rtn = robot.EndForceDragControl(0, 0, 0, 0, 1, M, B, K, F, 50, 100);
            Console.WriteLine($"force drag control end again rtn is {rtn}");
        }

        private void button77_Click(object sender, EventArgs e)
        {
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            // First set of positions
            JointPos JP1 = new JointPos(55.203, -69.138, 75.617, -103.969, -83.549, -0.001);
            DescPose DP1 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP2 = new JointPos(57.646, -61.846, 59.286, -69.645, -99.735, 3.824);
            DescPose DP2 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP3 = new JointPos(57.304, -61.380, 58.260, -67.641, -97.447, 2.685);
            DescPose DP3 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP4 = new JointPos(57.297, -61.373, 58.250, -67.637, -97.448, 2.677);
            DescPose DP4 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP5 = new JointPos(23.845, -108.202, 111.300, -80.971, -106.753, -30.246);
            DescPose DP5 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP6 = new JointPos(23.845, -108.202, 111.300, -80.971, -106.753, -30.246);
            DescPose DP6 = new DescPose(0, 0, 0, 0, 0, 0);

            robot.GetForwardKin(JP1, ref DP1);
            robot.GetForwardKin(JP2, ref DP2);
            robot.GetForwardKin(JP3, ref DP3);
            robot.GetForwardKin(JP4, ref DP4);
            robot.GetForwardKin(JP5, ref DP5);
            robot.GetForwardKin(JP6, ref DP6);

            robot.MoveJ(JP1, DP1, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.MoveJ(JP2, DP2, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            robot.MoveJ(JP3, DP3, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            robot.MoveJ(JP4, DP4, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            robot.MoveJ(JP5, DP5, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            robot.MoveJ(JP6, DP6, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);

            // Second set of positions
            JointPos JP7 = new JointPos(-10.503, -93.654, 111.333, -84.702, -103.479, -30.179);
            DescPose DP7 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP8 = new JointPos(-10.503, -93.654, 111.333, -84.702, -103.479, -30.179);
            DescPose DP8 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP9 = new JointPos(-10.503, -93.654, 111.333, -84.702, -103.479, -30.179);
            DescPose DP9 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP10 = new JointPos(-30.623, -74.158, 89.844, -91.942, -97.060, -30.180);
            DescPose DP10 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP11 = new JointPos(-34.797, -72.641, 93.917, -104.961, -84.449, -30.287);
            DescPose DP11 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP12 = new JointPos(-17.454, -58.309, 82.054, -111.034, -109.900, -30.241);
            DescPose DP12 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP13 = new JointPos(-4.930, -72.469, 100.631, -109.906, -76.760, -10.947);
            DescPose DP13 = new DescPose(0, 0, 0, 0, 0, 0);

            robot.GetForwardKin(JP7, ref DP7);
            robot.GetForwardKin(JP8, ref DP8);
            robot.GetForwardKin(JP9, ref DP9);
            robot.GetForwardKin(JP10, ref DP10);
            robot.GetForwardKin(JP11, ref DP11);
            robot.GetForwardKin(JP12, ref DP12);
            robot.GetForwardKin(JP13, ref DP13);

            robot.MoveJ(JP7, DP7, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            robot.MoveL(JP8, DP8, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            robot.MoveJ(JP9, DP9, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            robot.MoveL(JP10, DP10, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            robot.MoveJ(JP11, DP11, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            // robot.MoveC(JP12, DP12, 0, 0, 100, 100, exaxisPos, 0, offdese, JP13, DP13, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 20);

            // Third set of positions
            JointPos JP14 = new JointPos(9.586, -66.925, 85.589, -99.109, -103.403, -30.280);
            DescPose DP14 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP15 = new JointPos(23.056, -59.187, 76.487, -102.155, -77.560, -30.250);
            DescPose DP15 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP16 = new JointPos(28.028, -71.754, 91.463, -102.182, -102.361, -30.253);
            DescPose DP16 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP17 = new JointPos(38.974, -62.622, 79.068, -102.543, -101.630, -30.253);
            DescPose DP17 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP18 = new JointPos(-34.797, -72.641, 93.917, -104.961, -84.449, -30.287);
            DescPose DP18 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP19 = new JointPos(-17.454, -58.309, 82.054, -111.034, -109.900, -30.241);
            DescPose DP19 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP20 = new JointPos(-4.930, -72.469, 100.631, -109.906, -76.760, -10.947);
            DescPose DP20 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP21 = new JointPos(3.021, -76.365, 81.332, -98.130, -68.530, -30.284);
            DescPose DP21 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP22 = new JointPos(12.532, -94.241, 106.254, -87.131, -102.719, -30.227);
            DescPose DP22 = new DescPose(0, 0, 0, 0, 0, 0);

            robot.GetForwardKin(JP14, ref DP14);
            robot.GetForwardKin(JP15, ref DP15);
            robot.GetForwardKin(JP16, ref DP16);
            robot.GetForwardKin(JP17, ref DP17);
            robot.GetForwardKin(JP18, ref DP18);
            robot.GetForwardKin(JP19, ref DP19);
            robot.GetForwardKin(JP20, ref DP20);
            robot.GetForwardKin(JP21, ref DP21);
            robot.GetForwardKin(JP22, ref DP22);

            robot.MoveJ(JP14, DP14, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            //robot.Circle(JP15, DP15, 0, 0, 100, 100, exaxisPos, JP16, DP16, 0, 0, 100, 100, exaxisPos, 100, 0, offdese, 100, 20);
            //robot.MoveJ(JP17, DP17, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            //robot.MoveL(JP18, DP18, 0, 0, 100, 100, 100, 100, 0, exaxisPos, 0, 0, offdese);
            //robot.MoveC(JP19, DP19, 0, 0, 100, 100, exaxisPos, 0, offdese, JP20, DP20, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 20);
            //robot.MoveC(JP21, DP21, 0, 0, 100, 100, exaxisPos, 0, offdese, JP22, DP22, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 20);

            // Fourth set of positions
            JointPos JP23 = new JointPos(9.586, -66.925, 85.589, -99.109, -103.403, -30.280);
            DescPose DP23 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP24 = new JointPos(23.056, -59.187, 76.487, -102.155, -77.560, -30.250);
            DescPose DP24 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP25 = new JointPos(28.028, -71.754, 91.463, -102.182, -102.361, -30.253);
            DescPose DP25 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP26 = new JointPos(-11.207, -81.555, 110.050, -108.983, -74.292, -30.249);
            DescPose DP26 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP27 = new JointPos(18.930, -70.987, 100.659, -115.974, -115.465, -30.231);
            DescPose DP27 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP28 = new JointPos(32.493, -65.561, 86.053, -109.669, -103.427, -30.267);
            DescPose DP28 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP29 = new JointPos(21.954, -87.113, 123.299, -109.730, -72.157, -9.013);
            DescPose DP29 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP30 = new JointPos(19.084, -69.127, 104.304, -109.629, -106.997, -9.011);
            DescPose DP30 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP31 = new JointPos(38.654, -60.146, 93.485, -109.637, -87.023, -8.989);
            DescPose DP31 = new DescPose(0, 0, 0, 0, 0, 0);

            robot.GetForwardKin(JP23, ref DP23);
            robot.GetForwardKin(JP24, ref DP24);
            robot.GetForwardKin(JP25, ref DP25);
            robot.GetForwardKin(JP26, ref DP26);
            robot.GetForwardKin(JP27, ref DP27);
            robot.GetForwardKin(JP28, ref DP28);
            robot.GetForwardKin(JP29, ref DP29);
            robot.GetForwardKin(JP30, ref DP30);
            robot.GetForwardKin(JP31, ref DP31);

            //robot.MoveL(JP23, DP23, 0, 0, 100, 100, 100, 20, 1, exaxisPos, 0, 0, offdese);
            //robot.Circle(JP24, DP24, 0, 0, 100, 100, exaxisPos, JP25, DP25, 0, 0, 100, 100, exaxisPos, 100, 0, offdese, 100, 20);
            //robot.Circle(JP26, DP26, 0, 0, 100, 100, exaxisPos, JP27, DP27, 0, 0, 100, 100, exaxisPos, 100, 0, offdese, 100, 20);
            //robot.MoveC(JP28, DP28, 0, 0, 100, 100, exaxisPos, 0, offdese, JP29, DP29, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 20);
            //robot.Circle(JP30, DP30, 0, 0, 100, 100, exaxisPos, JP31, DP31, 0, 0, 100, 100, exaxisPos, 100, 0, offdese, 100, 20);

            // Fifth set of positions
            JointPos JP32 = new JointPos(38.654, -60.146, 93.485, -109.637, -87.023, -8.989);
            DescPose DP32 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP33 = new JointPos(55.203, -69.138, 75.617, -103.969, -83.549, -0.001);
            DescPose DP33 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP34 = new JointPos(57.646, -61.846, 59.286, -69.645, -99.735, 3.824);
            DescPose DP34 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP35 = new JointPos(57.304, -61.380, 58.260, -67.641, -97.447, 2.685);
            DescPose DP35 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP36 = new JointPos(57.297, -61.373, 58.250, -67.637, -97.448, 2.677);
            DescPose DP36 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP37 = new JointPos(23.845, -108.202, 111.300, -80.971, -106.753, -30.246);
            DescPose DP37 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP38 = new JointPos(23.845, -108.202, 111.300, -80.971, -106.753, -30.246);
            DescPose DP38 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP39 = new JointPos(-10.503, -93.654, 111.333, -84.702, -103.479, -30.179);
            DescPose DP39 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP40 = new JointPos(-30.623, -74.158, 89.844, -91.942, -97.060, -30.180);
            DescPose DP40 = new DescPose(0, 0, 0, 0, 0, 0);

            robot.GetForwardKin(JP32, ref DP32);
            robot.GetForwardKin(JP33, ref DP33);
            robot.GetForwardKin(JP34, ref DP34);
            robot.GetForwardKin(JP35, ref DP35);
            robot.GetForwardKin(JP36, ref DP36);
            robot.GetForwardKin(JP37, ref DP37);
            robot.GetForwardKin(JP38, ref DP38);
            robot.GetForwardKin(JP39, ref DP39);
            robot.GetForwardKin(JP40, ref DP40);

            robot.MoveL(JP32, DP32, 0, 0, 100, 100, 100, 20, 1, exaxisPos, 0, 0, offdese);
            robot.MoveJ(JP33, DP33, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.MoveL(JP34, DP34, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            robot.MoveL(JP35, DP35, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            robot.MoveL(JP36, DP36, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            robot.MoveL(JP37, DP37, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            robot.MoveL(JP38, DP38, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            robot.MoveL(JP39, DP39, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            robot.MoveJ(JP40, DP40, 0, 0, 100, 100, 100, exaxisPos, 20, 0, offdese);

            // Sixth set of positions
            JointPos JP50 = new JointPos(-34.797, -72.641, 93.917, -104.961, -84.449, -30.287);
            DescPose DP50 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP41 = new JointPos(-17.454, -58.309, 82.054, -111.034, -109.900, -30.241);
            DescPose DP41 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP42 = new JointPos(-4.930, -72.469, 100.631, -109.906, -76.760, -10.947);
            DescPose DP42 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP43 = new JointPos(9.586, -66.925, 85.589, -99.109, -103.403, -30.280);
            DescPose DP43 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP44 = new JointPos(23.056, -59.187, 76.487, -102.155, -77.560, -30.250);
            DescPose DP44 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP45 = new JointPos(28.028, -71.754, 91.463, -102.182, -102.361, -30.253);
            DescPose DP45 = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP46 = new JointPos(38.974, -62.622, 79.068, -102.543, -101.630, -30.253);
            DescPose DP46 = new DescPose(0, 0, 0, 0, 0, 0);

            robot.GetForwardKin(JP50, ref DP50);
            robot.GetForwardKin(JP41, ref DP41);
            robot.GetForwardKin(JP42, ref DP42);
            robot.GetForwardKin(JP43, ref DP43);
            robot.GetForwardKin(JP44, ref DP44);
            robot.GetForwardKin(JP45, ref DP45);
            robot.GetForwardKin(JP46, ref DP46);

            //robot.MoveL(JP50, DP50, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            //robot.MoveC(JP41, DP41, 0, 0, 100, 100, exaxisPos, 0, offdese, JP42, DP42, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 20);
            //robot.MoveL(JP43, DP43, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            //robot.Circle(JP44, DP44, 0, 0, 100, 100, exaxisPos, JP45, DP45, 0, 0, 100, 100, exaxisPos, 100, 0, offdese, 100, 20);
            //robot.MoveL(JP46, DP46, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);


        }

        private void button78_Click(object sender, EventArgs e)
        {
            var pkg = new ROBOT_STATE_PKG();
            robot.SetWideBoxTempFanMonitorParam(1, 2);
            int enable = 0;
            int period = 0;
            robot.GetWideBoxTempFanMonitorParam(ref enable, ref period);
            Console.WriteLine($"GetWideBoxTempFanMonitorParam enable is {enable}   period is {period}");
            for (int i = 0; i < 100; i++)
            {
                robot.GetRobotRealTimeState(ref pkg);
                Console.WriteLine($"robot ctrl box temp is {pkg.wideVoltageCtrlBoxTemp}, fan current is {pkg.wideVoltageCtrlBoxFanVel}");
                Thread.Sleep(100);
            }
            int rtn = robot.SetWideBoxTempFanMonitorParam(0, 2);
            Console.WriteLine($"SetWideBoxTempFanMonitorParam rtn is {rtn}");
            enable = 0;
            period = 0;
            robot.GetWideBoxTempFanMonitorParam(ref enable, ref period);
            Console.WriteLine($"GetWideBoxTempFanMonitorParam enable is {enable}   period is {period}");
            for (int i = 0; i < 100; i++)
            {
                robot.GetRobotRealTimeState(ref pkg);
                Console.WriteLine($" robot ctrl box temp is {pkg.wideVoltageCtrlBoxTemp}, fan current is {pkg.wideVoltageCtrlBoxFanVel}");
                Thread.Sleep(100);
            }




        }

        private void button79_Click(object sender, EventArgs e)
        {
            JointPos j = new JointPos(0, 0, 0, 0, 0, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);

            float vel = 0.0f;
            float acc = 0.0f;
            float cmdT = 0.008f;
            float filterT = 0.0f;
            float gain = 0.0f;
            byte flag = 0;
            int count = 500;
            float dt = 0.1f;
            int cmdID = 0;
            int ret = robot.GetActualJointPosDegree(flag, ref j);
            if (ret == 0)
            {
                robot.ServoMoveStart();

                try
                {
                    while (count > 0)
                    {

                        robot.ServoJ(j, epos, acc, vel, cmdT, filterT, gain, cmdID);


                        j.jPos[0] += dt;
                        count--;


                        robot.WaitMs((int)(cmdT * 1000));
                    }
                }
                finally
                {

                    robot.ServoMoveEnd();
                }
            }
            else
            {
                Console.WriteLine($"GetActualJointPosDegree error code: {ret}");

            }
        }

        private void button80_Click(object sender, EventArgs e)
        {
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos JP1 = new JointPos(43.849, -71.535, 109.564, -135.187, -89.016, 18.225);
            DescPose DP1 = new DescPose(-331.581, -462.334, 225.274, -173.501, 3.161, 115.864);
            JointPos JP2 = new JointPos(67.950, -31.106, 58.618, -135.151, -89.017, 18.226);
            DescPose DP2 = new DescPose(-219.871, -819.093, 124.722, -163.475, 6.333, 140.797);
            JointPos JP3 = new JointPos(97.159, -26.141, 53.021, -120.936, -103.329, 18.230);
            DescPose DP3 = new DescPose(183.319, -826.070, 70.807, -171.844, -11.320, 167.645);

            JointPos JP4 = new JointPos(43.849, -71.535, 109.564, -135.187, -89.016, 18.225);
            DescPose DP4 = new DescPose(-331.581, -462.334, 225.274, -173.501, 3.161, 115.864);

            JointPos JP5 = new JointPos(67.950, -31.106, 58.618, -135.151, -89.017, 18.226);
            DescPose DP5 = new DescPose(-219.871, -819.093, 124.722, -163.475, 6.333, 140.797);

            JointPos JP6 = new JointPos(105.694, -125.732, 124.263, -105.860, -90.554, 18.230);
            DescPose DP6 = new DescPose(171.338, -236.287, 442.053, -163.332, 4.843, 178.090);
            robot.LinArcFIRPlanningStart(2000, 10000, 720, 1440);

            robot.MoveL(JP1, DP1, 0, 0, 100, 100, 100, 20, -1, exaxisPos, 0, 0, offdese);

            robot.MoveL(JP2, DP2, 0, 0, 100, 100, 100, 20, 100, exaxisPos, 0, 0, offdese);
            robot.MoveL(JP3, DP3, 0, 0, 100, 100, 100, 20, 100, exaxisPos, 0, 0, offdese);



            robot.LinArcFIRPlanningEnd();

            robot.PtpFIRPlanningStart(240, 1200);

            robot.MoveJ(JP4, DP4, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.MoveJ(JP5, DP5, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            robot.MoveJ(JP6, DP6, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);

            robot.PtpFIRPlanningEnd();

            JointPos JP7 = new JointPos(138.430, -103.926, 135.390, -120.507, -116.912, 18.198);
            DescPose DP7 = new DescPose(288.379, -179.924, 267.471, -171.989, -25.794, -151.376);

            JointPos JP8 = new JointPos(122.158, -69.748, 92.480, -120.510, -116.988, 18.175);
            DescPose DP8 = new DescPose(380.357, -498.600, 323.600, -163.066, -22.643, -171.300);
            JointPos JP88 = new JointPos(70.960, -53.189, 85.689, -123.253, -116.780, 18.175);
            DescPose DP88 = new DescPose(-171.581, -671.727, 192.097, -170.274, -25.085, 140.438);

            robot.LinArcFIRPlanningStart(2000, 10000, 720, 1440);
            robot.MoveL(JP7, DP7, 0, 0, 100, 100, 100, 20, 50, exaxisPos, 0, 0, offdese);
            robot.MoveC(JP8, DP8, 0, 0, 100, 100, exaxisPos, 0, offdese, JP88, DP88, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1,100, 0);
            robot.LinArcFIRPlanningEnd();
            JointPos JP9 = new JointPos(138.430, -103.926, 135.390, -120.507, -116.912, 18.198);
            DescPose DP9 = new DescPose(288.379, -179.924, 267.471, -171.989, -25.794, -151.376);
            JointPos JP10 = new JointPos(122.158, -69.748, 92.480, -120.510, -116.988, 18.175);
            DescPose DP10 = new DescPose(380.357, -498.600, 323.600, -163.066, -22.643, -171.300);

            JointPos JP10_ = new JointPos(70.960, -53.189, 85.689, -123.253, -116.780, 18.175);
            DescPose DP10_ = new DescPose(-171.581, -671.727, 192.097, -170.274, -25.085, 140.438);

            JointPos JP11 = new JointPos(38.619, -93.376, 100.695, -79.572, -116.773, 18.172);
            DescPose DP11 = new DescPose(-305.647, -317.052, 409.820, 169.616, -30.178, 117.509);
            JointPos JP11_ = new JointPos(110.873, -113.738, 126.180, -79.561, -116.964, 18.173);
            DescPose DP11_ = new DescPose(150.549, -235.789, 334.164, 163.763, -31.210, -167.182);
            robot.LinArcFIRPlanningStart(2000, 10000, 720, 1440);
            robot.MoveL(JP9, DP9, 0, 0, 100, 100, 100, 20, -1, exaxisPos, 0, 0, offdese);
            //robot.MoveC(JP10, DP10, 0, 0, 100, 100, exaxisPos, 0, offdese, JP10_, DP10_, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 120);
            //robot.MoveC(JP11, DP11, 0, 0, 100, 100, exaxisPos, 0, offdese, JP11_, DP11_, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1);

            robot.LinArcFIRPlanningEnd(); ;
            JointPos JP12 = new JointPos(138.430, -103.926, 135.390, -120.507, -116.912, 18.198);
            DescPose DP12 = new DescPose(288.379, -179.924, 267.471, -171.989, -25.794, -151.376);
            JointPos JP13 = new JointPos(122.158, -69.748, 92.480, -120.510, -116.988, 18.175);
            DescPose DP13 = new DescPose(380.357, -498.600, 323.600, -163.066, -22.643, -171.300);

            JointPos JP13_ = new JointPos(70.960, -53.189, 85.689, -123.253, -116.780, 18.175);
            DescPose DP13_ = new DescPose(-171.581, -671.727, 192.097, -170.274, -25.085, 140.438);
            JointPos JP14 = new JointPos(38.619, -93.376, 100.695, -79.572, -116.773, 18.172);
            DescPose DP14 = new DescPose(-305.647, -317.052, 409.820, 169.616, -30.178, 117.509);
            robot.LinArcFIRPlanningStart(2000, 10000, 720, 1440);
            robot.MoveL(JP12, DP12, 0, 0, 100, 100, 100, 20, -1, exaxisPos, 0, 0, offdese);
            robot.MoveC(JP13, DP13, 0, 0, 100, 100, exaxisPos, 0, offdese, JP13_, DP13_, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1,100, 0);


            robot.MoveL(JP14, DP14, 0, 0, 100, 100, 100, 20, -1, exaxisPos, 0, 0, offdese);
            robot.LinArcFIRPlanningEnd();
        }

        private void button81_Click(object sender, EventArgs e)
        {
            DescPose p1Desc = new DescPose(186.331, 487.913, 209.850, 149.030, 0.688, -114.347);
            JointPos p1Joint = new JointPos(-127.876, -75.341, 115.417, -122.741, -59.820, 74.300);

            DescPose p2Desc = new DescPose(69.721, 535.073, 202.882, -144.406, -14.775, -89.012);
            JointPos p2Joint = new JointPos(-101.780, -69.828, 110.917, -125.740, -127.841, 74.300);

            DescPose p3Desc = new DescPose(146.861, 578.426, 205.598, 175.997, -36.178, -93.437);
            JointPos p3Joint = new JointPos(-112.851, -60.191, 86.566, -80.676, -97.463, 74.300);

            DescPose p4Desc = new DescPose(136.284, 509.876, 225.613, 178.987, 1.372, -100.696);
            JointPos p4Joint = new JointPos(-116.397, -76.281, 113.845, -128.611, -88.654, 74.299);

            DescPose p5Desc = new DescPose(138.395, 505.972, 298.016, 179.134, 2.147, -101.110);
            JointPos p5Joint = new JointPos(-116.814, -82.333, 109.162, -118.662, -88.585, 74.302);

            DescPose p6Desc = new DescPose(105.553, 454.325, 232.017, -179.426, 0.444, -99.952);
            JointPos p6Joint = new JointPos(-115.649, -84.367, 122.447, -128.663, -90.432, 74.303);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 100, 0, 0, 0);

            //robot.GetForwardKin(p1Joint, ref p1Desc);
            //robot.GetForwardKin(p2Joint, ref p2Desc);
            //robot.GetForwardKin(p3Joint, ref p3Desc);
            //robot.GetForwardKin(p4Joint, ref p4Desc);
            //robot.GetForwardKin(p5Joint, ref p5Desc);
            //robot.GetForwardKin(p6Joint, ref p6Desc);

            robot.MoveJ(p1Joint, p1Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetTcp4RefPoint(1);
            robot.MoveJ(p2Joint, p2Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetTcp4RefPoint(2);
            robot.MoveJ(p3Joint, p3Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetTcp4RefPoint(3);
            robot.MoveJ(p4Joint, p4Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.SetTcp4RefPoint(4);

            DescPose coordRtn = new DescPose(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            int rtn = robot.ComputeTcp4(ref coordRtn);
            Console.WriteLine($"4 Point ComputeTool      {rtn} coord is {coordRtn.tran.x} ,{coordRtn.tran.y} ,{coordRtn.tran.z} ,{coordRtn.rpy.rx} ,{coordRtn.rpy.ry} ,{coordRtn.rpy.rz} ");

            robot.SetToolCoord(1, coordRtn, 0, 0, 1, 0);

            robot.GetForwardKin(p1Joint, ref p1Desc);
            robot.GetForwardKin(p2Joint, ref p2Desc);
            robot.GetForwardKin(p3Joint, ref p3Desc);

            robot.SetFocusCalibPoint(1, p1Desc);
            robot.SetFocusCalibPoint(2, p2Desc);
            robot.SetFocusCalibPoint(3, p3Desc);

            DescTran resultPos = new DescTran(0.0, 0.0, 0.0);
            double accuracy = 0.0;
            rtn = robot.ComputeFocusCalib(3, ref resultPos, ref accuracy);
            Console.WriteLine($"ComputeFocusCalib coord is  {rtn},{resultPos.x} ,{resultPos.y}, {resultPos.z}, accuracy is {accuracy} ");
            rtn = robot.SetFocusPosition(resultPos);

            robot.GetForwardKin(p5Joint, ref p5Desc);
            robot.GetForwardKin(p6Joint, ref p6Desc);

            robot.MoveL(p5Joint, p5Desc, 1, 0, 10, 100, 100, -1, 0, exaxisPos, 0, 1, offdese);
            robot.MoveL(p6Joint, p6Desc, 1, 0, 10, 100, 100, -1, 0, exaxisPos, 0, 1, offdese);

            robot.FocusStart(50, 19, 710, 90, 0);
            robot.MoveL(p5Joint, p5Desc, 1, 0, 10, 100, 100, -1, 0, exaxisPos, 0, 1, offdese);
            robot.MoveL(p6Joint, p6Desc, 1, 0, 10, 100, 100, -1, 0, exaxisPos, 0, 1, offdese);
            robot.FocusEnd();
        }

        private void button82_Click(object sender, EventArgs e)
        {
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();

            int rtn;
            robot.SetSysServoBootMode();
            //robot.RobotEnable(0);
            Thread.Sleep(200);
            // rtn = robot.SetCtrlFirmwareUpgrade(1, "D://zUP/FR_CTRL_PRIMCU_FV201010_MAIN_U4_T01_20240529.bin");
            //rtn = robot.SetCtrlFirmwareUpgrade(1, "D://zUP/FR_CTRL_PRIMCU_FV201011_MAIN_U4_T01_20250208.bin");


            //Console.WriteLine($"robot SetCtrlFirmwareUpgrade rtn is{rtn}");
            //rtn = robot.SetEndFirmwareUpgrade(1, "D://zUP/FR_END_FV201008_MAIN_U01_T01_20250416.bin");
            //Console.WriteLine($"robot SetEndFirmwareUpgrade rtn is {rtn}");
            //rtn = robot.SetJointFirmwareUpgrade(1, "D://zUP/FR_SERVO_FV502211_MAIN_U7_T07_20250217.bin");
            //Console.WriteLine($"robot SetJointFirmwareUpgrade rtn is{rtn}");


            rtn = robot.SetCtrlFirmwareUpgrade(2, "D://zUP/2025_07_09_FAIR_Cobot_Axle_Asix_V2.4/FAIR_Cobot_Cbd_Asix_V2.0.bin");


            Console.WriteLine($"robot SetCtrlFirmwareUpgrade rtn is{rtn}");
            rtn = robot.SetEndFirmwareUpgrade(2, "D://zUP/2025_07_09_FAIR_Cobot_Axle_Asix_V2.4/FAIR_Cobot_Axle_Asix_V2.4.bin");
            Console.WriteLine($"robot SetEndFirmwareUpgrade rtn is {rtn}");

            //rtn = robot.JointAllParamUpgrade("D://zUP/11/jointallparameters.db");
            //Console.WriteLine($"robot JointAllParamUpgrade rtn is{rtn}");

            robot.CloseRPC();

        }

        private void button83_Click(object sender, EventArgs e)
        {
            robot.RobotEnable(0);
            Thread.Sleep(200);
            int rtn = robot.JointAllParamUpgrade("D://zUP/upgrade/jointallparameters.db");
            Console.WriteLine($"robot JointAllParamUpgrade rtn is{rtn}");

            //rtn = robot.SetCtrlFirmwareUpgrade(2, "D://zUP/upgrade/FAIR_Cobot_Cbd_Asix_V2.0.bin");
            //Console.WriteLine($"robot SetCtrlFirmwareUpgrade rtn is{rtn}");

            //rtn = robot.SetEndFirmwareUpgrade(2, "D://zUP/upgrade/FAIR_Cobot_Axle_Asix_V2.4.bin");
            //Console.WriteLine($"robot SetEndFirmwareUpgrade rtn is {rtn}");

            robot.SetSysServoBootMode();
            rtn = robot.SetCtrlFirmwareUpgrade(1, "D://zUP/upgrade/FR_CTRL_PRIMCU_FV201013_MAIN_U4_T01_20260424.bin");
            Console.WriteLine($"robot SetCtrlFirmwareUpgrade rtn is{rtn}");

            rtn = robot.SetEndFirmwareUpgrade(1, "D://zUP/upgrade/FR_END_FV201013_MAIN_U1_T01_20260407.bin");
            Console.WriteLine($"robot SetEndFirmwareUpgrade rtn is {rtn}");

            rtn = robot.SetJointFirmwareUpgrade(1, "D://zUP/upgrade/FR_SERVO_FV504316_MAIN_U7_T07_20250715.bin");
            Console.WriteLine($"robot SetJointFirmwareUpgrade rtn is{rtn}");

        }

        private void button84_Click(object sender, EventArgs e)
        {
            int rtn;
            JointPos joint_pos1 = new JointPos(-68.732, -99.773, -77.729, -77.167, 100.772, -13.317);
            JointPos joint_pos2 = new JointPos(-101.678, -102.823, -77.512, -77.185, 88.388, -13.317);
            JointPos joint_pos3 = new JointPos(-129.905, -99.715, -71.965, -77.209, 81.678, -13.317);
            DescPose desc_pos1 = new DescPose(103.887, -434.739, 244.938, -162.495, 6.575, -142.948);
            DescPose desc_pos2 = new DescPose(-196.883, -418.054, 218.942, -168.196, -4.388, -178.991);
            DescPose desc_pos3 = new DescPose(-396.665, -265.695, 284.380, -160.913, -12.378, 149.770);

            ExaxisPos epos1 = new ExaxisPos(0.000, 6.996, 0.000, 0.000);
            ExaxisPos epos2 = new ExaxisPos(0.000, 20.987, 0.000, 0.000);
            ExaxisPos epos3 = new ExaxisPos(-0.000, 30.982, 0.000, 0.000);

            DescPose offset_pos = new DescPose(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);


            rtn = robot.AccSmoothStart(false);
            Console.WriteLine($"AccSmoothStart rtn is {rtn}");
            Thread.Sleep(1000);
            rtn = robot.ExtAxisSyncMoveL(joint_pos1, desc_pos1, 1, 0, 100, 100, 100, 100, epos1, 0, offset_pos);
            Console.WriteLine($"ExtAxisSyncMoveL 1 rtn is  {rtn}");
            rtn = robot.ExtAxisSyncMoveL(joint_pos2, desc_pos2, 1, 0, 100, 100, 100, 200, epos2, 0, offset_pos);
            Console.WriteLine($"ExtAxisSyncMoveL 2 rtn is {rtn}");
            rtn = robot.ExtAxisSyncMoveL(joint_pos3, desc_pos3, 1, 0, 100, 100, 100, 300, epos3, 0, offset_pos);
            Console.WriteLine($"ExtAxisSyncMoveL 3 rtn is  {rtn}");
            Thread.Sleep(8000);
            rtn = robot.AccSmoothEnd(false);
            Console.WriteLine($"AccSmoothEnd rtn is %d\n", rtn);


            return;
        }

        private void button85_Click(object sender, EventArgs e)
        {


            robot.RobotEnable(0);
            Thread.Sleep(200);
            int rtn = robot.JointAllParamUpgrade("D://zUP/standardQX/jointallparametersFR56.0.db");
            Console.WriteLine($"robot JointAllParamUpgrade rtn is{rtn}");

            rtn = robot.SetCtrlFirmwareUpgrade(2, "D://zUP/upgrade/FAIR_Cobot_Cbd_Asix_V2.0.bin");
            Console.WriteLine($"robot SetCtrlFirmwareUpgrade rtn is{rtn}");


            rtn = robot.SetEndFirmwareUpgrade(2, "D://zUP/upgrade/FAIR_Cobot_Axle_Asix_V2.4.bin");
            Console.WriteLine($"robot SetEndFirmwareUpgrade rtn is {rtn}");

            robot.SetSysServoBootMode();
            rtn = robot.SetCtrlFirmwareUpgrade(1, "D://zUP/standardQX/FR_CTRL_PRIMCU_FV201010_MAIN_U4_T01_20240529.bin");
            Console.WriteLine($"robot SetCtrlFirmwareUpgrade rtn is{rtn}");

            rtn = robot.SetEndFirmwareUpgrade(1, "D://zUP/standardQX/FR_END_FV201010_MAIN_U01_T01_20250522.bin");
            Console.WriteLine($"robot SetEndFirmwareUpgrade rtn is {rtn}");

            rtn = robot.SetJointFirmwareUpgrade(1, "D://zUP/standardQX/FR_SERVO_FV502211_MAIN_U7_T07_20250217.bin");
            Console.WriteLine($"robot SetJointFirmwareUpgrade rtn is{rtn}");

        }

        private void button86_Click(object sender, EventArgs e)
        {

            DescTran directionPoint = new DescTran(0, 0, 0);


            int rtn = robot.LaserTrackingSearchStart(2, directionPoint, 30, 100, 10000, 4);
            Console.WriteLine($"LaserTrackingSearchStart rtn is {rtn}");


            robot.LaserTrackingSearchStop();

            int coordID = 4;
            DescPose desc = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos joint = new JointPos(0, 0, 0, 0, 0, 0);
            ExaxisPos exaxis = new ExaxisPos(0, 0, 0, 0);


            rtn = robot.LaserRecordPoint(coordID, ref desc, ref joint, ref exaxis);
            Console.WriteLine($"rtn is {rtn}");


            Console.WriteLine($"desc_pos:{desc.tran.x},{desc.tran.y},{desc.tran.z},{desc.rpy.rx},{desc.rpy.ry},{desc.rpy.rz}");
            Console.WriteLine($"joint_pos:{joint.jPos[0]},{joint.jPos[1]},{joint.jPos[2]},{joint.jPos[3]},{joint.jPos[4]},{joint.jPos[5]}");
            Console.WriteLine($"exaxis pos is {exaxis.ePos[0]} {exaxis.ePos[1]} {exaxis.ePos[2]} {exaxis.ePos[3]}");


            DescPose off = new DescPose(0, 0, 0, 0, 0, 0);
            robot.MoveJ(joint, desc, 3, 0, 100, 100, 50, exaxis, -1, 0, off);
        }

        private void button87_Click(object sender, EventArgs e)
        {
            // Upload trajectory file
            int rtn = robot.TrajectoryJUpLoad(@"D:\zUP\horse.txt");
            Console.WriteLine($"Upload TrajectoryJ A {rtn}");

            string trajFileName = "/fruser/traj/horse.txt";
            rtn = robot.LoadTrajectoryLA(trajFileName, 2, 0, 0, 1, 40, 100, 100, 1);
            Console.WriteLine($"LoadTrajectoryLA {trajFileName}, rtn is: {rtn}");

            DescPose trajStartPose = new DescPose();
            rtn = robot.GetTrajectoryStartPose(trajFileName, ref trajStartPose);
            Console.WriteLine($"GetTrajectoryStartPose is: {rtn}");
            Console.WriteLine($"desc_pos: {trajStartPose.tran.x},{trajStartPose.tran.y},{trajStartPose.tran.z},{trajStartPose.rpy.rx},{trajStartPose.rpy.ry},{trajStartPose.rpy.rz}");

            Thread.Sleep(1000);

            robot.SetSpeed(50);
            robot.MoveCart(trajStartPose, 0, 0, 100, 100, 100, -1, -1);

            rtn = robot.MoveTrajectoryLA();
            Console.WriteLine($"MoveTrajectoryLA rtn is: {rtn}");
        }

        private void button88_Click(object sender, EventArgs e)
        {
            JointPos j1 = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
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

            int blendMode = 0;
            byte flag = 0;
            float oacc = 0.0f;
            byte search = 0;
            int config = -1;
            robot.SetSpeed(20);
            int rtn;
            rtn = robot.MoveJ(j1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            Console.WriteLine($"MoveJ errcode:{rtn}");

            rtn = robot.MoveL(desc_pos2, tool, user, vel, acc, ovl, blendR, blendMode, epos, search, flag, offset_pos, config, 0);
            Console.WriteLine($"MoveL errcode:{rtn}");

            rtn = robot.MoveC(desc_pos3, tool, user, vel, acc, epos, flag, offset_pos, desc_pos4, tool, user, vel, acc, epos, flag, offset_pos, ovl, blendR, config);
            Console.WriteLine($"MoveC errcode:{rtn}");

            rtn = robot.Circle(desc_pos3, tool, user, vel, acc, epos, desc_pos1, tool, user, vel, acc, epos, ovl, flag, offset_pos, oacc, blendR, config, 0);
            Console.WriteLine($"Circle errcode:{rtn}");
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

            //robot.SetSpeed(5);

            int err = -1;
            err = robot.MoveJ(j1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            Console.WriteLine($"movej errcode:  {err}");

            robot.SplineStart();
            robot.SplinePTP(j1, tool, user, vel, acc, ovl);
            robot.SplinePTP(j2, tool, user, vel, acc, ovl);
            robot.SplinePTP(j3, tool, user, vel, acc, ovl);
            robot.SplinePTP(j4, tool, user, vel, acc, ovl);
            robot.SplineEnd();
        }

        private void button89_Click(object sender, EventArgs e)
        {
            while (true)
            {
                JointPos j1 = new JointPos(-44.185, -95.599, 102.888, -100.999, -90.04, -54.095);
                JointPos j2 = new JointPos(39.128, -95.532, 102.739, -101.114, -90.038, -54.095);
                JointPos j3 = new JointPos(-29.777, -84.536, 109.275, -114.075, -86.655, 74.257);
                JointPos j4 = new JointPos(-31.154, -95.317, 94.276, -88.079, -89.740, 74.256);
                DescPose desc_pos1 = new DescPose(-399.733, 246.791, 430.238, -177.855, -3.027, 99.853);
                DescPose desc_pos2 = new DescPose(-292.353, -368.834, 431.221, -177.737, -3.186, -176.836);
                DescPose desc_pos3 = new DescPose(-599.313, -314.327, 365.756, 179.445, 0.718, 163.133);
                DescPose desc_pos4 = new DescPose(-430.217, -83.687, 370.316, 177.476, 3.436, 141.619);
                DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
                ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
                DescPose desc_pos = new DescPose(-443.453, 453.469, 197.678, -179.228, -0.561, 89.187);

                int tool = 0;
                int user = 0;
                float vel = 100.0f;
                float acc = 100.0f;
                float ovl = 100.0f;
                float blendT = 0.0f;

                byte flag = 0;
                //robot.SetSpeed(20);
                int rtn;
                rtn = robot.MoveJ(j2, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
                Console.WriteLine($"MoveJ errcode:{rtn}");
                //rtn = robot.MoveL(desc_pos1, tool, user, vel, acc, ovl, blendR, blendMode, epos, search, flag, offset_pos, config);
                //Console.WriteLine($"MoveL errcode:{rtn}");
                //rtn = robot.MoveL(desc_pos, tool, user, vel, acc, ovl, blendR, blendMode, epos, search, flag, offset_pos, config);
                //rtn = robot.MoveC(desc_pos1, tool, user, vel, acc, epos, flag, offset_pos, desc_pos2, tool, user, vel, acc, epos, flag, offset_pos, ovl, blendR, config);
                //Console.WriteLine($"MoveC errcode:{rtn}");
                //rtn = robot.Circle(desc_pos3, tool, user, vel, acc, epos, desc_pos4, tool, user, vel, acc, epos, ovl, flag, offset_pos, oacc, blendR, config);
                //Console.WriteLine($"Circle errcode:{rtn}");
                //btnNewSpline_Click(sender, e);  // 使用相同的参数调用
                //btnSplineMove_Click(sender, e);
                //btnDescSpiral_Click(sender, e);
            }

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

            //robot.SetSpeed(5);

            int err = -1;
            err = robot.MoveJ(j1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            Console.WriteLine($"movej errcode:  {err}");

            robot.NewSplineStart(1, 2000);
            robot.NewSplinePoint(desc_pos1, tool, user, vel, acc, ovl, -1, 0, -1);
            robot.NewSplinePoint(desc_pos2, tool, user, vel, acc, ovl, -1, 0, -1);
            robot.NewSplinePoint(desc_pos3, tool, user, vel, acc, ovl, -1, 0, -1);
            robot.NewSplinePoint(desc_pos4, tool, user, vel, acc, ovl, -1, 0, -1);
            robot.NewSplinePoint(desc_pos5, tool, user, vel, acc, ovl, -1, 0, -1);
            robot.NewSplineEnd();
        }

        private void button90_Click(object sender, EventArgs e)
        {
            JointPos j1 = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
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
            float blendR = 0.0f;

            byte flag = 0;
            int config = -1;
            //robot.SetSpeed(20);
            int rtn;
            //rtn = robot.MoveJ(j1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            //Console.WriteLine($"MoveJ errcode:{rtn}");

            //rtn = robot.MoveL(desc_pos2, tool, user, vel, acc, ovl, blendR, blendMode, epos, search, flag, offset_pos, config);
            //Console.WriteLine($"MoveL errcode:{rtn}");

            rtn = robot.MoveC(desc_pos3, tool, user, vel, acc, epos, flag, offset_pos, desc_pos4, tool, user, vel, acc, epos, flag, offset_pos, ovl, blendR, config, 0);
            Console.WriteLine($"MoveC errcode:{rtn}");

            //rtn = robot.Circle(desc_pos3, tool, user, vel, acc, epos, desc_pos1, tool, user, vel, acc, epos, ovl, flag, offset_pos, oacc, blendR, config);
            //Console.WriteLine($"Circle errcode:{rtn}");
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
            sp.circle_num = 1;
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

            //robot.SetSpeed(60);

            rtn = robot.MoveJ(j, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos1);
            Console.WriteLine($"MoveJ errcode:{rtn}");

          //  rtn = robot.NewSpiral(desc_pos, tool, user, vel, acc, epos, ovl, flag, offset_pos2, sp, -1);
            Console.WriteLine($"NewSpiral errcode:{rtn}");
        }

        private void button91_Click(object sender, EventArgs e)
        {

            JointPos joint_safe = new JointPos(33.513, -89.540, -19.754, -135.044, 64.077, 107.990);
            JointPos joint_pos1 = new JointPos(60.164, -104.046, -20.299, -157.828, 53.871, 108.125);
            JointPos joint_pos2 = new JointPos(58.054, -107.816, -15.798, -153.559, 49.501, 108.121);
            JointPos joint_pos3 = new JointPos(55.266, -89.767, -46.349, -128.985, 45.001, 108.13);

            DescPose desc_safe = new DescPose(423.659, -51.518, 366.413, -163.442, 32.248, -165.661);
            DescPose desc_pos1 = new DescPose(409.950, 35.714, 272.466, -142.158, -1.209, -134.392);
            DescPose desc_pos2 = new DescPose(456.062, 47.663, 291.916, -139.201, 4.688, -135.673);
            DescPose desc_pos3 = new DescPose(485.838, 25.316, 313.259, -137.616, 17.480, -138.072);

            ExaxisPos eposSafe = new ExaxisPos(35.00, 25.00, 0.000, 0.000);
            ExaxisPos epos1 = new ExaxisPos(35.00, 25.00, 0.000, 0.000);
            ExaxisPos epos2 = new ExaxisPos(35.00, -25.000, 0.000, 0.000);
            ExaxisPos epos3 = new ExaxisPos(35.00, -60.000, 0.000, 0.000);
            ExaxisPos epos4 = new ExaxisPos(35.00, 0.000, 0.000, 0.000);
            ExaxisPos epos5 = new ExaxisPos(35.00, 0.000, 0.000, 0.000);

            DescPose offset_pos = new DescPose(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            int tool = 1, user = 0, vel = 100, acc = 100, ovl = 100, blendT = -1;
            int rtn = 0;

            //moveJ
            robot.MoveJ(joint_safe, tool, user, vel, acc, ovl, eposSafe, blendT, 0, offset_pos);
            robot.ExtAxisMove(eposSafe, 100, -1);
            rtn = robot.ExtAxisSyncMoveJ(joint_pos1, tool, user, vel, acc, ovl, epos1, blendT, 0, offset_pos);
            rtn = robot.ExtAxisSyncMoveJ(joint_pos3, tool, user, vel, acc, ovl, epos2, blendT, 0, offset_pos);
            Console.WriteLine("ExtAxisSyncMoveJ rtn is: " + rtn);

            //moveL
            robot.MoveJ(joint_safe, tool, user, vel, acc, ovl, eposSafe, blendT, 0, offset_pos);
            robot.ExtAxisMove(eposSafe, 100, -1);
            rtn = robot.ExtAxisSyncMoveJ(joint_pos1, tool, user, vel, acc, ovl, epos1, blendT, 0, offset_pos);
            rtn = robot.ExtAxisSyncMoveL(desc_pos3, tool, user, vel, acc, ovl, -1, epos3, 0, offset_pos, -1);
            Console.WriteLine("ExtAxisSyncMoveL rtn is: " + rtn);

            //moveC
            robot.MoveJ(joint_safe, tool, user, vel, acc, ovl, eposSafe, blendT, 0, offset_pos);
            robot.ExtAxisMove(eposSafe, 100, -1);
            rtn = robot.ExtAxisSyncMoveJ(joint_pos1, tool, user, vel, acc, ovl, epos1, blendT, 0, offset_pos);
            rtn = robot.ExtAxisSyncMoveC(desc_pos2, tool, user, vel, acc, epos2, 0, offset_pos,
                                        desc_pos3, tool, user, vel, acc, epos3, 0, offset_pos, ovl, -1, -1);
            Console.WriteLine("ExtAxisSyncMoveC rtn is: " + rtn);

        }

        private void button92_Click(object sender, EventArgs e)
        {
            JointPos j1 = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
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
            float blendR = 0.0f;

            byte flag = 0;
            float oacc = 0.0f;
            int config = -1;
            robot.SetSpeed(20);
            int rtn;
            //rtn = robot.MoveJ(j1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            //Console.WriteLine($"MoveJ errcode:{rtn}");

            //rtn = robot.MoveL(desc_pos2, tool, user, vel, acc, ovl, blendR, blendMode, epos, search, flag, offset_pos, config);
            //Console.WriteLine($"MoveL errcode:{rtn}");

            //rtn = robot.MoveC(desc_pos3, tool, user, vel, acc, epos, flag, offset_pos, desc_pos4, tool, user, vel, acc, epos, flag, offset_pos, ovl, blendR, config);
            //Console.WriteLine($"MoveC errcode:{rtn}");

            rtn = robot.Circle(desc_pos3, tool, user, vel, acc, epos, desc_pos1, tool, user, vel, acc, epos, ovl, flag, offset_pos, oacc, blendR, config, 0);
            Console.WriteLine($"Circle errcode:{rtn}");
        }

        private void button93_Click(object sender, EventArgs e)
        {
            int num = 100;
            while (num > 0)
            {
                JointPos j1 = new JointPos(-44.185, -95.599, 102.888, -100.999, -90.04, -54.095);
                JointPos j2 = new JointPos(39.128, -95.532, 102.739, -101.114, -90.038, -54.095);
                JointPos j3 = new JointPos(-29.777, -84.536, 109.275, -114.075, -86.655, 74.257);
                JointPos j4 = new JointPos(-31.154, -95.317, 94.276, -88.079, -89.740, 74.256);
                DescPose desc_pos1 = new DescPose(-399.733, 246.791, 430.238, -177.855, -3.027, 99.853);
                DescPose desc_pos2 = new DescPose(-292.353, -368.834, 431.221, -177.737, -3.186, -176.836);
                DescPose desc_pos3 = new DescPose(-599.313, -314.327, 365.756, 179.445, 0.718, 163.133);
                DescPose desc_pos4 = new DescPose(-430.217, -83.687, 370.316, 177.476, 3.436, 141.619);
                DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
                ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
                DescPose desc_pos = new DescPose(-443.453, 453.469, 197.678, -179.228, -0.561, 89.187);
                int tool = 0;
                int user = 0;
                float vel = 100.0f;
                float acc = 100.0f;
                float ovl = 100.0f;
                float blendT = 0.0f;
                byte flag = 0;
                //robot.SetSpeed(20);
                int rtn;
                rtn = robot.MoveJ(j2, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
                //Console.WriteLine($"MoveJ errcode:{rtn}");
                //rtn = robot.MoveL(desc_pos1, tool, user, vel, acc, ovl, blendR, blendMode, epos, search, flag, offset_pos, config);
                //Console.WriteLine($"MoveL errcode:{rtn}");
                //rtn = robot.MoveL(desc_pos, tool, user, vel, acc, ovl, blendR, blendMode, epos, search, flag, offset_pos, config);
                //rtn = robot.MoveC(desc_pos1, tool, user, vel, acc, epos, flag, offset_pos, desc_pos2, tool, user, vel, acc, epos, flag, offset_pos, ovl, blendR, config);
                //Console.WriteLine($"MoveC errcode:{rtn}");
                //rtn = robot.Circle(desc_pos3, tool, user, vel, acc, epos, desc_pos4, tool, user, vel, acc, epos, ovl, flag, offset_pos, oacc, blendR, config);
                //Console.WriteLine($"Circle errcode:{rtn}");
                //btnNewSpline_Click(sender, e);  // 使用相同的参数调用
                //btnSplineMove_Click(sender, e);
                //btnDescSpiral_Click(sender, e);
                num--;
            }
            Console.WriteLine("稳定性测试完成");
        }

        private void button94_Click(object sender, EventArgs e)
        {

            JointPos joint_pos1 = new JointPos(-22.016, -49.217, 124.714, -161.100, -85.108, -0.333);
            JointPos joint_pos2 = new JointPos(-21.083, -46.613, 110.079, -147.796, -80.757, -0.330);
            JointPos joint_pos3 = new JointPos(-25.572, -60.090, 135.397, -163.889, -82.489, -0.345);


            DescPose desc_pos1 = new DescPose(2.637, -0.001, 30.673, 178.786, -4.134, 68.326);
            DescPose desc_pos2 = new DescPose(213.812, -1.440, 47.311, 177.410, 0.166, 68.946);
            DescPose desc_pos3 = new DescPose(444.342, -12.723, 82.470, -177.701, -1.325, 65.151);


            ExaxisPos epos1 = new ExaxisPos(0.001, 0.000, 0.000, 0.000);
            ExaxisPos epos2 = new ExaxisPos(299.977, 0.000, 0.000, 0.000);
            ExaxisPos epos3 = new ExaxisPos(399.969, 0.000, 0.000, 0.000);


            DescPose offset_pos = new DescPose(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);

            int rtn = robot.SetExAxisRobotPlan(0);
            Console.WriteLine($"SetExAxisRobotPlan rtn is {rtn}");


            Thread.Sleep(1000);

            rtn = robot.ExtAxisSyncMoveL(joint_pos1, desc_pos1, 1, 0, 100, 100, 100, -1, epos1, 0, offset_pos);
            Console.WriteLine($"ExtAxisSyncMoveL 1 rtn is {rtn}");

            rtn = robot.ExtAxisSyncMoveL(joint_pos2, desc_pos2, 1, 0, 100, 100, 100, -1, epos2, 0, offset_pos);
            Console.WriteLine($"ExtAxisSyncMoveL 2 rtn is {rtn}");

            rtn = robot.ExtAxisSyncMoveL(joint_pos3, desc_pos3, 1, 0, 100, 100, 100, -1, epos3, 0, offset_pos);
            Console.WriteLine($"ExtAxisSyncMoveL 3 rtn is {rtn}");


            Thread.Sleep(8000);
        }

        private void button95_Click(object sender, EventArgs e)
        {
            int[] ctrl = new int[8];

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"------------------第 {i + 1} 次测试------------------");
                // Control sucker in broadcast mode with maximum adsorption capacity
                ctrl[0] = 1;
                robot.SetSuckerCtrl(0, 1, ctrl);
                Console.WriteLine("sucker broadcast start");
                Thread.Sleep(3000);
                ctrl[0] = 3;
                robot.SetSuckerCtrl(0, 1, ctrl);
                Console.WriteLine("sucker broadcast stop");
                Thread.Sleep(3000);

                // Unicast mode test to control sucker with set value
                ctrl[0] = 2;
                robot.SetSuckerCtrl(1, 1, ctrl);
                robot.SetSuckerCtrl(12, 1, ctrl);
                Console.WriteLine("sucker unicast start");
                Thread.Sleep(2000);
                ctrl[0] = 3;
                robot.SetSuckerCtrl(1, 1, ctrl);
                robot.SetSuckerCtrl(12, 1, ctrl);
                Console.WriteLine("sucker unicast stop");
                Thread.Sleep(2000);
            }
        }

        private void TestSucker(Robot robot)
        {

            int[] ctrl = new int[20];
            int state = 0;
            int pressValue = 0;
            int error = 0;


            // Upload and load open protocol file
            robot.OpenLuaUpload(@"C:\项目\外设SDK\CtrlDev_sucker.lua");
            Thread.Sleep(2000);
            robot.UnloadCtrlOpenLUA(1);
            robot.LoadCtrlOpenLUA(1);
            Thread.Sleep(1000);

            // Control sucker in broadcast mode with maximum adsorption capacity
            ctrl[0] = 1;
            robot.SetSuckerCtrl(0, 1, ctrl);

            // Monitor states of sucker 1 and sucker 12 in a loop
            for (int i = 0; i < 100; i++)
            {
                robot.GetSuckerState(1, ref state, ref pressValue, ref error);
                Console.WriteLine($"sucker1 state is {state}, pressValue is {pressValue}, error num is {error}");
                robot.GetSuckerState(12, ref state, ref pressValue, ref error);
                Console.WriteLine($"sucker12 state is {state}, pressValue is {pressValue}, error num is {error}");
                Thread.Sleep(100);
            }
            // Wait for sucker 1 to reach adsorbed state, timeout 100ms
            int ret = robot.WaitSuckerState(1, 1, 100);
            Console.WriteLine($"WaitSuckerState result is {ret}");

            // Unicast mode to turn off sucker 1 and 12
            ctrl[0] = 3;
            robot.SetSuckerCtrl(1, 1, ctrl);
            robot.SetSuckerCtrl(12, 1, ctrl);

            robot.CloseRPC();
        }

        public void TestFieldBusBoard(Robot robot)
        {

        }

        public void TestSetSuckerCtrl(Robot robot)
        {
            int rtn = 0;
            int[] ctrl = new int[8];

            if (rtn != 0)
            {
                return;
            }


            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"------------------第 {i + 1} 次测试------------------");
                // Control sucker in broadcast mode with maximum adsorption capacity
                ctrl[0] = 1;
                robot.SetSuckerCtrl(0, 1, ctrl);
                Console.WriteLine("sucker broadcast start");
                Thread.Sleep(3000);
                ctrl[0] = 3;
                robot.SetSuckerCtrl(0, 1, ctrl);
                Console.WriteLine("sucker broadcast stop");
                Thread.Sleep(3000);

                // Unicast mode test to control sucker with set value
                ctrl[0] = 2;
                robot.SetSuckerCtrl(1, 1, ctrl);
                robot.SetSuckerCtrl(12, 1, ctrl);
                Console.WriteLine("sucker unicast start");
                Thread.Sleep(2000);
                ctrl[0] = 3;
                robot.SetSuckerCtrl(1, 1, ctrl);
                robot.SetSuckerCtrl(12, 1, ctrl);
                Console.WriteLine("sucker unicast stop");
                Thread.Sleep(2000);
            }
            robot.CloseRPC();
        }

        private void button96_Click(object sender, EventArgs e)
        {
            int[] ctrl = new int[20];
            int state = 0;
            int pressValue = 0;
            int error = 0;


            // Control sucker in broadcast mode with maximum adsorption capacity
            ctrl[0] = 1;
            robot.SetSuckerCtrl(0, 1, ctrl);

            // Monitor states of sucker 1 and sucker 12 in a loop
            for (int i = 0; i < 100; i++)
            {
                robot.GetSuckerState(1, ref state, ref pressValue, ref error);
                Console.WriteLine($"sucker1 state is {state}, pressValue is {pressValue}, error num is {error}");
                robot.GetSuckerState(12, ref state, ref pressValue, ref error);
                Console.WriteLine($"sucker12 state is {state}, pressValue is {pressValue}, error num is {error}");
                Thread.Sleep(100);
            }

            // Unicast mode to turn off sucker
            ctrl[0] = 3;
            robot.SetSuckerCtrl(0, 1, ctrl);
        }

        private void button97_Click(object sender, EventArgs e)
        {
            int rtn = 0;
            int[] ctrl = new int[20];
            int state = 0;
            int pressValue = 0;
            int error = 0;

            if (rtn != 0)
            {
                return;
            }


            // Control sucker in broadcast mode with maximum adsorption capacity
            ctrl[0] = 1;
            robot.SetSuckerCtrl(0, 1, ctrl);

            // Monitor states of sucker 1 and sucker 12 in a loop
            for (int i = 0; i < 100; i++)
            {
                robot.GetSuckerState(1, ref state, ref pressValue, ref error);
                Console.WriteLine($"sucker1 state is {state}");
                robot.GetSuckerState(12, ref state, ref pressValue, ref error);
                Console.WriteLine($"sucker12 state is {state}");
                Thread.Sleep(100);
            }

            // Wait for sucker 1 to reach adsorbed state, timeref 100ms
            int ret = robot.WaitSuckerState(1, 1, 100);
            Console.WriteLine($"WaitSuckerState1 result is {ret}");

            // Wait for sucker 12 to reach adsorbed state, timeref 100ms
            ret = robot.WaitSuckerState(12, 1, 100);
            Console.WriteLine($"WaitSuckerState12 result is {ret}");

            ctrl[0] = 3;
            robot.SetSuckerCtrl(0, 1, ctrl);
        }

        private void button98_Click(object sender, EventArgs e)
        {
            int[] ctrl = new int[20];
            int state = 0;
            int pressValue = 0;
            int error = 0;


            // Upload and load open protocol file
            robot.OpenLuaUpload("E://项目/外设/CtrlDev_sucker.lua");
            Thread.Sleep(2000);
            robot.UnloadCtrlOpenLUA(1);
            robot.LoadCtrlOpenLUA(1);
            Thread.Sleep(1000);

            // Control sucker in broadcast mode with maximum adsorption capacity
            ctrl[0] = 1;
            robot.SetSuckerCtrl(0, 1, ctrl);

            // Monitor states of sucker 1 and sucker 12 in a loop
            for (int i = 0; i < 100; i++)
            {
                robot.GetSuckerState(1, ref state, ref pressValue, ref error);
                Console.WriteLine($"sucker1 state is {state}, pressValue is {pressValue}, error num is {error}");
                robot.GetSuckerState(12, ref state, ref pressValue, ref error);
                Console.WriteLine($"sucker12 state is {state}, pressValue is {pressValue}, error num is {error}");
                Thread.Sleep(100);
            }
            // Wait for sucker 1 to reach adsorbed state, timeout 100ms
            int ret = robot.WaitSuckerState(1, 1, 100);
            Console.WriteLine($"WaitSuckerState result is {ret}");

            // Unicast mode to turn off sucker 1 and 12
            ctrl[0] = 3;
            robot.SetSuckerCtrl(1, 1, ctrl);
            robot.SetSuckerCtrl(12, 1, ctrl);
        }

        private void button99_Click(object sender, EventArgs e)
        {
            int[] ctrl = new int[20];
            int state = 0;
            int pressValue = 0;
            int error = 0;


            // Upload and load open protocol file
            robot.OpenLuaUpload("E://项目/外设SDK/CtrlDev_sucker.lua");
            Thread.Sleep(2000);
            robot.UnloadCtrlOpenLUA(1);
            robot.LoadCtrlOpenLUA(1);
            Thread.Sleep(1000);

            // Control sucker in broadcast mode with maximum adsorption capacity
            ctrl[0] = 1;
            robot.SetSuckerCtrl(0, 1, ctrl);

            // Monitor states of sucker 1 and sucker 12 in a loop
            for (int i = 0; i < 100; i++)
            {
                robot.GetSuckerState(1, ref state, ref pressValue, ref error);
                Console.WriteLine($"sucker1 state is {state}, pressValue is {pressValue}, error num is {error}");
                robot.GetSuckerState(12, ref state, ref pressValue, ref error);
                Console.WriteLine($"sucker12 state is {state}, pressValue is {pressValue}, error num is {error}");
                Thread.Sleep(100);
            }
            // Wait for sucker 1 to reach adsorbed state, timeout 100ms
            int ret = robot.WaitSuckerState(1, 1, 100);
            Console.WriteLine($"WaitSuckerState result is {ret}");

            // Unicast mode to turn off sucker 1 and 12
            ctrl[0] = 3;
            robot.SetSuckerCtrl(1, 1, ctrl);
            robot.SetSuckerCtrl(12, 1, ctrl);


        }

        private void button100_Click(object sender, EventArgs e)
        {

            int[] ctrl = new int[20];
            int state = 0;
            int pressValue = 0;
            int error = 0;
            int rtn = 0;
            if (rtn != 0)
            {
                return;
            }

            //上传并加载开放协议文件
            robot.OpenLuaUpload("E://项目/外设/CtrlDev_sucker.lua");
            Thread.Sleep(2000);
            robot.SetCtrlOpenLUAName(1, "CtrlDev_sucker.lua");
            robot.UnloadCtrlOpenLUA(1);
            robot.LoadCtrlOpenLUA(1);
            Thread.Sleep(2000);

            JointPos j1 = new JointPos(76.558, -81.447, 132.913, -145.499, -92.762, -0.485);
            DescPose desc_pos1 = new DescPose(-2.659, -429.194, 170.829, -175.985, -2.789, 166.848);

            JointPos j2 = new JointPos(76.559, -90.243, 128.285, -132.076, -92.762, -0.485);
            DescPose desc_pos2 = new DescPose(-2.658, -429.198, 241.123, -175.985, -2.789, 166.848);
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

            while (true)
            {
                rtn = robot.MoveJ(j1, desc_pos1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);

                //控制吸盘广播模式下，按照最大能力吸附
                ctrl[0] = 2;
                robot.SetSuckerCtrl(0, 1, ctrl);

                //循环监控1号吸盘和12号吸盘的状态
                for (int i = 0; i < 20; i++)
                {
                    robot.GetSuckerState(1, ref state, ref pressValue, ref error);
                    Console.WriteLine($"sucker1 state is {state}, pressVlaue is {pressValue}, error num is {error}");
                    robot.GetSuckerState(12, ref state, ref pressValue, ref error);
                    Console.WriteLine($"sucker12 state is {state}, pressVlaue is {pressValue}, error num is {error}");
                    Thread.Sleep(100);
                }
                //等待1号吸盘是否为吸附到物体的状态，等待时间100ms
                int ret = robot.WaitSuckerState(1, 1, 100);
                Console.WriteLine($"WaitSuckerState result is {ret}");
                rtn = robot.MoveJ(j2, desc_pos2, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
                if (ret == 0)
                {
                    Console.WriteLine("sucker1 吸附到物体");
                }
                else
                {
                    Console.WriteLine("sucker1 未吸附到物体");
                    continue;
                }

                //单播模式关闭1号和12号吸盘
                ctrl[0] = 3;
                robot.SetSuckerCtrl(1, 1, ctrl);
                robot.SetSuckerCtrl(12, 1, ctrl);

                Thread.Sleep(1000);
            }
        }

        private void button101_Click(object sender, EventArgs e)
        {

            int type = 0, version = 0, connState = 0;
            int[] ctrl = new int[8];
            double[] ctrlAO = new double[8];
            int[] DI = new int[8];
            double[] AI = new double[8];
            //if (rtn != 0)
            //{
            //    return;
            //}
            //// Upload and load open protocol file
            //robot.OpenLuaUpload("E://项目/外设/CtrlDev_field.lua");
            //Thread.Sleep(2000);
            //robot.SetCtrlOpenLUAName(3, "CtrlDev_field.lua");
            //robot.UnloadCtrlOpenLUA(3);
            //robot.LoadCtrlOpenLUA(3);
            //Thread.Sleep(8000);

            // Get protocol type, software version, and connection status with PLC
            robot.GetFieldBusConfig(ref type, ref version, ref connState);
            Console.WriteLine($"type is {type}, version is {version}, connState is {connState}");

            // Write DO0 = 1, DO1 = 0, DO2 = 1
            ctrl[0] = 1;
            ctrl[1] = 1;
            ctrl[2] = 1;
            robot.FieldBusSlaveWriteDO(0, 3, ctrl);

            // Write AO2 = 0x1000
            ctrlAO[0] = 0x1001;
            robot.FieldBusSlaveWriteAO(2, 1, ctrlAO);

            // Monitor DI0~DI3 and AI0~AI2 in a loop
            for (int i = 0; i < 100; i++)
            {
                robot.FieldBusSlaveReadDI(0, 4, ref DI);
                Console.WriteLine($"DI0 is {DI[0]}, DI1 is {DI[1]}, DI2 is {DI[2]}, DI3 is {DI[3]}");
                robot.FieldBusSlaveReadAI(0, 3, ref AI);
                Console.WriteLine($"AI0 is {AI[0]}, AI1 is {AI[1]}, AI2 is {AI[2]}");
                Thread.Sleep(10);
            }

            // Wait for DI0 to become 1, timeout 100ms
            int ret = robot.FieldBusSlaveWaitDI(0, 1, 100);
            Console.WriteLine($"FieldBusSlaveWaitDI result is {ret}");

            // Wait for AI0 to be greater than 400, timeout 100ms
            ret = robot.FieldBusSlaveWaitAI(0, 0, 400.00f, 100);
            Console.WriteLine($"FieldBusSlaveWaitAI result is {ret}");

        }

        private void button102_Click(object sender, EventArgs e)
        {
            int rtn = 0;
            int[] ctrl = new int[8];
            double[] ctrlAO = new double[8];
            int[] DI = new int[8];
            int[] AI = new int[8];

            if (rtn != 0)
            {
                return;
            }

            // Write DO0 = 1, DO1 = 1, DO2 = 0
            ctrl[0] = 1;
            ctrl[1] = 1;
            ctrl[2] = 0;
            ctrl[3] = 1;
            robot.FieldBusSlaveWriteDO(0, 4, ctrl);

            // Write AO2 = 0x1001
            ctrlAO[0] = 0x1001;
            robot.FieldBusSlaveWriteAO(2, 1, ctrlAO);

            robot.CloseRPC();
        }

        private void button103_Click(object sender, EventArgs e)
        {
            int[] ctrl = new int[8];
            int[] ctrlAO = new int[8];
            int[] DI = new int[8];
            double[] AI = new double[8];


            int rtn = robot.RPC("192.168.58.2");
            if (rtn != 0)
            {
                return;
            }

            // Monitor DI0~DI3 and AI0~AI2 in a loop
            for (int i = 0; i < 100; i++)
            {
                robot.FieldBusSlaveReadDI(0, 4, ref DI);
                Console.WriteLine($"DI0 is {DI[0]}, DI1 is {DI[1]}, DI2 is {DI[2]}, DI3 is {DI[3]}");
                robot.FieldBusSlaveReadAI(0, 3, ref AI);
                Console.WriteLine($"AI0 is {AI[0]}, AI1 is {AI[1]}, AI2 is {AI[2]}");
                Thread.Sleep(100);
            }

            // Wait for DI0 to become 1, timeref 100ms
            int ret = robot.FieldBusSlaveWaitDI(0, 1, 100);
            Console.WriteLine($"FieldBusSlaveWaitDI result is {ret}");

            // Wait for AI0 to be greater than 400, timeref 100ms
            ret = robot.FieldBusSlaveWaitAI(0, 0, 400.00f, 100);
            Console.WriteLine($"FieldBusSlaveWaitAI result is {ret}");
        }


        public void TestGetFieldBusConfig(Robot robot)
        {
            int rtn = 0;
            int type = 0, version = 0, connState = 0;
            int[] ctrl = new int[8];
            int[] ctrlAO = new int[8];
            int[] DI = new int[8];
            int[] AI = new int[8];

            if (rtn != 0)
            {
                return;
            }


            // Get protocol type, software version, and connection status with PLC
            robot.GetFieldBusConfig(ref type, ref version, ref connState);
            Console.WriteLine($"type is {type}, version is {version}, connState is {connState}");

            robot.CloseRPC();
        }

        public void TestFieldBusSlaveWriteDOAO(Robot robot)
        {
            int rtn = 0;
            int[] ctrl = new int[8];
            double[] ctrlAO = new double[8];
            int[] DI = new int[8];
            int[] AI = new int[8];

            if (rtn != 0)
            {
                return;
            }


            // Write DO0 = 1, DO1 = 1, DO2 = 0
            ctrl[0] = 1;
            ctrl[1] = 0;
            ctrl[2] = 0;
            ctrl[3] = 0;
            robot.FieldBusSlaveWriteDO(0, 4, ctrl);

            // Write AO2 = 0x1001
            ctrlAO[0] = 0x1011;
            robot.FieldBusSlaveWriteAO(2, 1, ctrlAO);

            robot.CloseRPC();
        }

        public void TestFieldBusSlaveReadDIAI_WaitDIAI(Robot robot)
        {

            int[] ctrl = new int[8];
            int[] ctrlAO = new int[8];
            int[] DI = new int[8];
            double[] AI = new double[8];
            int rtn = 0;
            if (rtn != 0)
            {
                return;
            }

            // Monitor DI0~DI3 and AI0~AI2 in a loop
            for (int i = 0; i < 100; i++)
            {
                robot.FieldBusSlaveReadDI(0, 4, ref DI);
                Console.WriteLine($"DI0 is {DI[0]}, DI1 is {DI[1]}, DI2 is {DI[2]}, DI3 is {DI[3]}");
                robot.FieldBusSlaveReadAI(0, 3, ref AI);
                Console.WriteLine($"AI0 is {AI[0]}, AI1 is {AI[1]}, AI2 is {AI[2]}");
                Thread.Sleep(100);
            }

            // Wait for DI0 to become 1, timeref 100ms
            int ret = robot.FieldBusSlaveWaitDI(0, 1, 100);
            Console.WriteLine($"FieldBusSlaveWaitDI result is {ret}");

            // Wait for AI0 to be greater than 400, timeref 100ms
            ret = robot.FieldBusSlaveWaitAI(0, 0, 400.00f, 100);
            Console.WriteLine($"FieldBusSlaveWaitAI result is {ret}");

            robot.CloseRPC();
        }

        private void TestMovePhy_Click(object sender, EventArgs e)
        {

            JointPos j1 = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            JointPos j2 = new JointPos(-45.615, -106.172, 124.296, -107.151, -91.282, 74.255);
            JointPos j3 = new JointPos(-29.777, -84.536, 109.275, -114.075, -86.655, 74.257);
            JointPos j4 = new JointPos(-31.154, -95.317, 94.276, -88.079, -89.740, 74.256);

            DescPose desc_pos1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose desc_pos2 = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);
            DescPose desc_pos3 = new DescPose(-487.434, 154.362, 308.576, 176.600, 0.268, -14.061);
            DescPose desc_pos4 = new DescPose(-443.165, 147.881, 480.951, 179.511, -0.775, -15.409);
            DescPose desc_pos5 = new DescPose(-385.268, -386.759, 238.349, 179.619, -2.046, 162.332);
            DescPose desc_pos6 = new DescPose(-257.470, -566.986, 241.908, -177.038, -2.886, -176.577);
            DescPose desc_pos7 = new DescPose(-190.925, -390.644, 240.374, 179.089, 0.019, 177.836);

            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);

            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 200.0f;
            float ovl = 100.0f;
            float blendR = -1.0f;
            byte flag = 0;
            byte search = 0;

            robot.SetSpeed(20);
            int rtn;

            rtn = robot.MoveL(desc_pos1, tool, user, vel, acc, ovl, blendR, 0, epos, search, flag, offset_pos, -1, 1);
            Console.WriteLine($"movel errcode:{rtn}");
            Console.WriteLine("movej errcode:" + rtn);

            rtn = robot.MoveC(desc_pos3, tool, user, vel, acc, epos, flag, offset_pos,
                             desc_pos4, tool, user, vel, acc, epos, flag, offset_pos,
                             ovl, blendR, -1, 1);
            Console.WriteLine($"movec errcode:{rtn}");


            rtn = robot.MoveL(desc_pos5, tool, user, vel, acc, ovl, blendR, 0, epos, search, flag, offset_pos, -1, 1);
            Console.WriteLine($"movel errcode:{rtn}");


            rtn = robot.Circle(desc_pos6, tool, user, vel, acc, epos,
                              desc_pos7, tool, user, vel, acc, epos,
                              ovl, flag, offset_pos, 100, -1, -1, 1);
            Console.WriteLine($"circle errcode:{rtn}");
        }

        private void TestDragSwitchDetect_Click(object sender, EventArgs e)
        {
            int maincode = 0;

            int subcode = 0;
            int rtn = robot.SetTorqueDetectionSwitch(1);
            Console.WriteLine("SetTorqueDetectionSwitch rtn : " + rtn);

            rtn = robot.DragTeachSwitch(1);
            Console.WriteLine("DragTeachSwitch in rtn : " + rtn);

            Thread.Sleep(1000);

            rtn = robot.DragTeachSwitch(0);
            Console.WriteLine("DragTeachSwitch out rtn : " + rtn);

            while (true)
            {

                robot.GetRobotErrorCode(ref maincode, ref subcode);
                Console.WriteLine($"robot maincode is {maincode}; subcode is {subcode}");

                Thread.Sleep(1000);
            }
        }




        private void button104_Click(object sender, EventArgs e)
        {
            //TestCoord();
            //TestStationaryTrack();
            //TestWorkPieceTrsf();
            //TestWeaveSpeedAndOffset();
            //TestCoordMain5();
            //RunTrajectoryJ("D://zUP/horse.txt", "/usr/local/etc/controller/lua/traj/horse.txt", 50, 1);
            //TestSplineWeave();
            //TestStable();
            //Test_UINT057_MoveL_ArrayCompatibility();
            //XmlrpcCompatibilityTest();
            //TestInstanceTest();

            // 依次调用所有测试函数
            //TestCtrlOpenLuaOperate();
            //TestUDPAxis();
            //testled();
            //TestSetVelReducePara();
            //TestOriginPointWeave();
            //TestServoJUDP();
            //ServoJTWithSafetyUDP();
            //ServoMITtest();
            //ServoJVtest();
            //GripperDropAlarmTest(robot);
            //RunTrajTest();

            //激光
            //testLaserConfig();
            //testLaserRecordAndReplay();
            //testLasertrack();
            //TestLaserTrackAndExitAxis();
            //LaserSensorRecordandReplay();

            //testTPDmove();
            //testAxleGenCom();
            //RunTrajectoryJ();
            //TestRobotStopOnComDisc();
            //TestRobotUDP();
            //TestIOConfig();
            //TestImpedanceControl1();
            //testGetLaserPoint();
            // testMoveToLaserRecordStart();
            // testMoveToLaserRecordEnd();
            //testLasertrack_xyz();
            // testLasertrack_point();
            //testLaserRecordAndReplay();
            // testLasertrack();
            // TestLaserTrackAndExitAxis();
            //TestImpedanceControl();
            //TestCoordMain();
            // TestCoordMain1();
            // TestCoordMain2();
            //TestCoordMain3();
            //TestCoordMain4();
            //TestKernelOTA();
            // TestLaserRecordAndReplayMoveC();
            // TestLaserTrackMoveC();
            //TestSensitivityCalib();
            //TestServoJ();
            //TestSlavePortErr();
            //TestVelFeedForwardRatio();
            //TestSpiral();
            //TestFTControlWithDamping();
            //ServoJTWithSafety();
            //TestLua();
            //TestIntersectLineMove();
            //TestFTControlWithAdjustCoeff();
            //TestRotInsert();
            //TestMove();
            //TestSegWeld1();
            //TestPhotoelectricSensorTCPCalib();
            //LaserSensorRecordandReplay();
            //Console.WriteLine("=== 机器人SDK连接断开测试 ===\n");

            // 测试1：正常连接测试
            //TestNormalConnection();

            // 测试2：CPU压力测试
            //  TestCPUStressImpactHigh();

            // 测试3：内存压力测试
            //   TestMemoryStressImpact();

            // 测试4：混合压力测试
            // TestMixedStressImpact();

            // 测试5：手动断开连接测试
            // TestManualDisconnection();

            //  TestMemoryStressImpactExtreme();

            //  Console.WriteLine("\n所有测试完成！");
            //TestRotInsert();

            //TestInverseKinExaxis();
            //TestServoCart();

            //TestDOReset();
        }
        public void TestDOReset()
        {
            for (int i = 0; i < 16; i++)
            {
                robot.SetDO(i, 0, 0, 0);
                Thread.Sleep(200);
            }

            int resetFlag = 1;
            int resumeReloadFlag = 1;
            int rtn = robot.SetOutputResetCtlBoxDO(resetFlag, resumeReloadFlag);
            robot.SetOutputResetCtlBoxAO(resetFlag, resumeReloadFlag);
            robot.SetOutputResetAxleDO(resetFlag, resumeReloadFlag);
            robot.SetOutputResetAxleAO(resetFlag, resumeReloadFlag);
            robot.SetOutputResetExtDO(resetFlag, resumeReloadFlag);
            robot.SetOutputResetExtAO(resetFlag, resumeReloadFlag);
            robot.SetOutputResetSmartToolDO(resetFlag, resumeReloadFlag);

            robot.ProgramLoad("test.lua");
            robot.ProgramRun();

            Thread.Sleep(2000);
            robot.PauseMotion();
            Thread.Sleep(2000);
            robot.ResumeMotion();
            Thread.Sleep(2000);
 
        }

        public void TestInverseKinExaxis()
        {
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();


            //DescPose desc = new DescPose(99.957f, -0.002f, 29.994f, -176.569f, -6.757f, -167.462f);
            DescPose desc = new DescPose(199.968, -542.109, 333.659, 90.072, 2.027, 92.026);
            
            ExaxisPos exaxis = new ExaxisPos(100.0f, 0.0f, 0.0f, 0.0f);
            JointPos jointPos = new JointPos(0,0,0,0,0,0);
            DescPose offsetPos = new DescPose(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
            robot.GetRobotRealTimeState(ref pkg);
            int toolnum = pkg.tool;
            int workPcsNum = pkg.user;

            robot.GetInverseKinExaxis(0, desc, exaxis, toolnum, workPcsNum, ref jointPos);
            Console.WriteLine($"GetInverseKinExaxis joint is {jointPos.jPos[0]}, {jointPos.jPos[1]}, {jointPos.jPos[2]}, {jointPos.jPos[3]}, {jointPos.jPos[4]}, {jointPos.jPos[5]}");

            //robot.ExtAxisMove(exaxis, 100, -1);

            robot.MoveJ(jointPos, desc, toolnum, workPcsNum, (float)100.0, (float)100.0, (float)100.0, exaxis, -1, 0, offsetPos);


        }

        public void TestServoCart()
        {
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();

            int rtn;
            DescPose desc_pos_dt = new DescPose(-396, -202, 475, 90, 2, 0);

            ExaxisPos exaxis = new ExaxisPos(0.0f, 0.0f, 0.0f, 0.0f);
            double[] pos_gain = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            int mode = 0;
            float vel = 0.0f;
            float acc = 0.0f;
            float cmdT = 0.001f;
            float filterT = 0.0f;
            float gain = 0.0f;
            int count = 5000;

            robot.SetSpeed(20);

            JointPos j = new JointPos(0, -90, 90, 0, 1, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            robot.MoveJ(j, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);

            robot.GetActualTCPPose(0, ref desc_pos_dt);

            while (count > 0)
            {
                rtn = robot.ServoCart(mode, desc_pos_dt, exaxis, pos_gain, acc, vel, cmdT, filterT, gain);
                Console.WriteLine($"ServoCart rtn is {rtn}");
                count -= 1;
                desc_pos_dt.tran.x += 0.01f;
                //exaxis.ePos[0] += 0.01f;
            }


        }
        public void TestRotInsert()
        {
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
            int rtn;

            float forceInsertion = 5.0f; // Force or torque threshold (0~100), unit N or Nm
            int angleMax = 300; // Maximum rotation angle, unit °
            byte orn = 1; // Force direction, 1-fz, 2-mz
            float angAccmax = 0; // Maximum rotational angular acceleration, unit °/s^2, not used temporarily
            byte status = 1;  // Constant force control enable flag, 0-off, 1-on
            int sensor_num = 11; // Force sensor number
            float[] gain = { 0.0001f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };  // Maximum threshold
            byte adj_sign = 0;  // Adaptive start/stop status, 0-off, 1-on
            byte ILC_sign = 0;  // ILC control start/stop status, 0-stop, 1-training, 2-operational
            float max_dis = 1000.0f;  // Maximum adjustment distance
            float max_ang = 20.0f;  // Maximum adjustment angle
            ForceTorque ft = new ForceTorque();
            int rcs = 0;  // Reference coordinate system, 0-tool coordinate system, 1-base coordinate system
            float angVelRot = 1.0f;  // Rotational angular velocity, unit °/s
            byte rotorn = 1; // Rotation direction, 1-clockwise, 2-counterclockwise
            JointPos j1 = new JointPos(100.968, -108.678, 126.166, -106.630, -93.253, 19.584);
            DescPose desc_p1 = new DescPose(159.473, -316.570, 334.560, -179.718, -3.352, 171.400);
            ExaxisPos epos = new ExaxisPos(0.0f, 0.0f, 0.0f, 0.0f);
            DescPose offset_pos = new DescPose(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

            robot.MoveL(j1, desc_p1, 2, 0, 100.0f, 180.0f, 100.0f, -1.0f, 0, epos, (byte)0, (byte)1, offset_pos);

            byte[] select3 = { 0, 0, 1, 0, 0, 0 };
            ft.fz = -5.0f;
            gain[0] = 0.0001f;
            status = 1;
            robot.FT_Control(status, sensor_num, select3, ft, gain, adj_sign, ILC_sign, max_dis, max_ang, 0, 0, 0);
            rtn = robot.FT_LinInsertion(rcs, 10, 1, 1, 100, 1);
            Console.WriteLine("FT_LinInsertion rtn is " + rtn);
            robot.FT_Control(0, sensor_num, select3, ft, gain, adj_sign, ILC_sign, max_dis, max_ang, 0, 0, 0);

            ft.fz = -30.0f;
            robot.FT_Control(1, sensor_num, select3, ft, gain, adj_sign, ILC_sign, max_dis, max_ang, 0, 0, 0);
            rtn = robot.FT_RotInsertion(rcs, angVelRot, forceInsertion, angleMax, orn, angAccmax, rotorn, 0);
            Console.WriteLine("FT_RotInsertion rtn is " + rtn);
            robot.FT_Control(0, sensor_num, select3, ft, gain, adj_sign, ILC_sign, max_dis, max_ang, 0, 0, 0);

            rtn = robot.FT_LinInsertion(0, 40, 3, 0, 100, 1);
            Console.WriteLine("FT_LinInsertion retract rtn is " + rtn);

            Thread.Sleep(1000);
            robot.GetRobotRealTimeState(ref pkg);
            Console.WriteLine("robot errcode " + pkg.main_code + "  " + pkg.sub_code);
        }
        public void TestCPUStressImpactHigh()
        {
            Console.WriteLine("=== 测试2：CPU压力对连接的影响（高强度版）===");

            // 先测试正常状态
            Console.WriteLine("CPU压力前测试...");
            ROBOT_STATE_PKG statePkg = new ROBOT_STATE_PKG();
            int initialResult = robot.GetRobotRealTimeState(ref statePkg);
            Console.WriteLine($"初始状态：返回 {initialResult}");

            // 创建高强度CPU压力
            Console.WriteLine("\n开始施加高强度CPU压力...");
            CancellationTokenSource cts = new CancellationTokenSource();
            List<Task> cpuTasks = new List<Task>();

            int coreCount = Environment.ProcessorCount;


            // 修改监控任务的启动时机和逻辑
            Task monitoringTask = Task.Run(() =>
            {
                Console.WriteLine("[监控] 监控线程启动");

                PerformanceCounter cpuCounter = null;
                try
                {
                    // 先初始化性能计数器
                    cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    cpuCounter.NextValue(); // 第一次调用需要初始化

                    Console.WriteLine("[监控] 性能计数器初始化完成");

                    while (!cts.Token.IsCancellationRequested)
                    {
                        // 先休眠再读取，确保有足够的采样间隔
                        Thread.Sleep(1000);

                        if (cts.Token.IsCancellationRequested)
                            break;

                        float cpuUsage = cpuCounter.NextValue();
                        Console.WriteLine($"[监控] CPU使用率: {cpuUsage:F1}%");

                        // 再休眠1秒，形成2秒间隔
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[监控] 错误: {ex.GetType().Name}: {ex.Message}");
                }
                finally
                {
                    cpuCounter?.Dispose();
                    Console.WriteLine("[监控] 监控线程结束");
                }
            });

            // 等待监控任务完全启动
            Thread.Sleep(1000);
            Console.WriteLine("监控任务已启动，开始创建压力线程...");
            // 高强度：4倍核心数，每个线程100%占用
            int threadCount = coreCount * 4;

            Console.WriteLine($"系统核心数: {coreCount}");
            Console.WriteLine($"创建压力线程数: {threadCount}");

            for (int i = 0; i < threadCount; i++)
            {
                cpuTasks.Add(Task.Run(() =>
                {
                    int threadId = i;
                    Random rnd = new Random(threadId + DateTime.Now.Millisecond);

                    // 记录线程开始时间
                    Stopwatch threadSw = Stopwatch.StartNew();

                    while (!cts.Token.IsCancellationRequested && threadSw.ElapsedMilliseconds < 35000) // 比测试时间长一点
                    {
                        // 高强度计算循环 - 尽量减少休眠
                        double result = 0;

                        // 根据线程ID调整计算模式
                        if (threadId % 4 == 0)
                        {
                            // 模式1：密集浮点计算
                            for (long j = 0; j < 8000000 && !cts.Token.IsCancellationRequested; j++)
                            {
                                result += Math.Sqrt(j * j + 1) * Math.Sin(j) * Math.Cos(j);
                                // 少量分支增加CPU压力
                                if (j % 100000 == 0)
                                {
                                    result = Math.Abs(result);
                                }
                            }
                        }
                        else if (threadId % 4 == 1)
                        {
                            // 模式2：密集整数和内存计算
                            long sum = 0;
                            for (long j = 0; j < 12000000 && !cts.Token.IsCancellationRequested; j++)
                            {
                                sum += j ^ (j >> 3) ^ (j << 5);
                                // 每100万次重置，避免溢出
                                if (j % 1000000 == 0 && j > 0)
                                {
                                    result += Math.Sqrt(sum);
                                    sum = 0;
                                }
                            }
                            result += Math.Sqrt(sum);
                        }
                        else if (threadId % 4 == 2)
                        {
                            // 模式3：混合计算
                            for (long j = 0; j < 10000000 && !cts.Token.IsCancellationRequested; j++)
                            {
                                double x = j * 0.001;
                                result += Math.Exp(-x * x) * Math.Log(x + 1.0) * Math.Atan(x);

                                // 添加一些条件分支增加压力
                                if ((j & 0xFFF) == 0)
                                {
                                    result = result > 1000000 ? result / 1000 : result;
                                }
                            }
                        }
                        else
                        {
                            // 模式4：矩阵计算模拟
                            for (long j = 0; j < 6000000 && !cts.Token.IsCancellationRequested; j++)
                            {
                                double a = j * 0.01;
                                double b = (j + 1000) * 0.01;
                                result += Math.Pow(Math.Sin(a), 2) + Math.Pow(Math.Cos(b), 2)
                                        - 2 * Math.Sin(a) * Math.Cos(b);
                            }
                        }

                        // 极短暂的休息（仅用于防止死循环优化）
                        if (!cts.Token.IsCancellationRequested && threadSw.ElapsedMilliseconds % 5000 < 10)
                        {
                            Thread.Sleep(1);
                        }

                        // 防止编译器优化掉计算结果
                        if (Math.Abs(result) < 0.0001 && threadSw.ElapsedMilliseconds > 10000)
                        {
                            // 重新开始计算
                            result = 0;
                        }
                    }
                }, cts.Token));
            }

            // 等待压力线程充分启动
            Console.WriteLine("等待压力线程启动...");
            Thread.Sleep(2000);

     
  

            // 在CPU压力下测试SDK连接
            Console.WriteLine("\n在高强度CPU压力下测试SDK连接...");
            int successCount = 0;
            int disconnectCount = 0;
            int otherErrors = 0;
            int totalAttempts = 0;

            Stopwatch testTimer = Stopwatch.StartNew();
            DateTime lastPrintTime = DateTime.Now;
            int consecutiveFails = 0;
            int maxConsecutiveFails = 0;

            List<string> errorPattern = new List<string>();

            while (testTimer.ElapsedMilliseconds < 30000) // 30秒测试
            {
                totalAttempts++;

                try
                {
                    int result = robot.GetRobotRealTimeState(ref statePkg);

                    if (result == 0)
                    {
                        successCount++;
                        Console.Write("✓");
                        consecutiveFails = 0;
                        errorPattern.Add("S");
                    }
                    else if (result == -2)
                    {
                        disconnectCount++;
                        Console.Write("X");
                        consecutiveFails++;
                        maxConsecutiveFails = Math.Max(maxConsecutiveFails, consecutiveFails);
                        errorPattern.Add("X");
                    }
                    else
                    {
                        otherErrors++;
                        Console.Write($"[{result}]");
                        consecutiveFails++;
                        errorPattern.Add($"E{result}");
                    }

                    // 定期显示详细状态
                    if (DateTime.Now - lastPrintTime > TimeSpan.FromSeconds(3))
                    {
                        double successRate = totalAttempts > 0 ? (double)successCount / totalAttempts * 100 : 0;
                        Console.WriteLine($"\n[状态] 成功: {successCount}/{totalAttempts} ({successRate:F1}%) | "
                            + $"断开: {disconnectCount} | 其他错误: {otherErrors} | "
                            + $"最大连续失败: {maxConsecutiveFails}");
                        lastPrintTime = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    otherErrors++;
                    Console.Write($"!{ex.GetType().Name[0]}");
                    errorPattern.Add("!");
                }

                // 动态调整测试间隔 - 根据失败率调整
                int delay = 300; // 基础间隔300ms

                if (consecutiveFails > 5)
                {
                    delay = 1000; // 连续失败时延长间隔
                }
                else if (disconnectCount > successCount * 3)
                {
                    delay = 800; // 失败率过高时延长间隔
                }
                else if (successCount > disconnectCount * 2)
                {
                    delay = 200; // 成功率较高时缩短间隔
                }

                Thread.Sleep(delay);
            }

            // 停止压力
            cts.Cancel();
            Console.WriteLine("\n\n停止CPU压力...");

            try
            {
                Task.WaitAll(cpuTasks.ToArray(), 3000);
            }
            catch (AggregateException) { }

            try
            {
                monitoringTask?.Wait(1000);
            }
            catch { }

            // 分析错误模式
            Console.WriteLine("\n=== 错误模式分析 ===");
            string patternStr = string.Join("", errorPattern);
            Console.WriteLine($"测试序列: {patternStr}");

            // 统计连续失败段
            var failSegments = new List<int>();
            int currentFail = 0;
            foreach (char c in patternStr)
            {
                if (c == 'X' || c == '!' || (c >= 'E' && c <= 'E' + 9))
                {
                    currentFail++;
                }
                else if (currentFail > 0)
                {
                    failSegments.Add(currentFail);
                    currentFail = 0;
                }
            }
            if (currentFail > 0) failSegments.Add(currentFail);

            if (failSegments.Count > 0)
            {
                Console.WriteLine($"连续失败段数: {failSegments.Count}");
                Console.WriteLine($"平均连续失败长度: {failSegments.Average():F1}");
                Console.WriteLine($"最大连续失败长度: {failSegments.Max()}");
            }

            Console.WriteLine("\n=== 测试结果 ===");
            Console.WriteLine($"总尝试次数：{totalAttempts}");
            Console.WriteLine($"成功调用：{successCount}次 ({(double)successCount / totalAttempts:P1})");
            Console.WriteLine($"连接断开(-2)：{disconnectCount}次 ({(double)disconnectCount / totalAttempts:P1})");
            Console.WriteLine($"其他错误：{otherErrors}次 ({(double)otherErrors / totalAttempts:P1})");

            double overallSuccessRate = totalAttempts > 0 ? (double)successCount / totalAttempts * 100 : 0;
            Console.WriteLine($"\n总体成功率：{overallSuccessRate:F1}%");

            // 恢复测试 - 更详细的恢复过程
            Console.WriteLine("\n=== 恢复测试 ===");
            Console.WriteLine("等待系统恢复...");

            // 给系统更多时间恢复
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(1000);
                Console.Write(".");
            }
            Console.WriteLine();

            int recoverySuccess = 0;
            for (int i = 0; i < 10; i++) // 尝试10次恢复
            {
                try
                {
                    int recoveryResult = robot.GetRobotRealTimeState(ref statePkg);

                    if (recoveryResult == 0)
                    {
                        recoverySuccess++;
                        Console.WriteLine($"恢复测试 {i + 1}: ✓ 成功");

                        // 连续成功3次认为已恢复
                        if (recoverySuccess >= 3)
                        {
                            Console.WriteLine("✓ 连接已稳定恢复");
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"恢复测试 {i + 1}: 返回 {recoveryResult}");
                        recoverySuccess = 0; // 重置连续成功计数
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"恢复测试 {i + 1}: 异常 {ex.Message}");
                    recoverySuccess = 0;
                }

                if (i < 9) Thread.Sleep(500);
            }

            if (recoverySuccess < 3)
            {
                Console.WriteLine("⚠️ 连接恢复不完全");
            }

            Console.WriteLine("\n测试完成！");
        }
        public void TestCPUStressImpactImproved()
        {
            Console.WriteLine("=== 测试2：CPU压力对连接的影响（改进版）===");

            // 先测试正常状态
            Console.WriteLine("CPU压力前测试...");
            ROBOT_STATE_PKG statePkg = new ROBOT_STATE_PKG();
            int initialResult = robot.GetRobotRealTimeState(ref statePkg);
            Console.WriteLine($"初始状态：返回 {initialResult}");

            // 创建可控的CPU压力
            Console.WriteLine("\n开始施加可控CPU压力...");
            CancellationTokenSource cts = new CancellationTokenSource();
            List<Task> cpuTasks = new List<Task>();

            int coreCount = Environment.ProcessorCount;

            // 使用更合理的压力级别
            for (int i = 0; i < coreCount; i++) // 1倍核心数
            {
                cpuTasks.Add(Task.Run(() =>
                {
                    int threadId = i;
                    Random rnd = new Random(threadId);

                    while (!cts.Token.IsCancellationRequested)
                    {
                        // 使用更可控的计算强度
                        DateTime start = DateTime.Now;
                        double result = 0;

                        // 根据线程ID调整计算强度
                        long iterations = 500000 + rnd.Next(500000);

                        for (long j = 0; j < iterations && !cts.Token.IsCancellationRequested; j++)
                        {
                            result += Math.Sqrt(j) * Math.Log(j + 1);

                            // 每计算一定次数后检查取消令牌
                            if (j % 10000 == 0 && cts.Token.IsCancellationRequested)
                                break;
                        }

                        // 更频繁的短暂休息，让出CPU时间
                        if (!cts.Token.IsCancellationRequested)
                        {
                            Thread.Sleep(10 + rnd.Next(20));
                        }
                    }
                }, cts.Token));
            }

            // 等待压力线程稳定
            Thread.Sleep(1000);

            // 测试SDK连接
            Console.WriteLine("在可控CPU压力下测试SDK连接...");
            int successCount = 0;
            int disconnectCount = 0;
            int totalAttempts = 0;

            Stopwatch testTimer = Stopwatch.StartNew();
            DateTime lastPrintTime = DateTime.Now;

            while (testTimer.ElapsedMilliseconds < 30000)
            {
                totalAttempts++;

                try
                {
                    int result = robot.GetRobotRealTimeState(ref statePkg);

                    if (result == 0)
                    {
                        successCount++;
                        Console.Write("✓");
                    }
                    else if (result == -2)
                    {
                        disconnectCount++;
                        Console.Write("X");
                    }
                    else
                    {
                        Console.Write($"[{result}]");
                    }

                    // 定期显示状态
                    if (DateTime.Now - lastPrintTime > TimeSpan.FromSeconds(5))
                    {
                        Console.WriteLine($"\n[进度] 成功: {successCount}/{totalAttempts} 失败: {disconnectCount}");
                        lastPrintTime = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    Console.Write($"E({ex.Message.Substring(0, Math.Min(10, ex.Message.Length))})");
                }

                // 动态调整测试间隔
                int delay = 500; // 500ms基础间隔
                if (disconnectCount > successCount * 2)
                {
                    delay = 1000; // 失败较多时延长间隔
                }

                Thread.Sleep(delay);
            }

            // 停止CPU压力
            cts.Cancel();
            Task.WaitAll(cpuTasks.ToArray(), 2000);

            Console.WriteLine($"\n\nCPU压力测试结果：");
            Console.WriteLine($"总尝试次数：{totalAttempts}");
            Console.WriteLine($"成功调用：{successCount}次 ({(double)successCount / totalAttempts:P1})");
            Console.WriteLine($"连接断开：{disconnectCount}次 ({(double)disconnectCount / totalAttempts:P1})");

            // 恢复测试
            Console.WriteLine("\n等待系统恢复...");
            Thread.Sleep(3000); // 更长的恢复时间

            for (int i = 0; i < 3; i++)
            {
                int recoveryResult = robot.GetRobotRealTimeState(ref statePkg);
                Console.WriteLine($"恢复测试 {i + 1}: 返回 {recoveryResult}");

                if (recoveryResult == 0)
                {
                    Console.WriteLine("✓ 连接已恢复");
                    break;
                }

                Thread.Sleep(1000);
            }
        }

        public void TestMemoryStressImpact()
        {
            Console.WriteLine("=== 测试3：内存压力对连接的影响 ===");

            // var robot = new fairino.Robot();
            ROBOT_STATE_PKG statePkg = new ROBOT_STATE_PKG();

            // 先测试正常状态
            Console.WriteLine("内存压力前测试...");
            int initialResult = robot.GetRobotRealTimeState(ref statePkg);
            Console.WriteLine($"初始状态：返回 {initialResult}");

            // 内存压力测试
            Console.WriteLine("\n开始施加内存压力...");
            List<byte[]> memoryBlocks = new List<byte[]>();
            List<object> objectList = new List<object>();

            int successCount = 0;
            int disconnectCount = 0;
            bool memoryException = false;

            try
            {
                for (int i = 0; i < 50; i++) // 最多尝试50次
                {
                    // 分配内存块 (10-50MB随机)
                    int blockSize = 10 + (i % 5) * 10; // 10, 20, 30, 40, 50 MB
                    byte[] block = new byte[blockSize * 1024 * 1024];
                    memoryBlocks.Add(block);

                    // 填充一些数据
                    for (int j = 0; j < 1000; j++)
                    {
                        objectList.Add(new string('X', 10000));
                    }

                    // 测试SDK连接
                    int result = robot.GetRobotRealTimeState(ref statePkg);

                    if (result == 0)
                    {
                        successCount++;
                        Console.Write(".");
                    }
                    else if (result == -2)
                    {
                        disconnectCount++;
                        Console.Write("X");

                        // 连接断开后尝试清理内存并重试
                        Console.WriteLine($"\n连接断开，尝试清理内存...");
                        memoryBlocks.Clear();
                        objectList.Clear();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        Thread.Sleep(1000);

                        // 重试连接
                        result = robot.GetRobotRealTimeState(ref statePkg);
                        Console.WriteLine($"清理后重试：返回 {result}");
                    }
                    else
                    {
                        Console.Write($"E{result}");
                    }

                    // 每10次换行并显示内存状态
                    if ((i + 1) % 10 == 0)
                    {
                        Console.WriteLine();
                        long totalMemory = GC.GetTotalMemory(false) / 1024 / 1024;
                        Console.WriteLine($"已分配：{memoryBlocks.Count}个块，总内存：{totalMemory}MB");
                    }

                    // 触发垃圾回收测试
                    if (i % 15 == 0 && i > 0)
                    {
                        Console.WriteLine($"\n触发垃圾回收...");
                        memoryBlocks.Clear();
                        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
                        GC.WaitForPendingFinalizers();
                        Thread.Sleep(1000);
                    }

                    Thread.Sleep(500);
                }
            }
            catch (OutOfMemoryException)
            {
                memoryException = true;
                Console.WriteLine("\n触发内存不足异常！");
            }
            finally
            {
                // 清理内存
                memoryBlocks.Clear();
                objectList.Clear();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Console.WriteLine($"\n内存压力测试结果：");
            Console.WriteLine($"成功调用：{successCount}次");
            Console.WriteLine($"连接断开：{disconnectCount}次");
            Console.WriteLine($"内存异常：{(memoryException ? "是" : "否")}");

            // 内存清理后测试恢复
            Console.WriteLine("\n内存清理后测试恢复...");
            Thread.Sleep(2000);

            int recoveryResult = robot.GetRobotRealTimeState(ref statePkg);
            Console.WriteLine($"恢复测试：返回 {recoveryResult}");

            Console.WriteLine();
        }

        public void TestMemoryStressImpactExtreme()
        {
            Console.WriteLine("=== 测试3：内存压力对连接的影响（激进版）===");

            // 先测试正常状态
            Console.WriteLine("内存压力前测试...");
            ROBOT_STATE_PKG statePkg = new ROBOT_STATE_PKG();
            int initialResult = robot.GetRobotRealTimeState(ref statePkg);
            Console.WriteLine($"初始状态：返回 {initialResult}");

            // 激进内存压力测试
            Console.WriteLine("\n开始施加激进内存压力...");
            List<object> memoryHolders = new List<object>();
            Random rnd = new Random();

            int successCount = 0;
            int disconnectCount = 0;
            bool outOfMemory = false;
            double totalAllocatedMB = 0; // 改为double类型
            int testCount = 0;

            Stopwatch testTimer = Stopwatch.StartNew();

            try
            {
                // 策略1：直接分配超大内存块（接近物理内存极限）
                Console.WriteLine("策略1：分配超大内存块...");

                // 尝试分配接近系统内存的大块
                long[] hugeBlockSizes = { 512, 1024, 2048, 4096 }; // MB

                foreach (long sizeMB in hugeBlockSizes)
                {
                    if (outOfMemory) break;

                    Console.Write($"尝试分配 {sizeMB}MB... ");
                    try
                    {
                        // 分配超大块（可能触发LOH和页面文件）
                        byte[] hugeBlock = new byte[sizeMB * 1024 * 1024];
                        memoryHolders.Add(hugeBlock);
                        totalAllocatedMB += sizeMB;

                        // 写入数据防止被优化
                        for (long i = 0; i < Math.Min(hugeBlock.LongLength, 1000000); i += 4096)
                        {
                            hugeBlock[i] = (byte)rnd.Next(256);
                        }

                        Console.WriteLine("✓");

                        // 分配成功后立即测试连接
                        TestConnectionExtreme(ref statePkg, ref successCount, ref disconnectCount, ref testCount);

                        // 故意不立即清理，保持内存压力
                    }
                    catch (OutOfMemoryException)
                    {
                        outOfMemory = true;
                        Console.WriteLine("✗ OOM");
                        break;
                    }
                }

                if (!outOfMemory)
                {
                    // 策略2：持续分配直到内存耗尽
                    Console.WriteLine("\n策略2：持续分配直到内存耗尽...");
                    int allocationAttempts = 0;

                    while (!outOfMemory && testTimer.ElapsedMilliseconds < 30000)
                    {
                        allocationAttempts++;

                        try
                        {
                            // 分配随机大小块（50-300MB）
                            int blockSizeMB = 50 + rnd.Next(250);
                            byte[] block = new byte[blockSizeMB * 1024 * 1024];
                            memoryHolders.Add(block);
                            totalAllocatedMB += blockSizeMB;

                            Console.Write($"+{blockSizeMB}MB ");

                            // 每分配2次测试一次连接
                            if (allocationAttempts % 2 == 0)
                            {
                                TestConnectionExtreme(ref statePkg, ref successCount, ref disconnectCount, ref testCount);
                            }

                            // 随机触发GC制造压力
                            if (rnd.NextDouble() < 0.3)
                            {
                                Console.Write("GC ");
                                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
                                Thread.Sleep(100);
                            }
                        }
                        catch (OutOfMemoryException)
                        {
                            outOfMemory = true;
                            Console.Write("!OOM! ");

                            // 内存耗尽后测试连接
                            TestConnectionExtreme(ref statePkg, ref successCount, ref disconnectCount, ref testCount);

                            // 尝试部分释放后继续
                            Console.Write("[清理部分...]");
                            int removeCount = Math.Min(5, memoryHolders.Count);
                            memoryHolders.RemoveRange(0, removeCount);
                            GC.Collect();
                            outOfMemory = false; // 尝试继续
                        }

                        // 每10次显示状态
                        if (allocationAttempts % 10 == 0)
                        {
                            Console.WriteLine();
                            ShowMemoryStatus(totalAllocatedMB, memoryHolders.Count);
                        }

                        Thread.Sleep(100);
                    }
                }

                // 策略3：碎片化攻击（如果还有内存）
                if (!outOfMemory && memoryHolders.Count > 0)
                {
                    Console.WriteLine("\n策略3：内存碎片化攻击...");

                    // 先释放一半
                    int halfCount = memoryHolders.Count / 2;
                    memoryHolders.RemoveRange(0, halfCount);
                    GC.Collect();

                    // 然后分配大量小对象填充空隙
                    for (int i = 0; i < 10000 && !outOfMemory; i++)
                    {
                        try
                        {
                            int size = 80000 + rnd.Next(20000); // 80-100KB，接近LOH边界
                            byte[] fragBlock = new byte[size];
                            memoryHolders.Add(fragBlock);
                            totalAllocatedMB += size / 1024.0 / 1024.0; // 使用double

                            if (i % 1000 == 0)
                            {
                                Console.Write(".");
                                TestConnectionExtreme(ref statePkg, ref successCount, ref disconnectCount, ref testCount);
                            }
                        }
                        catch (OutOfMemoryException)
                        {
                            Console.Write("F");
                            outOfMemory = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n测试异常: {ex.Message}");
            }
            finally
            {
                // 彻底清理内存
                Console.WriteLine("\n\n开始彻底清理内存...");

                // 阶段1：释放所有对象
                memoryHolders.Clear();

                // 阶段2：强制多次GC
                for (int i = 0; i < 5; i++)
                {
                    Console.Write($"[GC{i + 1}] ");
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
                    GC.WaitForPendingFinalizers();
                    Thread.Sleep(300);
                }

                // 阶段3：释放大对象堆
                if (Environment.Version.Major >= 4)
                {
                    Console.Write("[LOH压缩] ");
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
                }

                Console.WriteLine("✓ 清理完成");
            }

            // 显示详细结果
            Console.WriteLine($"\n=== 激进内存压力测试结果 ===");
            Console.WriteLine($"测试时长: {testTimer.Elapsed.TotalSeconds:F1}秒");
            Console.WriteLine($"连接测试次数: {testCount}次");
            Console.WriteLine($"总分配内存: {totalAllocatedMB:F0}MB");
            Console.WriteLine($"峰值对象数: {memoryHolders.Count}个");
            Console.WriteLine($"内存异常: {(outOfMemory ? "是 ✓" : "否")}");
            Console.WriteLine($"成功调用: {successCount}次");
            Console.WriteLine($"连接断开: {disconnectCount}次");

            if (testCount > 0)
            {
                double successRate = (double)successCount / testCount * 100;
                double disconnectRate = (double)disconnectCount / testCount * 100;
                Console.WriteLine($"\n成功率: {successRate:F1}%");
                Console.WriteLine($"断开率: {disconnectRate:F1}%");

                if (outOfMemory)
                {
                    Console.WriteLine($"OOM前成功率: {(double)successCount / testCount:P1}");
                }
            }

            // 激进恢复测试
            Console.WriteLine("\n=== 激进恢复测试 ===");
            Console.WriteLine("等待系统完全恢复...");

            for (int i = 0; i < 10; i++)
            {
                GC.Collect();
                Thread.Sleep(500);
                Console.Write(".");
            }
            Console.WriteLine();

            ShowMemoryStatus(totalAllocatedMB, 0);

            int recoverySuccess = 0;
            for (int i = 0; i < 15; i++) // 更长时间的恢复测试
            {
                try
                {
                    int result = robot.GetRobotRealTimeState(ref statePkg);

                    if (result == 0)
                    {
                        recoverySuccess++;
                        Console.Write($"R{i + 1}:✓ ");

                        if (recoverySuccess >= 5) // 需要连续5次成功
                        {
                            Console.WriteLine("\n✓ 连接已完全恢复");
                            break;
                        }
                    }
                    else
                    {
                        Console.Write($"R{i + 1}:{result} ");
                        recoverySuccess = 0;
                    }
                }
                catch
                {
                    Console.Write($"R{i + 1}:E ");
                    recoverySuccess = 0;
                }

                Thread.Sleep(800);
            }

            if (recoverySuccess < 5)
            {
                Console.WriteLine("\n⚠️ 警告：连接恢复不完全");
            }

            Console.WriteLine("\n测试结束！");
        }

        private void TestConnectionExtreme(ref ROBOT_STATE_PKG statePkg, ref int successCount, ref int disconnectCount, ref int testCount)
        {
            testCount++;

            try
            {
                int result = robot.GetRobotRealTimeState(ref statePkg);

                if (result == 0)
                {
                    successCount++;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("✓");
                    Console.ResetColor();
                }
                else if (result == -2)
                {
                    disconnectCount++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("X");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"[{result}]");
                    Console.ResetColor();
                }
            }
            catch (OutOfMemoryException)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("!");
                Console.ResetColor();
            }
            catch (Exception)
            {
                Console.Write("E");
            }
        }

        // 修改ShowMemoryStatus方法，参数改为double
        private void ShowMemoryStatus(double totalAllocatedMB, int objectCount)
        {
            long currentMemory = GC.GetTotalMemory(false) / 1024 / 1024;
            long privateMemory = 0;

            try
            {
                privateMemory = Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024;
            }
            catch { }

            Console.WriteLine($"[内存状态] 当前: {currentMemory}MB | 私有: {privateMemory}MB | 累计分配: {totalAllocatedMB:F0}MB | 对象数: {objectCount}");
        }

        public void TestMixedStressImpact()
        {
            Console.WriteLine("=== 测试4：CPU+内存混合压力测试 ===");

            // var robot = new fairino.Robot();
            ROBOT_STATE_PKG statePkg = new ROBOT_STATE_PKG();

            Console.WriteLine("开始混合压力测试（60秒）...");

            // CPU压力线程
            Thread cpuThread = new Thread(() =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < 60000)
                {
                    // CPU密集型计算
                    double result = 0;
                    for (long i = 0; i < 3000000; i++)
                    {
                        result += Math.Pow(i, 0.33) * Math.Sin(i);
                    }

                    // 每100ms休息一下
                    if (sw.ElapsedMilliseconds % 100 < 10)
                    {
                        Thread.Sleep(1);
                    }
                }
            });

            cpuThread.Priority = ThreadPriority.Highest;
            cpuThread.Start();

            // 内存压力
            List<byte[]> memoryBlocks = new List<byte[]>();

            int successCount = 0;
            int disconnectCount = 0;
            Stopwatch testTimer = Stopwatch.StartNew();

            try
            {
                while (testTimer.ElapsedMilliseconds < 60000)
                {
                    // 周期性分配内存
                    if (testTimer.ElapsedMilliseconds % 3000 < 100) // 每3秒分配一次
                    {
                        byte[] block = new byte[5 * 1024 * 1024]; // 5MB
                        memoryBlocks.Add(block);

                        // 定期清理
                        if (memoryBlocks.Count > 20)
                        {
                            memoryBlocks.RemoveRange(0, 10);
                            GC.Collect();
                        }
                    }

                    // 测试SDK连接
                    int result = robot.GetRobotRealTimeState(ref statePkg);

                    if (result == 0)
                    {
                        successCount++;
                        Console.Write(".");
                    }
                    else if (result == -2)
                    {
                        disconnectCount++;
                        Console.Write("X");

                        // 短暂清理内存
                        memoryBlocks.Clear();
                        GC.Collect();
                        Thread.Sleep(100);
                    }

                    // 每30次换行
                    if ((successCount + disconnectCount) % 30 == 0)
                    {
                        Console.WriteLine();
                    }

                    Thread.Sleep(500);
                }
            }
            finally
            {
                // 清理
                memoryBlocks.Clear();
                cpuThread.Join();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Console.WriteLine($"\n混合压力测试结果：");
            Console.WriteLine($"成功调用：{successCount}次");
            Console.WriteLine($"连接断开：{disconnectCount}次");

            // 压力结束后测试
            Console.WriteLine("\n压力结束后测试恢复...");
            Thread.Sleep(2000);

            int recoveryResult = robot.GetRobotRealTimeState(ref statePkg);
            Console.WriteLine($"恢复测试：返回 {recoveryResult}");

            Console.WriteLine();
        }

        public void MonitorSystemResources()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            PerformanceCounter memoryCounter = new PerformanceCounter("Memory", "Available MBytes");

            cpuCounter.NextValue(); // 第一次调用需要初始化
            Thread.Sleep(100);

            float cpuUsage = cpuCounter.NextValue();
            float availableMemory = memoryCounter.NextValue();

            Console.WriteLine($"CPU使用率: {cpuUsage:F1}%");
            Console.WriteLine($"可用内存: {availableMemory:F0}MB");
        }
        public void LaserSensorRecordandReplay()
        {
            int rtn = robot.LaserSensorRecordandReplay(0, 10, 1, 0, 0.1, 1, 0, 100, 100);
            Console.WriteLine($"LaserSensorRecordandReplay rtn is {rtn}");
            rtn = robot.MoveStationary();
            Console.WriteLine($"MoveStationary rtn is {rtn}");
            rtn = robot.LaserSensorRecord1(0, 10);
            Console.WriteLine($"LaserSensorRecord1 rtn is {rtn}"); 
        }

        public void TestPhotoelectricSensorTCPCalib()
        {
            ROBOT_STATE_PKG pkg =new ROBOT_STATE_PKG();
            DescTran offset = new DescTran( 10.0, 10.0, 3.0 );
            DescPose TCP = new DescPose();
            int rtn = robot.PhotoelectricSensorTCPCalibration("FR_CalibrateTheToolTcp-061101.lua", offset, out TCP);
            Console.WriteLine($"PhotoelectricSensorTCPCalibration 返回值: {rtn}");
            Console.WriteLine($"工具TCP坐标: X={TCP.tran.x:F3}, Y={TCP.tran.y:F3}, Z={TCP.tran.z:F3}");
            Console.WriteLine($"工具RPY姿态: RX={TCP.rpy.rx:F3}, RY={TCP.rpy.ry:F3}, RZ={TCP.rpy.rz:F3}");
        }
        public void TestSegWeld1()
        {
            robot.WeldingSetCurrent(0, 230, 0, 0);
            robot.WeldingSetVoltage(0, 24, 0, 1);

            DescPose p2Desc = new DescPose(228.879, -503.594, 453.984, -175.580, 8.293, 171.267);
            JointPos p2Joint = new JointPos(153.567, -78.601, 88.444, -88.802, -93.088, 124.632);

            DescPose p1Desc = new DescPose(-333.302, -435.580, 449.866, -174.997, 2.017, 109.815);
            JointPos p1Joint = new JointPos(112.528, -85.587, 94.358, -88.755, -98.871, 124.634);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.GetForwardKin(p1Joint, ref p1Desc);
            robot.GetForwardKin(p2Joint, ref p2Desc);

            int rtn = robot.SegmentWeldStart(p1Desc, p2Desc, p1Joint, p2Joint, 20,
                    20, 0, 0, 5000, false, 0,
                    0, 0, 30, 100, 100, -1, exaxisPos, 0, 0,
                    offdese);
           
        }
        public void TestMove()
        {
            int rtn;
            JointPos j1 = new JointPos(-11.904f, -99.669f, 117.473f, -108.616f, -91.726f, 74.256f);
            JointPos j2 = new JointPos(-45.615f, -106.172f, 124.296f, -107.151f, -91.282f, 74.255f);
            JointPos j3 = new JointPos(-29.777f, -84.536f, 109.275f, -114.075f, -86.655f, 74.257f);
            JointPos j4 = new JointPos(-31.154f, -95.317f, 94.276f, -88.079f, -89.740f, 74.256f);
            DescPose desc_pos1 = new DescPose(-419.524f, -13.000f, 351.569f, -178.118f, 0.314f, 3.833f);
            DescPose desc_pos2 = new DescPose(-321.222f, 185.189f, 335.520f, -179.030f, -1.284f, -29.869f);
            DescPose desc_pos3 = new DescPose(-487.434f, 154.362f, 308.576f, 176.600f, 0.268f, -14.061f);
            DescPose desc_pos4 = new DescPose(-443.165f, 147.881f, 480.951f, 179.511f, -0.775f, -15.409f);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 100.0f;
            float oacc = 100.0f;
            float blendT = 0.0f;
            float blendR = 0.0f;
            byte flag = 0;
            byte search = 0;
            int blendMode = 0;
            int velAccMode = 0;
            robot.SetSpeed(20);
            rtn = robot.MoveJ(j1, desc_pos1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            Console.WriteLine($"movej errcode:{rtn}");
            rtn = robot.MoveL(j2, desc_pos2, tool, user, vel, acc, ovl, blendR, blendMode, epos, search, flag, offset_pos, oacc, velAccMode,0,10);
            Console.WriteLine($"movel errcode:{rtn}");
            rtn = robot.MoveC(j3, desc_pos3, tool, user, vel, acc, epos, flag, offset_pos,j4, desc_pos4, tool, user, vel, acc, epos, flag, offset_pos, ovl, blendR, oacc, velAccMode);
            Console.WriteLine($"movec errcode:{rtn}");
            rtn = robot.MoveJ(j2, desc_pos2, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            Console.WriteLine($"movej errcode:{rtn}");
            rtn = robot.Circle(j3, desc_pos3, tool, user, vel, acc, epos,j1, desc_pos1, tool, user, vel, acc, epos,ovl, flag, offset_pos, oacc, -1, velAccMode);
            Console.WriteLine($"circle errcode:{rtn}");
            rtn = robot.MoveCart(desc_pos4, tool, user, vel, acc, ovl, blendT, -1);
            Console.WriteLine($"MoveCart errcode:{rtn}");
            rtn = robot.MoveJ(j1, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            Console.WriteLine($"movej errcode:{rtn}");
            rtn = robot.MoveL(desc_pos2, tool, user, vel, acc, ovl, blendR, blendMode, epos, search, flag, offset_pos, -1, velAccMode);
            Console.WriteLine($"movel errcode:{rtn}");
            rtn = robot.MoveC(desc_pos3, tool, user, vel, acc, epos, flag, offset_pos,desc_pos4, tool, user, vel, acc, epos, flag, offset_pos,ovl, blendR, -1, velAccMode);
            Console.WriteLine($"movec errcode:{rtn}");
            rtn = robot.MoveJ(j2, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos);
            Console.WriteLine($"movej errcode:{rtn}");
            rtn = robot.Circle(desc_pos3, tool, user, vel, acc, epos, desc_pos1, tool, user, vel, acc, epos,ovl, flag, offset_pos, oacc, blendR, -1, velAccMode);
            Console.WriteLine($"circle errcode:{rtn}");
        }
        public void TestFTControlWithAdjustCoeff()
        {

            int rtn;
            int sensor_id = 1;
            byte[] select = new byte[6] { 0, 0, 1, 0, 0, 0 };
            float[] ft_pid = new float[6] { 0.0008f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            byte adj_sign = 0;
            byte ILC_sign = 0;
            float max_dis = 1000.0f;
            float max_ang = 20.0f;

            ForceTorque ft = new ForceTorque();
            ft.fz = -10.0f;

            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            JointPos j1 = new JointPos(80.987, -98.000, 106.000, -97.000, -89.000, 94.000);
            JointPos j2 = new JointPos(80.979, -78.962, 104.646, -124.541, -89.583, 94.051);
            
            DescPose desc_p1 = new DescPose(34.747, -443.165, 416.139, 179.072, -1.068, 76.987);
            DescPose desc_p2 = new DescPose(14.665, -562.784, 314.841, 178.953, 8.805, 76.880);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);

            double[] M = new double[2] { 2.0, 2.0 };
            double[] B = new double[2] { 15.0, 15.0 };
            double[] threshold = new double[2] { 1.0, 1.0 };
            double[] adjustCoeff = new double[2] { 1.0, 0.8 };
            robot.MoveL(j1, desc_p1, 0, 0, 100, 100, 100, -1, 0, epos, 0, 0, offset_pos, 0, 0, 0, 10);
            robot.MoveL(j2, desc_p2, 0, 0, 100, 100, 100, -1, 0, epos, 0, 0, offset_pos, 0, 0, 0, 10);
            while (true)
            {

                rtn = robot.FT_Control(1, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang, M, B, threshold, adjustCoeff, 0, 0, 1, 0);
                Console.WriteLine($"FT_Control start rtn is {rtn}");

                rtn = robot.FT_Control(0, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang, M, B, threshold, adjustCoeff, 0, 0, 1, 0);
                Console.WriteLine($"FT_Control end rtn is {rtn}");

            }
        }
        public int TestSensitivityCalib()
        {
            int rtn;
   
            rtn = robot.JointSensitivityEnable(0);
            rtn = robot.JointSensitivityEnable(1);
            Console.WriteLine($"JointSensitivityEnable rtn is {rtn}");

            JointPos curJPos = new JointPos(0, 0, 0, 0, 0, 0);
            robot.GetActualJointPosDegree(0, ref curJPos);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            JointPos[] jointPoses = new JointPos[]
            {
                new JointPos(curJPos.jPos[0], 0, 0, -90, 0.02, curJPos.jPos[5]),
                new JointPos(curJPos.jPos[0], -30, 0, -90, 0.02, curJPos.jPos[5]),
                new JointPos(curJPos.jPos[0], -60, 0, -90, 0.02, curJPos.jPos[5]),
                new JointPos(curJPos.jPos[0], -90, 0, -90, 0.02, curJPos.jPos[5]),
                new JointPos(curJPos.jPos[0], -120, 0, -90, 0.02, curJPos.jPos[5]),
                new JointPos(curJPos.jPos[0], -150, 0, -90, 0.02, curJPos.jPos[5]),
                new JointPos(curJPos.jPos[0], -180, 0, -90, 0.02, curJPos.jPos[5])
            };
            for (int i = 0; i < jointPoses.Length; i++)
            {
                DescPose descPos = new DescPose(0, 0, 0, 0, 0, 0);
                robot.GetForwardKin(jointPoses[i], ref descPos);
                robot.MoveJ(jointPoses[i], descPos, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);

                Thread.Sleep(i == 0 ? 200 : 100);
                rtn = robot.JointSensitivityCollect();
                Console.WriteLine($"JointSensitivityCollect {i + 1} rtn is {rtn}");
                Thread.Sleep(100);
            }

            for (int i = jointPoses.Length - 2; i >= 0; i--)
            {
                DescPose descPos = new DescPose();
                robot.GetForwardKin(jointPoses[i], ref descPos);
                robot.MoveJ(jointPoses[i], descPos, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);

                Thread.Sleep(100);
                rtn = robot.JointSensitivityCollect();
                Console.WriteLine($"JointSensitivityCollect {jointPoses.Length + (jointPoses.Length - 1 - i)} rtn is {rtn}");
                Thread.Sleep(100);
            }

            double[] calibResult = new double[6];
            double[] linearity = new double[6];
            rtn = robot.JointSensitivityCalibration(ref calibResult, ref linearity);
            Console.WriteLine($"JointSensitivityCalibration rtn is {rtn}");

            rtn = robot.JointSensitivityEnable(0);
            Console.WriteLine($"JointSensitivityEnable rtn is {rtn}");

            Console.WriteLine($"jointSensor Calib result is {calibResult[0]:F6} {calibResult[1]:F6} {calibResult[2]:F6} {calibResult[3]:F6} {calibResult[4]:F6} {calibResult[5]:F6}");
            Console.WriteLine($"jointSensor linearity is {linearity[0]:F6} {linearity[1]:F6} {linearity[2]:F6} {linearity[3]:F6} {linearity[4]:F6} {linearity[5]:F6}");

   
            double[] hysteresisError = new double[6];
            rtn = robot.JointHysteresisError(ref hysteresisError);
            Console.WriteLine($"JointHysteresisError result is {hysteresisError[0]:F6} {hysteresisError[1]:F6} {hysteresisError[2]:F6} {hysteresisError[3]:F6} {hysteresisError[4]:F6} {hysteresisError[5]:F6}");

     
            double[] repeatability = new double[6];
            rtn = robot.JointRepeatability(ref repeatability);
            Console.WriteLine($"JointRepeatability result is {repeatability[0]:F6} {repeatability[1]:F6} {repeatability[2]:F6} {repeatability[3]:F6} {repeatability[4]:F6} {repeatability[5]:F6}");


            double[] M = new double[6] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
            double[] B = new double[6] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
            double[] K = new double[6] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] threshold = new double[6] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
            int setZeroFlag = 1;
            rtn = robot.SetAdmittanceParams(M, B, K, threshold, calibResult, setZeroFlag);
            Console.WriteLine($"SetAdmittanceParams rtn is {rtn}");

            robot.CloseRPC();
            return 0;
        }

        public void TestIntersectLineMove()
        {
            int rtn;
            DescPose[] mainPoint = new DescPose[6];
            DescPose[] piecePoint = new DescPose[6];

            ExaxisPos[] mainExaxisPos = new ExaxisPos[6];
            ExaxisPos[] pieceExaxisPos = new ExaxisPos[6];
            int extAxisFlag = 0;
            ExaxisPos[] exaxisPos = new ExaxisPos[4];
            DescPose offset = new DescPose(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);

            mainPoint[0] = new DescPose(-411.572, -516.869, 197.724, -111.821, 31.353, -145.537);
            mainPoint[1] = new DescPose(-430.242, -575.160, 205.215, -107.763, 47.877, -141.814);
            mainPoint[2] = new DescPose(-443.560, -608.068, 180.211, -139.983, 78.547, -170.874);
            mainPoint[3] = new DescPose(-443.718, -608.250, 130.382, -155.397, 80.964, 173.955);
            mainPoint[4] = new DescPose(-436.198, -582.428, 100.045, 174.899, 72.468, 157.366);
            mainPoint[5] = new DescPose(-420.815, -527.510, 106.649, 123.128, 67.885, 110.539);

            piecePoint[0] = new DescPose(-341.600, -568.334, 327.186, 5.404, -3.657, -145.629);
            piecePoint[1] = new DescPose(-319.224, -619.882, 330.833, 2.439, -3.294, -141.933);
            piecePoint[2] = new DescPose(-278.636, -609.413, 329.042, 4.194, -7.682, -138.522);
            piecePoint[3] = new DescPose(-270.948, -567.929, 326.010, 1.932, -4.908, -138.190);
            piecePoint[4] = new DescPose(-291.152, -544.315, 324.130, -1.220, -5.373, -139.433);
            piecePoint[5] = new DescPose(-316.419, -543.041, 324.621, 0.387, -5.188, -142.384);

            //mainPoint[0] = new DescPose(-411.572,- 516.869,  197.724, - 111.821, 31.353, - 145.537);
            //mainPoint[1] = new DescPose(444.950, -407.117, 389.011, -5.546, -2.196, 65.279);
            //mainPoint[2] = new DescPose(445.168, -463.605, 355.759, -1.544, -10.886, 57.104);
            //mainPoint[3] = new DescPose(507.529, -485.385, 343.013, -0.786, -4.834, 61.799);
            //mainPoint[4] = new DescPose(554.390, -442.647, 367.701, -4.761, -10.181, 64.925);
            //mainPoint[5] = new DescPose(532.552, -394.003, 396.467, -13.732, -13.592, 67.411);

            mainExaxisPos[0] = new ExaxisPos( 0.000, 0.000, 0.000, 0.000);
            mainExaxisPos[1] = new ExaxisPos( 0.000, 0.000, 0.000, 0.000);
            mainExaxisPos[2] = new ExaxisPos( 0.000, 0.000, 0.000, 0.000);
            mainExaxisPos[3] = new ExaxisPos( 0.000, 0.000, 0.000, 0.000);
            mainExaxisPos[4] = new ExaxisPos( 0.000, 0.000, 0.000, 0.000);
            mainExaxisPos[5] = new ExaxisPos( 0.000, 0.000, 0.000, 0.000);

            //piecePoint[0] = new DescPose(505.571, -192.408, 316.759, 38.098, 37.051, 139.447);
            //piecePoint[1] = new DescPose(533.837, -201.558, 332.340, 34.644, 42.339, 137.748);
            //piecePoint[2] = new DescPose(530.386, -225.085, 373.808, 35.431, 45.111, 137.560);
            //piecePoint[3] = new DescPose(485.646, -229.195, 383.778, 33.870, 45.173, 137.064);
            //piecePoint[4] = new DescPose(460.551, -212.161, 354.256, 28.856, 45.602, 135.930);
            //piecePoint[5] = new DescPose(474.217, -197.124, 324.611, 42.469, 41.133, 148.167);

            pieceExaxisPos[0] = new ExaxisPos(0.000, -0.000, 0.000, 0.000);
            pieceExaxisPos[1] = new ExaxisPos(0.000, -0.000, 0.000, 0.000);
            pieceExaxisPos[2] = new ExaxisPos(0.000, -0.000, 0.000, 0.000);
            pieceExaxisPos[3] = new ExaxisPos(0.000, -0.000, 0.000, 0.000);
            pieceExaxisPos[4] = new ExaxisPos(0.000, -0.000, 0.000, 0.000);
            pieceExaxisPos[5] = new ExaxisPos(0.000, -0.000, 0.000, 0.000);


            exaxisPos[0] =  new ExaxisPos( 0.000, 0.000, 0.000, 0.000);
            exaxisPos[1] =  new ExaxisPos( 0.000, 0.000, 0.000, 0.000);
            exaxisPos[2] =  new ExaxisPos( 0.000, 0.000, 0.000, 0.000);
            exaxisPos[3] = new ExaxisPos(0.000, 0.000, 0.000, 0.000);

            int tool = 2;
            int wobj = 0;
            double vel = 100.0;
            double acc = 100.0;
            double ovl = 12.0;
            double oacc = 12.0;
            int moveType = 0;
            int moveDirection = 0;

            rtn = robot.MoveToIntersectLineStart(mainPoint, mainExaxisPos, piecePoint, pieceExaxisPos, extAxisFlag, exaxisPos[0], tool, wobj, vel, acc, ovl, oacc, moveType, moveDirection, offset);
            Console.WriteLine($"MoveToIntersectLineStart rtn is {rtn}");

            rtn = robot.MoveIntersectLine(mainPoint, mainExaxisPos, piecePoint, pieceExaxisPos, extAxisFlag, exaxisPos, tool, wobj, vel, acc, 5.0, 5.0, moveDirection, offset);
            Console.WriteLine($"MoveIntersectLine rtn is {rtn}");

            return;
        }
        //public void TestIntersectLineMove()
        //{
        //    int rtn;


        //    DescPose[] mainPoint = new DescPose[6];
        //    DescPose[] piecePoint = new DescPose[6];

        //    mainPoint[0] = new DescPose(144.084, 512.064, 8.899, -58.958, 40.838, 23.295);
        //    mainPoint[1] = new DescPose(132.150, 512.638, 28.157, -58.626, 40.788, 24.966);
        //    mainPoint[2] = new DescPose(150.155, 514.479, 74.107, -47.740, 40.410, 27.552);
        //    mainPoint[3] = new DescPose(188.346, 518.501, 73.946, -17.227, 60.139, 15.265);
        //    mainPoint[4] = new DescPose(206.811, 520.214, 52.966, -18.198, 60.381, 12.624);
        //    mainPoint[5] = new DescPose(203.002, 518.627, 19.028, -23.830, 61.410, 8.343);


        //    piecePoint[0] = new DescPose(190.428, 480.862, 102.236, 8.966, 46.472, 35.582);
        //    piecePoint[1] = new DescPose(201.770, 510.904, 101.912, 12.079, 66.897, 26.452);
        //    piecePoint[2] = new DescPose(186.344, 533.294, 102.866, 1.980, 62.882, 25.094);
        //    piecePoint[3] = new DescPose(162.969, 537.537, 103.015, -22.013, 46.227, 28.606);
        //    piecePoint[4] = new DescPose(139.465, 510.090, 103.505, -23.996, 33.774, 41.829);
        //    piecePoint[5] = new DescPose(168.329, 475.251, 102.325, 4.241, 42.293, 38.624);

        //    int tool = 4;
        //    int wobj = 0;
        //    double vel = 100.0;
        //    double acc = 100.0;
        //    double ovl = 10.0;
        //    double oacc = 10.0;
        //    int moveType = 1;
        //    int moveDirection = 1;


        //    rtn = robot.MoveToIntersectLineStart(mainPoint, piecePoint, tool, wobj, vel, acc, ovl, oacc, moveType);
        //    Console.WriteLine($"MoveToIntersectLineStart rtn is {rtn}");


        //    rtn = robot.MoveIntersectLine(mainPoint, piecePoint, tool, wobj, vel, acc, ovl, oacc, moveDirection);
        //    Console.WriteLine($"MoveIntersectLine rtn is {rtn}");

        //    robot.CloseRPC();
        //    return;
        //}
        public void TestLua()
        {
            int rtn;
            string errStr = "";
            rtn = robot.LuaUpload("D://zUP/suoluomen/test1.lua", ref errStr);
            Console.WriteLine("LuaUpload rtn is {0}", errStr);
            Thread.Sleep(2000);
        }


        public int ServoJTWithSafety()
        {
            while (true)
            {
                robot.ResetAllError();
                Thread.Sleep(500);

                JointPos j = new JointPos(7.053, -89.699, 156.141, -72.751, 7.829, 1.889);
                ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
                DescPose offset_pos = new DescPose(-151.288, -321.186, 221.989, 89.140, 4.361, -0.795);
                robot.MoveJ(j, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);

                double[] torques = new double[6] { 0, 0, 0, 0, 0, 0 };
                robot.GetJointTorques(1, torques);

                robot.ServoJTStart(0);
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                robot.DragTeachSwitch(1);

                int checkFlag = 0;
                double[] jPowerLimit = new double[6] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
                double[] jVelLimit = new double[6] { 50, 50, 50, 50, 50, 50 };
                int error = 0;
                while (true)
                {

                    torques[0] = 0.1;
                    error = robot.ServoJT(torques, 0.008, checkFlag, jPowerLimit, jVelLimit, 0);

                    Console.WriteLine($"ServoJT rtn is {error}");
                    Thread.Sleep(1);

                    robot.GetRobotRealTimeState(ref pkg);
                    Console.WriteLine($"maincode {pkg.main_code}, subcode {pkg.sub_code}");
                    if (pkg.jt_cur_pos[0] > 30)
                    {
                        break;
                    }
                }

                while (true)
                {

                    torques[0] = -0.1;
                    error = robot.ServoJT(torques, 0.008, checkFlag, jPowerLimit, jVelLimit, 0);

                    Console.WriteLine($"ServoJT rtn is {error}");
                    Thread.Sleep(1);

                    robot.GetRobotRealTimeState(ref pkg);
                    Console.WriteLine($"maincode {pkg.main_code}, subcode {pkg.sub_code}");
                    if (pkg.jt_cur_pos[0] < 0)
                    {
                        break;
                    }
                }

                robot.DragTeachSwitch(0);
                error = robot.ServoJTEnd(0);
            }
        }

        public int ServoJTWithSafetyUDP()
        {
            // 订阅回调
            robot.OnUdpFrameReceived += (comType, frameCount, frameCmdID, contentLen, content) =>
            {
                Console.WriteLine($"[UDP响应] comType={comType}, count={frameCount}, cmdID={frameCmdID}, content={content}");
            };
            while (true)
            {
                robot.ResetAllError();
                Thread.Sleep(500);

                JointPos j = new JointPos(7.053, -89.699, 156.141, -72.751, 7.829, 1.889);
                ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
                DescPose offset_pos = new DescPose(-151.288, -321.186, 221.989, 89.140, 4.361, -0.795);
                robot.MoveJ(j, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);

                double[] torques = new double[6] { 0, 0, 0, 0, 0, 0 };
                robot.GetJointTorques(1, torques);

                robot.ServoJTStart(1);
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                robot.DragTeachSwitch(1);

                int checkFlag = 0;
                double[] jPowerLimit = new double[6] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
                double[] jVelLimit = new double[6] { 50, 50, 50, 50, 50, 50 };
                int error = 0;
                while (true)
                {

                    torques[0] = 0.1;
                    error = robot.ServoJT(torques, 0.008, checkFlag, jPowerLimit, jVelLimit, 1);

                    Console.WriteLine($"ServoJT rtn is {error}");
                    Thread.Sleep(1);

                    robot.GetRobotRealTimeState(ref pkg);
                    Console.WriteLine($"maincode {pkg.main_code}, subcode {pkg.sub_code}");
                    if (pkg.jt_cur_pos[0] > 30)
                    {
                        break;
                    }
                }

                while (true)
                {

                    torques[0] = -0.1;
                    error = robot.ServoJT(torques, 0.008, checkFlag, jPowerLimit, jVelLimit, 1);

                    Console.WriteLine($"ServoJT rtn is {error}");
                    Thread.Sleep(1);

                    robot.GetRobotRealTimeState(ref pkg);
                    Console.WriteLine($"maincode {pkg.main_code}, subcode {pkg.sub_code}");
                    if (pkg.jt_cur_pos[0] < 0)
                    {
                        break;
                    }
                }

                robot.DragTeachSwitch(0);
                error = robot.ServoJTEnd(1);
            }
            //return 0;
        }


        public void TestFTControlWithDamping()
        {
            int rtn;
            int sensor_id = 10;
            byte[] select = new byte[6] { 0, 0, 1, 0, 0, 0 };
            float[] ft_pid = new float[6] { 0.0008f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            byte adj_sign = 0;
            byte ILC_sign = 0;
            float max_dis = 100.0f;
            float max_ang = 20.0f;
            ForceTorque ft = new ForceTorque();
            ft.fz = -10.0;
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            JointPos j1 = new JointPos(-118.985, -86.882, -118.139, -65.019, 90.002, 54.951);
            JointPos j2 = new JointPos(-77.055, -77.218, -126.219, -66.591, 90.028, 96.881);
            DescPose desc_p1 = new DescPose(-300.856, -332.618, 309.240, 179.976, -0.031, 96.065);
            DescPose desc_p2 = new DescPose(-16.399, -383.760, 309.312, 179.975, -0.031, 96.064);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            double[] M = new double[2] { 2.0, 2.0 };
            double[] B = new double[2] { 8.0, 8.0 };
            double polishRadio = 0.0;
            int filter_Sign = 0;
            int posAdapt_sign = 1;
            int isNoBlock = 0;
            DescPose ftCoord = new DescPose();
            robot.FT_SetRCS(2, ftCoord);
            rtn = robot.FT_Control(1, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang, M, B, polishRadio, filter_Sign, posAdapt_sign, isNoBlock);
            Console.WriteLine($"FT_Control start rtn is {rtn}");
            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 20.0f;
            float blendT = -1.0f;
            byte offset_flag = 0;
            rtn = robot.MoveL(j1, desc_p1, tool, user, vel, acc, ovl, blendT, epos, offset_flag, 0, offset_pos, 0, 0, 10);
            rtn = robot.MoveL(j2, desc_p2, tool, user, vel, acc, ovl, blendT, epos, offset_flag, 0, offset_pos, 0, 0, 10);
            rtn = robot.FT_Control(0, sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang, M, B, polishRadio, filter_Sign, posAdapt_sign, isNoBlock);
            Console.WriteLine($"FT_Control end rtn is {rtn}");
            robot.CloseRPC();
        }
public void TestVelFeedForwardRatio()
{

    double[] setRadio = new double[6] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
    robot.SetVelFeedForwardRatio(setRadio);

    double[] getRadio = new double[6] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
    robot.GetVelFeedForwardRatio(ref getRadio);

    Console.WriteLine($" {getRadio[0]:F6} {getRadio[1]:F6} {getRadio[2]:F6} {getRadio[3]:F6} {getRadio[4]:F6} {getRadio[5]:F6}");

}
        public int TestSpiral()
        {

            int rtn;
            // 初始化关节位置
            JointPos j = new JointPos(67.957, -81.482, 87.595, -95.691, -94.899, -9.727);

            // 初始化笛卡尔位姿
            DescPose desc_pos = new DescPose(-123.142, -551.735, 430.549, 178.753, -4.757, 167.754);


            // 初始化偏移位姿
            DescPose offset_pos1 = new DescPose(50, 0, 0, -30, 0, 0);


            DescPose offset_pos2 = new DescPose(50, 0, 0, -30, 0, 0);


            // 初始化扩展轴位置
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);

            // 初始化螺旋参数
            SpiralParam sp = new SpiralParam(
                2,      // circle_num
                30.0f,  // circle_angle
                50.0f,  // rad_init
                10.0f,  // rad_add
                10.0f,  // rotaxis_add
                0,      // rot_direction
                1       // velAccMode
            );

            int tool = 0;
            int user = 0;
            float vel = 30.0f;
            float acc = 60.0f;
            float ovl = 100.0f;
            float blendT = -1.0f;
            byte flag = 2;

            robot.SetSpeed(20);

            // 执行关节运动
            rtn = robot.MoveJ(j, tool, user, vel, acc, ovl, epos, blendT, flag, offset_pos1);
            Console.WriteLine($"movej errcode:{rtn}");

            // 执行螺旋线运动
            rtn = robot.NewSpiral(j, desc_pos, tool, user, vel, acc, epos, ovl, flag, offset_pos2, sp);
            Console.WriteLine($"newspiral errcode:{rtn}");

            robot.CloseRPC();
            return 0;
        }
        public void TestSlavePortErr()
        {


            int[] inRecvErr = new int[8];
            int[] inCRCErr = new int[8];
            int[] inTransmitErr = new int[8];
            int[] inLinkErr = new int[8];
            int[] outRecvErr = new int[8];
            int[] outCRCErr = new int[8];
            int[] outTransmitErr = new int[8];
            int[] outLinkErr = new int[8];

            robot.GetSlavePortErrCounter(ref inRecvErr, ref inCRCErr, ref inTransmitErr, ref inLinkErr,
                ref outRecvErr, ref outCRCErr, ref outTransmitErr, ref outLinkErr);

            for (int i = 0; i < 8; i++)
            {
                if (inRecvErr[i] != 0)
                {
                    Console.WriteLine($"inRecvErr {i} is {inRecvErr[i]}");
                }

                if (inCRCErr[i] != 0)
                {
                    Console.WriteLine($"inCRCErr {i} is {inCRCErr[i]}");
                }

                if (inTransmitErr[i] != 0)
                {
                    Console.WriteLine($"inTransmitErr {i} is {inTransmitErr[i]}");
                }

                if (inLinkErr[i] != 0)
                {
                    Console.WriteLine($"inLinkErr {i} is {inLinkErr[i]}");
                }

                if (outRecvErr[i] != 0)
                {
                    Console.WriteLine($"outRecvErr {i} is {outRecvErr[i]}");
                }

                if (outCRCErr[i] != 0)
                {
                    Console.WriteLine($"outCRCErr {i} is {outCRCErr[i]}");
                }

                if (outTransmitErr[i] != 0)
                {
                    Console.WriteLine($"outTransmitErr {i} is {outTransmitErr[i]}");
                }

                if (outLinkErr[i] != 0)
                {
                    Console.WriteLine($"outLinkErr {i} is {outLinkErr[i]}");
                }
            }
            Console.WriteLine("others has no err!");

            for (int i = 0; i < 8; i++)
            {
                robot.SlavePortErrCounterClear(i);
            }

            robot.CloseRPC();
        }

        public void TestServoJ()
        {
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();



            JointPos j = new JointPos(0, 0, 0, 0, 0, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);

            float vel = 0.0f;
            float acc = 0.0f;
            float cmdT = 0.008f;
            float filterT = 0.0f;
            float gain = 0.0f;
            byte flag = 0;
            int count = 300;
            float dt = 0.1f;
            int cmdID = 0;

            int ret = robot.GetActualJointPosDegree(flag, ref j);
            if (ret == 0)
            {
                cmdID += 1;
                robot.ServoMoveStart(0);
                while (count > 0)
                {
                    robot.ServoJ( j,  epos, acc, vel, cmdT, filterT, gain, cmdID, 0);

                    j.jPos[4] += dt;
                    count -= 1;
                    robot.WaitMs((int)(cmdT * 1000));
                    robot.GetRobotRealTimeState(ref pkg);
                    Console.WriteLine($"Servoj Count {pkg.servoJCmdNum}; last pos is {pkg.lastServoTarget[0]} {pkg.lastServoTarget[1]} {pkg.lastServoTarget[2]} {pkg.lastServoTarget[3]} {pkg.lastServoTarget[4]} {pkg.lastServoTarget[5]}");

                    if (count < 50)
                    {
                        robot.MotionQueueClear();
                        Console.WriteLine($"After queue clear, Servoj Count {pkg.servoJCmdNum}; last pos is {pkg.lastServoTarget[0]} {pkg.lastServoTarget[1]} {pkg.lastServoTarget[2]} {pkg.lastServoTarget[3]} {pkg.lastServoTarget[4]} {pkg.lastServoTarget[5]}");
                        break;
                    }
                }
                robot.ServoMoveEnd(0);


            }
            else
            {
                Console.WriteLine($"GetActualJointPosDegree errcode:{ret}");
            }

            //robot.CloseRPC();
        }

        public void TestServoJUDP()
        {
            // 订阅回调
            robot.OnUdpFrameReceived += (comType, frameCount, frameCmdID, contentLen, content) =>
            {
                Console.WriteLine($"[] comType={comType}, count={frameCount}, cmdID={frameCmdID}, content={content}");
            };

            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();

            float vel = 0.0f;
            float acc = 0.0f;
            float cmdT = 0.008f;
            float filterT = 0.0f;
            float gain = 0.0f;
            byte flag = 0;
            int count = 300;
            float dt = 0.1f;
            int cmdID = 0;

            while (true)
            {
                JointPos j = new JointPos(0, -90, 90, 0, 0, 0);
                ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
                DescPose offset_pos = new DescPose(0, -90, 90, 0, 0, 0);
                robot.MoveJ(j, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);
                int ret = robot.GetActualJointPosDegree(flag, ref j);
                if (ret == 0)
                {
                    count = 300;
                    cmdID += 1;
                    robot.ServoMoveStart(0);

                    while (count > 0)
                    {
                        robot.ServoJ(j, epos, acc, vel, cmdT, filterT, gain, cmdID, 0);
                        j.jPos[0] += dt;
                        j.jPos[1] += dt;
                        j.jPos[3] += dt;
                        j.jPos[4] += dt;
                        j.jPos[5] += dt;
                        epos.ePos[0] += dt;
                        count -= 1;
                        Thread.Sleep(1);
                        robot.GetRobotRealTimeState(ref pkg);
                        Console.WriteLine($"Servoj命令数量: {pkg.servoJCmdNum}");
                        Console.WriteLine($"Servoj Count {pkg.servoJCmdNum}; last pos is {pkg.lastServoTarget[0]} {pkg.lastServoTarget[1]} {pkg.lastServoTarget[2]} {pkg.lastServoTarget[3]} {pkg.lastServoTarget[4]} {pkg.lastServoTarget[5]}");
                        if (pkg.jt_cur_pos != null && pkg.jt_cur_pos.Length >= 6)
                        {
                            Console.WriteLine($"  关节位置(°): J1={pkg.jt_cur_pos[0]:F2}, J2={pkg.jt_cur_pos[1]:F2}, J3={pkg.jt_cur_pos[2]:F2}, J4={pkg.jt_cur_pos[3]:F2}, J5={pkg.jt_cur_pos[4]:F2}, J6={pkg.jt_cur_pos[5]:F2}");
                        }
                        if (pkg.tl_cur_pos != null && pkg.tl_cur_pos.Length >= 6)
                        {
                            Console.WriteLine($"  工具位姿: X={pkg.tl_cur_pos[0]:F2}mm, Y={pkg.tl_cur_pos[1]:F2}mm, Z={pkg.tl_cur_pos[2]:F2}mm, RX={pkg.tl_cur_pos[3]:F2}°, RY={pkg.tl_cur_pos[4]:F2}°, RZ={pkg.tl_cur_pos[5]:F2}°");
                        }

                    }
                    robot.ServoMoveEnd(0);

                    Thread.Sleep(1000);
                    count = 300;
                    robot.ServoMoveStart(0);
                    while (count > 0)
                    {
                        robot.ServoJ(j, epos, acc, vel, cmdT, filterT, gain, cmdID, 0);
                        j.jPos[0] -= dt;
                        j.jPos[1] -= dt;
                        j.jPos[3] -= dt;
                        j.jPos[4] -= dt;
                        j.jPos[5] -= dt;
                        epos.ePos[0] -= dt;
                        count -= 1;
                        Thread.Sleep(1);
                        robot.GetRobotRealTimeState(ref pkg);
                        Console.WriteLine($"Servoj命令数量: {pkg.servoJCmdNum}");
                        Console.WriteLine($"Servoj Count {pkg.servoJCmdNum}; last pos is {pkg.lastServoTarget[0]} {pkg.lastServoTarget[1]} {pkg.lastServoTarget[2]} {pkg.lastServoTarget[3]} {pkg.lastServoTarget[4]} {pkg.lastServoTarget[5]}");
                        if (pkg.jt_cur_pos != null && pkg.jt_cur_pos.Length >= 6)
                        {
                            Console.WriteLine($"  关节位置(°): J1={pkg.jt_cur_pos[0]:F2}, J2={pkg.jt_cur_pos[1]:F2}, J3={pkg.jt_cur_pos[2]:F2}, J4={pkg.jt_cur_pos[3]:F2}, J5={pkg.jt_cur_pos[4]:F2}, J6={pkg.jt_cur_pos[5]:F2}");
                        }
                        if (pkg.tl_cur_pos != null && pkg.tl_cur_pos.Length >= 6)
                        {
                            Console.WriteLine($"  工具位姿: X={pkg.tl_cur_pos[0]:F2}mm, Y={pkg.tl_cur_pos[1]:F2}mm, Z={pkg.tl_cur_pos[2]:F2}mm, RX={pkg.tl_cur_pos[3]:F2}°, RY={pkg.tl_cur_pos[4]:F2}°, RZ={pkg.tl_cur_pos[5]:F2}°");
                        }

                    }
                    robot.ServoMoveEnd(0);
                }
                else
                {
                    Console.WriteLine($"GetActualJointPosDegree errcode:{ret}");
                }
            }
        }

        //robot.CloseRPC();

        //public void TestSensitivityCalib()
        //{
        //   int rtn = robot.JointSensitivityEnable(1);
        //    Console.WriteLine($"JointSensitivityEnable rtn is {rtn}");

        //    JointPos curJPos = new JointPos(0, 0, 0, 0, 0, 0);
        //    rtn = robot.GetActualJointPosDegree(0, ref curJPos);
        //    if (rtn != 0)
        //    {
        //        Console.WriteLine("Failed to get actual joint position.");
        //        robot.CloseRPC();
        //        return;
        //    }

        //    ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
        //    DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);

        //    double[] j2Angles = { 0, -30, -60, -90, -120, -150, -180 };

        //    foreach (double j2 in j2Angles)
        //    {
        //        JointPos jointPos = new JointPos(
        //            curJPos.jPos[0], j2, 0, -90, 0.02, curJPos.jPos[5]
        //        );

        //        DescPose descPos = new DescPose(0, 0, 0, 0, 0, 0);
        //        rtn = robot.GetForwardKin( jointPos, ref descPos);
        //        if (rtn != 0)
        //        {
        //            Console.WriteLine($"GetForwardKin failed at J2={j2}.");
        //            continue;
        //        }

        //        rtn = robot.MoveJ( jointPos,  descPos, 0, 0, 100, 100, 100,  epos, -1, 0,  offset_pos);
        //        if (rtn != 0)
        //        {
        //            Console.WriteLine($"MoveJ failed to J2={j2}, rtn={rtn}");
        //            continue;
        //        }
        //        Thread.Sleep(200); 
        //        rtn = robot.JointSensitivityCollect();
        //        Console.WriteLine($"JointSensitivityCollect at J2={j2} rtn is {rtn}");
        //        Thread.Sleep(100);
        //    }

        //    double[] calibResult = new double[6];
        //    //rtn = robot.JointSensitivityCalibration(ref calibResult);
        //    Console.WriteLine($"JointSensitivityCalibration rtn is {rtn}");

        //    rtn = robot.JointSensitivityEnable(0);
        //    Console.WriteLine($"JointSensitivityEnable (disable) rtn is {rtn}");

        //    Console.WriteLine($"Joint Sensor Calib result: " +
        //        $"{calibResult[0]:F6} {calibResult[1]:F6} {calibResult[2]:F6} " +
        //        $"{calibResult[3]:F6} {calibResult[4]:F6} {calibResult[5]:F6}");
        //    robot.CloseRPC();
        //}

        /// <summary>
        /// 测试从站端口错误计数器：读取并清零所有从站的通信错误帧
        /// </summary>

        public void TestLaserTrackMoveC()
        {

            byte[] ctrl = new byte[20];


            //上传并加载开放协议文件
            //robot.OpenLuaUpload("E://openlua/CtrlDev_laser_ruiniu-0117.lua");
            //robot.Sleep(2000);
            //robot.SetCtrlOpenLUAName(0, "CtrlDev_laser_ruiniu-0117.lua");
            //robot.UnloadCtrlOpenLUA(0);
            //robot.LoadCtrlOpenLUA(0);
            //robot.Sleep(8000);

            robot.ResetAllError();
            int cnt = 1;
            while (cnt < 2)
            {
                //运动到需要寻位的起始点
                JointPos startJointPos = new JointPos(40.947, -133.649, 128.497, -108.428, -87.159, -21.741);
                DescPose startDescPose = new DescPose(-167.396, -301.742, 224.468, -157.008, -6.084, 152.043);
                ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
                DescPose offDesc = new DescPose(0, 0, 0, 0, 0, 0);
                new DescTran();

                robot.MoveL(startJointPos, startDescPose, 1, 0, 50, 100, 100, -1, exaxisPos, 0, 0, offDesc, 0, 0, 10);
                Thread.Sleep(2000);

                //沿着-y方向开始寻位
                int ret = robot.LaserTrackingSearchStart_xyz(0, 100, 300, 1000, 2);
                robot.LaserTrackingSearchStop();

                //如果寻位成功
                if (ret == 0)
                {
                    //运动到寻位点
                    robot.MoveToLaserSeamPos(1, 30, 0, 0, 0, offDesc);
                    //开始沿着寻位点进行激光跟踪
                    robot.LaserTrackingTrackOnOff(1, 2);

                    JointPos midJointPos = new JointPos(23.925, -68.391, 72.649, -89.970, -102.641, 102.979);
                    DescPose midDescPose = new DescPose(-561.957, -282.616, 179.994, -166.732, -1.366, 11.262);
                    JointPos endJointPos = new JointPos(-11.146, -110.681, 112.893, -71.999, -83.325, 208.723);
                    DescPose endDescPose = new DescPose(-250.875, -93.272, 182.343, -159.126, 4.036, -130.316);

                    robot.MoveC(midJointPos, midDescPose, 1, 0, 30, 100, exaxisPos, 0, offDesc, endJointPos, endDescPose, 1, 0, 30, 100, exaxisPos, 0, offDesc, 100, -1,100, 0);
                   // robot.Circle(midJointPos, midDescPose, 1, 0, 30, 100, exaxisPos, endJointPos, endDescPose, 1, 0, 30, 100, exaxisPos, 100, 0, offDesc, 100, -1, 0);

                    //停止跟踪
                    robot.LaserTrackingTrackOnOff(0, 2);
                }
                cnt++;
            }
            robot.CloseRPC();
        }
        public void TestLaserRecordAndReplayMoveC()
        {

            byte[] ctrl = new byte[20];

            int cnt = 1;
            while (cnt < 2)
            {
                // 运动到扫描的起点
                JointPos startjointPos1 = new JointPos(-15.647, -119.042, 109.960, -71.222, -73.948, -122.687);
                DescPose startdescPose1 = new DescPose(-274.669, -122.896, 246.972, -161.315, -0.312, -164.381);
                ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
                DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

                robot.MoveJ( startjointPos1,  startdescPose1, 1, 0, 100, 100, 50,  exaxisPos, -1, 0,  offdese);

                // 运动到扫描的起点
                JointPos startjointPos = new JointPos(-23.965, -150.841, 137.707, -89.747, -56.114, -122.685);
                DescPose startdescPose = new DescPose(-274.002, -189.344, 194.938, -157.388, -28.759, -173.209);
                //robot.MoveL( startjointPos,  startdescPose, 1, 0, 10, 100, 100, -1,  exaxisPos, 0, 0,  offdese, 1, 1);
                robot.MoveL(startjointPos, startdescPose, 1, 0, 10, 100, 100, -1, exaxisPos, 0, 0, offdese, 0, 0, 10);
                // 开始轨迹记录
                robot.LaserSensorRecord1(2, 10);

                // 运动到需要记录的终点
                JointPos midjointPos = new JointPos(36.350, -59.819, 63.114, -51.373, -105.011, 98.495);
                DescPose middescPose = new DescPose(-370.608, -294.229, 181.531, -158.073, -39.221, 25.737);
                Console.WriteLine("111111");

                JointPos endjointPos = new JointPos(-26.944, -101.993, 115.794, -72.164, -53.080, 164.700);
                DescPose enddescPose = new DescPose(-353.625, -155.023, 185.415, -151.407, -39.177, -122.813);

                robot.MoveC( midjointPos,  middescPose, 1, 0, 10, 100,  exaxisPos, 0,  offdese,  endjointPos,  enddescPose, 1, 0, 10, 100,  exaxisPos, 0,  offdese, 100, -1,100, 0);
               // robot.Circle( midjointPos,  middescPose, 1, 0, 10, 100,  exaxisPos,  endjointPos,  enddescPose, 1, 0, 10, 100,  exaxisPos, 100, 0,  offdese, 100, -1, 0);
              //  robot.Circle(j3, desc_p3, 3, 0, 100, 100, epos, j2, desc_p2, 3, 0, 100, 100, epos, 10, -1, offset_pos, 100, -1, 0);
                Console.WriteLine("222222");
                // 停止记录
                robot.LaserSensorRecord1(0, 10);
                Console.WriteLine("333333");

                Thread.Sleep(2000);
                // robot.StopMotion();

                JointPos startjointPos2 = new JointPos(-6.592, -140.898, 122.764, -88.529, -81.143, -82.069);
                DescPose startdescPose2 = new DescPose(-251.875, -124.247, 250.719, -168.899, -15.289, 165.278);
                robot.MoveJ( startjointPos2,  startdescPose2, 1, 0, 100, 100, 50,  exaxisPos, -1, 0,  offdese);

                Console.WriteLine("4444444");
                // 运动到记录的焊缝起点
                robot.MoveToLaserRecordStart(1, 30);
                // 开始轨迹复现
                robot.LaserSensorReplay(10, 100);

                robot.MoveLTR();
                // 停止轨迹复现
                robot.LaserSensorRecord1(0, 10);
                cnt++;
            }

            robot.CloseRPC();
        }
        public int TestKernelOTA()
        {

            robot.KernelUpgrade("D://zUP/OTA/update_2024_head.img");

            int result = 0;
            robot.GetKernelUpgradeResult(ref result);
            Console.WriteLine($"OTA result: {result}");

            return result; // 
        }
        public void TestCoordMain5()
        {
            DescTran[] points = new DescTran[10];
            for (int i = 0; i < 10; i++)
            {
                points[i] = new DescTran();
            }

            points[0].x = -3;
            points[0].y = -3;
            points[0].z = 0;

            points[1].x = -6;
            points[1].y = 0;
            points[1].z = 0;

            points[2].x = -3;
            points[2].y = 3;
            points[2].z = 0;

            points[3].x = 0;
            points[3].y = 0;
            points[3].z = 0;
            double[] stayTimes = new double[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
           int rtn = robot.CustomWeaveSetPara(2, 4, points, stayTimes, 1.000, 0, 0);
            Console.WriteLine($"CustomWeaveSetPara rtn is {rtn}");
            System.Threading.Thread.Sleep(1000);
            int pointNum = 0;
            double frequency = 0;
            int incStayType = 0;
            int stationary = 0;
            rtn = robot.CustomWeaveGetPara(2, ref pointNum, ref points, ref stayTimes, ref frequency, ref incStayType, ref stationary);
            Console.WriteLine($"pointNum is {pointNum}");
            for (int i = 0; i < pointNum; i++)
            {
                Console.WriteLine($"point {i}, point x y z {points[i].x:F6} {points[i].y:F6} {points[i].z:F6}");
            }
            Console.WriteLine($"fre is {frequency:F6}, stay is {incStayType} {stationary}");
            robot.WeaveSetPara(0, 9, 1.000000, 1, 5.000000, 6.000000, 5.000000, 50, 100, 100, 0, 1, 0.000000, 0.000000);

            DescPose desc_p1 = new DescPose(441.901, 416.508, -51.979, -179.234, 0.718, -115.305);
            JointPos j1 = new JointPos(-146.22, -60.551, 104.859, -135.317, -90.289, 59.088);

            DescPose desc_p2 = new DescPose(441.901, 615.317, -51.979, -179.234, 0.718, -115.305);
            JointPos j2 = new JointPos(-133.22, -44.193, 74.934, -121.661, -90.509, 72.087);

            //DescPose desc_p1 = new DescPose(-288.650, 367.807, 288.404, 0.000, -0.001, 0.001);
            //DescPose desc_p2 = new DescPose(-431.714, 367.815, 288.415, 0.001, 0.001, 0.000);    
            DescPose desc_p3 = new DescPose(291.781, 682.326, -51.976, -179.234, 0.718, -115.305);
            //JointPos j1 = new JointPos(140.656,  -84.560,  -91.707, -93.734,  90.000,50.655 );
            //JointPos j2 = new JointPos ( 149.873, -98.298, -77.599,  -94.103,  90.000,  59.873 );
            JointPos j3 = new JointPos (-120.770, -45.957, 78.232, -123.063, -90.694, 84.535);
            ExaxisPos epos = new ExaxisPos(0,0,0,0);
            DescPose offset_pos = new DescPose(0,0,0,0,0,0);
            robot.MoveJ(j1, desc_p1, 3, 0, 100, 100, 100, epos, -1, 0, offset_pos);
            robot.WeaveStart(0);
            robot.Circle(j3, desc_p3, 3, 0, 100, 100, epos, j2, desc_p2, 3, 0, 100, 100, epos, 10, -1, offset_pos, 100, -1, 0);
            robot.WeaveEnd(0);
            robot.MoveJ(j1, desc_p1, 3, 0, 100, 100, 100, epos, -1, 0, offset_pos);
            robot.WeaveStart(0);
            robot.MoveC(j3, desc_p3, 3, 0, 100, 100, epos, 0, offset_pos, j2, desc_p2, 3, 0, 100, 100, epos, 0, offset_pos, 10, -1, 100, 0);
            robot.WeaveEnd(0);
            robot.MoveJ(j1, desc_p1, 3, 0, 100, 100, 100, epos, -1, 0, offset_pos);
            robot.WeaveStart(0);
            robot.MoveL(j2, desc_p2, 3, 0, 100, 100, 10, -1, epos, 0, 0, offset_pos, 0, 0, 10);
            robot.WeaveEnd(0);

        }
        public void TestCoordMain4()

        {
            int id = 2;
            for (int i = 0; i < 70; ++i)
            {
                DescPose toolCoord = new DescPose(0, 0, 0, 0, 0, 0);
                DescPose wobjCoord = new DescPose(0, 0, 0, 0, 0, 0);
                DescPose extoolCoord = new DescPose(0, 0, 0, 0, 0, 0);
                DescPose exAxisCoord = new DescPose(0, 0, 0, 0, 0, 0);
                new DescTran();

                //robot.GetCurToolCoord(ref toolCoord);
                //Console.WriteLine($"GetCurToolCoord {toolCoord.tran.x} {toolCoord.tran.y} {toolCoord.tran.z} {toolCoord.rpy.rx} {toolCoord.rpy.ry} {toolCoord.rpy.rz}");

                //robot.GetCurWObjCoord(ref wobjCoord);
                //Console.WriteLine($"GetCurWObjCoord {wobjCoord.tran.x} {wobjCoord.tran.y} {wobjCoord.tran.z} {wobjCoord.rpy.rx} {wobjCoord.rpy.ry} {wobjCoord.rpy.rz}");

                //robot.GetCurExToolCoord(ref extoolCoord);
                //Console.WriteLine($"GetExToolCoordWithID {extoolCoord.tran.x} {extoolCoord.tran.y} {extoolCoord.tran.z} {extoolCoord.rpy.rx} {extoolCoord.rpy.ry} {extoolCoord.rpy.rz}");

                //robot.GetCurExAxisCoord(ref exAxisCoord);
                //Console.WriteLine($"GetCurExAxisCoord {exAxisCoord.tran.x} {exAxisCoord.tran.y} {exAxisCoord.tran.z} {exAxisCoord.rpy.rx} {exAxisCoord.rpy.ry} {exAxisCoord.rpy.rz}");


                //robot.GetTargetPayload(0, ref weightT);
                //robot.GetTargetPayloadCog(0, ref cogT);
                //Console.WriteLine($"GetTargetPayload {weightT} {cogT.x} {cogT.y} {cogT.z}");
                //Thread.Sleep(500);
                // DescPose toolCoord = new DescPose();
                //robot.GetToolCoordWithID(id, ref toolCoord);
                //Console.WriteLine($"GetToolCoordWithID {id}, {toolCoord.tran.x} {toolCoord.tran.y} {toolCoord.tran.z} {toolCoord.rpy.rx} {toolCoord.rpy.ry} {toolCoord.rpy.rz}");

                //// DescPose wobjCoord = new DescPose();
                //robot.GetWObjCoordWithID(id, ref wobjCoord);
                //Console.WriteLine($"GetWObjCoordWithID {id}, {wobjCoord.tran.x} {wobjCoord.tran.y} {wobjCoord.tran.z} {wobjCoord.rpy.rx} {wobjCoord.rpy.ry} {wobjCoord.rpy.rz}");

                //// DescPose extoolCoord = new DescPose();
                //robot.GetExToolCoordWithID(id, ref extoolCoord);
                //Console.WriteLine($"GetExToolCoordWithID {id}, {extoolCoord.tran.x} {extoolCoord.tran.y} {extoolCoord.tran.z} {extoolCoord.rpy.rx} {extoolCoord.rpy.ry} {extoolCoord.rpy.rz}");

                //// DescPose exAxisCoord = new DescPose();
                //robot.GetExAxisCoordWithID(id, ref exAxisCoord);
                //Console.WriteLine($"GetExAxisCoordWithID {id}, {exAxisCoord.tran.x} {exAxisCoord.tran.y} {exAxisCoord.tran.z} {exAxisCoord.rpy.rx} {exAxisCoord.rpy.ry} {exAxisCoord.rpy.rz}");

                double weight = 0.0;
                DescTran cog = new DescTran();
                robot.GetTargetPayloadWithID(id, ref weight, ref cog);
                Console.WriteLine($"GetTargetPayloadWithID {id}, {weight} {cog.x} {cog.y} {cog.z}");
                Thread.Sleep(500);
                Console.WriteLine($"当前次数{i + 1}次");
            }


           }
            public void TestCoordMain3()

        {
            int id = 1;

            double weight = 0.0;
            DescTran cog1 = new DescTran();
            DescPose toolCoord = new DescPose(0, 0, 0, 0, 0, 0);
            DescPose wobjCoord = new DescPose(0, 0, 0, 0, 0, 0);
            DescPose extoolCoord = new DescPose(0, 0, 0, 0, 0, 0);
            DescPose exAxisCoord = new DescPose(0, 0, 0, 0, 0, 0);
            for (int i = 0; i < 50; ++i)
            {
                DescPose Coordset0 = new DescPose(0, 0, 0, 0, 0, 0);
                DescPose Coordset = new DescPose(1, 2, 3, 4, 5, 6);
                DescPose etcp = new DescPose(10, 20, 30, 40, 50, 60);
                DescPose etctool = new DescPose(0.1, 0.2, 0.3, 0.4, 0.5, 0.6);

                DescTran cog = new DescTran(1, 2, 3);
                if (i % 2 == 0)
                {
                    robot.SetToolCoord(id, Coordset, 0, 0, 1, 0);
                    Thread.Sleep(100);
                    robot.SetWObjCoord(id, Coordset, 0);
                    Thread.Sleep(100);
                    robot.ExtAxisActiveECoordSys(id, 1, Coordset, 0);
                    Thread.Sleep(100);
                    robot.SetExToolCoord(id, etcp, etctool);
                    Thread.Sleep(100);
                    robot.SetLoadWeight(id, (float)1.5);
                    //Thread.Sleep(500);
                    robot.SetLoadCoord(id, cog);
                    Thread.Sleep(100);
                }
                else
                {
                    robot.SetToolCoord(id, Coordset0, 0, 0, 1, 0);
                    Thread.Sleep(100);
                    robot.SetWObjCoord(id, Coordset0, 0);
                    Thread.Sleep(100);
                    robot.ExtAxisActiveECoordSys(id, 1, Coordset0, 0);
                    Thread.Sleep(100);
                    robot.SetExToolCoord(id, Coordset0, Coordset0);
                    Thread.Sleep(100);
                    robot.SetLoadWeight(id, 0);
                    //Thread.Sleep(500);
                    robot.SetLoadCoord(id, Coordset0.tran);
                    Thread.Sleep(100);
                }   
                //DescPose toolCoord = new DescPose();
                int _type=0,_install=0,_toolID=0,_loadNo=0,_refFrame=0,_axisCN=0,_calib=0;
                DescPose _tc = new DescPose(0,0,0,0,0,0);
                robot.GetToolCoordWithID(id, ref toolCoord, ref _type, ref _install, ref _toolID, ref _loadNo);
                Console.WriteLine($"GetToolCoordWithID {id}, {toolCoord.tran.x} {toolCoord.tran.y} {toolCoord.tran.z} {toolCoord.rpy.rx} {toolCoord.rpy.ry} {toolCoord.rpy.rz}");

                //DescPose wobjCoord = new DescPose();
                robot.GetWObjCoordWithID(id, ref wobjCoord, ref _refFrame);
                Console.WriteLine($"GetWObjCoordWithID {id}, {wobjCoord.tran.x} {wobjCoord.tran.y} {wobjCoord.tran.z} {wobjCoord.rpy.rx} {wobjCoord.rpy.ry} {wobjCoord.rpy.rz}");

                // DescPose extoolCoord = new DescPose();
                robot.GetExToolCoordWithID(id, ref extoolCoord, ref _tc);
                Console.WriteLine($"GetExToolCoordWithID {id}, {extoolCoord.tran.x} {extoolCoord.tran.y} {extoolCoord.tran.z} {extoolCoord.rpy.rx} {extoolCoord.rpy.ry} {extoolCoord.rpy.rz}");

                // DescPose exAxisCoord = new DescPose();
                robot.GetExAxisCoordWithID(id, ref exAxisCoord, ref _axisCN, ref _calib);
                Console.WriteLine($"GetExAxisCoordWithID {id}, {exAxisCoord.tran.x} {exAxisCoord.tran.y} {exAxisCoord.tran.z} {exAxisCoord.rpy.rx} {exAxisCoord.rpy.ry} {exAxisCoord.rpy.rz}");
          
                robot.GetTargetPayloadWithID(id, ref weight, ref cog);
                Console.WriteLine($"GetTargetPayloadWithID {id}, {weight} {cog1.x} {cog1.y} {cog1.z}");
                Thread.Sleep(500);
                Console.WriteLine($"当前次数{i + 1}次");
         
            }


        }
        public void TestCoordMain2()

        {
            int id = 1;
            //return;
            while (true)
            {

                double weight = 0.0;
                DescTran cog = new DescTran();
                robot.GetTargetPayloadWithID(id, ref weight, ref cog);
                Console.WriteLine($"GetTargetPayloadWithID {id}, {weight} {cog.x} {cog.y} {cog.z}");
                id++;
                if (id > 14)
                {
                    id = 1;
                }
                Thread.Sleep(200);

            }



            //DescPose extoolCoord = new DescPose();

            //DescPose exAxisCoord = new DescPose();
            //robot.GetExAxisCoordWithID(id, ref exAxisCoord);
            //Console.WriteLine($"GetExAxisCoordWithID {id}, {exAxisCoord.tran.x} {exAxisCoord.tran.y} {exAxisCoord.tran.z} {exAxisCoord.rpy.rx} {exAxisCoord.rpy.ry} {exAxisCoord.rpy.rz}");


        }
        public void TestCoordMain1()

        {
            //DescPose w_coord = new DescPose(0, 0, 0, 0, 0, 0);
            //w_coord.tran.x = 110.0;
            //w_coord.tran.y = 12.0;
            //w_coord.tran.z = 13.0;
            //w_coord.rpy.rx = 14.0;
            //w_coord.rpy.ry = 15.0;
            //w_coord.rpy.rz = 16.0;
            //int id = 1;

            //for (int i = 0; i < 10; i++)
            //{


            //    if (i % 2 == 0)
            //    {
            //        w_coord.tran.z = 100.0;
            //    }
            //    else
            //    {
            //        w_coord.tran.z = 300.0;
            //    }
            //    int rtn1 = robot.SetWObjCoord(id, w_coord,0);
            //    Thread.Sleep(1000);



            //}
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            //DescPose coordRtn = new DescPose(0, 0, 0, 0, 0, 0);
            //for (int i = 0; i < 10; i++)
            //{


            //    if (i % 2 == 0)
            //    {
            //        offdese.tran.z = 100.0;
            //        coordRtn.tran.z = 300.0;
            //    }
            //    else
            //    {
            //        offdese.tran.z = 300.0;
            //        coordRtn.tran.z = 100.0;
            //    }
            //    robot.SetExToolCoord(1, coordRtn, offdese);
            //    Thread.Sleep(1000);

            //}
            //DescPose axisCoord = new DescPose();
            //for (int i = 0; i < 10; i++)
            //{


            //    if (i % 2 == 0)
            //    {
            //        axisCoord.tran.z = 100.0;

            //    }
            //    else
            //    {
            //        axisCoord.tran.z = 300.0;

            //    }
            //   robot.ExtAxisActiveECoordSys(1, 1, axisCoord, 1);
            //    Thread.Sleep(1000);

            //}

            new DescTran();

            for (int i = 0; i < 100; i++)
            {


                //    if (i % 2 == 0)
                //    {
                //        loadCoord.x = 10.0f;
                //        loadCoord.z = 30.0f;
                //        robot.SetLoadWeight(1, 0f);
                //    }
                //    else
                //    {
                //        robot.SetExToolCoord(1, coordRtn, offdese);

                //    }
                //    robot.SetLoadCoord(1,loadCoord);
                //    Thread.Sleep(1000);

                //}


            }
        }
            public void TestCoordMain()
            {
                DescPose t_coord = new DescPose(0, 0, 0, 0, 0, 0);
                t_coord.tran.x = 1.0;
                t_coord.tran.y = 2.0;
                t_coord.tran.z = 300.0;
                t_coord.rpy.rx = 4.0;
                t_coord.rpy.ry = 5.0;
                t_coord.rpy.rz = 6.0;
                int id = 1;

                DescPose toolCoord = new DescPose();
                int _t=0,_i=0,_tid=0,_ln=0,_rf=0,_an=0,_cf=0;
                DescPose _tc2 = new DescPose(0,0,0,0,0,0);
                DescPose wobjCoord = new DescPose();
                DescPose extoolCoord = new DescPose();
                DescPose exAxisCoord = new DescPose();
                robot.GetToolCoordWithID(id, ref toolCoord, ref _t, ref _i, ref _tid, ref _ln);
                Console.WriteLine($"GetToolCoordWithID {id}, {toolCoord.tran.x} {toolCoord.tran.y} {toolCoord.tran.z} {toolCoord.rpy.rx} {toolCoord.rpy.ry} {toolCoord.rpy.rz}");

                robot.GetWObjCoordWithID(id, ref wobjCoord, ref _rf);
                Console.WriteLine($"GetWObjCoordWithID {id}, {wobjCoord.tran.x} {wobjCoord.tran.y} {wobjCoord.tran.z} {wobjCoord.rpy.rx} {wobjCoord.rpy.ry} {wobjCoord.rpy.rz}");

                robot.GetExToolCoordWithID(id, ref extoolCoord, ref _tc2);
                Console.WriteLine($"GetExToolCoordWithID {id}, {extoolCoord.tran.x} {extoolCoord.tran.y} {extoolCoord.tran.z} {extoolCoord.rpy.rx} {extoolCoord.rpy.ry} {extoolCoord.rpy.rz}");

                robot.GetExAxisCoordWithID(id, ref exAxisCoord, ref _an, ref _cf);
                Console.WriteLine($"GetExAxisCoordWithID {id}, {exAxisCoord.tran.x} {exAxisCoord.tran.y} {exAxisCoord.tran.z} {exAxisCoord.rpy.rx} {exAxisCoord.rpy.ry} {exAxisCoord.rpy.rz}");

                double weight = 0.0;
                DescTran cog = new DescTran();
                robot.GetTargetPayloadWithID(id, ref weight, ref cog);
                Console.WriteLine($"GetTargetPayloadWithID {id}, {weight} {cog.x} {cog.y} {cog.z}");

                robot.GetCurToolCoord(ref toolCoord);
                Console.WriteLine($"GetCurToolCoord {toolCoord.tran.x} {toolCoord.tran.y} {toolCoord.tran.z} {toolCoord.rpy.rx} {toolCoord.rpy.ry} {toolCoord.rpy.rz}");

                robot.GetCurWObjCoord(ref wobjCoord);
                Console.WriteLine($"GetCurWObjCoord {wobjCoord.tran.x} {wobjCoord.tran.y} {wobjCoord.tran.z} {wobjCoord.rpy.rx} {wobjCoord.rpy.ry} {wobjCoord.rpy.rz}");

                robot.GetCurExToolCoord(ref extoolCoord);
                Console.WriteLine($"GetExToolCoordWithID {extoolCoord.tran.x} {extoolCoord.tran.y} {extoolCoord.tran.z} {extoolCoord.rpy.rx} {extoolCoord.rpy.ry} {extoolCoord.rpy.rz}");

                robot.GetCurExAxisCoord(ref exAxisCoord);
                Console.WriteLine($"GetCurExAxisCoord {exAxisCoord.tran.x} {exAxisCoord.tran.y} {exAxisCoord.tran.z} {exAxisCoord.rpy.rx} {exAxisCoord.rpy.ry} {exAxisCoord.rpy.rz}");
                double weightT = 0.0f;
                DescTran cogT = new DescTran();
                robot.GetTargetPayload(0, ref weightT);
                robot.GetTargetPayloadCog(0, ref cogT);
                Console.WriteLine($"GetTargetPayload {weightT} {cogT.x} {cogT.y} {cogT.z}");
                DescPose coordSet = new DescPose(0, 10, 2, 3, 4, 5);
                robot.SetToolCoord(2, coordSet, 0, 0, 1, 0);

                DescPose Coordset0 = new DescPose(0, 0, 0, 0, 0, 0);
                DescPose Coordset = new DescPose(1, 2, 3, 4, 5, 6);
                DescPose etcp = new DescPose(10, 20, 30, 40, 50, 60);
                DescPose etctool = new DescPose(0.1, 0.2, 0.3, 0.4, 0.5, 0.6);
   
                robot.SetToolCoord(id, Coordset, 0, 0, 1, 0);
                Thread.Sleep(100);
                robot.SetWObjCoord(id, Coordset, 0);
                Thread.Sleep(100);
                robot.ExtAxisActiveECoordSys(id, 1, Coordset, 0);
                Thread.Sleep(100);
                robot.SetExToolCoord(id, etcp, etctool);
                Thread.Sleep(100);
                robot.SetLoadWeight(id, (float)1.5);
                //Thread.Sleep(500);
                robot.SetLoadCoord(id, cog);
                Thread.Sleep(100);

        }
        public void TestImpedanceControl()
        {
            int[] ctrl = new int[20];
            int rtn;
            JointPos j1 = new JointPos(102.622, -135.990, 120.769, -73.950, -90.848, 35.507);
            JointPos j2 = new JointPos(93.674, -80.062, 82.947, -92.199, -90.967, 26.559);
            DescPose desc_pos1 = new DescPose(136.552, -149.799, 449.532, 179.817, -1.172, 157.123);
            DescPose desc_pos2 = new DescPose(136.540, -561.048, 449.542, 179.819, -1.172, 157.122);

            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);

            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 200.0f;
            float ovl = 100.0f;
            float blendR = -1.0f;

            byte flag = 0;

            byte search = 0;
            robot.SetSpeed(20);
            int company = 22;
            int device = 0;
            int softversion = 0;
            int bus = 1;
            robot.FT_SetConfig(company, device, softversion, bus);
            Thread.Sleep(1000);
            robot.FT_GetConfig(ref company, ref device, ref softversion, ref bus);
            Console.WriteLine($"FT config:{company},{device},{softversion},{bus}");
            Thread.Sleep(1000);

            robot.FT_Activate(0);
            Thread.Sleep(1000);
            robot.FT_Activate(1);
            Thread.Sleep(1000);

            Thread.Sleep(1000);
            robot.FT_SetZero(0);
            Thread.Sleep(1000);
            robot.FT_SetZero(1);
            Thread.Sleep(1000);

            double[] forceThreshold = new double[] { 30, 30, 30, 5, 5, 5 };
            double[] m = new double[] { 0.1, 0.1, 0.1, 0.02, 0.02, 0.02 };
            double[] b = new double[] { 1, 1, 1, 0.08, 0.08, 0.08 };
            double[] k = new double[] { 0, 0, 0, 0, 0, 0 };

            rtn = robot.ImpedanceControlStartStop(1, 1, forceThreshold, m, b, k, 1000, 500, 100, 100);
            Console.WriteLine($"ImpedanceControlStartStop errcode:{rtn}");
            rtn = robot.MoveL(desc_pos1, tool, user, vel, acc, ovl, blendR, 0, epos, search, flag, offset_pos, -1, 0);
            rtn = robot.MoveL(desc_pos2, tool, user, vel, acc, ovl, blendR, 0, epos, search, flag, offset_pos, -1, 0);
            rtn = robot.MoveL(desc_pos1, tool, user, vel, acc, ovl, blendR, 0, epos, search, flag, offset_pos, -1, 0);
            rtn = robot.MoveL(desc_pos2, tool, user, vel, acc, ovl, blendR, 0, epos, search, flag, offset_pos, -1, 0);
            rtn = robot.MoveL(desc_pos1, tool, user, vel, acc, ovl, blendR, 0, epos, search, flag, offset_pos, -1, 0);
            rtn = robot.MoveL(desc_pos2, tool, user, vel, acc, ovl, blendR, 0, epos, search, flag, offset_pos, -1, 0);

            Console.WriteLine($"movel errcode:{rtn}");
            robot.ImpedanceControlStartStop(0, 1, forceThreshold, m, b, k, 1000, 500, 100, 100);
        }
        void testLaserConfig()
        {
            int[] ctrl = new int[20];
            robot.LaserTrackingSensorConfig("192.168.58.120", 502);
            robot.LaserTrackingSensorSamplePeriod(20);
            robot.LoadPosSensorDriver(103);
            robot.LaserTrackingLaserOnOff(0, 0);

            System.Threading.Thread.Sleep(3000);

            robot.LaserTrackingLaserOnOff(1, 0);
        }

        void testGetLaserPoint()
        {
            int[] ctrl = new int[20];
            string name = "laserPoint";
            double[] data = new double[20];
            robot.GetRobotTeachingPoint(name, ref data);
            Console.WriteLine("GetRobotTeachingPoint :{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}",
                data[0], data[1], data[2], data[3], data[4], data[5],
                data[6], data[7], data[8], data[9], data[10], data[11]);
            JointPos startjointPos = new JointPos(data[6], data[7], data[8], data[9], data[10], data[11]);
            DescPose startdescPose = new DescPose(data[0], data[1], data[2], data[3], data[4], data[5]);
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.MoveL(startjointPos, startdescPose, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0);
        }

        void testMoveToLaserRecordStart()
        {
            int[] ctrl = new int[20];



            JointPos startjointPos = new JointPos(56.205, -117.951, 141.872, -118.149, -94.217, -122.176);
            DescPose startdescPose = new DescPose(-97.552, -282.855, 26.675, 174.182, -1.338, -91.707);
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.MoveL(startjointPos, startdescPose, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0);
            robot.LaserSensorRecord1(2, 10);

            JointPos endjointPos = new JointPos(68.809, -87.100, 121.120, -127.233, -95.038, -109.555);
            DescPose enddescPose = new DescPose(-103.555, -464.234, 13.076, 174.179, -1.344, -91.709);
            robot.MoveL(endjointPos, enddescPose, 1, 0, 50, 100, 100, -1, exaxisPos, 0, 0, offdese, 0);

            robot.LaserSensorRecord1(0, 10);
            robot.MoveToLaserRecordStart(1, 30);
        }

        void testMoveToLaserRecordEnd()
        {
            int[] ctrl = new int[20];



            JointPos startjointPos = new JointPos(56.205, -117.951, 141.872, -118.149, -94.217, -122.176);
            DescPose startdescPose = new DescPose(-97.552, -282.855, 26.675, 174.182, -1.338, -91.707);
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.MoveL(startjointPos, startdescPose, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0);
            robot.LaserSensorRecord1(2, 10);

            JointPos endjointPos = new JointPos(68.809, -87.100, 121.120, -127.233, -95.038, -109.555);
            DescPose enddescPose = new DescPose(-103.555, -464.234, 13.076, 174.179, -1.344, -91.709);
            robot.MoveL(endjointPos, enddescPose, 1, 0, 50, 100, 100, -1, exaxisPos, 0, 0, offdese, 0);

            robot.LaserSensorRecord1(0, 10);
            robot.MoveToLaserRecordEnd(1, 30);
        }

        void testLasertrack_xyz()
        {
            int[] ctrl = new int[20];



            JointPos startjointPos = new JointPos(56.205, -117.951, 141.872, -118.149, -94.217, -122.176);
            DescPose startdescPose = new DescPose(-97.552, -282.855, 26.675, 174.182, -1.338, -91.707);
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            new DescTran();

            robot.MoveL(startjointPos, startdescPose, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0);
            robot.LaserTrackingSearchStart_xyz(3, 100, 300, 1000, 3);
            robot.LaserTrackingSearchStop();
            robot.MoveToLaserSeamPos(1, 30, 0, 0, 0, offdese);
        }

        void testLasertrack_point()
        {
            int[] ctrl = new int[20];

            string name = "laserEnd";
            double[] data = new double[20];



            JointPos startjointPos = new JointPos(56.205, -117.951, 141.872, -118.149, -94.217, -122.176);
            DescPose startdescPose = new DescPose(-97.552, -282.855, 26.675, 174.182, -1.338, -91.707);
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            DescTran directionPoint = new DescTran();

            robot.MoveL(startjointPos, startdescPose, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0);
            robot.GetRobotTeachingPoint(name, ref data);

            Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}",
                data[0], data[1], data[2], data[3], data[4], data[5],
                data[6], data[7], data[8], data[9], data[10], data[11]);

            directionPoint.x = data[0];
            directionPoint.y = data[1];
            directionPoint.z = data[2];

            Console.WriteLine("{0}, {1}, {2}", directionPoint.x, directionPoint.y, directionPoint.z);

            robot.LaserTrackingSearchStart_point(directionPoint, 100, 500, 1000, 3);
            robot.LaserTrackingSearchStop();
            robot.MoveToLaserSeamPos(1, 30, 0, 0, 0, offdese);
        }

        void testLaserRecordAndReplay()
        {
            int[] ctrl = new int[20];
            //robot.OpenLuaUpload("D://zUP/CtrlDev_laser_ruiniu-0117.lua");
            //System.Threading.Thread.Sleep(2000);
            //robot.SetCtrlOpenLUAName(0, "CtrlDev_laser_ruiniu-0117.lua");
            //robot.UnloadCtrlOpenLUA(0);
            //robot.LoadCtrlOpenLUA(0);
            //System.Threading.Thread.Sleep(8000);
            for (int i = 0; i < 10; ++i)
            {
                JointPos startjointPos = new JointPos(58.830, -92.757, 86.939, -81.135, -90.548, 26.358);
                DescPose startdescPose = new DescPose(-74.319, -312.541, 39.168, 177.512, -1.843, 122.527);
                ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
                DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

                robot.MoveL(startjointPos, startdescPose, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0);
                robot.LaserSensorRecord1(2, 10);

                JointPos endjointPos = new JointPos(76.229, -78.219, 71.540, -82.615, -88.277, 42.332);
                DescPose enddescPose = new DescPose(17.298, -408.461, 40.967, 178.317, 0.798, 123.875);
                robot.MoveL(endjointPos, enddescPose, 1, 0, 50, 100, 100, -1, exaxisPos, 0, 0, offdese, 0);

                robot.LaserSensorRecord1(0, 10);
                robot.MoveToLaserRecordStart(1, 30);
                robot.LaserSensorReplay(10, 100);
                robot.MoveLTR();
                robot.LaserSensorRecord1(0, 10);
                Console.WriteLine($"完成次数 : {i + 1} 次");
            }

        }

        void testLasertrack()
        {
            int[] ctrl = new int[20];
            //robot.OpenLuaUpload("D://zUP/CtrlDev_laser_ruiniu-0117.lua");
            //System.Threading.Thread.Sleep(2000);
            //robot.SetCtrlOpenLUAName(0, "CtrlDev_laser_ruiniu-0117.lua");
            //robot.UnloadCtrlOpenLUA(0);
            //robot.LoadCtrlOpenLUA(0);
            //System.Threading.Thread.Sleep(8000);
            for (int i = 0; i < 1; ++i)
            {

                JointPos startjointPos = new JointPos(58.830, -92.757, 86.939, -81.135, -90.548, 26.358);
                DescPose startdescPose = new DescPose(-74.319, -312.541, 39.168, 177.512, -1.843, 122.527);
                ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
                DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
                new DescTran();

                robot.MoveL(startjointPos, startdescPose, 1, 0, 20, 100, 100, -1, exaxisPos, 0, 0, offdese, 0);

                robot.LaserTrackingSearchStart_xyz(0, 10, 300, 10000, 2);
                robot.LaserTrackingSearchStop();
                robot.MoveToLaserSeamPos(1, 30, 0, 0, 0, offdese);

                robot.LaserTrackingTrackOnOff(1, 2);


                JointPos endjointPos = new JointPos(76.229, -78.219, 71.540, -82.615, -88.277, 42.332);
                DescPose enddescPose = new DescPose(17.298, -408.461, 40.967, 178.317, 0.798, 123.875);
                robot.MoveL(endjointPos, enddescPose, 1, 0, 5, 100, 100, -1, exaxisPos, 0, 0, offdese, 0);

                robot.LaserTrackingTrackOnOff(0, 2);
                Console.WriteLine($"完成次数 : {i + 1} 次");
            }
        }

        void testTPDmove()
        {
            string name = "tpd2025";
            int type = 1;
            int period_ms = 4;
            int rtn = 0;
            UInt16 di_choose = 0;
            UInt16 do_choose = 0;

            robot.SetTPDParam(type, name, period_ms, di_choose, do_choose);

            robot.Mode(1);
            Thread.Sleep(3000);
            robot.DragTeachSwitch(1);
            robot.SetTPDStart(type, name, period_ms, di_choose, do_choose);
            Thread.Sleep(3000);
            robot.SetWebTPDStop();
            robot.DragTeachSwitch(0);

            Thread.Sleep(1000);
            float ovl = 100.0f;
            byte blend = 0;
            DescPose start_pose = new DescPose();
            rtn = robot.LoadTPD(name);
            Console.WriteLine($"LoadTPD rtn is:{rtn}\n");

            robot.GetTPDStartPose(name, ref start_pose);
            Console.WriteLine($"start pose, xyz is: %f %f %f. rpy is: {start_pose.tran.x},{start_pose.tran.y}, {start_pose.tran.z}, {start_pose.rpy.rx}, {start_pose.rpy.ry}, {start_pose.rpy.rz}");

            rtn = robot.MoveToTPDStart(name, 0, 100.0);

            rtn = robot.MoveTPD(name, blend, ovl);
            Thread.Sleep(5000*5);

            robot.SetTPDDelete(name);
        }

        void testAxleGenCom()
        {
            int[] led_on = new int[6] { 0xAB, 0xBA, 0x12, 0x01, 0x01, 0x79 };
            int[] led_off = new int[6] { 0xAB, 0xBA, 0x12, 0x01, 0x00, 0x78 };
            int[] version = new int[5]{ 0xAB, 0xBA, 0x11, 0x00, 0x76 };
            int[] state = new int[6] { 0xAB, 0xBA, 0x1B,0x01, 0xAA, 0x2B };
            int[] cycleState = new int[6] { 0xAB, 0xBA, 0x12, 0x01, 0x00, 0x78 };

            int[] rcvdata = new int[16];
            int ret = 0;
            int cnt = 1;

            JointPos p1Joint = new JointPos(88.708, -86.178, 140.989, -141.825, -89.162, -49.879);
            DescPose p1Desc = new DescPose(188.007, -377.850, 260.207, 178.715, 2.823, -131.466);

            JointPos p2Joint = new JointPos(112.131, -75.554, 126.989, -139.027, -88.044, -26.477);
            DescPose p2Desc = new DescPose(368.003, -377.848, 260.211, 178.715, 2.823, -131.465);

            //JointPos p1Joint = new JointPos(100.616, -81.541, 135.364, -141.072, -88.565, -37.984);
            //DescPose p1Desc = new DescPose(271.904, -377.763, 260.153, 178.714, 2.823, -131.466);

            //JointPos p2Joint = new JointPos(104.460, -79.729, 132.954, -140.578, -88.383, -34.144);
            //DescPose p2Desc = new DescPose(301.885, -377.759, 260.165, 178.716, 2.822, -131.465);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            //开启末端透传功能
            robot.SetAxleGenComEnable(1);
            robot.SetAxleLuaEnable(1);

            while(cnt<=1300)
            { 
                //读取版本号
                ret = robot.SndRcvAxleGenComCmdData(5, version, 10, ref rcvdata);
                Console.WriteLine($" hard version : {rcvdata[4]},hard code:{rcvdata[5]}, soft version:{rcvdata[6]} {rcvdata[7]}, soft code:{rcvdata[8]}");
                if (ret != 0)
                {
                    break;
                }
                Thread.Sleep(1000);
                //读取艾灸头在位状态
                ret = robot.SndRcvAxleGenComCmdData(6, state, 6, ref rcvdata);
                Console.WriteLine($" state : {rcvdata[4]}");
                Thread.Sleep(1000);


                ////开启艾灸头激光
                //ret = robot.SndRcvAxleGenComCmdData(6, led_on, 6, ref rcvdata);
                //Console.WriteLine($"led on rcv data is: {rcvdata[0]},{rcvdata[1]}, {rcvdata[2]}, {rcvdata[3]}, {rcvdata[4]}, {rcvdata[5]}");
                //robot.MoveJ(p1Joint, p1Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
                //Thread.Sleep(4000);
                ////关闭艾灸头激光
                //ret = robot.SndRcvAxleGenComCmdData(6, led_off, 6, ref rcvdata);
                //Console.WriteLine($"led off rcv data is: {rcvdata[0]},{rcvdata[1]}, {rcvdata[2]}, {rcvdata[3]}, {rcvdata[4]}, {rcvdata[5]}");
                //robot.MoveJ(p2Joint, p2Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
                //Thread.Sleep(1000);
                Console.WriteLine($"***********************complate No. {cnt}  SDK test*****************************");
                cnt++;
            }

        }

       void TestRobotStopOnComDisc()
        {
            int rtn = 0;

            // 设置四个端口的参数
            rtn = robot.SetRobotStopOnComDisc(0, true, 330);
            rtn = robot.SetRobotStopOnComDisc(1, true, 550);
            rtn = robot.SetRobotStopOnComDisc(2, true, 110);
            rtn = robot.SetRobotStopOnComDisc(3, true, 220);
            Console.WriteLine($"SetRobotStopOnComDisc {rtn}");

            bool enable = false;
            int confirmTime = 0;

            // 获取并打印每个端口的设置
            robot.GetRobotStopOnComDisc(0, ref enable, ref confirmTime);
            Console.WriteLine($"GetRobotStopOnComDisc 8080 rtn {rtn}; enable is {(enable ? 1 : 0)}; confirm time is {confirmTime}");

            robot.GetRobotStopOnComDisc(1, ref enable, ref confirmTime);
            Console.WriteLine($"GetRobotStopOnComDisc 8083 rtn {rtn}; enable is {(enable ? 1 : 0)}; confirm time is {confirmTime}");

            robot.GetRobotStopOnComDisc(2, ref enable, ref confirmTime);
            Console.WriteLine($"GetRobotStopOnComDisc 20002 rtn {rtn}; enable is {(enable ? 1 : 0)}; confirm time is {confirmTime}");

            robot.GetRobotStopOnComDisc(3, ref enable, ref confirmTime);
            Console.WriteLine($"GetRobotStopOnComDisc 20004 rtn {rtn}; enable is {(enable ? 1 : 0)}; confirm time is {confirmTime}");

        }

        void TestRobotUDP()
        {
            robot.OnUdpFrameReceived += (comType, frameCount, frameCmdID, contentLen, content) =>
            {
                Console.WriteLine($"[UDP响应] comType={comType}, count={frameCount}, cmdID={frameCmdID}, content={content}");
            };


            //发送帧
            string frameToSend = "/f/bIII52III236III7IIIMode(1)III/b/f";
            robot.SendUDPFrame(frameToSend);
            Thread.Sleep(2000);
            frameToSend = "/f/bIII52III236III7IIIMode(0)III/b/f";
            robot.SendUDPFrame(frameToSend);
            Thread.Sleep(2000);
            frameToSend = "/f/bIII53III201III152IIIMoveJ(89.859,-67.507,83.574,-17.962,-2.166,-0.134,199.968,-542.109,333.659,90.072,2.027,92.026,0,0,100,100,100,0.000,0.000,0.000,0.000,-1,0,0,0,0,0,0,0)III/b/f";
            robot.SendUDPFrame(frameToSend);
            Thread.Sleep(2000);
            frameToSend = "/f/bIII54III201III150IIIMoveJ(89.859,-68.730,57.933,8.903,-2.166,-0.134,199.970,-542.108,520.221,90.072,2.026,92.026,0,0,100,100,100,0.000,0.000,0.000,0.000,-1,0,0,0,0,0,0,0)III/b/f";
            robot.SendUDPFrame(frameToSend);
            Thread.Sleep(2000);
            frameToSend = "/f/bIII47III400III15IIIGetMCVersion(1)III/b/f/f/bIII48III424III21IIIGetSlaveFirmVersion()III/b/f";
            robot.SendUDPFrame(frameToSend);
            Thread.Sleep(2000);

        }

        public void TestIOConfig()
        {
            int rtn = 0;

            // ---------- 测试可配置CI端口功能 ----------
            int[] setDIConfig = new int[] { 3, 9, 1, 4, 5, 6, 7, 8 };
            rtn = robot.SetDIConfig(setDIConfig);
            Console.WriteLine($"SetDIConfig rtn is {rtn}");

            // 使用 out 参数接收获取到的配置数组
            int[] getDIConfig;
            rtn = robot.GetDIConfig(out getDIConfig);  
            Console.WriteLine($"GetDIConfig rtn is {rtn}, value is {string.Join(" ", getDIConfig)}");

            // ---------- 测试可配置CO端口功能 ----------
            int[] setDOConfig = new int[] { 9, 10, 11, 12, 13, 14, 15, 16 };
            rtn = robot.SetDOConfig(setDOConfig);
            Console.WriteLine($"SetDOConfig rtn is {rtn}");

            int[] getDOConfig;
            rtn = robot.GetDOConfig(out getDOConfig);
            Console.WriteLine($"GetDOConfig rtn is {rtn}, value is {string.Join(" ", getDOConfig)}");

            // ---------- 测试末端可配置CI端口功能 ----------
            int[] setToolDIConfig = new int[] { 17, 18 };
            rtn = robot.SetToolDIConfig(setToolDIConfig);
            Console.WriteLine($"SetToolDIConfig rtn is {rtn}");

            int[] getToolDIConfig;
            rtn = robot.GetToolDIConfig(out getToolDIConfig);
            Console.WriteLine($"GetToolDIConfig rtn is {rtn}, value is {string.Join(" ", getToolDIConfig)}");

            // ---------- 测试控制箱可配置CI有效状态 ----------
            int[] setDIConfigLevel = new int[] { 1, 1, 1, 1, 0, 0, 0, 0 };
            rtn = robot.SetDIConfigLevel(setDIConfigLevel);
            Console.WriteLine($"SetDIConfigLevel rtn is {rtn}");

            int[] getDIConfigLevel;
            rtn = robot.GetDIConfigLevel(out getDIConfigLevel);
            Console.WriteLine($"GetDIConfigLevel rtn is {rtn}, value is {string.Join(" ", getDIConfigLevel)}");

            // ---------- 测试控制箱可配置CO有效状态 ----------
            int[] setDOConfigLevel = new int[] { 0, 0, 0, 0, 1, 1, 1, 1 };
            rtn = robot.SetDIConfigLevel(setDOConfigLevel);
            Console.WriteLine($"SetDOConfigLevel rtn is {rtn}");

            int[] getDOConfigLevel;
            rtn = robot.GetDOConfigLevel(out getDOConfigLevel);
            Console.WriteLine($"GetDOConfigLevel rtn is {rtn}, value is {string.Join(" ", getDOConfigLevel)}");

            // ---------- 测试末端可配置CI有效状态 ----------
            int[] setToolDIConfigLevel = new int[] { 1, 0 };
            rtn = robot.SetToolDIConfigLevel(setToolDIConfigLevel);
            Console.WriteLine($"SetToolDIConfigLevel rtn is {rtn}");

            int[] getToolDIConfigLevel;
            rtn = robot.GetToolDIConfigLevel(out getToolDIConfigLevel);
            Console.WriteLine($"GetToolDIConfigLevel rtn is {rtn}, value is {string.Join(" ", getToolDIConfigLevel)}");

            // ---------- 测试控制箱标准DI有效状态 ----------
            int[] setStandardDILevel = new int[] { 1, 1, 1, 1, 0, 0, 0, 0 };
            rtn = robot.SetStandardDILevel(setStandardDILevel);
            Console.WriteLine($"SetStandardDILevel rtn is {rtn}");

            int[] getStandardDILevel;
            rtn = robot.GetStandardDILevel(out getStandardDILevel);
            Console.WriteLine($"GetStandardDILevel rtn is {rtn}, value is {string.Join(" ", getStandardDILevel)}");

            // ---------- 测试控制箱标准DO有效状态 ----------
            int[] setStandardDOLevel = new int[] { 0, 0, 0, 0, 1, 1, 1, 1 };
            rtn = robot.SetStandardDOLevel(setStandardDOLevel);
            Console.WriteLine($"SetStandardDOLevel rtn is {rtn}");

            int[] getStandardDOLevel;
            rtn = robot.GetStandardDOLevel(out getStandardDOLevel);
            Console.WriteLine($"GetStandardDOLevel rtn is {rtn}, value is {string.Join(" ", getStandardDOLevel)}");

        }

        void TestImpedanceControl1()
        {
            JointPos j1 = new JointPos(102.622, -135.990, 120.769, -73.950, -90.848, 35.507);
            JointPos j2 = new JointPos(93.674, -80.062, 82.947, -92.199, -90.967, 26.559);

            DescPose desc_pos1 = new DescPose(136.552, -149.799, 449.532, 179.817, -1.172, 157.123);
            DescPose desc_pos2 = new DescPose(136.540, -561.048, 449.542, 179.819, -1.172, 157.122);

            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            ExaxisPos epos =new ExaxisPos(0, 0, 0, 0);

            int tool = 0;
            int user = 0;
            float vel = 100.0f;
            float acc = 200.0f;
            float ovl = 100.0f;

            robot.SetSpeed(20);

            double[] forceThreshold = new double[6] { 10, 10, 10, 1, 1, 1 };
            double[] m = new double[6] { 0.04, 0.04, 0.04, 0.01, 0.01, 0.01 };
            double[] b = new double[6] { 0.1, 0.1, 0.1, 0.08, 0.08, 0.08 };
            double[] k = new double[6] { 0, 0, 0, 0, 0, 0 };
            int rtn = 0;
            rtn = robot.ImpedanceControlStartStop(1, 0, forceThreshold, m, b, k, 50, 50, 100, 100);
            //printf("ImpedanceControlStartStop errcode:%d\n", rtn);
            robot.MoveJ(j1, tool, user, vel, acc, ovl, epos, -1, 0, offset_pos);
            robot.MoveJ(j2, tool, user, vel, acc, ovl, epos, -1, 0, offset_pos);
            robot.MoveJ(j1, tool, user, vel, acc, ovl, epos, -1, 0, offset_pos);
            robot.MoveJ(j2, tool, user, vel, acc, ovl, epos, -1, 0, offset_pos);

            //printf("moveJ errcode:%d\n", rtn);

            robot.ImpedanceControlStartStop(0, 0, forceThreshold, m, b, k, 50, 50, 100, 100);
        }

        public void TestLaserTrackAndExitAxis()
        {

            ExaxisPos startexaxisPos = new ExaxisPos(35, -60, 0, 0);
            ExaxisPos seamexaxisPos = new ExaxisPos(35, -60, 0, 0);
            ExaxisPos endexaxisPos = new ExaxisPos(25, -60, 0, 0);


            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            
            JointPos startjointPos = new JointPos(51.042, -93.542, 92.526, -86.288, -87.303, 62.104);
            DescPose startdescPose = new DescPose(-98.292, -313.725, 19.808, 176.356, -1.122, 78.910);

            for (int i = 0; i < 1; ++i)
            {
                robot.ExtAxisSyncMoveJ(startjointPos, startdescPose, 1, 0, 10, 100, 100, startexaxisPos, -1, 0, offdese);

                Console.WriteLine("11111");


                int ret = robot.LaserTrackingSearchStart_xyz(0, 100, 300, 1000, 2);
                robot.LaserTrackingSearchStop();

                Console.WriteLine("2222");


                int tool = 0;
                int user = 0;
                JointPos seamjointPos = new JointPos();
                DescPose seamdescPose = new DescPose();

                robot.GetLaserSeamPos(0, offdese, ref seamjointPos, ref seamdescPose, ref tool, ref user, ref startexaxisPos);

                Console.WriteLine($"{seamjointPos.jPos[0]}, {seamjointPos.jPos[1]}, {seamjointPos.jPos[2]}, " +
                                    $"{seamjointPos.jPos[3]}, {seamjointPos.jPos[4]}, {seamjointPos.jPos[5]}, " +
                                    $"{seamdescPose.tran.x}, {seamdescPose.tran.y}, {seamdescPose.tran.z}, " +
                                    $"{seamdescPose.rpy.rx}, {seamdescPose.rpy.ry}, {seamdescPose.rpy.rz}");


                if (ret == 0)
                {

                    robot.ExtAxisSyncMoveJ(seamjointPos, seamdescPose, 1, 0, 10, 10, 100, seamexaxisPos, -1, 0, offdese);


                    //Console.WriteLine("3333");
                    //robot.LaserTrackingTrackOnOff(1, 2);
                    
                    JointPos endjointPos = new JointPos(75.061, -74.543, 72.150, -87.550, -86.097, 83.669);
                    DescPose enddescPose = new DescPose(17.614, -435.270, 20.720, 176.114, 0.373, 81.377);

                    robot.ExtAxisSyncMoveL(endjointPos, enddescPose, 1, 0, 50, 5, 100, -1, endexaxisPos, 0, offdese);

                    // 停止跟踪
                    robot.LaserTrackingTrackOnOff(0, 2);

                }
                Console.WriteLine($"完成次数 : {i + 1} 次");
            }

        }

        public int TestSetVelReducePara()
        {

            int rtn = 0;
            JointPos j1 = new JointPos(10.220, -11.121, -118.086, -46.739, 82.036, 131.503);
            JointPos j2 = new JointPos(89.782, -11.122, -118.086, -46.740, 82.036, 131.504);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            double[] maxJointVel = new double[] { 100.0, 100.0, 100.0, 100.0, 100.0, 100.0 };

            robot.SetSpeed(20);
            rtn = robot.SetVelReducePara(0, 200, 0, maxJointVel);
            robot.MoveJ(j2, 1, 2, 100, 100, 100, epos, -1, 0, offset_pos);

            // 第一次
            rtn = robot.SetVelReducePara(2, 200, 0, maxJointVel);
            Console.WriteLine($"SetVelReduceParaA param error rtn is {rtn}");
            robot.MoveJ(j1, 1, 2, 100, 100, 100, epos, -1, 0, offset_pos);
            robot.MoveJ(j2, 1, 2, 100, 100, 100, epos, -1, 0, offset_pos);

            // 第二次
            maxJointVel = new double[] { 20.0, 20.0, 20.0, 20.0, 20.0, 20.0 };
            rtn = robot.SetVelReducePara(2, 200, 0, maxJointVel);
            Console.WriteLine($"SetVelReduceParaB reduce vel rtn is {rtn}");
            robot.MoveJ(j1, 1, 2, 100, 100, 100, epos, -1, 0, offset_pos);
            robot.MoveJ(j2, 1, 2, 100, 100, 100, epos, -1, 0, offset_pos);
            return 0;
            //int rtn = 0;
            //JointPos j1 = new JointPos(0, -90, 90, 0, 0, 0);
            //JointPos j2 = new JointPos(90, -90, 90, 0, 0, 0);
            //ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);


            //robot.SetSpeed(20);

            //// 测试参数错误
            //rtn = robot.SetVelReducePara(2, 30, 1);
            //Console.WriteLine($"SetVelReducePara param error rtn is {rtn}");

            //// 禁用减速
            //rtn = robot.SetVelReducePara(0, 30, 1);
            //Console.WriteLine($"SetVelReducePara disable reduce vel rtn is {rtn}");
            //robot.MoveJ(j1, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);
            //robot.MoveJ(j2, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);

            //// 启用减速（手动模式）
            //rtn = robot.SetVelReducePara(1, 30, 1);
            //Console.WriteLine($"SetVelReducePara reduce vel rtn is {rtn}");
            //robot.MoveJ(j1, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);
            //robot.MoveJ(j2, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);



            //// 所有模式启用，策略为停止报警并去使能
            //rtn = robot.SetVelReducePara(2, 30, 2);
            //Console.WriteLine($"SetVelReducePara disable robot rtn is {rtn}");
            //robot.MoveJ(j1, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);
            //robot.MoveJ(j2, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);

            //Thread.Sleep(2000);
            //robot.ResetAllError();
            //robot.RobotEnable(1);
            //Thread.Sleep(1000);

            //maxJointVel = new double[] { 100.0, 100.0, 100.0, 100.0, 100.0, 100.0 };
            //// 所有模式启用，策略为停止报警（正常参数）
            //rtn = robot.SetVelReducePara(2, 30, 0);
            //Console.WriteLine($"SetVelReducePara report error rtn is {rtn}");
            //robot.MoveJ(j1, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);
            //robot.MoveJ(j2, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);

            //Thread.Sleep(1000);
            //return 0;
        }
        void TestOriginPointWeave()
        {
            // 创建关节位置对象
            JointPos j = new JointPos(39.886, -98.580, -124.032, -47.393, 90.000, 40.842);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);

            // 参考点坐标
            DescPose refPoint = new DescPose(400.021, 300.022, 299.996, 179.997, -0.003, -90.956);

            //// 第一次运动
            robot.MoveJ(j, 1, 0, 100, 100, 100, epos, -1, 0, offset_pos);

            // 启动定点摆动（模式0）
            robot.OriginPointWeaveStart(0, 0, refPoint, 3);
            robot.MoveStationary();   // 执行固定运动
            robot.OriginPointWeaveEnd();

            Thread.Sleep(2000);         // 等待2秒

            // 第二次运动
            robot.MoveJ(j, 1, 0, 100, 100, 100, epos, -1, 0, offset_pos);

            // 启动定点摆动（模式1）
            robot.OriginPointWeaveStart(0, 1, refPoint, 3);
            robot.MoveStationary();
            robot.OriginPointWeaveEnd();

        }

        void TestOriginPointWeave2()
        {
            // 创建关节位置对象
            JointPos j = new JointPos(39.886, -98.580, -124.032, -47.393, 90.000, 40.842);
            ExaxisPos epos1 = new ExaxisPos(0, 0, 0, 0);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            ExaxisPos epos2 = new ExaxisPos(5, 0.000, 0.000, 0.000);

            // 参考点坐标
            DescPose refPoint = new DescPose(400.021, 300.022, 299.996, 179.997, -0.003, -90.956);

            int rtn = 0;
            robot.LaserTrackingSensorConfig("192.168.58.20", 5020);
            robot.LaserTrackingSensorSamplePeriod(20);
            robot.LoadPosSensorDriver(101);

            // 加载 UDP 驱动
            robot.ExtDevLoadUDPDriver();

            // 设置外部轴命令完成时间
            rtn = robot.SetExAxisCmdDoneTime(5000.0);
            Console.WriteLine("SetExAxisCmdDoneTime rtn is " + rtn);

            // 使能外部轴 1 和 2
            rtn = robot.ExtAxisServoOn(1, 1);
            Console.WriteLine("ExtAxisServoOn axis id 1 rtn is " + rtn);
            rtn = robot.ExtAxisServoOn(2, 1);
            Console.WriteLine("ExtAxisServoOn axis id 2 rtn is " + rtn);
            Thread.Sleep(2000);

            // 设置外部轴回零
            robot.ExtAxisSetHoming(1, 0, 10, 2);
            robot.LaserTrackingLaserOnOff(1);

            //// 1---不带扩展轴
            robot.LaserTrackingTrackOnOff(1, 4);
            robot.Sleep(200);
            // 启动定点摆动
            robot.OriginPointWeaveStart(0, 0, refPoint, 10);
            robot.MoveStationary();   // 执行固定运动（假设该方法存在）
            robot.OriginPointWeaveEnd();
            robot.LaserTrackingTrackOnOff(0, 4);

            Thread.Sleep(2000);         // 等待2秒

            //// 2----带扩展轴
            robot.ExtAxisMove(epos1, 100, -1);
            robot.LaserTrackingTrackOnOff(1, 4);
            // 启动定点摆动
            robot.OriginPointWeaveStart(0, 0, refPoint, 20);
            robot.ExtAxisMove(epos2, 100, -1);
            robot.OriginPointWeaveEnd();
            robot.LaserTrackingTrackOnOff(0, 4);
        }

        public int TestUDPAxis()
        {
            int rtn = 0;

            // 设置 UDP 通信参数
            rtn = robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 200, 1, 100, 5, 1);
            Console.WriteLine("ExtDevSetUDPComParam rtn is " + rtn);

            // 获取 UDP 通信参数（使用 out 参数）
            string ip = "";
            int port = 0, period = 0, lossPkgTime = 0, lossPkgNum = 0, disconnectTime = 0;
            int reconnectEnable = 0, reconnectPeriod = 0, reconnectNum = 0, selfConnect = 0;
            rtn = robot.ExtDevGetUDPComParam(ref ip, ref port, ref period, ref lossPkgTime, ref lossPkgNum,
                                             ref disconnectTime, ref reconnectEnable, ref reconnectPeriod, ref reconnectNum, ref selfConnect);
            string patam = "\nip " + ip +
                           "\nport " + port +
                           "\nperiod  " + period +
                           "\nlossPkgTime " + lossPkgTime +
                           "\nlossPkgNum  " + lossPkgNum +
                           "\ndisConntime  " + disconnectTime +
                           "\nreconnecable  " + reconnectEnable +
                           "\nreconnperiod  " + reconnectPeriod +
                           "\nreconnnun  " + reconnectNum +
                           "\nselfConnect  " + selfConnect;
            Console.WriteLine("ExtDevGetUDPComParam rtn is " + rtn + patam);

            // 加载 UDP 驱动
            robot.ExtDevLoadUDPDriver();

            // 设置外部轴命令完成时间
            rtn = robot.SetExAxisCmdDoneTime(5000.0);
            Console.WriteLine("SetExAxisCmdDoneTime rtn is " + rtn);



            // 使能外部轴 1 和 2
            rtn = robot.ExtAxisServoOn(1, 1);
            Console.WriteLine("ExtAxisServoOn axis id 1 rtn is " + rtn);
            rtn = robot.ExtAxisServoOn(2, 1);
            Console.WriteLine("ExtAxisServoOn axis id 2 rtn is " + rtn);
            Thread.Sleep(2000);


            // 设置外部轴回零
            robot.ExtAxisSetHoming(1, 0, 10, 2);
            //Thread.Sleep(2000);
            Console.WriteLine("ExtAxisSetHoming11111111111 rtnn is  ");
            rtn = robot.ExtAxisSetHoming(2, 0, 10, 2);
            Console.WriteLine("ExtAxisSetHoming rtnn is  " + rtn);
            Thread.Sleep(4000);

            // 配置机器人位置到轴
            rtn = robot.SetRobotPosToAxis(1);
            Console.WriteLine("SetRobotPosToAxis rtn is " + rtn);

            // 配置 DH 参数
            rtn = robot.SetAxisDHParaConfig(10, 20, 0, 0, 0, 0, 0, 0, 0);
            Console.WriteLine("SetAxisDHParaConfig rtn is " + rtn);

            // 配置外部轴参数（轴 1）
            rtn = robot.ExtAxisParamConfig(1, 1, 1, 1000, -1000, 1000, 1000, 1.905, 262144, 200, 1, 0, 0);
            Console.WriteLine("ExtAxisParamConfig axis 1 rtn is " + rtn);

            // 配置外部轴参数（轴 2）
            rtn = robot.ExtAxisParamConfig(2, 1, 1, 1000, -1000, 1000, 1000, 4.444, 262144, 200, 1, 0, 0);
            Console.WriteLine("ExtAxisParamConfig axis 1 rtn is " + rtn); // 原 C++ 输出文字为 "axis 1"，保持原样

            // 轴 1 点动测试
            Thread.Sleep(3000);
            robot.ExtAxisStartJog(1, 0, 10, 10, 30);
            Thread.Sleep(1000);
            robot.ExtAxisStopJog(1);
            Thread.Sleep(3000);
            robot.ExtAxisServoOn(1, 0);

            // 轴 2 点动测试
            Thread.Sleep(3000);
            robot.ExtAxisStartJog(2, 0, 10, 10, 30);
            Thread.Sleep(1000);
            robot.ExtAxisStopJog(2);
            Thread.Sleep(3000);
            robot.ExtAxisServoOn(2, 0);

            // 卸载 UDP 驱动
            robot.ExtDevUnloadUDPDriver();

            return 0;
        }

        public void testled()
        {
            robot.SetUserLEDColor(true, true, true);
            robot.Sleep(1000);
            robot.SetUserLEDColor(false, false, false);
            robot.Sleep(1000);
            robot.SetUserLEDColor(true, false, false);
            robot.Sleep(1000);
            robot.SetUserLEDColor(false, true, false);
            robot.Sleep(1000);
            robot.SetUserLEDColor(false, false, true);
        }

        public int TestCtrlOpenLuaOperate()
        {
            int rtn;

            // 上传 Lua 文件到机器人
            rtn = robot.OpenLuaUpload("D://zUP/openlua/CtrlDev_WELDING_B.lua");
            Console.WriteLine($"OpenLuaUpload rtn is {rtn}");
            rtn = robot.OpenLuaUpload("D://zUP/openlua/CtrlDev_SWDPOLISH.lua");
            Console.WriteLine($"OpenLuaUpload rtn is {rtn}");

            // 从机器人下载 Lua 文件
            rtn = robot.OpenLuaDownload("CtrlDev_WELDING_B.lua", "D://zDOWN/");
            Console.WriteLine($"OpenLuaDownload rtn is {rtn}");
            rtn = robot.OpenLuaDownload("CtrlDev_SWDPOLISH.lua", "D://zDOWN/");
            Console.WriteLine($"OpenLuaDownload rtn is {rtn}");

            // 设置控制开放协议 Lua 名称
            rtn = robot.SetCtrlOpenLUAName(0, "CtrlDev_WELDING_B.lua");
            Console.WriteLine($"SetCtrlOpenLUAName rtn is {rtn}");
            rtn = robot.SetCtrlOpenLUAName(1, "CtrlDev_SWDPOLISH.lua");
            Console.WriteLine($"SetCtrlOpenLUAName rtn is {rtn}");

            // 获取控制开放协议 Lua 名称
            string[] name = new string[4];
            rtn = robot.GetCtrlOpenLUAName(ref name);
            Console.WriteLine($"ctrl open lua names : {name[0]}, {name[1]}, {name[2]}, {name[3]}");

            // 加载和卸载开放协议 Lua
            rtn = robot.LoadCtrlOpenLUA(1);
            Console.WriteLine($"LoadCtrlOpenLUA rtn is {rtn}");
            robot.Sleep(2000);
            rtn = robot.UnloadCtrlOpenLUA(1);
            Console.WriteLine($"UnloadCtrlOpenLUA rtn is {rtn}");

            // 删除指定 Lua 文件和所有 Lua 文件
            rtn = robot.OpenLuaDelete("CtrlDev_WELDING_B.lua");
            Console.WriteLine($"OpenLuaDelete rtn is {rtn}");
            rtn = robot.AllOpenLuaDelete();
            Console.WriteLine($"AllOpenLuaDelete rtn is {rtn}");

            return 0;
        }

        /// <summary>
        /// 执行轨迹 J 文件的上传、加载、运动及运动中变速
        /// </summary>
        /// <param name="robot">已初始化的 Robot 实例</param>
        /// <param name="localFilePath">本地轨迹文件路径，例如 "D://zUP/trajHelix_aima_1.txt"</param>
        /// <param name="remoteFilePath">机器人端轨迹文件路径，例如 "trajHelix_aima_1.txt"</param>
        /// <param name="initialSpeedPercent">初始全局速度百分比，默认 50</param>
        /// <param name="trajSpeedMode">轨迹速度模式，默认 1</param>
        /// <returns>成功返回 0，失败返回错误码</returns>
public int RunTrajectoryJ(string localFilePath = "D://zUP/horse.txt", string remoteFilePath = "horse.txt",
    int initialSpeedPercent = 50, int trajSpeedMode = 1)
{
    int rtn;

    // 1. Upload trajectory J file
    rtn = robot.TrajectoryJUpLoad(localFilePath);
    if (rtn != 0)
    {
        Console.WriteLine($"Upload TrajectoryJ failed: {rtn}");
        return rtn;
    }
    Console.WriteLine($"Upload TrajectoryJ success: {localFilePath}");

    // 2. Load trajectory file
    rtn = robot.LoadTrajectoryJ(remoteFilePath, 100, 1);
    if (rtn != 0)
    {
        Console.WriteLine($"LoadTrajectoryJ failed: {rtn}");
        return rtn;
    }
    Console.WriteLine($"LoadTrajectoryJ success: {remoteFilePath}");

    // 3. Get trajectory start pose
    DescPose trajStartPose = new DescPose(0, 0, 0, 0, 0, 0);
    rtn = robot.GetTrajectoryStartPose(remoteFilePath, ref trajStartPose);
    if (rtn != 0)
    {
        Console.WriteLine($"GetTrajectoryStartPose failed: {rtn}");
        return rtn;
    }
    Console.WriteLine($"Trajectory start pose: ({trajStartPose.tran.x}, {trajStartPose.tran.y}, {trajStartPose.tran.z}, " +
                        $"{trajStartPose.rpy.rx}, {trajStartPose.rpy.ry}, {trajStartPose.rpy.rz})");

    // 4. Move to trajectory start point (using Cartesian PTP)
    robot.SetSpeed(initialSpeedPercent);
    rtn = robot.MoveCart(trajStartPose, 0, 0, 100, 100, 100, -1, -1);
    if (rtn != 0)
    {
        Console.WriteLine($"MoveCart to start pose failed: {rtn}");
        return rtn;
    }

    // 5. Get trajectory point count (optional, for display only)
    int trajPointNum = 0;
    rtn = robot.GetTrajectoryPointNum(ref trajPointNum);
    if (rtn != 0)
    {
        Console.WriteLine($"GetTrajectoryPointNum failed: {rtn}");
        // Do not return, continue execution
    }
    else
    {
        Console.WriteLine($"Trajectory points count: {trajPointNum}");
    }

    // 6. Start trajectory execution (non-blocking)
    rtn = robot.MoveTrajectoryJ();
    if (rtn != 0)
    {
        Console.WriteLine($"MoveTrajectoryJ failed: {rtn}");
        return rtn;
    }
    Console.WriteLine("MoveTrajectoryJ started.");

    // 7. Dynamically change speed during motion (alternate 10% and 80%)
    // Use GetRobotMotionDone to check if motion is complete
    byte motionDone = 0;
    robot.GetRobotMotionDone(ref motionDone);

    while (motionDone == 0)
    {
        // Set speed to 10%
        rtn = robot.SetTrajectoryJSpeed(10.0, trajSpeedMode);
        Console.WriteLine($"SetTrajectoryJSpeed to 10% returned: {rtn}");
        robot.Sleep(1000);

        // Re-check motion status
        robot.GetRobotMotionDone(ref motionDone);
        if (motionDone != 0) break;

        // Set speed to 80%
        rtn = robot.SetTrajectoryJSpeed(80.0, trajSpeedMode);
        Console.WriteLine($"SetTrajectoryJSpeed to 80% returned: {rtn}");
        robot.Sleep(1000);

        // Re-check motion status again
        robot.GetRobotMotionDone(ref motionDone);
    }

    Console.WriteLine("Trajectory J motion completed.");
    return 0;
}

        private void flowLayoutPanel8_Paint(object sender, PaintEventArgs e)
        {

        }

        public int ServoJVtest()
        {
            double[] joint_vel = new double[6] { 10, 0, 0, 0, 0, 0 };
            double[] exis_vel = new double[4] { 0, 0, 0, 0 };
            float acc = 0.0f; 
            float vel = 0.0f;
            float cmdT = 0.01f; 
            float filterT = 0.0f; 
            float gain = 0.0f;
            int cnt = 0;
            while (cnt < 200)
            {
                int error = robot.ServoJV(joint_vel, exis_vel, acc, vel, cmdT, filterT, gain,0,1);
                Console.WriteLine($"ServoJV rtn is {error}");
                cnt++;
            }
            return 0;
        }

        public int ServoMITtest()
        {
            // 订阅回调
            robot.OnUdpFrameReceived += (comType, frameCount, frameCmdID, contentLen, content) =>
            {
                Console.WriteLine($"[UDP响应] comType={comType}, count={frameCount}, cmdID={frameCmdID}, content={content}");
            };
            while (true)
            {
                robot.ResetAllError();
                Thread.Sleep(500);

                double[] posGain = new double[6] { 0, 0, 0, 0, 0, 0 };
                double[] desPos = new double[6] { 0, 0, 0, 0, 0, 0 };
                double[] velGain = new double[6] { 0, 0, 0, 0, 0, 0 };
                double[] desVel = new double[6] { 0, 0, 0, 0, 0, 0 };
                double[] torques = new double[6] { 0, 0, 0, 0, 0, 0 };
                robot.GetJointTorques(1, torques);
                Console.WriteLine($"111111");
                //robot.ServoMITEnd(0);
                robot.ServoMITStart(0);
                Console.WriteLine($"ServoMITStart");
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                robot.DragTeachSwitch(1);
                Console.WriteLine($"DragTeachSwitch");
                double intev = 0.008;
                double[] jPowerLimit = new double[6] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
                double[] jVelLimit = new double[6] { 50, 50, 50, 50, 50, 50 };
                int error = 0;
                while (true)
                {

                    torques[5] = 0.03;
                    Console.WriteLine($"ServoMIT call ");
                    error = robot.ServoMIT(posGain, desPos, velGain, desVel, torques, intev, 0);

                    Console.WriteLine($"ServoMIT111111 rtn is {error}");
                    Thread.Sleep(1);

                    robot.GetRobotRealTimeState(ref pkg);
                    //Console.WriteLine($"maincode {pkg.main_code}, subcode {pkg.sub_code}");
                    Console.WriteLine($"pkg.jt_cur_pos[5]:{pkg.jt_cur_pos[5]}");
                    if (pkg.jt_cur_pos[5] > 30)
                    {
                        break;
                    }
                }

                while (true)
                {

                    torques[5] = -0.03;
                    error = robot.ServoMIT(posGain, desPos, velGain, desVel, torques, intev, 0);

                    Console.WriteLine($"ServoJT222222 rtn is {error}");
                    Thread.Sleep(1);

                    robot.GetRobotRealTimeState(ref pkg);
                    //Console.WriteLine($"maincode {pkg.main_code}, subcode {pkg.sub_code}");
                    Console.WriteLine($"pkg.jt_cur_pos[5]:{pkg.jt_cur_pos[5]}");
                    if (pkg.jt_cur_pos[5] < 0)
                    {
                        break;
                    }
                }

                robot.DragTeachSwitch(0);
                error = robot.ServoMITEnd(0);
            }
            //return 0;
        }

        private void btnLaserWeld_Click(object sender, EventArgs e)
        {

            int rtn = -1;
            // 加载UDP扩展轴驱动
            rtn = robot.ExtDevLoadUDPDriver();
            if (rtn != 0)
            {
                Console.WriteLine("Failed to load UDP driver, error code: " + rtn);
            }
            Thread.Sleep(1000);

            // 设置激光焊接参数: io_type=1, num=3, scanSpeed=2000, scanWidth=3, peakPower=1500, dutyCycle=100, freq=1000
            rtn = robot.SetLaserWeldingParam(1, 3, 2000, 3, 1500, 100, 1000);
            if (rtn != 0)
            {
                Console.WriteLine("SetLaserWeldingParam failed, error code: " + rtn);
            }
            else
            {
                Console.WriteLine("SetLaserWeldingParam success");
            }

            // 设置启动的DO端口号
            rtn = robot.SetLaserWeldingStartExtDoNum(1);
            if (rtn != 0)
            {
                Console.WriteLine("SetLaserWeldingStartExtDoNum failed, error code: " + rtn);
            }

            // 设置为模式0（示教模式）
            rtn = robot.Mode(0);
            if (rtn != 0)
            {
                Console.WriteLine("Set mode 0 failed, error code: " + rtn);
            }
            Thread.Sleep(1000);


            //DescPose desc_pos1 = new DescPose(-303.721, -206.960, 297.105, 152.209, 19.857, 109.166);
            //DescPose desc_pos2 = new DescPose(-301.575, -254.888, 284.786, 155.919, 26.946, 111.629);
            //DescPose desc_safe = new DescPose(-344.386, -280.830, 435.073, 173.835, 15.333, 124.931);
            DescPose desc_pos1 = new DescPose(441.901, 416.508, -51.979, -179.234, 0.718, -115.305);
            JointPos startjointPos = new JointPos(-146.22, -60.551, 104.859, -135.317, -90.289, 59.088);

            DescPose desc_pos2 = new DescPose(441.901, 615.317, -51.979, -179.234, 0.718, -115.305);
            JointPos endjointPos = new JointPos(-133.22, -44.193, 74.934, -121.661, -90.509, 72.087);

            DescPose desc_safe = new DescPose(441.901, 416.508, -51.979, -179.234, 0.718, -115.305);
            JointPos safejointPos = new JointPos(-146.22, -60.551, 104.859, -135.317, -90.289, 59.088);

            ExaxisPos exaxis = new ExaxisPos(0.0, 0.0, 0.0, 0.0);
            DescPose offset = new DescPose(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);

            // 移动到第一个焊接点
            int error = robot.MoveL(desc_pos1, 0, 0, 100, 100, 100, -1, 0, exaxis, 0, 0, offset, -1, 0);
            Console.WriteLine("MoveL to pos1 return: " + error);

            // 开启激光（出光）
            rtn = robot.SetLaserWeldingStartEnd(1, 1, 10000);
            if (rtn != 0)
            {
                Console.WriteLine("SetLaserWeldingStartEnd (start) failed, error code: " + rtn);
            }
            else
            {
                Console.WriteLine("Laser started");
            }

            // 移动到第二个焊接点（焊接过程中）
            rtn = robot.MoveL(desc_pos2, 0, 0, 30, 100, 100, -1, 0, exaxis, 0, 0, offset, -1, 0);
            Console.WriteLine("MoveL to pos2 return: " + rtn);

            Thread.Sleep(500);
            // 关闭激光（收光）
            rtn = robot.SetLaserWeldingStartEnd(1, 0, 10000);
            if (rtn != 0)
            {
                Console.WriteLine("SetLaserWeldingStartEnd (stop) failed, error code: " + rtn);
            }
            else
            {
                Console.WriteLine("Laser stopped");
            }

            // 移动到安全点
            rtn = robot.MoveL(desc_safe, 0, 0, 100, 100, 100, -1, 0, exaxis, 0, 0, offset, -1, 0);
            Console.WriteLine("MoveL to safe_pos return: " + rtn);

            // 设置为模式1（远程模式）
            rtn = robot.Mode(1);
            if (rtn != 0)
            {
                Console.WriteLine("Set mode 1 failed, error code: " + rtn);
            }
            Thread.Sleep(1000);

            // 关闭连接
            robot.CloseRPC();
            Thread.Sleep(1000);

            Console.WriteLine("Test completed");

            return ;
        }

        private void button105_Click(object sender, EventArgs e)
        {
            TestFiveDexterousHands();
            //int id = 1;               // Slave station number
            //int slaveNum = 4;         // Control 4 fingers
            //int max_time = 8000;      // Maximum wait time 8 seconds
            //int[] speed = new int[16]; // Speed array, all 0 means use default speed
            //int[] force = new int[16]; // Torque array

            //// Initialize torque array: first 4 fingers set to 50%, the rest 0 (values sent via Move command)
            //for (int i = 0; i < 16; i++)
            //    force[i] = (i < 4) ? 50 : 0;

            //// Helper function: set position array (only first 4 fingers are effective)
            //double[] pos = new double[16];
            //void SetPositions(double v1, double v2, double v3, double v4)
            //{
            //    for (int i = 0; i < 16; i++)
            //        pos[i] = 0;
            //    pos[0] = v1;
            //    pos[1] = v2;
            //    pos[2] = v3;
            //    pos[3] = v4;
            //}

            //JointPos j1 = new JointPos(-91.876, -85.920, 109.279, -86.239, -96.664, -28.563);
            //JointPos j2 = new JointPos(-40.954, -85.920, 109.279, -86.239, -96.664, -28.563);
            //ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);

            //Console.WriteLine("===== Dexterous Hand Full Function Test Started =====");

            //// 1. Clear error
            //int ret = robot.ClearDexterousHandsError();
            //Console.WriteLine($"ClearDexterousHandsError -> {ret}");

            //// ========== 2. Set function switches ==========
            //int[] setFunc = new int[32];
            //setFunc[2] = 1;   // Enable position setting function
            //setFunc[4] = 1;   // Enable torque setting function
            //setFunc[9] = 1;   // Read position
            //setFunc[10] = 1;  // Read torque
            //setFunc[11] = 1;  // Read status
            //setFunc[22] = 1;  // Single-axis motion status

            //ret = robot.SetDexterousHandsFunc(id, setFunc);
            //Console.WriteLine($"SetDexterousHandsFunc(enable + init + position/torque functions enabled) -> {ret}");

            //// ========== 3. Read function status (verify settings took effect) ==========
            //int[] getFunc = new int[32];  // GetDexterousHandsFunc returns 32 integers
            //ret = robot.GetDexterousHandsFunc(id, ref getFunc);
            //Console.WriteLine($"GetDexterousHandsFunc -> {ret}");
            //if (ret == 0)
            //{
            //    // Print all 32 values
            //    Console.WriteLine("All 32 values returned by GetDexterousHandsFunc:");
            //    for (int i = 0; i < getFunc.Length; i++)
            //    {
            //        Console.Write($"  [{i}]={getFunc[i]}");
            //        if ((i + 1) % 8 == 0)
            //            Console.WriteLine();          // New line every 8 items
            //        else if (i < getFunc.Length - 1)
            //            Console.Write(", ");
            //    }
            //    if (getFunc.Length % 8 != 0)
            //        Console.WriteLine();              // Add newline if last line has fewer than 8 items
            //}

            //// ========== 4. Activate dexterous hand ==========
            //ret = robot.SetDexterousHandsAct(id, 1);
            //Console.WriteLine($"SetDexterousHandsAct(activate) -> {ret}");
            //if (ret != 0)
            //{
            //    Console.WriteLine("Activation failed, test aborted");
            //    return;
            //}

            //// ========== 5. Initial move to 20° (send position and torque values via Move command) ==========
            //SetPositions(20, 20, 20, 20);
            //ret = robot.SetDexterousHandsMove(id, slaveNum, pos, speed, force, max_time);
            //Console.WriteLine($"Initial move to 20° -> {ret}");
            //robot.Sleep(5000);

            //// ========== 6. Reciprocating motion 10 times (10° ↔ 50°) ==========
            //Console.WriteLine("Starting 10 reciprocating motions...");
            //for (int iteration = 1; iteration <= 10; iteration++)
            //{
            //    robot.MoveJ(j1, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);

            //    SetPositions(10, 10, 10, 10);
            //    ret = robot.SetDexterousHandsMove(id, slaveNum, pos, speed, force, max_time);
            //    Console.WriteLine($"[{iteration}] Move to 10° -> {ret}");
            //    robot.Sleep(1000);

            //    robot.MoveJ(j2, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);

            //    SetPositions(50, 50, 50, 50);
            //    ret = robot.SetDexterousHandsMove(id, slaveNum, pos, speed, force, max_time);
            //    Console.WriteLine($"[{iteration}] Move to 50° -> {ret}");
            //    robot.Sleep(1000);
            //}

            //Console.WriteLine("Test completed (function switch set/read + activation + 10 reciprocating motions).");
        }

        /// <summary>
        /// 五指灵巧手完整功能测试
        /// 测试流程：
        /// 1. 清除错误 → 2. 设置功能开关(主站funcA + 从站funcB) → 3. 读取验证 → 4. 激活
        /// 5. 等待5s → 6. 往复运动10次(j1↔j2, 手指A→B→A→C)
        /// </summary>
        /// <returns>0-成功, -2-激活失败</returns>
        private int TestFiveDexterousHands()
        {
            const int DEXTEROUS_ID = 1;
            const int FINGER_COUNT = 12;
            const int MOVE_TIMEOUT_MS = 12000;

            int[] speed = { 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 0, 0, 0, 0 };
            int[] force = { 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 0, 0, 0, 0 };

            double[] posA = { 5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5, 0, 0, 0, 0 };
            double[] posB = { 60, 10, 70, 30, 70, 70, 10, 10, 10, 10, 10, 10, 0, 0, 0, 0 };
            double[] posC = { 50, 50, 20, 20, 0,  0,  0,  0,  70, 70, 70, 70, 0, 0, 0, 0 };

            JointPos j1 = new JointPos(-172.132, -90.455, -102.422, -67.864, 95.273, -21.129);
            JointPos j2 = new JointPos(-173.180, -106.578, -83.661, -70.600, 95.440, -22.167);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);

            Console.WriteLine("===== 五指灵巧手完整功能测试开始 =====");

            // 1. 清除错误
            int rtn = robot.ClearDexterousHandsError();
            Console.WriteLine($"[清除错误] rtn = {rtn}");

            // 2. 设置功能开关 — 主站(funcA, 含多轴同步) + 从站(funcB)
            int[] funcA = new int[32];
            funcA[2] = 1;   // 位置设置
            funcA[3] = 1;   // 速度设置
            funcA[4] = 1;   // 力矩设置
            funcA[9] = 1;   // 读位置
            funcA[10] = 1;  // 读速度
            funcA[11] = 1;  // 读力矩
            funcA[20] = 1;  // 多轴同步运动
            funcA[22] = 1;  // 单轴运行状态

            int[] funcB = new int[32];
            funcB[2] = 1;
            funcB[3] = 1;
            funcB[4] = 1;
            funcB[9] = 1;
            funcB[10] = 1;
            funcB[11] = 1;
            funcB[22] = 1;

            rtn = robot.SetDexterousHandsFunc(DEXTEROUS_ID, funcA);
            Console.WriteLine($"[设置主站功能] rtn = {rtn}");

            for (int i = 2; i <= FINGER_COUNT; i++)
            {
                rtn = robot.SetDexterousHandsFunc(i, funcB);
            }
            Console.WriteLine($"[设置从站功能(2~12)] rtn = {rtn}");

            // 3. 读取功能状态
            int[] getFunc = new int[32];
            rtn = robot.GetDexterousHandsFunc(DEXTEROUS_ID, ref getFunc);
            Console.WriteLine($"[读取功能状态] rtn = {rtn}");
            if (rtn == 0)
            {
                Console.WriteLine("功能开关状态(32位):");
                for (int i = 0; i < 32; i++)
                {
                    Console.Write($"[{i}]={getFunc[i]}");
                    if ((i + 1) % 8 == 0 && i < 31)
                        Console.WriteLine();
                    else if (i < 31)
                        Console.Write(", ");
                }
                Console.WriteLine();
            }

            // 4. 激活
            rtn = robot.SetDexterousHandsAct(DEXTEROUS_ID, 1);
            Console.WriteLine($"[激活灵巧手] rtn = {rtn}");
            if (rtn != 0)
            {
                Console.WriteLine("激活失败，测试中止");
                return -2;
            }
            robot.Sleep(5000);

            // 5. 往复运动测试 10 次，每组 4 个动作：j1+A → j2+B → j1+A → j2+C
            Console.WriteLine("\n开始往复运动测试(共10次循环)...");
            Console.WriteLine("  位姿1: j1(左)  位姿2: j2(右)");
            Console.WriteLine("  手指目标: A→B→A→C(每组4个动作)\n");

            for (int iter = 1; iter <= 10; iter++)
            {
                Console.WriteLine($"--- 第 {iter,2} 次循环 ---");

                robot.MoveJ(j1, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);
                rtn = robot.SetDexterousHandsMove(DEXTEROUS_ID, FINGER_COUNT, posA, speed, force, MOVE_TIMEOUT_MS);
                Console.WriteLine($"  j1 + posA → {rtn}");
                robot.Sleep(1000);

                robot.MoveJ(j2, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);
                rtn = robot.SetDexterousHandsMove(DEXTEROUS_ID, FINGER_COUNT, posB, speed, force, MOVE_TIMEOUT_MS);
                Console.WriteLine($"  j2 + posB → {rtn}");
                robot.Sleep(1000);

                robot.MoveJ(j1, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);
                rtn = robot.SetDexterousHandsMove(DEXTEROUS_ID, FINGER_COUNT, posA, speed, force, MOVE_TIMEOUT_MS);
                Console.WriteLine($"  j1 + posA → {rtn}");
                robot.Sleep(1000);

                robot.MoveJ(j2, 0, 0, 100, 100, 100, epos, -1, 0, offset_pos);
                rtn = robot.SetDexterousHandsMove(DEXTEROUS_ID, FINGER_COUNT, posC, speed, force, MOVE_TIMEOUT_MS);
                Console.WriteLine($"  j2 + posC → {rtn}");
                robot.Sleep(1000);
            }

            Console.WriteLine("\n===== 测试完成 =====");
            Console.WriteLine("  功能开关设置/读取  ok");
            Console.WriteLine("  灵巧手激活        ok");
            Console.WriteLine("  10次往复运动      ok");
            return 0;
        }

        /// <summary>
        /// 夹爪工件掉落报警测试
        /// 测试要求:
        /// 1. 已通过web配置完成外设协议/使能/夹爪/CO2=53
        /// 2. 夹爪安装在机器人末端
        /// 3. 本函数通过SDK控制夹爪运动并检测工件掉落报警
        /// </summary>
        /// <param name="robot">已RPC连接的Robot实例</param>
        /// <param name="gripperIndex">夹爪索引(默认1)</param>
        /// <param name="vel">夹爪速度百分比</param>
        /// <param name="force">夹爪力矩百分比</param>
        /// <param name="testCycles">测试循环次数(默认10)</param>
        /// <returns>true: 测试通过; false: 测试失败</returns>
        private bool GripperDropAlarmTest(Robot robot, int gripperIndex = 1, int vel = 50, int force = 50, int testCycles = 10)
        {
            int passCount = 0;   // 检测到8-3报警的次数
            int failCount = 0;   // 未检测到8-3报警的次数

            Console.WriteLine("=== 夹爪工件掉落报警测试(优化版) 开始 ===");
            Console.WriteLine("测试次数: {0}, 夹爪索引: {1}", testCycles, gripperIndex);
            Console.WriteLine("点位1→夹爪0, 点位2→夹爪100, 点位3→夹爪0");

            // ========== 1. 硬编码3个点位 (关节位置1-6, 笛卡尔位姿7-12) ==========
            JointPos[] jointPosList = new JointPos[3];
            DescPose[] descPoseList = new DescPose[3];

            // 点位1 (原点位2)
            jointPosList[0] = new JointPos(-149.135, -27.245, 87.924, -155.015, -92.466, -85.943);
            descPoseList[0] = new DescPose(358.229, 327.918, -32.984, 177.847, -4.499, 26.800);

            // 点位2 (原点位1)
            jointPosList[1] = new JointPos(-149.135, -35.773, 90.296, -148.860, -92.466, -85.943);
            descPoseList[1] = new DescPose(358.233, 327.918, 16.329, 177.847, -4.500, 26.800);

            // 点位3 (不变)
            jointPosList[2] = new JointPos(-149.135, -46.698, 90.080, -137.719, -92.467, -85.943);
            descPoseList[2] = new DescPose(358.234, 327.920, 87.013, 177.847, -4.500, 26.800);

            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("点位{0}: Joint=[{1:F3},{2:F3},{3:F3},{4:F3},{5:F3},{6:F3}]",
                    i + 1, jointPosList[i].jPos[0], jointPosList[i].jPos[1], jointPosList[i].jPos[2],
                    jointPosList[i].jPos[3], jointPosList[i].jPos[4], jointPosList[i].jPos[5]);
                Console.WriteLine("       Desc=[{0:F3},{1:F3},{2:F3},{3:F3},{4:F3},{5:F3}]",
                    descPoseList[i].tran.x, descPoseList[i].tran.y, descPoseList[i].tran.z,
                    descPoseList[i].rpy.rx, descPoseList[i].rpy.ry, descPoseList[i].rpy.rz);
            }

            // ========== 2. 切换到手动模式 ==========
            int rtn = robot.Mode(1);
            Console.WriteLine("Mode(1) rtn={0}", rtn);

            ROBOT_STATE_PKG state = new ROBOT_STATE_PKG();
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            // 根据Z方向最低的点位自动设置夹爪=100(抓取), 其余点位夹爪=0
            int[] gripperTargetPos = { 0, 0, 0 };
            int lowestZIdx = 0;
            double lowestZ = descPoseList[0].tran.z;
            for (int i = 1; i < 3; i++)
            {
                if (descPoseList[i].tran.z < lowestZ)
                {
                    lowestZ = descPoseList[i].tran.z;
                    lowestZIdx = i;
                }
            }
            gripperTargetPos[lowestZIdx] = 100;
            Console.WriteLine("Z最低点位为{0} (z={1:F3}), 设为夹爪抓取位置(100)", lowestZIdx + 1, lowestZ);
            Console.WriteLine("夹爪目标: [{0}, {1}, {2}]", gripperTargetPos[0], gripperTargetPos[1], gripperTargetPos[2]);
            const int alarmPollTimeout = 8000;  // 夹爪100时轮询检测8-3的超时(ms)
            const int alarmPollInterval = 200;  // 轮询间隔(ms)

            // ========== 3. 主测试循环 ==========
            for (int cycle = 1; cycle <= testCycles; cycle++)
            {
                Console.WriteLine("\n========== 第 {0}/{1} 次测试 ==========", cycle, testCycles);
                bool alarmTriggered = false;

                try
                {
                    // 3.1 清除残留故障
                    robot.GetRobotRealTimeState(ref state);
                    Console.WriteLine("  初始状态: main_code={0}, sub_code={1}, CO={2}",
                        state.main_code, state.sub_code, state.cl_dgt_output_h);
                    if (state.main_code != 0 || state.sub_code != 0)
                    {
                        Console.WriteLine("  残留故障码[{0},{1}]，先清除", state.main_code, state.sub_code);
                        robot.ResetAllError();
                        robot.Sleep(800);
                    }

                    // 3.2 复位并激活夹爪 (激活后夹爪默认在100)
                    Console.WriteLine("  复位夹爪...");
                    rtn = robot.ActGripper(gripperIndex, 0);
                    Console.WriteLine("  ActGripper(reset) rtn={0}", rtn);
                    robot.Sleep(2000);

                    Console.WriteLine("  激活夹爪...");
                    rtn = robot.ActGripper(gripperIndex, 1);
                    Console.WriteLine("  ActGripper(activate) rtn={0}", rtn);
                    robot.Sleep(3000);

                    // 3.3 夹爪张开到0, 放入工件
                    Console.WriteLine("  夹爪张开到0...");
                    rtn = robot.MoveGripper(gripperIndex, 0, vel, force, 10000, 1, 0, 0, 0, 0);
                    Console.WriteLine("  MoveGripper(0) rtn={0}", rtn);
                    robot.Sleep(1000);

                    Console.WriteLine("  >>> 请放置工件 <<< (等待3秒)");
                    robot.Sleep(3000);

                    // 3.4 夹爪闭合到100, 抓取工件
                    Console.WriteLine("  夹爪闭合到100...");
                    rtn = robot.MoveGripper(gripperIndex, 100, vel, force, 10000, 1, 0, 0, 0, 0);
                    Console.WriteLine("  MoveGripper(100) rtn={0}", rtn);
                    robot.Sleep(1000);

                    // 检查是否夹持到物体, 多等几拍 (gripper_motiondone: 0=未完成, 1=完成未检测到物体, 2=完成检测到物体)
                    bool objGripped = false;
                    for (int waitCnt = 0; waitCnt < 5; waitCnt++)
                    {
                        robot.Sleep(300);
                        robot.GetRobotRealTimeState(ref state);
                        Console.WriteLine("  等待夹持检测({0}/5): gripper_motiondone={1} (2=夹持到物体)",
                            waitCnt + 1, state.gripper_motiondone);
                        if (state.gripper_motiondone == 2)
                        {
                            objGripped = true;
                            break;
                        }
                    }
                    if (!objGripped)
                    {
                        Console.WriteLine("  >>> 未夹持到物体, 跳过本轮掉落检测 <<<");
                        continue;  // 跳到下一轮
                    }
                    Console.WriteLine("  >>> 已夹持到物体, 开始检测 <<<");

                    // 3.5 遍历3个点位: MoveJ → 夹爪动作 → 检测8-3报警
                    for (int ptIdx = 0; ptIdx < 3; ptIdx++)
                    {
                        int targetPos = gripperTargetPos[ptIdx];
                        Console.WriteLine("\n  --- 点位{0} (夹爪→{1}) ---", ptIdx + 1, targetPos);

                        // MoveJ到目标点位
                        Console.WriteLine("  MoveJ到点位{0}...", ptIdx + 1);
                        rtn = robot.MoveJ(jointPosList[ptIdx], descPoseList[ptIdx],
                            0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
                        Console.WriteLine("  MoveJ rtn={0}", rtn);

                        if (rtn != 0)
                        {
                            Console.WriteLine("  MoveJ失败, 跳过此点位");
                            continue;
                        }
                        robot.Sleep(500);

                        // 获取MoveJ后的实时状态
                        robot.GetRobotRealTimeState(ref state);
                        Console.WriteLine("  MoveJ后状态: main_code={0}, sub_code={1}, CO={2}",
                            state.main_code, state.sub_code, state.cl_dgt_output_h);

                        // 夹爪运动到目标位置
                        Console.WriteLine("  夹爪运动到{0}...", targetPos);
                        rtn = robot.MoveGripper(gripperIndex, targetPos, vel, force, 10000, 1, 0, 0, 0, 0);
                        Console.WriteLine("  MoveGripper({0}) rtn={1}", targetPos, rtn);
                        robot.Sleep(500);

                        // 检测8-3报警: 夹爪=100时轮询等待, 夹爪=0时单次检测
                        if (targetPos == 100)
                        {
                            // 夹爪闭合到100后轮询等待检测工件掉落
                            Console.WriteLine("  轮询检测8-3报警(超时{0}ms)...", alarmPollTimeout);
                            DateTime pollStart = DateTime.Now;
                            while ((DateTime.Now - pollStart).TotalMilliseconds < alarmPollTimeout)
                            {
                                robot.GetRobotRealTimeState(ref state);
                                Console.WriteLine("  检测中: main_code={0}, sub_code={1}, CO={2}, gripper_motiondone={3}",
                                    state.main_code, state.sub_code, state.cl_dgt_output_h, state.gripper_motiondone);
                                if (state.main_code == 8 && state.sub_code == 3)
                                {
                                    double elapsed = (DateTime.Now - pollStart).TotalMilliseconds;
                                    Console.WriteLine("  >>> 检测到8-3工件掉落报警! (用时{0:F0}ms) <<<", elapsed);
                                    alarmTriggered = true;
                                    break;
                                }
                                robot.Sleep(alarmPollInterval);
                            }
                            if (!alarmTriggered)
                                Console.WriteLine("  轮询结束, 未检测到8-3报警");
                        }
                        else
                        {
                            // 夹爪=0时单次检测
                            robot.Sleep(1000);
                            robot.GetRobotRealTimeState(ref state);
                            Console.WriteLine("  夹爪动作后状态: main_code={0}, sub_code={1}, CO={2}, gripper_motiondone={3}",
                                state.main_code, state.sub_code, state.cl_dgt_output_h, state.gripper_motiondone);
                            if (state.main_code == 8 && state.sub_code == 3)
                            {
                                Console.WriteLine("  >>> 检测到8-3工件掉落报警! <<<");
                                alarmTriggered = true;
                            }
                        }

                        if (alarmTriggered) break;  // 检测到报警后跳出点位循环
                    }

                    // 3.6 检测到报警后等待用户确认
                    if (alarmTriggered)
                    {
                        Console.WriteLine("\n  ===== 检测到8-3工件掉落报警! =====");
                        robot.GetRobotRealTimeState(ref state);
                        Console.WriteLine("  状态: main_code={0}, sub_code={1}, CO={2}, gripper_motiondone={3}",
                            state.main_code, state.sub_code, state.cl_dgt_output_h, state.gripper_motiondone);
                        MessageBox.Show(
                            string.Format("检测到8-3工件掉落报警!\n\nmain_code={0}, sub_code={1}\nCO={2}, gripper_motiondone={3}\n\n点击确定进入下一轮测试",
                                state.main_code, state.sub_code, state.cl_dgt_output_h, state.gripper_motiondone),
                            "掉落报警", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Console.WriteLine("  >>> 收到确认, 进入下一轮 <<<");
                        passCount++;
                    }
                    else
                    {
                        Console.WriteLine("\n  ===== 本次测试未检测到8-3报警 =====");
                        robot.GetRobotRealTimeState(ref state);
                        Console.WriteLine("  最终状态: main_code={0}, sub_code={1}, CO={2}",
                            state.main_code, state.sub_code, state.cl_dgt_output_h);
                        failCount++;
                    }

                    // 3.5 清除报警，准备下一轮
                    Console.WriteLine("  清除报警...");
                    rtn = robot.ResetAllError();
                    Console.WriteLine("  ResetAllError rtn={0}", rtn);
                    robot.Sleep(1500);

                    robot.GetRobotRealTimeState(ref state);
                    Console.WriteLine("  清除后状态: main_code={0}, sub_code={1}, CO={2}",
                        state.main_code, state.sub_code, state.cl_dgt_output_h);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("  异常: {0}", ex.Message);
                    failCount++;
                    try { robot.ResetAllError(); } catch { }
                }
            }

            Console.WriteLine("\n========== 测试结果 ==========");
            Console.WriteLine("检测到8-3报警: {0} 次, 未检测到: {1} 次, 总计: {2} 次",
                passCount, failCount, testCycles);
            Console.WriteLine("报警检出率: {0}%", testCycles > 0 ? (double)passCount / testCycles * 100 : 0);

            return failCount == 0;
        }

        /// <summary>
        /// 程序加载测试 - 对应test0602.py中的ProgramLoad_test
        /// </summary>
        private void ProgramLoad_test()
        {
            int error;
            string loadednamestr = "";

            error = robot.ProgramLoad("test.lua");
            Console.WriteLine("ProgramLoad return {0}", error);

            error = robot.GetLoadedProgram(ref loadednamestr);
            Console.WriteLine("GetLoadedProgram return {0}", error);
            Console.WriteLine("Loaded lua name is : {0}", loadednamestr);

            robot.Mode(0);
            Thread.Sleep(1000);
            robot.ProgramRun();
            Thread.Sleep(1000);
            robot.Mode(1);
        }

        /// <summary>
        /// 加载默认程序配置测试 - 对应test0602.py中的LoadDefaultProgConfig_test
        /// </summary>
        private void LoadDefaultProgConfig_test()
        {
            string loadednamestr = "";

            robot.LoadDefaultProgConfig(1, "test.lua");
            int error = robot.GetLoadedProgram(ref loadednamestr);
            Console.WriteLine("Loaded lua name is : {0}", loadednamestr);
            Console.WriteLine("GetLoadedProgram return {0}", error);

            robot.Mode(0);
            Thread.Sleep(100000);
            robot.Mode(1);
        }

        /// <summary>
        /// 轨迹测试 - 对应test0602.py中的Traj_test
        /// </summary>
        private void Traj_test()
        {
            int rtn;
            DescPose traj_start_pose = new DescPose();

            // 上传轨迹文件 horse.txt
            rtn = robot.TrajectoryJUpLoad("D://zUP/horse.txt");
            Console.WriteLine("Upload TrajectoryJ horse.txt, rtn is: {0}", rtn);

            string traj_file_name1 = "horse.txt";
            // 加载轨迹文件，参数：文件名，速度百分比，是否循环（1:循环）
            rtn = robot.LoadTrajectoryJ(traj_file_name1, 100, 1);
            Console.WriteLine("LoadTrajectoryJ {0}, rtn is: {1}", traj_file_name1, rtn);

            // 获取轨迹起始点位姿
            rtn = robot.GetTrajectoryStartPose(traj_file_name1, ref traj_start_pose);
            Console.WriteLine("GetTrajectoryStartPose is: {0}", rtn);
            Console.WriteLine("desc_pos:{0},{1},{2},{3},{4},{5}",
                traj_start_pose.tran.x, traj_start_pose.tran.y, traj_start_pose.tran.z,
                traj_start_pose.rpy.rx, traj_start_pose.rpy.ry, traj_start_pose.rpy.rz);

            Thread.Sleep(1000);

            // 上传轨迹文件 fivestart.txt
            rtn = robot.TrajectoryJUpLoad("D://zUP/trajHelix_aima_1.txt");
            Console.WriteLine("Upload TrajectoryJ trajHelix_aima_1.txt, rtn is: {0}", rtn);

            // 加载轨迹，使用LA参数
            string traj_file_name2 = "trajHelix_aima_1.txt";
            rtn = robot.LoadTrajectoryLA(traj_file_name2, 2, 0, 0, 1, 40, 100, 100, 1);
            Console.WriteLine("LoadTrajectoryLA {0}, rtn is: {1}", traj_file_name2, rtn);

            // 获取起始点位姿
            rtn = robot.GetTrajectoryStartPose(traj_file_name2, ref traj_start_pose);
            Console.WriteLine("GetTrajectoryStartPose is: {0}", rtn);
            Console.WriteLine("desc_pos:{0},{1},{2},{3},{4},{5}",
                traj_start_pose.tran.x, traj_start_pose.tran.y, traj_start_pose.tran.z,
                traj_start_pose.rpy.rx, traj_start_pose.rpy.ry, traj_start_pose.rpy.rz);

            Thread.Sleep(1000);
        }

        /// <summary>
        /// 轨迹测试入口 - 对应test0602.py中的Traj_test主调用
        /// </summary>
        public void RunTrajTest()
        {
            Traj_test();
            //ProgramLoad_test();
            //LoadDefaultProgConfig_test();
        }

        /// <summary>
        /// 稳定性测试 - 循环执行核心测试项
        /// 依次执行: Mode兼容测试 -> MoveL兼容测试 -> 轨迹测试
        /// 
        /// </summary>
        public void TestStable()
        {
            int cycles = 30000;
            int successCount = 0;
            int failCount = 0;

            Console.WriteLine("============================================================");
            Console.WriteLine("  稳定性测试 (Stability Test)");
            Console.WriteLine("  循环次数: {0}", cycles);
            Console.WriteLine("  测试项: RunTrajectoryJ -> Mode兼容 -> MoveL兼容");
            Console.WriteLine("============================================================");

            if (robot == null)
            {
                Console.WriteLine("ERROR: 机器人未连接!");
                return;
            }

            for (int i = 0; i < cycles; i++)
            {
                Console.WriteLine("\n============================================================");
                Console.WriteLine("  >>> 第 {0}/{1} 轮测试 <<<", i + 1, cycles);
                Console.WriteLine("============================================================");

                try
                {
                    // Step 1: 轨迹测试 (上传/加载/起始点/运动/速度切换)
                    Console.WriteLine("\n--- Step 1/{0}: RunTrajectoryJ 轨迹测试 ---", i + 1);
                    int trajRtn = RunTrajectoryJ();
                    if (trajRtn != 0)
                    {
                        Console.WriteLine("  RunTrajectoryJ 返回错误: {0}", trajRtn);
                    }
                    Thread.Sleep(500);

                    // Step 2: Mode参数兼容测试 (UINT-056)
                    Console.WriteLine("\n--- Step 2/{0}: SetTrajectoryJSpeed参数兼容测试 (UINT-058) ---", i + 1);
                    Test_UINT058_SetTrajectoryJSpeed_Compatibility();
                    Thread.Sleep(500);

                    robot.StopMotion();

                    // Step 3: MoveL数组兼容测试 (UINT-057)
                    Console.WriteLine("\n--- Step 3/{0}: MoveL数组兼容测试 (UINT-057) ---", i + 1);
                    Test_UINT057_MoveL_ArrayCompatibility();
                    Thread.Sleep(500);

                    successCount++;
                    Console.WriteLine("\n  >>> 第 {0} 轮完成 \u2713 <<<", i + 1);
                }
                catch (Exception ex)
                {
                    failCount++;
                    Console.WriteLine("\n  >>> 第 {0} 轮失败: {1} <<<", i + 1, ex.Message);
                    Thread.Sleep(2000);
                    continue;
                }

                if (robot.GetReconnectState())
                {
                    Console.WriteLine("\n  >>> 重连中，提前结束测试 <<<");
                    break;
                }

                Thread.Sleep(1000);
            }

            Console.WriteLine("\n============================================================");
            Console.WriteLine("  稳定性测试 完成");
            Console.WriteLine("  成功: {0}/{1}, 失败: {2}/{1}", successCount, cycles, failCount);
            Console.WriteLine("============================================================");
        }

        public void TestSplineWeave()
        {
            int rtn;

            //robot.SetReconnectParam(true, 30000, 500);

            // 摆动回中心配置
            robot.SetWeavebackCenterConfig(1);
            int weaveBackConfig = 0;
            robot.GetWeavebackCenterConfig(ref weaveBackConfig);
            Console.WriteLine("GetWeavebackCenterConfig: {0}", weaveBackConfig);




            JointPos j1 = new JointPos(9.000, -66.067, 67.706, -103.217, -90.151, 100.669);
            JointPos j2 = new JointPos(-4.660, -107.973, 103.734, -76.214, -89.999, 90.886);
            JointPos j3 = new JointPos(-36.762, -77.380, 91.364, -127.159, -90.024, 54.833);
            JointPos j4 = new JointPos(-62.875, -89.460, 86.437, -77.030, -90.012, 31.539);
            DescPose desc_pos1 = new DescPose(-654.129, -235.344, 246.543, 6.010, -11.535, -176.787);
            DescPose desc_pos2 = new DescPose(-273.710, -100.871, 280.935, 5.692, 9.522, 179.512);
            DescPose desc_pos3 = new DescPose(-566.093, 311.278, 215.008, -10.453, -17.486, -174.209);
            DescPose desc_pos4 = new DescPose(-246.558, 328.240, 292.173, 13.912, 4.437, -179.067);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            int tool = 2;
            int user = 0;
            float vel = 100.0f;
            float acc = 100.0f;
            float ovl = 20.0f;
            float blendT = 0.0f;
            byte flag = 0;

            robot.SetSpeed(1);

            // 移动到起始点j1
            rtn = robot.MoveJ(j1, desc_pos1, tool, user, vel, acc, 100.0f, epos, blendT, flag, offset_pos);
            Console.WriteLine("MoveJ to j1 rtn: {0}", rtn);

            // 摆动 + 样条曲线运动
            robot.WeaveStart(0);
            robot.NewSplineStart(0, 6000);
            robot.NewSplinePoint(j1, desc_pos1, tool, user, vel, acc, ovl, -1.0f, 0);
            robot.NewSplinePoint(j2, desc_pos2, tool, user, vel, acc, ovl, -1.0f, 0);
            robot.NewSplinePoint(j3, desc_pos3, tool, user, vel, acc, ovl, -1.0f, 0);
            robot.NewSplinePoint(j4, desc_pos4, tool, user, vel, acc, ovl, -1.0f, 1);
            robot.NewSplineEnd();

            Console.WriteLine("TestSplineWeave completed");
        }

        // ========== 01 xmlrpc接口兼容测试 ==========

        /// <summary>
        /// UINT-056: Mode() 多参数通配符兼容测试
        /// 验证xmlrpc服务端在接收超过需要的参数时, 通配符*能正常匹配并忽略多余参数
        /// 正常参数: 1个 → 测试: 1个、2个、3个
        /// </summary>
        private void Test_UINT056_Mode_Compatibility()
        {
            Console.WriteLine("\n========== UINT-056: Mode() 多参数通配符兼容测试 ==========");
            Console.WriteLine("当前版本Mode参数: 1个 (int mode)");

            // 创建测试用xmlrpc代理
            var testProxy = CookComputing.XmlRpc.XmlRpcProxyGen.Create<ITestXmlrpcProxy>();
            testProxy.Url = "http://192.168.58.2:20003/RPC2";
            testProxy.Timeout = 1800000;

            // 测试1: 正常1个参数
            Console.WriteLine("\n--- 测试1: 1个参数(正常) ---");
            try
            {
                int rtn = testProxy.Mode_1Param(0);
                Console.WriteLine("  Mode(0) → rtn={0} ✓", rtn);
                Thread.Sleep(1500);
                rtn = testProxy.Mode_1Param(1);
                Console.WriteLine("  Mode(1) → rtn={0} ✓", rtn);
                Console.WriteLine("  结果: 通过");
            }
            catch (Exception ex)
            {
                Console.WriteLine("  异常: {0}", ex.Message);
            }

            Thread.Sleep(1000);
            // 测试2: 多余2个参数 (正常1个 + 1个多余)
            Console.WriteLine("\n--- 测试2: 2个参数(多余1个) ---");
            try
            {
                int rtn = testProxy.Mode_2Params(0, 999);
                Console.WriteLine("  Mode(0, 999) → rtn={0}", rtn);
                if (rtn == 0)
                    Console.WriteLine("  结果: 通过 (通配符*正常匹配并忽略多余参数)");
                else
                    Console.WriteLine("  结果: 失败 (rtn={0})", rtn);
            }
            catch (Exception ex)
            {
                Console.WriteLine("  异常: {0} (可能是旧版本不支持)", ex.Message);
            }

            Thread.Sleep(1000);
            // 测试3: 多余3个参数 (正常1个 + 2个多余)
            Console.WriteLine("\n--- 测试3: 3个参数(多余2个) ---");
            try
            {
                int rtn = testProxy.Mode_3Params(1, 999, 888);
                Console.WriteLine("  Mode(1, 999, 888) → rtn={0}", rtn);
                if (rtn == 0)
                    Console.WriteLine("  结果: 通过 (通配符*正常匹配并忽略多余参数)");
                else
                    Console.WriteLine("  结果: 失败 (rtn={0})", rtn);
            }
            catch (Exception ex)
            {
                Console.WriteLine("  异常: {0} (可能是旧版本不支持)", ex.Message);
            }
        }

        /// <summary>
        /// UINT-057: MoveL() 数组元素个数兼容测试
        /// 使用GripperDropAlarmTest中的3个点位进行MoveL运动测试
        /// 验证xmlrpc服务端在接收超过需要数组元素时, 通配符*能正常匹配并忽略多余元素
        /// 正常元素数: 33个 → 测试: 33个、34个、35个
        /// </summary>
        private void Test_UINT057_MoveL_ArrayCompatibility()
        {
            Console.WriteLine("\n========== UINT-057: MoveL() 数组元素个数兼容测试 ==========");
            Console.WriteLine("当前版本MoveL数组元素: 33个");
            Console.WriteLine("使用GripperDropAlarmTest中的3个点位");

            // ===== C++ TestStable 中的3个点位 =====
            JointPos[] jointPosList = new JointPos[3];
            DescPose[] descPoseList = new DescPose[3];

            //// 点位1: j1 + desc_pos1
            //jointPosList[0] = new JointPos(-11.001, -99.000, 116.999, -108.543, -91.589, 74.859);
            //descPoseList[0] = new DescPose(-428.836, -46.244, 350.722, -178.325, 0.110, 4.134);
            //// 点位2: j2 + desc_pos2
            //jointPosList[1] = new JointPos(-11.001, -95.337, 123.348, -118.545, -91.588, 74.859);
            //descPoseList[1] = new DescPose(-428.834, -46.246, 290.645, -178.327, 0.101, 4.134);
            //// 点位3: j3 + desc_pos3
            //jointPosList[2] = new JointPos(-9.607, -86.852, 115.046, -118.695, -91.601, 76.252);
            //descPoseList[2] = new DescPose(-488.922, -46.255, 290.676, -178.326, 0.107, 4.135);

            jointPosList[0] = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            jointPosList[1] = new JointPos(-45.615, -106.172, 124.296, -107.151, -91.282, 74.255);
            jointPosList[2] = new JointPos(-29.777, -84.536, 109.275, -114.075, -86.655, 74.257);
            descPoseList[0] = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            descPoseList[1] = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);
            descPoseList[2] = new DescPose(-487.434, 154.362, 308.576, 176.600, 0.268, -14.061);



            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            DescPose offset = new DescPose(0, 0, 0, 0, 0, 0);
            int tool = 0, user = 0;
            double vel = 100, acc = 100, ovl = 100, blendR = -1, oacc = 100;
            int blendMode = 0, search = 0, offset_flag = 0, velAccParamMode = 0;

            // 切换到手动模式
            robot.Mode(1);
            Thread.Sleep(500);

            // ===== 测试1: 正常33个元素, 遍历3个点位 =====
            Console.WriteLine("\n--- 测试1: 33个元素(正常), 3个点位 ---");
            for (int ptIdx = 0; ptIdx < 3; ptIdx++)
            {
                Console.WriteLine("  点位{0}...", ptIdx + 1);
                try
                {
                    int rtn = robot.MoveL(jointPosList[ptIdx], descPoseList[ptIdx], tool, user,
                        (float)vel, (float)acc, (float)ovl, (float)blendR, blendMode, epos,
                        search, offset_flag, offset, (float)oacc, velAccParamMode);
                    Console.WriteLine("  MoveL(33元素, 点位{0}) → rtn={1} {2}", ptIdx + 1, rtn, rtn == 0 ? "✓" : "✗");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("  异常: {0}", ex.Message);
                }
                Thread.Sleep(500);
            }

            // ===== 测试2: 34个元素 (追加1个额外元素), 使用点位1 =====
            Console.WriteLine("\n--- 测试2: 34个元素(多余1个), 使用点位1 ---");
            try
            {
                object[] arr34 = new object[34];
                FillMoveLArray(arr34, 33, jointPosList[0], descPoseList[0], epos, offset);
                arr34[33] = 999;
                var testProxy = CookComputing.XmlRpc.XmlRpcProxyGen.Create<ITestXmlrpcProxy>();
                testProxy.Url = "http://192.168.58.2:20003/RPC2";
                testProxy.Timeout = 1800000;
                int rtn = testProxy.MoveL_N(arr34);
                Console.WriteLine("  MoveL(34元素) → rtn={0} {1}", rtn,
                    rtn == 0 ? "通过(通配符*忽略多余元素)" : "失败");
            }
            catch (Exception ex)
            {
                Console.WriteLine("  异常: {0}", ex.Message);
            }

            // ===== 测试3: 35个元素 (追加2个额外元素), 使用点位1 =====
            Console.WriteLine("\n--- 测试3: 35个元素(多余2个), 使用点位2 ---");
            try
            {
                object[] arr35 = new object[35];
                FillMoveLArray(arr35, 33, jointPosList[1], descPoseList[1], epos, offset);
                arr35[33] = 999;
                arr35[34] = 888;
                var testProxy = CookComputing.XmlRpc.XmlRpcProxyGen.Create<ITestXmlrpcProxy>();
                testProxy.Url = "http://192.168.58.2:20003/RPC2";
                testProxy.Timeout = 1800000;
                int rtn = testProxy.MoveL_N(arr35);
                Console.WriteLine("  MoveL(35元素) → rtn={0} {1}", rtn,
                    rtn == 0 ? "通过(通配符*忽略多余元素)" : "失败");
            }
            catch (Exception ex)
            {
                Console.WriteLine("  异常: {0}", ex.Message);
            }
        }

        /// <summary>
        /// 填充MoveL参数数组
        /// </summary>
        private void FillMoveLArray(object[] arr, int count, JointPos jointPos, DescPose descPos,
            ExaxisPos epos, DescPose offset)
        {
            if (count > arr.Length) count = arr.Length;
            int idx = 0;
            if (idx < count) { for (int i = 0; i < 6 && idx < count; i++) arr[idx++] = (double)jointPos.jPos[i]; }
            else return;
            if (idx < count) { arr[idx++] = (double)descPos.tran.x; } else return;
            if (idx < count) { arr[idx++] = (double)descPos.tran.y; } else return;
            if (idx < count) { arr[idx++] = (double)descPos.tran.z; } else return;
            if (idx < count) { arr[idx++] = (double)descPos.rpy.rx; } else return;
            if (idx < count) { arr[idx++] = (double)descPos.rpy.ry; } else return;
            if (idx < count) { arr[idx++] = (double)descPos.rpy.rz; } else return;
            if (idx < count) arr[idx++] = 0;   // tool
            if (idx < count) arr[idx++] = 0;   // user
            if (idx < count) arr[idx++] = 100.0; // vel
            if (idx < count) arr[idx++] = 100.0; // acc
            if (idx < count) arr[idx++] = 100.0; // ovl
            if (idx < count) arr[idx++] = -1.0;  // blendR
            if (idx < count) arr[idx++] = 0;     // blendMode
            if (idx < count) { for (int i = 0; i < 4 && idx < count; i++) arr[idx++] = (double)epos.ePos[i]; }
            else return;
            if (idx < count) arr[idx++] = 0;   // search
            if (idx < count) arr[idx++] = 0;   // offset_flag
            if (idx < count) { arr[idx++] = (double)offset.tran.x; } else return;
            if (idx < count) { arr[idx++] = (double)offset.tran.y; } else return;
            if (idx < count) { arr[idx++] = (double)offset.tran.z; } else return;
            if (idx < count) { arr[idx++] = (double)offset.rpy.rx; } else return;
            if (idx < count) { arr[idx++] = (double)offset.rpy.ry; } else return;
            if (idx < count) { arr[idx++] = (double)offset.rpy.rz; } else return;
            if (idx < count) arr[idx++] = 100.0; // oacc
            if (idx < count) arr[idx++] = 0;     // velAccParamMode
        }

        /// <summary>
        /// UINT-058: SetTrajectoryJSpeed() 多版本参数兼容测试
        /// 正常参数: 2个 → 测试: 1个、2个、3个
        /// 少于需要参数时使用默认值, 多余时通配符忽略
        /// </summary>
        private void Test_UINT058_SetTrajectoryJSpeed_Compatibility()
        {
            Console.WriteLine("\n========== UINT-058: SetTrajectoryJSpeed() 多版本参数兼容测试 ==========");
            Console.WriteLine("当前版本参数: 2个 (double ovl, int mode=0)");

            var testProxy = CookComputing.XmlRpc.XmlRpcProxyGen.Create<ITestXmlrpcProxy>();
            testProxy.Url = "http://192.168.58.2:20003/RPC2";
            testProxy.Timeout = 1800000;

            // 测试1: 1个参数 (少于需要的2个, 应自动使用默认值)
            Console.WriteLine("\n--- 测试1: 1个参数(少1个, 应使用默认值) ---");
            try
            {
                int rtn = testProxy.SetTrajectoryJSpeed_1Param(100.0);
                Console.WriteLine("  SetTrajectoryJSpeed(100) → rtn={0}", rtn);
                Console.WriteLine("  结果: {0}", rtn == 0 ? "通过 (自动使用默认值)" : "需确认 (rtn={0})", rtn);
            }
            catch (Exception ex)
            {
                Console.WriteLine("  异常: {0} (可能是旧版本不支持)", ex.Message);
            }
            Thread.Sleep(5000);
            // 测试2: 正常2个参数
            Console.WriteLine("\n--- 测试2: 2个参数(正常) ---");
            try
            {
                int rtn = testProxy.SetTrajectoryJSpeed_2Params(80.0, 0);
                Console.WriteLine("  SetTrajectoryJSpeed(80, 0) → rtn={0}", rtn);
                Console.WriteLine("  结果: {0}", rtn == 0 ? "通过" : "需确认");
            }
            catch (Exception ex)
            {
                Console.WriteLine("  异常: {0}", ex.Message);
            }
            Thread.Sleep(5000);
            // 测试3: 3个参数 (多于需要的2个, 应通配符忽略)
            Console.WriteLine("\n--- 测试3: 3个参数(多余1个, 应忽略) ---");
            try
            {
                int rtn = testProxy.SetTrajectoryJSpeed_3Params(60.0, 0, 999);
                Console.WriteLine("  SetTrajectoryJSpeed(60, 0, 999) → rtn={0}", rtn);
                Console.WriteLine("  结果: {0}", rtn == 0 ? "通过 (通配符*忽略多余参数)" : "需确认 (rtn={0})", rtn);
            }
            catch (Exception ex)
            {
                Console.WriteLine("  异常: {0} (可能是旧版本不支持)", ex.Message);
            }
        }

        /// <summary>
        /// UINT-059: MoveL() 少参数默认值填充测试
        /// 使用GripperDropAlarmTest中的3个点位
        /// 正常元素数: 33个 → 测试: 31个(缺少oacc和velAccParamMode)
        /// 应自动通过数组元素个数判断, 将缺失参数设为默认值
        /// </summary>
        private void Test_UINT059_MoveL_FewerParams()
        {
            Console.WriteLine("\n========== UINT-059: MoveL() 少参数默认值填充测试 ==========");
            Console.WriteLine("当前版本MoveL数组元素: 33个");
            Console.WriteLine("测试: 31个元素 (缺少oacc加速度缩放因子, velAccParamMode物理速度模式切换)");
            Console.WriteLine("使用GripperDropAlarmTest中的3个点位");

            // ===== GripperDropAlarmTest 中的3个点位 =====
            JointPos[] jointPosList = new JointPos[3];
            DescPose[] descPoseList = new DescPose[3];

            // 点位1 (原点位2)
            jointPosList[0] = new JointPos(-151.316, -78.804, -126.568, -52.793, 79.482, -15.531);
            descPoseList[0] = new DescPose(-403.698, -83.798, 324.342, -171.154, -13.123, 132.102);

            // 点位2 (原点位1)
            jointPosList[1] = new JointPos(-156.550, -74.728, -121.418, -61.111, 80.596, -20.724);
            descPoseList[1] = new DescPose(-403.710, -46.124, 378.425, -171.145, -13.127, 132.100);
            // 点位3 (不变)
            jointPosList[2] = new JointPos(-151.316, -78.804, -126.568, -52.793, 79.482, -15.531);
            descPoseList[2] = new DescPose(-403.698, -83.798, 324.342, -171.154, -13.123, 132.102);

            ExaxisPos ep = new ExaxisPos(0, 0, 0, 0);
            DescPose off = new DescPose(0, 0, 0, 0, 0, 0);

            // 切换到手动模式
            robot.Mode(1);
            Thread.Sleep(500);

            // 遍历3个点位
            for (int ptIdx = 0; ptIdx < 3; ptIdx++)
            {
                Console.WriteLine("\n--- 点位{0}: 31个元素(少2个, 应使用默认值) ---", ptIdx + 1);
                try
                {
                    object[] arr31 = new object[31];
                    FillMoveLArray(arr31, 31, jointPosList[ptIdx], descPoseList[ptIdx], ep, off);

                    var testProxy = CookComputing.XmlRpc.XmlRpcProxyGen.Create<ITestXmlrpcProxy>();
                    testProxy.Url = "http://192.168.58.2:20003/RPC2";
                    testProxy.Timeout = 1800000;
                    int rtn = testProxy.MoveL_N(arr31);
                    Console.WriteLine("  MoveL(31元素, 点位{0}) → rtn={1} {2}", ptIdx + 1, rtn,
                        rtn == 0 ? "通过(缺失参数使用默认值)" : "需确认");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("  异常: {0}", ex.Message);
                }
                Thread.Sleep(500);
            }
        }

        // ========== 02 机器人基础指令测试 ==========

        /// <summary>
        /// UINT-060: 机器人基础控制指令测试 (Mode, RobotEnable, DragTeachSwitch)
        /// 验证Mode、RobotEnable等基础控制指令示例程序执行正常生效
        /// </summary>
        private void Test_UINT060_BasicCtrlCommands()
        {
            Console.WriteLine("\n========== UINT-060: 机器人基础控制指令测试 ==========");
            Console.WriteLine("复用 btnStandard_Click 中的测试逻辑");

            // ---- 复用 btnStandard_Click ----
            string ip = "";
            string version = "";
            byte state = 0;

            // GetSDKVersion
            robot.GetSDKVersion(ref version);
            Console.WriteLine("  SDK version : {0}", version);

            // GetControllerIP
            robot.GetControllerIP(ref ip);
            Console.WriteLine("  controller ip : {0}", ip);

            // Mode 手自动切换
            robot.Mode(1);
            Thread.Sleep(1000);
            Console.WriteLine("  Mode(1) 手动模式 ✓");

            // DragTeachSwitch 拖动示教
            robot.DragTeachSwitch(1);
            Console.WriteLine("  DragTeachSwitch(1) 进入拖动");
            int rtn = robot.IsInDragTeach(ref state);
            Console.WriteLine("  IsInDragTeach → rtn={0}, state={1} (1=拖动中)", rtn, state);
            Thread.Sleep(3000);
            robot.DragTeachSwitch(0);
            Console.WriteLine("  DragTeachSwitch(0) 退出拖动");
            Thread.Sleep(1000);
            robot.IsInDragTeach(ref state);
            Console.WriteLine("  IsInDragTeach → state={0} (0=非拖动)", state);

            // RobotEnable 使能
            robot.RobotEnable(0);
            Console.WriteLine("  RobotEnable(0) 去使能");
            Thread.Sleep(3000);
            robot.RobotEnable(1);
            Console.WriteLine("  RobotEnable(1) 上使能");

            // Mode 自动→手动
            robot.Mode(0);
            Console.WriteLine("  Mode(0) 自动模式");
            Thread.Sleep(1000);
            robot.Mode(1);
            Console.WriteLine("  Mode(1) 手动模式");

            Console.WriteLine("\n  UINT-060 测试完成");
        }

        /// <summary>
        /// UINT-061: 获取机器人软固件版本测试
        /// 验证GetSDKVersion、GetSoftwareVersion、GetHardwareVersion、GetFirmwareVersion正常生效
        /// </summary>
        private void Test_UINT061_GetVersion()
        {
            Console.WriteLine("\n========== UINT-061: 获取机器人软固件版本测试 ==========");
            Console.WriteLine("复用 btnGetVersions_Click 中的测试逻辑");

            // ---- 复用 btnGetVersions_Click ----
            string[] ver = new string[20];

            robot.GetSoftwareVersion(ref ver[0], ref ver[1], ref ver[2]);
            robot.GetHardwareVersion(ref ver[3], ref ver[4], ref ver[5], ref ver[6], ref ver[7], ref ver[8], ref ver[9], ref ver[10]);
            robot.GetFirmwareVersion(ref ver[11], ref ver[12], ref ver[13], ref ver[14], ref ver[15], ref ver[16], ref ver[17], ref ver[18]);

            Console.WriteLine("  --- 软件版本 ---");
            Console.WriteLine("  robotModel        : {0}", ver[0]);
            Console.WriteLine("  webVersion        : {0}", ver[1]);
            Console.WriteLine("  controllerVersion : {0}", ver[2]);
            Console.WriteLine("  --- 硬件版本 ---");
            Console.WriteLine("  ctrlBox  : {0}", ver[3]);
            Console.WriteLine("  driver1  : {0}", ver[4]);
            Console.WriteLine("  driver2  : {0}", ver[5]);
            Console.WriteLine("  driver3  : {0}", ver[6]);
            Console.WriteLine("  driver4  : {0}", ver[7]);
            Console.WriteLine("  driver5  : {0}", ver[8]);
            Console.WriteLine("  driver6  : {0}", ver[9]);
            Console.WriteLine("  endBoard : {0}", ver[10]);
            Console.WriteLine("  --- 固件版本 ---");
            Console.WriteLine("  ctrlBox  : {0}", ver[11]);
            Console.WriteLine("  driver1  : {0}", ver[12]);
            Console.WriteLine("  driver2  : {0}", ver[13]);
            Console.WriteLine("  driver3  : {0}", ver[14]);
            Console.WriteLine("  driver4  : {0}", ver[15]);
            Console.WriteLine("  driver5  : {0}", ver[16]);
            Console.WriteLine("  driver6  : {0}", ver[17]);
            Console.WriteLine("  endBoard : {0}", ver[18]);

            Console.WriteLine("\n  UINT-061 测试完成");
        }

        /// <summary>
        /// 02 机器人基础指令测试 - 总入口
        // ========== 15 测试实例 ==========

        /// <summary>
        /// 15测试实例-Row3: SetTrajectoryJSpeed() 多参数兼容测试
        /// 步骤完全按照excel Row3执行: 1个→3个→2个参数
        /// </summary>
        private void Test_Instance_SetTrajectoryJSpeed()
        {
            Console.WriteLine("\n========== 测试实例: SetTrajectoryJSpeed 多参数兼容 ==========");
            Console.WriteLine("验证多参数xmlrpc接口兼容多版本参数功能正常生效");

            var testProxy = CookComputing.XmlRpc.XmlRpcProxyGen.Create<ITestXmlrpcProxy>();
            testProxy.Url = "http://192.168.58.2:20003/RPC2";
            testProxy.Timeout = 1800000;

            // 步骤1: 1个参数 (当前版本实际参数为2个, 应自动将mode设为0)
            Console.WriteLine("\n--- 步骤1: SetTrajectoryJSpeed(ovl) 1个参数 ---");
            Console.WriteLine("期望: 自动将mode默认设为0(降速模式)");
            try
            {
                int rtn = testProxy.SetTrajectoryJSpeed_1Param(100.0);
                Console.WriteLine("  SetTrajectoryJSpeed(100) → rtn={0} {1}", rtn,
                    rtn == 0 ? "通过(自动使用默认值)" : "需确认");
            }
            catch (Exception ex)
            {
                Console.WriteLine("  异常: {0}", ex.Message);
            }
            Thread.Sleep(500);

            // 步骤2: 3个参数 (当前版本实际参数为2个, 应自动丢弃最后一个)
            Console.WriteLine("\n--- 步骤2: SetTrajectoryJSpeed(ovl, mode, extra) 3个参数 ---");
            Console.WriteLine("期望: 自动丢弃最后一个参数");
            try
            {
                int rtn = testProxy.SetTrajectoryJSpeed_3Params(100.0, 0, 999);
                Console.WriteLine("  SetTrajectoryJSpeed(100, 0, 999) → rtn={0} {1}", rtn,
                    rtn == 0 ? "通过(自动丢弃多余参数)" : "需确认");
            }
            catch (Exception ex)
            {
                Console.WriteLine("  异常: {0}", ex.Message);
            }
            Thread.Sleep(500);

            // 步骤3: 2个参数 (正常)
            Console.WriteLine("\n--- 步骤3: SetTrajectoryJSpeed(ovl, mode) 2个参数(正常) ---");
            Console.WriteLine("期望: 正常运行");
            try
            {
                int rtn = testProxy.SetTrajectoryJSpeed_2Params(100.0, 0);
                Console.WriteLine("  SetTrajectoryJSpeed(100, 0) → rtn={0} {1}", rtn,
                    rtn == 0 ? "通过" : "需确认");
            }
            catch (Exception ex)
            {
                Console.WriteLine("  异常: {0}", ex.Message);
            }

            Console.WriteLine("\n  测试实例 SetTrajectoryJSpeed 完成");
        }

        /// <summary>
        /// 15测试实例-Row4: MoveL() 单数组参数个数兼容测试
        /// 步骤完全按照excel Row4: 31个→34个→33个元素
        /// 使用GripperDropAlarmTest中的3个点位
        /// </summary>
        private void Test_Instance_MoveL_ArrayCompat()
        {
            Console.WriteLine("\n========== 测试实例: MoveL 单数组参数个数兼容 ==========");
            Console.WriteLine("验证单数组参数传递接口数组元素个数判断兼容多版本功能正常生效");
            Console.WriteLine("使用GripperDropAlarmTest中的3个点位");

            // ===== GripperDropAlarmTest 点位 =====
            JointPos[] jps = new JointPos[3];
            DescPose[] dps = new DescPose[3];
            // 点位1 (原点位2)
            jps[0] = new JointPos(-151.316, -78.804, -126.568, -52.793, 79.482, -15.531);
            dps[0] = new DescPose(-403.698, -83.798, 324.342, -171.154, -13.123, 132.102);
            // 点位2 (原点位1)
            jps[1] = new JointPos(-156.550, -74.728, -121.418, -61.111, 80.596, -20.724);
            dps[1] = new DescPose(-403.710, -46.124, 378.425, -171.145, -13.127, 132.100);
            // 点位3 (不变)
            jps[2] = new JointPos(-151.316, -78.804, -126.568, -52.793, 79.482, -15.531);
            dps[2] = new DescPose(-403.698, -83.798, 324.342, -171.154, -13.123, 132.102);

            ExaxisPos ep = new ExaxisPos(0, 0, 0, 0);
            DescPose off = new DescPose(0, 0, 0, 0, 0, 0);

            robot.Mode(1);
            Thread.Sleep(500);

            var testProxy = CookComputing.XmlRpc.XmlRpcProxyGen.Create<ITestXmlrpcProxy>();
            testProxy.Url = "http://192.168.58.2:20003/RPC2";
            testProxy.Timeout = 1800000;

            // ---- 步骤1: 31个元素 (缺少oacc和velAccParamMode, 应自动使用默认值) ----
            Console.WriteLine("\n--- 步骤1: MoveL 31个元素 (少2个) ---");
            Console.WriteLine("期望: 自动将oacc=ovl, velAccParamMode=0");
            for (int ptIdx = 0; ptIdx < 3; ptIdx++)
            {
                Console.WriteLine("  点位{0}...", ptIdx + 1);
                try
                {
                    object[] arr31 = new object[31];
                    FillMoveLArray(arr31, 31, jps[ptIdx], dps[ptIdx], ep, off);
                    int rtn = testProxy.MoveL_N(arr31);
                    Console.WriteLine("  MoveL(31, 点位{0}) → rtn={1} {2}", ptIdx + 1, rtn,
                        rtn == 0 ? "通过" : "需确认");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("  异常: {0}", ex.Message);
                }
                Thread.Sleep(500);
            }

            // ---- 步骤2: 34个元素 (多余1个, 应自动舍弃) ----
            Console.WriteLine("\n--- 步骤2: MoveL 34个元素 (多余1个) ---");
            Console.WriteLine("期望: 自动舍弃第34个多余参数");
            for (int ptIdx = 0; ptIdx < 3; ptIdx++)
            {
                Console.WriteLine("  点位{0}...", ptIdx + 1);
                try
                {
                    object[] arr34 = new object[34];
                    FillMoveLArray(arr34, 33, jps[ptIdx], dps[ptIdx], ep, off);
                    arr34[33] = 999;
                    int rtn = testProxy.MoveL_N(arr34);
                    Console.WriteLine("  MoveL(34, 点位{0}) → rtn={1} {2}", ptIdx + 1, rtn,
                        rtn == 0 ? "通过" : "需确认");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("  异常: {0}", ex.Message);
                }
                Thread.Sleep(500);
            }

            // ---- 步骤3: 33个元素 (正常) ----
            Console.WriteLine("\n--- 步骤3: MoveL 33个元素 (正常) ---");
            Console.WriteLine("期望: 正常运行");
            for (int ptIdx = 0; ptIdx < 3; ptIdx++)
            {
                Console.WriteLine("  点位{0}...", ptIdx + 1);
                try
                {
                    int rtn = robot.MoveL(jps[ptIdx], dps[ptIdx], 0, 0, 100f, 100f, 100f,
                        -1f, 0, ep, 0, 0, off, 100f, 0);
                    Console.WriteLine("  MoveL(33, 点位{0}) → rtn={1} {2}", ptIdx + 1, rtn,
                        rtn == 0 ? "通过" : "需确认");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("  异常: {0}", ex.Message);
                }
                Thread.Sleep(500);
            }

            Console.WriteLine("\n  测试实例 MoveL 数组兼容 完成");
        }

        /// <summary>
        /// 15 测试实例 - 总入口
        /// 依次执行 SetTrajectoryJSpeed 和 MoveL 兼容测试实例
        /// </summary>
        public void TestInstanceTest()
        {
            Console.WriteLine("============================================================");
            Console.WriteLine("  15 测试实例 (多版本xmlrpc接口兼容)");
            Console.WriteLine("  SDK版本: V3.9.7  目标机器人: QX/LA397");
            Console.WriteLine("============================================================");

            if (robot == null)
            {
                Console.WriteLine("ERROR: 机器人未连接!");
                return;
            }

            Test_Instance_SetTrajectoryJSpeed();
            Thread.Sleep(500);

            //Test_Instance_MoveL_ArrayCompat();

            Console.WriteLine("\n============================================================");
            Console.WriteLine("  15 测试实例 完成");
            Console.WriteLine("============================================================");
        }

        /// <summary>
        /// 依次执行 UINT-060 ~ UINT-061
        /// </summary>
        public void BasicCommandTest()
        {
            Console.WriteLine("============================================================");
            Console.WriteLine("  02 机器人基础指令测试 (FRC10-SDK-UINT-060 ~ 061)");
            Console.WriteLine("  SDK版本: V3.9.7  目标机器人: QX/LA397");
            Console.WriteLine("============================================================");

            if (robot == null)
            {
                Console.WriteLine("ERROR: 机器人未连接!");
                return;
            }

            Test_UINT060_BasicCtrlCommands();
            Thread.Sleep(500);

            Test_UINT061_GetVersion();

            Console.WriteLine("\n============================================================");
            Console.WriteLine("  02 机器人基础指令测试 完成");
            Console.WriteLine("============================================================");
        }

        /// <summary>
        /// 01 xmlrpc接口兼容测试 - 总入口
        /// 依次执行 UINT-056 ~ UINT-059 四项测试
        /// </summary>
        public void XmlrpcCompatibilityTest()
        {
            Console.WriteLine("============================================================");
            Console.WriteLine("  01 xmlrpc接口兼容测试 (FRC10-SDK-UINT-056 ~ 059)");
            Console.WriteLine("  基于: RD36-机器人SDK多版本xmlrpc接口参数兼容集成测试方案");
            Console.WriteLine("  SDK版本: V3.9.7  目标机器人: QX/LA397");
            Console.WriteLine("============================================================");

            // 确认机器人已连接
            if (robot == null)
            {
                Console.WriteLine("ERROR: 机器人未连接!");
                return;
            }

            // UINT-056: Mode() 多参数通配符
            //Test_UINT056_Mode_Compatibility();
            //Thread.Sleep(500);

            //// UINT-057: MoveL() 数组元素个数兼容
            Test_UINT057_MoveL_ArrayCompatibility();
            Thread.Sleep(500);

            //// UINT-058: SetTrajectoryJSpeed() 多版本参数兼容
            Test_UINT058_SetTrajectoryJSpeed_Compatibility();
            Thread.Sleep(500);

            // UINT-059: MoveL() 少参数默认值填充
            Test_UINT059_MoveL_FewerParams();

            Console.WriteLine("\n============================================================");
            Console.WriteLine("  01 xmlrpc接口兼容测试 完成");
            Console.WriteLine("============================================================");
        }

        public void TestWeaveSpeedAndOffset()
        {
            Console.WriteLine("============================================================");
            Console.WriteLine("  Weave Speed and Offset Test");
            Console.WriteLine("============================================================");

            if (robot == null)
            {
                Console.WriteLine("ERROR: Robot not connected!");
                return;
            }

            int rtn;
            ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);

            JointPos j1 = new JointPos(5.027, -84.331, -75.139, -103.690, 86.379, 20.794);
            DescPose d1 = new DescPose(324.752, -83.339, 366.314, -172.321, -0.936, -106.047);

            JointPos j2 = new JointPos(-35.335, -117.598, -57.174, -95.234, 90.001, -19.560);
            DescPose d2 = new DescPose(324.999, -355.439, 260.000, 179.995, 0.003, -105.775);

            JointPos j3 = new JointPos(59.787, -117.594, -57.183, -95.222, 90.006, 75.562);
            DescPose d3 = new DescPose(324.998, 355.441, 260.002, 179.995, 0.003, -105.775);

            // ---- Step 1: MoveJ to start point ----
            Console.WriteLine("\nStep 1: MoveJ to start point");
            rtn = robot.MoveJ(j1, d1, 1, 0, 100, 100, 50, epos, -1, 0, offset_pos);
            Console.WriteLine("  MoveJ(j1) rtn={0}", rtn);
            Thread.Sleep(500);

            // ---- Step 2: MoveJ to weave entry ----
            Console.WriteLine("\nStep 2: MoveJ to weave entry point");
            rtn = robot.MoveJ(j2, d2, 1, 0, 100, 100, 50, epos, -1, 0, offset_pos);
            Console.WriteLine("  MoveJ(j2) rtn={0}", rtn);
            Thread.Sleep(500);

            // ---- Step 3: WeaveStart, launch weave MoveL thread ----
            Console.WriteLine("\nStep 3: WeaveStart + MoveL in background thread");
            robot.WeaveStart(0);

            bool weaveRunning = true;
            Thread weaveThread = new Thread(() =>
            {
                rtn = robot.MoveL(j3, d3, 1, 0, 100, 100, 5, -1, 0, epos, 0, 0, offset_pos, 5, 0, 0, 10);
                Console.WriteLine("  MoveL(weave) thread finished, rtn={0}", rtn);
                weaveRunning = false;
            });
            weaveThread.IsBackground = true;
            weaveThread.Start();
            Thread.Sleep(500);  // Wait for motion to start

            // ---- Step 4: Speed test (main thread, weave MoveL in background) ----
            Console.WriteLine("\nStep 4: SetSpeed test during weaving");
            int[] speedValues = { 20, 50, 80, 30, 60, 10 };
            foreach (int speed in speedValues)
            {
                if (!weaveRunning) break;
                rtn = robot.SetSpeedInstant(speed);
                robot.GetRobotRealTimeState(ref pkg);
                Console.WriteLine("  SetSpeed({0}) -> rtn={1}, TCP_CmpSpeed={2}", speed, rtn, pkg.target_TCP_CmpSpeed);
                Thread.Sleep(5000);
            }


            Thread.Sleep(5000);
            // ---- Step 5: SetWeaveOffsetRT offset test (main thread, weave MoveL in background) ----
            Console.WriteLine("\nStep 5: SetWeaveOffsetRT test (50 iterations, delta=0.1)");
            double accumOffset = 0.0;
            for (int i = 0; i < 50 && weaveRunning; i++)
            {
                accumOffset += 0.1;
                DescPose weaveOffset = new DescPose(0, 0, accumOffset, 0, 0, 0);
                rtn = robot.SetWeaveOffsetRT(weaveOffset);
                robot.GetRobotRealTimeState(ref pkg);
                Console.WriteLine("  [{0}/50] SetWeaveOffsetRT(x={1:F1}) -> rtn={2}, TCP_pos=({3:F2},{4:F2},{5:F2})",
                    i + 1, accumOffset, rtn,
                    pkg.tl_cur_pos[0], pkg.tl_cur_pos[1], pkg.tl_cur_pos[2]);
                Thread.Sleep(100);
            }

            // ---- Step 6: Wait for weave MoveL, then WeaveEnd ----
            Console.WriteLine("\nStep 6: Wait for weave MoveL, then WeaveEnd");
            weaveThread.Join();
            robot.WeaveEnd(0);
            Thread.Sleep(500);

            // ---- Step 7: MoveL back to start ----
            Console.WriteLine("\nStep 7: MoveL back to start");
            rtn = robot.MoveL(j1, d1, 1, 0, 100, 100, 50, -1, 0, epos, 0, 0, offset_pos, 50, 0, 0, 10);
            Console.WriteLine("  MoveL(back) rtn={0}", rtn);

            robot.GetRobotRealTimeState(ref pkg);
            Console.WriteLine("\n  Final robot state: main_code={0}, sub_code={1}", pkg.main_code, pkg.sub_code);
            Console.WriteLine("============================================================");
            Console.WriteLine("  Weave Speed and Offset Test Complete");
            Console.WriteLine("============================================================");
        }

        /// <summary>
        /// 测试工件坐标系点位转换 (WorkPieceTrsfStart/End)
        /// 在坐标系1执行运动 → 切换到坐标系2 → 重复相同运动 → 结束转换
        /// </summary>
        public int TestWorkPieceTrsf()
        {
            Console.WriteLine("\n========== 工件坐标系点位转换测试 ==========");

            // ---- 点位定义 (与 C++ 一致) ----
            JointPos j1 = new JointPos(-11.188, -64.165, -107.299, -76.706, 89.590, 92.983);
            DescPose d1 = new DescPose(225.986, 190.694, 394.238, -6.230, -23.797, -98.972);
            JointPos j2 = new JointPos(-38.148, -97.408, -133.704, -30.999, 89.584, 92.986);
            DescPose d2 = new DescPose(52.741, 262.917, 30.824, -5.696, -9.864, -126.092);
            JointPos j3 = new JointPos(-25.561, -123.131, -85.736, -94.911, 89.582, 93.006);
            DescPose d3 = new DescPose(70.455, 88.410, 45.299, -4.101, 31.775, -113.199);
            JointPos j4 = new JointPos(-8.013, -125.881, -79.196, -84.440, 89.564, 93.005);
            DescPose d4 = new DescPose(209.453, -73.895, 56.416, -4.727, 17.523, -95.906);
            JointPos j5 = new JointPos(-2.722, -94.518, -119.965, -54.518, 89.563, 93.005);
            DescPose d5 = new DescPose(274.800, 81.106, 102.977, -5.467, -2.980, -90.711);
            JointPos j6 = new JointPos(-2.671, -56.234, -138.914, -25.099, 95.355, 92.967);
            DescPose d6 = new DescPose(300.392, 177.281, 300.926, -1.909, -51.894, -89.703);
            JointPos j7 = new JointPos(-1.229, -121.184, -63.201, -122.331, 93.045, 93.019);
            DescPose d7 = new DescPose(296.856, -31.294, 215.698, -0.589, 34.594, -88.954);

            ExaxisPos ex = new ExaxisPos(0, 0, 0, 0);
            DescPose zeroOff = new DescPose(0, 0, 0, 0, 0, 0);

            int tool = 1;
            int workpiece = 1;
            float blend = 5.0f;

            // ===== 坐标系1 =====
            // Home
            robot.MoveJ(j1, d1, tool, workpiece, 100, 100, 100, ex, -1, 0, zeroOff);
            // PTP
            robot.MoveJ(j2, d2, tool, workpiece, 100, 100, 100, ex, blend, 0, zeroOff);
            // LIN
            robot.MoveL(j3, d3, tool, workpiece, 10, 100, 100, blend, 0, ex, 0, 1, zeroOff, 0, 90);
            // ARC
            robot.MoveC(j4, d4, tool, workpiece, 100, 100, ex, 0, zeroOff,
                        j5, d5, tool, workpiece, 100, 100, ex, 0, zeroOff,
                        10, blend, 100, 0);
            // CIR
            robot.Circle(j6, d6, tool, workpiece, 100, 100, ex,
                         j7, d7, tool, workpiece, 100, 100, ex,
                         10, 0, zeroOff, 100.0, blend, 0);

            // ===== WorkPieceTrsfStart(2) =====
            int rtn = robot.WorkPieceTrsfStart(2);
            Console.WriteLine("  WorkPieceTrsfStart(2) rtn={0}", rtn);

            // ===== 坐标系2 (转换后) =====
            robot.MoveJ(j1, d1, tool, workpiece, 100, 100, 100, ex, -1, 0, zeroOff);
            robot.MoveJ(j2, d2, tool, workpiece, 100, 100, 100, ex, blend, 0, zeroOff);
            robot.MoveL(j3, d3, tool, workpiece, 10, 100, 100, blend, 0, ex, 0, 1, zeroOff, 0, 90);
            robot.MoveC(j4, d4, tool, workpiece, 100, 100, ex, 0, zeroOff,
                        j5, d5, tool, workpiece, 100, 100, ex, 0, zeroOff,
                        10, blend, 100, 0);
            robot.Circle(j6, d6, tool, workpiece, 100, 100, ex,
                         j7, d7, tool, workpiece, 100, 100, ex,
                         10, 0, zeroOff, 100.0, blend, 0);

            // ===== WorkPieceTrsfEnd =====
            rtn = robot.WorkPieceTrsfEnd();
            Console.WriteLine("  WorkPieceTrsfEnd() rtn={0}", rtn);

            //robot.CloseRPC();
            Console.WriteLine("\n========== 工件坐标系点位转换测试完成 ==========");
            return rtn;
        }

        /// <summary>
        /// 测试静止跟踪 (SetStationaryTrackPara + MoveStationary)
        /// SetDO(6,1) → ConveyorTrackStart → ConveyorIODetect → ConveyorGetTrackData
        /// → SetStationaryTrackPara → MoveStationary → ConveyorTrackEnd → SetDO(6,0)
        /// </summary>
        public int TestStationaryTrack()
        {
            Console.WriteLine("\n========== 传送带静止跟踪测试 ==========");

            int rtn;

            JointPos j1 = new JointPos(-35.146, -102.684, 120.805, -100.401, -90.295, 150.105);
            DescPose d1 = new DescPose(-121.814, -348.341, 209.978, -173.152, -3.585, -5.446);

            ExaxisPos ex = new ExaxisPos(0, 0, 0, 0);
            DescPose zeroOff = new DescPose(0, 0, 0, 0, 0, 0);

            int tool = 1;
            int workpiece = 1;

            rtn = robot.ConveyorSetParam(0, 10000, 200, 0, 0, 10);


            robot.MoveJ(j1, d1, tool, workpiece, 100, 100, 100, ex, -1, 0, zeroOff);

            // Step 1: SetDO 控制信号
            Console.WriteLine("--- Step 1: SetDO(6,1) ---");
            rtn = robot.SetDO(6, 1, 0, 0);
            Console.WriteLine("  SetDO(6,1) rtn={0}", rtn);

            // Step 2: 传送带跟踪开始
            Console.WriteLine("--- Step 2: ConveyorTrackStart(2) ---");
            rtn = robot.ConveyorTrackStart(2);
            Console.WriteLine("  ConveyorTrackStart(2) rtn={0}", rtn);

            // Step 3: 工件IO检测
            Console.WriteLine("--- Step 3: ConveyorIODetect(10000) ---");
            rtn = robot.ConveyorIODetect(10000);
            Console.WriteLine("  ConveyorIODetect(10000) rtn={0}", rtn);

            // Step 4: 获取跟踪数据
            Console.WriteLine("--- Step 4: ConveyorGetTrackData(2) ---");
            rtn = robot.ConveyorGetTrackData(2);
            Console.WriteLine("  ConveyorGetTrackData(2) rtn={0}", rtn);

            // Step 5: 静止跟踪参数配置 (时间模式, 200s, 距离5)
            Console.WriteLine("--- Step 5: SetStationaryTrackPara(0,200,5) ---");
            rtn = robot.SetStationaryTrackPara(0, 5, 5);
            Console.WriteLine("  SetStationaryTrackPara(0,200,5) rtn={0}", rtn);

            // Step 6: 执行静止跟踪运动
            Console.WriteLine("--- Step 6: MoveStationary() ---");
            rtn = robot.MoveStationary();
            Console.WriteLine("  MoveStationary() rtn={0}", rtn);

            // Step 7: 传送带跟踪结束
            Console.WriteLine("--- Step 7: ConveyorTrackEnd() ---");
            rtn = robot.ConveyorTrackEnd();
            Console.WriteLine("  ConveyorTrackEnd() rtn={0}", rtn);

            // Step 8: SetDO 关闭信号
            Console.WriteLine("--- Step 8: SetDO(6,0) ---");
            rtn = robot.SetDO(6, 0, 0, 0);
            Console.WriteLine("  SetDO(6,0) rtn={0}", rtn);

            Console.WriteLine("\n========== 静止跟踪测试完成 ==========");
            return 0;
        }

        /// <summary>
        /// 测试坐标系查询函数 (Get*WithID / GetCur* / Set*)
        /// </summary>
        public int TestCoord()
        {

            int rtn;
            int id = 1;

            // GetToolCoordWithID
            DescPose toolCoord = new DescPose(0, 0, 0, 0, 0, 0);
            int type = 0, install = 0, toolID = 0, loadNo = 0;
            rtn = robot.GetToolCoordWithID(id, ref toolCoord, ref type, ref install, ref toolID, ref loadNo);
            Console.WriteLine("GetToolCoordWithID {0}, {1:F3} {2:F3} {3:F3} {4:F3} {5:F3} {6:F3}, type={7}, install={8}, toolID={9}, loadNo={10}",
                id, toolCoord.tran.x, toolCoord.tran.y, toolCoord.tran.z,
                toolCoord.rpy.rx, toolCoord.rpy.ry, toolCoord.rpy.rz, type, install, toolID, loadNo);

            // GetWObjCoordWithID
            DescPose wobjCoord = new DescPose(0, 0, 0, 0, 0, 0);
            int refFrame = 0;
            rtn = robot.GetWObjCoordWithID(id, ref wobjCoord, ref refFrame);
            Console.WriteLine("GetWObjCoordWithID {0}, {1:F3} {2:F3} {3:F3} {4:F3} {5:F3} {6:F3}, refFrame={7}",
                id, wobjCoord.tran.x, wobjCoord.tran.y, wobjCoord.tran.z,
                wobjCoord.rpy.rx, wobjCoord.rpy.ry, wobjCoord.rpy.rz, refFrame);

            // GetExToolCoordWithID
            DescPose extoolCoord = new DescPose(0, 0, 0, 0, 0, 0);
            DescPose exworkpieceCoord = new DescPose(0, 0, 0, 0, 0, 0);
            rtn = robot.GetExToolCoordWithID(21, ref extoolCoord, ref exworkpieceCoord);
            Console.WriteLine("GetExToolCoordWithID 21, {0:F3} {1:F3} {2:F3} {3:F3} {4:F3} {5:F3}",
                extoolCoord.tran.x, extoolCoord.tran.y, extoolCoord.tran.z,
                extoolCoord.rpy.rx, extoolCoord.rpy.ry, extoolCoord.rpy.rz);
            Console.WriteLine("  tcoord: {0:F3} {1:F3} {2:F3} {3:F3} {4:F3} {5:F3}",
                exworkpieceCoord.tran.x, exworkpieceCoord.tran.y, exworkpieceCoord.tran.z,
                exworkpieceCoord.rpy.rx, exworkpieceCoord.rpy.ry, exworkpieceCoord.rpy.rz);

            // GetExAxisCoordWithID
            DescPose exAxisCoord = new DescPose(0, 0, 0, 0, 0, 0);
            int axisCoordNum = 0, calibFlag = 0;
            rtn = robot.GetExAxisCoordWithID(id, ref exAxisCoord, ref axisCoordNum, ref calibFlag);
            Console.WriteLine("GetExAxisCoordWithID {0}, {1:F3} {2:F3} {3:F3} {4:F3} {5:F3} {6:F3}, axisCoordNum={7}, calibFlag={8}",
                id, exAxisCoord.tran.x, exAxisCoord.tran.y, exAxisCoord.tran.z,
                exAxisCoord.rpy.rx, exAxisCoord.rpy.ry, exAxisCoord.rpy.rz, axisCoordNum, calibFlag);

            // GetTargetPayloadWithID
            double weight = 0.0;
            DescTran cog = new DescTran(0, 0, 0);
            rtn = robot.GetTargetPayloadWithID(id, ref weight, ref cog);
            Console.WriteLine("GetTargetPayloadWithID {0}, {1:F3} {2:F3} {3:F3} {4:F3}",
                id, weight, cog.x, cog.y, cog.z);

            // GetCurToolCoord
            rtn = robot.GetCurToolCoord(ref toolCoord);
            Console.WriteLine("GetCurToolCoord {0:F3} {1:F3} {2:F3} {3:F3} {4:F3} {5:F3}",
                toolCoord.tran.x, toolCoord.tran.y, toolCoord.tran.z,
                toolCoord.rpy.rx, toolCoord.rpy.ry, toolCoord.rpy.rz);

            // GetCurWObjCoord
            rtn = robot.GetCurWObjCoord(ref wobjCoord);
            Console.WriteLine("GetCurWObjCoord {0:F3} {1:F3} {2:F3} {3:F3} {4:F3} {5:F3}",
                wobjCoord.tran.x, wobjCoord.tran.y, wobjCoord.tran.z,
                wobjCoord.rpy.rx, wobjCoord.rpy.ry, wobjCoord.rpy.rz);

            // GetCurExToolCoord
            rtn = robot.GetCurExToolCoord(ref extoolCoord);
            Console.WriteLine("GetCurExToolCoord {0:F3} {1:F3} {2:F3} {3:F3} {4:F3} {5:F3}",
                extoolCoord.tran.x, extoolCoord.tran.y, extoolCoord.tran.z,
                extoolCoord.rpy.rx, extoolCoord.rpy.ry, extoolCoord.rpy.rz);

            // GetCurExAxisCoord
            rtn = robot.GetCurExAxisCoord(ref exAxisCoord);
            Console.WriteLine("GetCurExAxisCoord {0:F3} {1:F3} {2:F3} {3:F3} {4:F3} {5:F3}",
                exAxisCoord.tran.x, exAxisCoord.tran.y, exAxisCoord.tran.z,
                exAxisCoord.rpy.rx, exAxisCoord.rpy.ry, exAxisCoord.rpy.rz);

            // GetTargetPayload / GetTargetPayloadCog
            double weightT = 0.0;
            DescTran cogT = new DescTran(0, 0, 0);
            robot.GetTargetPayload(0, ref weightT);
            robot.GetTargetPayloadCog(0, ref cogT);
            Console.WriteLine("GetTargetPayload {0:F3} {1:F3} {2:F3} {3:F3}",
                weightT, cogT.x, cogT.y, cogT.z);

            // SetToolCoord
            DescPose coordSet = new DescPose(0, 1, 2, 3, 4, 5);
            rtn = robot.SetToolCoord(1, coordSet, 0, 0, 1, 0);
            Console.WriteLine("SetToolCoord(1) rtn={0}", rtn);

            // SetWObjCoord
            rtn = robot.SetWObjCoord(1, coordSet, 0);
            Console.WriteLine("SetWObjCoord(1) rtn={0}", rtn);

            // SetLoadWeight + SetLoadCoord
            rtn = robot.SetLoadWeight(1, 1.3f);
            Console.WriteLine("SetLoadWeight(1,1.3) rtn={0}", rtn);

            DescTran loadCog = new DescTran(10, 20, 30);
            rtn = robot.SetLoadCoord(1, loadCog);
            Console.WriteLine("SetLoadCoord(1,10,20,30) rtn={0}", rtn);

            // SetExToolCoord
            DescPose etcp = new DescPose(0, 0, 100, 0, 0, 0);
            DescPose etool = new DescPose(0, 0, 50, 0, 0, 0);
            rtn = robot.SetExToolCoord(21, etcp, etool);
            Console.WriteLine("SetExToolCoord(21) rtn={0}", rtn);

            // ExtAxisActiveECoordSys
            rtn = robot.ExtAxisActiveECoordSys(1, 1, coordSet, 1);
            Console.WriteLine("ExtAxisActiveECoordSys(1,1,..,1) rtn={0}", rtn);

            return 0;
        }

    }

    /// <summary>
    /// xmlrpc兼容测试专用代理接口
    /// 用于发送不同参数个数的xmlrpc调用, 测试服务端通配符/默认值兼容性
    /// </summary>
    [CookComputing.XmlRpc.XmlRpcUrl("http://192.168.58.2:20003/RPC2")]
    public interface ITestXmlrpcProxy : CookComputing.XmlRpc.IXmlRpcProxy
    {
        // === Mode 多参数测试 (UINT-056) ===
        [CookComputing.XmlRpc.XmlRpcMethod("Mode")]
        int Mode_1Param(int mode);

        [CookComputing.XmlRpc.XmlRpcMethod("Mode")]
        int Mode_2Params(int mode, int extra1);

        [CookComputing.XmlRpc.XmlRpcMethod("Mode")]
        int Mode_3Params(int mode, int extra1, int extra2);

        // === MoveL 数组测试 (UINT-057 / UINT-059) ===
        [CookComputing.XmlRpc.XmlRpcMethod("MoveL")]
        int MoveL_N(object[] moveLParams);

        // === SetTrajectoryJSpeed 多参数测试 (UINT-058) ===
        [CookComputing.XmlRpc.XmlRpcMethod("SetTrajectoryJSpeed")]
        int SetTrajectoryJSpeed_1Param(double ovl);

        [CookComputing.XmlRpc.XmlRpcMethod("SetTrajectoryJSpeed")]
        int SetTrajectoryJSpeed_2Params(double ovl, int mode);

        [CookComputing.XmlRpc.XmlRpcMethod("SetTrajectoryJSpeed")]
        int SetTrajectoryJSpeed_3Params(double ovl, int mode, int extra);
    }
 }


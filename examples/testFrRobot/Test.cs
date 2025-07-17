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
            //bool saveFlag=false;

            //int rtn = 0;

            //JointPos p1Joint = new JointPos(88.927, -85.834, 80.289, -85.561, -91.388, 108.718);
            //DescPose p1Desc = new DescPose(88.739, -527.617, 514.939, -179.039, 1.494, 70.209);

            //JointPos p2Joint = new JointPos(27.036, -83.909, 80.284, -85.579, -90.027, 108.604);
            //DescPose p2Desc = new DescPose(-433.125, -334.428, 497.139, -179.723, -0.745, 8.437);

            //ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);


            //robot.AccSmoothStart(saveFlag);

            //robot.MoveJ(p1Joint, p1Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            //robot.MoveJ(p2Joint, p2Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            //robot.MoveJ(p1Joint, p1Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            //robot.MoveJ(p2Joint, p2Desc, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);

            //robot.AccSmoothEnd(saveFlag);

            bool saveFlag = false;

            int rtn = 0;

            DescPose p1Desc = new DescPose(-319.303, -240.689, 116.379, -175.879, -0.337, 148.239);
            JointPos p1Joint = new JointPos(20.474, -103.554, 126.774, -116.682, -87.746, -37.709);

            DescPose p2Desc = new DescPose(-454.166, -327.159, 62.217, 177.199, -2.276, 154.955);
            JointPos p2Joint = new JointPos(27.176, -74.423, 104.557, -119.315, -93.514, -37.698);

            DescPose p3Desc = new DescPose(-375.533, -543.319, 19.798, 177.486, -2.489, 175.825);
            JointPos p3Joint = new JointPos(48.074, -59.714, 89.955, -119.777, -93.508, -37.683);


            //JointPos p1Joint = new JointPos(88.927, -85.834, 80.289, -85.561, -91.388, 108.718);
            //DescPose p1Desc = new DescPose(88.739, -527.617, 514.939, -179.039, 1.494, 70.209);

            //JointPos p2Joint = new JointPos(27.036, -83.909, 80.284, -85.579, -90.027, 108.604);
            //DescPose p2Desc = new DescPose(-433.125, -334.428, 497.139, -179.723, -0.745, 8.437);


            //JointPos p3Joint = new JointPos(60.219, -94.324, 62.906, -62.005, -87.159, 108.598);
            //DescPose p3Desc = new DescPose(-112.215, -409.323, 686.497, 176.217, 2.338, 41.625);
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);


            robot.AccSmoothStart(saveFlag);
            //robot.MoveL(p1Joint, p1Desc, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0, 10);
            //robot.MoveL(p2Joint, p2Desc, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0, 10);
            //robot.MoveL(p1Joint, p1Desc, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0, 10);
            //robot.MoveL(p2Joint, p2Desc, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0, 10);

            robot.AccSmoothEnd(saveFlag);

            //robot.AccSmoothStart(saveFlag);
            // rtn = robot.MoveC(p3Joint, p3Desc, 0, 0, 100, 100, exaxisPos, 0, offdese, p1Joint, p1Desc, 0, 0, 100, 100, exaxisPos, 100, offdese, 100, -1);
            //robot.AccSmoothEnd(saveFlag);

            //robot.AccSmoothStart(saveFlag);
            //robot.Circle(p3Joint, p3Desc, 1, 0, 100, 100, exaxisPos, p2Joint, p2Desc, 0, 0, 100, 100, exaxisPos, 100, 0, offdese);
            ////rtn = robot.Circle(p3Joint, p3Desc, 0, 0, 100, 100, exaxisPos, 0, offdese, p1Joint, p1Desc, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1);
            //robot.AccSmoothEnd(saveFlag);
            //rtn = robot.MoveC(midjointPos, middescPose, 0, 0, 30, 100, exaxisPos, 0, offdese, endjointPos, enddescPose, 0, 0, 30, 100, exaxisPos, 0, offdese, 100, -1);
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
            int index = 1;
            int max_time = 30000;
            bool block = false;
            retval = 0;

            /* 传送带抓取流程 */
            DescPose startdescPose = new DescPose(139.176f, 4.717f, 9.088f, -179.999f, -0.004f, -179.990f);
            JointPos startjointPos = new JointPos(-34.129f, -88.062f, 97.839f, -99.780f, -90.003f, -34.140f);

            DescPose homePose = new DescPose(139.177f, 4.717f, 69.084f, -180.000f, -0.004f, -179.989f);
            JointPos homejointPos = new JointPos(-34.129f, -88.618f, 84.039f, -85.423f, -90.003f, -34.140f);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            // 移动到安全位置
            //retval = robot.MoveL(homejointPos, homePose, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            Console.WriteLine($"MoveL 到安全位置返回值: {retval}");

            // 传送带检测
            retval = robot.ConveryComDetect(1000 * 10);
            Console.WriteLine($"ConveyorComDetect 返回值: {retval}");

            // 获取跟踪数据
            retval = robot.ConveyorGetTrackData(2);
            Console.WriteLine($"ConveyorGetTrackData 返回值: {retval}");

            // 开始跟踪
            retval = robot.ConveyorTrackStart(2);
            Console.WriteLine($"ConveyorTrackStart 返回值: {retval}");

            // 移动到起始位置
            //robot.MoveL(startjointPos, startdescPose, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            //robot.MoveL(startjointPos, startdescPose, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);

            // 结束跟踪
            retval = robot.ConveyorTrackEnd();
            Console.WriteLine($"ConveyorTrackEnd 返回值: {retval}");

            // 返回安全位置
           // robot.MoveL(homejointPos, homePose, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);

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
            robot.MoveL(startjointPos, startdescPose, 2, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            robot.WeaveStart(0);
            robot.MoveL(endjointPos, enddescPose, 2, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            robot.WeaveEnd(0);

            robot.WeaveSetPara(0, 3, 2.000000, 0, 10.000000, 0.000000, 0.000000, 0, 0, 0, 0, 0, 0, 30);
            robot.MoveL(startjointPos, startdescPose, 2, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            robot.WeaveStart(0);
            robot.MoveL(endjointPos, enddescPose, 2, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            robot.WeaveEnd(0);

        }

        private void button8_Click(object sender, EventArgs e)
        {
            DescPose safetydescPose = new DescPose(-504.043, 275.181, 40.908, -28.002, -42.025, -14.044);
            JointPos safetyjointPos = new JointPos(-39.078, -76.732, 87.227, -99.47, -94.301, 18.714);
            DescPose startdescPose = new DescPose(-473.86, 257.879, -20.849, -37.317, -42.021, 2.543);
            JointPos startjointPos = new JointPos(-43.487, -76.526, 95.568, -104.445, -89.356, 3.72);



            DescPose enddescPose = new DescPose(-499.844, 141.225, 7.72, -34.856, -40.17, 13.13);
            JointPos endjointPos = new JointPos(-31.305, -82.998, 99.401, -104.426, -89.35, 3.696);

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
            rtn = robot.ArcWeldTraceCurrentPara((float)0, (float)5, (float)0, (float)500);
            Console.WriteLine("ArcWeldTraceCurrentPara rtn is " + rtn);
            rtn = robot.ArcWeldTraceVoltagePara((float)1.018, (float)10, (float)0, (float)50);
            Console.WriteLine("ArcWeldTraceVoltagePara rtn is " + rtn);

            robot.MoveJ(startjointPos, startdescPose, 1, 0, 20, 100, 100, exaxisPos, -1, 0, offdese);
            robot.ArcWeldTraceControl(1, 0, 1, 0.08, 5, 5, 300, 1, 0.06, 4, 4, 300, 1, 0, 4, 1, 10, 0, 0);
            robot.ARCStart(0, 0, 10000);
            robot.WeaveStart(0);
           // robot.MoveL(endjointPos, enddescPose, 1, 0, 100, 100, 2, -1, exaxisPos, 0, 0, offdese);
            robot.ARCEnd(0, 0, 10000);
            robot.WeaveEnd(0);
            robot.ArcWeldTraceControl(0, 0, 1, 0.08, 5, 5, 300, 1, 0.06, 4, 4, 300, 1, 0, 4, 1, 10, 0, 0);
            robot.MoveJ(safetyjointPos, safetydescPose, 1, 0, 20, 100, 100, exaxisPos, -1, 0, offdese);

        }

        private void button9_Click(object sender, EventArgs e)
        {

            DescPose startdescPose = new DescPose(-319.303, -240.689, 116.379, -175.879, -0.337, 148.239);
            JointPos startjointPos = new JointPos(20.474, -103.554, 126.774, -116.682, -87.746, -37.709);

            DescPose enddescPose = new DescPose(-454.166, -327.159, 62.217, 177.199, -2.276, 154.955);
            JointPos endjointPos = new JointPos(27.176, -74.423, 104.557, -119.315, -93.514, -37.698);

            DescPose safedescPose = new DescPose(-375.533, -543.319, 19.798, 177.486, -2.489, 175.825);
            JointPos safejointPos = new JointPos(48.074, -59.714, 89.955, -119.777, -93.508, -37.683);

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
            //robot.MoveL(endjointPos, enddescPose, 1, 0, 100, 100, 2, -1, exaxisPos, 0, 0, offdese);
            robot.ARCEnd(0, 0, 10000);
            robot.WeaveChangeEnd();
            robot.WeaveEnd(0);
            robot.ArcWeldTraceControl(0, 0, 1, 0.08, 5, 5, 300, 1, 0.06, 4, 4, 300, 1, 0, 4, 1, 10, 0, 0);
            robot.WeldingSetCurrentGradualChangeEnd();
            robot.WeldingSetVoltageGradualChangeEnd();

        }

        private void button10_Click(object sender, EventArgs e)
        {

            bool saveFlag = false;
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
            JointPos JP1 = new JointPos(47.944, -74.115, 99.306, -129.280, -90.062, -98.421 );
            robot.GetForwardKin(JP1, ref DP1);
            DescPose DP2 = new DescPose(-387.275, -328.129, 340.563, -159.121, 16.169, -174.292);
            JointPos JP2 = new JointPos(23.798, -86.390, 105.682, -100.633, -65.192, -70.820 );
            robot.GetForwardKin(JP2, ref DP2);
            DescPose DP3 = new DescPose(-492.692, -49.563, 375.256, 161.781, -14.476, 159.830);
            JointPos JP3 = new JointPos(-1.812, -89.883, 108.067, -116.040, -111.809, -70.825 );
            robot.GetForwardKin(JP3, ref DP3);
            DescPose DP4 = new DescPose(-432.689, -287.194, 305.739, -177.999, 1.920, -177.450);
            JointPos JP4 = new JointPos(21.721, -83.395, 108.235, -113.684, -87.480, -70.821 );
            robot.GetForwardKin(JP4, ref DP4);
            DescPose DP5 = new DescPose(-232.690, -287.193, 305.746, -177.999, 1.919, -177.450);
            JointPos JP5 = new JointPos(34.158, -105.217, 128.305, -112.503, -87.290, -58.372);
            robot.GetForwardKin(JP5, ref DP5);
            DescPose DP6 = new DescPose(-232.695, -487.192, 305.744, -177.999, 1.919, -177.452);
            JointPos JP6 = new JointPos(53.031, -80.893, 105.748, -115.179, -87.247, -39.476 );
            robot.GetForwardKin(JP6, ref DP6);
            JointPos JP7 = new JointPos(38.933, -66.532, 86.532, -109.644, -87.251, -53.590);
            DescPose DP7 = new DescPose(-432.695, -487.196, 305.749, -177.999, 1.918, -177.452 );
            robot.GetForwardKin(JP7, ref DP7);
            JointPos JP8 = new JointPos(42.245, -82.011, 99.838, -116.087, -69.438, -70.824);
            DescPose DP8 = new DescPose(-315.138, -471.802, 373.506, -157.941, -1.233, -155.671 );
            robot.GetForwardKin(JP8, ref DP8);
            DescPose DP9 = new DescPose(-513.450, -302.627, 402.163, 171.249, -16.204, -176.411);
            JointPos JP9 = new JointPos(22.919, -78.425, 92.035, -116.080, -103.583, -70.913 );
            robot.GetForwardKin(JP9, ref DP9);
            DescPose DP10 = new DescPose(-428.141, -188.113, 351.314, 176.576, -19.670, 142.831);
            JointPos JP10 = new JointPos(14.849, -92.942, 114.901, -121.601, -107.553, -38.881 );
            robot.GetForwardKin(JP10, ref DP10);
            DescPose DP11 = new DescPose(-587.412, -70.091, 370.337, 177.676, -23.575, 127.293);
            JointPos JP11 = new JointPos(0.209, -77.444, 96.217, -121.606, -110.075, -38.879 );
            robot.GetForwardKin(JP11, ref DP11);
            JointPos JP12 = new JointPos(-21.947, -88.425, 108.395, -111.062, -77.881, -38.879);
            DescPose DP12 = new DescPose(-498.493, 67.966, 345.644, -171.472, 8.710, 107.699 );
            robot.GetForwardKin(JP12, ref DP12);
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0 );
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0 );

            robot.MoveJ(JP1, DP1, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.MoveJ(JP2, DP2, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            robot.MoveJ(JP3, DP3, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            robot.MoveJ(JP4, DP4, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);

            robot.MoveL(JP5, DP5, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            robot.MoveL(JP6, DP6, 0, 0, 100, 100, 100, 20, 1, exaxisPos, 0, 0, offdese);
            robot.MoveL(JP7, DP7, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);

            robot.MoveJ(JP8, DP8, 0, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            robot.MoveC(JP9, DP9, 0, 0, 100, 100, exaxisPos, 0, offdese, JP10, DP10, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 30);
            robot.MoveC(JP11, DP11, 0, 0, 100, 100, exaxisPos, 0, offdese, JP12, DP12, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1);

        }

        private void button14_Click(object sender, EventArgs e)
        {
            byte status = 1;
            byte smooth = 0;
            byte block = 0;
            byte di = 0, tool_di = 0;
            float ai = 0.0f, tool_ai = 0.0f;
            float value = 0.0f;


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
            byte status = 1;
            byte smooth = 0;
            byte block = 0;
            byte di = 0, tool_di = 0;
            float ai = 0.0f, tool_ai = 0.0f;
            float value = 0.0f;

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
            byte status = 1;
            byte smooth = 0;
            byte block = 0;
            byte di = 0, tool_di = 0;
            float ai = 0.0f, tool_ai = 0.0f;
            float value = 0.0f;

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
            for (int i = 0; i < 16; i++)
            {
                robot.SetDO(i, 1, 0, 0);
                Thread.Sleep(300);
            }

            int resetFlag = 1;
            int rtn = robot.SetOutputResetCtlBoxDO(resetFlag);
            robot.SetOutputResetCtlBoxAO(resetFlag);
            robot.SetOutputResetAxleDO(resetFlag);
            robot.SetOutputResetAxleAO(resetFlag);
            robot.SetOutputResetExtDO(resetFlag);
            robot.SetOutputResetExtAO(resetFlag);
            robot.SetOutputResetSmartToolDO(resetFlag);

            robot.ProgramLoad("/fruser/Text1.lua");
            robot.ProgramRun();

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

            robot.MoveJ( p1Joint,  p1Desc, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetToolPoint(1);
            robot.MoveJ( p2Joint,  p2Desc, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetToolPoint(2);
            robot.MoveJ( p3Joint,  p3Desc, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetToolPoint(3);
            robot.MoveJ( p4Joint,  p4Desc, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetToolPoint(4);
            robot.MoveJ( p5Joint,  p5Desc, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetToolPoint(5);
            robot.MoveJ( p6Joint,  p6Desc, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetToolPoint(6);
            rtn = robot.ComputeTool(ref coordRtn);
            Console.WriteLine($"6 Point ComputeTool        {rtn}  coord is {coordRtn.tran.x} {coordRtn.tran.y} {coordRtn.tran.z} {coordRtn.rpy.rx} {coordRtn.rpy.ry} {coordRtn.rpy.rz}");
            robot.SetToolList(1,  coordRtn, 0, 0, 0);

            robot.MoveJ( p1Joint,  p1Desc, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetTcp4RefPoint(1);
            robot.MoveJ( p2Joint,  p2Desc, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetTcp4RefPoint(2);
            robot.MoveJ( p3Joint,  p3Desc, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetTcp4RefPoint(3);
            robot.MoveJ( p4Joint,  p4Desc, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
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

            robot.GetForwardKin(p1Joint,ref p1Desc);
            robot.GetForwardKin(p2Joint,ref p2Desc);
            robot.GetForwardKin(p3Joint, ref p3Desc);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            DescPose[] posTCP = new DescPose[] { p1Desc, p2Desc, p3Desc };
            DescPose coordRtn = new DescPose();
            int rtn = robot.ComputeWObjCoordWithPoints(1, posTCP, 0, ref coordRtn);
            Console.WriteLine($"ComputeWObjCoordWithPoints    {rtn}  coord is {coordRtn.tran.x} {coordRtn.tran.y} {coordRtn.tran.z} {coordRtn.rpy.rx} {coordRtn.rpy.ry} {coordRtn.rpy.rz}");

            robot.MoveJ( p1Joint,  p1Desc, 1, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetWObjCoordPoint(1);
            robot.MoveJ( p2Joint,  p2Desc, 1, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetWObjCoordPoint(2);
            robot.MoveJ( p3Joint,  p3Desc, 1, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetWObjCoordPoint(3);
            rtn = robot.ComputeWObjCoord(1, 0, ref coordRtn);
            Console.WriteLine($"ComputeWObjCoord                   {rtn}  coord is {coordRtn.tran.x} {coordRtn.tran.y} {coordRtn.tran.z} {coordRtn.rpy.rx} {coordRtn.rpy.ry} {coordRtn.rpy.rz}");

            robot.SetWObjCoord(1,  coordRtn, 0);
            robot.SetWObjList(1,  coordRtn, 0);

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

            robot.MoveJ( p1Joint,  p1Desc, 1, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetExTCPPoint(1);
            robot.MoveJ( p2Joint,  p2Desc, 1, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetExTCPPoint(2);
            robot.MoveJ( p3Joint,  p3Desc, 1, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.SetExTCPPoint(3);
            int rtn = robot.ComputeExTCF(ref coordRtn);
            Console.WriteLine($"ComputeExTCF                   {rtn}  coord is {coordRtn.tran.x} {coordRtn.tran.y} {coordRtn.tran.z} {coordRtn.rpy.rx} {coordRtn.rpy.ry} {coordRtn.rpy.rz}");

            robot.SetExToolCoord(1,  coordRtn,  offdese);
            robot.SetExToolList(1,  coordRtn,  offdese);
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
            robot.SetLoadCoord( loadCoord);

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
            int maincode=0, subcode=0;
            robot.GetRobotErrorCode(ref maincode, ref subcode);
            Console.WriteLine($"robot maincode is{maincode};  subcode is {subcode}" );

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
            double[] level2 = { 50.0f, 20.0f, 30.0f, 40.0f, 50.0f, 60.0f };

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
            robot.MoveL( p2Joint,  p2Desc, 0, 0, 100, 100, 100, 2,  exaxisPos, 0, 0,  offdese);
            robot.ResetAllError();
            int[] safety = { 5, 5, 5, 5, 5, 5 };
            rtn = robot.SetCollisionStrategy(3, 1000, 150, 250, safety);
            Console.WriteLine($"SetCollisionStrategy rtn is {rtn}");

            double[] jointDetectionThreshould = { 0.1, 0.1, 0.1, 0.1, 0.1, 0.1 };
            double[] tcpDetectionThreshould = { 60, 60, 60, 60, 60, 60 };
            rtn = robot.CustomCollisionDetectionStart(3, jointDetectionThreshould, tcpDetectionThreshould, 0);
            Console.WriteLine($"CustomCollisionDetectionStart rtn is {rtn}");

            robot.MoveL( p1Joint,  p1Desc, 0, 0, 100, 100, 100, -1,  exaxisPos, 0, 0,  offdese);
            robot.MoveL( p2Joint,  p2Desc, 0, 0, 100, 100, 100, -1,  exaxisPos, 0, 0,  offdese);
            rtn = robot.CustomCollisionDetectionEnd();
            Console.WriteLine($"CustomCollisionDetectionEnd rtn is {rtn}");
        }

        private void button25_Click(object sender, EventArgs e)
        {
            double[] plimit = { 170.0f, 80.0f, 150.0f, 80.0f, 170.0f, 160.0f };
            robot.SetLimitPositive(plimit);
            double[] nlimit = { -170.0f, -260.0f, -150.0f, -260.0f, -170.0f, -160.0f };
            robot.SetLimitNegative(nlimit);

            double[] neg_deg = new double[6] {0,0,0,0,0,0 };
            double[] pos_deg = new double[6] { 0, 0, 0, 0, 0, 0 };
            robot.GetJointSoftLimitDeg(0, ref neg_deg,ref pos_deg);
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

            JointPos j_deg = new JointPos(0,0,0,0,0,0);
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
            robot.GetTargetTCPSpeed(0,ref targetSpeed);
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
            robot.GetInverseKinRef(0,  desc_pos1, j1, ref inverseRtn);
            Console.WriteLine($"dcs1 GetInverseKinRef rtn is {inverseRtn.jPos[0]} {inverseRtn.jPos[1]} {inverseRtn.jPos[2]} {inverseRtn.jPos[3]} {inverseRtn.jPos[4]} {inverseRtn.jPos[5]}");

            bool hasResut = false;
            robot.GetInverseKinHasSolution(0,  desc_pos1,  j1, ref hasResut);
            Console.WriteLine($"dcs1 GetInverseKinRef result {hasResut}");

            DescPose forwordResult = new DescPose(0, 0, 0, 0, 0, 0);
            robot.GetForwardKin(j1, ref forwordResult);
            Console.WriteLine($"jpos1 forwordResult rtn is {forwordResult.tran.x} {forwordResult.tran.y} {forwordResult.tran.z} {forwordResult.rpy.rx} {forwordResult.rpy.ry} {forwordResult.rpy.rz}");
        }

        private void button31_Click(object sender, EventArgs e)
        {
            string name = "A0";
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
            int rtn = robot.TrajectoryJUpLoad("D://zUP/spray_traj1.txt");
            Console.WriteLine("Upload TrajectoryJ A {0}\n", rtn);

            string traj_file_name = "/fruser/traj/spray_traj1.txt";
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
            string program_name = "/fruser/Text1.lua";
            string loaded_name = "";
            byte state=0;
            int line=0;

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
            int company = 4;
            int device = 0;
            int softversion = 0;
            int bus = 2;
            int index = 2;
            byte act = 0;
            int max_time = 30000;
            byte block = 0;
            int status=0;
            int fault=0;
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
            Thread.Sleep(1000);

            robot.MoveGripper(index, 90, 50, 50, max_time, block, 0, 0, 0, 0);
            Thread.Sleep(1000);
            robot.MoveGripper(index, 30, 50, 0, max_time, block, 0, 0, 0, 0);

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

            int retval = 0;
            DescPose prepick_pose = new DescPose();
            DescPose postpick_pose = new DescPose();

            DescPose p1Desc = new DescPose(-419.524f, -13.000f, 351.569f, -178.118f, 0.314f, 3.833f);
            DescPose p2Desc = new DescPose(-321.222f, 185.189f, 335.520f, -179.030f, -1.284f, -29.869f);

            retval = robot.ComputePrePick(p1Desc, 10, 0, ref prepick_pose);
            Console.WriteLine("ComputePrePick retval is: {0}\n", retval);
            Console.WriteLine("xyz is: {0}, {1}, {2}; rpy is: {3}, {4}, {5}\n",
                prepick_pose.tran.x, prepick_pose.tran.y, prepick_pose.tran.z,
                prepick_pose.rpy.rx, prepick_pose.rpy.ry, prepick_pose.rpy.rz);

            retval = robot.ComputePostPick( p2Desc, -10, 0, ref postpick_pose);
            Console.WriteLine("ComputePostPick retval is: {0}\n", retval);
            Console.WriteLine("xyz is: {0}, {1}, {2}; rpy is: {3}, {4}, {5}\n",
                postpick_pose.tran.x, postpick_pose.tran.y, postpick_pose.tran.z,
                postpick_pose.rpy.rx, postpick_pose.rpy.ry, postpick_pose.rpy.rz);

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
            robot.AxleLuaUpload("D://zUP/AXLE_LUA_End_JunDuo_Xinjingcheng.lua");

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
            robot.SetAxleLuaEnableDeviceType(0, 1, 0);

            int forceEnable = 0;
            int gripperEnable = 0;
            int ioEnable = 0;
            robot.GetAxleLuaEnableDeviceType(ref forceEnable, ref gripperEnable, ref ioEnable);
            Console.WriteLine("GetAxleLuaEnableDeviceType param is {0} {1} {2}", forceEnable, gripperEnable, ioEnable);

            int[] func = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            robot.SetAxleLuaGripperFunc(1, func);
            int[] getFunc = new int[16];
            robot.GetAxleLuaGripperFunc(1, ref getFunc);
            int[] getforceEnable = new int[16];
            int[] getgripperEnable = new int[16];
            int[] getioEnable = new int[16];
            robot.GetAxleLuaEnableDevice(ref getforceEnable, ref getgripperEnable, ref getioEnable);
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
            Thread.Sleep(2000);
            robot.ActGripper(1, 1);
            Thread.Sleep(2000);
            robot.MoveGripper(1, 90, 10, 100, 50000, 0, 0, 0, 0, 0);
            int pos = 0;
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
                robot.SetWeldMachineCtrlMode(0);
                Thread.Sleep(1000);
                robot.SetWeldMachineCtrlMode(1);
                Thread.Sleep(1000);
            }
        }

        private void button43_Click(object sender, EventArgs e)
        {
            robot.WeldingSetCurrent(1, 230, 0, 0);
            robot.WeldingSetVoltage(1, 24, 0, 1);

            DescPose p1Desc = new DescPose(228.879, -503.594, 453.984, -175.580, 8.293, 171.267);
            JointPos p1Joint = new JointPos(102.700, -85.333, 90.518, -102.365, -83.932, 22.134);

            DescPose p2Desc = new DescPose(-333.302, -435.580, 449.866, -174.997, 2.017, 109.815);
            JointPos p2Joint = new JointPos(41.862, -85.333, 90.526, -100.587, -90.014, 22.135);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.MoveJ(p1Joint, p1Desc, 13, 0, 20, 100, 100, exaxisPos, -1, 0, offdese);
            robot.ARCStart(1, 0, 10000);
            robot.WeaveStart(0);
            robot.MoveL (p2Joint, p2Desc, 13, 0, 20, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            robot.ARCEnd(1, 0, 10000);
            robot.WeaveEnd(0);
        }

        private void button44_Click(object sender, EventArgs e)
        {
            robot.WeldingSetCurrent(1, 230, 0, 0);
            robot.WeldingSetVoltage(1, 24, 0, 1);

            DescPose p1Desc = new DescPose(228.879, -503.594, 453.984, -175.580, 8.293, 171.267);
            JointPos p1Joint = new JointPos(102.700, -85.333, 90.518, -102.365, -83.932, 22.134);

            DescPose p2Desc = new DescPose(-333.302, -435.580, 449.866, -174.997, 2.017, 109.815);
            JointPos p2Joint = new JointPos(41.862, -85.333, 90.526, -100.587, -90.014, 22.135);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            int rtn = robot.SegmentWeldStart( p1Desc,  p2Desc,  p1Joint,  p2Joint, 20, 20, 0, 0, 5000, false, 0, 0, 0, 100, 100, 100, -1,  exaxisPos, 0, 0,  offdese);
            Console.WriteLine("SegmentWeldStart rtn is {0}", rtn);
        }

        private void button45_Click(object sender, EventArgs e)
        {
            DescPose p1Desc = new DescPose(228.879, -503.594, 453.984, -175.580, 8.293, 171.267);
            JointPos p1Joint = new JointPos(102.700, -85.333, 90.518, -102.365, -83.932, 22.134);

            DescPose p2Desc = new DescPose(-333.302, -435.580, 449.866, -174.997, 2.017, 109.815);
            JointPos p2Joint = new JointPos(41.862, -85.333, 90.526, -100.587, -90.014, 22.135);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.MoveJ(p1Joint, p1Desc, 13, 0, 20, 100, 100, exaxisPos, -1, 0, offdese);
            robot.WeaveStartSim(0);
            robot.MoveL(p2Joint, p2Desc, 13, 0, 20, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            robot.WeaveEndSim(0);
            robot.MoveJ(p1Joint, p1Desc, 13, 0, 20, 100, 100, exaxisPos, -1, 0, offdese);
            robot.WeaveInspectStart(0);
            robot.MoveL(p2Joint, p2Desc, 13, 0, 20, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            robot.WeaveInspectEnd(0);

            robot.WeldingSetVoltage(1, 19, 0, 0);
            robot.WeldingSetCurrent(1, 190, 0, 0);
            robot.MoveL( p1Joint,  p1Desc, 1, 1, 100, 100, 50, -1,  exaxisPos, 0, 0,  offdese);
            robot.ARCStart(1, 0, 10000);
            robot.ArcWeldTraceControl(1, 0, 1, 0.06, 5, 5, 60, 1, 0.06, 5, 5, 80, 0, 0, 4, 1, 10, 0, 0);
            robot.WeaveStart(0);
            robot.WeaveChangeStart(1, 0, 50, 30);
            robot.MoveL( p2Joint,  p2Desc, 1, 1, 100, 100, 1, -1,  exaxisPos, 0, 0,  offdese);
            robot.WeaveChangeEnd();
            robot.WeaveEnd(0);
            robot.ArcWeldTraceControl(0, 0, 1, 0.06, 5, 5, 60, 1, 0.06, 5, 5, 80, 0, 0, 4, 1, 10, 0, 0);
            robot.ARCEnd(1, 0, 10000);
        }

        private void button46_Click(object sender, EventArgs e)
        {
            string file_path = "/fruser/airlab.lua";
            string md5 = "";
            byte emerg_state = 0;
            byte si0_state = 0;
            byte si1_state = 0;
            int sdk_com_state = 0;

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
            robot.SoftwareUpgrade("D://zUP/QNX382/software.tar.gz", false);
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

            string upload_path = "D://zUP/test_point_A.db";
            rtn = robot.PointTableUpLoad(upload_path);
            Console.WriteLine("retval is: {0}", rtn);

            string point_tablename = "test_point_A.db";
            string lua_name = "Text1.lua";

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
        }

        private void button52_Click(object sender, EventArgs e)
        {
            JointPos mulitilineorigin1_joint = new JointPos(-24.090, -63.501, 84.288, -111.940, -93.426, 57.669);
            DescPose mulitilineorigin1_desc = new DescPose(-677.559, 190.951, -1.205, 1.144, -41.482, -82.577);

            DescTran mulitilineX1_desc = new DescTran();
            mulitilineX1_desc.x = -677.556;
            mulitilineX1_desc.y = 211.949;
            mulitilineX1_desc.z = -1.206;

            DescTran mulitilineZ1_desc = new DescTran();
            mulitilineZ1_desc.x = -677.564;
            mulitilineZ1_desc.y = 190.956;
            mulitilineZ1_desc.z = 19.817;

            JointPos mulitilinesafe_joint = new JointPos(-25.734, -63.778, 81.502, -108.975, -93.392, 56.021);
            DescPose mulitilinesafe_desc = new DescPose(-677.561, 211.950, 19.812, 1.144, -41.482, -82.577);
            JointPos mulitilineorigin2_joint = new JointPos(-29.743, -75.623, 101.241, -116.354, -94.928, 55.735);
            DescPose mulitilineorigin2_desc = new DescPose(-563.961, 215.359, -0.681, 2.845, -40.476, -87.443);

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
            int error = robot.MoveJ( mulitilinesafe_joint,  mulitilinesafe_desc, 13, 0, 10, 100, 100,  epos, -1, 0,  offset);
            Console.WriteLine("MoveJ return: {0}", error);

            error = robot.MoveL( mulitilineorigin1_joint,  mulitilineorigin1_desc, 13, 0, 10, 100, 100, -1,  epos, 0, 0,  offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
            Console.WriteLine("MoveJ return: {0}", error);

            error = robot.MoveL(mulitilineorigin2_joint, mulitilineorigin2_desc, 13, 0, 10, 100, 100, -1,  epos, 0, 0,  offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
            Console.WriteLine("MoveJ return: {0}", error);

            error = robot.MoveL( mulitilineorigin1_joint,  mulitilineorigin1_desc, 13, 0, 10, 100, 100, -1,  epos, 0, 0,  offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.ARCStart(1, 0, 3000);
            Console.WriteLine("ARCStart return: {0}", error);

            error = robot.WeaveStart(0);
            Console.WriteLine("WeaveStart return: {0}", error);

            error = robot.ArcWeldTraceControl(1, 0, 1, 0.06, 5, 5, 50, 1, 0.06, 5, 5, 55, 0, 0, 4, 1, 10);
            Console.WriteLine("ArcWeldTraceControl return: {0}", error);

            error = robot.MoveL( mulitilineorigin2_joint,  mulitilineorigin2_desc, 13, 0, 1, 100, 100, -1,  epos, 0, 0,  offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.ArcWeldTraceControl(0, 0, 1, 0.06, 5, 5, 50, 1, 0.06, 5, 5, 55, 0, 0, 4, 1, 10);
            Console.WriteLine("ArcWeldTraceControl return: {0}", error);

            error = robot.WeaveEnd(0);
            Console.WriteLine("WeaveEnd return: {0}", error);

            error = robot.ARCEnd(1, 0, 10000);
            Console.WriteLine("ARCEnd return: {0}", error);

            error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
            Console.WriteLine("MoveJ return: {0}", error);

            error = robot.MultilayerOffsetTrsfToBase(mulitilineorigin1_desc.tran, mulitilineX1_desc, mulitilineZ1_desc, 10.0, 0.0, 0.0, ref offset);
            Console.WriteLine("MultilayerOffsetTrsfToBase return: {0}  offect is {1} {2} {3}", error, offset.tran.x, offset.tran.y, offset.tran.z);

            error = robot.MoveL( mulitilineorigin1_joint,  mulitilineorigin1_desc, 13, 0, 10, 100, 100, -1,  epos, 0, 1,  offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.ARCStart(1, 0, 3000);
            Console.WriteLine("ARCStart return: {0}", error);

            error = robot.MultilayerOffsetTrsfToBase(mulitilineorigin2_desc.tran, mulitilineX2_desc, mulitilineZ2_desc, 10, 0, 0, ref offset);
            Console.WriteLine("MultilayerOffsetTrsfToBase return: {0}  offect is {1} {2} {3}", error, offset.tran.x, offset.tran.y, offset.tran.z);

            error = robot.ArcWeldTraceReplayStart();
            Console.WriteLine("ArcWeldTraceReplayStart return: {0}", error);

            error = robot.MoveL( mulitilineorigin2_joint,  mulitilineorigin2_desc, 13, 0, 2, 100, 100, -1,  epos, 0, 1,  offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.ArcWeldTraceReplayEnd();
            Console.WriteLine("ArcWeldTraceReplayEnd return: {0}", error);

            error = robot.ARCEnd(1, 0, 10000);
            Console.WriteLine("ARCEnd return: {0}", error);

            error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
            Console.WriteLine("MoveJ return: {0}", error);

            error = robot.MultilayerOffsetTrsfToBase(mulitilineorigin1_desc.tran, mulitilineX1_desc, mulitilineZ1_desc, 0, 10, 0, ref offset);
            Console.WriteLine("MultilayerOffsetTrsfToBase return: {0}  offect is {1} {2} {3}", error, offset.tran.x, offset.tran.y, offset.tran.z);

            error = robot.MoveL( mulitilineorigin1_joint,  mulitilineorigin1_desc, 13, 0, 10, 100, 100, -1,  epos, 0, 1,  offset, 0, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.ARCStart(1, 0, 3000);
            Console.WriteLine("ARCStart return: {0}", error);

            error = robot.MultilayerOffsetTrsfToBase(mulitilineorigin2_desc.tran, mulitilineX2_desc, mulitilineZ2_desc, 0, 10, 0, ref offset);
            Console.WriteLine("MultilayerOffsetTrsfToBase return: {0}  offect is {1} {2} {3}", error, offset.tran.x, offset.tran.y, offset.tran.z);

            error = robot.ArcWeldTraceReplayStart();
            Console.WriteLine("MoveJ return: {0}", error);

            error = robot.MoveL(mulitilineorigin2_joint, mulitilineorigin2_desc, 13, 1, 2, 100, 100, -1, epos, 1, 1, offset, 1, 100);
            Console.WriteLine("MoveL return: {0}", error);

            error = robot.ArcWeldTraceReplayEnd();
            Console.WriteLine("ArcWeldTraceReplayEnd return: {0}", error);

            error = robot.ARCEnd(1, 0, 3000);
            Console.WriteLine("ARCEnd return: {0}", error);

            error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
            Console.WriteLine("MoveJ return: {0}", error);
        }

        private void button53_Click(object sender, EventArgs e)
        {
            DescPose toolCoord=new DescPose(0, 0, 200, 0, 0, 0);
            robot.SetToolCoord(1, toolCoord, 0, 0, 1, 0);
            DescPose wobjCoord=new DescPose(0, 0, 0, 0, 0, 0);
            robot.SetWObjCoord(1, wobjCoord, 0);

            int rtn0, rtn1, rtn2 = 0;
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            DescPose descStart = new DescPose(216.543, 445.175, 93.465, 179.683, 1.757, -112.641);
            JointPos jointStart = new JointPos(-128.345, -86.660, 114.679, -119.625, -89.219, 74.303);

            DescPose descEnd = new DescPose(111.143, 523.384, 87.659, 179.703, 1.835, -97.750);
            JointPos jointEnd = new JointPos(-113.454, -81.060, 109.328, -119.954, -89.218, 74.302);

            robot.MoveL(jointStart, descStart, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            robot.MoveL(jointEnd, descEnd, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);

            DescPose descREF0A = new DescPose(142.135, 367.604, 86.523, 179.728, 1.922, -111.089);
            JointPos jointREF0A = new JointPos(-126.794, -100.834, 128.922, -119.864, -89.218, 74.302);

            DescPose descREF0B = new DescPose(254.633, 463.125, 72.604, 179.845, 2.341, -114.704);
            JointPos jointREF0B = new JointPos(-130.413, -81.093, 112.044, -123.163, -89.217, 74.303);

            DescPose descREF1A = new DescPose(92.556, 485.259, 47.476, -179.932, 3.130, -97.512);
            JointPos jointREF1A = new JointPos(-113.231, -83.815, 119.877, -129.092, -89.217, 74.303);

            DescPose descREF1B = new DescPose(203.103, 583.836, 63.909, 179.991, 2.854, -103.372);
            JointPos jointREF1B = new JointPos(-119.088, -69.676, 98.692, -121.761, -89.219, 74.303);

            rtn0 = robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF0A, descREF0A, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);  //起点
            robot.MoveL(jointREF0B, descREF0B, 1, 1, 100, 100, 100, -1, exaxisPos, 1, 0, offdese);  //方向点
            rtn1 = robot.WireSearchWait("REF0");
            rtn2 = robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            rtn0 = robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF1A, descREF1A, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);  //起点
            robot.MoveL(jointREF1B, descREF1B, 1, 1, 100, 100, 100, -1, exaxisPos, 1, 0, offdese);  //方向点
            rtn1 = robot.WireSearchWait("REF1");
            rtn2 = robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            rtn0 = robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF0A, descREF0A, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);  //起点
            robot.MoveL(jointREF0B, descREF0B, 1, 1, 100, 100, 100, -1, exaxisPos, 1, 0, offdese);  //方向点
            rtn1 = robot.WireSearchWait("RES0");
            rtn2 = robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            rtn0 = robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF1A, descREF1A, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);  //起点
            robot.MoveL(jointREF1B, descREF1B, 1, 1, 100, 100, 100, -1, exaxisPos, 1, 0, offdese);  //方向点
            rtn1 = robot.WireSearchWait("RES1");
            rtn2 = robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            string[] varNameRef = { "REF0", "REF1", "#", "#", "#", "#" };
            string[] varNameRes = { "RES0", "RES1", "#", "#", "#", "#" };
            int offectFlag = 0;
            DescPose offectPos = new DescPose(0, 0, 0, 0, 0, 0);
            rtn0 = robot.GetWireSearchOffset(0, 0, varNameRef, varNameRes, ref offectFlag, ref offectPos);
            robot.PointsOffsetEnable(0, offectPos);
            robot.MoveL(jointStart, descStart, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            robot.MoveL(jointEnd, descEnd, 1, 1, 100, 100, 100, -1, exaxisPos, 1, 0, offdese);
            robot.PointsOffsetDisable();
        }

        private void button54_Click(object sender, EventArgs e)
        {
            int company = 24;
            int device = 0;
            int softversion = 0;
            int bus = 1;
            int index = 1;

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

            double computeWeight = 0;
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

            ForceTorque ft = new ForceTorque(0,0,0,0,0,0);
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

            robot.MoveCart( desc_p1, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            Thread.Sleep(1000);
            robot.FT_PdCogIdenRecord(10, 1);
            robot.MoveCart( desc_p2, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            Thread.Sleep(1000);
            robot.FT_PdCogIdenRecord(10, 2);
            robot.MoveCart( desc_p3, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            Thread.Sleep(1000);
            robot.FT_PdCogIdenRecord(10, 3);

            DescTran cog = new DescTran(0,0,0);
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

            robot.FT_Guard(1, sensor_id, select,  ft, max_threshold, min_threshold);
            robot.MoveCart( desc_p1, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            robot.MoveCart( desc_p2, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            robot.MoveCart( desc_p3, 0, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);

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

            byte sensor_id = 1;
            int[] select = { 0, 0, 1, 0, 0, 0 };
            double[] ft_pid = { 0.0005f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            byte adj_sign = 0, ILC_sign = 0;
            float max_dis = 100.0f, max_ang = 0.0f;

            ForceTorque ft = new ForceTorque( 0.0, 0.0, -10.0, 0.0, 0.0, 0.0 );
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
            JointPos j1 = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            JointPos j2 = new JointPos(-45.615, -106.172, 124.296, -107.151, -91.282, 74.255);
            DescPose desc_p1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose desc_p2 = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);

            int rtn = robot.MoveJ( j1,  desc_p1, 0, 0, 100.0f, 180.0f, 100.0f,  epos, -1.0f, 0,  offset_pos);
            robot.FT_Control(1, sensor_id, select,  ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
            rtn = robot.MoveJ(j2, desc_p2, 0, 0, 100.0f, 180.0f, 100.0f, epos, -1.0f, 0, offset_pos);
            robot.FT_Control(0, sensor_id, select,  ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
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
            //int company = 24;
            //int device = 0;
            //int softversion = 0;
            //int bus = 1;
            //int index = 1;

            //robot.FT_SetConfig(company, device, softversion, bus);
            //Thread.Sleep(1000);
            //robot.FT_GetConfig(ref company, ref device, ref softversion, ref bus);
            //Console.WriteLine($"FT config:{company},{device},{softversion},{bus}");
            //Thread.Sleep(1000);

            //robot.FT_Activate(0);
            //Thread.Sleep(1000);
            //robot.FT_Activate(1);
            //Thread.Sleep(1000);

            //Thread.Sleep(1000);
            //robot.FT_SetZero(0);
            //Thread.Sleep(1000);

            //int rcs = 0;
            //byte dir = 1;
            //byte axis = 1;
            //float lin_v = 3.0f;
            //float lin_a = 0.0f;
            //float maxdis = 50.0f;
            //float ft_goal = 2.0f;
            //DescPose desc_pos = new DescPose(-419.524f, -13.000f, 351.569f, -178.118f, 0.314f, 3.833f);
            //DescPose xcenter = new DescPose(0, 0, 0, 0, 0, 0);
            //DescPose ycenter = new DescPose(0, 0, 0, 0, 0, 0);

            //ForceTorque ft = new ForceTorque();
            //// In C#, new objects are initialized to zero by default, but if you need explicit zeroing:
            //ft.fx = 0;
            //ft.fy = 0;
            //ft.fz = 0;
            //ft.tx = 0;
            //ft.ty = 0;
            //ft.tz = 0;

            //ft.fx = -2.0f;

            //robot.MoveCart( desc_pos, 9, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);

            //robot.FT_CalCenterStart();
            //robot.FT_FindSurface(rcs, dir, axis, lin_v, lin_a, maxdis, ft_goal);
            //robot.MoveCart( desc_pos, 9, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            //robot.WaitMs(1000);

            //dir = 2;
            //robot.FT_FindSurface(rcs, dir, axis, lin_v, lin_a, maxdis, ft_goal);
            //robot.FT_CalCenterEnd(ref xcenter);
            //Console.WriteLine($"xcenter:{xcenter.tran.x},{xcenter.tran.y},{xcenter.tran.z},{xcenter.rpy.rx},{xcenter.rpy.ry},{xcenter.rpy.rz}");
            //robot.MoveCart(ref xcenter, 9, 0, 60.0f, 50.0f, 50.0f, -1.0f, -1);

            //robot.FT_CalCenterStart();
            //dir = 1;
            //axis = 2;
            //lin_v = 6.0f;
            //maxdis = 150.0f;
            //robot.FT_FindSurface(rcs, dir, axis, lin_v, lin_a, maxdis, ft_goal);
            //robot.MoveCart( desc_pos, 9, 0, 100.0f, 100.0f, 100.0f, -1.0f, -1);
            //robot.WaitMs(1000);

            //dir = 2;
            //robot.FT_FindSurface(rcs, dir, axis, lin_v, lin_a, maxdis, ft_goal);
            //robot.FT_CalCenterEnd(ref ycenter);
            //Console.WriteLine($"ycenter:{ycenter.tran.x},{ycenter.tran.y},{ycenter.tran.z},{ycenter.rpy.rx},{ycenter.rpy.ry},{ycenter.rpy.rz}");
            //robot.MoveCart( ycenter, 9, 0, 60.0f, 50.0f, 50.0f, 0.0f, -1);

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

            byte flag = 1;
            int sensor_id = 1;
            int[] select = { 1, 1, 1, 0, 0, 0 };
            double[] ft_pid = { 0.0005f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            byte adj_sign = 0, ILC_sign = 0;
            float max_dis = 100.0f, max_ang = 0.0f;

            ForceTorque ft = new ForceTorque { fx = -10.0, fy = -10.0, fz = -10.0 };
            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);

            JointPos j1 = new JointPos(-11.904, -99.669, 117.473, -108.616, -91.726, 74.256);
            JointPos j2 = new JointPos(-45.615, -106.172, 124.296, -107.151, -91.282, 74.255);
            DescPose desc_p1 = new DescPose(-419.524, -13.000, 351.569, -178.118, 0.314, 3.833);
            DescPose desc_p2 = new DescPose(-321.222, 185.189, 335.520, -179.030, -1.284, -29.869);

            robot.FT_Control(flag, (byte)sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
            float p = 0.00005f;
            float force = 30.0f;
            int rtn = robot.FT_ComplianceStart(p, force);
            Console.WriteLine($"FT_ComplianceStart rtn is {rtn}");

            int count = 5;
            while (count-- > 0)
            {
                robot.MoveL(j1, desc_p1, 0, 0, 100.0f, 180.0f, 100.0f, -1.0f, epos, 0, 1, offset_pos);
                robot.MoveL(j2, desc_p2, 0, 0, 100.0f, 180.0f, 100.0f, -1.0f, epos, 0, 0, offset_pos);
            }

            robot.FT_ComplianceStop();
            Console.WriteLine($"FT_ComplianceStop rtn is {rtn}");

            flag = 0;
            robot.FT_Control(flag, (byte)sensor_id, select, ft, ft_pid, adj_sign, ILC_sign, max_dis, max_ang);
        }

        private void button63_Click(object sender, EventArgs e)
        {
            int que_len = 0;
            int rtn = robot.GetMotionQueueLength(ref que_len);
            Console.WriteLine($"GetMotionQueueLength rtn is:  {rtn}, queue length is:{que_len}");
            double[] dh = { 0,0,0,0,0,0 };
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
            int servoerrcode = 0;
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
            int rtn = robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 200, 1, 100, 5,1);
            Console.WriteLine("ExtDevSetUDPComParam rtn is " + rtn);
            string ip = ""; int port = 0; int period = 0; int lossPkgTime = 0; int lossPkgNum = 0; int disconnectTime = 0; int reconnectEnable = 0; int reconnectPeriod = 0; int reconnectNum = 0;
            rtn = robot.ExtDevGetUDPComParam(ref ip, ref port, ref period, ref lossPkgTime, ref lossPkgNum, ref disconnectTime, ref reconnectEnable, ref reconnectPeriod, ref reconnectNum);
            string param = "\nip " + ip + "\nport " + port.ToString() + "\nperiod  " + period.ToString() + "\nlossPkgTime " + lossPkgTime.ToString() + "\nlossPkgNum  " + lossPkgNum.ToString() + "\ndisConntime  " + disconnectTime.ToString() + "\nreconnecable  " + reconnectEnable.ToString() + "\nreconnperiod  " + reconnectPeriod.ToString() + "\nreconnnun  " + reconnectNum.ToString();
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
            rtn = robot.ExtAxisParamConfig(1, 1, 1, 1000, -1000, 1000, 1000, 1.905f, 262144, 200, 1, 0, 0);
            Console.WriteLine("ExtAxisParamConfig axis 1 rtn is " + rtn);
            rtn = robot.ExtAxisParamConfig(2, 1, 1, 1000, -1000, 1000, 1000, 4.444f, 262144, 200, 1, 0, 0);
            Console.WriteLine("ExtAxisParamConfig axis 2 rtn is " + rtn);

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
        }

        private void button66_Click(object sender, EventArgs e)
        {
            int rtn = robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 200, 1, 100, 5,1);
            Console.WriteLine("ExtDevSetUDPComParam rtn is " + rtn);
            string ip = ""; int port = 0; int period = 0; int lossPkgTime = 0; int lossPkgNum = 0; int disconnectTime = 0; int reconnectEnable = 0; int reconnectPeriod = 0; int reconnectNum = 0;
            rtn = robot.ExtDevGetUDPComParam(ref ip, ref port, ref period, ref lossPkgTime, ref lossPkgNum, ref disconnectTime, ref reconnectEnable, ref reconnectPeriod, ref reconnectNum);
            string param = "\nip " + ip + "\nport " + port.ToString() + "\nperiod  " + period.ToString() + "\nlossPkgTime " + lossPkgTime.ToString() + "\nlossPkgNum  " + lossPkgNum.ToString() + "\ndisConntime  " + disconnectTime.ToString() + "\nreconnecable  " + reconnectEnable.ToString() + "\nreconnperiod  " + reconnectPeriod.ToString() + "\nreconnnun  " + reconnectNum.ToString();
            Console.WriteLine("ExtDevGetUDPComParam rtn is " + rtn + param);

            robot.ExtDevLoadUDPDriver();

            rtn = robot.ExtAxisServoOn(1, 1);
            Console.WriteLine("ExtAxisServoOn axis id 1 rtn is " + rtn);
            rtn = robot.ExtAxisServoOn(2, 1);
            Console.WriteLine("ExtAxisServoOn axis id 2 rtn is " + rtn);
            Thread.Sleep(2000);

            robot.ExtAxisSetHoming(1, 0, 10, 2);
            Thread.Sleep(2000);
            rtn = robot.ExtAxisSetHoming(2, 0, 10, 2);
            Console.WriteLine("ExtAxisSetHoming rtnn is  " + rtn);

            Thread.Sleep(4000);

            rtn = robot.SetRobotPosToAxis(1);
            Console.WriteLine("SetRobotPosToAxis rtn is " + rtn);
            rtn = robot.SetAxisDHParaConfig(1, 128.5f, 206.4f, 0, 0, 0, 0, 0, 0);
            Console.WriteLine("SetAxisDHParaConfig rtn is " + rtn);
            rtn = robot.ExtAxisParamConfig(1, 1, 1, 1000, -1000, 1000, 1000, 1.905f, 262144, 200, 1, 0, 0);
            Console.WriteLine("ExtAxisParamConfig axis 1 rtn is " + rtn);
            rtn = robot.ExtAxisParamConfig(2, 1, 1, 1000, -1000, 1000, 1000, 4.444f, 262144, 200, 1, 0, 0);
            Console.WriteLine("ExtAxisParamConfig axis 1 rtn is " + rtn);

            DescPose toolCoord = new DescPose(0, 0, 210, 0, 0, 0);
            robot.SetToolCoord(1, toolCoord, 0, 0, 1, 0);

            JointPos jSafe = new JointPos(115.193f, -96.149f, 92.489f, -87.068f, -89.15f, -83.488f);
            JointPos j1 = new JointPos(117.559f, -92.624f, 100.329f, -96.909f, -94.057f, -83.488f);
            JointPos j2 = new JointPos(112.239f, -90.096f, 99.282f, -95.909f, -89.824f, -83.488f);
            JointPos j3 = new JointPos(110.839f, -83.473f, 93.166f, -89.22f, -90.499f, -83.487f);
            JointPos j4 = new JointPos(107.935f, -83.572f, 95.424f, -92.873f, -87.933f, -83.488f);

            DescPose descSafe = new DescPose();
            DescPose desc1 = new DescPose();
            DescPose desc2 = new DescPose();
            DescPose desc3 = new DescPose();
            DescPose desc4 = new DescPose();
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.GetForwardKin( jSafe,  ref descSafe);
            robot.MoveJ( jSafe,  descSafe, 1, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            Thread.Sleep(2000);

            robot.GetForwardKin( j1, ref desc1);
            robot.MoveJ( j1,  desc1, 1, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            Thread.Sleep(2000);

            DescPose actualTCPPos = new DescPose();
            robot.GetActualTCPPose(0, ref actualTCPPos);
            robot.SetRefPointInExAxisEnd(actualTCPPos);
            rtn = robot.PositionorSetRefPoint(1);
            Console.WriteLine("PositionorSetRefPoint 1 rtn is " + rtn);
            Thread.Sleep(2000);

            robot.MoveJ( jSafe,  descSafe, 1, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.ExtAxisStartJog(1, 0, 50, 50, 10);
            Thread.Sleep(1000);
            robot.ExtAxisStartJog(2, 0, 50, 50, 10);
            Thread.Sleep(1000);
            robot.GetForwardKin( j2, ref desc2);
            rtn = robot.MoveJ( j2,  desc2, 1, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            rtn = robot.PositionorSetRefPoint(2);
            Console.WriteLine("PositionorSetRefPoint 2 rtn is " + rtn);
            Thread.Sleep(2000);

            robot.MoveJ( jSafe,  descSafe, 1, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.ExtAxisStartJog(1, 0, 50, 50, 10);
            Thread.Sleep(1000);
            robot.ExtAxisStartJog(2, 0, 50, 50, 10);
            Thread.Sleep(1000);
            robot.GetForwardKin( j3, ref desc3);
            robot.MoveJ( j3,  desc3, 1, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            rtn = robot.PositionorSetRefPoint(3);
            Console.WriteLine("PositionorSetRefPoint 3 rtn is " + rtn);
            Thread.Sleep(2000);

            robot.MoveJ( jSafe,  descSafe, 1, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.ExtAxisStartJog(1, 0, 50, 50, 10);
            Thread.Sleep(1000);
            robot.ExtAxisStartJog(2, 0, 50, 50, 10);
            Thread.Sleep(1000);
            robot.GetForwardKin(j4, ref desc4);
            robot.MoveJ(j4, desc4, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            rtn = robot.PositionorSetRefPoint(4);
            Console.WriteLine("PositionorSetRefPoint 4 rtn is " + rtn);
            Thread.Sleep(2000);

            DescPose axisCoord = new DescPose();
            robot.PositionorComputeECoordSys(ref axisCoord);
            robot.MoveJ(jSafe, descSafe, 1, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            Console.WriteLine("PositionorComputeECoordSys rtn is {0} {1} {2} {3} {4} {5}", axisCoord.tran.x, axisCoord.tran.y, axisCoord.tran.z, axisCoord.rpy.rx, axisCoord.rpy.ry, axisCoord.rpy.rz);
            rtn = robot.ExtAxisActiveECoordSys(3, 1, axisCoord, 1);
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
            robot.ExtDevSetUDPComParam("192.168.58.2", 2021, 2, 50, 5, 50, 1, 50, 10,1);
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

            rtn = robot.PtpFIRPlanningStart(1000,1000);
            Console.WriteLine("PtpFIRPlanningStart rtn is " + rtn);
            robot.MoveJ( startjointPos,  startdescPose, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.MoveJ( endjointPos,  enddescPose, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.PtpFIRPlanningEnd();
            Console.WriteLine("PtpFIRPlanningEnd rtn is " + rtn);

            robot.LinArcFIRPlanningStart(1000, 1000, 1000, 1000);
            Console.WriteLine("LinArcFIRPlanningStart rtn is " + rtn);
            robot.MoveL( startjointPos,  startdescPose, 0, 0, 100, 100, 100, -1,  exaxisPos, 0, 0,  offdese, 1, 1);
            robot.MoveC( midjointPos,  middescPose, 0, 0, 100, 100,  exaxisPos, 0,  offdese,  endjointPos,  enddescPose, 0, 0, 100, 100,  exaxisPos, 0,  offdese, 100, -1);
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
            robot.MoveJ( startjointPos,  startdescPose, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.MoveJ( endjointPos,  enddescPose, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
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
            robot.MoveJ( startjointPos,  startdescPose, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.MoveJ( endjointPos,  enddescPose, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
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
            robot.MoveJ( startjointPos,  startdescPose, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            robot.MoveJ( endjointPos,  enddescPose, 0, 0, 100, 100, 100,  exaxisPos, -1, 0,  offdese);
            rtn = robot.SingularAvoidEnd();
            Console.WriteLine("SingularAvoidEnd rtn is " + rtn);
        }

        private void button73_Click(object sender, EventArgs e)
        {
            int rtn;
            rtn = robot.TrajectoryJUpLoad("D://zUP/traj.txt");
            Console.WriteLine("Upload TrajectoryJ A " + rtn);

            string traj_file_name = "/fruser/traj/traj.txt";
            rtn = robot.LoadTrajectoryLA(traj_file_name, 1, 2, 0, 2, 100, 200, 1000);
            Console.WriteLine("LoadTrajectoryLA " + traj_file_name + ", rtn is: " + rtn);

            DescPose traj_start_pose = new DescPose();
            rtn = robot.GetTrajectoryStartPose(traj_file_name, ref traj_start_pose);
            Console.WriteLine("GetTrajectoryStartPose is: " + rtn);
            Console.WriteLine("desc_pos:{0},{1},{2},{3},{4},{5}", traj_start_pose.tran.x, traj_start_pose.tran.y, traj_start_pose.tran.z, traj_start_pose.rpy.rx, traj_start_pose.rpy.ry, traj_start_pose.rpy.rz);

            Thread.Sleep(1000);

            robot.SetSpeed(50);
            robot.MoveCart( traj_start_pose, 0, 0, 100, 100, 100, -1, -1);

            rtn = robot.MoveTrajectoryLA();
            Console.WriteLine("MoveTrajectoryLA rtn is: " + rtn);
        }

        private void button74_Click(object sender, EventArgs e)
        {
            int rtn;
            int retval = 0;

            retval = robot.LoadIdentifyDynFilterInit();
            Console.WriteLine("LoadIdentifyDynFilterInit retval is: " + retval);

            retval = robot.LoadIdentifyDynVarInit();
            Console.WriteLine("LoadIdentifyDynVarInit retval is: " + retval);

            JointPos posJ = new JointPos(0,0,0,0,0,0);
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
            int rtn = 0;
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


            robot.MoveJ(startjointPos, startdescPose, 3, 0, 100, 100, 100, exaxisPos, -1, 0, offdese);
            rtn = robot.Circle(midjointPosCir1, middescPoseCir1, 3, 0, 100, 100, exaxisPos, endjointPosCir1, enddescPoseCir1, 3, 0, 100, 100, exaxisPos, 100, -1, offdese, 100, 20);
            Console.WriteLine("Circle1" + rtn);



            rtn = robot.Circle(midjointPosCir2, middescPoseCir2, 3, 0, 100, 100, exaxisPos, endjointPosCir2, enddescPoseCir2, 3, 0, 100, 100, exaxisPos, 100, -1, offdese, 100, 20);
            Console.WriteLine("Circle2" + rtn);

            robot.MoveC(midjointPosMoveC, middescPoseMoveC, 3, 0, 100, 100, exaxisPos, 0, offdese, endjointPosmoveC, enddescPoseMoveC, 3, 0, 100, 100, exaxisPos, 0, offdese, 100, 20);
            rtn = robot.Circle(midjointPosCir3, middescPoseCir3, 3, 0, 100, 100, exaxisPos, endjointPosCir3, enddescPoseCir3, 3, 0, 100, 100, exaxisPos, 100, -1, offdese, 100, 20);
            Console.WriteLine("Circle3" + rtn);
            rtn = robot.MoveL(linejointPos, linedescPose, 3, 0, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            Console.WriteLine("MoveL " + rtn);
            rtn = robot.Circle(midjointPosCir4, middescPoseCir4, 3, 0, 100, 100, exaxisPos, endjointPosCir4, enddescPoseCir4, 3, 0, 100, 100, exaxisPos, 100, -1, offdese, 100, 20);
            Console.WriteLine("Circle4" + rtn);
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
            robot.MoveC(JP12, DP12, 0, 0, 100, 100, exaxisPos, 0, offdese, JP13, DP13, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 20);

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
            robot.Circle(JP15, DP15, 0, 0, 100, 100, exaxisPos, JP16, DP16, 0, 0, 100, 100, exaxisPos, 100, 0, offdese, 100, 20);
            robot.MoveJ(JP17, DP17, 0, 0, 100, 100, 100, exaxisPos, 200, 0, offdese);
            robot.MoveL(JP18, DP18, 0, 0, 100, 100, 100, 100, 0, exaxisPos, 0, 0, offdese);
            robot.MoveC(JP19, DP19, 0, 0, 100, 100, exaxisPos, 0, offdese, JP20, DP20, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 20);
            robot.MoveC(JP21, DP21, 0, 0, 100, 100, exaxisPos, 0, offdese, JP22, DP22, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 20);

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

            robot.MoveL(JP23, DP23, 0, 0, 100, 100, 100, 20, 1, exaxisPos, 0, 0, offdese);
            robot.Circle(JP24, DP24, 0, 0, 100, 100, exaxisPos, JP25, DP25, 0, 0, 100, 100, exaxisPos, 100, 0, offdese, 100, 20);
            robot.Circle(JP26, DP26, 0, 0, 100, 100, exaxisPos, JP27, DP27, 0, 0, 100, 100, exaxisPos, 100, 0, offdese, 100, 20);
            robot.MoveC(JP28, DP28, 0, 0, 100, 100, exaxisPos, 0, offdese, JP29, DP29, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 20);
            robot.Circle(JP30, DP30, 0, 0, 100, 100, exaxisPos, JP31, DP31, 0, 0, 100, 100, exaxisPos, 100, 0, offdese, 100, 20);

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

            robot.MoveL(JP50, DP50, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            robot.MoveC(JP41, DP41, 0, 0, 100, 100, exaxisPos, 0, offdese, JP42, DP42, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 20);
            robot.MoveL(JP43, DP43, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);
            robot.Circle(JP44, DP44, 0, 0, 100, 100, exaxisPos, JP45, DP45, 0, 0, 100, 100, exaxisPos, 100, 0, offdese, 100, 20);
            robot.MoveL(JP46, DP46, 0, 0, 100, 100, 100, 20, 0, exaxisPos, 0, 0, offdese);


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
            robot.MoveC(JP8, DP8, 0, 0, 100, 100, exaxisPos, 0, offdese, JP88, DP88, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1);
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
            robot.MoveC(JP10, DP10, 0, 0, 100, 100, exaxisPos, 0, offdese, JP10_, DP10_, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 120);
            robot.MoveC(JP11, DP11, 0, 0, 100, 100, exaxisPos, 0, offdese, JP11_, DP11_, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1);

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
            robot.MoveC(JP13, DP13, 0, 0, 100, 100, exaxisPos, 0, offdese, JP13_, DP13_, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1);


            robot.MoveL(JP14, DP14, 0, 0, 100, 100, 100, 20, -1, exaxisPos, 0, 0, offdese);
            robot.LinArcFIRPlanningEnd();
        }

        private void button81_Click(object sender, EventArgs e)
        {
            DescPose p1Desc=new DescPose(186.331, 487.913, 209.850, 149.030, 0.688, -114.347);
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

            robot.GetForwardKin(p1Joint,ref p1Desc);
            robot.GetForwardKin(p2Joint, ref p2Desc);
            robot.GetForwardKin(p3Joint, ref p3Desc);
            robot.GetForwardKin(p4Joint, ref p4Desc);
            robot.GetForwardKin(p5Joint, ref p5Desc);
            robot.GetForwardKin(p6Joint, ref p6Desc);

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
            Console.WriteLine($"ComputeFocusCalib coord is  {rtn},{ resultPos.x} ,{ resultPos.y}, { resultPos.z}, accuracy is {accuracy} ");
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

            rtn = robot.SetCtrlFirmwareUpgrade(2, "D://zUP/upgrade/FAIR_Cobot_Cbd_Asix_V2.0.bin");
            Console.WriteLine($"robot SetCtrlFirmwareUpgrade rtn is{rtn}");

            rtn = robot.SetEndFirmwareUpgrade(2, "D://zUP/upgrade/FAIR_Cobot_Axle_Asix_V2.4.bin");
            Console.WriteLine($"robot SetEndFirmwareUpgrade rtn is {rtn}");

            robot.SetSysServoBootMode();
            rtn = robot.SetCtrlFirmwareUpgrade(1, "D://zUP/upgrade/FR_CTRL_PRIMCU_FV201212_MAIN_U4_T01_20250428(MT).bin");
            Console.WriteLine($"robot SetCtrlFirmwareUpgrade rtn is{rtn}");

            rtn = robot.SetEndFirmwareUpgrade(1, "D://zUP/upgrade/FR_END_FV201009_MAIN_U1_T01_20250428.bin");
            Console.WriteLine($"robot SetEndFirmwareUpgrade rtn is {rtn}");

            rtn = robot.SetJointFirmwareUpgrade(1, "D://zUP/upgrade/FR_SERVO_FV504214_MAIN_U7_T07_20250519.bin");
            Console.WriteLine($"robot SetJointFirmwareUpgrade rtn is{rtn}");

        }

        private void button84_Click(object sender, EventArgs e)
        {
            int rtn;
            JointPos joint_pos1 = new JointPos(-68.732, -99.773, -77.729, -77.167, 100.772, -13.317);
            JointPos joint_pos2 = new JointPos(-101.678, -102.823, -77.512, -77.185, 88.388, -13.317 );
            JointPos joint_pos3 = new JointPos(-129.905, -99.715, -71.965, -77.209, 81.678, -13.317 );
            DescPose desc_pos1 =new DescPose(103.887, -434.739, 244.938, -162.495, 6.575, -142.948 );
            DescPose desc_pos2 = new DescPose(-196.883, -418.054, 218.942, -168.196, -4.388, -178.991);
            DescPose desc_pos3 = new DescPose(-396.665, -265.695, 284.380, -160.913, -12.378, 149.770 );

            ExaxisPos epos1 = new ExaxisPos(0.000, 6.996, 0.000, 0.000 );
            ExaxisPos epos2 = new ExaxisPos(0.000, 20.987, 0.000, 0.000 );
            ExaxisPos epos3 = new ExaxisPos(-0.000, 30.982, 0.000, 0.000 );

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
            robot.RobotEnable(0);
            Thread.Sleep(200);
            int rtn = robot.JointAllParamUpgrade("D://zUP/standardQX/jointallparametersFR56.0.db");
            Console.WriteLine($"robot JointAllParamUpgrade rtn is{rtn}");

            rtn = robot.SetCtrlFirmwareUpgrade(2, "D://zUP/upgrade/FAIR_Cobot_Cbd_Asix_V2.0.bin");
            Console.WriteLine($"robot SetCtrlFirmwareUpgrade rtn is{rtn}");


            rtn = robot.SetEndFirmwareUpgrade(2, "D://zUP/upgrade/FAIR_Cobot_Axle_Asix_V2.4.bin");
            Console.WriteLine($"robot SetEndFirmwareUpgrade rtn is {rtn}");

            robot.SetSysServoBootMode();
            rtn = robot.SetCtrlFirmwareUpgrade(1, "D://zUP/standardQX/FR_CTRL_PRIMCU_FV201011_MAIN_U4_T01_20250208.bin");
            Console.WriteLine($"robot SetCtrlFirmwareUpgrade rtn is{rtn}");

            rtn = robot.SetEndFirmwareUpgrade(1, "D://zUP/standardQX/FR_END_FV201008_MAIN_U01_T01_20250416.bin");
            Console.WriteLine($"robot SetEndFirmwareUpgrade rtn is {rtn}");

            rtn = robot.SetJointFirmwareUpgrade(1, "D://zUP/standardQX/FR_SERVO_FV502211_MAIN_U7_T07_20250217.bin");
            Console.WriteLine($"robot SetJointFirmwareUpgrade rtn is{rtn}");
        }
    }

}

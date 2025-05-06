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

            JointPos p1Joint = new JointPos(88.927, -85.834, 80.289, -85.561, -91.388, 108.718);
            DescPose p1Desc = new DescPose(88.739, -527.617, 514.939, -179.039, 1.494, 70.209);

            JointPos p2Joint = new JointPos(27.036, -83.909, 80.284, -85.579, -90.027, 108.604);
            DescPose p2Desc = new DescPose(-433.125, -334.428, 497.139, -179.723, -0.745, 8.437);


            JointPos p3Joint = new JointPos(60.219, -94.324, 62.906, -62.005, -87.159, 108.598);
            DescPose p3Desc = new DescPose(-112.215, -409.323, 686.497, 176.217, 2.338, 41.625);
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);


            //robot.AccSmoothStart(saveFlag);
            //robot.MoveL(p1Joint, p1Desc, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0, 10);
            // robot.MoveL(p2Joint, p2Desc, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0, 10);
            //robot.MoveL(p1Joint, p1Desc, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0, 10);
            //robot.MoveL(p2Joint, p2Desc, 0, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 0, 10);

            //robot.AccSmoothEnd(saveFlag);

            //robot.AccSmoothStart(saveFlag);
            // rtn = robot.MoveC(p3Joint, p3Desc, 0, 0, 100, 100, exaxisPos, 0, offdese, p1Joint, p1Desc, 0, 0, 100, 100, exaxisPos, 100, offdese, 100, -1);
            //robot.AccSmoothEnd(saveFlag);

            robot.AccSmoothStart(saveFlag);
            robot.Circle(p3Joint, p3Desc, 0, 0, 100, 100, exaxisPos, p2Joint, p2Desc, 0, 0, 100, 100, exaxisPos, 100, 0, offdese);
            //rtn = robot.Circle(p3Joint, p3Desc, 0, 0, 100, 100, exaxisPos, 0, offdese, p1Joint, p1Desc, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1);
            robot.AccSmoothEnd(saveFlag);
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
            retval = robot.MoveL(homejointPos, homePose, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
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
            robot.MoveL(startjointPos, startdescPose, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            robot.MoveL(startjointPos, startdescPose, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);

            // 结束跟踪
            retval = robot.ConveyorTrackEnd();
            Console.WriteLine($"ConveyorTrackEnd 返回值: {retval}");

            // 返回安全位置
            robot.MoveL(homejointPos, homePose, 1, 1, 100, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);

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

            //Console.WriteLine("RbLogDownload start");
            //int rtn = robot.RbLogDownload(@"D:\zDOWN1\");
            //Console.WriteLine($"RbLogDownload rtn is {rtn}");


            Console.WriteLine("AllDataSourceDownload start");
            int rtn = robot.AllDataSourceDownload(@"D:\zDOWN\");
            Console.WriteLine($"AllDataSourceDownload rtn is {rtn}");

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




        }

        private void Test_Load(object sender, EventArgs e)
        {

        }




    }

}

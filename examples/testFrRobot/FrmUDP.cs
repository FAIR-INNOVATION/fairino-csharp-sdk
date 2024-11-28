using fairino;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace testFrRobot
{
    public partial class FrmUDP : Form
    {
        Robot robot;
        public FrmUDP(Robot tmpRobot)
        {
            robot = tmpRobot;
            InitializeComponent();
            comboAxisID.SelectedIndex = 0;
        }



        private void btnSetDO_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 128; i++)
            {
                robot.SetAuxDO(i, true, false, true);
                Thread.Sleep(200);
                Console.WriteLine($"SetAuxDO Value{true}");
            }
            for (int i = 0; i < 128; i++)
            {
                robot.SetAuxDO(i, false, false, true);
                Thread.Sleep(200);
                Console.WriteLine($"SetAuxDO Value{false}");
            }
        }

        private void btnSetAO_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 409; i++)
            {
                robot.SetAuxAO(0, i * 10, true);
                Console.WriteLine($"SetAuxAO id{0}, Value{i * 10}");
                robot.SetAuxAO(1, 4095 - i * 10, true);
                Console.WriteLine($"SetAuxAO id{1}, Value{4095 - i * 10}");
                robot.SetAuxAO(2, i * 10, true);
                Console.WriteLine($"SetAuxAO id{2}, Value{i * 10}");
                robot.SetAuxAO(3, 4095 - i * 10, true);
                Console.WriteLine($"SetAuxAO id{3}, Value{4095 - i * 10}");
                Thread.Sleep(10);

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int rtn = robot.SetAuxDIFilterTime(10);
            Console.WriteLine(rtn);
            robot.SetAuxAIFilterTime(0, 10);

            //for (int i = 0; i < 20; i++)
            //{
            //    bool curValue = false;
            //    int rtn = robot.GetAuxDI(i, false, ref curValue);
            //    txtRtn.Text = rtn.ToString();
            //    Console.Write($"DI{i}  {curValue}  ");
            //    Console.WriteLine("  ");
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int curValue = -1;
            int rtn = 0;
            for (int i = 0; i < 4; i++)
            {
                rtn = robot.GetAuxAI(i, true, ref curValue);
                txtRtn.Text = rtn.ToString();
                Console.Write($"AI{i} {curValue}   rtn is {rtn} ");
                Console.WriteLine("  ");
            }

            robot.WaitAuxDI(1, true, 1000, false);
            robot.WaitAuxAI(1, 1, 132, 1000, false);
        }

        private void btnSetUDPParam_Click(object sender, EventArgs e)
        {
            robot.ExtDevSetUDPComParam(txtip.Text, int.Parse(txtport.Text), int.Parse(txtperiod.Text), int.Parse(txtchackTime.Text), int.Parse(txtLossnum.Text), int.Parse(txtDisconntime.Text), int.Parse(tctenabke.Text), int.Parse(txtreconntperiod.Text), int.Parse(txtreconntnum.Text));
            robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 100, 1, 100, 10);
        }

        private void btnGetStartPos_Click(object sender, EventArgs e)
        {
            DescPose descPose = new DescPose(0, 0, 0, 0, 0, 0);
            int rtn = robot.GetActualTCPPose(0, ref descPose);
            txtStartX.Text = descPose.tran.x.ToString();
            txtStartY.Text = descPose.tran.y.ToString();
            txtStartZ.Text = descPose.tran.z.ToString();
            txtStartRX.Text = descPose.rpy.rx.ToString();
            txtStartRY.Text = descPose.rpy.ry.ToString();
            txtStartRZ.Text = descPose.rpy.rz.ToString();

            JointPos jointPos = new JointPos(0, 0, 0, 0, 0, 0);
            rtn = robot.GetActualJointPosDegree(0, ref jointPos);
            txtStartJ1.Text = jointPos.jPos[0].ToString();
            txtStartJ2.Text = jointPos.jPos[1].ToString();
            txtStartJ3.Text = jointPos.jPos[2].ToString();
            txtStartJ4.Text = jointPos.jPos[3].ToString();
            txtStartJ5.Text = jointPos.jPos[4].ToString();
            txtStartJ6.Text = jointPos.jPos[5].ToString();
        }

        private void btnGetEndPos_Click(object sender, EventArgs e)
        {
            DescPose descPose = new DescPose(0, 0, 0, 0, 0, 0);
            int rtn = robot.GetActualTCPPose(0, ref descPose);
            txtEndX.Text = descPose.tran.x.ToString();
            txtEndY.Text = descPose.tran.y.ToString();
            txtEndZ.Text = descPose.tran.z.ToString();
            txtEndRX.Text = descPose.rpy.rx.ToString();
            txtEndRY.Text = descPose.rpy.ry.ToString();
            txtEndRZ.Text = descPose.rpy.rz.ToString();

            JointPos jointPos = new JointPos(0, 0, 0, 0, 0, 0);
            rtn = robot.GetActualJointPosDegree(0, ref jointPos);
            txtEndJ1.Text = jointPos.jPos[0].ToString();
            txtEndJ2.Text = jointPos.jPos[1].ToString();
            txtEndJ3.Text = jointPos.jPos[2].ToString();
            txtEndJ4.Text = jointPos.jPos[3].ToString();
            txtEndJ5.Text = jointPos.jPos[4].ToString();
            txtEndJ6.Text = jointPos.jPos[5].ToString();
        }

        private void btnSyncJ_Click(object sender, EventArgs e)
        {
            //1.标定并应用机器人工具坐标系，您可以使用四点法或六点法进行工具坐标系的标定和应用，涉及工具坐标系标定的接口如下：
            //    int SetToolPoint(int point_num);  //设置工具参考点-六点法
            //    int ComputeTool(ref DescPose tcp_pose);  //计算工具坐标系
            //    int SetTcp4RefPoint(int point_num);    //设置工具参考点-四点法
            //    int ComputeTcp4(ref DescPose tcp_pose);   //计算工具坐标系-四点法
            //    int SetToolCoord(int id, DescPose coord, int type, int install);  //设置应用工具坐标系
            //    int SetToolList(int id, DescPose coord, int type, int install);   //设置应用工具坐标系列表

            //2.设置UDP通信参数，并加载UDP通信
            robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 100, 1, 100, 10);
            robot.ExtDevLoadUDPDriver();

            //3.设置扩展轴参数，包括扩展轴类型、扩展轴驱动器参数、扩展轴DH参数
            robot.SetAxisDHParaConfig(4, 200, 200, 0, 0, 0, 0, 0, 0); //单轴变位机及DH参数
            robot.SetRobotPosToAxis(1);  //扩展轴安装位置
            robot.ExtAxisParamConfig(1, 0, 1, 100, -100, 10, 10, 12, 131072, 0, 1, 0, 0); //伺服驱动器参数，本示例为单轴变位机，因此只需要设置一个驱动器参数，若您选择包含多个轴的扩展轴类型，需要每一个轴设置驱动器参数

            //4.设置所选的轴使能、回零
            robot.ExtAxisServoOn(1, 0);
            robot.ExtAxisSetHoming(1, 0, 20, 3);

            //5.进行扩展轴坐标系标定及应用
            DescPose pos = new DescPose(/* 输入您的标定点坐标 */);
            robot.SetRefPointInExAxisEnd(pos);
            robot.PositionorSetRefPoint(1); /*您需要通过四个不同位置的点来标定扩展轴，因此需要调用此接口4次才能完成标定 */
            DescPose coord = new DescPose();
            robot.PositionorComputeECoordSys(ref coord); //计算扩展轴标定结果
            robot.ExtAxisActiveECoordSys(1, 1, coord, 1);  //将标定结果应用到扩展轴坐标系

            //6.在扩展轴上标定工件坐标系，您需要用到以下接口
            //int SetWObjCoordPoint(int point_num);
            //int ComputeWObjCoord(int method, ref DescPose wobj_pose);
            //int SetWObjCoord(int id, DescPose coord);
            //int SetWObjList(int id, DescPose coord);

            //7.记录您的同步关节运动起始点
            DescPose startdescPose = new DescPose(/*输入您的坐标*/);
            JointPos startjointPos = new JointPos(/*输入您的坐标*/);
            ExaxisPos startexaxisPos = new ExaxisPos(/* 输入您的扩展轴起始点坐标 */);

            //8.记录您的同步关节运动终点坐标
            DescPose enddescPose = new DescPose(/*输入您的坐标*/);
            JointPos endjointPos = new JointPos(/*输入您的坐标*/);
            ExaxisPos endexaxisPos = new ExaxisPos(/* 输入您的扩展轴终点坐标 */);

            //9.编写同步运动程序
            //运动到起始点，假设应用的工具坐标系、工件坐标系都是1
            robot.ExtAxisMove(startexaxisPos, 20);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            robot.MoveJ(startjointPos, startdescPose, 1, 1, 100, 100, 100, startexaxisPos, 0, 0, offdese);

            //开始同步运动
            robot.ExtAxisSyncMoveJ(endjointPos, enddescPose, 1, 1, 100, 100, 100, endexaxisPos, -1, 0, offdese);

            //DescPose startdescPose = new DescPose(double.Parse(txtStartX.Text), double.Parse(txtStartY.Text), double.Parse(txtStartZ.Text), double.Parse(txtStartRX.Text), double.Parse(txtStartRY.Text), double.Parse(txtStartRZ.Text));
            //JointPos startjointPos = new JointPos(double.Parse(txtStartJ1.Text), double.Parse(txtStartJ2.Text), double.Parse(txtStartJ3.Text), double.Parse(txtStartJ4.Text), double.Parse(txtStartJ5.Text), double.Parse(txtStartJ6.Text));

            //ExaxisPos exaxisPos = new ExaxisPos(double.Parse(textBox4.Text), double.Parse(textBox3.Text), double.Parse(textBox2.Text), double.Parse(textBox1.Text));
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            //int rtn = robot.ExtAxisSyncMoveJ(startjointPos, startdescPose, 1, 1, 100, 100, 100, exaxisPos, -1, 0, offdese);
            //txtRtn.Text = rtn.ToString();

            //在同步运动前，请检查以下配置

            //1.工具坐标系标定及应用
            //2.扩展轴UDP通信加载
            //3.扩展轴类型及DH参数配置（变位机、直线导轨等DH参数配置），配置的轴已使能且完成回零
            //4.扩展轴坐标系标定及应用（用四点法进行扩展轴坐标系标定，并应用该坐标系）
            //5.扩展轴上工件坐标系标定及应用
            //6.输入的工具、工件坐标系要与标定完成并应用的坐标系对应
            //7.输入的笛卡尔坐标为工件坐标系上的坐标



            ExaxisPos exaxisPos = new ExaxisPos();
            DescPose offdesc = new DescPose(0, 0, 0, 0, 0, 0);

            int rtn = robot.ExtAxisSyncMoveJ(startjointPos, startdescPose, 1, 1, 100, 100, 100, exaxisPos, -1, 0, offdesc);
        }

        private void btnsyncL_Click(object sender, EventArgs e)
        {
            DescPose startdescPose = new DescPose(double.Parse(txtStartX.Text), double.Parse(txtStartY.Text), double.Parse(txtStartZ.Text), double.Parse(txtStartRX.Text), double.Parse(txtStartRY.Text), double.Parse(txtStartRZ.Text));
            JointPos startjointPos = new JointPos(double.Parse(txtStartJ1.Text), double.Parse(txtStartJ2.Text), double.Parse(txtStartJ3.Text), double.Parse(txtStartJ4.Text), double.Parse(txtStartJ5.Text), double.Parse(txtStartJ6.Text));

            ExaxisPos exaxisPos = new ExaxisPos(double.Parse(textBox4.Text), double.Parse(textBox3.Text), double.Parse(textBox2.Text), double.Parse(textBox1.Text));
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            //robot.MoveL(startjointPos, startdescPose, 1, 1, 100, 100, 100, 0, exaxisPos, 0, 0, offdese);
            robot.ExtAxisSyncMoveL(startjointPos, startdescPose, 1, 1, 100, 100, 100, 0, exaxisPos, 0, offdese);
        }

        private void btnSyncC_Click(object sender, EventArgs e)
        {
            DescPose startdescPose = new DescPose(double.Parse(txtStartX.Text), double.Parse(txtStartY.Text), double.Parse(txtStartZ.Text), double.Parse(txtStartRX.Text), double.Parse(txtStartRY.Text), double.Parse(txtStartRZ.Text));
            JointPos startjointPos = new JointPos(double.Parse(txtStartJ1.Text), double.Parse(txtStartJ2.Text), double.Parse(txtStartJ3.Text), double.Parse(txtStartJ4.Text), double.Parse(txtStartJ5.Text), double.Parse(txtStartJ6.Text));

            DescPose enddescPose = new DescPose(double.Parse(txtEndX.Text), double.Parse(txtEndY.Text), double.Parse(txtEndZ.Text), double.Parse(txtEndRX.Text), double.Parse(txtEndRY.Text), double.Parse(txtEndRZ.Text));
            JointPos endjointPos = new JointPos(double.Parse(txtEndJ1.Text), double.Parse(txtEndJ2.Text), double.Parse(txtEndJ3.Text), double.Parse(txtEndJ4.Text), double.Parse(txtEndJ5.Text), double.Parse(txtEndJ6.Text));
            ExaxisPos exaxisPos1 = new ExaxisPos(-10, 0, 0, 0);
            ExaxisPos exaxisPos = new ExaxisPos(double.Parse(textBox4.Text), double.Parse(textBox3.Text), double.Parse(textBox2.Text), double.Parse(textBox1.Text));
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            //robot.MoveC(startjointPos, startdescPose, 1, 1, 100, 100, exaxisPos, 0, offdese, endjointPos, enddescPose, 1, 1, 100, 100, exaxisPos, 0, offdese, 100, 0);
            robot.ExtAxisSyncMoveC(startjointPos, startdescPose, 1, 1, 100, 100, exaxisPos1, 0, offdese, endjointPos, enddescPose, 1, 1, 100, 100, exaxisPos, 0, offdese, 100, 0);
        }

        private void btnMoveAO_Click(object sender, EventArgs e)
        {
            //DescPose startdescPose = new DescPose(double.Parse(txtStartX.Text), double.Parse(txtStartY.Text), double.Parse(txtStartZ.Text), double.Parse(txtStartRX.Text), double.Parse(txtStartRY.Text), double.Parse(txtStartRZ.Text));
            //JointPos startjointPos = new JointPos(double.Parse(txtStartJ1.Text), double.Parse(txtStartJ2.Text), double.Parse(txtStartJ3.Text), double.Parse(txtStartJ4.Text), double.Parse(txtStartJ5.Text), double.Parse(txtStartJ6.Text));
            //DescPose enddescPose = new DescPose(double.Parse(txtEndX.Text), double.Parse(txtEndY.Text), double.Parse(txtEndZ.Text), double.Parse(txtEndRX.Text), double.Parse(txtEndRY.Text), double.Parse(txtEndRZ.Text));
            //JointPos endjointPos = new JointPos(double.Parse(txtEndJ1.Text), double.Parse(txtEndJ2.Text), double.Parse(txtEndJ3.Text), double.Parse(txtEndJ4.Text), double.Parse(txtEndJ5.Text), double.Parse(txtEndJ6.Text));
            //DescPose CPose = new DescPose(double.Parse(txtCX.Text), double.Parse(txtCY.Text), double.Parse(txtCZ.Text), double.Parse(txtCRX.Text), double.Parse(txtCRY.Text), double.Parse(txtCRZ.Text));
            //JointPos CJPos = new JointPos(double.Parse(txtCJ1.Text), double.Parse(txtCJ2.Text), double.Parse(txtCJ3.Text), double.Parse(txtCJ4.Text), double.Parse(txtCJ5.Text), double.Parse(txtCJ6.Text));
            //DescPose DPose = new DescPose(double.Parse(txtDX.Text), double.Parse(txtDY.Text), double.Parse(txtDZ.Text), double.Parse(txtDRX.Text), double.Parse(txtDRY.Text), double.Parse(txtDRZ.Text));
            //JointPos DJPos = new JointPos(double.Parse(txtDJ1.Text), double.Parse(txtDJ2.Text), double.Parse(txtDJ3.Text), double.Parse(txtDJ4.Text), double.Parse(txtDJ5.Text), double.Parse(txtDJ6.Text));

            DescPose startdescPose = new DescPose();
            JointPos startjointPos = new JointPos();
            DescPose enddescPose = new DescPose();
            JointPos endjointPos = new JointPos();
            DescPose CPose = new DescPose();
            JointPos CJPos = new JointPos();
            DescPose DPose = new DescPose();
            JointPos DJPos = new JointPos();

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            int rtn = robot.MoveToolAOStart(0, 100, 80, 1);
            //int rtn = robot.MoveAOStart(0, 100, 80, 1);
            Console.WriteLine(rtn);

            rtn = robot.MoveL(startjointPos, startdescPose, 0, 0, 100, 100, 100, 0, exaxisPos, 0, 0, offdese);
            //robot.MoveJ(startjointPos, startdescPose, 0, 0, 100, 100, 100, exaxisPos, 0, 0, offdese);
            //robot.MoveC(startjointPos, startdescPose, 0, 0, 100, 100, exaxisPos, 0, offdese, endjointPos, enddescPose, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, 0);
            //robot.Circle(startjointPos, startdescPose, 0, 0, 100, 100, exaxisPos, endjointPos, enddescPose, 0, 0, 100, 100, exaxisPos, 100, 0, offdese);
            //robot.SplineStart();
            //robot.SplinePTP(startjointPos, startdescPose, 0, 0, 100, 100, 100);
            //robot.SplinePTP(endjointPos, enddescPose, 0, 0, 100, 100, 100);
            //robot.SplinePTP(CJPos, CPose, 0, 0, 100, 100, 100);
            //robot.SplinePTP(DJPos, DPose, 0, 0, 100, 100, 100);
            //robot.SplineEnd();

            //robot.NewSplineStart(0, 5000);
            //robot.NewSplinePoint(startjointPos, startdescPose, 0, 0, 100, 100, 100, 5, 0);
            //robot.NewSplinePoint(endjointPos, enddescPose, 0, 0, 100, 100, 100, 5, 0);
            //robot.NewSplinePoint(CJPos, CPose, 0, 0, 100, 100, 100, 5, 0);
            //robot.NewSplinePoint(DJPos, DPose, 0, 0, 100, 100, 100, 5, 1);
            //robot.NewSplineEnd();
            //int count = 1000;
            //while (count > 0)
            //{
            //    robot.ServoJ(startjointPos, 0, 0, 0.008f, 0, 0);
            //    startjointPos.jPos[0] += 0.01;//0关节位置增加
            //    count -= 1;
            //}


            rtn = robot.MoveToolAOStop();
            //rtn = robot.MoveAOStop();
            Console.WriteLine(rtn);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DescPose descPose = new DescPose(0, 0, 0, 0, 0, 0);
            int rtn = robot.GetActualTCPPose(0, ref descPose);
            txtCX.Text = descPose.tran.x.ToString();
            txtCY.Text = descPose.tran.y.ToString();
            txtCZ.Text = descPose.tran.z.ToString();
            txtCRX.Text = descPose.rpy.rx.ToString();
            txtCRY.Text = descPose.rpy.ry.ToString();
            txtCRZ.Text = descPose.rpy.rz.ToString();

            JointPos jointPos = new JointPos(0, 0, 0, 0, 0, 0);
            rtn = robot.GetActualJointPosDegree(0, ref jointPos);
            txtCJ1.Text = jointPos.jPos[0].ToString();
            txtCJ2.Text = jointPos.jPos[1].ToString();
            txtCJ3.Text = jointPos.jPos[2].ToString();
            txtCJ4.Text = jointPos.jPos[3].ToString();
            txtCJ5.Text = jointPos.jPos[4].ToString();
            txtCJ6.Text = jointPos.jPos[5].ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DescPose descPose = new DescPose(0, 0, 0, 0, 0, 0);
            int rtn = robot.GetActualTCPPose(0, ref descPose);
            txtDX.Text = descPose.tran.x.ToString();
            txtDY.Text = descPose.tran.y.ToString();
            txtDZ.Text = descPose.tran.z.ToString();
            txtDRX.Text = descPose.rpy.rx.ToString();
            txtDRY.Text = descPose.rpy.ry.ToString();
            txtDRZ.Text = descPose.rpy.rz.ToString();

            JointPos jointPos = new JointPos(0, 0, 0, 0, 0, 0);
            rtn = robot.GetActualJointPosDegree(0, ref jointPos);
            txtDJ1.Text = jointPos.jPos[0].ToString();
            txtDJ2.Text = jointPos.jPos[1].ToString();
            txtDJ3.Text = jointPos.jPos[2].ToString();
            txtDJ4.Text = jointPos.jPos[3].ToString();
            txtDJ5.Text = jointPos.jPos[4].ToString();
            txtDJ6.Text = jointPos.jPos[5].ToString();
        }

        private void btnGetUDPParam_Click(object sender, EventArgs e)
        {
            string ip = "";
            int port = 0;
            int period = 0;
            int checktime = 0;
            int checknum = 0;
            int disconntime = 0;
            int reconnenable = 0;
            int reconntime = 0;
            int reconnnum = 0;
            robot.ExtDevGetUDPComParam(ref ip, ref port, ref period, ref checktime, ref checknum, ref disconntime, ref reconntime, ref reconnenable, ref reconnnum);
            lblgetUDP.Text = $"{ip}  {port}  {period} {checktime}  {checknum}  {disconntime}  {reconnenable}  {reconntime}  {reconnnum}";
        }

        private void lblgetUDP_Click(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            int rtn = robot.ExtDevLoadUDPDriver();
            txtRtn.Text = rtn.ToString();
        }

        private void btnUnLoad_Click(object sender, EventArgs e)
        {
            txtRtn.Text = "";
            int rtn = robot.ExtDevUnloadUDPDriver();
            txtRtn.Text = rtn.ToString();
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            int rtn = robot.ExtAxisServoOn(int.Parse(comboAxisID.Text), 1);
            txtRtn.Text = rtn.ToString();
        }

        private void btnUnable_Click(object sender, EventArgs e)
        {
            int rtn = robot.ExtAxisServoOn(int.Parse(comboAxisID.Text), 0);
            txtRtn.Text = rtn.ToString();
        }

        private void btnJogR_Click(object sender, EventArgs e)
        {
            int rtn = robot.ExtAxisStartJog(int.Parse(comboAxisID.Text), 1, int.Parse(txtMoveApeed.Text), int.Parse(txtAcc.Text), int.Parse(txtJogDis.Text));
            txtRtn.Text = rtn.ToString();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            int rtn = robot.ExtAxisStopJog(int.Parse(comboAxisID.Text));
            txtRtn.Text = rtn.ToString();
        }

        private void btnHoming_Click(object sender, EventArgs e)
        {
            int rtn = robot.ExtAxisSetHoming(1, 0, 10, 3);
            txtRtn.Text = rtn.ToString();
        }

        private void btnJogL_Click(object sender, EventArgs e)
        {
            int rtn = robot.ExtAxisStartJog(int.Parse(comboAxisID.Text), 0, int.Parse(txtMoveApeed.Text), int.Parse(txtAcc.Text), int.Parse(txtJogDis.Text));
            txtRtn.Text = rtn.ToString();
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            ExaxisPos pos = new ExaxisPos(double.Parse(textBox4.Text), double.Parse(textBox3.Text), double.Parse(textBox2.Text), double.Parse(textBox1.Text));
            int rtn = robot.ExtAxisMove(pos, 10);
            txtRtn.Text = rtn.ToString();
        }

        private void btnPoint1_Click(object sender, EventArgs e)
        {
            int rtn = robot.ExtAxisSetRefPoint(1);
            txtRtn.Text = rtn.ToString();
        }

        private void btnPoint2_Click(object sender, EventArgs e)
        {
            int rtn = robot.ExtAxisSetRefPoint(2);
            txtRtn.Text = rtn.ToString();
        }

        private void btnPoint3_Click(object sender, EventArgs e)
        {
            int rtn = robot.ExtAxisSetRefPoint(3);
            txtRtn.Text = rtn.ToString();
        }

        private void btnpoint4_Click(object sender, EventArgs e)
        {
            int rtn = robot.ExtAxisSetRefPoint(4);
            txtRtn.Text = rtn.ToString();
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            DescPose pos = new DescPose();
            int rtn = robot.ExtAxisComputeECoordSys(ref pos);
            txtRtn.Text = rtn.ToString();

            txtCoordx.Text = pos.tran.x.ToString();
            txtCoordy.Text = pos.tran.y.ToString();
            txtCoordz.Text = pos.tran.z.ToString();
            txtCoordrx.Text = pos.rpy.rx.ToString();
            txtCoordry.Text = pos.rpy.ry.ToString();
            txtCoordrz.Text = pos.rpy.rz.ToString();

            DescPose refPointPos = new DescPose(122.0, 312.0, 0, 0, 0, 0);
            robot.SetRefPointInExAxisEnd(refPointPos);

            robot.PositionorSetRefPoint(1);
            //robot.PositionorSetRefPoint(2);
            //robot.PositionorSetRefPoint(3);
            //robot.PositionorSetRefPoint(4);

            //DescPose coord = new DescPose();
            //robot.PositionorComputeECoordSys(ref coord);
        }

        private void btnApplyCoord_Click(object sender, EventArgs e)
        {
            DescPose pos = new DescPose(double.Parse(txtCoordx.Text), double.Parse(txtCoordy.Text), double.Parse(txtCoordz.Text), double.Parse(txtCoordrx.Text), double.Parse(txtCoordry.Text), double.Parse(txtCoordrz.Text));
            int rtn = robot.ExtAxisActiveECoordSys(2, 2, pos, 1);
            txtRtn.Text = rtn.ToString();
        }

        private void btnSetAxisparam_Click(object sender, EventArgs e)
        {
            int rtn = 0;

            rtn = robot.SetAxisDHParaConfig(4, 200, 200, 0, 0, 0, 0, 0, 0);
            Console.WriteLine($"SetAxisDHParaConfig rtn is {rtn}");
            rtn = robot.SetRobotPosToAxis(1);
            Console.WriteLine($"SetRobotPosToAxis rtn is {rtn}");
            rtn = robot.ExtAxisParamConfig(1, 0, 1, 100, -100, 10, 10, 12, 131072, 0, 1, 0, 0);
            Console.WriteLine($"ExtAxisParamConfig rtn is {rtn}");

        }

        private void btnStable_Click(object sender, EventArgs e)
        {
            ROBOT_STATE_PKG pKG = new ROBOT_STATE_PKG();
            robot.GetRobotRealTimeState(ref pKG);
            Console.WriteLine(pKG.extDIState[2]);
            Console.WriteLine(Marshal.SizeOf(pKG));
            return;

            while (true)
            {

                int rtn = 0;
                ExaxisPos pos = new ExaxisPos(double.Parse(textBox4.Text), double.Parse(textBox3.Text), double.Parse(textBox2.Text), double.Parse(textBox1.Text));
                rtn = robot.ExtAxisMove(pos, 10);
                pos.ePos[1] += 10;
                txtRtn.Text = rtn.ToString();
                rtn = robot.ExtAxisMove(pos, 10);
                txtRtn.Text = rtn.ToString();
                for (int i = 0; i < 6; i++)
                {
                    robot.SetAuxDO(i, true, false, true);
                    Thread.Sleep(200);
                }
                for (int i = 0; i < 6; i++)
                {
                    robot.SetAuxDO(i, false, false, true);
                    Thread.Sleep(200);
                }

                for (int i = 0; i < 4; i++)
                {
                    robot.SetAuxAO(0, i * 10, true);
                    robot.SetAuxAO(1, 4095 - i * 10, true);
                    robot.SetAuxAO(2, i * 10, true);
                    robot.SetAuxAO(3, 4095 - i * 10, true);
                    Thread.Sleep(10);
                }
                Thread.Sleep(1000);
            }


        }

        private void tesy()
        {
            //1.标定并应用机器人工具坐标系，您可以使用四点法或六点法进行工具坐标系的标定和应用，涉及工具坐标系标定的接口如下：
            //    int SetToolPoint(int point_num);  //设置工具参考点-六点法
            //    int ComputeTool(ref DescPose tcp_pose);  //计算工具坐标系
            //    int SetTcp4RefPoint(int point_num);    //设置工具参考点-四点法
            //    int ComputeTcp4(ref DescPose tcp_pose);   //计算工具坐标系-四点法
            //    int SetToolCoord(int id, DescPose coord, int type, int install);  //设置应用工具坐标系
            //    int SetToolList(int id, DescPose coord, int type, int install);   //设置应用工具坐标系列表

            //2.设置UDP通信参数，并加载UDP通信
            robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 100, 1, 100, 10);
            robot.ExtDevLoadUDPDriver();

            //3.设置扩展轴参数，包括扩展轴类型、扩展轴驱动器参数、扩展轴DH参数
            robot.SetAxisDHParaConfig(4, 200, 200, 0, 0, 0, 0, 0, 0); //单轴变位机及DH参数
            robot.SetRobotPosToAxis(1);  //扩展轴安装位置
            robot.ExtAxisParamConfig(1, 0, 1, 100, -100, 10, 10, 12, 131072, 0, 1, 0, 0); //伺服驱动器参数，本示例为单轴变位机，因此只需要设置一个驱动器参数，若您选择包含多个轴的扩展轴类型，需要每一个轴设置驱动器参数

            //4.设置所选的轴使能、回零
            robot.ExtAxisServoOn(1, 0);
            robot.ExtAxisSetHoming(1, 0, 20, 3);

            //5.进行扩展轴坐标系标定及应用
            DescPose pos = new DescPose(/* 输入您的标定点坐标 */);
            robot.SetRefPointInExAxisEnd(pos);
            robot.PositionorSetRefPoint(1); /*您需要通过四个不同位置的点来标定扩展轴，因此需要调用此接口4次才能完成标定 */
            DescPose coord = new DescPose();
            robot.PositionorComputeECoordSys(ref coord); //计算扩展轴标定结果
            robot.ExtAxisActiveECoordSys(1, 1, coord, 1);  //将标定结果应用到扩展轴坐标系

            //6.在扩展轴上标定工件坐标系，您需要用到以下接口
            //int SetWObjCoordPoint(int point_num);
            //int ComputeWObjCoord(int method, ref DescPose wobj_pose);
            //int SetWObjCoord(int id, DescPose coord);
            //int SetWObjList(int id, DescPose coord);

            //7.记录您的同步关节运动起始点
            DescPose startdescPose = new DescPose(/*输入您的坐标*/);
            JointPos startjointPos = new JointPos(/*输入您的坐标*/);
            ExaxisPos startexaxisPos = new ExaxisPos(/* 输入您的扩展轴起始点坐标 */);

            //8.记录您的同步关节运动终点坐标
            DescPose enddescPose = new DescPose(/*输入您的坐标*/);
            JointPos endjointPos = new JointPos(/*输入您的坐标*/);
            ExaxisPos endexaxisPos = new ExaxisPos(/* 输入您的扩展轴终点坐标 */);

            //9.编写同步运动程序
            //运动到起始点，假设应用的工具坐标系、工件坐标系都是1
            robot.ExtAxisMove(startexaxisPos, 20);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            robot.MoveJ(startjointPos, startdescPose, 1, 1, 100, 100, 100, startexaxisPos, 0, 0, offdese);

            //开始同步运动
            robot.ExtAxisSyncMoveJ(endjointPos, enddescPose, 1, 1, 100, 100, 100, endexaxisPos, -1, 0, offdese);



        }

        private void testJEng()
        {
            //1.Calibrate and apply the robot tool coordinate system. You can use the four-point method or the six-point method to calibrate and apply the tool coordinate system. The interfaces involved in the calibration of the tool coordinate system are as follows：
            //    int SetToolPoint(int point_num);  //Set tool reference point - six point method
            //    int ComputeTool(ref DescPose tcp_pose);  //Computational tool coordinate system
            //    int SetTcp4RefPoint(int point_num);    //Set tool reference point - four point method
            //    int ComputeTcp4(ref DescPose tcp_pose);   //Calculating tool coordinate system - four-point method
            //    int SetToolCoord(int id, DescPose coord, int type, int install);  //Set the application tool coordinate system
            //    int SetToolList(int id, DescPose coord, int type, int install);   //Sets the list of application tool coordinate systems

            //2.Set UDP communication parameters and load UDP communication
            robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 100, 1, 100, 10);
            robot.ExtDevLoadUDPDriver();

            //3.Set the extension shaft parameters, including the extension shaft type, extension shaft drive parameters, and extension shaft DH parameters
            robot.SetAxisDHParaConfig(4, 200, 200, 0, 0, 0, 0, 0, 0); //Single axis positioner and DH parameters
            robot.SetRobotPosToAxis(1);  //Expansion shaft mounting position
            robot.ExtAxisParamConfig(1, 0, 1, 100, -100, 10, 10, 12, 131072, 0, 1, 0, 0); //Servo drive parameters, this example is a single-axis positioner, so only one drive parameter needs to be set. If you choose an extension shaft type with multiple axes, you need to set the drive parameters for each axis

            //4.Set the selected axis to enable and homing
            robot.ExtAxisServoOn(1, 0);
            robot.ExtAxisSetHoming(1, 0, 20, 3);

            //5.Carry out calibration and application of extended axis coordinate system
            DescPose pos = new DescPose(/* Enter your marker coordinates */);
            robot.SetRefPointInExAxisEnd(pos);
            robot.PositionorSetRefPoint(1); /*You need to calibrate the extension axis through four points in different locations, so you need to call this interface four times to complete the calibration */
            DescPose coord = new DescPose();
            robot.PositionorComputeECoordSys(ref coord); //Calculate the calibration results of the extension axis
            robot.ExtAxisActiveECoordSys(1, 1, coord, 1);  //The calibration results are applied to the extended axis coordinate system

            //6.To calibrate the workpiece coordinate system on the extension axis, you need the following interfaces
            //int SetWObjCoordPoint(int point_num);
            //int ComputeWObjCoord(int method, ref DescPose wobj_pose);
            //int SetWObjCoord(int id, DescPose coord);
            //int SetWObjList(int id, DescPose coord);

            //7.Record the start point of your synchronous joint movement
            DescPose startdescPose = new DescPose(/*Enter your coordinates*/);
            JointPos startjointPos = new JointPos(/*Enter your coordinates*/);
            ExaxisPos startexaxisPos = new ExaxisPos(/* Enter your extension axis start point coordinates */);

            //8.Record the coordinates of the end point of your synchronous joint movement
            DescPose enddescPose = new DescPose(/*Enter your coordinates*/);
            JointPos endjointPos = new JointPos(/*Enter your coordinates*/);
            ExaxisPos endexaxisPos = new ExaxisPos(/* Enter your extension axis endpoint coordinates */);

            //9.Write synchronous motion program
            //Move to the starting point, assuming that the tool coordinate system and the workpiece coordinate system are both 1
            robot.ExtAxisMove(startexaxisPos, 20);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            robot.MoveJ(startjointPos, startdescPose, 1, 1, 100, 100, 100, startexaxisPos, 0, 0, offdese);

            //Start synchronized motion
            robot.ExtAxisSyncMoveJ(endjointPos, enddescPose, 1, 1, 100, 100, 100, endexaxisPos, -1, 0, offdese);



        }

        private void testL()
        {
            //1.标定并应用机器人工具坐标系，您可以使用四点法或六点法进行工具坐标系的标定和应用，涉及工具坐标系标定的接口如下：
            //    int SetToolPoint(int point_num);  //设置工具参考点-六点法
            //    int ComputeTool(ref DescPose tcp_pose);  //计算工具坐标系
            //    int SetTcp4RefPoint(int point_num);    //设置工具参考点-四点法
            //    int ComputeTcp4(ref DescPose tcp_pose);   //计算工具坐标系-四点法
            //    int SetToolCoord(int id, DescPose coord, int type, int install);  //设置应用工具坐标系
            //    int SetToolList(int id, DescPose coord, int type, int install);   //设置应用工具坐标系列表

            //2.设置UDP通信参数，并加载UDP通信
            robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 100, 1, 100, 10);
            robot.ExtDevLoadUDPDriver();

            //3.设置扩展轴参数，包括扩展轴类型、扩展轴驱动器参数、扩展轴DH参数
            robot.SetAxisDHParaConfig(4, 200, 200, 0, 0, 0, 0, 0, 0); //单轴变位机及DH参数
            robot.SetRobotPosToAxis(1);  //扩展轴安装位置
            robot.ExtAxisParamConfig(1, 0, 1, 100, -100, 10, 10, 12, 131072, 0, 1, 0, 0); //伺服驱动器参数，本示例为单轴变位机，因此只需要设置一个驱动器参数，若您选择包含多个轴的扩展轴类型，需要每一个轴设置驱动器参数

            //4.设置所选的轴使能、回零
            robot.ExtAxisServoOn(1, 0);
            robot.ExtAxisSetHoming(1, 0, 20, 3);

            //5.进行扩展轴坐标系标定及应用
            DescPose pos = new DescPose(/* 输入您的标定点坐标 */);
            robot.SetRefPointInExAxisEnd(pos);
            robot.PositionorSetRefPoint(1); /*您需要通过四个不同位置的点来标定扩展轴，因此需要调用此接口4次才能完成标定 */
            DescPose coord = new DescPose();
            robot.PositionorComputeECoordSys(ref coord); //计算扩展轴标定结果
            robot.ExtAxisActiveECoordSys(1, 1, coord, 1);  //将标定结果应用到扩展轴坐标系

            //6.在扩展轴上标定工件坐标系，您需要用到以下接口
            //int SetWObjCoordPoint(int point_num);
            //int ComputeWObjCoord(int method, ref DescPose wobj_pose);
            //int SetWObjCoord(int id, DescPose coord);
            //int SetWObjList(int id, DescPose coord);

            //7.记录您的同步直线运动起始点
            DescPose startdescPose = new DescPose(/*输入您的坐标*/);
            JointPos startjointPos = new JointPos(/*输入您的坐标*/);
            ExaxisPos startexaxisPos = new ExaxisPos(/* 输入您的扩展轴起始点坐标 */);

            //8.记录您的同步直线运动终点坐标
            DescPose enddescPose = new DescPose(/*输入您的坐标*/);
            JointPos endjointPos = new JointPos(/*输入您的坐标*/);
            ExaxisPos endexaxisPos = new ExaxisPos(/* 输入您的扩展轴终点坐标 */);

            //9.编写同步运动程序
            //运动到起始点，假设应用的工具坐标系、工件坐标系都是1
            robot.ExtAxisMove(startexaxisPos, 20);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            robot.MoveJ(startjointPos, startdescPose, 1, 1, 100, 100, 100, startexaxisPos, 0, 0, offdese);

            //开始同步运动
            robot.ExtAxisSyncMoveL(endjointPos, enddescPose, 1, 1, 100, 100, 100, 0, endexaxisPos, 0, offdese);



        }

        private void testLEng()
        {
            //1.Calibrate and apply the robot tool coordinate system. You can use the four-point method or the six-point method to calibrate and apply the tool coordinate system. The interfaces involved in the calibration of the tool coordinate system are as follows：
            //    int SetToolPoint(int point_num);  //Set tool reference point - six point method
            //    int ComputeTool(ref DescPose tcp_pose);  //Computational tool coordinate system
            //    int SetTcp4RefPoint(int point_num);    //Set tool reference point - four point method
            //    int ComputeTcp4(ref DescPose tcp_pose);   //Calculating tool coordinate system - four-point method
            //    int SetToolCoord(int id, DescPose coord, int type, int install);  //Set the application tool coordinate system
            //    int SetToolList(int id, DescPose coord, int type, int install);   //Sets the list of application tool coordinate systems

            //2.Set UDP communication parameters and load UDP communication
            robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 100, 1, 100, 10);
            robot.ExtDevLoadUDPDriver();

            //3.Set the extension shaft parameters, including the extension shaft type, extension shaft drive parameters, and extension shaft DH parameters
            robot.SetAxisDHParaConfig(4, 200, 200, 0, 0, 0, 0, 0, 0); //Single axis positioner and DH parameters
            robot.SetRobotPosToAxis(1);  //Expansion shaft mounting position
            robot.ExtAxisParamConfig(1, 0, 1, 100, -100, 10, 10, 12, 131072, 0, 1, 0, 0); //Servo drive parameters, this example is a single-axis positioner, so only one drive parameter needs to be set. If you choose an extension shaft type with multiple axes, you need to set the drive parameters for each axis

            //4.Set the selected axis to enable and homing
            robot.ExtAxisServoOn(1, 0);
            robot.ExtAxisSetHoming(1, 0, 20, 3);

            //5.Carry out calibration and application of extended axis coordinate system
            DescPose pos = new DescPose(/* Enter your marker coordinates */);
            robot.SetRefPointInExAxisEnd(pos);
            robot.PositionorSetRefPoint(1); /*You need to calibrate the extension axis through four points in different locations, so you need to call this interface four times to complete the calibration */
            DescPose coord = new DescPose();
            robot.PositionorComputeECoordSys(ref coord); //Calculate the calibration results of the extension axis
            robot.ExtAxisActiveECoordSys(1, 1, coord, 1);  //The calibration results are applied to the extended axis coordinate system

            //6.To calibrate the workpiece coordinate system on the extension axis, you need the following interfaces
            //int SetWObjCoordPoint(int point_num);
            //int ComputeWObjCoord(int method, ref DescPose wobj_pose);
            //int SetWObjCoord(int id, DescPose coord);
            //int SetWObjList(int id, DescPose coord);

            //7.Record the start point of your synchronous line movement
            DescPose startdescPose = new DescPose(/*Enter your coordinates*/);
            JointPos startjointPos = new JointPos(/*Enter your coordinates*/);
            ExaxisPos startexaxisPos = new ExaxisPos(/* Enter your extension axis start point coordinates */);

            //8.Record the coordinates of the end point of your synchronous line movement
            DescPose enddescPose = new DescPose(/*Enter your coordinates*/);
            JointPos endjointPos = new JointPos(/*Enter your coordinates*/);
            ExaxisPos endexaxisPos = new ExaxisPos(/* Enter your extension axis endpoint coordinates */);

            //9.Write synchronous motion program
            //Move to the starting point, assuming that the tool coordinate system and the workpiece coordinate system are both 1
            robot.ExtAxisMove(startexaxisPos, 20);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            robot.MoveJ(startjointPos, startdescPose, 1, 1, 100, 100, 100, startexaxisPos, 0, 0, offdese);

            //Start synchronized motion
            robot.ExtAxisSyncMoveL(endjointPos, enddescPose, 1, 1, 100, 100, 100, 0, endexaxisPos, 0, offdese);



        }

        private void testC()
        {
            //1.标定并应用机器人工具坐标系，您可以使用四点法或六点法进行工具坐标系的标定和应用，涉及工具坐标系标定的接口如下：
            //    int SetToolPoint(int point_num);  //设置工具参考点-六点法
            //    int ComputeTool(ref DescPose tcp_pose);  //计算工具坐标系
            //    int SetTcp4RefPoint(int point_num);    //设置工具参考点-四点法
            //    int ComputeTcp4(ref DescPose tcp_pose);   //计算工具坐标系-四点法
            //    int SetToolCoord(int id, DescPose coord, int type, int install);  //设置应用工具坐标系
            //    int SetToolList(int id, DescPose coord, int type, int install);   //设置应用工具坐标系列表

            //2.设置UDP通信参数，并加载UDP通信
            robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 100, 1, 100, 10);
            robot.ExtDevLoadUDPDriver();

            //3.设置扩展轴参数，包括扩展轴类型、扩展轴驱动器参数、扩展轴DH参数
            robot.SetAxisDHParaConfig(4, 200, 200, 0, 0, 0, 0, 0, 0); //单轴变位机及DH参数
            robot.SetRobotPosToAxis(1);  //扩展轴安装位置
            robot.ExtAxisParamConfig(1, 0, 1, 100, -100, 10, 10, 12, 131072, 0, 1, 0, 0); //伺服驱动器参数，本示例为单轴变位机，因此只需要设置一个驱动器参数，若您选择包含多个轴的扩展轴类型，需要每一个轴设置驱动器参数

            //4.设置所选的轴使能、回零
            robot.ExtAxisServoOn(1, 0);
            robot.ExtAxisSetHoming(1, 0, 20, 3);

            //5.进行扩展轴坐标系标定及应用
            DescPose pos = new DescPose(/* 输入您的标定点坐标 */);
            robot.SetRefPointInExAxisEnd(pos);
            robot.PositionorSetRefPoint(1); /*您需要通过四个不同位置的点来标定扩展轴，因此需要调用此接口4次才能完成标定 */
            DescPose coord = new DescPose();
            robot.PositionorComputeECoordSys(ref coord); //计算扩展轴标定结果
            robot.ExtAxisActiveECoordSys(1, 1, coord, 1);  //将标定结果应用到扩展轴坐标系

            //6.在扩展轴上标定工件坐标系，您需要用到以下接口
            //int SetWObjCoordPoint(int point_num);
            //int ComputeWObjCoord(int method, ref DescPose wobj_pose);
            //int SetWObjCoord(int id, DescPose coord);
            //int SetWObjList(int id, DescPose coord);

            //7.记录您的同步圆弧运动起始点
            DescPose startdescPose = new DescPose(/*输入您的坐标*/);
            JointPos startjointPos = new JointPos(/*输入您的坐标*/);
            ExaxisPos startexaxisPos = new ExaxisPos(/* 输入您的扩展轴起始点坐标 */);

            //8.记录您的同步圆弧运动终点坐标
            DescPose enddescPose = new DescPose(/*输入您的坐标*/);
            JointPos endjointPos = new JointPos(/*输入您的坐标*/);
            ExaxisPos endexaxisPos = new ExaxisPos(/* 输入您的扩展轴终点坐标 */);

            //8.记录您的同步圆弧运动中间点坐标
            DescPose middescPose = new DescPose(/*输入您的坐标*/);
            JointPos midjointPos = new JointPos(/*输入您的坐标*/);
            ExaxisPos midexaxisPos = new ExaxisPos(/* 输入机器人圆弧中间点时的扩展轴坐标 */);

            //9.编写同步运动程序
            //运动到起始点，假设应用的工具坐标系、工件坐标系都是1
            robot.ExtAxisMove(startexaxisPos, 20);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            robot.MoveJ(startjointPos, startdescPose, 1, 1, 100, 100, 100, startexaxisPos, 0, 0, offdese);

            //开始同步运动
            robot.ExtAxisSyncMoveC(midjointPos, middescPose, 1, 1, 100, 100, midexaxisPos, 0, offdese, endjointPos, enddescPose, 1, 1, 100, 100, endexaxisPos, 0, offdese, 100, 0);


        }

        private void testCEng()
        {
            //1.Calibrate and apply the robot tool coordinate system. You can use the four-point method or the six-point method to calibrate and apply the tool coordinate system. The interfaces involved in the calibration of the tool coordinate system are as follows：
            //    int SetToolPoint(int point_num);  //Set tool reference point - six point method
            //    int ComputeTool(ref DescPose tcp_pose);  //Computational tool coordinate system
            //    int SetTcp4RefPoint(int point_num);    //Set tool reference point - four point method
            //    int ComputeTcp4(ref DescPose tcp_pose);   //Calculating tool coordinate system - four-point method
            //    int SetToolCoord(int id, DescPose coord, int type, int install);  //Set the application tool coordinate system
            //    int SetToolList(int id, DescPose coord, int type, int install);   //Sets the list of application tool coordinate systems

            //2.Set UDP communication parameters and load UDP communication
            robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 100, 1, 100, 10);
            robot.ExtDevLoadUDPDriver();

            //3.Set the extension shaft parameters, including the extension shaft type, extension shaft drive parameters, and extension shaft DH parameters
            robot.SetAxisDHParaConfig(4, 200, 200, 0, 0, 0, 0, 0, 0); //Single axis positioner and DH parameters
            robot.SetRobotPosToAxis(1);  //Expansion shaft mounting position
            robot.ExtAxisParamConfig(1, 0, 1, 100, -100, 10, 10, 12, 131072, 0, 1, 0, 0); //Servo drive parameters, this example is a single-axis positioner, so only one drive parameter needs to be set. If you choose an extension shaft type with multiple axes, you need to set the drive parameters for each axis

            //4.Set the selected axis to enable and homing
            robot.ExtAxisServoOn(1, 0);
            robot.ExtAxisSetHoming(1, 0, 20, 3);

            //5.Carry out calibration and application of extended axis coordinate system
            DescPose pos = new DescPose(/* Enter your marker coordinates */);
            robot.SetRefPointInExAxisEnd(pos);
            robot.PositionorSetRefPoint(1); /*You need to calibrate the extension axis through four points in different locations, so you need to call this interface four times to complete the calibration */
            DescPose coord = new DescPose();
            robot.PositionorComputeECoordSys(ref coord); //Calculate the calibration results of the extension axis
            robot.ExtAxisActiveECoordSys(1, 1, coord, 1);  //The calibration results are applied to the extended axis coordinate system

            //6.To calibrate the workpiece coordinate system on the extension axis, you need the following interfaces
            //int SetWObjCoordPoint(int point_num);
            //int ComputeWObjCoord(int method, ref DescPose wobj_pose);
            //int SetWObjCoord(int id, DescPose coord);
            //int SetWObjList(int id, DescPose coord);

            //7.Record the start point of your synchronous line movement
            DescPose startdescPose = new DescPose(/*Enter your coordinates*/);
            JointPos startjointPos = new JointPos(/*Enter your coordinates*/);
            ExaxisPos startexaxisPos = new ExaxisPos(/* Enter your extension axis start point coordinates */);

            //8.Record the coordinates of the end point of your synchronous line movement
            DescPose enddescPose = new DescPose(/*Enter your coordinates*/);
            JointPos endjointPos = new JointPos(/*Enter your coordinates*/);
            ExaxisPos endexaxisPos = new ExaxisPos(/* Enter your extension axis endpoint coordinates */);

            //9.Record the coordinates of the intermediate point of your synchronous circular motion
            DescPose middescPose = new DescPose(/*Enter your coordinates*/);
            JointPos midjointPos = new JointPos(/*Enter your coordinates*/);
            ExaxisPos midexaxisPos = new ExaxisPos(/* Expand axis coordinates when entering the robot arc midpoint */);

            //10.Write synchronous motion program
            //Move to the starting point, assuming that the tool coordinate system and the workpiece coordinate system are both 1
            robot.ExtAxisMove(startexaxisPos, 20);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            robot.MoveJ(startjointPos, startdescPose, 1, 1, 100, 100, 100, startexaxisPos, 0, 0, offdese);

            //Start synchronized motion
            robot.ExtAxisSyncMoveC(midjointPos, middescPose, 1, 1, 100, 100, midexaxisPos, 0, offdese, endjointPos, enddescPose, 1, 1, 100, 100, endexaxisPos, 0, offdese, 100, 0);



        }

        private void btnOverVelA_Click(object sender, EventArgs e)
        {
            DescPose startdescPose = new DescPose(double.Parse(txtStartX.Text), double.Parse(txtStartY.Text), double.Parse(txtStartZ.Text), double.Parse(txtStartRX.Text), double.Parse(txtStartRY.Text), double.Parse(txtStartRZ.Text));
            JointPos startjointPos = new JointPos(double.Parse(txtStartJ1.Text), double.Parse(txtStartJ2.Text), double.Parse(txtStartJ3.Text), double.Parse(txtStartJ4.Text), double.Parse(txtStartJ5.Text), double.Parse(txtStartJ6.Text));

            ExaxisPos exaxisPos = new ExaxisPos(double.Parse(textBox4.Text), double.Parse(textBox3.Text), double.Parse(textBox2.Text), double.Parse(textBox1.Text));
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            //robot.MoveL(startjointPos, startdescPose, 1, 1, 100, 100, 100, 0, exaxisPos, 0, 0, offdese);
            robot.MoveL(startjointPos, startdescPose, 0, 0, 100, 100, 100, 0, exaxisPos, 0, 0, offdese, 0, 10);
        }

        private void btnOverVelB_Click(object sender, EventArgs e)
        {

        }

        private void btnTractorEnable_Click(object sender, EventArgs e)
        {




        }

        private void button5_Click(object sender, EventArgs e)
        {
            //************************************************设置UDP扩展DIO
            //for (int i = 0; i < 128; i++)
            //{
            //    robot.SetAuxDO(i, true, false, true);
            //    Thread.Sleep(100);
            //    Console.WriteLine($"SetAuxDO Value{true}");
            //}
            //for (int i = 0; i < 128; i++)
            //{
            //    robot.SetAuxDO(i, false, false, true);
            //    Thread.Sleep(100);
            //    Console.WriteLine($"SetAuxDO Value{false}");
            //}

            //for (int i = 0; i < 409; i++)
            //{
            //    robot.SetAuxAO(0, i * 10, true);
            //    Console.WriteLine($"SetAuxAO id{0}, Value{i * 10}");
            //    robot.SetAuxAO(1, 4095 - i * 10, true);
            //    Console.WriteLine($"SetAuxAO id{1}, Value{4095 - i * 10}");
            //    robot.SetAuxAO(2, i * 10, true);
            //    Console.WriteLine($"SetAuxAO id{2}, Value{i * 10}");
            //    robot.SetAuxAO(3, 4095 - i * 10, true);
            //    Console.WriteLine($"SetAuxAO id{3}, Value{4095 - i * 10}");
            //    Thread.Sleep(2000);
            //    robot.SetAuxAO(0, 0, true);
            //    Console.WriteLine($"SetAuxAO id{0}, Value{0}");
            //    robot.SetAuxAO(1, 0, true);
            //    Console.WriteLine($"SetAuxAO id{1}, Value{0}");
            //    robot.SetAuxAO(2, 0, true);
            //    Console.WriteLine($"SetAuxAO id{2}, Value{0}");
            //    robot.SetAuxAO(3, 0, true);
            //    Console.WriteLine($"SetAuxAO id{3}, Value{0}");
            //    Thread.Sleep(2000);
            //}

            //***********************************************小车运动和中途停止
            robot.ExtDevSetUDPComParam("192.168.58.2", 2021, 2, 50, 5, 50, 1, 50, 10);
            int tru = robot.ExtDevLoadUDPDriver();
            Thread.Sleep(2000);
            Console.WriteLine("tru" + tru);
            //robot.ExtAxisParamConfig(1, 0, 0, 50000, -50000, 1000, 1000, 6.280, 16384, 200, 0, 0, 0);
            //robot.ExtAxisParamConfig(2, 0, 0, 50000, -50000, 1000, 1000, 6.280, 16384, 200, 0, 0, 0);
            //robot.SetAxisDHParaConfig(5, 0, 0, 0, 0, 0, 0, 0, 0);
            int tru1 = robot.TractorEnable(true);
            //Console.WriteLine("tru1"+tru1);
            ////Thread.Sleep(3000);
            ////robot.TractorEnable(false);
            //Thread.Sleep(2000);
            robot.TractorHoming();
            //Thread.Sleep(2000);
            robot.TractorMoveL(100, 20);
            Thread.Sleep(2000);
            robot.TractorMoveL(-100, 20);
            Thread.Sleep(2000);
            robot.TractorMoveC(50, 60, 20);
            Thread.Sleep(2000);
            robot.TractorMoveC(50, -60, 20);
            Thread.Sleep(1000);
            robot.TractorStop();//中途停止
            //Console.WriteLine("TractorStop");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //UDP焊丝寻位
            robot.ExtDevSetUDPComParam("192.168.58.2", 2021, 2, 50, 5, 50, 1, 50, 10);
            robot.ExtDevLoadUDPDriver();
            robot.SetWireSearchExtDIONum(0, 0);

            int rtn0, rtn1, rtn2 = 0;
            ExaxisPos exaxisPos = new ExaxisPos(0.0, 0.0, 0.0, 0.0);
            DescPose offdese = new DescPose(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);

            DescPose descStart = new DescPose(-158.767, -510.596, 271.709, -179.427, -0.745, -137.349);
            JointPos jointStart = new JointPos(61.667, -79.848, 108.639, -119.682, -89.700, -70.985);

            DescPose descEnd = new DescPose(0.332, -516.427, 270.688, 178.165, 0.017, -119.989);
            JointPos jointEnd = new JointPos(79.021, -81.839, 110.752, -118.298, -91.729, -70.981);

            robot.MoveL(jointStart, descStart, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            robot.MoveL(jointEnd, descEnd, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);

            DescPose descREF0A = new DescPose(-66.106, -560.746, 270.381, 176.479, -0.126, -126.745);
            JointPos jointREF0A = new JointPos(73.531, -75.588, 102.941, -116.250, -93.347, -69.689);

            DescPose descREF0B = new DescPose(-66.109, -528.440, 270.407, 176.479, -0.129, -126.744);
            JointPos jointREF0B = new JointPos(72.534, -79.625, 108.046, -117.379, -93.366, -70.687);

            DescPose descREF1A = new DescPose(72.975, -473.242, 270.399, 176.479, -0.129, -126.744);
            JointPos jointREF1A = new JointPos(87.169, -86.509, 115.710, -117.341, -92.993, -56.034);

            DescPose descREF1B = new DescPose(31.355, -473.238, 270.405, 176.480, -0.130, -126.745);
            JointPos jointREF1B = new JointPos(82.117, -87.146, 116.470, -117.737, -93.145, -61.090);

            rtn0 = robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF0A, descREF0A, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);  //起点
            robot.MoveL(jointREF0B, descREF0B, 1, 0, 10, 100, 100, -1, exaxisPos, 1, 0, offdese);  //方向点
            rtn1 = robot.WireSearchWait("REF0");
            rtn2 = robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            rtn0 = robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF1A, descREF1A, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);  //起点
            robot.MoveL(jointREF1B, descREF1B, 1, 0, 10, 100, 100, -1, exaxisPos, 1, 0, offdese);  //方向点
            rtn1 = robot.WireSearchWait("REF1");
            rtn2 = robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            rtn0 = robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF0A, descREF0A, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);  //起点
            robot.MoveL(jointREF0B, descREF0B, 1, 0, 10, 100, 100, -1, exaxisPos, 1, 0, offdese);  //方向点
            rtn1 = robot.WireSearchWait("RES0");
            rtn2 = robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            rtn0 = robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF1A, descREF1A, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);  //起点
            robot.MoveL(jointREF1B, descREF1B, 1, 0, 10, 100, 100, -1, exaxisPos, 1, 0, offdese);  //方向点
            rtn1 = robot.WireSearchWait("RES1");
            rtn2 = robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);
            List<string> varNameRef1 = new List<string> { "REF0", "REF1", "#", "#", "#", "#" };
            List<string> varNameRes1 = new List<string> { "RES0", "RES1", "#", "#", "#", "#" };
            string[] varNameRef = varNameRef1.ToArray();
            string[] varNameRes = varNameRes1.ToArray();
            int offectFlag = 0;
            DescPose offectPos = new DescPose(0, 0, 0, 0, 0, 0);
            rtn0 = robot.GetWireSearchOffset(0, 0, varNameRef, varNameRes, ref offectFlag, ref offectPos);
            robot.PointsOffsetEnable(0, offectPos);
            robot.MoveL(jointStart, descStart, 1, 0, 100, 100, 100, -1, exaxisPos, 0, 0, offdese);
            robot.MoveL(jointEnd, descEnd, 1, 0, 100, 100, 100, -1, exaxisPos, 1, 0, offdese);
            robot.PointsOffsetDisable();
        }

        private void button7_Click(object sender, EventArgs e)
        {

            //***********************************************UPD扩展轴参数配置与获取 begin**************************************

            robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 100, 3, 100, 1, 100, 10);
            string ip = "";
            int port = 0;
            int period = 0;
            int checktime = 0;
            int checknum = 0;
            int disconntime = 0;
            int reconnenable = 0;
            int reconntime = 0;
            int reconnnum = 0;
            robot.ExtDevGetUDPComParam(ref ip, ref port, ref period, ref checktime, ref checknum, ref disconntime, ref reconntime, ref reconnenable, ref reconnnum);
            lblgetUDP.Text = $"{ip}  {port}  {period} {checktime}  {checknum}  {disconntime}  {reconnenable}  {reconntime}  {reconnnum}";

            //***********************************************UPD扩展轴参数配置与获取 end **************************************

            //***********************************************UPD加载与卸载 begin **************************************
            robot.ExtDevUnloadUDPDriver();//卸载UDP通信
            Thread.Sleep(1000);
            robot.ExtDevLoadUDPDriver();//加载UDP通信
            Thread.Sleep(1000);
            //***********************************************UPD加载与卸载 end **************************************

            //***********************************************UPD扩展轴按照位置、DH参数、轴参数设置 begin **************************************
            //robot.ExtAxisServoOn(1, 1);//扩展轴1使能
            //robot.ExtAxisServoOn(2, 1);//扩展轴2使能
            //Thread.Sleep(1000);
            //robot.ExtAxisSetHoming(1, 0, 10, 3);//1,2扩展轴都回零
            //robot.ExtAxisSetHoming(2, 0, 10, 3);
            //Thread.Sleep(1000);

            //int rtn = 0;
            //rtn = robot.SetAxisDHParaConfig(1, 128.5, 206.4, 0, 0, 0, 0, 0, 0);
            //Console.WriteLine("SetAxisDHParaConfig rtn is " + rtn);
            //rtn = robot.SetRobotPosToAxis(1);
            //Console.WriteLine("SetRobotPosToAxis rtn is " + rtn);
            //rtn = robot.ExtAxisParamConfig(1, 1, 0, 1000, -1000, 1000, 1000, 1.905, 262144, 200, 0, 0, 0);
            //Console.WriteLine("ExtAxisParamConfig rtn is " + rtn);
            //rtn = robot.ExtAxisParamConfig(2, 2, 0, 1000, -1000, 1000, 1000, 4.444, 262144, 200, 0, 0, 0);
            //Console.WriteLine("ExtAxisParamConfig rtn is " + rtn);
            ////标定
            //rtn = 0;
            //rtn = robot.SetAxisDHParaConfig(1, 128.5, 206.4, 0, 0, 0, 0, 0, 0);
            //Console.WriteLine("SetAxisDHParaConfig rtn is " + rtn);
            //rtn = robot.SetRobotPosToAxis(1);
            //Console.WriteLine("SetRobotPosToAxis rtn is " + rtn);
            //rtn = robot.ExtAxisParamConfig(1, 1, 0, 1000, -1000, 100, 100, 1.905, 262144, 200, 0, 0, 0);
            //Console.WriteLine("ExtAxisParamConfig rtn is " + rtn);
            //rtn = robot.ExtAxisParamConfig(2, 2, 0, 1000, -1000, 100, 100, 4.444, 262144, 200, 0, 0, 0);
            //Console.WriteLine("ExtAxisParamConfig rtn is " + rtn);

            //int tool = 1;
            //int user = 0;
            //double vel = 20;
            //double acc = 100;
            //double ovl = 100;
            //byte offset_flag = 0;

            //ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);


            //DescPose descPose0 = new DescPose(311.189, -309.688, 401.836, -174.375, -1.409, -82.354);
            //JointPos jointPos0 = new JointPos(118.217, -99.669, 79.928, -73.559, -85.229, -69.359);
            //robot.MoveJ(jointPos0, descPose0, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, offset_flag, offdese);

            //robot.ExtAxisMove(exaxisPos, 20);//UDP扩展轴运动,速度100

            //DescPose descPose1 = new DescPose(359.526, -516.038, 194.469, -175.689, 2.781, -87.609);
            //JointPos jointPos1 = new JointPos(113.015, -72.49, 80.079, -96.505, -84.986, -69.309);

            //ExaxisPos pos1 = new ExaxisPos(0.000, 0.000, 0.000, 0.000);
            //robot.ExtAxisMove(pos1, 20);//UDP扩展轴运动,速度100
            //robot.MoveJ(jointPos1, descPose1, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, offset_flag, offdese);
            //robot.PositionorSetRefPoint(1);

            //robot.MoveJ(jointPos0, descPose0, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, offset_flag, offdese);

            //ExaxisPos pos2 = new ExaxisPos(-10.000, -10.000, 0.000, 0.000);
            //robot.ExtAxisMove(pos2, 20);
            //DescPose descPose2 = new DescPose(333.347, -541.958, 164.894, -176.47, 4.284, -90.725);
            //JointPos jointPos2 = new JointPos(109.989, -69.637, 80.146, -97.755, -85.188, -69.269);
            //robot.MoveJ(jointPos2, descPose2, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, offset_flag, offdese);
            //robot.PositionorSetRefPoint(2);

            //robot.MoveJ(jointPos0, descPose0, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, offset_flag, offdese);

            //ExaxisPos pos3 = new ExaxisPos(-20.000, -20.000, 0.000, 0.00);
            //robot.ExtAxisMove(pos3, 20);
            //DescPose descPose3 = new DescPose(306.488, -559.238, 135.948, -174.925, 0.235, -93.517);
            //JointPos jointPos3 = new JointPos(107.137, -71.377, 87.975, -108.167, -85.169, -69.269);
            //robot.MoveJ(jointPos3, descPose3, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, offset_flag, offdese);
            //robot.PositionorSetRefPoint(3);

            //robot.MoveJ(jointPos0, descPose0, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, offset_flag, offdese);

            //ExaxisPos pos4 = new ExaxisPos(-30.000, -30.000, 0.000, 0.000);
            //robot.ExtAxisMove(pos4, 20);
            //DescPose descPose4 = new DescPose(285.528, -569.999, 108.568, -174.367, -1.239, -95.643);
            //JointPos jointPos4 = new JointPos(105.016, -71.137, 92.326, -114.339, -85.169, -69.269);
            //robot.MoveJ(jointPos4, descPose4, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, offset_flag, offdese);
            //robot.PositionorSetRefPoint(4);

            //DescPose axisCoord = new DescPose(0, 0, 0, 0, 0, 0);
            //robot.PositionorComputeECoordSys(ref axisCoord);
            //robot.ExtAxisActiveECoordSys(3, 1, axisCoord, 1);
            //Console.WriteLine("axis coord is " + axisCoord);

            //robot.MoveJ(jointPos0, descPose0, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, offset_flag, offdese);
            //robot.ExtAxisMove(pos1, 20);
            //int tool = 1;
            //int user = 0;
            //double vel = 20;
            //double acc = 100;
            //double ovl = 100;
            //byte offset_flag = 0;
            //ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);


            //DescPose descPose0 = new DescPose(311.189, -309.688, 401.836, -174.375, -1.409, -82.354);
            //JointPos jointPos0 = new JointPos(118.217, -99.669, 79.928, -73.559, -85.229, -69.359);
            //robot.MoveJ(jointPos0, descPose0, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, offset_flag, offdese);

            //ExaxisPos pos = new ExaxisPos(0, 0, 0, 0);
            //robot.ExtAxisMove(pos, 40);
            ////DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);
            //ExaxisPos exaxisPos1 = new ExaxisPos(20, 100, 0, 0);



            //robot.ExtAxisMove(exaxisPos1, 40);

            //ExaxisPos exaxisPos2 = new ExaxisPos(-20, -100, 0, 0);
            //robot.ExtAxisMove(exaxisPos2, 40);
            //ExaxisPos exaxisPos3 = new ExaxisPos(0, 0, 0, 0);
            //robot.ExtAxisMove(exaxisPos3, 40);
            //robot.Mode(1);

            robot.Mode(0);
            int tool = 1;
            int user = 0;
            double vel = 20.0;
            double acc = 100.0;
            double ovl = 100.0;
            float blendT = -1;
            float blendR = -1;
            int flag = 0;
            int type = 1;

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);

            DescPose d0 = new DescPose(311.189, -309.688, 401.836, -174.375, -1.409, -82.354);
            JointPos j0 = new JointPos(118.217, -99.669, 79.928, -73.559, -85.229, -69.359);

            JointPos joint_pos0 = new JointPos(111.549, -99.821, 108.707, -99.308, -85.305, -41.515);
            DescPose desc_pos0 = new DescPose(273.499, -345.746, 201.573, -176.566, 3.235, -116.819);
            ExaxisPos e_pos0 = new ExaxisPos(0, 0, 0, 0);

            JointPos joint_pos1 = new JointPos(112.395, -65.118, 67.815, -61.449, -88.669, -41.517);
            DescPose desc_pos1 = new DescPose(291.393, -420.519, 201.089, 156.297, 21.019, -120.919);
            ExaxisPos e_pos1 = new ExaxisPos(-30, -30, 0, 0);


            JointPos j2 = new JointPos(111.549, -98.369, 108.036, -103.789, -95.203, -69.358);
            DescPose desc_pos2 = new DescPose(234.544, -392.777, 205.566, 176.584, -5.694, -89.109);
            ExaxisPos epos2 = new ExaxisPos(0.000, 0.000, 0.000, 0.000);

            JointPos j3 = new JointPos(113.908, -61.947, 63.829, -64.478, -85.406, -69.256);
            DescPose desc_pos3 = new DescPose(336.049, -444.969, 192.799, 173.776, 27.104, -89.455);
            ExaxisPos epos3 = new ExaxisPos(-30.000, -30.000, 0.000, 0.000);

            //圆弧的起点
            JointPos j4 = new JointPos(137.204, -98.475, 106.624, -97.769, -90.634, -69.24);
            DescPose desc_pos4 = new DescPose(381.269, -218.688, 205.735, 179.274, 0.128, -63.556);

            JointPos j5 = new JointPos(115.069, -92.709, 97.285, -82.809, -90.455, -77.146);
            DescPose desc_pos5 = new DescPose(264.049, -329.478, 220.747, 176.906, 11.359, -78.044);
            ExaxisPos epos5 = new ExaxisPos(-15, 0, 0, 0);


            JointPos j6 = new JointPos(102.409, -63.115, 70.559, -70.156, -86.529, -77.148);
            DescPose desc_pos6 = new DescPose(232.407, -494.228, 158.115, 176.803, 27.319, -92.056);
            ExaxisPos epos6 = new ExaxisPos(-30, 0, 0, 0);

            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);

            //同步关节运动
            robot.MoveJ(j0, d0, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, 0, offset_pos);
            robot.ExtAxisMove(exaxisPos, 40);
            robot.ExtAxisSyncMoveJ(joint_pos0, desc_pos0, 1, 0, 20, 100, 100, e_pos0, -1, 0, offset_pos);
            robot.ExtAxisSyncMoveJ(joint_pos1, desc_pos1, 1, 0, 20, 100, 100, e_pos1, -1, 0, offset_pos);


            //同步直线运动
            robot.MoveJ(j0, d0, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, 0, offset_pos);
            robot.ExtAxisMove(exaxisPos, 40);
            robot.ExtAxisSyncMoveL(j2, desc_pos2, tool, user, 40, 100, 100, -1, epos2, 0, offset_pos);
            robot.ExtAxisSyncMoveL(j3, desc_pos3, tool, user, 40, 100, 100, -1, epos3, 0, offset_pos);

            //同步圆弧运动
            robot.MoveJ(j0, d0, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, 0, offset_pos);
            robot.ExtAxisMove(exaxisPos, 20);
            robot.MoveJ(j4, desc_pos4, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, 0, offset_pos);

            robot.ExtAxisSyncMoveC(j5, desc_pos5, tool, user, 40, 100, epos5, 0, offset_pos, j6, desc_pos6, tool, user, 40, 100, epos6, 0, offset_pos, 100, 0);
            Thread.Sleep(3000);

            robot.MoveJ(j0, d0, 1, 0, (float)vel, (float)acc, (float)ovl, exaxisPos, -1, 0, offset_pos);
            robot.ExtAxisMove(exaxisPos, 40);
            robot.Mode(1);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //焊接控制模式切换
            robot.ExtDevSetUDPComParam("192.168.58.88", 2021, 2, 50, 5, 50, 1, 50, 10);
            robot.ExtDevLoadUDPDriver();

            robot.SetWeldMachineCtrlModeExtDoNum(17);
            for (int i = 0; i < 5; i++)
            {
                robot.SetWeldMachineCtrlMode(1);
                Thread.Sleep(500);
                robot.SetWeldMachineCtrlMode(0);
                Thread.Sleep(500);
            }

            robot.SetWeldMachineCtrlModeExtDoNum(18);
            for (int i = 0; i < 5; i++)
            {
                robot.SetWeldMachineCtrlMode(1);
                Thread.Sleep(500);
                robot.SetWeldMachineCtrlMode(0);
                Thread.Sleep(500);
            }

            robot.SetWeldMachineCtrlModeExtDoNum(19);
            for (int i = 0; i < 5; i++)
            {
                robot.SetWeldMachineCtrlMode(1);
                Thread.Sleep(500);
                robot.SetWeldMachineCtrlMode(0);
                Thread.Sleep(500);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {

            ////**********************************TestSingularAvoidSLin******************************////
            //
            //DescPose startdescPose = new DescPose(-352.437, -88.350, 226.471, 177.222, 4.924, 86.631);
            //JointPos startjointPos = new JointPos(-3.463, -84.308, 105.579, -108.475, -85.087, -0.334);

            //DescPose middescPose = new DescPose(-518.339, -23.706, 207.899, -178.420, 0.171, 71.697);
            //JointPos midjointPos = new JointPos(-8.587, -51.805, 64.914, -104.695, -90.099, 9.718);

            //DescPose enddescPose = new DescPose(-273.934, 323.003, 227.224, 176.398, 2.783, 66.064);
            //JointPos endjointPos = new JointPos(-63.460, -71.228, 88.068, -102.291, -90.149, -39.605);


            //ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            //robot.MoveL(startjointPos, startdescPose, 0, 0, 50, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            //robot.SingularAvoidStart(1, 100, 50, 10);
            //robot.MoveC(midjointPos, middescPose, 0, 0, 50, 100, exaxisPos, 0, offdese, endjointPos, enddescPose, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1);
            //robot.SingularAvoidEnd();


            ////**********************************TestSingularAvoidSArc******************************////
            //robot.SingularAvoidEnd();
            DescPose startdescPose = new DescPose(-379.749, -113.569, 262.288, -178.716, 2.620, 91.597);
            JointPos startjointPos = new JointPos(1.208, -80.436, 93.788, -104.620, -87.372, -0.331);

            DescPose middescPose = new DescPose(-151.941, -155.742, 262.756, 177.693, 2.571, 106.941);
            JointPos midjointPos = new JointPos(16.727, -121.385, 124.147, -90.442, -87.440, -0.318);

            DescPose enddescPose = new DescPose(-211.982, 218.852, 280.712, 176.819, -4.408, 26.857);
            JointPos endjointPos = new JointPos(-63.754, -98.766, 105.961, -94.052, -94.435, -0.366);

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.MoveL(startjointPos, startdescPose, 0, 0, 50, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            robot.SingularAvoidStart(0, 150, 50, 20);
            robot.MoveC(midjointPos, middescPose, 0, 0, 50, 100, exaxisPos, 0, offdese, endjointPos, enddescPose, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1);
            robot.SingularAvoidEnd();


            ////**********************************TestSingularAvoidEArc******************************////

            //DescPose startdescPose = new DescPose(-352.794, -164.582, 132.122, 176.136, 50.177, 85.343);
            //JointPos startjointPos = new JointPos(-2.048, -66.683, 121.240, -141.651, -39.776, -0.564);

            //DescPose middescPose = new DescPose(-352.353, -3.338, 299.600, -1.730, 58.744, -136.276);
            //JointPos midjointPos = new JointPos(-30.807, -92.341, 126.259, -102.944, 33.740, -25.798);

            //DescPose enddescPose = new DescPose(-352.353, -3.337, 353.164, -1.729, 58.744, -136.276);
            //JointPos endjointPos = new JointPos(-30.807, -98.084, 116.943, -87.886, 33.740, -25.798);

            //DescPose descPose = new DescPose(-402.473, -185.876, 103.985, -175.367, 59.682, 94.221);
            //JointPos jointPos = new JointPos(-0.095, -50.828, 109.737, -150.708, -30.225, -0.623);

            //ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            //robot.MoveL(startjointPos, startdescPose, 0, 0, 50, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            //robot.SingularAvoidStart(0, 150, 50, 20);
            //robot.MoveC(midjointPos, middescPose, 0, 0, 50, 100, exaxisPos, 0, offdese, endjointPos, enddescPose, 0, 0, 100, 100, exaxisPos, 0, offdese, 100, -1);
            //robot.MoveL(jointPos, descPose, 0, 0, 50, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            //robot.SingularAvoidEnd();

            ////**********************************TestSingularAvoidWLin******************************////

            //DescPose startdescPose = new DescPose(-379.749, -113.569, 262.293, -178.715, 2.620, 91.597);
            //JointPos startjointPos = new JointPos(1.208, -80.436, 93.788, -104.620, -87.372, -0.331);

            //DescPose enddescPose = new DescPose(252.972, -74.287, 316.795, -177.588, 2.451, 97.588);
            //JointPos endjointPos = new JointPos(7.165, -170.868, 63.507, 14.965, -87.534, -0.319);

            //ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            //robot.MoveL(startjointPos, startdescPose, 0, 0, 50, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            //robot.SingularAvoidStart(0, 150, 50, 20);
            //robot.MoveL(endjointPos, enddescPose, 0, 0, 50, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            //robot.SingularAvoidEnd();

            ////**********************************TestSingularAvoidWArc******************************////

            //DescPose startdescPose = new DescPose(-402.473, -185.876, 103.985, -175.367, 59.682, 94.221);
            //JointPos startjointPos = new JointPos(-0.095, -50.828, 109.737, -150.708, -30.225, -0.623);

            //DescPose enddescPose = new DescPose(-399.264, -184.434, 296.022, -4.402, 58.061, -94.161);
            //JointPos endjointPos = new JointPos(-0.095, -65.547, 105.145, -131.397, 31.851, -0.622);

            //ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            //robot.MoveL(startjointPos, startdescPose, 0, 0, 50, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            //robot.SingularAvoidStart(0, 150, 50, 20);
            //robot.MoveL(endjointPos, enddescPose, 0, 0, 50, 100, 100, -1, exaxisPos, 0, 0, offdese, 1, 1);
            //robot.SingularAvoidEnd();

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using fairino;

namespace testFrRobot
{
    public partial class FrmWeld : Form
    {
        Robot robot;
        public FrmWeld(Robot rob)
        {
            InitializeComponent();
            robot = rob;
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

        private void btnArcstart_Click(object sender, EventArgs e)
        {
            robot.ARCStart(int.Parse(txtIOType.Text), int.Parse(txtArcNum.Text), int.Parse(txtWeldTimeOut.Text));
        }

        private void btnArcEnd_Click(object sender, EventArgs e)
        {
            robot.ARCEnd(int.Parse(txtIOType.Text), int.Parse(txtArcNum.Text), int.Parse(txtWeldTimeOut.Text));
        }

        private void btnSetRelation_Click(object sender, EventArgs e)
        {

            robot.WeldingSetCurrentRelation(0, 1000, 0, 10, 0);
            robot.WeldingSetVoltageRelation(0, 100, 0, 10, 1);

            double a = 0, b = 0, c = 0, d = 0;
            int ee = 0;

            robot.WeldingGetCurrentRelation(ref a, ref b, ref c, ref d, ref ee);
            Console.WriteLine($"value is {a}, {b}, {c}, {d}, {ee}");
            robot.WeldingGetVoltageRelation(ref a, ref b, ref c, ref d, ref ee);
            Console.WriteLine($"value is {a}, {b}, {c}, {d}, {ee}");

            robot.WeldingSetCurrent(0, 600, 0, 0);
            robot.WeldingSetVoltage(0, 40, 1, 0);
            //return 0;

            //int rtn = robot.WeldingSetCurrentRelation(double.Parse(txtCurrentMin.Text), double.Parse(txtCurrentMax.Text), double.Parse(txtCurrentVoltageMin.Text), double.Parse(txtCurrentVoltageMax.Text));
            //Console.WriteLine(rtn);
            //rtn = robot.WeldingSetVoltageRelation(double.Parse(txtweldVoltageMin.Text), double.Parse(txtweldVoltageMax.Text), double.Parse(txtVoltageVoltageMin.Text), double.Parse(txtVoltageVoltageMax.Text));
            //Console.WriteLine(rtn);
        }

        private void btnGetRelation_Click(object sender, EventArgs e)
        {
            double curmin = 0;
            double curmax = 0;
            double vurvolmin = 0;
            double curvolmax = 0;

            double volmax = 0;
            double volmin = 0;
            double volvolmin = 0;
            double volvolmax = 0;
            int AOIndex = 0;

            robot.WeldingGetCurrentRelation(ref curmin, ref curmax, ref vurvolmin, ref curvolmax, ref AOIndex);
            robot.WeldingGetVoltageRelation(ref volmin, ref volmax, ref volvolmin, ref volvolmax, ref AOIndex);
            txtCurrentMin.Text = curmin.ToString();
            txtCurrentMax.Text = curmax.ToString();
            txtCurrentVoltageMin.Text = vurvolmin.ToString();
            txtCurrentVoltageMax.Text = curvolmax.ToString();

            txtweldVoltageMin.Text = volmin.ToString();
            txtweldVoltageMax.Text = volmax.ToString();
            txtVoltageVoltageMin.Text = volvolmin.ToString();
            txtVoltageVoltageMax.Text = volvolmax.ToString();
        }

        private void btnSetWeldParam_Click(object sender, EventArgs e)
        {
            int rtn = robot.WeldingSetCurrent(int.Parse(txtIOType.Text), double.Parse(txtCurrent.Text), int.Parse(txtCurrentAOindex.Text), 0);
            Console.WriteLine(rtn);
            rtn = robot.WeldingSetVoltage(int.Parse(txtIOType.Text), double.Parse(txtWeldVoltage.Text), int.Parse(txtWeldVoltageindex.Text), 0);
            Console.WriteLine(rtn);
        }

        private void btnWireR_Click(object sender, EventArgs e)
        {
            robot.SetForwardWireFeed(int.Parse(txtIOType.Text), 1);
        }

        private void btnStopWireR_Click(object sender, EventArgs e)
        {
            robot.SetForwardWireFeed(int.Parse(txtIOType.Text), 0);
        }

        private void btnWireL_Click(object sender, EventArgs e)
        {
            robot.SetReverseWireFeed(int.Parse(txtIOType.Text), 1);
        }

        private void btnStioWireL_Click(object sender, EventArgs e)
        {
            robot.SetReverseWireFeed(int.Parse(txtIOType.Text), 0);
        }

        private void btngas_Click(object sender, EventArgs e)
        {
            robot.SetAspirated(int.Parse(txtIOType.Text), 1);
        }

        private void btnStopgas_Click(object sender, EventArgs e)
        {
            robot.SetAspirated(int.Parse(txtIOType.Text), 0);
        }

        private void btnSetWeaveParam_Click(object sender, EventArgs e)
        {
            //robot.WeaveSetPara(int.Parse(txtWeavNum.Text),
            //                    int.Parse(txtWeavType.Text),
            //                    double.Parse(txtWeaveFre.Text),
            //                    int.Parse(txtWeavType.Text),
            //                    double.Parse(txtWeaveRange.Text),
            //                    double.Parse(txtLeftRange.Text),
            //                    double.Parse(txtRightRange.Text),
            //                    int.Parse(txtPointStayTime.Text),
            //                    int.Parse(txtWeaveLeftStay.Text),
            //                    int.Parse(txtWeavRightStay.Text),
            //                    int.Parse(txtWeaveCircle.Text),
            //                    int.Parse(txtWeaveStation.Text)) ;

        }

        private void btnSetWeaveImm_Click(object sender, EventArgs e)
        {
            robot.WeaveOnlineSetPara(int.Parse(txtWeavNum.Text),
                    int.Parse(txtWeavType.Text),
                    double.Parse(txtWeaveFre.Text),
                    int.Parse(txtWeavType.Text),
                    double.Parse(txtWeaveRange.Text),
                    int.Parse(txtWeaveLeftStay.Text),
                    int.Parse(txtWeavRightStay.Text),
                    int.Parse(txtWeaveCircle.Text),
                    int.Parse(txtWeaveStation.Text));
        }

        private void btnWeldStart_Click(object sender, EventArgs e)
        {
            DescPose startdescPose = new DescPose(double.Parse(txtStartX.Text), double.Parse(txtStartY.Text), double.Parse(txtStartZ.Text), double.Parse(txtStartRX.Text), double.Parse(txtStartRY.Text), double.Parse(txtStartRZ.Text));
            JointPos startjointPos = new JointPos(double.Parse(txtStartJ1.Text), double.Parse(txtStartJ2.Text), double.Parse(txtStartJ3.Text), double.Parse(txtStartJ4.Text), double.Parse(txtStartJ5.Text), double.Parse(txtStartJ6.Text));

            DescPose enddescPose = new DescPose(double.Parse(txtEndX.Text), double.Parse(txtEndY.Text), double.Parse(txtEndZ.Text), double.Parse(txtEndRX.Text), double.Parse(txtEndRY.Text), double.Parse(txtEndRZ.Text));
            JointPos endjointPos = new JointPos(double.Parse(txtEndJ1.Text), double.Parse(txtEndJ2.Text), double.Parse(txtEndJ3.Text), double.Parse(txtEndJ4.Text), double.Parse(txtEndJ5.Text), double.Parse(txtEndJ6.Text));

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.SetSpeed(int.Parse(txtWeldSpeed.Text));

            robot.MoveL(startjointPos, startdescPose, 1, 0, 100, 100, 100, 0, 0, exaxisPos, 0, 0, offdese);
            //robot.ARCStart(int.Parse(txtIOType.Text), int.Parse(txtArcNum.Text), int.Parse(txtWeldTimeOut.Text));
            if (chkWeave.Checked)
            {
                int rtn = robot.WeaveStart(0);
                Console.WriteLine(rtn);
            }

            robot.MoveL(endjointPos, enddescPose, 1, 0, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            //robot.ARCEnd(int.Parse(txtIOType.Text), int.Parse(txtArcNum.Text), int.Parse(txtWeldTimeOut.Text));
            Console.WriteLine("44444444444444444444");
            if (chkWeave.Checked)
            {
                robot.WeaveEnd(0);
                Console.WriteLine("55555555555555555555");
            }
            Console.WriteLine("6666666666666666");

            //DescPose startdescPose = new DescPose(-525.55, 562.3, 417.199, -178.325, 0.847, 31.109);
            //JointPos startjointPos = new JointPos(-58.978, -76.817, 112.494, -127.348, -89.145, -0.063);

            //DescPose enddescPose = new DescPose(-345.155, 535.733, 421.269, 179.475, 0.571, 18.332);
            //JointPos endjointPos = new JointPos(-71.746, -87.177, 123.953, -126.25, -89.429, -0.089);

            //ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            //robot.WeldingSetCurrentRelation(0, 400, 0, 10);
            //robot.WeldingSetVoltageRelation(0, 40, 0, 10);
            //double curmin = 0;
            //double curmax = 0;
            //double vurvolmin = 0;
            //double curvolmax = 0;

            //double volmax = 0;
            //double volmin = 0;
            //double volvolmin = 0;
            //double volvolmax = 0;

            //robot.WeldingGetCurrentRelation(ref curmin, ref curmax, ref vurvolmin, ref curvolmax);
            //robot.WeldingGetVoltageRelation(ref volmin, ref volmax, ref volvolmin, ref volvolmax);

            //robot.WeldingSetCurrent(0, 100, 0); 
            //robot.WeldingSetVoltage(0, 19, 1);

            ////robot.WeaveSetPara(0,0,1,0,10,100,100,0,0);

            //robot.SetForwardWireFeed(0, 1);
            //Thread.Sleep(1000);
            //robot.SetForwardWireFeed(0, 0);
            //robot.SetReverseWireFeed(0, 1);
            //Thread.Sleep(1000);
            //robot.SetReverseWireFeed(0, 0);
            //robot.SetAspirated(0, 1);
            //Thread.Sleep(1000);
            //robot.SetAspirated(0, 0);

            //robot.SetSpeed(5);
            //robot.MoveL(startjointPos, startdescPose, 1, 0, 100, 100, 100, 0, exaxisPos, 0, 0, offdese);
            //robot.ARCStart(0, 0, 1000);
            //robot.WeaveStart(0);
            //robot.MoveL(endjointPos, enddescPose, 1, 0, 100, 100, 100, 0, exaxisPos, 0, 0, offdese);
            //robot.ARCEnd(0, 0, 1000);
            //robot.WeaveEnd(0);
        }

        private void btnJumpWeldStart_Click(object sender, EventArgs e)
        {
            DescPose startdescPose = new DescPose(double.Parse(txtStartX.Text), double.Parse(txtStartY.Text), double.Parse(txtStartZ.Text), double.Parse(txtStartRX.Text), double.Parse(txtStartRY.Text), double.Parse(txtStartRZ.Text));
            JointPos startjointPos = new JointPos(double.Parse(txtStartJ1.Text), double.Parse(txtStartJ2.Text), double.Parse(txtStartJ3.Text), double.Parse(txtStartJ4.Text), double.Parse(txtStartJ5.Text), double.Parse(txtStartJ6.Text));

            DescPose enddescPose = new DescPose(double.Parse(txtEndX.Text), double.Parse(txtEndY.Text), double.Parse(txtEndZ.Text), double.Parse(txtEndRX.Text), double.Parse(txtEndRY.Text), double.Parse(txtEndRZ.Text));
            JointPos endjointPos = new JointPos(double.Parse(txtEndJ1.Text), double.Parse(txtEndJ2.Text), double.Parse(txtEndJ3.Text), double.Parse(txtEndJ4.Text), double.Parse(txtEndJ5.Text), double.Parse(txtEndJ6.Text));

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);
            robot.SetSpeed(int.Parse(txtWeldSpeed.Text));


            robot.SegmentWeldStart(startdescPose, enddescPose, startjointPos, endjointPos, double.Parse(txtWeldLength.Text), double.Parse(txtNoWeldLength.Text),
                int.Parse(txtIOType.Text), int.Parse(txtArcNum.Text), int.Parse(txtWeldTimeOut.Text), chkWeave.Checked, 0, 1, 0, 100, 100, 100, 0, exaxisPos, 0, 0, offdese);
        }

        private void btnWeldEnd_Click(object sender, EventArgs e)
        {

        }

        private void btnWeldStable_Click(object sender, EventArgs e)
        {
            DescPose startdescPose = new DescPose(double.Parse(txtStartX.Text), double.Parse(txtStartY.Text), double.Parse(txtStartZ.Text), double.Parse(txtStartRX.Text), double.Parse(txtStartRY.Text), double.Parse(txtStartRZ.Text));
            JointPos startjointPos = new JointPos(double.Parse(txtStartJ1.Text), double.Parse(txtStartJ2.Text), double.Parse(txtStartJ3.Text), double.Parse(txtStartJ4.Text), double.Parse(txtStartJ5.Text), double.Parse(txtStartJ6.Text));

            DescPose enddescPose = new DescPose(double.Parse(txtEndX.Text), double.Parse(txtEndY.Text), double.Parse(txtEndZ.Text), double.Parse(txtEndRX.Text), double.Parse(txtEndRY.Text), double.Parse(txtEndRZ.Text));
            JointPos endjointPos = new JointPos(double.Parse(txtEndJ1.Text), double.Parse(txtEndJ2.Text), double.Parse(txtEndJ3.Text), double.Parse(txtEndJ4.Text), double.Parse(txtEndJ5.Text), double.Parse(txtEndJ6.Text));

            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            robot.SetSpeed(int.Parse(txtWeldSpeed.Text));

            while (true)
            {
                robot.MoveL(startjointPos, startdescPose, 1, 0, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);

                robot.SetForwardWireFeed(int.Parse(txtIOType.Text), 1);
                robot.SetReverseWireFeed(int.Parse(txtIOType.Text), 1);
                robot.SetAspirated(int.Parse(txtIOType.Text), 1);
                robot.WeldingSetCurrentRelation(double.Parse(txtCurrentMin.Text), double.Parse(txtCurrentMax.Text), double.Parse(txtCurrentVoltageMin.Text), double.Parse(txtCurrentVoltageMax.Text), 0);
                robot.WeldingSetVoltageRelation(double.Parse(txtweldVoltageMin.Text), double.Parse(txtweldVoltageMax.Text), double.Parse(txtVoltageVoltageMin.Text), double.Parse(txtVoltageVoltageMax.Text), 1);
                Thread.Sleep(1000);
                robot.SetForwardWireFeed(int.Parse(txtIOType.Text), 0);
                robot.SetReverseWireFeed(int.Parse(txtIOType.Text), 0);
                robot.SetAspirated(int.Parse(txtIOType.Text), 0);
                /*robot.WeaveSetPara(int.Parse(txtWeavNum.Text),
                               int.Parse(txtWeavType.Text),
                               double.Parse(txtWeaveFre.Text),
                               int.Parse(txtWeavType.Text),
                               double.Parse(txtWeaveRange.Text),
                               int.Parse(txtWeaveLeftStay.Text),
                               int.Parse(txtWeavRightStay.Text),
                               int.Parse(txtWeaveCircle.Text),
                               int.Parse(txtWeaveStation.Text));*/
                robot.WeldingSetCurrent(int.Parse(txtIOType.Text), double.Parse(txtCurrent.Text), int.Parse(txtCurrentAOindex.Text), 0);
                robot.WeldingSetVoltage(int.Parse(txtIOType.Text), double.Parse(txtWeldVoltage.Text), int.Parse(txtWeldVoltageindex.Text), 0);
                double curmin = 0;
                double curmax = 0;
                double vurvolmin = 0;
                double curvolmax = 0;

                double volmax = 0;
                double volmin = 0;
                double volvolmin = 0;
                double volvolmax = 0;
                int AOIndex = 0;
                robot.WeldingGetCurrentRelation(ref curmin, ref curmax, ref vurvolmin, ref curvolmax, ref AOIndex);
                robot.WeldingGetVoltageRelation(ref volmin, ref volmax, ref volvolmin, ref volvolmax, ref AOIndex);
                robot.MoveL(endjointPos, enddescPose, 1, 0, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
                Thread.Sleep(1000);
            }


        }

        private void FrmWeld_Load(object sender, EventArgs e)
        {

        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void btnSetAcc_Click(object sender, EventArgs e)
        {
            //robot.SetOaccScale(int.Parse(txtAcc.Text));
            robot.SetSpeed(int.Parse(txtAcc.Text));
        }

        private void btnWiresearch_Click(object sender, EventArgs e)
        {
            ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
            DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

            DescPose descStart = new DescPose(203.061, 56.768, 62.719, -177.249, 1.456, -83.597);
            JointPos jointStart = new JointPos(-127.012, -112.931, -94.078, -62.014, 87.186, 91.326);

            DescPose descEnd = new DescPose(122.471, 55.718, 62.209, -177.207, 1.375, -76.310);
            JointPos jointEnd = new JointPos(-119.728, -113.017, -94.027, -62.061, 87.199, 91.326);

            robot.MoveL(jointStart, descStart, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
            robot.MoveL(jointEnd, descEnd, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);

            DescPose descREF0A = new DescPose(147.139, -21.436, 60.717, -179.633, -3.051, -83.170);
            JointPos jointREF0A = new JointPos(-121.731, -106.193, -102.561, -64.734, 89.972, 96.171);

            DescPose descREF0B = new DescPose(139.247, 43.721, 65.361, -179.634, -3.043, -83.170);
            JointPos jointREF0B = new JointPos(-122.364, -113.991, -90.860, -68.630, 89.933, 95.540);

            DescPose descREF1A = new DescPose(289.747, 77.395, 58.390, -179.074, -2.901, -89.790);
            JointPos jointREF1A = new JointPos(-135.719, -119.588, -83.454, -70.245, 88.921, 88.819);

            DescPose descREF1B = new DescPose(259.310, 79.998, 64.774, -179.073, -2.900, -89.790);
            JointPos jointREF1B = new JointPos(-133.133, -119.029, -83.326, -70.976, 89.069, 91.401);

            robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF0A, descREF0A, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);  //起点
            robot.MoveL(jointREF0B, descREF0B, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 1, 0, offdese);  //方向点
            robot.WireSearchWait("REF0");
            robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF1A, descREF1A, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);  //起点
            robot.MoveL(jointREF1B, descREF1B, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 1, 0, offdese);  //方向点
            robot.WireSearchWait("REF1");
            robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);


            robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF0A, descREF0A, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);  //起点
            robot.MoveL(jointREF0B, descREF0B, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 1, 0, offdese);  //方向点
            robot.WireSearchWait("RES0");
            robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            robot.WireSearchStart(0, 10, 100, 0, 10, 100, 0);
            robot.MoveL(jointREF1A, descREF1A, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);  //起点
            robot.MoveL(jointREF1B, descREF1B, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 1, 0, offdese);  //方向点
            robot.WireSearchWait("RES1");
            robot.WireSearchEnd(0, 10, 100, 0, 10, 100, 0);

            string[] varNameRef = new string[6] { "REF0", "REF1", "#", "#", "#", "#" };
            string[] varNameRes = new string[6] { "RES0", "RES1", "#", "#", "#", "#" };
            int offectFlag = 0;
            DescPose offectPos = new DescPose();
            robot.GetWireSearchOffset(0, 0, varNameRef, varNameRes, ref offectFlag, ref offectPos);
            robot.PointsOffsetEnable(0, offectPos);


            {
                robot.MoveL(jointStart, descStart, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
                robot.MoveL(jointEnd, descEnd, 1, 1, 100, 100, 100, -1, 0, exaxisPos, 1, 0, offdese);
                robot.PointsOffsetDisable();
            }
        }
            private void btnArcTracking_Click(object sender, EventArgs e)
            {
                //DescPose startdescPose = new DescPose(double.Parse(txtStartX.Text), double.Parse(txtStartY.Text), double.Parse(txtStartZ.Text), double.Parse(txtStartRX.Text), double.Parse(txtStartRY.Text), double.Parse(txtStartRZ.Text));
                //JointPos startjointPos = new JointPos(double.Parse(txtStartJ1.Text), double.Parse(txtStartJ2.Text), double.Parse(txtStartJ3.Text), double.Parse(txtStartJ4.Text), double.Parse(txtStartJ5.Text), double.Parse(txtStartJ6.Text));

                //DescPose enddescPose = new DescPose(double.Parse(txtEndX.Text), double.Parse(txtEndY.Text), double.Parse(txtEndZ.Text), double.Parse(txtEndRX.Text), double.Parse(txtEndRY.Text), double.Parse(txtEndRZ.Text));
                //JointPos endjointPos = new JointPos(double.Parse(txtEndJ1.Text), double.Parse(txtEndJ2.Text), double.Parse(txtEndJ3.Text), double.Parse(txtEndJ4.Text), double.Parse(txtEndJ5.Text), double.Parse(txtEndJ6.Text));

                //ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
                //DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

                //robot.WeldingSetCurrent(1, 230, 0);
                //robot.WeldingSetVoltage(1, 24, 0);

                //robot.SetSpeed(int.Parse(txtWeldSpeed.Text));
                //robot.MoveL(startjointPos, startdescPose, 13, 0, 5, 100, 100, -1, exaxisPos, 0, 0, offdese);
                //robot.ArcWeldTraceControl(1, 0, 0, 0.06, 5, 5, 300, 1, -0.06, 5, 5, 300, 1, 0, 4, 1, 10);
                //robot.ARCStart(1, 0, 10000);
                //robot.MoveL(endjointPos, enddescPose, 13, 0, 5, 100, 100, -1, exaxisPos, 0, 0, offdese);
                //robot.ARCEnd(1, 0, 10000);

                //robot.ArcWeldTraceControl(0, 0, 0, 0.06, 5, 5, 300, 1, -0.06, 5, 5, 300, 1, 0, 4, 1, 10);
                //robot.ArcWeldTraceExtAIChannelConfig(1);

            }

            private void btnSetDatabase_Click(object sender, EventArgs e)
            {
                DescPose startdescPose = new DescPose(double.Parse(txtStartX.Text), double.Parse(txtStartY.Text), double.Parse(txtStartZ.Text), double.Parse(txtStartRX.Text), double.Parse(txtStartRY.Text), double.Parse(txtStartRZ.Text));
                JointPos startjointPos = new JointPos(double.Parse(txtStartJ1.Text), double.Parse(txtStartJ2.Text), double.Parse(txtStartJ3.Text), double.Parse(txtStartJ4.Text), double.Parse(txtStartJ5.Text), double.Parse(txtStartJ6.Text));
                robot.SetPointToDatabase("StartPt", startdescPose);
            }

            private void btnSetWeldProcess_Click(object sender, EventArgs e)
            {
                robot.WeldingSetProcessParam(1, 177, 27, 1000, 178, 28, 176, 26, 1000);
                robot.WeldingSetProcessParam(2, 188, 28, 555, 199, 29, 133, 23, 333);
            }

            private void btnGetWeldProcess_Click(object sender, EventArgs e)
            {
                double startCurrent = 0;
                double startVoltage = 0;
                double startTime = 0;
                double weldCurrent = 0;
                double weldVoltage = 0;
                double endCurrent = 0;
                double endVoltage = 0;
                double endTime = 0;

                robot.WeldingGetProcessParam(1, ref startCurrent, ref startVoltage, ref startTime, ref weldCurrent, ref weldVoltage, ref endCurrent, ref endVoltage, ref endTime);
                Console.WriteLine($"the Num 1 process param is {startCurrent} {startVoltage} {startTime} {weldCurrent} {weldVoltage} {endCurrent} {endVoltage} {endTime}");

                robot.WeldingGetProcessParam(2, ref startCurrent, ref startVoltage, ref startTime, ref weldCurrent, ref weldVoltage, ref endCurrent, ref endVoltage, ref endTime);
                Console.WriteLine($"the Num 2 process param is {startCurrent} {startVoltage} {startTime} {weldCurrent} {weldVoltage} {endCurrent} {endVoltage} {endTime}");
            }

            private void btnSetExtIO_Click(object sender, EventArgs e)
            {
                robot.SetArcStartExtDoNum(1);
                robot.SetAirControlExtDoNum(2);
                robot.SetWireForwardFeedExtDoNum(3);
                robot.SetWireReverseFeedExtDoNum(4);

                robot.SetWeldReadyExtDiNum(5);
                robot.SetArcDoneExtDiNum(6);
                robot.SetExtDIWeldBreakOffRecover(7, 8);
            }

            private void btnWeaveSim_Click(object sender, EventArgs e)
            {
                //DescPose startdescPose = new DescPose(double.Parse(txtStartX.Text), double.Parse(txtStartY.Text), double.Parse(txtStartZ.Text), double.Parse(txtStartRX.Text), double.Parse(txtStartRY.Text), double.Parse(txtStartRZ.Text));
                //JointPos startjointPos = new JointPos(double.Parse(txtStartJ1.Text), double.Parse(txtStartJ2.Text), double.Parse(txtStartJ3.Text), double.Parse(txtStartJ4.Text), double.Parse(txtStartJ5.Text), double.Parse(txtStartJ6.Text));

                //DescPose enddescPose = new DescPose(double.Parse(txtEndX.Text), double.Parse(txtEndY.Text), double.Parse(txtEndZ.Text), double.Parse(txtEndRX.Text), double.Parse(txtEndRY.Text), double.Parse(txtEndRZ.Text));
                //JointPos endjointPos = new JointPos(double.Parse(txtEndJ1.Text), double.Parse(txtEndJ2.Text), double.Parse(txtEndJ3.Text), double.Parse(txtEndJ4.Text), double.Parse(txtEndJ5.Text), double.Parse(txtEndJ6.Text));

                DescPose startdescPose = new DescPose(238.209, -403.633, 251.291, 177.222, -1.433, 133.675);
                JointPos startjointPos = new JointPos(-48.728, -86.235, -95.288, -90.025, 92.715, 87.595);
                DescPose enddescPose = new DescPose(238.207, -596.305, 251.294, 177.223, -1.432, 133.675);
                JointPos endjointPos = new JointPos(-60.240, -110.743, -66.784, -94.531, 92.351, 76.078);

                ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
                DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

                robot.MoveL(startjointPos, startdescPose, 1, 0, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
                robot.WeaveStartSim(0);
                robot.MoveL(endjointPos, enddescPose, 1, 0, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
                robot.WeaveEndSim(0);
            }

            private void btnWeaveInspect_Click(object sender, EventArgs e)
            {
                //DescPose startdescPose = new DescPose(double.Parse(txtStartX.Text), double.Parse(txtStartY.Text), double.Parse(txtStartZ.Text), double.Parse(txtStartRX.Text), double.Parse(txtStartRY.Text), double.Parse(txtStartRZ.Text));
                //JointPos startjointPos = new JointPos(double.Parse(txtStartJ1.Text), double.Parse(txtStartJ2.Text), double.Parse(txtStartJ3.Text), double.Parse(txtStartJ4.Text), double.Parse(txtStartJ5.Text), double.Parse(txtStartJ6.Text));

                //DescPose enddescPose = new DescPose(double.Parse(txtEndX.Text), double.Parse(txtEndY.Text), double.Parse(txtEndZ.Text), double.Parse(txtEndRX.Text), double.Parse(txtEndRY.Text), double.Parse(txtEndRZ.Text));
                //JointPos endjointPos = new JointPos(double.Parse(txtEndJ1.Text), double.Parse(txtEndJ2.Text), double.Parse(txtEndJ3.Text), double.Parse(txtEndJ4.Text), double.Parse(txtEndJ5.Text), double.Parse(txtEndJ6.Text));

                DescPose startdescPose = new DescPose(238.209, -403.633, 251.291, 177.222, -1.433, 133.675);
                JointPos startjointPos = new JointPos(-48.728, -86.235, -95.288, -90.025, 92.715, 87.595);
                DescPose enddescPose = new DescPose(238.207, -596.305, 251.294, 177.223, -1.432, 133.675);
                JointPos endjointPos = new JointPos(-60.240, -110.743, -66.784, -94.531, 92.351, 76.078);

                ExaxisPos exaxisPos = new ExaxisPos(0, 0, 0, 0);
                DescPose offdese = new DescPose(0, 0, 0, 0, 0, 0);

                robot.MoveL(startjointPos, startdescPose, 1, 0, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
                robot.WeaveInspectStart(0);
                robot.MoveL(endjointPos, enddescPose, 1, 0, 100, 100, 100, -1, 0, exaxisPos, 0, 0, offdese);
                robot.WeaveInspectEnd(0);
            }

            private void btnAngleWeave_Click(object sender, EventArgs e)
            {
                DescPose desc_p1, desc_p2;

                desc_p1 = new DescPose(-299.979, -399.974, 74.979, 0.009, 0.001, -41.530);
                desc_p2 = new DescPose(-49.985, -399.956, 74.978, 0.009, 0.005, -41.530);

                JointPos j1 = new JointPos(41.476, -77.300, 118.714, -131.405, -90.002, -51.993);
                JointPos j2 = new JointPos(68.366, -89.562, 133.018, -133.446, -90.002, -25.105);

                ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
                DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);

                robot.WeaveSetPara(0, 3, 2.000000, 0, 10.000000, 0.000000, 0.000000, 0, 0, 0, 0, 0, 0, 0);
                robot.MoveJ(j1, desc_p1, 13, 0, 100, 100, 100, epos, -1, 0, offset_pos);
                robot.WeaveStart(0);
                robot.MoveL(j2, desc_p2, 13, 0, 10, 100, 100, -1, 0, epos, 0, 0, offset_pos, 0, 100);
                robot.WeaveEnd(0);

                robot.WeaveSetPara(0, 3, 2.000000, 0, 10.000000, 0.000000, 0.000000, 0, 0, 0, 0, 0, 0, 0);
                robot.MoveJ(j1, desc_p1, 13, 0, 100, 100, 100, epos, -1, 0, offset_pos);
                robot.WeaveStart(0);
                robot.MoveL(j2, desc_p2, 13, 0, 10, 100, 100, -1, 0, epos, 0, 0, offset_pos, 0, 100);
                robot.WeaveEnd(0);
            }

            private void btnWeldReply_Click(object sender, EventArgs e)
            {
                JointPos mulitilineorigin1_joint = new JointPos(-24.090, -63.501, 84.288, -111.940, -93.426, 57.669);
                DescPose mulitilineorigin1_desc = new DescPose(-677.559, 190.951, -1.205, 1.144, -41.482, -82.577);

                JointPos mulitilineX1_joint = new JointPos(-25.734, -62.843, 83.315, -111.723, -93.392, 56.021);
                DescPose mulitilineX1_desc = new DescPose(-677.556, 211.949, -1.206, 1.145, -41.482, -82.577);

                JointPos mulitilineZ1_joint = new JointPos(-24.090, -64.449, 82.477, -109.183, -93.427, 57.668);
                DescPose mulitilineZ1_desc = new DescPose(-677.564, 190.956, 19.817, 1.143, -41.481, -82.576);

                JointPos mulitilinesafe_joint = new JointPos(-25.734, -63.778, 81.502, -108.975, -93.392, 56.021);
                DescPose mulitilinesafe_desc = new DescPose(-677.561, 211.950, 19.812, 1.144, -41.482, -82.577);

                JointPos mulitilineorigin2_joint = new JointPos(-29.743, -75.623, 101.241, -116.354, -94.928, 55.735);
                DescPose mulitilineorigin2_desc = new DescPose(-563.961, 215.359, -0.681, 2.845, -40.476, -87.443);

                JointPos mulitilineX2_joint = new JointPos(-30.182, -75.433, 101.005, -116.346, -94.922, 55.294);
                DescPose mulitilineX2_desc = new DescPose(-563.965, 220.355, -0.680, 2.845, -40.476, -87.442);

                JointPos mulitilineZ2_joint = new JointPos(-29.743, -75.916, 100.817, -115.637, -94.928, 55.735);
                DescPose mulitilineZ2_desc = new DescPose(-563.968, 215.362, 4.331, 2.844, -40.476, -87.442);

                ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
                DescPose offset = new DescPose(0, 0, 0, 0, 0, 0);


                int error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
                Console.WriteLine("MoveJ return:  " + error);

                error = robot.MoveL(mulitilineorigin1_joint, mulitilineorigin1_desc, 13, 0, 10, 100, 100, -1, 0, epos, 0, 0, offset, 0, 100);
                Console.WriteLine("MoveL return:  " + error);

                error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
                Console.WriteLine("MoveJ return:  " + error);

                error = robot.MoveL(mulitilineorigin2_joint, mulitilineorigin2_desc, 13, 0, 10, 100, 100, -1, 0, epos, 0, 0, offset, 0, 100);
                Console.WriteLine("MoveL return:  " + error);

                error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
                Console.WriteLine("MoveJ return:  " + error);

                error = robot.MoveL(mulitilineorigin1_joint, mulitilineorigin1_desc, 13, 0, 10, 100, 100, -1, 0, epos, 0, 0, offset, 0, 100);
                Console.WriteLine("MoveJ return:  " + error);

                error = robot.ARCStart(1, 0, 3000);
                Console.WriteLine("ARCStart return:  " + error);

                error = robot.WeaveStart(0);
                Console.WriteLine("WeaveStart return:  " + error);

                error = robot.ArcWeldTraceControl(1, 0, 0, 0.06, 5, 5, 300, 1, -0.06, 5, 5, 300, 1, 0, 4, 1, 10, 0, 0); ;
                Console.WriteLine("ArcWeldTraceControl return:  " + error);

                error = robot.MoveL(mulitilineorigin2_joint, mulitilineorigin2_desc, 13, 0, 1, 100, 100, -1, 0, epos, 0, 0, offset, 0, 100);
                Console.WriteLine("MoveL return:  " + error);

                error = robot.ArcWeldTraceControl(1, 0, 0, 0.06, 5, 5, 300, 1, -0.06, 5, 5, 300, 1, 0, 4, 1, 10, 0, 0); ;
                Console.WriteLine("ArcWeldTraceControl return:  " + error);

                error = robot.WeaveEnd(0);
                Console.WriteLine("WeaveEnd return:  " + error);

                error = robot.ARCEnd(1, 0, 10000);
                Console.WriteLine("ARCEnd return:  " + error);


                error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
                Console.WriteLine("MoveJ return:  " + error);

                error = robot.MultilayerOffsetTrsfToBase(mulitilineorigin1_desc.tran, mulitilineX1_desc.tran, mulitilineZ1_desc.tran, 10.0, 0.0, 0.0, ref offset);
                Console.WriteLine("MultilayerOffsetTrsfToBase return:  " + error);

                error = robot.MoveL(mulitilineorigin1_joint, mulitilineorigin1_desc, 13, 0, 10, 100, 100, -1, 0, epos, 0, 1, offset, 0, 100);
                Console.WriteLine("MoveJ return:  " + error);

                error = robot.ARCStart(1, 0, 3000);
                Console.WriteLine("ARCStart return:  " + error);

                error = robot.MultilayerOffsetTrsfToBase(mulitilineorigin2_desc.tran, mulitilineX2_desc.tran, mulitilineZ2_desc.tran, 10, 0, 0, ref offset);
                Console.WriteLine("MultilayerOffsetTrsfToBase return:  " + error);

                error = robot.ArcWeldTraceReplayStart();
                Console.WriteLine("ArcWeldTraceReplayStart return:  " + error);

                error = robot.MoveL(mulitilineorigin2_joint, mulitilineorigin2_desc, 13, 0, 2, 100, 100, -1, 0, epos, 0, 1, offset, 0, 100);
                Console.WriteLine("MoveL return:  " + error);

                error = robot.ArcWeldTraceReplayEnd();
                Console.WriteLine("ArcWeldTraceReplayEnd return:  " + error);

                error = robot.ARCEnd(1, 0, 10000);
                Console.WriteLine("ARCEnd return:  " + error);

                error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
                Console.WriteLine("MoveJ return:  " + error);

                error = robot.MultilayerOffsetTrsfToBase(mulitilineorigin1_desc.tran, mulitilineX1_desc.tran, mulitilineZ1_desc.tran, 0, 10, 0, ref offset);
                Console.WriteLine("MultilayerOffsetTrsfToBase return:  " + error);

                error = robot.MoveL(mulitilineorigin1_joint, mulitilineorigin1_desc, 13, 0, 10, 100, 100, -1, 0, epos, 0, 1, offset, 0, 100);
                Console.WriteLine("MoveJ return:  " + error);

                error = robot.ARCStart(1, 0, 3000);
                Console.WriteLine("ARCStart return:  " + error);

                error = robot.MultilayerOffsetTrsfToBase(mulitilineorigin2_desc.tran, mulitilineX2_desc.tran, mulitilineZ2_desc.tran, 0, 10, 0, ref offset);
                Console.WriteLine("MultilayerOffsetTrsfToBase return:  " + error);

                error = robot.ArcWeldTraceReplayStart();
                Console.WriteLine("ArcWeldTraceReplayStart return:  " + error);

                error = robot.MoveL(mulitilineorigin2_joint, mulitilineorigin2_desc, 13, 0, 2, 100, 100, -1, 0, epos, 0, 1, offset, 0, 100);
                Console.WriteLine("MoveL return:  " + error);

                error = robot.ArcWeldTraceReplayEnd();
                Console.WriteLine("ArcWeldTraceReplayEnd return:  " + error);

                error = robot.ARCEnd(1, 0, 3000);
                Console.WriteLine("ARCEnd return:  " + error);

                error = robot.MoveJ(mulitilinesafe_joint, mulitilinesafe_desc, 13, 0, 10, 100, 100, epos, -1, 0, offset);
                Console.WriteLine("MoveJ return:  " + error);
            }

            private void btnAngluar_Click(object sender, EventArgs e)
            {
                JointPos JP1 = new JointPos(-68.030, -63.537, -105.223, -78.368, 72.828, 24.876);
                DescPose DP1 = new DescPose(-60.984, -533.958, 279.089, -22.052, -4.777, 172.406);

                JointPos JP2 = new JointPos(-80.916, -76.030, -108.901, -70.956, 99.026, -74.533);
                DescPose DP2 = new DescPose(36.750, -488.721, 145.781, -37.539, -11.211, -96.491);

                JointPos JP3 = new JointPos(-86.898, -95.200, -103.665, -70.570, 98.266, -93.321);
                DescPose DP3 = new DescPose(-21.462, -509.234, 25.706, -41.780, -1.042, -83.611);

                JointPos JP4 = new JointPos(-85.364, -102.697, -94.674, -70.557, 95.302, -93.116);
                DescPose DP4 = new DescPose(-24.075, -580.525, 25.881, -44.818, -2.357, -82.259);

                JointPos JP5 = new JointPos(-78.815, -94.279, -105.315, -65.348, 87.328, 3.220);
                DescPose DP5 = new DescPose(-29.155, -580.477, 25.884, -44.795, -2.374, -172.261);

                JointPos JP6 = new JointPos(-81.057, -94.494, -105.107, -65.241, 87.527, 0.987);
                DescPose DP6 = new DescPose(-49.270, -580.460, 25.886, -44.796, -2.374, -172.263);

                JointPos JP7 = new JointPos(-76.519, -101.428, -94.915, -76.521, 85.041, 95.758);
                DescPose DP7 = new DescPose(-54.189, -580.362, 25.878, -44.779, -2.353, 97.740);

                JointPos JP8 = new JointPos(-74.406, -90.991, -106.574, -75.480, 85.150, 97.875);
                DescPose DP8 = new DescPose(-54.142, -503.358, 25.865, -44.780, -2.353, 97.740);

                ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
                DescPose offset = new DescPose(0, 0, 0, 0, 0, 0);

                int tool = 7;
                int user = 0;
                float vel = 100.0f;
                float acc = 100.0f;
                float ovl = 50.0f;
                int blend = -1;
                byte offsetFlag = 0;

                int error = robot.MoveJ(JP1, DP1, tool, user, vel, acc, ovl, epos, blend, offsetFlag, offset);
                error = robot.MoveJ(JP2, DP2, tool, user, vel, acc, ovl, epos, blend, offsetFlag, offset);
                error = robot.MoveL(JP3, DP3, tool, user, vel, acc, ovl, blend, 0, epos, 0, offsetFlag, offset, 0, 100);
                robot.SetOaccScale(100);
                error = robot.MoveL(JP4, DP4, tool, user, vel, acc, ovl * 0.1f, blend, 0, epos, 0, offsetFlag, offset, 0, 100);
                robot.AngularSpeedStart(50);
                error = robot.MoveL(JP5, DP5, tool, user, vel, acc, ovl * 0.1f, blend, 0, epos, 0, offsetFlag, offset, 0, 100);
                robot.AngularSpeedEnd();
                error = robot.MoveL(JP6, DP6, tool, user, vel, acc, ovl * 0.1f, blend, 0, epos, 0, offsetFlag, offset, 0, 100);
                robot.AngularSpeedStart(50);
                error = robot.MoveL(JP7, DP7, tool, user, vel, acc, ovl * 0.1f, blend, 0, epos, 0, offsetFlag, offset, 0, 100);
                robot.AngularSpeedEnd();
                error = robot.MoveL(JP8, DP8, tool, user, vel, acc, ovl * 0.1f, blend, 0, epos, 0, offsetFlag, offset, 0, 100);
            }
        }
    }


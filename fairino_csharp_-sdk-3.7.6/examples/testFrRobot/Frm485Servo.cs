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
using fairino;

namespace testFrRobot
{
    public partial class Frm485Servo : Form
    {
        Robot robot;
        public Frm485Servo(Robot rob)
        {
            InitializeComponent();
            robot = rob;
        }

        private void Frm485Servo_Load(object sender, EventArgs e)
        {
            comboHomeMode.SelectedIndex = 0;
            comboMode.SelectedIndex = 0;
        }

        private void btnSetParam_Click(object sender, EventArgs e)
        {
            robot.AuxServoSetParam(int.Parse(txtID.Text), int.Parse(txtCompany.Text), int.Parse(txtModel.Text), int.Parse(txtSoft.Text), int.Parse(txtFenbian.Text), double.Parse(txtRadio.Text));
        }

        private void btnUnEnable_Click(object sender, EventArgs e)
        {
            robot.AuxServoEnable(int.Parse(txtID.Text), 0);
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            robot.AuxServoEnable(int.Parse(txtID.Text), 1);
        }

        private void btnClearError_Click(object sender, EventArgs e)
        {
            robot.AuxServoClearError(int.Parse(txtID.Text));
        }

        private void btnSetMode_Click(object sender, EventArgs e)
        {
            robot.AuxServoSetControlMode(int.Parse(txtID.Text), comboMode.SelectedIndex);
        }

        private void btnTargetSpeed_Click(object sender, EventArgs e)
        {
            robot.AuxServoSetTargetSpeed(int.Parse(txtID.Text), double.Parse(txtTargetSpeed.Text));
        }

        private void btnSetPos_Click(object sender, EventArgs e)
        {
            robot.AuxServoSetTargetPos(int.Parse(txtID.Text), double.Parse(txtTargetPos.Text), double.Parse(txtTargetSpeed.Text));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            robot.AuxServoHoming(int.Parse(txtID.Text), comboHomeMode.SelectedIndex + 1, double.Parse(txtSearchVel.Text), double.Parse(txtLatchVel.Text));
        }

        private void btnGetParam_Click(object sender, EventArgs e)
        {
            int ID = -1, company = -1, model = -1, soft = -1, fenbian = -1;
            double radio = 0;
            int errCode = 0;
            int servoState = 0;
            double pos = 0;
            double speed = 0;
            double torque = 0;


            robot.AuxServoGetParam(int.Parse(txtID.Text), ref company, ref model, ref soft, ref fenbian, ref radio);
            lblGetParam.Text = $"ID:     厂商:{company}     型号：{model}     软件版本：{soft}     分辨率：{fenbian}      机械传动比：{radio}";
            ROBOT_STATE_PKG pKG = new ROBOT_STATE_PKG();
            robot.GetRobotRealTimeState(ref pKG);
            lblFedBack.Text = "状态反馈---错误码：" + pKG.auxState.servoErrCode  +  "         状态：" + Convert.ToString(pKG.auxState.servoState,2) +  "      位置：" + pKG.auxState.servoPos.ToString("f2") + "        速度：" + pKG.auxState.servoVel.ToString("f2")  +    "        力矩：" + pKG.auxState.servoTorque.ToString("f2");
            if ((pKG.auxState.servoState & 0x20) != 0)
            {
               // homeOver = true;
                Console.WriteLine("success");
            }
            else
            {
                Console.WriteLine("fail");
            }


            robot.AuxServoGetStatus(1, ref errCode, ref servoState, ref pos, ref speed, ref torque);
            lblCureState.Text = "接口获取---错误码：" + errCode + "         状态：" + Convert.ToString(servoState, 2) + "      位置：" + pos.ToString("f2") + "        速度：" + speed.ToString("f2") + "        力矩：" + torque.ToString("f2");
            int protocol = 0;
            robot.GetExDevProtocol(ref protocol);
            string proname = "";
            if(protocol == 4096)
            {
                proname = "扩展轴";
            }
            else if(protocol == 4097)
            {
                proname = "ModbusSlave";
            }
            else if(protocol == 4098)
            {
                proname = "ModbusMaster";
            }

            lblDevProtocol.Text = "外设协议：   " + proname;
        }

        private void btnSetFeadBackNUm_Click(object sender, EventArgs e)
        {
            robot.AuxServosetStatusID(int.Parse(txtFeadBackNum.Text));
        }

        private void btnSetProto_Click(object sender, EventArgs e)
        {
            robot.SetExDevProtocol(comboProtocol.SelectedIndex + 4096);
        }

        private void btnStable_Click(object sender, EventArgs e)
        {
            
                robot.AuxServoSetParam(int.Parse(txtID.Text), int.Parse(txtCompany.Text), int.Parse(txtModel.Text), int.Parse(txtSoft.Text), int.Parse(txtFenbian.Text), double.Parse(txtRadio.Text));
                int ID = -1, company = -1, model = -1, soft = -1, fenbian = -1;
                double radio = 0;
                int errCode = 0;
                int servoState = 0;
                double pos = 0;
                double speed = 0;
                double torque = 0;
                bool homeOver = false;


                robot.AuxServoGetParam(int.Parse(txtID.Text), ref company, ref model, ref soft, ref fenbian, ref radio);
                lblGetParam.Text = $"ID:     厂商:{company}     型号：{model}     软件版本：{soft}     分辨率：{fenbian}      机械传动比：{radio}";
                ROBOT_STATE_PKG pKG = new ROBOT_STATE_PKG();
                robot.GetRobotRealTimeState(ref pKG);
                Console.WriteLine($"the state is {pKG.main_code}");
                lblFedBack.Text = "状态反馈---错误码：" + pKG.auxState.servoErrCode + "         状态：" + Convert.ToString(pKG.auxState.servoState, 2) + "      位置：" + pKG.auxState.servoPos.ToString("f2") + "        速度：" + pKG.auxState.servoVel.ToString("f2") + "        力矩：" + pKG.auxState.servoTorque.ToString("f2");
                robot.AuxServoGetStatus(1, ref errCode, ref servoState, ref pos, ref speed, ref torque);
                lblCureState.Text = "接口获取---错误码：" + errCode + "         状态：" + Convert.ToString(servoState, 2) + "      位置：" + pos.ToString("f2") + "        速度：" + speed.ToString("f2") + "        力矩：" + torque.ToString("f2");
                int protocol = 0;
                robot.GetExDevProtocol(ref protocol);
                string proname = "";
                if (protocol == 4096)
                {
                    proname = "扩展轴";
                }
                else if (protocol == 4097)
                {
                    proname = "ModbusSlave";
                }
                else if (protocol == 4098)
                {
                    proname = "ModbusMaster";
                }

                lblDevProtocol.Text = "外设协议：   " + proname;
                Thread.Sleep(100);
                robot.AuxServoEnable(int.Parse(txtID.Text), 0);
                Thread.Sleep(100);
                robot.AuxServoSetControlMode(int.Parse(txtID.Text), 0);
                Thread.Sleep(100);
                robot.AuxServoEnable(int.Parse(txtID.Text), 1);
                robot.GetRobotRealTimeState(ref pKG);
                lblFedBack.Text = "状态反馈---错误码：" + pKG.auxState.servoErrCode + "         状态：" + Convert.ToString(pKG.auxState.servoState, 2) + "      位置：" + pKG.auxState.servoPos.ToString("f2") + "        速度：" + pKG.auxState.servoVel.ToString("f2") + "        力矩：" + pKG.auxState.servoTorque.ToString("f2");
                Thread.Sleep(100);
                robot.AuxServoHoming(int.Parse(txtID.Text), 1, 20, 5);
                Thread.Sleep(4000);
            //homeOver = false;
            //while(!homeOver)
            //{
            //    robot.GetRobotRealTimeState(ref pKG);
            //    Console.WriteLine(pKG.auxState.servoState & 0x20);
            //    if((pKG.auxState.servoState & 0x20) != 0)
            //    {
            //        homeOver = true;
            //        Console.WriteLine("success");
            //    }
            //    else
            //    {
            //        Console.WriteLine("fail");
            //    }
            //    Thread.Sleep(1);
            //}
            while (true)
            {
                robot.AuxServoSetTargetPos(int.Parse(txtID.Text), 1000, 100);
                Thread.Sleep(1000);
                robot.AuxServoSetTargetPos(int.Parse(txtID.Text), 0, 100);
                Thread.Sleep(1000);

            }

            robot.AuxServoEnable(int.Parse(txtID.Text), 0);
                Thread.Sleep(100);
                robot.AuxServoSetControlMode(int.Parse(txtID.Text), 1);
                Thread.Sleep(100);
                robot.AuxServoEnable(int.Parse(txtID.Text), 1);
                Thread.Sleep(100);
                robot.AuxServoHoming(int.Parse(txtID.Text), 1, 20, 5);
                Thread.Sleep(4000);
                robot.AuxServoSetTargetSpeed(int.Parse(txtID.Text), 50);
                Thread.Sleep(3000);

                robot.AuxServoSetTargetSpeed(int.Parse(txtID.Text), -300);
                Thread.Sleep(3000);
                robot.AuxServoSetTargetSpeed(int.Parse(txtID.Text), 0);
                Thread.Sleep(100);



            

        }

        private void btnACC_Click(object sender, EventArgs e)
        {
            robot.AuxServoSetParam(1, 1, 1, 1, 130172, 15.45);
            robot.AuxServoEnable(1, 0);
            Thread.Sleep(1000);
            robot.AuxServoSetControlMode(1, 1);
            Thread.Sleep(1000);
            robot.AuxServoEnable(1, 1);
            Thread.Sleep(1000);
            robot.AuxServoHoming(1, 1, 10, 10, 100);
            Thread.Sleep(4000);
            robot.AuxServoSetAcc(3000, 3000);
            robot.AuxServoSetEmergencyStopAcc(5000, 5000);
            Thread.Sleep(1000);
            double emagacc = 0;
            double emagdec = 0;
            robot.AuxServoGetEmergencyStopAcc(ref emagacc, ref emagdec);
            Console.WriteLine($"emergency acc is {emagacc}  dec is {emagdec} ");

            robot.AuxServoSetTargetSpeed(1, 500, 100);

            robot.ProgramLoad("/fruser/testPTP.lua");
            robot.ProgramRun();
            int i = 0;
            while (true)
            {
                i++;
                if(i > 400)
                {
                    robot.ResetAllError();
                    i = 0; 

                    robot.AuxServoSetTargetSpeed(1, 500, 100);
                }
                ROBOT_STATE_PKG pkg = new ROBOT_STATE_PKG();
                robot.GetRobotRealTimeState(ref pkg);
                Console.WriteLine($"cur velocity is {pkg.auxState.servoVel}   cur 485 axis emergency state is {((pkg.auxState.servoState >> 7) & 0x01)}   robot collision state is {pkg.collisionState}  robot emergency state is {pkg.EmergencyStop}");
                Thread.Sleep(5);
                /*


                robot.AuxServoSetAcc(5000, 5000);
                robot.AuxServoSetTargetPos(1, 1000, 500, 100);
                Thread.Sleep(2000);
                robot.AuxServoSetTargetPos(1, 0, 500, 100);
                Thread.Sleep(3000);

                robot.AuxServoSetAcc(500, 500);
                robot.AuxServoSetTargetPos(1, 1000, 500, 100);
                Thread.Sleep(2000);
                robot.AuxServoSetTargetPos(1, 0, 500, 100);
                Thread.Sleep(3000);

                robot.AuxServoSetTargetPos(1, 1000, 500, 10);
                Thread.Sleep(5000);
                robot.AuxServoSetTargetPos(1, 0, 500, 10);
                Thread.Sleep(5000);
                robot.AuxServoSetTargetSpeed(1, 500, 100);
                Thread.Sleep(2000);
                robot.AuxServoSetTargetSpeed(1, 0, 100);
                Thread.Sleep(2000);
                robot.AuxServoSetTargetSpeed(1, 500, 10);
                Thread.Sleep(2000);
                robot.AuxServoSetTargetSpeed(1, 0, 10);
                Thread.Sleep(2000);
                */
            }
        }
    }
}

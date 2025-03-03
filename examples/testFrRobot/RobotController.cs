//using fairino;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace testFrRobot
//{
//    internal class RobotController
//    {
//        private Robot robot;
//        private int rrpc;
//        private ROBOT_STATE_PKG currentState;
//        private object stateLock = new object();
//        private bool isRunning = true;
//        private Thread stateUpdateThread;

//        // 运动参数配置
//        private const float JumpHeight = 50.0f; // 拱门高度
//        private const float DefaultVel = 30.0f;
//        private const float DefaultAcc = 20.0f;
//        private const int ToolNumber = 1;
//        private const int UserFrame = 0;

//        public RobotController()
//        {
//            robot = new Robot();
//            robot.SetReconnectParam(true, 1000, 20);
//            robot.LoggerInit(FrLogType.BUFFER, FrLogLevel.INFO, "D://log/", 5, 5);
//        }

//        // 连接管理
//        public bool Connect(string ip)
//        {
//            try
//            {
//                rrpc = robot.RPC(ip);
//                if (GetConnectionState())
//                {
//                    ResetAllError();
//                    //RobotEnable(true);
//                    StartStateUpdateThread();
//                    return true;
//                }
//                return false;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"连接失败: {ex.Message}");
//                return false;
//            }
//        }

//        public void Disconnect()
//        {
//            isRunning = false;
//            RobotEnable(false);
//            robot.CloseRPC();
//            if (stateUpdateThread != null && stateUpdateThread.IsAlive)
//                stateUpdateThread.Join();
//        }

//        private bool GetConnectionState()
//        {
//            int state = 0;
//            robot.GetSDKComState(ref state);
//            return state == 1;
//        }

//        // 状态更新线程
//        private void StartStateUpdateThread()
//        {
//            stateUpdateThread = new Thread(() =>
//            {
//                while (isRunning)
//                {
//                    try
//                    {
//                        var pkg = new ROBOT_STATE_PKG();
//                        if (robot.GetRobotRealTimeState(ref pkg) == 0)
//                        {
//                            lock (stateLock)
//                            {
//                                currentState = pkg;
//                            }
//                        }
//                        Thread.Sleep(50); // 50ms更新间隔
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"状态更新异常: {ex.Message}");
//                    }
//                }
//            });
//            stateUpdateThread.Start();
//        }

//        // 基础运动控制
//        public void ExecuteJumpMovement(DescPose target)
//        {
//            // 获取当前坐标
//            double[] currentPos = GetCurrentPosition();

//            // 抬起拱门高度
//            var liftPos = currentPos;
//            liftPos.z += JumpHeight;
//            MoveL(liftPos);

//            // 移动到目标XY
//            var xyPos = new DescPose(target.x, target.y, liftPos.z, currentPos.rx, currentPos.ry, currentPos.rz);
//            MoveJ(xyPos);

//            // 下降至目标Z
//            MoveL(target);
//        }

//        public void ExecuteMovel(DescPose target)
//        {
//            MoveL(target);
//        }

//        // 运动指令封装
//        private void MoveL(DescPose pos)
//        {
//            PrepareMovement();
//            var jointPos = CalculateInverseKinematics(pos);
//            robot.MoveL(jointPos, pos, ToolNumber, UserFrame,
//                       DefaultVel, DefaultAcc, 0, 0,
//                       new ExaxisPos(), 0, 0, new DescPose());
//        }

//        private void MoveJ(DescPose pos)
//        {
//            PrepareMovement();
//            var jointPos = CalculateInverseKinematics(pos);
//            robot.MoveJ(jointPos, pos, ToolNumber, UserFrame,
//                       DefaultVel, DefaultAcc, 0,
//                       new ExaxisPos(), 0, 0, new DescPose());
//        }
//        private void JUMP(DescPose pos)
//        {
//            PrepareMovement();
//            var jointPos = CalculateInverseKinematics(pos);
//            robot.MoveJ(jointPos, pos, ToolNumber, UserFrame,
//                       DefaultVel, DefaultAcc, 0,
//                       new ExaxisPos(), 0, 0, new DescPose());
//        }
//        // 运动准备步骤
//        private void PrepareMovement()
//        {
//            VerifySpeedSettings();
//            GetJointConfig();
//            CheckSafety();

//            robot.SetSpeed();
//        }

//        // 工具方法
//        private double[] GetCurrentPosition()
//        {
//            lock (stateLock)
//            {
//                return currentState.jt_cur_pos;
//            }
//        }

//        private JointPos CalculateInverseKinematics(DescPose target)
//        {
//            // 实现逆运动学计算
//            DescPose p1Desc = new DescPose(-424.459, 7.448, 215.42, -175.985, -30.876, -31.116);
//            JointPos p1Joint = new JointPos(-19.346, -84.594, 118.696, -93.052, -87.944, 100.089);

//            DescPose p2Desc = new DescPose(-283.836, -320.673, 346.828, 174.554, -9.608, 18.926);
//            JointPos p2Joint = new JointPos(29.084, -98.238, 114.794, -98.047, -82.947, 100.089);

//            ExaxisPos epos = new ExaxisPos(0, 0, 0, 0);
//            DescPose desc_p1 = new DescPose(0, 0, 0, 0, 0, 0);
//            DescPose desc_p2 = new DescPose(0, 0, 0, 0, 0, 0);
//            DescPose offset_pos = new DescPose(0, 0, 0, 0, 0, 0);

//            JointPos j1, j2;
//            j2 = new JointPos(0, 0, 0, 0, 0, 0);
//            j1 = new JointPos(0, 0, 0, 0, 0, 0);

//            error = robot.GetInverseKin(0, p1Desc, -1, ref j1);

//            return robot.GetInverseKin();
//        }

//        private void VerifySpeedSettings()
//        {
//            // 验证速度参数
//        }

//        private void GetJointConfig()
//        {

//        }

//        private void CheckSafety()
//        {
//            // 安全检查
//        }


//        public void ResetAllError()
//        {
//            robot.ResetAllError();
//        }
//    }
//}

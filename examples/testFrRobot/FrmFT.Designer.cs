namespace testFrRobot
{
    partial class FrmFT
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDragStart = new System.Windows.Forms.Button();
            this.btnStopDrag = new System.Windows.Forms.Button();
            this.btnSixStart = new System.Windows.Forms.Button();
            this.btnSixEnd = new System.Windows.Forms.Button();
            this.btnGetDragState = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnFtAutoZero = new System.Windows.Forms.Button();
            this.btnStaticStart = new System.Windows.Forms.Button();
            this.btnStaticEnd = new System.Windows.Forms.Button();
            this.btnPowerStart = new System.Windows.Forms.Button();
            this.btnpowerEnd = new System.Windows.Forms.Button();
            this.btnAutoOnFT = new System.Windows.Forms.Button();
            this.btnAutoCloseFT = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.btn_RegWritre = new System.Windows.Forms.Button();
            this.btnEndLuaDrag = new System.Windows.Forms.Button();
            this.btnEndGripper = new System.Windows.Forms.Button();
            this.btnUploadAxleLua = new System.Windows.Forms.Button();
            this.btn_getGripper = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.TestTCP = new System.Windows.Forms.Button();
            this.TestTCP6 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.ExtAxisLaserTracking = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDragStart
            // 
            this.btnDragStart.Location = new System.Drawing.Point(61, 45);
            this.btnDragStart.Name = "btnDragStart";
            this.btnDragStart.Size = new System.Drawing.Size(113, 43);
            this.btnDragStart.TabIndex = 0;
            this.btnDragStart.Text = "开始力传感器辅助拖动";
            this.btnDragStart.UseVisualStyleBackColor = true;
            this.btnDragStart.Click += new System.EventHandler(this.btnDragStart_Click);
            // 
            // btnStopDrag
            // 
            this.btnStopDrag.Location = new System.Drawing.Point(61, 128);
            this.btnStopDrag.Name = "btnStopDrag";
            this.btnStopDrag.Size = new System.Drawing.Size(113, 43);
            this.btnStopDrag.TabIndex = 0;
            this.btnStopDrag.Text = "停止力传感器辅助拖动";
            this.btnStopDrag.UseVisualStyleBackColor = true;
            this.btnStopDrag.Click += new System.EventHandler(this.btnStopDrag_Click);
            // 
            // btnSixStart
            // 
            this.btnSixStart.Location = new System.Drawing.Point(61, 208);
            this.btnSixStart.Name = "btnSixStart";
            this.btnSixStart.Size = new System.Drawing.Size(113, 43);
            this.btnSixStart.TabIndex = 1;
            this.btnSixStart.Text = "六维力辅助拖动开启";
            this.btnSixStart.UseVisualStyleBackColor = true;
            this.btnSixStart.Click += new System.EventHandler(this.btnSixStart_Click);
            // 
            // btnSixEnd
            // 
            this.btnSixEnd.Location = new System.Drawing.Point(61, 287);
            this.btnSixEnd.Name = "btnSixEnd";
            this.btnSixEnd.Size = new System.Drawing.Size(113, 43);
            this.btnSixEnd.TabIndex = 1;
            this.btnSixEnd.Text = "六维力辅助拖动关闭";
            this.btnSixEnd.UseVisualStyleBackColor = true;
            this.btnSixEnd.Click += new System.EventHandler(this.btnSixEnd_Click);
            // 
            // btnGetDragState
            // 
            this.btnGetDragState.Location = new System.Drawing.Point(330, 45);
            this.btnGetDragState.Name = "btnGetDragState";
            this.btnGetDragState.Size = new System.Drawing.Size(113, 53);
            this.btnGetDragState.TabIndex = 2;
            this.btnGetDragState.Text = "获取力传感器拖动开关状态";
            this.btnGetDragState.UseVisualStyleBackColor = true;
            this.btnGetDragState.Click += new System.EventHandler(this.btnGetDragState_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(330, 128);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 43);
            this.button1.TabIndex = 3;
            this.button1.Text = "力传感器下负载设置获取";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnFtAutoZero
            // 
            this.btnFtAutoZero.Location = new System.Drawing.Point(330, 208);
            this.btnFtAutoZero.Name = "btnFtAutoZero";
            this.btnFtAutoZero.Size = new System.Drawing.Size(113, 43);
            this.btnFtAutoZero.TabIndex = 4;
            this.btnFtAutoZero.Text = "力传感器自动校零";
            this.btnFtAutoZero.UseVisualStyleBackColor = true;
            this.btnFtAutoZero.Click += new System.EventHandler(this.btnFtAutoZero_Click);
            // 
            // btnStaticStart
            // 
            this.btnStaticStart.Location = new System.Drawing.Point(569, 45);
            this.btnStaticStart.Name = "btnStaticStart";
            this.btnStaticStart.Size = new System.Drawing.Size(119, 43);
            this.btnStaticStart.TabIndex = 5;
            this.btnStaticStart.Text = "静态碰撞检测开启";
            this.btnStaticStart.UseVisualStyleBackColor = true;
            this.btnStaticStart.Click += new System.EventHandler(this.btnStaticStart_Click);
            // 
            // btnStaticEnd
            // 
            this.btnStaticEnd.Location = new System.Drawing.Point(569, 128);
            this.btnStaticEnd.Name = "btnStaticEnd";
            this.btnStaticEnd.Size = new System.Drawing.Size(119, 43);
            this.btnStaticEnd.TabIndex = 5;
            this.btnStaticEnd.Text = "静态碰撞检测关闭";
            this.btnStaticEnd.UseVisualStyleBackColor = true;
            this.btnStaticEnd.Click += new System.EventHandler(this.btnStaticEnd_Click);
            // 
            // btnPowerStart
            // 
            this.btnPowerStart.Location = new System.Drawing.Point(569, 208);
            this.btnPowerStart.Name = "btnPowerStart";
            this.btnPowerStart.Size = new System.Drawing.Size(119, 43);
            this.btnPowerStart.TabIndex = 6;
            this.btnPowerStart.Text = "功率检测开启";
            this.btnPowerStart.UseVisualStyleBackColor = true;
            this.btnPowerStart.Click += new System.EventHandler(this.btnPowerStart_Click);
            // 
            // btnpowerEnd
            // 
            this.btnpowerEnd.Location = new System.Drawing.Point(569, 287);
            this.btnpowerEnd.Name = "btnpowerEnd";
            this.btnpowerEnd.Size = new System.Drawing.Size(119, 43);
            this.btnpowerEnd.TabIndex = 6;
            this.btnpowerEnd.Text = "功率检测关闭";
            this.btnpowerEnd.UseVisualStyleBackColor = true;
            this.btnpowerEnd.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnAutoOnFT
            // 
            this.btnAutoOnFT.Location = new System.Drawing.Point(771, 45);
            this.btnAutoOnFT.Name = "btnAutoOnFT";
            this.btnAutoOnFT.Size = new System.Drawing.Size(210, 43);
            this.btnAutoOnFT.TabIndex = 7;
            this.btnAutoOnFT.Text = "报错清除后力传感器自动开启";
            this.btnAutoOnFT.UseVisualStyleBackColor = true;
            this.btnAutoOnFT.Click += new System.EventHandler(this.btnAutoOnFT_Click);
            // 
            // btnAutoCloseFT
            // 
            this.btnAutoCloseFT.Location = new System.Drawing.Point(771, 128);
            this.btnAutoCloseFT.Name = "btnAutoCloseFT";
            this.btnAutoCloseFT.Size = new System.Drawing.Size(210, 43);
            this.btnAutoCloseFT.TabIndex = 7;
            this.btnAutoCloseFT.Text = "报错清除后力传感器自动关闭";
            this.btnAutoCloseFT.UseVisualStyleBackColor = true;
            this.btnAutoCloseFT.Click += new System.EventHandler(this.btnAutoCloseFT_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(61, 368);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(113, 44);
            this.button2.TabIndex = 8;
            this.button2.Text = "辅助传感器参数配置获取";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(262, 368);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(200, 88);
            this.button3.TabIndex = 9;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btn_RegWritre
            // 
            this.btn_RegWritre.Location = new System.Drawing.Point(61, 441);
            this.btn_RegWritre.Name = "btn_RegWritre";
            this.btn_RegWritre.Size = new System.Drawing.Size(100, 39);
            this.btn_RegWritre.TabIndex = 10;
            this.btn_RegWritre.Text = "写寄存器";
            this.btn_RegWritre.UseVisualStyleBackColor = true;
            this.btn_RegWritre.Click += new System.EventHandler(this.btn_RegWritre_Click);
            // 
            // btnEndLuaDrag
            // 
            this.btnEndLuaDrag.Location = new System.Drawing.Point(771, 208);
            this.btnEndLuaDrag.Name = "btnEndLuaDrag";
            this.btnEndLuaDrag.Size = new System.Drawing.Size(181, 43);
            this.btnEndLuaDrag.TabIndex = 11;
            this.btnEndLuaDrag.Text = "末端LUA力传感器拖动";
            this.btnEndLuaDrag.UseVisualStyleBackColor = true;
            this.btnEndLuaDrag.Click += new System.EventHandler(this.btnEndLuaDrag_Click);
            // 
            // btnEndGripper
            // 
            this.btnEndGripper.Location = new System.Drawing.Point(771, 287);
            this.btnEndGripper.Name = "btnEndGripper";
            this.btnEndGripper.Size = new System.Drawing.Size(181, 43);
            this.btnEndGripper.TabIndex = 12;
            this.btnEndGripper.Text = "末端LUA夹爪";
            this.btnEndGripper.UseVisualStyleBackColor = true;
            this.btnEndGripper.Click += new System.EventHandler(this.btnEndGripper_Click);
            // 
            // btnUploadAxleLua
            // 
            this.btnUploadAxleLua.Location = new System.Drawing.Point(771, 356);
            this.btnUploadAxleLua.Name = "btnUploadAxleLua";
            this.btnUploadAxleLua.Size = new System.Drawing.Size(181, 44);
            this.btnUploadAxleLua.TabIndex = 13;
            this.btnUploadAxleLua.Text = "上传末端LUA文件";
            this.btnUploadAxleLua.UseVisualStyleBackColor = true;
            this.btnUploadAxleLua.Click += new System.EventHandler(this.btnUploadAxleLua_Click);
            // 
            // btn_getGripper
            // 
            this.btn_getGripper.Location = new System.Drawing.Point(771, 433);
            this.btn_getGripper.Name = "btn_getGripper";
            this.btn_getGripper.Size = new System.Drawing.Size(181, 47);
            this.btn_getGripper.TabIndex = 14;
            this.btn_getGripper.Text = "奇异保护";
            this.btn_getGripper.UseVisualStyleBackColor = true;
            this.btn_getGripper.Click += new System.EventHandler(this.btn_getGripper_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(771, 553);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(152, 39);
            this.button4.TabIndex = 15;
            this.button4.Text = "滤波";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(771, 504);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(152, 31);
            this.button5.TabIndex = 16;
            this.button5.Text = "夹爪状态";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(771, 623);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(152, 38);
            this.button6.TabIndex = 17;
            this.button6.Text = "碰撞守护";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(998, 292);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(116, 33);
            this.button7.TabIndex = 18;
            this.button7.Text = "TestReWeld";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // TestTCP
            // 
            this.TestTCP.Location = new System.Drawing.Point(998, 362);
            this.TestTCP.Name = "TestTCP";
            this.TestTCP.Size = new System.Drawing.Size(116, 33);
            this.TestTCP.TabIndex = 19;
            this.TestTCP.Text = "TestTCP";
            this.TestTCP.UseVisualStyleBackColor = true;
            this.TestTCP.Click += new System.EventHandler(this.TestTCP_Click);
            // 
            // TestTCP6
            // 
            this.TestTCP6.Location = new System.Drawing.Point(998, 423);
            this.TestTCP6.Name = "TestTCP6";
            this.TestTCP6.Size = new System.Drawing.Size(116, 33);
            this.TestTCP6.TabIndex = 20;
            this.TestTCP6.Text = "TestTCP6";
            this.TestTCP6.UseVisualStyleBackColor = true;
            this.TestTCP6.Click += new System.EventHandler(this.TestTCP6_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(998, 480);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(116, 33);
            this.button10.TabIndex = 21;
            this.button10.Text = "稳定性";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // ExtAxisLaserTracking
            // 
            this.ExtAxisLaserTracking.Location = new System.Drawing.Point(998, 553);
            this.ExtAxisLaserTracking.Name = "ExtAxisLaserTracking";
            this.ExtAxisLaserTracking.Size = new System.Drawing.Size(116, 25);
            this.ExtAxisLaserTracking.TabIndex = 22;
            this.ExtAxisLaserTracking.Text = "ExtAxisLaserTracking";
            this.ExtAxisLaserTracking.UseVisualStyleBackColor = true;
            this.ExtAxisLaserTracking.Click += new System.EventHandler(this.ExtAxisLaserTracking_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(998, 601);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(116, 25);
            this.button8.TabIndex = 23;
            this.button8.Text = "轨迹前瞻";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(531, 368);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(157, 88);
            this.button9.TabIndex = 24;
            this.button9.Text = "自定义碰撞检测阈值";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(286, 480);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(157, 88);
            this.button11.TabIndex = 25;
            this.button11.Text = "状态反馈";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(531, 480);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(157, 88);
            this.button12.TabIndex = 26;
            this.button12.Text = "清除错误";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // FrmFT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1142, 718);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.ExtAxisLaserTracking);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.TestTCP6);
            this.Controls.Add(this.TestTCP);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btn_getGripper);
            this.Controls.Add(this.btnUploadAxleLua);
            this.Controls.Add(this.btnEndGripper);
            this.Controls.Add(this.btnEndLuaDrag);
            this.Controls.Add(this.btn_RegWritre);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnAutoCloseFT);
            this.Controls.Add(this.btnAutoOnFT);
            this.Controls.Add(this.btnpowerEnd);
            this.Controls.Add(this.btnPowerStart);
            this.Controls.Add(this.btnStaticEnd);
            this.Controls.Add(this.btnStaticStart);
            this.Controls.Add(this.btnFtAutoZero);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnGetDragState);
            this.Controls.Add(this.btnSixEnd);
            this.Controls.Add(this.btnSixStart);
            this.Controls.Add(this.btnStopDrag);
            this.Controls.Add(this.btnDragStart);
            this.Name = "FrmFT";
            this.Text = "FrmFT";
            this.Load += new System.EventHandler(this.FrmFT_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDragStart;
        private System.Windows.Forms.Button btnStopDrag;
        private System.Windows.Forms.Button btnSixStart;
        private System.Windows.Forms.Button btnSixEnd;
        private System.Windows.Forms.Button btnGetDragState;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnFtAutoZero;
        private System.Windows.Forms.Button btnStaticStart;
        private System.Windows.Forms.Button btnStaticEnd;
        private System.Windows.Forms.Button btnPowerStart;
        private System.Windows.Forms.Button btnpowerEnd;
        private System.Windows.Forms.Button btnAutoOnFT;
        private System.Windows.Forms.Button btnAutoCloseFT;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btn_RegWritre;
        private System.Windows.Forms.Button btnEndLuaDrag;
        private System.Windows.Forms.Button btnEndGripper;
        private System.Windows.Forms.Button btnUploadAxleLua;
        private System.Windows.Forms.Button btn_getGripper;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button TestTCP;
        private System.Windows.Forms.Button TestTCP6;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button ExtAxisLaserTracking;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
    }
}
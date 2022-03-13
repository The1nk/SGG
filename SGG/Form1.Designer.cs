
namespace ScratchPad
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbAdbPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbAdbHost = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbWave6 = new System.Windows.Forms.CheckBox();
            this.tbDeviceRestartInterval = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbAppRestartInterval = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbCollectWeeklies = new System.Windows.Forms.CheckBox();
            this.cbCollectAchievements = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbSellerOther = new System.Windows.Forms.CheckBox();
            this.cbSellerOrbs = new System.Windows.Forms.CheckBox();
            this.cbSellerGems = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbMonitorOther = new System.Windows.Forms.CheckBox();
            this.cbMonitorGems = new System.Windows.Forms.CheckBox();
            this.cbMonitorOrbs = new System.Windows.Forms.CheckBox();
            this.cbMonitorCoins = new System.Windows.Forms.CheckBox();
            this.cbMonitorAttack = new System.Windows.Forms.CheckBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.tbDiscordHookUrl = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnSet = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbAdbPort);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbAdbHost);
            this.groupBox1.Location = new System.Drawing.Point(3, 39);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(296, 80);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ADB";
            // 
            // tbAdbPort
            // 
            this.tbAdbPort.Location = new System.Drawing.Point(219, 29);
            this.tbAdbPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbAdbPort.Name = "tbAdbPort";
            this.tbAdbPort.Size = new System.Drawing.Size(49, 27);
            this.tbAdbPort.TabIndex = 2;
            this.tbAdbPort.Text = "56114";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(201, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(12, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = ":";
            // 
            // tbAdbHost
            // 
            this.tbAdbHost.Location = new System.Drawing.Point(7, 29);
            this.tbAdbHost.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbAdbHost.Name = "tbAdbHost";
            this.tbAdbHost.Size = new System.Drawing.Size(187, 27);
            this.tbAdbHost.TabIndex = 0;
            this.tbAdbHost.Text = "127.0.0.1";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(3, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(296, 29);
            this.btnStart.TabIndex = 11;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Location = new System.Drawing.Point(14, 16);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(339, 708);
            this.panel1.TabIndex = 9;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnStart);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.groupBox4);
            this.flowLayoutPanel1.Controls.Add(this.groupBox2);
            this.flowLayoutPanel1.Controls.Add(this.groupBox3);
            this.flowLayoutPanel1.Controls.Add(this.groupBox9);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(302, 676);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnSet);
            this.groupBox4.Controls.Add(this.cbWave6);
            this.groupBox4.Controls.Add(this.tbDeviceRestartInterval);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.tbAppRestartInterval);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.cbCollectWeeklies);
            this.groupBox4.Controls.Add(this.cbCollectAchievements);
            this.groupBox4.Location = new System.Drawing.Point(3, 127);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Size = new System.Drawing.Size(296, 242);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "General";
            // 
            // cbWave6
            // 
            this.cbWave6.AutoSize = true;
            this.cbWave6.Location = new System.Drawing.Point(6, 87);
            this.cbWave6.Name = "cbWave6";
            this.cbWave6.Size = new System.Drawing.Size(131, 24);
            this.cbWave6.TabIndex = 8;
            this.cbWave6.Text = "Stop at Wave 6";
            this.cbWave6.UseVisualStyleBackColor = true;
            // 
            // tbDeviceRestartInterval
            // 
            this.tbDeviceRestartInterval.Location = new System.Drawing.Point(7, 207);
            this.tbDeviceRestartInterval.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbDeviceRestartInterval.Name = "tbDeviceRestartInterval";
            this.tbDeviceRestartInterval.Size = new System.Drawing.Size(73, 27);
            this.tbDeviceRestartInterval.TabIndex = 7;
            this.tbDeviceRestartInterval.Text = "5";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 183);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(255, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Device Restart Interval (App Restarts)";
            // 
            // tbAppRestartInterval
            // 
            this.tbAppRestartInterval.Location = new System.Drawing.Point(7, 138);
            this.tbAppRestartInterval.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbAppRestartInterval.Name = "tbAppRestartInterval";
            this.tbAppRestartInterval.Size = new System.Drawing.Size(73, 27);
            this.tbAppRestartInterval.TabIndex = 5;
            this.tbAppRestartInterval.Text = "5";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(185, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "App Restart Interval (Runs)";
            // 
            // cbCollectWeeklies
            // 
            this.cbCollectWeeklies.AutoSize = true;
            this.cbCollectWeeklies.Enabled = false;
            this.cbCollectWeeklies.Location = new System.Drawing.Point(6, 58);
            this.cbCollectWeeklies.Name = "cbCollectWeeklies";
            this.cbCollectWeeklies.Size = new System.Drawing.Size(139, 24);
            this.cbCollectWeeklies.TabIndex = 3;
            this.cbCollectWeeklies.Text = "Collect Weeklies";
            this.cbCollectWeeklies.UseVisualStyleBackColor = true;
            // 
            // cbCollectAchievements
            // 
            this.cbCollectAchievements.AutoSize = true;
            this.cbCollectAchievements.Enabled = false;
            this.cbCollectAchievements.Location = new System.Drawing.Point(6, 29);
            this.cbCollectAchievements.Name = "cbCollectAchievements";
            this.cbCollectAchievements.Size = new System.Drawing.Size(173, 24);
            this.cbCollectAchievements.TabIndex = 0;
            this.cbCollectAchievements.Text = "Collect Achievements";
            this.cbCollectAchievements.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbSellerOther);
            this.groupBox2.Controls.Add(this.cbSellerOrbs);
            this.groupBox2.Controls.Add(this.cbSellerGems);
            this.groupBox2.Location = new System.Drawing.Point(3, 377);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(296, 93);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Seller";
            // 
            // cbSellerOther
            // 
            this.cbSellerOther.AutoSize = true;
            this.cbSellerOther.Checked = true;
            this.cbSellerOther.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSellerOther.Location = new System.Drawing.Point(6, 57);
            this.cbSellerOther.Name = "cbSellerOther";
            this.cbSellerOther.Size = new System.Drawing.Size(224, 24);
            this.cbSellerOther.TabIndex = 2;
            this.cbSellerOther.Text = "Other (Summoning Stones, ?)";
            this.cbSellerOther.UseVisualStyleBackColor = true;
            // 
            // cbSellerOrbs
            // 
            this.cbSellerOrbs.AutoSize = true;
            this.cbSellerOrbs.Checked = true;
            this.cbSellerOrbs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSellerOrbs.Location = new System.Drawing.Point(81, 27);
            this.cbSellerOrbs.Name = "cbSellerOrbs";
            this.cbSellerOrbs.Size = new System.Drawing.Size(62, 24);
            this.cbSellerOrbs.TabIndex = 1;
            this.cbSellerOrbs.Text = "Orbs";
            this.cbSellerOrbs.UseVisualStyleBackColor = true;
            // 
            // cbSellerGems
            // 
            this.cbSellerGems.AutoSize = true;
            this.cbSellerGems.Checked = true;
            this.cbSellerGems.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSellerGems.Location = new System.Drawing.Point(7, 27);
            this.cbSellerGems.Name = "cbSellerGems";
            this.cbSellerGems.Size = new System.Drawing.Size(68, 24);
            this.cbSellerGems.TabIndex = 0;
            this.cbSellerGems.Text = "Gems";
            this.cbSellerGems.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbMonitorOther);
            this.groupBox3.Controls.Add(this.cbMonitorGems);
            this.groupBox3.Controls.Add(this.cbMonitorOrbs);
            this.groupBox3.Controls.Add(this.cbMonitorCoins);
            this.groupBox3.Controls.Add(this.cbMonitorAttack);
            this.groupBox3.Location = new System.Drawing.Point(3, 478);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Size = new System.Drawing.Size(296, 91);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Monitor";
            // 
            // cbMonitorOther
            // 
            this.cbMonitorOther.AutoSize = true;
            this.cbMonitorOther.Enabled = false;
            this.cbMonitorOther.Location = new System.Drawing.Point(6, 57);
            this.cbMonitorOther.Name = "cbMonitorOther";
            this.cbMonitorOther.Size = new System.Drawing.Size(224, 24);
            this.cbMonitorOther.TabIndex = 3;
            this.cbMonitorOther.Text = "Other (Summoning Stones, ?)";
            this.cbMonitorOther.UseVisualStyleBackColor = true;
            // 
            // cbMonitorGems
            // 
            this.cbMonitorGems.AutoSize = true;
            this.cbMonitorGems.Enabled = false;
            this.cbMonitorGems.Location = new System.Drawing.Point(7, 27);
            this.cbMonitorGems.Name = "cbMonitorGems";
            this.cbMonitorGems.Size = new System.Drawing.Size(68, 24);
            this.cbMonitorGems.TabIndex = 4;
            this.cbMonitorGems.Text = "Gems";
            this.cbMonitorGems.UseVisualStyleBackColor = true;
            // 
            // cbMonitorOrbs
            // 
            this.cbMonitorOrbs.AutoSize = true;
            this.cbMonitorOrbs.Enabled = false;
            this.cbMonitorOrbs.Location = new System.Drawing.Point(81, 27);
            this.cbMonitorOrbs.Name = "cbMonitorOrbs";
            this.cbMonitorOrbs.Size = new System.Drawing.Size(62, 24);
            this.cbMonitorOrbs.TabIndex = 3;
            this.cbMonitorOrbs.Text = "Orbs";
            this.cbMonitorOrbs.UseVisualStyleBackColor = true;
            // 
            // cbMonitorCoins
            // 
            this.cbMonitorCoins.AutoSize = true;
            this.cbMonitorCoins.Enabled = false;
            this.cbMonitorCoins.Location = new System.Drawing.Point(228, 27);
            this.cbMonitorCoins.Name = "cbMonitorCoins";
            this.cbMonitorCoins.Size = new System.Drawing.Size(67, 24);
            this.cbMonitorCoins.TabIndex = 1;
            this.cbMonitorCoins.Text = "Coins";
            this.cbMonitorCoins.UseVisualStyleBackColor = true;
            // 
            // cbMonitorAttack
            // 
            this.cbMonitorAttack.AutoSize = true;
            this.cbMonitorAttack.Enabled = false;
            this.cbMonitorAttack.Location = new System.Drawing.Point(149, 27);
            this.cbMonitorAttack.Name = "cbMonitorAttack";
            this.cbMonitorAttack.Size = new System.Drawing.Size(73, 24);
            this.cbMonitorAttack.TabIndex = 0;
            this.cbMonitorAttack.Text = "Attack";
            this.cbMonitorAttack.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.tbDiscordHookUrl);
            this.groupBox9.Controls.Add(this.label11);
            this.groupBox9.Location = new System.Drawing.Point(3, 577);
            this.groupBox9.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox9.Size = new System.Drawing.Size(296, 95);
            this.groupBox9.TabIndex = 10;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "WebHooks";
            // 
            // tbDiscordHookUrl
            // 
            this.tbDiscordHookUrl.Location = new System.Drawing.Point(7, 45);
            this.tbDiscordHookUrl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbDiscordHookUrl.Name = "tbDiscordHookUrl";
            this.tbDiscordHookUrl.Size = new System.Drawing.Size(282, 27);
            this.tbDiscordHookUrl.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 21);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(100, 20);
            this.label11.TabIndex = 0;
            this.label11.Text = "Discord Hook";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(349, 16);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(649, 708);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(196, 27);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(94, 29);
            this.btnSet.TabIndex = 10;
            this.btnSet.Text = "Set ...";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1013, 738);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Summoner\'s Greed\'s Greed";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbAdbPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbAdbHost;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.TextBox tbDiscordHookUrl;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbSellerOther;
        private System.Windows.Forms.CheckBox cbSellerOrbs;
        private System.Windows.Forms.CheckBox cbSellerGems;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbMonitorGems;
        private System.Windows.Forms.CheckBox cbMonitorOrbs;
        private System.Windows.Forms.CheckBox cbMonitorCoins;
        private System.Windows.Forms.CheckBox cbMonitorAttack;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbCollectWeeklies;
        private System.Windows.Forms.CheckBox cbCollectAchievements;
        private System.Windows.Forms.TextBox tbAppRestartInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDeviceRestartInterval;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbMonitorOther;
        private System.Windows.Forms.CheckBox cbWave6;
        private System.Windows.Forms.Button btnSet;
    }
}


namespace OpcUaSample.Gui.Client
{
    partial class Client
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
            TxtLog = new TextBox();
            BtnConnect = new Button();
            BtnRead = new Button();
            BtnSubscribe = new Button();
            BtnWrite = new Button();
            BtnUnsubscribe = new Button();
            BtnDisconnect = new Button();
            SuspendLayout();
            // 
            // TxtLog
            // 
            TxtLog.Font = new Font("맑은 고딕", 10F);
            TxtLog.Location = new Point(26, 22);
            TxtLog.Multiline = true;
            TxtLog.Name = "TxtLog";
            TxtLog.ScrollBars = ScrollBars.Vertical;
            TxtLog.Size = new Size(480, 352);
            TxtLog.TabIndex = 1;
            // 
            // BtnConnect
            // 
            BtnConnect.Location = new Point(26, 393);
            BtnConnect.Name = "BtnConnect";
            BtnConnect.Size = new Size(105, 51);
            BtnConnect.TabIndex = 2;
            BtnConnect.Text = "Connect";
            BtnConnect.UseVisualStyleBackColor = true;
            BtnConnect.Click += BtnConnect_Click;
            // 
            // BtnRead
            // 
            BtnRead.Location = new Point(401, 393);
            BtnRead.Name = "BtnRead";
            BtnRead.Size = new Size(105, 51);
            BtnRead.TabIndex = 3;
            BtnRead.Text = "Read";
            BtnRead.UseVisualStyleBackColor = true;
            BtnRead.Click += BtnRead_Click;
            // 
            // BtnSubscribe
            // 
            BtnSubscribe.Location = new Point(290, 393);
            BtnSubscribe.Name = "BtnSubscribe";
            BtnSubscribe.Size = new Size(105, 51);
            BtnSubscribe.TabIndex = 4;
            BtnSubscribe.Text = "Subscribe";
            BtnSubscribe.UseVisualStyleBackColor = true;
            BtnSubscribe.Click += BtnSubscribe_Click;
            // 
            // BtnWrite
            // 
            BtnWrite.Location = new Point(401, 450);
            BtnWrite.Name = "BtnWrite";
            BtnWrite.Size = new Size(105, 51);
            BtnWrite.TabIndex = 5;
            BtnWrite.Text = "Write";
            BtnWrite.UseVisualStyleBackColor = true;
            BtnWrite.Click += BtnWrite_Click;
            // 
            // BtnUnsubscribe
            // 
            BtnUnsubscribe.Enabled = false;
            BtnUnsubscribe.Location = new Point(290, 450);
            BtnUnsubscribe.Name = "BtnUnsubscribe";
            BtnUnsubscribe.Size = new Size(105, 51);
            BtnUnsubscribe.TabIndex = 6;
            BtnUnsubscribe.Text = "Unsubscribe";
            BtnUnsubscribe.UseVisualStyleBackColor = true;
            BtnUnsubscribe.Click += BtnUnsubscribe_Click;
            // 
            // BtnDisconnect
            // 
            BtnDisconnect.Enabled = false;
            BtnDisconnect.Location = new Point(26, 450);
            BtnDisconnect.Name = "BtnDisconnect";
            BtnDisconnect.Size = new Size(105, 51);
            BtnDisconnect.TabIndex = 7;
            BtnDisconnect.Text = "Disconnect";
            BtnDisconnect.UseVisualStyleBackColor = true;
            BtnDisconnect.Click += BtnDisconnect_Click;
            // 
            // Client
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(535, 524);
            Controls.Add(BtnDisconnect);
            Controls.Add(BtnUnsubscribe);
            Controls.Add(BtnWrite);
            Controls.Add(BtnSubscribe);
            Controls.Add(BtnRead);
            Controls.Add(BtnConnect);
            Controls.Add(TxtLog);
            Name = "Client";
            Text = "Client";
            FormClosing += OnFormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox TxtLog;
        private Button BtnConnect;
        private Button BtnRead;
        private Button BtnSubscribe;
        private Button BtnWrite;
        private Button BtnUnsubscribe;
        private Button BtnDisconnect;
    }
}

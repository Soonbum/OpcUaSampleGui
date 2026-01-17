namespace OpcUaSample.Gui.Server
{
    partial class Server
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
            lblServerStatus = new Label();
            BtnStartServer = new Button();
            BtnStopServer = new Button();
            SuspendLayout();
            // 
            // lblServerStatus
            // 
            lblServerStatus.AutoSize = true;
            lblServerStatus.Location = new Point(28, 30);
            lblServerStatus.Name = "lblServerStatus";
            lblServerStatus.Size = new Size(73, 15);
            lblServerStatus.TabIndex = 0;
            lblServerStatus.Text = "ServerStatus";
            // 
            // BtnStartServer
            // 
            BtnStartServer.Location = new Point(28, 85);
            BtnStartServer.Name = "BtnStartServer";
            BtnStartServer.Size = new Size(130, 53);
            BtnStartServer.TabIndex = 1;
            BtnStartServer.Text = "StartServer";
            BtnStartServer.UseVisualStyleBackColor = true;
            BtnStartServer.Click += BtnStartServer_Click;
            // 
            // BtnStopServer
            // 
            BtnStopServer.Location = new Point(183, 85);
            BtnStopServer.Name = "BtnStopServer";
            BtnStopServer.Size = new Size(130, 53);
            BtnStopServer.TabIndex = 2;
            BtnStopServer.Text = "StopServer";
            BtnStopServer.UseVisualStyleBackColor = true;
            BtnStopServer.Click += BtnStopServer_Click;
            // 
            // Server
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(343, 163);
            Controls.Add(BtnStopServer);
            Controls.Add(BtnStartServer);
            Controls.Add(lblServerStatus);
            Name = "Server";
            Text = "Server";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblServerStatus;
        private Button BtnStartServer;
        private Button BtnStopServer;
    }
}

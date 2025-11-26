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
            btnStartServer = new Button();
            btnStopServer = new Button();
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
            // btnStartServer
            // 
            btnStartServer.Location = new Point(28, 85);
            btnStartServer.Name = "btnStartServer";
            btnStartServer.Size = new Size(130, 53);
            btnStartServer.TabIndex = 1;
            btnStartServer.Text = "StartServer";
            btnStartServer.UseVisualStyleBackColor = true;
            btnStartServer.Click += btnStartServer_Click;
            // 
            // btnStopServer
            // 
            btnStopServer.Location = new Point(183, 85);
            btnStopServer.Name = "btnStopServer";
            btnStopServer.Size = new Size(130, 53);
            btnStopServer.TabIndex = 2;
            btnStopServer.Text = "StopServer";
            btnStopServer.UseVisualStyleBackColor = true;
            btnStopServer.Click += btnStopServer_Click;
            // 
            // Server
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(343, 163);
            Controls.Add(btnStopServer);
            Controls.Add(btnStartServer);
            Controls.Add(lblServerStatus);
            Name = "Server";
            Text = "Server";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblServerStatus;
        private Button btnStartServer;
        private Button btnStopServer;
    }
}

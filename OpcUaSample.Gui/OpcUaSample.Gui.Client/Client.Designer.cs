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
            BtnRead = new Button();
            txtResult = new TextBox();
            SuspendLayout();
            // 
            // BtnRead
            // 
            BtnRead.Location = new Point(63, 71);
            BtnRead.Name = "BtnRead";
            BtnRead.Size = new Size(130, 53);
            BtnRead.TabIndex = 0;
            BtnRead.Text = "Read";
            BtnRead.UseVisualStyleBackColor = true;
            BtnRead.Click += BtnRead_Click;
            // 
            // txtResult
            // 
            txtResult.Font = new Font("맑은 고딕", 10F);
            txtResult.Location = new Point(26, 22);
            txtResult.Name = "txtResult";
            txtResult.Size = new Size(203, 25);
            txtResult.TabIndex = 1;
            // 
            // Client
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(257, 145);
            Controls.Add(txtResult);
            Controls.Add(BtnRead);
            Name = "Client";
            Text = "Client";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnRead;
        private TextBox txtResult;
    }
}

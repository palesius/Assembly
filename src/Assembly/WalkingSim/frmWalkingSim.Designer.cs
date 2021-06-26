namespace Assembly
{
    partial class frmStatus
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
            this.pbMap = new System.Windows.Forms.ProgressBar();
            this.lblMap = new System.Windows.Forms.Label();
            this.lblTag = new System.Windows.Forms.Label();
            this.pbTag = new System.Windows.Forms.ProgressBar();
            this.txtMap = new System.Windows.Forms.TextBox();
            this.txtTag = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // pbMap
            // 
            this.pbMap.Location = new System.Drawing.Point(46, 12);
            this.pbMap.Name = "pbMap";
            this.pbMap.Size = new System.Drawing.Size(146, 20);
            this.pbMap.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbMap.TabIndex = 0;
            // 
            // lblMap
            // 
            this.lblMap.AutoSize = true;
            this.lblMap.Location = new System.Drawing.Point(12, 15);
            this.lblMap.Name = "lblMap";
            this.lblMap.Size = new System.Drawing.Size(28, 13);
            this.lblMap.TabIndex = 1;
            this.lblMap.Text = "Map";
            // 
            // lblTag
            // 
            this.lblTag.AutoSize = true;
            this.lblTag.Location = new System.Drawing.Point(12, 44);
            this.lblTag.Name = "lblTag";
            this.lblTag.Size = new System.Drawing.Size(26, 13);
            this.lblTag.TabIndex = 2;
            this.lblTag.Text = "Tag";
            // 
            // pbTag
            // 
            this.pbTag.Location = new System.Drawing.Point(46, 41);
            this.pbTag.Name = "pbTag";
            this.pbTag.Size = new System.Drawing.Size(146, 20);
            this.pbTag.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbTag.TabIndex = 3;
            // 
            // txtMap
            // 
            this.txtMap.Location = new System.Drawing.Point(198, 12);
            this.txtMap.Name = "txtMap";
            this.txtMap.ReadOnly = true;
            this.txtMap.Size = new System.Drawing.Size(600, 20);
            this.txtMap.TabIndex = 4;
            // 
            // txtTag
            // 
            this.txtTag.Location = new System.Drawing.Point(198, 41);
            this.txtTag.Name = "txtTag";
            this.txtTag.ReadOnly = true;
            this.txtTag.Size = new System.Drawing.Size(600, 20);
            this.txtTag.TabIndex = 5;
            // 
            // frmStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 71);
            this.ControlBox = false;
            this.Controls.Add(this.txtTag);
            this.Controls.Add(this.txtMap);
            this.Controls.Add(this.pbTag);
            this.Controls.Add(this.lblTag);
            this.Controls.Add(this.lblMap);
            this.Controls.Add(this.pbMap);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmStatus";
            this.Text = "Walking Sim Status";
            this.Load += new System.EventHandler(this.frmStatus_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbMap;
        private System.Windows.Forms.Label lblMap;
        private System.Windows.Forms.Label lblTag;
        private System.Windows.Forms.ProgressBar pbTag;
        private System.Windows.Forms.TextBox txtMap;
        private System.Windows.Forms.TextBox txtTag;
    }
}
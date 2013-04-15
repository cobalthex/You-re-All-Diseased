namespace MapEditor
{
    partial class HelpDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpDialog));
            this.titleText = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.mainText = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // titleText
            // 
            resources.ApplyResources(this.titleText, "titleText");
            this.titleText.Name = "titleText";
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // mainText
            // 
            resources.ApplyResources(this.mainText, "mainText");
            this.mainText.BackColor = System.Drawing.SystemColors.Control;
            this.mainText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mainText.Name = "mainText";
            this.mainText.ReadOnly = true;
            // 
            // HelpDialog
            // 
            this.AcceptButton = this.CloseButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainText);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.titleText);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HelpDialog";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleText;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.RichTextBox mainText;
    }
}
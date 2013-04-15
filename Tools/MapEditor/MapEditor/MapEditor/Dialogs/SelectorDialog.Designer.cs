namespace MapEditor
{
    partial class SelectorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectorDialog));
            this.tileTool = new System.Windows.Forms.RadioButton();
            this.collisionTool = new System.Windows.Forms.RadioButton();
            this.entityTool = new System.Windows.Forms.RadioButton();
            this.newButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.mucusTool = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // tileTool
            // 
            resources.ApplyResources(this.tileTool, "tileTool");
            this.tileTool.Checked = true;
            this.tileTool.Name = "tileTool";
            this.tileTool.TabStop = true;
            this.tileTool.UseVisualStyleBackColor = true;
            this.tileTool.CheckedChanged += new System.EventHandler(this.tileTool_CheckedChanged);
            // 
            // collisionTool
            // 
            resources.ApplyResources(this.collisionTool, "collisionTool");
            this.collisionTool.Name = "collisionTool";
            this.collisionTool.UseVisualStyleBackColor = true;
            this.collisionTool.CheckedChanged += new System.EventHandler(this.collisionTool_CheckedChanged);
            // 
            // entityTool
            // 
            resources.ApplyResources(this.entityTool, "entityTool");
            this.entityTool.Name = "entityTool";
            this.entityTool.UseVisualStyleBackColor = true;
            this.entityTool.CheckedChanged += new System.EventHandler(this.entityTool_CheckedChanged);
            // 
            // newButton
            // 
            resources.ApplyResources(this.newButton, "newButton");
            this.newButton.Name = "newButton";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // openButton
            // 
            resources.ApplyResources(this.openButton, "openButton");
            this.openButton.Name = "openButton";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // saveButton
            // 
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.Name = "saveButton";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // mucusTool
            // 
            resources.ApplyResources(this.mucusTool, "mucusTool");
            this.mucusTool.Name = "mucusTool";
            this.mucusTool.UseVisualStyleBackColor = true;
            this.mucusTool.CheckedChanged += new System.EventHandler(this.mucusPlacement_CheckedChanged);
            // 
            // SelectorDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mucusTool);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.newButton);
            this.Controls.Add(this.entityTool);
            this.Controls.Add(this.collisionTool);
            this.Controls.Add(this.tileTool);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectorDialog";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.SelectorDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button saveButton;
        public System.Windows.Forms.RadioButton tileTool;
        public System.Windows.Forms.RadioButton collisionTool;
        public System.Windows.Forms.RadioButton entityTool;
        public System.Windows.Forms.RadioButton mucusTool;
    }
}
namespace MapEditor
{
    partial class LoadTilesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadTilesDialog));
            this.backColorButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.openTilesButton = new System.Windows.Forms.Button();
            this.tilesetBox = new System.Windows.Forms.TextBox();
            this.backImageBox = new System.Windows.Forms.TextBox();
            this.backImageOpen = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.LoadButton = new System.Windows.Forms.Button();
            this.imageLoad = new System.Windows.Forms.OpenFileDialog();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.mapHeight = new MapEditor.NumericTextBox();
            this.mapWidth = new MapEditor.NumericTextBox();
            this.tileHeight = new MapEditor.NumericTextBox();
            this.tileWidth = new MapEditor.NumericTextBox();
            this.entListBox = new System.Windows.Forms.TextBox();
            this.entsOpen = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // backColorButton
            // 
            this.backColorButton.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.backColorButton, "backColorButton");
            this.backColorButton.Name = "backColorButton";
            this.backColorButton.UseVisualStyleBackColor = false;
            this.backColorButton.Click += new System.EventHandler(this.backColorButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // openTilesButton
            // 
            resources.ApplyResources(this.openTilesButton, "openTilesButton");
            this.openTilesButton.Name = "openTilesButton";
            this.openTilesButton.UseVisualStyleBackColor = true;
            this.openTilesButton.Click += new System.EventHandler(this.openTilesButton_Click);
            // 
            // tilesetBox
            // 
            resources.ApplyResources(this.tilesetBox, "tilesetBox");
            this.tilesetBox.Name = "tilesetBox";
            // 
            // backImageBox
            // 
            resources.ApplyResources(this.backImageBox, "backImageBox");
            this.backImageBox.Name = "backImageBox";
            // 
            // backImageOpen
            // 
            resources.ApplyResources(this.backImageOpen, "backImageOpen");
            this.backImageOpen.Name = "backImageOpen";
            this.backImageOpen.UseVisualStyleBackColor = true;
            this.backImageOpen.Click += new System.EventHandler(this.backImageOpen_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // LoadButton
            // 
            resources.ApplyResources(this.LoadButton, "LoadButton");
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.UseVisualStyleBackColor = true;
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // imageLoad
            // 
            resources.ApplyResources(this.imageLoad, "imageLoad");
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // mapHeight
            // 
            this.mapHeight.AllowSpace = false;
            resources.ApplyResources(this.mapHeight, "mapHeight");
            this.mapHeight.Name = "mapHeight";
            // 
            // mapWidth
            // 
            this.mapWidth.AllowSpace = false;
            resources.ApplyResources(this.mapWidth, "mapWidth");
            this.mapWidth.Name = "mapWidth";
            // 
            // tileHeight
            // 
            this.tileHeight.AllowSpace = false;
            resources.ApplyResources(this.tileHeight, "tileHeight");
            this.tileHeight.Name = "tileHeight";
            // 
            // tileWidth
            // 
            this.tileWidth.AllowSpace = false;
            resources.ApplyResources(this.tileWidth, "tileWidth");
            this.tileWidth.Name = "tileWidth";
            // 
            // entListBox
            // 
            resources.ApplyResources(this.entListBox, "entListBox");
            this.entListBox.Name = "entListBox";
            // 
            // entsOpen
            // 
            resources.ApplyResources(this.entsOpen, "entsOpen");
            this.entsOpen.Name = "entsOpen";
            this.entsOpen.UseVisualStyleBackColor = true;
            this.entsOpen.Click += new System.EventHandler(this.entsOpen_Click);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // LoadTilesDialog
            // 
            this.AcceptButton = this.LoadButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.entListBox);
            this.Controls.Add(this.entsOpen);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.mapHeight);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.mapWidth);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.LoadButton);
            this.Controls.Add(this.tileHeight);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tileWidth);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.backImageBox);
            this.Controls.Add(this.backImageOpen);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tilesetBox);
            this.Controls.Add(this.openTilesButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.backColorButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadTilesDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button backColorButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button openTilesButton;
        private System.Windows.Forms.TextBox tilesetBox;
        private System.Windows.Forms.TextBox backImageBox;
        private System.Windows.Forms.Button backImageOpen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private MapEditor.NumericTextBox tileWidth;
        private MapEditor.NumericTextBox tileHeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.OpenFileDialog imageLoad;
        private NumericTextBox mapHeight;
        private System.Windows.Forms.Label label5;
        private NumericTextBox mapWidth;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox entListBox;
        private System.Windows.Forms.Button entsOpen;
        private System.Windows.Forms.Label label7;
    }
}
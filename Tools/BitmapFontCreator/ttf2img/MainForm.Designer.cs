//-----------------------------------------------------------------------------
// MainForm.Designer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

namespace TrueTypeConverter
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.FontName = new System.Windows.Forms.ComboBox();
            this.FontStyle = new System.Windows.Forms.ComboBox();
            this.FontSize = new System.Windows.Forms.ComboBox();
            this.Antialias = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Sample = new System.Windows.Forms.Label();
            this.MaxChar = new System.Windows.Forms.TextBox();
            this.MinChar = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.previewText = new System.Windows.Forms.TextBox();
            this.customFont = new System.Windows.Forms.LinkLabel();
            this.ExportXNA = new System.Windows.Forms.Button();
            this.Export = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.fontColor = new System.Windows.Forms.Button();
            this.ExportClassic = new System.Windows.Forms.Button();
            this.separation = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.separation)).BeginInit();
            this.SuspendLayout();
            // 
            // FontName
            // 
            this.FontName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FontName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.FontName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.FontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.FontName.FormattingEnabled = true;
            this.FontName.Location = new System.Drawing.Point(15, 30);
            this.FontName.Name = "FontName";
            this.FontName.Size = new System.Drawing.Size(189, 176);
            this.FontName.TabIndex = 0;
            this.FontName.SelectedIndexChanged += new System.EventHandler(this.FontName_SelectedIndexChanged);
            // 
            // FontStyle
            // 
            this.FontStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FontStyle.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.FontStyle.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.FontStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.FontStyle.FormattingEnabled = true;
            this.FontStyle.Items.AddRange(new object[] {
            "Regular",
            "Italic",
            "Bold",
            "Bold, Italic"});
            this.FontStyle.Location = new System.Drawing.Point(218, 30);
            this.FontStyle.Name = "FontStyle";
            this.FontStyle.Size = new System.Drawing.Size(80, 176);
            this.FontStyle.TabIndex = 2;
            this.FontStyle.Text = "Regular";
            this.FontStyle.SelectedIndexChanged += new System.EventHandler(this.FontStyle_SelectedIndexChanged);
            // 
            // FontSize
            // 
            this.FontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.FontSize.FormattingEnabled = true;
            this.FontSize.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "22",
            "23",
            "24",
            "26",
            "28",
            "32",
            "36",
            "42",
            "48",
            "54",
            "64",
            "72",
            "96",
            "128",
            "144"});
            this.FontSize.Location = new System.Drawing.Point(312, 30);
            this.FontSize.Name = "FontSize";
            this.FontSize.Size = new System.Drawing.Size(49, 176);
            this.FontSize.TabIndex = 3;
            this.FontSize.Text = "16";
            this.FontSize.SelectedIndexChanged += new System.EventHandler(this.FontSize_SelectedIndexChanged);
            this.FontSize.TextUpdate += new System.EventHandler(this.FontSize_TextUpdate);
            // 
            // Antialias
            // 
            this.Antialias.AutoSize = true;
            this.Antialias.Checked = true;
            this.Antialias.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Antialias.Location = new System.Drawing.Point(178, 233);
            this.Antialias.Name = "Antialias";
            this.Antialias.Size = new System.Drawing.Size(79, 17);
            this.Antialias.TabIndex = 7;
            this.Antialias.Text = "&Antialiasing";
            this.Antialias.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "M&in char";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(215, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Font s&tyle";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(310, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "&Size";
            // 
            // Sample
            // 
            this.Sample.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Sample.AutoEllipsis = true;
            this.Sample.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Sample.Location = new System.Drawing.Point(12, 280);
            this.Sample.Name = "Sample";
            this.Sample.Size = new System.Drawing.Size(354, 187);
            this.Sample.TabIndex = 20;
            this.Sample.Text = "The quick brown fox jumped over the LAZY camel";
            this.Sample.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MaxChar
            // 
            this.MaxChar.Location = new System.Drawing.Point(71, 231);
            this.MaxChar.Name = "MaxChar";
            this.MaxChar.Size = new System.Drawing.Size(50, 20);
            this.MaxChar.TabIndex = 5;
            this.MaxChar.Text = "0x7F";
            // 
            // MinChar
            // 
            this.MinChar.Location = new System.Drawing.Point(15, 231);
            this.MinChar.Name = "MinChar";
            this.MinChar.Size = new System.Drawing.Size(50, 20);
            this.MinChar.TabIndex = 4;
            this.MinChar.Text = "0x20";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(68, 215);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Max c&har";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "&Font";
            // 
            // previewText
            // 
            this.previewText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewText.ImeMode = System.Windows.Forms.ImeMode.On;
            this.previewText.Location = new System.Drawing.Point(15, 257);
            this.previewText.Name = "previewText";
            this.previewText.Size = new System.Drawing.Size(346, 20);
            this.previewText.TabIndex = 9;
            this.previewText.Text = "The quick brown fox jumped over the LAZY camel";
            this.previewText.TextChanged += new System.EventHandler(this.previewText_TextChanged);
            // 
            // customFont
            // 
            this.customFont.ActiveLinkColor = System.Drawing.SystemColors.ActiveCaption;
            this.customFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.customFont.AutoSize = true;
            this.customFont.LinkColor = System.Drawing.SystemColors.ControlText;
            this.customFont.Location = new System.Drawing.Point(124, 14);
            this.customFont.Name = "customFont";
            this.customFont.Size = new System.Drawing.Size(80, 13);
            this.customFont.TabIndex = 1;
            this.customFont.TabStop = true;
            this.customFont.Text = "Select Font File";
            this.customFont.VisitedLinkColor = System.Drawing.SystemColors.ControlText;
            this.customFont.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.customFont_LinkClicked);
            // 
            // ExportXNA
            // 
            this.ExportXNA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ExportXNA.Location = new System.Drawing.Point(210, 481);
            this.ExportXNA.Name = "ExportXNA";
            this.ExportXNA.Size = new System.Drawing.Size(75, 23);
            this.ExportXNA.TabIndex = 11;
            this.ExportXNA.Text = "Export &XNA";
            this.ExportXNA.UseVisualStyleBackColor = true;
            this.ExportXNA.Click += new System.EventHandler(this.ExportXNA_Click);
            // 
            // Export
            // 
            this.Export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Export.Location = new System.Drawing.Point(291, 481);
            this.Export.Name = "Export";
            this.Export.Size = new System.Drawing.Size(75, 23);
            this.Export.TabIndex = 12;
            this.Export.Text = "&Export";
            this.Export.UseVisualStyleBackColor = true;
            this.Export.Click += new System.EventHandler(this.Export_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(258, 215);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Font &color";
            // 
            // fontColor
            // 
            this.fontColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fontColor.BackColor = System.Drawing.Color.Black;
            this.fontColor.Location = new System.Drawing.Point(261, 231);
            this.fontColor.Name = "fontColor";
            this.fontColor.Size = new System.Drawing.Size(100, 20);
            this.fontColor.TabIndex = 8;
            this.fontColor.UseVisualStyleBackColor = false;
            this.fontColor.Click += new System.EventHandler(this.fontColor_Click);
            // 
            // ExportClassic
            // 
            this.ExportClassic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ExportClassic.Location = new System.Drawing.Point(118, 481);
            this.ExportClassic.Name = "ExportClassic";
            this.ExportClassic.Size = new System.Drawing.Size(86, 23);
            this.ExportClassic.TabIndex = 10;
            this.ExportClassic.Text = "Export &Classic";
            this.ExportClassic.UseVisualStyleBackColor = true;
            this.ExportClassic.Click += new System.EventHandler(this.ExportClassic_Click);
            // 
            // separation
            // 
            this.separation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separation.Location = new System.Drawing.Point(127, 231);
            this.separation.Name = "separation";
            this.separation.Size = new System.Drawing.Size(43, 20);
            this.separation.TabIndex = 6;
            this.separation.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(124, 215);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Padding";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 516);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.separation);
            this.Controls.Add(this.ExportClassic);
            this.Controls.Add(this.fontColor);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Export);
            this.Controls.Add(this.customFont);
            this.Controls.Add(this.previewText);
            this.Controls.Add(this.MinChar);
            this.Controls.Add(this.MaxChar);
            this.Controls.Add(this.ExportXNA);
            this.Controls.Add(this.Sample);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Antialias);
            this.Controls.Add(this.FontSize);
            this.Controls.Add(this.FontStyle);
            this.Controls.Add(this.FontName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bitmap Font Creator";
            ((System.ComponentModel.ISupportInitialize)(this.separation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox FontName;
        private System.Windows.Forms.ComboBox FontStyle;
        private System.Windows.Forms.ComboBox FontSize;
        private System.Windows.Forms.CheckBox Antialias;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label Sample;
        private System.Windows.Forms.TextBox MaxChar;
        private System.Windows.Forms.TextBox MinChar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox previewText;
        private System.Windows.Forms.LinkLabel customFont;
        private System.Windows.Forms.Button ExportXNA;
        private System.Windows.Forms.Button Export;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button fontColor;
        private System.Windows.Forms.Button ExportClassic;
        private System.Windows.Forms.NumericUpDown separation;
        private System.Windows.Forms.Label label7;
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapEditor
{
    public partial class OptionsDialog : Form
    {
        public OptionsDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create a new options Dialog
        /// </summary>
        /// <param name="Title">Title of the Dialog</param>
        /// <param name="Caption">Description of what this text box is used for</param>
        /// <param name="Text">Text pre-entered into the edit box</param>
        /// <param name="Picture">An optional picture (leave null for none)</param>
        public OptionsDialog(string Title, string Caption, string Text, Image Picture)
        {
            InitializeComponent();

            this.Text = Title;
            textLabel.Text = Caption;
            inputBox.Text = Text;
            if (Picture != null)
                imageBox.Image = Picture;
        }

        /// <summary>
        /// Get the text of the text box
        /// </summary>
        public string Result
        {
            get { return inputBox.Text; }
        }

        private void OptionsDialog_Load(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
        }
    }
}

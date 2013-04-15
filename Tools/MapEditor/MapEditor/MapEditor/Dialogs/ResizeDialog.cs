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
    public partial class ResizeDialog : Form
    {
        public ResizeDialog()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult == System.Windows.Forms.DialogResult.OK && width.Text == "" || height.Text == "")
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}

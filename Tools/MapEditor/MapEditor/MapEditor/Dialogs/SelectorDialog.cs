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
    public partial class SelectorDialog : Form
    {
        /// <summary>
        /// owner of this dialog
        /// </summary>
        Game owner;

        /// <summary>
        /// Create a new tool selector dialog
        /// </summary>
        /// <param name="Owner">The tool screen</param>
        public SelectorDialog(Game Owner)
        {
            owner = Owner;
            InitializeComponent();
        }

        private void SelectorDialog_Load(object sender, EventArgs e)
        {
            this.SetDesktopLocation(Location.X - (Width >> 1) - 12 - (owner.Window.ClientBounds.Width >> 1), 
                Location.Y - (owner.Window.ClientBounds.Height >> 1) + (Height >> 1) + 6);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Are you sure you wish to exit?", "Exit",
                System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                owner.gSM.GameExit(null);
            else
                e.Cancel = true;
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            HelpDialog hD = new HelpDialog();
            hD.ShowDialog();
            e.Cancel = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            owner.sFileDlg.FileName = owner.map.filename;
            System.Windows.Forms.DialogResult dR = owner.sFileDlg.ShowDialog();
            if (dR == System.Windows.Forms.DialogResult.OK)
            {
                owner.map.Save(owner.sFileDlg.FileName);
                owner.map.filename = owner.sFileDlg.FileName;
            }
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            if (owner.map.Edited() && System.Windows.Forms.MessageBox.Show("Are you sure you wish to create a new map?", "Warning",
                    System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                owner.map.lines.Clear();
                owner.map = new Map();
                owner.map.cViewPos = Microsoft.Xna.Framework.Vector2.Zero;
            }
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            if ((!owner.map.loaded && !owner.map.Edited()) || System.Windows.Forms.MessageBox.Show("Warning: This will overwrite any unsaved data\nContinue?", "Warning",
                        System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                System.Windows.Forms.DialogResult dR = owner.oFileDlg.ShowDialog();
                if (dR == System.Windows.Forms.DialogResult.OK)
                {
                    owner.map.lines.Clear();
                    owner.map = new Map(owner.oFileDlg.FileName, owner);
                    owner.map.cViewPos = Microsoft.Xna.Framework.Vector2.Zero;
                }
            }
        }

        private void tileTool_CheckedChanged(object sender, EventArgs e)
        {
            if (tileTool.Checked)
                owner.SetMode(Game.Mode.Tiles);
        }

        private void collisionTool_CheckedChanged(object sender, EventArgs e)
        {
            if (collisionTool.Checked)
                owner.SetMode(Game.Mode.Collision);
        }

        private void entityTool_CheckedChanged(object sender, EventArgs e)
        {
            if (entityTool.Checked)
                owner.SetMode(Game.Mode.Entities);
        }

        private void mucusPlacement_CheckedChanged(object sender, EventArgs e)
        {
            if (mucusTool.Checked)
                owner.SetMode(Game.Mode.Mucus);
        }
    }
}

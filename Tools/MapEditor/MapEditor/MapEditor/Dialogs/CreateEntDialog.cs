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
    public partial class CreateEntDialog : Form
    {
        public Entity Result { get { return result; } }

        /// <summary>
        /// The result of this dialog
        /// </summary>
        Entity result = null;

        public CreateEntDialog()
        {
            InitializeComponent();
            Game.isFocused = false;
        }

        private void otherOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game.isFocused = false;

            if (otherOptions.SelectedItem != null && otherOptions.SelectedItem.Equals("Add..."))
            {
                OptionsDialog oDialog = new OptionsDialog("New Property", "Create a new property for this entity", "", null);
                oDialog.ShowDialog();

                string rslt = oDialog.Result;

                if (oDialog.DialogResult == System.Windows.Forms.DialogResult.OK && rslt.Length > 0)
                    otherOptions.Items.Insert(otherOptions.Items.Count - 1, rslt);
            }
        }

        void otherOptions_DoubleClick(object sender, System.EventArgs e)
        {
            if (!otherOptions.SelectedItem.Equals("Add..."))
                otherOptions.Items.Remove(otherOptions.SelectedItem);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //fill the ent
            if (entTypes.SelectedIndex > 0)
            {
                int id = int.Parse(((string)entTypes.SelectedItem).Split(':')[0]);
                List<string> items = new List<string>(entTypes.Items.Count);
                for (int i = 0; i < otherOptions.Items.Count - 1; i++)
                    items.Add((string)otherOptions.Items[i]);

                result = new Entity();
                result.id = id;
                result.other = items;
            }
            else
                result = null;

            Game.isFocused = true;
            base.OnClosing(e);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

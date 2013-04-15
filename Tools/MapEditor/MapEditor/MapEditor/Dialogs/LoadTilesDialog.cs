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
    public partial class LoadTilesDialog : Form
    {
        Game owner;

        /// <summary>
        /// Create a new tileset loading dialog
        /// </summary>
        /// <param name="owner">Who owns this dialog</param>
        public LoadTilesDialog(Game owner)
        {
            InitializeComponent();
            Game.isFocused = false;
            this.owner = owner;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Game.isFocused = true;
        }

        private void backImageOpen_Click(object sender, EventArgs e)
        {
            imageLoad.FileName = "";
            imageLoad.ShowDialog();
            if (imageLoad.FileName != "")
            {
                backImageBox.Text = imageLoad.FileName;
                backColorButton.Focus();
            }
        }

        private void openTilesButton_Click(object sender, EventArgs e)
        {
            imageLoad.FileName = "";
            imageLoad.ShowDialog();
            if (imageLoad.FileName != "")
            {
                tilesetBox.Text = imageLoad.FileName;
                tileWidth.Focus();
            }
        }

        private void entsOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog oFD = new OpenFileDialog();
            oFD.Filter = "Entity list (elst)|elst";
            oFD.Title = "Open Entity list";
            oFD.CheckFileExists = true;
            oFD.CheckPathExists = true;
            oFD.FileName = "elst";
            oFD.ShowDialog();
            if (oFD.FileName != "")
            {
                entListBox.Text = oFD.FileName;
                backImageBox.Focus();
            }
        }

        private void backColorButton_Click(object sender, EventArgs e)
        {
            ColorDialog cD = new ColorDialog();
            cD.ShowDialog();

            backColorButton.BackColor = cD.Color; 
            backColorButton.ForeColor = cD.Color.GetBrightness() < 0.5f ? Color.White : Color.Black;
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            Map m = new Map();

            if (tilesetBox.Text == "" || tileWidth.Text == "" || tileHeight.Text == "" || mapWidth.Text == "" || mapHeight.Text == "" ||
                entListBox.Text == "" || backColorButton.BackColor == Color.Transparent)
            {
                MessageBox.Show("Fields left blank", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information); //anything blank, ignore
                return;
            }

            try
            {
                m.filename = "Untitled.2mcp";

               m.backgroundColor = new Microsoft.Xna.Framework.Color((int)backColorButton.BackColor.R,
                   (int)backColorButton.BackColor.G, (int)backColorButton.BackColor.B); //bg color

               if (backImageBox.Text != "") //background image
               {
                   m.background = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(owner.GraphicsDevice,
                       new System.IO.StreamReader(backImageBox.Text).BaseStream);
                   m.backgroundName = System.IO.Path.GetFileNameWithoutExtension(backImageBox.Text);
               }

                m.tileset = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(owner.GraphicsDevice,
                    new System.IO.StreamReader(tilesetBox.Text).BaseStream);
                m.tilesetName = System.IO.Path.GetFileNameWithoutExtension(tilesetBox.Text);

                int w = int.Parse(mapWidth.Text), h = int.Parse(mapHeight.Text);
                m.tiles = new int[h, w];
                m.tileWidth = int.Parse(tileWidth.Text);
                m.tileHeight = int.Parse(tileHeight.Text);
                m.width = w; m.height = h;

                //read ent types list (from elst)
                System.IO.StreamReader reader = new System.IO.StreamReader(entListBox.Text);
                string line;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    //comment or not enough data
                    if (line != "" && line[0] == '`')
                        continue;

                    string[] toks = line.Split(',');
                    if (toks.Length < 3) //not enough data
                        continue;

                    //load image
                    string s = toks[2];
                    System.IO.StreamReader sr = new System.IO.StreamReader(System.IO.Path.GetDirectoryName(entListBox.Text) + "\\" + s);
                    Microsoft.Xna.Framework.Graphics.Texture2D img = 
                        Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(owner.GraphicsDevice, sr.BaseStream);

                    m.entTypes.Add(new EntType(int.Parse(toks[0]), toks[1], img));
                }

                m.cViewPos = new Microsoft.Xna.Framework.Vector2(m.tileWidth, m.tileHeight);

                //center map if smaller than window size
                if (m.width * m.tileWidth < owner.GraphicsDevice.Viewport.Width)
                    m.cViewPos.X = -(owner.GraphicsDevice.Viewport.Width >> 1) + ((m.width * m.tileWidth) >> 1);
                if (m.height * m.tileHeight < owner.GraphicsDevice.Viewport.Height)
                    m.cViewPos.Y = -(owner.GraphicsDevice.Viewport.Height >> 1) + ((m.height * m.tileHeight) >> 1);

                m.loaded = true;
            }
            catch (Exception expt)
            {
                MessageBox.Show(expt.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m = new Map();
            }

            owner.map = m;

            Close();
        }
    }
}

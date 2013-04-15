//Collision.cs
//Copyright Dejitaru Forge 2011

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor.Screens
{
    public class Collision : GameScreen
    {
        /// <summary>
        /// close the drawn shape with a line (assuming enough points)
        /// </summary>
        public bool closeShape = true;

        Rectangle screenRect;

        public Texture2D lineBob; //a little bob to define the points of the line
        public Texture2D noProjectBg; //a simple texture to display when no project is loaded
        
        #region Initialization

        public override void LoadContent(List<object> args)
        {
            lineBob = content.Load<Texture2D>("Bob");
            noProjectBg = content.Load<Texture2D>("noproject");
        }

        #endregion 


        #region Update/Draw

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void HandleInput(GameTime gameTime, InputManager input)
        {
            Game g = ((Game)parent.Game);
            Map map = g.map;
            screenRect = new Rectangle((int)map.cViewPos.X, (int)map.cViewPos.Y, parent.GraphicsDevice.Viewport.Width, parent.GraphicsDevice.Viewport.Height);

            //ignore all mouse input if mouse isnt in window
            if (new Rectangle(0, 0, screenRect.Width, screenRect.Height).Contains(input.ms.X, input.ms.Y))
            {
                //add lines
                if (input.ms.LeftButton == ButtonState.Pressed && input.pms.LeftButton == ButtonState.Released)
                {
                    map.lines.Add(new Vector2(input.ms.X, input.ms.Y) + map.cViewPos);
                    map.hasAddedTiles = true;
                }

                //remove lines
                if (input.ms.RightButton == ButtonState.Pressed && input.pms.RightButton == ButtonState.Released)
                {
                    //find closest line (search from last to first)
                    Vector2 mPos = new Vector2(input.ms.X, input.ms.Y);
                    for (int i = map.lines.Count - 1; i >= 0; i--)
                        if (Vector2.Distance(mPos + map.cViewPos, map.lines[i]) < (lineBob.Width + lineBob.Height) / 2) //found line
                        {
                            map.lines.RemoveAt(i);
                            map.hasAddedTiles = true;
                            break;
                        }
                }

                //reset view
                if (input.ms.MiddleButton == ButtonState.Pressed && input.pms.MiddleButton == ButtonState.Released &&
                    (input.kb.IsKeyDown(Keys.LeftControl) || input.kb.IsKeyDown(Keys.RightControl)))
                    map.cViewPos = Vector2.Zero;

                //move view
                else if (input.ms.MiddleButton == ButtonState.Pressed)
                {
                    map.cViewPos += new Vector2(input.pms.X - input.ms.X, input.pms.Y - input.ms.Y);
                    if (map.loaded)
                    {
                        screenRect.X = (int)map.cViewPos.X;
                        screenRect.Y = (int)map.cViewPos.Y;
                    }
                }
            }

            //toggle the closing of shape with a line
            if (input.kb.IsKeyDown(Keys.F) && input.pkb.IsKeyUp(Keys.F))
                closeShape = !closeShape;

            //toggle grid
            if (input.kb.IsKeyDown(Keys.G) && input.pkb.IsKeyUp(Keys.G))
                ((Game)parent.Game).showGrid = !((Game)parent.Game).showGrid;

            //toggle bounding box
            if (input.kb.IsKeyDown(Keys.V) && input.pkb.IsKeyUp(Keys.V))
                ((Game)parent.Game).showBounds = !((Game)parent.Game).showBounds;

            //take screenshot
            if (!((Game)parent.Game).takingPic && input.kb.IsKeyDown(Keys.P) && input.pkb.IsKeyUp(Keys.P))
                ((Game)parent.Game).takingPic = true;

            //exit
            if (input.kb.IsKeyDown(Keys.X) && input.pkb.IsKeyUp(Keys.X) &&
                System.Windows.Forms.MessageBox.Show("Are you sure you wish to exit?", "Exit",
                        System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                parent.GameExit(null);

            //open
            if (input.kb.IsKeyDown(Keys.B) && input.pkb.IsKeyUp(Keys.B))
            {
                if ((!map.loaded && !map.Edited()) || System.Windows.Forms.MessageBox.Show("Warning: This will overwrite any unsaved data\nContinue?", "Warning",
                        System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    System.Windows.Forms.DialogResult dR = ((Game)parent.Game).oFileDlg.ShowDialog();
                    if (dR == System.Windows.Forms.DialogResult.OK)
                    {
                        g.map.lines.Clear();
                        g.map = new Map(((Game)parent.Game).oFileDlg.FileName, (Game)parent.Game);
                        g.map.cViewPos = Vector2.Zero;
                    }
                }
            }

            //save
            if (input.kb.IsKeyDown(Keys.S) && input.pkb.IsKeyUp(Keys.S))
            {
                g.sFileDlg.FileName = g.map.filename;
                System.Windows.Forms.DialogResult dR = g.sFileDlg.ShowDialog();
                if (dR == System.Windows.Forms.DialogResult.OK)
                {
                    g.map.Save(g.sFileDlg.FileName);
                    g.map.filename = g.sFileDlg.FileName;
                }
            }

            //new
            if (input.kb.IsKeyDown(Keys.N) && input.pkb.IsKeyUp(Keys.N))
            {
                if (map.lines.Count != 0 && System.Windows.Forms.MessageBox.Show("Are you sure you wish to create a new map?", "Warning",
                        System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    g.map.lines.Clear();
                    g.map = new Map();
                    g.map.cViewPos = Vector2.Zero;
                }
            }

            //change mode
            if ((input.kb.IsKeyDown(Keys.NumPad1) && input.pkb.IsKeyUp(Keys.NumPad1)) || (input.kb.IsKeyDown(Keys.D1) && input.pkb.IsKeyUp(Keys.D1)))
                ((Game)parent.Game).SetMode(Game.Mode.Tiles);
            else if ((input.kb.IsKeyDown(Keys.NumPad2) && input.pkb.IsKeyUp(Keys.NumPad2)) || (input.kb.IsKeyDown(Keys.D2) && input.pkb.IsKeyUp(Keys.D2)))
                ((Game)parent.Game).SetMode(Game.Mode.Collision);
            else if ((input.kb.IsKeyDown(Keys.NumPad3) && input.pkb.IsKeyUp(Keys.NumPad3)) || (input.kb.IsKeyDown(Keys.D3) && input.pkb.IsKeyUp(Keys.D3)))
                ((Game)parent.Game).SetMode(Game.Mode.Entities);
            else if ((input.kb.IsKeyDown(Keys.NumPad4) && input.pkb.IsKeyUp(Keys.NumPad4)) || (input.kb.IsKeyDown(Keys.D4) && input.pkb.IsKeyUp(Keys.D4)))
                ((Game)parent.Game).SetMode(Game.Mode.Mucus);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            Map map = ((Game)parent.Game).map;

            if (!map.loaded)
                parent.GraphicsDevice.Clear(Color.Black);

            if (((Game)parent.Game).takingPic)
            {
                parent.Game.Window.Title = "Rendering...";
                Vector2 origPos = map.cViewPos;

                string path = System.IO.Directory.GetCurrentDirectory() + "\\Render_map_" + System.IO.Path.GetFileNameWithoutExtension(map.filename) + "_" + System.DateTime.Now.ToShortDateString().Replace('/', '-');
                System.IO.FileStream f;
                int xdim = map.width * map.tileWidth;
                int ydim = map.height * map.tileHeight;
                int ycur = ydim;
                int maxRenderSize = 2048;

                for (int y = 0; y < (int)System.Math.Ceiling((float)ydim / maxRenderSize); y++)
                {
                    int xcur = xdim;
                    for (int x = 0; x < (int)System.Math.Ceiling((float)xdim / maxRenderSize); x++)
                    {
                        parent.GraphicsDevice.SetRenderTarget((((Game)parent.Game).render = new RenderTarget2D(parent.GraphicsDevice, (xcur > maxRenderSize ? maxRenderSize : xcur), (ycur > maxRenderSize ? maxRenderSize : ycur))));
                        xcur -= maxRenderSize;
                        map.cViewPos = new Vector2(x * maxRenderSize, y * maxRenderSize);
                        map.Draw(ref spriteBatch, new Rectangle(x * maxRenderSize, y * maxRenderSize, ((Game)parent.Game).render.Width, ((Game)parent.Game).render.Height));
                        parent.GraphicsDevice.SetRenderTarget(null);

                        f = new System.IO.FileStream(path + "_" + x + "x." + y + "y.png", System.IO.FileMode.Create);
                        ((Game)parent.Game).render.SaveAsPng(f, ((Game)parent.Game).render.Width, ((Game)parent.Game).render.Height);
                        f.Close();
                    }
                    ycur -= maxRenderSize;
                }

                map.cViewPos = origPos;
                ((Game)parent.Game).takingPic = false;
                parent.Game.Window.Title = "Finished rendering.";
            }
            else
                map.Draw(ref spriteBatch, screenRect);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if (!map.loaded)
                for (int x = 0; x < (int)(screenRect.Width / noProjectBg.Width) + 1; x++)
                    for (int y = 0; y < (int)(screenRect.Height / noProjectBg.Height) + 1; y++)
                        spriteBatch.Draw(noProjectBg, 
                            new Rectangle(x * noProjectBg.Width, y * noProjectBg.Height, noProjectBg.Width, noProjectBg.Height),
                            new Rectangle(0, 0, noProjectBg.Width, noProjectBg.Height), 
                            Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);

            //draw info text
            string infoText = "Mouse Position: " + parent.InputState.ms.X + ", " + parent.InputState.ms.Y + "\nMap Position: " + map.cViewPos.X + ", " + map.cViewPos.Y +
                "\nLines: " + (map.lines.Count > 0 ? map.lines.Count - 1 : 0);
            spriteBatch.DrawString(parent.Font, infoText, new Vector2(4, screenRect.Height - 42), Color.White);

            int selLines = 0; //number of selected lines (for drawing multiple positions)

            //draw grid
            if (((Game)parent.Game).showGrid)
            {
                Vector2 dPos = map.cViewPos;
                dPos.X %= map.tileWidth;
                dPos.Y %= map.tileHeight;

                for (int x = 0; x <= screenRect.Width / map.tileWidth + 1; x++)
                    Liner.DrawLine(ref spriteBatch, Game.gridColor, new Vector2(x * map.tileWidth - dPos.X, 0), new Vector2(x * map.tileWidth - dPos.X, screenRect.Height));
                for (int y = 0; y <= screenRect.Height / map.tileHeight + 1; y++)
                    Liner.DrawLine(ref spriteBatch, Game.gridColor, new Vector2(0, y * map.tileHeight - dPos.Y), new Vector2(screenRect.Width, y * map.tileHeight - dPos.Y));
            }

            //draw rectangle around map
            if (map.loaded && ((Game)parent.Game).showBounds)
                Liner.DrawRect(ref spriteBatch, new Rectangle((int)-map.cViewPos.X - 1, (int)-map.cViewPos.Y - 1, map.width * map.tileWidth + 2, map.height * map.tileHeight + 2), Game.boundsColor);

            //draw lines here
            for (int i = 0; i < map.lines.Count; i++)
            {
                //draw a selected point's position
                if (Vector2.Distance(new Vector2(parent.InputState.ms.X, parent.InputState.ms.Y), map.lines[i]) < (lineBob.Width + lineBob.Height) / 2)
                    spriteBatch.DrawString(parent.Font, "Point " + i + ": " + map.lines[i].X + ", " + map.lines[i].Y, new Vector2(4, 4 + selLines++ * 10), Color.White);
                
                if (i > 0) //start at point 1
                    Liner.DrawLine(ref spriteBatch, Color.White, map.lines[i - 1] - map.cViewPos, map.lines[i] - map.cViewPos, 0.5f);
                spriteBatch.Draw(lineBob, map.lines[i] - new Vector2(lineBob.Width >> 1, lineBob.Height >> 1) - map.cViewPos, Color.White);
            }

            //connect the last point to the first to make a closed shape (if more than 2 points)
            if (map.lines.Count > 2 && closeShape)
                Liner.DrawLine(ref spriteBatch, Color.White, map.lines[map.lines.Count - 1] - map.cViewPos, map.lines[0] - map.cViewPos, 0.5f);

            spriteBatch.End();

            string dispStr = res.titlebar + " - Collision - FPS " + (int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds);
            parent.Game.Window.Title = dispStr;
        }

        #endregion
    }
}

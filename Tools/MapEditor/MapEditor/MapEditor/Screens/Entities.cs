//Entities.cs
//Copyright Dejitaru Forge 2011

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor.Screens
{
    public class Entities : GameScreen
    {
        /// <summary>
        /// Is the editor waiting for the user to select an angle?
        /// </summary>
        bool selectingAngle = false;
        /// <summary>
        /// The ent that is being rotated
        /// </summary>
        Entity activeEnt = null;

        Rectangle screenRect;

        public Texture2D noProjectBg; //a simple texture to display when no project is loaded
        public Texture2D arrow; //the arrow for when selecting an ents angle

        /// <summary>
        /// is the user dragging an object?
        /// </summary>
        bool dragging = false;

        /// <summary>
        /// The currently selected entity (-1 for none)
        /// </summary>
        int selectedEnt = -1;

        #region Initialization

        public override void LoadContent(List<object> args)
        {
            noProjectBg = content.Load<Texture2D>("noproject");
            arrow = content.Load<Texture2D>("Arrow");
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

            //modify selected ent
            if (selectedEnt > -1 && input.kb.IsKeyDown(Keys.Enter) && input.pkb.IsKeyUp(Keys.Enter))
            {
                CreateEntDialog cED = new CreateEntDialog();

                //load other options
                cED.otherOptions.Items.Clear(); //remove 'Add...'
                cED.otherOptions.Items.AddRange(map.ents[selectedEnt].other.ToArray());
                cED.otherOptions.Items.Add("Add..."); //re add the 'Add...'

                cED.entTypes.Items.Add("");
                for (int j = 0; j < map.entTypes.Count; j++)
                {
                    cED.entTypes.Items.Add(map.entTypes[j].uid + ": " + map.entTypes[j].name);

                    //select the existing ent type
                    if (map.entTypes[j].uid == map.ents[selectedEnt].id)
                        cED.entTypes.SelectedIndex = j + 1;
                }

                cED.ShowDialog();

                //delete the ent
                if (cED.Result == null)
                {
                    map.ents.RemoveAt(selectedEnt);
                }
                //update ent
                else
                {
                    Entity e = cED.Result;
                    map.ents[selectedEnt].other = e.other;
                    map.ents[selectedEnt].id = e.id;
                }
            }

            //ignore all mouse input if mouse isnt in window
            if (new Rectangle(0, 0, screenRect.Width, screenRect.Height).Contains(input.ms.X, input.ms.Y))
            {
                if (selectingAngle) //clicking sets the angle
                {
                    if (input.ms.LeftButton == ButtonState.Pressed && input.pms.LeftButton == ButtonState.Released)
                    {
                        selectedEnt = map.ents.IndexOf(activeEnt); //set to active index
                        activeEnt = null;
                        selectingAngle = false;
                    }
                }
                else
                {
                    EntType eT = null;

                    if (selectedEnt > -1)
                    {
                        eT = EntType.GetTypeInfo(map.entTypes, map.ents[selectedEnt].id);

                        if (input.ms.LeftButton == ButtonState.Pressed && (input.pms.LeftButton == ButtonState.Pressed ||
                            new Rectangle((int)map.ents[selectedEnt].position.X - (eT.w >> 1), (int)map.ents[selectedEnt].position.Y - (eT.h >> 1), eT.w, eT.h).Contains(
                                (int)(input.ms.X + (eT.w >> 1) + map.cViewPos.X), (int)(input.ms.Y + (eT.h >> 1) + map.cViewPos.Y))))
                            dragging = true;
                        else
                            dragging = false;

                        if (dragging)
                        {
                            dragging = true;
                            map.ents[selectedEnt].position += new Vector2(input.ms.X - input.pms.X, input.ms.Y - input.pms.Y);
                        }
                    }

                    //add an entity
                    if (!dragging && input.ms.LeftButton == ButtonState.Pressed && input.pms.LeftButton == ButtonState.Released)
                    {
                        Point mouseLocation = new Point(input.ms.X, input.ms.Y);
                        bool foundEnt = false;

                        for (int i = 0; i < map.ents.Count; i++)
                        {
                            eT = EntType.GetTypeInfo(map.entTypes, map.ents[i].id);
                            Rectangle r = new Rectangle((int)map.ents[i].position.X - (eT.w >> 1), (int)map.ents[i].position.Y - (eT.h >> 1), eT.w, eT.h);
                            if (r.Contains((int)(input.ms.X + (eT.w >> 1) + map.cViewPos.X), (int)(input.ms.Y + (eT.h >> 1) + map.cViewPos.Y)))
                            {
                                selectedEnt = i;
                                foundEnt = true;
                                break;
                            }
                        }

                        if (!foundEnt)
                        {
                            CreateEntDialog cED = new CreateEntDialog();
                            cED.entTypes.Items.Add("");
                            cED.entTypes.SelectedIndex = 0;
                            for (int i = 0; i < map.entTypes.Count; i++)
                                cED.entTypes.Items.Add(map.entTypes[i].uid + ": " + map.entTypes[i].name);

                            cED.ShowDialog();

                            //An ent was created
                            if (cED.Result != null)
                            {
                                Entity e = cED.Result;
                                e.angle = 0;
                                eT = EntType.GetTypeInfo(map.entTypes, e.id);
                                e.position = new Vector2(mouseLocation.X + (eT.w >> 1), mouseLocation.Y + (eT.h >> 1)) + map.cViewPos;

                                selectingAngle = true;
                                activeEnt = e;

                                map.ents.Add(e);
                            }
                        }
                        map.hasAddedTiles = true;
                    }

                    //deselect entity
                    if (input.ms.RightButton == ButtonState.Pressed && input.pms.RightButton == ButtonState.Released)
                    {
                        selectedEnt = -1;
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

            //move ent using arrow keys
            if (selectedEnt > -1)
            {
                int delay = 100;

                int shiftAmount = 10; //how much to move when holding shift

                if (input.kb.IsKeyDown(Keys.Down) && (gameTime.TotalGameTime.Milliseconds % delay == 0 || input.pkb.IsKeyUp(Keys.Down)))
                    map.ents[selectedEnt].position.Y += (input.kb.IsKeyDown(Keys.LeftShift) || input.kb.IsKeyDown(Keys.RightShift) ? shiftAmount : 1);
                if (input.kb.IsKeyDown(Keys.Up) && (gameTime.TotalGameTime.Milliseconds % delay == 0 || input.pkb.IsKeyUp(Keys.Up)))
                    map.ents[selectedEnt].position.Y -= (input.kb.IsKeyDown(Keys.LeftShift) || input.kb.IsKeyDown(Keys.RightShift) ? shiftAmount : 1);

                if (input.kb.IsKeyDown(Keys.Left) && (gameTime.TotalGameTime.Milliseconds % delay == 0 || input.pkb.IsKeyUp(Keys.Left)))
                    map.ents[selectedEnt].position.X -= (input.kb.IsKeyDown(Keys.LeftShift) || input.kb.IsKeyDown(Keys.RightShift) ? shiftAmount : 1);
                if (input.kb.IsKeyDown(Keys.Right) && (gameTime.TotalGameTime.Milliseconds % delay == 0 || input.pkb.IsKeyUp(Keys.Right)))
                    map.ents[selectedEnt].position.X += (input.kb.IsKeyDown(Keys.LeftShift) || input.kb.IsKeyDown(Keys.RightShift) ? shiftAmount : 1);
            }

            //rotate ent
            if (input.kb.IsKeyDown(Keys.R) && input.pkb.IsKeyUp(Keys.R) && selectedEnt > -1)
            {
                activeEnt = map.ents[selectedEnt];
                selectingAngle = true;
            }

            //delete ent
            if (input.kb.IsKeyDown(Keys.Delete) && input.pkb.IsKeyUp(Keys.Delete) && selectedEnt > -1 &&
                System.Windows.Forms.MessageBox.Show("Are you sure you wish to delete this entity?", "Delete",
                    System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                if (selectingAngle)
                {
                    selectingAngle = false;
                    activeEnt = null;
                }
                map.ents.RemoveAt(selectedEnt);

                selectedEnt = -1;
            }

            if (selectedEnt > -1 && input.kb.IsKeyDown(Keys.D) && input.pkb.IsKeyUp(Keys.D))
            {
                Entity n = new Entity();
                n.angle = map.ents[selectedEnt].angle;
                n.id = map.ents[selectedEnt].id;
                n.other = map.ents[selectedEnt].other;
                n.position = map.ents[selectedEnt].position;
                map.ents.Add(n);
                selectedEnt = map.ents.Count - 1;
            }

            //toggle grid
            if (input.kb.IsKeyDown(Keys.G) && input.pkb.IsKeyUp(Keys.G))
                ((Game)parent.Game).showGrid = !((Game)parent.Game).showGrid;

            //toggle bounding box
            if (input.kb.IsKeyDown(Keys.V) && input.pkb.IsKeyUp(Keys.V))
                ((Game)parent.Game).showBounds = !((Game)parent.Game).showBounds;

            //toggle collision lines
            if (input.kb.IsKeyDown(Keys.C) && input.pkb.IsKeyUp(Keys.C))
                ((Game)parent.Game).showCollision = !((Game)parent.Game).showCollision;

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

            //update to make sure no error
            if (map.ents.Count - 1 < selectedEnt)
                selectedEnt = -1;

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if (!map.loaded)
                for (int x = 0; x < (int)(screenRect.Width / noProjectBg.Width) + 1; x++)
                    for (int y = 0; y < (int)(screenRect.Height / noProjectBg.Height) + 1; y++)
                        spriteBatch.Draw(noProjectBg,
                            new Rectangle(x * noProjectBg.Width, y * noProjectBg.Height, noProjectBg.Width, noProjectBg.Height),
                            new Rectangle(0, 0, noProjectBg.Width, noProjectBg.Height),
                            Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);

            if (((Game)parent.Game).showCollision)
            {
                //draw lines onion-skinned
                Color lineColor = Game.lineColor;
                for (int i = 0; i < map.lines.Count; i++)
                    if (i > 0) //start at point 1
                        Liner.DrawLine(ref spriteBatch, lineColor, map.lines[i - 1] - map.cViewPos, map.lines[i] - map.cViewPos, 0.5f);
                //connect the last point to the first to make a closed shape (if more than 2 points)
                if (map.lines.Count > 2 && ((Game)parent.Game).collisionEditor.closeShape)
                    Liner.DrawLine(ref spriteBatch, lineColor, map.lines[map.lines.Count - 1] - map.cViewPos, map.lines[0] - map.cViewPos, 0.5f);
            }

            //draw info text
            string infoText = "Mouse Position: " + parent.InputState.ms.X + ", " + parent.InputState.ms.Y + "\nMap Position: " + map.cViewPos.X + ", " + map.cViewPos.Y;
            spriteBatch.DrawString(parent.Font, infoText, new Vector2(4, screenRect.Height - 30), Color.White);

            if (selectedEnt > -1)
            {
                EntType eT = EntType.GetTypeInfo(map.entTypes, map.ents[selectedEnt].id);
                int sZ = (eT.w > eT.h ? eT.w : eT.h) + 2;
                Vector2 pos = map.ents[selectedEnt].position - map.cViewPos - new Vector2(sZ >> 1);
                Rectangle r = new Rectangle((int)pos.X - (eT.w >> 1), (int)pos.Y - (eT.h >> 1), sZ, sZ);
                Liner.DrawRect(ref spriteBatch, r, Color.White);
                r.X--; r.Y--; r.Width += 2; r.Height += 2;
                Liner.DrawRect(ref spriteBatch, r, Color.Black);

                //draw ent info
                spriteBatch.DrawString(parent.Font, map.ents[selectedEnt].ToString(map.entTypes), new Vector2(4), Color.White);
            }

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

            if (selectingAngle)
            {
                EntType eT = EntType.GetTypeInfo(map.entTypes, activeEnt.id);

                float round = MathHelper.Pi / 6; //how much to round to (default 30 degrees) when holding shift

                float a = (float)System.Math.Atan2(parent.InputState.ms.Y - (activeEnt.position.Y - (eT.w >> 1) - map.cViewPos.Y), parent.InputState.ms.X - (activeEnt.position.X - (eT.h >> 1) - map.cViewPos.X));
                if (parent.InputState.kb.IsKeyDown(Keys.LeftShift) || parent.InputState.kb.IsKeyDown(Keys.RightShift))
                    a = (int)System.Math.Round(a / round, 0) * round; //clamp to a certain degree
                Vector2 pos = activeEnt.position + new Vector2(64 * (float)System.Math.Cos(a), 64 * (float)System.Math.Sin(a)) - new Vector2(eT.w >> 1, eT.h >> 1) - map.cViewPos;
                spriteBatch.Draw(arrow, new Rectangle((int)pos.X, (int)pos.Y, arrow.Width, arrow.Height), null, Color.White,
                    a, new Vector2(arrow.Width >> 1, arrow.Height >> 1), SpriteEffects.None, 0);
                activeEnt.angle = a;

                spriteBatch.DrawString(parent.Font, 180 + MathHelper.ToDegrees(a) + "°", new Vector2(4), Color.White);
            }

            spriteBatch.End();

            string dispStr = res.titlebar + " - Entities - FPS " + (int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds);
            parent.Game.Window.Title = dispStr;
        }

        #endregion
    }
}

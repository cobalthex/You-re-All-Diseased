﻿//Tiles.cs
//Copyright Dejitaru Forge 2011

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor.Screens
{
    public class Tiles : GameScreen
    {
        Rectangle screenRect;

        public Texture2D noProjectBg; //a simple texture to display when no project is loaded

        public TileSelector tiles;

        /// <summary>
        /// The tile that the mouse cursor is over (-1 for none)
        /// </summary>
        public int litTile = -1;

        #region Initialization

        public override void LoadContent(List<object> args)
        {
            noProjectBg = content.Load<Texture2D>("noproject");

            tiles = new TileSelector();
            parent.AddScreen(tiles, null, null);
            tiles.screenState = ScreenState.Inactive;
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
            Game g = (Game)parent.Game;
            Map map = g.map;
            screenRect = new Rectangle((int)map.cViewPos.X, (int)map.cViewPos.Y, parent.GraphicsDevice.Viewport.Width, parent.GraphicsDevice.Viewport.Height);

            litTile = -1; //reset

            //ignore all mouse input if mouse isnt in window
            if (new Rectangle(0, 0, screenRect.Width, screenRect.Height).Contains(input.ms.X, input.ms.Y))
            {
                //hilight tile
                if (map.loaded && new Rectangle((int)-map.cViewPos.X, (int)-map.cViewPos.Y, map.width * map.tileWidth, map.height * map.tileHeight).Contains(input.ms.X, input.ms.Y))
                {
                    litTile = ((int)(input.ms.X + map.cViewPos.X) / map.tileWidth) + ((int)(input.ms.Y + map.cViewPos.Y) / map.tileHeight) * map.width;

                    //place tile
                    if (tiles.selectedItem > -1 && litTile > -1 && input.ms.LeftButton == ButtonState.Pressed)
                    {
                        map.tiles[litTile / map.width, litTile % map.width] = tiles.selectedItem;
                        map.hasAddedTiles = true;
                    }

                    //remove tile
                    else if (litTile > -1 && input.ms.RightButton == ButtonState.Pressed)
                    {
                        map.tiles[litTile / map.width, litTile % map.width] = 0;
                        map.hasAddedTiles = true;
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

            //resize
            if (map.loaded && input.kb.IsKeyDown(Keys.R) && input.pkb.IsKeyUp(Keys.R))
            {
                ResizeDialog resizer = new ResizeDialog();
                resizer.width.Text = map.width.ToString();
                resizer.height.Text = map.height.ToString();
                if (resizer.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    int w = resizer.width.IntValue, h = resizer.height.IntValue;

                    if (w == map.width && h == map.height)
                        return; //same size; don't change

                    int[,] newTiles = new int[h, w];

                    for (int i = 0; i < (w < map.width ? w : map.width); i++)
                        for (int j = 0; j < (h < map.height ? h : map.height); j++)
                            newTiles[j, i] = map.tiles[j, i];

                    map.tiles = newTiles;
                    map.width = w;
                    map.height = h;
                }
            }

            //show tile selector
            if (map.loaded && input.kb.IsKeyDown(Keys.Tab) && input.pkb.IsKeyUp(Keys.Tab))
                tiles.screenState = ScreenState.Active;

            //Open tileset
            if (input.kb.IsKeyDown(Keys.T) && input.pkb.IsKeyUp(Keys.T))
            {
                LoadTilesDialog lT = new LoadTilesDialog((Game)parent.Game);
                lT.ShowDialog();
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

            //Change mode
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

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

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

            if (map.loaded)
            {
                //draw rectangle around map
                if (((Game)parent.Game).showBounds)
                    Liner.DrawRect(ref spriteBatch, new Rectangle((int)-map.cViewPos.X - 1, (int)-map.cViewPos.Y - 1, map.width * map.tileWidth + 2, map.height * map.tileHeight + 2), Game.boundsColor);

                //draw active map piece
                if (tiles.selectedItem > -1)
                {
                    int tilesPerMapRow = map.tileset.Width / map.tileWidth; //number of horizonatl tiles on tileset image

                    Vector2 pos = new Vector2(screenRect.Width - map.tileWidth - 4, 4);
                    spriteBatch.Draw(map.tileset, pos, new Rectangle(((tiles.selectedItem - 1) % tilesPerMapRow) * map.tileWidth,
                        ((tiles.selectedItem - 1) / tilesPerMapRow) * map.tileHeight, map.tileWidth, map.tileHeight), Color.White);

                    Liner.DrawRect(ref spriteBatch, new Rectangle((int)pos.X - 1, (int)pos.Y - 1, map.tileWidth + 2, map.tileHeight + 2), Color.Black);
                    Liner.DrawRect(ref spriteBatch, new Rectangle((int)pos.X, (int)pos.Y, map.tileWidth, map.tileHeight), Color.White);
                }

                //draw lit tile
                if (litTile > -1)
                {
                    Rectangle pos = new Rectangle((litTile % map.width) * map.tileWidth - (int)map.cViewPos.X,
                        (litTile / map.width) * map.tileHeight - (int)map.cViewPos.Y, map.tileWidth, map.tileHeight);

                    Liner.DrawRect(ref spriteBatch, new Rectangle((int)pos.X - 1, (int)pos.Y - 1, map.tileWidth + 2, map.tileHeight + 2), Color.Black);
                    Liner.DrawRect(ref spriteBatch, new Rectangle((int)pos.X, (int)pos.Y, map.tileWidth, map.tileHeight), Color.White);
                }
            }

            //draw info text
            string infoText = "Mouse Position: " + parent.InputState.ms.X + ", " + parent.InputState.ms.Y + "\nView Position: " + map.cViewPos.X + ", " + map.cViewPos.Y;
            spriteBatch.DrawString(parent.Font, infoText, new Vector2(4, screenRect.Height - 40), Color.White);
            if (map.loaded)
            {
                infoText = "Map Position: " + (litTile % map.width) + "," + (litTile / map.width);
                spriteBatch.DrawString(parent.Font, infoText, new Vector2(4, screenRect.Height - 18), Color.White);
            }

            spriteBatch.End();

            string dispStr = res.titlebar + " - Tiles - FPS " + (int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds);
            parent.Game.Window.Title = dispStr;
        }

        #endregion
    }
}

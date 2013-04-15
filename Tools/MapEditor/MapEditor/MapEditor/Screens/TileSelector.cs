//TileSelector.cs
//Copyright Dejitaru Forge 2011

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor.Screens
{
    public class TileSelector : GameScreen
    {
        Rectangle screenRect;
        /// <summary>
        /// The size of the tile window
        /// </summary>
        public Rectangle windowRect;

        /// <summary>
        /// The selected item, -1 for none
        /// </summary>
        public int selectedItem = -1;

        /// <summary>
        /// The background (by default a plain white texture)
        /// </summary>
        Texture2D bg;

        /// <summary>
        /// Where the scroller is
        /// </summary>
        public int scrollPosition;

        #region Initialization

        public override void LoadContent(List<object> args)
        {
            bg = new Texture2D(parent.GraphicsDevice, 1, 1);
            bg.SetData<Color>(new Color[] { Color.White });
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

            int w = 2 + (map.tileWidth + 1) * ((screenRect.Width >> 2) / (map.tileWidth + 1)), h = screenRect.Height + 2;
            windowRect = new Rectangle(screenRect.Width - w + 2, -2, w, h);

            //ignore all mouse input if mouse isnt in window
            if (new Rectangle(0, 0, screenRect.Width, screenRect.Height).Contains(input.ms.X, input.ms.Y))
            {
                if (windowRect.Contains(input.ms.X,  input.ms.Y) && input.ms.LeftButton == ButtonState.Pressed && input.pms.LeftButton == ButtonState.Released)
                {
                    int tilesPerRow = windowRect.Width / map.tileWidth;

                    int x = input.ms.X - windowRect.X, y = input.ms.Y - windowRect.Y;
                    selectedItem = (y / map.tileHeight) * tilesPerRow + x / map.tileWidth;
                    selectedItem += scrollPosition * tilesPerRow + 1;
                }

                //scroll
                if (input.ms.MiddleButton == ButtonState.Pressed)
                {
                    int delta = input.ms.Y - input.pms.Y;
                    scrollPosition += -delta / 10;
                }
            }

            //scroll with mouse wheel
            scrollPosition -= (input.ms.ScrollWheelValue - input.pms.ScrollWheelValue) / 120;

            //scroll up
            if (input.kb.IsKeyDown(Keys.W) && input.pkb.IsKeyUp(Keys.W))
                scrollPosition++;
            else if (input.kb.IsKeyDown(Keys.W) && parent.frameTicks % 10 == 0)
                scrollPosition++;

            //scroll down
            if (input.kb.IsKeyDown(Keys.S) && input.pkb.IsKeyUp(Keys.S))
                scrollPosition--;
            else if (input.kb.IsKeyDown(Keys.S) && parent.frameTicks % 10 == 0)
                scrollPosition--;

            //do not allow scrolling of no tiles
            if (scrollPosition < 0)
                scrollPosition = 0;

            if (input.kb.IsKeyUp(Keys.Tab) && input.pkb.IsKeyDown(Keys.Tab))
                screenState = ScreenState.Inactive;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            Map map = ((Game)parent.Game).map;
            int tilesPerRow = windowRect.Width / map.tileWidth; //number of horizontal tiles displayed on screen
            int tilesPerMapRow = map.tileset.Width / map.tileWidth; //number of horizonatl tiles on tileset image
            int tileCount = (map.tileset.Width / map.tileWidth) * (map.tileset.Height / map.tileHeight);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            spriteBatch.Draw(bg, windowRect, Color.White);
            Liner.DrawRect(ref spriteBatch, windowRect, Color.Black);

            if (tilesPerRow > 0)
            {
                for (int i = scrollPosition * tilesPerRow; i < tileCount; i++)
                {
                    int n = i - (scrollPosition * tilesPerRow);

                    if (i < 0) //invalid region
                        i = 0;

                    spriteBatch.Draw(map.tileset,
                        new Vector2(windowRect.X + 1, windowRect.Y + 2) +
                        new Vector2((n % tilesPerRow) * (map.tileWidth + 1), (n / tilesPerRow) * (map.tileHeight + 1)),
                        new Rectangle((i % tilesPerMapRow) * map.tileWidth, (i / tilesPerMapRow) * map.tileHeight, map.tileWidth, map.tileHeight),
                        Color.White);
                }

                //force actual tile
                if (selectedItem - 1 >= tileCount)
                    selectedItem = -1;

                int nn = selectedItem - (scrollPosition * tilesPerRow) - 1;
                //draw box around selected item
                if (selectedItem > -1 && nn >= 0)
                {
                    Rectangle rct = new Rectangle(windowRect.X + 1 + (nn % tilesPerRow) * (map.tileWidth + 1),
                        windowRect.Y + 2 + (nn / tilesPerRow) * (map.tileHeight + 1), map.tileWidth, map.tileHeight);

                    Liner.DrawRect(ref spriteBatch, rct, Color.Red);
                    rct.X++; rct.Y++; rct.Width -= 2; rct.Height -= 2;
                    Liner.DrawRect(ref spriteBatch, rct, Color.White);
                    rct.X++; rct.Y++; rct.Width -= 2; rct.Height -= 2;
                    Liner.DrawRect(ref spriteBatch, rct, Color.Red);
                }
            }

            spriteBatch.End();
        }

        #endregion
    }
}

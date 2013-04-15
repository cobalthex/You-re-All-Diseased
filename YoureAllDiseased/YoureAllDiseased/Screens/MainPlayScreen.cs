//TestPlayScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace YoureAllDiseased
{
    /// <summary>
    /// The game title intro screen (Second)
    /// </summary>
    class MainPlayScreen : GameScreen
    {
        #region Data

        /// <summary>
        /// The loaded map
        /// </summary>
        Map map;
        /// <summary>
        /// The current level of the game (level #, 0 = intro)
        /// </summary>
        int level = 1;

        /// <summary>
        /// show the collision lines (used for debugging purposes)
        /// </summary>
        bool showCollisLines = true;

        /// <summary>
        /// A reference rectangle for the size of the screen
        /// </summary>
        Rectangle screenRect;
        /// <summary>
        /// Spritebatch
        /// </summary>
        SpriteBatch sB;

        /// <summary>
        /// The player's representation in game
        /// </summary>
        PlayerEnt player;
        /// <summary>
        /// the current sector (which coll. triangle) of the player
        /// </summary>
        int sector = 0;

        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.IEnumerable<object> args)
        {
            map = new Map("test.2mcp", parent.Content);
            screenRect = new Rectangle(0, 0, parent.GraphicsDevice.Viewport.Width, parent.GraphicsDevice.Viewport.Height);

            sB = new SpriteBatch(parent.GraphicsDevice);

            player = new PlayerEnt();
            player.Load(ref content);
            map.sections[0].entities.Add(player);

            //calculate circumcenter of first triangle and place player there
            if (map.triPoints.Length > 2)
            {
                player.position = new Vector2(100, 300);
            }
        }
        #endregion


        #region Update & Draw

        public override void HandleInput(GameTime gameTime, InputManager input)
        {
            if (input.accelReading.X != 0)
                player.velocity.X = Math.Min(player.velocity.X + input.accelReading.X, 8);

            if (input.accelReading.Y != 0)
                player.velocity.Y = Math.Min(player.velocity.Y + input.accelReading.Y, 8);

            if (player.velocity.X > 0)
                player.velocity.X = Math.Max(0, player.velocity.X - 0.4f);
            else if (player.velocity.X < 0)
                player.velocity.X = Math.Min(0, player.velocity.X + 0.4f);

            if (player.velocity.Y > 0)
                player.velocity.Y = Math.Max(0, player.velocity.Y - 0.4f);
            else if (player.velocity.Y < 0)
                player.velocity.Y = Math.Min(0, player.velocity.Y + 0.4f);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool isVisible, bool isCovered)
        {
            //check collision
            if (player.velocity != Vector2.Zero)
            {
                if (!InPoly(player.position + player.velocity, map.triPoints, 3, sector))
                {
                    if (player.velocity.X < 0 || player.velocity.Y < 0)
                        if (sector > 0)
                        {
                            if (!InPoly(player.position + player.velocity, map.triPoints, 4, sector - 1))
                                player.velocity = Vector2.Zero;
                            else
                                sector--;
                        }
                        else
                            player.velocity = Vector2.Zero;

                    if (player.velocity.X > 0 || player.velocity.Y > 0)
                        if (sector < map.triPoints.Length - 1)
                        {
                            if (!InPoly(player.position + player.velocity, map.triPoints, 4, sector))
                                player.velocity = Vector2.Zero;
                            else
                                sector++;
                        }
                        else
                            player.velocity = Vector2.Zero;
                }
            }

            player.position += player.velocity;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            sB.Begin();

            //set the map position relative to the player
            screenRect.X = (int)player.position.X - (screenRect.Width >> 1);
            screenRect.Y = (int)player.position.Y - (screenRect.Height >> 1);

            map.Draw(sB, screenRect);

            for (int i = 0; i < map.sections.Length; i++)
            {
                for (int j = 0; j < map.sections[i].entities.Count; j++)
                {
                    Texture2D s = map.sections[i].entities[j].sprite;
                    if (map.sections[i].entities[j] != player) //player is drawn in the middle of the map
                        sB.Draw(s, map.sections[i].entities[j].position - new Vector2(s.Width >> 1, s.Height >> 1), Color.White);
                }
            }

            if (showCollisLines)
            {
                Vector2 scrRctO2 = player.position - new Vector2(screenRect.Width >> 1, screenRect.Height >> 1);
                for (int i = 0; i < map.triPoints.Length - 2; i++)
                {
                    Liner.DrawLine(ref sB, Color.White, map.triPoints[i] - scrRctO2, map.triPoints[i + 1] - scrRctO2);
                    Liner.DrawLine(ref sB, Color.White, map.triPoints[i + 1] - scrRctO2, map.triPoints[i + 2] - scrRctO2);
                    Liner.DrawLine(ref sB, Color.White, map.triPoints[i + 2] - scrRctO2, map.triPoints[i] - scrRctO2);
                }
            }
            sB.DrawString(parent.Font, sector.ToString(), new Vector2(4), Color.White);

            sB.Draw(player.sprite, new Vector2((screenRect.Width >> 1), (screenRect.Height >> 1)) -
                new Vector2(player.sprite.Width >> 1, player.sprite.Height >> 1), Color.White); //draw the player in the middle of the map

            sB.End();
        }

        #endregion


        #region Other

        /// <summary>
        /// 2D Check if a point lies withing a polygon.
        /// </summary>
        /// <param name="polygonVerticies">The points of the polygon.</param>
        /// <param name="testVertex">The point to check.</param>
        /// <param name="nVerts">the number of verticies to check (leave zero to check all)</param>
        /// <param name="start">the vertex in the polygon to start</param>
        /// <returns>
        /// A boolean flag indicating if the test vertex
        /// is inside the polygon.
        /// </returns>
        public static bool InPoly(Vector2 testVertex, Vector2[] polygonVerticies, int nVerts, int start)
        {
            bool c = false;
            int nvert = nVerts > 0 && nVerts <= polygonVerticies.Length ? nVerts : polygonVerticies.Length;
            if (nvert > 2)
            {
                int i, j;
                for (i = (start < polygonVerticies.Length ? start : 0), j = nvert - 1; i < nvert; j = i++)
                {
                    if (((polygonVerticies[i].Y > testVertex.Y) != (polygonVerticies[j].Y > testVertex.Y)) &&
                     (testVertex.X < (polygonVerticies[j].X - polygonVerticies[i].X) *
                     (testVertex.Y - polygonVerticies[i].Y) /
                     (polygonVerticies[j].Y - polygonVerticies[i].Y) + polygonVerticies[i].X))
                    {
                        c = !c;
                    }
                }
            }

            return c;
        }

        #endregion
    }
}

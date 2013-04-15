//NameEntryScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace YoureAllDiseased
{
    /// <summary>
    /// The high scores screen
    /// </summary>
    public class NameEntryScreen : GameScreen
    {
        #region Data

        /// <summary>
        /// font to draw name entry with
        /// </summary>
        SpriteFont font;

        /// <summary>
        /// font for title and lower button
        /// </summary>
        SpriteFont titleFont;

        /// <summary>
        /// arrows to draw above/below current pos 
        /// </summary>
        Texture2D upDown;

        /// <summary>
        /// Name of the user to be entered
        /// </summary>
        int[] name;
        /// <summary>
        /// The score of the player
        /// </summary>
        int score = 0;

        /// <summary>
        /// current char being modified
        /// </summary>
        int currentPos = 0;

        /// <summary>
        /// has the 'continue' button been pressed
        /// </summary>
        bool pressed = false;

        #region Chars
        /// <summary>
        /// available chars for user to use for name
        /// </summary>
        char[] availableChars = new char[] {
            ' ','_','A','B','C','D','E','F',
            'G','H','I','J','K','L','M','N',
            'O','P','Q','R','S','T','U','V',
            'W','X','Y','Z','0','1','2','3',
            '4','5','6','7','8','9'
        };

        /// <summary>
        /// screen size
        /// </summary>
        int w, h;
        #endregion

        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
            font = content.Load<SpriteFont>("Fonts/Scores");
            titleFont = content.Load<SpriteFont>("Fonts/Menu");
            upDown = content.Load<Texture2D>("Graphics/UpDown");

            name = new int[] {2, 2, 2};

            if (args != null && args.Count > 0)
                score = (int)args[0];
        }

        #endregion


        #region Update & Draw

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool isVisible, bool isCovered)
        {
#if XNA31 //force zune to these dimensions (renders to a render target which is then scaled to display)
            w = 800;
            h = 480;
#else
            w = parent.GraphicsDevice.Viewport.Width;
            h = parent.GraphicsDevice.Viewport.Height;
#endif
        }

        public override void HandleInput(GameTime gameTime, InputManager input)
        {
            if (input.isBackButtonPressed)
            {
                string n = "";
                for (int i = 0; i < name.Length; i++)
                    n += availableChars[name[i]];

                parent.NextScreen(this, new HighScoresScreen(), new System.Collections.Generic.List<object> { score, n }, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
            }

#if WINDOWS || XBOX
            if (input.accelReading.X > 0)
                currentPos++;
            else if (input.accelReading.X < 0)
                currentPos--;

            //move between chars
            input.gpState.ThumbSticks.Left.Normalize();
            if ((input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadRight) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.DPadRight)) || 
                (input.gpState.ThumbSticks.Left.X > 0 && input.pGPState.ThumbSticks.Left.X == 0))
                currentPos++;
            else if ((input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadLeft) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.DPadLeft)) || 
                (input.gpState.ThumbSticks.Left.X < 0 && input.pGPState.ThumbSticks.Left.X == 0))
                currentPos--;

            if (currentPos > name.Length)
                currentPos = 0;
            if (currentPos < 0)
                currentPos = name.Length;

            //change current char
            if (currentPos != name.Length)
            {
                if (input.accelReading.Y < 0)
                {
                    name[currentPos]--;
                    if (name[currentPos] < 0)
                        name[currentPos] = availableChars.Length - 1;
                }
                else if (input.accelReading.Y > 0)
                    name[currentPos] = (name[currentPos] + 1) % availableChars.Length;

                if ((input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadDown) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.DPadDown)) ||
                (input.gpState.ThumbSticks.Left.Y < 0 && input.pGPState.ThumbSticks.Left.Y == 0))
                    name[currentPos] = (name[currentPos] + 1) % availableChars.Length;
                else if ((input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadUp) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.DPadUp)) ||
                    (input.gpState.ThumbSticks.Left.Y > 0 && input.pGPState.ThumbSticks.Left.Y == 0))
                {
                    name[currentPos]--;
                    if (name[currentPos] < 0)
                        name[currentPos] = availableChars.Length - 1;
                }
            }
            
            Microsoft.Xna.Framework.Input.KeyboardState ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            if ((input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.A) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.A)) || 
                (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter)) || (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space)))
            {
                if (currentPos == name.Length) //pressed continue
                {
                    string n = "";
                    for (int i = 0; i < name.Length; i++)
                        n += availableChars[name[i]];

                    pressed = true;
                    parent.NextScreen(this, new HighScoresScreen(), new System.Collections.Generic.List<object> { score, n }, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
                }
                else
                    currentPos = name.Length;
            }
#endif

            if (input.touches.Count > 0)
            {
                if ((int)input.touches[0].position.Y > (h >> 1) + 80)
                {
                    currentPos = name.Length;

#if WINDOWS
                    if (input.touches[0].state == TouchState.Pressed && input.pTouches[0].state == TouchState.Released)
#else
                    if (input.touches[0].state == TouchState.Pressed)
#endif
                    {
                        string n = "";
                        for (int i = 0; i < name.Length; i++) 
                            n += availableChars[name[i]];

                        pressed = true;
                        parent.NextScreen(this, new HighScoresScreen(), new System.Collections.Generic.List<object> { score, n }, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
                    }
                }

                else if (new Rectangle((w >> 1) - (name.Length - 1) * 50, 50, (name.Length - 1) * 110, h - 100).Contains((int)input.touches[0].position.X, (int)input.touches[0].position.Y))
                {
                    currentPos = ((int)input.touches[0].position.X + 50 - ((w >> 1) - (name.Length - 1) * 50)) / 100;
#if WINDOWS
                    if (input.touches[0].state == TouchState.Pressed && input.pTouches[0].state == TouchState.Released)
#else
                    if (input.touches[0].state == TouchState.Pressed)
#endif
                    {
                        if ((int)input.touches[0].position.Y < h >> 1)
                        {
                            name[currentPos]--;
                            if (name[currentPos] < 0)
                                name[currentPos] = availableChars.Length - 1;
                        }
                        else
                            name[currentPos] = (name[currentPos] + 1) % availableChars.Length;
                    }
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.DrawString(titleFont, "Enter your name", new Vector2(w >> 1, 100) - titleFont.MeasureString("Enter your name") / 2, Color.White);
            if (currentPos == name.Length)
                spriteBatch.DrawString(titleFont, "Continue", new Vector2(w >> 1, (h >> 1) + 140) - titleFont.MeasureString("Continue") / 2, pressed ? new Color(128, 255, 2) : new Color(164, 255, 128));
            else
                spriteBatch.DrawString(titleFont, "Continue", new Vector2(w >> 1, (h >> 1) + 140) - titleFont.MeasureString("Continue") / 2, pressed ? new Color(128, 255, 2) : Color.White);


            for (int i = 0; i < name.Length; i++)
            {
                string str = new string(availableChars[name[i]], 1);
                Vector2 txtSz = font.MeasureString(str);
                Vector2 pos = new Vector2((w >> 1) - (name.Length - 1) * 50 + i * 100, h >> 1) - txtSz / 2;

                spriteBatch.DrawString(font, str, pos, currentPos == i ? Color.White : Color.SteelBlue);

                //draw up down arrows
                if (currentPos == i)
                {
                    int x = (int)pos.X + ((int)(txtSz.X - 42) >> 1);
                    int y = h >> 1;
                    spriteBatch.Draw(upDown, new Vector2(x, y - 79), new Rectangle(0, 0, 42, 29), Color.White);
                    spriteBatch.Draw(upDown, new Vector2(x, y + 34), new Rectangle(0, 30, 42, 29), Color.White);
                }
            }

            spriteBatch.End();
        }

        #endregion
    }
}

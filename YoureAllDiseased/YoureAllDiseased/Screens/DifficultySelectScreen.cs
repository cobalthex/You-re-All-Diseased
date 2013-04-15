//DifficultySelectScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace YoureAllDiseased
{
    /// <summary>
    /// The difficulty selection screen
    /// </summary>
    public class DifficultySelectScreen : GameScreen
    {
        #region Data

        SpriteFont font; //font to be used for options

        Microsoft.Xna.Framework.Audio.SoundEffect tick;

        //used for glowing text on selection
        long glowStart = 0; //when the glow started (0 for not active)
        short whichItem = 0; //which item to glow

        int w, h; //width and height of screen

        System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>() {
            "Easy",
            "Medium",
            "Hard",
        };

        Microsoft.Xna.Framework.Input.KeyboardState kb;
        Microsoft.Xna.Framework.Input.KeyboardState pkb;

        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
#if WINDOWS_PHONE
            ((Main)parent.Game).gameAd300.Visible = true;
            ((Main)parent.Game).gameAd480.Visible = false;
            ((Main)parent.Game).gameAd300.DisplayRectangle = new Rectangle(0, 0, 800, 70);
#endif

            font = content.Load<SpriteFont>("Fonts/Menu");

            tick = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>("Audio/Sounds/Tick");

            kb = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            pkb = Microsoft.Xna.Framework.Input.Keyboard.GetState();
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
            int lastItem = whichItem;

            if (input.isBackButtonPressed)
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition); 

#if XBOX || WINDOWS
            pkb = kb;
            kb = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            
            if (kb.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.H) && pkb.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.H))
                parent.NextScreen(this, new HighScoresScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);

            //press enter or space to select option
            if ((kb.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter) && pkb.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Enter)) ||
                (kb.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space) && pkb.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Space)))
            {
                SelectItem(whichItem);
                glowStart = DateTime.UtcNow.Ticks;
            }
            else if (kb.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down) && pkb.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Down))
                whichItem = (short)((whichItem + 1) % options.Count);
            else if (kb.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up) && pkb.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Up))
                whichItem = (short)(whichItem - 1 < 0 ? (options.Count - 1) - whichItem : whichItem - 1);

            //press A to select option
            if ((input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.A) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.A)) || 
                (input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Start) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.Start)))
            {
                SelectItem(whichItem);
                glowStart = DateTime.UtcNow.Ticks;
            }
            //press B to go back to 'start' screen
            else if (input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.B) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.B))
            {
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
            }
            //move down or up
            else if ((input.gpState.ThumbSticks.Left.Y < 0 && input.pGPState.ThumbSticks.Left.Y >= 0) ||
                (input.gpState.DPad.Down == Microsoft.Xna.Framework.Input.ButtonState.Pressed && input.pGPState.DPad.Down == Microsoft.Xna.Framework.Input.ButtonState.Released))
                whichItem = (short)((whichItem + 1) % options.Count);
            else if ((input.gpState.ThumbSticks.Left.Y > 0 && input.pGPState.ThumbSticks.Left.Y <= 0) ||
                (input.gpState.DPad.Up == Microsoft.Xna.Framework.Input.ButtonState.Pressed && input.pGPState.DPad.Up == Microsoft.Xna.Framework.Input.ButtonState.Released))
                whichItem = (short)(whichItem - 1 < 0 ? (options.Count - 1) - whichItem : whichItem - 1);
#endif
#if !XBOX
            bool contains = false;
            if (input.touches.Count > 0)
                contains = new Rectangle(0, h + 50 - ((options.Count + 1) * 80), w, options.Count * 80).Contains((int)input.touches[0].position.X, (int)input.touches[0].position.Y); //is the mouse in the buttons area
#if WINDOWS
            if (contains)
                whichItem = (short)(((int)input.touches[0].position.Y - (h + 50 - ((options.Count + 1) * 80))) / 80);
#endif
            if (input.touches.Count > 0 && input.touches[0].state == TouchState.Pressed && contains)
            {
                whichItem = (short)(((int)input.touches[0].position.Y - (h + 50 - ((options.Count + 1) * 80))) / 80);
                SelectItem(whichItem);
                glowStart = DateTime.UtcNow.Ticks;
            }
#endif

            if (lastItem != whichItem && OptionsScreen.canPlayAudio && OptionsScreen.playSounds)
                tick.Play(0.2f, 0f, 0f);
        }

        /// <summary>
        /// Perform an action when 
        /// </summary>
        /// <param name="item"></param>
        void SelectItem(int item)
        {
            OptionsScreen.Difficulty = item;
            parent.NextScreen(this, (Main.pScreen = new PlayScreen()), new System.Collections.Generic.List<object> { 0 }, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            int cGlowPos = 0; //current position in glow animation

            Color glowColor = Color.White;

#if XBOX || WINDOWS
                glowColor = new Color(164, 255, 128);
#endif

            if (glowStart != 0) //animation is running
            {
                cGlowPos = (int)(DateTime.UtcNow.Ticks - glowStart) / 100000; //calculate current time in animation (to 100th of sec)
                glowColor = new Color(128 + (128 * cGlowPos / 128), 255, (255 * cGlowPos / 128)); //set the glow color for that time 
            }
            if (cGlowPos > 128) //reset the animation
                glowStart = 0;

            spriteBatch.DrawString(font, "Select Difficulty", new Vector2((w - (int)font.MeasureString("Select Difficulty").X) >> 1, 100), Color.Tomato);

            //draw the button text
            for (int i = 0; i < options.Count; i++)
                spriteBatch.DrawString(font, options[i], new Vector2((w - (int)font.MeasureString(options[i]).X) >> 1, 
                    h + 50 - ((options.Count + 1) * 80) + (i * 80)), whichItem == i ? glowColor : Color.White);

            spriteBatch.End();
        }

        #endregion
    }
}

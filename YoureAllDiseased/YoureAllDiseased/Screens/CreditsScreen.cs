//CreditsScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace YoureAllDiseased
{
    /// <summary>
    /// The game over screen
    /// </summary>
    public class CreditsScreen : GameScreen
    {
        #region Data

        /// <summary>
        /// The font for the credits
        /// </summary>
        SpriteFont font;
        /// <summary>
        /// large font for titles
        /// </summary>
        SpriteFont fontLarge;

        Texture2D gameLogo;
        Texture2D studioLogo;

        int cPos; //current scroll position

        /// <summary>
        /// the credits text
        /// Use ; to split lines, first line is in big font, rest are in small
        /// use { for period
        /// </summary>
        public readonly string[] creditsText = {
            "Created by;Matt Hines",
            "Code and graphics;Matt Hines",
            "Music;Kristoffer Malmgren;Marco Marold;Credits by Raphael Quercy",
            "Testing;Matt Hines;And many others",
            "Contact me at;dejitaruforge{co{cc"
        };

        /// <summary>
        /// The credits song
        /// </summary>
        Microsoft.Xna.Framework.Media.Song song;

        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
#if WINDOWS_PHONE
            ((Main)parent.Game).gameAd480.DisplayRectangle = new Rectangle(0, 0, 800, 80);
#endif
            font = content.Load<SpriteFont>("Fonts/Credits");
            fontLarge = content.Load<SpriteFont>("Fonts/CreditsLg");

            gameLogo = content.Load<Texture2D>("Graphics/Logo");
            studioLogo = content.Load<Texture2D>("Graphics/StudioLogo");

            song = content.Load<Microsoft.Xna.Framework.Media.Song>("Audio/Songs/Credits");

            //force lower case for all and use ` for spaces
            for (int i = 0; i < creditsText.Length; i++)
                creditsText[i] = creditsText[i].ToLower().Replace(' ', '`');

            cPos = parent.GraphicsDevice.Viewport.Height + 10;

            if (OptionsScreen.playMusic)
            {
                Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = true;
                Main.PlaySong(song, true);
            }
        }

        #endregion


        #region Update & Draw

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool isVisible, bool isCovered)
        {
#if WINDOWS || XBOX
            cPos -= 2;
#else
            cPos -= 4;
#endif

            if (cPos < -1500)
                cPos = parent.GraphicsDevice.Viewport.Height + 10;
        }

        public override void HandleInput(GameTime gameTime, InputManager input)
        {
            if (input.touches.Count > 0 && input.touches[0].state == TouchState.Pressed)
            {
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
#if !DEBUG
                if (OptionsScreen.playMusic)
                    Main.isMusicFading = true;
#endif
            }

            if (input.isBackButtonPressed)
            {
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
#if !DEBUG
                if (OptionsScreen.playMusic)
                    Main.isMusicFading = true;
#endif
            }

#if XBOX || WINDOWS
            if ((input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Start) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.Start)) ||
                (input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.B) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.B)) ||
                (input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.A) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.A)))
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
#endif

        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            int cYPos = 450 + cPos;

            for (int i = 0; i < creditsText.Length; i++)
            {
                string[] tokens = creditsText[i].Split(';');

                Vector2 screenSize = new Vector2(parent.GraphicsDevice.Viewport.Width, 0);
#if XBOX
                Vector2 offset = new Vector2(-100, cYPos);
#else
                Vector2 offset = new Vector2(-50, cYPos);
#endif

                for (int j = 0; j < tokens.Length; j++)
                {
                    Vector2 textSize = Vector2.Zero;
                    if (j == 0)
                        textSize = fontLarge.MeasureString(tokens[j]);
                    else
                        textSize = font.MeasureString(tokens[j]);

                    textSize.X = (int)textSize.X; textSize.Y = (int)textSize.Y; //force integer values

                    spriteBatch.DrawString(j == 0 ? fontLarge : font, tokens[j], new Vector2(screenSize.X - textSize.X, 10) + offset, j == 0 ? Color.Tomato : new Color(135, 235, 20));

                    offset.Y += textSize.Y - 20;
                }

#if XBOX
                spriteBatch.Draw(gameLogo, new Vector2(80, 50 + cPos), Color.White);
                spriteBatch.Draw(studioLogo, new Vector2(120, 300 + cPos), Color.White);
#else
                spriteBatch.Draw(gameLogo, new Vector2(50, 50 + cPos), Color.White);
                spriteBatch.Draw(studioLogo, new Vector2(90, 300 + cPos), Color.White);
#endif

                cYPos = (int)offset.Y;
            }

            spriteBatch.End();
        }

        #endregion
    }
}

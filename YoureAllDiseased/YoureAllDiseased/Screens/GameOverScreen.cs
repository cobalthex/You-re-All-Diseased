//GameOverScreen.cs
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
    public class GameOverScreen : GameScreen
    {
        #region Data

        /// <summary>
        /// The image saying "Game Over" displayed in the center of the screen
        /// </summary>
        Texture2D gameOverLogo;

        /// <summary>
        /// Bitmap numbers used to show score
        /// </summary>
        Texture2D numbersSprite;

        /// <summary>
        /// Player's score, represented in string form
        /// </summary>
        string scoreTxt = "00000000";
        /// <summary>
        /// player score
        /// </summary>
        int score;
        /// <summary>
        /// number of kills
        /// </summary>
        string deathCount = "00000000";

        /// <summary>
        /// The font for the subtext
        /// </summary>
        SpriteFont subTextFont;


        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
#if WINDOWS_PHONE
            ((Main)parent.Game).gameAd300.Visible = false;
            ((Main)parent.Game).gameAd480.Visible = false;
#endif

#if WINDOWS || XBOX
            Microsoft.Xna.Framework.Input.GamePad.SetVibration(parent.InputState.activePlayerIndex, 0, 0);
#endif

            gameOverLogo = content.Load<Texture2D>("Graphics/GameOver");
            numbersSprite = content.Load<Texture2D>("Graphics/Numbers");
            subTextFont = content.Load<SpriteFont>("Fonts/Menu");

            if (args.Count > 2)
            {
                scoreTxt = args[0].ToString().PadLeft(8, '0');
                score = (int)args[0];
                deathCount = args[1].ToString().PadLeft(8, '0');
            }
        } 

        #endregion


        #region Update & Draw
        
        public override void HandleInput(GameTime gameTime, InputManager input)
        {
#if WINDOWS || XBOX
            if (input.isBackButtonPressed || input.gpState.Buttons.A == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                NextScreen();
#endif
#if !XBOX
            if (input.isBackButtonPressed || input.touches.Count > 0 && input.touches[0].state == TouchState.Pressed)
                NextScreen();
#endif
        }

        void NextScreen()
        {
#if XBOX || WINDOWS_PHONE
            if (Microsoft.Xna.Framework.GamerServices.Guide.IsTrialMode)
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
            else
                parent.NextScreen(this, new NameEntryScreen(), new System.Collections.Generic.List<object> { score }, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
#else
            parent.NextScreen(this, new NameEntryScreen(), new System.Collections.Generic.List<object> { score }, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
#endif
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(gameOverLogo, new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) - (gameOverLogo.Width >> 1),
                (parent.GraphicsDevice.Viewport.Height >> 1) - (gameOverLogo.Height >> 1) - 100), Color.White);

            int drawPosX = (parent.GraphicsDevice.Viewport.Width >> 1) - (scoreTxt.Length * 25);
            int drawPosY = parent.GraphicsDevice.Viewport.Height - (parent.GraphicsDevice.Viewport.Height >> 2) - 150;
            for (int i = 0; i < scoreTxt.Length; i++)
            {
                int n = scoreTxt[i] - 48; //ascii '0' starts at chr 48
                spriteBatch.Draw(numbersSprite, new Vector2(drawPosX + (i * 50), n == 4 ? drawPosY - 2 : drawPosY),
                    new Rectangle(51, 0 + (scoreTxt[i] - 48) * 50, 51, 50), Color.White);
            }

            spriteBatch.DrawString(subTextFont, "Lives saved!", new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) -
                ((int)(subTextFont.MeasureString("Lives saved!").X) >> 1), drawPosY + 60), new Color(240, 48, 0));

            drawPosY += 150;
            for (int i = 0; i < deathCount.Length; i++)
            {
                int n = scoreTxt[i] - 48; //ascii '0' starts at chr 48
                spriteBatch.Draw(numbersSprite, new Vector2(drawPosX + (i * 50), n == 4 ? drawPosY - 2 : drawPosY),
                    new Rectangle(51, 0 + (deathCount[i] - 48) * 50, 51, 50), Color.White);
            }

            spriteBatch.DrawString(subTextFont, "vaccines produced!", new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) -
                ((int)(subTextFont.MeasureString("vaccines produced!").X) >> 1), drawPosY + 60), new Color(240, 48, 0));

            spriteBatch.End();
        }

        #endregion
    }
}

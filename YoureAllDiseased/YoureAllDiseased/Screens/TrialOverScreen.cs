//TrialOverScreen.cs
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
    public class TrialOverScreen : GameScreen
    {
        #region Data

        /// <summary>
        /// The image saying "DEMO"
        /// </summary>
        Texture2D demoLogo;
        SpriteFont font;

        System.Collections.Generic.List<object> args;

        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
#if WINDOWS || XBOX
            Microsoft.Xna.Framework.Input.GamePad.SetVibration(parent.InputState.activePlayerIndex, 0, 0);
#endif

            demoLogo = content.Load<Texture2D>("Graphics/DemoBanner");
            font = content.Load<SpriteFont>("Fonts/Menu");

            this.args = args;
        }

        #endregion


        #region Update & Draw

        public override void HandleInput(GameTime gameTime, InputManager input)
        {
#if XBOX
            if (input.isBackButtonPressed || input.gpState.Buttons.A == Microsoft.Xna.Framework.Input.ButtonState.Pressed || input.gpState.Buttons.Start == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                if (!Microsoft.Xna.Framework.GamerServices.Guide.IsTrialMode && args != null)
                    parent.NextScreen(this, new PlayScreen(), args, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
                else
#if PLAYTEST
                    parent.GameExit(null);
#else
                    parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
#endif
            }
#else
            if (input.isBackButtonPressed || (input.touches.Count > 0 && input.touches[0].position.Y <= (parent.GraphicsDevice.Viewport.Height >> 1) + 100))
            {
#if WINDOWS_PHONE
                if (!Microsoft.Xna.Framework.GamerServices.Guide.IsTrialMode && args != null)
                    parent.NextScreen(this, new PlayScreen(), args, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
                else
#endif
                    parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
            }
#endif

            try
            {
#if XBOX
                if (input.gpState.Buttons.X == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                    Microsoft.Xna.Framework.GamerServices.Guide.IsTrialMode)
                {
                    Microsoft.Xna.Framework.GamerServices.SignedInGamer gamer = Microsoft.Xna.Framework.GamerServices.Gamer.SignedInGamers[parent.InputState.activePlayerIndex];

                    if (gamer != null)
                    {

                        Microsoft.Xna.Framework.GamerServices.Guide.ShowMarketplace(parent.InputState.activePlayerIndex);
                    }
                    else
                    {
                        Microsoft.Xna.Framework.GamerServices.Guide.ShowSignIn(1, true);
                    }
                }
#elif WINDOWS_PHONE
                if (input.touches.Count > 0 && input.touches[0].position.Y > (parent.GraphicsDevice.Viewport.Height >> 1) + 100 &&
                    Microsoft.Xna.Framework.GamerServices.Guide.IsTrialMode)
                {
                    Microsoft.Xna.Framework.GamerServices.Guide.ShowMarketplace(PlayerIndex.One);
                }
#endif
            }
            catch
            {
#if PLAYTEST
                parent.GameExit(null);
#else
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
#endif
            }

            if (input.touches.Count > 0 && input.touches[0].state == TouchState.Pressed)
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(demoLogo, new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) - (demoLogo.Width >> 1),
                (parent.GraphicsDevice.Viewport.Height >> 1) - (demoLogo.Height >> 1) - 100), Color.White);

            spriteBatch.DrawString(font, "OVER", new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) -
                ((int)(font.MeasureString("OVER").X) >> 1), (parent.GraphicsDevice.Viewport.Height >> 1) - 20), Color.DeepPink);

            Vector2 pos = new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) - ((int)(font.MeasureString("Thanks for playing").X) >> 1), (parent.GraphicsDevice.Viewport.Height >> 1) + 80);

            #region Thanks for playing
            spriteBatch.DrawString(font, "Thanks for playing", pos + new Vector2(-3), Color.DeepPink);
            spriteBatch.DrawString(font, "Thanks for playing", pos + new Vector2(3, -3), Color.DeepPink);
            spriteBatch.DrawString(font, "Thanks for playing", pos + new Vector2(-3, 3), Color.DeepPink);
            spriteBatch.DrawString(font, "Thanks for playing", pos + new Vector2(3), Color.DeepPink);
            spriteBatch.DrawString(font, "Thanks for playing", pos + new Vector2(-3, 0), Color.DeepPink);
            spriteBatch.DrawString(font, "Thanks for playing", pos + new Vector2(3, 0), Color.DeepPink);
            spriteBatch.DrawString(font, "Thanks for playing", pos + new Vector2(0, -3), Color.DeepPink);
            spriteBatch.DrawString(font, "Thanks for playing", pos + new Vector2(0, 3), Color.DeepPink);
            spriteBatch.DrawString(font, "Thanks for playing", pos, Color.White);
            #endregion

#if XBOX
            if (Microsoft.Xna.Framework.GamerServices.Guide.IsTrialMode)
            {
                pos = new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) - ((int)font.MeasureString("Press X to buy the game").X >> 1), (parent.GraphicsDevice.Viewport.Height >> 1) + 160);
                spriteBatch.DrawString(font, "Press X to buy the game", pos, Color.White);

                pos.X += font.MeasureString("Press ").X;
                spriteBatch.DrawString(font, "X", pos, new Color(0, 128, 255));
            }
            else
            {
                pos = new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) - ((int)font.MeasureString("Press A to continue").X >> 1), (parent.GraphicsDevice.Viewport.Height >> 1) + 160);
                spriteBatch.DrawString(font, "Press A to continue", pos, Color.White);

                pos.X += font.MeasureString("Press ").X;
                spriteBatch.DrawString(font, "A", pos, new Color(144, 255, 0));
            }
#elif WINDOWS_PHONE
            pos = new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) - ((int)font.MeasureString("Press here to buy the game").X >> 1), (parent.GraphicsDevice.Viewport.Height >> 1) + 160);
            spriteBatch.DrawString(font, "Press here to buy the game", pos, Color.White);

            pos.X += font.MeasureString("Press ").X;
            spriteBatch.DrawString(font, "here", pos, new Color(128, 255, 0));
#endif

            spriteBatch.End();
        }

        #endregion
    }
}

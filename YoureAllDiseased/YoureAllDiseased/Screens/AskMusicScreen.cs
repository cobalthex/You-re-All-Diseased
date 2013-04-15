//AskMusicScreen.cs
//Copyright Dejitaru Forge 2011

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace YoureAllDiseased
{
    /// <summary>
    /// The screen choose whether or not to play music
    /// </summary>
    public class AskMusicScreen : GameScreen
    {
        #region Update & Draw

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
#if WINDOWS_PHONE
            ((Main)parent.Game).gameAd300.Visible = true;
#endif
        }

        public override void HandleInput(GameTime gameTime, InputManager input)
        {
            if (input.isBackButtonPressed)
                parent.GameExit(((Main)parent.Game).fadeOutTransition);

            if (input.touches.Count > 0)
            {
                if (new Rectangle(0, 200, 800, 80).Contains((int)input.touches[0].position.X, (int)input.touches[0].position.Y))
                {
                    OptionsScreen.canPlayAudio = true;
                    OptionsScreen.playMusic = true;
                    parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
                }
                else if (new Rectangle(0, 280, 800, 80).Contains((int)input.touches[0].position.X, (int)input.touches[0].position.Y))
                {
                    OptionsScreen.canPlayAudio = false;
                    parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.DrawString(parent.Font, "Play in-game audio?", new Vector2((parent.GraphicsDevice.Viewport.Width - (int)parent.Font.MeasureString("Play in-game audio?").X) >> 1, 100), Color.White);
            spriteBatch.DrawString(parent.Font, "(will stop already playing music)", new Vector2((parent.GraphicsDevice.Viewport.Width - (int)parent.Font.MeasureString("(will stop already playing music)").X) >> 1, 130), Color.Gray);

            spriteBatch.DrawString(parent.Font, "yes", new Vector2((parent.GraphicsDevice.Viewport.Width - (int)parent.Font.MeasureString("yes").X) >> 1, 240), Color.LawnGreen);
            spriteBatch.DrawString(parent.Font, "no", new Vector2((parent.GraphicsDevice.Viewport.Width - (int)parent.Font.MeasureString("no").X) >> 1, 320), Color.Tomato);

            spriteBatch.End();
        }

        #endregion
    }
}

//PauseScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace YoureAllDiseased
{
    /// <summary>
    /// The pause screen
    /// </summary>
    public class PauseScreen : GameScreen
    {
        #region Data

        /// <summary>
        /// The image saying "Paused" displayed in the center of the screen
        /// </summary>
        Texture2D pauseLogo;

#if !XBOX
        /// <summary>
        /// Mute button image
        /// </summary>
        Texture2D muteImg;

        /// <summary>
        /// Image for button for recalibrating the orientation
        /// </summary>
        Texture2D recalibrateImg;
#endif

        /// <summary>
        /// a simple black texture
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.Texture2D black;

        /// <summary>
        /// Who owns this screen
        /// </summary>
        PlayScreen owner;

        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
#if WINDOWS || XBOX
            Microsoft.Xna.Framework.Input.GamePad.SetVibration(parent.InputState.activePlayerIndex, 0, 0);
#endif

            screenType = ScreenType.Popup;

            pauseLogo = content.Load<Texture2D>("Graphics/Paused");

#if !XBOX
            muteImg = content.Load<Texture2D>("Graphics/Mute");
            recalibrateImg = content.Load<Texture2D>("Graphics/Recalibrate");
#endif

            black = new Microsoft.Xna.Framework.Graphics.Texture2D(parent.GraphicsDevice, 1, 1);
#if XNA31
            black.SetData<Microsoft.Xna.Framework.Graphics.Color>(new Microsoft.Xna.Framework.Graphics.Color[] { Microsoft.Xna.Framework.Graphics.Color.Black });
#else
            black.SetData<Microsoft.Xna.Framework.Color>(new Microsoft.Xna.Framework.Color[] { Microsoft.Xna.Framework.Color.Black });
#endif

            if (args != null && args.Count > 0)
                owner = (PlayScreen)args[0];
        }

        #endregion


        #region Update & Draw

        public override void HandleInput(GameTime gameTime, InputManager input)
        {
            //close the pause screen if user touches screen
#if WINDOWS
            if (input.touches[0].state == TouchState.Pressed)
#else
            if (input.touches.Count == 1 && input.touches[0].state == TouchState.Pressed)
#endif
            {
                int screenWid = parent.Game.GraphicsDevice.Viewport.Width;
#if ZUNE
                screenWid = 480;
#endif

                if (OptionsScreen.canPlayAudio && new Rectangle(35, 35, 100, 100).Contains((int)input.touches[0].position.X, (int)input.touches[0].position.Y))
                {
                    OptionsScreen.playMusic = !OptionsScreen.playMusic;
                    if (!OptionsScreen.playMusic)
                        Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
                }
                else if (!OptionsScreen.useJoyNotAccel && new Rectangle(screenWid - 150, 35, 150, 150).Contains((int)input.touches[0].position.X, (int)input.touches[0].position.Y))
                {
#if WINDOWS_PHONE || ZUNE
                    Main.calibScreen.Show(owner);
#endif
                }
                else
                {
                    if (OptionsScreen.playMusic && OptionsScreen.canPlayAudio)
                    {
                        Main.isMusicFading = false;
                        Microsoft.Xna.Framework.Media.MediaPlayer.Resume();
                    }
                    this.screenState = ScreenState.Inactive;
                }
            }
#if ZUNE
            else if (input.touches.Count == 2 && input.touches[0].state == TouchState.Pressed && input.touches[1].state == TouchState.Pressed)
            {
                Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
                parent.ForceRemove(owner);
            }
#endif

#if XBOX || WINDOWS
            if ((input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Start) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.Start)) ||
                (input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.B) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.B)))
            {
                if (OptionsScreen.playMusic)
                    Microsoft.Xna.Framework.Media.MediaPlayer.Resume();
                this.screenState = ScreenState.Inactive;
            }
#endif

            if (input.isBackButtonPressed)
            {
#if PLAYTEST
                parent.GameExit(((Main)parent.Game).fadeOutTransition);
#else
                Main.isMusicFading = true;
                OptionsScreen.showHints = false;
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
                parent.ForceRemove(owner);
#endif
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {

#if XNA31
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
#else
            spriteBatch.Begin();
#endif

            spriteBatch.Draw(black, new Rectangle(0, 0, parent.GraphicsDevice.Viewport.Width, parent.GraphicsDevice.Viewport.Height), new Color(0, 0, 0, 0.8f));

            spriteBatch.Draw(pauseLogo, new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) - (pauseLogo.Width >> 1),
                (parent.GraphicsDevice.Viewport.Height >> 1) - (pauseLogo.Height >> 1)), Color.White);

#if XBOX
            spriteBatch.DrawString(parent.Font, "Press back to exit", new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) -
                ((int)(parent.Font.MeasureString("Press back to exit").X) >> 1), 100), Color.White);
#else
            spriteBatch.DrawString(parent.Font, "Press anywhere to resume", new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) -
                ((int)(parent.Font.MeasureString("Press anywhere to resume").X) >> 1), 80), Color.White);

            if (OptionsScreen.canPlayAudio)
            {
                spriteBatch.Draw(muteImg, new Vector2(50), new Rectangle(0, 0, 67, 83), Color.White);
                if (!OptionsScreen.playMusic)
                    spriteBatch.Draw(muteImg, new Vector2(50, 50), new Rectangle(67, 0, 77, 77), Color.White);
            }

#endif

#if WINDOWS_PHONE || ZUNE
            if (!OptionsScreen.useJoyNotAccel)
                spriteBatch.Draw(recalibrateImg, new Vector2(parent.GraphicsDevice.Viewport.Width - 50 - recalibrateImg.Width, 50), Color.White);
#endif

#if ZUNE
            spriteBatch.DrawString(parent.Font, "Dual press to exit", new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) -
                ((int)(parent.Font.MeasureString("Dual press to exit").X) >> 1), 110), Color.White);
#endif

            string time = System.DateTime.Now.ToLongTimeString();
#if XBOX

            spriteBatch.DrawString(parent.Font, time, new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) -
                ((int)(parent.Font.MeasureString(time).X) >> 1), parent.GraphicsDevice.Viewport.Height - 120), Color.White);
#else
            spriteBatch.DrawString(parent.Font, time, new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) -
                ((int)(parent.Font.MeasureString(time).X) >> 1), parent.GraphicsDevice.Viewport.Height - 120), Color.White);
#endif

            spriteBatch.End();
        }

        #endregion
    }
}

//IntroScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace YoureAllDiseased
{
    /// <summary>
    /// The game intro screen
    /// </summary>
    public class IntroScreen : GameScreen
    {
        #region Data

        /// <summary>
        /// The logo of the Dejitaru Forge
        /// </summary>
        public Texture2D studioLogo;

#if WINDOWS || XBOX
        //for checking input
        char done = (char)0;
#endif

        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
            OptionsScreen.LoadSettings();

            //load main game transitions
            ((Main)parent.Game).fadeInTransition = new FadeInTransition(parent.GraphicsDevice);
            ((Main)parent.Game).fadeOutTransition = new FadeOutTransition(parent.GraphicsDevice);
            ((Main)parent.Game).gameOverTransition = new GameOverTransition(parent.GraphicsDevice);
            ((Main)parent.Game).lvlCompleteTransition = new LvlCompleteTransition(parent.GraphicsDevice);

            studioLogo = content.Load<Texture2D>("Graphics/IntroLogo");
        }

        #endregion


        #region Update & Draw

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool isVisible, bool isCovered)
        {
            if ((DateTime.UtcNow - screenStartTime).TotalSeconds > 3)
                NextScreen();
        }

        public override void HandleInput(GameTime gameTime, InputManager input)
        {
            if (input.isBackButtonPressed)
            {
                parent.GraphicsDevice.Clear(Color.Black);
                parent.GameExit(null);
            }

#if WINDOWS || XBOX
            //input of all 4 controllers
            GamePadState gp = GamePad.GetState(PlayerIndex.One);
            if (done == 0 && gp.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.A) || gp.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Start))
            {
                input.activePlayerIndex = PlayerIndex.One;
                done = (char)1;
            }
            gp = GamePad.GetState(PlayerIndex.Two);
            if (done == 0 && gp.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.A) || gp.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Start))
            {
                input.activePlayerIndex = PlayerIndex.Two;
                done = (char)1;
            }
            gp = GamePad.GetState(PlayerIndex.Three);
            if (done == 0 && gp.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.A) || gp.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Start))
            {
                input.activePlayerIndex = PlayerIndex.Three;
                done = (char)1;
            }
            gp = GamePad.GetState(PlayerIndex.Four);
            if (done == 0 && gp.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.A) || gp.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Start))
            {
                input.activePlayerIndex = PlayerIndex.Four;
                done = (char)1;
            }

            if (done == 1)
            {
                done = (char)2;
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
            }
#endif

            if (input.touches.Count > 0 && input.touches[0].state == TouchState.Pressed)
                NextScreen();
        }

        void NextScreen()
        {
#if XBOX
            parent.NextScreen(this, new StartScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
#elif WINDOWS_PHONE || ZUNE
            if (OptionsScreen.playMusic || Microsoft.Xna.Framework.Media.MediaPlayer.State == Microsoft.Xna.Framework.Media.MediaState.Playing)
                parent.NextScreen(this, new AskMusicScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
            else
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
#else
            parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
#endif
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(studioLogo, new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) - (studioLogo.Width >> 1),
                (parent.GraphicsDevice.Viewport.Height >> 1) - (studioLogo.Height >> 1)), Color.White);

            spriteBatch.End();
        }

        #endregion
    }
}

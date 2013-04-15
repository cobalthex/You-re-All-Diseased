//StartScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace YoureAllDiseased
{
    /// <summary>
    /// The xbox start screen (selects active controller)
    /// </summary>
    public class StartScreen : GameScreen
    {
        #region Data

        /// <summary>
        /// The Press Start image
        /// </summary>
        public Texture2D startLogo;

#if XBOX
        /// <summary>
        /// has the player pressed start
        /// 0 = no, 1 = yes, 2 = moving to next screen
        /// </summary>
        char done = (char)0;
#endif

        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
            startLogo = content.Load<Texture2D>("Graphics/Start");
        }

        #endregion


        #region Update & Draw
        
        public override void HandleInput(GameTime gameTime, InputManager input)
        {
#if XBOX
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
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, null);
            }
#else
            if (input.touches.Count > 0 && input.touches[0].state == TouchState.Pressed)
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, null);
#endif
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(startLogo, new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) - (startLogo.Width >> 1),
                (parent.GraphicsDevice.Viewport.Height >> 1) - (startLogo.Height >> 1)), Color.White);

            spriteBatch.End();
        }

        #endregion
    }
}

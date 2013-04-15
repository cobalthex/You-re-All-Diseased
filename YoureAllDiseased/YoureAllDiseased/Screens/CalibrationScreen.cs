//CalibrationScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace YoureAllDiseased
{
    /// <summary>
    /// The Calibration screen for zune/phone
    /// Screen displayed for variable time
    /// </summary>
    public class CalibrationScreen : GameScreen
    {
        #region Data

        /// <summary>
        /// Has this been calibrated (so the device isn't calibrated every level, handled outside of this class)
        /// </summary>
        public bool hasCalibrated = false;

        /// <summary>
        /// How long this screen should be alive for
        /// </summary>
        public TimeSpan aliveTime = new TimeSpan(0, 0, 2);

        /// <summary>
        /// The image saying "Calibrating..." displayed in the center of the screen
        /// </summary>
        Texture2D calibrationLogo;

        /// <summary>
        /// Who owns this screen
        /// </summary>
        public PlayScreen owner;

        /// <summary>
        /// The raw acceleration data for each frame
        /// </summary>
        Vector3[] accelData;
        /// <summary>
        /// current calibration frame (for the accelData array)
        /// </summary>
        int cFrame = 0;

        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
            screenType = ScreenType.Popup;

            calibrationLogo = content.Load<Texture2D>("Graphics/Calibrating");

            if (args != null && args.Count > 0)
                owner = (PlayScreen)args[0];

            hasCalibrated = false;

            accelData = new Vector3[aliveTime.Seconds * 30];
            Accelerometer.calibration = Vector3.Zero; //reset
        }

        #endregion


        #region Update & Draw

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool isVisible, bool isCovered)
        {

            if ((DateTime.UtcNow - screenStartTime).TotalMilliseconds > aliveTime.TotalMilliseconds)
            {
                Accelerometer.calibration = CalculateCalibration();
                this.screenState = ScreenState.Inactive;

                hasCalibrated = true;
            }
        }

        public override void HandleInput(GameTime gameTime, InputManager input)
        {
            //add to calibration data
            if (cFrame < accelData.Length)
                accelData[cFrame++] = input.accelReading;

            if (input.isBackButtonPressed)
            {
                Main.isMusicFading = true;
                this.screenState = ScreenState.Inactive;
                parent.NextScreen(owner, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            float alpha = 1;

            int t = (int)(DateTime.UtcNow - screenStartTime).TotalMilliseconds; //current time
            int t2 = (int)aliveTime.TotalMilliseconds >> 3; //time offset
            if (t < t2)
                alpha = (float)t / (float)t2;
            if (t > (int)aliveTime.TotalMilliseconds - t2)
                alpha = (int)aliveTime.TotalMilliseconds - (float)t / (float)t2;

            Color c = Color.White;
            c.A = (byte)(alpha * 255);

#if XNA31
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
#else
            spriteBatch.Begin();
#endif

            spriteBatch.Draw(calibrationLogo, new Vector2((parent.GraphicsDevice.Viewport.Width >> 1) - (calibrationLogo.Width >> 1),
                (parent.GraphicsDevice.Viewport.Height >> 1) - (calibrationLogo.Height >> 1)), c);

            spriteBatch.End();
        }

        #endregion

        /// <summary>
        /// Show the calibrator
        /// </summary>
        /// <param name="owner">who owns this screen</param>
        public void Show(PlayScreen owner)
        {
            screenState = ScreenState.Active;
            this.owner = owner;
            parent.Screens.Remove(this);
            parent.Screens.Add(this);
            hasCalibrated = false;
            Reset(); 
            accelData = new Vector3[aliveTime.Seconds * 30];
            Accelerometer.calibration = Vector3.Zero; //reset
            cFrame = 0;
        }

        /// <summary>
        /// Average all of the acceleration data together
        /// </summary>
        /// <returns>the calibration</returns>
        public Vector3 CalculateCalibration()
        {
            Vector3 calibration = Vector3.Zero;

            for (int i = 0; i < accelData.Length; i++)
                calibration += accelData[i];

            calibration /= accelData.Length;

            return calibration;
        }
    }
}

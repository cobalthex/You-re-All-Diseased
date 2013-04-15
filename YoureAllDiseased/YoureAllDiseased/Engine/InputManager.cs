//InputManager.cs
//Copyright Dejitaru Forge 2011

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#if WINDOWS
//accelerometer emulator

#endif

#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif

namespace YoureAllDiseased
{
    /// <summary>
    /// A simple virtualized input manager
    /// </summary>
    public class InputManager
    {
        /// <summary>
        /// Touches
        /// </summary>
        public System.Collections.Generic.List<Touch> touches;
        /// <summary>
        /// Previous touches
        /// </summary>
        public System.Collections.Generic.List<Touch> pTouches;

        /// <summary>
        /// Thread locking for reading accel data
        /// </summary>
        protected static object threadLock = new object();

        public PlayerIndex activePlayerIndex = PlayerIndex.One;

#if WINDOWS || XBOX
        public GamePadState gpState;
        public GamePadState pGPState;
#endif

#if WINDOWS_PHONE || ZUNE
        /// <summary>
        /// The raw touch data
        /// </summary>
        protected TouchCollection tchClctn;

#elif WINDOWS

        /// <summary>
        /// The raw mouse data
        /// </summary>
        protected MouseState mState;
#endif

#if !ZUNE //keyboard on all platforms except zune
        /// <summary>
        /// The raw keyboard data (for accelerometer)
        /// </summary>
        protected KeyboardState keyboard;
        protected KeyboardState pKeyboard;
#endif

        /// <summary>
        /// acceleration of device (keyboard arrows on windows)
        /// </summary>
        public Vector3 accelReading = Vector3.Zero;
        /// <summary>
        /// Is the acelerometer active?
        /// </summary>
        public bool accelerometerActive { get; protected set; }

        /// <summary>
        /// Is the back button (Windows phone/xbox) pressed? (Esc. on windows)
        /// </summary>
        public bool isBackButtonPressed = false;

        /// <summary>
        /// Create a new Input Manager
        /// </summary>
        public InputManager()
        {
#if WINDOWS || XBOX
            gpState = GamePad.GetState(activePlayerIndex);
            pGPState = GamePad.GetState(activePlayerIndex);
#endif

            touches = new System.Collections.Generic.List<Touch>(4);
            pTouches = new System.Collections.Generic.List<Touch>(4);

#if WINDOWS_PHONE
            Accelerometer.Initialize();
#endif
        }

#if ZUNE
        //multiplier for touch input on zune
        Vector2 mul = new Vector2(800, 480) / new Vector2(480, 272);
#endif

        /// <summary>
        /// Update all of the inputs
        /// </summary>
        public void Update()
        {
            pTouches.Clear();
            for (int i = 0; i < touches.Count; i++)
                pTouches.Add(touches[i]);
            touches.Clear();
            isBackButtonPressed = false;

#if WINDOWS_PHONE
            tchClctn = TouchPanel.GetState();

            for (int i = 0; i < tchClctn.Count; i++)
                touches.Add(new Touch(tchClctn[i].Position, 1, (uint)tchClctn[i].Id, (TouchState)tchClctn[i].State));
            
            if (Microsoft.Xna.Framework.Input.GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One).IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Back))
                isBackButtonPressed = true;

            lock (threadLock)
            {
                accelReading = Accelerometer.GetState().Acceleration;
            }
#elif ZUNE
            Vector3 accel = Microsoft.Xna.Framework.Input.Accelerometer.GetState().Acceleration;
            float sensitivity = 1; //how sensitive the accelerometer is (multiplier)
            accelReading = new Vector3(-accel.Y * sensitivity, -accel.X * sensitivity, accel.Z * sensitivity) - Accelerometer.calibration;
            tchClctn = TouchPanel.GetState();

            for (int i = 0; i < tchClctn.Count; i++)
            {
                Vector2 pos = tchClctn[i].Position;
                pos.Y = 272 - pos.X;
                pos.X = tchClctn[i].Position.Y;
                touches.Add(new Touch(pos * mul, tchClctn[i].Pressure, (uint)tchClctn[i].Id, (TouchState)tchClctn[i].State));
            }
#elif WINDOWS
            mState = Mouse.GetState();

            touches.Add(new Touch(new Vector2(mState.X, mState.Y), 1, 0, mState.LeftButton == ButtonState.Pressed ? TouchState.Pressed : TouchState.Released));
            touches.Add(new Touch(new Vector2(mState.X, mState.Y), 1, 1, mState.RightButton == ButtonState.Pressed ? TouchState.Pressed : TouchState.Released));
            touches.Add(new Touch(new Vector2(mState.X, mState.Y), 1, 2, mState.MiddleButton == ButtonState.Pressed ? TouchState.Pressed : TouchState.Released));
#endif

#if !ZUNE
            pKeyboard = keyboard;
            keyboard = Keyboard.GetState();
#endif

#if WINDOWS || XBOX         
            pGPState = gpState;
            gpState = GamePad.GetState(activePlayerIndex);
            if (gpState.IsButtonDown(Buttons.Back) && pGPState.IsButtonUp(Buttons.Back))
                isBackButtonPressed = true;


            if (keyboard.IsKeyDown(Keys.Escape) && pKeyboard.IsKeyUp(Keys.Escape))
                isBackButtonPressed = true;

            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
                accelReading.X = 1;
            else if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
                accelReading.X = -1;
            else
                accelReading.X = 0;

            if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
                accelReading.Y = 1;
            else if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
                accelReading.Y = -1;
            else
                accelReading.Y = 0;

            if (keyboard.IsKeyDown(Keys.RightShift) || keyboard.IsKeyDown(Keys.Q))
                accelReading.Z = 1;
            else if (keyboard.IsKeyDown(Keys.RightControl) || keyboard.IsKeyDown(Keys.E))
                accelReading.Z = -1;
            else
                accelReading.Z = 0;
#endif
        }
    }

    /// <summary>
    /// A single touch
    /// </summary>
    public struct Touch
    {
        /// <summary>
        /// The position on screen of the touch
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// How strong the touch is
        /// </summary>
        public float pressure;
        /// <summary>
        /// The ID of this touch
        /// </summary>
        public uint id;
        /// <summary>
        /// The current state of the object
        /// </summary>
        public TouchState state;

        /// <summary>
        /// Create a new touch
        /// </summary>
        /// <param name="Position">Position on screen</param>
        /// <param name="Pressure">How strong</param>
        /// <param name="Id">ID</param>
        /// <param name="State">State of touch</param>
        public Touch(Vector2 Position, float Pressure, uint Id, TouchState State)
        {
            position = Position;
            pressure = Pressure;
            id = Id;
            state = State;
        }
    }

    /// <summary>
    /// A simple list of touch states
    /// </summary>
    public enum TouchState
    {
        /// <summary>
        /// Invalid/unavailable
        /// </summary>
        Invalid,
        /// <summary>
        /// Released
        /// </summary>
        Released,
        /// <summary>
        /// Pressed/held
        /// </summary>
        Pressed,
        /// <summary>
        /// Moved away from previous point
        /// </summary>
        Moved
    }
}
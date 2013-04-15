//InputManager.cs
//Copyright Dejitaru Forge 2011

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MapEditor
{
    /// <summary>
    /// A simple input manager
    /// </summary>
    public class InputManager
    {
        /// <summary>
        /// Current keyboard state
        /// </summary>
        public KeyboardState kb;
        /// <summary>
        /// Last frame's keyboard state
        /// </summary>
        public KeyboardState pkb;

        /// <summary>
        /// Current mouse state
        /// </summary>
        public MouseState ms;
        /// <summary>
        /// Last frame's mouse state
        /// </summary>
        public MouseState pms;

        /// <summary>
        /// Create a new Input Manager
        /// </summary>
        public InputManager()
        {
            kb = Keyboard.GetState();
            pkb = new KeyboardState();

            ms = Mouse.GetState();
            pms = new MouseState();
        }

        /// <summary>
        /// Update all of the inputs
        /// </summary>
        public void Update()
        {
            pkb = kb;
            kb = Keyboard.GetState();

            pms = ms;
            ms = Mouse.GetState();
        }
    }
}
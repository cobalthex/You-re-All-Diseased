//Transition.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased
{
    /// <summary>
    /// A one-way screen transition fo the Game State Manager
    /// 
    /// note: transition automatically ends at end of first cycle of animation
    /// </summary>
    public class Transition : Animation
    {
        /// <summary>
        /// The reference graphics device
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.GraphicsDevice gd;

        /// <summary>
        /// Create a new transition
        /// </summary>
        /// <param name="Type">The measurement of time for the animation</param>
        /// <param name="FrameLength">The length of the transition (in the specified time type)</param>
        /// <param name="Frames">The total number of frames</param>
        /// <param name="GraphicsDevice">The graphics device used to drawing</param>
        public Transition(Microsoft.Xna.Framework.Graphics.GraphicsDevice GraphicsDevice, FrameTimeType Type, int FrameLength, 
            int Frames) : base(Type, 0, false, Frames, 0, FrameLength)
        {
            gd = GraphicsDevice;
        }

        public Transition() : base()
        {
            gd = null;
        }

        /// <summary>
        /// Draw the transition
        /// All logic should be handled here
        /// 
        /// note: does (should) not call Begin or End
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used to draw the transition</param>
        public virtual void Draw(ref Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
        }
    }
}

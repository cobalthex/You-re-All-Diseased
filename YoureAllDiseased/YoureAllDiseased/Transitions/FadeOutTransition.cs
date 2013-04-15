//FadeOutTransition.cs
//Copyright Dejitaru Forge 2011

#if XNA31
using Microsoft.Xna.Framework.Graphics;
#else
using Microsoft.Xna.Framework;
#endif

namespace YoureAllDiseased
{
    /// <summary>
    /// A simple fade out (to black) transition
    /// </summary>
    public class FadeOutTransition : Transition
    {
        public Microsoft.Xna.Framework.Graphics.Texture2D black;

        public FadeOutTransition(Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
            : base(gd, FrameTimeType.Milliseconds, 10, 20) //half a second transition
        {
            black = new Microsoft.Xna.Framework.Graphics.Texture2D(gd, 1, 1);
            black.SetData<Color>(new Color[] {Color.Black});
        }

        public override void Draw(ref Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            float alpha = (float)currentFrame / (float)frames;
            spriteBatch.Draw(black, new Microsoft.Xna.Framework.Rectangle(0, 0, gd.Viewport.Width,
                gd.Viewport.Height), new Color(0, 0, 0, (byte)(alpha * 255)));
        }
    }
}

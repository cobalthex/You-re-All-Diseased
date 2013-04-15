//LvlCompleteTransition.cs
//Copyright Dejitaru Forge 2011

#if XNA31
using Microsoft.Xna.Framework.Graphics;
#else
using Microsoft.Xna.Framework;
#endif

namespace YoureAllDiseased
{
    /// <summary>
    /// A simple fade effect for when the player beats the level
    /// </summary>
    public class LvlCompleteTransition : Transition
    {
        public Microsoft.Xna.Framework.Graphics.Texture2D white;

        public LvlCompleteTransition(Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
            : base(gd, FrameTimeType.Milliseconds, 40, 40)
        {
            white = new Microsoft.Xna.Framework.Graphics.Texture2D(gd, 1, 1);

            white.SetData<Color>(new Color[] { Color.White });
        }

        public override void Draw(ref Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            float alpha = 1 - (float)currentFrame / (float)frames;

            spriteBatch.Draw(white, new Microsoft.Xna.Framework.Rectangle(0, 0, gd.Viewport.Width, gd.Viewport.Height), new Color(alpha * 4, alpha, alpha));
        }
    }
}

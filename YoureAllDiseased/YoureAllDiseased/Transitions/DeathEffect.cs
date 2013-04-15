//DeathEffect.cs
//Copyright Dejitaru Forge 2011

#if XNA31
using Microsoft.Xna.Framework.Graphics;
#else
using Microsoft.Xna.Framework;
#endif

namespace YoureAllDiseased
{
    /// <summary>
    /// A simple fade effect for when the player dies (and respawns)
    /// </summary>
    public class DeathEffect : Transition
    {
        public Microsoft.Xna.Framework.Graphics.Texture2D red;

        public DeathEffect(Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
            : base(gd, FrameTimeType.Milliseconds, 10, 20)
        {
            red = new Microsoft.Xna.Framework.Graphics.Texture2D(gd, 1, 1);

            red.SetData<Color>(new Color[] { Color.Red });
        }

        public override void Draw(ref Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            float alpha = (float)currentFrame / (float)(frames >> 1);
            if (currentFrame > frames >> 1)
                alpha = 1 - (float)(currentFrame - (frames >> 1)) / (float)(frames >> 1);

            spriteBatch.Draw(red, new Microsoft.Xna.Framework.Rectangle(0, 0, gd.Viewport.Width, gd.Viewport.Height), new Color(1, 1, 1, alpha));
        }
    }
}

//GameOverTransition.cs
//Copyright Dejitaru Forge 2011

#if XNA31
using Microsoft.Xna.Framework.Graphics;
#else
using Microsoft.Xna.Framework;
#endif

namespace YoureAllDiseased
{
    /// <summary>
    /// A simple fade in (from black) transition
    /// </summary>
    public class GameOverTransition : Transition
    {
        public Microsoft.Xna.Framework.Graphics.Texture2D red;

        public GameOverTransition(Microsoft.Xna.Framework.Graphics.GraphicsDevice gd) : 
            base(gd, FrameTimeType.Milliseconds, 10, 20) //half a second transition
        {
            red = new Microsoft.Xna.Framework.Graphics.Texture2D(gd, 1, 1);
            red.SetData<Color>(new Color[] {Color.Red});
        }

        public override void Draw(ref Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            float alpha = ((float)currentFrame / (float)frames);
            spriteBatch.Draw(red, new Microsoft.Xna.Framework.Rectangle(0, 0, gd.Viewport.Width, gd.Viewport.Height), alpha > 0.5 ? Color.Red : new Color(1, 1, 1, alpha * 2));
            if (currentFrame > frames >> 1)
            spriteBatch.Draw(red, new Microsoft.Xna.Framework.Rectangle(0, 0, gd.Viewport.Width, gd.Viewport.Height), new Color(0, 0, 0, (byte)(((alpha - 0.5) * 2) * 255)));
        }
    }
}

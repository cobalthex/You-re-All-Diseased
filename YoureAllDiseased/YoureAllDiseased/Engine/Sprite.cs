//Sprite.cs
//Copyright DejitaruForge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoureAllDiseased
{
    /// <summary>
    /// An animatable sprite class
    /// </summary>
    public class Sprite : Animation
    {
        #region Data

        /// <summary>
        /// the image for the sprite
        /// </summary>
        public Texture2D texture { get; protected set; }

        /// <summary>
        /// position on screen
        /// </summary>
        public Rectangle position = Rectangle.Empty;

        /// <summary>
        /// location of sprite in image, and/or frame size & offset
        /// </summary>
        public Rectangle frameSize = Rectangle.Empty;
        /// <summary>
        /// Is the sprite visible
        /// </summary>
        public bool isVisible = true;

        /// <summary>
        /// Any mirroring options (vert/horiz)
        /// </summary>
        public SpriteEffects mirrorOptions;
        /// <summary>
        /// Rotation of the image
        /// </summary>
        public float rotation;
        /// <summary>
        /// The origin (only use for rotation)
        /// </summary>
        public Vector2 origin;

        /// <summary>
        /// Color tint for this sprite
        /// </summary>
        public Color hue;

        #endregion


        #region Initialization

        /// <summary>
        /// Create a new sprite
        /// </summary>
        /// <param name="Texture">The sprite image</param>
        /// <param name="Position">Position on screen (and sizing, leave 0 for default size)</param>
        /// <param name="frames">The number of frames (less than 2 for not animated)</param>
        /// <param name="FrameSize">The size of individual frames and/or the sprite and offset (leave width and height empty for whole image (use only for single frame images)</param>
        /// <param name="Delay">The length between each frame</param>
        /// <param name="FrameType">The time measurement for animation</param>
        /// <param name="Length">The length of each frame</param>
        /// <param name="Looping">Is the animation looping?</param>
        /// <param name="StartFrame">The frame that the animation starts on</param>
        public Sprite(Texture2D Texture, Rectangle Position, int frames, Rectangle FrameSize, FrameTimeType FrameType, int Length, float Delay, bool Looping, int StartFrame) :
            base(FrameType, Delay, Looping, frames, StartFrame, Length)
        {
            texture = Texture;
            frameSize = FrameSize;
            position = Position;

            if (frameSize.Width == 0)
                frameSize.Width = texture.Width;
            if (frameSize.Height == 0)
                frameSize.Height = texture.Height;

            if (position.Width == 0)
                position.Width = frameSize.Width;
            if (position.Height == 0)
                position.Height = frameSize.Height;

            hue = Color.White;
            origin = new Vector2(frameSize.Width >> 1, frameSize.Height >> 1);
        }

        #endregion


        #region Draw

        /// <summary>
        /// Draw the sprite (but does not update)
        /// </summary>
        /// <param name="spriteBatch">sprite batch to use (does not call begin/end)</param>
        public virtual void Draw(ref SpriteBatch spriteBatch)
        {
            UpdateFrame();

            if (currentFrame == -1)
                return;

            if (frames > 1) //animated
            {
                int framesPerRow = texture.Width / frameSize.Width;
                spriteBatch.Draw(texture, position, new Rectangle(frameSize.X + frameSize.Width * (int)(currentFrame % framesPerRow),
                    frameSize.Y + frameSize.Height * (int)(currentFrame / framesPerRow), frameSize.Width, frameSize.Height), hue, rotation, origin, mirrorOptions, 0);
            }
            else
                spriteBatch.Draw(texture, position, frameSize, hue, rotation, origin, mirrorOptions, 0);
        }

        /// <summary>
        /// Draw the sprite
        /// </summary>
        /// <param name="spriteBatch">spritebatch to use (does not call begin/end)</param>
        /// <param name="pos">specify coords (if w|h == 0, uses texture size)</param>
        /// <param name="angle">angle to draw at</param>
        public virtual void Draw(ref SpriteBatch spriteBatch, Rectangle pos, float angle)
        {
            UpdateFrame();

            if (currentFrame == -1)
                return;

            if (pos.Width == 0)
                pos.Width = frameSize.Width;
            if (pos.Height == 0)
                pos.Height = frameSize.Height;

            if (frames > 1) //animated
            {
                int framesPerRow = texture.Width / frameSize.Width;
                spriteBatch.Draw(texture, pos, new Rectangle(frameSize.X + frameSize.Width * (int)(currentFrame % framesPerRow),
                    frameSize.Y + frameSize.Height * (int)(currentFrame / framesPerRow), frameSize.Width, frameSize.Height), hue, angle + rotation, origin, mirrorOptions, 0);
            }
            else
                spriteBatch.Draw(texture, pos, frameSize, hue, angle + rotation, origin, mirrorOptions, 0);
        }

        #endregion
    }
}

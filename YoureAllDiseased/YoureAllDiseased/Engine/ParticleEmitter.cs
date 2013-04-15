//ParticleEmitter.cs
//Copyright Dejitaru Forge 2011

#if XNA31
using Microsoft.Xna.Framework.Graphics;
#endif

namespace YoureAllDiseased
{
    /// <summary>
    /// A single type of particle
    /// </summary>
    public class ParticleType
    {
        /// <summary>
        /// The image for this particle
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.Texture2D texture;
        /// <summary>
        /// width of the texture
        /// </summary>
        public int width;
        /// <summary>
        /// height of the texture
        /// </summary>
        public int height;
        /// <summary>
        /// A hue to the texture
        /// </summary>
#if XNA31
        public Microsoft.Xna.Framework.Graphics.Color hue;
#else
        public Microsoft.Xna.Framework.Color hue;
#endif

        /// <summary>
        /// The type of blending to use when rendering
        /// </summary>
#if XNA31
        public Microsoft.Xna.Framework.Graphics.SpriteBlendMode blendState;
#else
        public Microsoft.Xna.Framework.Graphics.BlendState blendState;
#endif

        /// <summary>
        /// Create a new particle type
        /// </summary>
        /// <param name="Texture">The texture to use for the particles</param>
        /// <param name="Hue">Any color effects to be applied</param>
        /// <param name="BlendState">The blending to use for these particles</param>
#if XNA31
        public ParticleType(Microsoft.Xna.Framework.Graphics.Texture2D Texture, Microsoft.Xna.Framework.Graphics.Color Hue,
            Microsoft.Xna.Framework.Graphics.SpriteBlendMode BlendState)
#else
        public ParticleType(Microsoft.Xna.Framework.Graphics.Texture2D Texture, Microsoft.Xna.Framework.Color Hue,
            Microsoft.Xna.Framework.Graphics.BlendState BlendState)
#endif
        {
            texture = Texture;
            width = Texture.Width;
            height = Texture.Height;
            hue = Hue;
            blendState = BlendState;
        }
    }

    /// <summary>
    /// A single particle emitter
    /// </summary>
    public class ParticleEmitter
    {
        /// <summary>
        /// The type of particles to emit
        /// </summary>
        public ParticleType type;
        /// <summary>
        /// the list of the particles
        /// Format: {x, y, velocity (if negative, represents time out, per frame), angle (in radians)}
        /// </summary>
        public System.Collections.Generic.List<Microsoft.Xna.Framework.Vector4> particles;

        /// <summary>
        /// How far the particle can be from the origin before it is destroyed
        /// </summary>
        public int lifeSpan;

        /// <summary>
        /// how much to lower from velocity each frame (positive)
        /// </summary>
        public float decay = 0;

        /// <summary>
        /// Speed multiplier for all particles
        /// </summary>
        public float speedMultiplier = 1;

        /// <summary>
        /// Amount of gravity (0 for none, down/right = +)
        /// </summary>
        public Microsoft.Xna.Framework.Vector2 gravity = Microsoft.Xna.Framework.Vector2.Zero;

        /// <summary>
        /// 4
        /// </summary>
        System.Random r = new System.Random();

        /// <summary>
        /// Create an empty particle emitter
        /// </summary>
        /// <param name="Type">The type of particle</param>
        public ParticleEmitter(ParticleType Type)
        {
            type = Type;
            particles = new System.Collections.Generic.List<Microsoft.Xna.Framework.Vector4>(256);
        }

        /// <summary>
        /// Create a new particle emitter
        /// </summary>
        /// <param name="Type">The type of particle</param>
        /// <param name="numParticles">The number of particles to start with</param>
        /// <param name="maxAngle">Maximum angle to spawn from 0 rads</param>
        /// <param name="minAngle">Minimum angle to spawn from 0 rads</param>
        /// <param name="Origin">Where to spawn</param>
        /// <param name="maxVelocity">Maximum velocity of particle</param>
        /// <param name="minVelocity">Minimum velocity of particle</param>
        /// <param name="LifeSpan">Max distance from origin before destruction</param>
        public ParticleEmitter(ParticleType Type, int numParticles, Microsoft.Xna.Framework.Vector2 Origin, int minVelocity, int maxVelocity, float minAngle, float maxAngle, int LifeSpan)
        {
            type = Type;

            particles = new System.Collections.Generic.List<Microsoft.Xna.Framework.Vector4>(256);
            
            for (int i = 0; i < numParticles; i++)
                particles.Add(new Microsoft.Xna.Framework.Vector4(Origin, r.Next(minVelocity, maxVelocity), 
                    minAngle + (float)(r.NextDouble() * (maxAngle - minAngle))));

            lifeSpan = LifeSpan;
        }

        /// <summary>
        /// Add more particle to the emitter
        /// </summary>
        /// <param name="numParticles">The number of particles to start with</param>
        /// <param name="maxAngle">Maximum angle to spawn from 0 rads</param>
        /// <param name="minAngle">Minimum angle to spawn from 0 rads</param>
        /// <param name="Origin">Where to spawn</param>
        /// <param name="maxVelocity">Maximum velocity of particle</param>
        /// <param name="minVelocity">Minimum velocity of particle</param>
        public virtual void Particulate(int numParticles, Microsoft.Xna.Framework.Vector2 Origin, int minVelocity, int maxVelocity, float minAngle, float maxAngle)
        {
            for (int i = 0; i < numParticles; i++)
                particles.Add(new Microsoft.Xna.Framework.Vector4(Origin, r.Next(minVelocity, maxVelocity),
                    minAngle + (float)(r.NextDouble() * (maxAngle - minAngle))));
        }

        /// <summary>
        /// Draw and update all of the particles (use own spritebatch)
        /// </summary>
        /// <param name="gameTime">game time</param>
        /// <param name="gDev">Graphics device</param>
        /// <param name="drawPos">Where to draw relative to rest of the map</param>
        public virtual void Draw(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.GraphicsDevice gDev, Microsoft.Xna.Framework.Vector2 drawPos)
        {
            Microsoft.Xna.Framework.Graphics.SpriteBatch sB = new Microsoft.Xna.Framework.Graphics.SpriteBatch(gDev);
#if XNA31
            sB.Begin(type.blendState);
#else
            sB.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Deferred, type.blendState);
#endif

            for (int i = 0; i < particles.Count; i++)
            {
                float x = particles[i].X + particles[i].Z * (float)System.Math.Cos(particles[i].W) + gravity.X;
                float y = particles[i].Y + particles[i].Z * (float)System.Math.Sin(particles[i].W) + gravity.Y;
                particles[i] = new Microsoft.Xna.Framework.Vector4(x, y, particles[i].Z < 0 ? particles[i].Z + decay : particles[i].Z - decay, particles[i].W);

#if ZUNE
                if (System.Math.Abs(particles[i].Z) < decay || (decay < 1 && !new Microsoft.Xna.Framework.Rectangle(0, 0, 800, 480).
                    Contains((int)(particles[i].X - drawPos.X), (int)(particles[i].Y - drawPos.Y))))
#else
                if (System.Math.Abs(particles[i].Z) < decay || (decay < 1 && !new Microsoft.Xna.Framework.Rectangle(0, 0, gDev.Viewport.Width, gDev.Viewport.Height).
                    Contains((int)(particles[i].X - drawPos.X), (int)(particles[i].Y - drawPos.Y))))
#endif
                {
                    particles.RemoveAt(i--);
                    continue;
                }

                sB.Draw(type.texture, new Microsoft.Xna.Framework.Rectangle((int)(particles[i].X - drawPos.X), (int)(particles[i].Y - drawPos.Y), type.width, type.height),
                    new Microsoft.Xna.Framework.Rectangle(0, 0, type.width, type.height), type.hue, particles[i].W + particles[i].Z, 
                    new Microsoft.Xna.Framework.Vector2(type.width >> 1, type.height >> 1), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            }

            sB.End();
        }
    }
}

//MucusPickup.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Powerups.Weapons
{
    /// <summary>
    /// A package of mucus to shoot
    /// </summary>
    public class MucusPickup : Entity
    {
        /// <summary>
        /// How much ammo is defaulted to pickup
        /// </summary>
        public static int defaultAmmoCount = 128;

        /// <summary>
        /// how much ammo this pickup has
        /// </summary>
        public int ammoCount;

        /// <summary>
        /// Is this the first time that a mucus pickup has been picked up. If so, a message will be shown on how to use it
        /// </summary>
        public static bool hasCollectedBefore = false;

        /// <summary>
        /// Create a new mucus pickup
        /// </summary>
        public MucusPickup()
            : base("Mucus Pickup", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 0, -1, 0)
        {
            canHit = false;
            currentHealth = 1;
            ammoCount = defaultAmmoCount;
        }

        /// <summary>
        /// Create a new granzyme pickup
        /// </summary>
        /// <param name="AmmoCount">how much ammo this has</param>
        public MucusPickup(int AmmoCount)
            : base("Mucus Pickup", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 0, -1, 0)
        {
            canHit = false;
            currentHealth = 1;
            ammoCount = AmmoCount;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Powerups/Weapons/MucusPickup"), Microsoft.Xna.Framework.Rectangle.Empty, 6,
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), FrameTimeType.Milliseconds, 100, 0, true, 0);
        }

        public override void OnDie(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (owner.player.currentMucus > 0)
                owner.mucusMeterAnim = 34;
            else
                owner.mucusMeterAnim = 1;

            owner.player.currentMucus += ammoCount;

            if (OptionsScreen.showHints && !hasCollectedBefore)
            {
                hasCollectedBefore = true;
                PlayScreen.helpCreationTime = System.DateTime.UtcNow.Ticks;
#if XBOX
                PlayScreen.helpTxt = "Hold the right trigger to\nactivate mucus mode";
#else
                PlayScreen.helpTxt = "Press The green drop to\nactivate/deactivate mucus mode";
#endif
                PlayScreen.helpTxtPos = new Microsoft.Xna.Framework.Vector2(140, 80);
                PlayScreen.helpShowTime = 5;
            }
        }

        //particle animation
        float n = -18;
        bool neg = false;
        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (neg)
                n -= 0.5f;
            else
                n += 0.5f;

            if (n > 18)
                neg = true;
            else if (n < -18)
                neg = false;

            owner.weaponParticles.Particulate(2 * owner.particleMultiplier, position + new Microsoft.Xna.Framework.Vector2(0, n), 2, 6, 0 - .2f, 0 + .2f);
            owner.weaponParticles.Particulate(2 * owner.particleMultiplier, position + new Microsoft.Xna.Framework.Vector2(n, 0), 2, 6, Microsoft.Xna.Framework.MathHelper.PiOver2 - .2f, Microsoft.Xna.Framework.MathHelper.PiOver2 + .2f);
            owner.weaponParticles.Particulate(2 * owner.particleMultiplier, position - new Microsoft.Xna.Framework.Vector2(0, n), 2, 6, Microsoft.Xna.Framework.MathHelper.Pi - .2f, Microsoft.Xna.Framework.MathHelper.Pi + .2f);
            owner.weaponParticles.Particulate(2 * owner.particleMultiplier, position - new Microsoft.Xna.Framework.Vector2(n, 0), 2, 6, -Microsoft.Xna.Framework.MathHelper.PiOver2 - .2f, -Microsoft.Xna.Framework.MathHelper.PiOver2 + .2f);
        }
    }

    /// <summary>
    /// A single mucus blob
    /// </summary>
    public struct Mucus
    {
        /// <summary>
        /// x velocity
        /// </summary>
        public float x;
        /// <summary>
        /// y position
        /// </summary>
        public float y;
        /// <summary>
        /// X velocity
        /// </summary>
        public float vX;
        /// <summary>
        /// Y velocity
        /// </summary>
        public float vY;
        /// <summary>
        /// size of mucus blob
        /// </summary>
        public int size;
        /// <summary>
        /// when this blob was created
        /// </summary>
        public long creationTime; 

        /// <summary>
        /// Create a new blob of mucus
        /// </summary>
        /// <param name="X">X position</param>
        /// <param name="Y">Y position</param>
        /// <param name="VX">X velocity</param>
        /// <param name="VY">Y Velocity</param>
        /// <param name="Size">Size of blob</param>
        /// <param name="CreationTime">When this was created</param>
        public Mucus(float X, float Y, float VX, float VY, int Size, long CreationTime)
        {
            x = X;
            y = Y;
            vX = VX;
            vY = VY;
            size = Size;
            creationTime = CreationTime;
        }
    }
}

//DeathMachinePickup.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Powerups.Weapons
{
    /// <summary>
    /// A package of granzyme granules
    /// </summary>
    public class DeathMachinePickup : Entity
    {
        /// <summary>
        /// How much ammo is defaulted to pickup
        /// </summary>
        public static int defaultAmmoCount = 200;

        /// <summary>
        /// how much ammo this pickup has
        /// </summary>
        public int ammoCount;

        /// <summary>
        /// Create a new death machine pickup
        /// </summary>
        public DeathMachinePickup()
            : base("Death Machine Pickup", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), 0, -1, 0)
        {
            canHit = false;
            currentHealth = 1;
            ammoCount = defaultAmmoCount;
        }

        /// <summary>
        /// Create a new death machine pickup
        /// </summary>
        /// <param name="AmmoCount">how much ammo this has</param>
        public DeathMachinePickup(int AmmoCount)
            : base("Death Machine Pickup", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), 0, -1, 0)
        {
            canHit = false;
            currentHealth = 1;
            ammoCount = AmmoCount;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Powerups/Weapons/DeathMachinePickup"), Microsoft.Xna.Framework.Rectangle.Empty, 1,
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), FrameTimeType.Milliseconds, 100, 0, true, 0);
        }

        public override void OnDie(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            owner.player.currentWeapon = Weapon.DeathMachine;
            owner.player.currentAmmo = ammoCount;
            owner.player.maxAmmo = ammoCount;
            owner.player.weaponDelay = 5;
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
}

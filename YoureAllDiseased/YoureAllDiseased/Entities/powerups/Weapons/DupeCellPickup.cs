//DupeCellPickup.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Powerups.Weapons
{
    /// <summary>
    /// A package of duplicating cells
    /// </summary>
    public class DupeCellPickup : Entity
    {
        /// <summary>
        /// How much ammo is defaulted to pickup
        /// </summary>
        public static int defaultAmmoCount = 50;

        /// <summary>
        /// how much ammo this pickup has
        /// </summary>
        public int ammoCount;

        /// <summary>
        /// has this powerup been seen before (if false, shows a tip)
        /// </summary>
        public static bool hasSeenBefore = false;

        /// <summary>
        /// Create a new dupe cell pickup
        /// </summary>
        public DupeCellPickup()
            : base("Duplicating Cell Pickup", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 48), 0, -1, 0)
        {
            canHit = false;
            currentHealth = 1;
            ammoCount = defaultAmmoCount;
        }

        /// <summary>
        /// Create a new dupe cell pickup
        /// </summary>
        /// <param name="AmmoCount">how much ammo this has</param>
        public DupeCellPickup(int AmmoCount)
            : base("Duplicating Cell Pickup", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 48), 0, -1, 0)
        {
            canHit = false;
            currentHealth = 1;
            ammoCount = AmmoCount;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Powerups/Weapons/DupeCellPickup"), Microsoft.Xna.Framework.Rectangle.Empty, 8,
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 48), FrameTimeType.Milliseconds, 100, 0, true, 0);
        }

        public override void OnDie(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            owner.player.currentWeapon = Weapon.DupeCell;
            owner.player.currentAmmo = ammoCount;
            owner.player.maxAmmo = ammoCount;
            owner.player.weaponDelay = 300;
        }

        //particle animation
        float n = -18;
        bool neg = false;
        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (OptionsScreen.showHints && !hasSeenBefore && Microsoft.Xna.Framework.Vector2.DistanceSquared(owner.player.position, position) < 50000)
            {
                hasSeenBefore = true;
                PlayScreen.helpCreationTime = System.DateTime.UtcNow.Ticks;
                PlayScreen.helpShowTime = 5;
                PlayScreen.helpTxt = "This 'glowing' blob is a powerup.\nMove over it to use it.";
                PlayScreen.helpTxtPos = new Microsoft.Xna.Framework.Vector2(100, 140);
            }

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

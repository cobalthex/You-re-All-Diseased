//HealthPowerup.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Powerups
{
    /// <summary>
    /// A single health regenerator for the player
    /// </summary>
    public class HealthPowerup : Entity
    {
        /// <summary>
        /// How powerful this powerup is
        /// </summary>
        public int charge = 30;

        /// <summary>
        /// Create a new health powerup
        /// </summary>
        /// <param name="Charge">How powerful this charge is</param>
        public HealthPowerup(int Charge) : base("Health regenerator", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), 0, -1, 0)
        {
            charge = Charge;
            canHit = false;
            currentHealth = 1;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Powerups/Health"), Microsoft.Xna.Framework.Rectangle.Empty, 13,
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), FrameTimeType.Milliseconds, 100, 0, true, 0);
        }

        public override void OnDie(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            owner.player.currentHealth += charge;

            if (owner.player.currentHealth > owner.player.maxHealth * 1.5)
                owner.player.currentLives++;
            if (owner.player.currentHealth > owner.player.maxHealth) //make sure health stays in valid range
                owner.player.currentHealth = owner.player.maxHealth;
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

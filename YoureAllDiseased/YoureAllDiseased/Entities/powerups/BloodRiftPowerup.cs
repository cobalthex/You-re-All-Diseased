//BloodRiftPowerup.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Powerups
{
    /// <summary>
    /// A single health regenerator for the player
    /// </summary>
    public class BloodRiftPowerup : Entity
    {
        /// <summary>
        /// How powerful this powerup is (per sec)
        /// </summary>
        public int charge = 3;

        /// <summary>
        /// has this powerup been seen before (if false, shows a tip)
        /// </summary>
        public static bool hasSeenBefore = false;

        /// <summary>
        /// Create a new blood rift
        /// </summary>
        /// <param name="Charge">How powerful this charge is</param>
        public BloodRiftPowerup(int Charge) : base("Blood Rift", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 96, 22), 0, -1, 0)
        {
            charge = Charge;
            canHit = false;
            currentHealth = 2; //does not destroy on collision (1 does)
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Powerups/Bloodrift"), Microsoft.Xna.Framework.Rectangle.Empty, 2,
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 96, 22), FrameTimeType.Milliseconds, 100, 0, true, 0);
            sprite.origin = new Microsoft.Xna.Framework.Vector2(sprite.frameSize.Width >> 1, sprite.frameSize.Height >> 1);
        }

        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (OptionsScreen.showHints && !hasSeenBefore && Microsoft.Xna.Framework.Vector2.DistanceSquared(owner.player.position, position) < 50000)
            {
                hasSeenBefore = true;
                PlayScreen.helpCreationTime = System.DateTime.UtcNow.Ticks;
                PlayScreen.helpShowTime = 3;
                PlayScreen.helpTxt = "This rift is a health regeneration area.\nMove onto it to use it.";
                PlayScreen.helpTxtPos = new Microsoft.Xna.Framework.Vector2(100, 140);
            }

            if (new Microsoft.Xna.Framework.Rectangle((int)position.X - (size.Width >> 1), (int)position.Y - (size.Height >> 1), size.Width, size.Height).
                Contains((int)owner.player.position.X, (int)owner.player.position.Y))
                owner.player.currentHealth += charge;

            if (owner.player.currentHealth > owner.player.maxHealth) //make sure health stays in valid range
                owner.player.currentHealth = owner.player.maxHealth;
        }
    }
}

//Syphilis.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Enemies
{
    /// <summary>
    /// Syphilis
    /// </summary>
    public class Syphilis : Entity
    {
        /// <summary>
        /// The last time the player respawned
        /// </summary>
        public long respawnTime = 0;

        float maxVelocity = 5;

        public Syphilis()
            : base("Syphilis", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 36, 70), 0, 40, 1)
        {
            pointsOnDeath = 100;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/Syphilis"), Microsoft.Xna.Framework.Rectangle.Empty, 4,
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 36, 70), FrameTimeType.Milliseconds, 100, 0, true, 0);
        }

        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (!CanSee(owner.player.position, position, ref owner.map))
                return;

            if (respawnTime != 0)
            {
                if ((System.DateTime.UtcNow.Ticks - respawnTime) / 10000 < 1000) //safety
                    isInvulnerable = true;
                else
                {
                    isInvulnerable = false;
                    respawnTime = 0;
                }
            }

            Microsoft.Xna.Framework.Vector2 length = owner.player.position - position;
            float ang = (float)System.Math.Atan2(length.Y, length.X);
            velocity = maxVelocity * new Microsoft.Xna.Framework.Vector2((float)System.Math.Cos(ang), (float)System.Math.Sin(ang));

            if (System.Math.Abs(length.X) < 36 && (length.Y) < 70)
            {
                if (!owner.player.isInvulnerable)
                    owner.player.currentHealth -= 40;
                currentHealth = 0;
                currentLives = 0;
            }
        }
    }
}
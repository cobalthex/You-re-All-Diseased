//BonusCoin.cs
//Copyright Dejitaru Forge 2011

using Microsoft.Xna.Framework;

namespace YoureAllDiseased.Entities.Powerups
{
    /// <summary>
    /// A bonus coin
    /// </summary>
    public class BonusCoin : Entity
    {
        /// <summary>
        /// How powerful this powerup is
        /// </summary>
        const int points = 100;

        /// <summary>
        /// Create a new bonus coin
        /// </summary>
        public BonusCoin() : base("Bonus Coin", Vector2.Zero, new Rectangle(0, 0, 48, 48), 0, -1, 0)
        {
            canHit = false;
            currentHealth = 1;
            pointsOnDeath = points;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Powerups/BonusCoin"), Rectangle.Empty, 10,
                   new Rectangle(0, 0, 48, 48), FrameTimeType.Milliseconds, 50, 0, true, 0);
        }

        public override void Think(GameTime gameTime, PlayScreen owner)
        {
            position += new Vector2((int)(2 * System.Math.Cos(gameTime.TotalGameTime.TotalMilliseconds / 100 % MathHelper.TwoPi)), (int)(2 * System.Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / 100 % MathHelper.TwoPi)));
        }
    }
}

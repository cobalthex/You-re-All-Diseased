//WestNile.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Enemies
{
    /// <summary>
    /// West Nile Virus
    /// </summary>
    public class WestNile : Entity
    {
        public WestNile()
            : base("WestNile", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 56), 0, 150, 1)
        {
            pointsOnDeath = 180;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/WestNile"), Microsoft.Xna.Framework.Rectangle.Empty, 3,
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 56), FrameTimeType.Milliseconds, 100, 0, true, 0);
        }

        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            //if (!CanSee(owner.player.position, position, ref owner.map))
            //   return;

            if (gameTime.TotalGameTime.TotalMilliseconds % 1500 < 30)
            {
                for (int i = 0; i < 8; i++)
                {
                    float ang = i * Microsoft.Xna.Framework.MathHelper.PiOver4;
                    owner.shots.Add(new Microsoft.Xna.Framework.Vector4((int)Weapon.Decay, position.X + 40 * (float)System.Math.Cos(ang), position.Y + 40 * (float)System.Math.Sin(ang), ang));
                }
            }

            velocity = 3 * new Microsoft.Xna.Framework.Vector2((float)owner.parent.random.NextDouble() - 0.5f, (float)owner.parent.random.NextDouble() - 0.5f);
        }
    }
}
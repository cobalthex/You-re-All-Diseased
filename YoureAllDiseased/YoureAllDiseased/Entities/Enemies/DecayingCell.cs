//DecayingCell.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Enemies
{
    /// <summary>
    /// An even more upgraded version of the infected cell. It has more health and can shoot at the player
    /// </summary>
    public class DecayingCell : Entity
    {
        float maxVelocity = 2;

        public DecayingCell() : base("Decaying Cell", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 96, 96), 0, 60, 1)
        {
            pointsOnDeath = 120;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/DecayingCell"), Microsoft.Xna.Framework.Rectangle.Empty, 6, 
                new Microsoft.Xna.Framework.Rectangle(0, 0, 96, 96), FrameTimeType.Milliseconds, 75, 0, true, 0);
        }

        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            Microsoft.Xna.Framework.Vector2 ln = owner.player.position - position;
            float a = (float)System.Math.Atan2(ln.Y, ln.X);

            float dist = Microsoft.Xna.Framework.Vector2.Distance(owner.player.position, position);
            if (dist < 800)
            {
                 Microsoft.Xna.Framework.Vector2 length = owner.player.position - position;

                 if (CanSee(position, owner.player.position, ref owner.map))
                 {
                     velocity.X = maxVelocity * (float)System.Math.Cos(length.X / dist);
                     velocity.Y = -maxVelocity * (float)System.Math.Sin(length.Y / dist);

                     if (length.X < 0 && velocity.X > 0)
                         velocity.X *= -1;

                     if (gameTime.TotalGameTime.Seconds % 2 == 0 && gameTime.TotalGameTime.Milliseconds < 50)
                     {
                         ln = (owner.player.position + owner.player.velocity) - position;
                         a = (float)System.Math.Atan2(ln.Y, ln.X);
                         float x = 50 * (float)System.Math.Cos(a);
                         float y = 50 * (float)System.Math.Sin(a);
                         owner.shots.Add(new Microsoft.Xna.Framework.Vector4((int)Weapon.Decay, position.X + x, position.Y + y, a));
                     }
                 }
            }
        }
    }
}
//Influenza.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Enemies
{
    /// <summary>
    /// The flu
    /// </summary>
    public class Influenza : Entity
    {
        public Influenza()
            : base("Influenza", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 48), 0, 60, 1)
        {
            pointsOnDeath = 60;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/Influenza"), Microsoft.Xna.Framework.Rectangle.Empty, 3,
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 48), FrameTimeType.Milliseconds, 100, 0, true, 0);
        }

        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (!CanSee(owner.player.position, position, ref owner.map))
                return;

            float dist = Microsoft.Xna.Framework.Vector2.Distance(owner.player.position, position);
            if (dist < 48)
            {
                owner.player.velocity /= 2;

                if (gameTime.TotalGameTime.TotalMilliseconds % 500 < 16 && !owner.player.isInvulnerable)
                    owner.player.currentHealth -= 40;
            }


            Microsoft.Xna.Framework.Vector2 ln = position - owner.player.position;
            float atan = (float)System.Math.Atan2(ln.Y, ln.X) + Microsoft.Xna.Framework.MathHelper.Pi;
            //follow player
            float maxTrackAngle = 0.05f; //how much angle to give when following player

            if (System.Math.Abs(atan - angle) < maxTrackAngle)
                angle = atan;
            else
            {
                if (angle < atan)
                    angle += maxTrackAngle;
                else if (angle > atan)
                    angle -= maxTrackAngle;
            }

            velocity.X = 3 * (float)System.Math.Cos(angle);
            velocity.Y = 3 * (float)System.Math.Sin(angle);
        }
    }
}
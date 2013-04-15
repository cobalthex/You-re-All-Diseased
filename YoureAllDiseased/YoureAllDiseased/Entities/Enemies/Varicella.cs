//Varicella.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Enemies
{
    /// <summary>
    /// Chickenpox
    /// </summary>
    public class Varicella : Entity
    {
        public Varicella()
            : base("Varicella", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 54), 0, 220, 1)
        {
            pointsOnDeath = 160;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/Varicella"), Microsoft.Xna.Framework.Rectangle.Empty, 1,
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 54), FrameTimeType.Milliseconds, 100, 0, true, 0);
        }

        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (!CanSee(owner.player.position, position, ref owner.map))
                return;

            var len = owner.player.position - position;

            float dist = Microsoft.Xna.Framework.Vector2.Distance(position, owner.player.position);

            if (dist < 72)
                angle += Microsoft.Xna.Framework.MathHelper.PiOver4 / 2;
            else
                angle += Microsoft.Xna.Framework.MathHelper.PiOver4 / 4;

            angle %= Microsoft.Xna.Framework.MathHelper.TwoPi;

            //attract player
            if (dist < 96)
            {
                float ang = (float)System.Math.Atan2(len.Y, len.X);

                //attract player somewhat
                if (dist > 32)
                owner.player.velocity -= new Microsoft.Xna.Framework.Vector2((float)System.Math.Cos(ang), (float)System.Math.Sin(ang));

                //if blade side is facing player, cut them
                if (dist < 48)
                {
                    if (!owner.player.isInvulnerable && Microsoft.Xna.Framework.MathHelper.WrapAngle(System.Math.Abs(angle - ang)) < Microsoft.Xna.Framework.MathHelper.PiOver2)
                        owner.player.currentHealth -= 2;

                    owner.explosionParticles.Particulate(2 * owner.particleMultiplier, (position + owner.player.position) / 2, 2, 10,
                        ang - Microsoft.Xna.Framework.MathHelper.PiOver2, ang + Microsoft.Xna.Framework.MathHelper.PiOver2);
                }
            }
        }
    }
}
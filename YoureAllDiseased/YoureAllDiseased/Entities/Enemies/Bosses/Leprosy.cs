//Leprosy.cs
//Copyright Dejitaru Forge 2011

using Microsoft.Xna.Framework;

namespace YoureAllDiseased.Entities.Enemies.Bosses
{
    /// <summary>
    /// A very strong disease that should not be toyed with
    /// </summary>
    public class Leprosy : Entity
    {
        bool cutOffPlayer = false;

        //regain health up to maxRegens times
        int regenCount = 0;
        int maxRegens = 3;

        int prevHealth;

        public Leprosy() : base("Leprosy", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(8, 8, 178, 178), 0, 2500, 15000, 250) { prevHealth = maxHealth; }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/Bosses/Leprosy"), Microsoft.Xna.Framework.Rectangle.Empty, 3,
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 194, 194), FrameTimeType.Milliseconds, 100, 0, true, 0);
        }

        public override void OnDie(GameTime gameTime, PlayScreen owner)
        {
            owner.player.currentLives++; //bonus life
        }

        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (currentHealth < 1)
            {
                owner.bossDeathAnimLen = 400;

                if (gameTime.TotalGameTime.TotalMilliseconds % 100 < 20)
                {
                    for (int i = 0; i < 24; i++) //big explosion
                    {
                        float r = (float)owner.parent.random.NextDouble() * 40 + 20;
                        float a = MathHelper.ToRadians(i * 15);
                        owner.decayParticles.Particulate(20 * owner.particleMultiplier, new Vector2(position.X + r * (float)System.Math.Cos(a), position.Y + r * (float)System.Math.Sin(a)), 5, 15, 0, MathHelper.TwoPi);
                    }
                    sprite.position.Width += 5;
                    sprite.position.Height += 5;
                    angle = MathHelper.TwoPi * (float)owner.parent.random.NextDouble();
                }

                return;
            }

            if (owner.currentPlayMode != PlayScreen.PlayMode.BossMode)
                return;

            if (!cutOffPlayer)
            {
                //modify map so player cannot escape
                owner.map.tiles[0, 73] = 6;
                owner.map.tiles[1, 73] = 15;
                owner.map.tiles[2, 73] = 7;

                owner.map.tiles[0, 74] = 0;
                owner.map.tiles[1, 74] = 0;
                owner.map.tiles[2, 74] = 0;

                owner.map.tiles[0, 75] = 5;
                owner.map.tiles[1, 75] = 13;
                owner.map.tiles[2, 75] = 8;

                cutOffPlayer = true;
            }

            float dist = Microsoft.Xna.Framework.Vector2.Distance(position, owner.player.position);
            var len = owner.player.position - position;
            float ang = (float)System.Math.Atan2(len.Y, len.X);

            if (prevHealth > currentHealth && currentHealth % 250 < 30)
            {
                int numShots = 12 - (int)(currentHealth / 250);
                float angle = -MathHelper.PiOver2;

                for (int i = 0; i < numShots; i++)
                {
                    angle -= MathHelper.PiOver2 / numShots;
                    owner.shots.Add(new Vector4((int)Weapon.RhinoRocket, position.X + 100 * (float)System.Math.Cos(angle), position.Y + 100 * (float)System.Math.Sin(angle), angle));
                }
            }

            if (gameTime.TotalGameTime.TotalMilliseconds % 600 < 30)
            {
                owner.shots.Add(new Vector4((int)Weapon.BossFlower, position.X + 100 * (float)System.Math.Cos(ang), position.Y + 100 * (float)System.Math.Sin(ang), ang));
            }

            if (currentHealth < 250 && owner.parent.random.Next(0,100) == 32 && regenCount < maxRegens)
            {
                currentHealth += 750;
                owner.weaponParticles.Particulate(50 * owner.particleMultiplier, position, 10, 14, 0, MathHelper.TwoPi);
                regenCount++;
            }
            //attract player
            if (dist < 180)
            {

                //attract player somewhat
                if (owner.useCollision)
                    owner.player.velocity -= (dist < 64 ? 1 : 0.5f) * new Microsoft.Xna.Framework.Vector2((float)System.Math.Cos(ang), (float)System.Math.Sin(ang));

                if (dist < 16 && !owner.player.isInvulnerable && gameTime.TotalGameTime.TotalMilliseconds % 3000 < 50)
                    owner.player.currentHealth -= 20;
            }

            prevHealth = currentHealth;
        }
    }
}
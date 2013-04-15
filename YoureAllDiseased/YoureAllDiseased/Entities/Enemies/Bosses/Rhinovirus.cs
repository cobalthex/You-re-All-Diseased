//Rhinovirus.cs
//Copyright Dejitaru Forge 2011

using Microsoft.Xna.Framework;

namespace YoureAllDiseased.Entities.Enemies.Bosses
{
    /// <summary>
    /// The first boss in the game, the common cold
    /// </summary>
    public class Rhinovirus : Entity
    {
        bool hasGivenPlayerHealth = false;
        bool hasUsedFlutter = false;
        bool hasUsedFlower = false;
        bool hasUsedRhinoRocket = false;

        public Rhinovirus() : base("Rhinovirus", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 96, 96), 0, 1000, 5000, 400) { }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/Bosses/Rhinovirus"), Microsoft.Xna.Framework.Rectangle.Empty, 1,
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 128, 128), FrameTimeType.Milliseconds, 100, 0, true, 0);
        }

        public override void OnDie(GameTime gameTime, PlayScreen owner)
        {
            owner.player.currentLives++; //bonus life
        }

        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (owner.currentPlayMode != PlayScreen.PlayMode.BossMode)
                return;

            if (currentHealth < 1)
            {
                owner.bossDeathAnimLen = 200;

                velocity = new Vector2(16) - new Vector2((float)owner.parent.random.NextDouble(), (float)owner.parent.random.NextDouble()) * 32;
                if (gameTime.TotalGameTime.TotalMilliseconds % 100 < 20)
                {
                    for (int i = 0; i < 12; i++) //big explosion
                    {
                        float r = (float)owner.parent.random.NextDouble() * 40 + 60;
                        float a = MathHelper.ToRadians(i * 30);
                        owner.weaponParticles.Particulate(30 * owner.particleMultiplier, new Vector2(position.X + r * (float)System.Math.Cos(a), position.Y + r * (float)System.Math.Sin(a)), 2, 5, 0, MathHelper.TwoPi);
                    }
                }

                sprite.rotation += 0.2f;

                return;
            }

            float dist = Vector2.Distance(position, owner.player.position); //distance from player
            Vector2 delta = position - owner.player.position; //delta position of player vs this

            //follow player
            angle = -(float)System.Math.Atan2(delta.Y, delta.X);

            //push player away if too close
            if (dist < 80)
            {
                owner.player.velocity.Normalize();
                angle += 3.14159f;
                owner.player.position = position + new Vector2(82) * new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(-angle));
                owner.player.velocity.X *= -20;
                owner.player.velocity.Y *= -20;

                if (!owner.player.isInvulnerable)
                    owner.player.currentHealth -= 5;
            }

            //one time only: give player health if they're dying
            if (owner.player.currentHealth < 30 && owner.player.currentLives == 0 && !hasGivenPlayerHealth)
            {
                Entity e = new Entities.Powerups.HealthPowerup(40);
                e.position = position + new Vector2(100, 0);
                e.Load(ref owner.content);
                owner.map.ents.Add(e);

                hasGivenPlayerHealth = true;

                owner.explosionParticles.Particulate(5 * owner.particleMultiplier, e.position, 3, 5, 0, MathHelper.TwoPi);
            }

            //if player is too far away, pull player back in
            if (dist > 500 && owner.useCollision)
            {
                angle += 3.14159f;
                owner.player.position = position + new Vector2(499) * new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(-angle));
                owner.explosionParticles.Particulate(5 * owner.particleMultiplier, owner.player.position, 3, 10, 0, MathHelper.TwoPi);
            }

            int timer = 0;

            //flower missiles
            if (currentHealth < maxHealth / 2)
            {
                timer = gameTime.TotalGameTime.Seconds % 8;
                if ((timer == 0 || timer == 4) && !hasUsedFlower)
                {
                    hasUsedFlower = true;

                    for (int i = 0; i < 12; i++) //big explosion
                    {
                        float a = MathHelper.ToRadians(i * 30);

                        int x = (int)(100 * System.Math.Cos(a));
                        int y = (int)(100 * System.Math.Sin(a));

                        owner.shots.Add(new Vector4((float)Weapon.BossFlower, x, y, a));

                        for (int j = 0; j < 6; j++)
                        {
                            float aa = MathHelper.ToRadians(j * 60);
                            owner.shots.Add(new Vector4((float)Weapon.BossFlower, x + (int)(10 * System.Math.Cos(aa)) + position.X, y + (int)(10 * System.Math.Sin(aa)) + position.Y, aa));
                        }
                    }
                }
                else if (timer == 1 || timer == 5) //reset flutter missle timer
                    hasUsedFlower = false;
            }

            //flutter missiles
            timer = gameTime.TotalGameTime.Seconds % (currentHealth < maxHealth / 2.5f ? 2 : 6);
            if ((timer == 0 || timer == 4) && !hasUsedFlutter)
            {
                hasUsedFlutter = true;

                int count = 4;

                for (int i = 0; i < count; i++)
                    for (int j = 0; j < 18; j++) //shoot out ever 20deg
                    {
                        float a = MathHelper.ToRadians(j * 20) + (timer == 4 ? 10 : 0);
                        owner.shots.Add(new Vector4((int)Weapon.BossFlutter, position.X + (80 + (i * 20)) * (float)System.Math.Cos(a), position.Y + (80 + (i * 20)) * (float)System.Math.Sin(a), a));
                    }
            }
            else if (timer == 1 || timer == 5) //reset flutter missle timer
                hasUsedFlutter = false;

            //fire rhino rocket
            timer = gameTime.TotalGameTime.Seconds % 5;
            if (dist > 100 & timer == 0 && !hasUsedRhinoRocket)
            {
                hasUsedRhinoRocket = true;
                float a = -angle - (currentHealth < maxHealth / 10 ? 0.8f : 0.4f) + MathHelper.Pi;

                for (int i = 0; i < (currentHealth < maxHealth / 10 ? 4 : 2); i++)
                    owner.shots.Add(new Vector4((int)Weapon.RhinoRocket, position.X + 80 * (float)System.Math.Cos(a + (i * 0.4f)), position.Y + 80 * (float)System.Math.Sin(a + (i * 0.4f)), a + (i * 0.4f)));
            }
            if (timer == 1)
                hasUsedRhinoRocket = false;
        }
    }
}
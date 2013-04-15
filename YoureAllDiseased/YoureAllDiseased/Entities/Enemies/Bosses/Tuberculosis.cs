//Tuberculosis.cs
//Copyright Dejitaru Forge 2011

using Microsoft.Xna.Framework;

namespace YoureAllDiseased.Entities.Enemies.Bosses
{
    /// <summary>
    /// Tuberculosis
    /// </summary>
    public class Tuberculosis : Entity
    {
        bool set = false;

        public Tuberculosis() : base("Tuberculosis", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 128, 128), 0, 1500, 2000, 300) { }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/Bosses/Tuberculosis"), Microsoft.Xna.Framework.Rectangle.Empty, 2,
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 128, 128), FrameTimeType.Milliseconds, 50, 0, true, 0);
        }

        public override void Think(GameTime gameTime, PlayScreen owner)
        {
            if (currentHealth < 1)
            {
                owner.bossDeathAnimLen = 200;

                velocity = new Vector2(4) - new Vector2((float)owner.parent.random.NextDouble(), (float)owner.parent.random.NextDouble()) * 8;
                if (gameTime.TotalGameTime.TotalMilliseconds % 100 < 20)
                {
                    for (int i = 0; i < 24; i++) //big explosion
                    {
                        float r = (float)owner.parent.random.NextDouble() * 40 + 20;
                        float a = MathHelper.ToRadians(i * 15);
                        owner.weaponParticles.Particulate(10 * owner.particleMultiplier, new Vector2(position.X + r * (float)System.Math.Cos(a), position.Y + r * (float)System.Math.Sin(a)), 5, 15, 0, MathHelper.TwoPi);
                    }
                }

                for (int i = 0; i < 4; i++) //big explosion
                {
                    float r = (float)owner.parent.random.NextDouble() * 40 + 80;
                    float a = (float)owner.parent.random.NextDouble() * MathHelper.TwoPi;
                    owner.shots.Add(new Vector4((int)Weapon.Tuberculosis, position.X + r * (float)System.Math.Cos(a), position.Y + r * (float)System.Math.Sin(a), a));
                }

                return;
            }

            if (owner.currentPlayMode != PlayScreen.PlayMode.BossMode)
                return;

            if (!set)
            {
                owner.map.mucus.Clear();
                for (int i = 1; i < owner.map.ents.Count; i++)
                    if (owner.map.ents[i].GetType() != typeof(Tuberculosis))
                        owner.map.ents.RemoveAt(i);
                set = true;
            }

            float dist = Vector2.Distance(position, owner.player.position);

            if (dist > 400)
            {
                currentHealth += 5;
            }

            if (gameTime.TotalGameTime.TotalMilliseconds % 1500 < 20)
                for (int i = 0; i < 1 + (int)((maxHealth - currentHealth) / 200); i++)
                {
                    Vector2 len = position - owner.player.position;
                    float angle = (float)System.Math.Atan2(len.Y, len.X) + MathHelper.Pi + (i * 0.04f);
                    int pos = 100 - i * 2;
                    owner.shots.Add(new Vector4((int)Weapon.Tuberculosis, pos * (float)System.Math.Cos(angle) + position.X, pos * (float)System.Math.Sin(angle) + position.Y, angle));
                }
        }
    }
}
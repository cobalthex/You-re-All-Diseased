//Cancer.cs
//Copyright Dejitaru Forge 2011

using Microsoft.Xna.Framework;

namespace YoureAllDiseased.Entities.Enemies.Bosses
{
    /// <summary>
    /// The first boss in the game, the common cold
    /// </summary>
    public class Cancer : Entity
    {
        bool spawnedMut = false;
        bool spawnedInf = false;
        bool spawnedDec = false;

        public Cancer() : base("Cancer", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(8, 8, 128, 128), 0, 500, 10000, 350) { }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/Bosses/Cancer"), Microsoft.Xna.Framework.Rectangle.Empty, 8,
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 144, 144), FrameTimeType.Milliseconds, 100, 0, true, 0);
        }

        public override void OnDie(GameTime gameTime, PlayScreen owner)
        {
            for (int i = 0; i < 12; i++) //big explosion
            {
                float a = MathHelper.ToRadians(i * 30);
                owner.explosionParticles.Particulate(10 * owner.particleMultiplier, new Vector2(position.X + 120 * (float)System.Math.Cos(a), position.Y + 120 * (float)System.Math.Sin(a)), 10, 15, 0, MathHelper.TwoPi);
                owner.weaponParticles.Particulate(30 * owner.particleMultiplier, new Vector2(position.X + 80 * (float)System.Math.Cos(a), position.Y + 80 * (float)System.Math.Sin(a)), 2, 5, 0, MathHelper.TwoPi);
            }

            owner.player.currentLives++; //bonus life
        }

        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (currentHealth < 1)
            {
                owner.bossDeathAnimLen = 200;

                if (gameTime.TotalGameTime.TotalMilliseconds % 100 < 20)
                {
                    for (int i = 0; i < 24; i++) //big explosion
                    {
                        float r = (float)owner.parent.random.NextDouble() * 40 + 20;
                        float a = MathHelper.ToRadians(i * 15);
                        owner.weaponParticles.Particulate(10 * owner.particleMultiplier, new Vector2(position.X + r * (float)System.Math.Cos(a), position.Y + r * (float)System.Math.Sin(a)), 5, 15, 0, MathHelper.TwoPi);
                    }
                    sprite.position.Width -= 5;
                    sprite.position.Height -= 5;
                }

                return;
            }

            if (owner.currentPlayMode != PlayScreen.PlayMode.BossMode)
                return;


            if (spawnedMut && gameTime.TotalGameTime.TotalMilliseconds % (currentHealth < 800 ? 500 : 1500) < 20)
                spawnedMut = false;

            if (!spawnedMut && gameTime.TotalGameTime.TotalMilliseconds % (currentHealth < 800 ? 1200 : 2000) < 20)
            {
                MutatedCell cell = new MutatedCell();
                cell.Load(ref owner.content);
                cell.position = position - new Vector2(65);
                owner.map.ents.Add(cell);
                spawnedMut = true;
            }

            if (spawnedInf && gameTime.TotalGameTime.TotalMilliseconds % (currentHealth < 600 ? 500 : 1400) < 20)
                spawnedInf = false;

            if (!spawnedInf && gameTime.TotalGameTime.TotalMilliseconds % (currentHealth < 600 ? 800 : 1600) < 20)
            {
                InfectedCell cell = new InfectedCell();
                cell.Load(ref owner.content);
                cell.position = position - new Vector2(0, 100);
                owner.map.ents.Add(cell);
                spawnedInf = true;
            }

            if (spawnedDec && gameTime.TotalGameTime.TotalMilliseconds % (currentHealth < 400 ? 1500 : 5000) < 20)
                spawnedDec = false;

            if (!spawnedDec && gameTime.TotalGameTime.TotalMilliseconds % (currentHealth < 400 ? 3000 : 4000) < 20)
            {
                DecayingCell cell = new DecayingCell();
                cell.Load(ref owner.content);
                cell.position = position - new Vector2(100, 0);
                owner.map.ents.Add(cell);
                spawnedDec = true;
            }
        }
    }
}
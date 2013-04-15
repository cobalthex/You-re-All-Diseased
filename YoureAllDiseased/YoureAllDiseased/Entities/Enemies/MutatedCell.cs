//MutatedCell.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Enemies
{
    /// <summary>
    /// An upgraded version of the infected cell. It has more health and moves slightly faster, it can also turn infected cells into mutated cells
    /// </summary>
    public class MutatedCell : Entity
    {
        /// <summary>
        /// maximum velocity that this object can move at
        /// </summary>
        int maxVelocity = 0;
        float ang = 0;

        public MutatedCell() : base("Mutated Cell", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 48), 0, 25, 1)
        {
            pointsOnDeath = 80;
            ang = angle;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/MutatedCell"), Microsoft.Xna.Framework.Rectangle.Empty, 8, 
                new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 48), FrameTimeType.Milliseconds, 75, 0, true, 0);
        }

        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (maxVelocity == 0)
                maxVelocity = owner.parent.random.Next(8, 13);

            float dist = Microsoft.Xna.Framework.Vector2.Distance(owner.player.position, position);

            if (dist < 800)
            {
                Microsoft.Xna.Framework.Vector2 length = owner.player.position - position;

                if (CanSee(position, owner.player.position, ref owner.map))
                {
                    Microsoft.Xna.Framework.Vector2 vel = new Microsoft.Xna.Framework.Vector2(maxVelocity * (float)System.Math.Cos(length.X / dist), maxVelocity * (float)System.Math.Sin(length.Y / dist));
                    if (dist > 600) vel /= 2;

                    if (length.X < 0 && vel.X > 0)
                        vel.X *= -1;
                    vel.Normalize(); vel *= 2;

                    velocity += vel;
                    if (velocity.X > maxVelocity) velocity.X = maxVelocity;
                    if (velocity.X < -maxVelocity) velocity.X = -maxVelocity;
                    if (velocity.Y > maxVelocity) velocity.Y = maxVelocity;
                    if (velocity.Y < -maxVelocity) velocity.Y = -maxVelocity;

                    if (dist < 32 && currentHealth > 0) //collide with player and die
                    {
                        if (!owner.player.isInvulnerable)
                        {
                            owner.player.currentHealth -= 20;
                            owner.comboCount = 0;
                        }

                        currentHealth = 0;
                    }
                }
            }

            for (int i = 0; i < owner.map.ents.Count; i++)
            {
                if (owner.map.ents[i].GetType() != typeof(InfectedCell)) //only check infected cells
                    continue;

                float dst = Microsoft.Xna.Framework.Vector2.Distance(position, owner.map.ents[i].position);
                if (dst < 32)
                {
                    MutatedCell c = new MutatedCell();
                    c.Load(ref owner.content);
                    c.angle = owner.map.ents[i].angle;
                    c.position = owner.map.ents[i].position;
                    owner.map.ents[i] = c;
                    owner.weaponParticles.Particulate(10 * owner.particleMultiplier, c.position, 4, 8, 0, Microsoft.Xna.Framework.MathHelper.TwoPi);
                }
            }
        }
    }
}
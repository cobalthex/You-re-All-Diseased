//Poliomyelitis.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Enemies
{
    /// <summary>
    /// Polio
    /// </summary>
    public class Polio : Entity
    {
        /// <summary>
        /// the polio that this is linked to (null for none)
        /// </summary>
        Polio bound = null;

        /// <summary>
        /// How far away ent can be to link with another
        /// </summary>
        public static int linkDist = 200;

        public Polio()
            : base("Poliomyelitis", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 48), 0, 18, 1)
        {
            pointsOnDeath = 100;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/Polio"), Microsoft.Xna.Framework.Rectangle.Empty, 5,
                new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 48), FrameTimeType.Milliseconds, 100, 0, true, 0);
        }
        
        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            //stay chained to bound
            if (bound != null)
            {
                if (bound.currentHealth <= 0 && bound.currentLives < 1)
                {
                    bound = null; //dead
                    return;
                }

                angle = bound.angle;

                Microsoft.Xna.Framework.Vector2 length = position - bound.position;
                float dist = Microsoft.Xna.Framework.Vector2.Distance(position, bound.position);
                float a = (float)System.Math.Atan2(length.Y, length.X) + 3.14159f;
                int sz = (size.Width + size.Height);
                
                if (dist > sz && dist < linkDist)
                {
                    if (CanSee(position, owner.player.position, ref owner.map))
                        velocity = new Microsoft.Xna.Framework.Vector2((float)System.Math.Cos(a), (float)System.Math.Sin(a)) * 6;
                    else
                        velocity = Microsoft.Xna.Framework.Vector2.Zero;
                }
                else if (dist < sz)
                {
                    length = position - owner.player.position;
                    dist = Microsoft.Xna.Framework.Vector2.Distance(position, owner.player.position);
                    a = (float)System.Math.Atan2(length.Y, length.X) + 3.14159f;

                    if (dist > 48 && dist < linkDist << 1 && CanSee(position, owner.player.position, ref owner.map)) //only attract to a certain distance
                        velocity = new Microsoft.Xna.Framework.Vector2((float)System.Math.Cos(a), (float)System.Math.Sin(a)) * 13;
                    else
                        velocity = Microsoft.Xna.Framework.Vector2.Zero;

                    if (Microsoft.Xna.Framework.Vector2.Distance(position, bound.position) < sz)
                        bound.velocity = velocity;

                    if (dist < 50 && !owner.player.isInvulnerable)
                        owner.player.currentHealth--;
                }
            }
            else
            {
                //find new binder
                for (int i = 0; i < owner.map.ents.Count; i++)
                {
                    if (owner.map.ents[i].GetType() != typeof(Polio) || owner.map.ents[i] == this) //only check polio and make sure not itself
                        continue;

                    float dst = Microsoft.Xna.Framework.Vector2.Distance(position, owner.map.ents[i].position);
                    if (dst < linkDist)
                        bound = (Polio)owner.map.ents[i];
                }

                float dist = Microsoft.Xna.Framework.Vector2.Distance(position, owner.player.position);
                if (dist > 50)
                {
                    Microsoft.Xna.Framework.Vector2 length = position - owner.player.position;
                    float a = (float)System.Math.Atan2(length.Y, length.X) + 3.14159f;
                    if (CanSee(position, owner.player.position, ref owner.map))
                        velocity = new Microsoft.Xna.Framework.Vector2((float)System.Math.Cos(a), (float)System.Math.Sin(a)) * 8;
                }
                else
                    velocity /= 2;
                if (dist < 52 && !owner.player.isInvulnerable)
                    owner.player.currentHealth--;
            }
        }
    }
}
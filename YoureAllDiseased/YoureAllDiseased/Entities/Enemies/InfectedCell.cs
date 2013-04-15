//InfectedCell.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Enemies
{
    /// <summary>
    /// A basic infected cell that tries to directly attack the player by swarming
    /// </summary>
    public class InfectedCell : Entity
    {
        /// <summary>
        /// maximum velocity that this object can move at
        /// </summary>
        float maxVelocity = 0;

        /// <summary>
        /// should this cell blow itself up when the player comes close?
        /// </summary>
        bool? suicide = null;

        Sprite explodeSprite = null;

        public InfectedCell()
            : base("Infected Cell", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), 0, 10, 1)
        {
            pointsOnDeath = 50;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/InfectedCell"), Microsoft.Xna.Framework.Rectangle.Empty, 8,
                new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), FrameTimeType.Milliseconds, 75, 0, true, 0);

            explodeSprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/InfectedCellExplode"), Microsoft.Xna.Framework.Rectangle.Empty, 4,
                    new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 48), FrameTimeType.Milliseconds, 75, 0, true, 0);
        }

        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (sprite == explodeSprite && sprite.currentFrame >= 3)
            {
                currentHealth = 0;
                explodeSprite.ResetFrame();
            }

            if (maxVelocity == 0)
                maxVelocity = owner.parent.random.Next(12, 16);

            float dist = Microsoft.Xna.Framework.Vector2.Distance(owner.player.position, position);

            if (dist < 500)
            {
                if (suicide == null)
                    suicide = owner.parent.random.Next(0, 10) > 7;

                Microsoft.Xna.Framework.Vector2 length = owner.player.position - position;

                if (CanSee(position, owner.player.position, ref owner.map))
                {
                    if (suicide == true && sprite != explodeSprite)
                    {
                        float vel = maxVelocity / 3;
                        velocity.X = vel * (float)System.Math.Cos(length.X / dist);
                        velocity.Y = vel * (float)System.Math.Sin(length.Y / dist);
                    }
                    else
                    {
                        velocity.X = maxVelocity * (float)System.Math.Cos(length.X / dist);
                        velocity.Y = maxVelocity * (float)System.Math.Sin(length.Y / dist);
                    }

                    if (length.X < 0 && velocity.X > 0)
                        velocity.X *= -1;

                    if (dist < 96 && suicide == true && sprite != explodeSprite) //a grenade effect
                    {
                        owner.player.currentHealth -= 96 - (int)dist;
                        owner.player.isHit = true;

                        if (dist < 64)
                            owner.comboCount = 0;

                        sprite = explodeSprite;
                        explodeSprite.ResetFrame();
                    }
                    else if (suicide != true && dist < 48 && currentHealth > 0) //collide with player and die
                    {
                        if (!owner.player.isInvulnerable)
                        {
                            owner.player.currentHealth -= 10;
                            owner.comboCount = 0;
                        }

                        currentHealth = 0;
                    }
                }
            }
        }
    }
}
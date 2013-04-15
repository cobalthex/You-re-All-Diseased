//Spawner.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities.Enemies
{
    /// <summary>
    /// A generic swarming type enemy
    /// </summary>
    public class Spawner : Entity
    {
        #region Data

        /// <summary>
        /// All of the entities to be spawned out of this spawner
        /// </summary>
        System.Collections.Generic.List<Entity> spawnList = new System.Collections.Generic.List<Entity>(8);

        /// <summary>
        /// The sprite to display while the spawner is inactive
        /// </summary>
        Sprite deadSprite;
        /// <summary>
        /// The sprite to display while the spawner is active
        /// </summary>
        Sprite aliveSprite;

        /// <summary>
        /// how long to wait (in ms) before spawning a new ent (only used if useTimer = true)
        /// </summary>
        long waitTime;
        /// <summary>
        /// Use the timer to spawn enemies if true
        /// wait until previously spawned enemy is dead to spawn new
        /// </summary>
        bool useTimer;
        /// <summary>
        /// The currently spawned entity (only used if useTimer = false)
        /// </summary>
        Entity spawnedEnt;

        #endregion


        #region Initialization

        /// <summary>
        /// Create a new spawner
        /// </summary>
        /// <param name="spawnList">The list of entities to spawn</param>
        /// <param name="waitTime">How long to wait before spawning each item</param>
        public Spawner(System.Collections.Generic.List<Entity> spawnList, long waitTime)
            : base("Spawner", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), 0, 200, 1)
        {
            useTimer = true;
            spawnedEnt = null;
            this.waitTime = waitTime; //3 seconds

            this.spawnList = spawnList;

            pointsOnDeath = 500;
        }
        /// <summary>
        /// Create a new spawner
        /// </summary>
        /// <param name="spawnList">The list of entiteis to spawn</param>
        public Spawner(System.Collections.Generic.List<Entity> spawnList)
            : base("Spawner", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), 0, 300, 1)
        {
            useTimer = false;
            spawnedEnt = null;
            waitTime = 3000; //3 seconds

            this.spawnList = spawnList;

            pointsOnDeath = 500;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            aliveSprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/Spawner"), Microsoft.Xna.Framework.Rectangle.Empty, 5,
                new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), FrameTimeType.Milliseconds, 100, 0, false, 0);

            deadSprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Enemies/SpawnerDead"), Microsoft.Xna.Framework.Rectangle.Empty, 3,
                new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), FrameTimeType.Milliseconds, 100, 0, false, 0);

            sprite = aliveSprite;
            sprite.isUpdating = false;

            angle = (float)System.Math.PI / 2;
        }

        #endregion


        #region Logic

        bool hasDied = false;
        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (hasDied && sprite.currentFrame == 2)
                sprite.isUpdating = false;

            if (spawnList == null || spawnList.Count < 1) //nothing left to spawn
            {

                if (!hasDied)
                {
                    sprite = deadSprite;
                    sprite.isUpdating = true;

                    owner.explosionParticles.Particulate(5 * owner.particleMultiplier, position, 8, 12, angle + 0.1f, angle - 0.1f);

                    size = new Microsoft.Xna.Framework.Rectangle(6, 6, 18, 18);

                    hasDied = true;
                }

                return;
            }

            if (Microsoft.Xna.Framework.Vector2.DistanceSquared(position, owner.player.position) < 90000 && !sprite.isUpdating && 
                sprite.currentFrame == 0 && CanSee(position, owner.player.position, ref owner.map))
            {
                if (!useTimer)
                {
                    if (spawnedEnt == null)
                        sprite.isUpdating = true;
                }
                else
                {
                    if ((System.DateTime.UtcNow.Ticks - sprite.startTime) / 10000 >= waitTime)
                        sprite.isUpdating = true;
                }
            }

            if (sprite.currentFrame > 4)
            {
                owner.weaponParticles.Particulate(5 * owner.particleMultiplier, position, 4, 8, angle + 0.2f, angle - 0.2f);

                spawnedEnt = spawnList[0];
                spawnedEnt.Load(ref owner.content);
                spawnedEnt.position = position + new Microsoft.Xna.Framework.Vector2(30 * (float)System.Math.Cos(angle), 30 * (float)System.Math.Sin(angle));
                spawnedEnt.currentHealth = spawnedEnt.maxHealth;
                owner.map.ents.Add(spawnedEnt);
                spawnList.RemoveAt(0);

                sprite.ResetFrame();
                sprite.isUpdating = false;
            }

            if (spawnedEnt != null && spawnedEnt.currentHealth < 1 && spawnedEnt.currentLives < 1) //if the spawned ent dies, unlink it
                spawnedEnt = null;
        }

        #endregion
    }
}
//Entity.cs
//Copyright Dejitaru Forge 2011

using Microsoft.Xna.Framework;

namespace YoureAllDiseased
{
    /// <summary>
    /// The type of weapon that an ent is using (first are for player, starting at EnemyGeneric are enemys'); value = damage
    /// </summary>
    public enum Weapon : int
    {
        /// <summary>
        /// Special shot that is not used and is designed just to run through shot update loop
        /// </summary>
        None = -1,

        /// <summary>
        /// A simple histamine excreter
        /// </summary>
        Histamine = 0,
        /// <summary>
        /// A cytoplasmic granule that contains a very effective venom
        /// </summary>
        Granzyme,
        /// <summary>
        /// A cell that duplicates every half second
        /// </summary>
        DupeCell,

        /// <summary>
        /// Decaying matter shot by a decaying cell
        /// </summary>
        Decay = 100,

        /// <summary>
        /// A basic flutter missle that is fired in large quantities which fire out in straight lines
        /// </summary>
        BossFlutter = 1000,
        /// <summary>
        /// Fired out in a flower shape
        /// </summary>
        BossFlower, 
        /// <summary>
        /// Rhino virus's primary weapon, shoots out tracking missiles 
        /// </summary>
        RhinoRocket,
        /// <summary>
        /// The shards of a tuberculosis
        /// </summary>
        Tuberculosis,

        /// <summary>
        /// Special debug weapons (super cheat weapons)
        /// </summary>
        DeathMachine = 100000
    }

    /// <summary>
    /// An inheritable entity class that controls all interactive objects
    /// </summary>
    public class Entity
    {
        #region Data

        /// <summary>
        /// current weapon (if no ammo left, defaults to antibody
        /// </summary>
        public Weapon currentWeapon;
        /// <summary>
        /// max ammo for gun (-1 for none)
        /// </summary>
        public int maxAmmo = -1;
        /// <summary>
        /// current ammo
        /// </summary>
        public int currentAmmo = 0;
        /// <summary>
        /// delay between shots (in ms)
        /// </summary>
        public int weaponDelay = 100;

        /// <summary>
        /// current amount of mucus available to shoot, max is 1000
        /// </summary>
        public int currentMucus = 0;

        /// <summary>
        /// Name of entity (like blue soldier, ammo box, etc.)
        /// </summary>
        public string name;

        /// <summary>
        /// absolute position (in px)
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// any movement (0 for none)
        /// </summary>
        public Vector2 velocity;
        /// <summary>
        /// the current angle that the entity is facing (in rad)
        /// </summary>
        public float angle;
        /// <summary>
        /// The collision rectangle for the entity
        /// </summary>
        public Rectangle size;
        /// <summary>
        /// The size of the radius, squared (for better collision detection)
        /// squared so that no sqrts have to be done
        /// </summary>
        public int radiusSquared;
        /// <summary>
        /// max hitpoints (0 for irrelevent)
        /// </summary>
        public int maxHealth;
        /// <summary>
        /// max number of lives (starting # of lives)
        /// </summary>
        public int maxLives;
        /// <summary>
        /// Current # of lives -- does not apply if using hearts
        /// </summary>
        public int currentLives;
        /// <summary>
        /// Currrent # of hitpoints (if current health & lives less than/equal to 0, dead)
        /// </summary>
        public int currentHealth;
        
        /// <summary>
        /// Can this ent be hit? (invincibility)
        /// </summary>
        public bool canHit;

        /// <summary>
        /// Is this entity currently invulnerable (will cause the ent to 'glow')
        /// </summary>
        public bool isInvulnerable = false;

        /// <summary>
        /// specifies whether this has been hit; used for drawing, reset after each frame
        /// </summary>
        public bool isHit = false;

        /// <summary>
        /// The corresponding sprite 
        /// </summary>
        public Sprite sprite;

        /// <summary>
        /// The number of points awarded to the player on this entity's death
        /// </summary>
        public int pointsOnDeath { get; protected set; }

        /// <summary>
        /// Is this a boss?
        /// </summary>
        public readonly bool isBoss;

        /// <summary>
        /// How close the player must be to the boss for boss mode to begin
        /// </summary>
        public int bossRadius;

        #endregion


        #region Initialization

        /// <summary>
        /// Create a new entity
        /// </summary>
        /// <param name="Name">The name/category of the entity</param>
        /// <param name="Position">Position on the map of the entity</param>
        /// <param name="CollisionRect">The rectangle for collision dectection</param>
        /// <param name="AngleFacing">The angle that the sprite is facing (in radians, 0 is to the right)</param>
        /// <param name="MaxHealth">total health that the entity has available</param>
        /// <param name="MaxLives">The total number of lives (0 for irrelevent)</param>
        public Entity(string Name, Vector2 Position, Rectangle CollisionRect, float AngleFacing, int MaxHealth, int MaxLives)
        {
            name = Name;
            position = Position;
            size = CollisionRect;
            angle = AngleFacing;
            velocity = Vector2.Zero;
            maxHealth = MaxHealth;
            currentHealth = MaxHealth;
            maxLives = MaxLives;
            currentLives = MaxLives;
            pointsOnDeath = 10;
            isBoss = false;
            bossRadius = 0;
            canHit = true;

            int lg = (CollisionRect.Width > CollisionRect.Height ? CollisionRect.Width : CollisionRect.Height) >> 1;
            radiusSquared = lg * lg;
        }

        /// <summary>
        /// Create a new boss
        /// </summary>
        /// <param name="Name">The name/category of the entity</param>
        /// <param name="Position">Position on the map of the entity</param>
        /// <param name="CollisionRect">The rectangle for collision dectection</param>
        /// <param name="AngleFacing">The angle that the sprite is facing (in radians, 0 is to the right)</param>
        /// <param name="MaxHealth">total health that the entity has available</param>
        /// <param name="PointsOnDeath">Total points awarded for killing this boss</param>
        /// <param name="BossRadius">How far away the boss should be from the player before the game enters boss mode</param>
        public Entity(string Name, Vector2 Position, Rectangle CollisionRect, float AngleFacing, int MaxHealth, int PointsOnDeath, int BossRadius)
        {
            name = Name;
            position = Position;
            size = CollisionRect;
            angle = AngleFacing;
            velocity = Vector2.Zero;
            maxHealth = MaxHealth;
            currentHealth = MaxHealth;
            maxLives = 0;
            currentLives = 0;
            pointsOnDeath = PointsOnDeath;
            isBoss = true;
            bossRadius = BossRadius;
            canHit = true;

            int lg = (CollisionRect.Width > CollisionRect.Height ? CollisionRect.Width : CollisionRect.Height) >> 1;
            radiusSquared = lg * lg;
        }

        public virtual void Load(ref Microsoft.Xna.Framework.Content.ContentManager content) { }

        #endregion


        #region Logic

        /// <summary>
        /// Update the entity and perform any AI
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        /// <param name="owner">The owner of this entity, for referencing parts of the game like other entities (namely, the player)</param>
        public virtual void Think(GameTime gameTime, PlayScreen owner) { }

        /// <summary>
        /// Called when this ent dies
        /// </summary>
        /// <param name="gameTime">game time</param>
        /// <param name="owner">The owner of this entity</param>
        public virtual void OnDie(GameTime gameTime, PlayScreen owner) { }

        /// <summary>
        /// See if point a can see point b from within the map
        /// </summary>
        /// <param name="a">The first point</param>
        /// <param name="b">The second point</param>
        /// <param name="map">The map to use</param>
        /// <returns>True if the two points can see eachother</returns>
        public static bool CanSee(Vector2 a, Vector2 b, ref Map map)
        {
            bool canSee = false; //can't see is the most likely case

            Vector2 ln = b - a;
            ln.Normalize();
            int incX = (int)((map.tileWidth >> 2) * ln.X);
            int incY = (int)((map.tileHeight >> 2) * ln.Y);
            Point aa = new Point((int)a.X / map.tileWidth, (int)a.Y / map.tileHeight); //tile that a is in
            Point bb = new Point((int)b.X / map.tileWidth, (int)b.Y / map.tileHeight); //tile that b is in

            //navigate from a to b
            while (new Rectangle(0, 0, map.width, map.height).Contains(aa))
            {
                /* tile based
                //if not in the map, exit
                if (map.tiles[(int)(a.Y / map.tileHeight), (int)(a.X / map.tileWidth)] == 0)
                    break;
                */
                if (!map.Inside(a)) //per pixel collision detection
                    break;

                //a has reached b (a can see b)
                if (aa == bb)
                {
                    canSee = true;
                    break;
                }
                a.X += incX;
                a.Y += incY;

                //recalc current tile
                aa.X = (int)a.X / map.tileWidth;
                aa.Y = (int)a.Y / map.tileHeight;
            }

            return canSee;
        }

        #endregion
    }
}
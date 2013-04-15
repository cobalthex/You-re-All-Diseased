//Player.cs
//Copyright Dejitaru Forge 2011

namespace YoureAllDiseased.Entities
{
    /// <summary>
    /// The player's entity
    /// </summary>
    public class Player : Entity
    {
        /// <summary>
        /// Last shot (in ticks)
        /// </summary>
        public long lastShot = 0;

        /// <summary>
        /// The last time the player respawned
        /// </summary>
        public long respawnTime = 0;

        public Player()
            : base("Player", Microsoft.Xna.Framework.Vector2.Zero, new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 48), 0, 100, 3)
        {
            currentWeapon = Weapon.Histamine;
            currentAmmo = maxAmmo;
            weaponDelay = 150;
        }

        public override void Load(ref Microsoft.Xna.Framework.Content.ContentManager content)
        {
            sprite = new Sprite(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Graphics/Entities/Player"), Microsoft.Xna.Framework.Rectangle.Empty, 3,
                new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 48), FrameTimeType.Milliseconds, 100, 0, true, 0);
        }

        public override void OnDie(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            owner.parent.NextScreen(owner, new GameOverScreen(), new System.Collections.Generic.List<System.Object> { owner.playerScore, owner.playerKillCount, owner.screenStartTime },
                ((Main)owner.parent.Game).gameOverTransition, ((Main)owner.parent.Game).fadeInTransition);
        }

        public override void Think(Microsoft.Xna.Framework.GameTime gameTime, PlayScreen owner)
        {
            if (respawnTime != 0)
            {
                if ((System.DateTime.UtcNow.Ticks - respawnTime) / 10000 < 1500) //1sec safety
                    isInvulnerable = true;
                else
                {
                    isInvulnerable = false;
                    respawnTime = 0;
                }
            }
        }
    }
}

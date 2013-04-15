//PlayScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace YoureAllDiseased
{
    /// <summary>
    /// The actual game
    /// </summary>
    public class PlayScreen : GameScreen
    {
        #region Data

        /// <summary>
        /// The types of play available (hopefully) in this game
        /// </summary>
        public enum PlayMode
        {
            /// <summary>
            /// The main top down style shooting play mode
            /// </summary>
            MainMode,
            /// <summary>
            /// When the player is in a boss battle 
            /// </summary>
            BossMode,
            /// <summary>
            /// The tube (veins) shooting mode -- unused
            /// </summary>
            TubeMode,
            /// <summary>
            /// The racing through veins part of the game (tube mode occurs during this mode) -- unused
            /// </summary>
            RaceMode
        }

        #region Game specific

        /// <summary>
        /// Is collision detection turned on?
        /// </summary>
        public bool useCollision = true;

        /// <summary>
        /// Conserve ammo
        /// </summary>
        public bool bottomlessClip = false;

        /// <summary>
        /// The current active play mode
        /// </summary>
        public PlayMode currentPlayMode = PlayMode.MainMode;

        /// <summary>
        /// The loaded map
        /// </summary>
        public Map map;
        /// <summary>
        /// The current level of the game (level #)
        /// </summary>
        public int level = 0;

        /// <summary>
        /// show the collision lines (used for debugging purposes)
        /// </summary>
        bool showCollisLines = false;

        /// <summary>
        /// A reference rectangle for the size of the screen
        /// </summary>
        Rectangle screenRect;
        /// <summary>
        /// Spritebatch
        /// </summary>
        SpriteBatch sB;

        #endregion

        #region Content

        /// <summary>
        /// The player's health bar
        /// </summary>
        Texture2D playerHealthBar;
        /// <summary>
        /// The player's ammo meter
        /// </summary>
        Texture2D playerWeaponBar;
        /// <summary>
        /// The player's mucus meter
        /// </summary>
        Texture2D playerMucusBar;
        /// <summary>
        /// Boss health
        /// </summary>
        Texture2D bossHealthBar;
        /// <summary>
        /// The bitmap number font for score, combos, and others
        /// </summary>
        Texture2D numbersSprite;
        /// <summary>
        /// The icon to represent the number of lives the player has
        /// </summary>
        Texture2D livesIcon;
        /// <summary>
        /// The pause button
        /// </summary>
        Texture2D pauseButton;
        /// <summary>
        /// The critical hit logo
        /// </summary>
        Texture2D criticalHitLogo;
        /// <summary>
        /// Critical hit glow
        /// </summary>
        Texture2D criticalHitGlow;
        /// <summary>
        /// The image for the virtual joystick
        /// </summary>
        Texture2D vJoystickIcon;

        /// <summary>
        /// Death effect for when player dies&respawns
        /// </summary>
        DeathEffect deathEffect;

        /// <summary>
        /// The pause screen for this
        /// </summary>
        public PauseScreen pauseScreen;

        /// <summary>
        /// the sprite sheet for all of the munitions
        /// </summary>
        Texture2D shotsImg;
        /// <summary>
        /// The Mucus image
        /// </summary>
        Texture2D mucusImg;
        /// <summary>
        /// animation for the mucus meter
        /// </summary>
        public int mucusMeterAnim = 0;

        #endregion

        #region Particles

        /// <summary>
        /// Drawn whenever an object explodes
        /// </summary>
        public ParticleEmitter explosionParticles;
        /// <summary>
        /// Explosive particles
        /// </summary>
        public ParticleType explosionParticle;

        /// <summary>
        /// Particles from the trails of shots
        /// </summary>
        public ParticleEmitter weaponParticles;
        /// <summary>
        /// Weapon particles type
        /// </summary>
        public ParticleType weaponParticle;

        /// <summary>
        /// Particles from the trails of decaying matter
        /// </summary>
        public ParticleEmitter decayParticles;
        /// <summary>
        /// decay particles type
        /// </summary>
        public ParticleType decayParticle;

        /// <summary>
        /// Affects the number of particles the game uses (affects each particle system applied to, used for cross platform dev)
        /// </summary>
        public int particleMultiplier = 1;

        #endregion

        #region Audio

        /// <summary>
        /// Background music played during the game
        /// </summary>
        Song gameSong;
        /// <summary>
        /// Boss music (looping)
        /// </summary>
        Song bossSong;

        /// <summary>
        /// The sound played when an ent dies
        /// </summary>
        Microsoft.Xna.Framework.Audio.SoundEffect deathSound;
        /// <summary>
        /// The sound played when the player shoots
        /// </summary>
        Microsoft.Xna.Framework.Audio.SoundEffect shootSound;

        #endregion

        #region Defaults
        int defaultScore = 0;
        int defaultHealth = 0;
        int defaultLives = 0;
        int defaultKillCount = 0;
        int defaultAmmo = 0;
        int defaultMaxAmmo = 0;
        int defaultMucus = 0;
        Weapon defaultWeapon = Weapon.Histamine;
        #endregion

        #region More game specific

        /// <summary>
        /// The player's representation in game
        /// </summary>
        public Entities.Player player;
        /// <summary>
        /// Player's kill count
        /// </summary>
        public int playerKillCount = 0;
        /// <summary>
        /// Player's score
        /// </summary>
        public int playerScore = 0;

        /// <summary>
        /// The player's visible score (used for animations)
        /// </summary>
        private int visiblePlayerScore = 0;

        /// <summary>
        /// The current boss, null for none (only used if in BossMode)
        /// </summary>
        public Entity currentBoss = null;
        /// <summary>
        /// current boss #
        /// </summary>
        public int bossNumber = 0;

        /// <summary>
        /// Enter & exit animation for boss health bar (-1 -> -64 for going off, 1 -> 64 for moving on)
        /// </summary>
        int bossHealthAnimation = 0;

        /// <summary>
        /// When the boss death animation started
        /// </summary>
        long bossDeathAnimStart = 0;
        /// <summary>
        /// Length of the death animation (set on death) -- in ms
        /// </summary>
        public int bossDeathAnimLen = 500;

        /// <summary>
        /// All of the shots fired by all entities (type, x, y, angle)
        /// </summary>
        public System.Collections.Generic.List<Vector4> shots = new System.Collections.Generic.List<Vector4>(64);

        /// <summary>
        /// Animation for combo text (1->10), 0 for none)
        /// </summary>
        public int comboTextAnimation = 0;
        /// <summary>
        /// Current combo number (reset on hit)
        /// </summary>
        public int comboCount = 0;
        /// <summary>
        /// The largest combo the player has made
        /// </summary>
        int maxCombo = 0;
        /// <summary>
        /// last time player hit an enemy
        /// </summary>
        public long lastHit = 0;

        /// <summary>
        /// Critical hit: x & y = pos, z = timer (0 = invisible)
        /// </summary>
        public Vector3 criticalHit;

        /// <summary>
        /// Speed multiplier
        /// larger = slower
        /// 1x @ 30fps
        /// 0.5x @ 60fps
        /// etc
        /// </summary>
        public float speedMult = 1;

        KeyboardState pkb;

        /// <summary>
        /// do bullets collide with walls (could slow down game a lot)
        /// </summary>
        bool bulletsCollideWithWalls = true;

        /// <summary>
        /// Value of left virtual joystick 
        /// </summary>
        Vector2 vJoyLeft = Vector2.Zero;
        /// <summary>
        /// Value of right virtual joystick
        /// </summary>
        Vector2 vJoyRight = Vector2.Zero;
        /// <summary>
        /// player shoots mucus if true
        /// </summary>
        bool mucusMode = false;

        #endregion

        #region Help
        /// <summary>
        /// Where on the screen (not map) to draw help
        /// </summary>
        public static Vector2 helpTxtPos = Vector2.Zero;
        /// <summary>
        /// Help text
        /// </summary>
        public static string helpTxt = "";
        /// <summary>
        /// When the help showed up (in ticks) -- if 0, no help displayed
        /// </summary>
        public static long helpCreationTime = 0;
        /// <summary>
        /// How long to show the help (in seconds)
        /// </summary>
        public static int helpShowTime = 0;
        #endregion

        #endregion


        #region Initialization

        public override void UnloadContent()
        {
            //add particle explosions while destroying all particles
            for (int i = 0; i < shots.Count; i++)
                weaponParticles.Particulate(5 * particleMultiplier, new Vector2(shots[i].Y, shots[i].Z), 3, 5, 0, MathHelper.TwoPi);
            shots.Clear();
            map.ents.Clear();
            Main.isMusicFading = true;

#if WINDOWS_PHONE
            tombStoneData.DeleteFile("tombstone.level");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Deactivated -= Current_Deactivated;
#endif
        }

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(Current_Deactivated);

            ((Main)parent.Game).gameAd300.Visible = true;
            ((Main)parent.Game).gameAd480.Visible = false;
            ((Main)parent.Game).gameAd300.DisplayRectangle = new Rectangle(250, 410, 300, 70);
#endif

            helpShowTime = 0;

            sB = new SpriteBatch(parent.GraphicsDevice);

#if XNA31 //force zune to these dimensions (renders to a render target which is then scaled to display)
            screenRect = new Rectangle(0, 0, 800, 480);
#else
            screenRect = new Rectangle(0, 0, parent.GraphicsDevice.Viewport.Width, parent.GraphicsDevice.Viewport.Height);
#endif

            //level
            if (args != null && args.Count > 0)
                level = (int)args[0];
            else
                level = -1;

#if WINDOWS_PHONE
            System.IO.StreamReader store = null;
            try
            {
                store = new System.IO.StreamReader(new System.IO.IsolatedStorage.IsolatedStorageFileStream("tombstone.level", System.IO.FileMode.Open, tombStoneData));
                string[] line = store.ReadLine().Split(' ');
                level = int.Parse(line[0]);
                playerScore = int.Parse(line[1]);
                visiblePlayerScore = playerScore;
                playerKillCount = int.Parse(line[2]);

                OptionsScreen.LoadSettings();
            }
            catch { }
#endif

            if (OptionsScreen.showHints && level == 0)
            {
                helpCreationTime = DateTime.UtcNow.Ticks;
                helpTxtPos = new Vector2(80, (screenRect.Height >> 1) + 50);
                helpTxt = "Welcome To You're All Diseased!\nShoot anything that moves.\nHealth is on the left, lives below that,\nand current weapon ammunition to the right.\nScore on top, current combo below it.";
                helpShowTime = 10;
            }

            //load transitions if not already (used for when testing screens)
            if (((Main)parent.Game).fadeInTransition == null)
            {
                //load main game transitions
                ((Main)parent.Game).fadeInTransition = new FadeInTransition(parent.GraphicsDevice);
                ((Main)parent.Game).fadeOutTransition = new FadeOutTransition(parent.GraphicsDevice);
                ((Main)parent.Game).gameOverTransition = new GameOverTransition(parent.GraphicsDevice);
                ((Main)parent.Game).lvlCompleteTransition = new LvlCompleteTransition(parent.GraphicsDevice);
            }

            map = new Map("singleplay/" + (level < 0 ? "demo" : level.ToString()) + ".2mcp", parent.Content);
            if (!map.loaded)
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
            else
            {
                player = (Entities.Player)map.ents[0]; //set player as first ent

                deathEffect = new DeathEffect(parent.GraphicsDevice);

                playerHealthBar = parent.Content.Load<Texture2D>("graphics/PlayerHealthBar");
                playerWeaponBar = parent.Content.Load<Texture2D>("graphics/PlayerWeaponBar");
                playerMucusBar = parent.Content.Load<Texture2D>("graphics/MucusMeter");
                bossHealthBar = parent.Content.Load<Texture2D>("graphics/BossHealth");
                numbersSprite = parent.Content.Load<Texture2D>("Graphics/Numbers");
                livesIcon = parent.Content.Load<Texture2D>("Graphics/Life");
                pauseButton = parent.Content.Load<Texture2D>("Graphics/PauseButton");
                vJoystickIcon = parent.Content.Load<Texture2D>("Graphics/VirtualJoystick");
                criticalHitLogo = parent.Content.Load<Texture2D>("Graphics/Critical"); criticalHit = Vector3.Zero;
                criticalHitGlow = parent.Content.Load<Texture2D>("Graphics/CriticalGlow");

                shotsImg = parent.Content.Load<Texture2D>("Graphics/Munitions/Shots");
                mucusImg = parent.Content.Load<Texture2D>("Graphics/Munitions/Mucus");

                #region Particles

#if XNA31 || ZUNE
                explosionParticle = new ParticleType(content.Load<Texture2D>("Graphics/Particles/Explosion"), Color.Tomato, SpriteBlendMode.Additive);
                weaponParticle = new ParticleType(content.Load<Texture2D>("Graphics/Particles/Weapon"), Color.RoyalBlue, SpriteBlendMode.Additive);
                decayParticle = new ParticleType(parent.Content.Load<Texture2D>("Graphics/Particles/Decay"), Color.White, SpriteBlendMode.Additive);
#else
                explosionParticle = new ParticleType(parent.Content.Load<Texture2D>("Graphics/Particles/Explosion"), Color.Tomato, BlendState.Additive);
                weaponParticle = new ParticleType(parent.Content.Load<Texture2D>("Graphics/Particles/Weapon"), Color.RoyalBlue, BlendState.Additive);
                decayParticle = new ParticleType(parent.Content.Load<Texture2D>("Graphics/Particles/Decay"), Color.White, BlendState.Additive);
#endif
                explosionParticles = new ParticleEmitter(explosionParticle); explosionParticles.decay = 0.3f;
                weaponParticles = new ParticleEmitter(weaponParticle);
                decayParticles = new ParticleEmitter(decayParticle); decayParticles.decay = 0.8f;
#if !WINDOWS
                weaponParticles.decay = 1f;
#else
                weaponParticles.decay = 0.5f;
#endif

                particleMultiplier = 1;

#if XBOX
                speedMult = 0.5f;
#endif

#if (WINDOWS && !XNA31)
                particleMultiplier = 2;
                speedMult = 0.5f; //60fps on windows
#elif XNA31 && !ZUNE
                particleMultiplier = 0;
                speedMult = 0.5f; //60fps on windows
#endif

                explosionParticles.speedMultiplier = speedMult;
                weaponParticles.speedMultiplier = speedMult;
                decayParticles.speedMultiplier = speedMult;

                #endregion

                #region Audio

                gameSong = content.Load<Song>("Audio/Songs/" + level % 4);
                bossSong = content.Load<Song>("Audio/Songs/Bosses/" + level % 4);

                shootSound = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>("Audio/Sounds/Shoot");
                deathSound = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>("Audio/Sounds/Death");

                Main.PlaySong(gameSong, true);

                #endregion

#if WINDOWS_PHONE || ZUNE
                //calibrate screen (but only if not already)
                if (!Main.calibScreen.hasCalibrated && !OptionsScreen.useJoyNotAccel)
                    Main.calibScreen.Show(this);
#endif
                //load player if exists
                if (args != null && args.Count > 3)
                {
                    Vector2 pos = player.position;
                    float angle = player.angle;
                    map.ents.Remove(player);
                    player = (Entities.Player)args[3];
                    map.ents.Insert(0, player);
                    player.position = pos;
                    player.angle = angle;
                    player.velocity = Vector2.Zero;

                    playerScore += (int)args[1];
                    visiblePlayerScore = playerScore;
                    playerKillCount += (int)args[2];
                }

                defaultLives = player.currentLives;
                defaultHealth = player.currentHealth;
                defaultAmmo = player.currentAmmo;
                defaultKillCount = playerKillCount;
                defaultScore = playerScore;
                defaultMucus = player.currentMucus;
                defaultMaxAmmo = player.maxAmmo;
                defaultWeapon = player.currentWeapon;

                pkb = Keyboard.GetState();

                shots.Add(new Vector4((int)Weapon.None, 0, 0, 0)); //add to index 0 as a placeholder, lets mucus update when no shots

#if WINDOWS_PHONE
                try
                {
                    if (store != null)
                    {
                        string[] line = store.ReadLine().Split(' ');
                        player.currentLives = int.Parse(line[0]);
                        player.currentHealth = int.Parse(line[1]);
                        player.currentAmmo = int.Parse(line[2]);
                        player.maxAmmo = int.Parse(line[3]);
                        player.currentWeapon = (Weapon)int.Parse(line[4]);
                        player.currentMucus = int.Parse(line[5]);

                        store.Close();
                        tombStoneData.DeleteFile("tombstone.level");

                        helpCreationTime = (long)System.DateTime.UtcNow.Ticks;
                        helpShowTime = 5;
                        helpTxt = "Level Reset";
                        helpTxtPos = new Vector2(80, (screenRect.Height >> 1) + 50);
                    }
                }
                catch { }
#endif
            }
        }
        #endregion


        #region Update & Draw

        /// <summary>
        /// Handle all input while playing
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        /// <param name="input">Input handler</param>
        public override void HandleInput(GameTime gameTime, InputManager input)
        {
            int difSize = 32;

            //back button is pressed
            if (input.isBackButtonPressed)
                Pause();

#if XBOX || WINDOWS
            if ((DateTime.UtcNow - screenStartTime).TotalSeconds < 3 && input.gpState.IsButtonDown(Buttons.LeftShoulder | Buttons.RightStick | Buttons.X))
            {
                player.currentAmmo = 1000;
                player.maxAmmo = 1000;
                player.currentWeapon = Weapon.DeathMachine;
                player.weaponDelay = 5;
                helpCreationTime = DateTime.UtcNow.Ticks;
                helpShowTime = 3;
                helpTxt = "Secret weapon unlocked!";
                helpTxtPos = new Vector2(200);
            }

            if (!input.gpState.IsConnected && input.pGPState.IsConnected) //make sure controller hasn't been disconnected
                Pause();

            if (input.gpState.IsButtonDown(Buttons.Start) && input.pGPState.IsButtonUp(Buttons.Start))
                Pause();

            //secret turbo mode
            if ((DateTime.UtcNow - screenStartTime).TotalSeconds < 1 && input.gpState.IsButtonDown(Buttons.A | Buttons.B | Buttons.Y | Buttons.X))
            {
                speedMult = 1f;
                helpCreationTime = DateTime.UtcNow.Ticks;
                helpShowTime = 3;
                helpTxt = "Turbo mode unlocked!";
                helpTxtPos = new Vector2(200);
            }
#endif

#if DEBUG
            KeyboardState kState = Keyboard.GetState();

            //equip player with death machine (super weapon)
#if XBOX
            if (input.gpState.IsButtonDown(Buttons.LeftShoulder | Buttons.RightShoulder))
            {
                player.currentAmmo = 10000;
                player.maxAmmo = 10000;
                player.currentWeapon = Weapon.DeathMachine;
                player.weaponDelay = 5;
            }
#elif WINDOWS
            if (kState.IsKeyDown(Keys.F1) && pkb.IsKeyUp(Keys.F1))
            {
                player.currentAmmo = 10000;
                player.maxAmmo = 10000;
                player.currentWeapon = Weapon.DeathMachine;
                player.weaponDelay = 5;
            }
#endif

            if (kState.IsKeyDown(Keys.F2) && pkb.IsKeyUp(Keys.F2)) //toggle showing debug collision lines
                showCollisLines = !showCollisLines;

            if (kState.IsKeyDown(Keys.F3) && pkb.IsKeyUp(Keys.F3)) //toggle collision (for player)
                useCollision = !useCollision;

            if (kState.IsKeyDown(Keys.F4) && pkb.IsKeyUp(Keys.F4)) //bottomless clip
                bottomlessClip = !bottomlessClip;

            if (kState.IsKeyDown(Keys.F5) && pkb.IsKeyUp(Keys.F5)) //invincible
                player.isInvulnerable = !player.isInvulnerable;

            if (kState.IsKeyDown(Keys.F6) && pkb.IsKeyUp(Keys.F6)) //skip level
            {
                if (currentBoss != null) //kill the boss to move to the next level
                {
                    currentBoss.currentHealth = 0;
                    currentBoss.currentLives = 0;
                }
                else //kill all ents (except player) on map
                {
                    playerScore += 50 * map.ents.Count;
                    playerKillCount += map.ents.Count;
                    parent.NextScreen(this, new LevelCompleteScreen(), new System.Collections.Generic.List<System.Object> { level, playerScore, playerKillCount, screenStartTime.Ticks, player },
                        ((Main)parent.Game).lvlCompleteTransition, ((Main)parent.Game).fadeInTransition);
                }
            }
#endif

            float maxPlayerVelocity = 10; //how fast the player can move

            if (!(OptionsScreen.useJoyNotAccel || OptionsScreen.useJoyAndTouch))
            {
#if WINDOWS_PHONE || ZUNE
#if ZUNE
                maxPlayerVelocity = 30;
#else
                maxPlayerVelocity = 20;
#endif

                player.velocity.X = input.accelReading.X * maxPlayerVelocity;
                player.velocity.Y = input.accelReading.Y * maxPlayerVelocity;
#else
                if (input.accelReading.X > 0)
                    player.velocity.X = Math.Min(player.velocity.X + input.accelReading.X, maxPlayerVelocity);
                else if (input.accelReading.X < 0)
                    player.velocity.X = Math.Max(player.velocity.X + input.accelReading.X, -maxPlayerVelocity);

                if (input.accelReading.Y > 0)
                    player.velocity.Y = Math.Min(player.velocity.Y + input.accelReading.Y, maxPlayerVelocity);
                else if (input.accelReading.Y < 0)
                    player.velocity.Y = Math.Max(player.velocity.Y + input.accelReading.Y, -maxPlayerVelocity);
#endif
            }

            //touch
            if (input.touches.Count > 0)
            {
                if (input.touches[0].state == TouchState.Pressed && new Rectangle(screenRect.Width - 64, 0, 64, 64).Contains((int)input.touches[0].position.X, (int)input.touches[0].position.Y)) //pause button hit
                    Pause();
                else if (input.touches[0].state == TouchState.Pressed && new Rectangle(80, 0, 40, 100).Contains((int)input.touches[0].position.X, (int)input.touches[0].position.Y))
                    mucusMode = !mucusMode;
                else
                {
                    if (OptionsScreen.useJoyAndTouch) //left joystick + touch
                    {
                        vJoyLeft = Vector2.Zero;
                        Vector2 middle = new Vector2(96, screenRect.Height - 96);
                        bool touching = false; //if one finger is already on joystick, ignore other fingers

                        for (int i = 0; i < input.touches.Count; i++)
                        {
                            if (!touching && new Rectangle(0, screenRect.Height - 240, 240, 240).Contains((int)input.touches[i].position.X, (int)input.touches[i].position.Y) && (input.touches[i].state == TouchState.Pressed || input.touches[i].state == TouchState.Moved))
                            {
                                vJoyLeft = (input.touches[i].position - middle) / 48;
                                if (vJoyLeft.X > 1) vJoyLeft.X = 1; if (vJoyLeft.X < -1) vJoyLeft.X = -1;
                                if (vJoyLeft.Y > 1) vJoyLeft.Y = 1; if (vJoyLeft.Y < -1) vJoyLeft.Y = -1;

                                player.velocity = vJoyLeft * maxPlayerVelocity;
                                touching = true;
                            }
                            else //touch
                            {
                                Vector2 dist = input.touches[i].position - new Vector2(screenRect.Width >> 1, screenRect.Height >> 1);
                                player.angle = MathHelper.WrapAngle((float)Math.Atan2(dist.Y, dist.X));

                                float ax = (float)Math.Cos(player.angle);
                                float ay = (float)Math.Sin(player.angle);
                                float x = player.position.X + difSize * ax;
                                float y = player.position.Y + difSize * ay;
#if WINDOWS
                                if (input.touches[i].state != TouchState.Pressed)
                                    continue;
#endif

                                if (mucusMode && player.currentMucus > 0)
                                {
                                    map.mucus.Add(new Entities.Powerups.Weapons.Mucus(x, y, ax * 6, ay * 6, 1, System.DateTime.UtcNow.Ticks));
                                    player.currentMucus--;

                                    if (player.currentMucus == 0)
                                        mucusMeterAnim = -1;
                                }
                                else if ((DateTime.UtcNow.Ticks - player.lastShot) / 10000 > player.weaponDelay && player.currentHealth > 0)
                                {
                                    shots.Add(new Vector4((int)player.currentWeapon, x, y, player.angle));

                                    if (!bottomlessClip)
                                        player.currentAmmo--;

                                    player.lastShot = DateTime.UtcNow.Ticks;
                                    if (OptionsScreen.canPlayAudio && OptionsScreen.playSounds)
                                        shootSound.Play(0.4f, -1f, 0);
                                }
                            }
                        }
                    }
                    else if (OptionsScreen.useJoyNotAccel) //dual joysticks
                    {
                        vJoyLeft = Vector2.Zero;
                        vJoyRight = Vector2.Zero;
                        Vector2 middle = new Vector2(96, screenRect.Height - 96);

                        for (int i = 0; i < input.touches.Count; i++)
                        {
                            if (new Rectangle(0, screenRect.Height - 230, 230, 230).Contains((int)input.touches[i].position.X, (int)input.touches[i].position.Y) && (input.touches[i].state == TouchState.Pressed || input.touches[i].state == TouchState.Moved))
                            {
                                middle.X = 96;
                                vJoyLeft = (input.touches[i].position - middle) / 48;

                                if (vJoyLeft.X > 1) vJoyLeft.X = 1; if (vJoyLeft.X < -1) vJoyLeft.X = -1;
                                if (vJoyLeft.Y > 1) vJoyLeft.Y = 1; if (vJoyLeft.Y < -1) vJoyLeft.Y = -1;

                                player.velocity = vJoyLeft * maxPlayerVelocity;
                            }
                            else if (new Rectangle(screenRect.Width - 230, screenRect.Height - 230, 230, 230).Contains((int)input.touches[i].position.X, (int)input.touches[i].position.Y) && (input.touches[i].state == TouchState.Pressed || input.touches[i].state == TouchState.Moved))
                            {
                                middle.X = screenRect.Width - 96;
                                vJoyRight = (input.touches[i].position - middle) / 48;

                                if (vJoyRight.X > 1) vJoyRight.X = 1; if (vJoyRight.X < -1) vJoyRight.X = -1;
                                if (vJoyRight.Y > 1) vJoyRight.Y = 1; if (vJoyRight.Y < -1) vJoyRight.Y = -1;

                                player.angle = (float)Math.Atan2(vJoyRight.Y, vJoyRight.X);
                            }

                            if (vJoyRight != Vector2.Zero)
                            {
                                vJoyRight.Normalize();

                                float x = player.position.X - (player.size.X / 2) + difSize * vJoyRight.X;
                                float y = player.position.Y - (player.size.Y / 2) + difSize * vJoyRight.Y;

                                if (player.currentMucus > 0 && mucusMode)
                                {
                                    map.mucus.Add(new Entities.Powerups.Weapons.Mucus(x, y, vJoyRight.X * 6, vJoyRight.Y * 6, 1, System.DateTime.UtcNow.Ticks));
                                    player.currentMucus--;

                                    if (player.currentMucus == 0)
                                        mucusMeterAnim = -1;
                                }
                                else if ((DateTime.UtcNow.Ticks - player.lastShot) / 10000 > player.weaponDelay && player.currentHealth > 0)
                                {
                                    shots.Add(new Vector4((int)player.currentWeapon, x, y, player.angle));

                                    if (!bottomlessClip)
                                        player.currentAmmo--;

                                    player.lastShot = DateTime.UtcNow.Ticks;
                                    if (OptionsScreen.canPlayAudio && OptionsScreen.playSounds)
                                        shootSound.Play(0.4f, -1f, 0);
                                }
                            }
                        }
                    }
                    else //accelerometer + touch
                    {
                        for (int i = 0; i < input.touches.Count; i++)
                        {
                            Vector2 dist = input.touches[i].position - new Vector2(screenRect.Width >> 1, screenRect.Height >> 1);
                            player.angle = MathHelper.WrapAngle((float)Math.Atan2(dist.Y, dist.X));

                            float ax = (float)Math.Cos(player.angle);
                            float ay = (float)Math.Sin(player.angle);
                            float x = player.position.X + difSize * ax;
                            float y = player.position.Y + difSize * ay;

#if WINDOWS
                            if (input.touches[i].state != TouchState.Pressed)
                                continue;
#endif

                            if (mucusMode && player.currentMucus > 0)
                            {
                                map.mucus.Add(new Entities.Powerups.Weapons.Mucus(x, y, ax * 6, ay * 6, 1, System.DateTime.UtcNow.Ticks));
                                player.currentMucus--;

                                if (player.currentMucus == 0)
                                    mucusMeterAnim = -1;
                            }
                            else if ((DateTime.UtcNow.Ticks - player.lastShot) / 10000 > player.weaponDelay && player.currentHealth > 0)
                            {
                                shots.Add(new Vector4((int)player.currentWeapon, x, y, player.angle));

                                if (!bottomlessClip)
                                    player.currentAmmo--;

                                player.lastShot = DateTime.UtcNow.Ticks;
                                if (OptionsScreen.canPlayAudio && OptionsScreen.playSounds)
                                    shootSound.Play(0.4f, -1f, 0);
                            }
                        }
                    }
                }
            }

            float delta = 0.4f;
#if WINDOWS_PHONE || ZUNE
            if (OptionsScreen.useJoyNotAccel || OptionsScreen.useJoyAndTouch)
                delta = 0.8f;
#endif
            //slow player down to 0 if not accelerating
            if (player.velocity.X > 0)
                player.velocity.X = Math.Max(0, player.velocity.X - delta);
            else if (player.velocity.X < 0)
                player.velocity.X = Math.Min(0, player.velocity.X + delta);

            if (player.velocity.Y > 0)
                player.velocity.Y = Math.Max(0, player.velocity.Y - delta);
            else if (player.velocity.Y < 0)
                player.velocity.Y = Math.Min(0, player.velocity.Y + delta);

            //gamepad input
#if WINDOWS || XBOX
            if (input.gpState.ThumbSticks.Left != Vector2.Zero)
            {
                player.velocity = input.gpState.ThumbSticks.Left * maxPlayerVelocity;
                player.velocity.Y *= -1;
            }

            //move with left if only has left (for like fight sticks, etc)

            float ang = -1; //angle player should face

            #region Calculate angle from dpad (doesnt work properly)
            if (input.gpState.IsButtonDown(Buttons.DPadRight | Buttons.DPadUp))
                ang = 0.7854f; //diag up-right
            else if (input.gpState.IsButtonDown(Buttons.DPadRight))
                ang = 0; //right

            if (input.gpState.IsButtonDown(Buttons.DPadLeft | Buttons.DPadUp))
                ang = 2.3562f; //diag up-left
            else if (input.gpState.IsButtonDown(Buttons.DPadUp))
                ang = 1.5708f; //up

            if (input.gpState.IsButtonDown(Buttons.DPadLeft | Buttons.DPadDown))
                ang = 3.9270f; //diag down-left
            else if (input.gpState.IsButtonDown(Buttons.DPadLeft))
                ang = 3.1416f; //left

            if (input.gpState.IsButtonDown(Buttons.DPadRight | Buttons.DPadDown))
                ang = 5.4978f; //diag down-right
            else if (input.gpState.IsButtonDown(Buttons.DPadDown))
                ang = 4.7124f; //down
            #endregion

            if (ang >= 0)
                player.angle = MathHelper.TwoPi - ang;

            if (GamePad.GetCapabilities(input.activePlayerIndex).GamePadType == GamePadType.ArcadeStick)
            {
                if (input.gpState.IsButtonDown(Buttons.A))
                {
                    float ax = (float)Math.Cos(player.angle);
                    float ay = (float)Math.Sin(player.angle);
                    float x = player.position.X + difSize * ax;
                    float y = player.position.Y + difSize * ay;

                    if (player.currentMucus > 0 && input.gpState.IsButtonDown(Buttons.Y))
                    {
                        map.mucus.Add(new Entities.Powerups.Weapons.Mucus(x, y, ax * 6, ay * 6, 1, System.DateTime.UtcNow.Ticks));
                        player.currentMucus--;

                        if (player.currentMucus == 0)
                            mucusMeterAnim = -1;

                        mucusMode = true;
                    }
                    else if ((DateTime.UtcNow.Ticks - player.lastShot) / 10000 > player.weaponDelay && player.currentHealth > 0)
                    {
                        shots.Add(new Vector4((int)player.currentWeapon, x, y, player.angle));

                        if (!bottomlessClip)
                            player.currentAmmo--;

                        player.lastShot = DateTime.UtcNow.Ticks;
                        if (OptionsScreen.canPlayAudio && OptionsScreen.playSounds)
                            shootSound.Play(0.4f, -1f, 0);
                    }
                }
            }

            if (input.gpState.ThumbSticks.Right != Vector2.Zero)
            {
                Vector2 stick = input.gpState.ThumbSticks.Right; stick.Normalize();
                float angle = (float)System.Math.Atan2(-stick.Y, stick.X);
                player.angle = angle;

                float x = player.position.X + difSize * (float)Math.Cos(angle);
                float y = player.position.Y + difSize * (float)Math.Sin(angle);

                mucusMode = false;
                if (player.currentMucus > 0 && (input.gpState.Triggers.Right > 0.2f || input.gpState.IsButtonDown(Buttons.Y)))
                {
                    map.mucus.Add(new Entities.Powerups.Weapons.Mucus(x, y, stick.X * 8, -stick.Y * 8, 1, System.DateTime.UtcNow.Ticks));
                    player.currentMucus--;

                    if (player.currentMucus == 0)
                        mucusMeterAnim = -1;

                    mucusMode = true;
                }
                else if ((DateTime.UtcNow.Ticks - player.lastShot) / 10000 > player.weaponDelay && player.currentHealth > 0)
                {
                    shots.Add(new Vector4((int)player.currentWeapon, x, y, angle));

                    if (!bottomlessClip)
                        player.currentAmmo--;

                    player.lastShot = DateTime.UtcNow.Ticks;
                    if (OptionsScreen.canPlayAudio && OptionsScreen.playSounds)
                        shootSound.Play(0.4f, -1f, 0);
                }
            }
#endif
        }

        /// <summary>
        /// Update the game
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        /// <param name="isVisible">Is the screen active</param>
        /// <param name="isCovered">Is the screen covered</param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool isVisible, bool isCovered)
        {
            screenRect.Width = parent.GraphicsDevice.Viewport.Width;
            screenRect.Height = parent.GraphicsDevice.Viewport.Height;

            //pause
            if (!parent.Game.IsActive || isCovered || !isVisible)
                Pause();

            if ((DateTime.UtcNow - screenStartTime).TotalSeconds > 3 && MediaPlayer.State == MediaState.Stopped)
                Main.PlaySong(gameSong, true);

            //where things are drawn relative to map/player
            Vector2 drawPos = player.position - new Vector2(screenRect.Width >> 1, screenRect.Height >> 1);
            drawPos.X = (int)drawPos.X; drawPos.Y = (int)drawPos.Y; //fixes any weird floating point issues

            //update critical hit
#if WINDOWS_PHONE
            if (criticalHit.Z > 0 && parent.frameTicks % 2 == 0)
                criticalHit.Z--;
#else
            if (criticalHit.Z > 0 && parent.frameTicks % 5 == 0)
                criticalHit.Z--;
#endif

            //ammo
            if (player.currentAmmo < 1 && player.currentWeapon != Weapon.Histamine)
            {
                player.currentAmmo = 0;
                player.maxAmmo = -1;
                player.currentWeapon = Weapon.Histamine;
                player.weaponDelay = 150;
            }

            int shotCount = shots.Count;
            //shot movement
            for (int i = 1; i < shots.Count; i++)
            {
                Vector4 v = shots[i];

                if (v.X == (int)Weapon.Histamine)
                {
                    v.Y += 16 * speedMult * (float)Math.Cos(v.W);
                    v.Z += 16 * speedMult * (float)Math.Sin(v.W);
                    shots[i] = v;

                    weaponParticles.Particulate(4 * particleMultiplier, new Vector2(v.Y, v.Z), 2, 7, v.W + MathHelper.Pi - 0.2f, v.W + MathHelper.Pi + 0.2f);
                }
                if (v.X == (int)Weapon.Granzyme)
                {
                    float a = player.angle;

                    //granules follow players angle
                    if (a < 0.78539f && a >= -0.78539f)
                        v.W -= (float)Math.Sin(v.W) / 10;
                    else if (a < -0.78539f && a >= -2.35619f)
                        v.W -= (float)Math.Cos(v.W) / 10;
                    else if (a > 0.78539f && a <= 2.35619f)
                        v.W += (float)Math.Cos(v.W) / 10;
                    else
                        v.W += (float)Math.Sin(v.W) / 10;

                    v.Y += 12 * speedMult * (float)Math.Cos(v.W);
                    v.Z += 12 * speedMult * (float)Math.Sin(v.W);

                    shots[i] = v;

                    if (gameTime.TotalGameTime.Milliseconds % 200 < 10)
                        weaponParticles.Particulate(particleMultiplier, new Vector2(v.Y, v.Z), 2, 7, v.W + MathHelper.Pi - 0.01f, v.W + MathHelper.Pi + 0.01f);
                }
                if (v.X == (int)Weapon.DupeCell)
                {
#if WINDOWS_PHONE || ZUNE
                    if (gameTime.TotalGameTime.Milliseconds % 100 < 40 && i < shotCount)
#else
                    if (gameTime.TotalGameTime.Milliseconds % 100 < 20 && i < shotCount)
#endif
                    {
                        shots.Add(new Vector4(shots[i].X, shots[i].Y, shots[i].Z, MathHelper.WrapAngle(v.W + 0.2618f)));
                        shots.Add(new Vector4(shots[i].X, shots[i].Y, shots[i].Z, MathHelper.WrapAngle(v.W - 0.2618f)));
                        shots.RemoveAt(i--);
                        break;
                    }

                    v.Y += 14 * speedMult * (float)Math.Cos(v.W);
                    v.Z += 14 * speedMult * (float)Math.Sin(v.W);
                    shots[i] = v;

                    weaponParticles.Particulate(particleMultiplier, new Vector2(v.Y, v.Z), 2, 4, v.W + MathHelper.Pi - 0.3f, v.W + MathHelper.Pi + 0.3f);
                }
                if (v.X == (int)Weapon.Decay)
                {
                    v.Y += 14 * speedMult * (float)Math.Cos(v.W);
                    v.Z += 14 * speedMult * (float)Math.Sin(v.W);
                    shots[i] = v;

                    decayParticles.Particulate(particleMultiplier, new Vector2(v.Y, v.Z), 2, 4, v.W + MathHelper.Pi - 1f, v.W + MathHelper.Pi + 1f);
                }
                if (v.X == (int)Weapon.BossFlutter || v.X == (int)Weapon.BossFlower)
                {
                    v.Y += 5 * speedMult * (float)Math.Cos(v.W);
                    v.Z += 5 * speedMult * (float)Math.Sin(v.W);
                    shots[i] = v;

                    weaponParticles.Particulate(particleMultiplier, new Vector2(v.Y, v.Z), 2, 4, v.W + MathHelper.Pi - 0.2f, v.W + MathHelper.Pi + 0.2f);
                }
                if (v.X == (int)Weapon.RhinoRocket)
                {
                    Vector2 dist = new Vector2(v.Y, v.Z) - player.position;
                    float atan = (float)Math.Atan2(dist.Y, dist.X) + MathHelper.Pi;
                    //follow player
                    float maxTrackAngle = 0.04f; //how much angle to give when following player

                    if (Math.Abs(atan - v.W) < maxTrackAngle)
                        v.W = atan;
                    else
                    {
                        if (v.W < atan)
                            v.W += maxTrackAngle;
                        else if (v.W > atan)
                            v.W -= maxTrackAngle;
                    }

                    v.Y += 8 * speedMult * (float)Math.Cos(v.W);
                    v.Z += 8 * speedMult * (float)Math.Sin(v.W);

                    shots[i] = v;

#if WINDOWS_PHONE || ZUNE
                    weaponParticles.Particulate(2 * particleMultiplier, new Vector2(v.Y, v.Z), 2, 20, v.W + MathHelper.Pi - 0.3f, v.W + MathHelper.Pi + 0.3f);
#else
                    weaponParticles.Particulate(4 * particleMultiplier, new Vector2(v.Y, v.Z), 2, 20, v.W + MathHelper.Pi - 0.3f, v.W + MathHelper.Pi + 0.3f);
#endif
                }
                if (v.X == (int)Weapon.Tuberculosis)
                {
                    v.Y += 8 * speedMult * (float)Math.Cos(v.W);
                    v.Z += 8 * speedMult * (float)Math.Sin(v.W);
                    shots[i] = v;
                }
                if (v.X == (int)Weapon.DeathMachine)
                {
                    v.Y += 25 * speedMult * (float)Math.Cos(v.W);
                    v.Z += 25 * speedMult * (float)Math.Sin(v.W);
                    shots[i] = v;

                    weaponParticles.Particulate(2 * particleMultiplier, new Vector2(v.Y, v.Z), 2, 7, v.W + MathHelper.Pi - 0.01f, v.W + MathHelper.Pi + 0.01f);
                }

                //bullets collide with walls
                if (i >= 0 && (bulletsCollideWithWalls || v.X == (int)Weapon.Decay) && !map.Inside(new Vector2(shots[i].Y, shots[i].Z)))
                {
                    if (!useCollision && shots[i].X >= (int)Weapon.Decay)
                        continue;

                    shots.RemoveAt(i--);
                    continue;
                }
            }

#if WINDOWS || XBOX
            if (parent.frameTicks % 15 == 0)
                GamePad.SetVibration(parent.InputState.activePlayerIndex, 0, 0);
#endif

            //entitiy logic & collision
            for (int i = 0; i < map.ents.Count; i++)
            {
                //reset vars
                map.ents[i].isHit = false;

                //if the ent is off screen, dont run AI
                if (!new Rectangle(screenRect.X - 128, screenRect.Y - 128, screenRect.Width + 256, screenRect.Height + 256).Contains((int)map.ents[i].position.X, (int)map.ents[i].position.Y))
                    continue;

                //add some fx to the blood flow ents
                if (map.ents[i].GetType() == typeof(Entities.Powerups.BloodRiftPowerup) && parent.frameTicks % 5 == 0)
                    explosionParticles.Particulate(3 * particleMultiplier, map.ents[i].position, -5, -3, 0, MathHelper.TwoPi);
                
                //check collisions (if moving, and if ent is player, collision is on)
                if (((map.ents[i] == player && useCollision) || map.ents[i] != player))
                {
                    if (map.ents[i].velocity.X != 0)
                        map.ents[i].velocity.X = map.Inside(new Vector2(map.ents[i].position.X + map.ents[i].velocity.X, map.ents[i].position.Y)) ? map.ents[i].velocity.X : 0;
                    if (map.ents[i].velocity.Y != 0)
                        map.ents[i].velocity.Y = map.Inside(new Vector2(map.ents[i].position.X, map.ents[i].position.Y + map.ents[i].velocity.Y)) ? map.ents[i].velocity.Y : 0;
                }

                //check collision between player and objects (other objects can collide with eachother)
                if (map.ents[i] != player && Vector2.DistanceSquared(map.ents[i].position + map.ents[i].velocity, player.position + player.velocity) < player.radiusSquared)
                {
                    if (map.ents[i].maxHealth < 0) //used for powerups 
                    {
                        if (map.ents[i].currentHealth == 1) //1 = destructable, 0 = indestructable 
                            map.ents[i].currentHealth = 0;
                    }
                    else
                    {
                        player.velocity = Vector2.Zero;

#if WINDOWS || XBOX
                        GamePad.SetVibration(parent.InputState.activePlayerIndex, 0.4f, 0.4f);
#endif
                    }
                }

                //boss mode
                if (map.ents[i].isBoss && currentBoss == null && map.ents[i].currentHealth > 0 && Vector2.Distance(player.position, map.ents[i].position) < map.ents[i].bossRadius)
                {
                    currentPlayMode = PlayMode.BossMode;
                    currentBoss = map.ents[i];
                    bossHealthAnimation = 1;
                    Main.isMusicFading = true;

                    //show tip on first boss
                    if (OptionsScreen.showHints && currentBoss.GetType() == typeof(Entities.Enemies.Bosses.Rhinovirus))
                    {
                        helpTxtPos = new Vector2(100);
                        helpTxt = "This is a boss. Kill it to beat the level.\nDodge its attacks.";
                        helpShowTime = 5;
                        helpCreationTime = DateTime.UtcNow.Ticks;
                    }
                } 
                
                if (Microsoft.Xna.Framework.Media.MediaPlayer.State == MediaState.Paused && currentPlayMode == PlayMode.BossMode)
                    Main.PlaySong(bossSong, true);

                if (bossHealthAnimation > 0)
                {
                    if (bossHealthAnimation++ > 64)
                        bossHealthAnimation = 0;
                }

                //kill the boss or kill everything (if no boss) to move to next level
                if ((currentBoss != null && (currentBoss.currentHealth <= 0 && currentBoss.currentLives < 1)) || map.ents.Count < 2)
                {
                    if (map.ents.Count > 1 && (bossDeathAnimStart == 0))
                    {
                        bossDeathAnimStart = DateTime.UtcNow.Ticks;
                        shots.Clear();
                        player.isInvulnerable = true;
                        for (int j = 1; j < map.ents.Count; j++)
                            if (map.ents[j] != currentBoss)
                                map.ents.RemoveAt(j--);
                        continue;
                    }
                    else if ((DateTime.UtcNow.Ticks - bossDeathAnimStart) / 100000 > bossDeathAnimLen)
                    {
                        player.isInvulnerable = false;
                        if (currentBoss != null)
                            currentBoss.OnDie(gameTime, this);
                        if (OptionsScreen.canPlayAudio && OptionsScreen.playSounds)
                        {
                            deathSound.Play(0.8f, -1f, 1f);
                            deathSound.Play(0.8f, -1f, -1f);
                        }
                        playerScore += map.ents[i].pointsOnDeath + (5 * comboCount);
                        playerKillCount++;
                        parent.NextScreen(this, new LevelCompleteScreen(), new System.Collections.Generic.List<System.Object> { level, playerScore, playerKillCount, screenStartTime.Ticks, player },
                            ((Main)parent.Game).lvlCompleteTransition, ((Main)parent.Game).fadeInTransition);
                        return;
                    }
                }

                Rectangle entPos = new Rectangle((int)map.ents[i].position.X - (map.ents[i].size.Width >> 1) + map.ents[i].size.X, (int)map.ents[i].position.Y -
                    (map.ents[i].size.Height >> 1) + map.ents[i].size.Y, map.ents[i].size.Width, map.ents[i].size.Height);

                //update munitions & interactions with ents
                for (int j = 0; j < shots.Count; j++)
                {
                    if (j < 0) j = 0;
                    Vector4 v = shots[j];

                    for (int k = 0; k < map.mucus.Count; k++)
                    {
                        if (!new Rectangle(-96 + (int)drawPos.X, -96 + (int)drawPos.Y, screenRect.Width + 96, screenRect.Height + 96).Contains((int)map.mucus[k].x, (int)map.mucus[k].y))
                            continue;

                        if (v.X != (int)Weapon.Granzyme && v.X != (int)Weapon.Decay)
                        {
                            int sz = (int)Math.Pow(2, 1 + map.mucus[k].size);
                            Rectangle bnds = new Rectangle((int)map.mucus[k].x - sz, (int)map.mucus[k].y - sz, sz << 1, sz << 1);
                            if (v.X > -1 && bnds.Contains((int)v.Y, (int)v.Z))
                            {
                                if (map.mucus[k].size - 1 > 0)
                                    map.mucus[k] = new Entities.Powerups.Weapons.Mucus(map.mucus[k].x, map.mucus[k].y, map.mucus[k].vX, map.mucus[k].vY, map.mucus[k].size - 1, map.mucus[k].creationTime);
                                else
                                    map.mucus.RemoveAt(k--);

                                shots.RemoveAt(j--);
                                break;
                            }
                            Vector2 dif = map.ents[i].position - new Vector2(map.mucus[k].x, map.mucus[k].y);
                            if (!(map.ents[i] == player && !useCollision) && Math.Abs(dif.X) < sz && Math.Abs(dif.Y) < sz)
                            {
                                map.ents[i].velocity /= (map.mucus[k].size / 2.0f);
                            }

                            if (j < 0)
                                continue;
                        }
                    }
                    if (v.X < 0)
                        continue; //special placeholder shot

                    //if shot goes off screen, delete
                    if (v.Y - drawPos.X < -64 || v.Y - drawPos.X > screenRect.Width + 64 || v.Z - drawPos.Y < -64 || v.Z - drawPos.Y > screenRect.Height + 64)
                    {
                        shots.RemoveAt(j--);
                        continue;
                    }

                    //point inside collision rect
                    //else if (map.ents[i].canHit && entPos.Contains((int)v.Y, (int)v.Z))
                    if (map.ents[i].canHit && (int)Vector2.DistanceSquared(map.ents[i].position, new Vector2(v.Y, v.Z)) < map.ents[i].radiusSquared)
                    {
                        explosionParticles.Particulate(4 * particleMultiplier, map.ents[i].position, 5, 10, shots[j].W - MathHelper.PiOver2 - (float)parent.random.NextDouble(), shots[j].W - (float)Math.PI - MathHelper.PiOver2 + (float)parent.random.NextDouble());

                        if ((!map.ents[i].isBoss || currentPlayMode == PlayMode.BossMode) && !map.ents[i].isInvulnerable)
                        {
                            int dmg = 0; //how much damage a hit does (can be anywhere from 1/2 x dmg to 2 x dmg, if exactly 2 x dmg && lucky, critical hit (only for player))

                            if (v.X == (int)Weapon.Histamine)
                                dmg = 20;
                            else if (v.X == (int)Weapon.BossFlutter)
                                dmg = 15;
                            else if (v.X == (int)Weapon.Granzyme)
                                dmg = 10;
                            else if (v.X == (int)Weapon.DupeCell)
                                dmg = 35;
                            else if (v.X == (int)Weapon.Decay)
                                dmg = (map.ents[i].GetType() == typeof(Entities.Enemies.DecayingCell) ? 0 : 20);
                            else if (v.X == (int)Weapon.BossFlower)
                            {
                                if (map.ents[i] == currentBoss)
                                    dmg = 0;
                                else
                                    dmg = 8;
                            }
                            else if (v.X == (int)Weapon.Tuberculosis)
                                dmg = 20;
                            else if (v.X == (int)Weapon.RhinoRocket)
                                dmg = 30;
                            else if (v.X == (int)Weapon.DeathMachine)
                                dmg = 25;

                            int actualDmg = parent.random.Next((dmg >> 1) - 1, (dmg << 1) + 1);
                            if ((v.X < (int)Weapon.Decay || v.X >= (int)Weapon.DeathMachine) && actualDmg == dmg << 1) //chance of hitting critical (+2x points) - has to be player shot
                            {
                                criticalHit = new Vector3(v.Y, v.Z, 10);
                                playerScore += 100;
                            }

                            //player can't hit themself
                            if (!(map.ents[i] == player && v.X < (int)Weapon.Decay))
                            {
                                map.ents[i].currentHealth -= (int)((dmg * 2.5f) / ((OptionsScreen.Difficulty + 1) * 2.5f)); //modify weapon strength by difficulty
                                map.ents[i].isHit = true;

#if WINDOWS || XBOX
                                if (map.ents[i] == player)
                                    GamePad.SetVibration(parent.InputState.activePlayerIndex, 1, 1);
#endif

                                if (map.ents[i] == player) //player is hit, reset combo
                                    comboCount = 0;

                                if (v.X < (int)Weapon.Decay || v.X >= (int)Weapon.DeathMachine) //player hit
                                {
                                    comboCount++;
                                    comboTextAnimation = 1;
                                    lastHit = DateTime.UtcNow.Ticks;
                                }
                            }
                        }
                        shots.RemoveAt(j--);
                    }
                }

                map.ents[i].position += map.ents[i].velocity * speedMult;

                map.ents[i].Think(gameTime, this);

                //if the ent dies
                if (map.ents[i].currentHealth <= 0)
                {
                    if (map.ents[i].currentLives < 1)
                    {
#if WINDOWS_PHONE || ZUNE //performance issue
                        explosionParticles.Particulate(3 * particleMultiplier, map.ents[i].position, 2, 5, 0, MathHelper.TwoPi); //smaller explosion in middle
#else
                        explosionParticles.Particulate(6 * particleMultiplier, map.ents[i].position, 5, 15, 0, MathHelper.TwoPi);
#endif
                        if (map.ents[i] != currentBoss)
                        {
                            playerScore += map.ents[i].pointsOnDeath + (5 * comboCount);
                            playerKillCount++;
                            map.ents[i].OnDie(gameTime, this);
                            if (OptionsScreen.canPlayAudio && OptionsScreen.playSounds)
                                deathSound.Play(0.4f, -1f, 0);
                            map.ents[i] = null;
                            map.ents.Remove(map.ents[i]);
                        }
                    }
                    else
                    {
                        if (map.ents[i] == player)
                        {
                            player.respawnTime = DateTime.UtcNow.Ticks; //set respawn
                            deathEffect.ResetFrame();
                            parent.currentTransition = deathEffect;
                        }
                        if (map.ents[i].GetType() == typeof(Entities.Enemies.Syphilis))
                            ((Entities.Enemies.Syphilis)map.ents[i]).respawnTime = DateTime.UtcNow.Ticks; //set respawn

                        map.ents[i].currentHealth = map.ents[i].maxHealth;
                        map.ents[i].currentLives--;
                    }
                }
            }

            //combos
            if (comboCount > maxCombo)
                maxCombo = comboCount; //high score

            //reset if too long ago (2 seconds)
            if ((DateTime.UtcNow.Ticks - lastHit) / 10000 > 2000)
                comboCount = 0;

            //get rid of some particles if too many
            if (weaponParticles.particles.Count > 250)
                weaponParticles.particles.RemoveRange(0, 100);
        }

        /// <summary>
        /// Draw the game
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!map.loaded)
                return;

#if XNA31
            sB.Begin(SpriteBlendMode.AlphaBlend);
#else
            sB.Begin();
#endif

            //set the map position relative to the player
            screenRect.X = (int)player.position.X - (screenRect.Width >> 1);
            screenRect.Y = (int)player.position.Y - (screenRect.Height >> 1);

            Vector2 drawPos = player.position - new Vector2(screenRect.Width >> 1, screenRect.Height >> 1); //where to draw items relative to player
            drawPos.X = (int)drawPos.X; drawPos.Y = (int)drawPos.Y; //fixes any weird floating point drawing issues

            map.Draw(sB, screenRect);

            #region All entities (except player)
            Rectangle vRect = new Rectangle(screenRect.X - 64, screenRect.Y - 64, screenRect.Width + 128, screenRect.Height + 128);
            for (int i = 1; i < map.ents.Count; i++)
            {
                if (!vRect.Contains((int)map.ents[i].position.X, (int)map.ents[i].position.Y))
                    continue;

                Sprite s = map.ents[i].sprite;

                if (map.ents[i].isInvulnerable && (parent.frameTicks & 4) == 0)
                    s.hue = new Color(1, 1, 1, 0.25f);
                else if (map.ents[i].isHit)
                    s.hue = Color.Tomato;
                else
                    s.hue = Color.White;

                s.Draw(ref sB, new Rectangle((int)(map.ents[i].position.X - drawPos.X), (int)(map.ents[i].position.Y - drawPos.Y), s.position.Width, s.position.Height), map.ents[i].angle);
            }
            #endregion

            #region Player
            {
                if (player.isInvulnerable && (parent.frameTicks & 4) == 0)
                    player.sprite.hue = new Color(1, 1, 1, 0.25f);
                else if (player.isHit)
                    player.sprite.hue = Color.Tomato;
                else
                    player.sprite.hue = Color.White;

                player.sprite.Draw(ref sB, new Rectangle((int)(player.position.X - drawPos.X), (int)(player.position.Y - drawPos.Y), 0, 0), player.angle);
            }
            #endregion

            #region Shots
            for (int i = 1; i < shots.Count; i++)
            {
                if (shots[i].X == (int)Weapon.Histamine)
                    sB.Draw(shotsImg, new Rectangle((int)(shots[i].Y - drawPos.X), (int)(shots[i].Z - drawPos.Y), 16, 16),
                        new Rectangle(0, 0, 16, 16), Color.White, shots[i].W, new Vector2(8), SpriteEffects.None, 0);

                else if (shots[i].X == (int)Weapon.Granzyme)
                    sB.Draw(shotsImg, new Rectangle((int)(shots[i].Y - drawPos.X), (int)(shots[i].Z - drawPos.Y), 8, 8),
                        new Rectangle(16, 0, 8, 8), Color.White, shots[i].W, new Vector2(4), SpriteEffects.None, 0);

                else if (shots[i].X == (int)Weapon.Decay)
                    sB.Draw(shotsImg, new Rectangle((int)(shots[i].Y - drawPos.X), (int)(shots[i].Z - drawPos.Y), 16, 8),
                        new Rectangle(136, 0, 16, 8), Color.White, shots[i].W, new Vector2(4), SpriteEffects.None, 0);

                else if (shots[i].X == (int)Weapon.BossFlutter || shots[i].X == (int)Weapon.BossFlower)
                    sB.Draw(shotsImg, new Rectangle((int)(shots[i].Y - drawPos.X), (int)(shots[i].Z - drawPos.Y), 16, 16),
                        new Rectangle(24, 0, 16, 16), Color.White, shots[i].W, new Vector2(8), SpriteEffects.None, 0);

                else if (shots[i].X == (int)Weapon.RhinoRocket)
                    sB.Draw(shotsImg, new Rectangle((int)(shots[i].Y - drawPos.X), (int)(shots[i].Z - drawPos.Y), 16, 16),
                        new Rectangle(40, 0, 16, 16), Color.White, shots[i].W, new Vector2(8), SpriteEffects.None, 0);

                else if (shots[i].X == (int)Weapon.DeathMachine)
                    sB.Draw(shotsImg, new Rectangle((int)(shots[i].Y - drawPos.X), (int)(shots[i].Z - drawPos.Y), 32, 16),
                        new Rectangle(56, 0, 32, 16), Color.White, shots[i].W, new Vector2(8), SpriteEffects.None, 0);


                else if (shots[i].X == (int)Weapon.DupeCell)
                    sB.Draw(shotsImg, new Rectangle((int)(shots[i].Y - drawPos.X), (int)(shots[i].Z - drawPos.Y), 16, 16),
                        new Rectangle(104, 0, 16, 16), Color.White, shots[i].W, new Vector2(8), SpriteEffects.None, 0);

                else if (shots[i].X == (int)Weapon.Tuberculosis)
                    sB.Draw(shotsImg, new Rectangle((int)(shots[i].Y - drawPos.X), (int)(shots[i].Z - drawPos.Y), 44, 10),
                        new Rectangle(166, 0, 44, 10), Color.White, shots[i].W, new Vector2(22, 5), gameTime.TotalGameTime.TotalMilliseconds % 100 < 20 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            }

            for (int i = 0; i < map.mucus.Count; i++)
            {
                int siz = map.mucus[i].size;
                long timeOffset = System.DateTime.UtcNow.Ticks - map.mucus[i].creationTime;
                if (timeOffset > 10000000)
                    siz--;
                if (siz < 1)
                {
                    map.mucus.RemoveAt(i--);
                    continue;
                }

                if (map.mucus[i].vX != 0 || map.mucus[i].vY != 0)
                {
                    float xDec = 0.5f * (map.mucus[i].vX > 0 ? 1 : -1);
                    float yDec = 0.5f * (map.mucus[i].vY > 0 ? 1 : -1);
                    float xV = map.mucus[i].vX - xDec; if (Math.Abs(xV) <= 0.5f) xV = 0;
                    float yV = map.mucus[i].vY - yDec; if (Math.Abs(yV) <= 0.5f) yV = 0;
                    map.mucus[i] = new Entities.Powerups.Weapons.Mucus(map.mucus[i].x + map.mucus[i].vX, map.mucus[i].y + map.mucus[i].vY, xV, yV, siz, map.mucus[i].creationTime);
                }
                else if (siz < 4 && timeOffset < 10000000)
                {
                    for (int j = i + 1; j < map.mucus.Count; j++)
                    {
                        int syz = (int)Math.Pow(2, 1 + siz);
                        if (siz == map.mucus[j].size && Math.Abs(map.mucus[i].x - map.mucus[j].x) < syz && Math.Abs(map.mucus[i].y - map.mucus[j].y) < syz)
                        {
                            map.mucus.RemoveAt(j);
                            map.mucus[i] = new Entities.Powerups.Weapons.Mucus(map.mucus[i].x, map.mucus[i].y, map.mucus[i].vX, map.mucus[i].vY, siz + 1, System.DateTime.UtcNow.Ticks);
                            break;
                        }
                    }
                }

                int sz = (int)Math.Pow(2, 2 + map.mucus[i].size);
                int szo2 = sz >> 1;
                if (!screenRect.Intersects(new Rectangle((int)map.mucus[i].x - szo2, (int)map.mucus[i].y - szo2, sz, sz)))
                    continue;

                sB.Draw(mucusImg, new Vector2(map.mucus[i].x - (szo2), map.mucus[i].y - (szo2)) - drawPos, new Rectangle(sz - 8, 0, sz, sz), Color.White);
            }
            #endregion

            #region HUD
#if XBOX
            {
                //draw critical hit text
                if (criticalHit.Z > 0)
                {
                    if (criticalHit.Z < 9)
                        sB.Draw(criticalHitGlow, new Vector2(criticalHit.X - 34, criticalHit.Y - 5) - drawPos, new Rectangle(0, 0, 232, 50), Color.White);
                    if (criticalHit.Z < 10 && criticalHit.Z > 1)
                        sB.Draw(criticalHitGlow, new Vector2(criticalHit.X - 24, criticalHit.Y - 5) - drawPos, new Rectangle(233, 0, 189, 50), Color.White);
                    if (criticalHit.Z > 2)
                        sB.Draw(criticalHitGlow, new Vector2(criticalHit.X - 22, criticalHit.Y - 5) - drawPos, new Rectangle(422, 0, 207, 50), Color.White);

                    if (criticalHit.Z > 3)
                        sB.Draw(criticalHitLogo, new Vector2(criticalHit.X, criticalHit.Y) - drawPos, Color.White);
                }

                //player health bar
                int hpPercent = (int)(((float)player.currentHealth / (float)player.maxHealth) * 239);
                sB.Draw(playerHealthBar, new Vector2(62, 290 - hpPercent), new Rectangle(62, 239 - hpPercent, 55, hpPercent), Color.White);
                sB.Draw(playerHealthBar, new Vector2(60, 50), new Rectangle(0, 0, 62, 242), Color.White);

                //player weapon bar
                Color wpCol = (player.currentWeapon == Weapon.Histamine ? new Color(1, 1, 1, 0.2f) : Color.White);
                int wpPercent = (int)(((player.currentWeapon == Weapon.Histamine ? 0 : player.currentAmmo) / (float)player.maxAmmo) * 130);
                sB.Draw(playerWeaponBar, new Vector2(124, 182 - wpPercent), new Rectangle(36, 130 - wpPercent, 31, wpPercent), bottomlessClip ? Color.Cyan : wpCol);
                sB.Draw(playerWeaponBar, new Vector2(122, 50), new Rectangle(0, 0, 36, 132), wpCol);

            #region Mucus Meter
                Color mucColor = mucusMode ? Color.Cyan : Color.White;
                if (mucusMeterAnim > 0)
                {
                    if (mucusMeterAnim > 60)
                        mucusMeterAnim = 0;
                    else
                        mucusMeterAnim += 4;

                    sB.Draw(playerMucusBar, new Vector2(160, -12 + mucusMeterAnim), new Rectangle(0, 0, 34, 62), mucColor);
                }
                else if (mucusMeterAnim < 0)
                {
                    if (mucusMeterAnim < -16)
                        sB.Draw(playerMucusBar, new Vector2(168, 48 + mucusMeterAnim), new Rectangle(38, 0, 22, 15), mucColor);
                    int n = mucusMeterAnim * mucusMeterAnim;
                    sB.Draw(playerMucusBar, new Vector2(166, 68 + n), new Rectangle(37, 25, 23, 37), mucColor);

                    mucusMeterAnim--;
                    if (n > screenRect.Height)
                        mucusMeterAnim = 0;
                }
                else if (player.currentMucus > 0)
                {
                    int mPercent = (int)(((float)player.currentMucus / 1000) * 100);
                    sB.Draw(playerMucusBar, new Vector2(160, 50), new Rectangle(0, 0, 34, 23), mucColor);
                    sB.Draw(playerMucusBar, new Rectangle(160, 73, 34, mPercent), new Rectangle(0, 24, 34, 4), mucColor);
                    sB.Draw(playerMucusBar, new Vector2(160, 73 + mPercent), new Rectangle(0, 27, 34, 35), mucColor);
                }
            #endregion

                //score
                string playerScoreTxt = visiblePlayerScore.ToString().PadLeft(8, '0');
                for (int i = 0; i < playerScoreTxt.Length; i++)
                {
                    int n = playerScoreTxt[i] - 48; //ascii '0' starts at chr 48
                    sB.Draw(numbersSprite, new Vector2(200 + (i * 50), n == 4 ? 56 : 58), new Rectangle(102, n * 50, 51, 50), Color.White);
                }
                //player score animations
                if (visiblePlayerScore < playerScore)
                    visiblePlayerScore += 10;
                if (visiblePlayerScore > playerScore)
                    visiblePlayerScore = playerScore;

                //lives
                for (int i = player.currentLives - 1; i >= 0; i--)
                    sB.Draw(livesIcon, new Vector2(66, 300 + (i * 20)), Color.White);

            #region Combos
                if (comboCount > 0) //only draw when theres a combo
                {
                    string comboTxt = "x" + comboCount.ToString();
                    for (int i = 0; i < Math.Min(comboCount / 10, 5); i++)
                        comboTxt += "!"; //add exclamations

                    int color = comboCount % 3;
                    Rectangle size = Rectangle.Empty; //size for animations
                    if (comboTextAnimation > 0)
                    {
                        size.Width = 10 - comboTextAnimation * 2;
                        size.Height = 10 - comboTextAnimation * 2;

                        if (comboTextAnimation++ > 4)
                            comboTextAnimation = 0; //reset
                    }
                    size.X = size.Width >> 1; size.Y = size.Height >> 1;

                    for (int i = 0, cXPos = 0; i < comboTxt.Length; i++)
                    {
                        int n = comboTxt[i] - 48;

                        if (comboTxt[i] == 'x')
                        {
                            sB.Draw(numbersSprite, new Rectangle(200 + cXPos - size.X, 106 - size.Y, 51 + size.Width, 50 + size.Width), new Rectangle(color * 51, 550, 51, 50), Color.White);
                            cXPos += 42;
                        }
                        else if (comboTxt[i] == '!')
                        {
                            sB.Draw(numbersSprite, new Rectangle(200 + cXPos - size.X - 8, 108 - size.Y, 51 + size.Width, 50 + size.Width), new Rectangle(color * 51, 500, 51, 50), Color.White);
                            cXPos += 30;
                        }
                        else if (n >= 0 && n < 10)
                        {
                            sB.Draw(numbersSprite, new Rectangle(200 + cXPos - size.X, (n == 4 ? 108 : 110) - size.Y, 51 + size.Width, 50 + size.Width), new Rectangle(color * 51, n * 50, 51, 50), Color.White);
                            if (n == 1) cXPos += 38;
                            else if (n == 4) cXPos += 52;
                            else cXPos += 48;
                        }
                    }
                }
            #endregion

                //draw boss health bar
                if (currentPlayMode == PlayMode.BossMode)
                {
                    int dX = parent.GraphicsDevice.Viewport.Width - 270, dY = parent.GraphicsDevice.Viewport.Height - 256;

                    int animPos = 0;
                    if (bossHealthAnimation > 0)
                        animPos = 256 - bossHealthAnimation * 4;
                    else if (bossHealthAnimation < 0)
                        animPos = -bossHealthAnimation * 4;

                    sB.Draw(bossHealthBar, new Vector2(dX, dY + animPos), new Rectangle(0, 0, 205, 256), Color.White);
                    if (currentBoss != null)
                    {
                        hpPercent = (int)(((float)currentBoss.currentHealth / (float)currentBoss.maxHealth) * 136);

                        sB.Draw(bossHealthBar, new Vector2(dX + 53, dY + 91 + 136 - hpPercent + animPos), new Rectangle(205, 137 - hpPercent, 99, hpPercent), new Color(1, 1, 1, 0.8f));
                    }
                }
            }
#else
            {
                //draw critical hit text
                if (criticalHit.Z > 0)
                {
                    if (criticalHit.Z < 9)
                        sB.Draw(criticalHitGlow, new Vector2(criticalHit.X - 34, criticalHit.Y - 5) - drawPos, new Rectangle(0, 0, 232, 50), Color.White);
                    if (criticalHit.Z < 10 && criticalHit.Z > 1)
                        sB.Draw(criticalHitGlow, new Vector2(criticalHit.X - 24, criticalHit.Y - 5) - drawPos, new Rectangle(233, 0, 189, 50), Color.White);
                    if (criticalHit.Z > 2)
                        sB.Draw(criticalHitGlow, new Vector2(criticalHit.X - 22, criticalHit.Y - 5) - drawPos, new Rectangle(422, 0, 207, 50), Color.White);

                    if (criticalHit.Z > 3)
                        sB.Draw(criticalHitLogo, new Vector2(criticalHit.X, criticalHit.Y) - drawPos, Color.White);
                }

                //player health bar
                int hpPercent = (int)(((float)player.currentHealth / (float)player.maxHealth) * 239);
                sB.Draw(playerHealthBar, new Vector2(12, 240 - hpPercent), new Rectangle(62, 239 - hpPercent, 55, hpPercent), Color.White);
                sB.Draw(playerHealthBar, new Vector2(10, 0), new Rectangle(0, 0, 62, 242), Color.White);

                //player weapon bar
                Color wpCol = (player.currentWeapon == Weapon.Histamine ? new Color(1, 1, 1, 0.2f) : Color.White);
                int wpPercent = (int)(((player.currentWeapon == Weapon.Histamine ? 0 : player.currentAmmo) / (float)player.maxAmmo) * 130);
                sB.Draw(playerWeaponBar, new Vector2(74, 132 - wpPercent), new Rectangle(36, 130 - wpPercent, 31, wpPercent), bottomlessClip ? Color.Cyan : wpCol);
                sB.Draw(playerWeaponBar, new Vector2(72, 0), new Rectangle(0, 0, 36, 132), wpCol);

                #region Mucus Meter
                Color mucColor = mucusMode ? Color.Cyan : Color.White;
                if (mucusMeterAnim > 0)
                {
                    if (mucusMeterAnim > 60)
                        mucusMeterAnim = 0;
                    else
                        mucusMeterAnim += 4;

                    sB.Draw(playerMucusBar, new Vector2(104, -64 + mucusMeterAnim), new Rectangle(0, 0, 34, 62), mucColor);
                }
                else if (mucusMeterAnim < 0)
                {
                    if (mucusMeterAnim < -16)
                        sB.Draw(playerMucusBar, new Vector2(110, -2 + mucusMeterAnim), new Rectangle(38, 0, 22, 15), mucColor);
                    int n = mucusMeterAnim * mucusMeterAnim;
                    sB.Draw(playerMucusBar, new Vector2(110, 16 + n), new Rectangle(37, 25, 23, 37), mucColor);

                    mucusMeterAnim--;
                    if (n > screenRect.Height)
                        mucusMeterAnim = 0;
                }
                else if (player.currentMucus > 0)
                {
                    int mPercent = (int)(((float)player.currentMucus / 1000) * 100);
                    sB.Draw(playerMucusBar, new Vector2(104, -2), new Rectangle(0, 0, 34, 23), mucColor);
                    sB.Draw(playerMucusBar, new Rectangle(104, 21, 34, mPercent), new Rectangle(0, 24, 34, 4), mucColor);
                    sB.Draw(playerMucusBar, new Vector2(104, 21 + mPercent), new Rectangle(0, 27, 34, 35), mucColor);
                }
                #endregion

                //draw joystick if in joystick mode
                if (OptionsScreen.useJoyNotAccel)
                {
                    //left joystick
                    sB.Draw(vJoystickIcon, new Vector2(48, screenRect.Height - 144), new Rectangle(0, 0, 96, 96), new Color(1, 1, 1, 0.5f));
                    sB.Draw(vJoystickIcon, new Vector2(61, screenRect.Height - 131) + vJoyLeft * 24, new Rectangle(96, 26, 70, 70), Color.White);

                    //right
                    if (!OptionsScreen.useJoyAndTouch)
                    {
                        sB.Draw(vJoystickIcon, new Vector2(screenRect.Width - 144, screenRect.Height - 144), new Rectangle(0, 0, 96, 96), new Color(1, 1, 1, 0.5f));
                        sB.Draw(vJoystickIcon, new Vector2(screenRect.Width - 131, screenRect.Height - 131) + vJoyRight * 24, new Rectangle(96, 26, 70, 70), Color.White);
                    }
                }

                //score
                string playerScoreTxt = visiblePlayerScore.ToString().PadLeft(8, '0');
                for (int i = 0; i < playerScoreTxt.Length; i++)
                {
                    int n = playerScoreTxt[i] - 48; //ascii '0' starts at chr 48
                    sB.Draw(numbersSprite, new Vector2(135 + (i * 50), n == 4 ? 6 : 8), new Rectangle(102, n * 50, 51, 50), Color.White);
                }
                //player score animations
                if (visiblePlayerScore < playerScore)
                    visiblePlayerScore += 10;
                if (visiblePlayerScore > playerScore)
                    visiblePlayerScore = playerScore;

                //lives
                for (int i = player.currentLives - 1; i >= 0; i--)
                    sB.Draw(livesIcon, new Vector2(14, 250 + (i * 20)), Color.White);

                //pause
                sB.Draw(pauseButton, new Vector2(screenRect.Width - 64, 0), Color.White);

                #region Combos
                if (comboCount > 0) //only draw when theres a combo
                {
                    string comboTxt = "x" + comboCount.ToString();
                    for (int i = 0; i < Math.Min(comboCount / 10, 5); i++)
                        comboTxt += "!"; //add exclamations

                    int color = comboCount % 3;
                    Rectangle size = Rectangle.Empty; //size for animations
                    if (comboTextAnimation > 0)
                    {
                        size.Width = 10 - comboTextAnimation * 2;
                        size.Height = 10 - comboTextAnimation * 2;

                        if (comboTextAnimation++ > 4)
                            comboTextAnimation = 0; //reset
                    }
                    size.X = size.Width >> 1; size.Y = size.Height >> 1;

                    for (int i = 0, cXPos = 0; i < comboTxt.Length; i++)
                    {
                        int n = comboTxt[i] - 48;

                        if (comboTxt[i] == 'x')
                        {
                            sB.Draw(numbersSprite, new Rectangle(135 + cXPos - size.X, 56 - size.Y, 51 + size.Width, 50 + size.Width), new Rectangle(color * 51, 550, 51, 50), Color.White);
                            cXPos += 42;
                        }
                        else if (comboTxt[i] == '!')
                        {
                            sB.Draw(numbersSprite, new Rectangle(135 + cXPos - size.X - 8, 58 - size.Y, 51 + size.Width, 50 + size.Width), new Rectangle(color * 51, 500, 51, 50), Color.White);
                            cXPos += 30;
                        }
                        else if (n >= 0 && n < 10)
                        {
                            sB.Draw(numbersSprite, new Rectangle(135 + cXPos - size.X, (n == 4 ? 58 : 60) - size.Y, 51 + size.Width, 50 + size.Width), new Rectangle(color * 51, n * 50, 51, 50), Color.White);
                            if (n == 1) cXPos += 38;
                            else if (n == 4) cXPos += 52;
                            else cXPos += 48;
                        }
                    }
                }
                #endregion

                //draw boss health bar
                if (currentPlayMode == PlayMode.BossMode)
                {
                    int dX = parent.GraphicsDevice.Viewport.Width - 220, dY = parent.GraphicsDevice.Viewport.Height - 256;

                    int animPos = 0;
                    if (bossHealthAnimation > 0)
                        animPos = 256 - bossHealthAnimation * 4;
                    else if (bossHealthAnimation < 0)
                        animPos = -bossHealthAnimation * 4;

                    sB.Draw(bossHealthBar, new Vector2(dX, dY + animPos), new Rectangle(0, 0, 205, 256), Color.White);
                    if (currentBoss != null)
                    {
                        hpPercent = (int)(((float)currentBoss.currentHealth / (float)currentBoss.maxHealth) * 136);

                        sB.Draw(bossHealthBar, new Vector2(dX + 53, dY + 91 + 136 - hpPercent + animPos), new Rectangle(205, 137 - hpPercent, 99, hpPercent), new Color(1, 1, 1, 0.8f));
                    }
                }
            }
#endif
            #endregion

            #region Help Text
            if (helpCreationTime > 0)
            {
                if (DateTime.UtcNow.Ticks - helpCreationTime > 10000000 * helpShowTime)
                    helpCreationTime = 0;

                sB.DrawString(parent.Font, helpTxt, helpTxtPos + new Vector2(2), Color.Black);
                sB.DrawString(parent.Font, helpTxt, helpTxtPos, Color.White);
            }
            #endregion

            #region Debug
#if DEBUG && !WINDOWS
            string fpsTxt = "FPS " + Convert.ToString((int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds));
            sB.DrawString(parent.Font, fpsTxt, new Vector2(10, parent.GraphicsDevice.Viewport.Height - 34), Color.Black);
            sB.DrawString(parent.Font, fpsTxt, new Vector2(8, parent.GraphicsDevice.Viewport.Height - 36), Color.White);
#endif
            if (showCollisLines)
            {
                int x = (int)player.position.X / map.tileWidth;
                int y = (int)player.position.Y / map.tileHeight;

                Vector2 dPos = new Vector2(x * map.tileWidth, y * map.tileHeight) - drawPos;
                Liner.DrawLine(ref sB, Color.White, new Vector2(x, y) + dPos, new Vector2(x + map.tileWidth, y) + dPos);
                Liner.DrawLine(ref sB, Color.White, new Vector2(x + map.tileWidth, y) + dPos, new Vector2(x + map.tileWidth, y + map.tileHeight) + dPos);
                Liner.DrawLine(ref sB, Color.White, new Vector2(x + map.tileWidth, y + map.tileHeight) + dPos, new Vector2(x, y + map.tileHeight) + dPos);
                Liner.DrawLine(ref sB, Color.White, new Vector2(x, y + map.tileHeight) + dPos, new Vector2(x, y) + dPos);

                int t = map.tiles[y, x] - 1;
                Vector2 pos = new Vector2(10, screenRect.Height - map.tileHeight - 10);
                sB.Draw(map.tileset, pos, new Rectangle((t % (map.tileset.Width / map.tileWidth)) * map.tileWidth,
                    (t / (map.tileset.Height / map.tileHeight)) * map.tileHeight, map.tileWidth, map.tileHeight), Color.White);

                pos.Y -= 18;
                sB.DrawString(parent.Font, new Point((int)player.position.X % map.tileWidth, (int)player.position.Y % map.tileHeight).ToString() + "\n" + player.position, pos, Color.White);
            }
            #endregion

            sB.End();

            #region Particles
            explosionParticles.Draw(gameTime, parent.GraphicsDevice, drawPos);
            weaponParticles.Draw(gameTime, parent.GraphicsDevice, drawPos);
            decayParticles.Draw(gameTime, parent.GraphicsDevice, drawPos);
            #endregion
        }

        #endregion


        #region Other

        /// <summary>
        /// Go to the pause screen
        /// </summary>
        public void Pause()
        {
            Main.isMusicFading = true;
            if (pauseScreen == null)
            {
                //load the pause screen
                pauseScreen = new PauseScreen();
                parent.AddScreen(pauseScreen, new System.Collections.Generic.List<object> { this }, null);
            }
            pauseScreen.screenState = ScreenState.Active;
        }

#if WINDOWS_PHONE

        System.IO.IsolatedStorage.IsolatedStorageFile tombStoneData = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();

        void Current_Deactivated(object sender, Microsoft.Phone.Shell.DeactivatedEventArgs e)
        {
            OptionsScreen.SaveSettings();

            System.IO.StreamWriter store = new System.IO.StreamWriter(new System.IO.IsolatedStorage.IsolatedStorageFileStream("tombstone.level", System.IO.FileMode.Create, tombStoneData));
            store.WriteLine(level + " " + defaultScore + " " + defaultKillCount);
            store.Write(defaultLives + " " + defaultHealth + " " + defaultAmmo + " " + defaultMaxAmmo + " " + (int)defaultWeapon + " " + defaultMucus);

            store.Close();
        }
#endif

        #endregion
    }
}
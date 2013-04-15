//Main.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#if WINDOWS_PHONE
using Microsoft.Advertising.Mobile.Xna;
#endif

//empty class to test on pc with xbox settings
#if XBOX && WINDOWS
namespace Microsoft.Xna.Framework.GamerServices
{
    class Guide { public static bool IsTrialMode = false; public static void ShowMarketplace(PlayerIndex p) { } }
}
#endif

namespace YoureAllDiseased
{
    #region startup (Windows/Xbox/Zune)
#if !WINDOWS_PHONE
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Main game = new Main())
            {
                game.Run();
            }
        }
    }
#endif
    #endregion

    /// <summary>
    /// Main Entry for game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public StateManager gSM;

#if WINDOWS_PHONE
        public DrawableAd gameAd480;
        public DrawableAd gameAd300;
#endif

        /// <summary>
        /// maximum number of highscores to be saved
        /// </summary>
        public static int maxHighScores = 20;

        /// <summary>
        /// The main play screen (public and static for referencing)
        /// </summary>
        public static PlayScreen pScreen;
        /// <summary>
        /// The main calibration screen (used on phone and zune)
        /// </summary>
        public static CalibrationScreen calibScreen;

#if XNA31
        /// <summary>
        /// the main storage device for storing on xna 3.1 platforms (like the Zune)
        /// </summary>
        public static Microsoft.Xna.Framework.Storage.StorageDevice sd;
#elif WINDOWS_PHONE || XBOX
        public static System.IO.IsolatedStorage.IsolatedStorageFile isoStore;
#endif

        /// <summary>
        /// A custom mouse pointer
        /// </summary>
        Texture2D pointer;

        #region public transitions

        public FadeInTransition fadeInTransition = null;
        public FadeOutTransition fadeOutTransition = null;
        public GameOverTransition gameOverTransition = null;
        public LvlCompleteTransition lvlCompleteTransition = null;

        #endregion

        /// <summary>
        /// The main program
        /// </summary>
        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30f); //30 fps

            #region Create HighScores

            //Create high scores file if it doesnt exist
#if !(WINDOWS_PHONE || XBOX)
            string path = "top";
#if ZUNE || XNA31
            //create storage device
            IAsyncResult r = Microsoft.Xna.Framework.GamerServices.Guide.BeginShowStorageDeviceSelector(null, null);
            while (!r.IsCompleted) ;
            sd = Microsoft.Xna.Framework.GamerServices.Guide.EndShowStorageDeviceSelector(r);

            //create top file if doesn't exist
            using (Microsoft.Xna.Framework.Storage.StorageContainer ctn = sd.OpenContainer("You're All Diseased!"))
            {
                path = ctn.Path + "/top";
                if (!System.IO.File.Exists(path))
#elif WINDOWS
            {
#endif
                if (!System.IO.File.Exists(path))
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
                    for (int i = 0; i < maxHighScores; i++)
                        sw.WriteLine("HEX=0");
                    sw.Close();
                }

            }
#else
#if !WINDOWS
            isoStore = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
            if (!isoStore.FileExists("top"))
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(new System.IO.IsolatedStorage.IsolatedStorageFileStream("top", System.IO.FileMode.CreateNew, isoStore));
                for (int i = 0; i < maxHighScores; i++)
                    sw.WriteLine("HEX=0");
                sw.Close();
            }
#endif
#endif
            #endregion


#if ZUNE
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 272;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000/30); //30 fps
#elif WINDOWS_PHONE
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;

            graphics.IsFullScreen = true; //make sure it's fullscreen (so no scaling)
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
            graphics.ApplyChanges();
#elif WINDOWS
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000/60f); //120 fps
            
#if DEBUG
            IsFixedTimeStep = true;
#endif

            graphics.PreferredBackBufferWidth = 800;
#if XBOX && WINDOWS
            graphics.PreferredBackBufferHeight = 600;
#else
            graphics.PreferredBackBufferHeight = 480;
#endif
            graphics.IsFullScreen = false;
            Window.AllowUserResizing = true;
            IsMouseVisible = false; //use custom mouse pointer
#elif XBOX
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000/60); //60 fps
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

#endif

#if XBOX
            Components.Add(new Microsoft.Xna.Framework.GamerServices.GamerServicesComponent(this));
#if DEMO
            Microsoft.Xna.Framework.GamerServices.Guide.SimulateTrialMode = true;
#endif
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Liner.Init(GraphicsDevice); //initialize line drawing

            gSM = new StateManager(this);
            Components.Add(gSM);

#if WINDOWS_PHONE
            AdGameComponent.Initialize(this, "25b23b88-d872-40ab-9e15-8afb01b64c3d");
            //AdGameComponent.Initialize(this, "test_client");
            Components.Add(AdGameComponent.Current);

            gameAd300 = Microsoft.Advertising.Mobile.Xna.AdGameComponent.Current.CreateAd("76716", new Rectangle(250, 410, 300, 70), true);
            gameAd300.DropShadowEnabled = false;
            gameAd300.Visible = false;

            gameAd480 = Microsoft.Advertising.Mobile.Xna.AdGameComponent.Current.CreateAd("76717", new Rectangle(0, 400, 480, 80), true);
            gameAd480.DropShadowEnabled = false;
            gameAd480.Keywords += "game";
            gameAd480.Visible = false;

#endif

            base.Initialize();
        }

        /// <summary>
        /// An event called when the game is exiting
        /// </summary>
        /// <param name="sender">the class calling this</param>
        /// <param name="args">any arguments passed</param>
        protected override void OnExiting(object sender, EventArgs args)
        {
            gSM.isExiting = true;
#if WINDOWS || XBOX
            Microsoft.Xna.Framework.Input.GamePad.SetVibration(gSM.InputState.activePlayerIndex, 0, 0);
#endif
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pointer = Content.Load<Texture2D>("Graphics/Pointer");

            gSM.AddScreen((calibScreen = new CalibrationScreen()), null, null);
            calibScreen.screenState = ScreenState.Inactive;

#if PLAYTEST
            gSM.AddScreen((pScreen = new PlayScreen()), new System.Collections.Generic.List<object> { 0 }, null);
#elif DEBUG
            //gSM.AddScreen(new IntroScreen(), null, null);
            gSM.AddScreen((pScreen = new PlayScreen()), new System.Collections.Generic.List<object> { 1 }, null);
#else
#if WINDOWS_PHONE
            if (isoStore.FileExists("tombstone.level"))
                gSM.AddScreen(new PlayScreen(), null, null);
            else
#endif
                gSM.AddScreen(new IntroScreen(), null, null);
                //gSM.AddScreen((pScreen = new PlayScreen()), new System.Collections.Generic.List<object> { 4 }, null);
#endif
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (isMusicFading)
                FadeOutMusic();

            //toggle fullscreen on windows
            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F11))
                graphics.ToggleFullScreen();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

#if WINDOWS //only on windows
            if (gSM.InputState.touches.Count > 0)
            {
                spriteBatch.Begin(); //draw pointer
                spriteBatch.Draw(pointer, gSM.InputState.touches[0].position, Color.White);
                spriteBatch.End();
            }
#endif

#if WINDOWS && DEBUG //draw fps & accelerometer position on windows titlebar
            Window.Title = "You're All Diseased! - FPS: " + Convert.ToString((int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds)) + " - Accel: " + gSM.InputState.accelReading.ToString();
#endif
        }

        public static bool isMusicFading = false;

        /// <summary>
        /// Will fade music to out to pause if isMusicFading = true
        /// </summary>
        void FadeOutMusic()
        {
            if (Microsoft.Xna.Framework.Media.MediaPlayer.State == Microsoft.Xna.Framework.Media.MediaState.Playing && OptionsScreen.playMusic && OptionsScreen.canPlayAudio)
            {
                Microsoft.Xna.Framework.Media.MediaPlayer.Volume -= 0.05f;
                if (Microsoft.Xna.Framework.Media.MediaPlayer.Volume <= 0.1f)
                {
                    Microsoft.Xna.Framework.Media.MediaPlayer.Pause();
                    isMusicFading = false;
                    Microsoft.Xna.Framework.Media.MediaPlayer.Volume = 1;
                }
            }
            else
                isMusicFading = false;
        }

        /// <summary>
        /// Play a song (only if OptionsScreen.playMusic = true)
        /// </summary>
        /// <param name="song">The song to play</param>
        public static void PlaySong(Microsoft.Xna.Framework.Media.Song song, bool loop)
        {
            if (!OptionsScreen.playMusic || !OptionsScreen.canPlayAudio)
                return;

            isMusicFading = false;
            Microsoft.Xna.Framework.Media.MediaPlayer.Volume = 1;
            Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = loop;

            System.Threading.ThreadStart thS = delegate { Microsoft.Xna.Framework.Media.MediaPlayer.Play(song); };
            System.Threading.Thread th = new System.Threading.Thread(thS);
            th.Start();
        }

        /// <summary>
        /// Save a single high scores entry (will auto adjust list), default max 50 entries
        /// </summary>
        /// <param name="name">the name to associate with this score</param>
        /// <param name="score">The score to save</param>
        /// <returns>the position on the list, 0 -> maxHighScores-1, -1 on error or if not on list</returns>
        public static int SaveHighScore(string name, int score)
        {
            try
            {
                int position = -1;
                string path = "top";
#if XNA31
            using (Microsoft.Xna.Framework.Storage.StorageContainer ctn = sd.OpenContainer("You're All Diseased!"))
            {
                path = ctn.Path + "/top";
                System.IO.StreamReader topper = new System.IO.StreamReader(path);
#elif WINDOWS_PHONE || XBOX
                {
                    System.IO.StreamReader topper = new System.IO.StreamReader(new System.IO.IsolatedStorage.IsolatedStorageFileStream("top", System.IO.FileMode.Open, isoStore));
#else
                {
                    System.IO.StreamReader topper = new System.IO.StreamReader(path);
#endif
                    System.Collections.Generic.List<string> names = new System.Collections.Generic.List<string>(maxHighScores);
                    System.Collections.Generic.List<int> scores = new System.Collections.Generic.List<int>(maxHighScores);

                    //read all of the existing scores into a buffer
                    while (!topper.EndOfStream)
                    {
                        string[] s = topper.ReadLine().Split('=');
                        names.Add(s[0]);
                        scores.Add(int.Parse(s[1]));
                    }

                    //if there are no names in the file or the list is less than the maxHighScores count, just write to the list
                    if (names.Count < 1)
                    {
                        names.Add(name);
                        scores.Add(score);
                        position = names.Count;
                    }
                    else
                    {
                        bool added = false;
                        //find who the user is better than
                        for (int i = 0; i < maxHighScores; i++)
                        {
                            if (score > scores[i])
                            {
                                names.Insert(i, name);
                                scores.Insert(i, score);
                                position = i;
                                added = true;
                                break;
                            }
                        }
                        if (!added)
                        {
                            names.Add(name);
                            scores.Add(score);
                            position = names.Count - 1;
                        }
                    }
                    topper.Close();

#if WINDOWS_PHONE || XBOX

                    System.IO.StreamWriter writer = new System.IO.StreamWriter(new System.IO.IsolatedStorage.IsolatedStorageFileStream("top", System.IO.FileMode.OpenOrCreate, isoStore));
#else
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(path);
#endif
                    for (int i = 0; i < (names.Count < maxHighScores ? names.Count : maxHighScores); i++)
                        writer.WriteLine(names[i] + "=" + scores[i]);
                    writer.Close();
                }

                return position;
            }
            catch (Exception expt) //any error, just return -1
            {
                Console.WriteLine(expt.Message);
                return -1;
            }
        }

        /// <summary>
        /// Clear the highscores table
        /// </summary>
        public static void ClearHighscores()
        {
#if !(WINDOWS_PHONE || XBOX)
            string path = "top";
#if ZUNE || XNA31
            //create storage device
            IAsyncResult r = Microsoft.Xna.Framework.GamerServices.Guide.BeginShowStorageDeviceSelector(null, null);
            while (!r.IsCompleted) ;
            sd = Microsoft.Xna.Framework.GamerServices.Guide.EndShowStorageDeviceSelector(r);

            //create top file if doesn't exist
            using (Microsoft.Xna.Framework.Storage.StorageContainer ctn = sd.OpenContainer("You're All Diseased!"))
            {
                path = ctn.Path + "/top";
#elif WINDOWS
            {
#endif
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path, false);
                for (int i = 0; i < maxHighScores; i++)
                    sw.WriteLine("HEX=0");
                sw.Close();
            }
#else
            isoStore = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(new System.IO.IsolatedStorage.IsolatedStorageFileStream("top", System.IO.FileMode.Create, isoStore));
                for (int i = 0; i < maxHighScores; i++)
                    sw.WriteLine("HEX=0");
                sw.Close();
            }
#endif
        }
    }
}
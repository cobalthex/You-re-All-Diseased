//OptionsScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace YoureAllDiseased
{
    /// <summary>
    /// Options for the game
    /// </summary>
    public class OptionsScreen : GameScreen
    {
        #region Options

        /// <summary>
        /// Is this the first time playing the game
        /// </summary>
        public static bool firstRun = true;

        /// <summary>
        /// play audio in the game?
        /// </summary>
        public static bool playMusic = true;

        /// <summary>
        /// play game sounds
        /// </summary>
        public static bool playSounds = true;

        /// <summary>
        /// use virtual joystick to move, touch to shoot
        /// </summary>
        public static bool useJoyAndTouch = false;
        /// <summary>
        /// use joystick over accelerometer
        /// </summary>
        public static bool useJoyNotAccel = false;

        /// <summary>
        /// Show hints? (only on first play or when highscores are reset)
        /// </summary>
        public static bool showHints = true;

        /// <summary>
        /// Did the user let the game play audio (music & sounds - selected at startup)
        /// </summary>
        public static bool canPlayAudio = true;

        /// <summary>
        /// Difficulty
        /// 0 being easy
        /// 1 being medium
        /// 2 being hard 
        /// 3+ being insane (not shown)
        /// </summary>
        public static int Difficulty = 1;

        #endregion


        #region Data
        SpriteFont font;

        /// <summary>
        /// A simple tick to play when moving through items
        /// </summary>
        Microsoft.Xna.Framework.Audio.SoundEffect tick;
        Microsoft.Xna.Framework.Audio.SoundEffect audioEnabled;

        //used for glowing text on selection
        long glowStart = 0; //when the glow started (0 for not active)
        short whichItem = 0; //which item to glow

        int w, h; //width and height of screen

        System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>() {
            (playMusic && canPlayAudio ? "" : "not ") + "playing music",
            (playSounds && canPlayAudio ? "" : "not ") + "playing sounds",
#if ZUNE || WINDOWS_PHONE || (DEBUG && WINDOWS)
            "using " + (useJoyNotAccel ? useJoyAndTouch ? "touch+joystick" : "joysticks" : "accelerometer"),
#endif
            "reset highscores",
            "back"
        };

        Microsoft.Xna.Framework.Input.KeyboardState kb;
        Microsoft.Xna.Framework.Input.KeyboardState pkb;

        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
#if WINDOWS_PHONE
            ((Main)parent.Game).gameAd300.Visible = false;
            ((Main)parent.Game).gameAd480.Visible = false;
#endif

            font = content.Load<SpriteFont>("Fonts/Menu");

            tick = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>("Audio/Sounds/Tick");
            audioEnabled = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>("Audio/Sounds/Audio");

            kb = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            pkb = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        }

        #endregion


        #region Update & Draw

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool isVisible, bool isCovered)
        {
#if XNA31 //force zune to these dimensions (renders to a render target which is then scaled to display)
            w = 800;
            h = 480;
#else
            w = parent.GraphicsDevice.Viewport.Width;
            h = parent.GraphicsDevice.Viewport.Height;
#endif
        }

        public override void HandleInput(GameTime gameTime, InputManager input)
        {
            int lastItem = whichItem;

            if (input.isBackButtonPressed)
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);

#if XBOX || WINDOWS
            pkb = kb;
            kb = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            if (kb.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.H) && pkb.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.H))
                parent.NextScreen(this, new HighScoresScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);

            //press enter or space to select option
            if ((kb.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter) && pkb.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Enter)) ||
                (kb.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space) && pkb.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Space)))
            {
                SelectItem(whichItem);
                glowStart = DateTime.UtcNow.Ticks;
            }
            else if (kb.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down) && pkb.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Down))
                whichItem = (short)((whichItem + 1) % options.Count);
            else if (kb.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up) && pkb.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Up))
                whichItem = (short)(whichItem - 1 < 0 ? (options.Count - 1) - whichItem : whichItem - 1);

            //press A to select option
            if ((input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Start) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.Start)) ||
                (input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.A) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.A)))
            {
                SelectItem(whichItem);
                glowStart = DateTime.UtcNow.Ticks;
            }
            //move down or up
            else if ((input.gpState.ThumbSticks.Left.Y < 0 && input.pGPState.ThumbSticks.Left.Y >= 0) ||
                (input.gpState.DPad.Down == Microsoft.Xna.Framework.Input.ButtonState.Pressed && input.pGPState.DPad.Down == Microsoft.Xna.Framework.Input.ButtonState.Released))
                whichItem = (short)((whichItem + 1) % options.Count);
            else if ((input.gpState.ThumbSticks.Left.Y > 0 && input.pGPState.ThumbSticks.Left.Y <= 0) ||
                (input.gpState.DPad.Up == Microsoft.Xna.Framework.Input.ButtonState.Pressed && input.pGPState.DPad.Up == Microsoft.Xna.Framework.Input.ButtonState.Released))
                whichItem = (short)(whichItem - 1 < 0 ? (options.Count - 1) - whichItem : whichItem - 1);
#endif
#if !XBOX
            bool contains = false;
            if (input.touches.Count > 0)
                contains = new Rectangle(w - 800, h - ((options.Count + 1) * 50), 800, options.Count * 50).Contains((int)input.touches[0].position.X, (int)input.touches[0].position.Y); //is the mouse in the buttons area
#if WINDOWS
            if (contains)
                whichItem = (short)(((int)input.touches[0].position.Y - (h - ((options.Count + 1) * 50))) / 50);
            if (input.touches.Count > 0 && input.touches[0].state == TouchState.Pressed && input.pTouches[0].state == TouchState.Released && contains)
#endif
            if (input.touches.Count > 0 && input.touches[0].state == TouchState.Pressed && contains)
            {
                whichItem = (short)(((int)input.touches[0].position.Y - (h - ((options.Count + 1) * 50))) / 50);
                SelectItem(whichItem);
                glowStart = DateTime.UtcNow.Ticks;
            }
#endif

            if (lastItem != whichItem && OptionsScreen.canPlayAudio && OptionsScreen.playSounds)
                tick.Play(0.2f, 0f, 0f);
        }

        /// <summary>
        /// Perform an action when an item is selected
        /// </summary>
        /// <param name="item"></param>
        void SelectItem(int item)
        {
            if (options[item].Contains("playing music"))
            {
                playMusic = !playMusic;
                if (!canPlayAudio)
                    playMusic = true;

                if (!playMusic)
                {
                    options[item] = "not playing music";
                    Main.isMusicFading = true;
                }
                else
                {
                    options[item] = "playing music";
                    if (OptionsScreen.canPlayAudio && OptionsScreen.playSounds)
                        audioEnabled.Play(0.4f, 0f, 0f);
                    canPlayAudio = true;
                }
            }
            else if (options[item].Contains("playing sounds"))
            {
                playSounds = !playSounds;
                if (!canPlayAudio)
                    playSounds = true;

                if (!playSounds)
                {
                    options[item] = "not playing sounds";
                }
                else
                {
                    options[item] = "playing sounds"; 
                    if (OptionsScreen.canPlayAudio && OptionsScreen.playSounds)
                        audioEnabled.Play(0.4f, 0f, 0f);
                    canPlayAudio = true;
                }
            }
            else if (options[item].Contains("using"))
            {
                if (useJoyNotAccel && !useJoyAndTouch)
                {
                    useJoyAndTouch = true;
                    options[item] = "using touch+joystick";
                }
                else if (useJoyNotAccel && useJoyAndTouch)
                {
                    useJoyAndTouch = false;
                    useJoyNotAccel = false;
                    options[item] = "using accelerometer";
                }
                else
                {
                    useJoyNotAccel = true;
                    useJoyAndTouch = false;
                    options[item] = "using joysticks";
                }
            }
            else if (options[item] == "clear highscores")
            {
                Main.ClearHighscores();
                showHints = true;
            }
            else
            {
                SaveSettings();
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
            }
        }

#if XBOX
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.DrawString(font, "Options", new Vector2(80), Color.White);

            int cGlowPos = 0; //current position in glow animation

            Color glowColor = Color.White;

#if XBOX || WINDOWS
            glowColor = new Color(164, 255, 128);
#endif

            if (glowStart != 0) //animation is running
            {
                cGlowPos = (int)(DateTime.UtcNow.Ticks - glowStart) / 100000; //calculate current time in animation (to 100th of sec)
                glowColor = new Color(128 + (128 * cGlowPos / 128), 255, (255 * cGlowPos / 128)); //set the glow color for that time 
            }
            if (cGlowPos > 128) //reset the animation
                glowStart = 0;

            //draw the button text
            for (int i = 0; i < options.Count; i++)
                spriteBatch.DrawString(font, options[i], new Vector2(w - 90 - font.MeasureString(options[i]).X, h - 30 - ((options.Count + 1) * 50) + (i * 50)), whichItem == i ? glowColor : Color.White);

            spriteBatch.End();
        }
#else
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.DrawString(font, "Options", new Vector2(50), Color.White);

            int cGlowPos = 0; //current position in glow animation

            Color glowColor = Color.White;

#if XBOX || WINDOWS
            glowColor = new Color(164, 255, 128);
#endif

            if (glowStart != 0) //animation is running
            {
                cGlowPos = (int)(DateTime.UtcNow.Ticks - glowStart) / 100000; //calculate current time in animation (to 100th of sec)
                glowColor = new Color(128 + (128 * cGlowPos / 128), 255, (255 * cGlowPos / 128)); //set the glow color for that time 
            }
            if (cGlowPos > 128) //reset the animation
                glowStart = 0;

            //draw the button text
            for (int i = 0; i < options.Count; i++)
                spriteBatch.DrawString(font, options[i], new Vector2(w - 60 - font.MeasureString(options[i]).X, h - ((options.Count + 1) * 50) + (i * 50)), whichItem == i ? glowColor : Color.White);

            spriteBatch.End();
        }
#endif

        #endregion


        #region Other

        /// <summary>
        /// Save the game settings to the settings file
        /// </summary>
        public static void SaveSettings()
        {
            System.IO.BinaryWriter bw = null;

            try
            {
#if WINDOWS_PHONE || XBOX

                bw = new System.IO.BinaryWriter(new System.IO.IsolatedStorage.IsolatedStorageFileStream("settings", System.IO.FileMode.OpenOrCreate, Main.isoStore));
#else
                bw = new System.IO.BinaryWriter(new System.IO.FileStream("settings", System.IO.FileMode.OpenOrCreate));
#endif
                bw.Write(playMusic);
                bw.Write(playSounds);
                bw.Write((byte)(useJoyAndTouch ? 2 : (useJoyNotAccel ? 1 : 0)));
                bw.Close();

                bw.Close();
            }
            catch
            {
                if (bw != null)
                    bw.Close();
            }
        }

        /// <summary>
        /// Load the game settings from the settings file
        /// </summary>
        public static void LoadSettings()
        {
            System.IO.BinaryReader br = null;

            try
            {
#if WINDOWS_PHONE || XBOX

                br = new System.IO.BinaryReader(new System.IO.IsolatedStorage.IsolatedStorageFileStream("settings", System.IO.FileMode.Open, Main.isoStore));
#else
                br = new System.IO.BinaryReader(new System.IO.FileStream("settings", System.IO.FileMode.Open));
#endif

                firstRun = false;
                showHints = false;

                playMusic = br.ReadBoolean();
                playSounds = br.ReadBoolean();
                byte b = br.ReadByte();
                useJoyAndTouch = false;
                if (b == 0)
                    useJoyNotAccel = false;
                else if (b == 2)
                    useJoyAndTouch = true;
                if (b > 0)
                    useJoyNotAccel = true;

                br.Close();
            }
            catch
            {
                if (br != null)
                    br.Close();

                firstRun = true;
                showHints = true;
                playMusic = false;
                useJoyAndTouch = false;
                useJoyNotAccel = false;
            }
        }

        #endregion
    }
}

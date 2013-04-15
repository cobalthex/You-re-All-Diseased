//MainMenuScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace YoureAllDiseased
{
    /// <summary>
    /// The game menu screen
    /// </summary>
    public class MainMenuScreen : GameScreen
    {
        #region Data

        Texture2D titleLogo;
        Texture2D banner; //build banner (preview, beta, etc.)
        Texture2D splatterImg;
        Texture2D introImg1; //the image containing parts of the intro animation
        Texture2D introImg2; //the image containing parts of the intro animation
        SpriteFont font; //font to be used for options

        /// <summary>
        /// A simple tick to play when moving through items
        /// </summary>
        Microsoft.Xna.Framework.Audio.SoundEffect tick;

        //used for glowing text on selection
        long glowStart = 0; //when the glow started (0 for not active)
        short whichItem = 0; //which item to glow

        int w, h; //width and height of screen

        System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>() {
            "Play",
            "Credits",
            "Options",
            "Exit",
        };

#if XBOX || WINDOWS_PHONE
        public bool isTrial = false;
#endif

        /// <summary>
        /// Play the intro, if the user touches anywhere, this changes to false
        /// </summary>
        static bool playIntro = true;

        Microsoft.Xna.Framework.Input.KeyboardState kb;
        Microsoft.Xna.Framework.Input.KeyboardState pkb;

        Rectangle screenRect;

        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
#if WINDOWS_PHONE
            ((Main)parent.Game).gameAd300.Visible = false;
            ((Main)parent.Game).gameAd480.Visible = true;
            ((Main)parent.Game).gameAd480.DisplayRectangle = new Rectangle(0, 400, 480, 80);
#endif

            if (DateTime.Now.Month == 12 && DateTime.Now.Day > 20 && DateTime.Now.Day < 27) //holiday logo
                titleLogo = content.Load<Texture2D>("Graphics/ChristmasLogo");
            else
                titleLogo = content.Load<Texture2D>("Graphics/Logo");

            introImg1 = content.Load<Texture2D>("Graphics/Blood1");
            introImg2 = content.Load<Texture2D>("Graphics/Blood2");

            tick = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>("Audio/Sounds/Tick");

            playIntro = OptionsScreen.firstRun;
            OptionsScreen.firstRun = false;
            OptionsScreen.SaveSettings();

            if (OptionsScreen.canPlayAudio && OptionsScreen.playMusic)
                Microsoft.Xna.Framework.Media.MediaPlayer.Stop();

#if XNA31 //force zune to these dimensions (renders to a render target which is then scaled to display)
            screenRect = new Rectangle(0, 0, 800, 480);
#else
            screenRect = new Rectangle(0, 0, parent.GraphicsDevice.Viewport.Width, parent.GraphicsDevice.Viewport.Height);
#endif


            banner = null;

#if XBOX || WINDOWS_PHONE
            isTrial = Microsoft.Xna.Framework.GamerServices.Guide.IsTrialMode;

            if (isTrial)
            {
                banner = content.Load<Texture2D>("Graphics/DemoBanner");
                options.Insert(1, "Buy");
            }
#endif

#if DEMO
            banner = content.Load<Texture2D>("Graphics/DemoBanner");
#elif PREVIEW
            banner = content.Load<Texture2D>("Graphics/PreviewBanner");
#elif BETA
            banner = content.Load<Texture2D>("Graphics/BetaBanner");
#endif
            splatterImg = content.Load<Texture2D>("Graphics/Splatter");
            font = content.Load<SpriteFont>("Fonts/Menu");


            kb = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            pkb = Microsoft.Xna.Framework.Input.Keyboard.GetState();

#if XBOX
           Microsoft.Xna.Framework.Input.GamePad.SetVibration(parent.InputState.activePlayerIndex, 0, 0);
#endif
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

            if ((System.DateTime.UtcNow - screenStartTime).TotalSeconds > 15)
                parent.NextScreen(this, new HighScoresScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
        }

        public override void HandleInput(GameTime gameTime, InputManager input)
        {
            int lastItem = whichItem;

#if XBOX || WINDOWS_PHONE
            if (!gameHasFocus)
            {
                gameHasFocus = true;
                isTrial = Microsoft.Xna.Framework.GamerServices.Guide.IsTrialMode;
                if (!isTrial)
                {
                    options.Remove("Buy");
                    banner = null;
                    screenStartTime = DateTime.UtcNow;
                }
            }
#endif

            if (playIntro)
            {
#if XBOX || WINDOWS
                if (kb.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter | Microsoft.Xna.Framework.Input.Keys.Space) &&
                    pkb.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Enter | Microsoft.Xna.Framework.Input.Keys.Space))
                    playIntro = false;

                if ((input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Start) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.Start)) ||
                    (input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.A) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.A)))
                    playIntro = false;
#endif

                if (input.touches.Count > 0 && input.touches[0].state == TouchState.Pressed)
                    playIntro = false;

                return; //ignore rest
            }

            if (input.isBackButtonPressed && !playIntro)
            {
                parent.GameExit((Transition)((Main)parent.Game).fadeOutTransition);
                return;
            }
            else if (input.isBackButtonPressed)
                playIntro = false;

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
            if ((input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.A) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.A)) ||
                (input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Start) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.Start)))
            {
                SelectItem(whichItem);
                glowStart = DateTime.UtcNow.Ticks;
            }
            //press B to go back to 'start' screen
            else if (input.gpState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.B) && input.pGPState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.B))
            {
                parent.NextScreen(this, new StartScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
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
                contains = new Rectangle(w - 300, h - ((options.Count + 1) * 50), 250, options.Count * 50).Contains((int)input.touches[0].position.X, (int)input.touches[0].position.Y); //is the mouse in the buttons area
#if WINDOWS
            if (contains)
                whichItem = (short)(((int)input.touches[0].position.Y - (h - ((options.Count + 1) * 50))) / 50);
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
        /// used to check if player is on market screen
        /// </summary>
        bool gameHasFocus = true;

        /// <summary>
        /// Perform an action when 
        /// </summary>
        /// <param name="item"></param>
        void SelectItem(int item)
        {
            if (options[item] == "Play")
                parent.NextScreen(this, new DifficultySelectScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);

            else if (options[item] == "Credits")
                parent.NextScreen(this, new CreditsScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
#if XBOX || WINDOWS_PHONE
            else if (options[item] == "Buy")
            {
                gameHasFocus = false;
                Microsoft.Xna.Framework.GamerServices.SignedInGamer gamer = Microsoft.Xna.Framework.GamerServices.Gamer.SignedInGamers[parent.InputState.activePlayerIndex];

                if (gamer != null)
                {
                    try
                    {
                        Microsoft.Xna.Framework.GamerServices.Guide.ShowMarketplace(parent.InputState.activePlayerIndex);
                    }
                    catch (Exception expt)
                    {
                        Microsoft.Xna.Framework.GamerServices.Guide.BeginShowMessageBox("Error", "Error: You must be signed into an Xbox Live online account to puchase this game.", new string[] { "OK" }, 0, 
                            Microsoft.Xna.Framework.GamerServices.MessageBoxIcon.Error, null, null);
                    }
                }
                else
                {
                    Microsoft.Xna.Framework.GamerServices.Guide.ShowSignIn(1, true);
                }
            }
#endif
            else if (options[item] == "Options") 
                parent.NextScreen(this, new OptionsScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
            else if (options[item] == "Exit")
                parent.GameExit(((Main)parent.Game).fadeOutTransition);
        }

#if XBOX
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            if (playIntro)
            {
                int time = (int)(System.DateTime.UtcNow - screenStartTime).TotalMilliseconds;

                Vector2 center = Vector2.Zero;
                if (time < 2800)
                    center = new Vector2(screenRect.Width >> 1, screenRect.Height >> 1) - new Vector2(titleLogo.Width >> 1, titleLogo.Height >> 1);

                if (time >= 3300 + options.Count * 100)
                    playIntro = false;
                else
                {
                    if (time >= 3200)
                    {
                        int n = options.Count * 100;
                        for (int i = 0; i < (n - (3200 + n - time)) / 100; i++)
                            spriteBatch.DrawString(font, options[i], new Vector2(w - 60 - font.MeasureString(options[i]).X,
                                h - ((options.Count + 1) * 50) + (i * 50)), Color.White);
                    }
                    if (time >= 2800)
                        spriteBatch.Draw(splatterImg, new Vector2(w - splatterImg.Width, h - splatterImg.Height), Color.White);
                    if (time >= 2200)
                        spriteBatch.Draw(titleLogo, center + new Vector2(10), Color.White);
                    else if (time >= 1200)
                    {
                        float c = ((2200f - time) / 1000f);
                        spriteBatch.Draw(introImg2, center + new Vector2(10, 25), new Color(c, 1, c));
                    }
                    else if (time >= 200)
                    {
                        float c = ((1200f - time) / 1000f);
                        spriteBatch.Draw(introImg1, center + new Vector2(15, 45), new Color(c, c, c));
                    }
                    else
                        spriteBatch.Draw(introImg1, center + new Vector2(15, 45), new Rectangle(0, 0, 264, 147), Color.White);
                }
            }
            if (!playIntro) //add this test for when playIntro finishes
            {
                spriteBatch.Draw(splatterImg, new Vector2(w - splatterImg.Width, h - splatterImg.Height), Color.White);

                spriteBatch.Draw(titleLogo, new Vector2(30), Color.White);

                if (banner != null)
                    spriteBatch.Draw(banner, new Vector2(80, 40 + titleLogo.Height), Color.White);

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
            }

            spriteBatch.End();
        }
#else
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            if (playIntro)
            {
                int time = (int)(System.DateTime.UtcNow - screenStartTime).TotalMilliseconds;

                Vector2 center = Vector2.Zero;
                if (time < 2800)
                    center = new Vector2(screenRect.Width >> 1, screenRect.Height >> 1) - new Vector2(titleLogo.Width >> 1, titleLogo.Height >> 1);

                if (time >= 3300 + options.Count * 100)
                    playIntro = false;
                else
                {
                    if (time >= 3200)
                    {
                        int n = options.Count * 100;
                        for (int i = 0; i < (n - (3200 + n - time)) / 100; i++)
                            spriteBatch.DrawString(font, options[i], new Vector2(w - 60 - font.MeasureString(options[i]).X,
                                h - ((options.Count + 1) * 50) + (i * 50)), Color.White);
                    }
                    if (time >= 2800)
                        spriteBatch.Draw(splatterImg, new Vector2(w - splatterImg.Width, h - splatterImg.Height), Color.White);
                    if (time >= 2200)
                        spriteBatch.Draw(titleLogo, center + new Vector2(10), Color.White);
                    else if (time >= 1200)
                    {
                        float c = ((2200f - time) / 1000f);
                        spriteBatch.Draw(introImg2, center + new Vector2(10, 25), new Color(c, 1, c));
                    }
                    else if (time >= 200)
                    {
                        float c = ((1200f - time) / 1000f);
                        spriteBatch.Draw(introImg1, center + new Vector2(15, 45), new Color(c, c, c));
                    }
                    else
                        spriteBatch.Draw(introImg1, center + new Vector2(15, 45), new Rectangle(0, 0, 264, 147), Color.White);
                }
            }
            if (!playIntro) //add this test for when playIntro finishes
            {
                spriteBatch.Draw(splatterImg, new Vector2(w - splatterImg.Width, h - splatterImg.Height), Color.White);

#if WINDOWS_PHONE || ZUNE
            spriteBatch.Draw(titleLogo, new Vector2(10), Color.White);
            if (banner != null)
                spriteBatch.Draw(banner, new Vector2(50, titleLogo.Height - 10), Color.White);
#else
                spriteBatch.Draw(titleLogo, new Vector2(10), Color.White);

                if (banner != null)
                    spriteBatch.Draw(banner, new Vector2(50, 10 + titleLogo.Height), Color.White);
#endif

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
                    spriteBatch.DrawString(font, options[i], new Vector2(w - 50 - font.MeasureString(options[i]).X, h - ((options.Count + 1) * 50) + (i * 50)), whichItem == i ? glowColor : Color.White);
            }

            spriteBatch.End();
        }
#endif

        #endregion
    }
}

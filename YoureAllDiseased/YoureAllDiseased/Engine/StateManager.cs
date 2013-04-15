//StateManager.cs
//Copyright Dejitaru Forge 2011

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;

#if XNA31 || XBOX
using Microsoft.Xna.Framework.Graphics;
#endif

namespace YoureAllDiseased
{
    /// <summary>
    /// The Main state manager for the game that handles all assets, input, and screens
    /// </summary>
    public class StateManager : DrawableGameComponent
    {
        #region data

        /// <summary>
        /// True if the game is exiting (use for separate threads)
        /// </summary>
        public bool isExiting = false;

        /// <summary>
        /// Is the game in trial mode? (use this not Guide.IsTrialMode, faster)
        /// </summary>
        public bool isTrialMode { get; set; }

        /// <summary>
        /// The current game screens in use
        /// </summary>
        public List<GameScreen> Screens
        {
            get { return screens; }
            protected set { screens = value; }
        }
        List<GameScreen> screens = new List<GameScreen>(4);

        /// <summary>
        /// The main input manager for the game state manager
        /// </summary>
        public InputManager InputState
        {
            get { return inputState; }
            protected set { inputState = value; }
        }
        InputManager inputState;

        /// <summary>
        /// The main/default font to be used by game screens
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.SpriteFont Font
        {
            get { return font; }
            protected set { font = value; }
        }
        Microsoft.Xna.Framework.Graphics.SpriteFont font;

        /// <summary>
        /// A white texture the size of the screen
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.Texture2D BlankTexture
        {
            get { return blankTexture; }
            protected set { blankTexture = value; }
        }
        Microsoft.Xna.Framework.Graphics.Texture2D blankTexture;

        /// <summary>
        /// The main spritebatch that all screens inherit from
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            protected set { spriteBatch = value; }
        }
        Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;

        /// <summary>
        /// The current screen needing removed
        /// Used for transitions
        /// If this is not null when another screen is to be removed, force remove the current
        /// </summary>
        GameScreen screenToRemove = null;

        /// <summary>
        /// A screen waiting to be added
        /// Used for transitions
        /// If this is not null when another screen is added, force add the current
        /// </summary>
        GameScreen screenToAdd = null;
        /// <summary>
        /// The arguments for the screen waiting to be added
        /// </summary>
        List<Object> argsForAdd = null;

        public Transition currentTransition = null;
        public Transition queuedTransition = null;

        /// <summary>
        /// The main content manager that all game screens should inherit from
        /// </summary>
        public ContentManager Content { get; protected set; }

        /// <summary>
        /// Ticks based on frames (not time) for the whole game, only updated when drawing
        /// </summary>
        public long frameTicks { get; private set; }

        /// <summary>
        /// A randomizer for use
        /// </summary>
        public Random random = new Random();

#if  XNA31
        /// <summary>
        /// The game is rendered to this then scaled
        /// </summary>
        RenderTarget2D render;
        /// <summary>
        /// Flip the display updside down (for device left/right displaying) - false = left
        /// </summary>
        bool flip = false;
        Viewport large = new Viewport();
        Viewport small = new Viewport();
#endif

        #endregion


        #region Initialization

        public StateManager(Main main)
            : base(main)
        {
            Content = Game.Content;
            frameTicks = 0;

            inputState = new InputManager();
        }

        /// <summary>
        /// This loads the font and a few settings
        /// </summary>
        protected override void LoadContent()
        {
#if !XNA31
            GraphicsDevice.BlendState = Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend; //default enable alpha blending
#endif
            spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);

            font = Content.Load<Microsoft.Xna.Framework.Graphics.SpriteFont>("Fonts/System");

#if XNA31
            small = GraphicsDevice.Viewport;
            large.X = 0; large.Y = 0; large.Width = 800; large.Height = 480;
#endif
#if ZUNE
            render = new RenderTarget2D(GraphicsDevice, 800, 480, 1, SurfaceFormat.Color); //size of the Windows phone display
#endif
        }

        protected override void UnloadContent() { }

        #endregion


        #region Update & Draw

        /// <summary>
        /// Update all of the screens
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Update(GameTime gameTime)
        {
            inputState.Update();

            bool isCovered = false; //is the screen covered by another (like a popup)?
            bool isVisible = Game.IsActive; //is the screen visible?

            //loop through all of the screens (backwards)
            for (int i = screens.Count - 1; i >= 0; i--)
            {
                //only update input if visible (or going to be)
                if ((screens[i].screenState == ScreenState.TransitionOn || screens[i].screenState == ScreenState.Active) && !isExiting)
                {
                    ScreenType type = screens[i].screenType; //placed here so no crashes/bugs when a screen is destroyed in the input update

                    //only update when not leaving
                    if (!isCovered)
                        screens[i].Update(gameTime, isVisible, isCovered);

                    //unset transitions
                    if (currentTransition == null)
                    {
                        if (screens[i].screenState == ScreenState.TransitionOn)
                            screens[i].screenState = ScreenState.Active;
                        if (screens[i].screenState == ScreenState.TransitionOff)
                            screens[i].screenState = ScreenState.Inactive;
                    }

                    //only handle input if this is the active screen
                    if (isVisible && screens[i].screenState == ScreenState.Active && !isCovered)
                    {
                        //this screen is covering one below it
                        if (screens[i].screenType == ScreenType.Screen)
                            isVisible = false;

                        screens[i].HandleInput(gameTime, inputState);
                    }

                    if (type != ScreenType.Overlay)
                        isCovered = true;
                }
            }

            if (isExiting && currentTransition == null)
                Game.Exit();

            //handle transitions
            if (currentTransition != null)
            {
                //done transitioning off, remove screen
                if (currentTransition.currentFrame >= currentTransition.frames)
                {
                    //exit the game if supposed to (second transition is ignored)
                    if (isExiting)
                        Game.Exit();

                    if (screenToAdd != null)
                    {
                        screens.Add(screenToAdd);
                        screenToAdd.parent = this;
                        screenToAdd.content = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
                        screenToAdd.LoadContent(argsForAdd);
                        screenToAdd.spriteBatch = spriteBatch;
                        screenToAdd.screenStartTime = DateTime.UtcNow;

                        if (queuedTransition != null)
                        {
                            currentTransition = queuedTransition; //start the transition
                            queuedTransition.ResetFrame();

                            screenToAdd.screenState = ScreenState.TransitionOn;
                        }
                        else
                        {
                            screenToAdd.screenState = ScreenState.Active;
                        }

                        screenToAdd = null;
                        argsForAdd = null;
                    }

                    currentTransition = null; //remove current transition (reset on reload)

                    if (queuedTransition != null) //if theres a second transition, load it
                    {
                        currentTransition = queuedTransition;
                        currentTransition.ResetFrame();
                        queuedTransition = null;
                    }

                    ForceRemove(screenToRemove);
                }
            }
        }


        /// <summary>
        /// Draw all of the screens
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Draw(GameTime gameTime)
        {
            frameTicks++; //update frame count

#if ZUNE
            GraphicsDevice.SetRenderTarget(0, render);
            GraphicsDevice.Viewport = large;
#endif

            if (screens.Count < 1)
            {
#if XNA31
                GraphicsDevice.Clear(Microsoft.Xna.Framework.Graphics.Color.Black);
#else
                GraphicsDevice.Clear(Color.Black);
#endif
            }
            else
            {

                for (int i = 0; i < screens.Count; i++)
                {
                    if (screens[i].screenState != ScreenState.Inactive)
                        screens[i].Draw(gameTime);
                }

                //draw transition
                if (currentTransition != null)
                {
#if XNA31
                    spriteBatch.Begin(Microsoft.Xna.Framework.Graphics.SpriteBlendMode.AlphaBlend);
#else
                spriteBatch.Begin();
#endif

                    currentTransition.UpdateFrame();
                    currentTransition.Draw(ref spriteBatch);

                    spriteBatch.End();
                }
            }
#if ZUNE
            GraphicsDevice.SetRenderTarget(0, null);
            GraphicsDevice.Viewport = small;
            Texture2D resolveTx = render.GetTexture();

            spriteBatch.Begin();
            //draw the rendered game to the screen, stretched to fit and rotated
            spriteBatch.Draw(resolveTx, new Rectangle(0, 0, 480, 272), null, Color.White, MathHelper.PiOver2,
                new Vector2(0, 480), flip ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);

            spriteBatch.End();
#endif
        }

        #endregion


        #region Public

        /// <summary>
        /// Add a screen to the state manager
        /// </summary>
        /// <param name="screen">The screen to add</param>
        /// <param name="args">Arguments to send to the new screen (like level parameters); null for none</param>
        /// <returns>True on successful addition</returns>
        public bool AddScreen(GameScreen screen, List<Object> args, Transition transition)
        {
            //no active transition, add it immediately
            if (currentTransition == null)
            {
                //add the screen and load it
                screens.Add(screen);
                screen.parent = this;
                screen.content = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
                screen.LoadContent(args);
                screen.spriteBatch = spriteBatch;
                screen.screenStartTime = DateTime.UtcNow;

                if (transition != null)
                {
                    currentTransition = transition; //start the transition
                    transition.ResetFrame();

                    screen.screenState = ScreenState.TransitionOn;
                }
                else
                {
                    screen.screenState = ScreenState.Active;
                }

                screenToAdd = null;
            }
            else
            {
                if (screenToAdd == null)
                {
                    screenToAdd = screen;
                    argsForAdd = args;
                    queuedTransition = transition;
                }

                else
                {
                    screens.Add(screenToAdd);
                    screen.parent = this;
                    screen.content = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
                    screen.LoadContent(argsForAdd);
                    screen.spriteBatch = spriteBatch;
                    screen.screenStartTime = DateTime.UtcNow;
                    screen.screenState = ScreenState.Active;

                    screenToAdd = screen;
                    argsForAdd = args;
                    queuedTransition = transition;
                }

            }

            return true;
        }

        /// <summary>
        /// Remove a screen from the state manager
        /// </summary>
        /// <param name="screen">The screen to remove</param>
        /// <param name="transition">The transition to use</param>
        /// <returns>True on successful removal</returns>
        public bool RemoveScreen(GameScreen screen, Transition transition)
        {
            if (!screens.Contains(screen))
                return false;

            //make sure the last screen was removed
            if (screenToRemove != null)
                ForceRemove(screenToRemove);

            //add the screen to the list of screens to remove
            screenToRemove = screen;

            //if no transition, remove immediately
            if (transition == null)
            {
                ForceRemove(screen);
                return true;
            }

            //start the transition
            if (currentTransition == null)
            {
                currentTransition = transition;
                transition.ResetFrame();
            }

            screen.screenState = ScreenState.TransitionOff;

            return true;
        }

        /// <summary>
        /// Properly remove and load the next screen so that transitions work properly
        /// </summary>
        /// <param name="current">the current screen to remove</param>
        /// <param name="next">the next screen to load after that</param>
        /// <param name="nextArgs">arguments for the next screen</param>
        public void NextScreen(GameScreen current, GameScreen next, List<Object> nextArgs,
            Transition partingTransition, Transition comingTransition)
        {
            RemoveScreen(current, partingTransition);
            AddScreen(next, nextArgs, comingTransition);
        }

        /// <summary>
        /// Add a transition before exiting
        /// </summary>
        public void GameExit(Transition exitTransition)
        {
            isExiting = true;
            Main.isMusicFading = true;
            currentTransition = exitTransition;
            if (currentTransition != null)
                currentTransition.ResetFrame();
        }

        #endregion


        #region Other

        /// <summary>
        /// Forcibly remove a screen (don't transition off)
        /// </summary>
        /// <param name="screen">The screen to remove</param>
        /// <returns>True on successful removal</returns>
        public bool ForceRemove(GameScreen screen)
        {
            if (screens.Contains(screen))
            {
                //see if it's scheduled to be removed
                if (screenToRemove == screen)
                    screenToRemove = null;

                screen.parent = null;
                screen.screenState = ScreenState.Inactive;
                screens.Remove(screen);
                screen.UnloadContent();
                if (screen.spriteBatch != spriteBatch && screen.spriteBatch != null)
                    screen.spriteBatch.Dispose();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the safe area for drawing (used mainly for xbox)
        /// </summary>
        /// <param name="percent">percentage of total screen size (0-1), default: 0.2</param>
        /// <returns>the size of the safe rectangle</returns>
        public Rectangle GetTitleSafeArea(float percent)
        {
            Rectangle retVal = new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y,
                GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height); //default is whole screen
#if XBOX
            // Find Title Safe area for the Xbox 360.
            float border = (1.0f - percent) / 2.0f;
            retVal.X = (int)(border * retVal.Width);
            retVal.Y = (int)(border * retVal.Height);
            retVal.Width = (int)(percent * retVal.Width);
            retVal.Height = (int)(percent * retVal.Height);
#endif

            return retVal;
        }

        #endregion
    }
}

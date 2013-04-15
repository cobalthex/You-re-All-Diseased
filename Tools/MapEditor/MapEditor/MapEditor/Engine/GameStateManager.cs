//GameStateManager.cs
//Copyright Dejitaru Forge 2011

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#if XNA31
using Microsoft.Xna.Framework.Graphics;
#endif

namespace MapEditor
{
    /// <summary>
    /// The Main state manager for the game that handles all assets, input, and screens
    /// </summary>
    public class GameStateManager : DrawableGameComponent
    {
        #region data

        /// <summary>
        /// True while the state manager is initializing
        /// </summary>
        private bool loading; 

        /// <summary>
        /// True if the game is exiting (use for separate threads)
        /// </summary>
        public bool isExiting = false;

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

        #endregion


        #region Initialization

        public GameStateManager(Game main)
            : base(main)
        {
            Content = Game.Content;
            frameTicks = 0;

            inputState = new InputManager();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// This loads the font and a few settings
        /// </summary>
        protected override void LoadContent()
        {
            loading = true;
#if !XNA31
            GraphicsDevice.BlendState = Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend; //default enable alpha blending
#endif
            spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);

            font = Content.Load<Microsoft.Xna.Framework.Graphics.SpriteFont>("Font");

            loading = false;
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
            if (loading)
                return;

            inputState.Update();

            bool isCovered = false; //is the screen covered by another (like a popup)?
            bool isVisible = Game.IsActive; //is the screen visible?

            //loop through all of the screens (backwards)
            for (int i = screens.Count - 1; i >= 0; i--)
            {
                //only update input if visible (or going to be)
                if ((screens[i].screenState == ScreenState.TransitionOn || screens[i].screenState == ScreenState.Active) && !isExiting)
                {
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

                    bool covering = isCovered;
                    //only overlays don't cover
                    if (screens[i].screenType != ScreenType.Overlay)
                        covering = true;

                    //only handle input if this is the active screen
                    if (isVisible && screens[i].screenState == ScreenState.Active && covering)
                    {
                        //this screen is covering one below it
                        if (screens[i].screenType == ScreenType.Screen)
                            isVisible = false;

                        if (MapEditor.Game.isFocused)
                            screens[i].HandleInput(gameTime, inputState);
                    }

                    isCovered = covering; //set this with a second variable to make sure that if the player quits while in the input handler, it doesnt break
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
            GraphicsDevice.Clear(Color.Black);

            if (screens.Count < 1 || loading)
                return;

            frameTicks++; //update frame count

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
                screen.content = Content;
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
                    screen.content = Content;
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
            ((Game)Game).exiting = true;
            isExiting = true;
            if (exitTransition != null)
                currentTransition = exitTransition;
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
                if (screen.spriteBatch != spriteBatch)
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

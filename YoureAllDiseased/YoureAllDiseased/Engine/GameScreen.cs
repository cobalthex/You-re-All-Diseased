//GameScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;

namespace YoureAllDiseased
{
    /// <summary>
    /// The type of screen
    /// </summary>
    public enum ScreenType
    {
        /// <summary>
        /// A normal screen (covers and hides)
        /// </summary>
        Screen,
        /// <summary>
        /// A popup/dialog (covers but does not hide)
        /// </summary>
        Popup,
        /// <summary>
        /// A screen to be drawn over the one below (does not cover or hide)
        /// </summary>
        Overlay
    }

    /// <summary>
    /// The state of the screen
    /// </summary>
    public enum ScreenState
    {
        /// <summary>
        /// Currently running/visible
        /// </summary>
        Active,
        /// <summary>
        /// Waiting to load
        /// </summary>
        TransitionOn,
        /// <summary>
        /// Leaving
        /// </summary>
        TransitionOff,
        /// <summary>
        /// Not running/visible
        /// </summary>
        Inactive
    }

    /// <summary>
    /// The base type for all screens handled by the state manager
    /// </summary>
    public class GameScreen
    {
        #region Data

        /// <summary>
        /// The state manager handling this screen
        /// </summary>
        public StateManager parent = null;

        /// <summary>
        /// The type of screen
        /// </summary>
        public ScreenType screenType = ScreenType.Screen;

        /// <summary>
        /// The state of the screen
        /// </summary>
        public ScreenState screenState = ScreenState.Inactive;

        /// <summary>
        /// The content manager for this screen (loaded on screen open, unloaded after exit)
        /// </summary>
        public Microsoft.Xna.Framework.Content.ContentManager content;

        /// <summary>
        /// A pre-implemented spritebatch for use
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;

        /// <summary>
        /// The time that the screen was loaded
        /// </summary>
        public DateTime screenStartTime;

        #endregion


        #region Initialization

        /// <summary>
        /// Load graphics content for the screen.
        /// <param name="args">Arugments sent by the state manager</param>
        /// </summary>
        public virtual void LoadContent(System.Collections.Generic.List<object> args) { }


        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public virtual void UnloadContent() { }

        #endregion


        #region Update & Draw

        /// <summary>
        /// Handle any input
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="input">Input manager to read input from</param>
        public virtual void HandleInput(GameTime gameTime, InputManager input) { }

        /// <summary>
        /// Update the screen
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="isVisible">Is the screen visible by the user?</param>
        /// <param name="isCovered">Is the screen covered by another screen?</param>
        public virtual void Update(GameTime gameTime, bool isVisible, bool isCovered) { }

        /// <summary>
        /// Draw the screen
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public virtual void Draw(GameTime gameTime) { }

        #endregion


        #region Public

        /// <summary>
        /// Close this screen
        /// </summary>
        /// <param name="exitTransition">The optional exiting transition</param>
        public void Close(Transition exitTransition)
        {
            parent.RemoveScreen(this, exitTransition);
        }

        /// <summary>
        /// Reset this screen (does not reload content)
        /// </summary>
        public void Reset()
        {
            screenState = ScreenState.Active;
            screenStartTime = DateTime.UtcNow;
        }

        #endregion
    }
}

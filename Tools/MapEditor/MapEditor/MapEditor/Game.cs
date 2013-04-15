//Game.cs
//Copyright Dejitaru Forge 2011

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MapEditor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static bool isFocused = true;

        public bool exiting = false;

        #region Data

        /// <summary>
        /// The various editor modes
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Placing tiles
            /// </summary>
            Tiles,
            /// <summary>
            /// Creating collision maps
            /// </summary>
            Collision,
            /// <summary>
            /// Placing entities
            /// </summary>
            Entities,
            /// <summary>
            /// Adding mucus blobs
            /// </summary>
            Mucus
        }

        /// <summary>
        /// The current editor mode in use
        /// </summary>
        public Mode currentMode = Mode.Tiles;

        public SpriteBatch sB;
        public GraphicsDeviceManager graphics;

        public GameStateManager gSM;

        public RenderTarget2D render;
        public bool takingPic = false;

        /// <summary>
        /// Color of collision lines
        /// </summary>
        public static Color lineColor = new Color(192, 192, 192, 192);
        /// <summary>
        /// Color of grid
        /// </summary>
        public static Color gridColor = lineColor;
        /// <summary>
        /// Color of map bounding box
        /// </summary>
        public static Color boundsColor = gridColor;

        /// <summary>
        /// the main map
        /// </summary>
        public Map map;

        //main file dialog for opening/saving
        public System.Windows.Forms.OpenFileDialog oFileDlg;
        public System.Windows.Forms.SaveFileDialog sFileDlg;

        public SelectorDialog selector;

        public Screens.Collision collisionEditor;
        public Screens.Entities entitiesEditor;
        public Screens.Tiles tileEditor;
        public Screens.Mucus mucusEditor;

        /// <summary>
        /// Show/hide the grid
        /// </summary>
        public bool showGrid = false;
        /// <summary>
        /// show the bounding box
        /// </summary>
        public bool showBounds = true;
        /// <summary>
        /// Show/hide collision lines (irrelevent on Collision mode)
        /// </summary>
        public bool showCollision = false;

        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();

            using (Game game = new Game())
            {
                game.Run();
            }
        }

        /// <summary>
        /// The main constructor
        /// </summary>
        public Game()
        {
            graphics = new Microsoft.Xna.Framework.GraphicsDeviceManager(this);
            Content = new Microsoft.Xna.Framework.Content.ResourceContentManager(this.Services, res.ResourceManager);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferMultiSampling = true; //nice lines
            graphics.IsFullScreen = false;
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            selector = new SelectorDialog(this);
            selector.Show();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            /*if (!exiting && System.Windows.Forms.MessageBox.Show("Are you sure you wish to exit?", "Exit",
                System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Yes)
                return;*/
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            //set up file dialogs
            oFileDlg = new System.Windows.Forms.OpenFileDialog();
            oFileDlg.AddExtension = true;
            oFileDlg.Filter = "All supported types|*.2m;*.2mcp|2D maps (*.2m)|*.2m|2D collision maps (*.2mcp)|*.2mcp|All files (*.*)|*.*";
            oFileDlg.CheckFileExists = true;
            oFileDlg.ValidateNames = true;

            sFileDlg = new System.Windows.Forms.SaveFileDialog();
            sFileDlg.AddExtension = true;
            sFileDlg.DefaultExt = "2mcp";
            sFileDlg.Filter = "2D collision maps (*.2mcp)|*.2mcp|Other|*.*";
            sFileDlg.AutoUpgradeEnabled = true;
            sFileDlg.ValidateNames = true;

            Liner.Init(GraphicsDevice);

            map = new Map();

            gSM = new GameStateManager(this);
            gSM.Initialize();
            Components.Add(gSM);

            tileEditor = new Screens.Tiles();
            gSM.AddScreen(tileEditor, null, null);

            collisionEditor = new Screens.Collision();
            gSM.AddScreen(collisionEditor, null, null);
            collisionEditor.screenState = ScreenState.Inactive;

            entitiesEditor = new Screens.Entities();
            gSM.AddScreen(entitiesEditor, null, null); 
            entitiesEditor.screenState = ScreenState.Inactive;

            mucusEditor = new Screens.Mucus();
            gSM.AddScreen(mucusEditor, null, null);
            mucusEditor.screenState = ScreenState.Inactive;
        }

        public void SetMode(Mode newMode)
        {
            if (currentMode == newMode)
                return;

            currentMode = newMode;
            
            if (newMode == Mode.Tiles)
            {
                tileEditor.screenState = ScreenState.Active;
                collisionEditor.screenState = ScreenState.Inactive;
                entitiesEditor.screenState = ScreenState.Inactive;
                mucusEditor.screenState = ScreenState.Inactive;

                selector.tileTool.PerformClick();
            }
            else if (newMode == Mode.Collision)
            {
                tileEditor.screenState = ScreenState.Inactive;
                collisionEditor.screenState = ScreenState.Active;
                entitiesEditor.screenState = ScreenState.Inactive;
                mucusEditor.screenState = ScreenState.Inactive;

                selector.collisionTool.PerformClick();
            }
            else if (newMode == Mode.Entities)
            {
                tileEditor.screenState = ScreenState.Inactive;
                collisionEditor.screenState = ScreenState.Inactive;
                entitiesEditor.screenState = ScreenState.Active;
                mucusEditor.screenState = ScreenState.Inactive;

                selector.entityTool.PerformClick();
            }
            else if (newMode == Mode.Mucus)
            {
                tileEditor.screenState = ScreenState.Inactive;
                collisionEditor.screenState = ScreenState.Inactive;
                entitiesEditor.screenState = ScreenState.Inactive;
                mucusEditor.screenState = ScreenState.Active;

                selector.mucusTool.PerformClick();
            }
        }
    }
}

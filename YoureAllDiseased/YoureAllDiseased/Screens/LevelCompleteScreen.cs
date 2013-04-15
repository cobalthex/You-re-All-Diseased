//LevelCompleteScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace YoureAllDiseased
{
    /// <summary>
    /// The level completion screen
    /// </summary>
    public class LevelCompleteScreen : GameScreen
    {
        #region Data

        /// <summary>
        /// The image saying "Success!" displayed in the center of the screen
        /// </summary>
        Texture2D successLogo;

        /// <summary>
        /// Bitmap numbers used to show score
        /// </summary>
        Texture2D numbersSprite;

        /// <summary>
        /// Player's score, represented in string form
        /// </summary>
        string scoreTxt = "00000000";

        /// <summary>
        /// The font for the subtext
        /// </summary>
        SpriteFont subTextFont;

        /// <summary>
        /// A single piece of firework
        /// </summary>
        public ParticleType firework;
        /// <summary>
        /// fireworks!
        /// </summary>
        public ParticleEmitter fireworks;

        int score = 0;
        int lvl = 0;
        int kills = 0;
        long startTime = 0;

        Entities.Player p;

        //width & height of screen
        int w, h;

        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
#if WINDOWS_PHONE
            ((Main)parent.Game).gameAd300.Visible = false;
            ((Main)parent.Game).gameAd480.Visible = false;
#endif

#if WINDOWS || XBOX
            Microsoft.Xna.Framework.Input.GamePad.SetVibration(parent.InputState.activePlayerIndex, 0, 0);
#endif

#if XNA31 //force zune to these dimensions (renders to a render target which is then scaled to display)
            w = 800;
            h = 480;
#else
            w = parent.GraphicsDevice.Viewport.Width;
            h = parent.GraphicsDevice.Viewport.Height;
#endif

#if !WINDOWS && !ZUNE
            parent.isTrialMode = Microsoft.Xna.Framework.GamerServices.Guide.IsTrialMode;
#endif

            successLogo = content.Load<Texture2D>("Graphics/LevelComplete");
            numbersSprite = content.Load<Texture2D>("Graphics/Numbers");
            subTextFont = content.Load<SpriteFont>("Fonts/Menu");

#if XNA31
            firework = new ParticleType(content.Load<Texture2D>("Graphics/Particles/Explosion"), Color.White, Microsoft.Xna.Framework.Graphics.SpriteBlendMode.Additive);
#else
            firework = new ParticleType(content.Load<Texture2D>("Graphics/Particles/Explosion"), Color.White, BlendState.Additive);
#endif
            fireworks = new ParticleEmitter(firework); fireworks.decay = 0.1f; fireworks.gravity.Y = 0.2f;

            if (args.Count > 4)
            {
                lvl = (int)args[0];
                score = (int)args[1];
                scoreTxt = ((int)args[1]).ToString().PadLeft(8, '0');
                kills = (int)args[2];
                startTime = (long)args[3];
                p = (Entities.Player)args[4];
            }
        }

        #endregion


        #region Update & Draw

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool isVisible, bool isCovered)
        {
            if (parent.frameTicks % 5 == 0)
            {
                firework.hue.R = (byte)parent.random.Next(0, 255);
                firework.hue.G = (byte)parent.random.Next(0, 255);
                firework.hue.B = (byte)parent.random.Next(0, 255);
            }

            if (parent.frameTicks % 10 == 0)
            {
                float a = 0;

                //Vector2 pos1 = new Vector2(0.25f * w, 100);
                Vector2 pos1 = new Vector2(parent.random.Next(0, w), parent.random.Next(0, h));
                Vector2 pos2 = new Vector2(parent.random.Next(0, w), parent.random.Next(0, h));
                //Vector2 pos2 = new Vector2(0.75f * w, 100);

#if ZUNE
                for (int i = 0; i < 2; i++) //flower fireworks
#else
                for (int i = 0; i < 12; i++) //flower fireworks
#endif
                {
                    a = MathHelper.ToRadians(i * 36); //angle to increase by
                    int r = 30; //radius
                    fireworks.Particulate(10, pos1 + new Vector2(r * (float)System.Math.Cos(a), r * (float)System.Math.Sin(a)), 1, 3, 0, MathHelper.TwoPi);
                    fireworks.Particulate(10, pos2 + new Vector2(r * (float)System.Math.Cos(a), r * (float)System.Math.Sin(a)), 1, 3, 0, MathHelper.TwoPi);
                }

                //center spurts shooting out of flower
                fireworks.Particulate(10, pos1, 5, 10, 0, MathHelper.TwoPi);
                fireworks.Particulate(10, pos2, 5, 10, 0, MathHelper.TwoPi);
            }

        }

        public override void HandleInput(GameTime gameTime, InputManager input)
        {
            GC.Collect(); //good time to clean up

#if XBOX || WINDOWS
            if (input.isBackButtonPressed || input.gpState.Buttons.A == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                NextScreen();
                return;
            }
#endif
            //load the next level (if there is one) on touch
            if (input.isBackButtonPressed || (input.touches.Count > 0 && input.touches[0].state == TouchState.Pressed))
                NextScreen();
        }

        void NextScreen()
        {
#if !WINDOWS && !ZUNE
            if (Microsoft.Xna.Framework.GamerServices.Guide.IsTrialMode)
            {
                parent.NextScreen(this, new TrialOverScreen(), new System.Collections.Generic.List<object> { lvl + 1, score, kills, p }, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
                return;
            }
#endif

            string levl = "singleplay/" + (lvl + 1) + ".2mcp";
#if WINDOWS
            if (System.IO.File.Exists("content/Maps/" + levl))
                parent.NextScreen(this, new PlayScreen(), new System.Collections.Generic.List<object> { lvl + 1, score, kills, p }, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
            else
                parent.NextScreen(this, new NameEntryScreen(), new System.Collections.Generic.List<object> { score }, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
#else
            try
            {
#if ZUNE
                System.IO.StreamReader f = new System.IO.StreamReader(Microsoft.Xna.Framework.Storage.StorageContainer.TitleLocation + "/content/Maps/" + levl);
                f.Close();
#else
                System.IO.Stream s = TitleContainer.OpenStream( "Content/Maps/" + levl);
                s.Close();
#endif

                parent.NextScreen(this, new PlayScreen(), new System.Collections.Generic.List<object> { lvl + 1, score, kills, p }, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
            }
            catch
            {
#if PLAYTEST
                parent.GameExit(((Main)parent.Game).fadeOutTransition);
#else
                parent.NextScreen(this, new NameEntryScreen(), new System.Collections.Generic.List<object> { score }, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
#endif
            }
#endif
        }

#if XBOX
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(successLogo, new Vector2((w >> 1) - (successLogo.Width >> 1),
                (h >> 1) - (successLogo.Height >> 1)), Color.White);

            int drawPosX = (w >> 1) - (scoreTxt.Length * 25);
            int drawPosY = h - (h >> 2) - 60;
            for (int i = 0; i < scoreTxt.Length; i++)
            {
                int n = scoreTxt[i] - 48; //ascii '0' starts at chr 48
                spriteBatch.Draw(numbersSprite, new Vector2(drawPosX + (i * 50), n == 4 ? drawPosY - 2 : drawPosY),
                    new Rectangle(0, 0 + (scoreTxt[i] - 48) * 50, 51, 50), Color.White);
            }

            spriteBatch.DrawString(subTextFont, "Lives saved!", new Vector2((w >> 1) -
                ((int)(subTextFont.MeasureString("Lives saved!").X) >> 1), drawPosY + 60), new Color(128, 240, 0));

            spriteBatch.DrawString(parent.Font, "Press to continue", new Vector2((w >> 1) -
                ((int)(parent.Font.MeasureString("Press to continue").X) >> 1), 100), new Color(128, 240, 0));

            spriteBatch.End();

            fireworks.Draw(gameTime, parent.GraphicsDevice, Vector2.Zero);
        }
#else
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(successLogo, new Vector2((w >> 1) - (successLogo.Width >> 1),
                (h >> 1) - (successLogo.Height >> 1)), Color.White);

            int drawPosX = (w >> 1) - (scoreTxt.Length * 25);
            int drawPosY = h - (h >> 2) - 30;
            for (int i = 0; i < scoreTxt.Length; i++)
            {
                int n = scoreTxt[i] - 48; //ascii '0' starts at chr 48
                spriteBatch.Draw(numbersSprite, new Vector2(drawPosX + (i * 50), n == 4 ? drawPosY - 2 : drawPosY),
                    new Rectangle(0, 0 + (scoreTxt[i] - 48) * 50, 51, 50), Color.White);
            }

            spriteBatch.DrawString(subTextFont, "Lives saved!", new Vector2((w >> 1) -
                ((int)(subTextFont.MeasureString("Lives saved!").X) >> 1), drawPosY + 60), new Color(128, 240, 0));

            spriteBatch.DrawString(parent.Font, "Press anywhere to continue", new Vector2((w >> 1) -
                ((int)(parent.Font.MeasureString("Press anywhere to continue").X) >> 1), 80), new Color(128, 240, 0));

            spriteBatch.End();

            fireworks.Draw(gameTime, parent.GraphicsDevice, Vector2.Zero);
        }
#endif

        #endregion
    }
}

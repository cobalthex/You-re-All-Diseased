//HighScoresScreen.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace YoureAllDiseased
{
    /// <summary>
    /// The high scores screen
    /// </summary>
    public class HighScoresScreen : GameScreen
    {
        #region Data

        /// <summary>
        /// The image saying "High Scores" displayed in the center of the screen
        /// </summary>
        Texture2D highScoresLogo;

        /// <summary>
        /// font to draw scores with
        /// </summary>
        SpriteFont font;

        /// <summary>
        /// A black to transparent fade
        /// </summary>
        Texture2D fade;

        /// <summary>
        /// Player's score, represented in string form
        /// </summary>
        string scoreTxt = "00000000";

        /// <summary>
        /// the users score, -1 for none
        /// </summary>
        int myItem = -1;

        /// <summary>
        /// scrolling highscores
        /// </summary>
        int scrollPos = 0;

        System.Collections.Generic.List<string> names;
        System.Collections.Generic.List<int> scores;

        int w, h;

        #endregion


        #region Initialization

        public override void LoadContent(System.Collections.Generic.List<object> args)
        {
            highScoresLogo = content.Load<Texture2D>("Graphics/HighScores");
#if XBOX
            font = content.Load<SpriteFont>("Fonts/Scores_Xbox");
#else
            font = content.Load<SpriteFont>("Fonts/Scores");
#endif
            fade = content.Load<Texture2D>("Graphics/fade");

            if (args != null && args.Count > 1)
            {
                myItem = Main.SaveHighScore((string)args[1], (int)args[0]);
            }

            //load highscores
            names = new System.Collections.Generic.List<string>(Main.maxHighScores);
            scores = new System.Collections.Generic.List<int>(Main.maxHighScores);

#if XNA31
            using (Microsoft.Xna.Framework.Storage.StorageContainer ctn = Main.sd.OpenContainer("You're All Diseased!"))
            {
                System.IO.StreamReader topper = new System.IO.StreamReader(ctn.Path + "/top");
#elif WINDOWS_PHONE || XBOX
            {
                System.IO.StreamReader topper = new System.IO.StreamReader(new System.IO.IsolatedStorage.IsolatedStorageFileStream("top", System.IO.FileMode.Open, Main.isoStore));          
#else
            {
                System.IO.StreamReader topper = new System.IO.StreamReader("top");
#endif

                //read all of the existing scores into a buffer
                while (!topper.EndOfStream)
                {
                    string[] s = topper.ReadLine().Split('=');
                    names.Add(s[0]);
                    scores.Add(int.Parse(s[1]));
                }

                topper.Close();
            }
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

#if XBOX
            if (scrollPos / 70 > Main.maxHighScores)
#else
            if (scrollPos / 96 > Main.maxHighScores)
#endif
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);

            if ((int)(System.DateTime.UtcNow - screenStartTime).TotalSeconds > 1)
#if ZUNE || WINDOWS_PHONE
                scrollPos += 4;
#else
                scrollPos += 2;
#endif
        }

        public override void HandleInput(GameTime gameTime, InputManager input)
        {
#if WINDOWS || XBOX
            if (input.isBackButtonPressed || input.gpState.Buttons.A == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
#if PLAYTEST //playtest mode for xbox that just plays levels
                parent.GameExit(null);
#else
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
                return;
#endif
            }
#endif
            if (input.isBackButtonPressed || input.touches.Count > 0 && input.touches[0].state == TouchState.Pressed)
                parent.NextScreen(this, new MainMenuScreen(), null, ((Main)parent.Game).fadeOutTransition, ((Main)parent.Game).fadeInTransition);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            parent.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

#if XBOX
            int start = scrollPos / 70;
#else
            int start = scrollPos / 100;
#endif

            int center = w >> 1;
#if XBOX
            int x = center - 280; //position for left text
            int x2 = center + 280; //position for right text
#else
            int x = center - 380; //position for left text
            int x2 = center + 380; //position for right text
#endif
            int y = highScoresLogo.Height + 90 - scrollPos;

            spriteBatch.Draw(highScoresLogo, new Vector2(center - (highScoresLogo.Width >> 1), 10), Color.White);

#if XBOX
            for (int i = start; i < ((h - highScoresLogo.Height + 50) / 100) + start + 2; i++)
#else
            for (int i = start; i < ((h - highScoresLogo.Height + 50) / 100) + start + 1; i++)
#endif
            {
                if (i >= scores.Count)
                    break;

                scoreTxt = scores[i].ToString().PadLeft(8, '0');

#if XBOX
                spriteBatch.DrawString(font, (myItem == i ? ">" : "") + (i + 1).ToString().PadLeft(names.Count / 10, '0') + "." + names[i], new Vector2(x, y + (i * 70)), Color.White);
                spriteBatch.DrawString(font, scoreTxt, new Vector2(x2 - (int)font.MeasureString(scoreTxt).X, y + (i * 70)), Color.White);
#else
                spriteBatch.DrawString(font, (myItem == i ? ">" : "") + (i + 1).ToString().PadLeft(names.Count / 10, '0') + "." + names[i], new Vector2(x, y + (i * 100)), Color.White);
                spriteBatch.DrawString(font, scoreTxt, new Vector2(x2 - (int)font.MeasureString(scoreTxt).X, y + (i * 100)), Color.White);
#endif
            }

            spriteBatch.Draw(fade, new Rectangle(0, h - (fade.Height >> 1), w, fade.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
            spriteBatch.Draw(fade, new Rectangle(0, highScoresLogo.Height, w, fade.Height), Color.White);

            spriteBatch.End();
        }

        #endregion
    }
}

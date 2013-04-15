//Map.cs
//Copyright PicWin Studios 2010

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace YoureAllDiseased
{
    #region AnimTile class

    /// <summary>
    /// An animated map tile
    /// </summary>
    public class AnimTile
    {
        /// <summary>
        /// Refrence map tile index (for map array)
        /// </summary>
        public int baseIdx;
        /// <summary>
        /// Frame count
        /// </summary>
        public int frames;
        /// <summary>
        /// current frame
        /// </summary>
        public int curFrame;
        /// <summary>
        /// Offset X on tileset image
        /// </summary>
        public int offX;
        /// <summary>
        /// Offset Y
        /// </summary>
        public int offY;

        /// <summary>
        /// Create a new animated tile
        /// </summary>
        /// <param name="BaseIndex">Reference index for map array</param>
        /// <param name="Frames">Frame count</param>
        /// <param name="OffsetX">Offset X on tileset image</param>
        /// <param name="offsetY">Offset Y on tileset image</param>
        public AnimTile(int BaseIndex, int Frames, int OffsetX, int offsetY)
        {
            baseIdx = BaseIndex;
            frames = Frames;
            curFrame = 0;
            offX = OffsetX;
            offY = offsetY;
        }
    }

    #endregion


    /// <summary>
    /// A map
    /// </summary>
    public class Map
    {
        #region Data
        /// <summary>
        /// the name of the map
        /// </summary>
        public string filename;

        /// <summary>
        /// The indivdual tiles (y,x)
        /// </summary>
        public int[,] tiles;

        /// <summary>
        /// Is the map loaded?
        /// </summary>
        public bool loaded { get; private set; }

        /// <summary>
        /// the tileset image for all the tiles
        /// 
        /// first row is all solid block tiles
        /// second row is all 1 way blocks (can jump up on to, but not down)
        /// third row is all 45deg slope tiles
        /// forth row is all isometric slope tiles
        /// other rows are everything else
        /// </summary>
        public Texture2D tileset;
        /// <summary>
        /// The name of the tileset (no file extension or path, as listed in map file)
        /// </summary>
        public string tilesetName;
        /// <summary>
        /// Width of each tile
        /// </summary>
        public int tileWidth;
        /// <summary>
        /// Height of each tile
        /// </summary>
        public int tileHeight;

        /// <summary>
        /// Width of map (in tiles)
        /// </summary>
        public int width;
        /// <summary>
        /// Height of map (in tiles)
        /// </summary>
        public int height;

        /// <summary>
        /// The background image (null for none)
        /// </summary>
        public Texture2D background;
        /// <summary>
        /// A solid background color is used if no background file is specified
        /// </summary>
        public Color backgroundColor = Color.Black;
        /// <summary>
        /// The name of the background (as it appears in file)
        /// </summary>
        public string backgroundName;

        /// <summary>
        /// The last updated frame for animated tiles
        /// </summary>
        long lastUpdate;
        /// <summary>
        /// speed, in ms, of the tile animations (default is 50)
        /// </summary>
        int animSpeed = 50;

        /// <summary>
        /// The animated tiles
        /// </summary>
        public System.Collections.Generic.List<AnimTile> animTiles;

        public System.Collections.Generic.List<string> entData;

        #endregion


        #region Initialization

        /// <summary>
        /// Create a new map
        /// </summary>
        public Map()
        {
            filename = "";
            loaded = false;
            tiles = new int[0, 0];
            tileset = null;
            tilesetName = "null";
            tileWidth = 0;
            tileHeight = 0;
            width = 0;
            height = 0;
            lastUpdate = 0;
            animTiles = new System.Collections.Generic.List<AnimTile>(1);
            background = null;
            backgroundName = "null";
            entData = new System.Collections.Generic.List<string>(1);
        }

        /// <summary>
        /// Load a map from a .2m (2D map) file
        /// </summary>
        /// <param name="file">The map file (no path)</param>
        /// <param name="content">The content manager for loading entities and map tiles</param>
        public Map(string file, GraphicsDevice gd)
        {
            loaded = false;
            filename = file;

            StreamReader reader;
            string line;
            int cLine = 0;
            int t1 = 0, t2 = 0, t3 = 0; //temp vars

            animTiles = new System.Collections.Generic.List<AnimTile>(64);
            entData = new System.Collections.Generic.List<string>(64);

            try
            {

#if XNA31
#if ZUNE
            reader = new StreamReader(File.OpenRead(
                Microsoft.Xna.Framework.Storage.StorageContainer.TitleLocation + "\\Content\\Levels\\" + file));
#else
            reader = new StreamReader("Content/Levels/" + file);
#endif
#else
                reader = new StreamReader(file);
#endif
                //read the map
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (t3 != 0 || line.Length < 1 || line[0] == '`' || line[0] < 33) //if comment or space/newline/etc., ignore
                        continue;

                    else if (line[0] == '+') //anim tiles
                    {
                        string[] split = line.Substring(1).Split(',');
                        int idx, frames, x, y;

                        idx = int.Parse(split[0]);
                        frames = int.Parse(split[1]);
                        x = int.Parse(split[2]);
                        y = int.Parse(split[3]);

                        animTiles.Add(new AnimTile(idx, frames, x, y));
                    }

                    else if (line[0] == '!') //entities
                    {
                        entData.Add(line);
                    }

                    //specific lines
                    else if (cLine == 0) //wid&hgt
                    {
                        string[] split = line.Split(',');
                        width = int.Parse(split[0]);
                        height = int.Parse(split[1]);
                        tiles = new int[height, width];
                    }

                    else if (cLine == 1) //tileset,tile w&h
                    {
                        string[] split = line.Split(',');
                        string s = Path.GetDirectoryName(file) + "\\" + split[0] + ".png";
                        tilesetName = split[0];
                        StreamReader sr = new StreamReader(s);
                        tileset = Texture2D.FromStream(gd, sr.BaseStream);
                        sr.Close();
                        tileWidth = int.Parse(split[1]);
                        tileHeight = int.Parse(split[2]);
                    }

                    else if (cLine == 2) //backgrounds
                    {
                        if (line[0] == '#') //color
                        {
                            int c = int.Parse(line.Substring(1), System.Globalization.NumberStyles.HexNumber);
                            backgroundColor = new Color((c >> 16) % 256, (c >> 8) % 256, c % 256);
                        }

                        else if (line != "null")
                        {
                            string s = Path.GetDirectoryName(file) + "\\" + line + ".png";
                            StreamReader sr = new StreamReader(s);
                            background = Texture2D.FromStream(gd, sr.BaseStream);
                            sr.Close();
                        }
                        backgroundName = line;
                    }

                    else //map tiles
                    {
                        if (t1 > 0) //1 layer
                            continue;

                        string[] split = line.Split(',');
                        for (int i = 0; i < (split.Length < width ? split.Length : width); i++)
                            tiles[t2, i] = int.Parse(split[i]);
                        t2++;
                        if (t2 >= height)
                        {
                            t2 = 0;
                            t1++;
                            if (t1 > 1)
                                continue; //done
                        }
                    }

                    cLine++;

                }

                reader.Close();
            }
            catch (Exception expt)
            {
                Console.WriteLine(expt.ToString());
                return;
            }

            loaded = true;
        }

        #endregion


        #region Draw

        /// <summary>
        /// Draw the map
        /// </summary>
        /// <param name="sb">The reference sprite batch to draw the tiles (does not call begin/end)</param>
        /// <param name="viewport">The camera viewport for darawing (abs xy coords on map)</param>
        public void Draw(ref SpriteBatch sb, Rectangle viewport)
        {
            if (!loaded) //don't draw what isn't there
                return;

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (background != null)
                sb.Draw(background, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            else
                sb.GraphicsDevice.Clear(backgroundColor);

            //start pos & visable # of tiles
            int sX = viewport.X / tileWidth, sY = viewport.Y / tileHeight, w = viewport.Width / tileWidth + 1, h = viewport.Height / tileHeight + 1;
            Vector2 mapOffset = new Vector2(0 - viewport.X, 0 - viewport.Y);

            for (int x = sX; x <= sX + w; x++)
            {
                if (x < 0 || x >= tiles.GetLength(1))
                    continue; //out of bounds

                for (int y = sY; y <= sY + h; y++)
                {
                    if (y < 0 || y >= tiles.GetLength(0))
                        continue; //out of bounds

                    int wid = tileset.Width / tileWidth;

                    int tile = tiles[y, x] - 1; //fg
                    if (tile > 0)
                    {
                        bool anim = false;
                        for (int i = 0; i < animTiles.Count; i++)
                        {
                            int f = animTiles[i].curFrame;

                            if (animTiles[i].baseIdx == tile)
                            {
                                sb.Draw(tileset, new Vector2(x * tileWidth, y * tileHeight) + mapOffset, new Rectangle(
                                    (animTiles[i].offX + f) * tileWidth, animTiles[i].offY * tileHeight, tileWidth, tileHeight), Color.White);
                                anim = true;
                            }
                        }
                        if (!anim)
                            sb.Draw(tileset, new Vector2(x * tileWidth, y * tileHeight) + mapOffset, new Rectangle(
                                (tile % wid) * tileWidth, (tile / wid) * tileHeight, tileWidth, tileHeight), Color.White);
                    }
                }
            }

            //update animation
            if ((DateTime.UtcNow.Ticks - lastUpdate) / 10000 > animSpeed)
            {
                lastUpdate = DateTime.UtcNow.Ticks;
                foreach (AnimTile t in animTiles)
                    t.curFrame = (t.curFrame + 1) % t.frames;
            }

            sb.End();
        }

        #endregion
    }
}
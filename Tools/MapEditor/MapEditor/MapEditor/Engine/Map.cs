//Map.cs
//Copyright Dejitaru Forge 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace MapEditor
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
        /// have tiles been added to the map (for file io purposes)
        /// </summary>
        public bool hasAddedTiles = false;

        /// <summary>
        /// All of the collision lines
        /// </summary>
        public System.Collections.Generic.List<Vector2> lines = new System.Collections.Generic.List<Vector2>(256);

        /// <summary>
        /// The position of the view of the map
        /// </summary>
        public Vector2 cViewPos = Vector2.Zero; //current view position

        /// <summary>
        /// Is the map loaded?
        /// </summary>
        public bool loaded;

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

        public System.Collections.Generic.List<Mucosa> mucus;

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
        /// The image for mucus
        /// </summary>
        public Texture2D mucusImg;

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

        /// <summary>
        /// All of the entities for this map
        /// </summary>
        public System.Collections.Generic.List<Entity> ents;

        /// <summary>
        /// All of the types of ents
        /// </summary>
        public System.Collections.Generic.List<EntType> entTypes;

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
            tileWidth = 32;
            tileHeight = 32;
            width = 0;
            height = 0;
            lastUpdate = 0;
            animTiles = new System.Collections.Generic.List<AnimTile>(1);
            mucusImg = null;
            background = null;
            backgroundName = "null";
            ents = new System.Collections.Generic.List<Entity>(1);
            entTypes = new System.Collections.Generic.List<EntType>(1);
            mucus = new System.Collections.Generic.List<Mucosa>(1);
        }

        /// <summary>
        /// Load a map from a .2m (2D map) file
        /// </summary>
        /// <param name="file">The map file (no path)</param>
        /// <param name="g">Game</param>
        public Map(string file, Game g)
        {
            GraphicsDevice gd = g.GraphicsDevice;

            loaded = false;
            filename = file;

            StreamReader reader = null;
            string line;
            int cLine = 0;
            int t1 = 0, t2 = 0, t3 = 0; //temp vars

            animTiles = new System.Collections.Generic.List<AnimTile>(64);
            ents = new System.Collections.Generic.List<Entity>(64);
            entTypes = new System.Collections.Generic.List<EntType>(32);
            mucus = new System.Collections.Generic.List<Mucosa>(256);

            mucusImg = g.Content.Load<Texture2D>("Mucus");

            try
            {
                reader = new StreamReader(file);

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
                        string[] split = line.Substring(1).Split(',');

                        System.Collections.Generic.List<string> opts = new System.Collections.Generic.List<string>(5); //options
                        for (int i = 4; i < split.Length; i++) //add options to ent
                            opts.Add(split[i]);

                        ents.Add(new Entity(int.Parse(split[0]), new Vector2(int.Parse(split[1]), int.Parse(split[2])), MathHelper.ToRadians(int.Parse(split[3])), opts));
                    }

                    else if (line[0] == '%') //mucus
                    {
                        string[] split = line.Substring(1).Split(',');
                        mucus.Add(new Mucosa(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2])));
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
                        tilesetName = split[0];
                        tileWidth = int.Parse(split[1]);
                        tileHeight = int.Parse(split[2]);

                        string s = Path.GetDirectoryName(file) + "\\" + split[0] + ".png";
                        StreamReader sr = new StreamReader(s);
                        tileset = Texture2D.FromStream(gd, sr.BaseStream);
                        sr.Close();
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
                        if (line[0] == '.') //collision
                        {
                            string[] split = line.Substring(1).Split(',');
                            lines.Add(new Vector2(int.Parse(split[0]), int.Parse(split[1])));
                        }
                        else
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
                    }

                    cLine++;

                }
                reader.Close();

                reader = new StreamReader(Path.GetDirectoryName(file) + "\\elst");

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    //comment or not enough data
                    if (line != "" && line[0] == '`')
                        continue;

                    string[] toks = line.Split(',');
                    if (toks.Length < 3) //not enough data
                        continue;

                    //load image
                    Texture2D img = null;

                    string s = Path.GetDirectoryName(file) + "\\" + toks[2];
                    StreamReader sr = new StreamReader(s);
                    img = Texture2D.FromStream(gd, sr.BaseStream);


                    entTypes.Add(new EntType(int.Parse(toks[0]), toks[1], img));
                }

                reader.Close();
            }
            catch (Exception expt)
            {
                System.Windows.Forms.MessageBox.Show(expt.ToString(), "Error!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                reader.Close();
                return;
            }

            cViewPos = Vector2.Zero;

            //center map if smaller than window size
            if (width * tileWidth < gd.Viewport.Width)
                cViewPos.X = -(gd.Viewport.Width >> 1) + ((width * tileWidth) >> 1);
            if (height * tileHeight < gd.Viewport.Height)
                cViewPos.Y = -(gd.Viewport.Height >> 1) + ((height * tileHeight) >> 1);

            loaded = true;
        }

        #endregion


        #region Other

        /// <summary>
        /// Save the map
        /// </summary>
        /// <param name="file">name of the map</param>
        public void Save(string file)
        {
            System.IO.StreamWriter wrt = new System.IO.StreamWriter(file);

            wrt.WriteLine(width + "," + height + "," + lines.Count);
            wrt.WriteLine(tilesetName + "," + tileWidth + "," + tileHeight);
            if (backgroundName != "" && backgroundName != "null")
                wrt.WriteLine(backgroundName);
            else
            {
                int c = (backgroundColor.R << 16) + (backgroundColor.G << 8) + backgroundColor.B;
                wrt.WriteLine("#" + c.ToString("x"));
            }

            if (loaded)
            {
                //start writing map data
                string line = "";
                for (int y = 0; y < height; y++) //y
                {
                    for (int x = 0; x < width; x++) //x
                    {
                        line += tiles[y, x] + ",";
                    }
                    wrt.WriteLine(line);
                    line = "";
                }
            }

            for (int i = 0; i < lines.Count; i++)
                wrt.WriteLine("." + (int)lines[i].X + "," + (int)lines[i].Y);

            //write animated tiles
            for (int i = 0; i < animTiles.Count; i++)
                wrt.WriteLine("+" + animTiles[i].baseIdx + "," + animTiles[i].frames + "," +
                    animTiles[i].offY + "," + animTiles[i].offY);

            //write mucus
            for (int i = 0; i < mucus.Count; i++)
                wrt.WriteLine("%" + mucus[i].x + "," + mucus[i].y + "," + mucus[i].sz);

            //write entities
            for (int i = 0; i < ents.Count; i++)
            {
                Entity e = ents[i];

                int a = (int)Microsoft.Xna.Framework.MathHelper.ToDegrees(e.angle) % 360;

                string s = "!" + e.id + "," + (int)e.position.X + "," + (int)e.position.Y + "," + a;
                if (e.other != null)
                    for (int j = 0; j < e.other.Count; j++)
                        s += "," + e.other[j];

                wrt.WriteLine(s);
            }

            wrt.Close();
        }

        /// <summary>
        /// Has this map been modified?
        /// </summary>
        /// <returns>True if edited (unsaved)</returns>
        public bool Edited()
        {
            bool edited = false;

            if (lines.Count > 0)
                edited = true;

            if (hasAddedTiles)
                edited = true;

            if (mucus.Count > 0)
                edited = true;

            if (ents.Count > 0)
                edited = true;

            return edited;
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
            {
                for (int x = 0; x <= viewport.Width / background.Width; x++)
                    for (int y = 0; y <= viewport.Height / background.Height; y++)
                        sb.Draw(background, new Vector2(x * background.Width, y * background.Height), Color.White);
            }
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

                    int tile = tiles[y, x]; //fg
                    if (tile > 0)
                    {
                        tile -= 1;

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

            //draw ents
            for (int i = 0; i < ents.Count; i++)
            {
                EntType eT = EntType.GetTypeInfo(entTypes, ents[i].id);
                Vector2 pos = ents[i].position - cViewPos;
                sb.Draw(eT.sprite, new Rectangle((int)pos.X - (eT.w >> 1), (int)pos.Y - (eT.h >> 1), eT.w, eT.h), null, Color.White, ents[i].angle, new Vector2(eT.w >> 1, eT.h >> 1), SpriteEffects.None, 0);
            }

            //draw mucus
            for (int i = 0; i < mucus.Count; i++)
            {
                int sz = (int)Math.Pow(2, 2 + mucus[i].sz);
                int szo2 = sz >> 1;
                if (!viewport.Intersects(new Rectangle((int)mucus[i].x - szo2, (int)mucus[i].y - szo2, sz, sz)))
                    continue;

                sb.Draw(mucusImg, new Vector2(mucus[i].x - (szo2), mucus[i].y - (szo2)) - cViewPos, new Rectangle(sz - 8, 0, sz, sz), Color.White);
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
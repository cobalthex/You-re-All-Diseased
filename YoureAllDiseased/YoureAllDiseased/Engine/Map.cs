//Map.cs
//Copyright PicWin Studios 2011

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace YoureAllDiseased
{
    /// <summary>
    /// A map
    /// </summary>
    public class Map
    {
        #region Data

        /// <summary>
        /// The indivdual tiles (y,x)
        /// </summary>
        public int[,] tiles;

        /// <summary>
        /// All of the verticies that make up the collision mesh
        /// </summary>
        public Vector2[] points;

        /// <summary>
        /// Is the map loaded?
        /// </summary>
        public bool loaded { get; private set; }

        /// <summary>
        /// the tileset image for all the tiles
        /// </summary>
        public Texture2D tileset;
        /// <summary>
        /// transparency mask for tileset
        /// </summary>
        public Color[] tilesetMask;

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
        /// The background color (if no background image is supplied)
        /// </summary>
        public Color backgroundColor = Color.Black;

        /// <summary>
        /// All of the entities on the map
        /// </summary>
        public System.Collections.Generic.List<Entity> ents;

        /// <summary>
        /// mucus
        /// </summary>
        public System.Collections.Generic.List<Entities.Powerups.Weapons.Mucus> mucus;

        /// <summary>
        /// The size of the map (in tiles) -- for convience use
        /// </summary>
        public readonly Rectangle size;

        #endregion


        #region Initialization

        /// <summary>
        /// Create a new map
        /// </summary>
        public Map()
        {
            loaded = false;
            tiles = new int[0, 0];
            tileset = null;
            tileWidth = 0;
            tileHeight = 0;
            width = 0;
            height = 0;
            ents = new System.Collections.Generic.List<Entity>();
            mucus = new System.Collections.Generic.List<Entities.Powerups.Weapons.Mucus>();
            points = new Vector2[3];
            background = null;
        }

        /// <summary>
        /// Load a map from a .2mcp (2D map w/ collision data) file
        /// </summary>
        /// <param name="file">The map file (no path)</param>
        /// <param name="content">The content manager for loading entities and map tiles</param>
        public Map(string file, ContentManager content)
        {
            loaded = false;

            StreamReader reader = null;
            string line;
            int cLine = 0;
            int t1 = 0, t2 = 0, t3 = 0; //temp vars

            ents = new System.Collections.Generic.List<Entity>();
            mucus = new System.Collections.Generic.List<Entities.Powerups.Weapons.Mucus>(256);

            try
            {
#if XNA31
                reader = new StreamReader(Microsoft.Xna.Framework.Storage.StorageContainer.TitleLocation + "\\Content\\Maps\\" + file); 
#else
                reader = new StreamReader(TitleContainer.OpenStream("Content/Maps/" + file));
#endif
            }
            catch (Exception expt)
            {
                if (reader != null)
                    reader.Close();
                Console.WriteLine(expt.Message);
                loaded = false;
                return;
            }

            //read the map
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();

                if (line.Length < 1 || line[0] == '`' || line[0] < 33) //if comment or space/newline/etc., ignore
                    continue;

                else if (line[0] == '!') //entities
                {
                    string[] split = line.Substring(1).Split(',');
                    int id, x, y;
                    float angle;

                    id = int.Parse(split[0]);
                    x = int.Parse(split[1]);
                    y = int.Parse(split[2]);
                    angle = MathHelper.ToRadians(int.Parse(split[3]));

                    Entity e = LoadEnt(id, ref content, split);
                    if (e == null)
                        continue;

                    e.angle = angle;
                    e.position = new Vector2(x - (e.sprite.frameSize.Width >> 1), y - (e.sprite.frameSize.Height >> 1)); //use x&y as 0 coords not origin

                    if (e.GetType() == typeof(Entities.Player))
                        ents.Insert(0, e);
                    else
                        ents.Add(e);
                }

                else if (line[0] == '%') //mucus
                {
                    string[] split = line.Substring(1).Split(',');
                    mucus.Add(new Entities.Powerups.Weapons.Mucus(int.Parse(split[0]), int.Parse(split[1]), 0, 0, int.Parse(split[2]), System.DateTime.UtcNow.Ticks));
                }

                //specific lines
                else if (cLine == 0) //wid,hgt,&# of collision points
                {
                    string[] split = line.Split(',');
                    width = int.Parse(split[0]);
                    height = int.Parse(split[1]);
                    int points = int.Parse(split[2]);
                    this.points = new Vector2[points];
                    tiles = new int[height, width];
                }

                else if (cLine == 1) //tileset,tile w&h
                {
                    string[] split = line.Split(',');
                   
                    tileWidth = int.Parse(split[1]);
                    tileHeight = int.Parse(split[2]);

                    if (split[0] != "null")
                    {
                        tileset = content.Load<Texture2D>("Graphics/Tilesets/" + split[0]);

                        tilesetMask = new Color[tileset.Width * tileset.Height];
                        //tileset.GetData<Color>(tilesetMask);
                        content.Load<Texture2D>("Graphics/Tilesets/" + split[0] + "mask").GetData<Color>(tilesetMask);
                    }
                }

                else if (cLine == 2) //backgrounds
                {
                    if (line[0] == '#') //color
                    {
                        int c = int.Parse(line.Substring(1), System.Globalization.NumberStyles.HexNumber);
#if XNA31
                        backgroundColor = new Color((byte)((c >> 16) % 256), (byte)((c >> 8) % 256), (byte)(c % 256));
#else
                        backgroundColor = new Color((c >> 16) % 256, (c >> 8) % 256, c % 256);
#endif
                        background = content.Load<Texture2D>("Graphics/Tilesets/Background");
                    }
                    else if (line != "null")
                        background = content.Load<Texture2D>("Graphics/Tilesets/" + line);
                }

                else //map tiles
                {
                    if (t1 > 1) //3 layers: 1 collision, 2 tiles
                        continue;

                    if (line[0] == '.') //collision
                    {
                        string[] split = line.Substring(1).Split(',');
                        points[t3++] = new Vector2(int.Parse(split[0]), int.Parse(split[1]));
                    }
                    else
                    { //tile maps
                        string[] split = line.Split(',');
                        for (int i = 0; i < (split.Length < width ? split.Length : width); i++) //move until the smaller of the two: line length or width of map)
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

            GC.Collect(); //free unused stuff

            loaded = true;
        }

        /// <summary>
        /// The index of the entity to load (simply loads, does not set position or angle, etc.)
        /// </summary>
        /// <param name="id">ID of the entity to load</param>
        /// <param name="content">content loader reference</param>
        /// <param name="args">any arguments passed to this entity</param>
        /// <param name="loader">Used to use any special variables</param>
        /// <returns>The loaded ent</returns>
        public static Entity LoadEnt(int id, ref ContentManager content, string[] args)
        {
            Entity ent = null;

            //load ents
            if (id == 0) //player
                ent = new Entities.Player();
            else if (id == 1)
            {
                if (args != null && args.Length > 4)
                    ent = new Entities.Powerups.HealthPowerup(int.Parse(args[4]));
                else
                    ent = new Entities.Powerups.HealthPowerup(50);
            }
            else if (id == 2) //blood rift
            {
                if (args != null && args.Length > 4)
                    ent = new Entities.Powerups.BloodRiftPowerup(int.Parse(args[4]));
                else

                    ent = new Entities.Powerups.BloodRiftPowerup(2);
            }

            else if (id == 10) //spawner
            {
                int time = 0;
                if (args != null && args.Length > 4)
                    time = int.Parse(args[4]); //time for wating, 0 for wait until death

                System.Collections.Generic.List<Entity> spawns = new System.Collections.Generic.List<Entity>(4);
                for (int i = 5; i < args.Length; i++)
                    spawns.Add(LoadEnt(int.Parse(args[i]), ref content, null));

                if (time > 0)
                    ent = new Entities.Enemies.Spawner(spawns, time);
                else
                    ent = new Entities.Enemies.Spawner(spawns);
            }

            else if (id == 11) //Infected cell
                ent = new Entities.Enemies.InfectedCell();
            else if (id == 12) //polio
                ent = new Entities.Enemies.Polio();
            else if (id == 13) //influenza
                ent = new Entities.Enemies.Influenza();
            else if (id == 14) //Mutated cell
                ent = new Entities.Enemies.MutatedCell();
            else if (id == 15) //Decaying cell
                ent = new Entities.Enemies.DecayingCell();
            else if (id == 16) //Siphilis
                ent = new Entities.Enemies.Syphilis();
            else if (id == 17) //West Nile Virus
                ent = new Entities.Enemies.WestNile();
            else if (id == 18) //Chickenpox
                ent = new Entities.Enemies.Varicella();

            else if (id == 100) //Rhinovirus
                ent = new Entities.Enemies.Bosses.Rhinovirus();
            else if (id == 101) //Tuberculosis
                ent = new Entities.Enemies.Bosses.Tuberculosis();
            else if (id == 102) //Cancer
                ent = new Entities.Enemies.Bosses.Cancer();
            else if (id == 103) //Leprosy
                ent = new Entities.Enemies.Bosses.Leprosy();

            else if (id == 1000) //Granzyme pickup
            {
                if (args != null && args.Length > 4)
                    ent = new Entities.Powerups.Weapons.GranzymePickup(int.Parse(args[4]));
                else
                    ent = new Entities.Powerups.Weapons.GranzymePickup();
            }
            else if (id == 1001) //Duplicating Cell Pickup
            {
                if (args != null && args.Length > 4)
                    ent = new Entities.Powerups.Weapons.DupeCellPickup(int.Parse(args[4]));
                else
                    ent = new Entities.Powerups.Weapons.DupeCellPickup();
            }
            else if (id == 2000) //Mucus pickup
            {
                if (args != null && args.Length > 4)
                    ent = new Entities.Powerups.Weapons.MucusPickup(int.Parse(args[4]));
                else
                    ent = new Entities.Powerups.Weapons.MucusPickup();
            }
            else if (id == 10000) //Death Machine pickup
            {
                if (args != null && args.Length > 4)
                    ent = new Entities.Powerups.Weapons.DeathMachinePickup(int.Parse(args[4]));
                else
                    ent = new Entities.Powerups.Weapons.DeathMachinePickup();
            }
            else if (id == 100000) //Bonus Coin
                ent = new Entities.Powerups.BonusCoin();

            if (ent != null)
                ent.Load(ref content);

            return ent;
        }

        /// <summary>
        /// Reset map (only resets entities)
        /// </summary>
        /// <param name="file">map file</param>
        /// <param name="content">content manager for entity loading</param>
        public void Reset(string file, ref ContentManager content)
        {
            loaded = false;

            ents.Clear();


            StreamReader reader;
            string line;

            ents = new System.Collections.Generic.List<Entity>();

#if XNA31
#if ZUNE
            reader = new StreamReader(File.OpenRead(
                Microsoft.Xna.Framework.Storage.StorageContainer.TitleLocation + "\\Content\\Levels\\" + file));
#else
            reader = new StreamReader("Content/Levels/" + file);
#endif
#else
            reader = new StreamReader(TitleContainer.OpenStream("Content/Maps/" + file));
#endif
            //read the map
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();

                if (line.Length < 1 || line[0] == '`' || line[0] < 33) //if comment or space/newline/etc., ignore
                    continue;

                else if (line[0] == '!') //entities
                {
                    string[] split = line.Substring(1).Split(',');
                    int id, x, y;
                    float angle;

                    id = int.Parse(split[0]);
                    x = int.Parse(split[1]);
                    y = int.Parse(split[2]);
                    angle = (float)Math.PI * int.Parse(split[3]) / 180f;

                    Entity e = LoadEnt(id, ref content, split);
                    e.angle = angle;
                    e.position = new Vector2(x, y);
                    if (e.GetType() == typeof(Entities.Player))
                        ents.Insert(0, e);
                    else
                        ents.Add(e);
                }
            }

            reader.Close();

            loaded = true;
        }

        #endregion


        #region Draw

        /// <summary>
        /// Draw the map
        /// </summary>
        /// <param name="sb">The reference sprite batch to draw the tiles (does not call begin/end)</param>
        /// <param name="viewport">The camera viewport for darawing (abs xy coords on map)</param>
        public void Draw(SpriteBatch sb, Rectangle viewport)
        {
            if (!loaded) //don't draw what isn't there
                return;

            if (background != null)
            {
                //sb.Draw(background, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
                for (int x = 0; x <= viewport.Width / background.Width; x++)
                    for (int y = 0; y <= viewport.Height / background.Height; y++)
                        sb.Draw(background, new Vector2(x * background.Width, y * background.Height), Color.White);
            }
            else
                sb.GraphicsDevice.Clear(backgroundColor);

            //start pos & visable # of tiles
            if (tileWidth > 0 && tileHeight > 0) //make sure tiles exist
            {
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

                        int tile = tiles[y, x] - 1; //bg
                        if (tile >= 0)
                            sb.Draw(tileset, new Vector2(x * tileWidth, y * tileHeight) + mapOffset, new Rectangle((tile % wid) * tileWidth, (tile / wid) * tileHeight, tileWidth, tileHeight), Color.White);

                        tile = tiles[y, x] - 1; //fg
                    }
                }
            }
        }

        #endregion


        #region Other

        /// <summary>
        /// Check whether or not a point is inside a polygon
        /// </summary>
        /// <param name="position">position of object</param>
        /// <returns>true if inside, false if not</returns>
        public bool Inside(Vector2 position)
        {
            int x = (int)position.X / tileWidth;
            int y = (int)position.Y / tileHeight;

            if (position.X < 0 || x >= width || y >= height || position.Y < 0)
                return false;

            int tileIdx = tiles[y, x] - 1;
            int tilesPerRow = tileset.Width / tileWidth;

            if (tileIdx < 0) //empty
                return false;
            if (tileIdx < 4) //full square tile
                return true;

            x = tileIdx % tilesPerRow;
            y = tileIdx / tilesPerRow;
            x = (x * tileWidth) + ((int)position.X % tileWidth);
            y = (y * tileHeight) + ((int)position.Y % tileHeight);

            return tilesetMask[x + (y * tileset.Width)].R > 0;
        }

        #endregion
    }
}
//Entity.cs
//Copyright Dejitaru Forge 2011

namespace MapEditor
{
    public class EntType
    {
        /// <summary>
        /// unique id 
        /// </summary>
        public int uid;

        /// <summary>
        /// sprite
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.Texture2D sprite;

        /// <summary>
        /// width of sprite
        /// </summary>
        public readonly int w;
        /// <summary>
        /// height of sprite
        /// </summary>
        public readonly int h;

        /// <summary>
        /// name of ent
        /// </summary>
        public string name;

        public EntType()
        {
            uid = -1;
            sprite = null;
            w = 0;
            h = 0;
            name = "";
        }

        /// <summary>
        /// Create a new type of ent
        /// </summary>
        /// <param name="UID">unique ID</param>
        /// <param name="Name">name</param>
        /// <param name="Sprite">texture</param>
        public EntType(int UID, string Name, Microsoft.Xna.Framework.Graphics.Texture2D Sprite)
        {
            uid = UID;
            sprite = Sprite;
            w = Sprite.Width;
            h = Sprite.Height;
            name = Name;
        }

        /// <summary>
        /// Get the EntType based on an ID
        /// </summary>
        /// <param name="types">The types that exist</param>
        /// <param name="id">The ID to search for</param>
        /// <returns>null if not found, The type if found</returns>
        public static EntType GetTypeInfo(System.Collections.Generic.List<EntType> types, int id)
        {
            int idx = -1;
            for (int i = 0; i < types.Count; i++)
                if (types[i].uid == id)
                {
                    idx = i;
                    break;
                }

            if (idx < 0)
                return null;
            else
                return types[idx];
        }
    }

    /// <summary>
    /// A simple representation of an entity for the map editor
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// the entity's type 
        /// </summary>
        public int id;
        /// <summary>
        /// position in relation to the map
        /// </summary>
        public Microsoft.Xna.Framework.Vector2 position;
        /// <summary>
        /// angle facing (in radians)
        /// </summary>
        public float angle;

        /// <summary>
        /// other info about this sprite (1 per index)
        /// </summary>
        public System.Collections.Generic.List<string> other;

        public Entity()
        {
            id = -1;
            position = Microsoft.Xna.Framework.Vector2.Zero;
            angle = 0;
            other = null;
        }

        public Entity(int ID, Microsoft.Xna.Framework.Vector2 Position, float Angle, System.Collections.Generic.List<string> Other)
        {
            id = ID;
            position = Position;
            angle = Angle;
            other = Other;
        }

        public string ToString(System.Collections.Generic.List<EntType> types)
        {
            EntType info = EntType.GetTypeInfo(types, id);
            string s = "Type: " + info.name + " (id: " + info.uid + ")";
            s += "\nPosition: " + position.X + " , " + position.Y;
            s += "\nAngle: " + (int)(Microsoft.Xna.Framework.MathHelper.ToDegrees(angle) % 360) + "° , " + angle + " rads";
            s += "\nSize: " + info.w + " , " + info.h;

            return s;
        }
    }

    /// <summary>
    /// A single mucus blob (sz = 1-4)
    /// </summary>
    public struct Mucosa
    {
        public int x, y, sz;
        public Mucosa(int X, int Y, int Size) { x = X; y = Y; sz = Size; }
    }
}

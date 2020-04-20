public class MapObject
{
    public static MapObject[] Objects = new MapObject[1];

    public static int ID_Counter;

    public static MapObject Mushrooms   = new MapObject("Mushroom", ObjectType.plant, "red mushrooms", new int[] { Tile.Forest.id, Tile.Grass.id });
    public static MapObject Berries     = new MapObject("Berries",  ObjectType.plant, "lots of berries", new int[] { Tile.Forest.id, Tile.Grass.id });
    public static MapObject Apples      = new MapObject("Apples", ObjectType.plant, "apples growing on trees", new int[] { Tile.Forest.id, Tile.Grass.id });
    public static MapObject StrangeFlower = new MapObject("Strange flower", ObjectType.plant, "strange glowing flower", new int[] { Tile.Forest.id, Tile.Mountain.id });

    public static MapObject DeadForest = new MapObject("Dead forest", ObjectType.dead, "all plants seem to be dead", new int[] { Tile.Grass.id, Tile.Forest.id });
    public static MapObject DeepForest = new MapObject("Deep forest", ObjectType.nature, "plants grow everywhere", new int[] { Tile.Forest.id });
    public static MapObject Butterfly = new MapObject("Butterfly", ObjectType.nature, "air is full of butterflies", new int[] { Tile.Forest.id });
    public static MapObject LivingTree = new MapObject("Living tree", ObjectType.nature, "weird tree that looks to have a face", new int[] { Tile.Forest.id });
    public static MapObject ForestSing = new MapObject("ForestSing", ObjectType.nature, "feels like forest is singing with whispering voices", new int[] { Tile.Forest.id });

    public static MapObject Waterfall = new MapObject("Waterfall", ObjectType.nature, "large waterfall", new int[] { Tile.Forest.id, Tile.Mountain.id });
    public static MapObject LargeTree = new MapObject("Large tree", ObjectType.nature, "huge ancient tree", new int[] { Tile.Forest.id, Tile.Mountain.id });
    public static MapObject LargeStone = new MapObject("Large stone", ObjectType.nature, "huge boulder", new int[] {Tile.Grass.id, Tile.Forest.id, Tile.Mountain.id });
    public static MapObject Canyon = new MapObject("Canyon", ObjectType.nature, "deep canyon cuts through the land", new int[] { Tile.Grass.id, Tile.Mountain.id });
    public static MapObject Cave = new MapObject("Cave", ObjectType.tunnel, "deep cave in the ground", new int[] { Tile.Grass.id, Tile.Forest.id, Tile.Mountain.id });
    public static MapObject Hole = new MapObject("Hole", ObjectType.tunnel, "large hole in the ground", new int[] { Tile.Grass.id, Tile.Forest.id, Tile.Mountain.id });
    public static MapObject StoneCircle = new MapObject("Stone circle", ObjectType.nature, "weird stone circle", new int[] { Tile.Grass.id, Tile.Forest.id, Tile.Mountain.id });

    public static MapObject Golem = new MapObject("Golem", ObjectType.nature, "rocks look like living creatures", new int[] { Tile.Mountain.id });
    public static MapObject MountainTop = new MapObject("Mountain top", ObjectType.nature, "mountain top that rises to the clouds", new int[] {Tile.Mountain.id });
    public static MapObject Cliffs = new MapObject("Cliffs", ObjectType.nature, "mountain cliffs are hard to climp", new int[] { Tile.Mountain.id });
    public static MapObject Volcano = new MapObject("Volcano", ObjectType.nature, "red burning volcano", new int[] { Tile.Mountain.id });
    public static MapObject Metal = new MapObject("metal", ObjectType.nature, "veins of metal", new int[] { Tile.Mountain.id });

    public static MapObject Skulls = new MapObject("Skulls", ObjectType.dead, "lots of bones around");
    public static MapObject Holy = new MapObject("Holy", ObjectType.holy, "this area is holy");

    public static MapObject Rocks = new MapObject("Rocks", ObjectType.nature, "dangerous rocks just below water surface", new int[] { Tile.Water.id });
    public static MapObject TinyIsland = new MapObject("Tiny island", ObjectType.nature, "tiny island", new int[] { Tile.Water.id });
    public static MapObject WaterPlants = new MapObject("Water plants", ObjectType.nature, "plants growing on surface of water", new int[] { Tile.Water.id });
    public static MapObject Depth = new MapObject("Depth", ObjectType.nature, "large creatures are moving deep beneath the waves", new int[] { Tile.Water.id });

    public static MapObject Smell = new MapObject("Smell", ObjectType.nature, "there is weird smell in the air");
    public static MapObject Cold = new MapObject("Cold", ObjectType.nature, "air feels cold here");

    public readonly int ID;
    public string name;
    public ObjectType type;
    public string description;
    public int[] tiles;
    public int itemID;

    public MapObject(string name, ObjectType type, string description, int[] tiles = null, int itemID = -1)
    {
        this.ID = ID_Counter++;
        this.name = name;
        this.type = type;
        this.tiles = tiles;
        this.description = description;
        if (Objects.Length == ID)
        {
            MapObject[] newObjects = new MapObject[ID_Counter];
            for (int i = 0; i < Objects.Length; i++)
            {
                newObjects[i] = Objects[i];
            }
            Objects = newObjects;
        }
        Objects[ID] = this;
        this.itemID = itemID;
    }

    public bool Equals(MapObject mo)
    {
        return ID == mo.ID;
    }

    public bool IsType(ObjectType type)
    {
        return this.type == type;
    }

    public bool CanAppear(int tile)
    {
        if (tiles == null)
        {
            return true;
        }
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tile == tiles[i])
            {
                return true;
            }
        }
        return false;
    }

    public Item GetItem()
    {
        if (itemID == -1)
        {
            return null;
        }
        return Item.items[itemID];
    }
}

public enum ObjectType { plant, nature , tunnel, dead, holy }

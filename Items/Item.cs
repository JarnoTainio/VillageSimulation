public class Item
{
    public static Item[] items = new Item[1];
    public static byte ID;

    public static Item None = new Item("none", 0, new ItemType[] { });

    //=========================================================
    // MATERIALS
    //=========================================================

    public static Item Wood =       new Item("wood",    3, 
                                        new ItemType[] { ItemType.Material, ItemType.Nature });

    public static Item Stone =      new Item("stone",   4,
                                        new ItemType[] { ItemType.Material, ItemType.Stone });

    public static Item Ore =        new Item("ore",     5,
                                        new ItemType[] { ItemType.Material, ItemType.Stone });

    public static Item Metal =      new Item("metal",   8,
                                        new ItemType[] { ItemType.Material, ItemType.Stone });

    public static Item Flour =      new Item("flour",   4,
                                        new ItemType[] { ItemType.Material });

    //=========================================================
    // FOODS
    //=========================================================

    public static Food Fish =       new Food("fish",    3, 1f, Quality.ok,
                                        new ItemType[] { ItemType.Food, ItemType.Water });

    public static Food Meat =       new Food("meat",    3, 1f, Quality.ok,
                                        new ItemType[] { ItemType.Food });

    public static Food Bread =      new Food("bread",   5, 1.25f, Quality.good,
                                        new ItemType[] { ItemType.Food });

    public static Food Wheat =      new Food("wheat",   3, .75f, Quality.bad,
                                        new ItemType[] { ItemType.Food, ItemType.Process });

    public static Food Cheese =     new Food("cheese", 4, 1f, Quality.good,
                                        new ItemType[] { ItemType.Food });

    //public static Food Mushroom = new Food("mushrom", 1, 1f, Quality.good, new ItemType[] { ItemType.Food });

    //=========================================================
    // GOODS
    //=========================================================

    public static Item Ale =        new Item("ale",     3,
                                        new ItemType[] { ItemType.Treat });

    //=========================================================
    // TOOLS
    //=========================================================
    public static Item Tool =        new Item("tool", 15,
                                        new ItemType[] { ItemType.Tool });

    //=========================================================
    // POTIONS
    //=========================================================
    public static Item LifePotion = new Item("life potion", 50, new ItemType[] { ItemType.Potion_Heal, ItemType.Treasure });
    public static Item YouthPotion = new Item("youth elixir", 250, new ItemType[] { ItemType.Potion_Age, ItemType.Treasure });
    public static Item UndeadPotion = new Item("holy water", 100, new ItemType[] { ItemType.Kill, ItemType.Treasure, ItemType.Holy }, (int)MonsterTag.undead);

    //=========================================================
    // TREASURE
    //=========================================================

    public static Item ForestRelic = new Item("Forest seed", 200, new ItemType[] { ItemType.Tile, ItemType.Treasure, ItemType.Nature }, 1);
    public static Item SeaRealic = new Item("Water seed", 200, new ItemType[] { ItemType.Tile, ItemType.Treasure, ItemType.Water }, 2);
    public static Item MountainRelic = new Item("Mountain seed", 200, new ItemType[] { ItemType.Tile, ItemType.Treasure, ItemType.Stone }, 3);

    //public static Item Flower = new Item("glowing flower", 10, new ItemType[] { ItemType.Material });

    public readonly byte id;
    public readonly string name;
    public readonly ItemType[] types;
    public readonly int detail;
    public readonly int cost;
    
    public Item(string name, int cost, ItemType[] types, int detail = -1)
    {
        id = ID++;
        if (items.Length <= id)
        {
            Item[] newItems = new Item[ID];
            for (int i = 0; i < items.Length; i++) 
            {
                newItems[i] = items[i];
            }
            items = newItems;
        }
        items[id] = this;
        this.name = name;
        this.cost = cost;
        this.types = types;
        this.detail = detail;
    }

    public bool Equals(Item item)
    {
        if (item == null)
        {
            return false;
        }
        return id == item.id;
    }

    public override string ToString()
    {
        return name;
    }

    public bool IsType(ItemType type)
    {
        foreach(ItemType t in types)
        {
            if (t == type)
            {
                return true;
            }
        }
        return false;
    }
  
}

public enum ItemType { Food, Process, Tool, Material, Treat, Potion_Heal, Potion_Age, Kill, Tile, Treasure, Holy, Nature, Stone, Water };

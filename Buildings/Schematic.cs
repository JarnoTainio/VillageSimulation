using System.Collections;
using System.Collections.Generic;

public class Schematic
{
    public static byte ID;
    public static Schematic[] schematics = new Schematic[1];

    public static Schematic Construction = new Schematic(
        new Construction(null, new Point(), null, new SkillWork("Building", Skill.building, 300)),
        null,
        new Resource[] {

            }
        );

    public static Schematic House = new Schematic(
        new Building(null, new Point(), "House", new BuildingTag[] { BuildingTag.Bed }, null, 
            new MaintainWork("Repair", Skill.building, 60, 10, new ItemStack(Item.Wood))),
        Tile.Grass,
        new Resource[] {
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),

            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300)

            }
        );

    public static Schematic Farm = new Schematic(
        new Building(null, new Point(), "Field", new BuildingTag[] { }, new SkillWork("Farming", Skill.farming, 90, new ItemStack(Item.Wheat), null, 20, 25), 
            new MaintainWork("Blow", Skill.farming, 30, 5)),
        Tile.Grass,
        new Resource[] {
            new Resource(Item.Wood, 600),
            new Resource(Item.Wood, 600),
            new Resource(Item.Wood, 600)
        }
    );

    public static Schematic Boat = new Schematic(
        new Building(null, new Point(), "Boat", new BuildingTag[] { }, new SkillWork("Boat", Skill.fishing, 120, new ItemStack(Item.Fish), null, 30, 30, false), 
            new MaintainWork("Repair", Skill.building, 60, 10, new ItemStack(Item.Wood))),
        Tile.Water,
        new Resource[] {
            new Resource(Item.Wood, 420),
            new Resource(Item.Wood, 420),
            new Resource(Item.Wood, 420),
            new Resource(Item.Wood, 420),
            new Resource(Item.Wood, 420)
        }
    );

    public static Schematic Windmill = new Schematic(
        new Building(null, new Point(), "Windmill", new BuildingTag[] { }, new SkillWork("Windmill", Skill.farming, 45, new ItemStack(Item.Flour), new ItemStack(Item.Wheat), 20, 20, false),
            new MaintainWork("Repair", Skill.building, 60, 10, new ItemStack(Item.Wood))),
        Tile.Grass,
        new Resource[] {
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),

            new Resource(Item.Stone, 600),
            new Resource(Item.Stone, 600),
            new Resource(Item.Stone, 600),
            new Resource(Item.Stone, 600),
            new Resource(Item.Stone, 600),
        }
    );

    public static Schematic Saw = new Schematic(
    new Building(null, new Point(), "Saw", new BuildingTag[] { }, new SkillWork("Sawing", Skill.woodcutting, 120, new ItemStack(Item.Wood), null, 25, 20, false),
        new MaintainWork("Repair", Skill.building, 60, 10, new ItemStack(Item.Stone))),
    Tile.Forest,
    new Resource[] {
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),

                new Resource(Item.Stone, 600),
                new Resource(Item.Stone, 600)
        }
    );


    public static Schematic Bakery = new Schematic(
        new Building(null, new Point(), "Bakery", new BuildingTag[] { }, new SkillWork("Baking", Skill.baking, 30, new ItemStack(Item.Bread), new ItemStack(Item.Flour), 25, 20, false), 
            new MaintainWork("Refill", Skill.baking, 15, 5, new ItemStack(Item.Wood))),
        Tile.Grass,
        new Resource[] {
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),

            new Resource(Item.Stone, 600),
            new Resource(Item.Stone, 600),
            new Resource(Item.Stone, 600)
        }
    );

    public static Schematic Brewery = new Schematic(
        new Building(null, new Point(), "Brewery", new BuildingTag[] {  }, new SkillWork("Brewing", Skill.brewing, 30, new ItemStack(Item.Ale), new ItemStack(Item.Wheat), 25, 20, false), 
            new MaintainWork("Repair", Skill.building, 60, 10, new ItemStack(Item.Wood))),
        Tile.Grass,
        new Resource[] {
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),
            new Resource(Item.Wood, 300),

            new Resource(Item.Stone, 600),
            new Resource(Item.Stone, 600)
        }
    );

    public static Schematic Quarry = new Schematic(
        new Building(null, new Point(), "Quarry", new BuildingTag[] { }, new SkillWork("Stone cutting", Skill.mining, 120, new ItemStack(Item.Stone), null, 25, 20, false),
            new MaintainWork("Repair", Skill.building, 60, 10, new ItemStack(Item.Wood))),
        Tile.Mountain,
        new Resource[] {
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),

                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300)
        }
    );

    public static Schematic Mine = new Schematic(
    new Building(null, new Point(), "Mine", new BuildingTag[] { }, new SkillWork("Mining", Skill.mining, 120, new ItemStack(Item.Ore), null, 25, 40, false), 
        new MaintainWork("Expand", Skill.building, 60, 5, new ItemStack(Item.Wood))),
    Tile.Mountain,
    new Resource[] {
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),

                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300)
        }
    );

    public static Schematic Foundry = new Schematic(
    new Building(null, new Point(), "Foundry", new BuildingTag[] { }, new SkillWork("Smelting", Skill.metalwork, 120, new ItemStack(Item.Metal), new ItemStack(Item.Ore), 25, 20, false), 
        new MaintainWork("Refill", Skill.metalwork, 15, 5, new ItemStack(Item.Wood))),
    Tile.Grass,
    new Resource[] {
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),

                new Resource(Item.Stone, 600),
                new Resource(Item.Stone, 600),
                new Resource(Item.Stone, 600),
                new Resource(Item.Stone, 600),
                new Resource(Item.Stone, 600),
                new Resource(Item.Stone, 600)
        }
    );

    public static Schematic Smith = new Schematic(
    new Building(null, new Point(), "Smith", new BuildingTag[] { }, new SkillWork("Smithing", Skill.metalwork, 180, new ItemStack(Item.Tool), new ItemStack(Item.Metal), 30, 30, false), 
        new MaintainWork("Refill", Skill.metalwork, 10, 5, new ItemStack(Item.Wood))),
    Tile.Grass,
    new Resource[] {
                    new Resource(Item.Wood, 300),
                    new Resource(Item.Wood, 300),
                    new Resource(Item.Wood, 300),

                    new Resource(Item.Stone, 600),
                    new Resource(Item.Stone, 600),
                    new Resource(Item.Stone, 600),
                    new Resource(Item.Stone, 600),
                    new Resource(Item.Stone, 600),
        }
    );

    public static Schematic Animals = new Schematic(
    new Building(null, new Point(), "Farm", new BuildingTag[] { }, new SkillWork("Farm", Skill.animals, 60, new ItemStack(Item.Cheese), new ItemStack(Item.Wheat), 20, 20, true), 
        new MaintainWork("Repair", Skill.building, 60, 10, new ItemStack(Item.Wood))),
    Tile.Grass,
    new Resource[] {
                    new Resource(Item.Wood, 300),
                    new Resource(Item.Wood, 300),
                    new Resource(Item.Wood, 300),
                    new Resource(Item.Wood, 300),
                    new Resource(Item.Wood, 300),

                    new Resource(Item.Wood, 300),
                    new Resource(Item.Wood, 300),
                    new Resource(Item.Wood, 300)
        }
    );

    public static Schematic HuntingHut = new Schematic(
    new Building(null, new Point(), "Hunting hut", new BuildingTag[] { }, new SkillWork("Hunting", Skill.hunting, 180, new ItemStack(Item.Meat), null, 20, 30, true), 
        new MaintainWork("Restock", Skill.building, 60, 10, new ItemStack(Item.Wood))),
    Tile.Forest,
    new Resource[] {
                    new Resource(Item.Wood, 300),
                    new Resource(Item.Wood, 300),
                    new Resource(Item.Wood, 300),
                    new Resource(Item.Wood, 300),
                    new Resource(Item.Wood, 300)
        }
    );

    public static Schematic StoneHouse = new Schematic(
    new Building(null, new Point(), "House", new BuildingTag[] { BuildingTag.Bed }, null,
        new MaintainWork("Repair", Skill.building, 60, 10, new ItemStack(Item.Wood))),
    Tile.Grass,
    new Resource[] {
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),
                new Resource(Item.Wood, 300),

                new Resource(Item.Stone, 300),
                new Resource(Item.Stone, 300),
                new Resource(Item.Stone, 300),
                new Resource(Item.Stone, 300),
                new Resource(Item.Stone, 300)

        }
    );

    public static Schematic Ruins = new Schematic(
    new Building(null, new Point(), "Ruins", new BuildingTag[] {BuildingTag.Dungeon, BuildingTag.Spawner}, null, null),
    Tile.Grass,
    new Resource[] {}
    );

    public byte id;

    public Resource[] materials;
    public List<ItemStack> required;

    public Building building;

    public Tile requiredTile;
    public int workTime;

   public Schematic(Building building, Tile requiredTile, Resource[] materials)
   {
        id = ID++;
        if (schematics.Length == id)
        {
            Schematic[] newSchematics = new Schematic[ID];
            for (int i = 0; i < schematics.Length; i++)
            {
                newSchematics[i] = schematics[i];
            }
            schematics = newSchematics;
        }
        schematics[id] = this;
        this.building = building;
        this.building.id = id;
        this.requiredTile = requiredTile;
        this.materials = materials;

        required = new List<ItemStack>();

        foreach (Resource r in materials)
        {
            workTime += r.duration;
            bool found = false;
            foreach(ItemStack s in required)
            {
                if (s.Equals(r.item))
                {
                    s.count++;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                required.Add(new ItemStack(r.item, 1));
            }
        }
   }

}

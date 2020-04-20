using System.Collections.Generic;

public class Tile
{
    public static Tile[] tiles = new Tile[4];

    public static Tile Grass = new Tile(0, "Plains", 0, new SkillWork[] {
                                                            new SkillWork("Plains wood",    Skill.woodcutting, 240, new ItemStack(Item.Wood), null,  25, 35),
                                                            new SkillWork("Plains hunting", Skill.hunting,     180, new ItemStack(Item.Meat), null,  30, 50),
                                                            new SkillWork("Plains stone",   Skill.mining,      240, new ItemStack(Item.Stone), null, 20, 50)
    });

    public static Tile Forest = new Tile(1, "Forest", 5,new SkillWork[] {
                                                            new SkillWork("Forest wood",    Skill.woodcutting, 180, new ItemStack(Item.Wood), null,  20, 20),
                                                            new SkillWork("Forest hunting", Skill.hunting,     180, new ItemStack(Item.Meat), null,  25, 40),
                                                            new SkillWork("Forest stone",   Skill.mining,      240, new ItemStack(Item.Stone), null, 25, 50)
    });

    public static Tile Water = new Tile(2, "Lake", 1, new SkillWork[] {
                                                            new SkillWork("Water fishing", Skill.fishing,      120, new ItemStack(Item.Fish), null, 20, 50)
    });

    public static Tile Mountain = new Tile(3, "Mountain", 4, new SkillWork[] {
                                                            new SkillWork("Mountain wood",  Skill.woodcutting,  240, new ItemStack(Item.Wood), null,  30, 50),
                                                            new SkillWork("Mountain hunting",Skill.hunting,     180, new ItemStack(Item.Meat), null,  35, 70),
                                                            new SkillWork("Mountain stone", Skill.mining,       240, new ItemStack(Item.Stone), null, 25, 20)
    });

    public string name;
    public byte id;
    public SkillWork[] works;
    public int danger;

    public Tile(byte id, string name, int danger, SkillWork[] works)
    {
        tiles[id] = this;
        this.id = id;
        this.name = name;
        this.danger = danger;
        this.works = works;
    }

    public bool IsSource(Item item)
    {
        foreach (SkillWork sw in works)
        {
            if (sw.produced.Equals(item))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsSource(ItemType type)
    {
        foreach(SkillWork sw in works)
        {
            if (sw.produced.IsType(type))
            {
                return true;
            }
        }
        return false;
    }

    public SkillWork GetWork(Item item)
    {
        foreach (SkillWork sw in works)
        {
            if (sw.produced.Equals(item))
            {
                return sw;
            }
        }
        return null;
    }

    public SkillWork GetWork(ItemType type)
    {
        foreach (SkillWork sw in works)
        {
            if (sw.produced.IsType(type))
            {
                return sw;
            }
        }
        return null;
    }

    public override string ToString()
    {
        return name;
    }
}

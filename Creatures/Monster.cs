using System;
using System.Collections;
using System.Collections.Generic;

public class Monster
{
    public static int ID_Counter;
    public static Monster[] Monsters = new Monster[0];

    public static Monster Spider =  new Monster("Spider",       1, new MonsterTag[] { }, new Item[] { Item.Meat, null}, new Tile[] { });
    public static Monster Slime =   new Monster("Slime",        1, new MonsterTag[] { }, new Item[] { null }, new Tile[] { });
    public static Monster Skeleton =new Monster("Skeleton",     1, new MonsterTag[] { MonsterTag.undead }, new Item[] { null }, new Tile[] { });
    public static Monster Boulder = new Monster("Boulder",      2, new MonsterTag[] { }, new Item[] { Item.Stone, Item.Stone, Item.Ore, null }, new Tile[] { Tile.Mountain, Tile.Forest, Tile.Grass });
    public static Monster Treant =  new Monster("Treant",       2, new MonsterTag[] { MonsterTag.nature }, new Item[] { Item.Wood, Item.Wood, null }, new Tile[] { Tile.Forest });

    public static Monster SkeletonBoss = new Monster("Lich", 10, new MonsterTag[] {MonsterTag.boss, MonsterTag.undead }, new Item[] { }, new Tile[] { });

    public int id;
    public string name;
    public int strength;
    public MonsterTag[] tags;
    public Item[] loot;
    public Tile[] tiles;

    public Monster(string name, int strength, MonsterTag[] tags, Item[] loot, Tile[] tiles)
    {
        id = ID_Counter++;
        if (Monsters.Length <= id)
        {
            Monster[] newMonsters = new Monster[id + 1];
            for (int i = 0; i < Monsters.Length; i++)
            {
                newMonsters[i] = Monsters[i];
            }
            Monsters = newMonsters;
        }
        Monsters[id] = this;

        this.name = name;
        this.strength = strength;
        this.tags = tags;
        this.loot = loot;
        this.tiles = tiles;
    }

    public bool IsType(MonsterTag tag)
    {
        foreach(MonsterTag t in tags)
        {
            if (t == tag)
            {
                return true;
            }
        }
        return false;
    }

    public Item Kill(Random random)
    {
        if (loot.Length == 0)
        {
            return null;
        }

        Item item = loot[random.Next(loot.Length)];
        return item;
    }

    public bool Equals(Monster monster)
    {
        return monster.id == id;
    }

    public bool IsBoss()
    {
        for (int i = 0; i < tags.Length; i++) 
        {
            if (tags[i] == MonsterTag.boss){
                return true;
            }
        }
        return false;
    }

    public Monster GetMinion()
    {
        if (!IsType(MonsterTag.boss))
        {
            return this;
        }

        foreach(Monster monster in Monsters)
        {
            foreach(MonsterTag tag in monster.tags)
            {
                if (IsType(tag))
                {
                    return monster;
                }
            }
        }
        return null;
    }

    public Monster GetBoss()
    {
        if (IsType(MonsterTag.boss))
        {
            return this;
        }

        foreach (Monster monster in Monsters)
        {
            if (monster.IsType(MonsterTag.boss))
            {
                foreach (MonsterTag tag in monster.tags)
                {
                    if (IsType(tag))
                    {
                        return monster;
                    }
                }
            }
        }
        return null;
    }

    public List<Item> GetSlayingItem()
    {
        List<Item> killers = new List<Item>();
        foreach(Item item in Item.items)
        {
            if (item.IsType(ItemType.Kill) && IsType((MonsterTag)item.detail))
            {
                killers.Add(item);
            }
        }
        return killers;
    }
}

public enum MonsterTag { none, boss, undead, nature }

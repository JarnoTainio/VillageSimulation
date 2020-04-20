using System;
using System.Collections.Generic;

public class Location
{
    public static int MaxBuildings = 4;

    public Point point;
    public int tile;
    public Building building;
    public List<Creature> creatures;
    public ItemStackList items;

    public List<MapObject> objects;
    private Monster monster;
    private Building monsterSource;
    private Obstacle obstacle;

    public bool isActive;

    public Location(Point location, int tile)
    {
        this.point = location;
        this.tile = tile;
        creatures = new List<Creature>();
        items = new ItemStackList();
        objects = new List<MapObject>();
        isActive = false;
    }

    // Buildings
    public void Add(Building building)
    {
        this.building = building;
        this.building.Init();

        if (!isActive)
        {
            isActive = true;
        }

    }

    public void RemoveBuilding()
    {
        building.End();
        building = null;
    }

    // Creatures
    public void Add(Map map, Creature creature)
    {
        if (creatures == null)
        {
            creatures = new List<Creature>();
        }
        creatures.Add(creature);

        if (!isActive)
        {
            isActive = true;
        }
    }

    public void Remove(Creature creature)
    {
        creatures.Remove(creature);
    }

    // Items
    public void Add(ItemStack stack)
    {
        items.Add(stack);
    }

    public ItemStack Remove(ItemStack stack)
    {
        return items.Remove(stack);
    }

    public Tile GetTile()
    {
        return Tile.tiles[tile];
    }

    public List<Creature> GetCreatures()
    {
        return creatures;
    }

    public int ItemCount(Item item)
    {
        return items.Count(item);
    }

    private bool IsEmpty()
    {
        return building == null && creatures.Count == 0;
    }

    public void Add(MapObject mo)
    {
        objects.Add(mo);
    }

    public bool Add(Monster monster, Building source)
    {
        if (monster != null)
        {
            this.monster = monster;
            monsterSource = source;
            return true;
        }
        return false;
    }

    public Item Kill(Random random)
    {
        Item loot = monster.Kill(random);
        monster = null;
        monsterSource = null;
        return loot;
    }

    public Monster GetMonster()
    {
        return monster;
    }

    public Building GetSource()
    {
        return monsterSource;
    }

    public bool Contains(ObjectType type)
    {
        foreach(MapObject mo in objects)
        {
            if (mo.IsType(type))
            {
                return true;
            }
        }
        return false;
    }

    public bool Contains(MapObject mapObject)
    {
        foreach (MapObject mo in objects)
        {
            if (mo.ID == mapObject.ID)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanBePerformed(ActionType action)
    {
        switch (action)
        {
            case ActionType.move:
                return true;

            case ActionType.talk:
                return creatures.Count > 0;

            case ActionType.kill:
                return monster != null;

            case ActionType.explore:
                return true;
        }
        return false;
    }

    public bool HasMonster()
    {
        return monster != null;
    }

    public object UseItem(Map map, Player player, Item item)
    {
        if (obstacle != null && item.Equals(obstacle.key))
        {
            Obstacle obs = obstacle;
            obstacle = null;
            return obs;
        }

        // Player healing
        else if (item.IsType(ItemType.Potion_Heal))
        {
            if (player.life < player.maxLife)
            {
                player.ModifyLife(player.maxLife / 3);
                return "healed";
            }
        }

        // Undead killer
        else if (monster != null && item.IsType(ItemType.Kill) && monster.IsType((MonsterTag)item.detail))
        {
            // Using monster killing item counts as a kill
            map.storyManager.PlayerAction(new PlayerAction(ActionType.kill, point, monster), this);

            Item loot = map.KillMonster(player.random, point);
            if (loot == null)
            {
                return "no loot";
            }
            return loot;
        }

        else if (item.IsType(ItemType.Potion_Age))
        {
            if (player.selectedCreature != null)
            {
                (player.selectedCreature as Villager).oldAge *= 2;
                return "youth granted";
            }
        }

        else if (item.IsType(ItemType.Tile))
        {
            int t = item.detail;
            if (t != tile)
            {
                if (building != null)
                {
                    building.durability -= 50;
                }
                map.Replace(point, Tile.tiles[t]);
                return "tile changed";
            }
        }

        foreach(QuestStep quest in map.player.quests)
        {
            if (quest.CanUse(item, this))
            {
                // Close tunnel with stone
                if (item.Equals(Item.Stone))
                {
                    foreach(MapObject mo in objects)
                    {
                        if (mo.IsType(ObjectType.tunnel))
                        {
                            objects.Remove(mo);
                            break;
                        }
                    }
                }
                return quest;
            }
        }
        return null;
    }

    public Obstacle GetObstacle()
    {
        return obstacle;
    }

    public void Add(Obstacle obstacle)
    {
        if (this.obstacle == null)
        {
            this.obstacle = obstacle;
        }
    }

    public void CreateObstacle(Random random)
    {
        int roll = random.Next(100);
        if (roll < 10)
        {
            Add(new Obstacle(Item.Wood, "Overgrown"));
        }
        else if (roll < 20)
        {
            Add(new Obstacle(Item.Stone, "Rockwall"));
        }
        else if (roll < 30)
        {
            Add(new Obstacle(Item.Meat, "Blood altar"));
        }
        else if (roll < 40)
        {
            Add(new Obstacle(Item.Wheat, "Life altar"));
        }
        else if (roll < 50)
        {
            Add(new Obstacle(Item.Fish, "Sea altar"));
        }
        else if (roll < 60)
        {
            Add(new Obstacle(Item.YouthPotion, "Ancient guardian"));
        }
        else if (roll < 70)
        {
            Add(new Obstacle(Item.LifePotion, "Alchemical laboratory"));
        }
        else if (roll < 80)
        {
            Add(new Obstacle(Item.Metal, "Iron door"));
        }
        else if (roll < 90)
        {
            Add(new Obstacle(Item.UndeadPotion, "Unholy tomb"));
        }
        else
        {
            Add(new Obstacle(Item.Cheese, "Cow altar"));
        }
    }

    public override string ToString()
    {
        string str = "";
        str += "This is a " + Tile.tiles[tile].ToString();
        if (building != null)
        {
            str += " and there is " + building.DescriptiveString() + " here";
        }
        str += ".";
        if (creatures.Count > 0)
        {
            str += "\n";
            if (creatures.Count == 1)
            {
                str += "There is somebody here.";
                str += " " + creatures[0].name + " is " + creatures[0].currentEvent.ToString() + ".";
            }
            else
            {
                str += "There is " + creatures.Count + " people here:";
                foreach(Creature c in creatures)
                {
                    str += " " + c.name + " is " + c.currentEvent.ToString();
                }
                str += ".";
            }
        }

        for(int i = 0; i < objects.Count; i++)
        {
            if (i == 0)
            {
                str += "\nYou notice ";
            }
            else if (i == objects.Count - 1)
            {
                str += " and ";
            }
            else
            {
                str += ", ";
            }
            str += objects[i].description;
        }
        if (objects.Count > 0)
        {
            str += ".";
        }

        if (obstacle != null)
        {
            str += "\nThis place is blocked by " + obstacle.description + ", maybe " + obstacle.key + " would work?";
        }
        else if (items.list.Count > 0)
        {
            str += "\nIn the ground you notice ";
            str += items.ToString() + ".";
        }

        if (monster != null)
        {
            str += "\nThere is a " + monster.name + " monster here!";
        }

        return str;
    }
}

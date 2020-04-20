using System;
using System.Collections;
using System.Collections.Generic;

public class Map
{
    public static readonly int TileSize = 15;

    public int seed;

    public int width;
    public int height;

    public Location[] locations;
    public int[] adjacency;

    public List<Creature> creatures;
    public List<Creature> newCreatures;
    public List<Building> buildings;

    public StoryManager storyManager;
    public Player player;

    private bool debug = false;

    public List<Road> roads;

    public Map(int width, int height, int seed = -1)
    {
        new MapGenerator().GenerateMap(this, width, height, seed);
        roads = new List<Road>();
    }

    public int  UpdateAdjacency()
    {
        int calculations = 0;
        Point[] directions = new Point[] { Direction.North, Direction.East, Direction.South, Direction.West };
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                int tile = locations[x + y * width].tile;

                int adjacent = 0;
                int i = 1;
                foreach (Point p in directions)
                {
                    if (locations[x + p.x + (y + p.y) * width].tile == tile)
                    {
                        adjacent += i;
                    }
                    i *= 2;
                }
                adjacency[x + y * width] = adjacent;
                calculations++;

            }
        }
        return calculations;
    }

    List<Location> nonActive = new List<Location>();
    List<Location> newActive = new List<Location>();

    public void Tick(int delta, bool newDay)
    {
        if (newDay)
        {
            foreach(Road road in roads)
            {
                road.Tick();
            }
            foreach(Building b in buildings)
            {
                b.TickDay(this);
            }
            for(int i = 0; i < buildings.Count; i++)
            {
                if (!buildings[i].isActive)
                {
                    GetLocation(buildings[i].point).RemoveBuilding();
                    Dummy.instance.RemoveBuilding(buildings[i].point);
                    buildings.Remove(buildings[i]);
                    i--;
                }
            }
            List<Creature> remove = new List<Creature>();
            foreach(Creature c in creatures)
            {
                if (!c.alive)
                {
                    remove.Add(c);
                    GetLocation(c.location).Remove(c);
                    continue;
                }
                c.TickDay(this);
            }
            foreach(Creature c in remove)
            {
                creatures.Remove(c);
                Dummy.instance.Remove(c);
            }
        }
        foreach(Creature c in creatures)
        {
            c.Tick(this, delta);
        }
        foreach(Creature c in newCreatures)
        {
            creatures.Add(c);
        }
        newCreatures.Clear();

        if (player != null)
        {
            player.Tick(newDay);
            storyManager.Tick(newDay);
        }
    }

    public Location GetLocation(Point p)
    {
        return GetLocation(p.x, p.y);
    }

    public Location GetLocation(int x, int y)
    {
        return locations[x + y * height];
    }

    public void Event(Point point, Event e){


        if (e.result != EventResult.ongoing)
        {
            if (debug)
            {
                Dummy.PrintMessage("Event: " + e.ToString());
            }
        }
        foreach (Creature c in GetLocation(point).GetCreatures())
        {
            c.Observe(e);
            Point loc = c.location;
            foreach (Point d in Direction.Directions)
            {
                Point p = loc.Add(d);
                if (WithinBounds(p))
                {
                    foreach (Creature c2 in GetLocation(p).GetCreatures())
                    {
                        c2.Observe(e);
                    }
                }
            }
        }

    }

    // ToDo: Observe creature leaving location
    // ToDo: Observe action in location

    public void Move(Creature creature, Point from, Point to)
    {
        GetLocation(from).Remove(creature);
        GetLocation(to).Add(this, creature);

        // No roads in water
        if (GetLocation(from).tile == Tile.Water.id)
        {
            return;
        }

        // Check if road already exists
        bool found = false;
        foreach(Road r in roads)
        {
            if (r.Equals(from))
            {
                found = true;
                return;
            }
        }

        // Create new road
        if (!found)
        {
            roads.Add(new Road(from));
        }
    }

    public void Add(Creature creature)
    {
        GetLocation(creature.location).Add(this, creature);
        newCreatures.Add(creature);
        Dummy.instance.Add(creature);
    }

    public void Add(Building building)
    {
        Location location = GetLocation(building.point);
        location.Add(building);
        buildings.Add(building);
        Dummy.instance.Add(building);
    }

    public void Replace(Building old, Building b)
    {
        GetLocation(old.point).Add(b);
        b.Init();
        Dummy.instance.Add(b);
        buildings.Remove(old);
        buildings.Add(b);
    }

    public void Replace(Point point, Tile tile)
    {
        GetLocation(point).tile = tile.id;
        UpdateAdjacency();
        Dummy.instance.drawer.DrawMap(this);
    }

    public Building GetBuilding(Building building)
    {
        return GetLocation(building.point).building;
    }

    public void Add(ItemStack stack, Point point)
    {
        GetLocation(point).Add(stack);
    }

    public bool WithinBounds(Point p)
    {
        return WithinBounds(p.x, p.y);
    }

    public bool WithinBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    public Item KillMonster(Random random, Point p)
    {
        Dummy.instance.drawer.RemoveMonster(p.x, p.y);
        return GetLocation(p).Kill(random);
    }

    public void Add(Monster monster, Building source, Point point)
    {
        if (GetLocation(point).Add(monster, source))
        {
            Dummy.instance.drawer.AddObject(monster.id, point.x, point.y);
        }
    }

    public void Add(Obstacle obstacle, Point point)
    {
        GetLocation(point).Add(obstacle);
    }

}

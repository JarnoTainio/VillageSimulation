using System;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator
{
    Map map;

    int width = 1000;
    int height = 1000;

    int seed;

    int sectorSize = 25;

    int plains = 150;
    int forest = 25;
    int water = 2;
    int mountains = 3;

    int removeLonerTiles = 3;
    int spreadTiles = 4;

    int[] spreadBase = new int[] { 0, 3, 3, 4 };
    int[] tileWeights;
    int[] spreadWeights;

    int calculationCount = 0;

    public void GenerateMap(Map map, int width, int height, int seed = -1)
    {
        this.map = map;
        Random r;
        if (seed == -1)
        {
            Dummy.PrintMessage("Using random seed");
            r = new Random();
            this.seed = r.Next(int.MinValue, int.MaxValue);
            r = new Random(this.seed);
        }
        else
        {
            Dummy.PrintMessage("Using fixed seed");
            r = new Random(seed);
            this.seed = seed;
        }
        map.seed = this.seed;
        Dummy.PrintMessage("Creating map with seed " + this.seed);
        this.width = map.width = width;
        this.height = map.height = height;
        map.locations = new Location[width * height];
        map.adjacency = new int[width * height];

        CreateWeights(r);

        // Create locations and assign them random tiles
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map.locations[x + y * width] = new Location(new Point(x, y), tileWeights[r.Next(tileWeights.Length)]);
            }
        }

        int numberOfSectors = map.locations.Length / (sectorSize * sectorSize);
        int spread = r.Next(spreadTiles) + r.Next(spreadTiles);
        int remove = r.Next(removeLonerTiles) + r.Next(removeLonerTiles);
        float step = .25f;
        for (float i = 0; i < numberOfSectors - step * 2; i += step)
        {
            spread = r.Next(spreadTiles) + r.Next(spreadTiles);
            remove = r.Next(removeLonerTiles) + r.Next(removeLonerTiles);

            SpreadTiles(r, i);
            if (r.Next(2) == 0)
                SpreadTiles(r, i);

            RemoveLoners(r, i, remove);
            if (r.Next(2) == 0)
                RemoveLoners(r, i, remove);

            if (r.Next(3) == 0)
            {
                int tile = r.Next(3) + 1;
                SpreadTiles(r, i, remove, tile);
                if (r.Next(3) == 0)
                {
                    SpreadTiles(r, i, remove, tile);
                    if (r.Next(3) == 0)
                    {
                        SpreadTiles(r, i, remove, tile);
                    }
                }
            }
            if (r.Next(3) == 0)
            {
                int tile = r.Next(3) + 1;
                RemoveLoners(r, i, remove, tile);
                if (r.Next(3) == 0)
                {
                    RemoveLoners(r, i, remove, tile);
                }
            }
        }

        int steps = r.Next(numberOfSectors) + numberOfSectors / 2;
        int currentSector = 0;
        while (steps > 0)
        {
            steps--;
            currentSector += r.Next(4) + 1;
            while (currentSector >= numberOfSectors)
            {
                currentSector -= numberOfSectors;
            }

            //spread = r.Next(spreadTiles) + r.Next(spreadTiles);
            //SpreadTiles(r, sector, spread);

            // 50% chance to spread tiles from random tile type
            if (r.Next(3) == 0)
            {
                SpreadTiles(r, currentSector, r.Next(Tile.tiles.Length - 1) + 1);
            }

            remove = r.Next(removeLonerTiles) + r.Next(removeLonerTiles);
            //RemoveLoners(r, sector, remove);

            // 50% chance to remove loners from random tile type
            if (r.Next(3) == 0)
            {
                RemoveLoners(r, currentSector, remove, r.Next(Tile.tiles.Length - 1) + 1);
            }
        }
        int smooth = r.Next(3) + 2;
        for (float i = 0; i < numberOfSectors - step * 2; i += step)
        {
            int loops = r.Next(smooth / 2) + smooth / 2;
            for (int j = 0; j < loops; j++)
            {
                RemoveLoners(r, i, 1, 0);
            }

        }
        int[] tileCount = new int[Tile.tiles.Length];
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tileCount[map.locations[x + y * width].tile]++;
            }
        }
        int total = width * height;

        if (RemoveTiles(r, Tile.Water.id, -((total / 3) - tileCount[Tile.Water.id]), 300000))
        {
            smooth = r.Next(3) + 2;
            for (float i = 0; i < numberOfSectors - step * 2; i += step)
            {
                int loops = r.Next(smooth / 2) + smooth / 2;
                for (int j = 0; j < loops; j++)
                {
                    RemoveLoners(r, i, 1, Tile.Water.id);
                }

            }
        }
        if (RemoveTiles(r, Tile.Mountain.id, -((total / 3) - tileCount[Tile.Mountain.id]), 300000))
        {
            smooth = r.Next(3) + 2;
            for (float i = 0; i < numberOfSectors - step * 2; i += step)
            {
                int loops = r.Next(smooth / 2) + smooth / 2;
                for (int j = 0; j < loops; j++)
                {
                    RemoveLoners(r, i, 1, Tile.Mountain.id);
                }

            }
        }
        if (RemoveTiles(r, Tile.Forest.id, -((total / 2) - tileCount[Tile.Forest.id]), 300000))
        {
            smooth = r.Next(3) + 2;
            for (float i = 0; i < numberOfSectors - step * 2; i += step)
            {
                int loops = r.Next(smooth / 2) + smooth / 2;
                for (int j = 0; j < loops; j++)
                {
                    RemoveLoners(r, i, 1, Tile.Forest.id);
                }

            }
        }

        Dummy.PrintMessage("Water: " + ((float)tileCount[Tile.Water.id] / (width * height) * 100) + "%");
        Dummy.PrintMessage("Forest: " + ((float)tileCount[Tile.Forest.id] / (width * height) * 100) + "%");
        Dummy.PrintMessage("Mountains: " + ((float)tileCount[Tile.Mountain.id] / (width * height) * 100) + "%");
        calculationCount += map.UpdateAdjacency();

        map.buildings = new List<Building>();
        map.creatures = new List<Creature>();
        map.newCreatures = new List<Creature>();
        map.player = new Player(map, new Point(width / 2, height / 2 - 15));
        map.storyManager = new StoryManager(map);

        AddObjects(r);

        Dummy.PrintMessage("MAP_CALCULATIONS: " + calculationCount);
    }

    private bool RemoveTiles(Random r, int tileID, int toRemove, int loops)
    {
        if (toRemove <= 0)
        {
            return false;
        }
        Dummy.PrintMessage("Remove " + toRemove + " " + Tile.tiles[tileID].name);
        int removed = 0;
        while (toRemove > 0)
        {
            int i = r.Next(width * height);
            if (map.locations[i].tile == tileID)
            {
                if (i > width && i < width * (height - 1))
                {
                    int adj = 0;
                    if (map.locations[i + 1].tile == tileID)
                    {
                        adj++;
                    }
                    if (map.locations[i - 1].tile == tileID)
                    {
                        adj++;
                    }
                    if (map.locations[i + width].tile == tileID)
                    {
                        adj++;
                    }
                    if (map.locations[i - width].tile == tileID)
                    {
                        adj++;
                    }
                    if (adj < 3 || (toRemove > 2000 && adj < 4))
                    {
                        map.locations[i].tile = 0;
                        removed++;
                        toRemove--;
                    }
                }
            }
            loops--;
            if (loops == 0)
            {
                Dummy.PrintMessage("BREAK LOOP!");
                break;
            }
        }
        Dummy.PrintMessage("Removed " + removed + " " + Tile.tiles[tileID].name + " tiles");
        return true;
    }

    private int ConvertAdjacent(Point point, int target, int replace, int energy)
    {
        if (!map.WithinBounds(point) || energy == 0)
        {
            return 0;
        }
        Location loc = map.locations[point.x + point.y * width];
        int count = 0;
        if (loc.tile == target)
        {
            loc.tile = replace;
            count++;
            foreach(Point dir in Direction.Directions)
            {
                Point p = point.Add(dir);
                count += ConvertAdjacent(p, target, replace, energy - 1);
            }
        }
        return count;
    }

    private void CreateWeights(Random r)
    {
        int[] list = new int[] { plains / 2 + r.Next(plains), forest + r.Next(forest), water + r.Next(water), mountains + r.Next(mountains) };
        int total = 0;
        for (int i = 0; i < list.Length; i++)
        {
            total += list[i];
        }
        tileWeights = new int[total];
        int index = 0;
        for (int i = 0; i < list.Length; i++)
        {
            while (list[i] > 0)
            {
                tileWeights[index] = i;
                index++;
                list[i]--;
            }
        }

        spreadWeights = new int[Tile.tiles.Length];
        for (int i = 0; i < spreadWeights.Length; i++)
        {
            spreadWeights[i] = spreadTiles / 2 + r.Next(spreadBase[i]);
        }

    }

    public void SpreadTiles(Random r, float sector, int range = -1, int tileId = -1)
    {
        int sectorX = (int)(sector % (width / sectorSize)) * sectorSize;
        int sectorY = (int)(sector / (height / sectorSize)) * sectorSize;
        //Dummy.PrintMessage("Spread: Sector " + sector + " tile: " + (tileId == -1 ? "all" : Tile.tiles[tileId].name));

        // Spread non-plain tiles
        Point[] directions = new Point[] { Direction.North, Direction.South, Direction.West, Direction.East };
        for (int x = sectorX; x < sectorX + sectorSize; x++)
        {
            for (int y = sectorY; y < sectorY + sectorSize; y++)
            {
                int[] tiles = new int[Tile.tiles.Length];
                foreach (Point p in directions)
                {
                    int i = x + p.x + (y + p.y) * width;
                    if (i >= 0 && i < map.locations.Length)
                    {
                        tiles[map.locations[i].tile]++;
                    }
                }
                tiles[0]--;
                tiles[1]--;
                int best = 0;
                int t = 0;
                for (int i = 1; i < tiles.Length; i++)
                {
                    if (best < tiles[i])
                    {
                        best = tiles[i];
                        t = i;
                    }
                }

                int roll = range <= 0 ? spreadWeights[t] : range;
                if (roll < 2)
                {
                    roll = 2;
                }

                if (t > 0 && (tileId == -1 || t == tileId) && r.Next(roll) == 0)
                {
                    map.locations[x + y * width].tile = t;
                }
                calculationCount++;
            }
        }
    }

    public void RemoveLoners(Random r, float sector, int range, int tileId = -1)
    {
        int sectorX = (int)(sector % (width / sectorSize)) * sectorSize;
        int sectorY = (int)(sector / (height / sectorSize)) * sectorSize;
        //Dummy.PrintMessage("Remove: Sector " + sector + " tile: " + (tileId == -1 ? "all" : Tile.tiles[tileId].name));

        Point[] directions = new Point[] { Direction.North, Direction.South, Direction.West, Direction.East };
        // Remove loner tiles that are not plains or forests
        for (int x = sectorX; x < sectorX + sectorSize; x++)
        {
            for (int y = sectorY; y < sectorY + sectorSize; y++)
            {
                int tile = map.locations[x + y * width].tile;
                if (tile == 0 && tileId != 0)
                {
                    continue;
                }

                int adjacent = 0;
                foreach (Point p in directions)
                {
                    int i = x + p.x + (y + p.y) * width;
                    if (i >= 0 && i < map.locations.Length)
                    {
                        if (map.locations[i].tile == tile)
                        {
                            adjacent++;
                        }
                    }
                }

                if (adjacent == 4)
                {
                    continue;
                }

                if ((tileId == -1 || tile == tileId) && r.Next(range + adjacent * adjacent * 2) == 0)
                {
                    int replace = 0;
                    if (tileId == 0)
                    {
                        replace = 0;
                        foreach (Point p in directions)
                        {
                            int i = x + p.x + (y + p.y) * width;
                            if (i >= 0 && i < map.locations.Length)
                            {
                                if (map.locations[i].tile != tile)
                                {
                                    replace = map.locations[i].tile;
                                    break;
                                }
                            }
                        }
                    }
                    map.locations[x + y * width].tile = replace;
                }
                calculationCount++;
            }
        }
    }

    private void AddObjects(Random random)
    {
        int[] dangerGrid = new int[width * height];
        Point[] directions = new Point[] { Direction.North, Direction.East, Direction.South, Direction.West };
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                int tile = map.locations[x + y * width].tile;

                int danger = Tile.tiles[map.locations[x + y * width].tile].danger;
                foreach (Point p in directions)
                {
                    danger += Tile.tiles[map.locations[x + p.x + (y + p.y) * width].tile].danger / 2;
                }
                dangerGrid[x + y * width] = danger;
            }
        }
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                int i = x + y * width;
                int bonus = 0;
                if (random.Next(100) < 1)
                {
                    Building b = Schematic.Ruins.building.Copy();
                    b.point = new Point(x, y);
                    b.map = map;
                    map.Add(b);
                    bonus = 25;
                    //Dummy.instance.drawer.AddObject(0, x, y);
                }

                int roll = random.Next(100);
                int danger = dangerGrid[i] + bonus;
                if (roll < danger)
                {
                    int monsterIndex = random.Next(Monster.Monsters.Length);
                    Monster monster = Monster.Monsters[monsterIndex];

                    if (monster.IsType(MonsterTag.boss) && random.Next(100) > danger)
                    {
                        monsterIndex = random.Next(Monster.Monsters.Length);
                        monster = Monster.Monsters[monsterIndex];
                    }

                    map.locations[i].Add(monster, null);
                    Dummy.instance.drawer.AddObject(monsterIndex, x, y);
                }

                if (random.Next(100) < 20)
                {
                    List<MapObject> objects = new List<MapObject>();
                    foreach (MapObject mo in MapObject.Objects)
                    {
                        if (mo.CanAppear(map.locations[i].tile))
                        {
                            objects.Add(mo);
                        }
                    }
                    bool addNew = objects.Count > 0;
                    while (addNew)
                    {
                        MapObject mo = objects[random.Next(objects.Count)];
                        objects.Remove(mo);
                        map.locations[i].Add(mo);
                        addNew = objects.Count > 0 && random.Next(100) < 20;
                    }
                }
            }
        }
    }
}
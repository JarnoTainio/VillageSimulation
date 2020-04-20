using System.Collections;
using System.Collections.Generic;

public class MemoryLocation
{
    private MemoryManager memory;
    private List<Memory_Location> locations;
    private int[,] adjacencyGrid;
    private Memory_Location[,] areaGrid;
    private int visitedLocations;
    private int gridSize;

    public MemoryLocation(MemoryManager memory, int gridSize)
    {
        this.memory = memory;
        locations = new List<Memory_Location>();
        visitedLocations = 0;
        areaGrid = new Memory_Location[gridSize, gridSize];
        adjacencyGrid = new int[gridSize, gridSize];
        this.gridSize = gridSize;
    }

    public void Add(Creature creature, Location location)
    {
        Memory_Location newMemory = Get(location.point);

        // If found, then update
        if (newMemory != null)
        {
            newMemory.Update(location);
        }

        //If not found, then create new memory
        else
        {
            visitedLocations++;
            newMemory = new Memory_Location(100, location);
            locations.Add(newMemory);
            if (creature.hasHome && newMemory.point.Distance(creature.GetHome()) <= creature.personality.searchDistance)
            {
                UpdateGrid(creature);
            }
        }
    }

    private void UpdateGrid(Creature creature)
    {
        if (!creature.hasHome)
        {
            return;
        }
        Point home = creature.GetHome();
        Point offSet = home.Add(-creature.personality.searchDistance - 1, -creature.personality.searchDistance - 1);
        int size = creature.personality.searchDistance * 2 + 1;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                areaGrid[x, y] = null;
                adjacencyGrid[x, y] = 0;
            }
        }
        foreach (Memory_Location mem in locations)
        {
            if (mem.point.Distance(home) < creature.personality.searchDistance)
            {
                Point p = mem.point.Reduce(offSet);
                areaGrid[p.x, p.y] = mem;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0)
                        {
                            continue;
                        }
                        Point point = p.Add(x, y);
                        if (point.x < 0 || point.y < 0 || point.x >= size || point.y >= size)
                        {
                            continue;
                        }
                        adjacencyGrid[point.x, point.y]++;
                    }
                }
            }
        }
    }

    public int GetAdjacency(int x, int y)
    {
        if (x < 0 || y < 0 || x >= gridSize || y >= gridSize)
        {
            return 0;
        }
        return adjacencyGrid[x, y];
    }

    public List<Memory_Location> Get() { return locations; }

    public List<Memory_Location> Get(Tile tile, bool empty = false, List<Memory_Location> list = null)
    {
        if (tile == null)
        {
            return list ?? Get();
        }
        list = list ?? Get();
        List<Memory_Location> location_memories = new List<Memory_Location>();
        foreach (Memory_Location mem in list)
        {
            if (mem.IsType(tile) && (!empty || (!mem.HasMonster() && memory.buildings.GetAtPoint(mem.point) == null)))
            {
                location_memories.Add(mem);
            }
        }
        return location_memories;
    }

    public List<Memory_Location> GetObjects(MapObject mapObject, int lastVisited = int.MaxValue)
    {
        List<Memory_Location> found = new List<Memory_Location>();
        foreach(Memory_Location mem in locations)
        {
            if (mem.Contains(mapObject) && mem.LastSeen() < lastVisited)
            {
                found.Add(mem);
            }
        }
        return found;
    }

    public Point GetClosest(Point home, Tile tile, bool empty = false)
    {
        List<Memory_Location> list = Get(tile);
        Point closest = Point.None;
        int distance = int.MaxValue;
        foreach (Memory_Location mem in list)
        {
            int d = home.Distance(mem.point);
            if (d < distance && (!empty || memory.buildings.GetAtPoint(mem.point) == null))
            {
                distance = d;
                closest = mem.point;
            }
        }
        return closest;
    }


    public Memory_Location Get(Point point)
    {
        foreach (Memory_Location locationMemory in locations)
        {
            if (locationMemory.point.Equals(point))
            {
                return locationMemory;
            }
        }
        return null;
    }

    public void Organize()
    {
        foreach (Memory_Location mem in locations)
        {
            if (mem.lastSeen.DaysPassed() > 360 - locations.Count / 10 * locations.Count / 10)
            {
                locations.Remove(mem);
                visitedLocations--;
                return;
            }
        }
    }

    public int VisitedCount() { return visitedLocations; }
}

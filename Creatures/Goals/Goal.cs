using System;
using System.Collections.Generic;

public abstract class Goal
{
    public static int IdentifierCounter;
    public int identifier;

    public Time created;
    public Time questRequested;

    public Creature creature;
    public Need source;
    public string name;
    public string description;
    public int strength;

    //public AgentQuest quest;

    public int reactionBonus = 30;

   public Goal(Need source, string name, string description, int strength)
    {
        identifier = IdentifierCounter++;
        this.creature = source.creature;
        this.source = source;
        this.strength = strength;
        this.name = name + " [" + source.name + "]";
        this.description = description;
        created = TimeManager.Now();
        questRequested = TimeManager.Now();
    }

    public virtual Action GetReaction(Event e) { return null; }
    public virtual Action GetReaction(Location location) { return null; }

    public abstract Plan CreatePlan();

    public abstract bool IsSatisfied();

    public virtual void TickDay() { }

    protected Point FindNewLocation(int searchDepth, bool onlyNew)
    {
        // Get preferred location
        Point offSet = creature.GetHome();

        // Size of search
        int size = Math.Min(searchDepth * 2 + 1, creature.map.width);

        // Limit search box to map limits
        int startX = Math.Max(0, offSet.x - searchDepth);
        startX = Math.Min(creature.map.width - size - 1, startX);
        if (startX < 0)
        {
            startX = 0;
        }

        int startY = Math.Max(0, offSet.y - searchDepth);
        startY = Math.Min(creature.map.height - size - 1, startY);
        if (startY < 0)
        {
            startY = 0;
        }

        // Reduce by searchDepth to get corner of search box
        //offSet = offSet.Add(-searchDepth -1, -searchDepth -1);
        offSet = new Point(startX, startY);

        int[,] grid = new int[size, size];

        // Collect visited locations
        foreach(Memory_Location mem in creature.memory.locations.Get())
        {
            Point p = mem.point.Reduce(offSet);
            
            // Withing bounds
            if (p.x >= 0 && p.x < size && p.y >= 0 && p.y < size){

                // Set as verified(-2) or rumored(-1)
                grid[p.x, p.y] = onlyNew ? -1 : (mem.LastSeen() + 1);
            }
        }

        Point offSetHome = new Point(searchDepth + 1, searchDepth + 1);
        Point offSetCurrent = creature.location.Reduce(offSet);

        Point best = Point.None;
        int cost = int.MaxValue;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // -1 means that this location should be ignored
                if (grid[x,y] < 0)
                {
                    continue;
                }

                // Set memory weight
                if (grid[x, y] > 0)
                {
                    grid[x, y] = 10 / grid[x, y];
                }

                // Value based on current distance and distance from home
                grid[x, y] += (creature.hasHome ? offSetHome.Distance(x, y) * 3 : 0)      // Distance from home
                    + offSetCurrent.Distance(x, y) * 4              // Distance from current location
                    + creature.random.Next(3);                      // Random variation

                if (onlyNew && creature.hasHome)
                {
                    grid[x, y] -= 3 * creature.memory.locations.GetAdjacency(x, y);
                }

                if (grid[x, y] < cost || (grid[x,y] == cost && creature.random.Next(2) == 0))
                {
                    cost = grid[x, y];
                    best = offSet.Add(x, y);
                }
            }
        }

        if (!best.Equals(Point.None))
        {
            if (best.x < 0 || best.y < 0 || best.x >= creature.map.width || best.y >= creature.map.height)
                Dummy.PrintMessage("Problematic: " + startX + ", " + startY + " s: " + size); 
        }

        //Returning -1,-1 equals to not found.
        return best;
    }

    // Add traveling actions to the plan
    protected void AddTravelActions(Plan plan, Point target)
    {
        // Check that target is legal one
        if (!creature.map.WithinBounds(target))
        {
            Dummy.PrintMessage("Target outside of map! " + target + "[" + name + " / " + source + "]");
            return;
        }

        // Add traveling path to new location
        Point p = creature.location;
        int i = 50;
        while (i != 0 && !p.Equals(target))
        {
            // Get direction for current step to the next step
            Point dir = p.GetDirection(target, creature.random.Next(2) == 0);
            p = p.Add(dir);

            // Add travel to that direction to the plan
            plan.Add(new Travel(plan, strength, p, creature.GetTravelTime()));

            // Move point according to the direction
            i--;
        }

        // Safety net to avoid endless loop
        if (i == 0)
        {
            Dummy.PrintMessage("Avoided endless loop at Goal " + name);
        }
    }

    public Plan GoAndGet(ItemStack stack, List<Memory_Item> ownedItems, int threshold)
    {
        Point home = creature.GetHome();
        Memory_Item best = null;
        int distance = int.MaxValue;

        foreach (Memory_Item memory in ownedItems)
        {
            // If wanted material is found, then go pick it up
            if (stack.item.Equals(memory.stack.item))
            {
                int d = home.Distance(memory.point);
                if (best == null || d < distance)
                {
                    distance = d;
                    best = memory;
                }
            }
        }
        if (best != null && creature.GetTravelCost(best.point) <= threshold)
        {
            Plan plan = new Plan(this, "Get " + stack.item.name + " x" + stack.count, strength, .8f);
            AddTravelActions(plan, best.point);
            plan.Add(new Get(plan, strength, stack));
            return plan;
        }

        return null;
    }

    public Plan GoAndGet(ItemType type, List<Memory_Item> ownedItems, int threshold)
    {
        Point home = creature.GetHome();
        Memory_Item best = null;
        int distance = int.MaxValue;

        foreach (Memory_Item memory in ownedItems)
        {
            // If wanted material is found, then go pick it up
            if (memory.stack.item.IsType(type))
            {
                int d = home.Distance(memory.point);
                if (best == null || d < distance)
                {
                    distance = d;
                    best = memory;
                }
            }
        }
        if (best != null && creature.GetTravelCost(best.point) <= threshold)
        {
            Plan plan = new Plan(this, "Get " + best.stack.item + " x" + best.stack.count, strength, .8f);
            AddTravelActions(plan, best.point);
            plan.Add(new Get(plan, strength, best.stack));
            return plan;
        }

        return null;
    }

    public Plan GoAndDrop(Memory_Building memory, ItemStack stack, float threshold)
    {
        if (creature.inventory.Contains(stack))
        {
            Plan plan = new Plan(this, "Gather ", strength);
            AddTravelActions(plan, memory.point);
            plan.Add(new Drop(plan, strength, memory, stack.item, stack.count));
            return plan;
        }
        return null;
    }

    public Plan GoAndProduce(ItemStack stack, List<Memory_Building> ownedBuildings, float threshold)
    {
        if (threshold <= 0)
        {
            return null;
        }

        Point home = creature.GetHome();
        Memory_Building best = null;
        float cost = float.MaxValue;

        foreach (Memory_Building mem in ownedBuildings)
        {
            if (mem.IsSource(stack.item) && mem.CouldBeWorked(creature))
            {
                float c = home.Distance(mem.point) + mem.work.GetEffort(creature); ;
                if (mem.work.required != null && mem.workLeft == 0)
                {
                    c += mem.work.required.GetValue();
                }
                if (c < cost)
                {
                    best = mem;
                    cost = c;
                }
            }
        }
        if (best != null)
        {
            return WorkBuilding(best, ownedBuildings, threshold);
        }
        return null;
    }

    public Plan GoAndProduce(ItemType type, List<Memory_Building> ownedBuildings, float threshold)
    {
        if (threshold <= 0)
        {
            return null;
        }

        Point home = creature.GetHome();
        Memory_Building best = null;
        float cost = float.MaxValue;

        foreach (Memory_Building mem in ownedBuildings)
        {
            if (mem.IsSource(type) && mem.CouldBeWorked(creature))
            {
                float c = home.Distance(mem.point) + mem.work.GetEffort(creature); ;
                if (mem.work.required != null && mem.workLeft == 0)
                {
                    c += mem.work.required.GetValue();
                }
                if (c < cost)
                {
                    best = mem;
                    cost = c;
                }
            }
        }
        if (best != null)
        {
            return WorkBuilding(best, ownedBuildings, threshold);
        }
        return null;
    }

    protected Plan WorkBuilding(Memory_Building building, List<Memory_Building> ownedBuildings, float threshold)
    {
        // Produce required items
        if (building.work.required != null && building.workLeft == 0 && !creature.inventory.Contains(building.work.required))
        {
            return GoAndProduce(building.work.required, ownedBuildings, threshold - 10);
        }

        Item item = building.Source();
        Plan plan = new Plan(this, "Produce " + item.name, strength, .7f);
        AddTravelActions(plan, building.point);

        // Drop required items to building
        if (building.work.required != null && building.workLeft == 0)
        {
            plan.Add(new Drop(plan, strength, building, building.work.required.item, building.work.required.count));
        }

        // Work the building
        plan.Add(new Work(plan, strength, building, 15));
        return plan;
    }

    public Plan GoAndProduceFromTile(ItemStack stack, List<Memory_Location> locations, float threshold)
    {
        Tile tile = null;
        SkillWork work = null;
        float effort = float.MaxValue;

        foreach (Tile t in Tile.tiles)
        {
            // Has what we want and has been seen
            Point closest = creature.memory.locations.GetClosest(creature.GetHome(), t, true);
            if (t.IsSource(stack.item) && !closest.Equals(Point.None))
            {
                SkillWork w = t.GetWork(stack.item);
                float e = w.GetEffort(creature) + creature.GetHome().Distance(closest) * 5;

                if (e < effort)
                {
                    effort = e;
                    tile = t;
                    work = w;
                }
            }
        }
        
        return GoAndProduceFromTile(tile, work);
    }

    public Plan GoAndProduceFromTile(ItemType type, List<Memory_Location> locations, float threshold)
    {
        Tile tile = null;
        SkillWork work = null;
        float effort = float.MaxValue;

        foreach (Tile t in Tile.tiles)
        {
            // Has what we want and has been seen
            Point closest = creature.memory.locations.GetClosest(creature.GetHome(), t, true);
            if (t.IsSource(type) && !closest.Equals(Point.None))
            {
                SkillWork w = t.GetWork(type);
                float e = w.GetEffort(creature) + creature.GetHome().Distance(closest) * 5;

                if (e < effort)
                {
                    effort = e;
                    tile = t;
                    work = w;
                }
            }
        }
        return GoAndProduceFromTile(tile, work);
    }

    private Plan GoAndProduceFromTile(Tile tile, SkillWork work)
    {
        if (tile != null)
        {
            Point home = creature.GetHome();
            Point best = Point.None;
            int cost = int.MaxValue;
            foreach (Memory_Location mem in creature.memory.locations.Get(tile, true))
            {
                if (mem.tile == tile.id)
                {
                    int d = creature.GetTravelCost(mem.point);
                    if (d < cost)
                    {
                        cost = d;
                        best = mem.point;
                    }
                }
            }
            if (!best.Equals(Point.None))
            {
                Plan plan = new Plan(this, "Work " + tile.name, strength, .6f);
                AddTravelActions(plan, best);
                plan.Add(new WorkTile(plan, strength, tile, work));
                return plan;
            }
        }
        return null;
    }

    public Plan GoAndBuild(Schematic schematic, Point location, float threshold)
    {
        Plan plan = new Plan(this, "Begin building " + schematic.building.name, strength);
        AddTravelActions(plan, location);
        plan.Add(new Build(plan, strength, schematic));
        return plan;
    }

    public Plan GoAndWork(Memory_Building building, float threshold)
    {
        Plan plan = new Plan(this, "Build " + building.GetName(), strength);
        AddTravelActions(plan, building.point);

        // ToDo: Get better work duration
        int time = building.workLeft;
        if (time > 15)
        {
            time = 15;
        }
        plan.Add(new Work(plan, strength, building, time));
        return plan;
    }

    public Plan GoAndRepair(Memory_Building memory, float threshold)
    {
        if (memory.repair.required != null && !creature.inventory.Contains(memory.repair.required))
        {
            return null;
        }

        Plan plan = new Plan(this, "Repair " + memory.GetName(), strength);

        AddTravelActions(plan, memory.point);

        // ToDo: Get better work duration
        int time = memory.workLeft;
        if (time > 15)
        {
            time = 15;
        }
        plan.Add(new Maintain(plan, strength, memory, time));
        return plan;
    }

    public Plan GoAndFind(List<Memory_Agent> sellers, List<int> visited, int strength)
    {
        if (sellers.Count == 0)
        {
            return null;
        }
        Plan plan = new Plan(this, "Go and find", strength);

        Memory_Agent best = null;
        int cost = int.MaxValue;

        foreach(Memory_Agent mem in sellers)
        {
            if (!mem.hasHome || visited.Contains(mem.ID) || mem.lastSeen.DaysPassed() > 14)
            {
                continue;
            }

            int c = mem.hasHome ? creature.location.Distance(mem.home) : 255;
            if (c < cost)
            {
                cost = c;
                best = mem;
            }
        }
        if (best != null)
        {
            plan.name += " " + best.name;
            AddTravelActions(plan, best.home);
            visited.Add(best.ID);
            plan.Add(new Patrol(plan, strength, best.home, creature.GetTravelTime(), (int)(10 * creature.personality.patience)));
            return plan;
        }

        return null;
    }

    // Get location for new building
    // ToDo: Get better location
    protected Point GetLocation(Schematic schematic)
    {
        // No special tile required
        if (schematic.requiredTile == null)
        {
            return creature.GetHome();
        }

        // Find best required tile
        Point home = creature.GetHome();
        Point best = Point.None;
        int distance = int.MaxValue;

        foreach(Memory_Location mem in creature.memory.locations.Get(schematic.requiredTile))
        {
            int d = home.Distance(mem.point);
            if (creature.memory.buildings.GetAtPoint(mem.point) == null && d < distance)
            {
                best = mem.point;
            }
        }
        return best;
    }

    public override string ToString()
    {
        return "[" + identifier + "] " + name + "(" + source + ")";
    }

    public abstract string ToText();

}

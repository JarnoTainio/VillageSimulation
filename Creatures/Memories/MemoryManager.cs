using System.Collections;
using System.Collections.Generic;

public class MemoryManager
{
    private Creature creature;

    public MemoryLocation locations;
    public MemoryBuildings buildings;
    public MemoryItems items;
    public MemoryEvents events;
    public MemorySocial social;
    public EventObserver observer;
    
    public CounterList itemCounter;
    private List<CounterList> oldCounters;

    public MemoryManager(Creature creature)
    {
        this.creature = creature;
        itemCounter = new CounterList();

        locations = new MemoryLocation(this, creature.personality.searchDistance * 2 + 1);
        events = new MemoryEvents(creature);
        buildings = new MemoryBuildings();
        items = new MemoryItems();
        social = new MemorySocial(creature);
        observer = new EventObserver(this, creature);      
    }

    public void ObserveAndUpdate(Location location, bool deepObserve)
    {
        locations.Add(creature, location);
        buildings.Observe(creature, location.point, location.building);
        social.Observe(location);
        items.Observe(location);    // ToDo: Implement
        creature.goalManager.Trigger(location);
    }


    // React to events
    public void Add(Event e)
    {
        if (creature.GetMindState() == MindState.Sleeping)
        {
            // ToDo: Wake up on loud noices etc.
            return;
        }

        events.AddEvent(e);
        observer.UnderstandEvent(e);

        if (e.result == EventResult.failure && e.actorID == creature.ID)
        {
            ObserveAndUpdate(creature.map.GetLocation(e.point), true);
        }
        else
        {
            //Add(creature.map.GetLocation(e.actor.location));
        }

        if (e.actorID != creature.ID)
        {
            creature.goalManager.Trigger(e);
        }

    }

    /*========================================================================
    * SLEEPING
    *========================================================================*/

    public void Organize()
    {
        itemCounter.Update();
        events.Organize();
        locations.Organize();
        social.Organize();
    }

    /*========================================================================
    * UTILITY FUNCTIONS
    *========================================================================*/

    public Point GetBuildingPoint(Schematic schematic)
    {
        // Find best required tile
        Point home = creature.GetHome();
        Point best = Point.None;
        int distance = int.MaxValue;

        List<Memory_Location> knownPlaces = locations.Get(schematic.requiredTile, false);
        List<Memory_Location> buildingLocations = locations.Get(schematic.requiredTile, true);

        if (schematic.requiredTile == Tile.Grass)
        {
            foreach(Memory_Location m in locations.Get(Tile.Forest, true))
            {
                if (!m.HasMonster())
                {
                    buildingLocations.Add(m);
                }
            }
        }
        bool hasFoodSource = creature.memory.HasSourceOfFood();

        float[] tileFoodWorks = new float[Tile.tiles.Length];
        if (!hasFoodSource)
        {
            for(int i = 0; i < tileFoodWorks.Length; i++)
            {
                if (!Tile.tiles[i].IsSource(ItemType.Food))
                {
                    tileFoodWorks[i] = 0;
                }
                else
                {
                    tileFoodWorks[i] = Tile.tiles[i].GetWork(ItemType.Food).GetValue(creature);
                }
            }
        }

        foreach (Memory_Location mem in buildingLocations)
        {
            int d = creature.hasHome ? home.Distance(mem.point) - 5 : 0;

            if (mem.tile != schematic.requiredTile.id)
            {
                d += 2;
            }

            float bestWork = 1f;
            bool hasNeighbor = false;
            int grass = 0;

            foreach (Memory_Location m in knownPlaces)
            {
                int dis = mem.point.Distance(m.point);
                
                if (dis != 0 && dis <= 2)
                {
                    if (m.tile == Tile.Grass.id)
                    {
                        grass++;
                    }
                    Memory_Building b = creature.memory.buildings.GetAtPoint(m.point);

                    // Empty tiles are good if no source of food
                    if (b == null)
                    {
                        d -= 2;
                        if (!hasFoodSource)
                        {
                            if (tileFoodWorks[m.tile] > bestWork)
                            {
                                bestWork = tileFoodWorks[m.tile];
                            }
                        }
                    }
                    // Prefer adjacent tiles
                    else if (b.ownerID == creature.ID)
                    {
                        if (dis == 1)
                        {
                            d -= 3;
                        }
                        else
                        {
                            d -= 1;
                        }
                    }

                    // Avoid adjacency to other agents buildings
                    else
                    {
                        if (dis == 1)
                        {
                            d += 1;
                        }
                        hasNeighbor = true;
                    }
                }
            }
            d -= hasNeighbor ? (int) (30 * creature.personality.social) : 0;
            d -= (int)bestWork;


            if (schematic.requiredTile == Tile.Grass && grass < 4)
            {
                d += 8 - grass;
            }

            if (d < creature.memory.locations.Get().Count && d < distance)
            {
                best = mem.point;
                distance = d;
            }
        }
        return best;
    }

    public int Produced(Item item)
    {
        Counter c = itemCounter.GetCount(item);
        if (c == null)
        {
            return 0;
        }
        if (c.gained > c.used)
        {
            return c.total;
        }
        return 0;
    }

    public int Require(Item item)
    {
        Counter c = itemCounter.GetCount(item);
        if (c == null)
        {
            return 0;
        }
        if (c.used >= c.gained)
        {
            return c.total;
        }
        return 0;
    }

    public bool CanGet(Item item)
    {
        // Inventory
        if (creature.inventory.Contains(item))
        {
            return true;
        }

        // Buildings
        foreach (Memory_Building mem in buildings.GetOwned(creature.ID))
        {
            if (mem.IsSource(item) && mem.CouldBeWorked(creature))
            {
                return true;
            }
        }

        // Other agents
        foreach(Memory_Agent mem in social.GetProducers(item))
        {
            return true;
        }

        // Produce from nature
        foreach (Tile t in Tile.tiles)
        {
            // Has what we want and has been seen
            Point closest = locations.GetClosest(creature.GetHome(), t, true);
            if (t.IsSource(item) && !closest.Equals(Point.None))
            {
                return true;
            }
        }
        return false;
    }

    public bool HasSourceOfFood()
    {
        if (buildings.GetSources(ItemType.Food, buildings.GetOwned(creature.ID)).Count > 0)
        {
            return true;
        }
        if (social.Get(ItemType.Food).Count > 0)
        {
            return true;
        }
        return false;
    }

    Time talkedWithPlayer;

    public void PlayerKnowledge(PlayerMemoryManager playerMemory)
    {
        foreach(PlayerMemory memory in playerMemory.memories)
        {
            if (memory.time.time > talkedWithPlayer.time)
            {
                Dummy.PrintMessage("Memory: (" +memory.time.time +") " + memory.action);
                switch (memory.action.action)
                {
                    case (ActionType.kill):
                        Memory_Location locationMemory = locations.Get(memory.action.point);
                        if (locationMemory != null)
                        {
                            locationMemory.KillMonster();
                        }
                        break;
                }
            }
            else
            {
                Dummy.PrintMessage("And thats all since we last met");
                break;
            }
        }
        talkedWithPlayer = TimeManager.Now();
    }
}

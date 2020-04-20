using System.Collections;
using System.Collections.Generic;

public class MemoryBuildings
{
    private List<Memory_Building> buildings;

    public MemoryBuildings()
    {
        buildings = new List<Memory_Building>();
    }

    /*========================================================================
    * OWNERS
    *========================================================================*/
    
    // Add new owner memory from building memory
    public void AddOwner(Memory_Building mem, int ownerID)
    {
        mem.ownerID = ownerID;
    }

    // Add new owner memory for building
    public void AddOwner(Building building, int ownerID)
    {
        foreach (Memory_Building mem in buildings)
        {
            if (mem.point.Equals(building.point))
            {
                if (mem is Memory_Construction && !(building is Construction))
                {
                    buildings.Remove(mem);
                }
                else if (!(mem is Memory_Construction) && building is Construction)
                {
                    buildings.Remove(mem);
                }
                else
                {
                    mem.Update(building);
                    mem.ownerID = ownerID;
                    return;
                }
                break;
            }
        }
        Memory_Building memory;
        if (building is Construction)
        {
            memory  = new Memory_Construction(100, building as Construction, ownerID);
        }
        else
        {
            memory = new Memory_Building(100, building, ownerID);
        }
        buildings.Add(memory);
    }

    /*
    public void RemoveOwner(Building building, Point point, Creature owner = null)
    {
        foreach (Memory_Building mem in Get(owner))
        {
            if (mem.Equals(building)) 
            {
                mem.active = false;
            }
        }
    }
    */

    /*========================================================================
    * GET
    *========================================================================*/

    public List<Memory_Building> Get() { return buildings; }

    // Get all item memories based on BuildingTag given.
    // Can be used in given list or in full memory list.
    public List<Memory_Building> GetByTag(BuildingTag tag, List<Memory_Building> list = null)
    {
        list = list ?? buildings;
        List<Memory_Building> found = new List<Memory_Building>();
        foreach (Memory_Building mem in list)
        {
            if (mem.HasTag(tag))
            {
                found.Add(mem);
            }
        }

        return found;
    }

    // Get all item memories based on give buildings id
    // Can be used in given list or in full memory list.
    public List<Memory_Building> GetByID(int buildingId, List<Memory_Building> list = null)
    {
        list = list ?? buildings;
        List<Memory_Building> found = new List<Memory_Building>();

        foreach (Memory_Building mem in list)
        {
            if (mem.buildingId == buildingId)
            {
                found.Add(mem);
            }
        }

        return found;
    }

    // Get all building memories based on Point given.
    // Can be used in given list or in full memory list.
    public Memory_Building GetAtPoint(Point point, List<Memory_Building> list = null)
    {
        list = list ?? buildings;
        foreach (Memory_Building mem in list)
        {
            if (mem.point.Equals(point))
            {
                return mem;
            }
        }

        return null;
    }

    public List<Memory_Building> GetSources(ItemType type, List<Memory_Building> list = null)
    {
        list = list ?? buildings;
        List<Memory_Building> found = new List<Memory_Building>();

        foreach (Memory_Building mem in buildings)
        {
            if (mem.work?.produced != null && mem.work.produced.IsType(type))
            {
                found.Add(mem);
            }
        }
        return found;
    }

    // Get all building memories based on owners ID.
    // Can be used in given list or in full memory list.
    public List<Memory_Building> GetOwned(int ownerID)
    {
        List<Memory_Building> found = new List<Memory_Building>();
        foreach (Memory_Building mem in buildings)
        {
            if (mem.IsOwned(ownerID))
            {
                found.Add(mem);
            }
        }

        return found;
    }

    /*========================================================================
    * UPDATE
    *========================================================================*/

    public void Observe(Creature creature, Point point, Building observed)
    {
        Memory_Building memory = GetAtPoint(point);
        if (memory == null && observed == null)
        {
            return;
        }

        // Have memory of a building in here
        bool newMemory = true;

        if (memory != null)
        {
            // Memory doesn't match reality
            if (!memory.Equals(observed))
            {
                // Remove incorrect memory
                buildings.Remove(memory);

                // Was this my home?
                if (creature.hasHome && (observed == null || observed.point.Equals(creature.GetHome()) && observed.id != Schematic.House.id))
                {
                    creature.RemoveHome();
                }
            }
            else
            {
                memory.Update(observed);
                newMemory = false;
                return;
            }
        }

        // Add building to memory
        if (newMemory && observed != null)
        {
            if (observed is Construction)
            {
                memory = new Memory_Construction(100, observed as Construction);
            }
            else
            {
                memory = new Memory_Building(100, observed);
            }
            buildings.Add(memory);
        }
    }

    public void Produced(Point point)
    {
        Memory_Building mem = GetAtPoint(point);
        if (mem != null)
        {
            mem.Produced();
        }
    }
}

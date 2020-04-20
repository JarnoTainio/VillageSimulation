using System.Collections;
using System.Collections.Generic;

public class MemoryItems
{

    private List<Memory_Item> items;
    
    public MemoryItems()
    {
        items = new List<Memory_Item>();
    }

    /*========================================================================
    * OWNERS
    *========================================================================*/

    // Add new owner from item memory
    public void AddOwner(Memory_Item mem, int ownerID)
    {
        AddOwner(mem.stack, mem.point, ownerID);
    }

    // Add new owner for item
    public void AddOwner(ItemStack stack, Point point, int ownerID)
    {
        foreach (Memory_Item mem in Get(ownerID))
        {
            if (mem.point.Equals(point) && stack.item.Equals(mem.stack.item))
            {
                mem.stack.count += stack.count;
                return;
            }
        }
        items.Add(new Memory_Item(100, stack, point, ownerID));
    }

    public void RemoveOwner(ItemStack stack, Point point, int ownerID)
    {
        foreach (Memory_Item mem in Get(ownerID))
        {
            if (mem.point.Equals(point) && stack.item.Equals(mem.stack.item))
            {
                mem.stack.count -= stack.count;
                if (mem.stack.count == 0)
                {
                    items.Remove(mem);
                }
                return;
            }
        }

        Dummy.PrintMessage("REMOVE OWNER FAILED!");
    }

    /*========================================================================
    * GET
    *========================================================================*/

    public List<Memory_Item> Get() { return items; }

    // Get all item memories based on owners ID.
    // Can be used in given list or in full memory list.
    public List<Memory_Item> Get(int ownerID, ItemType type)
    {
        List<Memory_Item> list = Get(ownerID);
        List<Memory_Item> found = new List<Memory_Item>();
        foreach (Memory_Item mem in list)
        {
            if (mem.stack.item.IsType(type))
            {
                found.Add(mem);
            }
        }
        return found;
    }

    public List<Memory_Item> Get(int ownerID, List<Memory_Item> list = null)
    {
        list = list ?? items;
        if (ownerID == -1)
        {
            return list;
        }
        List<Memory_Item> found = new List<Memory_Item>();
        foreach (Memory_Item mem in list)
        {
            if (mem.IsOwned(ownerID))
            {
                found.Add(mem);
            }
        }
        return found;
    }

    public List<Memory_Item> Get(Point point, List<Memory_Item> list = null)
    {
        list = list ?? items;
        List<Memory_Item> found = new List<Memory_Item>();
        foreach (Memory_Item mem in list)
        {
            if (mem.point.Equals(point))
            {
                found.Add(mem);
            }
        }
        return found;
    }

    /*========================================================================
    * UPDATE
    *========================================================================*/

    public void Observe(Location location)
    {
        List<Memory_Item> memories = Get(location.point);
        foreach(ItemStack stack in location.items.list)
        {
            // ToDo: Check if everything is here
        }
    }
}

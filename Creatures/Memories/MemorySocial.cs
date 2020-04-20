using System.Collections;
using System.Collections.Generic;

public class MemorySocial
{
    private Creature owner;
    private List<Memory_Agent> agents;

    public MemorySocial(Creature owner)
    {
        this.owner = owner;
        agents = new List<Memory_Agent>();
    }

    public List<Memory_Agent> Get()
    {
        agents.Sort((x, y) => x.lastTalked.MinutesPassed().CompareTo(y.lastTalked.MinutesPassed()));
        return agents;
    }

    public List<Memory_Agent> GetProducers(Item item)
    {
        List<Memory_Agent> list = new List<Memory_Agent>();
        foreach (Memory_Agent mem in agents)
        {
            if (mem.Produces(item))
            {
                list.Add(mem);
            }
        }
        return list;
    }

    public List<Memory_Agent> GetRequirers(Item item)
    {
        List<Memory_Agent> list = new List<Memory_Agent>();
        foreach (Memory_Agent mem in agents)
        {
            if (mem.Requires(item))
            {
                list.Add(mem);
            }
        }
        return list;
    }

    public List<Memory_Agent> Get(ItemType type)
    {
        List<Memory_Agent> list = new List<Memory_Agent>();
        foreach (Memory_Agent mem in agents)
        {
            if (mem.Produces(type))
            {
                list.Add(mem);
            }
        }
        return list;
    }

    public List<Memory_Agent> GetKnown(int maxDays = -1)
    {
        List<Memory_Agent> known = new List<Memory_Agent>();
        foreach(Memory_Agent mem in agents)
        {
            if (mem.talkedTo && (maxDays == -1 || mem.lastSeen.DaysPassed() < maxDays))
            {
                known.Add(mem);
            }
        }
        return known;
    }

    public Memory_Agent Add(int otherID, Point point)
    {
        if (otherID == owner.ID)
        {
            return null;
        }
        Memory_Agent mem = GetAgent(otherID);

        if (mem == null)
        {
            agents.Add(new Memory_Agent(otherID, 100));
        }
        else
        {
            mem.Update(point);
        }
        return mem;
    }

    public Memory_Agent GetAgent(int ID)
    {
        if (ID == owner.ID)
        {
            return null;
        }

        foreach (Memory_Agent mem in agents)
        {
            if (mem.ID == ID)
            {
                return mem;
            }
        }
        Memory_Agent memory = new Memory_Agent(ID, 100);
        agents.Add(memory);
        return memory;
    }

    public void Observe(Location location)
    {
        foreach (Creature creature in location.creatures)
        {
            if (creature.Equals(this.owner))
            {
                continue;
            }
            Add(creature.ID, location.point);
        }
    }

    public void Organize()
    {
        foreach (Memory_Agent mem in agents)
        {
            if (mem.lastTalked.DaysPassed() > ((mem.talkedTo ? 1000 : 180) - agents.Count * agents.Count))
            {
                agents.Remove(mem);
                return;
            }
        }
    }
}

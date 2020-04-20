using System.Collections;
using System.Collections.Generic;

public class Memory_Agent : Memory
{
    // Identifier
    public int ID;
    public bool talkedTo;

    // Constant attributes
    public string name;
    public Time birthTime;

    // Home
    public bool hasHome;
    public Point home;

    // Produces and requires
    public ItemStackList produces;
    public ItemStackList requires;

    // Seen at
    public List<SeenAt> seen;
    public Time lastTalked;
    public Time lastUpdated;
    public Time lastFailed;

    // Opinion
    public float relationship;

    public Memory_Agent(int ID,  int importance) : base(importance)
    {
        name = "Unknown " + ID;
        this.ID = ID;
        produces = new ItemStackList();
        requires = new ItemStackList();
        seen = new List<SeenAt>();
        talkedTo = false;
        home = Point.None;
        Update(Point.None);
        lastFailed = Time.Zero;
    }

    public void Update(Point point)
    {
        lastUpdated = TimeManager.Now();
        UpdateSeen();

        if (point.Equals(Point.None))
        {
            return;
        }

        bool found = false;
        foreach(SeenAt s in seen)
        {
            if (s.Equals(point))
            {
                s.Add();
                found = true;
                break;
            }
        }
        if (!found)
        {
            SeenAt s = new SeenAt(point);
            seen.Add(s); 
        }
    }

    public void Relationship(int mod)
    {
        lastUpdated = TimeManager.Now();
        relationship += mod;
        if (relationship < 0)
        {
            relationship = 0;
        }
        else if (relationship > 100)
        {
            relationship = 100;
        }
    }

    public void Update(CounterList counterList)
    {
        lastUpdated = TimeManager.Now();
        UpdateSeen();
        produces.Clear();
        requires.Clear();
        foreach (Counter c in counterList.list)
        {
            if (c.total > 0)
            {
                produces.Add(new ItemStack(c.id, c.gained));
            }
            else if (c.total < 0)
            {
                requires.Add(new ItemStack(c.id, c.used));
            }
        }
    }

    public bool Requires(Item item)
    {
        return requires.Count(item) > 3;
    }

    public bool Produces(Item item)
    {
        return produces.Count(item) > 3;
    }

    public bool Produces(ItemType type)
    {
        return produces.Count(type) > 3;
    }

    public void Reset(Item item)
    {
        produces.Remove(item);
        requires.Remove(item);
    }

    public void Failed()
    {
        lastFailed = TimeManager.Now();
    }

    public void Hello(Creature creature)
    {
        if (creature.ID != ID)
        {
            Dummy.PrintMessage("TRYING TO OVERWRITE MEMORY WITH WRONG AGENT DATA!");
            return;
        }
        talkedTo = true;
        name = creature.name;
        home = creature.GetHome();
        hasHome = creature.hasHome;
        Talking();
    }

    public void Talking()
    {
        lastTalked = TimeManager.Now();
        UpdateSeen();
    }

    public CounterList BuildCounter()
    {
        CounterList counter = new CounterList();
        foreach (ItemStack s in produces.list)
        {
            counter.Produced(s);
        }
        foreach (ItemStack s in requires.list)
        {
            counter.Consumed(s);
        }
        return counter;
    }

    public bool Equals(Creature creature)
    {
        if (creature == null)
        {
            return false;
        }
        return ID == creature.ID;
    }
}

public class SeenAt
{
    public readonly Point point;
    public int count;
    public readonly Time first;
    public Time latest;
    public Time lastTick;
    public int[] hours;

    public SeenAt(Point point)
    {
        this.point = point;
        first = lastTick = TimeManager.Now();
        hours = new int[24];
        Add();
    }

    public void Add()
    {
        Time now = TimeManager.Now();
        // Update only if last update was more than 15 minutes ago
        if (latest.time + 15 < now.time)
        {
            hours[now.hour - 1]++;
            latest = now;
            count++;
        }

        int daysSinceTick = lastTick.DaysPassed();
        while (daysSinceTick > 5)
        {
            daysSinceTick -= 5;
            lastTick = now;
            count = count / 2;
            for(int i = 0; i < hours.Length; i++)
            {
                if (hours[i] > 1)
                {
                    hours[i] = hours[i] / 2;
                }
            }
        }
    }

    public bool Equals(Point point)
    {
        return this.point.Equals(point);
    }

    public string Print()
    {
        string str = "Seen at point " + point + " " +count +" times , hours: [";
        foreach(int i in hours)
        {
            str += i + ", ";
        }
        str += " first: " + first + ", last " + latest;
        return str;
    }
}


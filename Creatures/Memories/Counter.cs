
using System;
using System.Collections.Generic;

public class Counter
{
    public int id;
    public int gained;
    public int used;
    public int total;
    public Time time;

    public Counter(int id)
    {
        this.id = id;
        time = TimeManager.Now();
    }

    public void Add(int amount)
    {
        if (amount > 0)
        {
            gained += amount;
        }
        else
        {
            used -= amount;
        }
        total += amount;
        time = TimeManager.Now();
    }

    public bool Equals(int id)
    {
        return this.id == id;
    }

    public void Soften()
    {
        if (time.DaysPassed() > 1)
        {
            if (gained > 0)
            {
                gained--;
                total--;
            }
            if (used > 0)
            {
                used--;
                total++;
            }
            time = TimeManager.Now();
        }
    }
}

public class CounterList
{
    Time time;
    public List<Counter> list;

    public Counter GetCount(Item item)
    {
        foreach(Counter c in list) {
            if (c.Equals(item.id))
            {
                return c;
            }
        }
        return null;
    }

    public CounterList()
    {
        list = new List<Counter>();
        time = TimeManager.Now();
    }

    public void Consumed(ItemStack stack)
    {
        Consumed(stack.item.id, stack.count);
    }

    public void Consumed(int id, int amount = 1)
    {
        Produced(id, -amount);
    }

    public void Produced(ItemStack stack)
    {
        Produced(stack.item.id, stack.count);
    }

    public void Produced(int id, int amount = 1)
    {
        foreach(Counter c in list)
        {
            if (c.Equals(id))
            {
                c.Add(amount);
                return;
            }
        }
        Counter co = new Counter(id);
        list.Add(co);
        co.Add(amount);
        list.Sort((x, y) => y.total.CompareTo(x.total));
    }

    public Counter Get(Item item)
    {
        foreach(Counter c in list)
        {
            if (c.Equals(item))
            {
                return c;
            }
        }
        return null;
    }

    public void Update()
    {
        foreach(Counter c in list)
        {
            c.Soften();
        }
    }

    public string Print()
    {
        string str = "";
        foreach(Counter c in list)
        {
            str += Item.items[c.id] + " (" + c.gained +" - " +c.used +" = " + c.total + ")\n";
        }
        return str;
    }
}

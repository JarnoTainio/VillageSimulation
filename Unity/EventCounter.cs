using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EventCounter
{
    public string key;
    public int count;

    public EventCounter(string key)
    {
        this.key = key;
        count = 0;
    }
}

[System.Serializable]
public class EventCounterList
{
    List<EventCounter> eventList;

    public EventCounterList()
    {
        eventList = new List<EventCounter>();
    }

    public void Add(string key)
    {
        foreach(EventCounter ec in eventList)
        {
            if (ec.key.Equals(key))
            {
                ec.count++;
                Sort();
                return;
            }
        }
        eventList.Add(new EventCounter(key));
    }

    private void Sort()
    {
        eventList.Sort((x, y) => y.count.CompareTo(x.count));
    }

    public override string ToString()
    {
        string str = "";
        foreach(EventCounter e in eventList)
        {
            str += e.key + ": " + e.count + "\n";
        }
        return str;
    }
}

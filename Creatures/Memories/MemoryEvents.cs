using System.Collections;
using System.Collections.Generic;

public class MemoryEvents
{
    private Creature creature;

    public List<Memory_Day> dayEvents;
    public List<Memory_Day> dayActions;

    public List<Memory_Event> events;
    public List<Memory_Event> actions;

    public MemoryEvents(Creature creature)
    {
        events = new List<Memory_Event>();
        actions = new List<Memory_Event>();
        dayEvents = new List<Memory_Day>();
        dayActions = new List<Memory_Day>();
        this.creature = creature;
    }

    public List<Event> GetToday(int ID = -1)
    {
        List<Event> list = new List<Event>();
        foreach(Memory_Event mem in events)
        {
            if (mem.e.action == EventAction.talking && (ID == -1 || (int)(mem.e.target) == ID))
            {
                list.Add(mem.e);
            }
        }
        return list;
    }

    public void AddEvent(Event e)
    {
        // Personal actions
        if (e.actorID == creature.ID)
        {
            AddEventTo(actions, e);
        }

        // Other actions
        else
        {
            AddEventTo(events, e);
        }
    }

    private void AddEventTo(List<Memory_Event> list, Event e)
    {
        if (list.Count > 0)
        {

            Memory_Event last = list[list.Count - 1];
            if (last.e.action == e.action)
            {
                last.Update(e);
            }
            else
            {
                list.Add(new Memory_Event(e, 100));
            }
        }
        else
        {
            list.Add(new Memory_Event(e, 100));
        }
    }

    public void Organize()
    {
        if (actions.Count > 1)
        {
            //dayActions.Add(new Memory_Day(actions));
            while (actions.Count > 0)
            {
                actions.Remove(actions[0]);
            }
        }

        if (events.Count > 1)
        {
            //dayEvents.Add(new Memory_Day(events));
            while (events.Count > 0)
            {
                events.Remove(events[0]);
            }
        }

        //creature.memory.itemCounter.Modify(.9f);
    }

}

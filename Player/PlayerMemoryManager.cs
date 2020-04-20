using System.Collections;
using System.Collections.Generic;

public class PlayerMemoryManager
{
    public Stack<PlayerMemory> memories;

    public PlayerMemoryManager()
    {
        memories = new Stack<PlayerMemory>();
    }

    // Returns days passed since action has been done in the point.
    // If memory is not found, then returns -1.
    public int IsDone(PlayerAction action)
    {
        foreach(PlayerMemory mem in memories)
        {
            if (action.Equals(mem))
            {
                return mem.time.DaysPassed();
            }
        }
        return -1;
    }

    public void Add(PlayerAction action)
    {
        // Dont add movement actions
        if (action.action == ActionType.move)
        {
            return;
        }
        memories.Push(new PlayerMemory(action));
    }

    public bool ContainsAction(PlayerAction action, Time startingTime)
    {
        int start = startingTime.time;

        foreach(PlayerMemory a in memories)
        {
            // Happened after starting time
            if (start < a.time.time)
            {
                // Wanted action equals memory action
                if (a.action.Equals(action))
                {
                    return true;
                }
            }
            // Out of timescope
            else
            {
                break;
            }
        }
        return false;
    }
}

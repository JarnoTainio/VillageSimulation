using System.Collections.Generic;

public class Memory_Day
{
    List<Memory_Event> events;
    public Time start;
    public Time end;

    public Memory_Day(List<Memory_Event> events)
    {
        this.events = new List<Memory_Event>(events);
    }
}

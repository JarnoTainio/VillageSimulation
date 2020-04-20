public class Memory_Event : Memory
{
    public Event e;

    public Memory_Event(Event e, int importance) : base(importance)
    {
        this.e = e;
    }

    public void Update(Event e)
    {
        this.e.duration = time.Difference(TimeManager.Now());
        this.e.result = e.result;
        UpdateSeen();
    }

    public override string ToString()
    {
        return e.ToString();
    }
}
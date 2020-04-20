public class Memory
{
    public static int IdentifierCounter;
    public int identifier;

    public bool active;

    public Time time;
    public Time lastSeen;
    public int importance;

    public Memory(int importance)
    {
        identifier = IdentifierCounter++;
        time = lastSeen = TimeManager.Now();
        this.importance = importance;
        active = true;
    }

    protected void UpdateSeen()
    {
        lastSeen = TimeManager.Now();
    }

    public int LastSeen()
    {
        return lastSeen.DaysPassed();
    }
}

public abstract class Action
{
    public static int IdentifierCounter;
    public int identifier;

    public string name;
    public int timeCost;
    public int duration;
    public Plan source;

    public int strength;

    public MindState mindState;

    public Action(Plan source, string name, int strength, MindState mindState, int timeCost)
    {
        identifier = IdentifierCounter++;
        this.source = source;
        this.name = name;
        this.strength = strength;
        this.mindState = mindState;
        this.timeCost = timeCost;
        if (timeCost <= 0)
        {
            timeCost = 1;
        }
        else if (timeCost > 600)
        {
            Dummy.PrintMessage(name + " has timeCost of " + timeCost + "!");
        }
        duration = timeCost;

    }

    public abstract int Tick(Map map, Creature creature, int delta);

    protected void SetMind(Creature creature)
    {
        creature.UpdateMindState(mindState);
    }

    protected void Done(Map map, Creature creature, Event e, bool resetMind = true)
    {
        if (resetMind)
        {
            creature.ResetMindState();
        }
        if (e != null)
        {
            map.Event(creature.location, e);
        }
        creature.ActionDone(this);
    }

    protected void Failed(Map map, Creature creature, Event e)
    {
        creature.ResetMindState();
        if (e != null)
        {
            map.Event(creature.location, e);
        }
        creature.ActionFailed(this, e.failure);
    }

    protected void Ongoing(Map map, Creature creature, Event e)
    {
        map.Event(creature.location, e);
        creature.ActionOngoing(this);
    }

    public override string ToString()
    {
        return name;
    }

}

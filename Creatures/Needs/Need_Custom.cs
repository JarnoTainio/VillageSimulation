
using System.Collections.Generic;

public class Need_Custom : Need
{

    public Need_Custom(Creature creature, int strength, int growTime, string reason) : base(creature, "Quest", reason, strength, growTime)
    {

    }

    public override List<Goal> CreateGoals()
    {
        List<Goal> goals = new List<Goal>();
        return goals;
    }

}

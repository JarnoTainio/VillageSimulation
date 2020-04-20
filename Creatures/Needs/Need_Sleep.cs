
using System.Collections.Generic;

public class Need_Sleep : Need
{
    public Need_Sleep(Creature creature, int strength, int growTime) : base(creature, "Sleep", "I am tired", strength, growTime)
    {
    }

    public override List<Goal> CreateGoals()
    {
        List<Goal> goals = new List<Goal>();
        // Travel home if tired
        if (creature.hasHome && creature.distanceToHome > 0 && creature.CanSleep(creature.distanceToHome / creature.travelSpeed, true))
        {
            int value = GetValue(.5f + ((int)creature.tiredState) / 2 + creature.distanceToHome / 15);
            goals.Add( new Goal_Travel(this, GetValue(1f), creature.GetHome(), "Home to sleep"));
        }

        // Sleep if you can
        if (creature.CanSleep())
        {
            // 0.8 + 0.1 / tiredState
            goals.Add(new Goal_Sleep(this, GetValue(.8f + ((int)creature.tiredState) / 10f)));
        }

        return goals;
    }

}

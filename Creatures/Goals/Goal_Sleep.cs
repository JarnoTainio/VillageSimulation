using System;

public class Goal_Sleep : Goal
{
    public Goal_Sleep(Need source, int strength) : base(source, "Sleep", "I want to sleep", strength)
    {
    }

    public override Plan CreatePlan()
    {
        // Stop if creature cant sleep
        if (creature.CanSleep())
        {
            Plan plan = new Plan(this, "Sleep", strength);
            if (creature.distanceToHome > 0 && creature.tiredState < Tired.Extremely)
            {
                AddTravelActions(plan, creature.GetHome());
            }
            // Create plan
            int sleepTime = (int)(Math.Min(creature.tired / 2, creature.personality.sleepDuration));
            plan.Add(new Sleep(plan, strength, sleepTime));
            plan.name += " " + sleepTime +" hours";
            return plan;
        }

        return null;
    }

    public override bool IsSatisfied()
    {
        return !creature.CanSleep();
    }

    public override string ToText()
    {
        return "I need to sleep, because ";
    }
}

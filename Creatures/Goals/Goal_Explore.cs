using System;

public class Goal_Explore : Goal
{
    Point target;
    readonly int distance;
    readonly bool onlyNew;
    string reason;

    public Goal_Explore(Need source, int strength, int distance, bool onlyNew, string reason) : base(source, "Explore " + reason, "I want to explore", strength)
    {
        target = new Point();
        this.distance = distance;
        this.onlyNew = onlyNew;
        this.reason = reason;
    }

    public override Plan CreatePlan()
    {
        // Create plan
        Plan plan = new Plan(this, "Explore", strength);
        target = FindNewLocation(distance, onlyNew);
        Point p = creature.location;

        // Reduce traveling distance from the plans strength
        //plan.strength -= creature.location.Distance(target);

        // If creature has a home, then reduce that from strength also
        //plan.strength -= creature.GetHome().Distance(target);

        // New location not found
        if (target.Equals(-1, -1))
        {
            strength--;
            return null;
        }

        plan.name += target;

        // Add traveling path to new location
        AddTravelActions(plan, target);

        return plan;
    }

    public override bool IsSatisfied()
    {
        return creature.location.Equals(target);
    }

    public override string ToText()
    {
        string str = "I want to explore";
        if (onlyNew)
        {
            str +=  " new places";
        }
        str += ", because " + reason;
        return str;
    }
}

public class Goal_Travel: Goal
{
    Point target;
    string reason;

    public Goal_Travel(Need source, int strength, Point target, string reason) : base(source, "Travel to " + target, reason, strength)
    {
        this.target = target;
        this.reason = reason;
    }

    public override Plan CreatePlan()
    {
        // Create plan
        Plan plan = new Plan(this, "travel " + reason, strength);
        AddTravelActions(plan, target);
        return plan;
    }

    public override bool IsSatisfied()
    {
        return creature.location.Equals(target);
    }

    public override string ToText()
    {
        return "I want to travel to " + Direction.ToString(creature.location.GetDirection(target)) + ", because " + source.name +".";
    }
}

using System.Collections.Generic;

public class Goal_Custom : Goal
{
    public List<Action> actions;
    Plan plan;

    public Goal_Custom(Need source, string name, string description, int strength, List<Action> actions) : base(source, name, description, strength)
    {
        this.actions = actions;
    }

    public override Plan CreatePlan()
    {
        plan = new Plan(this, name, strength);
        foreach(Action a in actions)
        {
            plan.Add(a);
            a.source = plan;
        }
        return plan;
    }

    public override bool IsSatisfied()
    {
        return !plan.ActionsLeft();
    }

    public override string ToText()
    {
        return "Im just doing something weird.";
    }

}

using System.Collections.Generic;

public class Plan
{
    public Goal source;

    public string name;
    public int strength;
    public List<Action> actions;

    public Plan(Goal source, string name, int strength, float modifier = 1f)
    {
        this.source = source;
        this.name = name;
        Update(strength, modifier);
        actions = new List<Action>();
    }

    public void Update(int strength, float modifier)
    {
        this.strength = (int)(strength * modifier);
    }

    public void Add(Action action)
    {
        actions.Add(action);
    }


    public Action NextAction()
    {
        if (actions.Count > 0)
        {
            return actions[0];
        }
        return null;
    }

    public void ActionDone(Action a)
    {
        actions.Remove(a);
    }

    public bool ActionsLeft()
    {
        return actions.Count > 0;
    }

    public override string ToString()
    {
        return "(" + strength + ") " + name + "[" + source + "]";
    }
}

using System.Collections.Generic;

public class NeedManager
{
    private readonly Creature creature;
    public List<Need> needs;

    public NeedManager(Creature creature)
    {
        this.creature = creature;
        needs = creature.personality.GenerateNeeds();
        UpdateOrder();
    }

    public void GoalSatisfied(Goal g)
    {
        g.source.Tick(g);
    }

    public void UpdateOrder()
    {
        needs.Sort((x, y) => y.GetValue().CompareTo(x.GetValue()));
    }

    public List<Goal> GetGoals(GoalManager goalManager, int threshold)
    {
        List<Goal> goals = new List<Goal>();
        List<Goal> oldGoals = new List<Goal>(goalManager.GetGoals());

        // Get goal from each need
        foreach(Need need in needs)
        {
            List<Goal> newGoals = need.CreateGoals();
            foreach (Goal goal in newGoals)
            {
                bool found = false;
                foreach (Goal g in oldGoals)
                {
                    if (g.source == need && g.name == goal.name)
                    {
                        g.strength = goal.strength;
                        found = true;
                        goals.Add(g);
                        oldGoals.Remove(g);
                        break;
                    }
                }
                if (!found)
                {
                    // Add new goal to the list
                    goals.Add(goal);
                }
            }
        }

        foreach(Goal g in oldGoals)
        {
            goals.Add(g);
        }

        // If there is no goals, then return idling
        if (goals.Count == 0)
        {
            List<Action> actions = new List<Action>();
            actions.Add(new Idle(null, 0, "doing nothing"));
            goals.Add(new Goal_Custom(new Need(creature, "nothing better to do", "I have nothing better to do", 10, 10), "Idling","Idling", 10, actions));
        }
        return goals;
    }
}

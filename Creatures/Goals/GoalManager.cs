using System.Collections;
using System.Collections.Generic;

public class GoalManager
{
    private readonly Creature owner;

    private List<Goal> goals;
    public Action reaction;

    public Plan plan;
    public Action lastAction;

    public int abandonThreshold = 10;

    public int lastThinking;
    public int actionTicks;

    public GoalManager(Creature owner)
    {
        this.owner = owner;
        goals = new List<Goal>();
        lastThinking = 0;
    }

    /*========================================================================
    * N E E D S
    ========================================================================*/

    public void Think()
    {
        // Current goal
        Goal current = plan?.source;

        lastThinking = 0;
        // Get new goals
        goals = owner.needManager.GetGoals(this, 0);

        // Update order of goals
        UpdateOrder();

        // Set next plan, if current goal has changed
        if (current == null || goals.Count == 0 || current.name != goals[0].name)
        {
            SetNextPlan();
        }
    }

    private Plan SetNextPlan()
    {
        foreach (Goal g in goals)
        {
            Plan plan = g.CreatePlan();
            if (plan != null && plan.ActionsLeft())
            {
                if (SetPlan(plan))
                {
                    return plan;
                }
            }
        }
        return null;
    }

    private void UpdateOrder()
    {
        goals.Sort((x, y) => y.strength.CompareTo(x.strength));
    }

    /*========================================================================
    * G O A L S
    ========================================================================*/

    public List<Goal> GetGoals()
    {
        return goals;
    }

    public bool TestGoal(Goal g)
    {
        if (g.IsSatisfied())
        {
            GoalSatisfied(g);
            return true;
        }
        return false;
    }

    private void GoalSatisfied(Goal g)
    {
        RemoveGoal(g);
        g.source.Tick(g);

        // Notify need of satisfied goal
        owner.needManager.GoalSatisfied(g);

        // Remove current reaction if it's source is the goal
        if (reaction != null && reaction.source.source == g)
        {
            reaction = null;
        }

        // Abandon plan if source is this goal
        if (plan != null && plan.source == g)
        {
            AbandonPlan();
        }
    }

    public void Add(Goal g)
    {
        goals.Add(g);
        UpdateOrder();
    }

    private void RemoveGoal(Goal g)
    {
        goals.Remove(g);
        UpdateOrder();
    }

    /*========================================================================
     * P L A N S
     ========================================================================*/

    private bool SetPlan(Plan newPlan)
    {
        if (newPlan == null)
        {
            Dummy.PrintMessage("ERROR! New plan was null");
            return false;
        }
        plan = newPlan;
        return true;
    }

    private void AbandonPlan()
    {
        if (plan == null)
        {
            return;
        }
        plan = null;
    }

    /*========================================================================
     * A C T I O N S
     ========================================================================*/

    public Action NextAction()
    {
        actionTicks = 0;

        // Reaction
        if (reaction != null)
        {
            if (reaction.source.source.IsSatisfied())
            {
                RemoveGoal(reaction.source.source);
                reaction = null;
            }
            else
            {
                return reaction;
            }
        }

        // Have a plan and it has actions
        if (plan != null && plan.ActionsLeft())
        {
            lastAction = plan.NextAction();
            return lastAction;
        }

        // Select next plan
        else
        {
            Think();
            lastAction = plan?.NextAction();
            if (lastAction != null)
            {
                return lastAction;
            }
        }

        // No goals left, so think of a new one
        lastAction = new Think(null, 10);
        return lastAction;
    }

    public void ActionDone(Action action)
    {
        if (action == reaction)
        {
            //reaction.source.ActionDone(reaction);
            if (reaction.source.source.IsSatisfied())
            {
                RemoveGoal(reaction.source.source);
            }
            reaction = null;
            SetNextPlan();

            return;
        }

        // Completed action is part of current plan
        Plan plan = action.source;
        if (plan != null && action == plan.NextAction())
        {
            plan.ActionDone(action);

            // If plan's source is satisfied, then plan and source can be removed
            if (plan.source.IsSatisfied())
            {
                // Goal is satisfied, this also abandons the plan
                GoalSatisfied(plan.source);
            }
            
            // No actions left, so abandon plan
            else if (!plan.ActionsLeft())
            {
                AbandonPlan();
            }

            else
            {
                Think();
            }
        }
    }

    public void ActionFailed(Action action, Failure failure)
    {
        //Dummy.instance.CreateIcon(4, owner.location);
        //owner.PrintMessage("FAILED: " + action + ", because " + failure +" (" +action.source.source + ")", false);
        if (action == reaction)
        {
            reaction = null;
            return;
        }

        // If failed action is part of the plan, then plan is not valid anymore
        else if (plan != null && action == plan.NextAction())
        {
            if (plan.source.IsSatisfied())
            {
                GoalSatisfied(plan.source);
            }
        }
        
         SetNextPlan();
    }


    public bool ActionOngoing(Action action)
    {
        actionTicks++;
        if (actionTicks >= 480 && (owner.hungerState >= Hunger.Very || owner.tiredState >= Tired.Very))
        {
            actionTicks = 0;
            return false;
        }
        return true;
    }

    public void Trigger(Event e)
    {
        foreach (Goal g in goals)
        {
            Action r = g.GetReaction(e);
            if (r != null && (reaction == null || r.strength > reaction.strength))
            {
                reaction = r;
            }
        }
    }

    public void Trigger(Location location)
    {
        foreach (Goal g in goals)
        {
            Action r = g.GetReaction(location);
            if (r != null && (reaction == null || r.strength > reaction.strength))
            {
                reaction = r;
            }
        }
    }

    public void TickDay()
    {
        foreach(Goal goal in goals)
        {
            goal.TickDay();
        }
    }
}

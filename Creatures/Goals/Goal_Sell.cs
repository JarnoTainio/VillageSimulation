using System.Collections;
using System.Collections.Generic;

public class Goal_Sell : Goal
{
    Time started;
    ItemStack stack;
    Plan plan;

    public Goal_Sell(Need source, ItemStack stack, int strength) : base(source, "Sell " + stack, "I want to sell " +stack, strength)
    {
        this.stack = stack;
        plan = new Plan(this, "Sell " + stack, strength);
        started = TimeManager.Now();
    }

    public override Plan CreatePlan()
    {
        Memory_Agent best = null;
        int value = int.MinValue;
        foreach(Memory_Agent mem in creature.memory.social.GetProducers(stack.item))
        {
            if (!mem.hasHome)
            {
                continue;
            }
            int v = mem.requires.Count(stack.item) * 3;
            v -= mem.home.Distance(creature.GetHome());
            if (best == null || v > value)
            {
                value = v;
                best = mem;
            }
        }
        if (best != null)
        {
            plan = new Plan(this, "Find " + best.name, strength);
            AddTravelActions(plan, best.home);
            plan.Add(new Patrol(plan, strength, best.home, creature.GetTravelTime(), 6));
            return plan;
        }
        return null;
    }

    public override Action GetReaction(Event e)
    {
        if (e.actorID == -1)
        {
            return null;
        }

        Memory_Agent mem = creature.memory.social.GetAgent(e.actorID);
        if (mem != null && e.action != EventAction.sleep && mem.lastTalked.MinutesPassed() > 60)
        {
            //int want = mem.requires.Count(stack.item);
            if (mem.lastTalked.DaysPassed() > 0)
            {
                return new Talk(plan, strength + reactionBonus * 2, mem, new SocialMessage(SocialAction.sell, stack));
            }
        }
        return null;
    }

    public override bool IsSatisfied()
    {
        if (creature.inventory.Count(stack.item) < 5)
        {
            return true;
        }
        else if (started.MinutesPassed() > Time.Hour * 6)
        {
            return true;
        }
        return false;
    }

    public override string ToText()
    {
        return "ToDo: Implement";
    }
}

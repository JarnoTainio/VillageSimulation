using System.Collections;
using System.Collections.Generic;

public class Goal_MeetOthers : Goal
{
    Plan plan;
    int targetNumber;

    public Goal_MeetOthers(Need source, int strength, int targetNumber) : base(source, "Meet new people", "I want to meet new people", strength)
    {
        plan = new Plan(this, "Meet new people", strength);
        this.targetNumber = targetNumber;
    }

    public override Plan CreatePlan()
    {
        return null;
    }

    public override Action GetReaction(Event e)
    {
        if (e.actorID == -1 || e.actorID == creature.ID)
        {
            return null;
        }
        Memory_Agent mem = creature.memory.social.GetAgent(e.actorID);
        if (!mem.talkedTo)
        {
            if (e.point.Equals(creature.location))
            {
                return new Talk(plan, strength + reactionBonus, mem, new SocialMessage(SocialAction.hello));
            }
            else if (e.action != EventAction.travel && e.point.Distance(creature.location) == 1)
            {
                return new Travel(plan, strength + reactionBonus / 2, e.point, creature.GetTravelTime());
            }
        }
        return null;
    }

    public override Action GetReaction(Location location)
    {
        foreach (Creature c in location.GetCreatures())
        {
            if (c == creature)
            {
                continue;
            }
            Memory_Agent mem = creature.memory.social.GetAgent(c.ID);
            if (!mem.talkedTo)
            {
                if (location.point.Equals(creature.location))
                {
                    return new Talk(plan, strength + reactionBonus, mem, new SocialMessage(SocialAction.hello));
                }
            }

        }
        return null;
    }

    public override bool IsSatisfied()
    {
        return creature.memory.social.GetKnown().Count >= targetNumber;
    }

    public override string ToText()
    {
        return "I want to meet new people.";
    }
}

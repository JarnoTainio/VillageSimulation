using System.Collections;
using System.Collections.Generic;

public class Need_Social : Need
{
    public Need_Social(Creature creature, int strength, int growTime) : base(creature, "Social", "I want to talk to others", strength, growTime)
    {
    }

    public override List<Goal> CreateGoals()
    {
        List<Goal> goals = new List<Goal>();
        List<Memory_Agent> knownAgents = creature.memory.social.GetKnown(14);

        // Meet new people
        if (knownAgents.Count < 10 * (creature.personality.social - 0.5f))
        {
            goals.Add(new Goal_MeetOthers(this, (int)(strength * .8f), knownAgents.Count + 1));
        }

        // Talk to already met people and learn more about them
        foreach (Memory_Agent mem in knownAgents)
        {
            int lastTalked = mem.lastTalked.MinutesPassed();

            // Dont talk too often
            if (lastTalked < Time.Day * 2f - Time.Day * creature.personality.social)
            {
                continue;
            }

            int days = mem.lastTalked.DaysPassed();

            float v = (mem.relationship / 2 + 50) - mem.home.Distance(creature.GetHome()) * 2;
            v /= 100f;

            if (v < 0)
            {
                v = 0;
            }
            else if (days < 7)
            {
                v += days / 28;
            }
            else
            {
                v -= days / 100;
            }
            goals.Add(new Goal_Talk(this, (int)(strength * v), mem, new SocialMessage(SocialAction.info)));
        }

        return goals;
    }
}
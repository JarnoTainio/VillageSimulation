using System;
using System.Collections.Generic;

public class Goal_Talk : Goal
{
    Memory_Agent memory;
    readonly SocialMessage message;
    readonly Plan plan;
    Point target;
    bool visitedHome;

    public Goal_Talk(Need source, int strength, Memory_Agent memory, SocialMessage message) : base(source, "Talk to " + memory.name, "I want to talk to " + memory.name +" about " +message.action, strength)
    {
        this.memory = memory;
        this.message = message;
        visitedHome = false;
        plan = new Plan(this, "Talk to " + memory.name, strength);
    }

    public override Plan CreatePlan()
    {
        return null;
        /*
        if (memory.lastSeen.DaysPassed() > 7)
        {
            return null;
        }
        Time time = TimeManager.GetTime();

        Point target = memory.home;
        int value = int.MinValue;

        if (!visitedHome && memory.hasHome)
        {
            AddTravelActions(plan, memory.home);
            plan.Add(new Patrol(plan, strength, memory.home, creature.GetTravelTime(), 8));
            visitedHome = true;
            return plan;
        }

        foreach (SeenAt seen in memory.seen)
        {
            
            
            int distance = seen.point.Distance(creature.location);
            float hour = (float)distance / creature.travelTime + time.hour;
            if (hour > 24)
            {
                hour -= 24;
            }
            int count = seen.hours[(int)Math.Ceiling(hour - 1)];
            if (count == 0)
            {
                continue;
            }
            int v = distance + count * count;
            if (v > value)
            {
                target = seen.point;
                value = v;
            }
        }

        if (target.Equals(Point.None))
        {
            return null;
        }

        AddTravelActions(plan, target);
        plan.Add(new Patrol(plan, strength, target, creature.GetTravelTime(), 6));
        return plan;
        */
    }

    public override Action GetReaction(Event e)
    {
        if (e.actorID == -1 || e.actorID == creature.ID)
        {
            return null;
        }
        Memory_Agent mem = creature.memory.social.GetAgent(e.actorID);
        if (mem != null && mem.lastFailed.MinutesPassed() > 60 && e.action != EventAction.sleep && mem.lastTalked.MinutesPassed() > 240)
        {
            if (e.point.Equals(creature.location))
            {
                return new Talk(plan, strength + reactionBonus, mem, new SocialMessage(SocialAction.info));
            }
            else if (e.action != EventAction.travel && e.point.Distance(creature.location) == 1)
            {
                return new Travel(plan, strength + reactionBonus / 2, e.point, creature.GetTravelTime());
            }
        }
        return null;
    }

    public override bool IsSatisfied()
    {
        if (visitedHome && memory.lastSeen.DaysPassed() > 14)
        {
            return true;
        }
        // Seen agent in last minute
        foreach(Memory_Event mem in creature.memory.events.actions)
        {
            if (mem.e.result == EventResult.success && mem.e.action == EventAction.talking && mem.e.time.MinutesPassed() == 0 && (int)mem.e.target == memory.ID)
            {
                return true;
            }
        }
        return false;
    }

    public override string ToText()
    {
        string str = "I want to talk to "+memory.name;
        if (memory.lastSeen.DaysPassed() > 1)
        {
            str += ", but I havent seen him in while";
        }
        if (memory.hasHome)
        {
            str += ". He lives ";
            int d = creature.location.Distance(memory.home);
            if (d == 0)
            {
                str += "here.";
            }
            else
            {
                if (d > 5)
                {
                    str += "far ";
                }
                str += Direction.ToString(creature.location.GetDirection(memory.home)) + " from here.";
            }
        }
        return str;
    }
}

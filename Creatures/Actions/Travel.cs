public class Travel : Action
{
    Point target;

    public Travel(Plan source, int strength, Point target, int travelTime) : base(source, "traveling to " + target, strength, MindState.Traveling, travelTime)
    {
        this.target = target;
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        SetMind(creature);
        duration -= delta;

        if (duration <= 0)
        {
            if (map.WithinBounds(target))
            {
                if (target.Distance(creature.location) == 1)
                {
                    Location location = map.GetLocation(target);
                    creature.MoveTo(location);
                    Done(map, creature, new Event(creature.ID, target, EventAction.travel, EventResult.success, location));
                    creature.travelProgress = 0f;
                    creature.travelingTo = creature.location;
                }
                else
                {
                    //Dummy.PrintMessage("Trying to travel from " + creature.location + " to " + target + "Source: " + source);
                    Failed(map, creature, new Event(creature.ID, target, EventAction.travel, Failure.notAble));
                    creature.travelProgress = 0f;
                    creature.travelingTo = creature.location;
                }
            }
            else
            {
                Failed(map, creature, new Event(creature.ID, target, EventAction.travel, Failure.notAble));
                creature.travelProgress = 0f;
                creature.travelingTo = creature.location;
            }
            return duration;
        }

        Ongoing(map, creature, new Event(creature.ID, target, EventAction.travel, EventResult.ongoing));
        creature.travelProgress = ((float)timeCost - duration) / timeCost;
        creature.travelingTo = target;
        return 0;
    }
}

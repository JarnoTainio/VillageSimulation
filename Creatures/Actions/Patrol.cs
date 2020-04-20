using System.Collections.Generic;

public class Patrol : Action
{
    private int radius;
    private Point target;
    private Point current;
    private int rounds;

    public Patrol(Plan source, int strength, Point target, int travelTime, int rounds, int radius = 2) : base(source, "patrolling area " + target, strength, MindState.Traveling, travelTime)
    {
        this.target = target;
        this.radius = radius;
        current = target;
        this.rounds = rounds;
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        if (creature.location.Equals(current))
        {
            SetNext(creature);
        }

        SetMind(creature);
        duration -= delta;

        if (duration <= 0)
        {
            if (map.WithinBounds(current))
            {
                if (current.Distance(creature.location) == 1)
                {
                    Location location = map.GetLocation(current);
                    creature.MoveTo(location);
                    if (rounds > 0)
                    {
                        creature.travelProgress = 0f;
                        creature.travelingTo = creature.location;
                        Ongoing(map, creature, new Event(creature.ID, current, EventAction.travel, EventResult.success, location));
                        SetNext(creature);
                    }
                    else
                    {
                        creature.travelProgress = 0f;
                        creature.travelingTo = creature.location;
                        Done(map, creature, new Event(creature.ID, current, EventAction.travel, EventResult.success, location));
                    }
                }
                else
                {
                    Failed(map, creature, new Event(creature.ID, current, EventAction.travel, Failure.notAble));
                    creature.travelProgress = 0f;
                    creature.travelingTo = creature.location;
                }
            }
            else
            {
                Failed(map, creature, new Event(creature.ID, current, EventAction.travel, Failure.notAble));
                creature.travelProgress = 0f;
                creature.travelingTo = creature.location;
            }
            return duration;
        }

        Ongoing(map, creature, new Event(creature.ID, current, EventAction.travel, EventResult.ongoing));
        creature.travelProgress = ((float)timeCost - duration) / timeCost;
        creature.travelingTo = current;
        return 0;
    }

    private void SetNext(Creature creature)
    {
        rounds--;

        List<Point> dirs = new List<Point>();
        foreach (Point dir in Direction.Directions)
        {
            if (current.Add(dir).Distance(target) <= radius)
            {
                dirs.Add(dir);
            }
        }
        Point d = dirs[creature.random.Next(dirs.Count)];
        current = current.Add(d);
        duration = timeCost;
        creature.travelProgress = 0f;
        creature.travelingTo = creature.location;
    }
}

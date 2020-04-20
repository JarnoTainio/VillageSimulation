public class Maintain : Action
{
    public Building building;

    public Maintain(Plan source, int strength, Memory_Building phantom, int duration) : base(source, "maintaining " + phantom.GetName(), strength, MindState.Working, duration)
    {
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        if (building == null)
        {
            building = creature.map.GetLocation(creature.location).building;
        }
        if (building == null)
        {
            Failed(map, creature, new Event(creature.ID, creature.location, EventAction.repair, Failure.notFound, building));
            return 0;
        }
        SetMind(creature);
        duration -= delta;

        if (duration <= 0)
        {
            if (building.Repair(creature))
            {
                Done(map, creature, new Event(creature.ID, creature.location, EventAction.repair, EventResult.finished, building));
            }
            else
            {
                Failed(map, creature, new Event(creature.ID, creature.location, EventAction.repair, Failure.notAble, building));
            }
            return 0;
        }

        Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.repair, EventResult.ongoing, building));
        return 0;
    }

    public override string ToString()
    {
        return "working at " + building;
    }

}

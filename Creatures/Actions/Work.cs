public class Work : Action
{
    public Building building;
    public Memory_Building memory;

    public Work(Plan source, int strength, Memory_Building memory, int duration) : base(source, "working at " + memory.GetName(), strength, MindState.Working, duration)
    {
        this.memory = memory;
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        // Get real building
        if (building == null)
        {
            building = map.GetLocation(creature.location).building;
        }

        if (building == null)
        {
            Failed(map, creature, new Event(creature.ID, creature.location, EventAction.work, Failure.notFound, building));
            return 1 - delta;
        }
        if (!building.CanBeWorked(creature)){
            Failed(map, creature, new Event(creature.ID, creature.location, EventAction.work, Failure.notAble, building));
            return 1 - delta;
        }

        SetMind(creature);
        duration -= delta;

        int remaining = building.Work(creature, delta);
        creature.UseEnergy(delta + remaining);

        if (duration <= 0 || creature.memory.buildings.GetAtPoint(building.point).lastProduced.MinutesPassed() == 0)
        {
            Done(map, creature, new Event(creature.ID, creature.location, EventAction.work, EventResult.finished, building));
            if (duration <= 0)
            {
                return duration;
            }
            return remaining;
        }

        else if (building.CanBeWorked(creature))
        {
            Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.work, EventResult.success, building));
        }

        else
        {
            Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.work, EventResult.ongoing, building));
        }
        return remaining;
    }

    public override string ToString()
    {
        return "working at "+ memory.GetName();
    }

}

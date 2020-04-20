public class Sleep : Action
{

    int ticks;

    public Sleep(Plan source, int strength, int duration) : base(source, "sleeping", strength, MindState.Sleeping, (duration > 0 ? duration : 1) * 60)
    {
        ticks = 0;
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        SetMind(creature);

        if (creature.CanSleep())
        {
            ticks += delta;
            duration -= delta;

            if (ticks >= 60)
            {
                ticks -= 60;
                creature.memory.Organize();
            }

            // Sleeping completed
            if (duration <= 0)
            {
                creature.Rest((-duration * 2) / 60f);
                Done(map, creature, new Event(creature.ID, creature.location, EventAction.sleep, EventResult.success, creature.map.GetLocation(creature.location).building));
                return duration;
            }
            creature.Rest((delta * 2) / 60f);
            Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.sleep, EventResult.ongoing, creature.map.GetLocation(creature.location).building));
            return 0;
        }
        else
        {
            Failed(map, creature, new Event(creature.ID, creature.location, EventAction.sleep, Failure.notAble, creature.map.GetLocation(creature.location).building));
            return 5 - delta;
        }
    }
}

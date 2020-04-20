using System.Collections;
using System.Collections.Generic;

public class Combat : Action
{
    public Combat(Plan source, int strength) : base(source, "fighting monster", strength, MindState.Combat, 30)
    {
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        duration -= delta;
        if (!map.GetLocation(creature.location).HasMonster())
        {
            Failed(map, creature, new Event(creature.ID, creature.location, EventAction.fight, Failure.notFound));
            return -delta;
        }
        if (duration <= 0)
        {
            Item item = map.KillMonster(creature.random, creature.location);
            if (item != null)
            {
                creature.inventory.Add(item);
            }
            Done(map, creature, new Event(creature.ID, creature.location, EventAction.fight, EventResult.success));
            return delta;
        }
        Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.fight, EventResult.ongoing));
        return 0;
    }
}

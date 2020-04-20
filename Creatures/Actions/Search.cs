namespace AgentActions
{

    public class Search : Action
    {

        public Search(Plan source, int strength) : base(source, "searching location", strength, MindState.Exploring, 30)
        {
        }

        public override int Tick(Map map, Creature creature, int delta)
        {
            SetMind(creature);
            duration -= delta;

            if (duration <= 0)
            {
                if (map.GetLocation(creature.location).HasMonster())
                {
                    Failed(map, creature, new Event(creature.ID, creature.location, EventAction.search, Failure.notAble));
                    return duration;
                }
                else
                {
                    Done(map, creature, new Event(creature.ID, creature.location, EventAction.search, EventResult.success));
                    return duration;
                }
            }
            Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.search, EventResult.ongoing));
            return 0;
        }

    }


}

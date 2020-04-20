public class Think : Action
{
    public Think(Plan source, int strength) : base(source ,"think", strength, MindState.Thinking, 5)
    {
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        SetMind(creature);

        duration -= delta;
        if (duration <= 0)
        {
            creature.goalManager.Think();
            Done(map, creature, new Event(creature.ID, creature.location, EventAction.thinking, EventResult.success));
            return duration;
        }
        Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.thinking, EventResult.ongoing));
        return 0;
    }
}

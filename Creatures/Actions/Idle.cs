public class Idle : Action
{
    public Idle(Plan source, int strength, string description, int time = 15) : base(source, description, strength, MindState.Neutral, time)
    {
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        SetMind(creature);
        duration -= delta;

        if (duration <= 0)
        {
            // Rest a little
            creature.Rest(-duration / 6);
            Done(map, creature, new Event(creature.ID, creature.location, EventAction.idling, EventResult.success));
            return duration;
        }
        creature.Rest(delta / 6);
        Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.idling, EventResult.ongoing));
        return 0;
    }

}

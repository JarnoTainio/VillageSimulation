public class Build : Action
{
    public Schematic schematic;
    public Construction construction;

    public Build(Plan source, int strength, Schematic schematic) : base(source, "building " + schematic.building.name, strength, MindState.Working, 30)
    {
        this.schematic = schematic;
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        SetMind(creature);

        if (map.GetLocation(creature.location).building != null)
        {
            Failed(map, creature, new Event(creature.ID, creature.location, EventAction.build, Failure.blocked, schematic.building));
            return 0;
        }

        if (construction == null)
        {
            construction = new Construction(map, creature.location, schematic, Schematic.Construction.building.work, schematic.requiredTile);
            construction.id = 0;
        }

        duration -= delta;
        if (duration <= 0)
        {
            map.Add(construction);
            Done(map, creature, new Event(creature.ID, creature.location, EventAction.build, EventResult.started, construction));
            return duration;
        }
        Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.build, EventResult.ongoing, construction));
        return 0;
    }

}

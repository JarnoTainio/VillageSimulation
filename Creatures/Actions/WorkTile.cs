public class WorkTile : Action
{
    public Tile tile;
    public SkillWork skillWork;

    public WorkTile(Plan source, int strength, Tile tile, SkillWork skillWork) : base(source, skillWork.skill.name + " at " + tile.name, strength, MindState.Working, skillWork.worktime)
    {
        this.tile = tile;
        this.skillWork = skillWork;
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        SetMind(creature);
        duration -= delta;

        if (duration <= 0)
        {
            if (skillWork.Work(map, creature, tile))
            {
                Done(map, creature, null);
            }
            else
            {
                Failed(map, creature, new Event(creature.ID, creature.location, EventAction.produce, Failure.failChance, skillWork));
            }
            return duration;
        }
        else
        {
            Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.work, EventResult.ongoing, skillWork));
        }
        return 0;
    }

}

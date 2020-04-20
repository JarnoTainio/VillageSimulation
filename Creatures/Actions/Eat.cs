public class Eat : Action
{
    public ItemStack item;

    public Eat(Plan source, int strength, Item item) : base(source, "eating " + item, strength, MindState.Eating, 10)
    {
        this.item = new ItemStack(item, 1);
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        SetMind(creature);
        duration -= delta;

        if (duration <= 0)
        {
            ItemStack found = creature.inventory.Remove(item.item, 1);
            if (found != null && item.item is Food)
            {
                creature.Eat(found.item as Food);
                Done(map, creature, new Event(creature.ID, creature.location, EventAction.eat, EventResult.success, found));
                return duration;
            }
            else
            {
                Failed(map, creature, new Event(creature.ID, creature.location, EventAction.eat, (found == null ? Failure.notFound : Failure.notAble), found));
                return duration;
            }         
        }
        Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.eat, EventResult.ongoing, item));
        return 0;
    }

}

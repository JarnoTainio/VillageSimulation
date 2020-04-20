public class Get : Action
{
    public ItemStack item;
    public int count;

    public Get(Plan source, int strength, Item item, int count = -1) : base(source, "getting " + item, strength, MindState.Working, 5){
        this.item = new ItemStack(item, 1);
        this.count = count;
    }

    public Get(Plan source, int strength, ItemStack stack) : base(source, "Get " + stack, strength, MindState.Working, 5)
    {
        this.item = stack;
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        SetMind(creature);

        duration -= delta;
        if (duration <= 0)
        {
            ItemStack stack = map.GetLocation(creature.location).Remove(item);
            if (stack != null && stack.count > 0)
            {
                creature.inventory.Add(stack);
                Done(map, creature, new Event(creature.ID, creature.location, EventAction.get, EventResult.success, stack));
            }
            else
            {
                Failed(map, creature, new Event(creature.ID, creature.location, EventAction.get, Failure.notFound, item));
            }
            return duration;
        }
        Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.get, EventResult.ongoing, item));
        return 0;
    }

}

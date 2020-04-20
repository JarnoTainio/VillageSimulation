public class Drop : Action
{
    public ItemStack item;
    public Memory_Building memory;
    public Point buildingPoint;
    public int count;
    bool sellingToShop;

    public Drop(Plan source, int strength, Item item, int count = -1, bool sellingToShop = false) : base(source, "dropping " + item, strength, MindState.Working, 5)
    {
        this.item = new ItemStack(item, 1);
        this.count = count;
        this.sellingToShop = sellingToShop;
    }

    public Drop(Plan source, int strength, Memory_Building memory, Item item, int count = -1) : base(source, "Drop " + item + " to " + memory.GetName(), strength, MindState.Working, 5)
    {
        this.item = new ItemStack(item, 1);
        this.count = count;
        this.memory = memory;
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        SetMind(creature);
        duration -= delta;

        if (duration <= 0)
        {
            ItemStack stack = creature.inventory.Remove(item);
            if (stack != null && stack.count > 0)
            {
                Building b = map.GetLocation(creature.location).building;

                // Place items to the building
                if (memory != null)
                {
                    if (b != null)
                    {
                        if (memory.Equals(b))
                        {
                            // Add materials to the building
                            int itemCount = stack.count;
                            stack = b.AddMaterial(stack);

                            // Return unused items to creatures inventory
                            if (stack.count > 0)
                            {
                                creature.inventory.Add(stack);
                                if (stack.count == itemCount)
                                {
                                    Failed(map, creature, new Event(creature.ID, creature.location, EventAction.drop, EventResult.failure, Failure.notAble, item));
                                    return duration;
                                }
                            }

                            // Update used item count
                            item.count -= stack.count;

                            // Activate event
                            Done(map, creature, new Event(creature.ID, creature.location, EventAction.drop, EventResult.success, item, b));
                            return duration;
                        }
                    }
                    creature.inventory.Add(stack);
                    Failed(map, creature, new Event(creature.ID, creature.location, EventAction.drop, EventResult.failure, Failure.notFound, item));
                    return duration;
                }

                // Drop items to ground
                else
                {
                    map.Add(stack, creature.location);
                    Done(map, creature, new Event(creature.ID, creature.location, EventAction.drop, EventResult.success, item));
                    return duration;
                }
                
            }
            else
            {
                Failed(map, creature, new Event(creature.ID, creature.location, EventAction.drop, EventResult.failure, Failure.notFound, item));
            }
            return duration;
        }
        Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.get, EventResult.ongoing, item));
        return 0;
    }

}

public class Goal_Maintain : Goal
{
    Memory_Building phantom;

    public Goal_Maintain(Need source, int strength, Memory_Building phantom) : base(source, "Maintain " + phantom.GetName(), "I want to maintain " + phantom.GetName(), strength)
    {
        this.phantom = phantom;
    }

    public override Plan CreatePlan()
    {
        if (creature.CanWork())
        {
            return null;
        }

        if (phantom.durability < 100f)
        {
            ItemStack item = phantom.repair.required;

            Plan plan = GoAndRepair(phantom, strength);
            if (plan != null)
            {
                return plan;
            }

            plan = GoAndGet(item, creature.memory.items.Get(creature.ID), strength);
            if (plan != null)
            {
                return plan;
            }

            plan = GoAndProduce(item, creature.memory.buildings.GetOwned(creature.ID), strength);
            if (plan != null)
            {
                return plan;
            }

            plan = GoAndProduceFromTile(item, creature.memory.locations.Get(), strength * .9f);
            if (plan != null)
            {
                return plan;
            }

            return null;
        }
        return null;
    }

    public override bool IsSatisfied()
    {
        if (phantom.durability < 90f)
        {
            return true;
        }
        return !phantom.CouldBeWorked(creature);
    }

    public override string ToText()
    {
        string str = "I need to repair my " +phantom.GetName() +", because ";
        if (phantom.durability < .25f)
        {
            str += "It's falling apart.";
        }
        else if (phantom.durability < .5f)
        {
            str += "It's in bad shape.";
        }
        else if (phantom.durability < .75f)
        {
            str += "It's starting to show wear.";
        }
        else
        {
            str += "I want to keep it in good condition.";
        }
        return str;
    }
}

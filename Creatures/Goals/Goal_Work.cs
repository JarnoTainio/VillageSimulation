public class Goal_Work : Goal
{
    Memory_Building phantom;
    int state = 0;
    Plan plan;

    public Goal_Work(Need source, int strength, Memory_Building phantom) : base(source, "Work "+phantom.GetName(), "I want to work " + phantom.GetName(), strength)
    {
        this.phantom = phantom;
    }

    public override Plan CreatePlan()
    {
        if (creature.CanWork())
        {
            return null;
        }

        if (phantom.work.required != null)
        {
            plan = null;
            // Work if there is something to do
            if (phantom.workQueue > 0 || phantom.workLeft > 0)
            {
                plan = GoAndWork(phantom, strength);
                if (plan != null)
                {
                    state = 0;
                    return plan;
                }
            }

            // Drop materials if have any
            plan = GoAndDrop(phantom, phantom.work.required, strength);
            if (plan != null)
            {
                state = 1;
                return plan;
            }

            // Go get known materials
            plan = GoAndGet(phantom.work.required, creature.memory.items.Get(creature.ID), strength);
            if (plan != null)
            {
                state = 2;
                return plan;
            }

            // Go produce materials
            plan = GoAndProduce(phantom.work.required, creature.memory.buildings.GetOwned(creature.ID), strength);
            if (plan != null)
            {
                state = 3;
                return plan;
            }

            // Go produce materials from tile
            plan = GoAndProduceFromTile(phantom.work.required, creature.memory.locations.Get(), strength * .9f);
            if (plan != null)
            {
                state = 4;
                return plan;
            }
            return null;
        }
        if (phantom.CouldBeWorked(creature))
        {
            state = 0;
            plan = GoAndWork(phantom, strength);
            return plan;
        }
        return null;
    }

    public override bool IsSatisfied()
    {
        if (creature.inventory.Count(phantom.work.produced.item) > 28)
        {
            return true;
        }
        return !phantom.CouldBeWorked(creature) || phantom.lastProduced.MinutesPassed() == 0;
    }

    public override string ToText()
    {
        string str = "I need to work at " + phantom.GetName() + " to produce " +phantom.work.produced;
        if (state == 0)
        {
            if (phantom.workLeft > 120)
            {
                str += ". There is a lot of work left to do.";
            }
            else if (phantom.workLeft <= 30)
            {
                str += ". There is only little work left to do";
            }
        }
        else
        {
            str += ", but I need to get " + phantom.work.required + " first.";
        }

        if (state == 1)
        {
            str += " Luckily I already have it";
            if (creature.location.Distance(phantom.point) > 0)
            {
                str += ", so I just need to take it there";
            }
            str += ".";
        }
        else if (state == 2)
        {
            str += " I just need to get it.";
        }
        else if (state == 3)
        {
            str += " I need to go and produce it from ";
            foreach(Action a in plan.actions)
            {
                if (a is Work)
                {
                    str += (a as Work).memory.GetName() + ".";
                    break;
                }
            }
        }
        else if (state == 4)
        {
            str += " I need to go and produce it from ";
            foreach (Action a in plan.actions)
            {
                if (a is WorkTile)
                {
                    str += (a as WorkTile).tile.name + ".";
                    break;
                }
            }
        }
        return str;
    }
}

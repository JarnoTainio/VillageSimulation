using System.Collections.Generic;

public class Goal_Get : Goal
{
    ItemStack stack;
    ItemType type;
    int count;
    List<int> visited;

    bool gettingFromTile;

    string currentDescription;

    Plan plan;

    public Goal_Get(Need source, int strength, ItemStack stack) : base(source, "Get " + stack, "I want to get " + stack, strength)
    {
        this.stack = stack;
        plan = new Plan(this, "Get " + stack, strength);
        visited = new List<int>();
    }

    public Goal_Get(Need source, int strength, ItemType type, int count) : base(source, "Get " + type, "I want to get " + type, strength)
    {
        this.type = type;
        this.count = count;
        plan = new Plan(this, "Get " + type, strength);
        visited = new List<int>();
    }

    public override Plan CreatePlan()
    {
        if (stack == null)
        {
            return FindType();
        }
        else
        {
            return FindStack();
        }
    }

    private Plan FindType()
    {
        Plan plan = GoAndGet(type, creature.memory.items.Get(creature.ID), strength);
        if (plan != null)
        {
            currentDescription = "I am getting it.";
            gettingFromTile = false;
            return plan;
        }

        plan = GoAndProduce(type, creature.memory.buildings.GetOwned(creature.ID), strength);
        if (plan != null)
        {
            currentDescription = "I am going to ";
            foreach (Action a in plan.actions)
            {
                if (a is Work)
                {
                    currentDescription += a.name;
                    break;
                }
            }
            gettingFromTile = false;
            return plan;
        }

        plan = GoAndFind(creature.memory.social.Get(type), visited, strength);
        if (plan != null)
        {
            gettingFromTile = false;
            return plan;
        }

        plan = GoAndProduceFromTile(type, creature.memory.locations.Get(), strength * .9f);
        if (plan != null)
        {
            currentDescription = "I am going to ";
            foreach(Action a in plan.actions)
            {
                if (a is WorkTile)
                {
                    currentDescription += a.name;
                    break;
                }
            }
            gettingFromTile = true;
            return plan;
        }

        return null;
    }

    private Plan FindStack()
    {
        Plan plan = GoAndGet(stack, creature.memory.items.Get(creature.ID), strength);
        if (plan != null)
        {
            currentDescription = "I am getting it.";
            gettingFromTile = false;
            return plan;
        }

        plan = GoAndProduce(stack, creature.memory.buildings.GetOwned(creature.ID), strength);
        if (plan != null)
        {
            currentDescription = "I am going to ";
            foreach (Action a in plan.actions)
            {
                if (a is Work)
                {
                    currentDescription += a.name;
                    break;
                }
            }
            gettingFromTile = false;
            return plan;
        }


        plan = GoAndFind(creature.memory.social.GetProducers(stack.item), visited, strength);
        if (plan != null)
        {
            gettingFromTile = false;
            return plan;
        }
        

        plan = GoAndProduceFromTile(stack, creature.memory.locations.Get(), strength * .9f);
        if (plan != null)
        {
            currentDescription = "I am going to ";
            foreach (Action a in plan.actions)
            {
                if (a is WorkTile)
                {
                    currentDescription += a.name;
                    break;
                }
            }
            gettingFromTile = true;
            return plan;
        }

        return null;
    }

    public override Action GetReaction(Event e)
    {
        if (!gettingFromTile || e.actorID == -1)
        {
            return null;
        }

        Memory_Agent mem = creature.memory.social.GetAgent(e.actorID);
        if (mem != null)
        {
            if (stack != null)
            {
                if (mem.Produces(stack.item))
                {
                    if (e.point.Equals(creature.location))
                    {
                        return new Talk(plan, strength + reactionBonus, mem, new SocialMessage(SocialAction.buy, stack));
                    }
                }
            }
            else if (stack == null)
            {
                if (mem.Produces(type))
                {
                    ItemStack best = null;
                    foreach (ItemStack s in mem.produces.Get(type))
                    {
                        // ToDo: Chooce best item
                        best = new ItemStack(s.item, count);
                        break;
                    }
                    if (best != null)
                    {
                        if (e.point.Equals(creature.location))
                        {
                            return new Talk(plan, strength + reactionBonus, mem, new SocialMessage(SocialAction.buy, best));
                        }
                    }
                }
            }
        }

        return null;
    }

    public override bool IsSatisfied()
    {
        // Check if enough in inventory
        if (stack != null)
        {
            return creature.inventory.Count(stack.item) >= stack.count;
        }
        else
        {
            return creature.inventory.Count(type) >= count;
        }
    }

    public override string ToText()
    {
        string str = "I need " + (stack != null ? stack.ToString() : type.ToString());
        if (currentDescription != null && currentDescription.Length > 0)
        {
            str += " and " + currentDescription;
            str += " to get it.";
        }
        else
        {
            str += ", but dont know where to get it.";
        }
        return str;
    }

    public override void TickDay()
    {
        if (questRequested.DaysPassed() > 5)
        {
            if (creature.quest == null)
            {
                if (stack != null)
                {
                    creature.map.storyManager.RequestQuest(creature, this, new PlayerAction(ActionType.get_item, Point.None, stack),
                        "I want you to get me these, because " + source.description
                    );
                }
                else
                {
                    creature.map.storyManager.RequestQuest(creature, this, new PlayerAction(ActionType.get_item, Point.None, type),
                        "I want you to get me these, because " + source.description
                    );
                }
                questRequested = TimeManager.Now();
            }
        }
    }
}

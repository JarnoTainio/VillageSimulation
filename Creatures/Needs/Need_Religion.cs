using System.Collections.Generic;

public class Need_Vanity : Need
{
    MonsterTag hatedType;
    ItemType valuedType;

    ItemType useItem;
    ObjectType objectType;

    public Need_Vanity(Creature creature, int strength, int growTime, string name, string description, MonsterTag hatedType, ItemType valuedType, ItemType useItem, ObjectType objectType) : base(creature, name, description, strength, growTime)
    {
        this.hatedType = hatedType;
        this.valuedType = valuedType;
        this.useItem = useItem;
        this.objectType = objectType;
    }

    public override List<Goal> CreateGoals()
    {
        List<Goal> goals = new List<Goal>();

        // Kill hated enemy
        Memory_Location closestMonster = null;
        foreach(Memory_Location memory in creature.memory.locations.Get())
        {
            if (memory.HasMonster() && memory.GetMonster().IsType(hatedType))
            {
                if (closestMonster == null || creature.GetHome().Distance(memory.point) < creature.GetHome().Distance(closestMonster.point))
                {
                    closestMonster = memory;
                }
            }
        }
        if (closestMonster != null)
        {
            goals.Add(new Goal_Kill(this, strength, closestMonster, "I hate that monster"));
        }

        // Get valued item
        if (creature.inventory.Count(valuedType) < 5)
        {
            goals.Add(new Goal_Get(this, strength, valuedType, 1));
        }

        // Idle action
        int str = strength + lastTicked.DaysPassed();
        Goal_Custom goal = new Goal_Custom(this, name, "practice " + name, str, null);
        Plan plan = new Plan(goal, "practicing " + name, strength);
        plan.actions.Add(new Idle(plan, strength, name, 30));
        goal.actions = plan.actions;
        goals.Add(goal);

        if (creature.inventory.coins > 10)
        {
            creature.map.storyManager.RequestQuest(creature, null, new PlayerAction(ActionType.use_item, Point.None, useItem, new object[] { objectType }), "Could you do this for me?");
        }
        
        return goals;
    }
}

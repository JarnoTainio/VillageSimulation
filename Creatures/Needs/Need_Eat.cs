
using System.Collections.Generic;

public class Need_Eat : Need
{
    public Need_Eat(Creature creature, int strength, int growTime) : base(creature, "Eat", "Im hungry", strength, growTime)
    {
    }

    public override List<Goal> CreateGoals()
    {
        List<Goal> goals = new List<Goal>();

        // Eat if hungry
        if (creature.hunger >= 1)
        {
            if (creature.inventory.Contains(ItemType.Food))
            {
                goals.Add(new Goal_Eat(this, GetValue(1f)));
            }
            else
            {
                goals.Add(new Goal_Get(this, GetValue(.9f), ItemType.Food, 3));
            }
        }

        return goals;
    }

}

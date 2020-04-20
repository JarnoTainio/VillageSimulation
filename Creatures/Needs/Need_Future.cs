
using System.Collections.Generic;

public class Need_Future : Need
{
    public Need_Future(Creature creature, int strength, int growTime) : base(creature, "Future", "Im securing my future", strength, growTime)
    {
    }

    public override List<Goal> CreateGoals()
    {
        List<Goal> goals = new List<Goal>();

        // Have a home
        if (creature.hasHome)
        {
            int inventoryCount = creature.inventory.Count(ItemType.Food);
            if (inventoryCount < 5)
            {
                goals.Add(new Goal_Get(this, GetValue(), ItemType.Food, 5));
            }
        }

        return goals;
    }

}

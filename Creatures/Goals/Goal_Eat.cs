using System.Collections.Generic;

public class Goal_Eat : Goal
{
    public Goal_Eat(Need source, int strength) : base(source, "Eat", "I want to eat", strength)
    {
    }

    public override Plan CreatePlan()
    {
        Plan plan = new Plan(this, "Eat", strength);

        // Check if inventory contains food
        if (creature.inventory.Contains(ItemType.Food))
        {
            // Get all items that are food
            ItemStack[] stacks = creature.inventory.GetByType(ItemType.Food);
            Item item = null;
            float value = 0;

            // Find most caloeries giving food
            foreach(ItemStack stack in stacks)
            {
                Food f = stack.item as Food;
                float v = f.calories;
                if (item == null || value < v)
                {
                    item = stack.item;
                    value = v;
                }
            }

            // Add plan to eat the food
            plan.Add(new Eat(plan, strength, item));
            return plan;
        }

        // No food in inventory
        else
        {
            // Store best food so far
            Item food = null;
            Point target = new Point();
            float value = 0;

            // Loop through memories
            foreach (Memory_Item mem in creature.memory.items.Get(creature.ID))
            {
                if (mem.stack.item.IsType(ItemType.Food)){
                    Food item = mem.stack.item as Food;
                    float v = creature.location.Distance(mem.point) / item.calories;
                    if (food == null || v < value)
                    {
                        target = mem.point;
                        value = v;
                        food = item;
                    }
                }
            }
            if (food == null)
            {
                return null;
            }

            plan.name += " " + food.name;

            // Add traveling path to target
            AddTravelActions(plan, target);
            plan.Add(new Get(plan, strength, food));
            plan.Add(new Eat(plan, strength, food));
            return plan;
        }
    }

    public override bool IsSatisfied()
    {
        return creature.hunger < 1;
    }

    public override string ToText()
    {
        string str = "";
        if (creature.inventory.Contains(ItemType.Food))
        {
            str = "I want to eat my food.";
        }
        else
        {
            str = "Im trying to get food, so I can eat.";
        }
        return str;
    }

    public override void TickDay()
    {
        if (questRequested.DaysPassed() > 1)
        {
            if (creature.hungerState > Hunger.Little && creature.quest == null)
            {
                creature.map.storyManager.RequestQuest(creature, this, new PlayerAction(ActionType.get_item, Point.None, ItemType.Food),
                    "Please get me some food to eat"
                );
                questRequested = TimeManager.Now();
            }
        }
    }
}

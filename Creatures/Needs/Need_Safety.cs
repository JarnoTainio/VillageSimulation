using System.Collections.Generic;

public class Need_Safety : Need
{
    public Need_Safety(Creature creature, int strength, int growTime) : base(creature, "Safety", "I want to be safe", strength, growTime)
    {
    }

    public override List<Goal> CreateGoals()
    {
        List<Goal> goals = new List<Goal>();

        if (creature.inventory.coins > 50 && creature.daysLived > (creature as Villager).oldAge * .8f)
        {
            creature.map.storyManager.RequestQuest(creature, null, new PlayerAction(ActionType.use_item, Point.None, ItemType.Potion_Age, new object[] { creature }), "I am getting too old. Could you make me young again?");
        }

        // Own a home
        if (creature.hasHome)
        {
            // Explore nearby area
            int areaToCover = 10;
            int nearbyDistance = 2;
            int count = 0;
            int oldest = 0;
            Point point = Point.None;

            foreach(Memory_Location mem in creature.memory.locations.Get())
            {
                int distance = creature.GetHome().Distance(mem.point);
                if (distance <= 3)
                {
                    foreach (int mo in mem.objects)
                    {
                        if (creature.inventory.coins > 10 && MapObject.Objects[mo].IsType(ObjectType.tunnel))
                        {
                            creature.map.storyManager.RequestQuest(creature, null, new PlayerAction(ActionType.use_item, mem.point, ObjectType.tunnel), "That tunnel looks dangerous");
                        }
                    }
                }
                if (distance <= nearbyDistance)
                {
                    count++;
                    int age = mem.LastSeen() - distance;
                    if (age > oldest)
                    {
                        oldest = age;
                        point = mem.point;
                    }
                }
            }

            // Explore nearby area and old locations
            if (count < areaToCover)
            {
                goals.Add(new Goal_Explore(this, GetValue(1.8f - (float)count / areaToCover), nearbyDistance, true, "Nearby area"));
            }

            else if (oldest > 7 * ( 1.5f - creature.personality.curiosity))
            {
                goals.Add(new Goal_Travel(this, GetValue(.5f + oldest / 5f), point, "Recheck"));
            }

            // Fill food reserves
            int food = creature.inventory.Count(ItemType.Food);
            if (food < 7)
            {
                goals.Add(new Goal_Get(this, GetValue(.75f, -food * 5), ItemType.Food, 7 - food));
            }

            foreach(Memory_Building building in creature.memory.buildings.GetOwned(creature.ID))
            {
                foreach(Point dir in Direction.Directions)
                {
                    Memory_Location mem = creature.memory.locations.Get(building.point.Add(dir));
                    if (mem == null)
                    {
                        goals.Add(new Goal_Explore(this, strength, creature.GetHome().Distance(building.point.Add(dir)), true, "Explore property surroundings"));
                    }
                    else if (mem.HasMonster())
                    {
                        goals.Add(new Goal_Kill(this, (int)(strength * (creature.personality.GetSkill(Skill.combat) / 80f)), mem, mem.GetMonster().name + " nearby"));
                    }
                }
            }

        }

        // No home
        else
        {
            // Explore before settling down
            if (creature.memory.locations.VisitedCount() < 14 * (creature.personality.patience + creature.personality.curiosity + 1f))
            {
                goals.Add(new Goal_Explore(this, GetValue(), 2, true, "Before settling down"));
            }

            // Build home
            List<Memory_Building> buildings = creature.memory.buildings.GetOwned(creature.ID);
            if (buildings.Count > 0)
            {
                foreach (Memory_Building mem in buildings)
                {
                    if (mem.HasTag(BuildingTag.Bed))
                    {
                        creature.SetHome(mem.point);
                        return goals;
                    }
                }
                foreach (Memory_Building mem in buildings)
                {
                    if (mem.constructionResult == Schematic.House.building.id)
                    {
                        goals.Add(new Goal_Build(this, GetValue(.75f), Schematic.House));
                        return goals;
                    }
                }
            }
            //Point location = creature.memory.GetBuildingPoint(Schematic.House);
            //if (!location.Equals(Point.None))
            {
                goals.Add(new Goal_Build(this, GetValue(.75f), Schematic.House));
            }
        }
        return goals;
    }

    

}

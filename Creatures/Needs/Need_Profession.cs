using System.Collections.Generic;

public class Need_Profession : Need
{
    public Need_Profession(Creature creature, int strength, int growTime) : base(creature, "Profession", "I want to work", strength, growTime)
    {
    }

    public override List<Goal> CreateGoals()
    {
        List<Goal> goals = new List<Goal>();

        // Get job only if you have a home
        if (!creature.hasHome)
        {
            return goals;
        }

        // Work own buildings
        List<Memory_Building> ownedBuildings = creature.memory.buildings.GetOwned(creature.ID);
        Memory_Construction construction = null;
        Memory_Building oldest = null;
        int age = 1;

        // Get current best working value
        float max = .5f;
        foreach (Memory_Building mem in ownedBuildings)
        {
            SkillWork w = mem.work;
            if (w != null)
            {
                float v = w.GetValue(creature) / (w.oncePerDay ? 2 : 1);
                if (v > max)
                {
                    max = v;
                }
            }
        }

        foreach (Memory_Building mem in ownedBuildings)
        {
            int lastSeen = mem.LastSeen();
            if (lastSeen > age)
            {
                oldest = mem;
                age = lastSeen;
            }
            if (mem is Memory_Construction)
            {
                    construction = mem as Memory_Construction;
            }
            else { 

                if (mem.CouldBeWorked(creature) && creature.inventory.Count(mem.work.produced.item) < 28)
                {
                    float e = mem.work.GetValue(creature);
                    int str = (int)(strength * e / max);
                    goals.Add(new Goal_Work(this, str, mem));
                }

                if (mem.durability < 80f)
                {
                    float mod = mem.durability < 50 ? 1f : 1f - mem.durability / 200f;
                    if (mem.work != null)
                    {
                        mod *= mem.work.GetValue(creature) / max;
                    }
                    else
                    {
                        mod = mem.HasTag(BuildingTag.Bed) ? 1f : .75f;
                    }
                    goals.Add(new Goal_Maintain(this, (int)(strength * mod), mem));
                }
            }
        }

        if (construction != null)
        {
            goals.Add(new Goal_Build(this, strength, construction.schematic));
        }

        else if (oldest != null)
        {
            goals.Add(new Goal_Travel(this, (int)(strength * .75f), oldest.point, "Check property"));
        }

        if (goals.Count > 0)
        {
            return goals;
        }

        bool canBuild = true;
        int waitDays = (int)(creature.personality.patience * 10);
        
        foreach (Memory_Event mem in creature.memory.events.actions)
        {
            if (mem.e.action == EventAction.build && mem.e.result == EventResult.finished && mem.time.DaysPassed() < waitDays)
            {
                canBuild = false;
                break;
            }
        }
        ItemStackList othersProduce = new ItemStackList();
        ItemStackList othersRequire = new ItemStackList();

        foreach (Memory_Agent mem in creature.memory.social.Get())
        {
            othersProduce.Add(mem.produces);
            othersRequire.Add(mem.requires);
        }

        int foodRequired = 0;
        int foodProduced = 0;
        foreach (ItemStack s in othersProduce.list)
        {
            if (s.item.IsType(ItemType.Food))
            {
                foodProduced += s.count;
            }
        }
        foreach (ItemStack s in othersRequire.list)
        {
            if (s.item.IsType(ItemType.Food))
            {
                foodRequired += s.count;
            }
        }

        if (canBuild) {

            // Todo: Has source of food
            bool sourceOfFood = creature.memory.HasSourceOfFood();

            // Build new building
            float best = float.MinValue;
            Schematic schematic = null;
            Point closest = Point.None;

            foreach (Schematic current in Schematic.schematics)
            {
                Building b = current.building;

                // Produces goods
                if (b != null && b.work != null && b.work.produced != null)
                {
                    if (!sourceOfFood && !b.work.produced.IsType(ItemType.Food))
                    {
                        continue;
                    }

                    // Find closest usable tile
                    Point closestTile = creature.memory.locations.GetClosest(creature.GetHome(), current.requiredTile, true);

                    // Check alternative tiles
                    if (current.requiredTile == Tile.Grass)
                    {
                        Point alternativeTile = creature.memory.locations.GetClosest(creature.GetHome(), Tile.Forest, true);
                        if (closest.Equals(Point.None))
                        {
                            closestTile = alternativeTile;
                        }
                        else if (creature.GetHome().Distance(alternativeTile) < creature.GetHome().Distance(closestTile))
                        {
                            closestTile = alternativeTile;
                        }
                    }
                    if (closestTile.Equals(Point.None))
                    {
                        continue;
                    }
                    
                    float value = b.work.GetValue(creature);
                    int distanceToClosest = closestTile.Distance(creature.GetHome());

                    // See if already have better work that can be worked all the time
                    bool alreadyFound = false;
                    foreach(Memory_Building mem in ownedBuildings)
                    {
                        if (mem.work != null)
                        {
                            if (!mem.work.oncePerDay)
                            {
                                float v = mem.work.GetValue(creature);
                                if (v >= value)
                                {
                                    alreadyFound = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (alreadyFound)
                    {
                        continue;
                    }

                    value *= 100f;
                    value -= distanceToClosest * 5;

                    // Check if can get materials
                    bool canGetMaterials = true;
                    foreach(ItemStack required in current.required)
                    {
                        if (!creature.memory.CanGet(required.item))
                        {
                            canGetMaterials = false;
                            break;
                        }
                    }
                    if (!canGetMaterials)
                    {
                        continue;
                    }

                    // Already too much produced reduces value
                    value -= othersProduce.Count(b.work.produced.item);
                    bool alreadyProduced = othersProduce.Count(b.work.produced.item) > 0;
                    if (!alreadyProduced)
                    {
                        value += 40;
                    }
                    value -= creature.memory.Produced(b.work.produced.item) / 5;

                    // Amount of product required increases value
                    if (b.work.produced.item.IsType(ItemType.Food))
                    {
                        value += foodRequired - foodProduced / 4;
                    }
                    else
                    {
                        value += othersRequire.Count(b.work.produced.item) * 2;
                        value += creature.memory.Require(b.work.produced.item);
                    }

                    // Requires material
                    if (b.work.required != null)
                    {
                        if (creature.memory.Produced(b.work.required.item) == 0)
                        {
                            value -= 40;
                        }
                        // Increaase value if producing required material
                        value += creature.memory.Produced(b.work.required.item);
                        value += othersProduce.Count(b.work.required.item);
                        if (!alreadyProduced && othersProduce.Count(b.work.required.item) > 10)
                        {
                            value += 40;
                        }

                        // Decrease value if using required material
                        if (!b.work.required.item.IsType(ItemType.Food))
                        {
                            value -= creature.memory.Require(b.work.required.item);
                            value -= othersRequire.Count(b.work.required.item);
                        }
                    }

                    // Multiply result by skill and passion
                    if (value > 0)
                    {
                        value *= (creature.personality.GetSkill(b.work.skill) + 1) / 10f * creature.personality.GetPassion(b.work.skill);
                    }

                    //creature.PrintMessage("Build: " + current.building + " value: " + value + " skill: " + creature.personality.GetSkill(current.building.work.skill) +" motivation: " + creature.personality.GetPassion(current.building.work.skill), false);

                    if (value > best)
                    {
                        best = value;
                        schematic = current;
                        closest = closestTile;
                    }
                }
            }

            if (schematic != null)
            {
                goals.Add(new Goal_Build(this, strength, schematic));
            }
        }

        // Sell extra items
        ItemStackList producedItems = new ItemStackList();
        foreach(Memory_Building mem in creature.memory.buildings.GetOwned(creature.ID))
        {
            if (mem.work?.produced != null)
            {
                producedItems.Add(mem.work.produced);
            }
        }
        foreach(Counter c in creature.memory.itemCounter.list)
        {
            int count = creature.inventory.Count(Item.items[c.id]);
            int required = othersRequire.Count(Item.items[c.id]);

            if (c.total > 5 && producedItems.Contains(c.id) && count > 5 && required > 5)
            {
                int amount = count - 5;
                if (amount > 10)
                {
                    amount = 10;
                }
                goals.Add(new Goal_Sell(this, new ItemStack(Item.items[c.id], amount), (int)(strength * (count / 24f)) ));
            }
        }

        return goals;
    }

}

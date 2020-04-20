using System.Collections;
using System.Collections.Generic;

public class Construction : Building
{
    public Schematic schematic;
    public List<Resource> resourceStack;
    public Tile wantedTile;

    public Construction(Map map, Point point, Schematic schematic, SkillWork work, Tile wantedTile = null) : base(map, point, "Construction" + (schematic != null ? schematic.building.name : ""), new BuildingTag[] {}, work, null)
    {
        this.schematic = schematic;
        this.wantedTile = wantedTile;

        if (schematic != null)
        {
            workLeft = schematic.workTime + ((wantedTile != null || wantedTile != schematic.requiredTile) ? 600 : 0);

            resourceStack = new List<Resource>();
            foreach (Resource r in schematic.materials)
            {
                resourceStack.Add(r.Copy());
            }
        }
    }

    public override void Init()
    {

    }

    public bool HasAllMaterials()
    {
        foreach (Resource r in resourceStack)
        {
            if (!r.resourceAdded)
            {
                return false;
            }
        }
        return true;
    }

    public override ItemStack AddMaterial(ItemStack stack)
    {
        bool found = false;
        foreach(Resource r in resourceStack)
        {
            // See if item can be added
            if (r.AddResource(stack.item))
            {
                Repair(10f);

                // Add work that can be done
                workLeft += r.duration;
                
                // Reduce one from the stack
                stack.count -= 1;

                // If stack runs out, then break
                if (stack.count == 0)
                {
                    found = true;
                    break;
                }
            }
        }

        if (!found)
        {
            return stack;
        }

        // Remove resources that have been added from the stack
        found = true;
        while (found)
        {
            found = false;
            Resource res = null;
            foreach (Resource r in resourceStack)
            {
                if (r.resourceAdded)
                {
                    found = true;
                    res = r;
                    break;
                }
            }
            if (res != null)
            {
                resourceStack.Remove(res);
            }
        }

        // Return remaining items
        return stack;
    }

    public override int Work(Creature creature, int delta)
    {
        workLeft -= delta;
        Repair(delta);

        if (workLeft <= 0)
        {
            int remaining = workLeft;
            workLeft = 0;

            // No work left and has all materials, so it building is completed
            if (HasAllMaterials())
            {
                Building b = schematic.building.Copy();
                b.point = point;
                b.map = map;
                map.Replace(this, b);
                if (wantedTile != null)
                {
                    map.Replace(point, wantedTile);
                    creature.inventory.Add(new ItemStack(Item.Wood, 24));
                }
                map.Event(point, new Event(creature.ID, creature.location, EventAction.build, EventResult.finished, b));
            }
            return remaining;
            
        }
        return 0;
    }

    public override bool CanBeWorked(Creature creature) {
        return workLeft > 0;
    }

    public override Building Copy()
    {
        Construction copy = new Construction(map, point, schematic, work)
        {
            workLeft = workLeft,
            resourceStack = new List<Resource>(resourceStack)
        };
        CopyStats(copy);
        return copy;
    }

    public override void TickDay(Map map)
    {
        if (!isActive)
        {
            return;
        }
        Damage(10f);
        if (durability < 25)
        {
            int roll = random.Next((int)(durability * 10));
            if (roll == 0)
            {
                isActive = false;
                map.Event(point, new Event(-1, point, EventAction.destroy, EventResult.success, this));
                return;
            }
        }
    }

    public override void Update(Building building)
    {
        if (building is Construction c)
        {
            c.workLeft = workLeft;
            c.resourceStack = new List<Resource>(resourceStack);
            base.Update(building);
        }
    }

    public List<ItemStack> GetRequiredMaterials()
    {
        List<ItemStack> list = new List<ItemStack>();
        foreach(Resource r in resourceStack)
        {
            if (r.resourceAdded)
            {
                continue;
            }
            bool found = false;
            foreach (ItemStack stack in list)
            {
                if (stack.item.Equals(r.item))
                {
                    stack.count++;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                list.Add(new ItemStack(r.item, 1));
            }
        }
        return list;
    }
}

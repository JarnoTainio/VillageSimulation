using System;
using System.Collections.Generic;

public class Building
{
    public static int IdentifierCounter;
    public int identifier;

    public Map map;
    public int id;
    public Point point;
    protected Random random;

    public string name;
    public float durability;
    public SkillWork work;
    public MaintainWork repair;
    public int workLeft;
    public int workQueue;

    public bool isActive;

    public List<BuildingTag> tags;

    public Building(Map map, Point point, string name, BuildingTag[] tags, SkillWork work, MaintainWork repair)
    {
        identifier = IdentifierCounter++;
        this.map = map;
        this.point = point;
        this.name = name;
        this.tags = new List<BuildingTag>(tags);
        this.work = work;
        this.repair = repair;
        durability = 100;
        isActive = true;
        random = new Random();
    }

    public virtual void Init()
    {
        random = new Random();
        workLeft = (work != null && work.required == null) ? work.worktime : 0;
        workQueue = 0;
        durability = 100f;
        isActive = true;

        // Add treasure to dungeons
        if (HasTag(BuildingTag.Dungeon))
        {
            for (int i = 0; i < 5; i++)
            {
                Item item = Item.items[random.Next(Item.items.Length)];
                if (item.IsType(ItemType.Treasure))
                {
                    map.Add(new ItemStack(item), point);
                    map.GetLocation(point).CreateObstacle(random);
                    break;
                }
            }
        }
        
    }

    public void End()
    {
        if (HasTag(BuildingTag.Spawner))
        {
            // Remove source from locations?
        }
    }

    public virtual void TickDay(Map map)
    {
        if (!isActive)
        {
            return;
        }
        if (repair != null)
        {
            Damage(3f);
        }
        if (durability < 25)
        {
            int roll = random.Next((int)(durability * 10) + 1);
            if (roll == 0)
            {
                //Dummy.PrintMessage(this + " at " + point + " COLLAPSED!");
                isActive = false;
                map.Event(point, new Event(-1, point, EventAction.destroy, EventResult.success, this));
                return;
            }
        }
        if (work != null && work.required == null && workLeft == 0)
        {
            workLeft = work.worktime;
        }

        // Monster spawning
        if (HasTag(BuildingTag.Spawner))
        {
            workLeft += random.Next(5);
            Monster monster = map.GetLocation(point).GetMonster();
            if (monster != null && monster.IsBoss())
            {
                workLeft += random.Next(5);
            }
            if (workLeft > 20)
            {
                if (monster != null)
                {
                    int count = 0;
                    for (int x = -3; x <= 3; x++)
                    {
                        for (int y = -3; y <= 3; y++)
                        {
                            Point p = point.Add(x, y);
                            if (map.WithinBounds(p))
                            {
                                Location loc = map.GetLocation(point.Add(x, y));
                                if (loc.HasMonster())
                                {
                                    count++;
                                    if (loc.GetSource() == this)
                                    {
                                        count++;
                                    }
                                }
                            }
                        }
                        workLeft = 15 - count * count / 4;
                    }

                    // Chance to skip spawning
                    if (random.Next(100) < count - 5)
                    {
                        return;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        int x = random.Next(4) * (random.Next(2) == 0 ? 1 : -1);
                        int y = random.Next(4) * (random.Next(2) == 0 ? 1 : -1);
                        Point spawn = point.Add(x, y);

                        if (map.WithinBounds(spawn) && !map.GetLocation(spawn).HasMonster())
                        {
                            map.Add(monster.GetMinion(), this, spawn);
                            break;
                        }
                    }
                }
                
            }
        }
    }

    public virtual void Update(Building building)
    {
        building.name = name;
        building.point = point;
    }

    public virtual int Work(Creature creature, int delta)
    {
        if (!isActive || work == null)
        {
            return delta - 1;
        }

        if (workLeft <= 0)
        { 
            if (workQueue <= 0)
            {
                return delta - 1;
            }
            workQueue--;
            workLeft = work.worktime;
        }

        workLeft -= delta;
        if (workLeft <= 0)
        {
            if (work.required == null)
            {
                workLeft = work.oncePerDay ? 0 : work.worktime;
            }
            else
            {
                if (workQueue > 0)
                {
                    workQueue--;
                    workLeft = work.worktime;
                }
                else
                {
                    workLeft = 0;
                }
                
            }

            work.Work(map, creature, this);
            return workLeft;
        }

        return 0;
    }

    public virtual bool CanBeWorked(Creature creature)
    {
        if (!isActive)
        {
            return false;
        }
        if (creature.map.GetLocation(point).HasMonster())
        {
            return false;
        }
        if (workLeft > 0 || workQueue > 0)
        {
            return true;
        }
        if (work == null)
        {
            return false;
        }
        return false;
    }

    public virtual bool CouldBeWorked(Creature creature)
    {
        if (!isActive)
        {
            return false;
        }
        if (workLeft > 0 || workQueue > 0)
        {
            return true;
        }
        if (work == null)
        {
            return false;
        }
        if (work.required != null)
        {
            return creature.memory.CanGet(work.required.item);
        }
        return false;
    }

    public virtual ItemStack AddMaterial(ItemStack stack) {
        if (!isActive || work == null || work.required == null)
        {
            Dummy.PrintMessage("Why are you dropping that here? " +this + " " + point + " " + stack);
            return stack;
        }
        if (stack.item.Equals(work.required.item))
        {
            if (workLeft <= 0)
            {
                workLeft = work.worktime;
            }
            else
            {
                workQueue++;
            }
            return new ItemStack(stack.item, stack.count -1);
        }
        Dummy.PrintMessage("Why are you dropping that here? " + this + " " + point + " " + stack);
        return stack;
    }

    public bool Repair(Creature creature)
    {
        if (!isActive)
        {
            return false;
        }
        if (repair.required == null)
        {
            Repair(repair.value);
            return true;
        }
        else if (creature.inventory.Contains(repair.required))
        {
            creature.inventory.Remove(repair.required);
            creature.memory.itemCounter.Consumed(repair.required);
            Repair(repair.value);
            return true;
        }
        return false;
    }

    public void Repair(float amount)
    {
        durability += amount;
        if (durability > 100f)
        {
            durability = 100f;
        }
    }

    public void Damage(float amount)
    {
        durability -= amount;
        if (durability < 0f)
        {
            durability = 0f;
        }
    }

    public virtual Building Copy()
    {
        Building copy = new Building(map, point, name, tags.ToArray(), work, repair);
        CopyStats(copy);
        return copy;
    }

    public virtual void CopyStats(Building copy){
        copy.workLeft = workLeft;
        copy.workQueue = workQueue;
        copy.durability = durability;
        copy.id = id;
        copy.isActive = isActive;
    }

    public bool HasTag(BuildingTag tag)
    {
        return tags.Contains(tag);
    }

    public bool Equals(Building building)
    {
        if (building == null)
        {
            return false;
        }
        return Equals(building.point, building.id);
    }

    public bool Equals(Point point, int id)
    {
        return this.point.Equals(point) && this.id == id;
    }

    public bool IsSource(Item item) {
        if (item == null || Source() == null)
        {
            return false;
        }
        return Source().Equals(item);
    }

    public bool IsSource(ItemType type)
    {
        if (Source() == null)
        {
            return false;
        }
        return Source().IsType(type);
    }

    public bool Needs(Item item)
    {
        return Need().Equals(item);
    }

    public Item Source()
    {
        return work?.produced?.item;
    }

    public Item Need()
    {
        return work?.required.item;
    }

    public override string ToString()
    {
        return name;
    }

    public string DescriptiveString()
    {
        string str = name;
        if (durability < .25f)
        {
            str += " that looks ready to collapse";
        }
        else if (durability < .50f)
        {
            str += " that has seen better days";
        }
        else if (durability < .75f)
        {
            str += " that is little broken";
        }
        return str;
    }
}

public enum BuildingTag {Bed, Dungeon, Spawner}

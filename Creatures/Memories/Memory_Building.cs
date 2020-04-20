
public class Memory_Building : Memory
{
    public readonly Point point;
    public int ownerID;

    public SkillWork work;
    public MaintainWork repair;
    public BuildingTag[] tags;
    public float durability;
    public int workLeft;
    public int workQueue;
    public int buildingId;
    public int constructionResult;

    public Time lastProduced;

    public Memory_Building(int power, Building building, int ownerID = -1) : base(power)
    {
        this.ownerID = ownerID;
        point = building.point;
        constructionResult = -1;
        Update(building);
    }

    public virtual void Update(Building building)
    {
        UpdateSeen();

        // Copy stats
        tags = building.tags.ToArray();
        work = building.work;
        repair = building.repair;
        durability = building.durability;
        workLeft = building.workLeft;
        workQueue = building.workQueue;
        buildingId = building.id;
        if (building is Construction)
        {
            constructionResult = (building as Construction).schematic.building.id;
        }
    }

    public void Produced()
    {
        lastProduced = TimeManager.Now();
    }

    public bool CanBeWorked(Creature creature)
    {
        if (workLeft > 0 || workQueue > 0)
        {
            return true;
        }
        if (work == null)
        {
            return false;
        }
        if (work.required == null && work.oncePerDay && LastSeen() > 0)
        {
            return true;
        }
        return false;
    }

    public bool CouldBeWorked(Creature creature)
    {
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
        if (work.required == null && work.oncePerDay && LastSeen() > 0)
        {
            return true;
        }
        return false;
    }

    public bool Equals(Building building)
    {
        if (building == null)
        {
            return false;
        }
        return building.id == buildingId && building.point.Equals(point);
    }

    public bool IsType(Building building)
    {
        return building.id == buildingId;
    }

    public Item Source()
    {
        return work?.produced?.item;
    }

    public bool IsSource(ItemType type)
    {
        if (Source() == null)
        {
            return false;
        }
        return Source().IsType(type);
    }

    public bool IsSource(Item item)
    {
        if (item == null || Source() == null)
        {
            return false;
        }
        return Source().Equals(item);
    }

    public bool HasTag(BuildingTag tag)
    {
        foreach(BuildingTag t in tags)
        {
            if (t == tag)
            {
                return true;
            }
        }
        return false;
    }

    public string GetName()
    {
        return Schematic.schematics[buildingId].building.name;
    }

    public override string ToString()
    {
        string str = "";
        // ToDo: Update this to phantom
        str += buildingId + " at " + point;
        str += " (seen " + lastSeen.ToShortString() + " )";
        return str;
    }

    public bool IsOwned(int ID)
    {
        return ownerID == ID;
    }
}

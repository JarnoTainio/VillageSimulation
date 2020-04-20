
public class Resource
{
    public Item item;
    public int duration;

    public bool resourceAdded;

    public Item tool;

    public Resource(Item item, int duration)
    {
        this.item = item;
        this.duration = duration;
        this.tool = null;
        resourceAdded = false;
    }

    public bool AddResource(Item item)
    {
        if (!resourceAdded && this.item == item)
        {
            resourceAdded = true;
            return true;
        }
        return false;
    }

    public int ReduceTime(int delta)
    {
        duration -= delta;
        return duration;
    }

    public bool CanWork(Creature creature)
    {
        if (tool == null || creature.inventory.Contains(tool))
        {
            return true;
        }
        return false;
    }

    public Resource Copy()
    {
        Resource r = new Resource(item, duration);
        r.tool = tool;
        r.resourceAdded = resourceAdded;
        return r;
    }
}

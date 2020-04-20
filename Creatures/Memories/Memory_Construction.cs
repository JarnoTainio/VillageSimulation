using System.Collections;
using System.Collections.Generic;

public class Memory_Construction : Memory_Building
{
    public Schematic schematic;
    public List<ItemStack> requiredResources;

    public Memory_Construction(int power, Construction construction, int ownerID = -1) : base(power, construction, ownerID)
    {
        schematic = construction.schematic;
    }

    public override void Update(Building building)
    {
        requiredResources = new List<ItemStack>();
        Construction construction = building as Construction;
        schematic = construction.schematic;
        foreach (Resource r in construction.resourceStack)
        {
            bool found = false;
            foreach (ItemStack s in requiredResources)
            {
                if (s.Equals(r.item))
                {
                    s.count++;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                requiredResources.Add(new ItemStack(r.item, 1));
            }
        }
        base.Update(construction);
    }
}


using System.Collections.Generic;

public class Need_Curiosity : Need
{
    public Need_Curiosity(Creature creature, int strength, int growTime) : base(creature, "Curiosity", "Im curious", strength, growTime)
    {
    }

    public override List<Goal> CreateGoals()
    {
        List<Goal> goals = new List<Goal>();
        int count = 0;
        int oldest = 0;

        Point home = creature.GetHome();

        foreach(Memory_Location mem in creature.memory.locations.Get())
        {
            int age = mem.LastSeen();
            if (home.Distance(mem.point) <= creature.personality.searchDistance)
            {
                count++;
                if (age > oldest)
                {
                    oldest = age;
                }
            }
        }
        int range = creature.personality.searchDistance * 2 + 1;
        float explorationValue = (float)count / (range * range);

        if (explorationValue < .9f)
        {
            goals.Add(new Goal_Explore(this, GetValue(explorationValue), creature.personality.searchDistance, true, "curiosity"));
        }

        else if  (oldest > 7)
        {
            goals.Add( new Goal_Explore(this, GetValue(oldest / 28f), creature.personality.searchDistance, false, "recheck"));
        }

        if (creature.hasHome)
        {
            foreach (Memory_Location mem in creature.memory.locations.Get())
            {
                if (!mem.searched)
                {
                    Memory_Building mb = creature.memory.buildings.GetAtPoint(mem.point);
                    if (mb != null && mb.buildingId == Schematic.Ruins.id)
                    {
                        goals.Add(new Goal_Search(this, strength, mem, "interesting looking ruins"));
                    }
                }
            }
        }
        
        return goals;
    }

}

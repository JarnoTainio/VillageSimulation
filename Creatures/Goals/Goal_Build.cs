using System.Collections.Generic;

public class Goal_Build : Goal
{
    public Schematic schematic;
    public Memory_Construction phantom;
    public Point location;

    public Goal_Build(Need source, int strength, Schematic schematic) : base(source, "Build " + schematic.building.name, "I want to build "+ schematic.building.name, strength)
    {
        this.schematic = schematic;
    }

    private bool FindPhantom(int counter)
    {
        if (counter == 0)
        {
            return false;
        }

        Memory_Building mem = creature.memory.buildings.GetAtPoint(location);
        if (mem == null)
        {
            return false;
        }

        // Taken by another
        else if (mem.ownerID != creature.ID)
        {
            // Set new location
            location = creature.memory.GetBuildingPoint(schematic);
            return FindPhantom(counter -1);
        }

        // My construction
        else if (mem is Memory_Construction)
        {
            phantom = mem as Memory_Construction;
            return true;
        }

        return false;
    }

    public override Plan CreatePlan()
    {

        if (creature.CanWork())
        {
            return null;
        }

        bool seeker = creature.memory.locations.Get().Count > 300;
        bool found = FindPhantom(3);
        if (!found)
        {
            location = creature.memory.GetBuildingPoint(schematic);
        }
        // No suitable location found
        if (location.Equals(Point.None))
        {
            return null;
        }

        // We have construction, but not phantom, so construction is completed
        if (!found && phantom != null)
        {
            return null;
        }

        // If not found, then build one
        if (phantom == null)
        {
            //location = creature.memory.GetBuildingPoint(schematic);
            if (!creature.location.Equals(location))
            {
                Plan plan = new Plan(this, "Travel to " + location, strength, .8f);
                AddTravelActions(plan, location);
                return plan;
            }
            //return GoAndBuild(schematic, location, strength, replaceTile);
            return GoAndBuild(schematic, location, 0);
        }

        if (phantom.CanBeWorked(creature))
        {
            return GoAndWork(phantom, strength);
        }


        // Loop through required materials
        foreach (ItemStack required in phantom.requiredResources)
        {
            // If in inventory, then drop it to the construction site
            Plan plan = GoAndDrop(phantom, required, strength);
            if (plan != null)
            {
                return plan;
            }

            // If owned items in ground, then go get them
            plan = GoAndGet(required, creature.memory.items.Get(creature.ID), strength);
            if (plan != null)
            {
                return plan;
            }

            // Check if resource can be produced from owned buildings
            plan = GoAndProduce(required, creature.memory.buildings.GetOwned(creature.ID), strength);
            if (plan != null)
            {
                return plan;
            }

            // Check if resource can be produced from tiles
            plan = GoAndProduceFromTile(required, creature.memory.locations.Get(), strength);
            if (plan != null)
            {
                return plan;
            }
        }

        Point target = FindNewLocation(creature.personality.searchDistance, true);
        if (!target.Equals(-1, -1))
        {
            Plan plan = new Plan(this, "Explore", strength, .5f);
            AddTravelActions(plan, target);
            return plan;
        }
        return null;
    }

    public override bool IsSatisfied()
    {
        Memory_Building building = creature.memory.buildings.GetAtPoint(location);

        // Same location, same name as wanted building and same type as schematics building
        if (phantom != null && building != null && building.IsType(schematic.building))
        {
            return true;
        }
        return false;
    }

    public override string ToText()
    {
        string str = "I want to build a " +schematic.building;
        if (phantom != null)
        {
            str += ". I have already started working on it";
            List<ItemStack> neededItems = phantom.requiredResources;
            if (neededItems.Count > 0)
            {
                str += ", but I still need";
                for(int i = 0; i < neededItems.Count; i++)
                {
                    // First
                    if (i == 0)
                    {
                        str += " ";
                    }
                    // Last
                    else if (i == neededItems.Count - 1)
                    {
                        str += " and ";
                    }
                    else
                    {
                        str += ", ";
                    }
                    str += neededItems[i];
                }
                str += ".";
            }
            else if (phantom.workLeft > 120)
            {
                str += " and it still needs some work.";
            }
            else
            {
                str += " and it's almost complete!";
            }
            str += " Would you like to help me with this?";
        }
        else
        {
            str += ", but I havent yet decided where to build it.";
        }
        return str;
    }
}

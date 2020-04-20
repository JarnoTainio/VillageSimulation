using System.Collections;
using System.Collections.Generic;
public class QuestFind : QuestStep
{
    Point point;
    MapObject mapObject;
    Building building;
    Monster monster;
    string targetString;
    bool isCompleted;

    public QuestFind(Quest quest, QuestType type, object target, Point point, Point[] reservedPoints) : base(quest, type, reservedPoints)
    {
        if (target is MapObject)
        {
            mapObject = target as MapObject;
            targetString = mapObject.name;
        }
        else if (target is Building)
        {
            building = target as Building;
            targetString = building.name;
        }
        else if (target is Monster)
        {
            monster = target as Monster;
            targetString = monster.name;
        }
        else
        {
            throw new System.Exception("Not understood: " + target);
        }
        this.point = point;
        if (point.Equals(Point.None))
        {
            targetString = "any " + targetString;
        }
        else
        {
            targetString = "specific " + targetString;
        }
        request = "Find " + targetString;
    }

    public override void Start()
    {
        string reason = "";
        if (quest.step > 0)
        {
            reason += "Now that you " + quest.GetPreviousStep().history + ", ";
        }
        reason += "I want you to find " + targetString;
        history += "you found ";
        if (!point.Equals(Point.None))
        {
            reason += ", but It must be specific one and It must be found";
            history += "the";
        }
        history += targetString;
        string why = TypeToString();
        if (why.Length > 0)
        {
            reason += ", because I need to " + why;
        }
        reason += ".";
        QuestStep previous = (quest.step > 0 ? quest.events[quest.step - 1] : null);
        agentQuest = new AgentQuest(this, questGiver, reason, previous);
        questGiver.quest = agentQuest;
    }

    public override void Observe(PlayerAction action, Location location)
    {
        if (agentQuest.IsHidden())
        {
            return;
        }
        if (isCompleted)
        {
            return;
        }

        if (!point.Equals(Point.None))
        {
            if (location.point.Equals(point))
            {
                agentQuest.TaskCompleted();
                isCompleted = true;
            }
        }
        else
        {
            if (monster != null) 
            {
                if (location.GetMonster().Equals(monster))
                {
                    agentQuest.TaskCompleted();
                    isCompleted = true;
                }
            }
            else if (mapObject != null)
            {
                if (location.Contains(mapObject))
                {
                    agentQuest.TaskCompleted();
                    isCompleted = true;
                }
            }
            else if (building != null)
            {
                if (location.building != null && location.building.id == building.id)
                {
                    agentQuest.TaskCompleted();
                    isCompleted = true;
                }
            }
        }
    }

    public override string AlreadyDone()
    {
        return "You have already found " + targetString + "? Thats good!";
    }

    public override string Help(Creature creature)
    {
        if (point.Equals(Point.None))
        {
            return GetHelpAny(creature);
        }
        return GetHelpForPoint(creature);
    }

    private string GetHelpAny(Creature creature)
    {
        if (monster != null)
        {
            List<Memory_Location> monsterLocations = new List<Memory_Location>();
            foreach (Memory_Location mem in creature.memory.locations.Get())
            {
                Monster m = mem.GetMonster();
                if (m.Equals(monster))
                {
                    monsterLocations.Add(mem);
                }
            }
            if (monsterLocations.Count > 0)
            {
                string str = "I have seen those things around at";
                for(int i = 0; i < monsterLocations.Count; i++)
                {
                    if (i == 0)
                    {

                    }
                    else if (i == monsterLocations.Count - 1)
                    {
                        str += " and";
                    }
                    else
                    {
                        str += ",";
                    }
                    str += " " + monsterLocations[i].point;
                }
                str += ".";
                return str;
            }
            return "I dont know anything about that.";
        }
        else if (mapObject != null)
        {
            List<Memory_Location> objectLocations = new List<Memory_Location>();
            foreach (Memory_Location mem in creature.memory.locations.Get())
            {
                foreach(int i in mem.objects)
                {
                    if (i == mapObject.ID)
                    {
                        objectLocations.Add(mem);
                        break;
                    }
                }
            }
            if (objectLocations.Count > 0)
            {
                string str = "I have seen " + mapObject.name + "s  around at";
                for (int i = 0; i < objectLocations.Count; i++)
                {
                    if (i == 0)
                    {

                    }
                    else if (i == objectLocations.Count - 1)
                    {
                        str += " and";
                    }
                    else
                    {
                        str += ",";
                    }
                    str += " " + objectLocations[i].point;
                }
                str += ".";
                return str;
            }
            return "I dont know anything about that.";
        }
        else if (building != null)
        {
            List<Memory_Building> buildingLocations = new List<Memory_Building>();
            foreach (Memory_Building mem in creature.memory.buildings.Get())
            {
                if (mem.buildingId == building.id)
                {
                    buildingLocations.Add(mem);
                }
            }
            if (buildingLocations.Count > 0)
            {
                string str = "I have seen " + building.name + "s  around at";
                for (int i = 0; i < buildingLocations.Count; i++)
                {
                    if (i == 0)
                    {

                    }
                    else if (i == buildingLocations.Count - 1)
                    {
                        str += " and";
                    }
                    else
                    {
                        str += ",";
                    }
                    str += " " + buildingLocations[i].point;
                }
                str += ".";
                return str;
            }
            return "I dont know anything about that.";
        }
        return null;
    }

    private string GetHelpForPoint(Creature creature)
    {
        if (monster != null)
        {
            foreach (Memory_Location mem in creature.memory.locations.Get())
            {
                if (mem.point.Equals(point))
                {
                    string str = "";
                    if (mem.HasMonster())
                    {
                        str = "I think you can find that monster at " + mem.point;
                        if (mem.LastSeen() >= 7)
                        {
                            str += ", but it has been " + mem.LastSeen() + " days since I saw it.";
                        }
                    }
                    else
                    {
                        str = "I have visited the place you are looking for, but there wasnt any monster there";
                        if (mem.LastSeen() < 7)
                        {
                            str += " and I visited there recently.";
                        }
                        else
                        {
                            str += ". Altough, it has been " + mem.LastSeen() + " days since I last was there.";
                        }
                        str += " If you are still interested, it is at " + mem.point + ".";
                    }
                    return str;
                }
            }
            return "I dont know anything about that.";
        }
        else if (mapObject != null)
        {
            foreach (Memory_Location mem in creature.memory.locations.Get())
            {
                if (mem.point.Equals(point))
                {
                    string str = "";
                    bool isKnown = false;
                    foreach (int i in mem.objects)
                    {
                        if (i == mapObject.ID)
                        {
                            isKnown = true;
                            break;
                        }
                    }
                    if (isKnown)
                    {
                        str = "I think you can find " + mapObject.name + " at " + mem.point;
                        if (mem.LastSeen() >= 7)
                        {
                            str += ", but it has been " + mem.LastSeen() + " days since I saw it.";
                        }
                    }
                    else
                    {
                        str = "I have visited the place you are looking for, but there wasnt any " + mapObject.name + " there";
                        if (mem.LastSeen() < 7)
                        {
                            str += " and I visited there recently.";
                        }
                        else
                        {
                            str += ". Altough, it has been " + mem.LastSeen() + " days since I last was there.";
                        }
                        str += " If you are still interested, it is at " + mem.point + ".";
                    }
                    return str;
                }
            }
            return "I dont know anything about that.";
        }
        else if (building != null)
        {
            Memory_Building mem = creature.memory.buildings.GetAtPoint(point);
            {
                if (mem != null)
                {
                    string str = "";
                    if (mem.buildingId == building.id)
                    {
                        str = "I think you can find that " + building.name + " at " + mem.point;
                        if (mem.LastSeen() >= 7)
                        {
                            str += ", but it has been " + mem.LastSeen() + " days since I saw it.";
                        }
                    }
                    else
                    {
                        str = "I have visited the place you are looking for, but there wasnt any " + building.name + " there";
                        if (mem.LastSeen() < 7)
                        {
                            str += " and I visited there recently.";
                        }
                        else
                        {
                            str += ". Altough, it has been " + mem.LastSeen() + " days since I last was there.";
                        }
                        str += " If you are still interested, it is at " + mem.point + ".";
                    }
                    return str;
                }
                return "I dont know anything about that.";
            }
        }
        return null;
    }

    public override bool IsSatisfied()
    {
        foreach (PlayerMemory mem in questGiver.map.player.memory.memories)
        {
            if (mem.action.point.Equals(point))
            {
                return true;
            }
        }
        return false;
    }

    public override string ToString()
    {
        return "find " + targetString;
    }

    public override int GetValue()
    {
        if (point.Equals(Point.None))
        {
            return 15;
        }
        return questGiver.GetHome().Distance(point);
    }

    public override string ToSimpleString()
    {
        return "Find[" + (int)type + "](" + targetString + ")";
    }
}

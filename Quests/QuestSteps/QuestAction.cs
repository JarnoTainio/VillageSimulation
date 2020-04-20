using System.Collections;
using System.Collections.Generic;

public class QuestAction : QuestStep
{
    readonly PlayerAction wantedAction;
    bool isCompleted;
    Obstacle obstacle;

    public QuestAction(Quest quest, QuestType type, PlayerAction wantedAction, Point[] reservedPoints) : base(quest, type, reservedPoints)
    {
        this.wantedAction = wantedAction;
        isCompleted = false;
        request = wantedAction.ToString();
    }

    public override void Start()
    {
        string reason = "";
        if (quest.step > 0)
        {
            reason += "Now that you " + quest.GetPreviousStep().history + ", ";
        }
        reason += "I want you to ";
        reason += ActionToString(wantedAction);
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
        if (isCompleted)
        {
            return;
        }
        if (agentQuest.IsHidden())
        {
            return;
        }
        if (wantedAction.action == ActionType.explore)
        {
            if (location.GetObstacle() != null)
            {
                obstacle = location.GetObstacle();
            }
        }
        if (action.Equals(wantedAction))
        {
            UpdateMemory(action, location);
            agentQuest.TaskCompleted();
            isCompleted = true;
        }
        else if (location.point.Equals(wantedAction.point))
        {
            if (!wantedAction.IsPossible(location))
            {
                agentQuest.Impossible();
            }
        }
        else if (wantedAction.action == ActionType.use_item && wantedAction.point.Equals(Point.None)){
            if (wantedAction.args != null) {

                // Use item at creature
                if (wantedAction.args[0] is Creature c && location.creatures.Contains(c))
                {
                    agentQuest.TaskCompleted();
                    isCompleted = true;
                    return;
                }

                // Use item at mapobject
                if (wantedAction.args[0] is ObjectType t && location.Contains(t))
                {
                    agentQuest.TaskCompleted();
                    isCompleted = true;
                    return;
                }

            }
        }
    }

    public override bool IsSatisfied()
    {
        return isCompleted;
    }

    protected string ActionToString(PlayerAction action)
    {
        string str = "";
        history = "";
        Memory_Location location = questGiver.memory.locations.Get(action.point);
        Memory_Building building = questGiver.memory.buildings.GetAtPoint(action.point);

        switch (action.action)
        {
            case ActionType.explore:

                Monster monster = location?.GetMonster();
                // Quest request
                str = "explore ";
                
                if (building != null)
                {
                    str += building.GetName() + " ";
                }
                str += "at " + action.point;

                if (monster != null)
                {
                    str += " that has " + monster.name + " in there";
                }

                // History
                history = "you explored ";
                if (building != null)
                {
                    history += "the " + building.GetName();
                }
                else
                {
                    history += "that location";
                }
                break;

            case ActionType.kill:
                monster = wantedAction.target as Monster;
                str = "kill "; 
                if (monster == null)
                {
                    str += " NULL";
                    break;
                }
                if (monster.IsBoss())
                {
                    str += "deadly";
                }
                str += monster.name + " at location " + action.point;
                history = "you killed that " + monster.name; 
                break;

            case ActionType.use_item:
                str += "Use " + wantedAction.target + " at ";
                if (wantedAction.target != null){
                    str += wantedAction.target.ToString();
                }
                if (wantedAction.args != null)
                {
                    str += wantedAction.args[0].ToString();
                }
                break;
        }
        return str;
    }

    public override string ToString()
    {
        return ActionToString(wantedAction);
    }

    public override string Help(Creature creature)
    {
        if (obstacle != null)
        {
            return "There is " + obstacle.description + "? You need " + obstacle.key + " to pass.\n" + ItemHelp(creature, obstacle.key);
            
        }
        return "I quess you should go to " + wantedAction.point + " and do it?";
    }

    public override string AlreadyDone()
    {
        return "You have already done that? Thats good!";
    }

    public override int GetValue()
    {
        Location location = null;
        if (!wantedAction.point.Equals(Point.None)) {
            location = questGiver.map.GetLocation(wantedAction.point);
        }
        switch (wantedAction.action)
        {
            case ActionType.kill:
                Monster monster = location.GetMonster();
                if (monster != null)
                {
                    return monster.IsBoss() ? 50 : 5;
                }
                break;

            case ActionType.explore:
                if (location.building != null && location.building.HasTag(BuildingTag.Dungeon))
                {
                    if (location.GetObstacle() != null)
                    {
                        return 30;
                    }
                    return 10;
                }
                break;
        }
        return 5;
    }

    public override bool CanUse(Item item, Location location)
    {
        if (wantedAction.action == ActionType.use_item)
        {
            if (wantedAction.point.Equals(Point.None))
            {
                if (wantedAction.target is ObjectType)
                {
                    return location.Contains((ObjectType) wantedAction.target);
                }
            }
            else if (wantedAction.point.Equals(location.point))
            {
                return true;
            }
        }
        return false;
    }

    public override string ToSimpleString()
    {
        string str = "Action[" + (int)type + "](" + wantedAction.action;
        if (!wantedAction.point.Equals(Point.None))
        {
            str += ", " + wantedAction.point;
        }
        if (wantedAction.target != null)
        {
            str += ", " + wantedAction.target.ToString();
        }
        if (wantedAction.args != null)
        {
            str += ", " + wantedAction.args[0].ToString();
        }
        str += ")";
        return str;
    }
}

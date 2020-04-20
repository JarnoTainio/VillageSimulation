using System.Collections.Generic;

public abstract class QuestStep
{
    public Quest quest;
    protected QuestType type;
    public Creature questGiver;
    protected Time started;
    protected AgentQuest agentQuest;

    public Point[] reservedPoints;

    public string history;
    public string request;

    public QuestStep(Quest quest, QuestType type, Point[] reservedPoints)
    {
        this.quest = quest;
        this.questGiver = quest.questGiver;
        this.type = type;
        this.reservedPoints = reservedPoints;
        started = TimeManager.Now();
    }

    public abstract void Start();

    public virtual void End(bool completed)
    {
        if (quest.currentEvent == this)
        {
            questGiver.quest = null;
            FreeReserverd();
            if (completed)
            {
                quest.NextStep();
                questGiver.map.player.CompleteQuest(this);
            }
        }
        else
        {
            Dummy.PrintMessage("TRYING TO END WRONG QUEST_STEP");
        }
    }

    public void FreeReserverd()
    {
        foreach (Point p in reservedPoints)
        {
            quest.storyManager.FreeReserve(p);
        }
    }

    public abstract void Observe(PlayerAction action, Location location);
    public abstract bool IsSatisfied();

    public void UpdateMemory(PlayerAction action, Location location)
    {
        if (action.action == ActionType.kill)
        {
            Memory_Location memory = questGiver.memory.locations.Get(action.point);
            if (memory != null)
            {
                memory.KillMonster();
            }
        }
        else if (action.action == ActionType.explore)
        {
            Memory_Location memory = questGiver.memory.locations.Get(action.point);
            if (memory != null)
            {
                memory.Search();
            }
        }
    }

    public string TypeToString()
    {
        string str = "";
        switch (type)
        {
            case QuestType.goal:
                break;

            case QuestType.research:
                str += "to research why this is happening";
                break;

            case QuestType.test:
                str += "to test if you are strong enough";
                break;

            case QuestType.solution:
                str += "so we can progress";
                break;

            case QuestType.before:
                str += "before we can move forward";
                break;

            case QuestType.first:
                str += "before we can move forward";
                break;

            case QuestType.opportunity:
                str += "I need it dealt anyway and this is nice bonus";
                break;
        }
        return str;
    }

    public abstract string Help(Creature creature);
    public abstract string AlreadyDone();

    public override string ToString()
    {
        return request;
    }

    public string ItemHelp(Creature creature, Item item)
    {
        List<Memory_Agent> producingAgents = new List<Memory_Agent>();
        foreach (Memory_Agent mem in creature.memory.social.Get())
        {
            if (mem.LastSeen() < 7 && mem.Produces(item))
            {
                producingAgents.Add(mem);
            }
        }
        if (producingAgents.Count > 0)
        {
            if (producingAgents.Count == 1)
            {
                return "Maybe " + producingAgents[0].name + " could help you with that.";
            }
            else
            {
                return "There are many who can help you with that. Ask " + producingAgents[0].name + " or " + producingAgents[1].name + ".";
            }
        }
        else
        {
            return "I dont know anyone who produces " + item.name + ".";
        }
    }

    public virtual bool CanUse(Item item, Location location)
    {
        return false;
    }

    public abstract int GetValue();

    public abstract string ToSimpleString();
}

public enum QuestType { goal, research, test, solution, before, first, opportunity }
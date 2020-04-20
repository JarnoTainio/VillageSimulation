using System.Collections;
using System.Collections.Generic;

public class QuestKill : QuestStep
{
    protected Monster monster;
    protected int count;
    protected int killed;

    public QuestKill(Quest quest, QuestType type, Monster monster, int count, Point[] reservedPoints) : base(quest, type, reservedPoints)
    {
        this.monster = monster;
        this.count = count;
        request = "Kill " + count + " " + monster.name;
    }

    public override void Start()
    {
        string reason = "";
        if (quest.step > 0)
        {
            reason += "Now that you " + quest.events[quest.step - 1].history + ", ";
        }
        reason += "I want you to slay " + count + " " + monster.name + "s";
        string why = TypeToString();
        if (why.Length > 0)
        {
            reason += ", " + why;
        }
        reason += ".";
        history = "you killed those " + monster.name + "s";
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
        if (count == killed)
        {
            return;
        }

        if (action.action == ActionType.kill && (action.target as Monster).Equals(monster))
        {
            killed++;
            UpdateMemory(action, location);
            if (killed == count)
            {
                agentQuest.TaskCompleted();
            }
        }
    }

    public override bool IsSatisfied()
    {
        return killed >= count;
    }

    public override string ToString()
    {
        return "kill " + count + " " + monster.name + "s";
    }

    public override string Help(Creature creature)
    {
        List<Point> monsters = new List<Point>();
        foreach (Memory_Location mem in creature.memory.locations.Get())
        {
            if (mem.LastSeen() < 5)
            {
                Monster m = mem.GetMonster();
                if (m != null && m.Equals(monster))
                {
                    monsters.Add(mem.point);
                }
            }
        }
        if (monsters.Count == 0)
        {
            return "I havent seen those around.";
        }
        else if (monsters.Count == 1)
        {
            return "I have seen one at " + monsters[0] + "."; 
        }
        else if (monsters.Count < 4)
        {
            return "I have seen couple around. Go look at " + monsters[0] + " or " + monsters[1] +".";
        }
        return "Those things are all around, shouldn't be hard to find.";
    }

    public override string AlreadyDone()
    {
        return "You have already killed those " + monster + "s?";
    }

    public override int GetValue()
    {
        if (monster.IsBoss())
        {
            return 50 * count;
        }
        return 4 * count;
    }

    public override string ToSimpleString()
    {
        return "Kill[" + (int)type + "](" + monster.name + " x " + count + ")";
    }
}

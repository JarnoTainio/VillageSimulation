using System.Collections;
using System.Collections.Generic;

public class QuestCraft : QuestStep
{
    protected SkillWork craft;
    protected Player player;

    public QuestCraft(Quest quest, QuestType type, SkillWork craft, Point[] reservedPoints) : base(quest, type, reservedPoints)
    {
        this.craft = craft;
        player = questGiver.map.player;
        request = "Craft " + craft.produced;
    }

    public override void Start()
    {
        string reason = "";
        if (quest.step > 0)
        {
            reason += "Now that you " + quest.events[quest.step - 1].history + ", ";
        }
        reason += "We need to craft " + craft.produced;
        string why = TypeToString();
        if (why.Length > 0)
        {
            reason += ", we need it " + why;
        }
        reason += ".";
        history = "you now have " + craft.produced.item.name;
        QuestStep previous = (quest.step > 0 ? quest.events[quest.step - 1] : null);
        agentQuest = new AgentQuest(this, questGiver, reason, previous);
        agentQuest.craft = craft;
        questGiver.quest = agentQuest;
    }

    public override void Observe(PlayerAction action, Location location)
    {
        if (agentQuest.IsHidden())
        {
            return;
        }
        if (action.action == ActionType.get_item && player.inventory.Contains(craft.produced))
        {
            agentQuest.TaskCompleted();
        }
    }

    public override bool IsSatisfied()
    {
        return player.inventory.Contains(craft.produced);
    }

    public override string ToString()
    {
        return "craft " + craft.produced;
    }

    public override string Help(Creature creature)
    {
        if (player.inventory.Contains(craft.required))
        {
            if (creature.Equals(questGiver))
            {
                return "You seem to have everything, so let's craft that " + craft.produced.item.name + ".";
            }
            return "You seem to have everything, so go to " + questGiver.name + " and craft that " + craft.produced.item.name + ".";
        }
        string str = "You need more " + craft.required.item.name + " to craft " + craft.produced.item.name + ".";
        List<Memory_Agent> producingAgents = new List<Memory_Agent>();
        foreach(Memory_Agent mem in creature.memory.social.Get())
        {
            if (mem.LastSeen() < 7 && mem.Produces(craft.required.item))
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
            return "I dont know anyone who produces " + craft.required.item.name + ".";
        }
    }

    public override string AlreadyDone()
    {
        return "You already have that? Thats good!";
    }

    public override int GetValue()
    {
        return 5;
    }

    public override string ToSimpleString()
    {
        return "Craft[" + (int)type + "](" + craft.produced.item.name + ")";
    }
}
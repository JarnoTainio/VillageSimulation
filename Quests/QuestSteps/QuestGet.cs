using System.Collections;
using System.Collections.Generic;

public class QuestGet : QuestStep
{
    protected ItemStack stack;
    protected Player player;

    public QuestGet(Quest quest, QuestType type, ItemStack stack, Point[] reservedPoints) : base(quest, type, reservedPoints)
    {
        this.stack = stack;
        player = questGiver.map.player;
        request = "Get " + stack;
    }

    public override void Start()
    {
        string reason = "";
        if (quest.step > 0)
        {
            reason += "Now that you " + quest.events[quest.step - 1].history + ", ";
        }
        reason += "I want you to get " + stack;
        string why = TypeToString();
        if (why.Length > 0)
        {
            reason += ", " + why;
        }
        reason += ".";
        history = "you got the " + stack.item.name;
        QuestStep previous = (quest.step > 0 ? quest.events[quest.step - 1] : null);
        agentQuest = new AgentQuest(this, questGiver, reason, previous);
        questGiver.quest = agentQuest;
    }

    public override void End(bool completed)
    {
        if (completed)
        {
            // Remove wanted items
            for (int i = 0; i < stack.count; i++)
            {
                Dummy.instance.playerController.Remove(stack.item);
            }
            questGiver.inventory.Add(stack);
        }
        base.End(completed);
    }

    public override void Observe(PlayerAction action, Location location)
    {
        if (agentQuest.IsHidden())
        {
            return;
        }
        if (player.inventory.Contains(stack))
        {
            agentQuest.TaskCompleted();
        }
        else if (agentQuest.IsCompleted())
        {
            agentQuest.UnCompleted();
        }
    }

    public override bool IsSatisfied()
    {
        return player.inventory.Contains(stack);
    }

    public override string ToString()
    {
        return "get " + stack;
    }

    public override string Help(Creature creature)
    {
        return "So you need " + stack.item.name + "?\n" + ItemHelp(creature, stack.item);
    }

    public override string AlreadyDone()
    {
        return "You already have " + stack + " ? Thats good!";
    }

    public override int GetValue()
    {
        return stack.GetValue();
    }

    public override string ToSimpleString()
    {
        return "Get[" + (int)type + "](" + stack.item.name + " x " + stack.count + ")";
    }
}
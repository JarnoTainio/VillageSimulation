using System.Collections;
using System.Collections.Generic;

public class Quest
{
    public List<QuestStep> events;
    public QuestStep currentEvent;

    public StoryManager storyManager;
    public Creature questGiver;
    public Goal goal;
    public string reason;
    Map map;

    public ItemStack reward;
    public int rewardCoins;

    public int step;

    public bool isActive;

    public Quest(StoryManager storyManager, Creature creature, Goal goal, string reason)
    {
        this.storyManager = storyManager;
        events = new List<QuestStep>();
        questGiver = creature;
        map = creature.map;
        isActive = true;
        this.goal = goal;
        this.reason = reason;
    }

    public void AddStep(QuestStep step)
    {
        foreach(Point p in step.reservedPoints)
        {
            storyManager.Reserve(p, ActionType.reserve);
        }
        events.Add(step);
    }

    public void Start()
    {
        step = 0;
        currentEvent = events[0];
        currentEvent.Start();
    }

    public void End(bool completed)
    {
        if (completed)
        {
            if (reward != null)
            {
                for (int i = 0; i < reward.count; i++)
                {
                    Dummy.instance.playerController.Add(reward.item);
                }
            }
            if (rewardCoins > 0)
            {
                Dummy.instance.playerController.ModifyMoney(rewardCoins);
            }
        }
        //currentEvent.End();

        storyManager.Remove(this, true);
    }

    public void Abandon()
    {
        currentEvent.End(false);
        for(int i = step; i < events.Count; i++)
        {
            events[i].FreeReserverd();
        }
        storyManager.Remove(this, true);
    }

    public void Observe(PlayerAction action, Location location)
    {
        currentEvent.Observe(action, location);
    }

    public void TickDay(StoryManager manager)
    {
        if (!isActive)
        {
            return;
        }
    }

    public void NextStep()
    {
        step++;
        Dummy.PrintMessage("Next step " + step + "/" + events.Count);
        if (step == events.Count)
        {
            Dummy.PrintMessage("QUEST CHAIN COMPLETED!");
            End(true);
            isActive = false;
            return;
        }
        currentEvent = events[step];
        currentEvent.Start();
    }

    public bool IsCompleted()
    {
        if (events[events.Count - 1].IsSatisfied())
        {
            return true;
        }
        return step >= events.Count;
    }

    public bool IsValid()
    {
        return questGiver.alive;
    }

    public QuestStep GetPreviousStep()
    {
        if (step == 0)
        {
            return null;
        }
        return events[step - 1];
    }

    public QuestStep GetNext()
    {
        if (step < events.Count - 1)
        {
            return events[step + 1];
        }
        return null;
    }

    public override string ToString()
    {
        string str = questGiver.name + "(" + events.Count + ")";
        str += "\n" + (goal != null ? goal.name : "null") + ": " + reason;
        foreach(QuestStep step in events)
        {
            str += "\n -" + step;
        }
        return str;
    }
}

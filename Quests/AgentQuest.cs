using System.Collections;
using System.Collections.Generic;

public class AgentQuest
{
    public QuestStep quest;
    public QuestStep previous;
    readonly Creature creature;

    public readonly Time created;
    Time acceptedTime;

    readonly string description;
    public QuestState state;

    public SkillWork craft;

    public AgentQuest(QuestStep quest, Creature creature, string description, QuestStep previous = null)
    {
        this.quest = quest;
        this.creature = creature;
        this.description = description;
        this.previous = previous;
        state = QuestState.available;
        created = TimeManager.Now();
    }

    public int GetWaitTime()
    {
        int baseTime = 7 * (quest.quest.events.Count * 2);
        int chainBonus = quest.quest.step * 4;
        int acceptedBonus = state == QuestState.available ? 0 : 14;

        return baseTime + chainBonus + acceptedBonus;
    }

    public void Abandon()
    {
        quest.quest.Abandon();
        creature.quest = null;
    }

    // Remove quest from questgiver and mark quest as finished
    public void End()
    {
        quest.End(state == QuestState.completed);
        state = QuestState.finished;
    }

    // Player accepts quest from questgiver
    public void AcceptQuest()
    {
        state = QuestState.accepted;
        acceptedTime = TimeManager.Now();
    }

    // Observe action performed by player.
    // This is not known by questgiver and only used to track quest progression
    public void Observe(PlayerAction action, Location location)
    {
        // Quest is not completed and player enters quest location, but wanted task is not possible,
        // then the quest is impossible.
        if (state != QuestState.completed && location.point.Equals(action.point) && !location.CanBePerformed(action.action))
        {
            End();
            state = QuestState.impossible;
        }

        if (state == QuestState.hidden)
        {
            return;
        }
    }

    public void TaskCompleted()
    {
        state = QuestState.completed;
    }

    public void UnCompleted()
    {
        state = QuestState.accepted;
    }

    public void Impossible()
    {
        state = QuestState.impossible;
    }

    public string GetDialogue(Player player)
    {
        string str = "";

        if (state == QuestState.available)
        {
            if (quest.quest.GetPreviousStep() == null)
            {
                str = "I have a quest for you!\n" + description;
                str += quest.quest.reason;
                bool and = false;
                bool reward = false;
                if (quest.quest.reward != null)
                {
                    str += "\nFor completing this quest chain, I offer you " + quest.quest.reward;
                    and = true;
                    reward = true;
                }
                if (quest.quest.rewardCoins > 0)
                {
                    if (and)
                    {
                        str += " and";
                    }
                    else
                    {
                        str += "\nFor completing this quest chain, I offer you ";
                    }
                    str += quest.quest.rewardCoins + " coins";
                    reward = true;

                }
                if (reward)
                {
                    str += " as a reward.";
                }
                str += "\nAre you interested?";
            }
            else if (quest.quest.GetNext() == null)
            {
                str = "Let's continue our quest, shall we?\n" + description + "\nAre you interested?";
            }
            else
            {
                str = "This is last step in our quest!\n" + description + "\nAre you interested?";
            }

            if (quest.IsSatisfied())
            {
                str += "\n" + quest.AlreadyDone();
                End();
            }
        }
        else if (state == QuestState.accepted)
        {
            str = "Need a reminder?\n" + description;
        }

        else if (state == QuestState.impossible)
        {
            str = "It's impossible, isn't it..";
            End();
        }

        else if (state == QuestState.failed)
        {
            str = "You failed!";
            End();
        }

        else if (state == QuestState.hidden)
        {
            str = "This quest should be hidden! (You shouldnt see this message)";
        }

        else if (state == QuestState.completed)
        {
            str = "So " + quest.history +"?\n";
            
            if (quest.quest.events.Count == 1)
            {
                str += "Thank you for the help!";
            }
            else if (quest.quest.GetPreviousStep() == null)
            {
                str += "Now we can truly begin!";
            }
            else if (quest.quest.step == quest.quest.events.Count - 2)
            {
                str += "Time for one last task!";
            }
            else if (quest.quest.step == quest.quest.events.Count - 1)
            {
                str += "Thank you for all the things you did!";
            }
            else
            {
                str += "Good work, but I have new task for you!";
            }
            creature.playerReputation += 5;

            player.AddExperience(5);
            creature.QuestCompleted();
            End();
        }
        else if (state == QuestState.preDone)
        {
            str += "I have a quest for you, could you.. oh... you already completed it?\n Heres half of the reward, I quess..";
            state = QuestState.finished;
            player.AddExperience(2);
            End();
        }

        return str;
    }

    // Quest is available if state is available
    public bool IsAvailable()
    {
        return state == QuestState.available;
    }

    // QUest is completed if state is completed
    public bool IsCompleted()
    {
        return state == QuestState.completed;
    }

    public bool IsHidden()
    {
        return state == QuestState.hidden || state == QuestState.available;
    }
}

public enum QuestState {
    hidden,     // Quest has not been yet realized by questgiver
    available,  // Being offered by questgiver
    accepted,   // Aaccepted by player
    completed,  // Task is completed, but not yet returned
    finished,   // Completed and returned
    failed,     // Failed by player's actions
    impossible, // Not possible anymore
    preDone     // Completed without being accepted
}

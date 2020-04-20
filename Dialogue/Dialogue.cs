using System.Collections;
using System.Collections.Generic;


public class Dialogue
{
    public Player player;
    public Creature creature;
    public ItemStackList buy;
    public ItemStackList sell;
    Point point;

    public Dialogue(Player player)
    {
        this.player = player;
    }

    public void SetCreature(Creature creature)
    {
        this.creature = creature;
        point = creature.location;
        creature.memory.PlayerKnowledge(player.memory);
    }

    public string Description()
    {
        string str = "You see an villager.";
        return str;
    }

    public string Hello()
    {
        string str = "";
        if (!creature.hasTalkedToPlayer)
        {
            if (creature.playerReputation == 0)
            {
                str = "Hello, my name is " + creature.name + ", I am " + creature.daysLived + " days old.";
            }
            else if (creature.playerReputation < 5)
            {
                str = "I have heard of you, my name is " + creature.name + ", nice to meet you.";
            }
            else if (creature.playerReputation < 10)
            {
                str = "I have heard of all the things you have done to us, my name is " + creature.name + ", nice to meet you!";
            }
            else
            {
                str = "Nice to meet you! You are the hero everyone is talking about! My name is " + creature.name + ", how can I help you?";
            }
        }
        else
        {
            if (creature.playerReputation < 5)
            {
                str = "Hello again, my name is " + creature.name;
            }
            else if (creature.playerReputation < 25)
            {
                str = "Hello again, friend.";
            }
            else
            {
                str = "Hello there, hero!";
            }
        }
        creature.hasTalkedToPlayer = true;
        return str;
    }

    public string Home()
    {
        string str = "";
        if (creature.hasHome)
        {
            int d = point.Distance(creature.GetHome());
            if (d == 0)
            {
                str += "This is my home.";
            }
            else
            {
                string dir = Direction.ToString(point.GetDirection(creature.GetHome()));
                str += "My home is ";
                str += (d >= 5 ? "far " : "");
                str += dir + " from here.";
            }
        }
        else
        {
            str += "I dont have a home.";
        }
        str += " " + OwnedBuildings();
        str += " " + Skills();
        return str;
    }

    public string OwnedBuildings()
    {
        string str = "";
        List<Memory_Building> buildings = creature.memory.buildings.GetOwned(creature.ID);
        if (buildings.Count == 0)
        {
            str += "I dont own any buildings..";
        }
        else
        {
            str += "I own";
            for (int i = 0; i < buildings.Count; i++)
            {
                if (i != 0 && i == buildings.Count - 1)
                {
                    str += " and";
                }
                else if (i != 0)
                {
                    str += ",";
                }
                str += " " + buildings[i].GetName();
            }
            str += ".";
        }
        return str;
    }

    public string Skills()
    {
        float value = 0f;
        string best = "none";

        float favorite = 0f;
        string liked = "none";

        foreach(Skill skill in Skill.skills)
        {
            float s = creature.personality.GetSkill(skill);
            float p = creature.personality.GetPassion(skill);
            if (s > value)
            {
                best = skill.name;
                value = s;
            }
            if (s * p > favorite)
            {
                favorite = (s + 10) * p * p;
                liked = skill.name;
            }
        }

        string str = "";
        if (best.Equals(liked))
        {
            str = "I like " + best;
            if (value < 20)
            {
                str += ", but Im not very good";
            }
            else if (value < 40)
            {
                str += " and Im ok";
            }
            else if (value < 60)
            {
                str += " and Im good";
            }
            else if (value < 80)
            {
                str += " and Im very good";
            }
            else
            {
                str += " and Im master";
            }
            str += " at it.";
        }
        else
        {
            str = "My main skill is " + best + ", but I would prefer " + liked + ".";
        }

        return str;
    }

    public string What()
    {
        string str = "";
        Action a = creature.goalManager.lastAction;
        if (a == null)
        {
            str = "Im not doing anything.";
        }
        else
        {
            str += "I am " + a.name + ", because " + a.source.source.description + ".";
            //str += "I am " + a.name + ", because " + a.source.name + ". That is because "+a.source.source.description +".";
        }
        return str;
    }

    public string Personality()
    {
        string str = "";
        // Sleep
        str += "I usually go to sleep at " + creature.personality.sleepingTime + " and wake up around " + creature.personality.awakeningTime + ".";

        bool first = true;
        float high = .75f;
        float low = .25f;

        // Greed
        float greed = creature.personality.greed;
        if (greed > high)
        {
            if (!first)
            {
                str += ",";
            }
            str += " Money is important to me";
            first = false;
        }
        else if (greed < low)
        {
            if (!first)
            {
                str += ",";
            }
            str += " Money isnt important to me";
            first = false;
        }

        // Patience
        float patience = creature.personality.patience;
        if (patience > high)
        {
            if (!first)
            {
                str += ", ";
            }
            str += " I like to take my time";
            first = false;
        }
        else if (patience < low)
        {
            if (!first)
            {
                str += ", ";
            }
            str += " I want things done fast";
            first = false;
        }

        // Social
        float social = creature.personality.social;
        if (social > high)
        {
            if (!first)
            {
                str += ", ";
            }
            str += " I love talking to people";
            first = false;
        }
        else if (social < low)
        {
            if (!first)
            {
                str += ", ";
            }
            str += " I like to be alone";
            first = false;
        }

        // Curiosity
        float curiosity = creature.personality.curiosity;
        if (curiosity > high)
        {
            if (!first)
            {
                str += ", ";
            }
            str += " I love exploring";
            first = false;
        }
        else if (curiosity < low)
        {
            if (!first)
            {
                str += ", ";
            }
            str += " I like to stay home";
            first = false;
        }

        str += ".";
        return str;
    }

    public string Today()
    {
        string str = "Today I";
        List<Memory_Event> actions = new List<Memory_Event>(creature.memory.events.actions);
        List<MemoryEvents> used = new List<MemoryEvents>();
        List<string> parts = new List<string>();
 
        foreach (Memory_Event e in creature.memory.events.actions)
        {
            if (e.e.action == EventAction.travel)
            {
                continue;
            }
            str += " " + e;
        }
        
        return str;
    }

    public string Task(int index)
    {
        index = index % creature.goalManager.GetGoals().Count;
        return creature.goalManager.GetGoals()[index].ToText();
    }

    public string Trade()
    {
        string str = "";
        sell = new ItemStackList();
        buy = new ItemStackList();
        foreach (ItemStack stack in creature.inventory.GetItems())
        {
            int required = creature.memory.Require(stack.item);
            if (required < stack.count)
            {
                sell.Add(new ItemStack(stack.item, stack.count - required));
            }
            else if (required > 0)
            {
                buy.Add(new ItemStack(stack.item, required));
            }
        }

        if (sell.list.Count > 0)
        {
            str += "I sell " + sell.ItemsToString() + ".";
        }
        if (buy.list.Count > 0)
        {
            str += "I need " + buy.ItemsToString() + " .";
        }
        if (sell.list.Count == 0 && buy.list.Count == 0)
        {
            str += "I dont have, nor need anything.";
        }
        return str;
    }

    public string Quest()
    {
        return creature.quest.GetDialogue(player);
    }

    public string Help(QuestStep quest)
    {
        Creature questGiver = quest.questGiver;
        string str = "";
        if (!questGiver.Equals(creature))
        {
            if (creature.memory.social.GetAgent(questGiver.ID) != null)
            {
                str += questGiver.name + " wants you to " + quest.ToString() + "?";
            }
            else
            {
                str += "I dont know " + questGiver.name + ". He wants you to " + quest.ToString() + "?";
            }
        }
        else
        {
            str += "I want you to " + quest.ToString() + ".";
        }
        str += "\n" + quest.Help(creature);
        return str;
    }
}

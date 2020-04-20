using System.Collections;
using System.Collections.Generic;

public class SocialManager
{
    private Creature owner;
    public Memory_Agent current;
    public int currentSteps;
    public bool listening;
    bool messageReceived;

    public SocialManager(Creature owner)
    {
        this.owner = owner;
    } 

    public void EndTalk()
    {
        current = null;
        messageReceived = false;
        listening = false;
    }

    public void TalkTo(Creature other)
    {
        if (current != null && current.ID == other.ID)
        {
            return;
        }

        current = owner.memory.social.GetAgent(other.ID);

        if (current == null)
        {
            owner.memory.social.Add(other.ID, other.location);
            current = owner.memory.social.GetAgent(other.ID);
        }
        listening = false;
        currentSteps = 0;
        current.Talking();
    }

    public bool IsTalking()
    {
        return current != null;
    }

    public bool IsListening()
    {
        return IsTalking() && listening;
    }

    public void TickTalk(int delta)
    {
        if (!messageReceived)
        {
            EndTalk();
        }
        else
        {
            messageReceived = false;
        }
    }

    public SocialMessage GetAction(int otherID)
    {
        foreach (Event e in owner.memory.events.GetToday(otherID))
        {
            
        }
        EndTalk();
        if (currentSteps == 0)
        {
            return new SocialMessage(SocialAction.info);
        }
        return new SocialMessage(SocialAction.bye);
    }

    public SocialResponse Talk(Creature other, SocialMessage message)
    {
        if (other.location.Distance(owner.location) != 0)
        {
            EndTalk();
            return SocialResponse.nothing;
        }

        if (other.Equals(owner))
        {
            return SocialResponse.nothing;
        }

        if (current != null && current.ID != other.ID)
        {
            return SocialResponse.decline;
        }

        if (current != null && current.ID == other.ID)
        {
            messageReceived = true;
            currentSteps++;
        }

        switch (message.action)
        {
            // Start conversation
            case SocialAction.hello:
                Memory_Agent mem = owner.memory.social.GetAgent(other.ID);
                if (mem.lastTalked.MinutesPassed() < 5)
                {
                    return SocialResponse.decline;
                }

                current = owner.memory.social.GetAgent(other.ID);
                if (current == null)
                {
                    current = owner.memory.social.Add(other.ID, other.location);
                }
                listening = true;
                messageReceived = true;
                currentSteps = 0;
                current.Hello(other);
                owner.map.Event(owner.location, new Event(owner.ID, owner.location, EventAction.talking, EventResult.success, message, other.ID, new SocialMessage(SocialAction.info, owner.ID, owner.memory.itemCounter, TimeManager.Now())));
                return SocialResponse.agree;

            // End conversation
            case SocialAction.bye:
                EndTalk();
                return SocialResponse.agree;
            
            // Other wants to buy items
            // Object is ItemStack
            case SocialAction.buy:
                ItemStack stack = message.target as ItemStack;
                Counter c1 = owner.memory.itemCounter.Get(stack.item);
                if (owner.inventory.Contains(stack.item))
                {
                    Dummy.instance.CreateIcon(3, owner.location);

                    // Items
                    ItemStack bought = owner.inventory.Remove(stack);
                    other.inventory.Add(bought);
                    //owner.PrintMessage("SELL " + bought + " to " + other.name, true);
                    owner.memory.itemCounter.Consumed(bought);

                    // Cost
                    int cost = bought.GetValue();
                    other.inventory.RemoveCoins(cost);
                    owner.inventory.AddCoins(cost);

                    // Send event of succesfull transaction
                    owner.map.Event(owner.location, new Event(owner.ID, owner.location, EventAction.talking, EventResult.success, message, other.ID, SocialResponse.agree));

                    // Response
                    return SocialResponse.agree;
                }

                // Send event of refusing to sell
                owner.map.Event(owner.location, new Event(owner.ID, owner.location, EventAction.talking, EventResult.success, message, other.ID, SocialResponse.decline));
                return SocialResponse.decline;

            // Other wants to sell items
            // Object is ItemStack
            case SocialAction.sell:
                stack = message.target as ItemStack;
                bool buyFood = false;
                if (stack.item.IsType(ItemType.Food) && owner.inventory.Count(ItemType.Food) < 14)
                {
                    //owner.PrintMessage("That is food and I can use it, because I have " + owner.inventory.Count(ItemType.Food), true);
                    buyFood = true;
                }

                Counter c = owner.memory.itemCounter.Get(stack.item);
                //owner.PrintMessage("Do I need to buy " + stack +", from " +other.name +". My need is " + c?.used, true);
                if (buyFood || (c != null && c.used > 3))
                {
                    Dummy.instance.CreateIcon(5, owner.location);
                    //owner.PrintMessage("BUY " + stack + " from " + other.name, true);

                    // Items
                    ItemStack sell = other.inventory.Remove(stack);
                    owner.inventory.Add(sell);

                    // Cost
                    int cost = sell.GetValue();
                    owner.inventory.RemoveCoins(cost);
                    other.inventory.AddCoins(cost);

                    // Send event of succesfull transaction
                    owner.map.Event(owner.location, new Event(owner.ID, owner.location, EventAction.talking, EventResult.success, message, other.ID, SocialResponse.agree));

                    // Response
                    return SocialResponse.agree;
                }
                // Send event of refusing to buy
                owner.map.Event(owner.location, new Event(owner.ID, owner.location, EventAction.talking, EventResult.success, message, other.ID, SocialResponse.decline));
                return SocialResponse.decline;


            case SocialAction.info:
                SocialMessage givenInfo = null;
                int roll = owner.random.Next(2);
                if (roll == 0)
                {
                    givenInfo = new SocialMessage(SocialAction.info, owner.ID, owner.memory.itemCounter, TimeManager.Now());
                }
                else if (true)
                {
                    List<Memory_Agent> list = owner.memory.social.GetKnown(7);
                    foreach(Memory_Agent a in list)
                    {
                        if (a.ID == other.ID)
                        {
                            list.Remove(a);
                            break;
                        }
                    }
                    if (list.Count == 0)
                    {
                        givenInfo = new SocialMessage(SocialAction.info, owner.ID, owner.memory.itemCounter, TimeManager.Now());
                    }
                    else
                    {
                        Memory_Agent agentMemory = list[owner.random.Next(list.Count)];
                        givenInfo = new SocialMessage(SocialAction.info, agentMemory.ID, agentMemory.BuildCounter(), agentMemory.lastTalked);
                    }
                }

                if (givenInfo == null)
                {
                    //owner.PrintMessage("EMPTY MESSAGE!", true);
                }
 
                owner.map.Event(owner.location, new Event(owner.ID, owner.location, EventAction.talking, EventResult.success, message, other.ID, givenInfo));
                return SocialResponse.agree;
        }
        EndTalk();
        return SocialResponse.nothing;
    }

}

public enum SocialAction { hello, bye, buy, sell, info }
public enum SocialResponse { agree, decline, nothing }

public class SocialMessage
{
    public SocialAction action;
    public object target;
    public object detail;
    public object info;

    public SocialMessage(SocialAction action, object target = null, object detail = null, object info = null)
    {
        this.action = action;
        this.target = target;
        this.detail = detail;
        this.info = info;
    }

    public override string ToString()
    {
        string str = action + "";
        if (target != null)
        {
            str += " target: " + target;
        }

        if (detail != null)
        {
            str += " detail: " + detail;

        }

        if (info != null)
        {
            str += " info: " + info;
        }
        return str;
    }
}
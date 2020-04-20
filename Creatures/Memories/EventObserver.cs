using System.Collections;
using System.Collections.Generic;

public class EventObserver
{
    private Creature owner;
    private MemoryManager memory;

    public EventObserver(MemoryManager memory, Creature owner)
    {
        this.owner = owner;
        this.memory = memory;
    }

    public void UnderstandEvent(Event e)
    {
        if (e.actorID == -1)
        {
            NaturalEvent(e);
        }
        else if (e.actorID == owner.ID)
        {
            EventSelf(e);
        }
        else
        {
            EventOther(e);
        }
        
    }

    public void EventSelf(Event e)
    {
        switch (e.action)
        {
            case EventAction.build:
                Building building = e.target as Building;
                if (e.result == EventResult.finished)
                {
                    memory.buildings.AddOwner(building, e.actorID);
                    // Is this home for me?
                    // ToDo: place home searching logic to needs
                    if (!owner.hasHome && building.HasTag(BuildingTag.Bed))
                    {
                        owner.SetHome(building.point);
                    }
                }
                else if (e.result == EventResult.started)
                {
                    memory.buildings.AddOwner(building, e.actorID);
                }
                break;

            case EventAction.work:
                // Work completed
                break;

            case EventAction.produce:
                
                // Produced item
                if (e.result == EventResult.success)
                {
                    // Could be produced from building
                    if (e.info is Building)
                    {
                        memory.buildings.Produced(e.point);
                        memory.itemCounter.Produced((e.target as SkillWork).produced);
                    }
                }
                break;

            case EventAction.talking:
                break;


            case EventAction.repair:
                // Didnt drop to a building
                if (e.result == EventResult.success)
                {
                    memory.itemCounter.Consumed((e.target as Building).repair.required);
                }
                break;

            case EventAction.drop:
                // Didnt drop to a building
                if (e.result == EventResult.success)
                {
                    if (e.detail == null)
                    {
                        memory.items.AddOwner((e.target as ItemStack), e.point, e.actorID);
                    }
                    // Item used on building
                    else
                    {
                        memory.itemCounter.Consumed((e.target as ItemStack));
                    }
                }
                break;

            case EventAction.get:
                if (e.result == EventResult.success)
                {
                    memory.items.RemoveOwner((e.target as ItemStack), e.point, e.actorID);
                }
                break;

            case EventAction.eat:
                // Produced item
                if (e.result == EventResult.success)
                {
                    memory.itemCounter.Consumed((e.target as ItemStack));
                }
                break;

            case EventAction.fight:
                if (e.result == EventResult.success)
                {
                    memory.locations.Get(e.point).Update(owner.map.GetLocation(owner.location));
                }
                break;

            case EventAction.search:
                if (e.result == EventResult.success)
                {
                    memory.locations.Get(e.point).Search();
                }
                break;
        }
    }

    public void EventOther(Event e)
    {
        owner.memory.social.Add(e.actorID, e.point);
        switch (e.action)
        {
            case EventAction.build:

                if (e.result == EventResult.finished)
                {
                    memory.buildings.AddOwner((e.target as Building), e.actorID);
                }
                else if (e.result == EventResult.started)
                {
                    memory.buildings.AddOwner((e.target as Building), e.actorID);
                }
                break;

            case EventAction.produce:
                // Produced item
                if (e.result == EventResult.success)
                {

                }
                break;

            case EventAction.drop:
                // Didnt drop to a building
                if (e.result == EventResult.success)
                {
                    if (e.detail == null)
                    {
                        memory.items.AddOwner((e.target as ItemStack), e.point, e.actorID);
                    }
                    // Item used on building
                    else
                    {

                    }
                }
                break;

            case EventAction.get:
                if (e.result == EventResult.success)
                {
                    memory.items.RemoveOwner((e.target as ItemStack), e.point, e.actorID);
                }
                break;

            case EventAction.travel:
                if (e.result == EventResult.success)
                {
                    // Other arrives at my location
                    if (e.point.Equals(owner.location))
                    {
                        owner.memory.social.Add(e.actorID, e.point);
                    }
                }
                break;

            case EventAction.talking:
                // Target: SocialMessage, Detail: Target of the talk, Info: SocialMessage (SocialAction, target: target.id, detail: info, info: infoTime)

                Memory_Agent mem = owner.memory.social.Add(e.actorID, e.point);
                if (e.result == EventResult.failure)
                {
                    if (mem != null)
                    {
                        //mem.Failed();
                    }

                    break;
                }

                SocialMessage message = e.target as SocialMessage;

                // Info being talked
                if (message.action == SocialAction.info && e.result == EventResult.success)
                {
                    SocialMessage info = e.info as SocialMessage;
                    Memory_Agent talkedAgent = owner.memory.social.GetAgent((int)info.target);
                    if (talkedAgent == null)
                    {
                        return;
                    }
                    if (talkedAgent.ID != e.actorID)
                    {
                        Time t = (Time)info.info;
                        if (talkedAgent.lastTalked.MinutesPassed() < t.MinutesPassed())
                        {
                            return;
                        }
                    }
                    // Info given on required and produced items
                    if (info.detail is CounterList)
                    {
                        talkedAgent.Update(info.detail as CounterList);
                    }
                }
                else if (message.action == SocialAction.buy)
                {
                    // Info: Response
                    SocialResponse socialResponse = (SocialResponse) e.info;
                    if (socialResponse == SocialResponse.decline)
                    {
                        mem.Reset((message.target as ItemStack).item);
                    }
                }
                break;

            case EventAction.fight:
                if (e.result == EventResult.success)
                {
                    memory.locations.Get(e.point).Update(owner.map.GetLocation(owner.location));
                }
                break;
        }
    }

    public void NaturalEvent(Event e)
    {
        switch (e.action)
        {

            case EventAction.destroy:
                // Building collapsed
                if (e.target != null)
                {
                    //memory.RemoveOwner(e.building);
                }
                break;
        }
    }
}

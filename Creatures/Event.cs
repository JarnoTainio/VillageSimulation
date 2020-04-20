public class Event
{
    public static int IdentifierCounter;
    public readonly int identifier;

    public Time time;

    public readonly int actorID;
    public readonly Point point;
    public readonly EventAction action;
    public EventResult result;
    public readonly Failure failure;

    public readonly object target;
    public readonly object detail;
    public readonly object info;

    public int value;
    public int duration;

    // Generic
    public Event(int actorID, Point point, EventAction action, EventResult result, object target = null, object detail = null, object info = null)
    {
        identifier = IdentifierCounter++;
        this.failure = Failure.none;
        this.point = point;
        this.actorID = actorID;
        this.action = action;
        this.result = result;
        this.target = target;
        this.detail = detail;
        this.info = info;
        if (target is Failure)
        {
            Dummy.PrintMessage("BAD EVENT! " + this);
        }
        time = TimeManager.Now();
    }

    // Failure
    public Event(int actorID, Point point, EventAction action, Failure failure, object target = null, object detail = null, object info = null)
    {
        identifier = IdentifierCounter++;
        this.result = EventResult.failure;
        this.point = point;
        this.actorID = actorID;
        this.action = action;
        this.failure = failure;
        this.target = target;
        this.detail = detail;
        this.info = info;
        time = TimeManager.Now();
    }

    public bool IsSuccess()
    {
        return result == EventResult.success;
    }

    public override string ToString()
    {
        string str = "";
        /*
        if (actorID != -1)
        {
            str += actorID;
        }
        */
        switch (result)
        {
            case EventResult.success:
                str += " done";
                break;
            case EventResult.ongoing:
                //str += " doing";
                break;
            case EventResult.failure:
                str += " failed at";
                break;
            case EventResult.finished:
                str += " completed";
                break;
            case EventResult.started:
                str += " started";
                break;
            default:
                str += " ???";
                break;
        }
        
        switch (action)
        {
            case EventAction.eat:
                // target = Item
                str += " eating " + (target as Item);
                break;

            case EventAction.sleep:
                // target = Building?
                str += " sleeping" + (target != null ? " at " + (target as Building) :"");
                break;

            case EventAction.travel:
                // target = Location?
                if (target == null)
                {
                    str += " traveling to " + point;
                }
                else{
                    str += " traveling to " + (target as Location).point;
                }
                break;

            case EventAction.work:
                // target = Building?
                if (target != null)
                {
                    str += " working at " + (target as Building);
                }
                else
                {
                    str += " working at tile";
                }
                break;

            case EventAction.build:
                // target = Building
                str += " building " + (target as Building);
                break;

            case EventAction.repair:
                // target = Building
                str += " repairing " + (target as Building);
                break;

            case EventAction.produce:
                // target = ItemStack / Work
                if (target is SkillWork)
                {
                    SkillWork w = target as SkillWork;
                    ItemStack _item = w.produced;
                    str += " producing " + _item + " from " + w;
                }
                else
                {
                    Building b = target as Building;
                    ItemStack _item = b.work.produced;
                    str += " producing " + _item + " from " + b;
                }
                
                break;

            case EventAction.get:
                // target = ItemStack
                str += " getting " + (target as ItemStack);
                break;

            case EventAction.drop:
                // target = ItemStack
                // detail = Building?
                str += " dropping " + (target as ItemStack);
                if (detail != null)
                {
                    str += " to " + (detail as Building);
                }
                break;

            case EventAction.destroy:
                // target = Building?
                if (target is Building)
                {
                    str += (target as Building) + " collapsed";
                }
                break;

            case EventAction.thinking:
                str += " thinking";
                break;

            case EventAction.idling:
                str += " idling";
                break;

            case EventAction.talking:
                str += " talking";
                break;

            default:
                str += " ??? (" + action + ")";
                break;
        }

        if (failure != Failure.none)
        {
            str += ", because " + failure;
        }

        return str;
    }
}

public enum EventAction {
    eat,        // target = Item 
    sleep,      // target = Building?
    work,       // target = Building? 
    build,      // target = Building
    repair,     // target = Building
    get,        // target = ItemStack
    drop,       // target = ItemsStack
    thinking,   // 
    idling,     // 
    update,     
    produce,    // target = SkillWork / Building
    talking,    // target = SocialMessage, detail = targetID, info = Response/Detail
    travel,     // target = Location?
    destroy,     // target = Building?
    fight,
    search      // search location, target = MapObject ? Building
}
public enum EventResult { failure, ongoing, success, finished, started}
public enum Failure { none, notAble, notFound, failChance, blocked, unknown }
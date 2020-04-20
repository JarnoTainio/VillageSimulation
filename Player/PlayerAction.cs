public class PlayerAction
{
    public readonly ActionType action;
    public readonly Point point;
    public object target;
    public object[] args;

    public PlayerAction(ActionType action, Point point, object target = null, object[] args = null)
    {
        this.action = action;
        this.point = point;
        this.target = target;
        this.args = args;
    }

    public bool Equals(PlayerAction action)
    {
        if (action.action == this.action && action.point.Equals(point))
        {
            return true;
        }
        return false;
    }

    public bool IsPossible(Location location)
    {

        switch (action)
        {
            case ActionType.none:
                return false;

            case ActionType.move:
                return true;

            case ActionType.talk:
                return true;

            case ActionType.kill:
                return location.HasMonster();

            case ActionType.explore:
                return true;

            case ActionType.use_item:
                return true;

            case ActionType.get_item:
                return true;

            default:
                return false;
        }
    }

    public override string ToString()
    {
        return action + " at " + point + (target != null ? ",  " + target : "");
    }
}

public enum ActionType { none, move, talk, kill, explore, use_item, get_item, reserve }

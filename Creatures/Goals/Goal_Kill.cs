public class Goal_Kill : Goal
{
    Point point;
    Monster monster;

    public Goal_Kill(Need source, int strength, Memory_Location memory, string reason) : base(source, "Killing monster", reason, strength)
    {
        this.point = memory.point;
        this.monster = memory.GetMonster();
    }

    public override Plan CreatePlan()
    {
        if (creature.CanWork())
        {
            return null;
        }
        Plan plan = new Plan(this, "Kill monster", strength);
        AddTravelActions(plan, point);
        plan.Add(new Combat(plan, strength));
        return plan;
    }

    public override bool IsSatisfied()
    {
        Memory_Location mem = creature.memory.locations.Get(point);
        if (mem == null || !mem.HasMonster())
        {
            return true;
        }
        return false;
    }

    public override string ToText()
    {
        string str = "I am going to kill a monster.";
        return str;
    }

    public override void TickDay()
    {
        if (questRequested.DaysPassed() > 5)
        {
            if (creature.quest == null)
            {
                creature.map.storyManager.RequestQuest(creature, this, new PlayerAction(ActionType.kill, point),
                    "I want you to kill " + monster.name + " at " + point +
                    ", because it is dangerously close to my home. "
                );
                questRequested = TimeManager.Now();
            }
        }
    }
}

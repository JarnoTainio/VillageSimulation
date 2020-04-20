using AgentActions;

class Goal_Search : Goal
{
    string reason;
    Memory_Location memory;

    public Goal_Search(Need source, int strength, Memory_Location memory, string reason) : base(source, "Search " + reason, "I want to explore", strength)
    {
        this.memory = memory;
        this.reason = reason;
    }

    public override Plan CreatePlan()
    {
        // Create plan
        if (memory.GetMonster() == null || memory.lastSeen.DaysPassed() > 14 * creature.personality.patience)
        {
            Plan plan = new Plan(this, "Explore", strength);
            AddTravelActions(plan, memory.point);
            plan.Add(new Search(plan, strength));
            return plan;
        }
        
        return null;
    }

    public override bool IsSatisfied()
    {
        return memory.searched;
    }

    public override string ToText()
    {
        string str = "I want to search " + memory.point;
        str += ", because " + reason;
        return str;
    }

    public override void TickDay()
    {
        if (questRequested.DaysPassed() > 5)
        {
            if (creature.quest == null)
            {
                creature.map.storyManager.RequestQuest(creature, this, new PlayerAction(ActionType.explore, memory.point),
                    "I want you to explore location at " + memory.point +
                    ", but be careful, because there is a monster in there!"
                );
                questRequested = TimeManager.Now();
            }
        }
    }
}

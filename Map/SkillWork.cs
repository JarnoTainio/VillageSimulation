public class SkillWork
{
    public static int Count;

    public readonly string name;
    public readonly Skill skill;
    public readonly int worktime;
    public readonly ItemStack produced;
    public readonly ItemStack required;
    public readonly float value;
    public readonly bool oncePerDay;
    public readonly int difficulty;
    public readonly int failChance;

    public SkillWork(string name, Skill skill, int worktime, ItemStack produced = null, ItemStack required = null, int difficulty = 0, int failChance = 0, bool oncePerDay = true)
    {
        Count++;
        this.name = name;
        this.skill = skill;
        this.worktime = worktime;
        this.produced = produced;
        this.required = required;
        this.difficulty = difficulty;
        this.oncePerDay = oncePerDay;
        this.failChance = failChance;
        value = (produced != null ? produced.GetValue() : 0) - (required != null ? required.GetValue() : 0);
        value = (60 * value) / worktime;
        value *= (1 - failChance / 100f);
        //Dummy.PrintMessage(name + ": [" + value + "] skill: " + skill.name + " workTime: " + worktime + " produced: " + produced + " required: " + required + " failChance: " + failChance + " oncePerDay: " + oncePerDay);
    }

    public float GetEffort(Creature creature)
    {
        if (produced == null)
        {
            return 0;
        }
        float effort = worktime * (1 + GetFailChance(creature) / 100f);
        effort /= creature.personality.GetPassion(skill);
        return effort;
    }

    public float GetValue(Creature creature)
    {
        float value = (produced != null ? produced.GetValue() : 0) - (required != null ? required.GetValue() : 0);
        value = (60 * value) / worktime;
        value *= (1 - GetFailChance(creature) / 100f);
        value *= ((creature.personality.GetPassion(skill) + .5f) * creature.personality.GetSkill(skill) / 30f);
        return value;
    }

    private bool IsSuccess(Creature creature)
    {
        if (failChance == 0)
        {
            return true;
        }
        int roll = creature.random.Next(100);
        return roll >= failChance;
    }

    public bool Work(Map map, Creature creature, object source)
    {
        if (IsSuccess(creature))
        {
            creature.personality.IncreaseSkill(this);
            creature.inventory.Add(produced);
            Dummy.instance.Produced(produced);
            map.Event(creature.location, new Event(creature.ID, creature.location, EventAction.produce, EventResult.success, this, produced, source));
            return true;
        }
        else
        {
            map.Event(creature.location, new Event(creature.ID, creature.location, EventAction.produce, Failure.failChance, this));
            return false;
        }
    }

    public override string ToString()
    {
        return name;
    }

    private int GetFailChance(Creature creature)
    {
        float difference = creature.personality.GetSkill(skill) - difficulty;
        if (difference < 0)
        {
            // Maximum increase of +100% at skill 0 / 100
            difference *= -1;
            return (int)(failChance * (1 + 1f * (difference / difficulty)));
        }
        else
        {
            // Maximum decrease of -100% at skill 100 / 100
            return (int)(failChance * (1 + difference / (100 - difficulty)));
        }
    }
}
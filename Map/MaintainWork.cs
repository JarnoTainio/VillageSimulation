using System.Collections;
using System.Collections.Generic;

public class MaintainWork
{
    public readonly string name;
    public readonly Skill skill;
    public readonly int worktime;
    public readonly ItemStack required;
    public readonly bool oncePerDay;
    public readonly int difficulty;
    public readonly int failChance;
    public readonly float value;

    public MaintainWork(string name, Skill skill, int worktime, int value, ItemStack required = null, int difficulty = 0, int failChance = 0, bool oncePerDay = true)
    {
        this.name = name;
        this.skill = skill;
        this.worktime = worktime;
        this.value = value;
        this.required = required;
        this.difficulty = difficulty;
        this.oncePerDay = oncePerDay;
        this.failChance = failChance;
    }

    private bool IsSuccess(Creature creature)
    {
        if (failChance == 0)
        {
            return true;
        }
        int roll = creature.random.Next(100);
        Dummy.PrintMessage(creature.name + " " + name + " " + roll + " / " + failChance + " " + (roll >= failChance));
        return roll >= failChance;
    }

    public bool Work(Map map, Creature creature, Building building)
    {
        if (IsSuccess(creature))
        {
            building.Repair(value);
            creature.personality.IncreaseSkill(this);
            map.Event(creature.location, new Event(creature.ID, creature.location, EventAction.repair, EventResult.success, building));
            return true;
        }
        else
        {
            map.Event(creature.location, new Event(creature.ID, creature.location, EventAction.repair, EventResult.failure, Failure.failChance, building));
            return false;
        }
    }
}

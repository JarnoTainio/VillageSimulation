using System.Collections;
using System.Collections.Generic;

public class Personality
{
    private Creature creature;

    public float greed;
    public float patience;
    public float social;
    public float curiosity;

    // Skills
    public float[] skills;
    public float[] passion;

    // Sleep
    public int sleepingTime;
    public int sleepDuration;
    public int awakeningTime;

    // Exploration
    public int searchDistance;

    public Personality(Creature creature)
    {
        this.creature = creature;
        sleepingTime = 22 + creature.random.Next(0, 2) + creature.random.Next(0, 2);
        sleepDuration = 7 + creature.random.Next(0, 2) + creature.random.Next(0, 2);
        awakeningTime = sleepingTime + sleepDuration - 24;

        greed = (float)creature.random.NextDouble();
        patience = (float)creature.random.NextDouble();
        social = (float)creature.random.NextDouble();
        curiosity = (float)creature.random.NextDouble();

        searchDistance = 4 + creature.random.Next(0, 2) + creature.random.Next(0, 2);

        skills = new float[Skill.skills.Length];
        passion = new float[Skill.skills.Length];

        for (int i = 0; i < passion.Length; i++)
        {
            passion[i] = ((creature.random.Next(50) + creature.random.Next(50) + 1)) / 100f;
        }

        for (int i = 0; i < 15; i++)
        {
            skills[creature.random.Next(skills.Length)] += 5;
        }

    }

    public List<Need> GenerateNeeds()
    {
        List<Need> needs = new List<Need>();

        // Randomize needs
        needs.Add(new Need_Eat          (creature, 85 + creature.random.Next(15), 1 ));
        needs.Add(new Need_Sleep        (creature, 80 + creature.random.Next(20), 1 ));
        needs.Add(new Need_Safety       (creature, 70 + creature.random.Next(15), creature.random.Next(3) + 1 ));
        needs.Add(new Need_Profession   (creature, 40 + creature.random.Next(30), creature.random.Next(3) + 1 ));
        needs.Add(new Need_Social       (creature, 20 + creature.random.Next((int)(50*social)), creature.random.Next(3) + 1));
        // needs.Add(new Need_Future   (creature, 50 + creature.random.Next(20), creature.random.Next(3) + 1 ));
        needs.Add(new Need_Curiosity(creature, 25 + creature.random.Next(25), creature.random.Next(5) + 1 ));

        if (creature.random.Next(4) > 0) {
            string[] names = new string[] { "religion", "nature", "engineer" };
            string[] descriptions = new string[] { "believe in higher powers", "friend of nature", "power of machines" };
            MonsterTag[] hated = new MonsterTag[] { MonsterTag.undead, MonsterTag.none, MonsterTag.nature };
            ItemType[] valued = new ItemType[] { ItemType.Holy, ItemType.Nature, ItemType.Stone };
            ItemType[] use = new ItemType[] { ItemType.Food, ItemType.Nature, ItemType.Stone};
            ObjectType[] target = new ObjectType[] {ObjectType.holy, ObjectType.plant, ObjectType.tunnel };
            int roll = creature.random.Next(names.Length);
            needs.Add(new Need_Vanity(creature, creature.random.Next(50) + 15, 1, names[roll], descriptions[roll], hated[roll], valued[roll], use[roll], target[roll]));
        }
        return needs;
    }

    public void IncreaseSkill(SkillWork work)
    {
        IncreaseSkill(work.skill, work.difficulty, work.worktime);
    }

    public void IncreaseSkill(MaintainWork work)
    {
        IncreaseSkill(work.skill, work.difficulty, work.worktime);
    }

    public void IncreaseSkill(Skill skill, int difficulty, int worktime)
    {
        float amount = (difficulty * 1.5f - skills[skill.id]) * (101 - skills[skill.id]) / 1000f;        // Difficulty +50%, reduce skill level
        if (amount < 0)
        {
            return;
        }
        amount *= worktime / 600f;                                   // Multiply by workingtime
        if (amount > 1f)
        {
            amount = 1f;
        }
        skills[skill.id] += amount * passion[skill.id];             // Multiply by passion
    }

    public int GetSkill(Skill skill)
    {
        return (int)skills[skill.id];
    }

    public float GetPassion(Skill skill)
    {
        return passion[skill.id];
    }
}

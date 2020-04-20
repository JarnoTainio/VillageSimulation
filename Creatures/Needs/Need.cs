using System.Collections.Generic;

public class Need
{
    public Creature creature;
    public Goal goal;

    public string name;
    public string description;
    public int strength;
    public int growTime;

    protected Time lastTicked;

    public Need(Creature creature, string name, string description, int strength, int growTime)
    {
        this.creature = creature;
        this.name = name;
        this.description = description;
        this.strength = strength;
        this.growTime = growTime;
        Tick(null);
    }

    public virtual List<Goal> CreateGoals()
    {
        return null;
    }

    public void Tick(Goal goal)
    {
        lastTicked = TimeManager.Now();
    }

    public virtual int GetValue(float modifier = 1.0f, float flat = 0f)
    {
        // Clamp modifier between 0.1 and 1
        if (modifier <= 0)
        {
            modifier = 0.1f;
        }

        // Get value
        int value = (int)(strength * modifier + flat);

        // Cant be larger than max strength
        if (value > strength)
        {
            return strength;
        }
        // Minimum value
        else if (value < 1)
        {
            value = 1;
        }
        return value;
    }

}

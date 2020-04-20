using UnityEngine;

public class Stat
{
    public string name;
    public int max;
    public int min;
    public int current;

    public Stat(string name = "Stat", int current = 100, int max = 100, int min = 0)
    {
        this.name = name;
        this.current = current;
        this.max = max;
        this.min = min;
    }

    public bool Add(int amount)
    {
        if (amount < 0)
        {
            Debug.LogError("Negative add!");
        }
        current = Mathf.Min(current + amount, max);
        return IsFull();
    }

    public bool Reduce(int amount)
    {
        if (amount < 0)
        {
            Debug.LogError("Negative reduce!");
        }
        current = Mathf.Max(current - amount, min);
        return IsEmpty();
    }

    public bool IsFull()
    {
        return current == max;
    }

    public bool IsEmpty()
    {
        return current == min;
    }

    public override string ToString()
    {
        return name + " " + current;
    }
}

public enum State { None, Low, Medium, High, Extreme, Dead };

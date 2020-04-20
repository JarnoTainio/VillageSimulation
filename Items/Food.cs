public class Food : Item
{
    public readonly float calories;
    public readonly Quality quality;

    public Food(string name, int cost, float calories, Quality quality, ItemType[] types) : base(name, cost, types)
    {
        this.quality = quality;
        this.calories = calories;
    }
}

public enum Quality {awful, terrible, bad, ok, good, great, perfect };
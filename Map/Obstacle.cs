using System.Collections;
using System.Collections.Generic;

public class Obstacle
{
    public Item key;
    public string description;

    public Obstacle(Item key, string description)
    {
        this.key = key;
        this.description = description;
    }

    public bool IsKey(Item item)
    {
        return item.Equals(key);
    }
}

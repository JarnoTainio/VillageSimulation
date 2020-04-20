using System.Collections.Generic;

public class Memory_Location : Memory
{
    public Point point;
    public int tile;
    public int[] objects;
    public int monsterId;
    public bool searched;
    public int obstacleItemId;

    public Memory_Location(int power, Location location) : base(power)
    {
        point = location.point;
        Update(location);
        searched = false;
    }

    public Memory_Location Update(Location location)
    {
        objects = new int[location.objects.Count];
        for (int i = 0; i < location.objects.Count; i++)
        {
            objects[i] = location.objects[i].ID;
        }
        tile = location.tile;
        monsterId = location.GetMonster() == null ? -1 : location.GetMonster().id;
        obstacleItemId = location.GetObstacle() == null ? -1 : location.GetObstacle().key.id;
        UpdateSeen();
        return this;
    }

    public bool IsType(Tile tile)
    {
        return Tile.tiles[this.tile] == tile;
    }

    public override string ToString()
    {
        return Tile.tiles[tile].name + " " + point;
    }

    public bool HasMonster()
    {
        return monsterId != -1;
    }

    public bool Contains(ObjectType type)
    {
        for(int i = 0; i < objects.Length; i++)
        {
            if (MapObject.Objects[objects[i]].IsType(type))
            {
                return true;
            }
        }
        return false;
    }

    public bool Contains(MapObject mapObject)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == mapObject.ID)
            {
                return true;
            }
        }
        return false;
    }

    public Monster GetMonster()
    {
        if (monsterId == -1)
        {
            return null;
        }
        return Monster.Monsters[monsterId];
    }

    public Item GetObstacleKey()
    {
        if (obstacleItemId == -1)
        {
            return null;
        }
        return Item.items[obstacleItemId];
    }

    public void KillMonster()
    {
        monsterId = -1;
    }

    public void Search()
    {
        searched = true;
    }

}

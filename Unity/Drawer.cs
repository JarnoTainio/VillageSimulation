using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Drawer : MonoBehaviour
{
    public Dummy dummy;
    public TileBase none;

    [Header("Tiles")]
    public Tilemap groundTilemap;
    public Tilemap roadTileMap;
    public TileBase[] tiles;
    public TileBase[] waterTiles;
    public TileBase[] forestTiles;
    public TileBase[] mountainTiles;
    public TileBase road;

    [Header("Buildings")]
    public Tilemap buildingTilemap;
    public TileBase[] buildings;

    [Header("Creatures")]
    public CreatureObject creaturePrefab;
    public List<CreatureObject> creatureObjects;

    [Header("Objects")]
    public Tilemap objectTilemap;
    public TileBase[] objects;

    [Header("Enviroment")]
    public Image night;

    public void Add(Creature creature)
    {
        CreatureObject co = Instantiate(creaturePrefab);
        co.SetCreature(creature);
        co.name = creature.name;
        co.id = creature.ID;
        creatureObjects.Add(co);
    }

    readonly float morning = 8f;
    readonly float evening = 20f;

    public float GetDarkness()
    {
        float hour = TimeManager.hour + TimeManager.minutes / 60f;
        if (hour > morning && hour < evening)
        {
            return 0f;
        }
        if (hour < 20)
        {
            hour += 24;
        }
        hour -= 26; // -6 ... +6
        return (1 - Mathf.Abs(hour / 6f)) / 2f;
    }

    public void AddBuilding(int index, int x, int y)
    {
        buildingTilemap.SetTile(new Vector3Int(x, y, 0), index < buildings.Length ? buildings[index] : none);
    }

    public void RemoveBuilding(Point p)
    {
        buildingTilemap.SetTile(new Vector3Int(p.x, p.y, 0), null);
    }

    public void Remove(Creature c)
    {
        foreach(CreatureObject go in creatureObjects)
        {
            if (go.id == c.ID)
            {
                creatureObjects.Remove(go);
                Destroy(go.gameObject);
                return;
            }
        }
        Dummy.PrintMessage("CREATURE NOT FOUND!!!");
    }

    public void Draw(Map map)
    {
        Color color = night.color;
        color.a = GetDarkness();
        night.color = color;

        for (int i = 0; i < dummy.creatures.Count; i++)
        {
            Creature c = dummy.creatures[i];
            float offSetX = c.location.GetDirection(c.travelingTo).x * c.travelProgress;
            float offSetY = c.location.GetDirection(c.travelingTo).y * c.travelProgress;
            float x = c.location.x + .5f + offSetX;
            float y = c.location.y + .5f + offSetY;
            creatureObjects[i].transform.localPosition = new Vector3(x, y, 0);
            SetMood(c, creatureObjects[i]);
        }
        //DrawMap(map);
    }

    public void DrawMap(Map map)
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                int i = x + y * map.width;
                int id = map.GetLocation(x, y).tile;
                if (id == Tile.Water.id)
                {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), waterTiles[map.adjacency[i]]);
                }
                else if (id == Tile.Forest.id)
                {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), forestTiles[map.adjacency[i]]);
                }
                else if (id == Tile.Mountain.id)
                {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), mountainTiles[map.adjacency[i]]);
                }
                else
                {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), tiles[id]);
                }
            }
        }
    }

    public void DrawRoad(Point p, bool hasRoad)
    {
        roadTileMap.SetTile(new Vector3Int(p.x, p.y, 0), hasRoad ? road : null);
    }

    public void SetMood(Creature c, CreatureObject go)
    {
        return;
        /*
        MindState state = c.GetMindState();
        SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();

        if (state == MindState.Sleeping)
            spriteRenderer.color = Color.blue;

        else if (state == MindState.Working)
            spriteRenderer.color = Color.yellow;

        else if (state == MindState.Traveling)
            spriteRenderer.color = Color.green;

        else if (state == MindState.Thinking)
            spriteRenderer.color = Color.magenta;

        else if (state == MindState.Dead)
            spriteRenderer.color = Color.black;

        else
            spriteRenderer.color = Color.white;
            */
    }

    public void AddObject(int index, int x, int y)
    {
        if (index >= objects.Length)
        {
            Dummy.PrintMessage("Object out of bounds!");
            index = index % objects.Length;
        }
        objectTilemap.SetTile(new Vector3Int(x, y, 0), index < objects.Length ? objects[index] : none);
    }

    public void RemoveMonster(int x, int y)
    {
        objectTilemap.SetTile(new Vector3Int(x, y, 0), null);
    }
}

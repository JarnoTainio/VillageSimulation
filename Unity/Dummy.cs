using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class Dummy : MonoBehaviour
{
    public Canvas canvas;
    public int mapSeed;
    public bool autoSimulation;

    public static int tickCount;
    public static Dummy instance;

    public Drawer drawer;
    public Creature targetCreature;
    public CreatureInspector inspector;
    public EventCounterList counterList;
    public TextMeshProUGUI debugBox;
    public PlayerController playerController;

    [Header("Map")]
    public Map map;
    public int mapSize;

    [Header("GameSpeed")]
    public int targetDate;
    // How many time ticks are given per second
    public int ticksPerSecond;
    // How many minutes one tick is in the simulation
    public int delta;
    public string speed;
    public bool paused;

    public List<Creature> creatures;
    [Header("Villagers")]
    public int villages = 4;
    public int villagerCount = 1;
    public int villagerDistance = 2;

    [Header("ions")]
    public SpriteRenderer iconPrefab;
    public Sprite[] icons;
    public float iconLifeTime;

    ItemStackList produced;

    float timeCounter;
    float realTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        counterList = new EventCounterList();

        tickCount = 0;
        TimeManager.Reset();

        map = new Map(mapSize, mapSize, mapSeed);
        creatures = new List<Creature>();
        int villageSize = (int)Mathf.Sqrt(villages);
        int size = (int)Mathf.Sqrt(villagerCount);
        if (size == 0)
        {
            size = 1;
        }
        if (villageSize == 0)
        {
            villageSize = 1;
        }
        int middle = mapSize / 2 - size / 2;
        int area = map.width / 2;
        for (int k = 0; k < villages; k++)
        {
            Point village = new Point(
                (k % villageSize) * (area / villageSize) + (area / villageSize) / 2 + area / 2, 
                (k / villageSize) * (area / villageSize) + (area / villageSize) / 2 + area / 2
            );
            for (int i = 0; i < villagerCount; i++)
            {
                Point point = new Point(village.x + (i % size) * villagerDistance, village.y + (i / size) * villagerDistance);

                // Create villager
                Villager v = new Villager(map, point);
                v.inventory.Add(new ItemStack(Item.Bread, 7));
                v.inventory.Add(new ItemStack(Item.Wood, 5));
            }
        }
        drawer.DrawMap(map);
        targetCreature = creatures[0];
        inspector.SetCreature(targetCreature);
        delta = 64;
        ticksPerSecond = 512;
        paused = false;
        UpdateSpeed();
        produced = new ItemStackList();
    }

    public void Add(Creature c)
    {
        creatures.Add(c);
        drawer.Add(c);
    }

    public void Remove(Creature c)
    {
        creatures.Remove(c);
        drawer.Remove(c);
    }

    // Update is called once per frame
    int ticks = 0;
    int lastTicks = 0;
    float counter = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }

        if (paused)
        {
            return;
        }

        counter -= UnityEngine.Time.deltaTime;
        if (counter <= 0)
        {
            counter += 1;
            //Debug.Log(ticks);
            lastTicks = ticks;
            ticks = 0;
            UpdateSpeed();
        }

        if (!autoSimulation)
        {
            return;
        }

        timeCounter += UnityEngine.Time.deltaTime * ticksPerSecond;
        if (timeCounter >= 1)
        {
            tickCount++;
            timeCounter -= 1;

            Tick(delta);
        }
    }

    public void Tick(int delta)
    {
        int timeLeft = delta;
        realTime += UnityEngine.Time.deltaTime;

        while (timeLeft > 0)
        {
            ticks++;
            int tickDelta = Mathf.Min(1, timeLeft);
            timeLeft -= tickDelta;

            int d = TimeManager.day + (TimeManager.month - 1) * Time.DAYS_IN_MONTH;
            bool newHour = TimeManager.Tick(tickDelta);
            map.Tick(tickDelta, newHour);
            if (newHour)
            {
                //Debug.Log("TIME: " + TimeManager.Now());
                if (d == targetDate)
                {
                    PrintReport();
                }
                else
                {
                    Debug.Log(d + " / " + targetDate);
                }
            }
        }
        drawer.Draw(map);
    }

    public void IncreaseSpeed()
    {
        if (delta == 0)
        {
            delta = 1;
            ticksPerSecond = 1;
        }
        else if (ticksPerSecond < 60)
        {
            delta = 1;
            ticksPerSecond *= 2;
        }
        else
        {
            delta *= 2;
        }
        UpdateSpeed();
    }

    public void DecreaseSpeed()
    {
        
        if (delta > 1)
        {
            delta /= 2;
            if (delta < 1)
            {
                delta = 1;
            }
        }
        else if (ticksPerSecond > 1)
        {
            ticksPerSecond /= 2;
            if (ticksPerSecond < 1)
            {
                ticksPerSecond = 1;
            }
        }
        else
        {
            delta = 0;
            ticksPerSecond = 1;
        }
        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        speed = (lastTicks * 15) + "m";
    }

    public void Add(Building b)
    {
        drawer.AddBuilding(b.id, b.point.x, b.point.y);
    }

    public void RemoveBuilding(Point p)
    {
        drawer.RemoveBuilding(p);
    }

    public void CreateIcon(int i, Point p)
    {
        SpriteRenderer sr = Instantiate(iconPrefab);
        sr.transform.localPosition = new Vector3(p.x + .5f, p.y +.5f, 0);
        sr.sprite = icons[i];
        Destroy(sr.gameObject, iconLifeTime);
    }

    public void TogglePause()
    {
        paused = !paused;
    }

    public static void PrintMessage(string msg)
    {
        Debug.Log("[" + tickCount + "]" + TimeManager.GetCompactTime() + " " + msg);
    }

    private static void Log(string key)
    {
        //instance.counterList.Add(key);
    }

    private void PrintReport()
    {
        autoSimulation = false;
        paused = true;
        Debug.Log("===================================================");
        string str = "";
        Debug.Log("DAY " + targetDate + " REACHED! ");
        str += "Seed: " + map.seed+"\n";
        str += "Day " + targetDate + "\n";
        Debug.Log("Seed: " + map.seed);
        int minutes = (int)(realTime / 60);
        int hours = (int)(minutes / 60);
        minutes = minutes % 60;
        int seconds = (int)(realTime % 60);

        // Time
        Debug.Log("Time passed: " + hours + "h " + minutes + "m " + seconds + "s");
        str += "Time passed: " + hours + "h " + minutes + "m " + seconds + "s\n";

        // Village
        Debug.Log("Villagers: " + map.creatures.Count);
        
        int minX = map.width;
        int maxX = 0;
        int minY = map.height;
        int maxY = 0;
        foreach(Building b in map.buildings)
        {
            if (b.HasTag(BuildingTag.Dungeon))
            {
                continue;
            }
            Point p = b.point;
            if (p.x < minX)
            {
                minX = p.x;
            }
            if (p.x > maxX)
            {
                maxX = p.x;
            }
            if (p.y < minY)
            {
                minY = p.y;
            }
            if (p.y > maxY)
            {
                maxY = p.y;
            }
        }
        Debug.Log("Village size: " + (maxX - minX) + " - " + (maxY - minY));
        str += "Villagers: " + map.creatures.Count + "\nVillageSize: " + (maxX - minX) + " x " + (maxY - minY)+ "\n";

        // Buildings
        List<Building> buildings = new List<Building>();
        int[] buildingCount = new int[Schematic.schematics.Length];
        int uniqueBuildings = 0;
        int centerX = 0;
        int centerY = 0;
        foreach (Building b in map.buildings)
        {
            if (!b.HasTag(BuildingTag.Dungeon))
            {
                buildings.Add(b);
                if (buildingCount[b.id] == 0)
                {
                    uniqueBuildings++;
                }
                buildingCount[b.id]++;
                centerX += b.point.x;
                centerY += b.point.y;
            }
        }
        centerX = centerX / buildings.Count;
        centerY = centerY / buildings.Count;
        str += "VillageCenter: " + centerX + ", " + centerY + "\n";
        Debug.Log("Buildings: " + buildings.Count + " Unique: " + uniqueBuildings);
        string buildingListing = "";
        bool first = true;
        for(int i = 0; i < buildingCount.Length; i++)
        {
            
            if (buildingCount[i] > 0)
            {
                if (!first)
                {
                    buildingListing += ", ";
                }
                first = false;
                buildingListing += Schematic.schematics[i].building.name + " x" + buildingCount[i];
            }
        }
        Debug.Log(buildingListing);
        str += "BuildingCount: " + buildings.Count + "\nUniqueBuildings: " + uniqueBuildings + "\nBuildingList: " + buildingListing + "\n";

        // Quests
        Debug.Log("Quests: " + map.storyManager.quests.Count);
        str += "Quests: " + map.storyManager.quests.Count + "\n";
        foreach (Quest q in map.storyManager.quests)
        {
            str += "quest: ";
            string s = "quest: ";
            foreach(QuestStep qs in q.events)
            {
                s += qs.ToSimpleString() + " - ";
                str += ": " + qs.ToSimpleString() + " ";
            }
            Debug.Log(s + " Reward: " + q.reward + " " + q.rewardCoins);
            str += "Reward: ";
            if (q.reward != null)
            {
                str += q.reward + " ";
            }
            if (q.rewardCoins >0)
            {
                str += q.rewardCoins;
            }
            str += "\n";
        }

        // Production
        Debug.Log("Produced: " + produced);
        str += "Produced: " + produced + "\n";

        // File management
        string path = "C:\\results\\";
        string[] files = Directory.GetFiles(path);
        int id = 0;
        foreach(string f in files)
        {
            if (f.Contains(".txt"))
            {
                id++;
            }
        }
        File.WriteAllText(path + "result_" + id + ".txt", str);

        // Screenshot
        //drawer.night.gameObject.SetActive(false);
        canvas.gameObject.SetActive(false);
        ScreenCapture.CaptureScreenshot(path + "screenshot_" + id + ".png");

        // Load scene again
        SceneManager.LoadScene(0);
    }

    public void Produced(ItemStack stack)
    {
        produced.Add(stack);
    }
}

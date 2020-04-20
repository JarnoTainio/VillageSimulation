using System.Collections;
using System.Collections.Generic;

public abstract class Creature
{
    public static int ID_COUNT = 0;

    public static bool printAllMessages = false;

    public int ID;

    public Map map;

    //Basic
    public string name;
    public bool alive;
    public Time birthTime;
    public int age;
    public int daysLived;

    public System.Random random;

    public float travelProgress;
    public Point travelingTo;

    //Ancestry
    public Creature father;
    public Creature mother;

    //Stats

    //public Stat food;
    //public Stat energy;

    public float tired;
    public float hunger;

    public float energy;
    public float maxEnergy;
    public float life;
    public float travelSpeed;
    public int travelTime;

    // Flags
    protected MindState mindState = MindState.Sleeping;
    public Tired tiredState;
    public Hunger hungerState;

    public bool hasHome;
    public bool atHome;

    // Private value holders
    public int distanceToHome;

    // Locations
    private Point home;
    public Point location;

    // Actions
    public Action currentAction;
    public Event currentEvent;

    // Managers
    public Inventory inventory;
    public MemoryManager memory;
    public NeedManager needManager;
    public GoalManager goalManager;
    public Personality personality;
    public SocialManager social;

    // Quests
    public AgentQuest quest;
    public int playerReputation;
    public int questsCompleted;
    public bool hasTalkedToPlayer;

    public Creature(Map map, Point location, Creature parent = null)
    {
        this.father = parent;
        // Creature identifier number
        ID = ID_COUNT++;

        // Map
        this.map = map;
        this.location = location;
        this.home = new Point(location);

        // Personal random
        random = new System.Random(map.seed * ID);
        this.name = NameGenerator.New(random);
        this.name = name[0].ToString().ToUpper() + name.Substring(1);

        alive = true;
        birthTime = TimeManager.Now();
        age = 0;

        tired = 0;
        hunger = 1;
        energy = maxEnergy = 12f;
        life = 100f;
        travelSpeed = .8f + (random.Next(20) + random.Next(20)) / 100f;
        travelTime = (int)(Map.TileSize / travelSpeed);

        //food = new Stat("Food");
        //energy = new Stat("Energy");

        //Personality and needs
        personality = new Personality(this);
        needManager = new NeedManager(this);
        goalManager = new GoalManager(this);
        memory = new MemoryManager(this);
        inventory = new Inventory();
        social = new SocialManager(this);

        currentAction = null;

        map.Add(this);

        hasHome = false;
        atHome = false;

        UpdateTired();
        UpdateHunger();
        UpdateMindState(MindState.Neutral);

        questsCompleted = 0;
        playerReputation = 0;
        hasTalkedToPlayer = false;

    }

    /*========================================================================
     * B A S I C   L O G I C
    ========================================================================*/

    public abstract void Tick(Map map, int delta);

    public abstract void TickDay(Map map);

    public virtual void Die(string reason)
    {
        Dummy.instance.CreateIcon(0, location);
        mindState = MindState.Dead;
        alive = false;
        PrintMessage("DEAD! Hunger: " + hunger + ", Tired: " + tired +", DaysLived: " + daysLived + " REASON: " + reason, true);
        if (!map.GetLocation(location).HasMonster() && random.Next(100) < 20)
        {
            map.Add(Monster.Skeleton, null, location);
        }
    }

    public void MoveTo(Location newLocation)
    {
        map.Move(this, location, newLocation.point);
        location = newLocation.point;
        Observe(newLocation);
        foreach(Point dir in Direction.Directions)
        {
            Point p = location.Add(dir);
            if (map.WithinBounds(p))
            {
                Observe(map.GetLocation(p), false);
            }
        }
        UpdateDistanceToHome();
    }

    public void Observe(Location location, bool isCurrentLocation = true)
    {
        memory.ObserveAndUpdate(location, isCurrentLocation);
    }

    public void Observe(Event e)
    {
        if (e.actorID == ID)
        {
            currentEvent = e;
        }
        memory.Add(e);
    }

    /*========================================================================
    * V A L U E S
    ========================================================================*/

    protected void UpdateDistanceToHome()
    {
        distanceToHome = home.Distance(location);
        atHome = distanceToHome == 0;
    }

    protected void UpdateHunger()
    {
        Hunger state = Hunger.None;
        if (hunger < 1)
        {
            state = Hunger.None;
        }
        else if (hunger < 2)
        {
            state = Hunger.Little;
        }
        else if (hunger < 4)
        {
            state = Hunger.Very;
        }
        else
        {
            state = Hunger.Starving;
        }
        hungerState = state;
    }

    protected void UpdateTired()
    {
        tiredState = GetTired(tired);
    }

    protected Tired GetTired(float tired)
    {
        if (tired < 1)
        {
            return Tired.None;
        }
        else if (tired < 18)
        {
            return Tired.Little;
        }
        else if (tired < 32)
        {
            return Tired.Very;
        }
        else if (tired < 100)
        {
            return Tired.Extremely;
        }
        else
        {
            return Tired.Dead;
        }
    }

    public bool CanSleep()
    {
        return CanSleep(0, atHome);
    }

    public bool CanSleep(float modifier, bool atHome)
    {
        float value = tired + modifier;
        float hour = TimeManager.hour + TimeManager.minutes / 60f + modifier;
        bool isNight = (hour >= personality.sleepingTime) || (hour < personality.awakeningTime);
        bool sleeping = mindState == MindState.Sleeping;

        switch (GetTired(value)) {
            case Tired.None:
                return false;

            case Tired.Little:
                return 
                    // Continue sleeping if it is night or you are home
                    (sleeping && (isNight || atHome))   
                    // Sleep if at home and it is night
                    || (isNight && atHome);

            case Tired.Very:
                return (atHome || sleeping);

            case Tired.Extremely:
                return true;

            default:
                return true;
        }
    }

    public void Rest(float rest)
    {
        energy += rest / 2;
        tired -= rest;
        if (tired < 0)
        {
            tired = 0;
        }
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
        UpdateTired();
    }

    public void Eat(Food food)
    {
        hunger -= food.calories;
        if (hunger < 0)
        {
            hunger = 0;
        }
        UpdateHunger();
    }

    public void UseEnergy(float amount)
    {
        energy -= amount;
        if (energy < 0)
        {
            energy = 0;
        }
    }

    /*========================================================================
     * A C T I O N S
     ========================================================================*/

    public virtual void ActionDone(Action action)
    {
        currentAction = null;
        goalManager.ActionDone(action);
    }

    public virtual void ActionFailed(Action action, Failure failure)
    {
        currentAction = null;
        goalManager.ActionFailed(action, failure);
    }

    public virtual void ActionOngoing(Action action)
    {
        bool ok = goalManager.ActionOngoing(action);
        if (!ok)
        {
            currentAction = null;
        }
    }

    public void UpdateMindState(MindState newState)
    {
        // ToDo: Check if given mindstate is possible
        mindState = newState;
    }

    public void ResetMindState()
    {
        // ToDo: Check what is default mindstate
        mindState = MindState.Neutral;
    }

    public MindState GetMindState()
    {
        return mindState;
    }

    public bool Equals(Creature creature)
    {
        return ID == creature.ID;
    }

    public int GetTravelCost(Point point)
    {
        Point start = hasHome ? home : location;
        int d = start.Distance(point);
        return d * d;
    }

    public int GetWorkCost(SkillWork work)
    {
        float cost = -work.value * personality.greed;                   // Effort reduced by value times greed
        cost += work.worktime / personality.GetPassion(work.skill);     // Worktime added, divided by passion
        return (int)cost;
    }

    public Point GetHome()
    {
        return hasHome ? home : location;
    }

    public void SetHome(Point point)
    {
        hasHome = true;
        home = point;
        distanceToHome = 0;
    }

    public void RemoveHome()
    {
        PrintMessage("I lost my home!", true);
        hasHome = false;
        home = Point.None;
        distanceToHome = 0;
    }

    public int GetTravelTime()
    {
        return travelTime;
    }

    public bool CanWork()
    {
        return TimeManager.hour > personality.sleepingTime - (hasHome ? 4 : 2);
    }

    public void QuestCompleted()
    {
        playerReputation += 4;
        questsCompleted++;
        foreach(Creature creature in map.creatures)
        {
            creature.playerReputation++;
        }
    }

    public void PrintMessage(string message, bool important)
    {
        if (printAllMessages || important || ID == Dummy.instance.inspector.creature.ID)
        {
            Dummy.PrintMessage("[" + ID + "]" + name + ": " + message);
        }
    }
}

public enum Hunger { None, Little, Very, Starving, Dead };
public enum Tired { None, Little, Very, Extremely, Dead };
public enum MindState { Dead, Neutral, Sleeping, Tired, Eating, Hungry, Starving, Working, Thinking, Traveling, Exploring, Social, Combat };


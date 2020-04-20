using System;
using System.Collections;
using System.Collections.Generic;

public class StoryManager
{
    public readonly Map map;
    public List<Quest> quests;
    public Random random;
    private Player player;
    private Rectangle area;

    private QuestBuilder questBuilder;

    // uint ticks should last for 8000+ years in game time, until overflow
    public uint tick;
    private uint[,] playerKnowledgeGrid;
    private ActionType[,] reserveGrid;

    private Time questCompleted;
    private Time questAdded;
    private Time mapAltered;

    private float minimumQuestRatio = .2f;
    private float maximumQuestRatio = .75f;

    public StoryManager(Map map)
    {
        this.map = map;
        quests = new List<Quest>();
        random = new Random(map.seed * map.seed);
        player = map.player;
        playerKnowledgeGrid = new uint[map.width, map.height];
        reserveGrid = new ActionType[map.width, map.height];
        tick = 0;
        questBuilder = new QuestBuilder(this);
    }

    public void Tick(bool newDay)
    {
        tick++;

        // Update playergrid
        playerKnowledgeGrid[(uint)player.point.x, (uint)player.point.y] = tick;
        Dummy.instance.debugBox.text = "QUESTS:";
        foreach (Quest q in quests) {
            Dummy.instance.debugBox.text += "\n" + q.ToString();
        }

        // Progress stories
        if (newDay)
        {
            if (quests.Count < 3)
            {
                //CreateStory();
            }
            bool removeQuest = false;
            foreach (Quest story in quests)
            {
                story.TickDay(this);
                if (!story.IsValid())
                {
                    removeQuest = true;
                }
            }

            while (removeQuest)
            {
                removeQuest = false;
                foreach(Quest q in quests)
                {
                    if (!q.IsValid())
                    {
                        quests.Remove(q);
                        removeQuest = true;
                        break;
                    }
                }
            }

            // Spawn monster
            int size = 10;
            int sx = random.Next(map.width / 10);
            int sy = random.Next(map.height / 10);
            MonsterTick(sx, sy, size, size, 20);

            if (quests.Count < 3 || quests.Count < map.creatures.Count * minimumQuestRatio)
            {
                if (questCompleted.DaysPassed() > 1 && mapAltered.DaysPassed() > 7)
                {
                    // Attempt to add quest source
                    // Add monster to dungeon
                    // Add new dungeon
                    // Add monsters to caves
                }
            }
        }
    }

    public void MonsterTick(int sx, int sy, int w, int h, int targetCount)
    {
        int count = 0;
        for(int x = sx; x < w; x++)
        {
            for (int y = sy; y < h; y++)
            {
                Location location = map.GetLocation(new Point(x, y));
                if (location.HasMonster())
                {
                    count++;
                }
                if (location.building != null)
                {
                    // Dungeons increase monster count
                    if (location.building.HasTag(BuildingTag.Dungeon))
                    {
                        targetCount += 5;
                    }
                    // Buildings decrease monster count
                    else
                    {
                        targetCount -= 1;
                    }
                }
            }
        }
        if (count < targetCount)
        {
            bool spawn = false;
            while (!spawn)
            {
                Point p = new Point(random.Next(sx, sx + w), random.Next(sy, sy + h));
                if (!map.GetLocation(p).HasMonster())
                {
                    Monster m = Monster.Monsters[random.Next(Monster.Monsters.Length)];
                    map.Add(m, null, p);
                    spawn = true;
                }
            }
        }
    }

    public void RequestQuest(Creature questGiver, Goal goal, PlayerAction wantedAction, string reason)
    {
        // Too many quests
        if (quests.Count > map.creatures.Count * maximumQuestRatio)
        {
            return;
        }

        // Create quest
        Quest quest = questBuilder.CreateQuest(questGiver, goal, wantedAction, reason);
        if (quest != null)
        {
            string str = "";
            foreach (QuestStep qs in quest.events)
            {
                str += ":" + qs.ToSimpleString();
            }
            foreach (Quest q in quests)
            {
                string s = "";
                foreach (QuestStep qs in q.events)
                {
                    s += ":" + qs.ToSimpleString();
                }
                if (s == str)
                {
                    return;
                }
            }
            quests.Add(quest);
            quest.Start();
            questAdded = TimeManager.Now();
        }
    }

    public uint PlayerKnowledge(Point p)
    {
        return playerKnowledgeGrid[p.x, p.y];
    }

    public int DaysSincePlayerVisited(Point p)
    {
        uint knowledge = PlayerKnowledge(p);
        // Never visited
        if (knowledge == 0)
        {
            return int.MaxValue;
        }
        return (int) ((tick - knowledge) / Time.Day);
    }

    public void PlayerAction(PlayerAction action, Location location)
    {
        // Free reservation, when that action is performed at that location
        if (action.action == reserveGrid[action.point.x, action.point.y])
        {
            reserveGrid[action.point.x, action.point.y] = ActionType.none;
        }

        // Each quest observes player's action
        foreach (Quest quest in quests)
        {
            quest.Observe(action, location);
        }
    }

    public void Remove(Quest quest, bool completed)
    {
        quests.Remove(quest);
        if (completed)
        {
            questCompleted = TimeManager.Now();
        }

    }

    public ActionType Get(Point p)
    {
        return reserveGrid[p.x, p.y];
    }

    public void FreeReserve(Point p)
    {
        reserveGrid[p.x, p.y] = ActionType.none;
    }

    public void Reserve(Point p, ActionType type)
    {
        reserveGrid[p.x, p.y] = type;
    }

}
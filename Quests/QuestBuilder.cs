using System.Collections;
using System.Collections.Generic;
using System;

public class QuestBuilder 
{
    private StoryManager storyManager;
    private Map map;
    public Random random;

    public QuestBuilder(StoryManager storyManager)
    {
        this.storyManager = storyManager;
        map = storyManager.map;
        random = new Random(map.seed * (map.seed - map.width));
    }

    public Quest CreateQuest(Creature questGiver, Goal goal, PlayerAction wantedAction, string reason)
    {
        // Wanted point is reserved for another quest, so cant create new one
        if (!wantedAction.point.Equals(Point.None) && storyManager.Get(wantedAction.point) != ActionType.none)
        {
            return null;
        }

        // QuestGiver already has a quest
        if (questGiver.quest != null)
        {
            return null;
        }

        Quest quest = null;

        // Killing requested
        if (wantedAction.action == ActionType.kill)
        {
            quest = KillQuest(questGiver, goal, wantedAction, reason);
        }

        // Explore request
        else if (wantedAction.action == ActionType.explore)
        {
            quest = ExploreQuest(questGiver, goal, wantedAction, reason);
        }

        // Get request
        else if (wantedAction.action == ActionType.get_item)
        {
             quest = GetQuest(questGiver, goal, wantedAction, reason);
        }

        // Use request
        else if (wantedAction.action == ActionType.use_item)
        {
            quest = UseQuest(questGiver, goal, wantedAction, reason);
        }

        if (quest == null)
        {
            return null;
        }

        // Calculate quest's reward value
        int value = 0;
        foreach(QuestStep step in quest.events)
        {
            value += step.GetValue();
        }
        quest.rewardCoins = (int) (random.Next(value) + random.Next(value) * ( 1f -questGiver.personality.greed));
        
        // Compare value to available resources
        if (quest.rewardCoins > questGiver.inventory.coins)
        {
            quest.rewardCoins = questGiver.inventory.coins;
            if (quest.rewardCoins < 0)
            {
                quest.rewardCoins = 0;
            }
        }

        return quest;
    }

    private Quest KillQuest(Creature questGiver, Goal goal, PlayerAction wantedAction, string reason)
    {
        Point home = questGiver.GetHome();
        Monster monster = map.GetLocation(wantedAction.point).GetMonster();

        // Decline, because quest giver doesn't know that monster is already dead
        if (monster == null)
        {
            return null;
        }

        // Monster is from source
        if (map.GetLocation(wantedAction.point).GetSource() != null)
        {
            Quest sourceQuest = SourceQuest(questGiver, goal, wantedAction, map.GetLocation(wantedAction.point).GetSource(), reason);
            if (sourceQuest != null)
            {
                return sourceQuest;
            }
        }

        // From here on questBuilder is committed on creating the quest
        //=============================================================
        Quest quest = new Quest(storyManager, questGiver, goal, reason);
        Rectangle area = GetAdventureArea(home, 12);
        List<Location> monsters = GetMonsters(area, monster);

        // KILL THE BOSS
        if (monster.IsBoss())
        {
            CraftBossItem(quest, monster);
            quest.AddStep(new QuestAction(quest, QuestType.goal, wantedAction, new Point[] { wantedAction.point }));
            return quest;
        }

        // Kill X MONSTERS
        // =============================================================
        else if (monsters.Count >= random.Next(3) + 2)
        {
            int count = random.Next(monsters.Count);
            if (count > 5)
            {
                count = random.Next(monsters.Count);
            }
            if (count < 2)
            {
                count = 2;
            }

            // Reserve monsters
            Point[] reserve = new Point[count];
            for (int i = 0; i < count; i++)
            {
                Point p = monsters[i].point;
                storyManager.Reserve(p, ActionType.kill);
                reserve[i] = p;
            }

            // Should we add single monster killing as first step
            if (random.Next(monsters.Count) >= 3)
            {
                QuestType type = QuestType.research;

                // Player not trusted, so test his skills
                if (questGiver.playerReputation < 5 && monsters.Count >= 3)
                {
                    type = QuestType.test;
                }

                // Kill one monster
                quest.AddStep(new QuestAction(quest, type, new PlayerAction(ActionType.kill, monsters[0].point, monster), new Point[] { monsters[0].point }));
                count--;
            }

            // Kill X monsters
            if (monsters.Count - count >= 3)
            {
                bool getItems = false;
                foreach(Item item in monster.loot)
                {
                    if (item != null && questGiver.memory.Require(item) > 0)
                    {
                        int amount = monsters.Count / 2;
                        if (amount > 5)
                        {
                            amount = 5;
                        }
                        if (amount == 0)
                        {
                            amount = 1;
                        }
                        getItems = true;

                        // Get items dropped by monsters
                        quest.AddStep(new QuestGet(quest, QuestType.opportunity, new ItemStack(item, amount), reserve));
                        return quest;
                    }
                }

                // Kill X
                if (!getItems) {
                    quest.AddStep(new QuestKill(quest, QuestType.first, monster, 3, reserve));
                }
            }
            quest.AddStep(new QuestKill(quest, QuestType.goal, monster, count, reserve));

            return quest;
        }

        // KILL (NORMAL) MONSTER
        // =============================================================
        else
        {
            storyManager.Reserve(wantedAction.point, wantedAction.action);
            wantedAction.target = monster;
            quest.AddStep(new QuestAction(quest, QuestType.goal, wantedAction, new Point[] { wantedAction.point }));
            return quest;
        }
    }

    public Quest SourceQuest(Creature questGiver, Goal goal, PlayerAction wantedAction, Building source, string reason)
    {
        Quest quest = new Quest(storyManager, questGiver, goal, reason);

        // Does questGiver know of the source?
        bool dungeonKnown = questGiver.memory.locations.Get(source.point) != null;

        Monster monster = map.GetLocation(wantedAction.point).GetMonster();
        Location sourceLocation = map.GetLocation(source.point);
        Monster sourceMonster = sourceLocation.GetMonster();

        // Source location can be modified
        if (storyManager.DaysSincePlayerVisited(source.point) > 7)
        {
            // Add monster if there isnt one already
            if (sourceMonster == null)
            {
                // Add monster to guard the ruins based on one that questGiver wants to kill
                Monster guard = monster;
                if (guard == null)
                {
                    guard = Monster.Monsters[random.Next(Monster.Monsters.Length)];
                }

                // Set boss to guard the source
                if (guard.GetBoss() != null && random.Next(100) < 25)
                {
                    guard = guard.GetBoss();
                }
                map.Add(guard, source, source.point);
                sourceMonster = map.GetLocation(source.point).GetMonster();
            }

            // Add obstacle if there is
            if (sourceLocation.GetObstacle() == null)
            {
                if (random.Next(100) < 50)
                {
                    map.Add(new Obstacle(Item.Meat, "Blood altar"), source.point);
                }
            }
  
        }

        Rectangle area = GetAdventureArea(source.point, 6);
        List<Location> monsters = GetMonsters(area, monster, source);

        // Should player be tested?
        if (questGiver.playerReputation < 10)
        {

        }

        if (monsters.Count > 3)
        {
            // Reserved points
            List<Point> killPoints = new List<Point>();
            foreach (Location loc in monsters)
            {
                killPoints.Add(loc.point);
            }

            if (dungeonKnown)
            {
                int amount = random.Next(monsters.Count);
                if (amount < 3)
                {
                    amount = 3;
                }
                else if (amount > 5)
                {
                    amount = random.Next(monsters.Count);
                }
                quest.AddStep(new QuestKill(quest, QuestType.before, monster, amount, killPoints.ToArray()));
            }

            else 
            {
                // Dungeon not known, so kill monsters to learn it's location
                if (random.Next(100) < 50)
                {
                    quest.AddStep(new QuestKill(quest, QuestType.research, monster, monsters.Count - 1, killPoints.ToArray()));
                }

                // Dungeon not known, so kill monsters and learn it's location
                else
                {
                    quest.AddStep(new QuestKill(quest, QuestType.first, monster, monsters.Count - 2, killPoints.ToArray()));
                    quest.AddStep(new QuestFind(quest, QuestType.before, source, source.point, new Point[] { }));
                }
            }
        }

        // Monster guarding the source
        if (sourceMonster != null)
        { 
            // Boss is guarding the source
            if (sourceMonster.IsBoss())
            {
                // Add item crafting, if boss can be killed with item
                CraftBossItem(quest, sourceMonster);
            }

            // Normal monster at the source
            else
            {

            }
        }

        // Add quest event for exploring the location
        quest.AddStep(new QuestAction(quest, QuestType.goal, new PlayerAction(ActionType.explore, source.point), new Point[] { source.point }));
        return quest;
    }

    public Quest ExploreQuest(Creature questGiver, Goal goal, PlayerAction wantedAction, string reason)
    {
        Quest quest = new Quest(storyManager, questGiver, goal, reason);

        // Explore dungeon
        Location location = map.GetLocation(wantedAction.point);
        bool dungeon = location.building != null && location.building.HasTag(BuildingTag.Dungeon);
        if (dungeon && questGiver.random.Next(100) < 50)
        {
            return SourceQuest(questGiver, goal, wantedAction, location.building, reason);
        }
        
        // Explore location
        else
        {
            // Boss
            if (dungeon)
            {
                if (location.HasMonster() && location.GetMonster().IsBoss())
                {
                    CraftBossItem(quest, location.GetMonster());
                }
            }

            // Get valuable item
            if (location.items.Contains(ItemType.Treasure) && questGiver.random.Next(100) < questGiver.personality.greed * 50)
            {
                quest.AddStep(new QuestGet(quest, QuestType.opportunity, location.items.list[0], new Point[] { wantedAction.point }));
            }

            else if (questGiver.random.Next(100) < 30)
            {
                quest.AddStep(new QuestAction(quest, QuestType.solution, new PlayerAction(ActionType.use_item, wantedAction.point, Item.Wood), new Point[] { wantedAction.point }));
            }

            // Just explore
            else
            {
                quest.AddStep(new QuestAction(quest, QuestType.goal, wantedAction, new Point[] { wantedAction.point }));

                // Also seal
                if (questGiver.random.Next(100) < 30)
                {
                    quest.AddStep(new QuestAction(quest, QuestType.solution, new PlayerAction(ActionType.use_item, wantedAction.point, Item.Wood), new Point[] { wantedAction.point }));
                }
            }
        }
        return quest;
    }

    public Quest GetQuest(Creature questGiver, Goal goal, PlayerAction wantedAction, string reason)
    {
        Quest quest = new Quest(storyManager, questGiver, goal, reason);

        // Get food
        if (wantedAction.target is ItemType type)
        {
            if (type != ItemType.Food && questGiver.inventory.coins < 20)
            {
                return null;
            }

            Item bestItem = null;
            foreach (Item item in Item.items)
            {
                if (item.IsType(type))
                {
                    // ToDo: Check that item is available
                    bestItem = item;
                    break;
                }
            }
            if (bestItem == null)
            {
                return null;
            }

            // Expensive item can be crafted from recipe
            if (bestItem.cost > 50)
            {
                List<Memory_Item> mems = questGiver.memory.items.Get(-1, type);
                if (mems.Count > 0)
                {
                    //quest.AddStep(new QuestGet(quest, QuestType.goal, new ItemStack(bestItem, count), new Point[] { }));
                }

                if (questGiver.random.Next(100) < 50)
                {
                    quest.AddStep(new QuestCraft(quest, QuestType.solution, new SkillWork("expensive recipe", Skill.brewing, 60, new ItemStack(bestItem), new ItemStack(Item.LifePotion)), new Point[] { }));
                }
            }

            int count = questGiver.inventory.coins / bestItem.cost;
            if (count < 1)
            {
                count = 1;
            }
            if (bestItem.cost < 5 && count < 5)
            {
                count = 5;
            }
            quest.AddStep(new QuestGet(quest, QuestType.goal, new ItemStack(bestItem, count), new Point[] { }));
        }
        // Get specific stack
        else
        {
            quest.AddStep(new QuestGet(quest, QuestType.goal, wantedAction.target as ItemStack, new Point[] { }));
        }
        return quest;
    }

    public Quest UseQuest(Creature questGiver, Goal goal, PlayerAction wantedAction, string reason)
    {
        Quest quest = new Quest(storyManager, questGiver, goal, reason);
        if (wantedAction.point.Equals(Point.None))
        {
            quest.AddStep(new QuestAction(quest, QuestType.goal, wantedAction, new Point[] { }));
        }
        else
        {
            quest.AddStep(new QuestAction(quest, QuestType.goal, wantedAction, new Point[] { wantedAction.point }));
        }
        return quest;
    }

    private bool CraftBossItem(Quest quest, Monster boss)
    {
        List<Item> killers = boss.GetSlayingItem();
        if (killers.Count > 0)
        {
            quest.AddStep(new QuestCraft(quest, QuestType.solution, new SkillWork(boss.name + " slayer", Skill.brewing, 60, new ItemStack(killers[random.Next(killers.Count)]), new ItemStack(Item.Fish, 3)), new Point[] { }));
            return true;
        }
        return false;
    }

    /*==============================================================================
     * DATA GETTERS
     ==============================================================================*/

    private Rectangle GetVillageArea(int border)
    {
        //int xStart, xEnd, yStart, yEnd = 0;
        int xStart = map.width;
        int xEnd = 0;
        int yStart = map.height;
        int yEnd = 0;

        foreach (Creature c in map.creatures)
        {
            Point p = c.location;
            if (p.x < xStart)
            {
                xStart = p.x;
            }
            else if (p.x > xEnd)
            {
                xEnd = p.x;
            }

            if (p.y < yStart)
            {
                yStart = p.y;
            }
            else if (p.y > yEnd)
            {
                yEnd = p.y;
            }
        }

        xStart = Math.Max(0, xStart - border);
        yStart = Math.Max(0, yStart - border);
        xEnd = Math.Min(map.width - 1, xEnd + border);
        yEnd = Math.Min(map.height - 1, yEnd + border);
        return new Rectangle(new Point(xStart, yStart), new Point(xEnd, yEnd));
    }

    private Rectangle GetAdventureArea(Point center, int border)
    {
        //int xStart, xEnd, yStart, yEnd = 0;
        int xStart = center.x - border;
        int xEnd = center.x + border;
        int yStart = center.y - border;
        int yEnd = center.y + border;

        // x-bounds
        if (xStart < 0)
        {
            xStart = 0;
            xEnd = border * 2;
        }
        else if (xEnd >= map.width)
        {
            xStart = map.width - border * 2;
            xEnd = map.width - 1;
        }

        // y-bounds
        if (yStart < 0)
        {
            yStart = 0;
            yEnd = border * 2;
        }
        else if (yEnd >= map.height)
        {
            yStart = map.height - border * 2;
            yEnd = map.height - 1;
        }
        return new Rectangle(new Point(xStart, yStart), new Point(xEnd, yEnd));
    }

    private List<Location> GetMonsters(Rectangle area, Monster monster = null, Building source = null)
    {
        List<Location> locations = new List<Location>();
        for(int x = area.xStart; x < area.xEnd; x++)
        {
            for(int y = area.yStart; y < area.yEnd; y++)
            {
                Location location = map.GetLocation(new Point(x, y));

                // Location has a monster
                if (!location.HasMonster())
                {
                    continue;
                }

                // Monster equals required monster type
                if (monster != null && !monster.Equals(location.GetMonster()))
                {
                    continue;
                }

                // Monster is from required source
                if (source != null && location.GetSource() != source) {
                    continue;
                }

                // Not reserved by other quest
                if (storyManager.Get(new Point(x,y)) != ActionType.none)
                {
                    continue;
                }

                // Add to the list
                locations.Add(location);
            }
        }
        return locations;
    }

    private List<Location> GetMapObjects(Rectangle area, MapObject mapObject = null)
    {
        List<Location> locations = new List<Location>();
        for (int x = area.xStart; x < area.xEnd; x++)
        {
            for (int y = area.yStart; y < area.yEnd; y++)
            {
                Location location = map.GetLocation(new Point(x, y));
                if (location.objects.Count > 0 && (mapObject == null || location.Contains(mapObject)))
                {
                    // Not reserved by other quest
                    if (storyManager.Get(new Point(x, y)) == ActionType.none)
                    {
                        locations.Add(location);
                    }
                }
            }
        }
        return locations;
    }

    private List<Location> GetObjectTypes(Rectangle area, ObjectType type)
    {
        List<Location> locations = new List<Location>();
        for (int x = area.xStart; x < area.xEnd; x++)
        {
            for (int y = area.yStart; y < area.yEnd; y++)
            {
                Location location = map.GetLocation(new Point(x, y));
                if (location.Contains(type))
                {
                    // Not reserved by other quest
                    if (storyManager.Get(new Point(x, y)) == ActionType.none)
                    {
                        locations.Add(location);
                    }
                }
            }
        }
        return locations;
    }

    public List<Building> FindBuildings(int buildingId, Rectangle area)
    {
        List<Building> list = new List<Building>();
        for (int x = area.xStart; x <= area.xEnd; x++)
        {
            for (int y = area.yStart; y <= area.yEnd; y++)
            {
                Building b = map.GetLocation(x, y).building;
                if (b != null && (buildingId == -1 || buildingId == b.id))
                {
                    list.Add(b);
                }
            }
        }
        return list;
    }

    public List<Creature> KnownBy(MapObject mapObject, int lastVisited = int.MaxValue)
    {
        List<Creature> list = new List<Creature>();
        foreach(Creature creature in map.creatures)
        {
            if (creature.memory.locations.GetObjects(mapObject, lastVisited).Count > 0)
            {
                list.Add(creature);
            }
        }
        return list;
    }

    public List<Creature> KnownBy(Point point, int lastVisited = int.MaxValue)
    {
        List<Creature> list = new List<Creature>();
        foreach (Creature creature in map.creatures)
        {
            if (creature.memory.locations.Get(point).LastSeen() < lastVisited)
            {
                list.Add(creature);
            }
        }
        return list;
    }

    // THIS IS BROKEN! FIX PEFORE USING!
    /*
    public Quest CreateStory()
    {
        // Create story area
        Rectangle storyArea = GetAdventureArea(10);

        // Get ruins in the area
        List<Building> ruins = FindBuildings(Schematic.Ruins.id, storyArea);

        Creature questGiver = null;

        if (ruins.Count > 0)
        {
            // Player knowledge
            ObjectValueList list = new ObjectValueList();
            foreach (Building b in ruins)
            {
                Creature best = null;
                int bestValue = int.MinValue;
                foreach (Creature c in map.creatures)
                {
                    // Each agent can have only one quest
                    if (c.quest != null)
                    {
                        continue;
                    }

                    // See if agent has goal that will lead to a quest
                    //if (c.goalManager.HasQuestGoal())
                    {
                        //continue;
                    }

                    // Distance to ruin is reduced
                    int cv = -c.GetHome().Distance(b.point);

                    // Curiosity is added
                    cv += (int)(20 * c.personality.curiosity);

                    if (cv > bestValue)
                    {
                        bestValue = cv;
                        best = c;
                    }
                }
                int value = bestValue;

                // Player hasnt visited the location
                if (storyManager.PlayerKnowledge(b.point) == 0)
                {
                    value += 100;
                }
                // More than week since last visit
                else if (storyManager.PlayerKnowledge(b.point) > storyManager.tick - Time.Day * 7)
                {
                    value += 20;
                }
                //Less than week since last visit
                else
                {
                    value -= 100;
                }
                list.Add(b.point, value);
                if (list.list[0].data.Equals(b.point))
                {
                    questGiver = best;
                }
            }

            // No questgiver found
            if (questGiver == null)
            {
                return null;
            }

            // Create quest
            Quest quest = new Quest(storyManager, questGiver);
            Point p = (Point)list.list[0].data;
            Goal goal = new Goal_Travel(new Need_Custom(questGiver, 75, 1, "what is that"), 75, p, "that place looks interesting");
            //AgentQuest aq = new AgentQuest(quest, questGiver, goal, new PlayerAction(ActionType.explore, p), "I want you to explore ruins in " + list.list[0].data + ".");
            //quest.SetAgentQuest(aq);
            //goal.SetQuest(aq);
            return quest;

        }
        else
        {
            // Create place to investigate
        }

        // Find curious villagers
        return null;
    }
    */
}
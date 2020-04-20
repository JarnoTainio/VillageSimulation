using System;
using System.Collections;
using System.Collections.Generic;

public class Player
{
    Map map;

    public Point point;

    public PlayerMemoryManager memory;
    public List<QuestStep> quests;
    public Random random;

    public Inventory inventory;

    public Creature selectedCreature;

    public int maxLife;
    public int life;

    public int level;
    public int experience;
    public int requiredXp;

    public Player(Map map, Point p)
    {
        this.map = map;
        this.point = p;
        quests = new List<QuestStep>();
        memory = new PlayerMemoryManager();
        level = 0;
        LevelUp();
        maxLife = life = 20;
        random = new Random();
        inventory = new Inventory();
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        
        if (experience >= requiredXp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        experience -= requiredXp;
        level++;
        requiredXp = (level + 2) * level + 3;
        if (level == 1)
        {
            experience = 0;
        }
        maxLife += level;
        life = maxLife;
    }

    public void ModifyLife(int amount)
    {
        life += amount;
        if (life < 0)
        {
            life = 0;
        }
        else if (life > maxLife)
        {
            life = maxLife;
        }
    }

    public void Tick(bool newDay)
    {

    }

    public void Move(Point direction)
    {
        point = point.Add(direction);
        PerformAction(new PlayerAction(ActionType.move, point));
    }

    public Item Kill()
    {
        PerformAction(new PlayerAction(ActionType.kill, point, map.GetLocation(point).GetMonster()));
        ModifyLife(-map.GetLocation(point).GetMonster().strength);
        AddExperience(1);
        return map.KillMonster(random, point);
    }

    public void Explore()
    {
        PerformAction(new PlayerAction(ActionType.explore, point));
        Location location = map.GetLocation(point);
        foreach(ItemStack stack in location.items.list)
        {
            inventory.Add(stack);
            Dummy.instance.playerController.Add(stack);
        }
        location.items.Clear();
    }

    public void Talk(Creature target)
    {
        PerformAction(new PlayerAction(ActionType.talk, point, target));
    }

    public void UseItem(Item item)
    {
        PerformAction(new PlayerAction(ActionType.use_item, point, item));
    }

    public void AcceptQuest(Creature questGiver)
    {
        quests.Add(questGiver.quest.quest);
        questGiver.quest.AcceptQuest();
    }

    public void CompleteQuest(QuestStep quest)
    {
        quests.Remove(quest);
    }

    private void PerformAction(PlayerAction action)
    {
        memory.Add(action);
        map.storyManager.PlayerAction(action, map.GetLocation(point));
    }

    public void AddItem(Item item)
    {
        inventory.Add(item);
        PerformAction(new PlayerAction(ActionType.get_item, point, item));
    }

    public void RemoveItem(Item item)
    {
        inventory.Remove(item);
        PerformAction(new PlayerAction(ActionType.get_item, point, item));
    }

}


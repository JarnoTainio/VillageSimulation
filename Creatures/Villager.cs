using System.Collections.Generic;

public class Villager : Creature
{
    public static int AgeLimit = 28 * 4;

    public int oldAge;
    private int tickCount = 0;
    private int spawningDay;

    public Villager(Map map, Point location, Villager parent = null) : base(map, location, parent)
    {
        tickCount = 0;
        daysLived = 0;
        oldAge = AgeLimit / 2 + random.Next(AgeLimit);
        spawningDay = AgeLimit / 2 + random.Next(AgeLimit / 2);
        if (parent == null)
        {
            daysLived = random.Next(AgeLimit / 2) + random.Next(AgeLimit / 2);
        }
    }

    /*========================================================================
     * B A S I C   L O G I C
    ========================================================================*/

    public override void Tick(Map map, int delta)
    {
        
        if (!alive)
        {
            return;
        }

        // Observe current location before taking action
        Observe(map.GetLocation(location));

        if (tickCount == 0)
        {
            if (mindState != MindState.Sleeping)
            {
                tired += delta / 60f;
            }
            UpdateTired();
        }
        tickCount++;

        if (social.IsListening())
        {
            social.TickTalk(delta);
            return;
        }

        // If not doing anything, then make a plan
        if (currentAction == null || tiredState >= Tired.Very || hungerState >= Hunger.Very)
        {
            // Get next action from GoalManager
            currentAction = goalManager.NextAction();

            if (currentAction == null)
            {
                PrintMessage("ERROR! Current action still null!", true);
                currentAction = new Idle(null, 0, "confused..");
                tickCount = 0;
                return;
            }
        }

        // If action returns negative time, then that is unused time
        int remainingTime = -currentAction.Tick(map, this, delta);
        if (delta == remainingTime)
        {
            remainingTime--;
        }

        // If more than minute remain of time, then retick
        if (remainingTime > 0)
        {
            if (remainingTime >= delta)
            {
                PrintMessage("That didnt take any time... delta " +delta +" remainign: "+remainingTime + " Action: " + currentAction + " Last: " + goalManager.lastAction, true);
            }
            if (tickCount > 5)
            {
                PrintMessage("HIGH TICK COUNT! " + tickCount + " delta: " + delta + " remaining: " + remainingTime + " Action: " +currentAction + " Last: " + goalManager.lastAction, true);
                if (tickCount >= 10)
                {
                    tickCount = 0;
                    return;
                }
            }
            Tick(map, remainingTime);
        }
        tickCount = 0;
    }

    public override void TickDay(Map map)
    {
        if (!alive)
        {
            return;
        }

        foreach (Memory_Building bm in memory.buildings.GetOwned(ID))
        {
            if (bm.work?.required != null)
            {
                memory.itemCounter.Consumed(bm.work.required);
            }
            if (bm.work?.produced != null)
            {
                memory.itemCounter.Produced(bm.work.produced);
            }
        }

        // Safety kill for rare bug, that I dont have time to solve
        if (memory.locations.Get().Count > 1000)
        {
            Die("Went crazy..");
            return;
        }

        if (quest != null && quest.created.DaysPassed() > quest.GetWaitTime())
        {
            //PrintMessage("ABANDON QUEST", true);
            quest.Abandon();
        }

        random = new System.Random(map.seed * ID);
        hunger++;
        daysLived++;

        if ((((daysLived + 1) % (spawningDay) == 0)))
        {
            float ratio = Dummy.instance.villagerCount / map.creatures.Count;
            if (ratio < 1.25f || random.Next(100) < (100 / (ratio * ratio)))
            {
                Dummy.instance.CreateIcon(1, location);
                new Villager(map, location, this);
            }
            else
            {
                spawningDay += random.Next(AgeLimit / 10);
            }
        }

        if (daysLived > oldAge)
        {
            float diff = daysLived - oldAge;
            float rollSize = 1 - (diff / (oldAge / 2f));
            rollSize *= oldAge / 2;
            if (rollSize < 2)
            {
                rollSize = 2;
            }
            if (random.Next((int)rollSize) == 0)
            {
                Die("Old age " + daysLived +" / "+oldAge );
                return;
            }
        }


        UpdateHunger();
        UpdateTired();

        if (tiredState > Tired.Extremely)
        {
            Die("Tired");
            return;
        }
        else if (hungerState > Hunger.Starving)
        {
            Die("Hungry");
            return;
        }

        goalManager.TickDay();
    }

    public override void Die(string reason)
    {
        if (quest != null)
        {
            quest.Abandon();
        }
        base.Die(reason);
    }

    public override string ToString()
    {
        return name;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CreatureInspector : MonoBehaviour
{
    public Creature creature;
    public Dummy dummy;

    public bool follow;

    [Header("Info")]
    public TextMeshProUGUI clock;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI targetSpeed;
    public new TextMeshProUGUI name;

    [Header("Stats")]
    public TextMeshProUGUI hunger;
    public TextMeshProUGUI tired;

    [Header("Location")]
    public TextMeshProUGUI location;
    public TextMeshProUGUI homeDistance;

    [Header("Actions")]
    public TextMeshProUGUI action;
    public TextMeshProUGUI plan;
    public TextMeshProUGUI goal;
    public TextMeshProUGUI need;

    [Header("Data")]
    public TextMeshProUGUI goals;
    public TextMeshProUGUI inventory;

    [Header("Memory")]
    public TextMeshProUGUI memoryItems;
    public TextMeshProUGUI memoryBuildings;
    public TextMeshProUGUI memoryCounter;
    public TextMeshProUGUI memorySize;

    [Header("Skills")]
    public TextMeshProUGUI stats;
    public TextMeshProUGUI skills;

    [Header("Social")]
    public TextMeshProUGUI socialCounter;
    public TextMeshProUGUI talk;

    public void SetCreature(Creature c)
    {
        this.creature = c;
        name.text = "Name: " + creature.name;
        Camera.main.transform.localPosition = new Vector3(creature.location.x + 2.5f, creature.location.y + 2.5f, Camera.main.transform.localPosition.z);
    }

    public void Update()
    {
        if (creature == null)
        {
            return;
        }
        targetSpeed.text = "" + (dummy.ticksPerSecond * dummy.delta);
        speed.text = dummy.speed;
        clock.text = TimeManager.GetCompactTime();

        hunger.text     = "Hunger: " + (int)creature.hunger;
        tired.text      = "Tired: " + (int)creature.tired;

        location.text = "Location: " + creature.location + "(" + Tile.tiles[dummy.map.GetLocation(creature.location).tile].name + ")";
        homeDistance.text = "Home: " + (creature.hasHome ? ""+creature.distanceToHome : "null") + "\nState: "+creature.GetMindState();

        Action a = creature.goalManager.lastAction;
        Plan p = a != null ? a.source : null;
        Goal g = p != null ? p.source : null;
        Need n = g != null ? g.source : null;

        action.text     = "Action: "+ (a == null ? "null" : a.name);
        plan.text       = "Plan: " + (p == null ? "null" : p.name + " (" + p.actions.Count + ")");
        goal.text       = "Goal: "  + (g == null ? "null" : g.name);
        need.text       = "Need: "  + (n == null ? "null" : n.name);

        goals.text = "GOALS:\n";
        foreach(Goal goal in creature.goalManager.GetGoals())
        {
            goals.text += goal.name + "(" + goal.strength + ")\n";
        }

        inventory.text = "ITEMS: ("+creature.inventory.coins +" coins)\n";
        foreach (ItemStack stack in creature.inventory.GetItems())
        {
            inventory.text += stack.ToString() + "\n";
        }
        inventory.text += "-------------------------\n";
        foreach (Memory_Item mem in creature.memory.items.Get(creature.ID))
        {
            inventory.text += mem.stack.ToString() + mem.point + "\n";

            foreach(ItemStack stack in creature.map.GetLocation(mem.point).items.list)
            {
                inventory.text += "-" + stack + "\n";
            }
        }

        memoryItems.text = "Locations:  [" + creature.memory.locations.Get().Count + "] \n";
        memoryItems.text += "Buildings: [" + creature.memory.buildings.Get().Count + "] \n";
        memoryItems.text += "Events:    [" + creature.memory.events.events.Count + "] \n";
        memoryItems.text += "Actions:   [" + creature.memory.events.actions.Count + "] \n";

        int max = 20;
        for (int i = creature.memory.events.actions.Count - 1; i >= 0; i--)
        {
            Memory_Event me = creature.memory.events.actions[i];
            memoryItems.text += me.ToString() + "\n";
            max--;
            if (max == 0)
            {
                break;
            }
        }

        memoryBuildings.text = "Owned Buildings:\n";
        memoryBuildings.text = "Source of food: " + creature.memory.HasSourceOfFood() + "\n";
        foreach (Memory_Building mb in creature.memory.buildings.GetOwned(creature.ID))
        {
            memoryBuildings.text += mb.GetName() + " " + mb.point + "\n";
        }

        memorySize.text = "Loc: " + creature.memory.locations.Get().Count 
                        + " Buil: " + creature.memory.buildings.Get().Count
                        +" Item: " + creature.memory.items.Get().Count;

        memoryCounter.text = "Counter:\n" + creature.memory.itemCounter.Print();

        stats.text = "ATTRIBUTES:\n";
        stats.text += "Night:    " + creature.personality.sleepingTime + "\n";
        stats.text += "Sleep:    " + creature.personality.sleepDuration + "\n";
        stats.text += "Morning:  " + creature.personality.awakeningTime + "\n";
        stats.text += "Distance: " + creature.personality.searchDistance + "\n";
        stats.text += "\nPatience: " + Math.Round(creature.personality.patience, 2) + "\n";
        stats.text += "Greed:    " + Math.Round(creature.personality.greed, 2) + "\n";
        stats.text += "Social:   " + Math.Round(creature.personality.social, 2) + "\n";
        stats.text += "Curiosity:" + Math.Round(creature.personality.curiosity, 2) + "\n";
        stats.text += "\nTravel:   " + Math.Round(creature.travelSpeed, 2) + "\n";

        skills.text = "SKILLS:\n";
        for(int i = 0; i< creature.personality.skills.Length; i++)
        {
            if (creature.personality.skills[i] > 1)
            {
                skills.text += Skill.skills[i].name.Substring(0,6) + ": \t" + Math.Round(creature.personality.skills[i], 2) + "\t [" + Math.Round(creature.personality.passion[i], 2) + "]\n";
            }
        }

        socialCounter.text = "Agents: (" + creature.memory.social.Get().Count +  " / " + creature.memory.social.GetKnown().Count + " \n";
        talk.text = "Talking: " + creature.social.IsTalking();
        if (creature.social.IsTalking())
        {
            talk.text += "\n" + creature.social.current.name + " steps: " + creature.social.currentSteps +" listening: " + creature.social.listening;
        }
        foreach(Memory_Agent mem in creature.memory.social.Get())
        {
            talk.text += "\n" + mem.name + " seen: " + mem.lastSeen.DaysPassed() + " talked: " + mem.lastTalked.DaysPassed();
            if (mem.produces.list.Count > 0)
            {
                talk.text += "\nProduces: ";
                foreach (ItemStack s in mem.produces.list)
                {
                    talk.text += s + " ";
                }
            }
            if (mem.requires.list.Count > 0)
            {
                talk.text += "\nRequires: ";
                foreach (ItemStack s in mem.requires.list)
                {
                    talk.text += s + " ";
                }
            }
        }

        if (follow)
        {
            Camera.main.transform.localPosition = new Vector3(creature.location.x, creature.location.y, Camera.main.transform.localPosition.z);
        }
    }

    public void NextCreature()
    {
        List<Creature> creatures = Dummy.instance.creatures;
        if (creatures.Count == 0)
        {
            creature = null;
            return;
        }
        int index = creatures.IndexOf(creature) + 1;
        if (index >= creatures.Count)
        {
            index = 0;
        }
        SetCreature(creatures[index]);
    }

    public void PreviousCreature()
    {
        List<Creature> creatures = Dummy.instance.creatures;
        if (creatures.Count == 0)
        {
            creature = null;
            return;
        }
        int index = creatures.IndexOf(creature) - 1;
        if (index < 0)
        {
            index = creatures.Count - 1;
        }
        SetCreature(creatures[index]);
    }
}

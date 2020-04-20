using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CraftManager : MonoBehaviour
{
    public PlayerController playerController;
    public ItemSlot requiredSlot;
    public ItemSlot producedSlot;
    public TextMeshProUGUI costText;
    public Button craftButton;
    public Button nextButton;
    int cost;
    List<SkillWork> crafts;
    SkillWork work;

    int index;
    Creature crafter;

    public bool SetOptions(Creature creature)
    {
        index = 0;
        crafter = creature;
        crafts = new List<SkillWork>();
        foreach (Memory_Building building in creature.memory.buildings.GetOwned(creature.ID))
        {
            SkillWork w = building.work;
            if (w != null && w.required != null && !crafts.Contains(w))
            {
                crafts.Add(w);
            }
        }
        if (creature.quest?.craft != null)
        {
            crafts.Add(creature.quest.craft);
        }
        if (crafts.Count > 0) {
            ShowCraft();
            return true;
        }
        return false;
    }

    public void ShowCraft()
    {
        if (crafts.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        nextButton.gameObject.SetActive(crafts.Count > 1);
        work = crafts[index];
        craftButton.enabled = playerController.inventoryManager.Contains(work.required);
        Dummy.PrintMessage("CRAFT: Show " + work.name);

        // Set item data
        requiredSlot.SetItem(work.required);
        producedSlot.SetItem(work.produced);
        
        // Set sprites
        playerController.inventoryManager.SetSprite(requiredSlot);
        playerController.inventoryManager.SetSprite(producedSlot);

        // Update cost
        if (crafter.quest?.craft != null && crafter.quest.craft == work)
        {
            cost = 0;
        }
        else
        {
            cost = work.produced.item.cost - work.required.item.cost;
            if (cost <= 0)
            {
                cost = 1;
            }
        }
        costText.text = cost + " c";
    }

    public void Next()
    {
        index++;
        if (index >= crafts.Count)
        {
            index = 0;
        }
        ShowCraft();
    }

    public void Craft()
    {
        for (int i = 0; i < work.required.count; i++) {
            playerController.Remove(work.required.item);
        }
        for (int i = 0; i < work.produced.count; i++)
        {
            playerController.Add(work.produced.item);
        }
        playerController.ModifyMoney(-cost);
        ShowCraft();
    }
}

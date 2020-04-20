using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public PlayerController playerController;
    public DialogueManager dialogueManager;
    public List<ItemSlot> itemSlots;
    public ItemSlot itemSlotPrefab;

    public Sprite[] itemSprites;

    private void Start()
    {
        for (int i = 1; i < 6; i++)
        {
            Add(Item.items[i]);
            Add(Item.items[i]);
            Add(Item.items[i]);
            Add(Item.items[i]);
            Add(Item.items[i]);
        }
        Add(Item.items[13]);
        Add(Item.items[14]);
        Add(Item.items[15]);
    }

    public void Add(Item item)
    {
        foreach(ItemSlot slot in itemSlots)
        {
            if (slot.Equals(item)){
                slot.Add();
                return;
            }
        }
        ItemSlot newSlot = Instantiate(itemSlotPrefab, transform);
        newSlot.SetItem(item);
        if (item.id < itemSprites.Length)
        {
            newSlot.SetSprite(itemSprites[item.id]);
        }
        else
        {
            newSlot.SetSprite(itemSprites[0]);
        }
        itemSlots.Add(newSlot);
    }

    private void Update()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].isClicked)
            {
                itemSlots[i].isClicked = false;
                if (itemSlots[i].costObject.gameObject.activeSelf)
                {
                    dialogueManager.Sell(itemSlots[i].item);
                }
                else
                {
                    playerController.ItemClicked(itemSlots[i].item);
                }
                break;
            }
        }
    }

    public void Remove(Item item)
    {
        foreach (ItemSlot slot in itemSlots)
        {
            if (slot.Equals(item))
            {
                slot.Remove();
                return;
            }
        }
    }

    public void SetSellable(ItemStackList list)
    {
        foreach(ItemSlot slot in itemSlots)
        {
            slot.costObject.SetActive(false);
            foreach(ItemStack stack in list.list)
            {
                if (slot.Equals(stack.item))
                {
                    slot.costObject.SetActive(true);
                    break;
                }
            }
        }
    }

    public void HideCosts()
    {
        foreach (ItemSlot slot in itemSlots)
        {
            slot.costObject.SetActive(false);
        }
    }

    public void SetSprite(ItemSlot slot)
    {
        slot.SetSprite(itemSprites[slot.item.id - 1]);
    }

    public bool Contains(ItemStack stack)
    {
        foreach (ItemSlot slot in itemSlots)
        {
            if (slot.Equals(stack.item))
            {
                return slot.count >= stack.count;
            }
        }
        return false;
    }
}

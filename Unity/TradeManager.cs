using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeManager : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public ItemSlot[] slots;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].isClicked)
            {
                slots[i].isClicked = false;
                slots[i].Remove();
                dialogueManager.Buy(i);
            }
        }
    }

    public void SetItems(ItemStackList items)
    {
        foreach(ItemSlot slot in slots)
        {
            slot.gameObject.SetActive(false);
        }
        for (int i = 0; i < items.list.Count; i++) 
        {
            slots[i].gameObject.SetActive(true);
            slots[i].SetItem(items.list[i].item);
            slots[i].Add(items.list[i].count - 1);
            slots[i].costObject.SetActive(true);
            dialogueManager.playerController.inventoryManager.SetSprite(slots[i]);
        }
    }

}

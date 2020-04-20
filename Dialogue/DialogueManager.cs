using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueObject;
    public GameObject answerOptions;
    public TradeManager trade;
    public CraftManager craft;

    public TextMeshProUGUI response;
    public Button craftButton;
    public Button questButton;
    public Button helpButton;

    public PlayerController playerController;

    Dialogue dialogue;
    public int taskIndex;
    public int helpIndex;

    Creature creature;

    private void Start()
    {
    }

    public void StartDialogueWithCurrent()
    {
        playerController.GamePaused(true);
        StartDialogue(Dummy.instance.inspector.creature);
        taskIndex = 0;
        helpIndex = 0;
    }

    public void StartDialogue(Creature creature)
    {
        this.creature = creature;
        if (dialogue == null)
        {
            dialogue = new Dialogue(Dummy.instance.map.player);
        }
        //Dummy.instance.paused = true;
        questButton.gameObject.SetActive(creature.quest != null);
        dialogue.SetCreature(creature);
        SetMessage(dialogue.Hello());
        dialogueObject.SetActive(true);
        HideOptions();
        bool canCraft = craft.SetOptions(creature);
        craftButton.gameObject.SetActive(canCraft);
        helpButton.gameObject.SetActive(playerController.player.quests.Count > 0);
    }

    public void EndDialogue()
    {
        //Dummy.instance.paused = false;
        playerController.GamePaused(false);
        dialogueObject.SetActive(false);
        
    }

    public void HideOptions()
    {
        trade.gameObject.SetActive(false);
        craft.gameObject.SetActive(false);
        answerOptions.SetActive(false);
        playerController.inventoryManager.HideCosts();
    }

    private void SetMessage(string msg)
    {
        response.text = msg;
    }

    public void AskHome()
    {
        SetMessage(dialogue.Home());
        HideOptions();
    }

    public void AskWhat()
    {
        SetMessage(dialogue.What());
        HideOptions();
    }

    public void AskPersonality()
    {
        SetMessage(dialogue.Personality());
        HideOptions();
    }

    public void AskToday()
    {
        SetMessage(dialogue.Today());
        HideOptions();
    }

    public void AskTask()
    {
        SetMessage(dialogue.Task(taskIndex++));
        HideOptions();
    }

    public void AskQuest()
    {
        AgentQuest current = creature.quest;
        SetMessage(dialogue.Quest());
        HideOptions();
        if (creature.quest != null)
        {
            if (current == creature.quest)
            {
                answerOptions.SetActive(dialogue.creature.quest.IsAvailable());
            }
        }
        else
        {
            questButton.gameObject.SetActive(false);
        }
        bool canCraft = craft.SetOptions(creature);
        craftButton.gameObject.SetActive(canCraft);
        helpButton.gameObject.SetActive(playerController.player.quests.Count > 0);
    }

    public void AskTrade()
    {
        SetMessage(dialogue.Trade());
        HideOptions();
        trade.gameObject.SetActive(true);
        trade.SetItems(dialogue.sell);
        playerController.inventoryManager.SetSellable(dialogue.buy);
    }

    public void AskCraft()
    {
        SetMessage("");
        craft.SetOptions(creature);
        HideOptions();
        craft.gameObject.SetActive(true);
    }

    public void Accept()
    {
        SetMessage("Quest accepted!");
        HideOptions();
        playerController.AcceptQuest();
    }

    public void Decline()
    {
        SetMessage("Quest removed..");
        HideOptions();
        creature.quest.Abandon();
        questButton.gameObject.SetActive(false);
    }

    public void Buy(int index)
    {
        Item item = dialogue.sell.list[index].item;
        Dummy.PrintMessage("PLAYER BUY: " + item.name);
        playerController.ModifyMoney( -item.cost );
        playerController.inventory.Add(item);
        playerController.inventoryManager.Add(item);

        creature.inventory.Remove(item);
        creature.inventory.coins += item.cost;

        AskTrade();
    }

    public void Sell(Item item)
    {
        playerController.ModifyMoney(item.cost);
        playerController.inventory.Remove(item);
        playerController.inventoryManager.Remove(item);

        creature.inventory.Add(item);
        creature.inventory.coins -= item.cost;

        AskTrade();
    }

    public void AskHelp()
    {
        SetMessage(dialogue.Help(playerController.player.quests[helpIndex]));
        helpIndex++;
        if (helpIndex == playerController.player.quests.Count)
        {
            helpIndex = 0;
        }
    }

}

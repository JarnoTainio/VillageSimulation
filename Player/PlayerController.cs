using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public CameraController cameraController;
    public float zoom;
    public DialogueManager dialogue;
    public InventoryManager inventoryManager;
    public TextMeshProUGUI coinsText;

    Location location;
    Map map;
    int targetIndex;
    Creature target;
    public Player player;
    public Inventory inventory;

    public float gameSpeed = 1f;
    public bool gamePaused;
    float timeCounter;

    public Button talkButton;
    public Button nextButton;
    public Button killButton;
    public Button exploreButton;
    public Button restButton;
    public Button workButton;

    public TextMeshProUGUI textBox;
    public TextMeshProUGUI targetText;

    public Image lifeBar;
    public Image xpBar;

    public TextMeshProUGUI level;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI xpText;

    public Color high;
    public Color medium;
    public Color low;

    void Start()
    {
        map = Dummy.instance.map;
        player = map.player;
        transform.localPosition = new Vector3(player.point.x, player.point.y, 0);
        location = map.GetLocation(new Point(player.point.x, player.point.y));
        timeCounter = 0;

        UpdateLife();
        UpdateExperience();

        inventory = new Inventory();
        ModifyMoney(500);
    }

    void Update()
    {
        if (gamePaused)
        {
            return;
        }
        Controls();
        timeCounter += UnityEngine.Time.deltaTime * gameSpeed;
        while (timeCounter >= 1f)
        {
            timeCounter -= 1f;
            Dummy.instance.Tick(1);
            UpdateExperience();
            UpdateLife();
        }
    }

    private void UpdateLife()
    {
        lifeBar.transform.localScale = new Vector3((float)player.life / player.maxLife, 1f, 1f);
        lifeText.text = player.life + " / " + player.maxLife;
        SetColor(player.life, player.maxLife, lifeBar);
    }
    private void UpdateExperience()
    {
        xpBar.transform.localScale = new Vector3((float)player.experience / player.requiredXp, 1f, 1f);
        xpText.text = player.experience + " / " + player.requiredXp;
        level.text = player.level.ToString();
        //SetColor(player.experience, player.requiredXp, xpText);
    }

    private void SetColor(int value, int maxValue, Image image)
    {
        if (value < maxValue * 0.33f)
        {
            image.color = low;
        }
        else if ( value < maxValue * 0.66f)
        {
            image.color = medium;
        }
        else
        {
            image.color = high;
        }
    }

    private void Controls()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Move(Direction.North);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Direction.South);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Direction.West);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Direction.East);
        }
    }

    private void Move(Point direction)
    {
        targetIndex = 0;
        player.Move(direction);
        location = map.GetLocation(player.point);
        transform.localPosition = new Vector3(player.point.x + .5f, player.point.y + .5f);
        FocusCamera();
        //Dummy.instance.Tick(10);
        if (location.creatures.Count > 0)
        {
            player.selectedCreature = location.creatures[0];
        }
        else
        {
            player.selectedCreature = null;
        }
        Observe();
    }

    private void Observe()
    {
        textBox.text = location.ToString();

        if (location.creatures.Count == 0)
        {
            target = null;
            targetText.text = player.point.ToString();
            talkButton.gameObject.SetActive(false);
        }
        else
        {
            targetIndex = -1;
            NextCreature();
            talkButton.gameObject.SetActive(true);
        }
        nextButton.gameObject.SetActive(location.creatures.Count > 1);
        bool canFight = (location.GetMonster() != null && !location.GetMonster().IsBoss());
        killButton.gameObject.SetActive(canFight);
        exploreButton.gameObject.SetActive(!location.HasMonster() && location.GetObstacle() == null && (location.building != null || location.items.list.Count > 0));
        restButton.gameObject.SetActive(player.life < player.maxLife);
        workButton.gameObject.SetActive(!location.HasMonster());

    }

    public void FocusCamera()
    {
        if (player != null)
        {
            cameraController.CenterTo(new Vector2(player.point.x, player.point.y));
        }
    }

    public void NextCreature()
    {
        targetIndex++;
        if (location.creatures.Count > 0)
        {
            target = location.creatures[targetIndex % location.creatures.Count];
            player.selectedCreature = target;
            targetText.text = player.point.ToString() + "\nTarget: " + target.name;
        }
    }

    public void Talk()
    {
        player.Talk(target);
        dialogue.StartDialogue(target);
    }

    public void Kill()
    {
        Item item = player.Kill();
        if (item != null)
        {
            Add(item);
        }
        Observe();
        UpdateLife();
        UpdateExperience();
    }

    public void Explore()
    {
        player.Explore();
        Observe();
    }

    public void Rest()
    {
        Building b = map.GetLocation(player.point).building;
        if (b != null && b.HasTag(BuildingTag.Bed))
        {
            player.ModifyLife(2);
        }
        player.ModifyLife(1);
        UpdateLife();
        Dummy.instance.Tick(60);
        restButton.gameObject.SetActive(player.life < player.maxLife);
        Observe();

    }

    public void AcceptQuest()
    {
        player.AcceptQuest(target);
    }

    public void GamePaused(bool paused)
    {
        gamePaused = paused;
        Dummy.instance.paused = paused;
    }

    public void Add(ItemStack stack)
    {
        for(int i = 0; i < stack.count; i++)
        {
            Add(stack.item);
        }
    }

    public void Add(Item item)
    {
        player.AddItem(item);
        inventoryManager.Add(item);
    }

    public void Remove(Item item)
    {
        inventoryManager.Remove(item);
        player.RemoveItem(item);
    }

    public void ItemClicked(Item item)
    {
        object obj = map.GetLocation(player.point).UseItem(map, player, item);
        if (obj != null)
        {
            Remove(item);
            if (obj is Item)
            {
                Add(obj as Item);
            }
            else if (obj is Obstacle)
            {

            }
            Observe();
        }
    }

    public void ModifyMoney(int amount)
    {
        inventory.coins += amount;
        coinsText.text = inventory.coins + " c";
    }

    public void Work()
    {
        int tile = map.GetLocation(player.point).tile;
        if (tile == Tile.Forest.id)
        {
            Add(Item.Wood);
        }
        else if (tile == Tile.Mountain.id)
        {
            Add(Item.Stone);
        }
        else if (tile == Tile.Water.id)
        {
            Add(Item.Fish);
        }
        else if (tile == Tile.Grass.id)
        {
            Add(Item.Meat);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public GameObject costObject;

    public Image image;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI costText;

    public bool isClicked;

    public Item item;
    public int count;

    public void SetItem(Item item)
    {
        this.item = item;
        count = 0;
        costText.text = item.cost + " c";
        ModifyCount(1);
        costObject.SetActive(false);
    }

    public void SetItem(ItemStack stack)
    {
        this.item = stack.item;
        count = 0;
        costText.text = item.cost + " c";
        ModifyCount(stack.count);
        costObject.SetActive(false);
    }

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void Add(int amount = 1)
    {
        ModifyCount(amount);
    }

    public void Remove(int amount = 1)
    {
        ModifyCount(-amount);
    }

    public void ModifyCount(int amount)
    {
        count += amount;
        countText.text = count.ToString();
        gameObject.SetActive(count != 0);
    }

    public bool Equals(Item item)
    {
        return item.Equals(this.item);
    }

    public void ShowPrice(bool show)
    {
        costObject.SetActive(show);
    }

    public void Clicked()
    {
        isClicked = true;
    }
}

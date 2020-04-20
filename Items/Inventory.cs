using System.Collections.Generic;

public class Inventory 
{
    public int coins;

    private ItemStackList items;

    public Inventory()
    {
        items = new ItemStackList();
    }

    public int Count(Item item)
    {
        return items.Count(item);
    }


    public int Count(ItemType type)
    {
        return items.Count(type);
    }

    public void Add(ItemStack stack)
    {
        items.Add(stack);
    }

    public void Add(Item item, int count = 1)
    {
        Add(new ItemStack(item, count));
    }

    public ItemStack Remove(ItemStack stack)
    {
        return items.Remove(stack);
    }

    public ItemStack Remove(Item item, int count = 1)
    {
        return Remove(new ItemStack(item, count));
    }

    public bool Contains(ItemStack stack)
    {
        return items.Count(stack.item) >= stack.count;
    }

    public bool Contains(Item item)
    {
        return items.Contains(item);
    }

    public ItemStack[] GetByType(ItemType type)
    {
        List<ItemStack> found = new List<ItemStack>();
        foreach (ItemStack stack in items.list)
        {
            if (stack.item.IsType(type))
            {
                found.Add(stack);
            }
        }
        return found.ToArray();
    }

    public bool Contains(ItemType type, int count = 1)
    {
        return items.Count(type) >= count;
    }

    public List<ItemStack> GetItems()
    {
        return items.list;
    }

    public void RemoveCoins(int amount)
    {
        coins -= amount;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
    }
}

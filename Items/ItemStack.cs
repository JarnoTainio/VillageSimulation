using System.Collections.Generic;

public class ItemStack
{

    public Item item;
    public int count;

    public ItemStack(int id, int count = 1)
    {
        this.item = Item.items[id];
        this.count = count;
    }

    public ItemStack(Item item, int count = 1)
    {
        this.item = item;
        this.count = count;
    }

    public ItemStack(ItemStack stack)
    {
        this.item = stack.item;
        this.count = stack.count;
    }

    public void Add(ItemStack stack)
    {
        if (stack.item.name == item.name)
        {
            this.count += stack.count;
        }
    }

    public bool IsType(ItemType type)
    {
        return item.IsType(type);
    }

    public ItemStack Copy()
    {
        return new ItemStack(item, count);
    }

    public int GetValue(float modifier)
    {
        return (int)(item.cost * count * modifier);
    }

    public int GetValue()
    {
        return item.cost * count;
    }

    public bool Equals(Item item)
    {
        return this.item.Equals(item);
    }

    public override string ToString()
    {
        if (count > 1)
        {
            return count + " " + item.name + "s";
        }
        return item.name;
    }

}

public class ItemStackList
{
    public List<ItemStack> list;

    public ItemStackList()
    {
        list = new List<ItemStack>();
    }

    public void Add(ItemStack stack)
    {
        foreach(ItemStack s in list)
        {
            if (s.item.Equals(stack.item))
            {
                s.count += stack.count;
                return;
            }
        }
        list.Add(new ItemStack(stack.item.id, stack.count));
    }

    public bool Contains(Item item)
    {
        foreach (ItemStack s in list)
        {
            if (s.item.Equals(item))
            {
                return true;
            }
        }
        return false;
    }

    public bool Contains(ItemType type)
    {
        foreach (ItemStack s in list)
        {
            if (s.item.IsType(type))
            {
                return true;
            }
        }
        return false;
    }

    public List<ItemStack> Get(ItemType type)
    {
        List<ItemStack> found = new List<ItemStack>();
        foreach (ItemStack s in list)
        {
            if (s.item.IsType(type))
            {
                found.Add(s);
            }
        }
        return found;
    }

    public bool Contains(int itemId)
    {
        return Contains(Item.items[itemId]);
    }

    public int Count(Item item)
    {
        foreach (ItemStack s in list)
        {
            if (s.item.Equals(item))
            {
                return s.count;
            }
        }
        return 0;
    }

    public int Count(ItemType type)
    {
        int count = 0;
        foreach (ItemStack s in list)
        {
            if (s.item.IsType(type))
            {
                count += s.count;
            }
        }
        return count;
    }

    public void Remove(Item item)
    {
        foreach(ItemStack stack in list)
        {
            if (stack.item.Equals(item))
            {
                list.Remove(stack);
                return;
            }
        }
    }

    public ItemStack Remove(ItemStack wanted)
    {
        foreach (ItemStack stack in list)
        {
            if (stack.item.Equals(wanted.item))
            {
                if (wanted.count == -1 || stack.count < wanted.count)
                {
                    list.Remove(stack);
                    return stack;
                }
                else
                {
                    stack.count -= wanted.count;
                    return wanted;
                }
            }
        }
        return null;
    }

    public void Add(ItemStackList otherList)
    {
        foreach(ItemStack s in otherList.list)
        {
            Add(s);
        }
    }

    public void Clear()
    {
        list = new List<ItemStack>();
    }

    public override string ToString()
    {
        string str = "";
        for(int i = 0; i < list.Count; i++)
        {
            if (i == 0)
            {

            }
            else if (i == list.Count - 1)
            {
                str += " and ";
            }
            else
            {
                str += ", ";
            }
            str += list[i];
        }
        return str;
    }

    public string ItemsToString()
    {
        string str = "";
        for (int i = 0; i < list.Count; i++)
        {
            if (i == 0)
            {

            }
            else if (i == list.Count - 1)
            {
                str += " and ";
            }
            else
            {
                str += ", ";
            }
            str += list[i].item;
        }
        return str;
    }
}

public class Memory_Item : Memory
{
    public int ownerID;
    public ItemStack stack;
    public Point point;

    public Memory_Item(int power, ItemStack stack, Point point, int ownerID = -1) : base(power)
    {
        this.ownerID = ownerID;
        this.point = point;
        Update(stack);
    }

    public Memory_Item Update(ItemStack stack)
    {
        UpdateSeen();
        this.stack = new ItemStack(stack);
        return this;
    }

    public override string ToString()
    {
        return stack.ToString() + " at " + point;
    }

    public bool IsOwned(int ID)
    {
        return ownerID == ID;
    }
}

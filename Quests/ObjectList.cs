using System.Collections.Generic;

public class ObjectValue
{
    public readonly object data;
    public int value;

    public ObjectValue(object data, int value)
    {
        this.data = data;
        this.value = value;
    }
}

public class ObjectValueList
{
    public List<ObjectValue> list;

    public ObjectValueList()
    {
        list = new List<ObjectValue>();
    }

    public void Add(object obj, int value)
    {
        ObjectValue ov = new ObjectValue(obj, value);
        list.Add(ov);
        Sort();
    }

    public void Sort()
    {
        list.Sort((x, y) => y.value.CompareTo(x.value));
    }

}
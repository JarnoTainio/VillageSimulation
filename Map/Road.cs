public class Road
{
    public Point point;
    public Point to;

    public int count;
    public bool isVisible;
    Time lastUsed;

    public Road(Point point)
    {
        count = 1;
        lastUsed = TimeManager.Now();
        isVisible = false;
        this.point = point;
    }

    public void Tick()
    {
        if (lastUsed.DaysPassed() > 1)
        {
            count -= 3;
            if (count < 0)
            {
                count = 0;
            }
            if (isVisible)
            {
                UpdateVisible();
            }
        }
    }

    public bool Equals(Point start)
    {
        if (start.Equals(point))
        {
            if (count < 114)
            {
                count++;
                UpdateVisible();
            }
            lastUsed = TimeManager.Now();
            return true;
        }
        return false;
    }

    private void UpdateVisible()
    {
        bool newState = count > 100;
        if (newState != isVisible)
        {
            isVisible = newState;
            Dummy.instance.drawer.DrawRoad(point, isVisible);
        }
    }
}

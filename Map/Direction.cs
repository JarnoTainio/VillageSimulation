public class Direction
{
    public static Point North = new Point(0, 1);
    public static Point South = new Point(0, -1);
    public static Point East = new Point(1, 0);
    public static Point West = new Point(-1, 0);
    public static Point NorthWest = new Point(-1, 1);
    public static Point NorthEast = new Point(1, 1);
    public static Point SouthWest = new Point(-1, -1);
    public static Point SouthEast = new Point(1, -1);
    public static Point[] Directions = new Point[] { North, East, South, West };

    public static string ToString(Point direction)
    {
        if (direction.Equals(North))
        {
            return "north";
        }
        else if (direction.Equals(South))
        {
            return "south";
        }
        else if (direction.Equals(East))
        {
            return "east";
        }
        else if (direction.Equals(West))
        {
            return "west";
        }
        else if (direction.Equals(SouthWest))
        {
            return "south-west";
        }
        else if (direction.Equals(SouthEast))
        {
            return "south-east";
        }
        else if (direction.Equals(NorthWest))
        {
            return "north-west";
        }
        else if (direction.Equals(NorthEast))
        {
            return "north-east";
        }
        return "unknown";
    }
}

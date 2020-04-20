using System;

public struct Point 
{
    public static Point None = new Point(-1, -1);

    public readonly int x, y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Point(Point point)
    {
        this.x = point.x;
        this.y = point.y;
    }

    public Point Add(Point point)
    {
        return Add(point.x, point.y);
    }

    public Point Reduce(Point point)
    {
        return Add(-point.x, -point.y);
    }

    public Point Add(int x, int y)
    {
        return new Point(this.x + x, this.y + y);
    }

    public bool Equals(Point p)
    {
        return p.x == x && p.y == y;
    }

    public bool Equals(int x, int y)
    {
        return this.x == x && this.y == y;
    }

    public int Distance(Point point)
    {
        return Distance(point.x, point.y);
    }

    public int Distance(int x, int y)
    {
        return Math.Abs(this.x - x) + Math.Abs(this.y - y);
    }

    public Point GetDirection(Point point, bool preferX = true)
    {
        int dx = x - point.x;
        int dy = y - point.y;
        if (dx == dy)
        {
            return preferX
                ? (dx > 0 ? Direction.West : Direction.East)
                : (dy > 0 ? Direction.South : Direction.North);
        }
        else if (Math.Abs(dx) > Math.Abs(dy))
        {
            return dx > 0 ? Direction.West : Direction.East;
        }
        else
        {
            return dy > 0 ? Direction.South : Direction.North;
        }
    }

    public Point Reverse() {
        return new Point(-x, -y);
    }

    public string DistanceToString(Point target)
    {
        int d = Distance(target);
        string str = "";
        if (d > 12)
        {
            str += "very far";
        }
        else if (d > 5)
        {
            str += "far";
        }
        str += " " + Direction.ToString(GetDirection(target));
        return str;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }

}

public struct Rectangle
{
    public readonly int xStart, xEnd, yStart, yEnd;

    public Rectangle(Point p1, Point p2)
    {
        xStart = Math.Min(p1.x, p2.x);
        xEnd = Math.Max(p1.x, p2.x);
        yStart = Math.Min(p1.y, p2.y);
        yEnd = Math.Max(p1.y, p2.y);
    }

    public bool IsWithin(Point p)
    {
        return p.x <= xEnd && p.x >= xStart && p.y <= yEnd && p.y >= yStart; 
    }
}

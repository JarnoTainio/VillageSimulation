public struct Time
{
    public static Time Zero = new Time(0, 0, 0, 0, 0);

    public static int MONTHS_IN_YEAR = 12;
    public static int DAYS_IN_MONTH = 28;
    public static int HOURS_IN_DAY = 24;

    public static int Hour = 60;
    public static int Day = Hour * HOURS_IN_DAY;
    public static int Month = Day * DAYS_IN_MONTH;
    public static int Year = Month * MONTHS_IN_YEAR;

    public int year;
    public int month;
    public int day;
    public int hour;
    public int minutes;

    public int time;

    public Time(int y, int m, int d, int h, int min)
    {
        year = y;
        month = m;
        day = d;
        hour = h;
        minutes = min;
        time = year * MONTHS_IN_YEAR * DAYS_IN_MONTH * HOURS_IN_DAY * 60;
        time += month * DAYS_IN_MONTH * HOURS_IN_DAY * 60;
        time += day * HOURS_IN_DAY * 60;
        time += hour * 60;
        time += minutes;
    }

    public Time(string str)
    {
        string[] parts = str.Split();
        year = int.Parse(parts[0]);
        month = int.Parse(parts[1]);
        day = int.Parse(parts[2]);
        hour = int.Parse(parts[3]);
        minutes = int.Parse(parts[4]);
        time = year * MONTHS_IN_YEAR * DAYS_IN_MONTH * HOURS_IN_DAY * 60;
        time += month * DAYS_IN_MONTH * HOURS_IN_DAY * 60;
        time += day * HOURS_IN_DAY * 60;
        time += hour * 60;
        time += minutes;
    }

    public int Difference(Time t)
    {
        return t.time - time;
    }

    private void UpdateTime()
    {
        time = year * MONTHS_IN_YEAR * DAYS_IN_MONTH * HOURS_IN_DAY * 60;
        time += month * DAYS_IN_MONTH * HOURS_IN_DAY * 60;
        time += day * HOURS_IN_DAY * 60;
        time += hour * 60;
        time += minutes;
    }

    public int DaysPassed()
    {
        return DaysPassed(TimeManager.Now());
    }

    public int DaysPassed(Time time)
    {
        return time.ToDayInt() - ToDayInt();
    }

    public int MinutesPassed()
    {
        return MinutesPassed(TimeManager.Now());
    }

    public int MinutesPassed(Time time)
    {
        return time.time - this.time;
    }

    public int ToDayInt()
    {
        return ((year * MONTHS_IN_YEAR + month) * DAYS_IN_MONTH + day);
    }

    public override string ToString()
    {
        return "Year: " + year + " Month: " + month + " Day: " + day + " Hour: " + hour;
    }

    public string ToShortString()
    {
        return year + "." + month + "." + day + "." + hour + ":" + minutes;
    }

    public string ToDateString()
    {
        return "Year: " + year + " Month: " + month + " Day: " + day;
    }
}

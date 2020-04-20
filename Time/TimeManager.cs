using UnityEngine;

public class TimeManager
{
    public static int minutes;
    public static int hour;
    public static int day;
    public static int month;
    public static int year;

    public static void Reset()
    {
        year = 0;
        month = 1;
        day = 1;
        hour = 7;
        minutes = 0;
    }

    public static bool Tick(int delta)
    {
        if (delta > 60)
        {
            Dummy.PrintMessage("TIME ERROR!");
        }
        minutes += delta;
        while (minutes >= 60)
        {
            minutes -= 60;

            hour++;
            while (hour > 24)
            {
                hour = 1;
                day++;

                while (day > 28)
                {
                    day = 1;
                    month++;
                    while (month > 12)
                    {
                        month = 1;
                        year++;
                    }
                }
                return true;
            }
        }
        return false;
    }

    public static Time Now(bool onlyDay = false)
    {
        return new Time(year, month, day, onlyDay ? 0 : hour, onlyDay ? 0 : minutes);
    }

    public static string GetCompactTime()
    {
        string str = "";
        str += "[";
        str += (year < 10 ? "000" : (year < 100 ? "00" : (year < 1000 ? "0" : ""))) + year + ":";
        str += (month < 10 ? "0":"") + month + ":";
        str += (day < 10 ? "0" : "") + day + ":";
        str += (hour < 10 ? "0" : "") + hour + ":";
        str += (minutes < 10 ? "0" : "") + minutes;
        str += "]";
        return str;
    }
}

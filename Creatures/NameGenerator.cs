using System;

public class NameGenerator
{
    static string[] parts = new string[] {
        "a", "e", "i", "o", "u",
        "ma", "ri", "an", "ne", "ron", "ris", "lek", "si", "jar", "no", "tai", "ni", "o", "vai", "nio", "hei", "ko", "nu", "pu", "pla", "ty", "ket", "tu"
    };

    public static string New(Random random)
    {
        string str = "";
        int count = random.Next(2) + random.Next(2) + random.Next(1) + 1;
        while (count > 0)
        {
            count--;
            str += parts[random.Next(parts.Length)];
        }
        return str;
    }
}

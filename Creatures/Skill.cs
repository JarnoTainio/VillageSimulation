public class Skill
{
    public static int ID;
    public static Skill[] skills = new Skill[1];

    public static Skill farming =   new Skill("farming");
    public static Skill fishing =   new Skill("fishing");
    public static Skill hunting =   new Skill("hunting");
    public static Skill building =  new Skill("building");
    public static Skill woodcutting=new Skill("woodcutting");
    public static Skill mining =    new Skill("mining");
    public static Skill baking =    new Skill("baking");
    public static Skill brewing =   new Skill("brewing");
    public static Skill metalwork = new Skill("metalwork");
    public static Skill animals = new Skill("animals");
    public static Skill combat = new Skill("combat");

    public int id;
    public string name;

    public Skill(string name)
    {
        id = ID++;
        if (skills.Length == id)
        {
            Skill[] newSkills = new Skill[ID];
            for (int i = 0; i < skills.Length; i++)
            {
                newSkills[i] = skills[i];
            }
            skills = newSkills;
        }
        skills[id] = this;
        this.name = name;
    }

    public bool Equals(Skill skill)
    {
        return id == skill.id;
    }
}

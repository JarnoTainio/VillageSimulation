
public class Role
{
    public Creature creature;
    public StoryRole storyRole;

    public Role(Creature creature, StoryRole storyRole)
    {
        this.creature = creature;
        this.storyRole = storyRole;
    }
}

public enum StoryRole { none, finder, questGiver, victim }
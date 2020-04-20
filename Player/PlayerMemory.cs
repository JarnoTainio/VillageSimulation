public class PlayerMemory 
{
    public readonly PlayerAction action;
    public readonly Time time;
    
    public PlayerMemory(PlayerAction action)
    {
        this.action = action;
        time = TimeManager.Now();
    }
}

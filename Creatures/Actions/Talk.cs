public class Talk : Action
{
    private Memory_Agent memory;
    private SocialMessage message;
    private bool saidHello;
    private bool actionResolved;

    public Talk(Plan source, int strength, Memory_Agent memory, SocialMessage message) : base(source, "talking to " + memory.name, strength, MindState.Social, 5)
    {
        this.memory = memory;
        this.message = message;
        actionResolved = false;
        saidHello = false;
    }

    public override int Tick(Map map, Creature creature, int delta)
    {
        SetMind(creature);

        // Find target
        Creature target = null;
        foreach(Creature c in map.GetLocation(creature.location).creatures)
        {
            if (memory.ID == c.ID)
            {
                target = c;
                break;
            }
        }

        duration -= delta;
        strength -= delta;
        if (duration <= 0)
        {
            strength -= delta;
            // ToDo: React to talk duration
        }

        if (target != null)
        {
            // Set target as talking partner
            creature.social.TalkTo(target);

            // Send social message and receive response
            SocialMessage currentMessage;
            if (!saidHello)
            {
                //creature.PrintMessage("Starting conversation, because " + source);
                currentMessage = new SocialMessage(SocialAction.hello);
                saidHello = true;
            }
            else if (actionResolved)
            {
                //currentMessage = creature.social.GetAction(target.ID);
                currentMessage = new SocialMessage(SocialAction.bye);
            }
            else
            {
                currentMessage = message;
                if (message.action == SocialAction.buy)
                {
                    //creature.PrintMessage("Can I buy "+message.target, true);
                }
                if (message.action == SocialAction.sell)
                {
                    //creature.PrintMessage("I would like to sell "+message.target, true);
                }
            }
            SocialResponse response = target.social.Talk(creature, currentMessage);

            // No response
            if (response == SocialResponse.nothing)
            {
                creature.social.EndTalk();
                Failed(map, creature, new Event(creature.ID, creature.location, EventAction.talking, Failure.unknown, currentMessage, target.ID, response));
            }

            // Request declined
            else if (response == SocialResponse.decline)
            {
                creature.social.EndTalk();
                Failed(map, creature, new Event(creature.ID, creature.location, EventAction.talking, Failure.blocked, currentMessage, target.ID, response));
            }

            // Request accepted
            else if (response == SocialResponse.agree)
            {
                if (currentMessage.action == SocialAction.hello)
                {
                    memory.Hello(target);
                    saidHello = true;
                }
                if (currentMessage.action == message.action)
                {
                    memory.Relationship(1);
                    creature.goalManager.TestGoal(source.source);
                    actionResolved = true;
                }

                if (currentMessage.action == SocialAction.bye)
                {
                    creature.social.EndTalk();
                    Done(map, creature, new Event(creature.ID, creature.location, EventAction.talking, EventResult.success, currentMessage, target.ID, response));
                }
                else
                {
                    Ongoing(map, creature, new Event(creature.ID, creature.location, EventAction.talking, EventResult.started, currentMessage, target.ID, response));
                }
            }
            return -delta;
        }
        else
        {
            creature.social.EndTalk();
            Failed(map, creature, new Event(creature.ID, creature.location, EventAction.talking, Failure.notFound, null, memory.ID));
            return -delta;
        }

    }
}

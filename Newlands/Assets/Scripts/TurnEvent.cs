// An object that's used to represent what the GameManger did on a turn.

public class TurnEvent
{
    public int phase;
    public int playerId;
    public string operation;
    public string cardType;
    public int x;
    public int y;
    public string topCard;

    public TurnEvent(int phase, int playerId, string operation, string cardType, int x, int y, string topCard)
    {
        this.phase = phase;
        this.playerId = playerId;
        this.operation = operation;
        this.cardType = cardType;
        this.x = x;
        this.y = y;
        this.topCard = topCard;
    }

    public override string ToString()
    {
        return ("Phase: " + this.phase
            + ", Operation: " + this.operation
            + ", CardType: " + this.cardType
            + " [" + this.x
            + ", " + this.y + "]");
    }
}

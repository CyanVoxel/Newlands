// An object that's used to represent what the GameManger did on a turn.
// TODO: Better names? Builder class? So much room for improvement, but it works for now.

using UnityEngine;

public class TurnEvent
{
    [SerializeField]
    public int phase;
    [SerializeField]
    public int playerId;
    [SerializeField]
    public string operation;
    [SerializeField]
    public string cardType;
    [SerializeField]
    public int x;
    [SerializeField]
    public int y;
    [SerializeField]
    public string topCard;
    [SerializeField]
    public string card;
    [SerializeField]
    public int targetX;
    [SerializeField]
    public int targetY;
    [SerializeField]
    public string playedCard;

    // public TurnEvent(int phase, int playerId, string operation,
    //     string cardType, int x, int y,
    //     string topCard)
    // {
    //     this.phase = phase;
    //     this.playerId = playerId;
    //     this.operation = operation;
    //     this.cardType = cardType;
    //     this.x = x;
    //     this.y = y;
    //     this.topCard = topCard;
    // }

    // Constructor including Card field
    public TurnEvent(int phase, int playerId, string operation,
        string cardType, int x, int y,
        string topCard, string card)
    {
        this.phase = phase;
        this.playerId = playerId;
        this.operation = operation;
        this.cardType = cardType;
        this.x = x;
        this.y = y;
        this.topCard = topCard;
        this.card = card;

    }

    // Constructor including Card field
    public TurnEvent(int phase, int playerId, string operation,
        string cardType, int x, int y,
        string topCard, string card, int targetX, int targetY, string playedCard)
    {
        this.phase = phase;
        this.playerId = playerId;
        this.operation = operation;
        this.cardType = cardType;
        this.x = x;
        this.y = y;
        this.topCard = topCard;
        this.card = card;
        this.targetX = targetX;
        this.targetY = targetY;
        this.playedCard = playedCard;
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

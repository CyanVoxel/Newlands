// Model data used by MatchSetupController

using UnityEngine;

public class MatchConfig
{
	// FIELDS ##########################################################################################################
	[SerializeField]
    private string deckFlavor = "Vanilla";
    [SerializeField]
	private string winCondition;
    [SerializeField]
	private int gameGridHeight;
    [SerializeField]
	private int gameGridWidth;
    [SerializeField]
	private int maxPlayerCount;
    [SerializeField]
	private int playerHandSize;
	[SerializeField]
	private int graceRounds;
    [SerializeField]
	private bool initialized = false;

	// PROPERTIES ######################################################################################################
	public string DeckFlavor { get { return deckFlavor; } }
	public string WinCondition { get { return winCondition; } }
	public int GameGridHeight { get { return gameGridHeight; } }
	public int GameGridWidth { get { return gameGridWidth; } }
	public int MaxPlayerCount { get { return maxPlayerCount; } }
	public int PlayerHandSize { get { return playerHandSize; } }
	public int GraceRounds { get { return graceRounds; } }
	public bool Initialized { get { return initialized; } }

	public MatchConfig(string deckFlavor, string winCondition,
		int gameGridHeight, int gameGridWidth,
		int maxPlayerCount, int playerHandSize,
		int graceRounds)
	{
		this.deckFlavor = deckFlavor;
		this.winCondition = winCondition;
		this.gameGridHeight = gameGridHeight;
		this.gameGridWidth = gameGridWidth;
		this.maxPlayerCount = maxPlayerCount;
		this.playerHandSize = playerHandSize;
		this.graceRounds = graceRounds;
		this.initialized = true;
	}

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }

    // public MatchConfigData(MatchConfigData config)
    // {
    //     this.deckFlavor = config.DeckFlavor;
	// 	this.winCondition = config.WinCondition;
	// 	this.gameGridHeight = config.GameGridHeight;
	// 	this.gameGridWidth = config.GameGridWidth;
	// 	this.maxPlayerCount = config.MaxPlayerCount;
	// 	this.playerHandSize = config.PlayerHandSize;
	// 	this.initialized = true;
    // }
}

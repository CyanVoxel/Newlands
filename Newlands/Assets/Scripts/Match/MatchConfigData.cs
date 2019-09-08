// Model data used by MatchSetupController

public class MatchConfigData
{
	// FIELDS ##########################################################################################################
	private string deckFlavor = "Vanilla";
	private string winCondition;
	private int gameGridHeight;
	private int gameGridWidth;
	private int maxPlayerCount;
	private int playerHandSize;
	private bool initialized = false;

	// PROPERTIES ######################################################################################################
	public string DeckFlavor { get { return deckFlavor; } }
	public string WinCondition { get { return winCondition; } }
	public int GameGridHeight { get { return gameGridHeight; } }
	public int GameGridWidth { get { return gameGridWidth; } }
	public int MaxPlayerCount { get { return maxPlayerCount; } }
	public int PlayerHandSize { get { return playerHandSize; } }
	public bool Initialized { get { return initialized; } }

	public MatchConfigData(string deckFlavor, string winCondition,
		int gameGridHeight, int gameGridWidth,
		int maxPlayerCount, int playerHandSize)
	{
		this.deckFlavor = deckFlavor;
		this.winCondition = winCondition;
		this.gameGridHeight = gameGridHeight;
		this.gameGridWidth = gameGridWidth;
		this.maxPlayerCount = maxPlayerCount;
		this.playerHandSize = playerHandSize;
		this.initialized = true;
	}
}

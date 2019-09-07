// Model data used by MatchController

using Mirror;
using UnityEngine;

public class MatchData
{
	// FIELDS ##########################################################################################################
	private string deckFlavor = "Vanilla";
	private string winningCondition;
	private int gameGridHeight;
	private int gameGridWidth;
	private int maxPlayerCount;
	private int playerHandSize;
	private bool initialized = false;

	// PROPERTIES ######################################################################################################
	public string DeckFlavor { get { return deckFlavor; } set { deckFlavor = value; } }
	public string WinningCondition { get { return winningCondition; } set { winningCondition = value; } }
	public int GameGridHeight { get { return gameGridHeight; } set { gameGridHeight = value; } }
	public int GameGridWidth { get { return gameGridWidth; } set { gameGridWidth = value; } }
	public int MaxPlayerCount { get { return maxPlayerCount; } set { maxPlayerCount = value; } }
	public int PlayerHandSize { get { return playerHandSize; } set { playerHandSize = value; } }
	public bool Initialized { get { return initialized; } set { initialized = value; } }
}

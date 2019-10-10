// Model data used by MatchController

using UnityEngine;

public class MatchData
{
	// FIELDS ######################################################################################
	// private string deckFlavor = "Vanilla";
	// private string winCondition;
	// private int gameGridHeight;
	// private int gameGridWidth;
	// private int maxPlayerCount;
	// private int playerHandSize;

	[SerializeField]
	private int turn;
	[SerializeField]
	private int round;
	[SerializeField]
	private int phase;

	private bool initialized = false;

	// PROPERTIES ##################################################################################
	public int Turn { get { return turn; } set { turn = value; } }
	public int Round { get { return round; } set { round = value; } }
	public int Phase { get { return phase; } set { phase = value; } }
	// public string DeckFlavor { get { return deckFlavor; } }
	// public string WinCondition { get { return winCondition; } }
	// public int GameGridHeight { get { return gameGridHeight; } }
	// public int GameGridWidth { get { return gameGridWidth; } }
	// public int MaxPlayerCount { get { return maxPlayerCount; } }
	// public int PlayerHandSize { get { return playerHandSize; } }
	// public bool Initialized { get { return initialized; } }

	public MatchData()
	{
		this.turn = 1;
		this.round = 1;
		this.phase = 1;
		// this.deckFlavor = config.DeckFlavor;
		// this.winCondition = config.WinCondition;
		// this.gameGridHeight = config.GameGridHeight;
		// this.gameGridWidth = config.GameGridWidth;
		// this.maxPlayerCount = config.MaxPlayerCount;
		// this.playerHandSize = config.PlayerHandSize;
		// this.initialized = true;
	}

	// public static bool operator ==(MatchData left, MatchData right)
	// {
	// 	if (left.turn == right.turn
	// 		&& left.round == right.round
	// 		&& left.phase == right.phase)
	// 	{
	// 		return true;
	// 	}
	// 	else
	// 	{
	// 		return false;
	// 	}
	// }

	// public static bool operator !=(MatchData left, MatchData right)
	// {
	// 	if (left.turn != right.Turn
	// 		|| left.round != right.Round
	// 		|| left.phase != right.Phase)
	// 	{
	// 		return true;
	// 	}
	// 	else
	// 	{
	// 		return false;
	// 	}
	// }

	// public override bool Equals(object obj)
	// {
	// 	if (obj == null || !this.GetType().Equals(obj.GetType()))
	// 	{
	// 		return false;
	// 	}
	// 	else
	// 	{
	// 		MatchData md = (MatchData)obj;
	// 		return (turn == md.Turn
	// 			&& round == md.Round
	// 			&& phase == md.Phase);
	// 	}
	// }

	// public override int GetHashCode()
	// {
	// 	return round ^ turn;
	// }

	public override string ToString()
	{
		return JsonUtility.ToJson(this);
	}
}

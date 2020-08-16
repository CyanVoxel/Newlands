// The replacement for GameManager

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MatchController : NetworkBehaviour
{
	// FIELDS ######################################################################################
	[SerializeField]
	private MatchDataBroadcaster matchDataBroadcaster;
	private GridController gridController;
	private MatchData matchData;
	// public MatchData MatchData { get { return matchData; } }
	private MatchConfig config;
	public MatchConfig Config { get { return config; } }
	private NewlandsNetworkManager matchConnections;

	private static MasterDeck masterDeck;
	private static MasterDeck masterDeckMutable;

	private List<PlayerData> players = new List<PlayerData>();
	public List<PlayerData> Players { get { return players; } }
	private List<string> usernameCache = new List<string>();

	[SyncVar]
	private int playerIndex = 1; // This value increments when a new player joins
	public int PlayerIndex { get { return playerIndex; } set { playerIndex = value; } }

	public static MasterDeck MasterDeck { get { return masterDeck; } }
	public static MasterDeck MasterDeckMutable { get { return masterDeckMutable; } }

	public GameObject landTilePrefab;
	public GameObject gameCardPrefab;
	public GameObject marketCardPrefab;

	private static int cardsPlayed = 0;
	public static int CardsPlayed { get { return cardsPlayed; } }
	private bool winnerChosenFlag = false;

	private static DebugTag debugTag = new DebugTag("MatchController", "9C27B0");

	// METHODS #####################################################################################

	void Awake()
	{
		Debug.Log(debugTag + "The MatchController has been created!");
		DontDestroyOnLoad(this.gameObject);
		// this.gameObject.AddComponent<GridController>();
	}

	void Start()
	{
		Debug.Log(debugTag + "Initializing...");

		// [Client/Server]
		if (!hasAuthority)
		{
			this.config = JsonUtility.FromJson<MatchConfig>(matchDataBroadcaster.MatchConfigStr);
			Debug.Log(debugTag + "Grabbed config for client: " + matchDataBroadcaster.MatchConfigStr);

			Debug.Log(debugTag + "Creating Decks...");
			masterDeck = new MasterDeck(config.DeckFlavor);
			masterDeckMutable = new MasterDeck(config.DeckFlavor);

			return;
		}

		// [Server]
		InitializeMatch();
	}

	void Update()
	{
		if (!hasAuthority)
			return;

		if (masterDeck.gameCardDeck.Count == cardsPlayed && !winnerChosenFlag)
		{
			matchDataBroadcaster.BroadcastWinner(FindClientsideLeader(matchDataBroadcaster.PlayerMoneyStr));
			if (FindClientsideLeader(matchDataBroadcaster.PlayerMoneyStr) >= 0)
				winnerChosenFlag = true;
		}
	}

	// public int FindServersideLeader()
	// {
	// 	int winnerId = -1;
	// 	double winnerMoney = -1;

	// 	for (int i = 0; i < config.MaxPlayerCount; i++)
	// 	{
	// 		if (Players[i].Money > winnerMoney)
	// 			winnerId = Players[i].Id;
	// 	}

	// 	return winnerId;
	// }

	public void AssignUsername(string name)
	{
		Debug.Log(debugTag + "I've been told to assign a username! " + name);

		usernameCache.Add(name);

		UpdateUsernames();

		// players[playerIndex - 1].Username = name;
		// Debug.Log(debugTag + "Assigned Player " + Players[playerIndex - 1].Id + " username of " + name);

		// for (int i = 0; i < Players.Count; i++)
		// {
		// 	if (Players[i].Username == "")
		// 	{
		// 		Players[i].Username = name;
		// 		Debug.Log(debugTag + "Assigned Player " + Players[i].Id + " username of " + name);
		// 		i = players.Count;
		// 	}
		// }
	}

	public void UpdateUsernames()
	{
		for (int i = 0; i < usernameCache.Count; i++)
		{
			Players[i].Username = usernameCache[i];
			Debug.Log(debugTag + "I updated Player " + Players[i].Id + " to the name " + usernameCache[i]);
		}

		matchDataBroadcaster.UsernameListStr = MatchDataBroadcaster.PackData(usernameCache);
	}

	public static int FindClientsideLeader(string playerMoneyStr)
	{
		int winnerId = -1;
		double winnerMoney = -1;

		List<int> playerMoneyList = UnpackPlayerMoneyStr(playerMoneyStr);

		for (int i = 0; i < playerMoneyList.Count; i++)
		{
			if (playerMoneyList[i] > winnerMoney)
			{
				winnerId = (i + 1);
				winnerMoney = playerMoneyList[i];
			}

			Debug.Log(debugTag + "[FindLeader] Player " + (i + 1) + " has " + playerMoneyList[i]);
		}

		return winnerId;
	}

	public static List<int> UnpackPlayerMoneyStr(string playerMoneyStr)
	{
		List<int> playerMoneyAmounts = new List<int>();
		string[] playersMoneyStrSplit = playerMoneyStr.Split('_');
		// Debug.Log(debugTag + "Player Money String: " + this.lastKnownPlayerMoneyStr);

		for (int i = 0; i < playersMoneyStrSplit.Length; i++)
		{
			playerMoneyAmounts.Insert(i, (int)(double.Parse(playersMoneyStrSplit[i])));
		}

		return playerMoneyAmounts;
	}

	// void Update()
	// {
	// 	// Debug.Log(matchDataBroadcaster.MatchConfigDataStr);
	// }

	void OnDisable()
	{
		Debug.Log(debugTag + "The MatchController has been disabled/destroyed!");
	}

	// Returns an ID to a player based on their address
	// TODO: Change the argument passed to a username
	public int GetPlayerId(string address)
	{
		// if (matchConnections == null)
		// 	matchConnections = GameObject.Find("NetworkManager").GetComponent<MatchConnections>();

		int index = playerIndex;
		playerIndex++;

		// matchConnections.PlayerAddresses.TryGetValue(address, out index);

		// if (index >= 0)
		// 	Debug.Log(debugTag + "Giving address " + address + " an ID of " + index);
		// else
		// 	Debug.Log(debugTag + "Address " + address + " wanted an ID but has not been logged!");

		Debug.Log(debugTag + "Ignoring passed arguments until usernames are implemented");
		Debug.Log(debugTag + "Giving player an ID of " + index);

		return index;
	}

	// [Server] Grabs the MatchDataBroadcaster from this parent GameObject
	// On the client, they will grab the same MatchDataBroadcaster themselves in a different script.
	private void GrabMatchDataBroadCaster()
	{
		matchDataBroadcaster = this.gameObject.GetComponent<MatchDataBroadcaster>();
		if (matchDataBroadcaster != null)
		{
			Debug.Log(debugTag + "MatchDataBroadcaster was found!");
		}
		else
		{
			Debug.LogError(debugTag.error + "MatchDataBroadcaster was NOT found!");
		}
	}

	private void GrabGridController()
	{
		gridController = this.gameObject.GetComponent<GridController>();
		if (gridController != null)
		{
			Debug.Log(debugTag + "GridController was found!");
		}
		else
		{
			Debug.LogError(debugTag.error + "GridController was NOT found!");
		}
	}

	// [Server] Loads the config from MatchSetupController and initializes the match.
	private void InitializeMatch()
	{
		GrabMatchDataBroadCaster();
		GrabGridController();

		// TODO: Wrap this in a method like data broadcaster
		this.matchConnections = GameObject.Find("NetworkManager").GetComponent<NewlandsNetworkManager>();

		GameObject matchSetupManager = GameObject.Find("LobbyManager");
		if (matchSetupManager != null)
		{
			Debug.Log(debugTag + "LobbyManager was found!");

			LobbyController lobbyController = matchSetupManager.GetComponent<LobbyController>();
			StartCoroutine(LoadMatchConfigCoroutine(lobbyController));
			StartCoroutine(InitializeMatchCoroutine());
		}
		else
		{
			Debug.LogError(debugTag.error + "SetupManager (our dad) was NOT found!");
		}
	}

	// Initializes each player object and draws a hand for them
	private void InitPlayers()
	{
		for (int i = 0; i < config.MaxPlayerCount; i++)
		{
			players.Add(new PlayerData());
			players[i].Id = (i + 1);
			players[i].hand = DrawHand(config.PlayerHandSize);
		} // for playerCount
		UpdatePlayerMoneyStr(true);

		UpdateUsernames();
	}

	public string GetHandCard(int id, int index)
	{
		return JsonUtility.ToJson(players[id - 1].hand[index]);
	}

	private void UpdatePlayerMoneyStr(bool setInitialValues = false)
	{
		matchDataBroadcaster.PlayerMoneyStr = "";

		if (!setInitialValues)
			gridController.UpdatePlayerMoneyValues();

		for (int i = 0; i < config.MaxPlayerCount; i++)
		{
			matchDataBroadcaster.PlayerMoneyStr += players[i].Money;

			if (config.MaxPlayerCount - i > 1)
				matchDataBroadcaster.PlayerMoneyStr += "_";

		} // for playerCount
		Debug.Log(debugTag + "Player Money String: " + matchDataBroadcaster.PlayerMoneyStr);
	}

	// Draws random GameCards from the masterDeck and returns a deck of a specified size
	private Deck DrawHand(int handSize)
	{
		Deck deck = new Deck(); // The deck of drawn cards to return

		for (int i = 0; i < handSize; i++)
		{
			// Draw a card from the deck provided and add it to the deck to return.
			// NOTE: In the future, masterDeckMutable might need to be checked for cards
			// 	before preceding.
			// Card card = Card.CreateInstance<Card>();
			Card card;
			if (DrawCard(masterDeckMutable.gameCardDeck, masterDeck.gameCardDeck, out card))
			{
				deck.Add(card);
			}
			else
			{
				// Destroy(card);
			}
		} // for

		return deck;
	}

	// Draws a card from a deck. Random by default.
	public bool DrawCard(Deck deckMut, Deck deckPerm, out Card card, bool random = true)
	{
		// Card card;	// Card to return
		int cardsLeft = deckMut.Count; // Number of cards left from mutable deck
		int cardsTotal = deckPerm.Count; // Number of cards total from permanent deck

		// Draws a card from the mutable deck, then removes that card from the deck.
		// If all cards are drawn, draw randomly from the immutable deck.
		if (cardsLeft > 0)
		{
			if (random)
				card = deckMut[Random.Range(0, cardsLeft)];
			else
				card = deckMut[deckMut.Count - 1];

			deckMut.Remove(card);
			// Debug.Log("<b>[GameManager]</b> " +
			// 	cardsLeft +
			// 	" of " +
			// 	cardsTotal +
			// 	" cards left");
		}
		else
		{
			// This one HAS to be random anyways
			card = deckPerm[Random.Range(0, cardsTotal)];
			return false;
			// Debug.LogWarning("<b>[GameManager]</b> Warning: " +
			//  "All cards (" + cardsTotal + ") were drawn from a deck! " +
			//  " Now drawing from immutable deck...");
		}

		return true;
	}

	public bool BuyTile(Coordinate2 target)
	{
		int turn = matchData.Turn; // Don't want the turn changing while this is running
		int phase = matchData.Phase;
		bool purchaseSuccess = false;

		// Debug.Log(debug + "Player " + turn
		// 	+ " (ID: " + GameManager.players[turn - 1].Id
		// 	+ ") trying to buy tile " + target.ToString());

		// Check against the rest of the purchasing rules before proceding
		if (IsValidPurchase(target, turn))
		{
			players[turn - 1].ownedTiles.Add(target); // Server-side
			gridController.SetTileOwner(target.x, target.y, turn);

			TurnEvent turnEvent = new TurnEvent(phase, turn, "Buy", "Tile", target.x, target.y,
				matchDataBroadcaster.TopCardStr,
				JsonUtility.ToJson(new Card(gridController.GetServerTile("Tile", target.x, target.y))));
			matchDataBroadcaster.TurnEventStr = JsonUtility.ToJson(turnEvent);
			// Debug.Log(debug + "JSON: " + turnEvent);

			// Debug.Log(debug + "Player " + turn
			// 	+ " (ID: " + GameManager.players[turn - 1].Id
			// 	+ ") bought tile " + target.ToString());

			UpdatePlayersInfo();
			IncrementTurn();
			purchaseSuccess = true;
		}
		else
		{
			// Debug.Log(debug + "Can't purchase, tile " + target.ToString()
			// 	+ " is not valid for you! \nAlready Owned?\nOut of Range?\nBankrupt Tile?");
		}

		return purchaseSuccess;
	}

	// Overload of BuyTile(), taking in two ints instead of a Coordinate2.
	public bool BuyTile(int x, int y)
	{
		return BuyTile(new Coordinate2(x, y));
	}

	private bool IsValidPurchase(Coordinate2 tileBeingPurchased, int playerId)
	{
		int round = matchData.Round;
		bool isValid = false;
		List<Coordinate2> validCards = new List<Coordinate2>();

		if (round > config.GraceRounds)
		{
			// Find each unowned neighbor tiles for this player
			validCards = GetValidCards(playerId);
		}
		else
		{
			Debug.Log(debugTag + "Tile being purchased: " + tileBeingPurchased);
			// This is where the tile is checked against purchasing rules
			if (!gridController.IsTileOwned(tileBeingPurchased.x, tileBeingPurchased.y)
				&& !gridController.IsTileBankrupt(tileBeingPurchased.x, tileBeingPurchased.y))
				isValid = true;
		}

		if (validCards.Contains(tileBeingPurchased))
		{
			isValid = true;
		}

		return isValid;
	}

	// Overload of IsValidPurchase(), taking in two ints instead of a Coordinate2.
	private bool IsValidPurchase(int x, int y, int playerId)
	{
		return IsValidPurchase(new Coordinate2(x, y), playerId);
	}

	private List<Coordinate2> GetValidCards(int playerId)
	{
		List<Coordinate2> validCards = new List<Coordinate2>();

		for (int k = 0; k < players[playerId - 1].ownedTiles.Count; k++)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					// Set x and y equal to the coordinate of the owned tile being checked
					int x = players[playerId - 1].ownedTiles[k].x;
					int y = players[playerId - 1].ownedTiles[k].y;

					// Is the neighbor tile within the grid bounds?
					if (x + i >= 0
						&& x + i < config.GameGridWidth
						&& y + j >= 0
						&& y + j < config.GameGridHeight)
					{
						// This is where the tile is checked against purchasing rules
						if (!gridController.IsTileOwned(x + i, y + j)
							&& !gridController.IsTileBankrupt(x + i, y + j)
							&& !validCards.Contains(new Coordinate2((x + i), (y + j))))
						{
							Debug.Log(debugTag + "Found Valid Neighbor [" + (x + i) + ", " + (y + j) + "] for Player " + playerId);
							validCards.Add(new Coordinate2((x + i), (y + j)));
						}
					}
				}
			}
		}
		return validCards;
	}

	// Advance to the next turn
	public void IncrementTurn()
	{
		int turn = matchData.Turn;
		int round = matchData.Round;
		int phase = matchData.Phase;

		SkipChecker();

		if (GetValidNextTurn(turn) <= config.MaxPlayerCount)
		{
			// The next player can go with no issues.
			// Debug.Log(debug + "ValidNextTurn = " + GetValidNextTurn(turn));
			turn = GetValidNextTurn(turn);
			// this.round++;
		}
		else
		{
			if (GetValidNextTurn(0) <= config.MaxPlayerCount)
			{
				// The next player is not available, but some still are.
				// Debug.Log(debug + "ValidNextTurn = " + GetValidNextTurn(1));
				turn = GetValidNextTurn(0);
				round++;
			}
			else
			{
				// Nobody else can go, signaling the end of the phase.
				// Debug.Log(debug + "ValidNextTurn = " + GetValidNextTurn(turn));
				turn = 1;
				// this.round = 1;
				phase++;
				round++;
				foreach (PlayerData player in players)
				{
					player.shouldSkip = false;
				}
			}
		}

		// Update internal MatchData values, then push them to the broadcaster.
		matchData.Turn = turn;
		matchData.Round = round;
		matchData.Phase = phase;
		matchDataBroadcaster.MatchDataStr = JsonUtility.ToJson(matchData);

		Debug.Log(debugTag + "#################### Turn " + matchData.Turn
			+ " | Round " + matchData.Round
			+ " | Phase " + matchData.Phase + " ####################");
	}

	private int GetValidNextTurn(int turn)
	{
		// If the turn exceeds the player count, return the value anyway.
		if ((turn + 1) > config.MaxPlayerCount)
		{
			// Debug.Log(debug + "Returning " + (turn + 1));
			return (turn + 1);
		}

		// If the turn should be skipped, try again with the next turn up.
		if (players[turn].shouldSkip)
		{ // This doesn't need -1 because it's checking +1
			// Debug.Log(debug + "Recursive...");
			return GetValidNextTurn(turn + 1);
		}

		// If nothing interresting happens, return the next turn.
		// Debug.Log(debug + "Returning " + (turn + 1) + " (Nothing happened)");
		return (turn + 1);
	}

	// Checks if it's possible for any more cards to be bought by the player and sets a skip flag
	// on the player if necessary.
	private void SkipChecker()
	{
		for (int i = 0; i < config.MaxPlayerCount; i++)
		{
			if (matchData.Round > config.GraceRounds && matchData.Phase == 1)
			{
				if (GetValidCards(i + 1).Count == 0)
				{
					Debug.Log(debugTag + "Setting shouldSkip true for Player " + (i + 1)
						+ " on Phase " + matchData.Phase);
					players[i].shouldSkip = true;
				}
			}
		}
	}

	public void UpdatePlayersInfo()
	{
		// TODO: Fix this to work with new systems
		// gridMan.UpdateResourceValues();
		// Things that need to be updated for all players go here
		// for (int i = 0; i < config.MaxPlayerCount; i++)
		// {
		// 	players[i].CalcTotalMoney();
		// 	// Debug.Log("Player " + (i + 1) + "'s Money: $" + players[i].totalMoney);
		// } // for array length
		UpdatePlayerMoneyStr();
	}

	// Tries to play a Card on a Tile. Returns true is successful.
	// Assumes that the player whose turn it is can be the only one who calls this (for now).
	public bool PlayCard(int cardIndex, string targetTile)
	{
		bool wasPlayed = false;
		int locX = int.Parse(targetTile.Substring(1, 2));
		int locY = int.Parse(targetTile.Substring(5, 2));
		int round = matchData.Round;
		int turn = matchData.Turn;
		string tileType = targetTile.Substring(8);
		// GridUnit target = GridManager.grid[locX, locY];
		CardData target;
		Card card = players[turn - 1].hand[cardIndex];

		Debug.Log(debugTag + "Trying to play Card " + cardIndex + " on " + tileType
			+ " at " + locX + ", " + locY);

		target = gridController.GetServerTile(tileType, locX, locY);

		if (!target.IsBankrupt && RuleSet.IsLegal(target, card, turn))
		{
			// gridController.UpdateMarketCardValues();

			gridController.AddCardToStack(locX, locY, target.Category, card);
			UpdatePlayersInfo();

			// // If the card is meant to be stacked
			// if (!card.DiscardFlag)
			// {
			// 	gridController.IncrementStackSize(locY, target.Target);
			// 	gridController.AddCardToStack(locX, locY, target.Target, card);
			// 	// target.CalcTotalValue(); // This fixes Market Cards not calcing first time
			// 	UpdatePlayersInfo();

			// 	// if (gridController.ShiftRowCheck(locX, locY))
			// 	// {
			// 	// 	GameObject cardObj = (GameObject)Instantiate(gameCardPrefab,
			// 	// 		new Vector3(target.CardObject.transform.position.x,
			// 	// 			target.CardObject.transform.position.y
			// 	// 			- (gridController.shiftUnit * target.CardStack.Count),
			// 	// 			(target.CardObject.transform.position.z)
			// 	// 			+ (gridController.cardThickness * target.CardStack.Count)),
			// 	// 		Quaternion.identity);
			// 	// }
			// }

			Card topCard;
			if (DrawCard(masterDeckMutable.gameCardDeck, masterDeck.gameCardDeck, out topCard))
			{
				matchDataBroadcaster.TopCardStr = JsonUtility.ToJson(topCard);
				players[turn - 1].hand[cardIndex] = topCard;
			}
			else
			{
				matchDataBroadcaster.TopCardStr = "empty";
			}

			Debug.Log(debugTag.head + locX + ", " + locY);

			TurnEvent turnEvent = new TurnEvent(matchData.Phase, turn, "Play",
				"GameCard", turn, cardIndex, matchDataBroadcaster.TopCardStr,
				JsonUtility.ToJson(new Card(gridController.GetServerTile(tileType, locX, locY))),
				locX, locY,
				JsonUtility.ToJson(card));

			matchDataBroadcaster.TurnEventStr = JsonUtility.ToJson(turnEvent);

			this.IncrementTurn();
			wasPlayed = true;

			if (target.IsBankrupt)
				gridController.BankruptTile(locX, locY);

			gridController.UpdateMarketCardValues();
			UpdatePlayersInfo();

			cardsPlayed++;
		}

		Debug.Log(debugTag + "GameCards left: " + masterDeckMutable.gameCardDeck.Count + "/" + masterDeck.gameCardDeck.Count);
		Debug.Log(debugTag + "Cards Played: " + cardsPlayed);

		return wasPlayed;
	}

	// COROUTINES ##################################################################################

	// [Client/Server] The main initialization coroutine for the match.
	// Dependant on LoadMatchConfigCoroutine finishing.
	private IEnumerator InitializeMatchCoroutine()
	{
		// Will not run if LoadMatchConfigCoroutine() has not been run.
		// This gives the functionality of yield return LoadMatchConfigCoroutine()
		// without needing to pass this/it the MatchSetupController.
		while (this.config == null)
			yield return null;

		Debug.Log(debugTag + "Initializing Match from Config data...");

		masterDeck = new MasterDeck(config.DeckFlavor);
		masterDeckMutable = new MasterDeck(config.DeckFlavor);

		// [Server]
		if (hasAuthority)
		{
			InitPlayers();

			this.matchData = new MatchData();

			Debug.Log(debugTag + "Sending current Match Data to Broadcaster... " + this.matchData.ToString());
			matchDataBroadcaster.MatchDataStr = JsonUtility.ToJson(this.matchData);
		}

	}

	// [Server] Loads this MatchManager's config from the MatchSetupController's final config.
	private IEnumerator LoadMatchConfigCoroutine(LobbyController controller)
	{
		Debug.Log(debugTag + "Grabbing config from LobbyController...");

		while (!controller.ConfigCreated)
			yield return null;

		this.config = controller.InitialConfig;
		Debug.Log(debugTag + "Config loaded! Sending to Broadcaster...");
		matchDataBroadcaster.MatchConfigStr = JsonUtility.ToJson(controller.InitialConfig);

		Debug.Log(debugTag + "Destroying LobbyController.");
		Destroy(controller.gameObject);
	}

	public void RefreshDeck(string type)
	{
		switch (type)
		{
			case "LandTile":
				MasterDeck tempDeck = new MasterDeck(config.DeckFlavor);
				masterDeckMutable.landTileDeck = tempDeck.landTileDeck;
				break;
			case "CoastTile":
				// MasterDeck tempDeck = new MasterDeck(config.DeckFlavor);
				// masterDeckMutable.coastTileDeck = masterDeck.coastTileDeck;
				break;
			case "GameCard":
				// MasterDeck tempDeck = new MasterDeck(config.DeckFlavor);
				masterDeckMutable.gameCardDeck = masterDeck.gameCardDeck;
				break;
			default:
				break;
		}
	}

}

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
	private MatchConfigData config;
	public MatchConfigData Config { get { return config; } }
	private MatchConnections matchConnections;

	private MasterDeck masterDeck;
	private MasterDeck masterDeckMutable;

	private List<PlayerData> players = new List<PlayerData>();
	public List<PlayerData> Players { get { return players; } }

	[SyncVar]
	private int playerIndex = 1; // This value increments when a new player joins
	public int PlayerIndex { get { return playerIndex; } set { playerIndex = value; } }

	public MasterDeck MasterDeck { get { return masterDeck; } }
	public MasterDeck MasterDeckMutable { get { return masterDeckMutable; } }

	public GameObject landTilePrefab;
	public GameObject gameCardPrefab;
	public GameObject marketCardPrefab;

	private DebugTag debugTag = new DebugTag("MatchController", "9C27B0");

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
			this.config = JsonUtility.FromJson<MatchConfigData>(matchDataBroadcaster.MatchConfigStr);
			Debug.Log(debugTag + "Grabbed config for client: " + matchDataBroadcaster.MatchConfigStr);

			Debug.Log(debugTag + "Creating Decks...");
			this.masterDeck = new MasterDeck(config.DeckFlavor);
			this.masterDeckMutable = new MasterDeck(config.DeckFlavor);

			return;
		}

		// [Server]
		InitializeMatch();
	}

	// void Update()
	// {
	// 	// Debug.Log(matchDataBroadcaster.MatchConfigDataStr);
	// }

	void OnDisable()
	{
		Debug.Log(debugTag + "The MatchController has been disbaled/destroyed!");
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
		Debug.Log(debugTag + "Give player an ID of " + index);

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

	// [Server] Loads the config from MatchSetupController and initializes the match.
	private void InitializeMatch()
	{
		GrabMatchDataBroadCaster();

		// TODO: Wrap this in a method like data broadcaster
		this.matchConnections = GameObject.Find("NetworkManager").GetComponent<MatchConnections>();

		GameObject matchSetupManager = GameObject.Find("SetupManager");
		if (matchSetupManager != null)
		{
			Debug.Log(debugTag + "SetupManager (our dad) was found!");

			MatchSetupController setupController = matchSetupManager.GetComponent<MatchSetupController>();
			StartCoroutine(LoadMatchConfigCoroutine(setupController));
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
			players.Add(new PlayerData(this.config));
			players[i].Id = (i + 1);
			players[i].hand = DrawHand(config.PlayerHandSize);
		} // for playerCount
		UpdatePlayerMoneyStr();
	}

	private void UpdatePlayerMoneyStr()
	{
		matchDataBroadcaster.PlayerMoneyStr = "";

		for (int i = 0; i < config.MaxPlayerCount; i++)
		{
			matchDataBroadcaster.PlayerMoneyStr += players[i].totalMoney;

			if (config.MaxPlayerCount - i > 1)
			{
				matchDataBroadcaster.PlayerMoneyStr += "_";
			}
		} // for playerCount
		Debug.Log(debugTag + "Player Money String: " + matchDataBroadcaster.PlayerMoneyStr);
	} // UpdatePlayerMoneyStr()()

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
			{
				card = deckMut[Random.Range(0, cardsLeft)];
			}
			else
			{
				card = deckMut[deckMut.Count - 1];
			}

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

	// Tries to play a Card on a Tile. Returns true is successful.
	// Assumes that the player whose turn it is can be the only one who calls this (for now).
	public bool PlayCard(int cardIndex, string targetTile)
	{
		bool wasPlayed = false;
		int locX = int.Parse(targetTile.Substring(1, 2));
		int locY = int.Parse(targetTile.Substring(5, 2));
		int round = this.matchData.Round;
		int turn = this.matchData.Turn;
		string tileType = targetTile.Substring(8);
		// GridUnit target = GridManager.grid[locX, locY];
		CardData target;
		Card card = players[turn - 1].hand[cardIndex];

		Debug.Log(debugTag + "Trying to play Card " + cardIndex + " on " + tileType
			+ " at " + locX + ", " + locY);

		switch (tileType)
		{
			case "Tile":
				target = gridController.masterGrid[locX, locY];
				if (!target.IsBankrupt
					&& RuleSet.IsLegal(target, card))
				{
					// Carry out the actions of a Successful play
					UpdatePlayersInfo(); // Test to see if there only needs to be one of these at the end
					if (target.IsBankrupt) // Bankrupt check
					{
						BankruptTile(gridController.masterGrid[locX, locY]);
						UpdatePlayersInfo();
						// guiMan.UpdateUI();
						Debug.Log(debugTag + "Tile bankrupt! has value of "
							+ gridController.masterGrid[locX, locY].TotalValue);
					}

					if (!card.DiscardFlag)
					{
						gridController.masterGrid[locX, locY].StackSize++;
						gridController.masterGrid[locX, locY].CardStack.Add(new CardData(card));
						// target.CalcTotalValue(); // This fixes Market Cards not calcing first time
						UpdatePlayersInfo();
						// guiMan.UpdateUI();
						if (card.Title == "Tile Mod")
						{
							if (target.StackSize > gridController.MaxGridStack[target.Y])
							{
								gridController.MaxGridStack[target.Y]++;
								gridController.ShiftRow(target.Category, target.Y, 1);
							} // if stack size exceeds max stack recorded for row

							GameObject cardObj = (GameObject)Instantiate(gameCardPrefab,
								new Vector3(target.CardObject.transform.position.x,
									target.CardObject.transform.position.y
									- (gridController.shiftUnit * target.StackSize),
									(target.CardObject.transform.position.z)
									+ (gridController.cardThickness * target.StackSize)),
								Quaternion.identity);

							NetworkServer.Spawn(cardObj);

							// This is also done of the client via CardState
							// cardObj.transform.SetParent(target.tileObj.transform);

							// CardState cardState = cardObj.GetComponent<CardState>();
							// // Push new values to the CardState to be synced across the network
							// GridManager.FillOutCardState(card, ref cardState);

							// Generate and Push the string of the object's name
							// cardState.objectName = GameManager.CreateCardObjectName("StackedCard", 0,
							// 	target.stackSize - 1);
							// cardState.parent = GameManager.CreateCardObjectName("Tile", locX, locY);

							// Target
							string cardToDestroy = gridController.CreateCardObjectName("GameCard", turn - 1,
								cardIndex);
							// TargetDestroyGameObject(connectionToClient, cardToDestroy);

						}

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
						TurnEvent turnEvent = new TurnEvent(2, turn, "Play",
							"GameCard", turn, cardIndex, matchDataBroadcaster.TopCardStr);
						matchDataBroadcaster.TurnEventStr = JsonUtility.ToJson(turnEvent);

						UpdatePlayersInfo();
						IncrementTurn();
						wasPlayed = true;
					}
				}
				break;

			case "MarketCard":
				target = gridController.masterMarketGrid[locX, locY];
				if (!target.IsBankrupt
					&& RuleSet.IsLegal(target, card))
				{
					// Carry out the actions of a Successful play
					UpdatePlayersInfo(); // Test to see if there only needs to be one of these at the end
					if (target.IsBankrupt) // Bankrupt check
					{
						BankruptTile(gridController.masterMarketGrid[locX, locY]);
						UpdatePlayersInfo();
						// guiMan.UpdateUI();
						Debug.Log(debugTag + "Market Card bankrupt! has value of "
							+gridController.masterMarketGrid[locX, locY].TotalValue);
					}

					if (!card.DiscardFlag)
					{
						gridController.masterMarketGrid[locX, locY].StackSize++;
						gridController.masterMarketGrid[locX, locY].CardStack.Add(new CardData(card));
						// target.CalcTotalValue(); // This fixes Market Cards not calcing first time
						UpdatePlayersInfo();
						// guiMan.UpdateUI();
						if (card.Title == "Market Mod")
						{
							if (target.StackSize > gridController.MaxMarketStack[target.Y])
							{
								gridController.MaxMarketStack[target.Y]++;
								gridController.ShiftRow(target.Category, target.Y, 1);
							} // if stack size exceeds max stack recorded for row

							GameObject cardObj = (GameObject)Instantiate(gameCardPrefab,
								new Vector3(target.CardObject.transform.position.x,
									target.CardObject.transform.position.y
									- (gridController.shiftUnit * target.StackSize),
									(target.CardObject.transform.position.z)
									+ (gridController.cardThickness * target.StackSize)),
								Quaternion.identity);

							NetworkServer.Spawn(cardObj);

							// This is also done of the client via CardState
							// cardObj.transform.SetParent(target.tileObj.transform);

							// CardState cardState = cardObj.GetComponent<CardState>();
							// // Push new values to the CardState to be synced across the network
							// GridManager.FillOutCardState(card, ref cardState);

							// Generate and Push the string of the object's name
							// cardState.objectName = GameManager.CreateCardObjectName("StackedCard", 0,
							// 	target.stackSize - 1);
							// cardState.parent = GameManager.CreateCardObjectName("MarketCard", locX, locY);

							// Target
							string cardToDestroy = gridController.CreateCardObjectName("GameCard", turn - 1,
								cardIndex);
							// TargetDestroyGameObject(connectionToClient, cardToDestroy);

						}

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

						UpdatePriceListStr();

						TurnEvent turnEvent = new TurnEvent(2, turn, "Play",
							"GameCard", turn, cardIndex, matchDataBroadcaster.TopCardStr);
						matchDataBroadcaster.TurnEventStr = JsonUtility.ToJson(turnEvent);

						UpdatePlayersInfo();
						this.IncrementTurn();
						wasPlayed = true;
					}
				}
				break;

			default:
				Debug.LogError(debugTag.error + "Couldn't find Tile of type \"" + tileType + "\"");
				break;
		} // switch (tileType)

		Debug.Log(debugTag + "GameCards left: " + masterDeckMutable.gameCardDeck.Count);

		return wasPlayed;
	} // PlayCard()

	public bool BuyTile(Coordinate2 target)
	{
		int turn = matchData.Turn; // Don't want the turn changing while this is running
		bool purchaseSuccess = false;

		Debug.Log(debugTag + "Player " + turn
			+ " (ID: " + players[turn - 1].Id
			+ ") trying to buy tile " + target.ToString());

		// Check against the rest of the purchasing rules before proceding
		if (IsValidPurchase(target, turn))
		{
			players[turn - 1].ownedTiles.Add(target); // Server-side
			gridController.masterGrid[target.x, target.y].OwnerId = turn;

			TurnEvent turnEvent = new TurnEvent(matchData.Phase, turn, "Buy", "Tile", target.x, target.y,
				matchDataBroadcaster.TopCardStr);
			matchDataBroadcaster.TurnEventStr = JsonUtility.ToJson(turnEvent);
			Debug.Log(debugTag + "JSON: " + turnEvent);

			// this.turnEventBroadcast = turn + "_x" + target.x + "_y" + target.y;
			Debug.Log(debugTag + "Player " + turn
				+ " (ID: " + players[turn - 1].Id
				+ ") bought tile " + target.ToString());
			// Debug.Log(debugTag + "Advancing Turn; This is a temporary mechanic!");

			UpdatePlayersInfo();
			this.IncrementTurn();
			purchaseSuccess = true;
		}
		else
		{
			Debug.Log(debugTag + "Can't purchase, tile " + target.ToString()
				+ " is not valid for you! \nAlready Owned?\nOut of Range?\nBankrupt Tile?");
		}

		return purchaseSuccess;
	} // BuyTile()

	// Overload of BuyTile(), taking in two ints instead of a Coordinate2.
	// Assumes that the player whose turn it is can be the only one who calls this (for now).
	public bool BuyTile(int x, int y)
	{
		return BuyTile(new Coordinate2(x, y));
	} // BuyTile()

	public void UpdatePlayersInfo()
	{
		gridController.UpdateResourceValues();
		// Things that need to be updated for all players go here
		for (int i = 0; i < players.Count; i++)
		{
			players[i].CalcTotalMoney();
			// Debug.Log("Player " + (i + 1) + "'s Money: $" + players[i].totalMoney);
		} // for array length
		UpdatePlayerMoneyStr();
	} // UpdatePlayersInfo()

	public void BankruptTile(CardData tile)
	{
		Debug.Log(debugTag + "Bankrupting tile!");
		tile.OwnerId = 0;
		// CardDisplay.BankruptVisuals(tile.tileObj);
	} // BankruptTile()

	// Advance to the next turn
	public void IncrementTurn()
	{
		SkipChecker();

		if (GetValidNextTurn(matchData.Turn) <= config.MaxPlayerCount)
		{
			// The next player can go with no issues.
			// Debug.Log(debug + "ValidNextTurn = " + GetValidNextTurn(turn));
			matchData.Turn = GetValidNextTurn(matchData.Turn);
			// this.round++;
		}
		else
		{
			if (GetValidNextTurn(0) <= config.MaxPlayerCount)
			{
				// The next player is not available, but some still are.
				// Debug.Log(debug + "ValidNextTurn = " + GetValidNextTurn(1));
				matchData.Turn = GetValidNextTurn(0);
				matchData.Round++;
			}
			else
			{
				// Nobody else can go, signaling the end of the phase.
				// Debug.Log(debug + "ValidNextTurn = " + GetValidNextTurn(turn));
				matchData.Turn = 1;
				// this.round = 1;
				matchData.Phase++;
				foreach (PlayerData player in players)
				{
					player.shouldSkip = false;
				}
			}
		}

		Debug.Log(debugTag + "#################### Turn " + matchData.Turn
			+ " | Round " + matchData.Round
			+ " | Phase " + matchData.Phase + " ####################");
	}

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
						if (gridController.masterGrid[x + i, y + j].OwnerId == 0 // Is the tile unowned?
							&& !gridController.masterGrid[x + i, y + j].IsBankrupt // Is the tile not bankrupt?
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

	private bool IsValidPurchase(Coordinate2 tileBeingPurchased, int playerId)
	{
		bool isValid = false;
		List<Coordinate2> validCards = new List<Coordinate2>();

		if (matchData.Round > config.GraceRounds)
		{
			// Find each unowned neighbor tiles for this player
			validCards = GetValidCards(playerId);
		}
		else
		{
			int x = tileBeingPurchased.x;
			int y = tileBeingPurchased.y;
			// This is where the tile is checked against purchasing rules
			if (gridController.masterGrid[x, y].OwnerId == 0 // Is the tile unowned?
				&& !gridController.masterGrid[x, y].IsBankrupt)
			{ // Is the tile not bankrupt?

				isValid = true;
			}
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

	public void UpdatePriceListStr()
	{
		string newPriceListStr = "";

		for (int i = 1; i < ResourceInfo.resources.Count; i++)
		{
			int value = -1;
			newPriceListStr += ResourceInfo.resources[i] + "=";
			ResourceInfo.pricesMut.TryGetValue(ResourceInfo.resources[i], out value);
			newPriceListStr += value;

			if (ResourceInfo.resources.Count - i > 1)
			{
				newPriceListStr += "_";
			}
		}

		matchDataBroadcaster.PriceListStr = newPriceListStr;
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

		this.masterDeck = new MasterDeck(config.DeckFlavor);
		this.masterDeckMutable = new MasterDeck(config.DeckFlavor);

		// [Server]
		if (hasAuthority)
		{
			InitPlayers();

			this.matchData = new MatchData();

			Debug.Log(debugTag + "Sending current Match Data to Broadcaster... " + this.matchData);
			matchDataBroadcaster.MatchDataStr = JsonUtility.ToJson(this.matchData);
		}

	}

	// [Server] Loads this MatchManager's config from the MatchSetupController's final config.
	private IEnumerator LoadMatchConfigCoroutine(MatchSetupController controller)
	{
		Debug.Log(debugTag + "Grabbing config from MatchSetupController...");

		while (!controller.Ready)
			yield return null;

		this.config = controller.InitialConfig;
		Debug.Log(debugTag + "Config loaded! Sending to Broadcaster...");
		matchDataBroadcaster.MatchConfigStr = JsonUtility.ToJson(controller.InitialConfig);
	}

}

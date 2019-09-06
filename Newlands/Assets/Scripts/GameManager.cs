// Manages most UI Elements and game setup tasks.

using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
	public CardDisplay cardDis;
	// public GuiManager guiMan;
	public GridManager gridMan;

	// VARIABLES ###################################################################################

	public static MasterDeck masterDeck;
	public static MasterDeck masterDeckMutable;
	public readonly int playerCount = 2; // Number of players in the match
	[SyncVar]
	private int playerIndex = 1; // This value increments when a new player joins
	public static int localPlayerId = -1;
	[SyncVar]
	public int phase = 1; // The current phase of the game
	[SyncVar]
	public int round = 1; // The current round of turns
	[SyncVar(hook = "OnTurnChange")]
	public int turn = 1; // The current turn in the round
	public static int graceRounds = 1; // The # of rounds without neighbor rules

	[SyncVar]
	public string topCardStr = "";

	[SyncVar]
	public string playersMoneyStr = "";

	[SyncVar]
	public string priceListStr = "";

	public readonly int width = 7; // Width of the game grid in cards
	public readonly int height = 7; // Height of the game grid in cards
	public readonly int handSize = 5; // How many cards the player is dealt

	private static DebugTag debug = new DebugTag("GameManager", "FF6D00");

	[SerializeField]
	public static List<Player> players = new List<Player>(); // The player data objects

	// Broadcasts the results of the turn to all clients in the form of a parsable string.
	// NOTE: Currently, this in only used for broadcasting tile purchases.
	[SyncVar]
	public string turnEventBroadcast = "";
	// FUTURE IDEA: When the clients parse the string above, have them increment this number by one.
	// When this string is set by GameManager, reset this number to 0.
	// While the clients check for parsing, have them check this number to make sure it's not
	// equal to the number of players, implying a stale string.
	// [SyncVar]
	// public int purchasedTileInfoReceived = 0;

	// private static List<GameObject> playerMoneyObj = new List<GameObject>();
	// private static List<TMP_Text> playerMoneyText = new List<TMP_Text>();

	// Used for initialization
	public void InitGameManager()
	{
		if (!hasAuthority)
		{
			return;
		}

		Debug.Log(debug + "Initializing GameManager...");
		Debug.Log(debug + "Creating Master Deck \"Vanilla\"");
		masterDeck = new MasterDeck("Vanilla");
		masterDeckMutable = new MasterDeck("Vanilla");

		Debug.Log(debug + "Initializing Players...");
		InitPlayers();

		// Initialize the internal grids
		gridMan.InitGameGrid();
		gridMan.InitMarketGrid();

		// FINAL ###############################################################

		// Make sure that there is at least 1 Grace Round
		if (graceRounds < 1)
		{
			graceRounds = 1;
		} // if (graceRounds < 1)

		// GameObject cameraObj = transform.Find("Main Camera").gameObject;
		// mainCam = cameraObj.GetComponent<Camera>();

		// Push the first UI Update
		// guiMan.InitGuiManager();
		// guiMan.UpdateUI();

		Debug.Log(debug + "#################### Turn " + this.turn
			+ " | Round " + this.round
			+ " | Phase " + this.phase + " ####################");
	} // Start()

	void Update()
	{

	} // Update()

	// Draws a card from a deck. Random by default.
	public static bool DrawCard(Deck deckMut, Deck deckPerm, out CardData card, bool random = true)
	{
		// Card card;	// Card to return
		int cardsLeft = deckMut.Count(); // Number of cards left from mutable deck
		int cardsTotal = deckPerm.Count(); // Number of cards total from permanent deck

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
				card = deckMut[deckMut.Count() - 1];
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
	} // DrawCard()

	private int GetValidNextTurn(int turn)
	{
		// If the turn exceeds the player count, return the value anyway.
		if ((turn + 1) > this.playerCount)
		{
			// Debug.Log(debug + "Returning " + (turn + 1));
			return (turn + 1);
		}

		// If the turn should be skipped, try again with the next turn up.
		if (GameManager.players[turn].shouldSkip)
		{ // This doesn't need -1 because it's checking +1
			// Debug.Log(debug + "Recursive...");
			return GetValidNextTurn(turn + 1);
		}

		// If nothing interresting happens, return the next turn.
		// Debug.Log(debug + "Returning " + (turn + 1) + " (Nothing happened)");
		return (turn + 1);
	} // GetNextValidTurn()

	// Advance to the next turn
	public void IncrementTurn()
	{
		SkipChecker();

		if (GetValidNextTurn(turn) <= playerCount)
		{
			// The next player can go with no issues.
			// Debug.Log(debug + "ValidNextTurn = " + GetValidNextTurn(turn));
			this.turn = GetValidNextTurn(turn);
			// this.round++;
		}
		else
		{
			if (GetValidNextTurn(0) <= playerCount)
			{
				// The next player is not available, but some still are.
				// Debug.Log(debug + "ValidNextTurn = " + GetValidNextTurn(1));
				this.turn = GetValidNextTurn(0);
				this.round++;
			}
			else
			{
				// Nobody else can go, signaling the end of the phase.
				// Debug.Log(debug + "ValidNextTurn = " + GetValidNextTurn(turn));
				this.turn = 1;
				// this.round = 1;
				this.phase++;
				foreach (Player player in players)
				{
					player.shouldSkip = false;
				}
			}
		}

		Debug.Log(debug + "#################### Turn " + this.turn
			+ " | Round " + this.round
			+ " | Phase " + this.phase + " ####################");
	} //IncrementTurn()

	// End the current round, starts at next turn 0
	public void EndRound()
	{
		round++;
		turn = 1;
	} //EndRound()

	// End the current phase, starts at next round and turn 0
	public void EndPhase()
	{
		phase++;
		// round = 1;
		turn = 1;
	} //EndPhase()

	// Tries to play a Card on a TIle. Returns true is successful.
	// Assumes that the player whose turn it is can be the only one who calls this (for now).
	public bool PlayCard(int cardIndex, string targetTile)
	{
		bool wasPlayed = false;
		int locX = int.Parse(targetTile.Substring(1, 2));
		int locY = int.Parse(targetTile.Substring(5, 2));
		int round = this.round;
		int turn = this.turn;
		string tileType = targetTile.Substring(8);
		// GridUnit target = GridManager.grid[locX, locY];
		GridUnit target;
		CardData card = players[turn - 1].hand[cardIndex];

		Debug.Log(debug + "Trying to play Card " + cardIndex + " on " + tileType
			+ " at " + locX + ", " + locY);

		switch (tileType)
		{
			case "Tile":
				target = GridManager.grid[locX, locY];
				if (!target.bankrupt
					&& RuleSet.IsLegal(target, card))
				{
					// Carry out the actions of a Successful play
					UpdatePlayersInfo(); // Test to see if there only needs to be one of these at the end
					if (target.bankrupt) // Bankrupt check
					{
						BankruptTile(GridManager.grid[locX, locY]);
						UpdatePlayersInfo();
						// guiMan.UpdateUI();
						Debug.Log(debug + "Tile bankrupt! has value of "
							+ GridManager.grid[locX, locY].totalValue);
					}

					if (!card.doesDiscard)
					{
						GridManager.grid[locX, locY].stackSize++;
						GridManager.grid[locX, locY].cardStack.Add(card);
						// target.CalcTotalValue(); // This fixes Market Cards not calcing first time
						UpdatePlayersInfo();
						// guiMan.UpdateUI();
						if (card.title == "Tile Mod")
						{
							if (target.stackSize > GridManager.maxStack[target.y])
							{
								GridManager.maxStack[target.y]++;
								gridMan.ShiftRow(target.category, target.y, 1);
							} // if stack size exceeds max stack recorded for row

							GameObject cardObj = (GameObject)Instantiate(gridMan.gameCardPrefab,
								new Vector3(target.tileObj.transform.position.x,
									target.tileObj.transform.position.y
									- (GridManager.shiftUnit * target.stackSize),
									(target.tileObj.transform.position.z)
									+ (GridManager.cardThickness * target.stackSize)),
								Quaternion.identity);

							NetworkServer.Spawn(cardObj);

							// This is also done of the client via CardState
							// cardObj.transform.SetParent(target.tileObj.transform);

							CardState cardState = cardObj.GetComponent<CardState>();
							// Push new values to the CardState to be synced across the network
							GridManager.FillOutCardState(card, ref cardState);

							// Generate and Push the string of the object's name
							cardState.objectName = GameManager.CreateCardObjectName("StackedCard", 0,
								target.stackSize - 1);
							cardState.parent = GameManager.CreateCardObjectName("Tile", locX, locY);

							// Target
							string cardToDestroy = CreateCardObjectName("GameCard", turn - 1,
								cardIndex);
							// TargetDestroyGameObject(connectionToClient, cardToDestroy);

						}

						CardData topCard;

						if (DrawCard(masterDeckMutable.gameCardDeck, masterDeck.gameCardDeck, out topCard))
						{
							this.topCardStr = JsonUtility.ToJson(topCard);
							players[turn - 1].hand[cardIndex] = topCard;
						}
						else
						{
							this.topCardStr = "empty";
						}
						TurnEvent turnEvent = new TurnEvent(2, turn, "Play",
							"GameCard", turn, cardIndex, this.topCardStr);
						this.turnEventBroadcast = JsonUtility.ToJson(turnEvent);

						UpdatePlayersInfo();
						this.IncrementTurn();
						wasPlayed = true;
					}
				}
				break;

			case "MarketCard":
				target = GridManager.marketGrid[locX, locY];
				if (!target.bankrupt
					&& RuleSet.IsLegal(target, card))
				{
					// Carry out the actions of a Successful play
					UpdatePlayersInfo(); // Test to see if there only needs to be one of these at the end
					if (target.bankrupt) // Bankrupt check
					{
						BankruptTile(GridManager.marketGrid[locX, locY]);
						UpdatePlayersInfo();
						// guiMan.UpdateUI();
						Debug.Log(debug + "Market Card bankrupt! has value of "
							+ GridManager.marketGrid[locX, locY].totalValue);
					}

					if (!card.doesDiscard)
					{
						GridManager.marketGrid[locX, locY].stackSize++;
						GridManager.marketGrid[locX, locY].cardStack.Add(card);
						// target.CalcTotalValue(); // This fixes Market Cards not calcing first time
						UpdatePlayersInfo();
						// guiMan.UpdateUI();
						if (card.title == "Market Mod")
						{
							if (target.stackSize > GridManager.maxMarketStack[target.y])
							{
								GridManager.maxMarketStack[target.y]++;
								gridMan.ShiftRow(target.category, target.y, 1);
							} // if stack size exceeds max stack recorded for row

							GameObject cardObj = (GameObject)Instantiate(gridMan.gameCardPrefab,
								new Vector3(target.tileObj.transform.position.x,
									target.tileObj.transform.position.y
									- (GridManager.shiftUnit * target.stackSize),
									(target.tileObj.transform.position.z)
									+ (GridManager.cardThickness * target.stackSize)),
								Quaternion.identity);

							NetworkServer.Spawn(cardObj);

							// This is also done of the client via CardState
							// cardObj.transform.SetParent(target.tileObj.transform);

							CardState cardState = cardObj.GetComponent<CardState>();
							// Push new values to the CardState to be synced across the network
							GridManager.FillOutCardState(card, ref cardState);

							// Generate and Push the string of the object's name
							cardState.objectName = GameManager.CreateCardObjectName("StackedCard", 0,
								target.stackSize - 1);
							cardState.parent = GameManager.CreateCardObjectName("MarketCard", locX, locY);

							// Target
							string cardToDestroy = CreateCardObjectName("GameCard", turn - 1,
								cardIndex);
							// TargetDestroyGameObject(connectionToClient, cardToDestroy);

						}

						CardData topCard;

						if (DrawCard(masterDeckMutable.gameCardDeck, masterDeck.gameCardDeck, out topCard))
						{
							this.topCardStr = JsonUtility.ToJson(topCard);
							players[turn - 1].hand[cardIndex] = topCard;
						}
						else
						{
							this.topCardStr = "empty";
						}

						UpdatePriceListStr();

						TurnEvent turnEvent = new TurnEvent(2, turn, "Play",
							"GameCard", turn, cardIndex, this.topCardStr);
						this.turnEventBroadcast = JsonUtility.ToJson(turnEvent);

						UpdatePlayersInfo();
						this.IncrementTurn();
						wasPlayed = true;
					}
				}
				break;

			default:
				Debug.LogError(debug.error + "Couldn't find Tile of type \"" + tileType + "\"");
				break;
		} // switch (tileType)

		Debug.Log(debug + "GameCards left: " + masterDeckMutable.gameCardDeck.Count());

		return wasPlayed;
	} // PlayCard()

	public void UpdatePlayersInfo()
	{
		gridMan.UpdateResourceValues();
		// Things that need to be updated for all players go here
		for (int i = 0; i < players.Count; i++)
		{
			players[i].CalcTotalMoney();
			// Debug.Log("Player " + (i + 1) + "'s Money: $" + players[i].totalMoney);
		} // for array length
		UpdatePlayerMoneyStr();
	} // UpdatePlayersInfo()

	public static void BankruptTile(GridUnit tile)
	{
		Debug.Log(debug + "Bankrupting tile!");
		tile.ownerId = 0;
		CardDisplay.BankruptVisuals(tile.tileObj);
	} // BankruptTile()

	// Initializes each player object and draws a hand for them
	private void InitPlayers()
	{
		for (int i = 0; i < playerCount; i++)
		{
			players.Add(new Player());
			players[i].Id = (i + 1);
			players[i].hand = DrawHand(handSize);
		} // for playerCount
		UpdatePlayerMoneyStr();
	} // InitPlayers()

	private void UpdatePlayerMoneyStr()
	{
		playersMoneyStr = "";

		for (int i = 0; i < playerCount; i++)
		{
			playersMoneyStr += players[i].totalMoney;

			if (playerCount - i > 1)
			{
				playersMoneyStr += "_";
			}
		} // for playerCount
		Debug.Log(debug + "Player Money String: " + playersMoneyStr);
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
			CardData card;
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
	} // DrawHand()

	public void IncrementPlayerIndex()
	{
		this.playerIndex++;
	}

	public int GetPlayerIndex()
	{
		return this.playerIndex;
	}

	public void OnTurnChange(int newTurn)
	{

	} // OnTurnChange()

	public bool BuyTile(Coordinate2 target)
	{
		int turn = this.turn; // Don't want the turn changing while this is running
		bool purchaseSuccess = false;

		Debug.Log(debug + "Player " + turn
			+ " (ID: " + GameManager.players[turn - 1].Id
			+ ") trying to buy tile " + target.ToString());

		// Check against the rest of the purchasing rules before proceding
		if (IsValidPurchase(target, turn))
		{
			GameManager.players[turn - 1].ownedTiles.Add(target); // Server-side
			GridManager.grid[target.x, target.y].ownerId = turn;

			TurnEvent turnEvent = new TurnEvent(this.phase, turn, "Buy", "Tile", target.x, target.y,
				this.topCardStr);
			this.turnEventBroadcast = JsonUtility.ToJson(turnEvent);
			Debug.Log(debug + "JSON: " + turnEvent);

			// this.turnEventBroadcast = turn + "_x" + target.x + "_y" + target.y;
			Debug.Log(debug + "Player " + turn
				+ " (ID: " + GameManager.players[turn - 1].Id
				+ ") bought tile " + target.ToString());
			// Debug.Log(debug + "Advancing Turn; This is a temporary mechanic!");

			UpdatePlayersInfo();
			this.IncrementTurn();
			purchaseSuccess = true;
		}
		else
		{
			Debug.Log(debug + "Can't purchase, tile " + target.ToString()
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

	private bool IsValidPurchase(Coordinate2 tileBeingPurchased, int playerId)
	{
		bool isValid = false;
		List<Coordinate2> validCards = new List<Coordinate2>();

		if (this.round > graceRounds)
		{
			// Find each unowned neighbor tiles for this player
			validCards = GetValidCards(playerId);
		}
		else
		{
			int x = tileBeingPurchased.x;
			int y = tileBeingPurchased.y;
			// This is where the tile is checked against purchasing rules
			if (GridManager.grid[x, y].ownerId == 0 // Is the tile unowned?
				&& !GridManager.grid[x, y].bankrupt)
			{ // Is the tile not bankrupt?

				isValid = true;
			}
		}

		if (validCards.Contains(tileBeingPurchased))
		{
			isValid = true;
		}

		return isValid;
	} // GetValidTiles()

	// Overload of IsValidPurchase(), taking in two ints instead of a Coordinate2.
	private bool IsValidPurchase(int x, int y, int playerId)
	{
		return IsValidPurchase(new Coordinate2(x, y), playerId);
	} // IsValidPurchase()

	private List<Coordinate2> GetValidCards(int playerId)
	{
		List<Coordinate2> validCards = new List<Coordinate2>();

		for (int k = 0; k < GameManager.players[playerId - 1].ownedTiles.Count; k++)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					// Set x and y equal to the coordinate of the owned tile being checked
					int x = GameManager.players[playerId - 1].ownedTiles[k].x;
					int y = GameManager.players[playerId - 1].ownedTiles[k].y;

					// Is the neighbor tile within the grid bounds?
					if (x + i >= 0
						&& x + i < GridManager.grid.GetLength(0)
						&& y + j >= 0
						&& y + j < GridManager.grid.GetLength(1))
					{
						// This is where the tile is checked against purchasing rules
						if (GridManager.grid[x + i, y + j].ownerId == 0 // Is the tile unowned?
							&& !GridManager.grid[x + i, y + j].bankrupt // Is the tile not bankrupt?
							&& !validCards.Contains(new Coordinate2((x + i), (y + j))))
						{
							Debug.Log(debug + "Found Valid Neighbor [" + (x + i) + ", " + (y + j) + "] for Player " + playerId);
							validCards.Add(new Coordinate2((x + i), (y + j)));
						}
					}
				}
			}
		}

		return validCards;
	} // GetValidCards()

	// Checks if it's possible for any more cards to be bought by the player and sets a skip flag
	// on the player if necessary.
	private void SkipChecker()
	{
		for (int i = 0; i < this.playerCount; i++)
		{
			if (this.round > graceRounds && this.phase == 1)
			{
				if (GetValidCards(i + 1).Count == 0)
				{
					Debug.Log(debug + "Setting shouldSkip true for Player " + (i + 1)
						+ " on Phase " + this.phase);
					GameManager.players[i].shouldSkip = true;
				}
			}
		}
	} // CheckForSpaces()

	// Returns a formatted object name based on type and coordinate info
	public static string CreateCardObjectName(string type, int x, int y)
	{
		string xZeroes = "0";
		string yZeroes = "0";
		char xChar = 'x';
		char yChar = 'y';

		if (type == "GameCard")
		{
			xChar = 'p';
			yChar = 'i';
		}

		if (x >= 10)
		{
			xZeroes = "";
		}
		else
		{
			xZeroes = "0";
		}

		if (y >= 10)
		{
			yZeroes = "";
		}
		else
		{
			yZeroes = "0";
		} // zeroes calc

		return (xChar + xZeroes + x + "_" + yChar + yZeroes + y + "_" + type);
	} // CreateCardObjectName()

	// An overload of CreateCardObjectName that takes in a Coordinate2
	public static string CreateCardObjectName(string type, Coordinate2 location)
	{
		return CreateCardObjectName(type, location.x, location.y);
	} // CreateCardObjectName()

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

		this.priceListStr = newPriceListStr;
	}

} // GameManager class

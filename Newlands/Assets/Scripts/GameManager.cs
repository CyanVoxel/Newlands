// Manages most UI Elements and game setup tasks.

using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour {

	public CardDisplay cardDis;
	// public GuiManager guiMan;
	public GridManager gridMan;

	// VARIABLES ##################################################################################

	public static MasterDeck masterDeck;
	public static MasterDeck masterDeckMutable;
	public static readonly int playerCount = 2; // Number of players in the match
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

	public static readonly int width = 7; // Width of the game grid in cards
	public static readonly int height = 3; // Height of the game grid in cards
	public static readonly int handSize = 5; // How many cards the player is dealt

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
	void Start() {

		if (!hasAuthority) {
			return;
		}

		Debug.Log(debug.head + "Initializing GameManager...");

		Debug.Log(debug.head + "Creating Master Deck \"Vanilla\"");
		masterDeck = new MasterDeck("Vanilla");
		masterDeckMutable = new MasterDeck("Vanilla");

		Debug.Log(debug.head + "Initializing Players...");
		InitPlayers();

		// Initialize the internal grids
		gridMan.InitGameGrid();
		gridMan.InitMarketGrid();

		// FINAL ##############################################################

		// Make sure that there is at least 1 Grace Round
		if (graceRounds < 1) {
			graceRounds = 1;
		} // if (graceRounds < 1)

		// GameObject cameraObj = transform.Find("Main Camera").gameObject;
		// mainCam = cameraObj.GetComponent<Camera>();

		// Push the first UI Update
		// guiMan.InitGuiManager();
		// guiMan.UpdateUI();

	} // Start()

	void Update() {

	} // Update()

	// Draws a card from a deck. Random by default.
	public static bool DrawCard(Deck deckMut, Deck deckPerm, out Card card, bool random = true) {

		// Card card;	// Card to return
		int cardsLeft = deckMut.Count(); // Number of cards left from mutable deck
		int cardsTotal = deckPerm.Count(); // Number of cards total from permanent deck

		// Draws a card from the mutable deck, then removes that card from the deck.
		// If all cards are drawn, draw randomly from the immutable deck.
		if (cardsLeft > 0) {
			if (random) {
				card = deckMut[Random.Range(0, cardsLeft)];
			} else {
				card = deckMut[deckMut.Count() - 1];
			}

			deckMut.Remove(card);
			// Debug.Log("<b>[GameManager]</b> " +
			// 	cardsLeft +
			// 	" of " +
			// 	cardsTotal +
			// 	" cards left");
		} else {
			// This one HAS to be random anyways
			card = deckPerm[Random.Range(0, cardsTotal)];
			return false;
			// Debug.LogWarning("<b>[GameManager]</b> Warning: " +
			//  "All cards (" + cardsTotal + ") were drawn from a deck! " +
			//  " Now drawing from immutable deck...");
		}

		// return card;
		return true;

	} // DrawCard()

	// Advance to the next turn
	public void IncrementTurn(int turnChecked = 0, bool skipThis = false) {

		if (this.turn < playerCount) {
			this.turn++;
		} else {
			if (turnChecked > 0) { // If the turn checked was passed
				if (skipThis) { // And the turn passed should always be skipped now
					this.turn = turnChecked; // Set the turn to one past the skipable turn
				}
				this.round++;
			} else {
				this.round++; // NOTE: In the future where round are capped, this needs error detection too
				this.turn = 1;
			}

		}

		Debug.Log(debug.head + "########## Turn Advanced to " + this.turn);
		Debug.Log(debug.head + "########## Round Advanced to " + this.round);

	} //IncrementTurn()

	// End the current round, starts at next turn 0
	public void EndRound() {
		round++;
		turn = 1;
	} //EndRound()

	// End the current phase, starts at next round and turn 0
	public void EndPhase() {
		phase++;
		round = 1;
		turn = 1;
	} //EndPhase()


	// Returns the color associated with a player ID.
	// Strength paramater refers to a possible brighter color variant.
	public Color GetPlayerColor(int playerID, int strength = 500) {

		Color color = ColorPalette.tintCard;

		switch (playerID) {

			case 1:
				if (strength == 500) {
					color = ColorPalette.lightBlue500;
				} else if (strength == 400) {
					color = ColorPalette.lightBlue400;
				} else if (strength == 300) {
					color = ColorPalette.lightBlue300;
				} else if (strength == 200) {
					color = ColorPalette.lightBlue200;
				}
				break;
			case 2:
				if (strength == 500) {
					color = ColorPalette.red500;
				} else if (strength == 400) {
					color = ColorPalette.red400;
				} else if (strength == 300) {
					color = ColorPalette.red300;
				} else if (strength == 200) {
					color = ColorPalette.red300;
				}
				break;
			case 3:
				if (strength == 500) {
					color = ColorPalette.purple500;
				} else if (strength == 400) {
					color = ColorPalette.purple400;
				} else if (strength == 300) {
					color = ColorPalette.purple300;
				} else if (strength == 200) {
					color = ColorPalette.purple200;
				}
				break;
			case 4:
				if (strength == 500) {
					color = ColorPalette.amber500;
				} else if (strength == 400) {
					color = ColorPalette.amber400;
				} else if (strength == 300) {
					color = ColorPalette.amber300;
				} else if (strength == 200) {
					color = ColorPalette.amber200;
				}
				break;
			default:
				break;
		} // switch

		return color;

	} // GetPlayerColor()

	// Checks if a GameCard is allowed to be played on a Tile.
	public bool TryToPlay(GridUnit gridTile, GridUnit gameCard) {

		if (!gridTile.bankrupt && RuleSet.IsLegal(gridTile, gameCard)) {

			RuleSet ruleSet = new RuleSet();

			ruleSet.PlayCard(gridTile, gameCard.card);
			UpdatePlayersInfo();
			// guiMan.UpdateUI();

			Vector3 oldCardPosition = gameCard.tileObj.transform.position;
			int oldCardIndex = gameCard.x;
			Debug.Log(debug.head + "Old Card Index: " + oldCardIndex);

			if (gridTile.bankrupt) {
				BankruptTile(gridTile);
				UpdatePlayersInfo();
				// guiMan.UpdateUI();
				Debug.Log(debug.head + "Tile bankrupt! has value of " + gridTile.totalValue);
			}

			if (gameCard.stackable) {

				gridTile.stackSize++;
				gridTile.cardStack.Add(gameCard.card);
				gridTile.CalcTotalValue(); // This fixes Market Cards not calculating first time
				UpdatePlayersInfo();
				// guiMan.UpdateUI();

				if (gameCard.card.title == "Tile Mod") {

					// If the stack on the unit is larger than the stack count on the row, increase
					if (gridTile.stackSize > GridManager.maxStack[gridTile.y]) {
						GridManager.maxStack[gridTile.y]++;
						gridMan.ShiftRow(gridTile.category, gridTile.y, 1);
					} // if stack size exceeds max stack recorded for row

					gameCard.tileObj.transform.position = new Vector3(gridTile.tileObj.transform.position.x,
						gridTile.tileObj.transform.position.y
						- (GridManager.shiftUnit * gridTile.stackSize),
						(gridTile.tileObj.transform.position.z)
						+ (GridManager.cardThickness * gridTile.stackSize));

					gameCard.tileObj.transform.parent = gridTile.tileObj.transform;

					// TODO: Adjust for more than 10 cards on a given stack (unlikely, but possible)
					// TODO: Account for different players
					gameCard.tileObj.name = ("p00_"
						+ "i0" + (gridTile.stackSize - 1) + "_"
						+ "StackedCard");

				} else if (gameCard.card.title == "Market Mod") {

					// If the stack on the unit is larger than the stack count on the row, increase
					if (gridTile.stackSize > GridManager.maxMarketStack[gridTile.y]) {
						GridManager.maxMarketStack[gridTile.y]++;
						gridMan.ShiftRow(gridTile.card.category, gridTile.y, 1);
					} // if stack size exceeds max stack recorded for row

					gameCard.tileObj.transform.position = new Vector3(gridTile.tileObj.transform.position.x,
						gridTile.tileObj.transform.position.y
						- (GridManager.shiftUnit * gridTile.stackSize),
						(gridTile.tileObj.transform.position.z)
						+ (GridManager.cardThickness * gridTile.stackSize));

					gameCard.tileObj.transform.parent = gridTile.tileObj.transform;

					// TODO: Adjust for more than 10 cards on a given stack (unlikely, but possible)
					// TODO: Account for different players
					gameCard.tileObj.name = ("p00_"
						+ "i0" + (gridTile.stackSize - 1) + "_"
						+ "StackedCard");

				} // if market mod

			} else {
				// After ALL processing is done, destroy the game object
				Destroy(gameCard.tileObj);
				// guiMan.UpdateUI();
			} // if stackable

			// Create a new card to replace the old one
			Card newCard = Card.CreateInstance<Card>();
			if (DrawCard(masterDeckMutable.gameCardDeck, masterDeck.gameCardDeck, out newCard)
				&& masterDeckMutable.gameCardDeck.Count() > 0) {

				players[0].hand.Add(newCard);
				players[0].handUnits[oldCardIndex].LoadNewCard(newCard, players[0].handUnits[oldCardIndex].tileObj);
				players[0].hand[oldCardIndex] = newCard;
				// DisplayCard(newCard, 0, oldCardIndex);
				players[0].handUnits[oldCardIndex].tileObj.transform.position = oldCardPosition;

			} // if card can be drawn

			// gridTile.cardStack.Add(gameCard.tile);
			MouseManager.selection = -1;
			// WipeSelectionColors("Game Card", ColorPalette.tintCard);
			return true;

		} else {
			Debug.Log(debug.head + "Card move is illegal!");
			MouseManager.selection = -1;
			// WipeSelectionColors("Game Card", ColorPalette.tintCard);
			return false;
		} // If the Category AND Scope match

	} // TryToPlay()

	public GameObject FindCard(string type, int x, int y) {

		string strX = "x";
		string strY = "y";

		// Determines the number of zeroes to add in the object name
		string xZeroes = "0";
		string yZeroes = "0";
		if (x >= 10) {
			xZeroes = "";
		} // if x >= 10
		if (y >= 10) {
			yZeroes = "";
		} // if y >= 10

		// Specific type changes
		if (type == "GameCard") {
			strX = "p"; // Instead of x, use p for PlayerID
			strY = "i"; // Instead of y, use i for Index
		} // if GameCard

		if (gridMan.transform.Find(strX + xZeroes + x + "_" + strY + yZeroes + y + "_" + type)) {
			// GameObject gameObject = new GameObject();
			GameObject gameObject = gridMan.transform.Find(strX + xZeroes + x + "_"
				+ strY + yZeroes + y + "_"
				+ type).gameObject;
			return gameObject;
		} else {
			// Debug.LogError("[GameManager] Error: Could not find GameObject!");
			return null;
		}

	} // FindCard()

	public static void UpdatePlayersInfo() {
		// Things that need to be updated for all players go here
		for (int i = 0; i < players.Count; i++) {
			players[i].CalcTotalMoney();
			// Debug.Log("Player " + (i + 1) + "'s Money: $" + players[i].totalMoney);
		} // for array length

	} // UpdatePlayersInfo()

	public static void BankruptTile(GridUnit tile) {

		Debug.Log(debug.head + "Bankrupting tile!");
		tile.ownerId = 0;
		CardDisplay.BankruptVisuals(tile.tileObj);

	} // BankruptTile()

	// Initializes each player object and draws a hand for them
	private void InitPlayers() {

		for (int i = 0; i < playerCount; i++) {
			players.Add(new Player());
			players[i].Id = (i + 1);
			players[i].hand = DrawHand(handSize);
		} // for playerCount

	} // InitPlayers()

	// Draws random GameCards from the masterDeck and returns a deck of a specified size
	private Deck DrawHand(int handSize) {

		Deck deck = new Deck(); // The deck of drawn cards to return

		for (int i = 0; i < handSize; i++) {
			// Draw a card from the deck provided and add it to the deck to return.
			// NOTE: In the future, masterDeckMutable might need to be checked for cards
			// 	before preceding.
			Card card = Card.CreateInstance<Card>();
			if (DrawCard(masterDeckMutable.gameCardDeck, masterDeck.gameCardDeck, out card)) {
				deck.Add(card);
			} else {
				Destroy(card);
			}

		} // for

		return deck;

	} // DrawHand()

	public void IncrementPlayerIndex() {
		this.playerIndex++;
	}

	public int GetPlayerIndex() {
		return this.playerIndex;
	}

	public void OnTurnChange(int newTurn) {
		MouseManager.highlightFlag = false;
	} // OnTurnChange()

	public bool BuyTile(Coordinate2 target) {

		int turn = this.turn; // Don't want the turn changing while this is running
		bool purchaseSuccess = false;

		// Check against the rest of the purchasing rules before proceding
		if (IsValidPurchase(target, turn)) {

			GameManager.players[turn - 1].ownedTiles.Add(target); // Server-side
			// GridManager.grid[target.x, target.y].ownerId = turn;
			this.turnEventBroadcast = turn + "_x" + target.x + "_y" + target.y;
			Debug.Log(debug.head + "Player " + turn
				+ " (ID: " + GameManager.players[turn - 1].Id
				+ ") bought tile " + target.ToString());
			// Debug.Log(debug.head + "Advancing Turn; This is a temporary mechanic!");
			purchaseSuccess = true;
			this.IncrementTurn();

		} else {
			Debug.Log(debug.head + "Can't purchase, tile " + target.ToString()
				+ " is not valid for you! \nAlready Owned?\nOut of Range?\nBankrupt Tile?");
		}

		return purchaseSuccess;

	} // BuyTile()

	// Overload of BuyTile(), taking in two ints instead of a Coordinate2.
	public bool BuyTile(int x, int y) {
		return BuyTile(new Coordinate2(x, y));
	} // BuyTile()

	private bool IsValidPurchase(Coordinate2 tileBeingPurchased, int playerId) {

		bool isValid = false;
		List<Coordinate2> validCards = new List<Coordinate2>();

		if (this.round > graceRounds) {
			// Find each unowned neighbor tiles for this player
			for (int k = 0; k < GameManager.players[playerId - 1].ownedTiles.Count; k++) {
				for (int i = -1; i <= 1; i++) {
					for (int j = -1; j <= 1; j++) {

						// Set x and y equal to the coordinate of the owned tile being checked
						int x = GameManager.players[playerId - 1].ownedTiles[k].x;
						int y = GameManager.players[playerId - 1].ownedTiles[k].y;

						// Is the neighbor tile within the grid bounds?
						if (x + i >= 0
							&& x + i < GridManager.grid.GetLength(0)
							&& y + j >= 0
							&& y + j < GridManager.grid.GetLength(1)) {

							// This is where the tile is checked against purchasing rules
							if (GridManager.grid[x + i, y + j].ownerId == 0 // Is the tile unowned?
								&& !GridManager.grid[x + i, y + j].bankrupt // Is the tile not bankrupt?
								&& !validCards.Contains(new Coordinate2((x + i), (y + j)))) {

								validCards.Add(new Coordinate2((x + i), (y + j)));

							}

						}
					}
				}
			}

		} else {
			int x = tileBeingPurchased.x;
			int y = tileBeingPurchased.y;
			// This is where the tile is checked against purchasing rules
			if (GridManager.grid[x, y].ownerId == 0 // Is the tile unowned?
				&& !GridManager.grid[x, y].bankrupt) { // Is the tile not bankrupt?

				isValid = true;
			}
		}

		if (validCards.Contains(tileBeingPurchased)) {
			isValid = true;
		}

		return isValid;

	} // GetValidTiles()

	// Overload of IsValidPurchase(), taking in two ints instead of a Coordinate2.
	public bool IsValidPurchase(int x, int y, int playerId) {
		return IsValidPurchase(new Coordinate2(x, y), playerId);
	} // IsValidPurchase()

} // GameManager class

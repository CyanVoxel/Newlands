// Manages most UI Elements and game setup tasks.

using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour {

	public CardDisplay cardDis;
	public GuiManager guiMan;
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

	private static string debugH = "<color=#FF6D00FF><b>[GameManager] </b></color>";

	[SerializeField]
	public static List<Player> players = new List<Player>(); // The player data objects

	// private static List<GameObject> playerMoneyObj = new List<GameObject>();
	// private static List<TMP_Text> playerMoneyText = new List<TMP_Text>();

	// Used for initialization
	void Start() {

		if (!hasAuthority) {
			return;
		}

		Debug.Log(debugH + "Initializing GameManager...");

		Debug.Log(debugH + "Creating Master Deck \"Vanilla\"");
		masterDeck = new MasterDeck("Vanilla");
		masterDeckMutable = new MasterDeck("Vanilla");

		Debug.Log(debugH + "Initializing Players...");
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
		guiMan.InitGuiManager();
		guiMan.CmdUpdateUI();

	} // Start()

	void Update() {

	} // Update()

	// // Creates card game objects, places them on the screen, and populates them with deck data
	// private void DisplayCard(Card card, int playerNum, int index) {

	// 	// Populate the Card prefab
	// 	GameObject gameCardPrefab = Resources.Load<GameObject>("Prefabs/GameCard");
	// 	// NOTE: As of the time of the addition of the GridManager class, this should be split off into it's own class.
	// 	string pZeroes = "0";
	// 	string iZeroes = "0";

	// 	// Creates card prefabs and places them on the screen

	// 	float xOff = index * 11 + (((width - handSize) / 2f) * 11);
	// 	float yOff = -10;

	// 	// Determines the number of zeroes to add in the object name
	// 	if (playerCount >= 10) {
	// 		pZeroes = "";
	// 	} else {
	// 		pZeroes = "0";
	// 	}
	// 	if (index >= 10) {
	// 		iZeroes = "";
	// 	} else {
	// 		iZeroes = "0";
	// 	} // zeroes calc

	// 	GameObject cardObj = (GameObject) Instantiate(gameCardPrefab, new Vector3(xOff, yOff, 40), Quaternion.identity);
	// 	cardObj.name = ("p" + pZeroes + playerNum + "_"
	// 		+ "i" + iZeroes + index + "_"
	// 		+ "GameCard");
	// 	// cardObj.name = ("GameCard_p" + playerNum + "_i"+ i);

	// 	cardObj.transform.SetParent(this.transform);
	// 	cardObj.transform.rotation = new Quaternion(0, 0, 0, 0);

	// 	players[playerNum].handUnits[index] = new GridUnit(players[playerNum].hand[index],
	// 		cardObj,
	// 		index, 0);

	// 	try {
	// 		cardObj.SendMessage("DisplayCard", card);
	// 	} catch (UnassignedReferenceException e) {
	// 		Debug.LogError(debugH + "Error: "
	// 			+ "Card error at deck index " + index + ": " + e);
	// 	}

	// } // DisplayCard()

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
				if (skipThis) {	// And the turn passed should always be skipped now
					this.turn = turnChecked; // Set the turn to one past the skipable turn
				}
			} else {
				round++; // NOTE: In the future where round are capped, this needs error detection too
				this.turn = 1;
			}

		}

	} //IncrementTurn()

	// Rollback to the previous turn (for debugging only)
	public void RollbackTurn() {

		turn--;
		if (turn == 0) {
			// If the round is 1 or more, decrement it
			if (round > 0) {
				round--;
			} // if round > 0

			turn = playerCount;
		} // if turn == 0

	} //RollbackTurn()

	// End the current round, starts at next turn 0
	public void EndRound() {

		round++;
		turn = 0;

	} //EndRound()

	// End the current phase, starts at next round and turn 0
	public void EndPhase() {

		phase++;
		round = 1;
		turn = 1;

	} //EndPhase()

	public void HighlightNeighbors(int turn) {

		int oldTurn = this.turn;
		int id = (turn - 1);

		// if (turn != oldTurn) {
		WipeSelectionColors("LandTile", ColorPalette.tintCard);
		// }

		// Search through the grid
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (GridManager.grid[x, y].ownerId == players[id].Id) {
					Highlight(x, y);
				} // if Tile is owned by the player
			} // for y
		} // for x

		// Local function that recolors unowned neighbor tiles
		void Highlight(int gridX, int gridY) {

			Color playerColor = GetPlayerColor(turn, 200);
			bool[, ] highlighted = new bool[width, height];
			// needToSkip = true;

			// Find each unowned neighbor tiles
			for (int i = -1; i <= 1; i++) {
				for (int j = -1; j <= 1; j++) {
					if (gridX + i >= 0
						&& gridX + i < width
						&& gridY + j >= 0
						&& gridY + j < height) {

						if (GridManager.grid[gridX + i, gridY + j].ownerId == 0
							&& !GridManager.grid[gridX + i, gridY + j].bankrupt) {

							GameObject temp = FindCard("Tile", (gridX + i), (gridY + j));
							temp.GetComponentsInChildren<Renderer>() [0].material.color = playerColor;
							temp.GetComponentsInChildren<Renderer>() [1].material.color = playerColor;
						} // if the Tile is unowned
					} // if grid in bounds
				} // for j
			} // for i
		} // Highlight()

	} // HighlightNeighbors()

	// Verifies that the Tile highlighting used in Phase 1 is correct.
	// NOTE: This function will skip a players turn if they are unable to buy any more tiles.
	// Returns TRUE if it needs to go through a recursive iteration.
	public bool VerifyHighlight() {

		int id = (turn - 1);

		bool[, ] highlighted = new bool[width, height];
		int highlightCount = 0;

		if (round > graceRounds && phase == 1) {

			// Search through the grid
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					// Debug.Log("[VerifyHighlight] Turn " + turn + ", Player ID (Turn) " + id + ", (Real) " + players[id].Id);
					if (GridManager.grid[x, y].ownerId == players[id].Id) {
						Highlight(x, y);
					} // if player owns Tile
				} // for y
			} // for x
			// Debug.Log("[VerifyHighlight] Highlights: " + highlightCount);

			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (highlighted[x, y]) {
						highlightCount++;
						// Debug.Log("Highlighted:" + x + ", " + y);
					} //if grid location was highlighted
				} // for y
			} // for x

			// Debug.Log("Highlight Count:" + highlightCount);
			if (highlightCount == 0) {
				// Debug.Log("[VerifyHighlight] Turn " + turn + ", advancing...");
				IncrementTurn(turnChecked: this.turn, skipThis: true);
				// Debug.Log("[VerifyHighlight] Turn " + turn);

				int gridSpaceLeft = 0;

				// Test to see if the grid is full
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						if (GridManager.grid[x, y].ownerId == 0
							&& !GridManager.grid[x, y].bankrupt) {
							gridSpaceLeft++;
							// Debug.Log("Grid must NOT be full!");
						} // if Tile is unowned
					} // for y
				} // for x
				// Debug.Log("Grid Space Left: " + gridSpaceLeft);

				// If the grid is full, end the phase.
				// Else, continue recursively checking if this turn should be skipped
				if (gridSpaceLeft == 0) {
					// Debug.Log("Grid was full!");
					EndPhase();
					return false;
				} else {
					// return true;
					VerifyHighlight();
					// Debug.Log("[VerifyHighlight2] Turn "
					// 	+ turn + ", Player ID (Turn) " + id
					// 	+ ", (Real) " + players[id].Id);
				} // if-else
			} // if nothing was highlighted
		} else {
			return false;
		} // If the round was greater than 1 during phase 1

		return false;

		// Local function that recolors unowned neighbor tiles.
		void Highlight(int gridX, int gridY) {

			// Find each unowned neighbor tiles
			for (int i = -1; i <= 1; i++) {
				for (int j = -1; j <= 1; j++) {
					if (gridX + i >= 0
						&& gridX + i < width
						&& gridY + j >= 0
						&& gridY + j < height) {
						if (GridManager.grid[gridX + i, gridY + j].ownerId == 0
							&& !GridManager.grid[gridX + i, gridY + j].bankrupt) {

							highlighted[gridX + i, gridY + j] = true;
						} // if Tile is unowned
					} // if grid in bounds
				} // for j
			} // for i
		} // Highlight()
	} // GetHighlightCount()

	// Attempts to buy an unowned tile. Returns true if successful, or false if already owned.
	public bool BuyTile(int gridX, int gridY) {

		int id = (turn - 1);

		// TODO: When money is implemented, factor that into the buying process.
		//	It would also be nice to have a purchase conformation message

		bool followsRules = false; // Assume rules are not being followed

		// First check is the first round is finished
		if (round > graceRounds) {
			// Search through the grid
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (GridManager.grid[x, y].ownerId == players[id].Id) {
						ValidTile(x, y);
					} // if player owns tile
				} // for y
			} // for x
		} else {
			followsRules = true;
		} // if past first round

		// If the tile is unowned (Had owner ID of 0), assign this owner to it
		if (GridManager.grid[gridX, gridY].ownerId == 0
			&& followsRules && !GridManager.grid[gridX, gridY].bankrupt) {
			// players[id].ModifyMoney(-100);
			// players[id].CalcMoney();	//NOTE: Only need this OR ModifyMoney()
			players[id].CalcTotalMoney();
			GridManager.grid[gridX, gridY].ownerId = players[id].Id;
			// Debug.Log("Turn " + turn + ", buying for Player " + players[id].Id);
			return true;
		} else if (!followsRules) {
			Debug.Log(debugH + "You can't buy this tile!");
			return false;
		} else {
			Debug.Log(debugH + "This Tile is already owned!");
			return false;
		}

		// Local function that recolors unowned neighbor tiles
		void ValidTile(int gridLocX, int gridLocY) {

			// Find each unowned neighbor tiles
			for (int i = -1; i <= 1; i++) {
				for (int j = -1; j <= 1; j++) {
					if (gridLocX + i >= 0
						&& gridLocX + i < width
						&& gridLocY + j >= 0
						&& gridLocY + j < height) {

						if (GridManager.grid[gridLocX + i, gridLocY + j].ownerId == 0
							&& GridManager.grid[gridLocX + i, gridLocY + j] == GridManager.grid[gridX, gridY]
							&& !GridManager.grid[gridX, gridY].bankrupt) {
							followsRules = true; // Signal that this is a valid purchase
						} // if tile owned and in a valid spot
					} // if grid in bounds
				} // for j
			} // for i
		} // ValidTile()
	} // BuyTile()

	// Turns all Land Tiles on the grid white.
	public void WipeSelectionColors(string cardType, Color32 color) {

		if (cardType == "LandTile") {
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (GridManager.grid[x, y].ownerId == 0
						&& !GridManager.grid[x, y].bankrupt) {
						GameObject temp = FindCard("Tile", x, y);
						temp.GetComponentsInChildren<Renderer>() [0].material.color = color;
						temp.GetComponentsInChildren<Renderer>() [1].material.color = color;
					} //if tile unowned
				} // for height
			} // for width
		} else if (cardType == "GameCard") {
			for (int i = 0; i < handSize; i++) {
				if (FindCard("GameCard", localPlayerId, i)) {
					// GameObject temp = transform.Find("GameCard_p0_i" + i).gameObject;
					GameObject temp = FindCard("GameCard", localPlayerId, i);
					float x = temp.transform.position.x;
					float y = temp.transform.position.y;
					temp.transform.position = new Vector3(x, y, 40);
					temp.GetComponentsInChildren<Renderer>() [0].material.color = color;
					temp.GetComponentsInChildren<Renderer>() [1].material.color = color;
				} else {
					// Debug.LogWarning("[GameManager] Warning: Could not find a GameCard selection to wipe!");
				} // if the card could be found

			} // for handSize
		} else if (cardType == "MarketCard") {
			int marketWidth = Mathf.CeilToInt((float) GameManager.masterDeck.marketCardDeck.Count()
				/ (float) height);
			for (int x = 0; x < marketWidth; x++) {
				for (int y = 0; y < height; y++) {
					GameObject temp = FindCard("MarketCard", x, y);
					float posX = temp.transform.position.x;
					float posY = temp.transform.position.y;
					temp.transform.position = new Vector3(posX, posY, 40);
					temp.GetComponentsInChildren<Renderer>() [0].material.color = color;
					temp.GetComponentsInChildren<Renderer>() [1].material.color = color;
				} // for height
			} // for width
		} // if

	} // WipeSelectionColors()

	// Returns the color associated with a player ID.
	// Strength paramater refers to a possible brighter color variant.
	public Color GetPlayerColor(int playerID, int strength = 500) {

		Color color = ColorPalette.tintCard;

		switch (playerID) {

			case 1:
				if (strength == 500) {
					color = ColorPalette.LightBlue500;
				} else if (strength == 400) {
					color = ColorPalette.LightBlue400;
				} else if (strength == 300) {
					color = ColorPalette.LightBlue300;
				} else if (strength == 200) {
					color = ColorPalette.LightBlue200;
				}
				break;
			case 2:
				if (strength == 500) {
					color = ColorPalette.Red500;
				} else if (strength == 400) {
					color = ColorPalette.Red400;
				} else if (strength == 300) {
					color = ColorPalette.Red300;
				} else if (strength == 200) {
					color = ColorPalette.Red300;
				}
				break;
			case 3:
				if (strength == 500) {
					color = ColorPalette.Purple500;
				} else if (strength == 400) {
					color = ColorPalette.Purple400;
				} else if (strength == 300) {
					color = ColorPalette.Purple300;
				} else if (strength == 200) {
					color = ColorPalette.Purple200;
				}
				break;
			case 4:
				if (strength == 500) {
					color = ColorPalette.Amber500;
				} else if (strength == 400) {
					color = ColorPalette.Amber400;
				} else if (strength == 300) {
					color = ColorPalette.Amber300;
				} else if (strength == 200) {
					color = ColorPalette.Amber200;
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
			guiMan.CmdUpdateUI();

			Vector3 oldCardPosition = gameCard.tileObj.transform.position;
			int oldCardIndex = gameCard.x;
			Debug.Log(debugH + "Old Card Index: " + oldCardIndex);

			if (gridTile.bankrupt) {
				BankruptTile(gridTile);
				UpdatePlayersInfo();
				guiMan.CmdUpdateUI();
				Debug.Log(debugH + "Tile bankrupt! has value of " + gridTile.totalValue);
			}

			if (gameCard.stackable) {

				gridTile.stackSize++;
				gridTile.cardStack.Add(gameCard.card);
				gridTile.CalcTotalValue(); // This fixes Market Cards not calculating first time
				UpdatePlayersInfo();
				guiMan.CmdUpdateUI();

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
				guiMan.CmdUpdateUI();
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
			WipeSelectionColors("Game Card", ColorPalette.tintCard);
			return true;

		} else {
			Debug.Log(debugH + "Card move is illegal!");
			MouseManager.selection = -1;
			WipeSelectionColors("Game Card", ColorPalette.tintCard);
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

		Debug.Log(debugH + "Bankrupting tile!");
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

		bool highlightAllowed = false;
		if (round > GameManager.graceRounds) {
			highlightAllowed = true;
		}
		Debug.Log(debugH + "Round: " + round + "/" + GameManager.graceRounds + " | " + highlightAllowed);
		Debug.Log(debugH + "Turn: " + newTurn);

		if (round > graceRounds) {
			VerifyHighlight();
			if (!MouseManager.highlightFlag) {
				Debug.Log(debugH + "Highlighting...");
				HighlightNeighbors(newTurn);
				MouseManager.highlightFlag = true;
			}
		} // if
	}

} // GameManager class

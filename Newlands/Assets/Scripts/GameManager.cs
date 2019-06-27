// Manages most UI Elements and game setup tasks.
// TODO: Will probably want to move all of the UI stuff to a dedicated class.

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {

	// VARIABLES ##################################################################################

	public static MasterDeck masterDeck;
	public static MasterDeck masterDeckMutable;
	public static readonly byte playerCount = 4; 	// Number of players in the match
	public static byte phase = 1;					// The current phase of the game
	public static int round = 1;					// The current round of turns
	public static byte turn = 1;					// The current turn in the round
	public static int graceRounds = 1;				// The # of rounds without neighbor rules

	public static readonly byte width = 3;			// Width of the game grid in cards
	public static readonly byte height = 3;			// Height of the game grid in cards
	public static readonly byte handSize = 9;		// How many cards the player is dealt

	public static List<Player> players = new List<Player>();	// The player data objects

	// Grid specific
	public static GridUnit[,] grid;					// The internal grid, made up of GridUnits
	public static float[] rowPos;					// Row position
	private static byte[] maxStack;
	private static float cardThickness = 0.2f;
	private static float shiftUnit = 1.2f;
	private static float cardOffX = 11f;
	private static float cardOffY = 8f;
	
	public static Camera mainCam;
	private GameObject landTilePrefab;
	// private Card card;

	// UI ELEMENTS ################################################################################

	private static GameObject phaseNumberObj;
	private static GameObject roundNumberObj;
	private static GameObject turnNumberObj;
	private static TMP_Text phaseNumberText;
	private static TMP_Text roundNumberText;
	private static TMP_Text turnNumberText;

	// private static List<GameObject> playerMoneyObj = new List<GameObject>();
	// private static List<TMP_Text> playerMoneyText = new List<TMP_Text>();

	// Used for initialization
	void Start() {

		// DECKS ##############################################################

		masterDeck = new MasterDeck("Vanilla");
		masterDeckMutable = new MasterDeck("Vanilla");

		// PLAYERS ############################################################

		// Initializes each player object and draws a hand for them
		for (byte i = 0; i < playerCount; i++) {
			players.Add(new Player());
			players[i].Id = ((byte)(i + 1));
			players[i].hand = DrawHand(handSize);

		} // for playerCount

		// Displays those hands on screen
		// NOTE: Change 1 to playerCount in order to view all players' cards.
		for (byte i = 0; i < 1; i++) {
			DisplayHand(deck: players[i].hand, playerNum: i);
		} // for number of hands

		// UI #################################################################

		// Grab the UI elements
		phaseNumberObj = transform.Find("UI/PhaseNumber").gameObject;
		roundNumberObj = transform.Find("UI/RoundNumber").gameObject;
		turnNumberObj = transform.Find("UI/TurnNumber").gameObject;
		// Pick out the appropriate elements from the GameObjects that were grabbed
		phaseNumberText = phaseNumberObj.GetComponent<TMP_Text>();
		roundNumberText = roundNumberObj.GetComponent<TMP_Text>();
		turnNumberText = turnNumberObj.GetComponent<TMP_Text>();

		// Grabbing Money Objects/Text
		for (byte i = 0; i < playerCount; i++) {
			// playerMoneyObj.Add(new GameObject());
			// playerMoneyText.Add(new TMP_Text());
			// Debug.Log("UI/Money/Player (" + (i + 1) + ")");
			players[i].moneyObj = transform.Find("UI/Money/Player (" + (i + 1) + ")").gameObject;
			players[i].moneyText = players[i].moneyObj.GetComponent<TMP_Text>();
			players[i].moneyText.color = GetPlayerColor((byte)(i + 1), 500);
		} // for playerCount

		// GRID ###############################################################
		
		// Initialize the internal grid
		grid = new GridUnit[width, height];
		rowPos = new float[height];
		maxStack = new byte[height];

		// Create tile GameObjects and connect them to internal grid
		PopulateGrid();	
		// ShiftRow(row: 4, units: 2);

		// FINAL ##############################################################

		// Make sure that there is at least 1 Grace Round
		if (graceRounds < 1) {
			graceRounds = 1;
		} // if (graceRounds < 1)

		GameObject cameraObj = transform.Find("Main Camera").gameObject;
		mainCam = cameraObj.GetComponent<Camera>();

		// Push the first UI Update
		UpdateUI();

	} // Start()

	void Update() {

		// Scroll in and out
		Vector3 cameraPos = mainCam.transform.position;
		cameraPos.z += (Input.mouseScrollDelta.y * 3f);
		mainCam.transform.position = cameraPos;

	} // Update()

	private void PopulateGrid() {
		// Populate the Card prefab and create the Master Deck
		landTilePrefab = Resources.Load<GameObject>("Prefabs/Tile");
		string xZeroes = "0";
		string yZeroes = "0";
		//masterDeckMutable = masterDeck;	// Sets mutable deck version to internal one
		
		for (byte x = 0; x < width; x++) {
			for (byte y = 0; y < height; y++) {
				// Draw a card from the Land Tile deck
				Card card = Card.CreateInstance<Card>();
				card = DrawCard(masterDeckMutable.landTileDeck, masterDeck.landTileDeck);

				float xOff = x * cardOffX;
				float yOff = y * cardOffY;

				// Determines the number of zeroes to add in the object name
				if (x >= 10) {
					xZeroes = "";
				} else {
					xZeroes = "0";
				}
				if (y >= 10) {
					yZeroes = "";
				} else {
					yZeroes = "0";
				} // zeroes calc

				GameObject cardObj = (GameObject)Instantiate(this.landTilePrefab, new Vector3(xOff, yOff, 50), Quaternion.identity);
				cardObj.name = ("x" + xZeroes + x + "_" +
								"y" + yZeroes + y + "_" +
								"Tile");
				// cardObj.name = ("LandTile_x" + x + "_y" + y + "_z0");

				cardObj.transform.SetParent(this.transform);
				cardObj.transform.rotation = new Quaternion(0, 180, 0, 0);	// 0, 180, 0, 0

				// Connect thr drawn card to the internal grid
				grid[x, y] = new GridUnit(card: card, tileObj: cardObj, x: x, y: y);
				rowPos[y] = cardObj.transform.position.y;	// Row position
				// Set Tile's value based on its Resource
				grid[x, y].AssignTileValue(tile: card);

				// Connect the drawn card to the prefab that was just created
				cardObj.SendMessage("DisplayCard", card);
			} // y
		} // x

	} //PopulateGrid();

	// Draws random GameCards from the masterDeck and returns a deck of a specified size
	private Deck DrawHand(int handSize) {

		Deck deck = new Deck();		// The deck of drawn cards to return

		for (int i = 0; i < handSize; i++) {
			// Draw a card from the deck provided and add it to the deck to return.
			// NOTE: In the future, masterDeckMutable might need to be checked for cards
			// 	before preceding.
			deck.Add(DrawCard(masterDeckMutable.gameCardDeck, masterDeck.gameCardDeck));

		} // for

		return deck;

	} // DrawHand()

	// Creates card game objects, places them on the screen, and populates them with deck data
	private void DisplayHand(Deck deck, byte playerNum) {

		// Populate the Card prefab
		landTilePrefab = Resources.Load<GameObject>("Prefabs/GameCard");
		string pZeroes = "0";
		string iZeroes = "0";

		// Creates card prefabs and places them on the screen
		for (int i = 0; i < deck.Count(); i++) {

			float xOff = i * 11 + (((width - handSize) / 2f) * 11);
			float yOff = -10;

			// Determines the number of zeroes to add in the object name
				if (playerCount >= 10) {
					pZeroes = "";
				} else {
					pZeroes = "0";
				}
				if (i >= 10) {
					iZeroes = "";
				} else {
					iZeroes = "0";
				} // zeroes calc

			GameObject cardObj = (GameObject)Instantiate(this.landTilePrefab, new Vector3(xOff, yOff, 40), Quaternion.identity);
			cardObj.name = ("p" + pZeroes + playerNum + "_" +
								"i" + iZeroes + i + "_" +
								"GameCard");
			// cardObj.name = ("GameCard_p" + playerNum + "_i"+ i);

			cardObj.transform.SetParent(this.transform);
			cardObj.transform.rotation = new Quaternion(0, 0, 0, 0);

			players[playerNum].handUnits[i] = new GridUnit(players[playerNum].hand[i], 
				cardObj, 
				(byte)i, 0);

			try {
				cardObj.SendMessage("DisplayCard", deck[i]);
			}

			catch (UnassignedReferenceException e) {
				Debug.LogError("<b>[GameManager]</b> Error: " +
				"Card error at deck index " + i + ": " + e);
			}
			

		} // for

	} // DisplayHand()


	public Card DrawCard(Deck deckMut, Deck deckPerm) {

		Card card;	// Card to return
		int cardsLeft = deckMut.Count();	// Number of cards left from mutable deck
		int cardsTotal = deckPerm.Count();	// Number of cards total from permanent deck

		// Draws a card from the mutable deck, then removes that card from the deck.
		// If all cards are drawn, draw randomly from the immutable deck.
		if (cardsLeft > 0 ) {
			card = deckMut[Random.Range(0, cardsLeft)];
			deckMut.Remove(card);
			// Debug.Log("<b>[GameManager]</b> " + 
			// 	cardsLeft + 
			// 	" of " + 
			// 	cardsTotal + 
			// 	" cards left");
		} else {
			card = deckPerm[Random.Range(0, cardsTotal)];
			Debug.LogWarning("<b>[GameManager]</b> Warning: " +
			 "All cards (" + cardsTotal + ") were drawn from a deck! " +
			 " Now drawing from immutable deck...");
		}

		return card;

	} // DrawCard()

	// Advance to the next turn
	public void AdvanceTurn() {

		turn++;
		if (turn > playerCount) {
			round++;
			turn = 1;
		}

	} //AdvanceTurn()

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

	public void HighlightNeighbors() {

		byte id = (byte)(turn - 1);

		WipeSelectionColors("LandTile");

		// Search through the grid
		for (byte x = 0; x < width; x++) {
			for (byte y = 0; y < height; y++) {
				if (grid[x, y].ownerId == players[id].Id) {
					Highlight(x, y);
				} // if Tile is owned by the player
			} // for y
		} // for x

		// Local function that recolors unowned neighbor tiles
		void Highlight(byte gridX, byte gridY) {

			Color playerColor = GetPlayerColor(turn, 200);
			bool[,] highlighted = new bool[width, height];
			// needToSkip = true;

			// Find each unowned neighbor tiles
			for (int i = -1; i <= 1; i++) {
				for (int j = -1; j <= 1; j++) {
					if (gridX + i >= 0 && 
						gridX + i < width && 
						gridY + j >= 0 && 
						gridY + j < height) {

						if (grid[gridX + i, gridY + j].ownerId == 0) {
							GameObject temp = FindCard("Tile", (byte)(gridX + i), (byte)(gridY + j));
							temp.GetComponentsInChildren<Renderer>()[0].material.color = playerColor;
							temp.GetComponentsInChildren<Renderer>()[1].material.color = playerColor;
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

		byte id = (byte)(turn - 1);

		bool[,] highlighted = new bool[width, height];
		int highlightCount = 0;

		if (round > graceRounds && phase == 1) {

			// Search through the grid
			for (byte x = 0; x < width; x++) {
				for (byte y = 0; y < height; y++) {
					// Debug.Log("[VerifyHighlight] Turn " + turn + ", Player ID (Turn) " + id + ", (Real) " + players[id].Id);
					if (grid[x, y].ownerId == players[id].Id) {
						Highlight(x, y);
					} // if player owns Tile
				} // for y
			} // for x
			// Debug.Log("[VerifyHighlight] Highlights: " + highlightCount);

			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (highlighted[x,y]) {
						highlightCount++;
						// Debug.Log("Highlighted:" + x + ", " + y);
					} //if grid location was highlighted
				} // for y
			} // for x

			// Debug.Log("Highlight Count:" + highlightCount);
			if (highlightCount == 0) {
				// Debug.Log("[VerifyHighlight] Turn " + turn + ", advancing...");
				AdvanceTurn();
				// Debug.Log("[VerifyHighlight] Turn " + turn);

				int gridSpaceLeft = 0;

				// Test to see if the grid is full
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						if (grid[x,y].ownerId == 0) {
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
					// Debug.Log("[VerifyHighlight2] Turn " + turn + ", Player ID (Turn) " + id + ", (Real) " + players[id].Id);
				} // if-else
			} // if nothing was highlighted
		} else {
			return false;
		} // If the round was greater than 1 during phase 1

		return false;

		// Local function that recolors unowned neighbor tiles.
		void Highlight(byte gridX, byte gridY) {

			// Find each unowned neighbor tiles
			for (int i = -1; i <= 1; i++) {
				for (int j = -1; j <= 1; j++) {
					if (gridX + i >= 0 && 
						gridX + i < width && 
						gridY + j >= 0 && 
						gridY + j < height) {
						if (grid[gridX + i, gridY + j].ownerId == 0) {
							highlighted[gridX + i, gridY + j] = true;	
						} // if Tile is unowned
					} // if grid in bounds
				} // for j
			} // for i
		} // Highlight()
	} // GetHighlightCount()

	// Attempts to buy an unowned tile. Returns true if successful, or false if already owned.
	public bool BuyTile(byte gridX, byte gridY) {

		byte id = (byte)(turn - 1);

		// TODO: When money is implemented, factor that into the buying process.
		//	It would also be nice to have a purchase conformation message

		bool followsRules = false;	// Assume rules are not being followed

		// First check is the first round is finished
		if (round > graceRounds) {
			// Search through the grid
			for (byte x = 0; x < width; x++) {
				for (byte y = 0; y < height; y++) {
					if (grid[x, y].ownerId == players[id].Id) {
						ValidTile(x, y);
					} // if player owns tile
				} // for y
			} // for x
		} else {
			followsRules = true;
		} // if past first round

		// If the tile is unowned (Had owner ID of 0), assign this owner to it
		if (grid[gridX, gridY].ownerId == 0 && followsRules) {
			// players[id].ModifyMoney(-100);
			players[id].CalcMoney();	//NOTE: Only need this OR ModifyMoney()
			grid[gridX, gridY].ownerId = players[id].Id;
			// Debug.Log("Turn " + turn + ", buying for Player " + players[id].Id);
			return true;
		} else if (!followsRules) {
			Debug.Log("<b>[GameManager]</b> " +
						"You can't buy this tile, it's too far away!");
			return false;
		} else {
			Debug.Log("<b>[GameManager]</b> " +
						"This Tile is already owned!");
			return false;
		}

		// Local function that recolors unowned neighbor tiles
		void ValidTile(byte gridLocX, byte gridLocY) {

			// Find each unowned neighbor tiles
			for (int i = -1; i <= 1; i++) {
				for (int j = -1; j <= 1; j++) {
					if (gridLocX + i >= 0 && 
						gridLocX + i < width && 
						gridLocY + j >= 0 && 
						gridLocY + j < height) {

						if (grid[gridLocX + i, gridLocY + j].ownerId == 0 &&
							grid[gridLocX + i, gridLocY + j] == grid[gridX, gridY]) {
							followsRules = true;	// Signal that this is a valid purchase
						} // if tile owned and in a valid spot
					} // if grid in bounds
				} // for j
			} // for i
		} // ValidTile()
	} // BuyTile()

	// Turns all Land Tiles on the grid white.
	public void WipeSelectionColors(string cardType) {

		if (cardType == "LandTile") {
			for (byte x = 0; x < width; x++) {
				for (byte y = 0; y < height; y++) {
					if (grid[x, y].ownerId == 0) {

						GameObject temp = FindCard("Tile", (byte)x, (byte)y);

						temp.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintCard;
						temp.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintCard;
					} //if tile unowned
				} // for height
			} // for width
		} else if (cardType == "GameCard") {
			for (byte i = 0; i < handSize; i++) {
				if (FindCard("GameCard", 0, i)) {
					// GameObject temp = transform.Find("GameCard_p0_i" + i).gameObject;
					GameObject temp = FindCard("GameCard", 0, i);
					float x = temp.transform.position.x;
					float y = temp.transform.position.y;
					temp.transform.position = new Vector3(x, y, 40);
					temp.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintCard;
					temp.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintCard;
				} else {
					// Debug.LogWarning("[GameManager] Warning: Could not find a GameCard selection to wipe!");
				} // if the card could be found
				
			} // for handSize
		} // if

	} // WipeSelectionColors()

	// Turns all Land Tiles on the grid a specified color. (Overload)
	public void WipeSelectionColors(string cardType, Color32 color) {

		for (byte x = 0; x < width; x++) {
			for (byte y = 0; y < height; y++) {
				if (grid[x, y].ownerId == 0) {
					GameObject temp = FindCard("Tile", x, y);
					temp.GetComponentsInChildren<Renderer>()[0].material.color = color;
					temp.GetComponentsInChildren<Renderer>()[1].material.color = color;
				}
			}
		}

	} // WipeSelectionColors(color)

	// Returns the color associated with a player ID.
	// Strength paramater refers to a possible brighter color variant.
	public Color GetPlayerColor(byte playerID, int strength = 500) {

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

	// Shifts rows of cards up or down. Used to give room for cards under tiles.
	public void ShiftRow(byte row, int units) {

		for (byte x = 0; x < width; x++) {
			for (byte y = row; y < height; y++) {
				float oldX = grid[x, y].tile.transform.position.x;
				float oldY = grid[x, y].tile.transform.position.y;
				float oldZ = grid[x, y].tile.transform.position.z;
				grid[x, y].tile.transform.position = new Vector3
					(grid[x, y].tile.transform.position.x,
					(oldY += (shiftUnit * units)),
					grid[x, y].tile.transform.position.z);
			} // for y
		} // for x

	} // ShiftRow()

	// Updates the basic UI elements 
	public void UpdateUI() {

		GameManager.phaseNumberText.text = ("Phase " + GameManager.phase);
		GameManager.roundNumberText.text = ("Round " + GameManager.round);
		GameManager.turnNumberText.text = ("Player " + GameManager.turn + "'s Turn");

		// Tacks on "Grace Period" text if the round is a grace round
		if (phase == 1 && round <= graceRounds) {
			GameManager.roundNumberText.text += (" (Grace Period)");
		} // if

		switch (turn) {
			case 1:
				GameManager.turnNumberText.color = ColorPalette.lightBlue500;
				break;
			case 2:
				GameManager.turnNumberText.color = ColorPalette.red500;
				break;
			case 3:
				GameManager.turnNumberText.color = ColorPalette.purple500;
				break;
			case 4:
				GameManager.turnNumberText.color = ColorPalette.amber500;
				break;
			default:
				break;
		} // switch


		// Things that need to be updated for all players go here
		for (byte i = 0; i < (byte)players.Count; i++) {
			// Debug.Log(moneyText[i].text);
			players[i].CalcMoney();
			players[i].moneyText.text = "Player " + (i + 1) + ": $" + players[i].Money; 
		} // for array length

	} // UpdateUI()

	// Checks if a GameCard is allowed to be played on a Tile.
	public bool CheckRules(GridUnit gridTile, GridUnit gameCard) {

		bool categoryMatch = false;
		bool scopeMatch = false;

		Debug.Log("[GameManager]\n" +
				"Game Card Target Cat   : " + gameCard.targetCat + "\n" +
				"Game Card Target Scope : " + gameCard.targetScope + "\n" +
				"Land Tile Category     : " + gridTile.tileCat + "\n" +
				"Land Tile Scope        : " + gridTile.tileScope);

		// Debug.Log("[GameManager] Checking if " + gameCard.targetCat +
		// 				" can be played on " + gridTile.tileCat);


		// Card Target Category
		switch (gameCard.targetCat) {
			case "Any":
				categoryMatch = true;
				break;
			case "Tile":
				if (gridTile.tileCat == "Tile" ||
					gridTile.tileCat == "Land Tile" ||
					gridTile.tileCat == "Coast Tile") {
					categoryMatch = true;
				}
				break;
			case "Land Tile":
				if (gridTile.tileCat == "Tile") {
					categoryMatch = true;
				}
				break;
			case "Coast Tile":
				if (gridTile.tileCat == "Tile") {
					categoryMatch = true;
				}
				break;
			default:
				if (gameCard.targetCat == gridTile.tileCat) {
					categoryMatch = true;
				}
				break;
		} // Card Target Category

		// If the category matched
		if (categoryMatch) {

			// Card Target Scope
			switch (gameCard.targetScope) {
				case "Any":
					scopeMatch = true;
					break;
				case "Land Tile":
					if (gridTile.tileScope == "Forest" ||
						gridTile.tileScope == "Plains" ||
						gridTile.tileScope == "Quarry" ||
						gridTile.tileScope == "Farmland") {
						scopeMatch = true;
					}
					break;
				case "Coast Tile":
					if (gridTile.tileScope == "Forest" ||
						gridTile.tileScope == "Plains" ||
						gridTile.tileScope == "Quarry" ||
						gridTile.tileScope == "Farmland") {
						scopeMatch = true;
					}
					break;
				default:
					if (gameCard.targetScope == gridTile.tileScope) {
						scopeMatch = true;
					}
					break;
			} // Card Target Scope

		} // If the category matched

		// If the Category AND Scope match
		if (categoryMatch && scopeMatch) {

			ShiftRow(gridTile.y, 1);
			maxStack[gridTile.y]++;
			WipeSelectionColors("Game Card");

			gameCard.tile.transform.position = new Vector3
				(gridTile.tile.transform.position.x,
				gridTile.tile.transform.position.y - (shiftUnit * maxStack[gridTile.y]),
				(gridTile.tile.transform.position.z) + (cardThickness * maxStack[gridTile.y]));

			// players[turn].hand.Remove(players[turn].hand[gameCard.x]);
			gameCard.tile.transform.parent = gridTile.tile.transform;
			// gridTile.cardStack.Push(gameCard.tile);
			// gameCard.tile.name = ("Card_i" + 0);
			// CanvasGroup canvasGroup = gameCard.tile.GetComponentsInChildren<CanvasGroup>()[0];
			// canvasGroup.blocksRaycasts = false;

			// TODO: Name the card with the local child index included
			gameCard.tile.name = ("p00_" +
								  "i" + "00" + "_" +
								  "GameCard");

			players[turn].hand.Add(DrawCard(masterDeckMutable.gameCardDeck, 
											masterDeck.gameCardDeck));

			landTilePrefab = Resources.Load<GameObject>("Prefabs/GameCard");

			// gridTile.cardStack.Add(gameCard.tile);
			MouseManager.selection = -1;
			
			return true;

		} else {
			Debug.Log("[GameManager] You can't play a " +
						gameCard.targetScope +
						" card on a "  + 
						gridTile.tileScope + "!");
			MouseManager.selection = -1;
			WipeSelectionColors("Game Card");
			return false;
		} // If the Category AND Scope match


	} // CheckRules

	public GameObject FindCard(string type, byte x, byte y) {

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
			strX = "p";		// Instead of x, use p for PlayerID
			strY = "i";		// Instead of y, use i for Index
		} // if GameCard


		if (transform.Find(strX + xZeroes + x + "_" + strY + yZeroes + y + "_" + type)) {
			// GameObject gameObject = new GameObject();
			GameObject gameObject = transform.Find(strX + xZeroes + x + "_" +
										strY + yZeroes + y + "_" +
										type).gameObject;
			return gameObject;
		} else {
			// Debug.LogError("[GameManager] Error: Could not find GameObject!");	
			return null;
		}


	} // FindCard()
	
} // GameManager class

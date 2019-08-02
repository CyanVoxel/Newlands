using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class GridManager : NetworkBehaviour {

	// [SerializeField]
	private CardDisplay cardDis;

	public static GridUnit[, ] grid; // The internal grid, made up of GridUnits
	public static GridUnit[, ] marketGrid;
	public static float[] rowPos; // Row position
	public static int[] maxStack;
	public static int[] maxMarketStack;
	public static readonly float cardThickness = 0.2f;
	public static readonly float shiftUnit = 1.2f;
	public static readonly float cardOffX = 11f;
	public static readonly float cardOffY = 8f;

	public GameObject landTilePrefab;
	public GameObject gameCardPrefab;
	public GameObject marketCardPrefab;

	private static DebugTag debug = new DebugTag("GridManager", "FFAB00");

	void Start() {
		cardDis = FindObjectOfType<CardDisplay>();
		CreateGameGridObjects();
		CreateMarketGridObjects();
		// PreInitGameGrid();
		// InitGameGrid();
	}

	// Creates the GameObjects used in the Game Grid
	public void CreateGameGridObjects() {

		if (!hasAuthority) {
			return;
		}

		// Debug.Log("[GridManager] Creating Game Grid Objects...");

		string xZeroes = "0";
		string yZeroes = "0";

		for (int x = 0; x < grid.GetLength(0); x++) {
			for (int y = 0; y < grid.GetLength(1); y++) {

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

				GameObject cardObj = Instantiate(landTilePrefab,
					new Vector3(xOff, yOff, 50),
					Quaternion.identity);

				// Debug.Log("[GridManager] Trying to set parent of " + cardObj + " to " + this);
				// cardObj.transform.SetParent(this.transform);
				cardObj.transform.rotation = new Quaternion(0, 180, 0, 0); // 0, 180, 0, 0

				// Debug.Log("[GridManager] Spawning Card...");
				NetworkServer.Spawn(cardObj);

				// Grabs data from the internal grid and pushes it to the CardState scripts,
				// triggering them to update their visuals.
				// Debug.Log("[GridManager] Trying to fill out Tile info...");
				CardState cardState = cardObj.GetComponent<CardState>();

				if (cardState != null) {
					// Generate and Push the string of the object's name
					cardState.objectName = ("x" + xZeroes + x + "_"
						+ "y" + yZeroes + y + "_"
						+ "Tile");

					// Push new values to the CardState to be synchronized across the network
					cardState.category = grid[x, y].card.category;
					cardState.title = grid[x, y].card.title;
					cardState.subtitle = grid[x, y].card.subtitle;
					cardState.bodyText = grid[x, y].card.bodyText;
					cardState.footerText = grid[x, y].card.footerText;
					cardState.resource = grid[x, y].card.resource;
					cardState.footerValue = grid[x, y].card.footerValue;
					cardState.target = grid[x, y].card.target;
					cardState.resource = grid[x, y].card.resource;
					cardState.percFlag = grid[x, y].card.percFlag;
					cardState.moneyFlag = grid[x, y].card.moneyFlag;
					cardState.footerOpr = grid[x, y].card.footerOpr;
				} else {
					Debug.Log(debug.head + "This object's card state was null!");
				}

			} // y

		} // x

	} // CreateGameGridObjects();

	// Creates the GameObjects used in the Market Grid
	public void CreateMarketGridObjects() {

		if (!hasAuthority) {
			return;
		}

		// Debug.Log("[GridManager] Creating Market Grid Objects...");

		// Populate the Card prefab and create the Master Deck
		string xZeroes = "0";
		string yZeroes = "0";
		//masterDeckMutable = m

		for (int x = 0; x < marketGrid.GetLength(0); x++) {
			for (int y = 0; y < marketGrid.GetLength(1); y++) {

				if (marketGrid[x, y] != null) {

					float xOff = ((GameManager.width + 1) * cardOffX) + x * cardOffX;
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

					GameObject cardObj = (GameObject)Instantiate(marketCardPrefab,
						new Vector3(xOff, yOff, 50),
						Quaternion.identity);

					// Debug.Log("[GridManager] Spawning Card...");
					NetworkServer.Spawn(cardObj);

					// Debug.Log("[GridManager] Trying to fill out Market Card info...");
					CardState cardState = cardObj.GetComponent<CardState>();

					if (cardState != null) {
						// Generate and Push the string of the object's name
						cardState.objectName = ("x" + xZeroes + x + "_"
							+ "y" + yZeroes + y + "_"
							+ "MarketCard");

						// Debug.Log("[GridManager] Market Grid Size: "
						// 	+ marketGrid.GetLength(0) + ", "
						// 	+ marketGrid.GetLength(1));

						// Debug.Log("[GridManager] Accessing card at [" + x + ", " + y + "]...");

						// Push new values to the CardState to be synchronized across the network
						cardState.category = marketGrid[x, y].card.category;
						cardState.title = marketGrid[x, y].card.title;
						cardState.subtitle = marketGrid[x, y].card.subtitle;
						cardState.bodyText = marketGrid[x, y].card.bodyText;
						cardState.footerText = marketGrid[x, y].card.footerText;
						cardState.resource = marketGrid[x, y].card.resource;
						cardState.footerValue = marketGrid[x, y].card.footerValue;
						cardState.target = marketGrid[x, y].card.target;
						cardState.resource = marketGrid[x, y].card.resource;
						cardState.percFlag = marketGrid[x, y].card.percFlag;
						cardState.moneyFlag = marketGrid[x, y].card.moneyFlag;
						cardState.footerOpr = marketGrid[x, y].card.footerOpr;
					} else {
						Debug.Log(debug.head + "This object's card state was null!");
					} // if (cardState != null)

				} // if (marketGrid[x, y] != null)

			} // y
		} // x

	} // CreateMarketGridObjects()

	// Creates card game objects, places them on the screen, and populates them with deck data
	public void CreateHandObjects(int playerNum, SyncListCardData hand) {

		// if (!isLocalPlayer) {
		// 	Debug.Log("[GridManager] A non-local player tried to spawn hand objects!");
		// 	return;
		// }

		// Debug.Log(debug.head + "Spawning Cards for ID " + playerNum);
		// Debug.Log(debug.head + "Hand size of: " + hand.Count);

		// Populate the Card prefab
		string pZeroes = "0";
		string iZeroes = "0";

		// Creates card prefabs and places them on the screen
		for (int i = 0; i < hand.Count; i++) {

			float xOff = i * 11 + (((GameManager.width - GameManager.handSize) / 2f) * 11);
			float yOff = -10;

			// Determines the number of zeroes to add in the object name
			if (GameManager.playerCount >= 10) {
				pZeroes = "";
			} else {
				pZeroes = "0";
			}
			if (i >= 10) {
				iZeroes = "";
			} else {
				iZeroes = "0";
			} // zeroes calc

			GameObject cardObj = (GameObject)Instantiate(gameCardPrefab,
				new Vector3(xOff, yOff, 40),
				Quaternion.identity);

			// Debug.Log("[GridManager] Spawning Card...");
			// NetworkServer.Spawn(cardObj);

			// Debug.Log("[GridManager] Trying to fill out Hand Card info...");
			CardState cardState = cardObj.GetComponent<CardState>();

			if (cardState != null) {
				// Generate and Push the string of the object's name
				cardState.objectName = ("p" + pZeroes + playerNum + "_"
					+ "i" + iZeroes + i + "_"
					+ "GameCard");

				// Debug.Log(debug.head + "Spawned CardObj: " + hand[i].title);

				// Debug.Log("[GridManager] Accessing hand card at [" + i + ", " + playerNum + "]...");

				// Push new values to the CardState to be synchronized across the network
				cardState.category = hand[i].category;
				cardState.title = hand[i].title;
				cardState.subtitle = hand[i].subtitle;
				cardState.bodyText = hand[i].bodyText;
				cardState.footerText = hand[i].footerText;
				cardState.resource = hand[i].resource;
				cardState.footerValue = hand[i].footerValue;
				cardState.target = hand[i].target;
				cardState.resource = hand[i].resource;
				cardState.percFlag = hand[i].percFlag;
				cardState.moneyFlag = hand[i].moneyFlag;
				cardState.footerOpr = hand[i].footerOpr;
				cardState.footerColor = hand[i].footerColor;
				cardState.onlyColorCorners = hand[i].onlyColorCorners;
				// cardState.title = hand[i].title;
			} else {
				Debug.Log(debug.head + "This object's card state was null!");
			} // if (cardState != null)

			// Debug.Log(debug.head + "Player " + playerNum);
			// Debug.Log(debug.head + GameManager.players);
			// Debug.Log(debug.head + GameManager.players.Count);
			// Debug.Log(debug.head + GameManager.players[playerNum]);
			// Debug.Log(debug.head + GameManager.players[playerNum].handUnits);
			// Debug.Log(debug.head + GameManager.players[playerNum].handUnits[i]);
			// Debug.Log(debug.head + hand[i]);
			// Debug.Log(debug.head + i);

			// GameManager.players[playerNum].handUnits[i] = new GridUnit(hand[i],
			// 	cardObj,
			// 	i, playerNum);

		} // for

	} // CreateHandObjects()

	// Initialize the internal game grid
	public void InitGameGrid() {

		if (!hasAuthority) {
			// Debug.Log(debug.head + "No authority to initialize the internal game grid!");
			return;
		}

		// Game Grid ######################################
		grid = new GridUnit[GameManager.width, GameManager.height];
		rowPos = new float[GameManager.height];
		maxStack = new int[GameManager.height];

		for (int x = 0; x < GameManager.width; x++) {
			for (int y = 0; y < GameManager.height; y++) {
				// Draw a card from the Land Tile deck
				Card card = Card.CreateInstance<Card>();

				if (GameManager.DrawCard(GameManager.masterDeckMutable.landTileDeck,
						GameManager.masterDeck.landTileDeck,
						out card)) {
					// Debug.Log("[GridManager] Tile Draw successful!");
					// Connect the drawn card to the internal grid
					grid[x, y] = new GridUnit(card: card, x: x, y: y);
				} else {
					Debug.Log(debug.head + "Tile Draw failure!");
				}
			} // y
		} // x

	} // InitGameGrid()

	// Initialize the internal market grid
	public void InitMarketGrid() {

		if (!hasAuthority) {
			Debug.Log(debug.head + "No authority to initialize the internal market grid!");
			return;
		}

		// Market Grid ####################################
		marketGrid = new GridUnit[Mathf.CeilToInt((float)GameManager.masterDeck.marketCardDeck.Count()
			/ (float)GameManager.height), GameManager.height];
		maxMarketStack = new int[GameManager.height];

		int marketWidth = Mathf.CeilToInt((float)GameManager.masterDeck.marketCardDeck.Count()
			/ (float)GameManager.height);

		for (int x = 0; x < marketWidth; x++) {
			for (int y = 0; y < GameManager.height; y++) {
				// Draw a card from the Market Card deck
				Card card = Card.CreateInstance<Card>();
				if (GameManager.DrawCard(GameManager.masterDeckMutable.marketCardDeck,
						GameManager.masterDeck.marketCardDeck,
						out card, false)) {
					// Debug.Log("[GridManager] Saving card at [" + x + ", " + y + "]");
					// Connect the drawn card to the internal grid
					marketGrid[x, y] = new GridUnit(card: card, x: x, y: y);
					// Debug.Log("[GridManager] Card saved: " + marketGrid[x, y].card);
				}
			} // y
		} // x

	} // InitMarketGrid()

	// Shifts rows of cards up or down. Used to give room for cards under tiles.
	public void ShiftRow(string type, int row, int units) {

		if (type == "Tile") {
			for (int x = 0; x < GameManager.width; x++) {
				for (int y = row; y < GameManager.height; y++) {
					float oldX = grid[x, y].tileObj.transform.position.x;
					float oldY = grid[x, y].tileObj.transform.position.y;
					float oldZ = grid[x, y].tileObj.transform.position.z;
					grid[x, y].tileObj.transform.position = new Vector3(grid[x, y].tileObj.transform.position.x,
						(oldY += (shiftUnit * units)),
						grid[x, y].tileObj.transform.position.z);
				} // for y
			} // for x
		} else if (type == "Market") {
			int marketWidth = Mathf.CeilToInt((float)GameManager.masterDeck.marketCardDeck.Count()
				/ (float)GameManager.height);
			for (int x = 0; x < marketWidth; x++) {
				for (int y = row; y < GameManager.height; y++) {

					if (marketGrid[x, y] != null) {
						float oldX = marketGrid[x, y].tileObj.transform.position.x;
						float oldY = marketGrid[x, y].tileObj.transform.position.y;
						float oldZ = marketGrid[x, y].tileObj.transform.position.z;
						marketGrid[x, y].tileObj.transform.position = new Vector3(marketGrid[x, y].tileObj.transform.position.x,
							(oldY += (shiftUnit * units)),
							marketGrid[x, y].tileObj.transform.position.z);
					} // if tile at the location isn't null

				} // for y
			} // for x
		} else {
			Debug.Log(debug.head + "Not doing anything");
		} // type

	} // ShiftRow()

	// [Command]
	// private void CmdSaveHandUnit(int playerNum, int index, ) {
	// 	GameManager.players[playerNum].handUnits[i] = new GridUnit(hand[i],
	// 			cardObj,
	// 			i, playerNum);
	// }

}

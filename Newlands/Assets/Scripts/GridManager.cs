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
	public static byte[] maxStack;
	public static byte[] maxMarketStack;
	public static readonly float cardThickness = 0.2f;
	public static readonly float shiftUnit = 1.2f;
	public static readonly float cardOffX = 11f;
	public static readonly float cardOffY = 8f;

	public GameObject landTilePrefab;
	public GameObject gameCardPrefab;
	public GameObject marketCardPrefab;

	void Start() {
		cardDis = FindObjectOfType<CardDisplay>();
		// PreInitGameGrid();
		// InitGameGrid();
	}

	// Creates the GameObjects used in the Game Grid
	public void CreateGameGridObjects() {

		if (!hasAuthority) {
			return;
		}

		Debug.Log("[GridManager] Creating Game Grid Objects...");

		string xZeroes = "0";
		string yZeroes = "0";

		for (byte x = 0; x < grid.GetLength(0); x++) {
			for (byte y = 0; y < grid.GetLength(1); y++) {

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

				Debug.Log("[GridManager] Spawning Card...");
				NetworkServer.Spawn(cardObj);

				// Grabs data from the internal grid and pushes it to the CardState scripts,
				// triggering them to update their visuals.
				Debug.Log("[GridManager] Trying to fill out Tile info...");
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
					cardState.body = grid[x, y].card.bodyText;
					cardState.footer = grid[x, y].card.footerText;
					cardState.resource = grid[x, y].card.resource;
					cardState.footerValue = grid[x, y].card.footerValue;
					cardState.target = grid[x, y].card.target;
					cardState.resource = grid[x, y].card.resource;
					cardState.percFlag = grid[x, y].card.percFlag;
					cardState.moneyFlag = grid[x, y].card.moneyFlag;
					cardState.footerOpr = grid[x, y].card.footerOpr;
				} else {
					Debug.Log("[GridManager] This object's card state was null!");
				}

			} // y

		} // x

	} // CreateGameGridObjects();

	// Creates the GameObjects used in the Market Grid
	public void CreateMarketGridObjects() {

		if (!hasAuthority) {
			return;
		}

		Debug.Log("[GridManager] Creating Market Grid Objects...");

		// Populate the Card prefab and create the Master Deck
		// marketCardPrefab = Resources.Load<GameObject>("Prefabs/GameCard");
		string xZeroes = "0";
		string yZeroes = "0";
		//masterDeckMutable = m

		for (byte x = 0; x < marketGrid.GetLength(0); x++) {
			for (byte y = 0; y < marketGrid.GetLength(1); y++) {

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

					GameObject cardObj = (GameObject) Instantiate(marketCardPrefab,
						new Vector3(xOff, yOff, 50),
						Quaternion.identity);

					Debug.Log("[GridManager] Spawning Card...");
					NetworkServer.Spawn(cardObj);

					Debug.Log("[GridManager] Trying to fill out Market Card info...");
					CardState cardState = cardObj.GetComponent<CardState>();

					if (cardState != null) {
						// Generate and Push the string of the object's name
						cardState.objectName = ("x" + xZeroes + x + "_"
							+ "y" + yZeroes + y + "_"
							+ "MarketCard");

						Debug.Log("[GridManager] Market Grid Size: "
							+ marketGrid.GetLength(0) + ", "
							+ marketGrid.GetLength(1));

						Debug.Log("[GridManager] Accessing card at [" + x + ", " + y + "]...");

						// Push new values to the CardState to be synchronized across the network
						cardState.category = marketGrid[x, y].card.category;
						cardState.title = marketGrid[x, y].card.title;
						cardState.subtitle = marketGrid[x, y].card.subtitle;
						cardState.body = marketGrid[x, y].card.bodyText;
						cardState.footer = marketGrid[x, y].card.footerText;
						cardState.resource = marketGrid[x, y].card.resource;
						cardState.footerValue = marketGrid[x, y].card.footerValue;
						cardState.target = marketGrid[x, y].card.target;
						cardState.resource = marketGrid[x, y].card.resource;
						cardState.percFlag = marketGrid[x, y].card.percFlag;
						cardState.moneyFlag = marketGrid[x, y].card.moneyFlag;
						cardState.footerOpr = marketGrid[x, y].card.footerOpr;
					} else {
						Debug.Log("[GridManager] This object's card state was null!");
					} // if (cardState != null)

				} // if (marketGrid[x, y] != null)

			} // y
		} // x

	} // CreateMarketGridObjects()

	// Shifts rows of cards up or down. Used to give room for cards under tiles.
	public void ShiftRow(string type, byte row, int units) {

		if (type == "Tile") {
			for (byte x = 0; x < GameManager.width; x++) {
				for (byte y = row; y < GameManager.height; y++) {
					float oldX = grid[x, y].tileObj.transform.position.x;
					float oldY = grid[x, y].tileObj.transform.position.y;
					float oldZ = grid[x, y].tileObj.transform.position.z;
					grid[x, y].tileObj.transform.position = new Vector3(grid[x, y].tileObj.transform.position.x,
						(oldY += (shiftUnit * units)),
						grid[x, y].tileObj.transform.position.z);
				} // for y
			} // for x
		} else if (type == "Market") {
			int marketWidth = Mathf.CeilToInt((float) GameManager.masterDeck.marketCardDeck.Count()
				/ (float) GameManager.height);
			for (byte x = 0; x < marketWidth; x++) {
				for (byte y = row; y < GameManager.height; y++) {

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
			Debug.Log("Not doing anything");
		} // type

	} // ShiftRow()

	// Initialize the internal game grid
	public void InitGameGrid() {

		if (!hasAuthority) {
			Debug.Log("[GridManager] No authority to initialize the internal game grid!");
			return;
		}

		// Game Grid ######################################
		grid = new GridUnit[GameManager.width, GameManager.height];
		rowPos = new float[GameManager.height];
		maxStack = new byte[GameManager.height];

		for (byte x = 0; x < GameManager.width; x++) {
			for (byte y = 0; y < GameManager.height; y++) {
				// Draw a card from the Land Tile deck
				Card card = Card.CreateInstance<Card>();

				if (GameManager.DrawCard(GameManager.masterDeckMutable.landTileDeck,
						GameManager.masterDeck.landTileDeck,
						out card)) {
					// Debug.Log("[GridManager] Tile Draw successful!");
					// Connect the drawn card to the internal grid
					grid[x, y] = new GridUnit(card: card, x: x, y: y);
				} else {
					Debug.Log("[GridManager] Tile Draw failure!");
				}
			} // y
		} // x

	} // InitGameGrid()

	// Initialize the internal market grid
	public void InitMarketGrid() {

		if (!hasAuthority) {
			Debug.Log("[GridManager] No authority to initialize the internal market grid!");
			return;
		}

		// Market Grid ####################################
		marketGrid = new GridUnit[Mathf.CeilToInt((float) GameManager.masterDeck.marketCardDeck.Count()
			/ (float) GameManager.height), GameManager.height];
		maxMarketStack = new byte[GameManager.height];

		int marketWidth = Mathf.CeilToInt((float) GameManager.masterDeck.marketCardDeck.Count()
			/ (float) GameManager.height);

		for (byte x = 0; x < marketWidth; x++) {
			for (byte y = 0; y < GameManager.height; y++) {
				// Draw a card from the Market Card deck
				Card card = Card.CreateInstance<Card>();
				if (GameManager.DrawCard(GameManager.masterDeckMutable.marketCardDeck,
						GameManager.masterDeck.marketCardDeck,
						out card)) {
					Debug.Log("[GridManager] Saving card at [" + x + ", " + y + "]");
					// Connect the drawn card to the internal grid
					marketGrid[x, y] = new GridUnit(card: card, x: x, y: y);
					Debug.Log("[GridManager] Card saved: " + marketGrid[x, y].card);
				}
			} // y
		} // x

	} // InitMarketGrid()

	[Command]
	public void CmdGetTitle(int x, int y) {
		// return grid[x,y].subScope;
	}

}

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using TMPro;

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
	[SyncVar(hook = "OnTitleChange")]
	public string tempTitle = "wow";

	public GameObject landTilePrefab;
	public GameObject gameCardPrefab;
	public GameObject marketCardPrefab;

	void Start() {
		cardDis = FindObjectOfType<CardDisplay>();
		// PreInitGameGrid();
		// InitGameGrid();
	}

	public void OnTitleChange(string newTitle) {
		Debug.Log("Title was changed, this is the hook!");
	}

	// [ClientRpc]
	public void CreateGridObjects(NetworkConnection conn) {

		Debug.Log("Has authority? " + hasAuthority + ", connection to client: " + conn);

		if (!hasAuthority) {
			return;
		}

		Debug.Log("[GridManager] Creating Grid Objects...");
		// PreInitGameGrid();

		// Populate the Card prefab and create the Master Deck
		// landTilePrefab = Resources.Load<GameObject>("Prefabs/Tile");
		string xZeroes = "0";
		string yZeroes = "0";
		//masterDeckMutable = masterDeck;	// Sets mutable deck version to internal one

		Debug.Log("Grid: " + grid);
		Debug.Log("Grid Length 0: " + grid.GetLength(0));

		for (byte x = 0; x < grid.GetLength(0); x++) {
			for (byte y = 0; y < grid.GetLength(1); y++) {
				// // Draw a card from the Land Tile deck
				// Card card = Card.CreateInstance<Card>();
				// DrawCard(masterDeckMutable.landTileDeck, masterDeck.landTileDeck, out card);

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

				// NetworkServer.SpawnWithClientAuthority(cardObj, connectionToClient);

				cardObj.name = ("x" + xZeroes + x + "_"
					+ "y" + yZeroes + y + "_"
					+ "Tile");
				// cardObj.name = ("LandTile_x" + x + "_y" + y + "_z0");

				Debug.Log("[GridManager] Trying to set parent of " + cardObj + " to " + this);
				cardObj.transform.SetParent(this.transform);
				cardObj.transform.rotation = new Quaternion(0, 180, 0, 0); // 0, 180, 0, 0

				// RpcFillOutCard(cardObj, x, y);
				// cardDis.DisplayCard(obj: cardObj, card: grid[x, y].card);

				Debug.Log("[GridManager] Spawning Card...");
				// Test later if NetworkServer.Spawn() will work now
				NetworkServer.SpawnWithClientAuthority(cardObj, conn);

				Debug.Log("[GridManager] Trying to fill out card info...");
				// FillOutCard(cardObj, x, y);

				// TODO: Jobs from FillOutCard()/DisplayCard() need to apply to the cardState from now on.
				// They can still be on a separate CardDisplay script to keep things clean, as long as that script
				// is attached to the object in order to avoid passing a ton of large arguments.

				CardState cardState = cardObj.GetComponent<CardState>();

				if (cardState != null) {
					cardState.titleStr = tempTitle;
					cardState.objectName = ("x" + xZeroes + x + "_"
					+ "y" + yZeroes + y + "_"
					+ "Tile");
				} else {
					Debug.Log("[GridManager] This object's card state was null!");
				}

				GameObject titleObj = cardObj.transform.Find("Front Canvas/Title").gameObject;
				TMP_Text title = titleObj.GetComponent<TMP_Text>();
				title.text = tempTitle;

			} // y

		} // x

	} //CmdCreateGridObjects();

	// public void PopulateMarket() {
	//     // Populate the Card prefab and create the Master Deck
	//     marketCardPrefab = Resources.Load<GameObject>("Prefabs/GameCard");
	//     string xZeroes = "0";
	//     string yZeroes = "0";
	//     //masterDeckMutable = masterDeck;	// Sets mutable deck version to internal one

	//     int marketWidth = Mathf.CeilToInt((float) ResourceInfo.resources.Count
	//         / (float) GameManager.height);

	//     for (byte x = 0; x < marketWidth; x++) {
	//         for (byte y = 0; y < GameManager.height; y++) {

	//             Card card = Card.CreateInstance<Card>();
	//             // Try to draw a card from the Market Card deck
	//             if (DrawCard(GameManager.masterDeckMutable.marketCardDeck, GameManager.masterDeck.marketCardDeck, out card)) {

	//                 float xOff = ((GameManager.width + 1) * cardOffX) + x * cardOffX;
	//                 float yOff = y * cardOffY;

	//                 // Determines the number of zeroes to add in the object name
	//                 if (x >= 10) {
	//                     xZeroes = "";
	//                 } else {
	//                     xZeroes = "0";
	//                 }
	//                 if (y >= 10) {
	//                     yZeroes = "";
	//                 } else {
	//                     yZeroes = "0";
	//                 } // zeroes calc

	//                 GameObject cardObj = (GameObject) Instantiate(this.marketCardPrefab, new Vector3(xOff, yOff, 50), Quaternion.identity);
	//                 cardObj.name = ("x" + xZeroes + x + "_"
	//                     + "y" + yZeroes + y + "_"
	//                     + "MarketCard");
	//                 // cardObj.name = ("LandTile_x" + x + "_y" + y + "_z0");

	//                 cardObj.transform.SetParent(this.transform);
	//                 // cardObj.transform.rotation = new Quaternion(0, 0, 0, 0);	// 0, 180, 0, 0

	//                 // Connect thr drawn card to the internal grid
	//                 marketGrid[x, y] = new GridUnit(card: card, tileObj: cardObj, x: x, y: y);
	//                 // rowPos[y] = cardObj.transform.position.y;	// Row position

	//                 // Connect the drawn card to the prefab that was just created

	//                 // NetworkServer.Spawn(cardObj);
	//                 // cardObj.SendMessage("DisplayCard", card);
	//                 cardDis.DisplayCard(obj: cardObj, card: grid[x, y].card);

	//             } // if a market card could be drawn

	//         } // y
	//     } // x
	// } // PopulateMarket()

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
			int marketWidth = Mathf.CeilToInt((float) ResourceInfo.resources.Count
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

	[Command]
	public void CmdChangeTestName(string newTitle) {
		tempTitle = newTitle;
	}

	// Initialize the internal game grid
	public void InitGameGrid() {

		if (!hasAuthority) {
			Debug.Log("No authority to initialize the internal grid!");
			return;
		}

		string titleCheck = "Title Check #" + Random.Range(1, 100);
		Debug.Log("Sending request to change name to " + titleCheck);
		CmdChangeTestName(titleCheck);

		// Game Grid ######################################
		// PreInitGameGrid();
		grid = new GridUnit[GameManager.width, GameManager.height];
		rowPos = new float[GameManager.height];
		maxStack = new byte[GameManager.height];

		for (byte x = 0; x < GameManager.width; x++) {
			for (byte y = 0; y < GameManager.height; y++) {
				// Draw a card from the Land Tile deck
				Card card = Card.CreateInstance<Card>();
				GameManager.DrawCard(GameManager.masterDeckMutable.landTileDeck, GameManager.masterDeck.landTileDeck, out card);
				// Connect the drawn card to the internal grid
				grid[x, y] = new GridUnit(card: card, x: x, y: y);

				// rowPos[y] = cardObj.transform.position.y; // Row position
			} // y
		} // x

		// Market Grid ####################################
		marketGrid = new GridUnit[Mathf.CeilToInt((float) ResourceInfo.resources.Count
			/ (float) GameManager.height), GameManager.height];
		maxMarketStack = new byte[GameManager.height];

	} // InitGameGrid()

	// Initialize the internal market grid
	public void InitMarketGrid() {

	} // InitMarketGrid()

	[Command]
	public void CmdGetTitle(int x, int y) {
		// return grid[x,y].subScope;
	}

}

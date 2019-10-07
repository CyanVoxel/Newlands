// Used by both the Client and the Server to keep track of known Card/Tile data.

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GridController : NetworkBehaviour
{
	// private GameObject matchManager;
	private MatchDataBroadcaster matchDataBroadcaster;
	private MatchController matchController;

	private CardData[, ] masterGrid;
	private CardData[, ] knownOwnersGrid;
	private CardData[, ] masterMarketGrid;

	// private MatchData matchData;
	private TurnEvent lastKnownTurnEvent;
	private MatchConfig config;

	public readonly float cardThickness = 0.2f;
	public readonly float shiftUnit = 1.2f;
	private readonly float cardOffX = 11f;
	private readonly float cardOffY = 8f;
	private static float[] rowPosition;
	private static int[] maxGridStack;
	private static int[] maxMarketStack;

	public CardData[, ] KnownOwnersGrid { get { return knownOwnersGrid; } set { knownOwnersGrid = value; } }

	private DebugTag debugTag = new DebugTag("GridController", "00BCD4");

	void Awake()
	{
		Debug.Log(debugTag + "The GridController has been created!");
		// StartCoroutine(CheckForBroadcastUpdates());

		StartCoroutine(CreateMainGridCoroutine());
		StartCoroutine(CreateMarketGridCoroutine());
		// StartCoroutine(CreatePlayerHandCoroutine());

	}

	// void Start()
	// {
	// 	if (!hasAuthority)
	// 		return;
	// }

	// void Update()
	// {
	// 	if (!hasAuthority)
	// 		return;

	// 	// StartCoroutine(CheckForBroadcastUpdates());
	// }

	void OnDisable()
	{
		Debug.Log(debugTag + "The GridController has been disbaled/destroyed!");
	}

	// [Client/Server] Parses the Match Data from MatchDataBroadcaster
	public void ParseTurnEvent()
	{
		Debug.Log(debugTag + "Parsing Turn Event...");
		TurnEvent newTurnEvent = JsonUtility.FromJson<TurnEvent>(matchDataBroadcaster.TurnEventStr);
		if (this.lastKnownTurnEvent != newTurnEvent)
		{
			this.lastKnownTurnEvent = newTurnEvent;
		}
		Debug.Log(debugTag + "Turn Event as: " + this.lastKnownTurnEvent);
	}

	public void ParseUpdatedCards() { }

	// [Client with Server-Only section]
	public void CreateGameGridObjects()
	{
		GameObject gridParent = new GameObject("MainGrid");

		for (int x = 0; x < config.GameGridWidth; x++)
		{
			for (int y = 0; y < config.GameGridHeight; y++)
			{
				float xOff = x * cardOffX;
				float yOff = y * cardOffY;

				GameObject cardObj = Instantiate(matchController.landTilePrefab,
					new Vector3(xOff, yOff, 50),
					Quaternion.identity);

				// cardObj.GetComponent<Animator>().Play("Flip");
				cardObj.name = (CardUtility.CreateCardObjectName("Tile", x, y));
				cardObj.transform.SetParent(gridParent.transform);

				cardObj.transform.rotation = new Quaternion(0, 180, 0, 0); // 0, 180, 0, 0

				knownOwnersGrid[x, y] = new CardData();

				if (hasAuthority)
					masterGrid[x, y].CardObject = cardObj;
			}
		}
	}

	// [Client/Server]
	public void CreateMarketGridObjects()
	{
		InitMarketGrid();

		GameObject marketGridParent = new GameObject("MarketGrid");

		for (int x = 0; x < Mathf.CeilToInt(((float)ResourceInfo.resources.Count - 1) / (float)config.GameGridHeight); x++)
		{
			for (int y = 0; y < config.GameGridHeight; y++)
			{
				if (masterMarketGrid[x, y] != null)
				{
					float xOff = ((config.GameGridWidth + 1) * cardOffX) + x * cardOffX;
					float yOff = y * cardOffY;

					GameObject cardObj = (GameObject)Instantiate(matchController.marketCardPrefab,
						new Vector3(xOff, yOff, 50),
						Quaternion.identity);

					cardObj.name = (CardUtility.CreateCardObjectName("MarketCard", x, y));
					cardObj.transform.SetParent(marketGridParent.transform);

					masterMarketGrid[x, y].CardObject = cardObj;
				}
			}
		}
	}

	// [Client/Server]
	public void CreatePlayerHandObjects(int playerNum)
	{
		GameObject playerHandParent = new GameObject("PlayerHand");

		for (int i = 0; i < config.PlayerHandSize; i++)
		{
			float xOff = i * 11 + (((config.GameGridWidth - config.PlayerHandSize) / 2f) * 11);
			float yOff = -10;

			GameObject cardObj = (GameObject)Instantiate(matchController.gameCardPrefab,
				new Vector3(xOff, yOff, 40),
				Quaternion.identity);

			cardObj.name = (CardUtility.CreateCardObjectName("GameCard", playerNum, i));
			cardObj.transform.SetParent(playerHandParent.transform);
		}
	}

	// [Client/Server] Called by CreateMarketGridObjects(). There is no need to hide Market Grid
	// data, so it's done on both the server and the client.
	public void InitMarketGrid()
	{
		masterMarketGrid = new CardData[(int)Mathf.Ceil((float)ResourceInfo.resources.Count - 1
			/ (float)config.GameGridHeight), config.GameGridHeight];
		maxMarketStack = new int[config.GameGridHeight];

		int marketWidth = Mathf.CeilToInt(((float)ResourceInfo.resources.Count - 1)
			/ (float)config.GameGridHeight);
		// Debug.Log(debugTag + "x: " + marketWidth
		// 	+ ", y: " + config.GameGridHeight
		// 	+ ", res: " + ResourceInfo.resources.Count);

		for (int x = 0; x < marketWidth; x++)
		{
			for (int y = 0; y < config.GameGridHeight; y++)
			{
				// Draw a card from the Market Card deck
				Card card;
				if (matchController.DrawCard(matchController.MasterDeckMutable.marketCardDeck,
						matchController.MasterDeck.marketCardDeck,
						out card, false))
				{
					// Connect the drawn card to the internal grid
					masterMarketGrid[x, y] = new CardData(card);
					// Debug.Log(debugTag + "Created Market Card: " + card.Resource);
				}
			}
		}
	}

	public void SetTileOwner(int x, int y, int ownerId)
	{
		masterGrid[x, y].OwnerId = ownerId;
		knownOwnersGrid[x, y].OwnerId = ownerId;
	}

	public bool IsTileOwned(int x, int y)
	{
		if (knownOwnersGrid[x, y].OwnerId == 0)
			return false;
		else
			return true;
	}

	public bool IsTileBankrupt(int x, int y)
	{
		if (!knownOwnersGrid[x, y].IsBankrupt)
			return false;
		else
			return true;
	}

	public CardData GetServerTile(string type, int x, int y)
	{
		if (masterGrid[x, y] != null)
			return masterGrid[x, y];
		else
			return null;
	}

	public CardData GetClientTile(string type, int x, int y)
	{
		if (knownOwnersGrid[x, y] != null)
			return knownOwnersGrid[x, y];
		else
			return null;
	}

	public void BankruptTile(int x, int y)
	{
		Debug.Log(debugTag + "Bankrupting tile!");
		knownOwnersGrid[x, y].IsBankrupt = true;
		masterGrid[x, y].IsBankrupt = true;
		// CardDisplay.BankruptVisuals(tile.tileObj);
	}

	public void IncrementStackSize(int y, string gridType)
	{
		switch (gridType)
		{
			case "Market":
				maxMarketStack[y]++;
				break;
			default:
				maxGridStack[y]++;
				break;
		}
	}

	public void AddCardToStack(int x, int y, string gridType, Card card)
	{
		switch (gridType)
		{
			case "Market":
				if (hasAuthority)
					masterMarketGrid[x, y].CardStack.Add(card);
				break;
			default:
				knownOwnersGrid[x, y].CardStack.Add(card);
				if (hasAuthority)
					masterGrid[x, y].CardStack.Add(card);
				break;
		}
	}

	// Shifts a grid if needed based on the target. Returns true if a row was shifted.
	public bool ShiftRowCheck(string type, int x, int y)
	{
		bool didShift = false;
		int maxStack = 0;
		CardData target = GetClientTile(type, x, y);

		switch (type)
		{
			case "Tile":
				maxStack = maxGridStack[y];
				break;
			case "Market":
				maxStack = maxMarketStack[y];
				break;
			default:
				break;
		}

		if (target.CardStack.Count >= maxStack)
		{
			IncrementStackSize(y, target.Category);
			ShiftRow(target.Category, y, 1);
			didShift = true;
			Debug.Log(debugTag + "Shifting row " + y);
		}
		else
		{
			Debug.Log(debugTag + "Card stack of  " + target.CardStack.Count + " was not greater than " + maxStack);
		}

		return didShift;
	}

	// Shifts rows of cards up or down. Used to give room for cards under tiles.
	private void ShiftRow(string type, int row, int units)
	{
		switch (type)
		{
			case "Tile":
				ShiftMasterGrid(row, units);
				break;
			case "Market":
				ShiftMasterMarketGrid(row, units);
				break;
			default:
				break;
		}
	}

	// Used by ShiftRow to shift the Master Grid rows
	private void ShiftMasterGrid(int row, int units)
	{
		for (int x = 0; x < config.GameGridWidth; x++)
		{
			for (int y = row; y < config.GameGridHeight; y++)
			{
				Debug.Log(debugTag + "Looking for x" + x + ", y" + y);
				// Debug.Log(debug + "Shifting [" + x + ", " + y + "]");
				float oldX = knownOwnersGrid[x, y].CardObject.transform.position.x;
				float oldY = knownOwnersGrid[x, y].CardObject.transform.position.y;
				float oldZ = knownOwnersGrid[x, y].CardObject.transform.position.z;

				// StartCoroutine(CardUtility.MoveObjectCoroutine(masterGrid[x, y].CardObject,
				// 	new Vector3(oldX, (oldY += (shiftUnit * units)), oldZ), .1f));

				knownOwnersGrid[x, y].CardObject.transform.position = new Vector3(oldX,
					(oldY += (shiftUnit * units)),
					knownOwnersGrid[x, y].CardObject.transform.position.z);
			}
		}
	}

	// Used by ShiftRow to shift the Master Market Grid rows
	private void ShiftMasterMarketGrid(int row, int units)
	{
		int marketWidth = Mathf.CeilToInt((float)matchController.MasterDeck.marketCardDeck.Count
			/ (float)config.GameGridHeight);

		for (int x = 0; x < marketWidth; x++)
		{
			for (int y = row; y < config.GameGridHeight; y++)
			{

				if (masterMarketGrid[x, y] != null)
				{
					float oldX = masterMarketGrid[x, y].CardObject.transform.position.x;
					float oldY = masterMarketGrid[x, y].CardObject.transform.position.y;
					// float oldZ = masterMarketGrid[x, y].tileObj.transform.position.z;
					masterMarketGrid[x, y].CardObject.transform.position = new Vector3(oldX,
						(oldY += (shiftUnit * units)),
						masterMarketGrid[x, y].CardObject.transform.position.z);
				}
			}
		}
	}

	// COROUTINES ##################################################################################

	// [Client/Server] Parses the Match Config from MatchDataBroadcaster
	public IEnumerator ParseMatchConfigCoroutine()
	{
		yield return StartCoroutine(GrabMatchDataBroadCasterCoroutine());

		while (this.config == null)
		{
			Debug.Log(debugTag + "Parsing Config...");
			this.config = JsonUtility.FromJson<MatchConfig>(matchDataBroadcaster.MatchConfigStr);
			Debug.Log(debugTag + "Config parsed as: " + this.config);

			if (this.config == null)
				yield return null;
		}
	}

	// [Client] Grabs the MatchDataBroadcaster from the MatchManager GameObject.
	private IEnumerator GrabMatchDataBroadCasterCoroutine()
	{
		if (matchDataBroadcaster == null)
		{
			matchDataBroadcaster = this.gameObject.GetComponent<MatchDataBroadcaster>();
			if (matchDataBroadcaster != null)
			{
				Debug.Log(debugTag + "MatchDataBroadcaster was found!");
			}
		}

		while (this.gameObject == null || matchDataBroadcaster == null)
		{
			Debug.LogError(debugTag.error + "MatchDataBroadcaster was NOT found!");
			yield return null;
		}
	}

	// [Client] Grabs the MatchDataBroadcaster from the MatchManager GameObject.
	private IEnumerator GrabMatchControllerCoroutine()
	{
		if (matchController == null)
		{
			matchController = this.gameObject.GetComponent<MatchController>();
			if (matchController != null)
			{
				Debug.Log(debugTag + "MatchController was found!");
			}
		}

		while (this.gameObject == null || matchController == null)
		{
			Debug.LogError(debugTag.error + "MatchController was NOT found!");
			yield return null;
		}
	}

	// [Client/Server] Create the Tile GameObjects for the Main Game Grid.
	private IEnumerator CheckForBroadcastUpdates()
	{
		yield return StartCoroutine(GrabMatchDataBroadCasterCoroutine());

		while (this.config == null)
			yield return StartCoroutine(ParseMatchConfigCoroutine());

		ParseTurnEvent();
		ParseUpdatedCards();
	}

	// [Client/Server] Create the Tile GameObjects for the Main Game Grid.
	private IEnumerator CreateMainGridCoroutine()
	{
		yield return StartCoroutine(ParseMatchConfigCoroutine());
		yield return StartCoroutine(GrabMatchControllerCoroutine());

		yield return StartCoroutine(CreateInternalMainGridCoroutine());

		// [Client with Server-Only section]
		Debug.Log(debugTag + "Creating Main Grid objects...");
		CreateGameGridObjects();
	}

	// [Client/Server] Create the Tile GameObjects for the Market Game Grid.
	private IEnumerator CreateMarketGridCoroutine()
	{
		yield return StartCoroutine(ParseMatchConfigCoroutine());
		yield return StartCoroutine(GrabMatchControllerCoroutine());

		Debug.Log(debugTag + "Creating Market Grid objects...");
		CreateMarketGridObjects();
	}

	// // [Client/Server] Create the Tile GameObjects for the Market Game Grid.
	// public IEnumerator CreatePlayerHandCoroutine(int playerId)
	// {
	// 	yield return StartCoroutine(ParseMatchConfigCoroutine());
	// 	yield return StartCoroutine(GrabMatchControllerCoroutine());

	// 	Debug.Log(debugTag + "Creating Player Hand objects...");
	// 	CreatePlayerHandObjects(playerId);
	// }

	private IEnumerator CreateInternalMainGridCoroutine()
	{
		yield return StartCoroutine(ParseMatchConfigCoroutine());

		masterGrid = new CardData[config.GameGridWidth, config.GameGridHeight];
		knownOwnersGrid = new CardData[config.GameGridWidth, config.GameGridHeight];
		rowPosition = new float[config.GameGridHeight];
		maxGridStack = new int[config.GameGridHeight];

		for (int x = 0; x < config.GameGridWidth; x++)
		{
			for (int y = 0; y < config.GameGridHeight; y++)
			{
				// Draw a card from the Land Tile deck
				// Card card = Card.CreateInstance<Card>();
				// [Server]
				if (hasAuthority)
				{
					Card card;

					if (matchController.DrawCard(matchController.MasterDeckMutable.landTileDeck,
							matchController.MasterDeck.landTileDeck,
							out card))
					{
						// Debug.Log("[GridManager] Tile Draw successful!");
						// Connect the drawn card to the internal grid
						masterGrid[x, y] = new CardData(card);
					}
					else
					{
						Debug.LogWarning(debugTag + "The LandTile Deck is out of cards! Refreshing the mutable deck...");

						matchController.RefreshDeck("LandTile");
						y--;
					}
				}

			} // y
		} // x
	}
}

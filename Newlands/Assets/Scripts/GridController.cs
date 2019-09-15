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
	private MatchConfigData config;

	private readonly float cardThickness = 0.2f;
	private readonly float shiftUnit = 1.2f;
	private readonly float cardOffX = 11f;
	private readonly float cardOffY = 8f;
	private static float[] rowPosition;
	private static int[] maxGridStack;
	private static int[] maxMarketStack;

	private DebugTag debugTag = new DebugTag("GridController", "00BCD4");

	void Awake()
	{
		Debug.Log(debugTag + "The GridController has been created!");
		// StartCoroutine(CheckForBroadcastUpdates());

		StartCoroutine(CreateMainGridCoroutine());
		StartCoroutine(CreateMarketGridCoroutine());
		StartCoroutine(CreatePlayerHandCoroutine());

	}

	void Start()
	{
		if (!hasAuthority)
			return;
	}

	void Update()
	{
		if (!hasAuthority)
			return;

		// StartCoroutine(CheckForBroadcastUpdates());
	}

	void OnDisable()
	{
		Debug.Log(debugTag + "The GridController has been disbaled/destroyed!");
	}

	// [Client/Server] Parses the Match Data from MatchDataBroadcaster
	public void ParseTurnEvent()
	{
		Debug.Log(debugTag + "Parsing Turn Event...");
		TurnEvent newTurnEvent = JsonUtility.FromJson<TurnEvent>(matchDataBroadcaster.TurnEventBroadcastStr);
		if (this.lastKnownTurnEvent != newTurnEvent)
		{
			this.lastKnownTurnEvent = newTurnEvent;
		}
		Debug.Log(debugTag + "Turn Event as: " + this.lastKnownTurnEvent);
	}

	public void ParseUpdatedCards() { }

	// [Server]
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
				cardObj.name = (CreateCardObjectName("Tile", x, y));
				cardObj.transform.SetParent(gridParent.transform);

				cardObj.transform.rotation = new Quaternion(0, 180, 0, 0); // 0, 180, 0, 0

				// // FOR TESTING ONLY!
				// if (masterGrid != null)
				// 	masterGrid[x, y].CardObject = cardObj;
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

					cardObj.name = (CreateCardObjectName("MarketCard", x, y));
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

			cardObj.name = (CreateCardObjectName("GameCard", playerNum, i));
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
			xZeroes = "";
		else
			xZeroes = "0";

		if (y >= 10)
			yZeroes = "";
		else
			yZeroes = "0";

		return (xChar + xZeroes + x + "_" + yChar + yZeroes + y + "_" + type);
	}

	// [Client/Server] Parses the Match Config from MatchDataBroadcaster
	public IEnumerator ParseMatchConfigCoroutine()
	{
		yield return StartCoroutine(GrabMatchDataBroadCasterCoroutine());

		while (this.config == null)
		{
			Debug.Log(debugTag + "Parsing Config...");
			this.config = JsonUtility.FromJson<MatchConfigData>(matchDataBroadcaster.MatchConfigDataStr);
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

		// [Server]
		if (hasAuthority)
		{
			yield return StartCoroutine(CreateInternalMainGridCoroutine());
		}

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

	// [Client/Server] Create the Tile GameObjects for the Market Game Grid.
	private IEnumerator CreatePlayerHandCoroutine()
	{
		yield return StartCoroutine(ParseMatchConfigCoroutine());
		yield return StartCoroutine(GrabMatchControllerCoroutine());

		Debug.Log(debugTag + "Creating Player Hand objects...");
		CreatePlayerHandObjects(1);
	}

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
				Card card;

				if (matchController.DrawCard(matchController.MasterDeckMutable.landTileDeck,
						matchController.MasterDeck.landTileDeck,
						out card))
				{
					// Debug.Log("[GridManager] Tile Draw successful!");
					// Connect the drawn card to the internal grid
					masterGrid[x, y] = new CardData(card);
					knownOwnersGrid[x, y] = new CardData();
				}
				else
				{
					Debug.LogError(debugTag.error + "Tile Draw failure!");
				}
			} // y
		} // x
	}
}

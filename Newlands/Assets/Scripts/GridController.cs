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
	// private MatchData matchData;
	private TurnEvent lastKnownTurnEvent;
	private MatchConfigData config;

	private readonly float cardThickness = 0.2f;
	private readonly float shiftUnit = 1.2f;
	private readonly float cardOffX = 11f;
	private readonly float cardOffY = 8f;

	private DebugTag debugTag = new DebugTag("CardController", "00BCD4");

	void Awake()
	{
		Debug.Log(debugTag + "The CardController has been created!");
		// StartCoroutine(CheckForBroadcastUpdates());

		StartCoroutine(CreateMainGridObjectsCoroutine());
		StartCoroutine(CreateMarketGridObjectsCoroutine());

		if (!hasAuthority)
			return;
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
		Debug.Log(debugTag + "The CardController has been disbaled/destroyed!");
	}

	// [Client/Server] Parses the Match Data from MatchDataBroadcaster
	public void ParseTurnEvent()
	{
		Debug.Log(debugTag + "Parsing Turn Event...");
		TurnEvent newTurnEvent = JsonUtility.FromJson<TurnEvent>(matchDataBroadcaster.TurnEventBroadcast);
		if (this.lastKnownTurnEvent != newTurnEvent)
		{
			this.lastKnownTurnEvent = newTurnEvent;
		}
		Debug.Log(debugTag + "Turn Event as: " + this.lastKnownTurnEvent);
	}

	public void ParseUpdatedCards() { }

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

				cardObj.name = (CreateCardObjectName("Tile", x, y));
				cardObj.transform.SetParent(gridParent.transform);

				cardObj.transform.rotation = new Quaternion(0, 180, 0, 0); // 0, 180, 0, 0

				// NetworkServer.Spawn(cardObj);
				// grid[x, y].tileObj = cardObj;

				// Grabs data from the internal grid and pushes it to the CardState scripts,
				// triggering them to update their visuals.
				// Debug.Log("[GridManager] Trying to fill out Tile info...");
				// CardState cardState = cardObj.GetComponent<CardState>();

				// if (cardState != null)
				// 	// Generate and Push the string of the object's name
					// cardState.objectName = (CreateCardObjectName("Tile", x, y));
				// else
				// 	Debug.Log(debugTag + "This object's card state was null!");
			}
		}
	}

	public void CreateMarketGridObjects()
	{
		GameObject marketGridParent = new GameObject("MarketGrid");
		int cardsMade = 0;

		for (int x = 0; x < Mathf.Ceil((float)ResourceInfo.resources.Count / (float)config.GameGridHeight); x++)
		{
			for (int y = 0; y < config.GameGridHeight; y++)
			{
				if (cardsMade < ResourceInfo.resources.Count)
				{
					float xOff = ((config.GameGridWidth + 1) * cardOffX) + x * cardOffX;
					float yOff = y * cardOffY;

					GameObject cardObj = (GameObject)Instantiate(matchController.marketCardPrefab,
						new Vector3(xOff, yOff, 50),
						Quaternion.identity);

					cardObj.name = (CreateCardObjectName("MarketCard", x, y));
					cardObj.transform.SetParent(marketGridParent.transform);

					// Debug.Log("[GridManager] Spawning Card...");
					// NetworkServer.Spawn(cardObj);
					// marketGrid[x, y].tileObj = cardObj;

					// Debug.Log("[GridManager] Trying to fill out Market Card info...");
					// CardState cardState = cardObj.GetComponent<CardState>();

					// if (cardState != null)
					// {
					// 	// Generate and Push the string of the object's name
					// 	cardState.objectName = (CreateCardObjectName("MarketCard", x, y));
					// 	// Push new values to the CardState to be synchronized across the network
					// 	// FillOutCardState(marketGrid[x, y].card, ref cardState);
					// }
					// else
					// {
					// 	Debug.Log(debugTag + "This object's card state was null!");
					// } // if (cardState != null)
					cardsMade++;
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
	private IEnumerator CreateMainGridObjectsCoroutine()
	{
		yield return StartCoroutine(ParseMatchConfigCoroutine());
		yield return StartCoroutine(GrabMatchControllerCoroutine());

		Debug.Log(debugTag + "Creating Main Grid objects...");

		CreateGameGridObjects();
	}

	// [Client/Server] Create the Tile GameObjects for the Market Game Grid.
	private IEnumerator CreateMarketGridObjectsCoroutine()
	{
		yield return StartCoroutine(ParseMatchConfigCoroutine());
		yield return StartCoroutine(GrabMatchControllerCoroutine());

		Debug.Log(debugTag + "Creating Market Grid objects...");

		CreateMarketGridObjects();
	}
}

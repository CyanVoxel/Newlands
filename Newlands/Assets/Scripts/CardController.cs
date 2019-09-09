// Used by both the Client and the Server to keep track of known Card/Tile data.

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CardController : NetworkBehaviour
{
	// private GameObject matchManager;
	private MatchDataBroadcaster matchDataBroadcaster;
	// private MatchData matchData;
	private TurnEvent lastKnownTurnEvent;
	private MatchConfigData config;

	private DebugTag debugTag = new DebugTag("CardController", "00BCD4");

	void Awake()
	{
		Debug.Log(debugTag + "The CardController has been created!");
		// StartCoroutine(CheckForBroadcastUpdates());

		if (!hasAuthority)
			return;

		StartCoroutine(CreateMainGridObjectsCoroutine());
		StartCoroutine(CreateMarketGridObjectsCoroutine());
	}

	void Start()
	{

	}

	void Update()
	{
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
		this.lastKnownTurnEvent = JsonUtility.FromJson<TurnEvent>(matchDataBroadcaster.TurnEventBroadcast);
		Debug.Log(debugTag + "Turn Event as: " + this.lastKnownTurnEvent);
	}

	public void ParseUpdatedCards() { }

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

		Debug.Log(debugTag + "Creating Main Grid objects...");
	}

	// [Client/Server] Create the Tile GameObjects for the Market Game Grid.
	private IEnumerator CreateMarketGridObjectsCoroutine()
	{
		yield return StartCoroutine(ParseMatchConfigCoroutine());

		Debug.Log(debugTag + "Creating Market Grid objects...");
	}
}

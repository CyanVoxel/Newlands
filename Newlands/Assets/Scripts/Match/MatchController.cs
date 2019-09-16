// The replacement for GameManager

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MatchController : NetworkBehaviour
{
	// FIELDS ######################################################################################
	[SerializeField]
	private MatchDataBroadcaster matchDataBroadcaster;
	private GridController gridController;
	private MatchData matchData;
	// public MatchData MatchData { get { return matchData; } }
	private MatchConfigData config;
	public MatchConfigData Config { get { return config; } }
	private MatchConnections matchConnections;

	private MasterDeck masterDeck;
	private MasterDeck masterDeckMutable;

	private List<PlayerData> players = new List<PlayerData>();
	public List<PlayerData> Players { get { return players; } }

	[SyncVar]
	private int playerIndex = 1; // This value increments when a new player joins
	public int PlayerIndex { get { return playerIndex; } set { playerIndex = value; } }

	public MasterDeck MasterDeck { get { return masterDeck; } }
	public MasterDeck MasterDeckMutable { get { return masterDeckMutable; } }

	public GameObject landTilePrefab;
	public GameObject gameCardPrefab;
	public GameObject marketCardPrefab;

	private DebugTag debugTag = new DebugTag("MatchController", "9C27B0");

	// METHODS #####################################################################################

	void Awake()
	{
		Debug.Log(debugTag + "The MatchController has been created!");
		DontDestroyOnLoad(this.gameObject);
		// this.gameObject.AddComponent<GridController>();
	}

	void Start()
	{
		Debug.Log(debugTag + "Initializing...");

		// [Client/Server]
		if (!hasAuthority)
		{
			this.config = JsonUtility.FromJson<MatchConfigData>(matchDataBroadcaster.MatchConfigStr);
			Debug.Log(debugTag + "Grabbed config for client: " + matchDataBroadcaster.MatchConfigStr);

			Debug.Log(debugTag + "Creating Decks...");
			this.masterDeck = new MasterDeck(config.DeckFlavor);
			this.masterDeckMutable = new MasterDeck(config.DeckFlavor);

			return;
		}

		// [Server]
		InitializeMatch();
	}

	// void Update()
	// {
	// 	// Debug.Log(matchDataBroadcaster.MatchConfigDataStr);
	// }

	void OnDisable()
	{
		Debug.Log(debugTag + "The MatchController has been disbaled/destroyed!");
	}

	// Returns an ID to a player based on their address
	// TODO: Change the argument passed to a username
	public int GetPlayerId(string address)
	{
		// if (matchConnections == null)
		// 	matchConnections = GameObject.Find("NetworkManager").GetComponent<MatchConnections>();

		int index = playerIndex;
		playerIndex++;

		// matchConnections.PlayerAddresses.TryGetValue(address, out index);

		// if (index >= 0)
		// 	Debug.Log(debugTag + "Giving address " + address + " an ID of " + index);
		// else
		// 	Debug.Log(debugTag + "Address " + address + " wanted an ID but has not been logged!");

		Debug.Log(debugTag + "Ignoring passed arguments until usernames are implemented");
		Debug.Log(debugTag + "Give player an ID of " + index);

		return index;
	}

	// [Server] Grabs the MatchDataBroadcaster from this parent GameObject
	// On the client, they will grab the same MatchDataBroadcaster themselves in a different script.
	private void GrabMatchDataBroadCaster()
	{
		matchDataBroadcaster = this.gameObject.GetComponent<MatchDataBroadcaster>();
		if (matchDataBroadcaster != null)
		{
			Debug.Log(debugTag + "MatchDataBroadcaster was found!");
		}
		else
		{
			Debug.LogError(debugTag.error + "MatchDataBroadcaster was NOT found!");
		}
	}

	// [Server] Loads the config from MatchSetupController and initializes the match.
	private void InitializeMatch()
	{
		GrabMatchDataBroadCaster();

		// TODO: Wrap this in a method like data broadcaster
		this.matchConnections = GameObject.Find("NetworkManager").GetComponent<MatchConnections>();

		GameObject matchSetupManager = GameObject.Find("SetupManager");
		if (matchSetupManager != null)
		{
			Debug.Log(debugTag + "SetupManager (our dad) was found!");

			MatchSetupController setupController = matchSetupManager.GetComponent<MatchSetupController>();
			StartCoroutine(LoadMatchConfigCoroutine(setupController));
			StartCoroutine(InitializeMatchCoroutine());
		}
		else
		{
			Debug.LogError(debugTag.error + "SetupManager (our dad) was NOT found!");
		}
	}

	// Initializes each player object and draws a hand for them
	private void InitPlayers()
	{
		for (int i = 0; i < config.MaxPlayerCount; i++)
		{
			players.Add(new PlayerData());
			players[i].Id = (i + 1);
			players[i].hand = DrawHand(config.PlayerHandSize);
		} // for playerCount
		UpdatePlayerMoneyStr();
	}

	private void UpdatePlayerMoneyStr()
	{
		matchDataBroadcaster.PlayerMoneyStr = "";

		for (int i = 0; i < config.MaxPlayerCount; i++)
		{
			matchDataBroadcaster.PlayerMoneyStr += players[i].totalMoney;

			if (config.MaxPlayerCount - i > 1)
			{
				matchDataBroadcaster.PlayerMoneyStr += "_";
			}
		} // for playerCount
		Debug.Log(debugTag + "Player Money String: " + matchDataBroadcaster.PlayerMoneyStr);
	} // UpdatePlayerMoneyStr()()

	// Draws random GameCards from the masterDeck and returns a deck of a specified size
	private Deck DrawHand(int handSize)
	{
		Deck deck = new Deck(); // The deck of drawn cards to return

		for (int i = 0; i < handSize; i++)
		{
			// Draw a card from the deck provided and add it to the deck to return.
			// NOTE: In the future, masterDeckMutable might need to be checked for cards
			// 	before preceding.
			// Card card = Card.CreateInstance<Card>();
			Card card;
			if (DrawCard(masterDeckMutable.gameCardDeck, masterDeck.gameCardDeck, out card))
			{
				deck.Add(card);
			}
			else
			{
				// Destroy(card);
			}
		} // for

		return deck;
	}

	// Draws a card from a deck. Random by default.
	public bool DrawCard(Deck deckMut, Deck deckPerm, out Card card, bool random = true)
	{
		// Card card;	// Card to return
		int cardsLeft = deckMut.Count; // Number of cards left from mutable deck
		int cardsTotal = deckPerm.Count; // Number of cards total from permanent deck

		// Draws a card from the mutable deck, then removes that card from the deck.
		// If all cards are drawn, draw randomly from the immutable deck.
		if (cardsLeft > 0)
		{
			if (random)
			{
				card = deckMut[Random.Range(0, cardsLeft)];
			}
			else
			{
				card = deckMut[deckMut.Count - 1];
			}

			deckMut.Remove(card);
			// Debug.Log("<b>[GameManager]</b> " +
			// 	cardsLeft +
			// 	" of " +
			// 	cardsTotal +
			// 	" cards left");
		}
		else
		{
			// This one HAS to be random anyways
			card = deckPerm[Random.Range(0, cardsTotal)];
			return false;
			// Debug.LogWarning("<b>[GameManager]</b> Warning: " +
			//  "All cards (" + cardsTotal + ") were drawn from a deck! " +
			//  " Now drawing from immutable deck...");
		}

		return true;
	}

	// COROUTINES ##################################################################################

	// [Client/Server] The main initialization coroutine for the match.
	// Dependant on LoadMatchConfigCoroutine finishing.
	private IEnumerator InitializeMatchCoroutine()
	{
		// Will not run if LoadMatchConfigCoroutine() has not been run.
		// This gives the functionality of yield return LoadMatchConfigCoroutine()
		// without needing to pass this/it the MatchSetupController.
		while (this.config == null)
			yield return null;

		Debug.Log(debugTag + "Initializing Match from Config data...");

		this.masterDeck = new MasterDeck(config.DeckFlavor);
		this.masterDeckMutable = new MasterDeck(config.DeckFlavor);

		// [Server]
		if (hasAuthority)
		{
			InitPlayers();

			this.matchData = new MatchData();

			Debug.Log(debugTag + "Sending current Match Data to Broadcaster... " + this.matchData);
			matchDataBroadcaster.MatchDataStr = JsonUtility.ToJson(this.matchData);
		}

	}

	// [Server] Loads this MatchManager's config from the MatchSetupController's final config.
	private IEnumerator LoadMatchConfigCoroutine(MatchSetupController controller)
	{
		Debug.Log(debugTag + "Grabbing config from MatchSetupController...");

		while (!controller.Ready)
			yield return null;

		this.config = controller.InitialConfig;
		Debug.Log(debugTag + "Config loaded! Sending to Broadcaster...");
		matchDataBroadcaster.MatchConfigStr = JsonUtility.ToJson(controller.InitialConfig);
	}

}

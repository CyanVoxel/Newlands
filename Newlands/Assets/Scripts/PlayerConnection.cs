﻿// The class responsible for representing a Player Connection over the network. Required by Mirror.

// using System.Collections;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

// public class SyncListCard : SyncList<Card> { }

// public class SyncListCoordinate2 : SyncList<Coordinate2> { }

public class PlayerConnection : NetworkBehaviour
{
	// FIELDS ######################################################################################

	private MatchDataBroadcaster matchDataBroadcaster;
	private GridController gridController;
	private MatchController matchController;
	private HudController hudController;

	// 	public GameManager gameMan;
	// 	public GridManager gridMan;
	// 	// public MouseManager localMouseMan;
	// 	// public GuiManager guiMan;
	public GameObject mouseManPrefab;
	// public SyncListCard hand;
	private List<Card> hand = new List<Card>();

	private MatchConfig config;
	private MatchData matchData;
	// private TurnEvent turnEvent;

	private List<string> updatedCards = new List<string>();

	private static DebugTag debugTag = new DebugTag("PlayerConnection", "2196F3");
	// 	[SyncVar(hook = "OnIdChange")]
	// 	[SerializeField]
	[SyncVar]
	private int id = -1;
	[SyncVar]
	private string username;
	// 	[SyncVar]
	// private bool initIdFlag = false;
	// private int lastKnownTurn = -1;
	// private int lastKnownRound = -1;
	// private int lastKnownPhase = -1;
	private string lastKnownTopCard = "";
	private string lastKnownPriceList = "";

	// A JSON version of the match data, used to save parsing time unless needed.
	private string lastKnownMatchDataStr = "";

	// An array of Lists containing coordinates of all known Tiles owned by all players.
	// This is used for quicker access of THESE CARDS.
	private List<Coordinate2>[] knownOwnersList;
	// A 2-Dimensional array containing the location of all known Tiles owned by all players.
	// This is used for quicker access to NEIGHBOR CARDS.
	// private int[, ] knownOwnersGrid;
	// A List of CardData used for keeping references of the Market Cards.
	// Used for acessing the cards' CardStates to update their footer text.
	List<CardData> localMarketList = new List<CardData>();
	// private Dictionary<CardState, int> localPrices = new Dictionary<CardState, int>();
	private List<int> localPrices = new List<int>();
	private List<string> localResources = new List<string>();
	private string turnEventStr;

	private bool initialzed = false;

	// METHODS #####################################################################################

	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}

	// Start is called before the first frame update.
	void Start()
	{
		if (!isLocalPlayer)
		{
			return;
		}

		// this.hand.Callback += OnHandUpdated;
		// connection = connectionToClient;

		// TODO: Change the argument passed to a username
		Debug.Log(debugTag.head + connectionToServer.address);
		StartCoroutine(InitPlayer(connectionToServer.address));

		// CmdSpawnMouseManager();

	} //Start()

	void Update()
	{
		if (!isLocalPlayer || !initialzed)
			return;

		if (this.turnEventStr != matchDataBroadcaster.TurnEventStr
			&& this.turnEventStr != null)
		{
			HandleTurnEvent();
			// this.turnEventStr = gameMan.turnEventBroadcast; // Unnecessary, this gets done later.
		}

		if (this.lastKnownPriceList != matchDataBroadcaster.PriceListStr)
		{
			Debug.Log(debugTag + "Updating prices...");
			UpdateLocalPrices();
			UpdateMarketFooters();
			// this.lastKnownPriceList = gameMan.priceListStr; // Unnecessary, this gets done later.
		}

		// Highlight cards during Buying Phase
		if (CheckForNewMatchData(1))
		{
			if (this.matchData.Turn == this.id)
			{
				if (this.matchData.Round > matchController.Config.GraceRounds)
				{
					Debug.Log(debugTag + "Highlighting " + GetNeighbors().Count
						+ " when there's " + GetUnownedCards().Count
						+ " card(s) left");
					CardAnimations.HighlightCards(GetNeighbors(), this.id);
				}
				else
				{
					CardAnimations.HighlightCards(GetUnownedCards(), this.id);
				}
			}
			else
			{
				CardAnimations.HighlightCards(GetUnownedCards(), 0);
			}
		}

		if (matchData.Winner >= 0)
		{
			// Debug.Log("WINNER WINNER CHICKED DINNER! CONGRATS PLAYER " + matchData.Winner);
			this.hudController.DisplayWinner(matchData.Winner);
		}

		// This is at the bottom so handlers can compare old data with new data
		UpdateKnownInfo();
	} // Update()

	private IEnumerator InitPlayer(string address)
	{
		// Grab Components
		yield return StartCoroutine(GrabComponentsCoroutine());

		// Determine of the main grid has been created yet
		GameObject testCard = GameObject.Find(CardUtility.CreateCardObjectName("Tile", 0, 0));
		while (testCard == null)
		{
			testCard = GameObject.Find(CardUtility.CreateCardObjectName("Tile", 0, 0));
			yield return null;
		}

		// Get ID from MatchManager
		CmdInitId(address);
		while (this.id == -1)
			yield return null;

		// Fetch username from PlayerDataController and send it serverside to MatchController
		InitUsername();

		// Send our ID to the HudController, then force an update from it.
		this.hudController.GetComponent<HudController>().ThisPlayerId = this.id;
		this.hudController.UpdateHud();

		// Grab the config from the broadcaster
		this.config = JsonUtility.FromJson<MatchConfig>(matchDataBroadcaster.MatchConfigStr);
		while (this.config == null)
			yield return null;

		// Create hand card objects
		Debug.Log(debugTag + "Creating hand card objects for Player " + this.id);
		gridController.CreatePlayerHandObjects(this.id);

		// CmdGetHand(this.id);
		// Debug.Log(debug + "Hand size: " + this.hand.Count);
		// gridMan.CreateHandObjects(this.id, this.hand);
		this.knownOwnersList = new List<Coordinate2>[config.MaxPlayerCount];
		// this.knownOwnersGrid = new int[config.GameGridWidth, config.GameGridHeight];

		for (int i = 0; i < config.MaxPlayerCount; i++)
			this.knownOwnersList[i] = new List<Coordinate2>();

		InitLocalResourcePrices();

		this.matchData = JsonUtility.FromJson<MatchData>(matchDataBroadcaster.MatchDataStr);
		this.turnEventStr = matchDataBroadcaster.TurnEventStr;

		// InitLocalMarketGrid();
		UpdateKnownInfo();

		while (this.id == -1)
			yield return null;

		for (int i = 0; i < config.PlayerHandSize; i++)
			CmdGetHandCard(this.id, i);

		// If this is Player 1 and it's the first Turn of the first Round
		if (this.id == 1 && this.matchData.Turn == 1
			&& this.matchData.Round == 1
			&& this.matchData.Phase == 1)
		{
			CardAnimations.HighlightCards(GetUnownedCards(), this.id);
		}

		this.initialzed = true;
	}

	// // Tries to grab necessary components if they haven't been already.
	// // Returns true if all components were verified to be grabbed.
	// private bool TryToGrabComponents()
	// {
	// 	if (this.matchDataBroadcaster == null)
	// 		this.matchDataBroadcaster = FindObjectOfType<MatchDataBroadcaster>();

	// 	if (this.matchController == null)
	// 		this.matchController = FindObjectOfType<MatchController>();

	// 	if (this.matchDataBroadcaster == null)
	// 		return false;
	// 	if (this.matchController == null)
	// 		return false;

	// 	return true;
	// }

	private IEnumerator GrabComponentsCoroutine()
	{
		while (this.matchDataBroadcaster == null)
		{
			this.matchDataBroadcaster = FindObjectOfType<MatchDataBroadcaster>();
			yield return null;
		}

		while (this.matchController == null)
		{
			this.matchController = FindObjectOfType<MatchController>();
			yield return null;
		}

		while (this.gridController == null)
		{
			this.gridController = FindObjectOfType<GridController>();
			yield return null;
		}

		while (this.hudController == null)
		{
			this.hudController = FindObjectOfType<HudController>();
			yield return null;
		}
	}

	// // Fires when this PlayerConnection's ID changes.
	// private void OnIdChange(int newId)
	// {
	// 	this.id = newId;
	// 	this.transform.name = "Player (" + this.id + ")";
	// 	transform.GetComponent<MouseManager>().SetId(newId);
	// 	// mouseManObj.transform.name = "MouseManager (" + this.id + ")";
	// 	// GameManager.localPlayerId = this.id;
	// } // OnIdChange()

	private List<Coordinate2> GetNeighbors()
	{
		List<Coordinate2> cardsToSend = new List<Coordinate2>();

		// Adds all C2s of cards owned by this player to the list
		for (int k = 0; k < this.knownOwnersList[this.id - 1].Count; k++)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					int x = this.knownOwnersList[this.id - 1][k].x;
					int y = this.knownOwnersList[this.id - 1][k].y;

					if (x + i >= 0
						&& x + i < config.GameGridWidth
						&& y + j >= 0
						&& y + j < config.GameGridHeight)
					{
						if (gridController.KnownOwnersGrid[x + i, y + j].OwnerId == 0
							&& !cardsToSend.Contains(new Coordinate2((x + i), (y + j))))
						{
							cardsToSend.Add(new Coordinate2((x + i), (y + j)));
							// Debug.Log(debug + "Adding card to add [" + (x + i) + ", " + (y + j) + "]");
						}
					}
				}
			}
		}

		return cardsToSend;
	} // GetNeighbors()

	private List<Coordinate2> GetUnownedCards()
	{
		List<Coordinate2> cardsToSend = new List<Coordinate2>();

		for (int x = 0; x < config.GameGridWidth; x++)
		{
			for (int y = 0; y < config.GameGridWidth; y++)
			{
				if (gridController.KnownOwnersGrid[x, y].OwnerId == 0)
					cardsToSend.Add(new Coordinate2(x, y));
			}
		}

		return cardsToSend;
	} // GetUnownedCards()

	// Initializes this PlayerConnection's ID by asking the Server to assign one.
	[Command]
	private void CmdInitId(string address)
	{
		if (GameObject.Find("MatchManager") != null)
			this.id = GameObject.Find("MatchManager").GetComponent<MatchController>().GetPlayerId(address);
		else
			this.id = GameObject.Find("MatchManager(Clone)").GetComponent<MatchController>().GetPlayerId(address);

		transform.GetComponent<MouseManager>().SetId(this.id);

		Debug.Log(debugTag + "Assigned ID of " + this.id);
		this.transform.name = "Player (" + this.id + ")";
	}

	// public void DestroyCard(string objectName)
	// {
	// 	Debug.Log(debug + "Trying to destroy " + objectName);
	// 	Destroy(GameObject.Find(objectName));
	// }

	// // Fire's when this Player's hand of cards updates
	// private void OnHandUpdated(SyncListCardData.Operation op, int index, CardData card)
	// {
	// 	if (!isLocalPlayer)
	// 	{
	// 		return;
	// 	}

	// 	// Debug.Log(debug + "Index: " + index);

	// 	if (index == (GameManager.handSize - 1) && op == SyncListCardData.Operation.OP_ADD)
	// 	{
	// 		gridMan.CreateHandObjects(this.id, this.hand);
	// 	}
	// } // OnHandUpdated()

	// Checks for new match data in the MatchDataBroadcaster, given a Phase to watch for.
	// Updates known match data if newer data was found.
	private bool CheckForNewMatchData(int phase)
	{
		bool newData = false;

		if (lastKnownMatchDataStr != matchDataBroadcaster.MatchDataStr)
		{
			MatchData newMatchData = this.matchData;
			newMatchData = JsonUtility.FromJson<MatchData>(matchDataBroadcaster.MatchDataStr);

			if (newMatchData.Turn != this.matchData.Turn
				|| newMatchData.Round != this.matchData.Round
				&& newMatchData.Phase == phase)
			{
				this.matchData = newMatchData;
				newData = true;
			}
		}

		return newData;
	}

	private void UpdateKnownInfo()
	{
		this.lastKnownMatchDataStr = matchDataBroadcaster.MatchDataStr;
		this.matchData = JsonUtility.FromJson<MatchData>(matchDataBroadcaster.MatchDataStr);
		this.turnEventStr = matchDataBroadcaster.TurnEventStr;

		// this.lastKnownTurn = gameMan.turn;
		// this.lastKnownRound = gameMan.round;
		// this.lastKnownPhase = gameMan.phase;
		// this.turnEventStr = gameMan.turnEventBroadcast;
		// this.lastKnownTopCard = gameMan.topCardStr;
		// this.lastKnownPriceList = gameMan.priceListStr;
	} // UpdateKnownRounds()

	private void HandleTurnEvent()
	{
		GameObject cardObj;
		//Handle an event during Phase 2 on your turn
		this.turnEventStr = matchDataBroadcaster.TurnEventStr;
		TurnEvent turnEvent = JsonUtility.FromJson<TurnEvent>(this.turnEventStr);

		Card turnCard = JsonUtility.FromJson<Card>(turnEvent.card);
		CardData targetCard = JsonUtility.FromJson<CardData>(turnEvent.card);

		Debug.Log(debugTag + "TurnEvent: " + turnEvent);
		Debug.Log(debugTag + "Target Category: " + targetCard.Category);

		// Check if the message should be addressed by this player
		// if (turnEvent.phase != phase || turnEvent.playerId != this.id)
		// {
		// 	return;
		// }

		// Operations ==========================================================

		switch (turnEvent.operation)
		{
			case "Play":

				// Debug.Log(debugTag + "Trying to destroy " + cardObj.name);
				// Destroy(cardObj);

				// Debug.Log(debugTag + "Moving " + cardObj.name);

				// If the card is meant to be stacked
				if (!turnCard.DiscardFlag)
				{
					if (!hasAuthority)
						gridController.AddCardToStack(turnEvent.targetX, turnEvent.targetY, targetCard.Category, turnCard);

					// Debug.Log(debugTag + "Running Shift Row Check on " + targetCard.Category + ", " + turnEvent.targetX+ ", " + turnEvent.targetY);
					if (gridController.ShiftRowCheck(targetCard.Category, turnEvent.targetX, turnEvent.targetY))
						gridController.IncrementStackSize(turnEvent.targetY, targetCard.Category);

					Debug.Log(debugTag + "Trying to find " + CardUtility.CreateCardObjectName(targetCard.Category, turnEvent.targetX, turnEvent.targetY));
					GameObject targetObject = GameObject.Find(CardUtility.CreateCardObjectName(targetCard.Category, turnEvent.targetX, turnEvent.targetY));

					CardData tile = gridController.GetClientTile(targetCard.Category, turnEvent.targetX, turnEvent.targetY);

					// Debug.Log(debugTag.head + "Target Category: " + targetCard.Category);
					// Debug.Log(debugTag.head + targetObject.name + " Stack Size: " + tile.CardStack.Count);

					Vector3 gap = targetObject.transform.position - new Vector3(targetObject.transform.position.x,
						targetObject.transform.position.y
						- (gridController.shiftUnit * (tile.CardStack.Count)),
						(targetObject.transform.position.z)
						+ (gridController.cardThickness * (tile.CardStack.Count)));

					if (turnEvent.playerId == this.id)
					{
						cardObj = GameObject.Find(CardUtility.CreateCardObjectName(turnEvent.cardType,
							turnEvent.x, turnEvent.y));

						Debug.Log(debugTag + "Trying to move " + cardObj.name + " under " + targetObject.name);
						// gridController.GetTile(targetCard.x, targetCard.y)

						cardObj.transform.name = CardUtility.CreateCardObjectName("Stacked", turnEvent.x, tile.CardStack.Count - 1);
						cardObj.transform.SetParent(targetObject.transform);

						StartCoroutine(CardAnimations.MoveCardCoroutine(cardObj, targetObject, gap, .1f));

						CreateNewCardObject(turnEvent.y, turnEvent.topCard);
					}
					else
					{
						GameObject otherPlayersCard = (GameObject)Instantiate(matchController.gameCardPrefab,
							new Vector3(0, 60, 40),
							Quaternion.identity);
						otherPlayersCard.GetComponent<CardViewController>().Card = JsonUtility.FromJson<CardData>(turnEvent.playedCard);

						otherPlayersCard.transform.name = CardUtility.CreateCardObjectName("Stacked", turnEvent.x, tile.CardStack.Count - 1);
						otherPlayersCard.transform.SetParent(targetObject.transform);
						StartCoroutine(CardAnimations.MoveCardCoroutine(otherPlayersCard, targetObject, gap, .1f));
					}
				}
				else // Cards that don't get stacked (like discards)
				{
					if (turnEvent.playerId == this.id)
					{
						cardObj = GameObject.Find(CardUtility.CreateCardObjectName(turnEvent.cardType,
							turnEvent.x, turnEvent.y));
						Destroy(cardObj);
						CreateNewCardObject(turnEvent.y, turnEvent.topCard);
					}
				}

				// cardObj.transform.Translate(new Vector3(targetObject.transform.position.x,
				// 		targetObject.transform.position.y
				// 		- (gridController.shiftUnit * targetCard.CardStack.Count),
				// 		(targetObject.transform.position.z)
				// 		+ (gridController.cardThickness * targetCard.CardStack.Count)));

				if (turnEvent.topCard != "empty")
				{
					// this.hand[turnEvent.y] = JsonUtility.FromJson<CardData>(turnEvent.topCard);
					// CreateNewCardObject(turnEvent.y, turnEvent.topCard);
				}
				else
				{
					Debug.Log(debugTag + "GameCard deck must be empty!");
				}

				// TODO: Add code to refresh a market card's footer value if a card was played on it.

				break;
			case "Buy":
				// Add bought Tile to local knowledge base
				gridController.KnownOwnersGrid[turnEvent.x, turnEvent.y].OwnerId = turnEvent.playerId;
				this.knownOwnersList[turnEvent.playerId - 1].Add(new Coordinate2(turnEvent.x, turnEvent.y));

				// Grab the Tile GameObject that was bought
				Debug.Log(debugTag + CardUtility.CreateCardObjectName(turnEvent.cardType,
					turnEvent.x,
					turnEvent.y));
				cardObj = GameObject.Find(CardUtility.CreateCardObjectName(turnEvent.cardType,
					turnEvent.x,
					turnEvent.y));

				CardData boughtCard = JsonUtility.FromJson<CardData>(turnEvent.card);

				boughtCard.CardObject = cardObj;
				gridController.KnownOwnersGrid[turnEvent.x, turnEvent.y] = boughtCard;
				gridController.KnownOwnersGrid[turnEvent.x, turnEvent.y].OwnerId = turnEvent.playerId;

				CardAnimations.FlipCard(turnEvent.cardType, turnEvent.x, turnEvent.y);

				cardObj.GetComponent<CardViewController>().Card = JsonUtility.FromJson<Card>(turnEvent.card);

				// Depending on the player who bought the tile, change the Tile's color.
				cardObj.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.GetDefaultPlayerColor(turnEvent.playerId, 500, true);
				cardObj.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.GetDefaultPlayerColor(turnEvent.playerId, 500, true);
				break;
		}
	}

	private void CreateNewCardObject(int index, string cardStr)
	{
		if (cardStr != null)
		{
			float xOff = index * 11 + (((config.GameGridWidth - config.PlayerHandSize) / 2f) * 11);
			float yOff = -10;

			Vector3 finalPosition = new Vector3(xOff, yOff, 40);

			// If old card exists, destroy it
			GameObject oldCardObj = GameObject.Find(CardUtility.CreateCardObjectName("GameCard",
				this.id, index));

			if (oldCardObj != null)
			{
				Debug.Log(debugTag + "Trying to destroy " + oldCardObj.name);
				Destroy(oldCardObj);
			}

			// Create new card
			// NOTE: In the future when there is an actual deck model, have it originate from that.
			GameObject cardObj = (GameObject)Instantiate(matchController.gameCardPrefab,
				new Vector3(0, -40, 40),
				Quaternion.identity);

			cardObj.GetComponent<CardViewController>().Card = JsonUtility.FromJson<Card>(cardStr);
			cardObj.transform.SetParent(GameObject.Find("PlayerHand").transform);

			StartCoroutine(CardAnimations.MoveCardCoroutine(cardObj, finalPosition, 0.1f));

			cardObj.name = (CardUtility.CreateCardObjectName("GameCard", this.id, index));
		}
	}

	// private void InitLocalMarketGrid()
	// {
	// 	// Container used for storing a found Market Card.
	// 	GameObject marketCardObj;
	// 	int marketWidth = (int)Mathf.Ceil((float)ResourceInfo.resources.Count / (float)GameManager.height);
	// 	int marketHeight = GameManager.height;
	// 	// Initialize the local Market Grid
	// 	// CardState[,] localMarketGrid = new CardState[marketWidth, marketHeight];
	// 	// List<CardState> localMarketList = new List<CardState>();
	// 	// localPrices = new int[localMarketList.Count];
	// 	// localResources = new string[localMarketList.Count];

	// 	for (int x = 0; x < marketWidth; x++)
	// 	{
	// 		for (int y = 0; y < marketHeight; y++)
	// 		{
	// 			marketCardObj = GameObject.Find(GameManager.CreateCardObjectName("MarketCard", x, y));
	// 			if (marketCardObj != null)
	// 			{
	// 				// localMarketGrid[x, y] = marketCardObj.GetComponent<CardState>();
	// 				localMarketList.Add(marketCardObj.GetComponent<CardState>());
	// 				// Debug.Log(debug + "Added Market Card reference at location " + x + ", " + y);
	// 			}
	// 		} // y
	// 	} // x

	// 	// Initialize localPrices dict
	// 	for (int i = 0; i < localMarketList.Count; i++)
	// 	{
	// 		// if (ResourceInfo.resources.Contains(localMarketList[i].resource))
	// 		// {
	// 		// Get the value of the resource matched
	// 		int index = ResourceInfo.resources.IndexOf(localMarketList[i].resource);
	// 		int value = -1;
	// 		ResourceInfo.prices.TryGetValue(ResourceInfo.resources[index], out value);
	// 		// Use that value as well as the resource's associated CardState to add to dict
	// 		// localPrices.Add(localMarketList[i], value);
	// 		localResources.Add(localMarketList[i].resource);
	// 		Debug.Log(debugTag + "Adding " + localMarketList[i].resource + " to the localPrices...");
	// 		localPrices.Add(value);
	// 		// }
	// 	}

	// } // InitLocalMarketGrid()

	private void UpdateLocalPrices()
	{
		// Debug.Log(debug + this.lastKnownPriceList);
		Debug.Log(debugTag + matchDataBroadcaster.PriceListStr);
		this.lastKnownPriceList = matchDataBroadcaster.PriceListStr;

		string[] resourcesWithPrices = this.lastKnownPriceList.Split('_');

		// List<CardState> tempCardStateList = new List<CardState>();

		// for (int i = 0; i < this.localPrices.Count; i++)
		// {
		// 	tempCardStateList.Add(this.localPrices[i]);
		// }

		for (int i = 0; i < resourcesWithPrices.Length; i++)
		{
			string[] tempResPriceHolder = resourcesWithPrices[i].Split('=');
			// Debug.Log(debug + tempResPriceHolder[0] + "-" + tempResPriceHolder[1]);
			string resource = tempResPriceHolder[0];
			int price = int.Parse(tempResPriceHolder[1]);

			int resourceIndex = localResources.IndexOf(resource);
			localPrices[resourceIndex] = price;
		}
	} // UpdateLocalPrices()

	private void UpdateMarketFooters()
	{
		Debug.Log(debugTag + this.lastKnownPriceList);
		Debug.Log(debugTag + matchDataBroadcaster.PriceListStr);

		for (int i = 0; i < this.localMarketList.Count; i++)
		{
			// Debug.Log(debugTag.head + this.localMarketList[i].CardObject.name);
			// this.localMarketList[i].FooterValue = this.localPrices[this.localResources.IndexOf(this.localMarketList[i].Resource)];
			// this.localMarketList[i].CardObject.GetComponent<CardViewController>().FooterValue = this.localPrices[this.localResources.IndexOf(this.localMarketList[i].Resource)];
			// this.localMarketList[i].CardObject.GetComponent<CardViewController>().UpdateFooter();
		}
	} // UpdateMarketFooters()

	private void InitLocalResourcePrices()
	{
		for (int i = 0; i < ResourceInfo.resources.Count; i++)
		{
			int tempPrice;
			ResourceInfo.prices.TryGetValue(ResourceInfo.resources[i], out tempPrice);
			localPrices.Add(tempPrice);

			localResources.Add(ResourceInfo.resources[i]);
		}

		for (int x = 0; x < gridController.KnownMarketOwnersGrid.GetLength(0); x++)
		{
			for (int y = 0; y < gridController.KnownMarketOwnersGrid.GetLength(1); y++)
			{
				if (gridController.KnownMarketOwnersGrid[x, y] != null)
				{
					localMarketList.Add(gridController.KnownMarketOwnersGrid[x, y]);
				}
			}
		}
	}

	// COMMANDS ####################################################################################

	[Command]
	private void CmdGetHandCard(int id, int index)
	{
		TargetLoadHandCard(FindObjectOfType<MatchController>().GetHandCard(id, index), index);
	}

	[TargetRpc]
	private void TargetLoadHandCard(string hand, int index)
	{
		this.hand.Add(JsonUtility.FromJson<Card>(hand));
		string cardString = CardUtility.CreateCardObjectName("GameCard", this.id, index);
		// Debug.Log("Looking for " + cardString);
		GameObject.Find(cardString).GetComponent<CardViewController>().Card = this.hand[index];
	}

	// Fetches the username stored in PlayerDataContainer and ships it off to the server.
	private void InitUsername()
	{
		username = PlayerDataContainer.Username;
		CmdFetchUsername(id, username);
	}

	// Commands the Server to associate the player's requested username with their id.
	[Command]
	private void CmdFetchUsername(int id, string name)
	{
		Debug.Log(debugTag + "I've been told to assign matchController my username! : " + name);
		FindObjectOfType<MatchController>().AssignUsername(name);
	}

	// [TargetRpc]
	// public void TargetGetUsername()
	// {
	// 	Debug.Log("Giving up my username of " + username);
	// }
}

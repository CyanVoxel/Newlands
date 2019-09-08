// The class responsible for representing a Player Connection over the network. Required by Mirror.

// using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SyncListCardData : SyncList<CardData> { }

// public class SyncListCoordinate2 : SyncList<Coordinate2> { }

public class PlayerConnection : NetworkBehaviour
{
// 	// DATA FIELDS #################################################################################
// 	#region DATA FIELDS

// 	// Public ==================================================================
// 	public GameManager gameMan;
// 	public GridManager gridMan;
// 	// public MouseManager localMouseMan;
// 	// public GuiManager guiMan;
// 	public GameObject mouseManPrefab;
// 	public SyncListCardData hand;

// 	// Private =================================================================
// 	private static DebugTag debug = new DebugTag("PlayerConnection", "2196F3");
// 	[SyncVar(hook = "OnIdChange")]
// 	[SerializeField]
// 	private int id = -1;
// 	[SyncVar]
// 	private bool initIdFlag = false;
// 	private int lastKnownTurn = -1;
// 	private int lastKnownRound = -1;
// 	private int lastKnownPhase = -1;
// 	private string lastKnownTopCard = "";
// 	private string lastKnownPriceList = "";
// 	// An array of Lists containing coordinates of all known Tiles owned by all players.
// 	// This is used for quicker access of THESE CARDS.
// 	private List<Coordinate2>[] knownOwnersList;
// 	// A 2-Dimensional array containing the location of all known Tiles owned by all players.
// 	// This is used for quicker access to NEIGHBOR CARDS.
// 	private int[, ] knownOwnersGrid;
// 	// A 2-Dimensional array of GridUnits used for keeping references of the Market Cards.
// 	// Used for acessing the cards' CardStates to update their footer text.
// 	List<CardState> localMarketList = new List<CardState>();
// 	// private Dictionary<CardState, int> localPrices = new Dictionary<CardState, int>();
// 	private List<int> localPrices = new List<int>();
// 	private List<string> localResources = new List<string>();
// 	private string turnEventStr;

// 	#endregion

// 	// METHODS #####################################################################################
// 	#region METHODS

// 	// Start is called before the first frame update.
// 	void Start()
// 	{
// 		if (!isLocalPlayer)
// 		{
// 			return;
// 		}

// 		this.hand.Callback += OnHandUpdated;
// 		// connection = connectionToClient;

// 		if (TryToGrabComponents())
// 		{
// 			if (!initIdFlag)
// 			{
// 				InitId();
// 			}

// 			// CmdSpawnMouseManager();

// 			CmdGetHand();
// 			// Debug.Log(debug + "Hand size: " + this.hand.Count);
// 			// gridMan.CreateHandObjects(this.id, this.hand);
// 			this.knownOwnersList = new List<Coordinate2>[GameManager.playerCount];
// 			this.knownOwnersGrid = new int[GameManager.width, GameManager.height];

// 			for (int i = 0; i < GameManager.playerCount; i++)
// 			{
// 				this.knownOwnersList[i] = new List<Coordinate2>();
// 			}

// 			InitLocalMarketGrid();
// 			UpdateKnownInfo();

// 			// If this is Player 1 and it's the first Turn of the first Round
// 			if (this.id == 1 && this.lastKnownTurn == 1
// 				&& this.lastKnownRound == 1
// 				&& this.lastKnownPhase == 1)
// 			{
// 				CardAnimations.HighlightCards(GetUnownedCards(), this.id);
// 			}
// 		}
// 		else
// 		{
// 			Debug.LogError(debug + "ERROR: Could not grab all components!");
// 		}

// 	} //Start()

// 	void Update()
// 	{
// 		if (!isLocalPlayer)
// 		{
// 			return;
// 		}

// 		if (this.turnEventStr != gameMan.turnEventBroadcast)
// 		{
// 			HandleEvent();
// 			// this.turnEventStr = gameMan.turnEventBroadcast; // Unnecessary, this gets done later.
// 		}

// 		if (this.lastKnownPriceList != gameMan.priceListStr)
// 		{
// 			Debug.Log(debug + "Updating prices...");
// 			UpdateLocalPrices();
// 			UpdateMarketFooters();
// 			// this.lastKnownPriceList = gameMan.priceListStr; // Unnecessary, this gets done later.
// 		}

// 		// Highlight cards during Buying Phase
// 		if (PhaseCheck(1))
// 		{
// 			if (gameMan.turn == this.id)
// 			{
// 				if (gameMan.round > GameManager.graceRounds)
// 				{
// 					Debug.Log(debug + "Highlighting " + GetNeighbors().Count
// 						+ " when there's " + GetUnownedCards().Count
// 						+ " card(s) left");
// 					CardAnimations.HighlightCards(GetNeighbors(), this.id);
// 				}
// 				else
// 				{
// 					CardAnimations.HighlightCards(GetUnownedCards(), this.id);
// 				}
// 			}
// 			else
// 			{
// 				CardAnimations.HighlightCards(GetUnownedCards(), 0);
// 			}
// 		}

// 		// This is at the bottom so handlers can compare old data with new data
// 		UpdateKnownInfo();
// 	} // Update()

// 	// Tries to grab necessary components if they haven't been already.
// 	// Returns true if all components were verified to be grabbed.
// 	private bool TryToGrabComponents()
// 	{
// 		if (this.gameMan == null)
// 		{
// 			this.gameMan = FindObjectOfType<GameManager>();
// 		}

// 		if (this.gridMan == null)
// 		{
// 			this.gridMan = FindObjectOfType<GridManager>();
// 		}

// 		// if (this.guiMan == null)
// 		// {
// 		// 	this.guiMan = FindObjectOfType<GuiManager>();
// 		// }

// 		if (this.gameMan == null)return false;
// 		if (this.gridMan == null)return false;
// 		// if (this.guiMan == null) return false;
// 		return true;
// 	} // GrabComponents()

// 	// Fires when this PlayerConnection's ID changes.
// 	private void OnIdChange(int newId)
// 	{
// 		this.id = newId;
// 		this.transform.name = "Player (" + this.id + ")";
// 		// mouseManObj.transform.name = "MouseManager (" + this.id + ")";
// 		GameManager.localPlayerId = this.id;
// 	} // OnIdChange()

// 	private List<Coordinate2> GetNeighbors()
// 	{
// 		List<Coordinate2> cardsToSend = new List<Coordinate2>();

// 		// Adds all C2s of cards owned by this player to the list
// 		for (int k = 0; k < this.knownOwnersList[this.id - 1].Count; k++)
// 		{
// 			for (int i = -1; i <= 1; i++)
// 			{
// 				for (int j = -1; j <= 1; j++)
// 				{
// 					int x = this.knownOwnersList[this.id - 1][k].x;
// 					int y = this.knownOwnersList[this.id - 1][k].y;

// 					if (x + i >= 0
// 						&& x + i < this.knownOwnersGrid.GetLength(0)
// 						&& y + j >= 0
// 						&& y + j < this.knownOwnersGrid.GetLength(1))
// 					{
// 						if (this.knownOwnersGrid[x + i, y + j] == 0
// 							&& !cardsToSend.Contains(new Coordinate2((x + i), (y + j))))
// 						{
// 							cardsToSend.Add(new Coordinate2((x + i), (y + j)));
// 							// Debug.Log(debug + "Adding card to add [" + (x + i) + ", " + (y + j) + "]");
// 						}
// 					}
// 				}
// 			}
// 		}

// 		return cardsToSend;
// 	} // GetNeighbors()

// 	private List<Coordinate2> GetUnownedCards()
// 	{
// 		List<Coordinate2> cardsToSend = new List<Coordinate2>();

// 		for (int x = 0; x < this.knownOwnersGrid.GetLength(0); x++)
// 		{
// 			for (int y = 0; y < this.knownOwnersGrid.GetLength(1); y++)
// 			{
// 				if (this.knownOwnersGrid[x, y] == 0)
// 				{
// 					cardsToSend.Add(new Coordinate2(x, y));
// 				}
// 			}
// 		}

// 		return cardsToSend;
// 	} // GetUnownedCards()

// 	// Initializes this PlayerConnection's ID by asking the Server to assign one.
// 	private void InitId()
// 	{
// 		TryToGrabComponents();

// 		// Debug.Log(debug + "Chainging default id of "
// 		// 	+ this.id
// 		// 	+ " to " + gameMan.GetPlayerIndex());

// 		this.id = gameMan.GetPlayerIndex();
// 		gameMan.IncrementPlayerIndex();
// 		// mouseManObj.transform.name = "MouseManager (" + this.id + ")";

// 		Debug.Log(debug + "Assigned ID of " + this.id);

// 		// Check for authority or else it'll try to spawn an extra one
// 		if (hasAuthority)
// 		{
// 			CmdSpawnMouseManager();
// 		}

// 		// Debug.Log(debug + "Verifying new PlayerIndex: "
// 		// 	+ gameMan.GetPlayerIndex());

// 		// this.transform.name = "Player (" + this.id + ")";

// 		// Debug.Log(debug + "GameManager.handSize =  "
// 		// 	+ GameManager.handSize
// 		// 	+ " for player " + this.id);

// 		this.initIdFlag = true;
// 	} // InitId()

// 	// public void DestroyCard(string objectName)
// 	// {
// 	// 	Debug.Log(debug + "Trying to destroy " + objectName);
// 	// 	Destroy(GameObject.Find(objectName));
// 	// }

// 	// Fire's when this Player's hand of cards updates
// 	private void OnHandUpdated(SyncListCardData.Operation op, int index, CardData card)
// 	{
// 		if (!isLocalPlayer)
// 		{
// 			return;
// 		}

// 		// Debug.Log(debug + "Index: " + index);

// 		if (index == (GameManager.handSize - 1) && op == SyncListCardData.Operation.OP_ADD)
// 		{
// 			gridMan.CreateHandObjects(this.id, this.hand);
// 		}
// 	} // OnHandUpdated()

// 	// Checks of operations should be performed, given a phase to watch for
// 	private bool PhaseCheck(int phase)
// 	{
// 		if ((gameMan.turn != this.lastKnownTurn
// 				|| gameMan.round != this.lastKnownRound)
// 			&& gameMan.phase == phase)
// 		{
// 			return true;
// 		}
// 		else
// 		{
// 			return false;
// 		}
// 	} // PhaseCheck()

// 	private void UpdateKnownInfo()
// 	{
// 		this.lastKnownTurn = gameMan.turn;
// 		this.lastKnownRound = gameMan.round;
// 		this.lastKnownPhase = gameMan.phase;
// 		this.turnEventStr = gameMan.turnEventBroadcast;
// 		this.lastKnownTopCard = gameMan.topCardStr;
// 		this.lastKnownPriceList = gameMan.priceListStr;
// 	} // UpdateKnownRounds()

// 	// TODO: Rename to something more descriptive, or expand functionality.
// 	private void HandleEvent()
// 	{
// 		GameObject cardObj;
// 		//Handle an event during Phase 2 on your turn
// 		this.turnEventStr = gameMan.turnEventBroadcast;
// 		TurnEvent turnEvent = JsonUtility.FromJson<TurnEvent>(this.turnEventStr);
// 		Debug.Log(debug + "TurnEvent: " + turnEvent);

// 		// Check if the message should be addressed by this player
// 		// if (turnEvent.phase != phase || turnEvent.playerId != this.id)
// 		// {
// 		// 	return;
// 		// }

// 		// Operations ==========================================================

// 		switch (turnEvent.operation)
// 		{
// 			case "Play":
// 				cardObj = GameObject.Find(GameManager.CreateCardObjectName(turnEvent.cardType,
// 					turnEvent.x, turnEvent.y));
// 				if (cardObj != null)
// 				{
// 					Debug.Log(debug + "Trying to destroy " + cardObj.name);
// 					Destroy(cardObj);
// 					if (turnEvent.topCard != "empty")
// 					{
// 						// this.hand[turnEvent.y] = JsonUtility.FromJson<CardData>(turnEvent.topCard);
// 						CreateNewCardObject(turnEvent.y, turnEvent.topCard);
// 					}
// 					else
// 					{
// 						Debug.Log(debug + "GameCard deck must be empty!");
// 					}

// 				}
// 				else
// 				{
// 					Debug.Log(debug + "Could not find " + GameManager.CreateCardObjectName(turnEvent.cardType,
// 						turnEvent.x, turnEvent.y));
// 				}

// 				// TODO: Add code to refresh a market card's footer value if a card was played on it.

// 				break;
// 			case "Buy":
// 				// Add bought Tile to local knowledge base
// 				this.knownOwnersGrid[turnEvent.x, turnEvent.y] = turnEvent.playerId;
// 				this.knownOwnersList[turnEvent.playerId - 1].Add(new Coordinate2(turnEvent.x, turnEvent.y));

// 				// Grab the Tile GameObject that was bought
// 				Debug.Log(debug + GameManager.CreateCardObjectName("Tile",
// 					turnEvent.x,
// 					turnEvent.y));
// 				cardObj = GameObject.Find(GameManager.CreateCardObjectName("Tile",
// 					turnEvent.x,
// 					turnEvent.y));

// 				// Depending on the player who bought the tile, change the Tile's color.
// 				// NOTE: Move to CardAnimations or something.
// 				switch (turnEvent.playerId)
// 				{
// 					case 1:
// 						cardObj.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintRed500;
// 						cardObj.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintRed500;
// 						break;
// 					case 2:
// 						cardObj.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintBlueLight500;
// 						cardObj.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintBlueLight500;
// 						break;
// 					default:
// 						break;
// 				}

// 				break;
// 			default:
// 				break;
// 		}
// 	}

// 	private void CreateNewCardObject(int index, string cardStr)
// 	{
// 		float xOff = index * 11 + (((GameManager.width - GameManager.handSize) / 2f) * 11);
// 		float yOff = -10;

// 		// If old card exists, destroy it
// 		GameObject oldCardObj = GameObject.Find(GameManager.CreateCardObjectName("GameCard", this.id, index));
// 		if (oldCardObj != null)
// 		{
// 			Debug.Log(debug + "Trying to destroy " + oldCardObj.name);
// 			Destroy(oldCardObj);
// 		}

// 		// Create new card
// 		GameObject cardObj = (GameObject)Instantiate(gridMan.gameCardPrefab,
// 			new Vector3(xOff, yOff, 40),
// 			Quaternion.identity);

// 		// Debug.Log("[GridManager] Trying to fill out Hand Card info...");
// 		CardState cardState = cardObj.GetComponent<CardState>();

// 		if (cardState != null)
// 		{
// 			// Generate and Push the string of the object's name
// 			cardState.objectName = (GameManager.CreateCardObjectName("GameCard", this.id, index));
// 			// Push new values to the CardState to be synchronized across the network
// 			GridManager.FillOutCardState(JsonUtility.FromJson<CardData>(cardStr), ref cardState);
// 		}
// 		else
// 		{
// 			Debug.Log(debug + "This object's card state was null!");
// 		} // if (cardState != null)
// 	} // CreateNewCardObject()

// 	private void InitLocalMarketGrid()
// 	{
// 		// Container used for storing a found Market Card.
// 		GameObject marketCardObj;
// 		int marketWidth = (int)Mathf.Ceil((float)ResourceInfo.resources.Count / (float)GameManager.height);
// 		int marketHeight = GameManager.height;
// 		// Initialize the local Market Grid
// 		// CardState[,] localMarketGrid = new CardState[marketWidth, marketHeight];
// 		// List<CardState> localMarketList = new List<CardState>();
// 		// localPrices = new int[localMarketList.Count];
// 		// localResources = new string[localMarketList.Count];

// 		for (int x = 0; x < marketWidth; x++)
// 		{
// 			for (int y = 0; y < marketHeight; y++)
// 			{
// 				marketCardObj = GameObject.Find(GameManager.CreateCardObjectName("MarketCard", x, y));
// 				if (marketCardObj != null)
// 				{
// 					// localMarketGrid[x, y] = marketCardObj.GetComponent<CardState>();
// 					localMarketList.Add(marketCardObj.GetComponent<CardState>());
// 					// Debug.Log(debug + "Added Market Card reference at location " + x + ", " + y);
// 				}
// 			} // y
// 		} // x

// 		// Initialize localPrices dict
// 		for (int i = 0; i < localMarketList.Count; i++)
// 		{
// 			// if (ResourceInfo.resources.Contains(localMarketList[i].resource))
// 			// {
// 			// Get the value of the resource matched
// 			int index = ResourceInfo.resources.IndexOf(localMarketList[i].resource);
// 			int value = -1;
// 			ResourceInfo.prices.TryGetValue(ResourceInfo.resources[index], out value);
// 			// Use that value as well as the resource's associated CardState to add to dict
// 			// localPrices.Add(localMarketList[i], value);
// 			localResources.Add(localMarketList[i].resource);
// 			Debug.Log(debug + "Adding " + localMarketList[i].resource + " to the localPrices...");
// 			localPrices.Add(value);
// 			// }
// 		}

// 	} // InitLocalMarketGrid()

// 	private void UpdateLocalPrices()
// 	{
// 		// Debug.Log(debug + this.lastKnownPriceList);
// 		Debug.Log(debug + gameMan.priceListStr);
// 		this.lastKnownPriceList = gameMan.priceListStr;

// 		string[] resourcesWithPrices = this.lastKnownPriceList.Split('_');

// 		// List<CardState> tempCardStateList = new List<CardState>();

// 		// for (int i = 0; i < this.localPrices.Count; i++)
// 		// {
// 		// 	tempCardStateList.Add(this.localPrices[i]);
// 		// }

// 		for (int i = 0; i < resourcesWithPrices.Length; i++)
// 		{
// 			string[] tempResPriceHolder = resourcesWithPrices[i].Split('=');
// 			// Debug.Log(debug + tempResPriceHolder[0] + "-" + tempResPriceHolder[1]);
// 			string resource = tempResPriceHolder[0];
// 			int price = int.Parse(tempResPriceHolder[1]);

// 			int resourceIndex = localResources.IndexOf(resource);
// 			localPrices[resourceIndex] = price;
// 		}
// 	} // UpdateLocalPrices()

// 	private void UpdateMarketFooters()
// 	{
// 		Debug.Log(debug + this.lastKnownPriceList);
// 		Debug.Log(debug + gameMan.priceListStr);

// 		for (int i = 0; i < this.localMarketList.Count; i++)
// 		{
// 			this.localMarketList[i].footerValue = this.localPrices[this.localResources.IndexOf(this.localMarketList[i].resource)];
// 		}
// 	} // UpdateMarketFooters()

// 	#endregion

// 	// COMMANDS ####################################################################################
// 	#region COMMANDS

// 	// Asks the Server to give this Player a hand of cards
// 	[Command]
// 	public void CmdGetHand()
// 	{
// 		if (!initIdFlag)
// 		{
// 			InitId();
// 		}

// 		// Debug.Log(debug + "[CmdGetHand] GameManager.handSize =  "
// 		// 	+ GameManager.handSize
// 		// 	+ " for player " + this.id);

// 		for (int i = 0; i < GameManager.handSize; i++)
// 		{
// 			// if (GameManager.players[this.id - 1].hand[i] != null)
// 			// {
// 			Card card = GameManager.players[this.id - 1].hand[i];
// 			// Debug.Log(debug + "[CmdGetHand] Adding Card " + i
// 			// 	+ " to SyncList for player " + this.id);
// 			this.hand.Add(new CardData(card));
// 			// }
// 			// else
// 			// {
// 			// 	Debug.LogWarning(debug + "[CmdGetHand] Warning: "
// 			// 		+ "The player did not have a card in slot " + i);
// 			// }
// 		} // for

// 		// Debug.Log(debug + "[CmdGetHand] Finished grabbing "
// 		// 	+ this.hand.Count
// 		// 	+ " cards for player " + this.id);

// 		// TargetCreateHandObjects(connectionToClient, this.hand);
// 	} // CmdGetHand()

// 	// Spawns in a copy of MouseManager with Client Authority and feeds it a reference
// 	// to this PlayerConnection's connection.
// 	[Command]
// 	private void CmdSpawnMouseManager()
// 	{
// 		GameObject mouseManObj = (GameObject)Instantiate(mouseManPrefab,
// 			new Vector3(0, 0, 0),
// 			Quaternion.identity);

// 		// NetworkServer.SpawnWithClientAuthority(mouseManObj, connectionToClient);
// 		// localMouseMan = mouseManObj.GetComponent<MouseManager>();
// 		// localMouseMan.myClient = this.connectionToClient;
// 		// localMouseMan.myPlayerObj = this.gameObject;
// 		// Debug.Log(debug + "Giving MouseManager my ID of " + this.id);
// 		// localMouseMan.ownerId = this.id;
// 	} // CmdSpawnMouseManager()

// 	#endregion
} // class PlayerConnection

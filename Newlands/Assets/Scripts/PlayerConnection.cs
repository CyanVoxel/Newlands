// The class responsible for representing a Player Connection over the network. Required by Mirror.

// using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SyncListCardData : SyncList<CardData> { }

// public class SyncListCoordinate2 : SyncList<Coordinate2> { }

public class PlayerConnection : NetworkBehaviour
{
	// DATA FIELDS #################################################################################
	#region DATA FIELDS

	// Public ==================================================================
	public GameManager gameMan;
	public GridManager gridMan;
	// public GuiManager guiMan;
	public GameObject mouseManPrefab;
	public SyncListCardData hand;

	// Private =================================================================
	private static DebugTag debug = new DebugTag("PlayerConnection", "2196F3");
	[SyncVar(hook = "OnIdChange")]
	[SerializeField]
	private int id = -1;
	[SyncVar]
	private bool initIdFlag = false;
	[SyncVar]
	private int lastKnownTurn = -1;
	[SyncVar]
	private int lastKnownRound = -1;
	[SyncVar]
	private int lastKnownPhase = -1;
	// An array of Lists containing coordinates of all known Tiles owned by all players.
	// This is used for quicker access of THESE CARDS.
	private List<Coordinate2>[] knownOwnersList;
	// A 2-Dimensional array containing the location of all known Tiles owned by all players.
	// This is used for quicker access to NEIGHBOR CARDS.
	private int[, ] knownOwnersGrid;

	#endregion

	// METHODS #####################################################################################
	#region METHODS

	// Start is called before the first frame update.
	void Start()
	{
		if (!isLocalPlayer)
		{
			return;
		}

		this.hand.Callback += OnHandUpdated;
		// connection = connectionToClient;

		if (TryToGrabComponents())
		{
			if (!initIdFlag)
			{
				InitId();
			}

			// CmdSpawnMouseManager();

			CmdGetHand();
			// Debug.Log(debug + "Hand size: " + this.hand.Count);
			// gridMan.CreateHandObjects(this.id, this.hand);
			this.knownOwnersList = new List<Coordinate2>[GameManager.playerCount];
			this.knownOwnersGrid = new int[GameManager.width, GameManager.height];

			for (int i = 0; i < GameManager.playerCount; i++)
			{
				this.knownOwnersList[i] = new List<Coordinate2>();
			}

			UpdateKnownRounds();

			// If this is Player 1 and it's the first Turn of the first Round
			if (this.id == 1 && this.lastKnownTurn == 1
				&& this.lastKnownRound == 1
				&& this.lastKnownPhase == 1)
			{
				CardAnimations.HighlightCards(GetUnownedCards(), this.id);
			}
		}
		else
		{
			Debug.LogError(debug + "ERROR: Could not grab all components!");
		}

	} //Start()

	void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}

		// On New Turn
		if (PhaseCheck(1))
		{
			ColorPurchasedTile();
			// If the new turn is the player's turn and is past the grace rounds
			//&& gameMan.turn > GameManager.graceRounds
			if (gameMan.turn == this.id)
			{
				if (gameMan.round > GameManager.graceRounds)
				{
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
		else if (PhaseCheck(2))
		{
			// Phase 2 Operations go here

		}
		UpdateKnownRounds();
	} // Update()

	// Tries to grab necessary components if they haven't been already.
	// Returns true if all components were verified to be grabbed.
	private bool TryToGrabComponents()
	{
		if (this.gameMan == null)
		{
			this.gameMan = FindObjectOfType<GameManager>();
		}

		if (this.gridMan == null)
		{
			this.gridMan = FindObjectOfType<GridManager>();
		}

		// if (this.guiMan == null)
		// {
		// 	this.guiMan = FindObjectOfType<GuiManager>();
		// }

		if (this.gameMan == null)return false;
		if (this.gridMan == null)return false;
		// if (this.guiMan == null) return false;
		return true;
	} // GrabComponents()

	// Fires when this PlayerConnection's ID changes.
	private void OnIdChange(int newId)
	{
		this.id = newId;
		this.transform.name = "Player (" + this.id + ")";
		// mouseManObj.transform.name = "MouseManager (" + this.id + ")";
		GameManager.localPlayerId = this.id;
	} // OnIdChange()

	// TODO: Supplement this with a method that sends a list of coordinate2s to
	// CardAnimations to color all of
	// NOTE: This method currently does a lot more than it should and will be broken up.
	private void ColorPurchasedTile()
	{

		string turnEventStr = gameMan.turnEventBroadcast;
		TurnEvent turnEvent = JsonUtility.FromJson<TurnEvent>(turnEventStr);

		// Register cards and their owners with local List and Grid
		this.knownOwnersGrid[turnEvent.x, turnEvent.y] = turnEvent.playerId;
		this.knownOwnersList[turnEvent.playerId - 1].Add(new Coordinate2(turnEvent.x, turnEvent.y));

		Debug.Log(debug + GameManager.CreateCardObjectName("Tile",
			turnEvent.x,
			turnEvent.y));

		GameObject cardObj = GameObject.Find(GameManager.CreateCardObjectName("Tile",
			turnEvent.x,
			turnEvent.y));

		switch (turnEvent.playerId)
		{
			case 1:
				cardObj.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintRed500;
				cardObj.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintRed500;
				break;
			case 2:
				cardObj.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintBlueLight500;
				cardObj.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintBlueLight500;
				break;
			default:
				break;
		}
	} // ColorOwnedTiles()

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
						&& x + i < this.knownOwnersGrid.GetLength(0)
						&& y + j >= 0
						&& y + j < this.knownOwnersGrid.GetLength(1))
					{
						if (this.knownOwnersGrid[x + i, y + j] == 0
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

		for (int x = 0; x < this.knownOwnersGrid.GetLength(0); x++)
		{
			for (int y = 0; y < this.knownOwnersGrid.GetLength(1); y++)
			{
				if (this.knownOwnersGrid[x, y] == 0)
				{
					cardsToSend.Add(new Coordinate2(x, y));
				}
			}
		}

		return cardsToSend;
	} // GetUnownedCards()

	// Initializes this PlayerConnection's ID by asking the Server to assign one.
	private void InitId()
	{
		TryToGrabComponents();

		// Debug.Log(debug + "Chainging default id of "
		// 	+ this.id
		// 	+ " to " + gameMan.GetPlayerIndex());

		this.id = gameMan.GetPlayerIndex();
		gameMan.IncrementPlayerIndex();
		// mouseManObj.transform.name = "MouseManager (" + this.id + ")";

		Debug.Log(debug + "Assigned ID of " + this.id);

		// Check for authority or else it'll try to spawn an extra one
		if (hasAuthority)
		{
			CmdSpawnMouseManager();
		}

		// Debug.Log(debug + "Verifying new PlayerIndex: "
		// 	+ gameMan.GetPlayerIndex());

		// this.transform.name = "Player (" + this.id + ")";

		// Debug.Log(debug + "GameManager.handSize =  "
		// 	+ GameManager.handSize
		// 	+ " for player " + this.id);

		this.initIdFlag = true;
	} // InitId()

	public void DestroyCard(string objectName)
	{
		Debug.Log(debug + "Trying to destroy " + objectName);
		Destroy(GameObject.Find(objectName));
	}

	// Fire's when this Player's hand of cards updates
	private void OnHandUpdated(SyncListCardData.Operation op, int index, CardData card)
	{
		if (!isLocalPlayer)
		{
			return;
		}

		// Debug.Log(debug + "Index: " + index);

		if (index == (GameManager.handSize - 1) && op == SyncListCardData.Operation.OP_ADD)
		{
			gridMan.CreateHandObjects(this.id, this.hand);
		}
	} // OnHandUpdated()

	// Checks of operations should be performed, given a phase to watch for
	private bool PhaseCheck(int phase)
	{
		if (gameMan.turn != this.lastKnownTurn
			|| gameMan.round != this.lastKnownRound
			&& gameMan.phase == phase)
		{
			return true;
		}
		else
		{
			return false;
		}
	} // PhaseCheck()

	private void UpdateKnownRounds()
	{
		this.lastKnownTurn = gameMan.turn;
		this.lastKnownRound = gameMan.round;
		this.lastKnownPhase = gameMan.phase;
	} // UpdateKnownRounds()

	#endregion

	// COMMANDS ####################################################################################
	#region COMMANDS

	// Asks the Server to give this Player a hand of cards
	[Command]
	public void CmdGetHand()
	{
		if (!initIdFlag)
		{
			InitId();
		}

		// Debug.Log(debug + "[CmdGetHand] GameManager.handSize =  "
		// 	+ GameManager.handSize
		// 	+ " for player " + this.id);

		for (int i = 0; i < GameManager.handSize; i++)
		{
			// if (GameManager.players[this.id - 1].hand[i] != null)
			// {
			CardData card = GameManager.players[this.id - 1].hand[i];
			// Debug.Log(debug + "[CmdGetHand] Adding Card " + i
			// 	+ " to SyncList for player " + this.id);
			this.hand.Add(card);
			// }
			// else
			// {
			// 	Debug.LogWarning(debug + "[CmdGetHand] Warning: "
			// 		+ "The player did not have a card in slot " + i);
			// }
		} // for

		// Debug.Log(debug + "[CmdGetHand] Finished grabbing "
		// 	+ this.hand.Count
		// 	+ " cards for player " + this.id);

		// TargetCreateHandObjects(connectionToClient, this.hand);
	} // CmdGetHand()

	// Spawns in a copy of MouseManager with Client Authority and feeds it a reference
	// to this PlayerConnection's connection.
	[Command]
	private void CmdSpawnMouseManager()
	{
		GameObject mouseManObj = (GameObject)Instantiate(mouseManPrefab,
			new Vector3(0, 0, 0),
			Quaternion.identity);

		NetworkServer.SpawnWithClientAuthority(mouseManObj, connectionToClient);
		MouseManager localMouseMan = mouseManObj.GetComponent<MouseManager>();
		localMouseMan.myClient = this.connectionToClient;
		localMouseMan.myPlayerObj = this.gameObject;
		Debug.Log(debug + "Giving MouseManager my ID of " + this.id);
		localMouseMan.ownerId = this.id;
	} // CmdSpawnMouseManager()

	#endregion
} // class PlayerConnection

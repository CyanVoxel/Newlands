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
	private int[,] knownOwnersGrid;

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
			// Debug.Log(debug.head + "Hand size: " + this.hand.Count);
			// gridMan.CreateHandObjects(this.id, this.hand);
			this.knownOwnersList = new List<Coordinate2>[GameManager.playerCount];
			this.knownOwnersGrid = new int[GameManager.width, GameManager.height];

			for (int i = 0; i < GameManager.playerCount; i++)
			{
				this.knownOwnersList[i] = new List<Coordinate2>();
			}

			this.lastKnownTurn = gameMan.turn;
			this.lastKnownRound = gameMan.round;
			this.lastKnownPhase = gameMan.phase;

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
			Debug.LogError(debug.head + "ERROR: Could not grab all components!");
		}

	} //Start()

	void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}

		// On New Turn
		if (gameMan.turn != this.lastKnownTurn
			|| gameMan.round != this.lastKnownRound
			&& gameMan.phase == 1)
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

			this.lastKnownTurn = gameMan.turn;
			this.lastKnownRound = gameMan.round;
		}
		else if (this.lastKnownPhase != gameMan.phase)
		{
			this.lastKnownPhase = gameMan.phase;
		}
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

		if (this.gameMan == null) return false;
		if (this.gridMan == null) return false;
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
		string tile = gameMan.turnEventBroadcast;

		// Debug.Log(debug.head + tile);

		int pLength = 0;
		int xLength = 0;
		int yLength = 0;

		// Determines the number of zeroes to add
		if (GameManager.playerCount >= 10)
		{
			pLength = 0;
		}
		else
		{
			pLength = 1;
		}
		if (GameManager.width >= 10)
		{
			xLength = 0;
		}
		else
		{
			xLength = 1;
		}
		if (GameManager.height >= 10)
		{
			yLength = 0;
		}
		else
		{
			yLength = 1;
		} // zeroes calc

		int playerId = int.Parse(tile.Substring(0, pLength));
		// Debug.Log(debug.head + tile.Substring(tile.IndexOf("_x") + 2, xLength));
		// Debug.Log(debug.head + tile.Substring(tile.IndexOf("_y") + 2, yLength));
		int locX = int.Parse(tile.Substring(tile.IndexOf("_x") + 2, xLength));
		int locY = int.Parse(tile.Substring(tile.IndexOf("_y") + 2, yLength));
		// Register cards and their owners with local List and Grid
		this.knownOwnersGrid[locX, locY] = playerId;
		this.knownOwnersList[playerId - 1].Add(new Coordinate2(locX, locY));

		string xZeroes = "0";
		string yZeroes = "0";

		// Determines the number of zeroes to add in the object name
		if (GameManager.width >= 10)
		{
			xZeroes = "";
		}
		else
		{
			xZeroes = "0";
		}
		if (GameManager.height >= 10)
		{
			yZeroes = "";
		}
		else
		{
			yZeroes = "0";
		} // zeroes calc

		Debug.Log(debug.head + "x" + xZeroes + locX + "_"
			+ "y" + yZeroes + locY + "_"
			+ "Tile" + " - " + tile);

		GameObject cardObj = GameObject.Find("x" + xZeroes + locX + "_"
			+ "y" + yZeroes + locY + "_"
			+ "Tile");

		switch (playerId)
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
							// Debug.Log(debug.head + "Adding card to add [" + (x + i) + ", " + (y + j) + "]");
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

		// Debug.Log(debug.head + "Chainging default id of "
		// 	+ this.id
		// 	+ " to " + gameMan.GetPlayerIndex());

		this.id = gameMan.GetPlayerIndex();
		gameMan.IncrementPlayerIndex();
		// mouseManObj.transform.name = "MouseManager (" + this.id + ")";

		Debug.Log(debug.head + "Assigned ID of " + this.id);

		// Check for authority or else it'll try to spawn an extra one
		if (hasAuthority)
		{
			CmdSpawnMouseManager();
		}

		// Debug.Log(debug.head + "Verifying new PlayerIndex: "
		// 	+ gameMan.GetPlayerIndex());

		// this.transform.name = "Player (" + this.id + ")";

		// Debug.Log(debug.head + "GameManager.handSize =  "
		// 	+ GameManager.handSize
		// 	+ " for player " + this.id);

		this.initIdFlag = true;
	} // InitId()

	// Fire's when this Player's hand of cards updates
	private void OnHandUpdated(SyncListCardData.Operation op, int index, CardData card)
	{
		if (!isLocalPlayer)
		{
			return;
		}

		// Debug.Log(debug.head + "Index: " + index);

		if (index == (GameManager.handSize - 1) && op == SyncListCardData.Operation.OP_ADD)
		{
			gridMan.CreateHandObjects(this.id, this.hand);
		}
	} // OnHandUpdated()

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

		// Debug.Log(debug.head + "[CmdGetHand] GameManager.handSize =  "
		// 	+ GameManager.handSize
		// 	+ " for player " + this.id);

		for (int i = 0; i < GameManager.handSize; i++)
		{
			if (GameManager.players[this.id - 1].hand[i] != null)
			{
				CardData card = new CardData(GameManager.players[this.id - 1].hand[i]);
				// Debug.Log(debug.head + "[CmdGetHand] Adding Card " + i
				// 	+ " to SyncList for player " + this.id);
				this.hand.Add(card);
			}
			else
			{
				Debug.LogWarning(debug.head + "[CmdGetHand] Warning: "
					+ "The player did not have a card in slot " + i);
			}
		} // for

		// Debug.Log(debug.head + "[CmdGetHand] Finished grabbing "
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
		Debug.Log(debug.head + "Giving MouseManager my ID of " + this.id);
		localMouseMan.ownerId = this.id;
	} // CmdSpawnMouseManager()

	#endregion
} // class PlayerConnection

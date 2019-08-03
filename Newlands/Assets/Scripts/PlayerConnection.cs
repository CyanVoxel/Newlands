// The class responsible for representing a Player Connection over the network. Required by Mirror.

// using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SyncListCardData : SyncList<CardData> { }

public class SyncListCoordinate2 : SyncList<Coordinate2> { }

public class PlayerConnection : NetworkBehaviour {

	// DATA FIELDS #################################################################################
	#region DATA FIELDS

	// Public ==================================================================
	public GameManager gameMan;
	public GridManager gridMan;
	// public GuiManager guiMan;
	public GameObject mouseManPrefab;
	public SyncListCardData hand;

	// A local copy of Tiles known to be owned by this player
	public SyncListCoordinate2 ownedTiles;

	// Private =================================================================
	private static DebugTag debug = new DebugTag("PlayerConnection", "2196F3");
	[SyncVar(hook = "OnIdChange")][SerializeField]
	private int id = -1;
	[SyncVar]
	private bool initIdFlag = false;
	// public static NetworkConnection connection;
	// [SyncVar(hook = "OnMouseManObjChange")]
	// private GameObject mouseManObj;

	#endregion

	// METHODS #####################################################################################
	#region METHODS

	// Start is called before the first frame update.
	void Start() {

		if (!isLocalPlayer) {
			return;
		}

		this.hand.Callback += OnHandUpdated;
		this.ownedTiles.Callback += OnOwnedTilesUpdated;
		// connection = connectionToClient;

		if (TryToGrabComponents()) {

			if (!initIdFlag) {
				InitId();
			}

			// CmdSpawnMouseManager();

			CmdGetHand();
			// Debug.Log(debug.head + "Hand size: " + this.hand.Count);
			// gridMan.CreateHandObjects(this.id, this.hand);

		} else {
			Debug.LogError(debug.head + "ERROR: Could not grab all components!");
		}

	} //Start()

	// Tries to grab necessary components if they haven't been already.
	// Returns true if all components were verified to be grabbed.
	private bool TryToGrabComponents() {

		if (this.gameMan == null) {
			this.gameMan = FindObjectOfType<GameManager>();
		}

		if (this.gridMan == null) {
			this.gridMan = FindObjectOfType<GridManager>();
		}

		// if (this.guiMan == null) {
		// 	this.guiMan = FindObjectOfType<GuiManager>();
		// }

		if (this.gameMan == null)return false;
		if (this.gridMan == null)return false;
		// if (this.guiMan == null) return false;
		return true;

	} // GrabComponents()

	// Fires when this PlayerConnection's ID changes.
	private void OnIdChange(int newId) {

		this.id = newId;
		this.transform.name = "Player (" + this.id + ")";
		// mouseManObj.transform.name = "MouseManager (" + this.id + ")";
		GameManager.localPlayerId = this.id;

	} // OnIdChange()

	// Initializes this PlayerConnection's ID by asking the Server to assign one.
	private void InitId() {

		TryToGrabComponents();

		// Debug.Log(debug.head + "Chainging default id of "
		// 	+ this.id
		// 	+ " to " + gameMan.GetPlayerIndex());

		this.id = gameMan.GetPlayerIndex();
		gameMan.IncrementPlayerIndex();
		// mouseManObj.transform.name = "MouseManager (" + this.id + ")";

		Debug.Log(debug.head + "Assigned ID of " + this.id);

		// Check for authority or else it'll try to spawn an extra one
		if (hasAuthority) {
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
	private void OnHandUpdated(SyncListCardData.Operation op, int index, CardData card) {

		if (!isLocalPlayer) {
			return;
		}

		// Debug.Log(debug.head + "Index: " + index);

		if (index == (GameManager.handSize - 1) && op == SyncListCardData.Operation.OP_ADD) {
			gridMan.CreateHandObjects(this.id, this.hand);
		}

	} // OnHandUpdated()

	private void OnOwnedTilesUpdated(SyncListCoordinate2.Operation op, int index, Coordinate2 card) {

		if (!isLocalPlayer) {
			return;
		}

		string xZeroes = "0";
		string yZeroes = "0";

		// Determines the number of zeroes to add in the object name
		if (GameManager.width >= 10) {
			xZeroes = "";
		} else {
			xZeroes = "0";
		}
		if (GameManager.height >= 10) {
			yZeroes = "";
		} else {
			yZeroes = "0";
		} // zeroes calc

		Debug.Log(debug.head + "I'm the proud owner of card " + card);

		GameObject tileToColor = GameObject.Find("x" + xZeroes + card.x + "_"
			+ "y" + yZeroes + card.y + "_"
			+ "Tile");

		if (tileToColor != null) {
			tileToColor.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintBlueLight500;
			tileToColor.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintBlueLight500;
		} else {
			Debug.LogError(debug.error + "Could not find tile " + "x" + xZeroes + card.x + "_"
				+ "y" + yZeroes + card.y + "_"
				+ "Tile");
		}

	} // OnHandUpdated()

	// private void OnMouseManObjChange(GameObject mouseManObj) {

	// 	if (this.mouseManObj == null) {
	// 		return;
	// 	}

	// 	MouseManager localMouseMan = mouseManObj.GetComponent<MouseManager>();
	// 	localMouseMan.ownerID = this.id;

	// } // OnMouseManObjChange()

	#endregion

	// COMMANDS ####################################################################################
	#region COMMANDS

	// Asks the Server to give this Player a hand of cards
	[Command]
	public void CmdGetHand() {

		if (!initIdFlag) {
			InitId();
		}

		// Debug.Log(debug.head + "[CmdGetHand] GameManager.handSize =  "
		// 	+ GameManager.handSize
		// 	+ " for player " + this.id);

		for (int i = 0; i < GameManager.handSize; i++) {
			if (GameManager.players[this.id - 1].hand[i] != null) {
				CardData card = new CardData(GameManager.players[this.id - 1].hand[i]);
				// Debug.Log(debug.head + "[CmdGetHand] Adding Card " + i
				// 	+ " to SyncList for player " + this.id);
				this.hand.Add(card);
			} else {
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
	private void CmdSpawnMouseManager() {

		GameObject mouseManObj = (GameObject)Instantiate(mouseManPrefab,
			new Vector3(0, 0, 0),
			Quaternion.identity);

		NetworkServer.SpawnWithClientAuthority(mouseManObj, connectionToClient);
		MouseManager localMouseMan = mouseManObj.GetComponent<MouseManager>();
		localMouseMan.myClient = this.connectionToClient;
		localMouseMan.myPlayerObj = this.gameObject;
		Debug.Log(debug.head + "Giving MouseManager my ID of " + this.id);
		localMouseMan.ownerID = this.id;

	} // CmdSpawnMouseManager()

	#endregion

} // PlayerConnection class

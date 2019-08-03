using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SyncListCardData : SyncList<CardData> { }

public class PlayerConnection : NetworkBehaviour {

	public GuiManager guiMan;
	public GameManager gameMan;
	public GridManager gridMan;
	[SyncVar(hook = "OnIdChange")]
	private int id = -1;
	public SyncListCardData hand;
	[SyncVar]
	private bool initIdFlag = false;
	// public static NetworkConnection connection;
	public GameObject mouseManPrefab;
	private static DebugTag debug = new DebugTag("PlayerConnection", "2196F3");

	// Start is called before the first frame update
	void Start() {

		if (!isLocalPlayer) {
			return;
		}

		this.hand.Callback += OnHandUpdated;
		CmdSpawnMouseManager();
		// connection = connectionToClient;

		if (TryToGrabComponents()) {

			if (!initIdFlag) {
				InitId();
			}

			CmdGetHand();
			// Debug.Log(debug.head + "Hand size: " + this.hand.Count);
			// gridMan.CreateHandObjects(this.id, this.hand);

		} else {
			Debug.LogError(debug.head + "ERROR: Could not grab all components!");
		}

	} //Start()

	// // Update is called once per frame
	// void Update() {

	// 	if (!isLocalPlayer) {
	// 		return;
	// 	}

	// 	// Debug.Log(debug.head + "Hand size: " + this.hand.Count);

	// } // Update()

	// Tries to grab necessary components if they haven't been already.
	// Returns true if all components were verified to be grabbed
	private bool TryToGrabComponents() {

		if (this.gameMan == null) {
			this.gameMan = FindObjectOfType<GameManager>();
		}

		if (this.gridMan == null) {
			this.gridMan = FindObjectOfType<GridManager>();
		}

		if (this.guiMan == null) {
			this.guiMan = FindObjectOfType<GuiManager>();
		}

		if (this.gameMan == null) {
			return false;
		} else if (this.gridMan == null) {
			return false;
		} else if (this.guiMan == null) {
			return false;
		} else {
			return true;
		}

	} // GrabComponents()

	// COMMANDS ####################################################################################

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

	[Command]
	private void CmdSpawnMouseManager() {
		GameObject mouseMan = (GameObject)Instantiate(mouseManPrefab,
					new Vector3(0, 0, 0),
					Quaternion.identity);

		NetworkServer.SpawnWithClientAuthority(mouseMan, connectionToClient);
		MouseManager mouse = mouseMan.GetComponent<MouseManager>();
		mouse.myClient = this.connectionToClient;
	}

	private void OnIdChange(int newId) {
		this.id = newId;
		this.transform.name = "Player (" + this.id + ")";
		GameManager.localPlayerId = this.id;
	} // OnIdChange()

	private void InitId() {

		TryToGrabComponents();

		// Debug.Log(debug.head + "Chainging default id of "
		// 	+ this.id
		// 	+ " to " + gameMan.GetPlayerIndex());

		this.id = gameMan.GetPlayerIndex();
		gameMan.IncrementPlayerIndex();

		Debug.Log(debug.head + "Assigned ID of " + this.id);

		// Debug.Log(debug.head + "Verifying new PlayerIndex: "
		// 	+ gameMan.GetPlayerIndex());

		// this.transform.name = "Player (" + this.id + ")";

		// Debug.Log(debug.head + "GameManager.handSize =  "
		// 	+ GameManager.handSize
		// 	+ " for player " + this.id);

		this.initIdFlag = true;

	} // InitId()

	private void OnHandUpdated(SyncListCardData.Operation op, int index, CardData card) {

		if (!isLocalPlayer) {
			return;
		}

		// Debug.Log(debug.head + "Index: " + index);

		if (index == (GameManager.handSize - 1) && op == SyncListCardData.Operation.OP_ADD) {
			gridMan.CreateHandObjects(this.id, this.hand);
		}

	} // OnHandUpdated()

} // PlayerConnection class

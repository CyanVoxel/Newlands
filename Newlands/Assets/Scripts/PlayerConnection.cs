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

	private static string debugH = "<color=#304FFEFF><b>[PlayerConnection] </b></color>";

	// Start is called before the first frame update
	void Start() {

		if (!isLocalPlayer) {
			return;
		}

		hand.Callback += OnHandUpdated;

		if (TryToGrabComponents()) {

			if (!initIdFlag) {
				InitId();
			}

			CmdGetHand();
			// Debug.Log(debugH + "Hand size: " + this.hand.Count);
			// gridMan.CreateHandObjects(this.id, this.hand);

		} else {
			Debug.LogError(debugH + "ERROR: Could not grab all components!");
		}

	} //Start()

	// // Update is called once per frame
	// void Update() {

	// 	if (!isLocalPlayer) {
	// 		return;
	// 	}

	// 	// Debug.Log(debugH + "Hand size: " + this.hand.Count);

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

	// [Command]
	// private void CmdSpawnGrids() {
	// 	TryToGrabComponents();
	// 	gridMan.CreateGameGridObjects();
	// 	gridMan.CreateMarketGridObjects();
	// } // CmdSpawnCards();

	// Increments the PlayerIndex in gameMan
	// [Command]
	// private void CmdUpdateIdIndex() {
	// 	TryToGrabComponents();
	// 	gameMan.IncrementPlayerIndex();
	// 	Debug.Log(debugH + "[CmdUpdateIdIndex] Updating index to "
	// 		+ gameMan.playerIndex
	// 		+ " (Getter: " + gameMan.GetPlayerIndex() + ")");
	// } // CmdIncrementPlayerCount()

	[Command]
	public void CmdGetHand() {

		if (!initIdFlag) {
			InitId();
		}

		Debug.Log(debugH + "[CmdGetHand] GameManager.handSize =  "
			+ GameManager.handSize
			+ " for player " + this.id);

		for (int i = 0; i < GameManager.handSize; i++) {
			if (GameManager.players[this.id].hand[i] != null) {
				CardData card = new CardData(GameManager.players[this.id].hand[i]);
				Debug.Log(debugH + "[CmdGetHand] Adding Card " + i
					+ " to SyncList for player " + this.id);
				this.hand.Add(card);
			} else {
				Debug.LogWarning(debugH + "[CmdGetHand] Warning: "
					+ "The player did not have a card in slot " + i);
			}
		} // for

		Debug.Log(debugH + "[CmdGetHand] Finished grabbing "
			+ this.hand.Count
			+ " cards for player " + this.id);

		// TargetCreateHandObjects(connectionToClient, this.hand);

	} // CmdGetHand()

	private void OnIdChange(int newId) {
		this.id = newId;
		// this.transform.name = "Player (" + this.id + ")";
	} // OnIdChange()

	private void InitId() {

		TryToGrabComponents();

		Debug.Log(debugH + "Chainging default id of "
			+ this.id
			+ " to " + gameMan.GetPlayerIndex());

		this.id = gameMan.GetPlayerIndex();
		gameMan.IncrementPlayerIndex();

		Debug.Log(debugH + "Assigned ID of " + this.id);

		Debug.Log(debugH + "Verifying new PlayerIndex: "
			+ gameMan.GetPlayerIndex());

		// this.transform.name = "Player (" + this.id + ")";

		Debug.Log(debugH + "GameManager.handSize =  "
			+ GameManager.handSize
			+ " for player " + this.id);

		this.initIdFlag = true;

	} // InitId()

	private void OnHandUpdated(SyncListCardData.Operation op, int shrek, CardData card) {

		if (!isLocalPlayer) {
			return;
		}

		Debug.Log(debugH + "Index: " + shrek);

		if (shrek == (GameManager.handSize - 1) && op == SyncListCardData.Operation.OP_ADD) {
			gridMan.CreateHandObjects(this.id, this.hand);
		}

	} // OnHandUpdated()

} // PlayerConnection class

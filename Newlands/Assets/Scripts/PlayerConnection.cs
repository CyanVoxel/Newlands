using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerConnection : NetworkBehaviour {

	public GuiManager guiMan;
	public GameManager gameMan;
	public GridManager gridMan;
	public GameObject PlayerUnit; // The physical GameObject controlled by the player

	// Start is called before the first frame update
	void Start() {

		if (!isLocalPlayer) {
			return;
		}

		CmdSpawnUnit();

		if (TryToGrabComponents()) {
			CmdIncrementPlayerCount();
			CmdSpawnCards();
		} else {
			Debug.LogError("[PlayerConnection] ERROR: Could not grab all components!");
		}

	} //Start()

	// // Update is called once per frame
	// void Update() {

	// 	if (!isLocalPlayer) {
	// 		return;
	// 	}

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

	// COMMANDS ###################################################################################

	[Command]
	private void CmdSpawnUnit() {
		GameObject obj = Instantiate(PlayerUnit);
		NetworkServer.SpawnWithClientAuthority(obj, connectionToClient);
	} // SpawnUnit()

	[Command]
	private void CmdSpawnCards() {
		Debug.Log("[PlayerConnection] Creating Grid GameObjects...");
		gridMan.CreateGameGridObjects();
		gridMan.CreateMarketGridObjects();
	} // CmdSpawnCards();

	[Command]
	private void CmdIncrementPlayerCount() {
		GameManager.IncrementPlayerCount();
	} // CmdIncrementPlayerCount()

} // PlayerConnection class

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

		guiMan = FindObjectOfType<GuiManager>();
		gameMan = FindObjectOfType<GameManager>();
		gridMan = FindObjectOfType<GridManager>();

		if (guiMan != null) {
			// guiMan.CmdUpdateUI();
		} else {
			Debug.LogError("<b>[PlayerConnection]</b> "
				+ "GuiManager is null!");
		}

		if (gameMan != null) {
			Debug.Log("<b>[PlayerConnection]</b> "
				+ "Spawning Game Grid....");
			// gameMan.CreateGridObjects();
			CmdSpawnCards();
		} else {
			Debug.LogError("<b>[PlayerConnection]</b> "
				+ "GameManager is null!");
		}

	} //Start()

	[SyncVar(hook = "OnPlayerNameChange")]
	public string playerName = "PLAYER";

	// Update is called once per frame
	void Update() {

		if (!isLocalPlayer) {
			return;
		}

		if (Input.GetKeyDown(KeyCode.Q)) {
			string newName = "Voxel" + Random.Range(1, 100);
			Debug.Log("Sending request to change name to " + newName);
			CmdChangePlayerName(newName);
		}

	} // Update()

	void OnPlayerNameChange(string name) {

		Debug.Log("Player name was changed from " + playerName + " to " + name);
		gameObject.name = "PlayerConnection(" + name + ")";

	} // OnPlayerNameChange()

	// COMMANDS ###################################################################################

	[Command]
	void CmdSpawnUnit() {

		GameObject obj = Instantiate(PlayerUnit);

		// obj.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);

		NetworkServer.SpawnWithClientAuthority(obj, connectionToClient);

	} // SpawnUnit()

	[Command]
	void CmdChangePlayerName(string name) {

		Debug.Log("[CmdChangePlayerName] " + name);
		playerName = name;
		// RpcChangePlayerName(name);

		// Tell clients new name

	} // CmdChangePlayerName

	[Command]
	void CmdSpawnCards() {
		Debug.Log("[PlayerConnection] Creating Grid GameObjects...");
		gridMan.CmdCreateGridObjects();
		// gameMan.PopulateMarket();
	}

	[Command]
	void CmdDisplayCardInfo() {

	}

	// RPC ########################################################################################

	// [ClientRpc]
	// void RpcChangePlayerName(string name) {

	// 	Debug.Log("[RpcChangePlayerName] Was asked to change name to: " + name);
	// 	playerName = name;

	// }

} // PlayerConnection class

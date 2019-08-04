﻿// A class that manages mouse hit detection

using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : NetworkBehaviour {

	private static DebugTag debug = new DebugTag("MouseManager", "AA00FF");

	private GameManager gameMan;
	// public GuiManager guiMan;
	private GridManager gridMan;
	public static int selection = -1;
	// TODO: Create a dictionary of flags
	private int purchaseSuccessFlag = -1; // -1: Reset | 0: False | 1: True
	public static bool highlightFlag = false; // True if AttemptPurchaseVisuals() already highlighted
	private static GameObject objectHit;
	[SyncVar]
	public int ownerID = -1;

	// [SyncVar]
	public NetworkConnection myClient;
	public GameObject myPlayerObj;

	private int purchaseBufferX = -1;
	private int purchaseBufferY = -1;

	// Containers for object position and rotation info
	private static float objX;
	private static float objY;
	private static float objZ;
	private static float objRotX;
	private static float objRotY;
	private static float objRotZ;

	void Start() {

		// if (!hasAuthority) {
		// 	return;
		// }

		gridMan = FindObjectOfType<GridManager>();
		gameMan = FindObjectOfType<GameManager>();

		// myClient = GameObject.Find("Player (" + gameMan.GetPlayerIndex() + ")").GetComponent<NetworkIdentity>().connectionToClient;
		// this.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);

	} // Start()

	// Update is called once per frame
	void Update() {

		if (!hasAuthority) {
			// Debug.LogWarning(debug.warning + "No Authority!");
			return;
		} else {
			// Debug.Log(debug.head + "Authority!");
		}

		// If the cursor is over a UI element, return from Update()
		// NOTE: In order for Canvases on objects such as Cards to be ignored,
		//	they must contain a "Canvas Group" and have "Block Raycasts" turned off
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}

		if (gameMan == null) {
			gameMan = FindObjectOfType<GameManager>();
			Debug.Log(debug.head + "GameManager was set during update");
		}

		if (gridMan == null) {
			gridMan = FindObjectOfType<GridManager>();
			Debug.Log(debug.head + "GridManager was set during update");
		}

		// Flag Checkers -------------------------------------------------------
		CheckPurchaseSuccess();

		// If 'P' is pressed, end the phase - Debugging only
		if (Input.GetKeyDown(KeyCode.P)) {
			gameMan.EndPhase();
			// guiMan.CmdUpdateUI();
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		// bool highlightAllowed = false;
		// if (gameMan.round > GameManager.graceRounds) {
		// 	highlightAllowed = true;
		// }
		// Debug.Log(debug.head + "Round: " + gameMan.round + "/" + GameManager.graceRounds + " | " + highlightAllowed);
		// Debug.Log(debug.head + "Turn: " + gameMan.turn);
		// if (gameMan.round > GameManager.graceRounds) {
		// 	// gameMan.VerifyHighlight();
		// 	if (!highlightFlag) {
		// 		Debug.Log(debug.head + "Highlighting...");
		// 		gameMan.HighlightNeighbors(gameMan.turn);
		// 		highlightFlag = true;
		// 	}
		// } // if
		////////////////////////////////////////////////////////////////////////////////////////////

		// Initialize ray
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;

		// If an object was hit
		if (Physics.Raycast(ray, out hitInfo)) {

			// Checks if the object hit has a parent before assinging it to a local var.
			if (hitInfo.collider.transform.parent != null) {
				objectHit = hitInfo.collider.transform.parent.gameObject;
			} else {
				objectHit = hitInfo.collider.transform.gameObject;
			}

			// Debug.Log(objectHit.transform.parent.name);

			// Containers for object position and rotation info
			objX = objectHit.transform.parent.position.x;
			objY = objectHit.transform.parent.position.y;
			objZ = objectHit.transform.parent.position.z;

			objRotX = objectHit.transform.parent.rotation.x;
			objRotY = objectHit.transform.parent.rotation.y;
			objRotZ = objectHit.transform.parent.rotation.z;

			// LAND TILES #########################################################################

			if (objectHit.transform.parent.name.Contains("Tile")) {

				// TODO: Write a function that takes in Type and element to search for,
				//	returning the substring. Should be able to return variable lengths.
				//	e.x. GetValue(type: "Tile", element: "x")

				// Grab the grid coordinates stored in the object name
				int locX = int.Parse(objectHit.transform.parent.name.Substring(1, 2));
				int locY = int.Parse(objectHit.transform.parent.name.Substring(5, 2));

				// PHASE 1 ####################################################
				if (gameMan.phase == 1) {

					// Left Click #########################
					if (Input.GetMouseButtonDown(0)) {

						// gameMan.WipeSelectionColors("GameCard", ColorPalette.tintCard);

						////////////////////////////////////////////////////////////////////////////
						// If the grace rounds have passes, start highlighting the neighbors
						// highlightAllowed = false;
						// if (gameMan.round > GameManager.graceRounds) {
						// 	highlightAllowed = true;
						// }
						// Debug.Log(debug.head + "Round: " + gameMan.round + "/" + GameManager.graceRounds + " | " + highlightAllowed);
						// if (gameMan.round > GameManager.graceRounds) {
						// 	gameMan.VerifyHighlight();
						// 	if (!highlightFlag) {
						// 		Debug.Log(debug.head + "Highlighting...");
						// 		gameMan.HighlightNeighbors(gameMan.turn);
						// 		highlightFlag = true;
						// 	}
						// } // if
						////////////////////////////////////////////////////////////////////////////

						selection = -1;
						// guiMan.CmdUpdateUI();

						// If the tile can be bought
						if (gameMan.turn == this.ownerID) {
							Debug.Log(debug.head + "Trying to buy tile!");
							CmdBuyTile(locX, locY);
							this.purchaseBufferX = locX;
							this.purchaseBufferY = locY;
						} else if (this.ownerID == -1) {
							Debug.LogWarning(debug.warning + "This MouseManager has an ownerID of -1!");
						} else {
							Debug.Log(debug.head + "Player " + this.ownerID
								+ " can't buy a tile on Player " + gameMan.turn + "'s Turn!");
						}

						// CallCmdBuyTile(locX, locY);

						////////////////////////////////////////////////////////////////////////////
						// If the grace rounds have passed, start highlighting the neighbors
						// if (gameMan.round > GameManager.graceRounds) {
						// 	highlightAllowed = true;
						// }
						// Debug.Log(debug.head + "Round: " + gameMan.round + "/" + GameManager.graceRounds + " | " + highlightAllowed);
						// if (gameMan.round > GameManager.graceRounds) {
						// 	gameMan.VerifyHighlight();
						// 	if (!highlightFlag) {
						// 		Debug.Log(debug.head + "Highlighting...");
						// 		gameMan.HighlightNeighbors(gameMan.turn);
						// 		highlightFlag = true;
						// 	}
						// } // if
						////////////////////////////////////////////////////////////////////////////
						// guiMan.CmdUpdateUI();

					} // if Left Click

					// Right Click ########################
					if (Input.GetMouseButtonDown(1)) {
						objRotX = objectHit.transform.parent.rotation.x;
						objRotY = objectHit.transform.parent.rotation.y;
						objRotZ = objectHit.transform.parent.rotation.z;

						// gameMan.WipeSelectionColors("GameCard", ColorPalette.tintCard);

						objectHit.transform.parent.rotation = new Quaternion(objRotX, 1 + objRotY, objRotZ, 0);

						objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintCard;
						objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintCard;

						// "Sells" the tile - NOTE: this is for debugging ONLY
						GridManager.grid[locX, locY].ownerId = 0;

						// gameMan.RollbackTurn();
						// guiMan.CmdUpdateUI();

					} // if Right Click

				} else if (gameMan.phase == 2) {
					// PHASE 2 ####################################################################

					// Left Click #########################
					if (Input.GetMouseButtonDown(0)) {

						// gameMan.WipeSelectionColors("GameCard", ColorPalette.tintCard);

						if (selection >= 1) {
							if (gameMan.TryToPlay(GridManager.grid[locX, locY],
									GameManager.players[0].handUnits[selection - 1])) {
								// Debug.Log("Using GameCard " + selection
								// 	+ " on LandTile " + locX + ", " + locY);
								// GameManager.players[0].hand[selection]
							}
						} else {
							return;
						} // if a GameCard is selected

					} // Left Click

				} // Phase

			} // if LandTile

			// GAME CARDS #########################################################################

			if (objectHit.transform.parent.name.Contains("GameCard")) {

				// Grab the grid coordinates stored in the object name
				int locX = int.Parse(objectHit.transform.parent.name.Substring(1, 2));
				int locY = int.Parse(objectHit.transform.parent.name.Substring(5, 2));

				objX = objectHit.transform.parent.position.x;
				objY = objectHit.transform.parent.position.y;
				objZ = objectHit.transform.parent.position.z;

				// Debug.Log(locX + ", " + locY);

				// PHASES 2+ ##################################################
				if (gameMan.phase > 1) {

					// Left Click #########################
					if (Input.GetMouseButtonDown(0)) {

						// If the object clicked was already selected, deselect it
						if (selection == locY) {
							selection = -1;
							objectHit.transform.parent.position = new Vector3(objX, objY, 40f);
						} else {
							selection = locY;
							// gameMan.WipeSelectionColors("GameCard", ColorPalette.tintCard);
							objectHit.transform.parent.position = new Vector3(objX, objY, 38f);
							objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.cyan300;
							objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.cyan300;
						} // if already selected

						if (selection == -1) {
							// gameMan.WipeSelectionColors("GameCard", ColorPalette.tintCard);
						}

						// guiMan.CmdUpdateUI();

					} // if Left Click

				} // Phase 2+

			} // if GameCard

			// MARKET CARDS #######################################################################

			if (objectHit.transform.parent.name.Contains("MarketCard")) {

				// TODO: Write a function that takes in Type and element to search for,
				//	returning the substring. Should be able to return variable lengths.
				//	e.x. GetValue(type: "Tile", element: "x")

				// Grab the grid coordinates stored in the object name
				int locX = int.Parse(objectHit.transform.parent.name.Substring(1, 2));
				int locY = int.Parse(objectHit.transform.parent.name.Substring(5, 2));

				if (gameMan.phase == 2) {
					// PHASE 2 ####################################################################

					// Left Click #########################
					if (Input.GetMouseButtonDown(0)) {

						// gameMan.WipeSelectionColors("GameCard", ColorPalette.tintCard);

						if (selection >= 1) {
							if (gameMan.TryToPlay(GridManager.marketGrid[locX, locY],
									GameManager.players[0].handUnits[selection - 1])) {
								// Debug.Log("Using GameCard " + selection +
								//   " on LandTile " + locX + ", " + locY);
								//GameManager.players[0].hand[selection]
							}
						} else {
							selection = -1;
							// gameMan.WipeSelectionColors("GameCard", ColorPalette.tintCard);
							// guiMan.CmdUpdateUI();
							return;
						} // if a GameCard is selected

					} // Left Click

				} // Phase

			} // if LandTile

		} // if object hit

		//Debug.Log("World Point: " + worldPoint);

		AttemptPurchaseVisuals();

	} // Update()

	private void AttemptPurchaseVisuals() {

		switch (purchaseSuccessFlag) {
			case -1:
				break;

			case 0:
				purchaseSuccessFlag = -1;
				break;

			case 1:
				objX = objectHit.transform.parent.position.x;
				objY = objectHit.transform.parent.position.y;
				objZ = objectHit.transform.parent.position.z;

				objRotX = objectHit.transform.parent.rotation.x;
				objRotY = objectHit.transform.parent.rotation.y;
				objRotZ = objectHit.transform.parent.rotation.z;

				// Changes the material of the card depending on who clicked on it.
				// TODO: Create a method somewhere that changes the desired materials
				//	based on an object given and a playerId/turn
				// TODO: Nice card flip animation
				switch (gameMan.turn) {
					case 1:
						objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.lightBlue300;
						objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.lightBlue300;
						objectHit.transform.parent.rotation = new Quaternion(objRotX, 1 - objRotY, objRotZ, 0);
						gameMan.IncrementTurn();
						break;
					case 2:
						objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.red400;
						objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.red400;
						objectHit.transform.parent.rotation = new Quaternion(objRotX, 1 - objRotY, objRotZ, 0);
						gameMan.IncrementTurn();
						break;
					case 3:
						objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.purple300;
						objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.purple300;
						objectHit.transform.parent.rotation = new Quaternion(objRotX, 1 - objRotY, objRotZ, 0);
						gameMan.IncrementTurn();
						break;
					case 4:
						objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.orange300;
						objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.orange300;
						objectHit.transform.parent.rotation = new Quaternion(objRotX, 1 - objRotY, objRotZ, 0);
						gameMan.IncrementTurn();
						break;

					default:
						break;

				} // switch

				// Update the round/turn display text
				// GameManager.UpdatePlayersInfo();
				// if (!highlightFlag) {
				// 	Debug.Log(debug.head + "Highlighting...");
				// 	gameMan.HighlightNeighbors(gameMan.turn);
				// 	highlightFlag = true;
				// }
				// guiMan.CmdUpdateUI();
				purchaseSuccessFlag = -1;
				break;
			default:
				purchaseSuccessFlag = -1;
				break;
		} // switch (purchaseSuccessFlag)

	} // AttemptPurchaseVisuals()

	private void CheckPurchaseSuccess() {

		switch (this.purchaseSuccessFlag) {
			case -1:
				break;
			case 0:
				// False
				Debug.Log(debug.head + "Purchase Unsuccessful!");
				this.purchaseSuccessFlag = -1;
				break;
			case 1:
				// True
				Debug.Log(debug.head + "Purchase Successful!");
				CmdFlipCard("Tile", this.purchaseBufferX, this.purchaseBufferY);

				// GameObject playerRef = GameObject.Find("Player (" + turn + ")");

				// CmdUmm(this.purchaseBufferX, this.purchaseBufferY);

				this.purchaseSuccessFlag = -1;
				break;
			default:
				Debug.LogWarning(debug.head + "PurchaseSuccessFlag was set to " + this.purchaseSuccessFlag);
				this.purchaseSuccessFlag = -1;
				break;
		} // switch (purchaseSuccessFlag)

	} // CheckPurchaseSuccess()

	private void CallCmdBuyTile(int locX, int locY) {
		this.purchaseBufferX = locX;
		this.purchaseBufferY = locY;
		CmdBuyTile(locX, locY);
	} // CallCmdBuyTile()

	[Command]
	private void CmdFlipCard(string cardType, int locX, int locY) {
		CardAnimations.FlipCard(cardType, locX, locY);
	}

	[Command]
	private void CmdBuyTile(int locX, int locY) {

		// Debug.Log(debug.head + "Is Server? " + isServer);

		bool success = false;
		if (gameMan.BuyTile(locX, locY)) {
			success = true;
		}
		Debug.Log(debug.head + "About to call TargetBuyTile with connection: " + myClient);
		TargetBuyTile(myClient, success);

	} // CmdBuyTile()

	[TargetRpc]
	private void TargetBuyTile(NetworkConnection target, bool success) {

		// if (!isLocalPlayer) {
		// 	return;
		// }

		Debug.Log(debug.head + "Called TargetBuyTile!");
		if (success) {
			purchaseSuccessFlag = 1;
		} else {
			purchaseSuccessFlag = 0;
		}

	} // TargetBuyTile()

} // MouseManager class

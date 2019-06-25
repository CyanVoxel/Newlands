// A class that manages mouse hit detection

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour {

	public GameManager gameMan;
	int selection = -1;
	
	// Update is called once per frame
	void Update() {

		// If 'P' is pressed, end the phase - Debugging only
		if (Input.GetKeyDown(KeyCode.P)) {
			gameMan.EndPhase();
			gameMan.UpdateUI();
		}

		// If the cursor is over a UI element, return from Update()
		// NOTE: In order for Canvases on objects such as Cards to be ignored,
		//	they must contain a "Canvas Group" and have "Block Raycasts" turned off
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}

		// Initialize ray
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;

		// If an object was hit
		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.collider.transform.parent.gameObject;

			// Containers for object position and rotation info
			float objX = objectHit.transform.parent.position.x;
			float objY = objectHit.transform.parent.position.y;
			float objZ = objectHit.transform.parent.position.z;

			float objRotX = objectHit.transform.parent.rotation.x;
			float objRotY = objectHit.transform.parent.rotation.y;
			float objRotZ = objectHit.transform.parent.rotation.z;

			// LAND TILES #########################################################################

			if (objectHit.transform.parent.name.Contains("LandTile")) {

				// Grab the grid coordinates stored in the object name
				byte locX = byte.Parse(objectHit.transform.parent.name.Substring(10, 1));
				byte locY = byte.Parse(objectHit.transform.parent.name.Substring(13, 1));

				// PHASE 1 ####################################################
				if (GameManager.phase == 1) {

					// Left Click #########################
					if (Input.GetMouseButtonDown(0)) {

						// If the grace rounds have passes, start highlighting the neighbors
						if (GameManager.round > GameManager.graceRounds) {
							gameMan.VerifyHighlight();
							gameMan.HighlightNeighbors();
						} // if

						gameMan.WipeSelectionColors("GameCard");	//Deselect any GameCards
						selection = -1;
						gameMan.UpdateUI();

						// If the tile can be bought
						if (gameMan.BuyTile(locX, locY)) {

							objX = objectHit.transform.parent.position.x;
							objY = objectHit.transform.parent.position.y;
							objZ = objectHit.transform.parent.position.z;

							objRotX = objectHit.transform.parent.rotation.x;
							objRotY = objectHit.transform.parent.rotation.y;
							objRotZ = objectHit.transform.parent.rotation.z;
							
							// Changes the material of the card depending on the player who clicked on it.
							// TODO: Create a method somewhere that changes the desired materials
							//	based on an object given and a playerId/turn
							// TODO: Nice card flip animation
							switch (GameManager.turn) {
								case 1: 
									objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.lightBlue300;
									objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.lightBlue300;
									objectHit.transform.parent.rotation = new Quaternion(objRotX, 1-objRotY, objRotZ, 0);
									gameMan.AdvanceTurn();
									break;
								case 2: 
									objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.red400;
									objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.red400;
									objectHit.transform.parent.rotation = new Quaternion(objRotX, 1-objRotY, objRotZ, 0);
									gameMan.AdvanceTurn();
									break;
								case 3: 
									objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.purple300;
									objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.purple300;
									objectHit.transform.parent.rotation = new Quaternion(objRotX, 1-objRotY, objRotZ, 0);
									gameMan.AdvanceTurn();
									break;
								case 4: 
									objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.orange300;
									objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.orange300;
									objectHit.transform.parent.rotation = new Quaternion(objRotX, 1-objRotY, objRotZ, 0);
									gameMan.AdvanceTurn();
									break;
								default: 
									break;

							} // switch

							// Update the round/turn display text
							gameMan.UpdateUI();

						} // if the tile could be bought

						// If the grace rounds have passes, start highlighting the neighbors
						if (GameManager.round > GameManager.graceRounds) {
							gameMan.VerifyHighlight();
							gameMan.HighlightNeighbors();
						} // if
						gameMan.UpdateUI();

					} // if Left Click

					// Right Click ########################
					if (Input.GetMouseButtonDown(1)) {
						objRotX = objectHit.transform.parent.rotation.x;
						objRotY = objectHit.transform.parent.rotation.y;
						objRotZ = objectHit.transform.parent.rotation.z;

						objectHit.transform.parent.rotation = new Quaternion(objRotX, 1+objRotY, objRotZ, 0);

						objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintCard;
						objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintCard;

						// "Sells" the tile - NOTE: this is for debugging ONLY
						GameManager.grid[locX, locY].ownerId = 0;

						gameMan.RollbackTurn();
						gameMan.UpdateUI();

					} // if Right Click

				} else if (GameManager.phase == 2) {
				// PHASE 2 ########################################################################

					// Left Click #########################
					if (Input.GetMouseButtonDown(0)) {

						if (selection >= 0) {
							Debug.Log("Using GameCard " + selection + 
									  " on LandTile " + locX + ", " + locY);
						} else {
							return;
						} // if a GameCard is selected

					} // Left Click

				} // Phase

			} // if LandTile

			// GAME CARDS #########################################################################
			
			if (objectHit.transform.parent.name.Contains("GameCard")) {

				// Grab the grid coordinates stored in the object name
				byte locX = byte.Parse(objectHit.transform.parent.name.Substring(10, 1));
				byte locY = byte.Parse(objectHit.transform.parent.name.Substring(13, 1));

				objX = objectHit.transform.parent.position.x;
				objY = objectHit.transform.parent.position.y;
				objZ = objectHit.transform.parent.position.z;

				// PHASES 2+ ##################################################
				if (GameManager.phase > 1) {

					// Left Click #########################
					if (Input.GetMouseButtonDown(0)) {
						
						// If the object clicked was already selected, deselect it
						if (selection == locY) {
							selection = -1;
							objectHit.transform.parent.position = new Vector3(objX, objY, 40f);
						} else {
							selection = locY;
							gameMan.WipeSelectionColors("GameCard");
							objectHit.transform.parent.position = new Vector3(objX, objY, 38f);
							objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.cyan300;
							objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.cyan300;
						} // if already selected

						if (selection == -1) {
							gameMan.WipeSelectionColors("GameCard");
						}
					
						gameMan.UpdateUI();

					} // if Left Click


					// Right Click ########################
					// if (Input.GetMouseButtonDown(1)) {

					// 	objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.cardTintLight;
					// 	objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.cardTintLight;

					// 	gameMan.UpdateUI();

					// } // if Right Click

				} // Phase 1

			} // if GameCard

		} // if object hit

		//Debug.Log("World Point: " + worldPoint);
		
	} // Update()

} // MouseManager class

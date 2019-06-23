// A class that manages mouse hit detection

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour {

	public GameManager gameMan;
	
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

			// LAND TILES -----------------------------------------------------
			if (objectHit.transform.parent.name.Contains("LandTile")) {

				// Grab the grid coordinates stored in the object name
				byte locX = byte.Parse(objectHit.transform.parent.name.Substring(10, 1));
				byte locY = byte.Parse(objectHit.transform.parent.name.Substring(13, 1));

				// PHASE 1 ------------------------------------------
				if (GameManager.phase == 1) {

						// NOTE: GetNeighbors is currently doing more tasks than it should;
						//	i.e. things like changing material colors. This is only for testing.
						// gameMan.GetNeighbors(locX, locY);
						// gameMan.HighlightNeighbors(GameManager.turn);
						// gameMan.HighlightNeighbors(GameManager.turn);

					

					// Left Click ---------------------------------
					if (Input.GetMouseButtonDown(0)) {

						gameMan.VerifyHighlight();
						gameMan.HighlightNeighbors();
						gameMan.UpdateUI();
						

						// If the tile can be bought
						if (gameMan.BuyTile(locX, locY)) {

							// Debug output
							// Debug.Log("<b>[MouseManager]</b> " +
							// "Card Bought by Player " + GameManager.turn + ": " +
							// GameManager.grid[locX, locY].landType +
							// " " + 
							// GameManager.grid[locX, locY].value + 
							// " " +
							// GameManager.grid[locX, locY].resource);

							objX = objectHit.transform.parent.position.x;
							objY = objectHit.transform.parent.position.y;
							objZ = objectHit.transform.parent.position.z;

							objRotX = objectHit.transform.parent.rotation.x;
							objRotY = objectHit.transform.parent.rotation.y;
							objRotZ = objectHit.transform.parent.rotation.z;
							
							// Changes the material of the card depending on the player who clicked on it.
							// TODO: Might want to find a way to potentially improve performance here
							// TODO: Nice card flip animation
							switch (GameManager.turn) {
								case 1: 
									objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.inkCyan90p;
									objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.inkCyan90p;
									objectHit.transform.parent.rotation = new Quaternion(objRotX, 1-objRotY, objRotZ, 0);
									gameMan.AdvanceTurn();
									// GameManager.turnNumberText.color = ColorPalette.inkRed;	// One ahead
									break;
								case 2: 
									objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.inkRed90p;
									objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.inkRed90p;
									objectHit.transform.parent.rotation = new Quaternion(objRotX, 1-objRotY, objRotZ, 0);
									gameMan.AdvanceTurn();
									// GameManager.turnNumberText.color = ColorPalette.purple500;	// One ahead
									break;
								case 3: 
									objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.purple300;
									objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.purple300;
									objectHit.transform.parent.rotation = new Quaternion(objRotX, 1-objRotY, objRotZ, 0);
									gameMan.AdvanceTurn();
									// GameManager.turnNumberText.color = ColorPalette.amber500;	// One ahead
									break;
								case 4: 
									objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.orange300;
									objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.orange300;
									objectHit.transform.parent.rotation = new Quaternion(objRotX, 1-objRotY, objRotZ, 0);
									gameMan.AdvanceTurn();
									// GameManager.turnNumberText.color = ColorPalette.inkCyan;	// One ahead
									break;
								default: 
									break;

							} // switch

							// Debug output
							// Debug.Log("<b>[MouseManager]</b> " +
							// "Card Clicked: " +
							// GameManager.grid[locX, locY].landType +
							// " " + 
							// GameManager.grid[locX, locY].value + 
							// " " +
							// GameManager.grid[locX, locY].resource);

							// Update the round/turn display text
							gameMan.UpdateUI();

						} // if the tile could be bought

						gameMan.VerifyHighlight();
						gameMan.HighlightNeighbors();
						gameMan.UpdateUI();

					} // if Left Click


					// Right Click --------------------------------
					if (Input.GetMouseButtonDown(1)) {
						objRotX = objectHit.transform.parent.rotation.x;
						objRotY = objectHit.transform.parent.rotation.y;
						objRotZ = objectHit.transform.parent.rotation.z;

						objectHit.transform.parent.rotation = new Quaternion(objRotX, 1+objRotY, objRotZ, 0);

						objectHit.GetComponentsInChildren<Renderer>()[0].material.color = Color.white;
						objectHit.GetComponentsInChildren<Renderer>()[1].material.color = Color.white;

						// "Sells" the tile - NOTE: this is for debugging ONLY
						GameManager.grid[locX, locY].ownerId = 0;

						gameMan.RollbackTurn();
						gameMan.UpdateUI();

					} // if Right Click

				} // if LandTile

			} // Phase 1
			
			

		} // if object hit

		//Debug.Log("World Point: " + worldPoint);
		
	} // Update()

} // MouseManager class

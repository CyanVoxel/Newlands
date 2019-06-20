﻿// A class that manages mouse hit detection

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour {
	
	// Update is called once per frame
	void Update() {

		// If the cursor is over a UI element, return from Update()
		// NOTE: In order for Canvases on objects such as Cards to be ignored,
		// they must contain a "Canvas Group" and have "Block Raycasts" turned off
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

				// Left Click ---------------------------------
				if (Input.GetMouseButtonDown(0)) {
					objX = objectHit.transform.parent.position.x;
					objY = objectHit.transform.parent.position.y;
					objZ = objectHit.transform.parent.position.z;

					objRotX = objectHit.transform.parent.rotation.x;
					objRotY = objectHit.transform.parent.rotation.y;
					objRotZ = objectHit.transform.parent.rotation.z;

					//Debug.Log("Y: " + objRotY);

					objectHit.transform.parent.rotation = new Quaternion(objRotX, 1-objRotY, objRotZ, 0);


					// Changes the material of the card depending on the player who clicked on it.
					// TODO: Might want to find a way to potentially improve performance here
					switch (GameManager.turn) {
						case 1: 
							objectHit.GetComponentsInChildren<Renderer>()[0].material = Resources.Load("Materials/Cardstock_Cyan", typeof(Material)) as Material;
							objectHit.GetComponentsInChildren<Renderer>()[1].material = Resources.Load("Materials/Cardstock_Cyan", typeof(Material)) as Material;
							GameManager.turn++;
							GameManager.turnNumberText.color = ColorPalette.inkRed;	// One ahead
							break;
						case 2: 
							objectHit.GetComponentsInChildren<Renderer>()[0].material = Resources.Load("Materials/Cardstock_Red", typeof(Material)) as Material;
							objectHit.GetComponentsInChildren<Renderer>()[1].material = Resources.Load("Materials/Cardstock_Red", typeof(Material)) as Material;
							GameManager.turn++;
							GameManager.turnNumberText.color = ColorPalette.purple500;	// One ahead
							break;
						case 3: 
							objectHit.GetComponentsInChildren<Renderer>()[0].material = Resources.Load("Materials/Cardstock_Purple", typeof(Material)) as Material;
							objectHit.GetComponentsInChildren<Renderer>()[1].material = Resources.Load("Materials/Cardstock_Purple", typeof(Material)) as Material;
							GameManager.turn++;
							GameManager.turnNumberText.color = ColorPalette.amber500;	// One ahead
							break;
						case 4: 
							objectHit.GetComponentsInChildren<Renderer>()[0].material = Resources.Load("Materials/Cardstock_Amber", typeof(Material)) as Material;
							objectHit.GetComponentsInChildren<Renderer>()[1].material = Resources.Load("Materials/Cardstock_Amber", typeof(Material)) as Material;
							GameManager.turn++;
							GameManager.turnNumberText.color = ColorPalette.inkCyan;	// One ahead
							break;
						default: 
							if (GameManager.turn > GameManager.players) {
								GameManager.turn = 1;
								GameManager.round++;
							}
							break;

					} // switch

					// If the turn is greater than the number of players, 
					//	set to 0 and increment round.
					// TODO: Make this and related blocks into standalone methods
					if (GameManager.turn > GameManager.players) {
						GameManager.turn = 1;
						GameManager.round++;
					}

					// Grab the grid coordinates stored in the object name
					int locX = int.Parse(objectHit.transform.parent.name.Substring(10, 1));
					int locY = int.Parse(objectHit.transform.parent.name.Substring(13, 1));

					// Debug output
					Debug.Log("<b>[GameManager]</b> " +
					"Card Clicked: " +
					GameManager.grid[locX, locY].landType +
					" " + 
					GameManager.grid[locX, locY].value + 
					" " +
					GameManager.grid[locX, locY].resource);

					// Update the round/turn display text
					GameManager.roundNumberText.text = ("Round " + GameManager.round);
					GameManager.turnNumberText.text = ("Player " + GameManager.turn + "'s Turn");

				} // if Left Click

				// Right Click --------------------------------
				if (Input.GetMouseButtonDown(1)) {
					objRotX = objectHit.transform.parent.rotation.x;
					objRotY = objectHit.transform.parent.rotation.y;
					objRotZ = objectHit.transform.parent.rotation.z;

					objectHit.transform.parent.rotation = new Quaternion(objRotX, 1+objRotY, objRotZ, 0);

					// Undoes the turn
					if (GameManager.turn > 1) {
						GameManager.turn--;
					} else {
						GameManager.turn = GameManager.players;
						GameManager.turn--;
						GameManager.round--;
					}

					objectHit.GetComponentsInChildren<Renderer>()[0].material = Resources.Load("Materials/Cardstock", typeof(Material)) as Material;
					objectHit.GetComponentsInChildren<Renderer>()[1].material = Resources.Load("Materials/Cardstock", typeof(Material)) as Material;

				} // if Right Click

			} // if LandTile

			
			
			

		} // if object hit

		//Debug.Log("World Point: " + worldPoint);
		
	} // Update()
}

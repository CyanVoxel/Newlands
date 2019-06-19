// A class that manages the mouse cursor's screen/world position

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour {

	// Use this for initialization
	void Start() {
		
	}
	
	// Update is called once per frame
	void Update() {

		// If the cursor is over a UI element, return from Update()
		// NOTE: In order for Canvases on objects such as Cards to be ignored,
		// they must contain a "Canvas Group" and have "Block Raycasts" turned off
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}

		//Debug.Log("Mouse Position: " + Input.mousePosition);

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hitInfo;

		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.collider.transform.gameObject;

			//Debug.Log("Raycast Hit: " + hitInfo.collider.transform.parent.name);

			float objX = objectHit.transform.parent.position.x;
			float objY = objectHit.transform.parent.position.y;
			float objZ = objectHit.transform.parent.position.z;

			float objRotX = objectHit.transform.parent.rotation.x;
			float objRotY = objectHit.transform.parent.rotation.y;
			float objRotZ = objectHit.transform.parent.rotation.z;
			

			if (Input.GetMouseButtonDown(0)) {
				objX = objectHit.transform.parent.position.x;
				objY = objectHit.transform.parent.position.y;
				objZ = objectHit.transform.parent.position.z;

				objRotX = objectHit.transform.parent.rotation.x;
				objRotY = objectHit.transform.parent.rotation.y;
				objRotZ = objectHit.transform.parent.rotation.z;

				Debug.Log("Y: " + objRotY);

				objectHit.transform.parent.rotation = new Quaternion(objRotX, 1-objRotY, objRotZ, 0);

				switch (GameManager.turn) {
					case 0: objectHit.GetComponentInChildren<Renderer>().material = Resources.Load("Materials/Cardstock_Cyan", typeof(Material)) as Material;
						GameManager.turn++;
						break;
					case 1: objectHit.GetComponentInChildren<Renderer>().material = Resources.Load("Materials/Cardstock_Red", typeof(Material)) as Material;
						GameManager.turn++;
						break;
					case 2: objectHit.GetComponentInChildren<Renderer>().material = Resources.Load("Materials/Cardstock_Purple", typeof(Material)) as Material;
						GameManager.turn++;
						break;
					case 3: objectHit.GetComponentInChildren<Renderer>().material = Resources.Load("Materials/Cardstock_Amber", typeof(Material)) as Material;
						GameManager.turn++;
						break;
					default: 
						if (GameManager.turn >= GameManager.players) {
							GameManager.turn = 0;
							GameManager.round++;
						}
						break;
				}

				Debug.Log("Turn " + GameManager.turn + " of Round " + GameManager.round);

				if (GameManager.turn >= GameManager.players) {
					GameManager.turn = 0;
					GameManager.round++;
				}
					



				//objectHit.transform.parent.position = new Vector3(objX, objY, 49f);
				//objectHit.transform.parent.localScale += new Vector3(1f, 1f, 0f);
			}

			// if (Input.GetMouseButtonUp(0)) {
			// 	objectHit.transform.parent.position = new Vector3(objX, objY, 50f);
			// }

			if (Input.GetMouseButtonDown(1)) {
				objRotX = objectHit.transform.parent.rotation.x;
				objRotY = objectHit.transform.parent.rotation.y;
				objRotZ = objectHit.transform.parent.rotation.z;

				objectHit.transform.parent.rotation = new Quaternion(objRotX, 1+objRotY, objRotZ, 0);

				objectHit.GetComponentInChildren<Renderer>().material = Resources.Load("Materials/Cardstock", typeof(Material)) as Material;
			}

		}

		//Debug.Log("World Point: " + worldPoint);
		
	} // Update()
}

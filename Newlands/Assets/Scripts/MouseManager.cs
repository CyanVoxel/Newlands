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
			

			if (Input.GetMouseButton(0)) {
				objX = objectHit.transform.parent.position.x;
				objY = objectHit.transform.parent.position.y;
				objZ = objectHit.transform.parent.position.z;

				objectHit.transform.parent.position = new Vector3(objX, objY, 48f);
				//objectHit.transform.parent.localScale += new Vector3(1f, 1f, 0f);
			}

			if (Input.GetMouseButtonUp(0)) {
				objectHit.transform.parent.position = new Vector3(objX, objY, 50f);
			}

			if (Input.GetMouseButton(1)) {
				objectHit.transform.parent.localScale += new Vector3(1f, 1f, 0f);
			}

		}

		//Debug.Log("World Point: " + worldPoint);
		
	} // Update()
}

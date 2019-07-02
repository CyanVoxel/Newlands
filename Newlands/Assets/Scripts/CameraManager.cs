using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public static Camera mainCam;

    void Awake() {
		mainCam = Camera.main;
	}

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        // Scroll in and out
		Vector3 cameraPos = mainCam.transform.position;
		cameraPos.z += (Input.mouseScrollDelta.y * 3f);
		mainCam.transform.position = cameraPos;

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
			cameraPos.y += .75f;
			mainCam.transform.position = cameraPos;
		}

		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
			cameraPos.x -= .75f;
			mainCam.transform.position = cameraPos;
		}

		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
			cameraPos.y -= .75f;
			mainCam.transform.position = cameraPos;
		}

		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
			cameraPos.x += .75f;
			mainCam.transform.position = cameraPos;
		}

    }
}

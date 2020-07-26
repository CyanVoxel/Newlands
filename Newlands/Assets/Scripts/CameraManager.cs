using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public static Camera mainCam;

	private bool dragging = false;
	private float dragSpeed = 0f;
	private Vector3 mouseOrigin = new Vector3();
	private Vector3 oldCameraPos = new Vector3();

	private Vector3 uncheckedNewCamPos = new Vector3();
	private Vector3 checkedNewCamPos = new Vector3();

	public float baseDragSpeed = 60f;
	public float dragSpeedModifierClamp = 1.5f;

	void Awake()
	{
		mainCam = Camera.main;
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		// Scroll Camera Movement --------------------------------------------------------------------------------------
		uncheckedNewCamPos = mainCam.transform.position;
		uncheckedNewCamPos.z += (Input.mouseScrollDelta.y * 4f);
		// mainCam.transform.position = cameraPos;

		// WASD Camera Movement ----------------------------------------------------------------------------------------
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			uncheckedNewCamPos.y += .75f;
		}

		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			uncheckedNewCamPos.x -= .75f;
		}

		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			uncheckedNewCamPos.y -= .75f;
		}

		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			uncheckedNewCamPos.x += .75f;
		}

		// Dualshock Controller Camera Movement ------------------------------------------------------------------------
		uncheckedNewCamPos.x += (Input.GetAxis("DualshockHorizontal") * 1.5f);
		uncheckedNewCamPos.y += (Input.GetAxis("DualshockVertical") * 1.5f);
		uncheckedNewCamPos.z += (Input.GetAxis("DualshockZoom") * 2f);

		// Debug.Log("HORIZONTAL: " + Input.GetAxis("DualshockHorizontal")
		// 	+ " --- VERTICAL: " + Input.GetAxis("DualshockVertical")
		// 	+ " --- ZOOM: " + Input.GetAxis("DualshockZoom"));

		checkedNewCamPos = uncheckedNewCamPos;

		// Click-and-Drag Camera Movement ------------------------------------------------------------------------------
		if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
		{
			dragging = true;
			oldCameraPos = uncheckedNewCamPos;
			mouseOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		}

		if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
		{
			Vector3 mouseDisplacement = Camera.main.ScreenToViewportPoint(Input.mousePosition) - mouseOrigin;

			if (mainCam.transform.position.z < 0)
			{
				dragSpeed = baseDragSpeed + (Mathf.Abs(mainCam.transform.position.z) / dragSpeedModifierClamp);
				// Debug.Log("Drag Speed: " + dragSpeed + " - Base Drag Speed: " + baseDragSpeed + " - Modifier: " + (Mathf.Abs(mainCam.transform.position.z) / dragSpeedModifierClamp));
			}
			else
			{
				dragSpeed = baseDragSpeed - (Mathf.Abs(mainCam.transform.position.z) / dragSpeedModifierClamp);
				// Debug.Log("Drag Speed: " + dragSpeed + " - Base Drag Speed: " + baseDragSpeed + " - Modifier: -" + (Mathf.Abs(mainCam.transform.position.z) / dragSpeedModifierClamp));
			}

			if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
			{
				dragging = false;
			}

			uncheckedNewCamPos = oldCameraPos + -mouseDisplacement * dragSpeed;
			checkedNewCamPos = oldCameraPos + -mouseDisplacement * dragSpeed;
		}

		// Position Verification ---------------------------------------------------------------------------------------
		// These are hard-coded boundaries for the camera.
		if (uncheckedNewCamPos.x < 0)
		{
			checkedNewCamPos.x = 0;
		}
		else if (uncheckedNewCamPos.x > 100)
		{
			checkedNewCamPos.x = 100;
		}

		if (uncheckedNewCamPos.y < -15)
		{
			checkedNewCamPos.y = -15;
		}
		else if (uncheckedNewCamPos.y > 100)
		{
			checkedNewCamPos.y = 100;
		}

		if (uncheckedNewCamPos.z < -200)
		{
			checkedNewCamPos.z = -200;
		}
		else if (uncheckedNewCamPos.z > -5)
		{
			checkedNewCamPos.z = -5;
		}

		// Debug.Log("Settting Cam Position to: " + checkedNewCamPos + " after fixing " + uncheckedNewCamPos);
		mainCam.transform.position = checkedNewCamPos;
	}
}

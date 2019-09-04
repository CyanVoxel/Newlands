// A class that manages mouse hit detection

// TODO: Move the responsibility of knowing what turn you're allowed to play on
// to the server. Otherwise a modified client could send a command on any turn.

using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : NetworkBehaviour
{
	private static DebugTag debug = new DebugTag("MouseManager", "AA00FF");

	private GameManager gameMan;
	// public GuiManager guiMan;
	private GridManager gridMan;
	public int selection = -1;
	// TODO: Create a dictionary of flags
	private int purchaseSuccessFlag = -1; // -1: Reset | 0: False | 1: True
	public int playIndex = -1;
	public int playSuccessFlag = -1; // -1: Reset | 0: False | 1: True
	private static GameObject objectHit;
	[SyncVar]
	public int ownerId = -1;

	// [SyncVar]
	public NetworkConnection myClient;
	public GameObject myPlayerObj;

	private int purchaseBufferX = -1;
	private int purchaseBufferY = -1;

	// Containers for object position and rotation info
	private float objX;
	private float objY;
	private float objZ;
	private float objRotX;
	private float objRotY;
	private float objRotZ;

	void Start()
	{
		gridMan = FindObjectOfType<GridManager>();
		gameMan = FindObjectOfType<GameManager>();
	} // Start()

	// TODO: Clean this bad boy up and break it up into smaller methods
	// Update is called once per frame
	void Update()
	{
		// Authority Check -------------------------------------------------------------------------
		if (!hasAuthority)
		{
			// Debug.LogWarning(debug.warning + "No Authority!");
			return;
		}

		// Debug.Log(debug.head + this.selection);

		// Component Grabbers if null --------------------------------------------------------------
		if (gameMan == null)
		{
			gameMan = FindObjectOfType<GameManager>();
			Debug.Log(debug.head + "GameManager was set during update");
		}

		if (gridMan == null)
		{
			gridMan = FindObjectOfType<GridManager>();
			Debug.Log(debug.head + "GridManager was set during update");
		}

		// Flag Checkers ---------------------------------------------------------------------------
		CheckPurchaseSuccess();
		CheckPlaySuccess();

		// Raycast Handler -------------------------------------------------------------------------
		// If 'P' is pressed, end the phase - Debugging only
		if (Input.GetKeyDown(KeyCode.P))
		{
			gameMan.EndPhase();
			// guiMan.CmdUpdateUI();
		}

		// If the cursor is over a UI element, return from Update()
		// NOTE: In order for Canvases on objects such as Cards to be ignored,
		//	they must contain a "Canvas Group" and have "Block Raycasts" turned off
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}

		// Initialize ray
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;

		// If an object was hit
		if (Physics.Raycast(ray, out hitInfo))
		{
			// Checks if the object hit has a parent before assinging it to a local var.
			if (hitInfo.collider.transform.parent != null)
			{
				objectHit = hitInfo.collider.transform.parent.gameObject;
			}
			else
			{
				objectHit = hitInfo.collider.transform.gameObject;
			}

			HandleObjectHit(objectHit);

		} // If an object was hit
	}

	private void CheckPurchaseSuccess()
	{
		switch (this.purchaseSuccessFlag)
		{
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
				this.purchaseSuccessFlag = -1;
				break;

			default:
				Debug.LogWarning(debug.head + "PurchaseSuccessFlag was set to " + this.purchaseSuccessFlag);
				this.purchaseSuccessFlag = -1;
				break;
		} // switch (purchaseSuccessFlag)
	} // CheckPurchaseSuccess()

	private void CheckPlaySuccess()
	{
		switch (this.playSuccessFlag)
		{
			case -1:
				break;

			case 0:
				// False
				Debug.Log(debug.head + "Play Unsuccessful!");
				this.playSuccessFlag = -1;
				break;

			case 1:
				// True
				Debug.Log(debug.head + "Play Successful!");
				// CmdFlipCard("Tile", this.purchaseBufferX, this.purchaseBufferY);
				// this.purchaseSuccessRound = gameMan.round;
				this.playSuccessFlag = -1;
				break;

			default:
				Debug.LogWarning(debug.head + "PlaySuccessFlag was set to " + this.playSuccessFlag);
				this.playSuccessFlag = -1;
				break;
		} // switch (purchaseSuccessFlag)
	} // CheckPurchaseSuccess()

	private void CallCmdBuyTile(int locX, int locY)
	{
		this.purchaseBufferX = locX;
		this.purchaseBufferY = locY;
		CmdBuyTile(locX, locY);
	} // CallCmdBuyTile()

	// Handles what to do when the pointer is over an object
	private void HandleObjectHit(GameObject objectHit)
	{
		// Grab info from object name
		string[] nameArr = objectHit.transform.parent.name.Split('_');
		string type = nameArr[2];
		int x = int.Parse(nameArr[0].Substring(1));
		int y = int.Parse(nameArr[1].Substring(1));

		// Primary Click =======================================================
		if (Input.GetMouseButtonDown(0) && nameArr != null)
		{
			Debug.Log(debug + "Clicked on " + type + ", x: " + x + ", y: " + y);
			switch (gameMan.phase)
			{
				case 1:
					BuyingPhasePrimaryClick(type, x, y);
					break;
				case 2:
					PlayingPhasePrimaryClick(type, x, y);
					break;
				default:
					break;
			}
		} // Primary Click

	} // HandleObjectHit()

	private void BuyingPhasePrimaryClick(string type, int x, int y)
	{
		this.selection = -1;
		// If the tile can be bought
		if (gameMan.turn == this.ownerId)
		{
			Debug.Log(debug.head + "Trying to buy tile!");
			CmdBuyTile(x, y);
			this.purchaseBufferX = x;
			this.purchaseBufferY = y;
		}
		else if (this.ownerId == -1)
		{
			Debug.LogWarning(debug.warning + "This MouseManager has an ownerID of -1!");
		}
		else
		{
			Debug.Log(debug.head + "Player " + this.ownerId
				+ " can't buy a tile on Player " + gameMan.turn + "'s Turn!");
		}
	} // BuyingPhasePrimaryClick()

	private void PlayingPhasePrimaryClick(string type, int x, int y)
	{
		GameObject oldSelection;
		switch (type)
		{
			case "Tile":
			case "MarketCard":
				if (selection >= 0 && gameMan.turn == this.ownerId)
				{
					Debug.Log(debug.head + "Trying to play card " + selection
						+ " on " + objectHit.transform.parent.name);
					playIndex = selection;
					CmdPlayCard(selection, objectHit.transform.parent.name);
				}
				Debug.Log(debug + "Trying to find " + GameManager.CreateCardObjectName("GameCard", this.ownerId, selection));
				oldSelection = GameObject.Find(GameManager.CreateCardObjectName("GameCard", this.ownerId, selection));
				// oldSelection.transform.parent.position = new Vector3(oldSelection.transform.parent.position.x, oldSelection.transform.parent.position.y, objectHit.transform.parent.position.z);
				oldSelection.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintCard;
				oldSelection.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintCard;
				selection = -1;
				break;
			case "GameCard":
				// If the object clicked was already selected, deselect it
				if (selection == y)
				{
					selection = -1;
					// objectHit.transform.parent.position = new Vector3(objectHit.transform.parent.position.x, objectHit.transform.parent.position.y, objectHit.transform.parent.position.z);
					objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintCard;
					objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintCard;
				}
				else if (selection >= 0)
				{
					Debug.Log(debug + "Trying to find " + GameManager.CreateCardObjectName("GameCard", this.ownerId, selection));
					oldSelection = GameObject.Find(GameManager.CreateCardObjectName("GameCard", this.ownerId, selection));
					// oldSelection.transform.parent.position = new Vector3(oldSelection.transform.parent.position.x, oldSelection.transform.parent.position.y, objectHit.transform.parent.position.z);
					oldSelection.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintCard;
					oldSelection.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintCard;

					selection = y;
					// objectHit.transform.parent.position = new Vector3(objectHit.transform.parent.position.x, objectHit.transform.parent.position.y, (objectHit.transform.parent.position.z -2f));
					objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintCyan500;
					objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintCyan500;
				}
				else
				{
					selection = y;
					// objectHit.transform.parent.position = new Vector3(objectHit.transform.parent.position.x, objectHit.transform.parent.position.y, (objectHit.transform.parent.position.z -2f));
					objectHit.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintCyan500;
					objectHit.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintCyan500;
				}
				break;
			default:
				break;
		}

	}

	// COMMANDS ####################################################################################

	[Command]
	private void CmdFlipCard(string cardType, int locX, int locY)
	{
		CardAnimations.FlipCard(cardType, locX, locY);
	} // CmdFlipCard()

	[Command]
	private void CmdBuyTile(int locX, int locY)
	{
		// Debug.Log(debug.head + "Is Server? " + isServer);

		bool success = false;
		if (gameMan.BuyTile(locX, locY))
		{
			success = true;
		}
		Debug.Log(debug.head + "About to call TargetBuyTile with connection: " + myClient);
		TargetBuyTile(myClient, success);
	} // CmdBuyTile()

	[Command]
	private void CmdPlayCard(int selection, string targetTile)
	{
		bool success = false;
		if (gameMan.PlayCard(selection, targetTile))
		{
			success = true;
		}
		Debug.Log(debug.head + "About to call TargetPlay with connection: " + myClient);
		TargetPlayCard(myClient, success);
	} // CmdPlayCard()

	[TargetRpc]
	private void TargetBuyTile(NetworkConnection target, bool success)
	{
		Debug.Log(debug.head + "Called TargetBuyTile!");
		if (success)
		{
			purchaseSuccessFlag = 1;
		}
		else
		{
			purchaseSuccessFlag = 0;
		}
	} // TargetBuyTile()

	[TargetRpc]
	private void TargetPlayCard(NetworkConnection target, bool success)
	{
		Debug.Log(debug.head + "Called TargetPlay!");
		if (success)
		{
			playSuccessFlag = 1;
			selection = -1;
		}
		else
		{
			playSuccessFlag = 0;
		}
	} // TargetPlayCard()
} // class MouseManager

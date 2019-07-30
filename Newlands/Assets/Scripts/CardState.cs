// A test class used to test the functionality of scripts across the network

using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class CardState : NetworkBehaviour {

	// DATA FIELDS #################################################################################

	// [SerializeField]
	// public string titleStr = "test";
	private CardDisplay cardDis;
	[SerializeField]
	private GridManager gridMan;

	// USED FOR VISUALS ONLY -----------------------------------------------------------------------
	[SyncVar(hook = "OnObjectNameChange")]
	public string objectName; // The Card Object's Name
	[SyncVar(hook = "OnTitleChange")]
	public string title; // The Card's Title
	[SyncVar(hook = "OnSubtitleChange")]
	public string subtitle; // The Card's Subtitle
	[SyncVar(hook = "OnBodyChange")]
	public string body; // The Card's Body Text

	[SyncVar(hook = "OnFooterChange")]
	public string footer; // The Card's Footer Text
	[SyncVar]
	public int footerValue;
	[SyncVar] // No hook on values which won't change under current game rules
	public bool percFlag;
	[SyncVar]
	public bool moneyFlag;
	[SyncVar]
	public char footerOpr;

	[SyncVar(hook = "OnCategoryChange")]
	public string category = "Tile"; // The Card's Category (Used to determine misc visuals)
	[SyncVar]
	public string resource;
	[SyncVar]
	public string target;

	// METHODS #################################################################################

	// Start is called before the first frame update
	void Start() {

		TryToGrabComponents();

		OnObjectNameChange("Jerry");
		OnTitleChange("Jerry");
		OnSubtitleChange("Jerry");
		OnBodyChange("Jerry");
		OnFooterChange("Jerry");
		OnCategoryChange("Jerry");

		// GameObject titleObj = this.transform.Find("Front Canvas/Title").gameObject;
		// TMP_Text title = titleObj.GetComponent<TMP_Text>();
		// title.text = titleStr;

	} // Start()

	// // Update is called once per frame
	// void Update() {

	// 	// this.titleStr = gridMan.tempTitle;
	// 	// Debug.Log("Title:" + this.title);

	// 	// if (gridMan == null) {
	// 	// 	gridMan = FindObjectOfType<GridManager>();
	// 	// }

	// 	// GameObject titleObj = this.transform.Find("Front Canvas/Title").gameObject;
	// 	// TMP_Text title = titleObj.GetComponent<TMP_Text>();
	// 	// title.text = titleStr;

	// }

	// NOTE: The paramaters in the hook methods are not used, however they are required by Mirror.
	// The if-statement is there so that the display functions do not get run with empty internal
	// values on initialization. Checking for the passed value instead of the current internal state
	// would lead to unnecessary code being run as well as potential errors.
	// By extention, any calls of these methods can use any throwaway string.

	// Fires when the name destined for this object changes (Should only happen once!)
	private void OnObjectNameChange(string newName) {
		this.transform.name = this.objectName;
		if (FindObjectOfType<GridManager>() != null) {
			this.transform.SetParent(FindObjectOfType<GridManager>().transform);
		}
	} // OnObjectNameChange()

	private void OnFooterChange(string newFooterText) {

		TryToGrabComponents();

		if (this.footer != "") {
			cardDis.DisplayFooter(this.transform.gameObject);
		}

	} // OnFooterChange()

	private void OnCategoryChange(string newCategory) { }

	private void OnTitleChange(string newTitle) {

		TryToGrabComponents();

		if (this.title != "") {
			cardDis.DisplayTitle(this.transform.gameObject);
		}

	} // OnTitleChange()

	private void OnSubtitleChange(string newSubtitle) {

		TryToGrabComponents();

		if (this.subtitle != "") {
			cardDis.DisplaySubtitle(this.transform.gameObject);
		}

	} // OnSubtitleChange()

	private void OnBodyChange(string newBody) {

		TryToGrabComponents();

		if (this.body != "") {
			cardDis.DisplayBody(this.transform.gameObject);
		}

	} // OnBodyChange()

	// Tries to grab necessary components if they haven't been already.
	private void TryToGrabComponents() {
		if (cardDis == null) {
			cardDis = this.GetComponent<CardDisplay>();
		}

		if (gridMan == null) {
			gridMan = FindObjectOfType<GridManager>();
		}
	} // GrabComponents()

} // CardState class

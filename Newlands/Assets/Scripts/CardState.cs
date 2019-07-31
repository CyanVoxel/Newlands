// A class used to store relevent card data for visuals and synchronize it between the server and
// clients. Could be used with GridUnit in the future?

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CardState : NetworkBehaviour {

	// DATA FIELDS #################################################################################

	[SerializeField]
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
	public string bodyText; // The Card's Body Text

	[SyncVar(hook = "OnFooterChange")]
	public string footerText; // The Card's Footer Text
	[SyncVar]
	public int footerValue;
	[SyncVar] // No hook on values which won't change under current game rules
	public bool percFlag;
	[SyncVar]
	public bool moneyFlag;
	[SyncVar]
	public char footerOpr;
	[SyncVar]
	public bool onlyColorCorners;

	[SyncVar(hook = "OnCategoryChange")]
	public string category; // The Card's Category (Used to determine misc visuals)
	[SyncVar]
	public string resource;
	[SyncVar]
	public string target;
	[SyncVar]
	public string footerColor;

	// METHODS #################################################################################

	// Start is called before the first frame update
	void Start() {

		TryToGrabComponents();

		OnObjectNameChange("Jerry");
		OnTitleChange("George");
		OnSubtitleChange("Kramer");
		OnBodyChange("Elaine");
		OnFooterChange("Newman");
		OnCategoryChange("Leo");

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

		if (this.footerText != "") {
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

		if (this.bodyText != "") {
			cardDis.DisplayBody(this.transform.gameObject);
		}

	} // OnBodyChange()

	// Tries to grab necessary components if they haven't been already.
	private bool TryToGrabComponents() {

		if (this.cardDis == null) {
			this.cardDis = this.GetComponent<CardDisplay>();
		}

		if (this.gridMan == null) {
			this.gridMan = FindObjectOfType<GridManager>();
		}

		if (this.cardDis == null) {
			return false;
		} else if (this.gridMan == null) {
			return false;
		} else {
			return true;
		}

	} // TryToGrabComponents()

} // CardState class

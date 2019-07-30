// A test class used to test the functionality of scripts across the network

using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class CardState : NetworkBehaviour {

	// [SerializeField]
	// public string titleStr = "test";
	private CardDisplay cardDis;
	[SerializeField]
	private GridManager gridMan;

	// DATA FIELDS - USED FOR VISUALS ONLY ---------------------------------------------------------
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
	[SyncVar(hook = "OnCategoryChange")]
	public string category = "Tile"; // The Card's Category (Used to determine misc visuals)

	// Start is called before the first frame update
	void Start() {

		TryToGrabComponents();

		// OnObjectNameChange(this.objectName);
		OnTitleChange(this.title);
		OnSubtitleChange(this.subtitle);
		OnBodyChange(this.body);
		OnFooterChange(this.footer);
		OnCategoryChange(this.category);

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

	// Fires when the name destined for this object changes (Should only happen once!)
	private void OnObjectNameChange(string newName) {
		this.transform.name = newName;
		if (FindObjectOfType<GridManager>() != null) {
			this.transform.SetParent(FindObjectOfType<GridManager>().transform);
		}
	} // OnObjectNameChange()

	private void OnSubtitleChange(string newSubtitle) { }

	private void OnBodyChange(string newBody) { }

	private void OnFooterChange(string newFooter) { }

	private void OnCategoryChange(string newCategory) { }

	private void OnTitleChange(string newTitle) {

		TryToGrabComponents();

		if (this.title != "") {
			cardDis.DisplayTitle(this.transform.gameObject);
		}

	}

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

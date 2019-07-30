// A test class used to test the functionality of scripts across the network

using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class CardState : NetworkBehaviour {

	// [SerializeField]
	public string titleStr = "test";
	[SyncVar(hook = "OnObjectNameChange")]
	public string objectName;
	private CardDisplay cardDis;
	[SerializeField]
	private GridManager gridMan;

	// Start is called before the first frame update
	void Start() {

		cardDis = this.GetComponent<CardDisplay>();
		gridMan = FindObjectOfType<GridManager>();

		if (gridMan == null) {
			gridMan = FindObjectOfType<GridManager>();
		}

		GameObject titleObj = this.transform.Find("Front Canvas/Title").gameObject;
		TMP_Text title = titleObj.GetComponent<TMP_Text>();
		title.text = titleStr;

	} // Start()

	// Update is called once per frame
	void Update() {

		this.titleStr = gridMan.tempTitle;

		if (gridMan == null) {
			gridMan = FindObjectOfType<GridManager>();
		}

		GameObject titleObj = this.transform.Find("Front Canvas/Title").gameObject;
		TMP_Text title = titleObj.GetComponent<TMP_Text>();
		title.text = titleStr;

	}

	// Fires when the name destined for this object changes (Should only happen once!)
	private void OnObjectNameChange(string newName) {
		this.transform.name = newName;
		if (FindObjectOfType<GridManager>() != null) {
			this.transform.SetParent(FindObjectOfType<GridManager>().transform);
		}
	} // OnObjectNameChange()
}

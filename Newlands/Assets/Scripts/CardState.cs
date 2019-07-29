// A test class used to test the functionality of scripts across the network

using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class CardState : NetworkBehaviour {

	// [SerializeField]
	public string titleStr = "test";
	private CardDisplay cardDis;
	[SerializeField]
	private GridManager gridMan;

	// Start is called before the first frame update
	void Start() {

		// this.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);

		// if (!isLocalPlayer) {
		//     Debug.Log("This is <b>NOT</b> the local player!");
		//     return;
		// } else {
		//     Debug.Log("This <b>IS</b> the local player!");
		// }

		cardDis = this.GetComponent<CardDisplay>();
		gridMan = FindObjectOfType<GridManager>();

		// titleStr = "test";
		// if (GridManager.grid != null) {
		//     titleStr = GridManager.grid[2, 2].subScope;
		// } else {
		//     Debug.Log("Grid manager is null!");
		// }

		// RpcSetTitle();

		// titleStr = gridMan.CmdGetTitle(2, 2);

		if (gridMan == null) {
			gridMan = FindObjectOfType<GridManager>();
		}

		GameObject titleObj = this.transform.Find("Front Canvas/Title").gameObject;
		TMP_Text title = titleObj.GetComponent<TMP_Text>();
		title.text = titleStr;

	} // Start()

	// Update is called once per frame
	void Update() {

		// if (gridMan == null) {
		// 	gridMan = FindObjectOfType<GridManager>();

		// 	GameObject titleObj = this.transform.Find("Front Canvas/Title").gameObject;
		// 	TMP_Text title = titleObj.GetComponent<TMP_Text>();
		// 	title.text = titleStr;
		// 	// CmdRequestValues();
		// 	// gridMan.CmdFillOutCard(this.gameObject, 2, 2);
		// }

		this.titleStr = gridMan.tempTitle;

		if (gridMan == null) {
			gridMan = FindObjectOfType<GridManager>();
		}

		GameObject titleObj = this.transform.Find("Front Canvas/Title").gameObject;
		TMP_Text title = titleObj.GetComponent<TMP_Text>();
		title.text = titleStr;

	}

	[Command]
	private void CmdRequestValues() {
		RpcSetValues(GridManager.grid[2, 2].subScope);
		gridMan.CmdFillOutCard(this.gameObject, 2, 2);
	}

	[ClientRpc]
	private void RpcSetValues(string titleVal) {
		this.titleStr = titleVal;
	}

	[ClientRpc]
	private void RpcSetTitle() {
		this.titleStr = GridManager.grid[2, 2].subScope;
	}
}

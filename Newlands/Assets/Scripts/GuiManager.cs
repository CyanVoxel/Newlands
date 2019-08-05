// using System.Collections;
// using System.Collections.Generic;
// using Mirror;
using TMPro;
using UnityEngine;

public class GuiManager : MonoBehaviour {

	// // GUI ELEMENTS ###############################################################################

	// public GameManager gameMan;

	// private TMP_Text phaseNumberText;
	// private TMP_Text roundNumberText;
	// private TMP_Text turnNumberText;

	// // [SyncVar(hook = "OnPhaseNumChange")]
	// private string phaseNumStr;
	// // [SyncVar(hook = "OnRoundNumChange")]
	// private string roundNumStr;
	// // [SyncVar(hook = "OnTurnNumChange")]
	// private string turnNumStr;

	// // Start is called before the first frame update
	// void Start() {

	// } // Start()

	// // Update is called once per frame
	// void Update() {

	// } // Update()

	// // METHODS ####################################################################################

	// public void InitGuiManager() {

	// 	InitPlayerText();

	// 	// Grabbing Money Objects/Text
	// 	for (int i = 0; i < GameManager.playerCount; i++) {
	// 		// playerMoneyObj.Add(new GameObject());
	// 		// playerMoneyText.Add(new TMP_Text());
	// 		// Debug.Log("UI/Money/Player (" + (i + 1) + ")");
	// 		GameManager.players[i].moneyObj = transform.Find("Gui/Money/Player (" + (i + 1) + ")").gameObject;
	// 		GameManager.players[i].moneyText = GameManager.players[i].moneyObj.GetComponent<TMP_Text>();
	// 		// GameManager.players[i].moneyText.color = GetPlayerColor((i + 1), 500);
	// 	} // for playerCount

	// } // InitGuiManager()

	// void InitPlayerText() {

	// 	if (transform.Find("Gui/PhaseNumber") != null
	// 		&& (transform.Find("Gui/PhaseNumber").GetComponent<TMP_Text>() != null)) {
	// 		phaseNumberText = transform.Find("Gui/PhaseNumber").gameObject.GetComponent<TMP_Text>();
	// 	}

	// 	if (transform.Find("Gui/RoundNumber") != null
	// 		&& (transform.Find("Gui/RoundNumber").GetComponent<TMP_Text>() != null)) {
	// 		roundNumberText = transform.Find("Gui/RoundNumber").gameObject.GetComponent<TMP_Text>();
	// 	}

	// 	if (transform.Find("Gui/TurnNumber") != null
	// 		&& (transform.Find("Gui/TurnNumber").GetComponent<TMP_Text>() != null)) {
	// 		turnNumberText = transform.Find("Gui/TurnNumber").gameObject.GetComponent<TMP_Text>();
	// 	}

	// } //InitPlayerText()

	// private void OnPhaseNumChange(string stringToDisplay) {

	// 	if (phaseNumberText == null) {
	// 		return;
	// 	}
	// 	// phaseNumberText.text = stringToDisplay;
	// 	this.UpdateUI();

	// } // OnPhaseNumChange()

	// private void OnRoundNumChange(string stringToDisplay) {

	// 	if (roundNumberText == null) {
	// 		return;
	// 	}
	// 	// roundNumberText.text = stringToDisplay;
	// 	this.UpdateUI();

	// } // OnRoundNumChange()

	// private void OnTurnNumChange(string stringToDisplay) {

	// 	if (turnNumberText == null) {
	// 		return;
	// 	}
	// 	// turnNumberText.text = stringToDisplay;
	// 	this.UpdateUI();

	// } // OnRoundNumChange()

	// // COMMANDS ###################################################################################

	// // Updates the basic UI elements
	// // [Command]
	// public void UpdateUI() {

	// 	// if (!isLocalPlayer) {
	// 	// 	// Debug.Log("<b>[GuiManager]</b> "
	// 	// 	// + "Player does not have authority to update GUI!");
	// 	// 	return;
	// 	// }

	// 	phaseNumStr = ("Phase " + gameMan.phase);
	// 	this.phaseNumberText.text = phaseNumStr;

	// 	roundNumStr = ("Round " + gameMan.round);
	// 	this.roundNumberText.text = roundNumStr;

	// 	turnNumStr = ("Player " + gameMan.turn + "'s Turn");
	// 	this.turnNumberText.text = turnNumStr;

	// 	// Tacks on "Grace Period" text if the round is a grace round
	// 	if (gameMan.phase == 1 && gameMan.round <= GameManager.graceRounds) {
	// 		this.roundNumberText.text += (" (Grace Period)");
	// 	} // if

	// 	switch (gameMan.turn) {
	// 		case 1:
	// 			this.turnNumberText.color = ColorPalette.lightBlue500;
	// 			break;
	// 		case 2:
	// 			this.turnNumberText.color = ColorPalette.red500;
	// 			break;
	// 		case 3:
	// 			this.turnNumberText.color = ColorPalette.purple500;
	// 			break;
	// 		case 4:
	// 			this.turnNumberText.color = ColorPalette.amber500;
	// 			break;
	// 		default:
	// 			break;
	// 	} // switch

	// 	// Things that need to be updated for all players go here
	// 	for (int i = 0; i < GameManager.players.Count; i++) {
	// 		GameManager.players[i].moneyText.text = "Player " + (i + 1) + ": "
	// 			+ GameManager.players[i].totalMoney.ToString("C");
	// 	} // for array length

	// } // UpdateUI()

} // GuiManager class

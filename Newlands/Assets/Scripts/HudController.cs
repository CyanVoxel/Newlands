// Controls and updates the HUD based on changing values in GameManager.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudController : MonoBehaviour
{
	// public GameManager gameMan;
	// private MatchController matchController;
	private MatchDataBroadcaster matchDataBroadcaster;

	private TMP_Text phaseNumberText;
	private TMP_Text roundNumberText;
	private TMP_Text turnNumberText;

	private MatchConfigData config;

	private MatchData matchData;
	private string matchDataStr = "";
	private string lastKnownMatchDataStr = "";

	private int lastKnownTurn = -1;
	private int lastKnownRound = -1;
	private int lastKnownPhase = -1;
	private string lastKnownPlayerMoneyStr = "";
	private List<TMP_Text> playerMoneyText = new List<TMP_Text>();
	private List<int> playerMoneyAmounts = new List<int>();

	private static DebugTag debug = new DebugTag("HudController", "4CAF50");

	// Start is called before the first frame update
	void Start()
	{
		InitPlayerText();
		InitMoneyText();

		if (this.matchDataBroadcaster == null)
			this.matchDataBroadcaster = FindObjectOfType<MatchDataBroadcaster>();

		lastKnownMatchDataStr = matchDataBroadcaster.MatchDataStr;
		this.matchData = JsonUtility.FromJson<MatchData>(lastKnownMatchDataStr);
		this.lastKnownPlayerMoneyStr = matchDataBroadcaster.PlayerMoneyStr;

		// if (this.matchController == null)
		// 	this.matchController = FindObjectOfType<MatchController>();

		this.config = JsonUtility.FromJson<MatchConfigData>(matchDataBroadcaster.MatchConfigStr);

		// this.lastKnownTurn = gameMan.turn;
		// this.lastKnownRound = gameMan.round;
		// this.lastKnownPhase = gameMan.phase;
		// this.lastKnownPlayerMoneyStr = gameMan.playersMoneyStr;

		UpdateUI();
	} // Start()

	// Update is called once per frame
	void Update()
	{
		MatchData newMatchData = this.matchData;

		if (lastKnownMatchDataStr != matchDataBroadcaster.MatchDataStr)
		{
			newMatchData = JsonUtility.FromJson<MatchData>(matchDataBroadcaster.MatchDataStr);
		}

		if (newMatchData.Turn != this.matchData.Turn
			|| newMatchData.Round != this.matchData.Round
			|| newMatchData.Phase != this.matchData.Phase
			|| this.lastKnownPlayerMoneyStr != matchDataBroadcaster.PlayerMoneyStr)
		{
			UpdateUI();
			this.lastKnownPlayerMoneyStr = matchDataBroadcaster.PlayerMoneyStr;
		}

		// On New Turn
		// if (gameMan.turn != this.lastKnownTurn
		// 	|| gameMan.round != this.lastKnownRound
		// 	|| gameMan.phase != this.lastKnownPhase
		// 	|| gameMan.playersMoneyStr != this.lastKnownPlayerMoneyStr)
		// {
		// 	UpdateUI();
		// }

	} // Update()

	private void InitPlayerText()
	{
		if (transform.Find("Hud/PhaseNumber") != null
			&& (transform.Find("Hud/PhaseNumber").GetComponent<TMP_Text>() != null))
		{
			phaseNumberText = transform.Find("Hud/PhaseNumber").gameObject.GetComponent<TMP_Text>();
		}

		if (transform.Find("Hud/RoundNumber") != null
			&& (transform.Find("Hud/RoundNumber").GetComponent<TMP_Text>() != null))
		{
			roundNumberText = transform.Find("Hud/RoundNumber").gameObject.GetComponent<TMP_Text>();
		}

		if (transform.Find("Hud/TurnNumber") != null
			&& (transform.Find("Hud/TurnNumber").GetComponent<TMP_Text>() != null))
		{
			turnNumberText = transform.Find("Hud/TurnNumber").gameObject.GetComponent<TMP_Text>();
		}
	} //InitPlayerText()

	private void InitMoneyText()
	{
		for (int i = 0; i < config.MaxPlayerCount; i++)
		{
			if (transform.Find("Hud/Money/Player (" + (i + 1) + ")") != null
				&& (transform.Find("Hud/Money/Player (" + (i + 1) + ")").GetComponent<TMP_Text>() != null))
			{
				TMP_Text newPlayerMoneyText = transform.Find("Hud/Money/Player (" + (i + 1) + ")").gameObject.GetComponent<TMP_Text>();
				playerMoneyText.Insert(i, newPlayerMoneyText);
				playerMoneyText[i].text = "Player " + (i + 1) + "'s Cash: $0";
			}
			else
			{
				Debug.Log(debug + "Hud/Money/Player (" + (i + 1) + ")" + " was null?");
			}
		}
	} // InitMoneyText()

	private void UpdateUI()
	{
		UpdatePhaseRoundTurnUI();
		UpdateMoneyUI();
	}

	private void UpdatePhaseRoundTurnUI()
	{
		// this.lastKnownTurn = gameMan.turn;
		// this.lastKnownRound = gameMan.round;
		// this.lastKnownPhase = gameMan.phase;

		this.lastKnownMatchDataStr = matchDataBroadcaster.MatchDataStr;
		this.matchData = JsonUtility.FromJson<MatchData>(this.lastKnownMatchDataStr);

		this.phaseNumberText.text = ("Phase " + this.matchData.Phase);
		this.roundNumberText.text = ("Round " + this.matchData.Round);
		this.turnNumberText.text = ("Player " + this.matchData.Turn + "'s Turn");
	}

	private void UpdateMoneyUI()
	{
		this.lastKnownPlayerMoneyStr = matchDataBroadcaster.PlayerMoneyStr;

		if (this.lastKnownPlayerMoneyStr != "")
		{
			string[] playersMoneyStr = this.lastKnownPlayerMoneyStr.Split('_');
			Debug.Log(debug + "Player Money String: " + this.lastKnownPlayerMoneyStr);

			for (int i = 0; i < playersMoneyStr.Length; i++)
			{
				this.playerMoneyAmounts.Insert(i, (int)(double.Parse(playersMoneyStr[i])));
				this.playerMoneyText[i].text = "Player " + (i + 1) + "'s Cash: $" + playerMoneyAmounts[i];
			}
		}
	}

} // class HudController

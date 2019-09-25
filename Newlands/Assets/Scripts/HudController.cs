// Controls and updates the HUD based on changing values in MatchDataBroadcaster.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudController : MonoBehaviour
{
	private MatchDataBroadcaster matchDataBroadcaster;

	[SerializeField]
	private TMP_Text phaseNumberText;
	[SerializeField]
	private TMP_Text roundNumberText;
	[SerializeField]
	private TMP_Text turnNumberText;

	private MatchData matchData;
	private MatchConfig config;
	private string matchDataStr;
	private string lastKnownPlayerMoneyStr = "";

	[SerializeField]
	private List<TMP_Text> playerMoneyText = new List<TMP_Text>();
	private List<int> playerMoneyAmounts = new List<int>();

	private bool initialized = false;
	private static DebugTag debugTag = new DebugTag("HudController", "4CAF50");

	void Awake()
	{
		StartCoroutine(Initialize());
	}

	// void Start()
	// {
	// 	UpdateUI();
	// }

	void Update()
	{
		// On New Turn
		if (matchDataBroadcaster != null
			&& this.matchDataStr != matchDataBroadcaster.MatchDataStr)
		{
			this.matchData = JsonUtility.FromJson<MatchData>(matchDataBroadcaster.MatchDataStr);
			this.matchDataStr = matchDataBroadcaster.MatchDataStr;
			UpdateUI();
		}
	}

	private IEnumerator Initialize()
	{
		// Grab the MatchDataBroadcaster
		if (matchDataBroadcaster == null)
			matchDataBroadcaster = FindObjectOfType<MatchDataBroadcaster>();

		if (matchDataBroadcaster == null)
			yield return null;

		// Grab the Match Text
		yield return StartCoroutine(InitMatchText());

		// Get the match config
		while (matchDataBroadcaster.MatchConfigStr == null)
			yield return null;

		config = JsonUtility.FromJson<MatchConfig>(matchDataBroadcaster.MatchConfigStr);

		// Get the current match data
		while (matchDataBroadcaster.MatchDataStr == null)
			yield return null;

		this.matchData = JsonUtility.FromJson<MatchData>(matchDataBroadcaster.MatchDataStr);
		this.matchDataStr = matchDataBroadcaster.MatchDataStr;

		// Grab the Money Text
		yield return StartCoroutine(InitMoneyText());

		// Set the initialized flag to true
		initialized = true;

		// Finally, send the first UI update
		UpdateUI();
	}

	private IEnumerator InitMatchText()
	{
		if (transform.Find("Hud/PhaseNumber") != null
			&& (transform.Find("Hud/PhaseNumber").GetComponent<TMP_Text>() != null))
		{
			phaseNumberText = transform.Find("Hud/PhaseNumber").gameObject.GetComponent<TMP_Text>();

			if (phaseNumberText == null)
				yield return null;
		}

		if (transform.Find("Hud/RoundNumber") != null
			&& (transform.Find("Hud/RoundNumber").GetComponent<TMP_Text>() != null))
		{
			roundNumberText = transform.Find("Hud/RoundNumber").gameObject.GetComponent<TMP_Text>();

			if (roundNumberText == null)
				yield return null;
		}

		if (transform.Find("Hud/TurnNumber") != null
			&& (transform.Find("Hud/TurnNumber").GetComponent<TMP_Text>() != null))
		{
			turnNumberText = transform.Find("Hud/TurnNumber").gameObject.GetComponent<TMP_Text>();

			if (turnNumberText == null)
				yield return null;
		}
	}

	private IEnumerator InitMoneyText()
	{
		for (int i = 0; i < config.MaxPlayerCount; i++)
		{
			if (transform.Find("Hud/Money/Player (" + (i + 1) + ")") != null
				&& (transform.Find("Hud/Money/Player (" + (i + 1) + ")").GetComponent<TMP_Text>() != null))
			{
				TMP_Text newPlayerMoneyText = transform.Find("Hud/Money/Player (" + (i + 1) + ")").gameObject.GetComponent<TMP_Text>();
				playerMoneyText.Insert(i, newPlayerMoneyText);
				playerMoneyText[i].text = "Player " + (i + 1) + "'s Cash: $0";

				if (newPlayerMoneyText == null)
					yield return null;
			}
		}
	}

	private void UpdateUI()
	{
		if (initialized)
		{
			UpdatePhaseRoundTurnUI();
			UpdateMoneyUI();
		}
	}

	private void UpdatePhaseRoundTurnUI()
	{
		this.matchData = JsonUtility.FromJson<MatchData>(matchDataBroadcaster.MatchDataStr);

		if (this.matchData != null)
		{
			this.phaseNumberText.text = ("Phase " + this.matchData.Phase);
			this.roundNumberText.text = ("Round " + this.matchData.Round);
			this.turnNumberText.text = ("Player " + this.matchData.Turn + "'s Turn");
		}
	}

	private void UpdateMoneyUI()
	{
		this.lastKnownPlayerMoneyStr = matchDataBroadcaster.PlayerMoneyStr;

		if (this.lastKnownPlayerMoneyStr != "")
		{
			string[] playersMoneyStr = this.lastKnownPlayerMoneyStr.Split('_');
			Debug.Log(debugTag + "Player Money String: " + this.lastKnownPlayerMoneyStr);

			for (int i = 0; i < playersMoneyStr.Length; i++)
			{
				this.playerMoneyAmounts.Insert(i, (int)(double.Parse(playersMoneyStr[i])));
				this.playerMoneyText[i].text = "Player " + (i + 1) + "'s Cash: $" + playerMoneyAmounts[i];
			}
		}
	}

} // class HudController

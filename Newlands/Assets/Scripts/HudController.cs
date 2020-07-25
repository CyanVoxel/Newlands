// Controls and updates the HUD based on changing values in MatchDataBroadcaster.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
	private MatchDataBroadcaster matchDataBroadcaster;

	[SerializeField]
	private TMP_Text phaseNumberText;
	[SerializeField]
	private TMP_Text roundNumberText;
	[SerializeField]
	private TMP_Text turnNumberText;

	[SerializeField]
	private GameObject winnerObject;
	[SerializeField]
	private TMP_Text winnerText;

	[SerializeField]
	private TMP_Text roundLabel;
	[SerializeField]
	private TMP_Text roundNumber;
	[SerializeField]
	private Image roundCircle;
	[SerializeField]
	private Image roundCirclePinstripe;

	[SerializeField]
	private TMP_Text phaseLabel;
	[SerializeField]
	private Image phasePlate;

	private MatchData matchData;
	private MatchConfig config;
	private string matchDataStr;
	private string lastKnownPlayerMoneyStr = "";

	private int thisPlayerId = 0;
	public int ThisPlayerId { get { return thisPlayerId; } set { thisPlayerId = value; } }

	[SerializeField]
	private List<TMP_Text> playerMoneyText = new List<TMP_Text>();
	private List<int> playerMoneyAmounts = new List<int>();

	[SerializeField]
	private TMP_Text debugText;

	private bool initialized = false;
	private static DebugTag debugTag = new DebugTag("HudController", "4CAF50");

	void Awake()
	{
		Debug.Log(debugTag + "Initializing...");
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
			UpdateHud();
		}
	}

	private IEnumerator Initialize()
	{
		// Grab the MatchDataBroadcaster
		while (matchDataBroadcaster == null)
		{
			matchDataBroadcaster = FindObjectOfType<MatchDataBroadcaster>();
			yield return null;
		}

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
		UpdateHud();
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

		if (transform.Find("Hud/Winner") != null)
		{
			winnerObject = transform.Find("Hud/Winner").gameObject;

			if (winnerObject == null)
				yield return null;
		}

		if (transform.Find("Hud/Winner/WinnerText") != null
			&& (transform.Find("Hud/Winner/WinnerText").GetComponent<TMP_Text>() != null))
		{
			winnerText = transform.Find("Hud/Winner/WinnerText").gameObject.GetComponent<TMP_Text>();

			if (winnerText == null)
				yield return null;
		}

		if (transform.Find("Hud/DebugText") != null
			&& (transform.Find("Hud/DebugText").GetComponent<TMP_Text>() != null))
		{
			debugText = transform.Find("Hud/DebugText").gameObject.GetComponent<TMP_Text>();

			if (debugText == null)
				yield return null;
		}

		winnerObject.SetActive(false);
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

	// Updates all HUD elements
	public void UpdateHud()
	{
		if (initialized)
		{
			Debug.Log(debugTag + "Updating UI... (" + matchData + ")");
			UpdatePhaseRoundTurnHud();
			UpdateMoneyHud();
			UpdateDebugText();
		}
	}

	private void UpdatePhaseRoundTurnHud()
	{
		this.matchData = JsonUtility.FromJson<MatchData>(matchDataBroadcaster.MatchDataStr);

		if (this.matchData != null)
		{
			roundNumber.text = this.matchData.Round.ToString();

			if (thisPlayerId == this.matchData.Turn)
			{
				roundLabel.color = ColorPalette.GetNewlandsColor("Card", 500, false);
				roundNumber.color = ColorPalette.GetNewlandsColor("Card", 500, false);
				roundCircle.color = ColorPalette.GetDefaultPlayerColor(thisPlayerId, 500, true);
				roundCirclePinstripe.color = ColorPalette.GetNewlandsColor("Card", 500, false);
				phaseLabel.color = ColorPalette.GetNewlandsColor("Black", 500, false);
			}
			else
			{
				roundLabel.color = ColorPalette.GetDefaultPlayerColor(this.matchData.Turn, 500, false);
				roundNumber.color = ColorPalette.GetDefaultPlayerColor(this.matchData.Turn, 500, false);
				roundCirclePinstripe.color = ColorPalette.GetDefaultPlayerColor(this.matchData.Turn, 500, false);
				roundCircle.color = Color.white;
				phaseLabel.color = ColorPalette.GetNewlandsColor("Black", 500, false);
			}

			switch (this.matchData.Phase)
			{
				case 1:
					phaseLabel.text = "<b>Buying</b> Phase";
					break;
				case 2:
					phaseLabel.text = "<b>Playing</b> Phase";
					break;
				default:
					break;
			}

			// this.phaseNumberText.text = ("Phase " + this.matchData.Phase);
			// this.roundNumberText.text = ("Round " + this.matchData.Round);
			// this.turnNumberText.text = ("Player " + this.matchData.Turn + "'s Turn");
		}
	}

	private void UpdateMoneyHud()
	{
		this.lastKnownPlayerMoneyStr = matchDataBroadcaster.PlayerMoneyStr;

		if (this.lastKnownPlayerMoneyStr != "")
		{
			this.playerMoneyAmounts = MatchController.UnpackPlayerMoneyStr(this.lastKnownPlayerMoneyStr);

			Debug.Log(debugTag + "Player Money String: " + this.lastKnownPlayerMoneyStr);

			for (int i = 0; i < config.MaxPlayerCount; i++)
			{
				this.playerMoneyText[i].text = "Player " + (i + 1) + "'s Cash: $" + playerMoneyAmounts[i];
			}
		}
	}

	public void DisplayWinner(int winnerId)
	{
		winnerText.text = ("PLAYER " + winnerId + " WINS!");
		winnerObject.SetActive(true);
	}

	private void UpdateDebugText()
	{
		debugText.text = "";
		debugText.text += ("GAMECARD DECK SIZE: " + MatchController.MasterDeck.gameCardDeck.Count + "\n");
		debugText.text += ("CARDS LEFT IN DECK: " + MatchController.MasterDeckMutable.gameCardDeck.Count + "\n");
		debugText.text += ("# OF CARDS PLAYED: " + MatchController.CardsPlayed + "\n");
		debugText.text += ("CURRENT CLIENT LEADER: " + MatchController.FindClientsideLeader(lastKnownPlayerMoneyStr) + "\n");
		// debugText.text += ("CURRENT SERVER LEADER: "
	}

} // class HudController

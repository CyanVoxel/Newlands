// Controller for the Multiplayer Game Setup scene.
// Creates a game config that's later used by MatchController during a game.

using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchSetupController : NetworkBehaviour
{
	GameObject networkManagerObj;
	NetworkManager networkManager;
	TelepathyTransport telepathyTransport;

	private TMP_Dropdown playerCountDropdown;
	private TMP_Dropdown gridSizeDropdown;
	private TMP_Dropdown winConditionDropdown;
	private TMP_InputField ipInputField;
	private TMP_InputField portInputField;
	private Image noIpWarning;

	[SerializeField]
	private GameObject matchManagerPrefab;
	private GameObject matchManagerReference;
	private MatchController matchController;
	private bool ready = false;

	private MatchConfig initialConfig;
	public MatchConfig InitialConfig { get { return initialConfig; } }
	public bool Ready { get { return ready; } }

	private DebugTag debugTag = new DebugTag("MatchSetupController", "f44336");

	void Start()
	{
		Debug.Log(debugTag + "Initializing...");

		networkManagerObj = GameObject.Find("NetworkManager");
		networkManager = networkManagerObj.GetComponent<NetworkManager>();
		telepathyTransport = networkManagerObj.GetComponent<TelepathyTransport>();

		GrabGuiComponents();
	}

	void Update()
	{
		if (ipInputField != null)
		{
			if (ipInputField.text != "")
				noIpWarning.color = ColorPalette.alpha;
		}
		// Debug.Log(this.networkManager.networkAddress);
	}

	private void GrabGuiComponents()
	{
		// Grab the Player Count Dropdown
		GameObject playerCountDropdownObj = GameObject.Find("PlayersDropdown");
		if (playerCountDropdownObj != null)
			playerCountDropdown = playerCountDropdownObj.GetComponent<TMP_Dropdown>();
		else
			Debug.LogError(debugTag.error + "Could not find PlayersDropdown!");

		// Grab the Grid Size Dropdown
		GameObject gridSizeDropdownObj = GameObject.Find("GridSizeDropdown");
		if (gridSizeDropdownObj != null)
			gridSizeDropdown = gridSizeDropdownObj.GetComponent<TMP_Dropdown>();
		else
			Debug.LogError(debugTag.error + "Could not find GridSizeDropdown!");

		// Grab the Win Condition Dropdown
		GameObject winConditionDropdownObj = GameObject.Find("WinConditionDropdown");
		if (winConditionDropdownObj != null)
			winConditionDropdown = winConditionDropdownObj.GetComponent<TMP_Dropdown>();
		else
			Debug.LogError(debugTag.error + "Could not find WinConditionDropdown!");

		// Grab the IP Input Field
		GameObject ipInputFieldObj = GameObject.Find("IpInputField");
		if (ipInputFieldObj != null)
			ipInputField = ipInputFieldObj.GetComponent<TMP_InputField>();
		else
			Debug.LogError(debugTag.error + "Could not find IpInputField!");

		// Grab the IP Input Field
		GameObject portInputFieldObj = GameObject.Find("PortInputField");
		if (portInputFieldObj != null)
			portInputField = portInputFieldObj.GetComponent<TMP_InputField>();
		else
			Debug.LogError(debugTag.error + "Could not find PortInputField!");

		GameObject noIpWarningObj = GameObject.Find("NoIpWarning");
		if (noIpWarningObj != null)
			noIpWarning = noIpWarningObj.GetComponent<Image>();
		else
			Debug.LogError(debugTag.error + "Could not find NoIpWarning!");
	}

	public void HostGameButtonClick()
	{
		CreateInitialConfig();
		if (!NetworkClient.isConnected && !NetworkServer.active)
		{
			if (!NetworkClient.active)
			{
				networkManager.networkAddress = ipInputField.text; // Does this need to be here when hosting?
				telepathyTransport.port = ushort.Parse(portInputField.text);
				networkManager.StartHost();
				SceneManager.LoadScene("GameMultiplayer", LoadSceneMode.Single);
			}
		}
		CreateMatchManager();
	}

	public void JoinGameButtonClick()
	{
		noIpWarning.color = ColorPalette.alpha;
		if (!NetworkClient.isConnected && !NetworkServer.active)
		{
			if (!NetworkClient.active)
			{
				if (ipInputField.text != "")
				{
					networkManager.networkAddress = ipInputField.text;
					telepathyTransport.port = ushort.Parse(portInputField.text);
					networkManager.StartClient();
					SceneManager.LoadScene("GameMultiplayer", LoadSceneMode.Single);
				}
				else
				{
					noIpWarning.color = ColorPalette.GetNewlandsColor("Red", 500, false);
				}

			}
		}
	}

	private void CreateInitialConfig()
	{
		int playerCount = int.Parse(playerCountDropdown.options[playerCountDropdown.value].text);
		int height;
		int width;
		string[] tempGridDim = gridSizeDropdown.options[gridSizeDropdown.value].text.Split('x');
		height = int.Parse(tempGridDim[0]);
		width = int.Parse(tempGridDim[1]);

		// Determine the number of grace round to set based on the player count
		int finalGraceRounds;

		if (playerCount == 1)
			finalGraceRounds = 1;
		else
			finalGraceRounds = Mathf.CeilToInt(((float)playerCount / 2f));

		Debug.Log(debugTag + "Setting Grace Rounds to " + finalGraceRounds
			+ " for " + playerCount + " Players.");

		// NOTE: If a value is hard-coded here, then there is no option for it during Game Setup.
		this.initialConfig = new MatchConfig(
			deckFlavor: "Vanilla",
			winCondition: "WCplaceholder",
			gameGridHeight : height,
			gameGridWidth : width,
			maxPlayerCount : playerCount,
			playerHandSize : 5,
			graceRounds : finalGraceRounds);

		this.ready = true;
	}

	private void CreateMatchManager()
	{
		// Create the Match Manager
		matchManagerReference = Instantiate(matchManagerPrefab,
			new Vector3(0, 0, 0), Quaternion.identity);
		if (matchManagerReference != null)
		{
			NetworkServer.Spawn(matchManagerReference);
			matchController = matchManagerReference.GetComponent<MatchController>();
		}
	}
}

// Controller for the Lobby scene.
// Creates a game config that's later used by MatchController during a game.

using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyController : NetworkBehaviour
{
	// GameObject networkManagerObj;
	// [SerializeField]
	NewlandsNetworkManager newlandsNetworkManager;
	// [SerializeField]
	// TelepathyTransport telepathyTransport;

	[SerializeField]
	private GameObject matchManagerPrefab;
	private GameObject matchManagerReference;
	private MatchController matchController;

	// private TMP_Dropdown playerCountDropdown;
	[SerializeField]
	private TMP_Dropdown gridSizeDropdown;
	[SerializeField]
	private TMP_Dropdown winConditionDropdown;
	// private TMP_InputField ipInputField;
	// private TMP_InputField portInputField;
	// private TMP_InputField usernameInputField;
	// private TMP_Text usernamePlaceholder;
	// private Image noIpWarning;

	// [SerializeField]
	// private GameObject matchManagerPrefab;
	// private GameObject matchManagerReference;
	// private MatchController matchController;
	[SerializeField]
	// private NewlandsNetworkManager matchConnections;
	private bool configCreated = false;

	private MatchConfig initialConfig;
	public MatchConfig InitialConfig { get { return initialConfig; } }
	public bool ConfigCreated { get { return configCreated; } }

	private string username;

	private DebugTag debugTag = new DebugTag("LobbyController", "f44336");

	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}

	// void Start()
	// {
	// 	Debug.Log(debugTag + "Initializing...");

	// 	// networkManagerObj = GameObject.Find("NetworkManager");
	// 	// networkManager = networkManagerObj.GetComponent<NetworkManager>();
	// 	// telepathyTransport = networkManagerObj.GetComponent<TelepathyTransport>();

	// 	// GrabGuiComponents();

	// 	// if (usernamePlaceholder != null)
	// 	// 	usernamePlaceholder.text = UsernameUtility.GeneratePlaceholerUsername();
	// }

	void Update()
	{
		if (newlandsNetworkManager == null)
		{
			GameObject temp = GameObject.Find("NetworkManager");
			if (temp != null)
			{
				newlandsNetworkManager = temp.GetComponent<NewlandsNetworkManager>();
			}
		}
	}

	private void CreateInitialConfig()
	{
		// int playerCount = int.Parse(playerCountDropdown.options[playerCountDropdown.value].text);
		int playerCount = 2;
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

		this.configCreated = true;
	}

	// private void CreateMatchManager()
	// {
	// 	// Create the Match Manager
	// 	matchManagerReference = Instantiate(matchManagerPrefab,
	// 		new Vector3(0, 0, 0), Quaternion.identity);
	// 	if (matchManagerReference != null)
	// 	{
	// 		NetworkServer.Spawn(matchManagerReference);
	// 		matchController = matchManagerReference.GetComponent<MatchController>();
	// 	}
	// }

	// Called on the server when in the lobby are ready.
	// Changes scene for everyone, though.
	public void StartMatch()
	{
		// CreateMatchManager();
		// RpcClientBootstrapper();


		if (newlandsNetworkManager != null)
		{
			CreateInitialConfig();
			// CreateMatchManager();
			// Debug.Log(debugTag + "------------------------------------------ SCENE CHANGE -------------------------------------");
			newlandsNetworkManager.ServerChangeScene("GameMultiplayer");
		}
		else
		{
			Debug.LogWarning(debugTag.warning + "HEY I DON'T THINK YOU WANT TO START THE MATCH WITHOUT THE NETWORKMANAGER");
		}
	}
}

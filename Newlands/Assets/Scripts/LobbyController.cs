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

	[SerializeField]
	private GameObject matchManagerPrefab;
	private GameObject matchManagerReference;
	private MatchController matchController;
	[SerializeField]
	// private NewlandsNetworkManager matchConnections;
	private bool configCreated = false;

	private MatchConfig initialConfig;
	public MatchConfig InitialConfig { get { return initialConfig; } }
	public bool ConfigCreated { get { return configCreated; } }

	private string username;

	private DebugTag debugTag = new DebugTag("MatchSetupController", "f44336");

	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}

	void Start()
	{
		Debug.Log(debugTag + "Initializing...");

		// networkManagerObj = GameObject.Find("NetworkManager");
		// networkManager = networkManagerObj.GetComponent<NetworkManager>();
		// telepathyTransport = networkManagerObj.GetComponent<TelepathyTransport>();

		// GrabGuiComponents();

		// if (usernamePlaceholder != null)
		// 	usernamePlaceholder.text = UsernameUtility.GeneratePlaceholerUsername();
	}

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

	// void Update()
	// {
	// 	if (matchConnections == null)
	// 	{
	// 		GameObject matchConObj = GameObject.Find("NetworkManager");
	// 		if (matchConObj != null)
	// 		{
	// 			matchConnections = matchConObj.GetComponent<NewlandsNetworkManager>();
	// 		}
	// 	}
	// 	// if (ipInputField != null)
	// 	// {
	// 	// 	if (ipInputField.text != "")
	// 	// 		noIpWarning.color = ColorPalette.alpha;
	// 	// }
	// 	// Debug.Log(this.networkManager.networkAddress);
	// }

	// private void GrabGuiComponents()
	// {
	// 	// Grab the Player Count Dropdown
	// 	GameObject playerCountDropdownObj = GameObject.Find("PlayersDropdown");
	// 	if (playerCountDropdownObj != null)
	// 		playerCountDropdown = playerCountDropdownObj.GetComponent<TMP_Dropdown>();
	// 	else
	// 		Debug.Log(debugTag.warning + "Could not find PlayersDropdown!");

	// 	// Grab the Grid Size Dropdown
	// 	GameObject gridSizeDropdownObj = GameObject.Find("GridSizeDropdown");
	// 	if (gridSizeDropdownObj != null)
	// 		gridSizeDropdown = gridSizeDropdownObj.GetComponent<TMP_Dropdown>();
	// 	else
	// 		Debug.Log(debugTag.warning + "Could not find GridSizeDropdown!");

	// 	// Grab the Win Condition Dropdown
	// 	GameObject winConditionDropdownObj = GameObject.Find("WinConditionDropdown");
	// 	if (winConditionDropdownObj != null)
	// 		winConditionDropdown = winConditionDropdownObj.GetComponent<TMP_Dropdown>();
	// 	else
	// 		Debug.Log(debugTag.warning + "Could not find WinConditionDropdown!");

	// 	// Grab the IP Input Field
	// 	GameObject ipInputFieldObj = GameObject.Find("IpInputField");
	// 	if (ipInputFieldObj != null)
	// 		ipInputField = ipInputFieldObj.GetComponent<TMP_InputField>();
	// 	else
	// 		Debug.Log(debugTag.warning + "Could not find IpInputField!");

	// 	// Grab the Port Input Field
	// 	GameObject portInputFieldObj = GameObject.Find("PortInputField");
	// 	if (portInputFieldObj != null)
	// 		portInputField = portInputFieldObj.GetComponent<TMP_InputField>();
	// 	else
	// 		Debug.Log(debugTag.warning + "Could not find PortInputField!");

	// 	// Grab the little red arrow that shows up when you don't type in an ip
	// 	GameObject noIpWarningObj = GameObject.Find("NoIpWarning");
	// 	if (noIpWarningObj != null)
	// 		noIpWarning = noIpWarningObj.GetComponent<Image>();
	// 	else
	// 		Debug.Log(debugTag.warning + "Could not find NoIpWarning!");

	// 	// Grab the Username Input Field
	// 	GameObject usernameInputFieldObj = GameObject.Find("UsernameInputField");
	// 	if (usernameInputFieldObj != null)
	// 		usernameInputField = usernameInputFieldObj.GetComponent<TMP_InputField>();
	// 	else
	// 		Debug.Log(debugTag.warning + "Could not find UsernameInputField!");

	// 	// Grab the Username Placeholder
	// 	GameObject usernamePlaceholderObj = GameObject.Find("UsernamePlaceholder");
	// 	if (usernamePlaceholderObj != null)
	// 		usernamePlaceholder = usernamePlaceholderObj.GetComponent<TMP_Text>();
	// 	else
	// 		Debug.Log(debugTag.warning + "Could not find UsernamePlaceholder!");
	// }

	// public void HostGameButtonClick()
	// {
	// 	// CreateInitialConfig();
	// 	if (!NetworkClient.isConnected && !NetworkServer.active)
	// 	{
	// 		if (!NetworkClient.active)
	// 		{
	// 			// networkManager.networkAddress = ipInputField.text; // Does this need to be here when hosting?
	// 			telepathyTransport.port = ushort.Parse(portInputField.text);
	// 			networkManager.StartHost();
	// 			// SceneManager.LoadScene("GameMultiplayer", LoadSceneMode.Single);
	// 		}
	// 	}
	// 	CreateMatchManager();

	// 	FinalizeUsername();
	// }

	// public void JoinGameButtonClick()
	// {
	// 	noIpWarning.color = ColorPalette.alpha;
	// 	if (!NetworkClient.isConnected && !NetworkServer.active)
	// 	{
	// 		if (!NetworkClient.active)
	// 		{
	// 			if (ipInputField.text != "")
	// 			{
	// 				networkManager.networkAddress = ipInputField.text;
	// 				telepathyTransport.port = ushort.Parse(portInputField.text);
	// 				networkManager.StartClient();
	// 				// SceneManager.LoadScene("GameMultiplayer", LoadSceneMode.Single);
	// 			}
	// 			else
	// 			{
	// 				noIpWarning.color = ColorPalette.GetNewlandsColor("Red", 500, false);
	// 			}

	// 		}
	// 	}

	// 	FinalizeUsername();
	// 	// CmdRetrieveUsername(username);
	// }

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

	// Called when all players in the lobby are ready.
	public void StartMatch()
	{
		if (newlandsNetworkManager != null)
		{
			CreateInitialConfig();
			CreateMatchManager();
			// Debug.Log(debugTag + "------------------------------------------ SCENE CHANGE -------------------------------------");
			newlandsNetworkManager.ServerChangeScene("GameMultiplayer");
		}
		else
		{
			Debug.LogWarning(debugTag.warning + "HEY I DON'T THINK YOU WANT TO START THE MATCH WITHOUT THE NETWORKMANAGER");
		}

	}

	// private void FinalizeUsername()
	// {
	// 	// If PlayerDataContainer works, then setting the internal username is now useless.
	// 	if (usernameInputField.text != "")
	// 		username = usernameInputField.text;
	// 	else
	// 		username = usernamePlaceholder.text;

	// 	// Ship that bad boy off to the PlayerDataContainer, and wish it good luck.
	// 	PlayerDataContainer.Username = username;
	// }

	// private string GeneratePlaceholerUsername()
	// {
	// 	List<string> prefixes = new List<string>();
	// 	List<string> suffixes = new List<string>();
	// 	string generatedName = "";

	// 	prefixes.Add("Atom");
	// 	prefixes.Add("Graceful");
	// 	prefixes.Add("Angry");
	// 	prefixes.Add("Happy");
	// 	prefixes.Add("Sad");
	// 	prefixes.Add("Smart");
	// 	prefixes.Add("Mighty");
	// 	prefixes.Add("Cool");
	// 	prefixes.Add("Hot");
	// 	prefixes.Add("Sweaty");
	// 	prefixes.Add("Calm");
	// 	prefixes.Add("Cheesy");
	// 	prefixes.Add("Weird");
	// 	prefixes.Add("Spicy");
	// 	prefixes.Add("Sleepy");
	// 	prefixes.Add("Epic");

	// 	suffixes.Add("Gamer");
	// 	suffixes.Add("Thinker");
	// 	suffixes.Add("Sauce");
	// 	suffixes.Add("Player");
	// 	suffixes.Add("Newlandian");
	// 	suffixes.Add("Void");
	// 	suffixes.Add("Turtle");
	// 	suffixes.Add("Cat");
	// 	suffixes.Add("Dog");
	// 	suffixes.Add("Hamster");
	// 	suffixes.Add("Champ");
	// 	suffixes.Add("Peperoni");
	// 	suffixes.Add("Pizza");
	// 	suffixes.Add("Calzone");
	// 	suffixes.Add("Noodle");
	// 	suffixes.Add("Cannoli");

	// 	generatedName += prefixes[Random.Range(0, prefixes.Count)];
	// 	generatedName += suffixes[Random.Range(0, suffixes.Count)];
	// 	generatedName += Random.Range(0, 100);

	// 	return generatedName;
	// }

	// [Command]
	// public void CmdRetrieveUsername(string name)
	// {
	// 	Debug.Log("I've been told to grab a username.");
	// 	matchController.AssignUsername(name);
	// }

	// [TargetRpc]
	// public void TargetGetUsername()
	// {
	// 	Debug.Log("Giving up my username of " + username);
	// }
}

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

    private MatchConfigData initialConfig;
    public MatchConfigData InitialConfig { get { return initialConfig; } }
    public bool Ready { get { return ready; } }

    private DebugTag debugTag = new DebugTag("MatchSetupController", "f44336");

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(debugTag + "Initializing...");

        networkManagerObj = GameObject.Find("NetworkManager");
        networkManager = networkManagerObj.GetComponent<NetworkManager>();
        telepathyTransport = networkManagerObj.GetComponent<TelepathyTransport>();

        GrabGuiComponents();

    }

    // Update is called once per frame
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
        if (playerCountDropdown != null)
            playerCountDropdown = playerCountDropdownObj.GetComponent<TMP_Dropdown>();

        // Grab the Grid Size Dropdown
        GameObject gridSizeDropdownObj = GameObject.Find("GridSizeDropdown");
        if (gridSizeDropdownObj != null)
            gridSizeDropdown = gridSizeDropdownObj.GetComponent<TMP_Dropdown>();

        // Grab the Win Condition Dropdown
        GameObject winConditionDropdownObj = GameObject.Find("WinConditionDropdown");
        if (winConditionDropdownObj != null)
            winConditionDropdown = winConditionDropdownObj.GetComponent<TMP_Dropdown>();

        // Grab the IP Input Field
        GameObject ipInputFieldObj = GameObject.Find("IpInputField");
        if (ipInputFieldObj != null)
            ipInputField = ipInputFieldObj.GetComponent<TMP_InputField>();

        // Grab the IP Input Field
        GameObject portInputFieldObj = GameObject.Find("PortInputField");
        if (portInputField != null)
            portInputField = portInputFieldObj.GetComponent<TMP_InputField>();

        GameObject noIpWarningObj = GameObject.Find("NoIpWarning");
        if (noIpWarningObj != null)
            noIpWarning = noIpWarningObj.GetComponent<Image>();
    }

    public void HostGameButtonClick()
    {
        CreateInitialConfig();
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                networkManager.networkAddress = ipInputField.text; // Does this need to be here when hosting?
                // telepathyTransport.port = ushort.Parse(portInputField.text);
                networkManager.StartHost();
                // SceneManager.LoadScene("GameMultiplayer", LoadSceneMode.Single);
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
                    // telepathyTransport.port = ushort.Parse(portInputField.text);
                    networkManager.StartClient();
                    // SceneManager.LoadScene("GameMultiplayer", LoadSceneMode.Single);
                }
                else
                {
                    noIpWarning.color = ColorPalette.cardRed500;
                }

            }
        }
    }

    private void CreateInitialConfig()
    {
        // TODO: Connect these to grabbed valued from the fields
        this.initialConfig = new MatchConfigData("Vanilla", "WCplaceholder", 6, 9, 4, 8);
        this.ready = true;
    }

    private void CreateMatchManager()
    {
        // Create the Match Manager
        matchManagerReference = Instantiate(matchManagerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        if (matchManagerReference != null)
        {
            NetworkServer.Spawn(matchManagerReference);
            matchController = matchManagerReference.GetComponent<MatchController>();
        }
    }
}

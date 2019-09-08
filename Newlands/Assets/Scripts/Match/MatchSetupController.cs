// Sets up a match with data ready for MatchController

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MatchSetupController : NetworkBehaviour
{
    private TMP_Dropdown playerCountDropdown;
    private TMP_Dropdown gridSizeDropdown;
    private TMP_Dropdown winConditionDropdown;
    private TMP_InputField ipInputField;

    private Button hostButton;

    [SerializeField]
    private GameObject matchManagerPrefab;
    private GameObject matchManagerReference;
    private MatchController matchController;
    private bool ready = false;

    private MatchConfigData initialConfig;
    public MatchConfigData InitialConfig { get { return initialConfig; } }
    public bool Ready { get { return ready; } }

    // Start is called before the first frame update
    void Start()
    {

        // Grab the Player Count Dropdown
        GameObject playerCountDropdownObj = GameObject.Find("Players Dropdown");
        if (playerCountDropdown != null)
        {
            playerCountDropdown = playerCountDropdownObj.GetComponent<TMP_Dropdown>();
        }

        // Grab the Grid Size Dropdown
        GameObject gridSizeDropdownObj = GameObject.Find("Grid Size Dropdown");
        if (playerCountDropdown != null)
        {
            gridSizeDropdown = gridSizeDropdownObj.GetComponent<TMP_Dropdown>();
        }

        // Grab the Win Condition Dropdown
        GameObject winConditionDropdownObj = GameObject.Find("Win Condition Dropdown");
        if (playerCountDropdown != null)
        {
            winConditionDropdown = winConditionDropdownObj.GetComponent<TMP_Dropdown>();
        }

        // Grab the IP Input Field
        GameObject ipInputFieldObj = GameObject.Find("IP InputField");
        if (playerCountDropdown != null)
        {
            ipInputField = ipInputFieldObj.GetComponent<TMP_InputField>();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HostGameButtonClick()
    {
        CreateInitialConfig();
        NetworkManager networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        networkManager.StartHost();
        CreateMatchManager();
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

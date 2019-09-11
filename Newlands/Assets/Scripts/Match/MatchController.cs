// The replacement for GameManager

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MatchController : NetworkBehaviour
{
	// FIELDS ######################################################################################
	[SerializeField]
	private MatchDataBroadcaster matchDataBroadcaster;
	private MatchData matchData;
	private MatchConfigData config;
	public MatchData MatchData { get { return matchData; } }

	private MasterDeck masterDeck;
	private MasterDeck masterDeckMutable;

	public MasterDeck MasterDeck { get { return masterDeck; } }
	public MasterDeck MasterDeckMutable { get { return masterDeckMutable; } }

	public GameObject landTilePrefab;
	public GameObject gameCardPrefab;
	public GameObject marketCardPrefab;

	private DebugTag debugTag = new DebugTag("MatchController", "9C27B0");

	// METHODS #####################################################################################

	void Awake()
	{
		Debug.Log(debugTag + "The MatchController has been created!");
		DontDestroyOnLoad(this.gameObject);
		this.gameObject.AddComponent<GridController>();
	}

	void Start()
	{
		if (!hasAuthority)
			return;

		Debug.Log(debugTag + "Initializing...");
		InitializeMatch();
	}

	// void Update()
	// {
	// 	// Debug.Log(matchDataBroadcaster.MatchConfigDataStr);
	// }

	void OnDisable()
	{
		Debug.Log(debugTag + "The MatchController has been disbaled/destroyed!");
	}

	// [Server] Grabs the MatchDataBroadcaster from this parent GameObject
	// On the client, they will grab the same MatchDataBroadcaster themselves in a different script.
	private void GrabMatchDataBroadCaster()
	{
		matchDataBroadcaster = this.gameObject.GetComponent<MatchDataBroadcaster>();
		if (matchDataBroadcaster != null)
		{
			Debug.Log(debugTag + "MatchDataBroadcaster was found!");
		}
		else
		{
			Debug.LogError(debugTag.error + "MatchDataBroadcaster was NOT found!");
		}
	}

	// [Server] Loads the config from MatchSetupController and initializes the match.
	private void InitializeMatch()
	{
		GrabMatchDataBroadCaster();

		GameObject matchSetupManager = GameObject.Find("SetupManager");
		if (matchSetupManager != null)
		{
			Debug.Log(debugTag + "SetupManager (our dad) was found!");

			MatchSetupController setupController = matchSetupManager.GetComponent<MatchSetupController>();
			StartCoroutine(LoadMatchConfigCoroutine(setupController));
			StartCoroutine(InitializeMatchCoroutine());
		}
		else
		{
			Debug.LogError(debugTag.error + "SetupManager (our dad) was NOT found!");
		}
	}

	// COROUTINES ##################################################################################

	// [Server] The main initialization coroutine for the match.
	// Dependant on LoadMatchConfigCoroutine finishing.
	private IEnumerator InitializeMatchCoroutine()
	{
		// Will not run if LoadMatchConfigCoroutine() has not been run.
		// This gives the functionality of yield return LoadMatchConfigCoroutine()
		// without needing to pass this/it the MatchSetupController.
		while (this.config == null)
			yield return null;

		Debug.Log(debugTag + "Initializing Match from Config data...");

		this.masterDeck = new MasterDeck(config.DeckFlavor);
		this.masterDeckMutable = new MasterDeck(config.DeckFlavor);
	}

	// [Server] Loads this MatchManager's config from the MatchSetupController's final config.
	private IEnumerator LoadMatchConfigCoroutine(MatchSetupController controller)
	{
		Debug.Log(debugTag + "Grabbing config from MatchSetupController...");

		while (!controller.Ready)
			yield return null;

		this.config = controller.InitialConfig;
		Debug.Log(debugTag + "Config loaded! Sending to Broadcaster...");
		matchDataBroadcaster.MatchConfigDataStr = JsonUtility.ToJson(controller.InitialConfig);
	}

}

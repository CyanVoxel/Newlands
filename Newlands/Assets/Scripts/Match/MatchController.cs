// The replacement for GameManager

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MatchController : NetworkBehaviour
{
	[SerializeField]
	private MatchDataBroadcaster matchDataBroadcaster;
	private MatchData matchData;
	private MatchConfigData config;
	public MatchData MatchData { get { return matchData; } }

	private DebugTag debugTag = new DebugTag("MatchController", "9C27B0");

	void Awake()
	{
		Debug.Log(debugTag + "The MatchController has been created!");

		DontDestroyOnLoad(this.gameObject);
	}

	// Start is called before the first frame update
	void Start()
	{

		if (!hasAuthority)
		{
			return;
		}

		Debug.Log(debugTag + "Initializing...");

		GrabMatchDataBroadCaster();
		LoadMatchConfiguration();

	}

	// Update is called once per frame
	void Update()
	{
		// if (this.config != null)
		// {
		// 	Debug.Log(this.config.GameGridHeight);
		// }
		// Debug.Log(matchDataBroadcaster.MatchConfigDataStr);
	}

	void OnDisable()
	{
		Debug.Log(debugTag + "The MatchController has been disbaled/destroyed!");
	}

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

	private void LoadMatchConfiguration()
	{
		GameObject matchSetupManager = GameObject.Find("SetupManager");
		if (matchSetupManager != null)
		{
			Debug.Log(debugTag + "SetupManager (our dad) was found!");

			MatchSetupController setupController = matchSetupManager.GetComponent<MatchSetupController>();
			StartCoroutine(GrabMatchConfigCoroutine(setupController));
			StartCoroutine(MatchSetupCoroutine());

		}
		else
		{
			Debug.LogError(debugTag.error + "SetupManager (our dad) was NOT found!");
		}
	}

	private IEnumerator MatchSetupCoroutine()
	{
		while (this.config == null)
		{
			yield return null;
		}
		Debug.Log(debugTag + "Config loaded! Sending to Broadcaster...");
		matchDataBroadcaster.MatchConfigDataStr = "hello " + Random.Range(1, 100);
	}

	private IEnumerator GrabMatchConfigCoroutine(MatchSetupController controller)
	{
		Debug.Log(debugTag + "Grabbing config from MatchSetupController...");
		while (!controller.Ready)
		{
			// Debug.Log("[MatchController] [GrabMatchConfigCoroutine] Controller is not ready yet!");
			yield return null;
		}
		this.config = controller.InitialConfig;
	}
}

// The replacement for GameManager

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MatchController : NetworkBehaviour
{
	[SerializeField]
	private MatchDataBroadcaster matchDataBroadcaster;
	private MatchData matchData;
	private MatchConfigData config;
	public MatchData MatchData { get { return matchData; } }

	// Start is called before the first frame update
	void Start()
	{
		Debug.Log("[MatchController] The MatchController has been created!");

		if (!hasAuthority)
		{
			return;
		}

		Debug.Log("[MatchController] Initializing...");

		GameObject matchSetupManager = GameObject.Find("SetupManager");
		if (matchSetupManager != null)
		{
			Debug.Log("[MatchController] SetupManager (our dad) was found!");

			MatchSetupController setupController = matchSetupManager.GetComponent<MatchSetupController>();
			StartCoroutine(GrabMatchConfigCoroutine(setupController));
			StartCoroutine(MatchSetupCoroutine());

		}
		else
		{
			Debug.Log("[MatchController] ERROR: SetupManager (our dad) was NOT found!");
		}

		matchDataBroadcaster = this.gameObject.GetComponent<MatchDataBroadcaster>();
		if (matchDataBroadcaster != null)
		{
			Debug.Log("[MatchController] MatchDataBroadcaster was found!");
		}
		else
		{
			Debug.Log("[MatchController] ERROR: MatchDataBroadcaster was NOT found!");
		}

	}

	// Update is called once per frame
	void Update()
	{
		// if (this.config != null)
		// {
		// 	Debug.Log(this.config.GameGridHeight);
		// }
		Debug.Log(matchDataBroadcaster.MatchConfigDataStr);
	}

	void OnDisable()
	{
		Debug.Log("The MatchController has been disbaled/destroyed!");
	}

	private IEnumerator MatchSetupCoroutine()
	{
		while (this.config == null)
		{
			yield return null;
		}
		Debug.Log("Config loaded! Broadcasting...");
		matchDataBroadcaster.MatchConfigDataStr = "hello";
	}

	private IEnumerator GrabMatchConfigCoroutine(MatchSetupController controller)
	{
		Debug.Log("[MatchController] [GrabMatchConfigCoroutine] Running...");
		while (!controller.Ready)
		{
			// Debug.Log("[MatchController] [GrabMatchConfigCoroutine] Controller is not ready yet!");
			yield return null;
		}
		this.config = controller.InitialConfig;
	}
}

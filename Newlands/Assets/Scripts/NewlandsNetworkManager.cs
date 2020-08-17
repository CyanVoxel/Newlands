// TODO: Change the argument passed to a username

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NewlandsNetworkManager : NetworkManager
{
	[SerializeField]
	private GameObject matchManagerPrefab;
	private GameObject matchManagerReference;
	private MatchController matchController;

	private static DebugTag debugTag = new DebugTag("NewlandsNetworkManager", "8BC34A");
	private int index = 0;

	private Dictionary<string, int> playerAddresses = new Dictionary<string, int>();
	public Dictionary<string, int> PlayerAddresses
	{
		get { return playerAddresses; }
		set { playerAddresses = value; }
	}

	public override void OnServerConnect(NetworkConnection conn)
	{
		Debug.Log(debugTag + "Player trying to connect from: " + conn.address);

		if (!playerAddresses.ContainsKey(conn.address))
		{
			playerAddresses.Add(conn.address, index);
			Debug.Log(debugTag
				+ "Registered Player from address: " + conn.address
				+ ", Index: " + index);
			this.index++;
		}
		else
		{
			Debug.Log(debugTag
				+ "Player from address: " + conn.address
				+ " is already logged at index: " + index);
		}

		foreach (KeyValuePair<string, int> kvp in playerAddresses)
		{
			Debug.Log(debugTag.head + kvp.Key + ", " + kvp.Value);
		}
	}

	public override void OnClientDisconnect(NetworkConnection conn)
	{
		StopClient();
	}

	public override void OnServerSceneChanged(string sceneName)
	{
		if (sceneName == "GameMultiplayer")
		{
			Debug.Log(debugTag + "[Server] Creating Match Manager...");
			CreateMatchManager();
		}

		base.OnServerSceneChanged(sceneName);
	}

	public override void OnClientChangeScene(string newSceneName)
	{
		if (newSceneName == "GameMultiplayer")
		{
			Debug.Log(debugTag + "[Client] Creating Match Manager...");
			CreateMatchManager();
		}
	}

	public void CreateMatchManager()
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

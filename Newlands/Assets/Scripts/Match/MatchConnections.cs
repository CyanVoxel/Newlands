using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MatchConnections : NetworkManager
{
	private static DebugTag debugTag = new DebugTag("MatchConnections", "8BC34A");
	private int index = 0;

	private Dictionary<int, int> playerAddresses = new Dictionary<int, int>();
	public Dictionary<int, int> PlayerAddresses
	{
		get { return playerAddresses; }
		set { playerAddresses = value; }
	}

	public override void OnServerConnect(NetworkConnection conn)
	{
		Debug.Log(debugTag + "Player trying to connect from: " + conn.connectionId);

		if (!playerAddresses.ContainsKey(conn.connectionId))
		{
			playerAddresses.Add(conn.connectionId, index);
			Debug.Log(debugTag
				+ "Registered Player from address: " + conn.connectionId
				+ ", Index: " + index);
			this.index++;
		}
		else
		{
			Debug.Log(debugTag
				+ "Player from address: " + conn.connectionId
				+ " is already logged at index: " + index);
		}

		foreach (KeyValuePair<int, int> kvp in playerAddresses)
		{
			Debug.Log(debugTag.head + kvp.Key + ", " + kvp.Value);
		}
	}
}

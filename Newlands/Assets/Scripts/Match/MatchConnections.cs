// TODO: Change the argument passed to a username

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MatchConnections : NetworkManager
{
	private static DebugTag debugTag = new DebugTag("MatchConnections", "8BC34A");
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
}

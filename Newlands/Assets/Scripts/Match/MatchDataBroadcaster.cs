using Mirror;
using UnityEngine;

public class MatchDataBroadcaster : NetworkBehaviour
{
	private string matchConfigData = "";
	public string MatchConfigData { get { return matchConfigData; } set { matchConfigData = value; } }
}

using Mirror;
using UnityEngine;

public class MatchDataBroadcaster : NetworkBehaviour
{
	[SerializeField]
	[SyncVar]
	private string matchConfigDataStr = "";
	private SyncListString updatedCards;

	public string MatchConfigDataStr
	{
		get
		{
			return matchConfigDataStr;
		}
		set
		{
			if (hasAuthority)
			{
				matchConfigDataStr = value;
			} else {
				Debug.Log("[MatchDataBroadCaster] You don't have the authority to change me!");
			}

		}
	}

	public SyncListString UpdatedCards { get { return updatedCards; } set { updatedCards = value; } }

	// Start is called before the first frame update
	void Start()
	{
		Debug.Log("The MatchDataBroadcaster has been created!");
	}

	void OnDisable()
	{
		Debug.Log("The MatchDataBroadcaster has been disbaled/destroyed!");
	}
}

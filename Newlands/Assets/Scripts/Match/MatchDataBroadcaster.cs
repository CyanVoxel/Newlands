using Mirror;
using UnityEngine;

public class MatchDataBroadcaster : NetworkBehaviour
{
	// FIELDS ##########################################################################################################
	[SyncVar]
	private string matchConfigDataStr = "";
	private SyncListString updatedCardsStr;

	private DebugTag debugTag = new DebugTag("MatchDataBroadcaster", "2196F3");

	// PROPERTIES ######################################################################################################
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
			}
			else
			{
				Debug.Log(debugTag + "You don't have the authority to change MatchConfigDataStr!");
			}

		}
	}

	public SyncListString UpdatedCardsStr
	{
		get
		{
			return updatedCardsStr;
		}
		set
		{
			if (hasAuthority)
			{
				updatedCardsStr = value;
			}
			else
			{
				Debug.Log(debugTag + "You don't have the authority to change UpdatedCardsStr!");
			}

		}
	}

	void Awake()
	{
		Debug.Log(debugTag + "The MatchDataBroadcaster has been created!");
	}

	void Start() { }

	void OnDisable()
	{
		Debug.Log(debugTag + "The MatchDataBroadcaster has been disbaled/destroyed!");
	}
}

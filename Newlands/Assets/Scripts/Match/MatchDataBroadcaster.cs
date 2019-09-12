using Mirror;
using UnityEngine;

public class MatchDataBroadcaster : NetworkBehaviour
{
	// FIELDS ##########################################################################################################
	[SyncVar]
	private string matchConfigDataStr = "";
	[SyncVar]
	private string turnEventBroadcastStr = "";
	[SyncVar]
	private string playerMoneyStr = "";
	private SyncListString updatedCardsStr;
	private SyncListString playerStartingHands;

	private DebugTag debugTag = new DebugTag("MatchDataBroadcaster", "2196F3");

	// PROPERTIES ######################################################################################################
	public string MatchConfigDataStr
	{
		get { return matchConfigDataStr; }
		set
		{
			if (hasAuthority)
				matchConfigDataStr = value;
			else
				Debug.Log(debugTag + "You don't have the authority to change MatchConfigDataStr!");
		}
	}

	public string TurnEventBroadcastStr
	{
		get { return turnEventBroadcastStr; }
		set
		{
			if (hasAuthority)
				turnEventBroadcastStr = value;
			else
				Debug.Log(debugTag + "You don't have the authority to change TurnEventBroadcastStr!");
		}
	}

	public string PlayerMoneyStr
	{
		get { return playerMoneyStr; }
		set
		{
			if (hasAuthority)
				playerMoneyStr = value;
			else
				Debug.Log(debugTag + "You don't have the authority to change PlayerMoneyStr!");
		}
	}

	public SyncListString UpdatedCardsStr
	{
		get { return updatedCardsStr; }
		set
		{
			if (hasAuthority)
				updatedCardsStr = value;
			else
				Debug.Log(debugTag + "You don't have the authority to change UpdatedCardsStr!");
		}
	}

	public SyncListString PlayerStartingHands
	{
		get { return playerStartingHands; }
		set
		{
			if (hasAuthority)
				playerStartingHands = value;
			else
				Debug.Log(debugTag + "You don't have the authority to change PlayerStartingHands!");
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

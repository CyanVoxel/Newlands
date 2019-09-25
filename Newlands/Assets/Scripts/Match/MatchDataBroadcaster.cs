using Mirror;
using UnityEngine;

public class MatchDataBroadcaster : NetworkBehaviour
{
	// FIELDS ######################################################################################
	[SyncVar]
	private string matchConfigStr = "";
	[SyncVar]
	private string matchDataStr = "";
	[SyncVar]
	private string turnEventStr = "";
	[SyncVar]
	private string playerMoneyStr = "";
	[SyncVar]
	private string priceListStr = "";
	// NOTE: In the future, it would be best to use a different implementation.
	[SyncVar]
	private string topCardStr = "";
	private SyncListString updatedCardsStr;
	private SyncListString playerStartingHands;

	private DebugTag debugTag = new DebugTag("MatchDataBroadcaster", "2196F3");

	// PROPERTIES ##################################################################################
	public string MatchConfigStr
	{
		get { return matchConfigStr; }
		set
		{
			if (hasAuthority)
				matchConfigStr = value;
			else
				Debug.Log(debugTag + "You don't have authority to change MatchConfigStr!");
		}
	}

	public string MatchDataStr
	{
		get { return matchDataStr; }
		set
		{
			if (hasAuthority)
				matchDataStr = value;
			else
				Debug.Log(debugTag + "You don't have authority to change MatchDataStr!");
		}
	}

	public string TurnEventStr
	{
		get { return turnEventStr; }
		set
		{
			if (hasAuthority)
				turnEventStr = value;
			else
				Debug.Log(debugTag + "You don't have authority to change TurnEventBroadcastStr!");
		}
	}

	public string PriceListStr
	{
		get { return priceListStr; }
		set
		{
			if (hasAuthority)
				priceListStr = value;
			else
				Debug.Log(debugTag + "You don't have authority to change PriceListStr!");
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
				Debug.Log(debugTag + "You don't have authority to change PlayerMoneyStr!");
		}
	}

	public string TopCardStr
	{
		get { return topCardStr; }
		set
		{
			if (hasAuthority)
				topCardStr = value;
			else
				Debug.Log(debugTag + "You don't have authority to change TopCardStr!");
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
				Debug.Log(debugTag + "You don't have authority to change UpdatedCardsStr!");
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
				Debug.Log(debugTag + "You don't have authority to change PlayerStartingHands!");
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

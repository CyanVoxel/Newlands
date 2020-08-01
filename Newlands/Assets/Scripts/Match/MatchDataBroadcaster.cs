// Acts as a cobbled-together internal API. In the future I would LOVE to rework this
// after knowing the limitations I was up against.

using System.Collections.Generic;
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
	[SyncVar]
	private string usernameListStr = "";
	// NOTE: In the future, it would be best to use a different implementation.
	[SyncVar]
	private string topCardStr = "";
	// private SyncListString updatedCardsStr;
	// private SyncListString playerStartingHands;

	private static DebugTag debugTag = new DebugTag("MatchDataBroadcaster", "2196F3");

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

	public string UsernameListStr
	{
		get { return usernameListStr; }
		set
		{
			if (hasAuthority)
				usernameListStr = value;
			else
				Debug.Log(debugTag + "You don't have authority to change UsernameListStr!");
		}
	}

	// public SyncListString UpdatedCardsStr
	// {
	// 	get { return updatedCardsStr; }
	// 	set
	// 	{
	// 		if (hasAuthority)
	// 			updatedCardsStr = value;
	// 		else
	// 			Debug.Log(debugTag + "You don't have authority to change UpdatedCardsStr!");
	// 	}
	// }

	// public SyncListString PlayerStartingHands
	// {
	// 	get { return playerStartingHands; }
	// 	set
	// 	{
	// 		if (hasAuthority)
	// 			playerStartingHands = value;
	// 		else
	// 			Debug.Log(debugTag + "You don't have authority to change PlayerStartingHands!");
	// 	}
	// }

	void Awake()
	{
		Debug.Log(debugTag + "The MatchDataBroadcaster has been created!");
	}

	void Start() { }

	void OnDisable()
	{
		Debug.Log(debugTag + "The MatchDataBroadcaster has been disabled/destroyed!");
	}

	public void BroadcastWinner(int id)
	{
		MatchData unpacked = JsonUtility.FromJson<MatchData>(matchDataStr);
		unpacked.Winner = id;
		matchDataStr = JsonUtility.ToJson(unpacked);
	}

	public static string PackData<T>(List<T> list)
	{
		string formattedOutput = "";

		for (int i = 0; i < list.Count; i++)
		{
			formattedOutput += list[i].ToString();

			if (list.Count - i > 1)
				formattedOutput += "_";
		}
		Debug.Log(debugTag + "[PackData] Packed new string: " + formattedOutput);

		return formattedOutput;
	}

	// NOTE: Not sure if there's a way to get away with generics here...
	public static List<string> UnpackStringData(string packedData)
	{
		List<string> unpackedData = new List<string>();
		string[] unpackedDataSplit = packedData.Split('_');

		for (int i = 0; i < unpackedDataSplit.Length; i++)
		{
			unpackedData.Add(unpackedDataSplit[i]);
		}

		return unpackedData;
	}

	public static List<int> UnpackIntData(string packedData)
	{
		List<int> unpackedData = new List<int>();
		string[] unpackedDataSplit = packedData.Split('_');

		for (int i = 0; i < unpackedDataSplit.Length; i++)
		{
			unpackedData.Add(int.Parse(unpackedDataSplit[i]));
		}

		return unpackedData;
	}
}

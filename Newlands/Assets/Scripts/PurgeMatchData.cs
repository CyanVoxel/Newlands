using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PurgeMatchData : MonoBehaviour
{
	private GameObject referenceObj;

	void Awake()
	{
		referenceObj = GameObject.Find("MatchManager(Clone)");

		if (referenceObj != null)
		{
			MatchDataBroadcaster mdb = referenceObj.GetComponent<MatchDataBroadcaster>();
			MatchConfig config = JsonUtility.FromJson<MatchConfig>(mdb.MatchConfigStr);

			for (int i = 1; i <= config.MaxPlayerCount; i++)
			{
				string playerStr;
				playerStr = "Player (" + i + ")";
				referenceObj = GameObject.Find(playerStr);
				SafeDestroy(referenceObj);
			}

		}

		referenceObj = GameObject.Find("NetworkManager");

		if (referenceObj != null)
		{
			NetworkManager networkManager = referenceObj.GetComponent<NetworkManager>();

			if (networkManager != null)
			{
				networkManager.StopHost();
				// networkManager.StopClient();
			}

			SafeDestroy(referenceObj);
		}

		referenceObj = GameObject.Find("MatchManager(Clone)");
		SafeDestroy(referenceObj);

	}

	private void SafeDestroy(GameObject obj)
	{
		if (obj != null)
		{
			Debug.Log("Destroying " + obj.name + "...");
			Destroy(obj);
		}
		else
		{
			Debug.LogError("COULD NOT DESTROY AN OBJECT!");
		}
	}
}

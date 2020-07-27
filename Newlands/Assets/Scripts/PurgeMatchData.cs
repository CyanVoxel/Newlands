// Used to shutdown and destroy remnants from a previous match.
// Sits on the main menu, aka the intended place to return from after a match.

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PurgeMatchData : MonoBehaviour
{
	// GameObject empty reference used as a container.
	private GameObject referenceObj;

	void Awake()
	{
		// Look for the MatchManager first ---------------------------------------------------------
		referenceObj = GameObject.Find("MatchManager(Clone)");

		// If found, swipe the config info to know how many Player objects to destroy --------------

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

		// Attempt to stop hosting and destroy the NetworkManager ----------------------------------

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

		// Continue by destroying the MatchManager, as it's not needed anymore ---------------------

		referenceObj = GameObject.Find("MatchManager(Clone)");
		SafeDestroy(referenceObj);
	}

	// Tries to call Destroy() on a GameObject if it is not null.
	// Returns 0 if successful, -1 if not.
	private int SafeDestroy(GameObject obj)
	{
		if (obj != null)
		{
			Debug.Log("Destroying " + obj.name + "...");
			Destroy(obj);
			return 0;
		}
		else
		{
			return -1;
		}
	}
}

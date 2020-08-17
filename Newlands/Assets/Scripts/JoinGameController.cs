using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinGameController : MonoBehaviour
{
	// GameObject networkManagerObj;
	[SerializeField]
	private NetworkManager networkManager;
	[SerializeField]
	private TelepathyTransport telepathyTransport;
	[SerializeField]
	private UsernameInputController usernameInputController;
	[SerializeField]
	private PortInputController portInputController;
	[SerializeField]
	private IpInputController ipInputController;

	private DebugTag debugTag = new DebugTag("JoinGameController", "f44336");

	// Start is called before the first frame update
	// void Start()
	// {
	// 	Debug.Log(debugTag + "Initializing...");
	// }

	public void JoinGameButtonClick()
	{
		if (!NetworkClient.isConnected && !NetworkServer.active)
		{
			if (!NetworkClient.active)
			{
				string retrievedIp = ipInputController.GetIpAddress();
				Debug.Log(debugTag + "Retrieved IP: " + retrievedIp);

				if (!System.String.IsNullOrEmpty(retrievedIp))
				{
					if (ipInputController != null)
						networkManager.networkAddress = retrievedIp;
					else
						Debug.LogError(debugTag.error + "IpInputController is null!");

					if (portInputController != null)
						telepathyTransport.port = portInputController.GetPort();
					else
						Debug.LogError(debugTag.error + "PortInputController is null!");

					networkManager.StartClient();
					// SceneManager.LoadScene("GameMultiplayer", LoadSceneMode.Single);
					SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
					// SceneManager.LoadScene("GameMultiplayer", LoadSceneMode.Additive);
				}
			}
		}

		if (usernameInputController != null)
			PlayerDataContainer.Username = usernameInputController.GetUsername();
		else
			Debug.LogError(debugTag.error + "UsernameInputController is null!");

		Debug.Log(debugTag + "Trying to Join Game with Username: " + PlayerDataContainer.Username + " on Port " + telepathyTransport.port);

		// PlayerDataContainer.Username = UsernameController.GetUsername();

		// FinalizeUsername();
		// CmdRetrieveUsername(username);
	}
}

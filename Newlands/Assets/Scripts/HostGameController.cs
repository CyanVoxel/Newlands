using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostGameController : MonoBehaviour
{
	[SerializeField]
	private NetworkManager networkManager;
	[SerializeField]
	private TelepathyTransport telepathyTransport;
	[SerializeField]
	private UsernameInputController usernameInputController;
	[SerializeField]
	private PortInputController portInputController;

	private DebugTag debugTag = new DebugTag("HostGameController", "f44336");

	// Start is called before the first frame update
	void Start()
	{
		Debug.Log(debugTag + "Initializing...");
	}

	public void HostGameButtonClick()
	{
		// CreateInitialConfig();
		if (!NetworkClient.isConnected && !NetworkServer.active)
		{
			if (!NetworkClient.active)
			{
				if (portInputController != null)
					telepathyTransport.port = portInputController.GetPort();
				else
					Debug.LogError(debugTag.error + "PortInputController is null!");

				networkManager.StartHost();
				SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
				// SceneManager.LoadScene("GameMultiplayer", LoadSceneMode.Additive);
			}
		}
		// CreateMatchManager();

		if (usernameInputController != null)
			PlayerDataContainer.Username = usernameInputController.GetUsername();
		else
			Debug.LogError(debugTag.error + "UsernameInputController is null!");

		Debug.Log(debugTag + "Hosting Game with Username: " + PlayerDataContainer.Username + " on Port " + telepathyTransport.port);
	}
}

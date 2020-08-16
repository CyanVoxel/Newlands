using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonController : MonoBehaviour
{

	public void ChangeSceneHostGame()
	{
		Debug.Log("Switching scene to HostGame");
		SceneManager.LoadScene("HostGame");
	}

	public void ChangeSceneJoinGame()
	{
		Debug.Log("Switching scene to JoinGame");
		SceneManager.LoadScene("JoinGame");
	}

	public void ChangeSceneMainMenu()
	{
		Debug.Log("Switching scene to MainMenu");
		SceneManager.LoadScene("MainMenu");
	}

	public void ChangeSceneMultiplayerGame()
	{
		Debug.Log("Switching scene to GameMultiplayer");
		SceneManager.LoadScene("GameMultiplayer");
	}
}
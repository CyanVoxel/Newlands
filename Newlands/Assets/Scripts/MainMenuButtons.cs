using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
	public void ChangeSceneHostGame()
	{
		Debug.Log("Switching scene to GameSetup");
		SceneManager.LoadScene("GameSetup");
	}

	public void ChangeSceneJoinGame()
	{
		Debug.Log("Switching scene to GameJoin");
		SceneManager.LoadScene("GameJoin");
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
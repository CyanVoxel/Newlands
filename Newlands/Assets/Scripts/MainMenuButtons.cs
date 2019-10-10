using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
	public void ChangeSceneMultiplayerSetup()
	{
		Debug.Log("Switching scene to GameSetup");
		SceneManager.LoadScene("GameSetup");
	}

	public void ChangeSceneMultiplayerGame()
	{
		Debug.Log("Switching scene to GameMultiplayer");
		SceneManager.LoadScene("GameMultiplayer");
	}
}
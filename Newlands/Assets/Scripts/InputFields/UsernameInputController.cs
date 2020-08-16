// Controller for a Username Input Field.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UsernameInputController : MonoBehaviour
{
	// The TMP_InputField for the username.
	[SerializeField]
	private TMP_InputField usernameInputField;
	// The TMP_Text for the username placeholder text.
	[SerializeField]
	private TMP_Text usernamePlaceholder;

	// The internal username string.
	private string username;

	private DebugTag debugTag = new DebugTag("UsernameInputController", "#000000");

	void Start()
	{
		if (usernamePlaceholder != null)
		{
			// TODO: User playerprefs to remember the last used username and load that
			// into the INPUT FIELD, rather than the placeholder.
			usernamePlaceholder.text = GeneratePlaceholerUsername();
		}
		else
		{
			Debug.LogError(debugTag.error + "UsernamePlaceholder was null!");
		}
	}

	public string GetUsername()
	{
		if (usernamePlaceholder != null && usernameInputField != null)
		{
			// TODO: Sanitize this input further.
			if (!System.String.IsNullOrEmpty(usernameInputField.text))
				username = usernameInputField.text;
			else
				username = usernamePlaceholder.text;
		}
		else
		{
			Debug.LogError(debugTag.error + "UsernamePlaceholder or UsernameInputField was null!");
		}

		return username;

		// // Ship that bad boy off to the PlayerDataContainer, and wish it good luck.
		// PlayerDataContainer.Username = username;
	}

	private static string GeneratePlaceholerUsername()
	{
		List<string> prefixes = new List<string>();
		List<string> suffixes = new List<string>();
		string generatedName = "";

		prefixes.Add("Atom");
		prefixes.Add("Graceful");
		prefixes.Add("Angry");
		prefixes.Add("Happy");
		prefixes.Add("Sad");
		prefixes.Add("Smart");
		prefixes.Add("Mighty");
		prefixes.Add("Cool");
		prefixes.Add("Hot");
		prefixes.Add("Sweaty");
		prefixes.Add("Calm");
		prefixes.Add("Cheesy");
		prefixes.Add("Weird");
		prefixes.Add("Spicy");
		prefixes.Add("Sleepy");
		prefixes.Add("Epic");

		suffixes.Add("Gamer");
		suffixes.Add("Thinker");
		suffixes.Add("Sauce");
		suffixes.Add("Player");
		suffixes.Add("Newlandian");
		suffixes.Add("Void");
		suffixes.Add("Turtle");
		suffixes.Add("Cat");
		suffixes.Add("Dog");
		suffixes.Add("Hamster");
		suffixes.Add("Champ");
		suffixes.Add("Peperoni");
		suffixes.Add("Pizza");
		suffixes.Add("Calzone");
		suffixes.Add("Noodle");
		suffixes.Add("Cannoli");

		generatedName += prefixes[Random.Range(0, prefixes.Count)];
		generatedName += suffixes[Random.Range(0, suffixes.Count)];
		generatedName += Random.Range(0, 100);

		return generatedName;
	}
}

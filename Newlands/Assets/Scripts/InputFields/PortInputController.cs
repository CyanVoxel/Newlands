using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PortInputController : MonoBehaviour
{
	// The TMP_InputField for the port.
	[SerializeField]
	private TMP_InputField portInputField;

	// The internal port string.
	private string port;

	private DebugTag debugTag = new DebugTag("PortInputController", "#000000");

	public ushort GetPort()
	{
		ushort parsedPort = 7777;

		if (portInputField != null)
		{
			if (!System.String.IsNullOrEmpty(portInputField.text))
				port = portInputField.text;
		}
		else
		{
			Debug.LogError(debugTag.error + "UsernamePlaceholder was null!");
		}

		try
		{
			parsedPort = ushort.Parse(portInputField.text);
		}
		catch
		{
			Debug.LogError(debugTag.error + "Could not parse Port! Was it bigger than 65535?");
			parsedPort = 7777;
		}

		return parsedPort;
	}
}

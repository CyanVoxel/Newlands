using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IpInputController : MonoBehaviour
{
	// The TMP_InputField for the IP Address.
	[SerializeField]
	private TMP_InputField ipInputField;
	[SerializeField]
	private Image noIpWarning;

	// The internal port string.
	private string ip = "";

	private DebugTag debugTag = new DebugTag("IpController", "#000000");

	public string GetIpAddress()
	{

		if (ipInputField != null)
		{
			if (!System.String.IsNullOrEmpty(ipInputField.text))
				ip = ipInputField.text;

			if (System.String.IsNullOrEmpty(ip) && noIpWarning != null)
				noIpWarning.color = noIpWarning.color = ColorPalette.GetNewlandsColor("Red", 500, false);
			else
				noIpWarning.color = ColorPalette.alpha;
		}
		else
		{
			Debug.LogError(debugTag.error + "IpInputField was null!");
		}

		return ip;
	}
}

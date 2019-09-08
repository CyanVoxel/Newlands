﻿// A class used to store relevent card data for visuals and synchronize it between the server and
// clients. Could be used with GridUnit in the future?

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CardState : NetworkBehaviour
{
	// DATA FIELDS #################################################################################

	[SerializeField]
	private CardDisplay cardDis;

	// USED FOR VISUALS ONLY -----------------------------------------------------------------------
	[SyncVar(hook = "OnObjectNameChange")]
	public string objectName; // The Card Object's Name
	[SyncVar(hook = "OnTitleChange")]
	public string title; // The Card's Title
	[SyncVar(hook = "OnSubtitleChange")]
	public string subtitle; // The Card's Subtitle
	[SyncVar(hook = "OnBodyChange")]
	public string bodyText; // The Card's Body Text

	[SyncVar(hook = "OnFooterTextChange")]
	public string footerText; // The Card's Footer Text
	[SyncVar(hook = "OnFooterValueChange")]
	public int footerValue;
	[SyncVar] // No hook on values which won't change under current game rules
	public bool percFlag;
	[SyncVar]
	public bool moneyFlag;
	[SyncVar]
	public char footerOpr;
	[SyncVar]
	public bool onlyColorCorners;

	[SyncVar(hook = "OnCategoryChange")]
	public string category; // The Card's Category (Used to determine misc visuals)
	[SyncVar]
	public string resource;
	[SyncVar]
	public string target;
	[SyncVar]
	public string footerColor;
	[SyncVar(hook = "OnParentChange")]
	public string parent;

	private bool initialized = false;

	// METHODS #################################################################################

	// Start is called before the first frame update
	void Start()
	{
		TryToGrabComponents();

		OnObjectNameChange("Jerry");
		OnTitleChange("George");
		OnSubtitleChange("Kramer");
		OnBodyChange("Elaine");
		OnFooterTextChange("Newman");
		OnFooterValueChange(this.footerValue);
		OnCategoryChange("Leo");
	} // Start()

	// NOTE: This is a super-bodgy fix for the stacked cards sometimes not calling CardDisplay
	// properly. Already tried different things involving setting fields in hooks, removing
	// seinfeld paramaters, and removing hooks in Start. One fix was able to fix empty transform
	// names, but using that bug I'm able to patch this fix together with Update.
	// When the underlying cause is found, REMOVE THIS UPDATE METHOD!
	void Update()
	{
		if (this.transform.name == "")
		{
			Debug.Log("[CardState] Object's name was null, setting it to the variable!");
			OnObjectNameChange(this.objectName);
			OnObjectNameChange("Jerry");
			OnTitleChange("George");
			OnSubtitleChange("Kramer");
			OnBodyChange("Elaine");
			OnFooterTextChange("Newman");
			OnFooterValueChange(this.footerValue);
			OnCategoryChange("Leo");
		}
	}

	// NOTE: The paramaters in the hook methods are not used, however they are required by Mirror.
	// The if-statement is there so that the display functions do not get run with empty internal
	// values on initialization. Checking for the passed value instead of the current internal state
	// would lead to unnecessary code being run as well as potential errors.
	// By extention, any calls of these methods can use any throwaway string.

	// Fires when the name destined for this object changes (Should only happen once!)
	private void OnObjectNameChange(string newName)
	{
		this.transform.name = this.objectName;

		if (!initialized)
		{
			if (FindObjectOfType<GridManager>() != null)
			{
				this.transform.SetParent(FindObjectOfType<GridManager>().transform);
				this.initialized = true;
			}
			else if (this.transform.name == "DemoTile")
			{
				this.initialized = true;
			}
		}

	} // OnObjectNameChange()

	private void OnFooterTextChange(string newFooterText)
	{
		TryToGrabComponents();

		if (this.footerText != "")
		{
			cardDis.DisplayFooter(this.transform.gameObject);
		}
	} // OnFooterTextChange()

	private void OnFooterValueChange(int newFooterValue)
	{
		TryToGrabComponents();

		this.footerValue = newFooterValue;
		cardDis.DisplayFooter(this.transform.gameObject);

	} // OnFooterValueChange()

	private void OnCategoryChange(string newCategory)
	{

	} // OnCategoryChange()

	private void OnTitleChange(string newTitle)
	{
		TryToGrabComponents();

		if (this.title != "")
		{
			cardDis.DisplayTitle(this.transform.gameObject);
		}
	} // OnTitleChange()

	private void OnSubtitleChange(string newSubtitle)
	{
		TryToGrabComponents();

		if (this.subtitle != "")
		{
			cardDis.DisplaySubtitle(this.transform.gameObject);
		}
	} // OnSubtitleChange()

	private void OnBodyChange(string newBody)
	{
		TryToGrabComponents();

		if (this.bodyText != "")
		{
			cardDis.DisplayBody(this.transform.gameObject);
		}
	} // OnBodyChange()

	private void OnParentChange(string newParent)
	{
		TryToGrabComponents();

		if (newParent != "")
		{
			GameObject newParentObj;
			if (newParentObj = GameObject.Find(newParent))
			{
				// Debug.Log("Setting parent to " + newParent);
				this.transform.SetParent(GameObject.Find(newParent).transform);
				// this.transform.parent = GameObject.Find(newParent).transform;
			}
		}
	} // OnParentChange()

	// Tries to grab necessary components if they haven't been already.
	private bool TryToGrabComponents()
	{
		if (this.cardDis == null)
		{
			this.cardDis = this.GetComponent<CardDisplay>();
		}

		if (this.cardDis == null)
		{
			return false;
		}
		else
		{
			return true;
		}
	} // TryToGrabComponents()
} // class CardState
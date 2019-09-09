// A GridUnit object, used to represent the state and properties
// of an internal game grid.
// TODO: Manage the scope of the data fields after testing.

using System.Collections.Generic;
using UnityEngine;
// using Mirror;

public class GridUnit
{
	// CardDisplay cardDis;

	// DATA FIELDS ################################################################################
	public int ownerId = 0;
	public int x;
	public int y;
	public GameObject tileObj;
	public int stackSize = 0;
	public Card card;
	public List<Card> cardStack = new List<Card>();
	public bool bankrupt = false;

	// public string landType;
	public string resource;
	public int quantity;

	private int baseValue = 0;
	public int valueMod = 0;
	public double totalValue = 0;

	// public int baseValue;
	// public double valueMod;

	public bool stackable = true;
	// public string tileCat;
	// public string tileScope;
	public string category; // Category of unit (ex. Tile, Land Tile, Game Card, etc.)
	public string scope; // Scope of unit (ex. Land_Forest, Coast_Docks, Sabotage, etc.)
	public string subScope; // Scope of unit (ex. Land_Forest, Coast_Docks, Sabotage, etc.)
	public string target = null;

	private static DebugTag debug = new DebugTag("GridUnit", "FFD600");

	// METHODS ####################################################################################

	// Calculates the value of all resources attached to this card, and updates the
	// resourceValue field on this GridUnit object
	public void CalcBaseValue()
	{
		// Calculates the value of the resources built-in to the card
		if (this.category == "Tile")
		{
			this.baseValue = 0;
			int retrievedPrice = 0;
			ResourceInfo.pricesMut.TryGetValue(this.resource, out retrievedPrice);
			this.baseValue = (retrievedPrice * this.quantity); //Could be 0, that's okay

			// Calculates the value of the resources on cards in the stack, if any
			for (int i = 0; i < this.cardStack.Count; i++)
			{
				retrievedPrice = 0;

				if (this.cardStack[i].Subtitle == "Resource")
				{
					ResourceInfo.pricesMut.TryGetValue(this.cardStack[i].Resource, out retrievedPrice);
					this.baseValue += (retrievedPrice * this.cardStack[i].FooterValue);
				}
				else if (this.cardStack[i].Subtitle == "Investment" && !this.cardStack[i].PercFlag)
				{
					this.baseValue += this.cardStack[i].FooterValue;
				}
				else if (this.cardStack[i].Subtitle == "Sabotage" && !this.cardStack[i].PercFlag)
				{
					this.baseValue -= this.cardStack[i].FooterValue;
				} // if
			} // for cardStack size
		}
		else if (this.card.Category == "Market")
		{
			// Base Value will not be used for Market Cards - Reference ResourceInfo instead.
			// ResourceInfo.prices.TryGetValue(this.resource, out this.baseValue);
			// Debug.Log(debug + "Base value for " + this.resource + " was found to be " + this.baseValue);
		}

	} // CalcBaseValue()

	// Calculates the value of the resources on cards in the stack, if any
	public void CalcValueMod()
	{
		if (this.category == "Tile")
		{
			this.valueMod = 0;
			for (int i = 0; i < this.cardStack.Count; i++)
			{
				if (this.cardStack[i].Subtitle == "Investment" && this.cardStack[i].PercFlag)
				{
					this.valueMod += this.cardStack[i].FooterValue;
				}
				else if (this.cardStack[i].Subtitle == "Sabotage" && this.cardStack[i].PercFlag)
				{
					this.valueMod -= this.cardStack[i].FooterValue;
				} // if-else
			} // for cardStack size
		}
		else if (this.category == "Market")
		{
			// NOTE: Currently, this is the same code for Tile. This is here incase it needs to
			// to change at some point.
			this.valueMod = 0;
			// Debug.Log("Stack Size  :" + this.stackSize);
			// Debug.Log("Stack Count: " + this.cardStack.Count);

			for (int i = 0; i < this.cardStack.Count; i++)
			{
				if (this.cardStack[i].Subtitle == "Investment" && this.cardStack[i].PercFlag)
				{
					this.valueMod += this.cardStack[i].FooterValue;
					// Debug.Log("Should be working! " + this.cardStack[i].footerValue);
				}
				else if (this.cardStack[i].Subtitle == "Sabotage" && this.cardStack[i].PercFlag)
				{
					this.valueMod -= this.cardStack[i].FooterValue;
					// Debug.Log("Should be working! -" + this.cardStack[i].footerValue);
					// this.valueMod -= this.cardStack[i].footerValue; // NOTE: Only do locally!
				} // if-else
			} // for cardStack size
		} // if-else category
	} // CalcValueMod()

	// Calculates the total value this card, and updates the
	//	totalValue field on this GridUnit object
	public void CalcTotalValue()
	{
		// Debug.Log(debug + "Category: " + this.category);
		// Reset all value data and recalculate
		if (this.category == "Tile")
		{
			this.totalValue = 0;
			this.CalcBaseValue();
			this.CalcValueMod();

			this.totalValue = (double)this.baseValue
				+ ((double)this.baseValue * ((double)this.valueMod) / 100d);

			// Debug.Log(debug + "[GridUnit] Tile " + this.x + ", " + this.y + " base value:  " +
			// 	this.baseValue);
			// Debug.Log(debug + "[GridUnit] Tile " + this.x + ", " + this.y + " total value: " +
			// 	this.totalValue);

			if (this.totalValue < 0)
			{
				this.bankrupt = true;
				GameManager.BankruptTile(this);
			} // bankrupt check
		}
		else if (this.category == "Market")
		{

			// Reset all value data and recalculate
			this.totalValue = 0;
			this.CalcBaseValue();
			this.CalcValueMod();

			// Debug.Log(debug + "Resource: " + this.resource + " ----------------------");
			// Debug.Log(debug + "Old PriceMut: " + ResourceInfo.pricesMut[this.resource]);
			// // ResourceInfo.prices.TryGetValue(this.resource, out this.baseValue);
			// Debug.Log(debug + "Base Value  : " + ResourceInfo.prices[this.resource]);
			// Debug.Log(debug + "Value Mod   : " + this.valueMod);
			// Debug.Log(debug + "Calculating...");

			this.totalValue = (double)ResourceInfo.prices[this.resource]
				+ ((double)ResourceInfo.prices[this.resource] * ((double)this.valueMod) / 100d);

			// Debug.Log("Total Value: " + this.totalValue);

			ResourceInfo.pricesMut[this.resource] = (int)this.totalValue;
			// cardDis.UpdateFooter(this, ResourceInfo.pricesMut[this.resource]);
			// tileObj.GetComponent<CardState>().footerValue = ResourceInfo.pricesMut[this.resource];

			// Debug.Log(debug + "New PriceMut: " + ResourceInfo.pricesMut[this.resource]);
			// // ResourceInfo.prices.TryGetValue(this.resource, out this.baseValue);
			// Debug.Log(debug + "Base Value  : " + ResourceInfo.prices[this.resource]);
			// Debug.Log(debug + "Value Mod   : " + this.valueMod);
		} // if tile
	} // CalcTotalValue()

	private GameObject FindCard(string type, int x, int y)
	{
		string strX = "x";
		string strY = "y";

		// Determines the number of zeroes to add in the object name
		string xZeroes = "0";
		string yZeroes = "0";
		if (x >= 10)
		{
			xZeroes = "";
		} // if x >= 10
		if (y >= 10)
		{
			yZeroes = "";
		} // if y >= 10

		// Specific type changes
		if (type == "GameCard")
		{
			strX = "p"; // Instead of x, use p for PlayerID
			strY = "i"; // Instead of y, use i for Index
		} // if GameCard

		if (this.tileObj.transform.Find(strX + xZeroes + x + "_" + strY + yZeroes + y + "_" + type))
		{
			// GameObject gameObject = new GameObject();
			GameObject gameObject = this.tileObj.transform.Find(strX + xZeroes + x + "_"
				+ strY + yZeroes + y + "_"
				+ type).gameObject;
			return gameObject;
		}
		else
		{
			// Debug.LogError("[GameManager] Error: Could not find GameObject!");
			return null;
		}
	} // FindCard()

	public void LoadNewCard(CardData card, GameObject tileObj)
	{
		this.card = card;
		this.tileObj = tileObj;

		// this.landType = card.title;
		this.resource = card.Resource;
		this.quantity = card.FooterValue;
		this.category = card.Category; // The Category of this card (Tile, Game Card)
		this.scope = card.Subtitle; // The Scope of this card (Forest, Plains, Quarry)
		this.subScope = card.Title;
		// this.tileScope = card.title;
		// this.tileCat = card.category;
		// this.x = x;
		// this.y = y;

		if (card.Category == "Game Card")
		{
			// this.targetCat = card.TargetCategory;
			this.target = card.Target; // The Scope that this card targets
			this.stackable = !card.DiscardFlag;
		}

		this.CalcBaseValue();
	} // GridUnit constructor

	// CONSTRUCTORS ###############################################################################

	// Constructor that takes in necessary card info and populates the rest
	// public GridUnit(Card card, GameObject tileObj, int x, int y)
	// {
	// 	this.card = card;
	// 	this.tileObj = tileObj;
	// 	cardDis = this.tileObj.AddComponent<CardDisplay>();

	// 	// this.landType = card.title;
	// 	this.resource = card.Resource;
	// 	this.quantity = card.footerValue;
	// 	this.category = card.category; // The Category of this card (Tile, Game Card)
	// 	this.scope = card.Subtitle; // The Scope of this card (Forest, Plains, Quarry)
	// 	this.subScope = card.title;
	// 	// this.tileScope = card.title;
	// 	// this.tileCat = card.category;
	// 	this.x = x;
	// 	this.y = y;

	// 	if (card.category == "Game Card")
	// 	{
	// 		// this.targetCat = card.TargetCategory;
	// 		this.target = card.Target; // The Scope that this card targets
	// 		this.stackable = !card.doesDiscard;
	// 	}

	// 	if (card.category == "Market")
	// 	{
	// 		// this.targetCat = card.TargetCategory;
	// 		this.target = card.Target; // The Scope that this card targets
	// 		this.stackable = !card.doesDiscard;
	// 		// this.category = "Market";
	// 		ResourceInfo.prices.TryGetValue(card.Subtitle, out this.baseValue);
	// 	}

	// 	this.CalcBaseValue();
	// } // GridUnit constructor

	// Constructor that takes in necessary card info and populates the rest.
	// This uses a CardState instead of a Card object.
	public GridUnit(CardData cardData, GameObject tileObj, int x, int y)
	{
		// this.card = Card.CreateInstance<Card>();
		this.card = cardData;

		Debug.Log(debug.head + "Storing CardData for: " + cardData.Title);

		this.card = new Card(cardData);

		// this.card = card;
		this.tileObj = tileObj;
		// cardDis = this.tileObj.AddComponent<CardDisplay>();

		// this.landType = card.title;
		// this.resource = card.Resource;
		// this.quantity = card.footerValue;
		// this.category = card.category; // The Category of this card (Tile, Game Card)
		this.scope = card.Subtitle; // The Scope of this card (Forest, Plains, Quarry)
		this.subScope = card.Title;
		// this.tileScope = card.title;
		// this.tileCat = card.category;
		this.x = x;
		this.y = y;

		if (card.Category == "Game Card")
		{
			// this.targetCat = card.TargetCategory;
			this.target = card.Target; // The Scope that this card targets
			this.stackable = !card.DiscardFlag;
		}

		if (card.Category == "Market")
		{
			// this.targetCat = card.TargetCategory;
			this.target = card.Target; // The Scope that this card targets
			this.stackable = !card.DiscardFlag;
			// this.category = "Market";
			ResourceInfo.prices.TryGetValue(card.Subtitle, out this.baseValue);
		}

		this.CalcBaseValue();
	} // GridUnit constructor

	// Constructor that takes in necessary card info and populates the rest
	public GridUnit(CardData card, int x, int y)
	{
		this.card = card;
		// this.tileObj = tileObj;
		// cardDis = this.tileObj.AddComponent<CardDisplay>();

		// this.landType = card.title;
		this.resource = card.Resource;
		this.quantity = card.FooterValue;
		this.category = card.Category; // The Category of this card (Tile, Game Card)
		this.scope = card.Subtitle; // The Scope of this card (Forest, Plains, Quarry)
		this.subScope = card.Title;
		// this.tileScope = card.title;
		// this.tileCat = card.category;
		this.x = x;
		this.y = y;

		if (card.Category == "Game Card")
		{
			// this.targetCat = card.TargetCategory;
			this.target = card.Target; // The Scope that this card targets
			this.stackable = !card.DiscardFlag;
		}

		if (card.Category == "Market")
		{
			// this.targetCat = card.TargetCategory;
			this.target = card.Target; // The Scope that this card targets
			this.stackable = !card.DiscardFlag;
			// this.category = "Market";
			ResourceInfo.prices.TryGetValue(card.Subtitle, out this.baseValue);
		}

		this.CalcBaseValue();
	} // GridUnit constructor

	// Default no-argument constructor
	// public GridUnit() { }
} // GridUnit class

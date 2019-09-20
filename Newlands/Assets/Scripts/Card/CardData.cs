// A struct used to store any possible card data in a format that's able to be instantiated, used
// internally, or over the network.

using UnityEngine;
using System.Collections.Generic;

public class CardData : Card
{
	// FIELDS ######################################################################################
	// private Card card = new Card();
	private List<CardData> cardStack = new List<CardData>();

	private string objectName = "Default";
	private int ownerId = 0;
	private int baseValue = 0;
	private int valueMod = 0;
	private int totalValue = 0;
	private int stackSize = 0;
	private int x;
	private int y;
	private bool isBankrupt = false;

	private GameObject cardObject;

	// PROPERTIES ##################################################################################
	// public Card Card { get { return card; } set { card = value; } }
	public List<CardData> CardStack { get { return cardStack; } set { cardStack = value; } }
	public string ObjectName { get { return objectName; } set { objectName = value; } }
	public int OwnerId { get { return ownerId; } set { ownerId = value; } }
	public int BaseValue { get { return baseValue; } set { baseValue = value; } }
	public int ValueMod { get { return valueMod; } set { valueMod = value; } }
	public int StackSize { get { return stackSize; } set { stackSize = value; } }
	public int TotalValue { get { return totalValue; } set { totalValue = value; } }
	public int X { get { return x; } set { x = value; } }
	public int Y { get { return y; } set { y = value; } }
	public bool IsBankrupt { get { return isBankrupt; } set { isBankrupt = value; } }

	public GameObject CardObject
	{
		get
		{
			return cardObject;
		}
		set
		{
			cardObject = value;
			value.GetComponent<CardViewController>().Card = this;
		}
	}

	// METHODS #####################################################################################

	// Calculates the value of all resources attached to this card, and updates the
	// resourceValue field on this GridUnit object
	public void CalcBaseValue()
	{
		// Calculates the value of the resources built-in to the card
		if (this.Category == "Tile")
		{
			this.baseValue = 0;
			int retrievedPrice = 0;
			ResourceInfo.pricesMut.TryGetValue(this.Resource, out retrievedPrice);
			this.baseValue = (retrievedPrice * this.FooterValue); //Could be 0, that's okay

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
		else if (this.Category == "Market")
		{
			// Base Value will not be used for Market Cards - Reference ResourceInfo instead.
			// ResourceInfo.prices.TryGetValue(this.resource, out this.baseValue);
			// Debug.Log(debug + "Base value for " + this.resource + " was found to be " + this.baseValue);
		}

	} // CalcBaseValue()

	// Calculates the value of the resources on cards in the stack, if any
	public void CalcValueMod()
	{
		if (this.Category == "Tile")
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
		else if (this.Category == "Market")
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
	// totalValue field on this GridUnit object
	public void CalcTotalValue()
	{
		// Debug.Log(debug + "Category: " + this.category);
		// Reset all value data and recalculate
		if (this.Category == "Tile")
		{
			this.totalValue = 0;
			this.CalcBaseValue();
			this.CalcValueMod();

			this.totalValue = (int)((float)this.baseValue
				+ ((float)this.baseValue * (this.valueMod) / 100f));

			// Debug.Log(debug + "[GridUnit] Tile " + this.x + ", " + this.y + " base value:  " +
			// 	this.baseValue);
			// Debug.Log(debug + "[GridUnit] Tile " + this.x + ", " + this.y + " total value: " +
			// 	this.totalValue);

			if (this.totalValue < 0)
			{
				this.isBankrupt = true;
				// GameManager.BankruptTile(this);
			} // bankrupt check
		}
		else if (this.Category == "Market")
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

			this.totalValue = (int)((float)ResourceInfo.prices[this.Resource]
				+ ((float)ResourceInfo.prices[this.Resource] * ((float)this.valueMod) / 100f));

			// Debug.Log("Total Value: " + this.totalValue);

			ResourceInfo.pricesMut[this.Resource] = (int)this.totalValue;
			// cardDis.UpdateFooter(this, ResourceInfo.pricesMut[this.resource]);
			this.cardObject.GetComponent<CardViewController>().Card.FooterValue = ResourceInfo.pricesMut[this.Resource];

			// Debug.Log(debug + "New PriceMut: " + ResourceInfo.pricesMut[this.resource]);
			// // ResourceInfo.prices.TryGetValue(this.resource, out this.baseValue);
			// Debug.Log(debug + "Base Value  : " + ResourceInfo.prices[this.resource]);
			// Debug.Log(debug + "Value Mod   : " + this.valueMod);
		} // if tile
	}

	// CONSTRUCTORS ################################################################################

	public CardData() : base() { }

	public CardData(Card card) : base(card) { }

	public CardData(Card card, string objectName) : base(card)
	{
		this.objectName = objectName;
	}

}

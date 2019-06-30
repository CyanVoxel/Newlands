// A GridUnit object, used to represent the state and properties
// of an internal game grid.
// TODO: Manage the scope of the data fields after testing.

using System.Collections.Generic;
using UnityEngine;

public class GridUnit {

	CardDisplay cardDis;

	// DATA FIELDS ################################################################################
	public byte ownerId = 0;
	public byte x;
	public byte y;
	public GameObject tile;
	public byte stackSize = 0;
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

	// METHODS ####################################################################################

	// Calculates the value of all resources attached to this card, and updates the
	// resourceValue field on this GridUnit object
	public void CalcBaseValue() {
		// Calculates the value of the resources built-in to the card

		if (this.category == "Tile") {

			this.baseValue = 0;
			int retrievedPrice = 0;
			ResourceInfo.pricesMut.TryGetValue(this.resource, out retrievedPrice);
			this.baseValue = (retrievedPrice * this.quantity); //Could be 0, that's okay

			// Calculates the value of the resources on cards in the stack, if any
			for (int i = 0; i < this.cardStack.Count; i++) {
				retrievedPrice = 0;

				if (this.cardStack[i].subtitle == "Resource") {
					ResourceInfo.pricesMut.TryGetValue(this.cardStack[i].resource, out retrievedPrice);
					this.baseValue += (retrievedPrice * this.cardStack[i].footerValue);
				} else if (this.cardStack[i].subtitle == "Investment" && !this.cardStack[i].percFlag) {
					this.baseValue += this.cardStack[i].footerValue;
				} else if (this.cardStack[i].subtitle == "Sabotage" && !this.cardStack[i].percFlag) {
					this.baseValue -= this.cardStack[i].footerValue;
				} // if

			} // for cardStack size
		} else if (this.card.category == "Market") {
			ResourceInfo.prices.TryGetValue(this.resource, out this.baseValue);
		}

	} // CalcBaseValue()

	// Calculates the value of the resources on cards in the stack, if any
	public void CalcValueMod() {

		if (this.category == "Tile") {

			this.valueMod = 0;
			for (int i = 0; i < this.cardStack.Count; i++) {

				if (this.cardStack[i].subtitle == "Investment" && this.cardStack[i].percFlag) {
					this.valueMod += this.cardStack[i].footerValue;
				} else if (this.cardStack[i].subtitle == "Sabotage" && this.cardStack[i].percFlag) {
					this.valueMod -= this.cardStack[i].footerValue;
				} // if-else

			} // for cardStack size

		} else if (this.category == "Market") {

			// NOTE: Currently, this is the same code for Tile. This is here incase it needs to
			// to change at some point.
			this.valueMod = 0;
			Debug.Log("Stack Size  :" + this.stackSize);
			Debug.Log("Stack Count: " + this.cardStack.Count);

			for (int i = 0; i < this.cardStack.Count; i++) {

				if (this.cardStack[i].subtitle == "Investment" && this.cardStack[i].percFlag) {
					this.valueMod += this.cardStack[i].footerValue;
					Debug.Log("Should be working! " + this.cardStack[i].footerValue);
				} else if (this.cardStack[i].subtitle == "Sabotage" && this.cardStack[i].percFlag) {
					Debug.Log("Should be working! -" + this.cardStack[i].footerValue);
					this.valueMod -= this.cardStack[i].footerValue;
				} // if-else

			} // for cardStack size

		} // if-else category

	} // CalcValueMod()

	// Calculates the total value this card, and updates the
	//	totalValue field on this GridUnit object
	public void CalcTotalValue() {

		// Reset all value data and recalculate
		if (this.category == "Tile") {
			this.totalValue = 0;
			this.CalcBaseValue();
			this.CalcValueMod();

			this.totalValue = (double) this.baseValue
				+ ((double) this.baseValue * ((double) this.valueMod) / 100d);

			// Debug.Log("[GridUnit] Tile " + this.x + ", " + this.y + " base value:  " +
			// 	this.baseValue);
			// Debug.Log("[GridUnit] Tile " + this.x + ", " + this.y + " total value: " +
			// 	this.totalValue);

			if (this.totalValue < 0) {
				this.bankrupt = true;
				GameManager.BankruptTile(this);
			} // bankrupt check

		} else if (this.category == "Market") {

			// Reset all value data and recalculate
			this.totalValue = 0;
			this.CalcBaseValue();
			this.CalcValueMod();

			// Debug.Log("Old PriceMut: " + ResourceInfo.pricesMut[this.resource]);
			// Debug.Log("Base Value  : " + this.baseValue);
			// Debug.Log("Value Mod   : " + this.valueMod);
			// Debug.Log("Calculating...");

			this.totalValue = (double) this.baseValue
				+ ((double) this.baseValue * ((double) this.valueMod) / 100d);

			// Debug.Log("Total Value: " + this.totalValue);

			ResourceInfo.pricesMut[this.resource] = (int) this.totalValue;
			cardDis.UpdateFooter(this, ResourceInfo.pricesMut[this.resource]);

			// Debug.Log("New PriceMut: " + ResourceInfo.pricesMut[this.resource]);
			// Debug.Log("Base Value  : " + this.baseValue);
			// Debug.Log("Value Mod   : " + this.valueMod);

		} // if tile

	} // CalcTotalValue()

	private GameObject FindCard(string type, byte x, byte y) {

		string strX = "x";
		string strY = "y";

		// Determines the number of zeroes to add in the object name
		string xZeroes = "0";
		string yZeroes = "0";
		if (x >= 10) {
			xZeroes = "";
		} // if x >= 10
		if (y >= 10) {
			yZeroes = "";
		} // if y >= 10

		// Specific type changes
		if (type == "GameCard") {
			strX = "p"; // Instead of x, use p for PlayerID
			strY = "i"; // Instead of y, use i for Index
		} // if GameCard

		if (this.tile.transform.Find(strX + xZeroes + x + "_" + strY + yZeroes + y + "_" + type)) {
			// GameObject gameObject = new GameObject();
			GameObject gameObject = this.tile.transform.Find(strX + xZeroes + x + "_"
				+ strY + yZeroes + y + "_"
				+ type).gameObject;
			return gameObject;
		} else {
			// Debug.LogError("[GameManager] Error: Could not find GameObject!");
			return null;
		}

	} // FindCard()

	public void LoadNewCard(Card card, GameObject tileObj) {

		this.card = card;
		this.tile = tileObj;

		// this.landType = card.title;
		this.resource = card.resource;
		this.quantity = card.footerValue;
		this.category = card.category; // The Category of this card (Tile, Game Card)
		this.scope = card.subtitle; // The Scope of this card (Forest, Plains, Quarry)
		this.subScope = card.title;
		// this.tileScope = card.title;
		// this.tileCat = card.category;
		// this.x = x;
		// this.y = y;

		if (card.category == "Game Card") {
			// this.targetCat = card.targetCategory;
			this.target = card.target; // The Scope that this card targets
			this.stackable = !card.doesDiscard;
		}

		this.CalcBaseValue();

	} // GridUnit constructor

	// CONSTRUCTORS ###############################################################################

	// Constructor that takes in necessary card info and populates the rest
	public GridUnit(Card card, GameObject tileObj, byte x, byte y) {

		this.card = card;
		this.tile = tileObj;
		cardDis = this.tile.AddComponent<CardDisplay>();

		// this.landType = card.title;
		this.resource = card.resource;
		this.quantity = card.footerValue;
		this.category = card.category; // The Category of this card (Tile, Game Card)
		this.scope = card.subtitle; // The Scope of this card (Forest, Plains, Quarry)
		this.subScope = card.title;
		// this.tileScope = card.title;
		// this.tileCat = card.category;
		this.x = x;
		this.y = y;

		if (card.category == "Game Card") {
			// this.targetCat = card.targetCategory;
			this.target = card.target; // The Scope that this card targets
			this.stackable = !card.doesDiscard;
		}

		if (card.category == "Market") {
			// this.targetCat = card.targetCategory;
			this.target = card.target; // The Scope that this card targets
			this.stackable = !card.doesDiscard;
			// this.category = "Market";
			ResourceInfo.prices.TryGetValue(card.subtitle, out this.baseValue);
		}

		this.CalcBaseValue();

	} // GridUnit constructor

	// Default no-argument constructor
	// public GridUnit() { }

} // GridUnit class

// A GridUnit object, used to represent the state and properties
// of an internal game grid.
// TODO: Manage the scope of the data fields after testing.
// TODO: Instead of passing an entire Card object, only pass the directory.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUnit {

	// DATA FIELDS ################################################################################
	public byte ownerId = 0;
	public byte x;
	public byte y;
	public GameObject tile;
	public byte stackSize = 0;
	// public Stack<GameObject> cardStack;

	// public string landType;
	public string resource;
	public int quantity;
	public int value;
	// public string tileCat;
	// public string tileScope;
	public string category;		// Category of unit (ex. Tile, Land Tile, Game Card, etc.)
	public string scope;		// Scope of unit (ex. Land_Forest, Coast_Docks, Sabotage, etc.)
	public string subScope;		// Scope of unit (ex. Land_Forest, Coast_Docks, Sabotage, etc.)
	public string target = null;

	// METHODS ####################################################################################

	// Assigns the Tile's value based on its Resource
	public void AssignTileValue(Card tile) {
		int test = 0;
		ResourceInfo.prices.TryGetValue(tile.resource, out test);
		// If the Tile's resource's price can not be found, log an error
		if (!ResourceInfo.prices.TryGetValue(tile.resource, out this.value)) {
			Debug.LogError("<b>[GridUnit]</b> Error: " + 
			"Could not assign the price for \"" + tile.resource + "\"!");
		} // If the Tile's resource's price could not be found

	} // AssignTileValue()

	// Assigns the Tile's value based on a passed int value
	public void AssignTileValue(int newValue) {
		this.value = newValue;
	} // AssignTileValue(int newValue)

	// CONSTRUCTORS ###############################################################################

	// Constructor that takes in necessary card info and populates the rest
	public GridUnit(Card card, GameObject tileObj, byte x, byte y) {

		Stack<GameObject> cardStack = new Stack<GameObject>();

		this.tile = tileObj;
		// this.landType = card.title;
		this.resource = card.resource;
		this.quantity = card.footerValue;
		this.category = card.category;		// The Category of this card (Tile, Game Card)
		this.scope = card.subtitle;			// The Scope of this card (Forest, Plains, Quarry)
		this.subScope = card.title;
		// this.tileScope = card.title;
		// this.tileCat = card.category;
		this.x = x;
		this.y = y;

		if (card.category == "Game Card") {
			// this.targetCat = card.targetCategory;
			this.target = card.target;	// The Scope that this card targets
		}
		

	} // GridUnit constructor

	// Default no-argument constructor
	// public GridUnit() { }

} // GridUnit class
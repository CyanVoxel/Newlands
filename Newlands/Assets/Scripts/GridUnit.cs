// A GridUnit object, used to represent the state and properties
// of an internal game grid.
// TODO: Manage the scope of the data fields after testing

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUnit {

	// DATA FIELDS ################################################################################
	// public Card tile;
	public List<GameObject> cardStack;
	public int quantity;
	public int value;
	public GameObject tile;
	public byte ownerId = 0;

	public CardTitle landType;
	public string resource;

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
	public GridUnit(Card card, GameObject tileObj, float posX, float posY) {

		this.landType = card.title;
		this.resource = card.resource;
		this.quantity = card.footerValue;
		this.tile = tileObj;

	} // GridUnit constructor

	// Default no-argument constructor
	// public GridUnit() { }

} // GridUnit class
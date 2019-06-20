// A GridUnit object, used to represent the state and properties
// of an internal game grid.
// TODO: Protect some fields after testing is complete; organization

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUnit {

	// DATA FIELDS ------------------------------------------------------------
	public Card tile;
	public Deck cardStack;
	private float value;
	public int posX;
	public int posY;

	private CardEnums.Title landType;
	private CardEnums.Resource resource;

	// CONSTRUCTORS -----------------------------------------------------------

	// Constructor that takes in necessary card info and populates the rest
	public GridUnit(Card tile, int posX, int posY) {

		this.landType = tile.title;
		this.resource = tile.resource;
		this.value = tile.footerValue;

	} // GridUnit constructor

} // GridUnit class
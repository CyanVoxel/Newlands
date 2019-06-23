// A GridUnit object, used to represent the state and properties
// of an internal game grid.
// TODO: Manage the scope of the data fields after testing

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUnit {

	// DATA FIELDS ------------------------------------------------------------
	public Card tile;
	public Deck cardStack;
	public float value;
	public float posX;
	public float posY;
	public byte ownerId = 0;

	public CardTitle landType;
	public ResourceType resource;

	// CONSTRUCTORS -----------------------------------------------------------

	// Constructor that takes in necessary card info and populates the rest
	public GridUnit(Card tile, float posX, float posY) {

		this.landType = tile.title;
		this.resource = tile.resource;
		this.value = tile.footerValue;
		this.posX = posX;
		this.posY = posY;

	} // GridUnit constructor

	// Default no-argument constructor
	// public GridUnit() { }

} // GridUnit class
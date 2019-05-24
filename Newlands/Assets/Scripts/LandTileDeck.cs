// A class designed to hold various Land Tiles in a Deck

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandTileDeck : Deck {

	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public LandTileDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public LandTileDeck(CardEnums.Decks flavor) {
		
		// The Standard Deck's Land Tiles
		if (flavor == CardEnums.Decks.VanillaStandard) {
			// Card cardToAdd;

			// TODO: Implement Land Tile objects

			// cardToAdd = Resources.Load<Card>(dirGcMmI + "/add_20_perc");
			// this.Add(cardToAdd);

			// cardToAdd = Resources.Load<Card>(dirGcMmS + "/sub_10_perc");
			// this.Add(cardToAdd);

			// cardToAdd = Resources.Load<Card>(dirGcTmR + "/cashcrops_add_4");
			// this.Add(cardToAdd);
		} // if standard

	} // LandTileDeck(flavor) constructor

} //LandTileDeck class
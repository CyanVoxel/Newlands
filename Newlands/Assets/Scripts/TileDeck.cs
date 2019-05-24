// A class designed to hold and manage "decks" of Land Tile Card objects.
// To be used along with a main Deck.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDeck : Deck {

	// CONSTRUCTORS -----------------------------------------------------------

	// No-arg constructor
	public TileDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public TileDeck(string flavor) {
		
		// The Standard Deck's Land Tiles
		if (flavor == "standard") {
			Card cardToAdd;

			cardToAdd = Resources.Load<Card>(dirGcMmI + "/add_20_perc");
			this.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>(dirGcMmS + "/sub_10_perc");
			this.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>(dirGcTmR + "/cashcrops_add_4");
			this.Add(cardToAdd);
		} // if standard

	} // TileDeck(flavor) constructor

}
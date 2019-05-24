// A class designed to hold and manage "decks" of Land Tile Card objects.
// To be used along with a main Deck.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDeck : Deck {

	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public TileDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public TileDeck(CardEnums.Decks flavor) {
		
		// The Standard Deck's Land Tiles
		if (flavor == CardEnums.Decks.VanillaStandard) {
			Card cardToAdd;

			cardToAdd = Resources.Load<Card>(dirPc + "/lumber");
			this.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>(dirPc + "/cashcrops");
			this.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>(dirPc + "/oil");
			this.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>(dirPc + "/iron");
			this.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>(dirPc + "/silver");
			this.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>(dirPc + "/gold");
			this.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>(dirPc + "/gems");
			this.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>(dirPc + "/platinum");
			this.Add(cardToAdd);
		} // if standard

	} // TileDeck(flavor) constructor

}
// A class designed to hold various Game Cards in a Deck

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCardDeck : Deck {

	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public GameCardDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public GameCardDeck(CardEnums.Decks flavor) {
		
		// The Standard Deck's Land Tiles
		if (flavor == CardEnums.Decks.VanillaStandard) {
			Card cardToAdd;

			cardToAdd = Resources.Load<Card>(dirGcMmI + "/add_20_perc");
			this.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>(dirGcMmS + "/sub_10_perc");
			this.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>(dirGcTmR + "/cashcrops_add_4");
			this.Add(cardToAdd);
		} // if standard

	} // GameCardDeck(flavor) constructor

} //GameCardDeck class
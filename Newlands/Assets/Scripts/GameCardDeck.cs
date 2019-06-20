// A class designed to hold various Game Cards in a Deck

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCardDeck : Deck {

	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public GameCardDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public GameCardDeck(CardEnums.Deck flavor) {
		
		// The Vanilla Standard Deck's Game Cards
		if (flavor == CardEnums.Deck.VanillaStandard) {
			// Card cardToAdd;

			// Market Mods
			this.Add(dirGcMmI + "/add_20_perc", 2);
			this.Add(dirGcMmI + "/add_10_perc", 3);
			this.Add(dirGcMmI + "/sub_20_perc", 2);
			this.Add(dirGcMmI + "/sub_10_perc", 3);

			// Tile Mods
			this.Add(dirGcTmR + "/oil_add_1", 5);
			this.Add(dirGcTmR + "/oil_add_2", 4);
			this.Add(dirGcTmR + "/oil_add_3", 4);
			this.Add(dirGcTmR + "/oil_add_4", 3);
			this.Add(dirGcTmR + "/cashcrops_add_1", 7);
			this.Add(dirGcTmR + "/cashcrops_add_2", 6);
			this.Add(dirGcTmR + "/cashcrops_add_3", 3);
			this.Add(dirGcTmR + "/cashcrops_add_4", 2);
			this.Add(dirGcTmR + "/iron_add_1", 5);
			this.Add(dirGcTmR + "/iron_add_2", 4);
			this.Add(dirGcTmR + "/iron_add_3", 2);
			this.Add(dirGcTmR + "/lumber_add_1", 3);
			this.Add(dirGcTmR + "/lumber_add_2", 3);
			this.Add(dirGcTmR + "/lumber_add_3", 3);
			this.Add(dirGcTmR + "/gems_add_1", 3);
			this.Add(dirGcTmR + "/gems_add_2", 3);
			this.Add(dirGcTmR + "/silver_add_1", 4);
			this.Add(dirGcTmR + "/silver_add_2", 3);
			this.Add(dirGcTmR + "/gold_add_1", 3);
			this.Add(dirGcTmR + "/platinum_add_1", 2);



		} // if Vanilla Standard

	} // GameCardDeck(flavor) constructor

} //GameCardDeck class
// A class designed to hold various Land Tiles in a Deck

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandTileDeck : Deck {

	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public LandTileDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public LandTileDeck(DeckType flavor) {
		
		// The Standard Deck's Land Tiles
		if (flavor == DeckType.VanillaStandard) {
			Card cardToAdd;

			cardToAdd = Resources.Load<Card>(dirLt + "/forest_oil_1");
			this.Add(cardToAdd);
			this.Add(cardToAdd);
			this.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>(dirLt + "/forest_lumber_1");
			for (int i = 0; i < 8; i++) {
				this.Add(cardToAdd);
			}

			cardToAdd = Resources.Load<Card>(dirLt + "/forest_lumber_2");
			for (int i = 0; i < 5; i++) {
				this.Add(cardToAdd);
			}

			cardToAdd = Resources.Load<Card>(dirLt + "/forest_lumber_3");
			this.Add(cardToAdd);
			this.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>(dirLt + "/plains_oil_1");
			for (int i = 0; i < 6; i++) {
				this.Add(cardToAdd);
			}

			cardToAdd = Resources.Load<Card>(dirLt + "/plains_empty");
			for (int i = 0; i < 12; i++) {
				this.Add(cardToAdd);
			}

			cardToAdd = Resources.Load<Card>(dirLt + "/quarry_iron_1");
			for (int i = 0; i < 6; i++) {
				this.Add(cardToAdd);
			}

			cardToAdd = Resources.Load<Card>(dirLt + "/quarry_iron_2");
			for (int i = 0; i < 6; i++) {
				this.Add(cardToAdd);
			}

			cardToAdd = Resources.Load<Card>(dirLt + "/quarry_silver_1");
			for (int i = 0; i < 4; i++) {
				this.Add(cardToAdd);
			}

			cardToAdd = Resources.Load<Card>(dirLt + "/quarry_gold_1");
			this.Add(cardToAdd);
			this.Add(cardToAdd);

		} // if standard

	} // LandTileDeck(flavor) constructor

} //LandTileDeck class
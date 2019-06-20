// A class designed to hold various Price Cards in a Deck

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriceCardDeck : Deck {

	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public PriceCardDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public PriceCardDeck(CardEnums.Deck flavor) {
		
		// The Vanilla Standard Deck's Price Cards
		if (flavor == CardEnums.Deck.VanillaStandard) {
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
		} // if Vanilla Standard deck

	} // PriceCardDeck(flavor) constructor

} //PriceCardDeck class
// A class designed to hold various Coast Tiles in a Deck

public class CoastTileDeck : Deck {

	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public CoastTileDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public CoastTileDeck(string deckType) {

		// The Standard Deck's Coast Tiles
		if (deckType == "Vanilla") {

			// this.Add(dirTC + "/forest_oil_1", 3);
			// this.Add(dirTC + "/forest_lumber_1", 8);
			// this.Add(dirTC + "/forest_lumber_2", 5);
			// this.Add(dirTC + "/forest_lumber_3", 2);
			// this.Add(dirTC + "/plains_oil_1", 6);
			// this.Add(dirTC + "/plains_empty", 12);
			// this.Add(dirTC + "/quarry_iron_1", 6);
			// this.Add(dirTC + "/quarry_iron_2", 6);
			// this.Add(dirTC + "/quarry_silver_1", 4);
			// this.Add(dirTC + "/quarry_gold_1", 2);

		} // if Vanila

	} // CoastTileDeck(deckType) constructor

} //CoastTileDeck class

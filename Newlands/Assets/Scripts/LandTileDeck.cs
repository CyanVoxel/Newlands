// A class designed to hold various Land Tiles in a Deck

public class LandTileDeck : Deck {

	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public LandTileDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public LandTileDeck(string deckType) {

		// The Standard Deck's Land Tiles
		if (deckType == "Vanilla") {

			this.Add(dirTL + "/forest_oil_1", 3);
			this.Add(dirTL + "/forest_lumber_1", 8);
			this.Add(dirTL + "/forest_lumber_2", 5);
			this.Add(dirTL + "/forest_lumber_3", 2);
			this.Add(dirTL + "/plains_oil_1", 6);
			this.Add(dirTL + "/plains_empty", 12);
			this.Add(dirTL + "/quarry_iron_1", 6);
			this.Add(dirTL + "/quarry_iron_2", 6);
			this.Add(dirTL + "/quarry_silver_1", 4);
			this.Add(dirTL + "/quarry_gold_1", 2);

		} // if Vanila

	} // LandTileDeck(deckType) constructor

} //LandTileDeck class
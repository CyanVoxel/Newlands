// A class designed to hold various Market Cards in a Deck

public class MarketCardDeck : Deck {

	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public MarketCardDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public MarketCardDeck(string deckType) {
		
		// The Vanilla Standard Deck's Price Cards
		if (deckType == "Vanilla") {
			
			this.Add(dirMc + "/lumber");
			this.Add(dirMc + "/cashcrops");
			// this.Add(dirMc + "/fish");
			this.Add(dirMc + "/oil");
			this.Add(dirMc + "/iron");
			this.Add(dirMc + "/silver");
			this.Add(dirMc + "/gold");
			this.Add(dirMc + "/gems");
			this.Add(dirMc + "/platinum");

		} // if Vanilla Standard deck

	} // MarketCardDeck(deckType) constructor

} //MarketCardDeck class
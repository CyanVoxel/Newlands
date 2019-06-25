// A class designed to hold various Market Cards in a Deck

public class MarketCardDeck : Deck {

	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public MarketCardDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public MarketCardDeck(string deckType) {
		
		// The Vanilla Standard Deck's Price Cards
		if (deckType == "Vanilla") {
			
			this.Add(dirMc + "/lumber", 1);
			this.Add(dirMc + "/cashcrops", 1);
			this.Add(dirMc + "/oil", 1);
			this.Add(dirMc + "/iron", 1);
			this.Add(dirMc + "/silver", 1);
			this.Add(dirMc + "/gold", 1);
			this.Add(dirMc + "/gems", 1);
			this.Add(dirMc + "/platinum", 1);

		} // if Vanilla Standard deck

	} // MarketCardDeck(deckType) constructor

} //MarketCardDeck class
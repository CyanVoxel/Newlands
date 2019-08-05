// A class designed to hold various Market Cards in a Deck

public class MarketCardDeck : Deck
{
	// CONSTRUCTORS ################################################################################

	// Default no-arg constructor
	public MarketCardDeck()
	{

	} // MarketCardDeck() constructor

	// Constructor that takes in a string representing the name of premade deck
	public MarketCardDeck(string deckType)
	{
		// The Vanilla Standard Deck's Price Cards
		if (deckType == "Vanilla")
		{
			// NOTE: These are organized adding the most expensive first
			this.Add(dirMc + "/platinum");
			this.Add(dirMc + "/gold");
			this.Add(dirMc + "/gems");
			this.Add(dirMc + "/silver");
			this.Add(dirMc + "/iron");
			this.Add(dirMc + "/oil");
			// this.Add(dirMc + "/fish");
			this.Add(dirMc + "/cashcrops");
			this.Add(dirMc + "/lumber");
		} // if Vanilla Standard deck
	} // MarketCardDeck(deckType) constructor
} // class MarketCardDeck

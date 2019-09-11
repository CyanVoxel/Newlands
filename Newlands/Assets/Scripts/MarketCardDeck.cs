// A class designed to hold various Market Cards in a Deck

public class MarketCardDeck : Deck
{
	// CONSTRUCTORS ################################################################################

	// Default no-arg constructor
	public MarketCardDeck()
	{

	}

	// Constructor that takes in a string representing the name of premade deck
	public MarketCardDeck(string flavor = "Vanilla")
	{
		switch (flavor)
		{
			case "Vanilla":
				AddVanillaCards();
				break;
			default:
				break;
		}
	}

	// NOTE: In the future, this will be controlled by the manifest file and
	// there won't be a need for methods specailzed for each deck flavor.
	private void AddVanillaCards()
	{
		// Market Cards ========================================================
		this.Add("Cards/Vanilla/MarketCard/platinum");
		this.Add("Cards/Vanilla/MarketCard/gold");
		this.Add("Cards/Vanilla/MarketCard/gems");
		this.Add("Cards/Vanilla/MarketCard/silver");
		this.Add("Cards/Vanilla/MarketCard/iron");
		// this.Add("Cards/Vanilla/MarketCard/fish");
		this.Add("Cards/Vanilla/MarketCard/cashcrops");
		this.Add("Cards/Vanilla/MarketCard/lumber");
	}

}

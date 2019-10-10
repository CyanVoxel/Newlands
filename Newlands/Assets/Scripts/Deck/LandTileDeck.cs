// A class designed to hold various Land Tiles in a Deck

public class LandTileDeck : Deck
{
	// CONSTRUCTORS ################################################################################

	// Default no-arg constructor
	public LandTileDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public LandTileDeck(string flavor = "Vanilla")
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
		// Land Tiles ==========================================================
		this.Add("Cards/Vanilla/Tile/land_forest_oil_1", 3);
		this.Add("Cards/Vanilla/Tile/land_forest_lumber_1", 8);
		this.Add("Cards/Vanilla/Tile/land_forest_lumber_2", 5);
		this.Add("Cards/Vanilla/Tile/land_forest_lumber_3", 2);
		this.Add("Cards/Vanilla/Tile/land_plains_oil_1", 6);
		this.Add("Cards/Vanilla/Tile/land_plains_empty", 12);
		this.Add("Cards/Vanilla/Tile/land_mountain_iron_1", 6);
		this.Add("Cards/Vanilla/Tile/land_mountain_iron_2", 6);
		this.Add("Cards/Vanilla/Tile/land_mountain_silver_1", 4);
		this.Add("Cards/Vanilla/Tile/land_mountain_gold_1", 2);
	}

}

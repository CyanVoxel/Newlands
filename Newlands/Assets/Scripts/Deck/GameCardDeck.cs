// A class designed to hold various Game Cards in a Deck

using System.Collections.Generic;

public class GameCardDeck : Deck
{
	// CONSTRUCTORS ------------------------------------------------------------

	// Default no-arg constructor
	public GameCardDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public GameCardDeck(string flavor = "Vanilla")
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
		// Dictionary<string, int> manifest = new Dictionary<string, int>();

		// Market Mods =========================================================
		// this.Add("Cards/Vanilla/GameCard/marketmod_investment_20_perc", 8);
		// this.Add("Cards/Vanilla/GameCard/marketmod_investment_10_perc", 12);
		// this.Add("Cards/Vanilla/GameCard/marketmod_sabotage_20_perc", 8);
		// this.Add("Cards/Vanilla/GameCard/marketmod_sabotage_10_perc", 12);

		// Tile Mods ===========================================================
		this.Add("Cards/Vanilla/GameCard/tilemod_oil_1", 5);
		this.Add("Cards/Vanilla/GameCard/tilemod_oil_2", 4);
		this.Add("Cards/Vanilla/GameCard/tilemod_oil_3", 4);
		this.Add("Cards/Vanilla/GameCard/tilemod_oil_4", 3);
		this.Add("Cards/Vanilla/GameCard/tilemod_cashcrops_1", 7);
		this.Add("Cards/Vanilla/GameCard/tilemod_cashcrops_2", 6);
		this.Add("Cards/Vanilla/GameCard/tilemod_cashcrops_3", 3);
		this.Add("Cards/Vanilla/GameCard/tilemod_cashcrops_4", 2);
		// this.Add("Cards/Vanilla/GameCard/fish_1", 5);
		this.Add("Cards/Vanilla/GameCard/tilemod_iron_1", 7);
		this.Add("Cards/Vanilla/GameCard/tilemod_iron_2", 4);
		this.Add("Cards/Vanilla/GameCard/tilemod_iron_3", 2);
		this.Add("Cards/Vanilla/GameCard/tilemod_lumber_1", 3);
		this.Add("Cards/Vanilla/GameCard/tilemod_lumber_2", 3);
		this.Add("Cards/Vanilla/GameCard/tilemod_lumber_3", 3);
		this.Add("Cards/Vanilla/GameCard/tilemod_gems_1", 3);
		this.Add("Cards/Vanilla/GameCard/tilemod_gems_2", 3);
		this.Add("Cards/Vanilla/GameCard/tilemod_silver_1", 4);
		this.Add("Cards/Vanilla/GameCard/tilemod_silver_2", 3);
		this.Add("Cards/Vanilla/GameCard/tilemod_gold_1", 3);
		this.Add("Cards/Vanilla/GameCard/tilemod_platinum_1", 2);

		this.Add("Cards/Vanilla/GameCard/tilemod_investment_25_perc", 4);
		this.Add("Cards/Vanilla/GameCard/tilemod_investment_50_perc", 2);
		this.Add("Cards/Vanilla/GameCard/tilemod_investment_100_money", 8);
		this.Add("Cards/Vanilla/GameCard/tilemod_investment_200_money", 4);

		this.Add("Cards/Vanilla/GameCard/tilemod_sabotage_25_perc", 4);
		this.Add("Cards/Vanilla/GameCard/tilemod_sabotage_50_perc", 2);
		this.Add("Cards/Vanilla/GameCard/tilemod_sabotage_100_money", 8);
		this.Add("Cards/Vanilla/GameCard/tilemod_sabotage_200_money", 4);

		// this.Add("Cards/Vanilla/GameCard/tilemod_upgrade", 10);
		// this.Add("Cards/Vanilla/GameCard/tilemod_foreclosure", 2);
	}

}

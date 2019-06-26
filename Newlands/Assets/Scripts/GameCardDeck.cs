// A class designed to hold various Game Cards in a Deck

public class GameCardDeck : Deck {

	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public GameCardDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public GameCardDeck(string flavor) {
		
		// The Vanilla Standard Deck's Game Cards
		if (flavor == "Vanilla") {
			// Card cardToAdd;

			// Market Mods
			this.Add(dirGcMmI + "/add_20_perc", 4);
			this.Add(dirGcMmI + "/add_10_perc", 6);
			this.Add(dirGcMmS + "/sub_20_perc", 4);
			this.Add(dirGcMmS + "/sub_10_perc", 6);

			// Tile Mods (Resources)
			this.Add(dirGcTmR + "/oil_add_1", 5);
			this.Add(dirGcTmR + "/oil_add_2", 4);
			this.Add(dirGcTmR + "/oil_add_3", 4);
			this.Add(dirGcTmR + "/oil_add_4", 3);
			this.Add(dirGcTmR + "/cashcrops_add_1", 7);
			this.Add(dirGcTmR + "/cashcrops_add_2", 6);
			this.Add(dirGcTmR + "/cashcrops_add_3", 3);
			this.Add(dirGcTmR + "/cashcrops_add_4", 2);
			this.Add(dirGcTmR + "/fish_add_1", 5);
			this.Add(dirGcTmR + "/iron_add_1", 5);
			this.Add(dirGcTmR + "/iron_add_2", 4);
			this.Add(dirGcTmR + "/iron_add_3", 2);
			this.Add(dirGcTmR + "/lumber_add_1", 3);
			this.Add(dirGcTmR + "/lumber_add_2", 3);
			this.Add(dirGcTmR + "/lumber_add_3", 3);
			this.Add(dirGcTmR + "/gems_add_1", 3);
			this.Add(dirGcTmR + "/gems_add_2", 3);
			this.Add(dirGcTmR + "/silver_add_1", 4);
			this.Add(dirGcTmR + "/silver_add_2", 3);
			this.Add(dirGcTmR + "/gold_add_1", 3);
			this.Add(dirGcTmR + "/platinum_add_1", 2);

			// Tile Mods (Inv/Sab)
			this.Add(dirGcTmI+ "/add_25_perc", 4);
			this.Add(dirGcTmI+ "/add_50_perc", 2);
			this.Add(dirGcTmI+ "/add_100_money", 8);
			this.Add(dirGcTmI+ "/add_200_money", 4);
			
			this.Add(dirGcTmS+ "/sub_25_perc", 4);
			this.Add(dirGcTmS+ "/sub_50_perc", 2);
			this.Add(dirGcTmS+ "/sub_100_money", 8);
			this.Add(dirGcTmS+ "/sub_200_money", 4);

			// Tile Mods (Upgrade)
			this.Add(dirGcTmU+ "/farmland", 4);

		} // if Vanilla Standard

	} // GameCardDeck(flavor) constructor

} //GameCardDeck class
// A class that holds various Deck objects, meant to act as a set of all decks of
// an appropriate set.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDeck {

	// DATA FIELDS ------------------------------------------------------------

	public GameCardDeck gameCardDeck;
	public LandTileDeck landTileDeck;
	public PriceCardDeck priceCardDeck;

	// NOTE: Since this class doesn't need to inherit any other data fields or
	// 	methods from Deck, I've duplicated the directory strings here for use with
	// 	custom MasterDeck configurations.
	// Resource Directories -------------------------------
	protected string dirGcMmI = "Cards/GameCards/MarketMods/Investment";
	protected string dirGcMmS = "Cards/GameCards/MarketMods/Sabotage";
	protected string dirGcTmI = "Cards/GameCards/TileMods/Investment";
	protected string dirGcTmS = "Cards/GameCards/TileMods/Sabotage";
	protected string dirGcTmR = "Cards/GameCards/TileMods/Resource";
	protected string dirPc = "Cards/PriceCards";
	protected string dirLt = "Cards/LandTiles";

	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public MasterDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public MasterDeck(DeckType flavor) {
		
		// Fills in the subdecks with their preset cards for the deck flavor
		gameCardDeck = new GameCardDeck(flavor);
		priceCardDeck = new PriceCardDeck(flavor);
		landTileDeck = new LandTileDeck(flavor);

	} // TileDeck(flavor) constructor

} // MasterDeck class
// A class that holds various Deck objects, meant to act as a set of all decks of
// an appropriate set.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDeck {

	// DATA FIELDS ------------------------------------------------------------

	public GameCardDeck gameCardDeck;
	public LandTileDeck landTileDeck;
	public MarketCardDeck marketCardDeck;

	// NOTE: Since this class doesn't need to inherit any other data fields or
	// 	methods from Deck, I've duplicated the directory strings here for use with
	// 	custom MasterDeck configurations.
	// Resource Directories -------------------------------
	protected string dirGcMmI = "Cards/Game Cards/Market Mods/Investment";
	protected string dirGcMmS = "Cards/Game Cards/Market Mods/Sabotage";
	protected string dirGcTmI = "Cards/Game Cards/Tile Mods/Investment";
	protected string dirGcTmS = "Cards/Game Cards/Tile Mods/Sabotage";
	protected string dirGcTmR = "Cards/Game Cards/Tile Mods/Resource";
	protected string dirGcTmO = "Cards/Game Cards/Tile Mods/Other";
	protected string dirMc = "Cards/Market Cards";
	protected string dirTL = "Cards/Tiles/Land";
	protected string dirTC = "Cards/Tiles/Coast";

	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public MasterDeck() { }

	// Constructor that takes in a string representing the name of premade deck
	public MasterDeck(string deckType) {

		// Fills in the subdecks with their preset cards for the deck deckType
		gameCardDeck = new GameCardDeck(deckType);
		marketCardDeck = new MarketCardDeck(deckType);
		landTileDeck = new LandTileDeck(deckType);

	} // TileDeck(deckType) constructor

} // MasterDeck class

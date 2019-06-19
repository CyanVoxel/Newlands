// A class designed to hold and manage "decks" of Card objects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck {

	// DATA FIELDS ------------------------------------------------------------

	private List<Card> deck = new List<Card>();

	// Resource Directories -------------------------------
	protected string dirGcMmI = "Cards/GameCards/MarketMods/Investment";
	protected string dirGcMmS = "Cards/GameCards/MarketMods/Sabotage";
	protected string dirGcTmI = "Cards/GameCards/TileMods/Investment";
	protected string dirGcTmS = "Cards/GameCards/TileMods/Sabotage";
	protected string dirGcTmR = "Cards/GameCards/TileMods/Resource";
	protected string dirPc = "Cards/PriceCards";
	protected string dirLt = "Cards/LandTiles";

	// METHODS ----------------------------------------------------------------

	// Add a card to the deck
	public void Add(Card card) {
		this.deck.Add(card);
	} // Add()

	// Remove a card from the deck
	public void Remove(Card card) {
		this.deck.Remove(card);
	} // Remove()

	// Count the number of cards in the deck
	public int Count() {
		return this.deck.Count;
	} // Count()

	// Determine whether the deck contains a certain card
	public bool Contains(Card card) {
		return (this.deck.Contains(card));
	} // Contains()

	// Get the index of a card
	public int IndexOf(Card card) {
		return this.deck.IndexOf(card);
	} // IndexOf()

	//Indexer for the Deck class
	public Card this[int i] {
		get { return this.deck[i]; }
		set { this.deck[i] = value; }
	} // Indexer


	// CONSTRUCTORS -----------------------------------------------------------

	// No-arg constructor
	public Deck() { } // Deck() constructor
	
} // Deck class

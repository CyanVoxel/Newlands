// A class designed to hold and manage "decks" of Card objects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck {

	// DATA FIELDS ------------------------------------------------------------

	private List<Card> deck = new List<Card>();
	private TileDeck tileDeck;


	// METHODS ----------------------------------------------------------------

	// Add a card to the deck
	public void Add(Card card) {
		this.deck.Add(card);
	} //Add()

	// Remove a card from the deck
	public void Remove(Card card) {
		this.deck.Remove(card);
	} //Remove()

	// Count the number of cards in the deck
	public int Count() {
		return this.deck.Count;
	} //Count()

	// Determine whether the deck contains a certain card
	public bool Contains(Card card) {
		return (this.deck.Contains(card));
	} //Contains()

	// Get the index of a card
	public int IndexOf(Card card) {
		return this.deck.IndexOf(card);
	} //IndexOf()

	//Indexer for the Deck class
	public Card this[int i] {
		get { return this.deck[i]; }
		set { this.deck[i] = value; }
	} //Indexer


	// CONSTRUCTORS -----------------------------------------------------------

	// No-arg constructor
	public Deck() { } // Deck() constructor

	// Constructor that takes in a string representing the name of premade deck
	public Deck(string flavor) {

		// The Standard Deck ------------------------------
		if (flavor == "standard") {
			Card cardToAdd;							// Card variable to hold loaded data
			tileDeck = new TileDeck("standard");	// Creates a premade TileDeck
			
			cardToAdd = Resources.Load<Card>("Cards/MarketMods/Investment/add_20_perc");
			this.deck.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>("Cards/MarketMods/Sabotage/sub_10_perc");
			this.deck.Add(cardToAdd);

			cardToAdd = Resources.Load<Card>("Cards/TileMods/Resource/gems_add_1");
			this.deck.Add(cardToAdd);
		} // if standard

	} // Deck(flavor) constructor
	
} // Deck class

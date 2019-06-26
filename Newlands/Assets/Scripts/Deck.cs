// A class designed to hold and manage "decks" of Card objects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck {

	// DATA FIELDS ------------------------------------------------------------

	private List<Card> deck = new List<Card>();

	// Resource Directories -------------------------------
	protected string dirGcMmI = "Cards/Game Cards/Market Mods/Investment";
	protected string dirGcMmS = "Cards/Game Cards/Market Mods/Sabotage";
	protected string dirGcTmI = "Cards/Game Cards/Tile Mods/Investment";
	protected string dirGcTmS = "Cards/Game Cards/Tile Mods/Sabotage";
	protected string dirGcTmR = "Cards/Game Cards/Tile Mods/Resource";
	protected string dirGcTmU = "Cards/Game Cards/Tile Mods/Upgrade";
	protected string dirMc = "Cards/Market Cards";
	protected string dirTL = "Cards/Tiles/Land";
	protected string dirTC = "Cards/Tiles/Coast";

	// METHODS ----------------------------------------------------------------

	// Add a card to the deck
	public void Add(Card card) {
		this.deck.Add(card);
	} // Add()

	// Add a card to the deck, taking in a directory and an amount
	public void Add(string dir, int amount = 1) {
		//Card cardToAdd = Resources.Load<Card>(dir);
		for (int i = 0; i < amount; i++) {
			try {
				this.deck.Add(Resources.Load<Card>(dir));
			} // try

			catch (UnassignedReferenceException e) {
				Debug.LogError("<b>[Deck]</b> Error: " +
				"Could not add card to deck: " + e);
			} // catch
			
		} // for
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

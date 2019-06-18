// Used to create a grid of Tiles/Cards

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	// DATA FIELDS ------------------------------------------------------------
	private int width = 7;
	private int height = 7;
	public static MasterDeck masterDeck;
	public static MasterDeck masterDeckMutable;
	private static int playerCount = 4; //Temp value, real value will be determined at game setup
	private static int handSize = 5;
	private static Deck[] playerDecks;

	private GameObject card;
	//public GameObject card;		//For easy testing

	// Use this for initialization
	void Start() {

		Deck[] playerDecks = new Deck[playerCount];
		
		PopulateGrid();	//Sets up the tile grid

		// Draws a hand of cards for each player
		for (int i = 0; i < playerCount; i++) {
			playerDecks[i] = DrawHand(handSize);
		}
		


	} // Start()

	private void PopulateGrid() {
		// Populate the Card prefab and create the Master Deck
		card = Resources.Load<GameObject>("Prefabs/LandTile");
		masterDeck = new MasterDeck(CardEnums.Decks.VanillaStandard);
		masterDeckMutable = new MasterDeck(CardEnums.Decks.VanillaStandard);
		//masterDeckMutable = masterDeck;	// Sets mutable deck version to internal one
		
		for (int x = 0; x < width; x++) {

			for (int y = 0; y < height; y++) {

				float xOff = x * 11;
				float yOff = y * 8;

				GameObject cardObj = (GameObject)Instantiate(this.card, new Vector3(xOff, yOff, 50), Quaternion.identity);
				cardObj.name = ("Card_x" + x + "_y" + y + "_z0");
				cardObj.transform.SetParent(this.transform);
				cardObj.transform.rotation = new Quaternion(0, 0, 0, 0);	// 0, 0, 180, 0

			} // y
		} // x

	} //PopulateGrid();

	// Draws random GameCards from the masterDeck and returns a deck of a specified size
	private Deck DrawHand(int handSize) {

		Card card;					// The container variable for drawn cards
		Deck deck = new Deck();		// The deck of drawn cards to return

		for (int i = 0; i < handSize; i++) {

			int cardsLeft = masterDeckMutable.gameCardDeck.Count();
			int cardCount = masterDeck.gameCardDeck.Count();

			// Draws a card from the mutable deck, then removes that card from the deck.
			// If all cards are drawn, draw randomly from the immutable deck.
			if (cardsLeft > 0 ) {
				card = masterDeckMutable.gameCardDeck[Random.Range(0, cardsLeft)];
				GameManager.masterDeckMutable.gameCardDeck.Remove(card);
				Debug.Log("<b>[GameManager]</b> " + 
					cardsLeft + 
					" of " + 
					cardCount + 
					" Master Deck cards left");
			} else {
				card = masterDeck.gameCardDeck[Random.Range(0, cardCount)];
				Debug.LogWarning("<b>[GameManager]</b> Warning: " +
				"All Game Cards were drawn! Now drawing from immutable deck...");
			} // if else

		} // for

		return deck;

	} // DrawHand()

	// Connects the internal 
	private void DisplayHands() {

	} // DispalyHands()
	
} // GameManager class

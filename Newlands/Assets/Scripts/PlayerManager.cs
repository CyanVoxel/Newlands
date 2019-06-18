using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

	// DATA FIELDS ------------------------------------------------------------
	private static int playerCount = 4; //Temp value, real value will be determined at game setup
	private static int handSize = 5;

	// VARIABLES --------------------------------------------------------------
	private Card card;

	void Start() {

		// DrawHand();
		
	} //Start()
	
	//
	// void DrawHand() {

	// 	for (int i = 0; i < handSize; i++) {

	// 		// TEMP: Set the Card prefab to display one of the known Game Cards
	// 		int cardsLeft = GameManager.masterDeckMutable.gameCardDeck.Count();
	// 		int cardCount = GameManager.masterDeck.gameCardDeck.Count();

	// 		// Draws a card from the mutable deck, then removes that card from the deck.
	// 		// If all cards are drawn, draw randomly from the immutable deck.
	// 		if (cardsLeft > 0 ) {
	// 			card = GameManager.masterDeckMutable.gameCardDeck[Random.Range(0, cardsLeft)];
	// 			GameManager.masterDeckMutable.gameCardDeck.Remove(card);
	// 			// Debug.Log("<b>[CardDisplay]</b> " + 
	// 			// 	landTilesLeft + 
	// 			// 	" of " + 
	// 			// 	landTileCount + 
	// 			// 	" Master Deck cards left");
	// 		} else {
	// 			card = GameManager.masterDeck.gameCardDeck[Random.Range(0, cardCount)];
	// 			Debug.LogWarning("<b>[CardDisplay]</b> Warning: " +
	// 			"All Game Cards were drawn! Now drawing from immutable deck...");
	// 		} //if else

	// 	} //for

	// } //DrawHand()

} //PlayerManager class

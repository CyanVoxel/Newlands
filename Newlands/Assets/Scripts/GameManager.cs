// Used to create a grid of Tiles/Cards

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	// VARIABLES ------------------------------------------------------------

	public CardDisplay cardDis;

	private int width = 7;
	private int height = 7;
	public static MasterDeck masterDeck;
	public static MasterDeck masterDeckMutable;
	private static byte playerCount = 4; //Temp value, real value will be determined at game setup
	private static byte handSize = 5;
	private static Deck[] playerDecks;

	private GameObject cardContainer;
	private Card card;
	//public GameObject card;		//For easy testing

	// Use this for initialization
	void Start() {

		Deck[] playerDecks = new Deck[playerCount];
		
		PopulateGrid();	//Sets up the tile grid

		// Draws a hand of cards for each player
		for (byte i = 0; i < playerCount; i++) {
			playerDecks[i] = DrawHand(handSize);
			Debug.Log("Player " + i + " has a deck of size " + playerDecks[i].Count());
		}

		// Displays those hands on screen
		// NOTE: Change 1 to playerCount in order to view all players' cards.
		for (byte i = 0; i < 1; i++) {
			DisplayHand(deck: playerDecks[i], playerNum: i);
		}


	} // Start()

	private void PopulateGrid() {
		// Populate the Card prefab and create the Master Deck
		cardContainer = Resources.Load<GameObject>("Prefabs/LandTile");
		masterDeck = new MasterDeck(CardEnums.Decks.VanillaStandard);
		masterDeckMutable = new MasterDeck(CardEnums.Decks.VanillaStandard);
		//masterDeckMutable = masterDeck;	// Sets mutable deck version to internal one
		
		for (int x = 0; x < width; x++) {

			for (int y = 0; y < height; y++) {

				// Draw a card from the Land Tile deck
				card = DrawCard(masterDeckMutable.landTileDeck, masterDeck.landTileDeck);

				float xOff = x * 11;
				float yOff = y * 8;

				GameObject cardObj = (GameObject)Instantiate(this.cardContainer, new Vector3(xOff, yOff, 50), Quaternion.identity);
				cardObj.name = ("Card_x" + x + "_y" + y + "_z0");

				cardObj.transform.SetParent(this.transform);
				cardObj.transform.rotation = new Quaternion(0, 0, 0, 0);	// 0, 180, 0, 0

				// Connect the drawn card to the prefab that was just created
				cardObj.SendMessage("DisplayCard", card);

			} // y
		} // x

	} //PopulateGrid();

	// Draws random GameCards from the masterDeck and returns a deck of a specified size
	private Deck DrawHand(int handSize) {

		Deck deck = new Deck();		// The deck of drawn cards to return

		for (int i = 0; i < handSize; i++) {

			// Draw a card from the deck provided and add it to the deck to return.
			// NOTE: In the future, masterDeckMutable might need to be checked for cards
			// 	before preceding.
			deck.Add(DrawCard(masterDeckMutable.gameCardDeck, masterDeck.gameCardDeck));

		} // for

		return deck;

	} // DrawHand()

	// Creates card game objects, places them on the screen, and populates them with deck data
	private void DisplayHand(Deck deck, byte playerNum) {

		// // Creates hand GameObjects to contain the card prefabs for each player
		// for (byte i = 0; i < playerNum; i++) {

		// 	GameObject hand = new GameObject();			// Creates new empty GameObject
		// 	hand.name = ("Player" + i);					// Add index to name
		// 	hand.transform.SetParent(this.transform);	// Sets the parent

		// } // for each player

		// Populate the Card prefab
		cardContainer = Resources.Load<GameObject>("Prefabs/LandTile");

		// Creates card prefabs and places them on the screen
		for (int i = 0; i < deck.Count(); i++) {

			float xOff = i * 11 + 11;
			float yOff = -10;

			GameObject cardObj = (GameObject)Instantiate(this.cardContainer, new Vector3(xOff, yOff, 50), Quaternion.identity);
			cardObj.name = ("Card_p" + playerNum + "_i"+ i);

			cardObj.transform.SetParent(this.transform);
			cardObj.transform.rotation = new Quaternion(0, 0, 0, 0);	// 0, 180, 0, 0

			cardObj.SendMessage("DisplayCard", deck[i]);

		} // for

	} // DisplayHand()

	public static Card DrawCard(Deck deckMut, Deck deckPerm) {

		Card card;	// Card to return
		int cardsLeft = deckMut.Count();	// Number of cards left from mutable deck
		int cardsTotal = deckPerm.Count();	// Number of cards total from permanent deck

		// Draws a card from the mutable deck, then removes that card from the deck.
		// If all cards are drawn, draw randomly from the immutable deck.
		if (cardsLeft > 0 ) {
			card = deckMut[Random.Range(0, cardsLeft)];
			deckMut.Remove(card);
			// Debug.Log("<b>[GameManager]</b> " + 
			// 	cardsLeft + 
			// 	" of " + 
			// 	cardsTotal + 
			// 	" cards left");
		} else {
			card = deckPerm[Random.Range(0, cardsTotal)];
			Debug.LogWarning("<b>[GameManager]</b> Warning: " +
			 "All cards (" + cardsTotal + ") were drawn from a deck! " +
			 " Now drawing from immutable deck...");
		}

		return card;

	} // DrawCard()
	
} // GameManager class

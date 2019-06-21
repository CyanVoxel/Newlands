// Used to create a grid of Tiles/Cards

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {

	// VARIABLES --------------------------------------------------------------

	public static MasterDeck masterDeck;
	public static MasterDeck masterDeckMutable;
	public static byte players = 4; 	// Number of players in the match
	public static byte phase = 1;		// The current phase of the game
	public static int round = 1;		// The current round of turns
	public static byte turn = 1;		// The current turn in the round, == to player index

	public static byte width = 7;		// Width of the game grid in cards
	public static byte height = 7;		// Height of the game grid in cards
	private static byte handSize = 5;	// How many cards the player is dealt
	private static Deck[] playerDecks;	// The players' hands stored in decks

	public static GridUnit[,] grid;
	private GameObject cardContainer;
	// private Card card;

	// UI ELEMENTS ------------------------------------------------------------

	private static GameObject roundNumberObj;
	private static GameObject turnNumberObj;
	public static TMP_Text roundNumberText;
	public static TMP_Text turnNumberText;
	


	// Use this for initialization
	void Start() {

		// Grab the UI elements
		roundNumberObj = transform.Find("UI/RoundNumber").gameObject;
		turnNumberObj = transform.Find("UI/TurnNumber").gameObject;
		// Pick out the appropriate elements from the GameObjects that were grabbed
		roundNumberText = roundNumberObj.GetComponent<TMP_Text>();
		turnNumberText = turnNumberObj.GetComponent<TMP_Text>();

		Deck[] playerDecks = new Deck[players];
		
		// Initialize the internal grid
		grid = new GridUnit[width, height];

		// Create tile GameObjects and connect them to internal grid
		PopulateGrid();	

		// Draws a hand of cards for each player
		for (byte i = 0; i < players; i++) {
			playerDecks[i] = DrawHand(handSize);
			//Debug.Log("Player " + i + " has a deck of size " + playerDecks[i].Count());
		}

		// Displays those hands on screen
		// NOTE: Change 1 to playerCount in order to view all players' cards.
		for (byte i = 0; i < 1; i++) {
			DisplayHand(deck: playerDecks[i], playerNum: i);
		}

		// UI DISPLAY ---------------------------------------------------------
		UpdateUI();

	} // Start()

	private void PopulateGrid() {
		// Populate the Card prefab and create the Master Deck
		cardContainer = Resources.Load<GameObject>("Prefabs/LandTile");
		masterDeck = new MasterDeck(CardEnums.Deck.VanillaStandard);
		masterDeckMutable = new MasterDeck(CardEnums.Deck.VanillaStandard);
		//masterDeckMutable = masterDeck;	// Sets mutable deck version to internal one
		
		for (int x = 0; x < width; x++) {

			for (int y = 0; y < height; y++) {

				// Draw a card from the Land Tile deck
				Card card = new Card();
				card = DrawCard(masterDeckMutable.landTileDeck, masterDeck.landTileDeck);

				float xOff = x * 11;
				float yOff = y * 8;

				GameObject cardObj = (GameObject)Instantiate(this.cardContainer, new Vector3(xOff, yOff, 50), Quaternion.identity);
				cardObj.name = ("LandTile_x" + x + "_y" + y + "_z0");

				cardObj.transform.SetParent(this.transform);
				cardObj.transform.rotation = new Quaternion(0, 180, 0, 0);	// 0, 180, 0, 0

				// Connect thr drawn card to the internal grid
				grid[x,y] = new GridUnit(tile: card, posX: xOff, posY: yOff);

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
		cardContainer = Resources.Load<GameObject>("Prefabs/GameCard");

		// Creates card prefabs and places them on the screen
		for (int i = 0; i < deck.Count(); i++) {

			float xOff = i * 11 + (((width - handSize) / 2f) * 11);
			float yOff = -10;

			GameObject cardObj = (GameObject)Instantiate(this.cardContainer, new Vector3(xOff, yOff, 40), Quaternion.identity);
			cardObj.name = ("GameCard_p" + playerNum + "_i"+ i);

			cardObj.transform.SetParent(this.transform);
			cardObj.transform.rotation = new Quaternion(0, 0, 0, 0);

			try {
				cardObj.SendMessage("DisplayCard", deck[i]);
			}

			catch (UnassignedReferenceException e) {
				Debug.LogError("<b>[GameManager]</b> Error: " +
				"Card error at deck index " + i + ": " + e);
			}
			

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

	// Advance to the next turn
	public static void AdvanceTurn() {

		turn++;
		if (turn > players) {
			turn = 1;
			round++;
		}

	} //AdvanceTurn()

	// Rollback to the previous turn (for debugging only)
	public static void RollbackTurn() {

		turn--;
		if (turn == 0) {

			// If the round is 1 or more, decrement it
			if (round > 0) {
				round--;
			} // if round > 0
			
			turn = players;

		} // if turn == 0

	} //RollbackTurn()

	// End the current round, starts at next turn 0
	public static void EndRound() {

		round++;
		turn = 0;

	} //EndRound()

	// Attempts to buy an unowned tile. Returns true if successful, or false if already owned.
	public static bool BuyTile(byte playerID, byte gridX, byte gridY) {

		// TODO: When money is implemented, factor that into the buying process.
		//	It would also be nice to have a purchase conformation message

		// If the tile is unowned (Had owner ID of 0), assign this owner to it
		if (grid[gridX, gridY].ownerID == 0) {
			grid[gridX, gridY].ownerID = playerID;
			return true;
		} else {	// 
			Debug.Log("<b>[GameManager]</b> " +
						"Tile is already owned!");
			return false;
		}

	} // BuyTile()

	// Updates the UI elements 
	public static void UpdateUI() {

		GameManager.roundNumberText.text = ("Round " + GameManager.round);
		GameManager.turnNumberText.text = ("Player " + GameManager.turn + "'s Turn");

		switch (turn) {
			case 1:
				GameManager.turnNumberText.color = ColorPalette.inkCyan;
				break;
			case 2:
				GameManager.turnNumberText.color = ColorPalette.inkRed;
				break;
			case 3:
				GameManager.turnNumberText.color = ColorPalette.purple500;
				break;
			case 4:
				GameManager.turnNumberText.color = ColorPalette.amber500;
				break;
			default:
				break;
		}

	} // UpdateUI()
	
} // GameManager class

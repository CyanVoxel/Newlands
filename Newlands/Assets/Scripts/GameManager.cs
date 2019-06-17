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

	private GameObject card;
	//public GameObject card;		//For easy testing

	// Use this for initialization
	void Start() {
		
		PopulateGrid();


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
	
} // GameManager class

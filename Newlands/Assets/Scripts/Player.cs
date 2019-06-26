// A class to represent and hold player data
// TODO: Adjust scope as necessary

using UnityEngine;
using TMPro;

public class Player {

	// DATA FIELDS ################################################################################

	public byte Id { get; set; } = 0;
	public Deck hand;
	public GridUnit[] handUnits;
	private double baseMoney = 0;
	private double tileMoney = 0;
	private double totalMoney = 0;
	private bool initialized = false;

	// UI ELEMENTS ------------------------------------------------------------

	public GameObject moneyObj;
	public TMP_Text moneyText;

	// METHODS ####################################################################################

	public double Money { get { return totalMoney; } }

	public void CalcMoney() {
		tileMoney = 0;	// Reset tile money before recalculating

		// Checks to see if the object has been initialized. If not, assume it will be afterwards.
		if (this.initialized) {
			// Search the grid for owned tiles
			// TODO: Implement a list of known owned tile coordinates to replace these for loops
			for (byte x = 0; x < GameManager.width; x++) {
				for (byte y = 0; y < GameManager.height; y++) {
					if (GameManager.grid[x, y].ownerId == this.Id) {
						this.tileMoney += (GameManager.grid[x, y].value * GameManager.grid[x, y].quantity);
					} // if player owns tile
				} // for y
			} // for x

		} else {
			this.initialized = true;
		}// if initialized
		
		totalMoney = baseMoney + tileMoney;

	 } // CalcMoney()

	// Takes in a double and adds it to the baseMoney, then recalculates the totalMoney 
	 public void ModifyMoney(double amount) {
		 this.baseMoney += amount;
		 this.CalcMoney();
	 } // ModifyMoney

	// CONSTRUCTORS ###############################################################################

	public Player() {
		this.CalcMoney();
		handUnits = new GridUnit[GameManager.handSize];
		// this.hand = new Deck();
		// moneyObj = new GameObject();
	} // Player() constructor

} // Player class
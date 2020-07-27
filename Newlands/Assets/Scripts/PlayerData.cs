// A class to represent and hold player data
// TODO: Adjust scope as necessary

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerData
{
	// public GridManager gridMan;

	// DATA FIELDS ################################################################################

	public int Id { get; set; } = 0;
	public Deck hand;
	private string username = "";
	// public GridUnit[] handUnits;
	public List<Coordinate2> ownedTiles = new List<Coordinate2>();

	// private double tileMoney = 0;
	// private double baseMoney = 0;
	private double totalMoney = 0;

	private bool initialized = false;
	public bool shouldSkip = false;

	// UI ELEMENTS ------------------------------------------------------------

	public GameObject moneyObj;
	public TMP_Text moneyText;

	// METHODS ####################################################################################

	public double Money { get { return totalMoney; } }
	public string Username { get { return username; } set { username = value; } }

	public void ResetMoney()
	{
		totalMoney = 0;
	}

	public void AddMoney(int money)
	{
		totalMoney += money;
	}

	// void Start() {
	// 	gridMan = FindObjectOfType<GridManager>();
	// }

	// public void CalcTotalMoney()
	// {
	// 	this.totalMoney = 0;
	// 	this.tileMoney = 0; // Reset tile money before recalculating

	// 	// // Checks to see if the object has been initialized. If not, assume it will be afterwards.
	// 	// if (this.initialized)
	// 	// {
	// 	// 	for (int i = 0; i < ownedTiles.Count; i++)
	// 	// 	{

	// 	// 	}
	// 	// 	// Search the grid for owned tiles
	// 	// 	// TODO: Implement a list of known owned tile coordinates to replace these for loops
	// 	// 	// for (int x = 0; x < gridWidth; x++)
	// 	// 	// {
	// 	// 	// 	for (int y = 0; y < gridHeight; y++)
	// 	// 	// 	{
	// 	// 	// 		if (GridManager.grid[x, y].ownerId == this.Id)
	// 	// 	// 		{
	// 	// 	// 			GridManager.grid[x, y].CalcTotalValue();
	// 	// 	// 			this.tileMoney += (GridManager.grid[x, y].totalValue);
	// 	// 	// 		} // if player owns tile
	// 	// 	// 	} // for y
	// 	// 	// } // for x
	// 	// }
	// 	// else
	// 	// {
	// 	// 	this.initialized = true;
	// 	// } // if initialized

	// 	this.totalMoney = this.baseMoney + this.tileMoney;

	// } // CalcTotalMoney()

	// CONSTRUCTORS ###############################################################################

	public PlayerData()
	{
		// this.CalcTotalMoney();
		// this.handUnits = new GridUnit[GameManager.handSize];
	} // Player() constructor
} // class Player

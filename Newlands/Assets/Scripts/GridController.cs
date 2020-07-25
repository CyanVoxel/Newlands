// Used by both the Client and the Server to keep track of known Card/Tile data.

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GridController : NetworkBehaviour
{
	// private GameObject matchManager;
	private MatchDataBroadcaster matchDataBroadcaster;
	private MatchController matchController;

	private CardData[, ] masterGrid;
	private CardData[, ] knownOwnersGrid;
	private CardData[, ] masterMarketGrid;
	private CardData[, ] knownMarketOwnersGrid;

	// private MatchData matchData;
	private TurnEvent lastKnownTurnEvent;
	private MatchConfig config;

	public readonly float cardThickness = 0.2f;
	public readonly float shiftUnit = 1.2f;
	private readonly float cardOffX = 11f;
	private readonly float cardOffY = 8f;
	private static float[] rowPosition;
	private static int[] maxGridStack;
	private static int[] maxMarketStack;

	public CardData[, ] KnownOwnersGrid { get { return knownOwnersGrid; } set { knownOwnersGrid = value; } }
	// public CardData[, ] KnownMarketOwnersGrid { get { return knownMarketOwnersGrid; } set { knownMarketOwnersGrid = value; } }

	private DebugTag debugTag = new DebugTag("GridController", "00BCD4");

	void Awake()
	{
		Debug.Log(debugTag + "The GridController has been created!");
		// StartCoroutine(CheckForBroadcastUpdates());

		StartCoroutine(CreateMainGridCoroutine());
		StartCoroutine(CreateMarketGridCoroutine());
		// StartCoroutine(CreatePlayerHandCoroutine());

	}

	// void Start()
	// {
	// 	if (!hasAuthority)
	// 		return;
	// }

	// void Update()
	// {
	// 	if (!hasAuthority)
	// 		return;

	// 	// StartCoroutine(CheckForBroadcastUpdates());
	// }

	void OnDisable()
	{
		Debug.Log(debugTag + "The GridController has been disbaled/destroyed!");
	}

	// [Client/Server] Parses the Match Data from MatchDataBroadcaster
	public void ParseTurnEvent()
	{
		Debug.Log(debugTag + "Parsing Turn Event...");
		TurnEvent newTurnEvent = JsonUtility.FromJson<TurnEvent>(matchDataBroadcaster.TurnEventStr);
		if (this.lastKnownTurnEvent != newTurnEvent)
		{
			this.lastKnownTurnEvent = newTurnEvent;
		}
		Debug.Log(debugTag + "Turn Event as: " + this.lastKnownTurnEvent);
	}

	public void ParseUpdatedCards() { }

	// [Client with Server-Only section]
	public void CreateGameGridObjects()
	{
		GameObject gridParent = new GameObject("MainGrid");

		for (int x = 0; x < config.GameGridWidth; x++)
		{
			for (int y = 0; y < config.GameGridHeight; y++)
			{
				float xOff = x * cardOffX;
				float yOff = y * cardOffY;

				GameObject cardObj = Instantiate(matchController.landTilePrefab,
					new Vector3(xOff, yOff, 50),
					Quaternion.identity);

				// cardObj.GetComponent<Animator>().Play("Flip");
				cardObj.name = (CardUtility.CreateCardObjectName("Tile", x, y));
				cardObj.transform.SetParent(gridParent.transform);

				cardObj.transform.rotation = new Quaternion(0, 180, 0, 0); // 0, 180, 0, 0

				knownOwnersGrid[x, y] = new CardData();

				if (hasAuthority)
					masterGrid[x, y].CardObject = cardObj;
			}
		}
	}

	// [Client/Server]
	public void CreateMarketGridObjects()
	{
		InitMarketGrid();

		GameObject marketGridParent = new GameObject("MarketGrid");

		for (int x = 0; x < Mathf.CeilToInt(((float)ResourceInfo.resources.Count - 1) / (float)config.GameGridHeight); x++)
		{
			for (int y = 0; y < config.GameGridHeight; y++)
			{
				if (masterMarketGrid[x, y] != null)
				{
					float xOff = ((config.GameGridWidth + 1) * cardOffX) + x * cardOffX;
					float yOff = y * cardOffY;

					GameObject cardObj = (GameObject)Instantiate(matchController.marketCardPrefab,
						new Vector3(xOff, yOff, 50),
						Quaternion.identity);

					cardObj.name = (CardUtility.CreateCardObjectName("Market", x, y));
					cardObj.transform.SetParent(marketGridParent.transform);

					masterMarketGrid[x, y].CardObject = cardObj; // Physical representation
				}
			}
		}
	}

	// [Client/Server]
	public void CreatePlayerHandObjects(int playerNum)
	{
		GameObject playerHandParent = new GameObject("PlayerHand");

		for (int i = 0; i < config.PlayerHandSize; i++)
		{
			float xOff = i * 11 + (((config.GameGridWidth - config.PlayerHandSize) / 2f) * 11);
			float yOff = -10;

			GameObject cardObj = (GameObject)Instantiate(matchController.gameCardPrefab,
				new Vector3(xOff, yOff, 40),
				Quaternion.identity);

			cardObj.name = (CardUtility.CreateCardObjectName("GameCard", playerNum, i));
			cardObj.transform.SetParent(playerHandParent.transform);
		}
	}

	// [Client/Server] Called by CreateMarketGridObjects(). There is no need to hide Market Grid
	// data, so it's done on both the server and the client.
	public void InitMarketGrid()
	{
		masterMarketGrid = new CardData[(int)Mathf.Ceil((float)ResourceInfo.resources.Count - 1
			/ (float)config.GameGridHeight), config.GameGridHeight];
		maxMarketStack = new int[config.GameGridHeight];

		knownMarketOwnersGrid = new CardData[(int)Mathf.Ceil((float)ResourceInfo.resources.Count - 1
			/ (float)config.GameGridHeight), config.GameGridHeight];
		maxMarketStack = new int[config.GameGridHeight];

		int marketWidth = Mathf.CeilToInt(((float)ResourceInfo.resources.Count - 1)
			/ (float)config.GameGridHeight);
		// Debug.Log(debugTag + "x: " + marketWidth
		// 	+ ", y: " + config.GameGridHeight
		// 	+ ", res: " + ResourceInfo.resources.Count);

		for (int x = 0; x < marketWidth; x++)
		{
			for (int y = 0; y < config.GameGridHeight; y++)
			{
				// Draw a card from the Market Card deck
				Card card;
				if (matchController.DrawCard(matchController.MasterDeckMutable.marketCardDeck,
						matchController.MasterDeck.marketCardDeck,
						out card, false))
				{
					// Connect the drawn card to the internal grid
					masterMarketGrid[x, y] = new CardData(card); // Physical cards
					knownMarketOwnersGrid[x, y] = new CardData(card); // Client Logical representation
					// Debug.Log(debugTag + "Created Market Card: " + card.Resource);
				}
			}
		}
	}

	public void SetTileOwner(int x, int y, int ownerId)
	{
		masterGrid[x, y].OwnerId = ownerId;
		knownOwnersGrid[x, y].OwnerId = ownerId;
	}

	public bool IsTileOwned(int x, int y)
	{
		if (knownOwnersGrid[x, y].OwnerId == 0)
			return false;
		else
			return true;
	}

	public bool IsTileBankrupt(int x, int y)
	{
		if (!knownOwnersGrid[x, y].IsBankrupt)
			return false;
		else
			return true;
	}

	public CardData GetServerTile(string type, int x, int y)
	{
		switch (type)
		{
			case "Market":
				if (masterMarketGrid[x, y] != null)
					return masterMarketGrid[x, y];
				else
					return null;
			case "Tile":
				if (masterGrid[x, y] != null)
					return masterGrid[x, y];
				else
					return null;
			default:
				return null;
		}

	}

	public CardData GetClientTile(string type, int x, int y)
	{
		if (type == "Tile" && knownOwnersGrid[x, y] != null)
			return knownOwnersGrid[x, y];
		else if (type == "Market" && knownMarketOwnersGrid[x, y] != null)
			return knownMarketOwnersGrid[x, y];
		else
			return null;
	}

	public void BankruptTile(int x, int y)
	{
		Debug.Log(debugTag + "Bankrupting tile!");
		knownOwnersGrid[x, y].IsBankrupt = true;
		masterGrid[x, y].IsBankrupt = true;
		// CardDisplay.BankruptVisuals(tile.tileObj);
	}

	public void IncrementStackSize(int y, string gridType)
	{
		// Debug.Log(debugTag + "Incrementing card of type: " + gridType);
		switch (gridType)
		{
			case "Market":
				maxMarketStack[y]++;
				// Debug.Log(debugTag + "[AddCardToStack] MARKET STACK INCREMENTED TO " + maxMarketStack[y]);
				break;
			case "Tile":
				maxGridStack[y]++;
				// Debug.Log(debugTag + "[AddCardToStack] TILE STACK INCREMENTED TO " + maxGridStack[y]);
				break;
			default:
				break;
		}
	}

	public void AddCardToStack(int x, int y, string gridType, Card card)
	{
		switch (gridType)
		{
			case "Market":
				knownMarketOwnersGrid[x, y].CardStack.Add(card);
				if (hasAuthority)
				{
					// Debug.Log(debugTag + "[AddCardToStack] MARKET CARD ADDED TO STACK");
					masterMarketGrid[x, y].CardStack.Add(card);
				}
				break;
			default:
				knownOwnersGrid[x, y].CardStack.Add(card);
				if (hasAuthority)
				{
					// Debug.Log(debugTag + "[AddCardToStack] GAMECARD ADDED TO STACK");
					masterGrid[x, y].CardStack.Add(card);
					Debug.Log("[AddCardToStack] REAL Card Stack Size: " + masterGrid[x, y].CardStack.Count);
				}
				break;
		}
	}

	// Shifts a grid if needed based on the target. Returns true if a row was shifted.
	public bool ShiftRowCheck(string type, int x, int y)
	{
		bool didShift = false;
		int maxStack = 0;
		CardData target = GetClientTile(type, x, y);

		// Debug.Log(debugTag + "[Shift Row Check] Target Category: " + type);

		switch (type)
		{
			case "Tile":
				maxStack = maxGridStack[y];
				break;
			case "Market":
				maxStack = maxMarketStack[y];
				break;
			default:
				break;
		}

		// Debug.Log(debugTag + "[Shift Row Check] Target Card's stack is : " + target.CardStack.Count + ". Known max stack for type " + type + " is " + maxStack);
		if (target.CardStack.Count > maxStack)
		{
			// IncrementStackSize(y, target.Category);
			ShiftRow(type, y, 1);
			didShift = true;
			Debug.Log(debugTag + "Shifting row " + y + " for category: " + target.Category);
		}
		else
		{
			Debug.Log(debugTag + "Card stack of  " + target.CardStack.Count + " was not greater than " + maxStack);
		}

		return didShift;
	}

	// Shifts rows of cards up or down. Used to give room for cards under tiles.
	private void ShiftRow(string type, int row, int units)
	{
		switch (type)
		{
			case "Tile":
				ShiftMasterGrid(row, units);
				break;
			case "Market":
				ShiftMasterMarketGrid(row, units);
				break;
			default:
				break;
		}
	}

	// Used by ShiftRow to shift the Master Grid rows
	private void ShiftMasterGrid(int row, int units)
	{
		for (int x = 0; x < config.GameGridWidth; x++)
		{
			for (int y = row; y < config.GameGridHeight; y++)
			{
				// Debug.Log(debugTag + "Looking for x" + x + ", y" + y);
				// Debug.Log(debug + "Shifting [" + x + ", " + y + "]");
				float oldX = knownOwnersGrid[x, y].CardObject.transform.position.x;
				float oldY = knownOwnersGrid[x, y].CardObject.transform.position.y;
				float oldZ = knownOwnersGrid[x, y].CardObject.transform.position.z;

				StartCoroutine(CardAnimations.MoveTileCoroutine(knownOwnersGrid[x, y].CardObject,
					new Vector3(0, shiftUnit, 0), .1f));

				// knownOwnersGrid[x, y].CardObject.transform.position = new Vector3(oldX,
				// 	(oldY += (shiftUnit * units)),
				// 	knownOwnersGrid[x, y].CardObject.transform.position.z);
			}
		}
	}

	// Used by ShiftRow to shift the Master Market Grid rows
	// TODO: update to use animations
	private void ShiftMasterMarketGrid(int row, int units)
	{
		int marketWidth = Mathf.CeilToInt((float)matchController.MasterDeck.marketCardDeck.Count
			/ (float)config.GameGridHeight);

		for (int x = 0; x < marketWidth; x++)
		{
			for (int y = row; y < config.GameGridHeight; y++)
			{

				if (masterMarketGrid[x, y] != null)
				{
					float oldX = masterMarketGrid[x, y].CardObject.transform.position.x;
					float oldY = masterMarketGrid[x, y].CardObject.transform.position.y;
					// float oldZ = masterMarketGrid[x, y].tileObj.transform.position.z;

					StartCoroutine(CardAnimations.MoveTileCoroutine(masterMarketGrid[x, y].CardObject,
						new Vector3(0, shiftUnit, 0), .1f));

					// masterMarketGrid[x, y].CardObject.transform.position = new Vector3(oldX,
					// 	(oldY += (shiftUnit * units)),
					// 	masterMarketGrid[x, y].CardObject.transform.position.z);
				}
			}
		}
	}

	public void UpdatePlayerMoneyValues()
	{
		// Updates all tiles for each player.
		for (int i = 0; i < config.MaxPlayerCount; i++)
		{
			Debug.Log(debugTag + "[UpdateAllMoneyValues] Calculating CardData for Player " + (i + 1));
			matchController.Players[i].ResetMoney();

			for (int x = 0; x < config.GameGridWidth; x++)
			{
				for (int y = 0; y < config.GameGridHeight; y++)
				{
					// Debug.Log(debugTag + "[UpdateAllMoneyValues] Tile (" + x + ", " + y + ") Owner: " + masterGrid[x, y].OwnerId);
					// Debug.Log(debugTag + "[UpdateAllMoneyValues] Category: " + masterGrid[x, y].Category);
					if (masterGrid[x, y].OwnerId == (i + 1))
					{
						// Debug.Log(debugTag + "[UpdateAllMoneyValues] Match!");
						matchController.Players[i].AddMoney(CalcCardValue(masterGrid[x, y]));
					} // if player owns tile
				} // for y
			} // for x
		}
	}

	// Updates Market Card prices.
	public void UpdateMarketCardValues()
	{
		// for (int i = 0; i < ResourceInfo.prices.Count; i++)
		// {
		for (int x = 0; x < masterMarketGrid.GetLength(0); x++)
		{
			for (int y = 0; y < masterMarketGrid.GetLength(1); y++)
			{
				if (masterMarketGrid[x, y] != null)
					CalcCardValue(masterMarketGrid[x, y]);
			} // for y
		} // for x
		// }
	}

	private int CalcCardValue(CardData cardData)
	{
		double baseValue = 0;
		int retrievedPrice = 0;
		double valueMod = 0;
		double totalValue = 0;

		if (cardData == null)
		{
			Debug.LogError(debugTag.error + "[CalcCardValue] Passed CardData was null!");
		}

		Debug.Log(debugTag + "[CalcCardValue] Calculating data for  " + cardData.Title + " " + cardData.Resource + " " + cardData.FooterValue);

		// Debug.Log(debugTag + "[CalcCardValue] Category: " + cardData.Category);

		// Calculates the value of the resources built-in to the card
		if (cardData.Category == "Tile")
		{
			ResourceInfo.pricesMut.TryGetValue(cardData.Resource, out retrievedPrice);
			baseValue = (retrievedPrice * cardData.FooterValue); //Could be 0, that's okay

			Debug.Log(debugTag + "[CalcCardValue] Found price of " + cardData.Resource + ": $" + retrievedPrice);
		}
		else if (cardData.Category == "Market")
		{
			int baseResourcePrice;
			ResourceInfo.prices.TryGetValue(cardData.Resource, out baseResourcePrice);
			baseValue = baseResourcePrice;
		}

		// Calculates the value of the resources and pure cash on cards in the stack, if any.
		// Debug.Log(debugTag + "[CalcCardValue] CardStack Size: " + cardData.CardStack.Count);
		for (int i = 0; i < cardData.CardStack.Count; i++)
		{
			retrievedPrice = 0;

			Debug.Log(debugTag + "[CalcCardValue] Found Stack Card #" + i + ", Subtitle:" + cardData.CardStack[i].Subtitle);
			Debug.Log(debugTag + "[CalcCardValue] Found Stack Card #" + i + ", Old Base Value:" + baseValue);

			// Resources, Additive/Subtractive Cash Modifiers
			if (cardData.CardStack[i].Subtitle == "Resource")
			{
				ResourceInfo.pricesMut.TryGetValue(cardData.CardStack[i].Resource, out retrievedPrice);
				baseValue += (retrievedPrice * cardData.CardStack[i].FooterValue);
			}
			else if (cardData.CardStack[i].Subtitle == "Investment" && !cardData.CardStack[i].PercFlag)
			{
				baseValue += cardData.CardStack[i].FooterValue;
			}
			else if (cardData.CardStack[i].Subtitle == "Sabotage" && !cardData.CardStack[i].PercFlag)
			{
				baseValue -= cardData.CardStack[i].FooterValue;
			}

			Debug.Log(debugTag + "[CalcCardValue] Found Stack Card #" + i + ", New Base Value:" + baseValue);

			Debug.Log(debugTag + "[CalcCardValue] Found Stack Card #" + i + ", Old Mod Value:" + valueMod);

			// Percentage Modifiers
			if (cardData.CardStack[i].Subtitle == "Investment" && cardData.CardStack[i].PercFlag)
			{
				valueMod += cardData.CardStack[i].FooterValue;
			}
			else if (cardData.CardStack[i].Subtitle == "Sabotage" && cardData.CardStack[i].PercFlag)
			{
				valueMod -= cardData.CardStack[i].FooterValue;
			}

			Debug.Log(debugTag + "[CalcCardValue] Found Stack Card #" + i + ", New Mod Value:" + valueMod);
		}

		if (baseValue < 0 && valueMod < 0)
			valueMod = Mathf.Abs((float)valueMod);

		totalValue = baseValue + (baseValue * valueMod / 100d);

		// Update the mutable resource price, if necessary.
		if (cardData.Category == "Market")
		{
			ResourceInfo.pricesMut[cardData.Resource] = (int)totalValue;
			Debug.Log(debugTag + "[CalcCardValue] " + cardData.Resource + " Price:  $" + ResourceInfo.pricesMut[cardData.Resource]);
		}

		if (cardData.Category == "Tile")
		{
			Debug.Log(debugTag + "[CalcCardValue] " + cardData.CardObject.name + " Base:  $" + baseValue + ", Mod: %" + valueMod + ", Total: $" + totalValue);
		}

		// Debug.Log(debugTag + "[CalcCardValue] " + cardData.CardObject.name + " base value:  " + baseValue);
		// Debug.Log(debugTag + "[CalcCardValue] " + cardData.CardObject.name + " value mod: " + valueMod);
		// // Debug.Log(debugTag + "[CalcCardValue] " + cardData.Title + " total value: " + totalValue);

		// Debug.Log(debugTag + "[CalcCardValue] Total Value of card found: $" + totalValue);

		return (int)totalValue;
	}

	// public void CalcBaseValue()
	// {
	// 	// Calculates the value of the resources built-in to the card
	// 	if (this.category == "Tile")
	// 	{
	// 		this.baseValue = 0;
	// 		int retrievedPrice = 0;
	// 		ResourceInfo.pricesMut.TryGetValue(this.resource, out retrievedPrice);
	// 		this.baseValue = (retrievedPrice * this.quantity); //Could be 0, that's okay

	// 		// Calculates the value of the resources on cards in the stack, if any
	// 		for (int i = 0; i < this.cardStack.Count; i++)
	// 		{
	// 			retrievedPrice = 0;

	// 			if (this.cardStack[i].Subtitle == "Resource")
	// 			{
	// 				ResourceInfo.pricesMut.TryGetValue(this.cardStack[i].Resource, out retrievedPrice);
	// 				this.baseValue += (retrievedPrice * this.cardStack[i].FooterValue);
	// 			}
	// 			else if (this.cardStack[i].Subtitle == "Investment" && !this.cardStack[i].PercFlag)
	// 			{
	// 				this.baseValue += this.cardStack[i].FooterValue;
	// 			}
	// 			else if (this.cardStack[i].Subtitle == "Sabotage" && !this.cardStack[i].PercFlag)
	// 			{
	// 				this.baseValue -= this.cardStack[i].FooterValue;
	// 			} // if
	// 		} // for cardStack size
	// 	}
	// 	else if (this.card.Category == "Market")
	// 	{
	// 		// Base Value will not be used for Market Cards - Reference ResourceInfo instead.
	// 		// ResourceInfo.prices.TryGetValue(this.resource, out this.baseValue);
	// 		// Debug.Log(debug + "Base value for " + this.resource + " was found to be " + this.baseValue);
	// 	}

	// } // CalcBaseValue()

	// Calculates the value of the resources on cards in the stack, if any
	// public void CalcValueMod()
	// {
	// 	if (this.category == "Tile")
	// 	{
	// 		this.valueMod = 0;
	// 		for (int i = 0; i < this.cardStack.Count; i++)
	// 		{
	// 			if (this.cardStack[i].Subtitle == "Investment" && this.cardStack[i].PercFlag)
	// 			{
	// 				this.valueMod += this.cardStack[i].FooterValue;
	// 			}
	// 			else if (this.cardStack[i].Subtitle == "Sabotage" && this.cardStack[i].PercFlag)
	// 			{
	// 				this.valueMod -= this.cardStack[i].FooterValue;
	// 			} // if-else
	// 		} // for cardStack size
	// 	}
	// 	else if (this.category == "Market")
	// 	{
	// 		// NOTE: Currently, this is the same code for Tile. This is here incase it needs to
	// 		// to change at some point.
	// 		this.valueMod = 0;
	// 		// Debug.Log("Stack Size  :" + this.stackSize);
	// 		// Debug.Log("Stack Count: " + this.cardStack.Count);

	// 		for (int i = 0; i < this.cardStack.Count; i++)
	// 		{
	// 			if (this.cardStack[i].Subtitle == "Investment" && this.cardStack[i].PercFlag)
	// 			{
	// 				this.valueMod += this.cardStack[i].FooterValue;
	// 				// Debug.Log("Should be working! " + this.cardStack[i].footerValue);
	// 			}
	// 			else if (this.cardStack[i].Subtitle == "Sabotage" && this.cardStack[i].PercFlag)
	// 			{
	// 				this.valueMod -= this.cardStack[i].FooterValue;
	// 				// Debug.Log("Should be working! -" + this.cardStack[i].footerValue);
	// 				// this.valueMod -= this.cardStack[i].footerValue; // NOTE: Only do locally!
	// 			} // if-else
	// 		} // for cardStack size
	// 	} // if-else category
	// } // CalcValueMod()

	// Calculates the total value this card, and updates the
	//	totalValue field on this GridUnit object
	// public void CalcTotalValue()
	// {
	// 	// Debug.Log(debug + "Category: " + this.category);
	// 	// Reset all value data and recalculate
	// 	if (this.category == "Tile")
	// 	{
	// 		this.totalValue = 0;
	// 		this.CalcBaseValue();
	// 		this.CalcValueMod();

	// 		this.totalValue = (double)this.baseValue
	// 			+ ((double)this.baseValue * ((double)this.valueMod) / 100d);

	// 		// Debug.Log(debug + "[GridUnit] Tile " + this.x + ", " + this.y + " base value:  " +
	// 		// 	this.baseValue);
	// 		// Debug.Log(debug + "[GridUnit] Tile " + this.x + ", " + this.y + " total value: " +
	// 		// 	this.totalValue);

	// 		if (this.totalValue < 0)
	// 		{
	// 			this.bankrupt = true;
	// 			// GameManager.BankruptTile(this);
	// 		} // bankrupt check
	// 	}
	// 	else if (this.category == "Market")
	// 	{

	// 		// Reset all value data and recalculate
	// 		this.totalValue = 0;
	// 		this.CalcBaseValue();
	// 		this.CalcValueMod();

	// 		// Debug.Log(debug + "Resource: " + this.resource + " ----------------------");
	// 		// Debug.Log(debug + "Old PriceMut: " + ResourceInfo.pricesMut[this.resource]);
	// 		// // ResourceInfo.prices.TryGetValue(this.resource, out this.baseValue);
	// 		// Debug.Log(debug + "Base Value  : " + ResourceInfo.prices[this.resource]);
	// 		// Debug.Log(debug + "Value Mod   : " + this.valueMod);
	// 		// Debug.Log(debug + "Calculating...");

	// 		this.totalValue = (double)ResourceInfo.prices[this.resource]
	// 			+ ((double)ResourceInfo.prices[this.resource] * ((double)this.valueMod) / 100d);

	// 		// Debug.Log("Total Value: " + this.totalValue);

	// 		ResourceInfo.pricesMut[this.resource] = (int)this.totalValue;
	// 		// cardDis.UpdateFooter(this, ResourceInfo.pricesMut[this.resource]);
	// 		// tileObj.GetComponent<CardState>().footerValue = ResourceInfo.pricesMut[this.resource];

	// 		// Debug.Log(debug + "New PriceMut: " + ResourceInfo.pricesMut[this.resource]);
	// 		// // ResourceInfo.prices.TryGetValue(this.resource, out this.baseValue);
	// 		// Debug.Log(debug + "Base Value  : " + ResourceInfo.prices[this.resource]);
	// 		// Debug.Log(debug + "Value Mod   : " + this.valueMod);
	// 	} // if tile
	// } // CalcTotalValue()

	// COROUTINES ##################################################################################

	// [Client/Server] Parses the Match Config from MatchDataBroadcaster
	public IEnumerator ParseMatchConfigCoroutine()
	{
		yield return StartCoroutine(GrabMatchDataBroadCasterCoroutine());

		while (this.config == null)
		{
			Debug.Log(debugTag + "Parsing Config...");
			this.config = JsonUtility.FromJson<MatchConfig>(matchDataBroadcaster.MatchConfigStr);
			Debug.Log(debugTag + "Config parsed as: " + this.config);

			if (this.config == null)
				yield return null;
		}
	}

	// [Client] Grabs the MatchDataBroadcaster from the MatchManager GameObject.
	private IEnumerator GrabMatchDataBroadCasterCoroutine()
	{
		if (matchDataBroadcaster == null)
		{
			matchDataBroadcaster = this.gameObject.GetComponent<MatchDataBroadcaster>();
			if (matchDataBroadcaster != null)
			{
				Debug.Log(debugTag + "MatchDataBroadcaster was found!");
			}
		}

		while (this.gameObject == null || matchDataBroadcaster == null)
		{
			Debug.LogError(debugTag.error + "MatchDataBroadcaster was NOT found!");
			yield return null;
		}
	}

	// [Client] Grabs the MatchDataBroadcaster from the MatchManager GameObject.
	private IEnumerator GrabMatchControllerCoroutine()
	{
		if (matchController == null)
		{
			matchController = this.gameObject.GetComponent<MatchController>();
			if (matchController != null)
			{
				Debug.Log(debugTag + "MatchController was found!");
			}
		}

		while (this.gameObject == null || matchController == null)
		{
			Debug.LogError(debugTag.error + "MatchController was NOT found!");
			yield return null;
		}
	}

	// [Client/Server] Create the Tile GameObjects for the Main Game Grid.
	private IEnumerator CheckForBroadcastUpdates()
	{
		yield return StartCoroutine(GrabMatchDataBroadCasterCoroutine());

		while (this.config == null)
			yield return StartCoroutine(ParseMatchConfigCoroutine());

		ParseTurnEvent();
		ParseUpdatedCards();
	}

	// [Client/Server] Create the Tile GameObjects for the Main Game Grid.
	private IEnumerator CreateMainGridCoroutine()
	{
		yield return StartCoroutine(ParseMatchConfigCoroutine());
		yield return StartCoroutine(GrabMatchControllerCoroutine());

		yield return StartCoroutine(CreateInternalMainGridCoroutine());

		// [Client with Server-Only section]
		Debug.Log(debugTag + "Creating Main Grid objects...");
		CreateGameGridObjects();
	}

	// [Client/Server] Create the Tile GameObjects for the Market Game Grid.
	private IEnumerator CreateMarketGridCoroutine()
	{
		yield return StartCoroutine(ParseMatchConfigCoroutine());
		yield return StartCoroutine(GrabMatchControllerCoroutine());

		Debug.Log(debugTag + "Creating Market Grid objects...");
		CreateMarketGridObjects();
	}

	// // [Client/Server] Create the Tile GameObjects for the Market Game Grid.
	// public IEnumerator CreatePlayerHandCoroutine(int playerId)
	// {
	// 	yield return StartCoroutine(ParseMatchConfigCoroutine());
	// 	yield return StartCoroutine(GrabMatchControllerCoroutine());

	// 	Debug.Log(debugTag + "Creating Player Hand objects...");
	// 	CreatePlayerHandObjects(playerId);
	// }

	private IEnumerator CreateInternalMainGridCoroutine()
	{
		yield return StartCoroutine(ParseMatchConfigCoroutine());

		masterGrid = new CardData[config.GameGridWidth, config.GameGridHeight];
		knownOwnersGrid = new CardData[config.GameGridWidth, config.GameGridHeight];
		rowPosition = new float[config.GameGridHeight];
		maxGridStack = new int[config.GameGridHeight];

		for (int x = 0; x < config.GameGridWidth; x++)
		{
			for (int y = 0; y < config.GameGridHeight; y++)
			{
				// Draw a card from the Land Tile deck
				// Card card = Card.CreateInstance<Card>();
				// [Server]
				if (hasAuthority)
				{
					Card card;

					if (matchController.DrawCard(matchController.MasterDeckMutable.landTileDeck,
							matchController.MasterDeck.landTileDeck,
							out card))
					{
						// Debug.Log("[GridManager] Tile Draw successful!");
						// Connect the drawn card to the internal grid
						masterGrid[x, y] = new CardData(card);
					}
					else
					{
						Debug.LogWarning(debugTag + "The LandTile Deck is out of cards! Refreshing the mutable deck...");

						matchController.RefreshDeck("LandTile");
						y--;
					}
				}

			} // y
		} // x
	}
}

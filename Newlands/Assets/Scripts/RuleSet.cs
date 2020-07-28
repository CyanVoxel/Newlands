// The master set of game rules, which includes methods for determining legal actions as well as
// what happens with a specific card is played.

using UnityEngine;

public class RuleSet : MonoBehaviour
{
	private static DebugTag debugTag = new DebugTag("RuleSet", "91EAFF");

	// private GridManager gridMan;
	// private GameManager gameMan;

	// METHODS ####################################################################################

	void Start()
	{
		// gridMan = FindObjectOfType<GridManager>();
		// gameMan = FindObjectOfType<GameManager>();
	}

	// Compares a Game Card against a target Card/Tle to determine if it is allowed to be played
	// given the scope of the Game Card.
	// NOTE: This is the devil's work and must be destroyed.

	// Previously routed to what's not MatchAllScopes().
	// Determines if the card should be allowed to be played on your own or other player's tiles,
	// then verifies the scope is valid.
	public static bool IsLegal(CardData target, Card card, int playerId)
	{
		if (IsOwnerValidated(target, card, playerId) && MatchesAllScopes(target, card, playerId))
			return true;
		else
			return false;
	}

	// Descides if a Card's target matches the scope of the Tile it's trying to be played on.
	// NOTE: A REFACTOR WOULD BE SOOOOOO NICE!
	private static bool MatchesAllScopes(CardData target, Card card, int playerId)
	{
		string[] scopeLevel;
		string[] targetLevel = new string[3];
		bool[] scopeResults;

		if (card.Target != null)
		{
			scopeLevel = card.Target.Split('_');
			scopeResults = new bool[scopeLevel.Length];
		}
		else
		{
			Debug.Log(debugTag + "The card's target is null!");
			return false;
		}

		targetLevel[0] = target.Category;
		targetLevel[1] = target.Subtitle;
		targetLevel[2] = target.Title;

		Debug.Log(debugTag + "\n"
			+ "Scope:" + card.Target + "\n"
			+ "Target:" + targetLevel[0] + "_" + targetLevel[1] + "_" + targetLevel[2] + "\n");

		TestScope0();
		if (scopeLevel.Length == 1 && scopeResults[0])
		{
			// Debug.Log("<b>[RuleSet]</b> Level [0] Match! Returning True!");
			return true;
		}
		else if (scopeLevel.Length == 1 && !scopeResults[0])
		{
			// Debug.Log("<b>[RuleSet]</b> " +
			// 		"Returned false, failed Scope Pass [0] with length of 1");
			return false;
		}
		// Debug.Log("<b>[RuleSet]</b> Level [0] Match! Continuing...");

		TestScope1();
		if (scopeLevel.Length == 2 && scopeResults[1])
		{
			// Debug.Log("<b>[RuleSet]</b> Level [1] Match! Returning True!");
			return true;
		}
		else if (scopeLevel.Length == 2 && !scopeResults[1])
		{
			// Debug.Log("<b>[RuleSet]</b> " +
			// 		"Returned false, failed Scope Pass [1] with length of 2");
			return false;
		}
		// Debug.Log("<b>[RuleSet]</b> Level [1] Match! Continuing...");

		TestScope2();
		if (scopeLevel.Length == 3 && scopeResults[2])
		{
			// Debug.Log("<b>[RuleSet]</b> Level [2] Match! Returning True!");
			return true;
		}
		else if (scopeLevel.Length == 3 && !scopeResults[2])
		{
			// Debug.Log("<b>[RuleSet]</b> " +
			// 		"Returned false, failed Scope Pass [2] with length of 3");
			return false;
		}

		Debug.Log(debugTag + "Default return of False");
		return false;

		// Local Methods =======================================================
		// NOTE: This is where the actual rules are located.
		// TODO: A slightly different system will need to be used when custom resources are added.

		bool MatchScope0(string finalScope)
		{
			// Debug.Log("<b>[RuleSet]</b> Checking if " +
			// finalScope +
			// " matches target " +
			// targetLevel[0]);

			if (scopeLevel.Length >= 1)
			{
				// Checks the Scope Level 0 (Category)
				switch (finalScope)
				{
					case "Tile":
						if (targetLevel[0] == "Tile") { return true; }
						break;
					case "Market":
						if (targetLevel[0] == "Market") { return true; }
						break;
					default:
						Debug.Log(debugTag
							+ "The target " + card.Target
							+ " is out of scope for the card "
							+ target.Category + " at scope level 0");
						break;
				} // switch
			}

			return false;
		} // MatchScope0

		bool MatchScope1(string finalScope)
		{
			// Debug.Log("<b>[RuleSet]</b> Checking if " +
			// finalScope +
			// " matches target " +
			// targetLevel[1]);

			if (scopeLevel.Length >= 2)
			{
				// Checks the Scope Level 1
				switch (finalScope)
				{
					case "Any":
						return true;
					case "Land":
						if (targetLevel[1] == "Land") { return true; }
						break;
					case "Coast":
						if (targetLevel[1] == "Coast") { return true; }
						break;
					default:
						Debug.Log(debugTag
							+ "The target " + card.Target
							+ " is out of scope for the card "
							+ target.Category + " at scope level 1");
						break;
				} // switch
			}

			return false;
		} // MatchScope1

		bool MatchScope2(string finalScope)
		{
			// Debug.Log("<b>[RuleSet]</b> Checking if " +
			// finalScope +
			// " matches target " +
			// targetLevel[2]);

			if (scopeLevel.Length >= 3)
			{
				// Checks the Scope Level 2
				switch (finalScope)
				{
					case "Any":
						return true;

					case "Forest":
						if (targetLevel[2] == "Forest") { return true; }
						break;

					case "Plains":
						if (targetLevel[2] == "Plains")
						{
							return true;
						}
						else if (targetLevel[2] == "Farmland" && card.Subtitle != "Upgrade")
						{
							return true;
						}
						break;

					case "Farmland":
						if (targetLevel[2] == "Farmland") { return true; }
						break;

					case "Mountain":
						if (targetLevel[2] == "Mountain") { return true; }
						break;

					case "Beach":
						if (targetLevel[2] == "Beach") { return true; }
						break;

					case "Ocean":
						if (targetLevel[2] == "Ocean") { return true; }
						break;

					case "Docks":
						if (targetLevel[2] == "Docks") { return true; }
						break;

					default:
						Debug.Log(debugTag
							+ "The target " + card.Target
							+ " is out of scope for the card "
							+ target.Category + " at scope level 2");
						break;
				} // switch
			}

			return false;
		} // MatchScope2

		// Runs MatchScope0(), multiple times in the case of an '|' char included
		void TestScope0()
		{
			if (scopeLevel[0].Contains("|"))
			{
				string[] subLevel = scopeLevel[0].Split('|');

				for (int i = 0; i < subLevel.Length; i++)
				{
					if (scopeResults[0] = MatchScope0(subLevel[i]))
					{
						i = subLevel.Length; // break
					}
				} // for
			}
			else
			{
				scopeResults[0] = MatchScope0(scopeLevel[0]);
			} // if-else contains '|'
		} // TestScope

		// Runs MatchScope1(), multiple times in the case of an '|' char included
		void TestScope1()
		{
			if (scopeLevel[1].Contains("|"))
			{
				string[] subLevel = scopeLevel[1].Split('|');

				for (int i = 0; i < subLevel.Length; i++)
				{
					if (scopeResults[1] = MatchScope1(subLevel[i]))
					{
						i = subLevel.Length; // break
					}
				} // for
			}
			else
			{
				scopeResults[1] = MatchScope1(scopeLevel[1]);
			} // if-else contains '|'
		} // TestScope

		// Runs MatchScope2(), multiple times in the case of an '|' char included
		void TestScope2()
		{
			if (scopeLevel[2].Contains("|"))
			{
				string[] subLevel = scopeLevel[2].Split('|');

				for (int i = 0; i < subLevel.Length; i++)
				{
					if (scopeResults[2] = MatchScope2(subLevel[i]))
					{
						i = subLevel.Length; // break
					}
				} // for
			}
			else
			{
				scopeResults[2] = MatchScope2(scopeLevel[2]);
			} // if-else contains '|'
		} // TestScope
	} // IsLegal

	// Carries out the action that a legal Game Card intends
	// public void PlayCard(GridUnit target, Card cardToPlay)
	// {
	// 	string action = cardToPlay.Subtitle;
	// 	// NOTE: Cards promting tile value calculation have their calculations offset to
	// 	// the GridUnit class. When adding a new case here, you'll need to also add one
	// 	// in GridUnit so it knows how to do the calculations based on the card info.

	// 	// Debug.Log("<b>[RuleSet]</b> Playing a card...");

	// 	switch (action)
	// 	{
	// 		case "Investment":
	// 			target.CalcTotalValue();
	// 			// gameMan.UpdatePlayersInfo();
	// 			break;

	// 		case "Sabotage":
	// 			target.CalcTotalValue();
	// 			// gameMan.UpdatePlayersInfo();
	// 			break;

	// 		case "Resource":
	// 			target.CalcTotalValue();
	// 			// gameMan.UpdatePlayersInfo();
	// 			break;

	// 		case "Foreclosure":
	// 			target.bankrupt = true;
	// 			// GameManager.BankruptTile(target);
	// 			// gameMan.UpdatePlayersInfo();
	// 			break;
	// 		case "Upgrade":
	// 			if (GetScope(cardToPlay, 2) == "Plains" && target.subScope != "Farmland")
	// 			{
	// 				// Card newCard = Card.CreateInstance<Card>();
	// 				// CardData newCard;
	// 				// CardData newCard = new CardData(Resources.Load<CardTemplate>("Cards/Tiles/Land/farmland_cashcrops_5"));
	// 				// target.tileObj.SendMessage("DisplayCard", newCard);

	// 				// GridManager.grid[target.x, target.y].LoadNewCard(newCard, target.tileObj);
	// 				// target = GridManager.grid[target.x, target.y];

	// 				// target.CalcTotalValue();
	// 				// gameMan.UpdatePlayersInfo();
	// 			} // if Plains
	// 			break;
	// 		default:
	// 			Debug.LogWarning(debugTag.warning
	// 				+ "No actions found for " + action + "!");
	// 			break;
	// 	} // switch

	// } // PlayCard(Card, GridUnit)

	// // Carries out the action that a legal Game Card intends (Override)
	// public void PlayCard(GridUnit target, GridUnit cardToPlay)
	// {
	// 	PlayCard(target, cardToPlay.card);
	// } // PlayCard(GridUnit, GridUnit)

	private static string GetScope(Card card, int level = -1)
	{
		string[] scopeLevel = new string[3];

		if (card.Target != null)
		{
			scopeLevel = card.Target.Split('_');
		}
		else
		{
			Debug.Log(debugTag + "Hey, that card's target is null!");
			return "error";
		}

		switch (level)
		{
			case 0:
				return scopeLevel[0];
			case 1:
				return scopeLevel[1];
			case 2:
				return scopeLevel[2];
			default:
				return card.Target;
		}
	} // GetScope()

	// Determines if the Target is valid based on the type of Target Tile and
	// the person playing it.
	private static bool IsOwnerValidated(CardData target, Card card, int playerId)
	{
		bool result = false;

		// Market Tiles are except (for now?)
		if (target.Category != "Market")
		{
			// If it's a NEGATIVE Card (currently only Sabotage),
			if (card.Subtitle == "Sabotage")
			{
				// Then it's only okay to play on OTHER PEOPLE's Tiles.
				if (target.OwnerId != playerId)
					result = true;
			}
			else // Or if it's any other (POSITIVE) Card,
			{
				// Then it's only okay to play on YOUR OWN Tiles.
				if (target.OwnerId == playerId)
					result = true;
			}
		}
		else // Market Tiles get a free pass.
		{
			result = true;
		}

		return result;
	}
}

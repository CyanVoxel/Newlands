// The master set of game rules, which includes methods for determining legal action.

using UnityEngine;

public class RuleSet {

	// METHODS ####################################################################################

	public static bool IsLegal(GridUnit target, GridUnit card) {

		string[] scopeLevel;
		string[] targetLevel = new string[3];
		bool[] scopeResults;

		

		if (card.target != null) {
			scopeLevel = card.target.Split('_');
			scopeResults = new bool[scopeLevel.Length];
		} else {
			Debug.Log("Hey, that card's target is null!");
			return false;
		}

		
		targetLevel[0] = target.category;
		targetLevel[1] = target.scope;
		targetLevel[2] = target.subScope;

		Debug.Log("<b>[RuleSet]</b> " + "\n" +
					"Scope:" + card.target + "\n" +
					"Target:" + targetLevel[0] + "_" + targetLevel[1] + "_" + targetLevel[2] +"\n");



		TestScope0();
		if (scopeLevel.Length == 1 && scopeResults[0]) {
			Debug.Log("<b>[RuleSet]</b> Level [0] Match! Returning True!");
			return true;
		} else if (scopeLevel.Length == 1 && !scopeResults[0]) {
			Debug.Log("<b>[RuleSet]</b> " +
					"Returned false, failed Scope Pass [0] with length of 1");
			return false;
		}
		Debug.Log("<b>[RuleSet]</b> Level [0] Match! Continuing...");
		

		TestScope1();
		if (scopeLevel.Length == 2 && scopeResults[1]) {
			Debug.Log("<b>[RuleSet]</b> Level [1] Match! Returning True!");
			return true;
		} else if (scopeLevel.Length == 2 && !scopeResults[1]) {
			Debug.Log("<b>[RuleSet]</b> " +
					"Returned false, failed Scope Pass [1] with length of 2");
			return false;
		}
		Debug.Log("<b>[RuleSet]</b> Level [1] Match! Continuing...");

		TestScope2();
		if (scopeLevel.Length == 3 && scopeResults[2]) {
			Debug.Log("<b>[RuleSet]</b> Level [2] Match! Returning True!");
			return true;
		} else if (scopeLevel.Length == 3 && !scopeResults[2]) {
			Debug.Log("<b>[RuleSet]</b> " +
					"Returned false, failed Scope Pass [2] with length of 3");
			return false;
		}


		Debug.Log("<b>[RuleSet]</b> Default return of False");
		return false;

		// LOCAL METHODS ######################################################
		// NOTE: This is where the actual rules are located.
		// TODO: A slightly different system will need to be used when custom resources are added.

		bool MatchScope0(string finalScope) {

			// Debug.Log("<b>[RuleSet]</b> Checking if " + finalScope + " matches target " + targetLevel[0]);
			if (scopeLevel.Length >= 1) {
				// Checks the Scope Level 0 (Category)
				switch (finalScope) {
					case "Tile": if (targetLevel[0] == "Tile") { return true; } break;
					case "Market":if (targetLevel[0] == "Market") { return true; } break;
					default:
						Debug.Log("<b>[RuleSet]</b> " +
						"The target " + card.target +
						" is out of scope for the card " +
						target.category + " at scope level 0"); break;
				} // switch
			}
			return false;

		} // MatchScope0

		bool MatchScope1(string finalScope) {

			// Debug.Log("<b>[RuleSet]</b> Checking if " + finalScope + " matches target " + targetLevel[1]);
			if (scopeLevel.Length >= 2) {
				// Checks the Scope Level 0 (Category)
				switch (finalScope) {
					case "Any": return true;
					case "Land": if (targetLevel[1] == "Land") { return true; } break;
					case "Coast": if (targetLevel[1] == "Coast") { return true; } break;
					default:
						Debug.Log("<b>[RuleSet]</b> " +
						"The target " + card.target +
						" is out of scope for the card " +
						target.category + " at scope level 1"); break;
				} // switch
			}
			return false;
			
		} // MatchScope1

		bool MatchScope2(string finalScope) {

			// Debug.Log("<b>[RuleSet]</b> Checking if " + finalScope + " matches target " + targetLevel[2]);
			if (scopeLevel.Length >= 3) {
				// Checks the Scope Level 0 (Category)
				switch (finalScope) {
					case "Any": return true;
					case "Forest": if (targetLevel[2] == "Forest") { return true; } break;
					case "Plains": if (targetLevel[2] == "Plains") { return true; } break;
					case "Quarry": if (targetLevel[2] == "Quarry") { return true; } break;
					case "Farmland": if (targetLevel[2] == "Farmland") { return true; } break;
					case "Beach": if (targetLevel[2] == "Beach") { return true; } break;
					case "Ocean": if (targetLevel[2] == "Ocean") { return true; } break;
					case "Docks": if (targetLevel[2] == "Docks") { return true; } break;
					default:
						Debug.Log("<b>[RuleSet]</b> " +
						"The target " + card.target +
						" is out of scope for the card " +
						target.category + " at scope level 2"); break;
				} // switch
			}
			return false;
			
		} // MatchScope2

		// Runs MatchScope0(), multiple times in the case of an '|' char included
		void TestScope0() {

			if (scopeLevel[0].Contains("|")) {

				string[] subLevel = scopeLevel[0].Split('|');

				for (int i = 0; i < subLevel.Length; i++) {
					
					if (scopeResults[0] = MatchScope0(subLevel[i])) {
						i = subLevel.Length; // break
					}
				} // for

			} else {
				scopeResults[0] = MatchScope0(scopeLevel[0]);
			} // if-else contains '|'
		} // TestScope

		// Runs MatchScope1(), multiple times in the case of an '|' char included
		void TestScope1() {

			if (scopeLevel[1].Contains("|")) {
				string[] subLevel = scopeLevel[1].Split('|');

				for (int i = 0; i < subLevel.Length; i++) {
					
					if (scopeResults[1] = MatchScope1(subLevel[i])) {
						i = subLevel.Length; // break
					}
				} // for

			} else {
				scopeResults[1] = MatchScope1(scopeLevel[1]);
			} // if-else contains '|'
		} // TestScope

		// Runs MatchScope2(), multiple times in the case of an '|' char included
		void TestScope2() {

			if (scopeLevel[2].Contains("|")) {
				string[] subLevel = scopeLevel[2].Split('|');

				for (int i = 0; i < subLevel.Length; i++) {
					
					if (scopeResults[2] = MatchScope2(subLevel[i])) {
						i = subLevel.Length; // break
					}
				} // for

			} else {
				scopeResults[2] = MatchScope2(scopeLevel[2]);
			} // if-else contains '|'
		} // TestScope



	} // IsLegal

	

} // RuleSet()

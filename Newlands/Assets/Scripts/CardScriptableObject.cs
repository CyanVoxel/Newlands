// The template for the Card Scriptable Object.
// Members included here include all possible Card types.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardScriptableObject : ScriptableObject {

	// The overall category of the Card (ex. Game Card, Price Card)
	public CardEnums.Category category; 

	// The Title of the Card, specifying its type in its category
	public CardEnums.Title title;

	// The subtitle of the Card, further specifying the title subcategory
	public CardEnums.Subtitle subtitle;

	// The body text of a Card
	public string bodyText;

	// The footer text of the Card
	public string footerText;

	// The raw nummber value that is presented in the Card footer
	public int footerValue;

	// Is the footerValue a percentage?
	public bool percFlag;

	// Is the footerValue a monetary value?
	public bool moneyFlag;
	
} // end Card class

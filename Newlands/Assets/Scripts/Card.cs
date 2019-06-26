// The template for the Card Scriptable Object.
// Members included here include all possible Card types.
// TODO: Make some bodies simpler, and possibly add another felid for in-depth descriptions
//	that can appear when using a future help/info tool.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject {

	// DATA FIELDS ------------------------------------------------------------

	// The overall category of the Card (ex. Game Card, Price Card)
	public string category; 

	// The Title of the Card, specifying its type in its category
	public string title;

	// The subtitle of the Card, further specifying the title subcategory
	public string subtitle;

	// The body text of a Card
	public string bodyText;

	// What type of Card category this Card targets, if any
	public string targetCategory = "Tile";

	// What subtypes of Card this Card can target within the category
	public string targetScope = "Any";

	// The footer text of the Card
	public string footerText;

	// The resource of the card, if it had one
	public string resource;

	// The raw number value that is presented in the Card footer
	public int footerValue;

	// The color of the footer border
	public string FooterColor;

	// Flag indicating if only the corners of the footer should be colored
	public bool onlyColorCorners;

	// Flag indicating if the footerValue is a percentage
	public bool percFlag;

	// Flag indicating if the footerValue is a monetary value
	public bool moneyFlag;

	// The operator of the footerValue
	public char footerOpr;


	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public Card() { }

	// Constructor that (should) create a card from a resource path
	public Card(string path) {
		this.Equals(Resources.Load<Card>(path));
	} // Card(string) constructor
	
} // Card class

// The template for the Card Scriptable Object.
// Members included here include all possible Card types.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject {

	// DATA FIELDS ------------------------------------------------------------

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

	// The resource of the card, if it had one
	public CardEnums.Resource resource;

	// The raw number value that is presented in the Card footer
	public int footerValue;

	// The color of the footer border
	public CardEnums.FooterColor FooterColor;

	// Flag indicating if only the corners of the footer should be colored
	public bool onlyColorCorners;

	// Flag indicating if the footerValue is a percentage
	public bool percFlag;

	// Flag indicating if the footerValue is a monetary value
	public bool moneyFlag;

	// The operator of the footerValue
	public CardEnums.FooterOp footerOp;


	// CONSTRUCTORS -----------------------------------------------------------

	// Default no-arg constructor
	public Card() { }

	// Constructor that (should) create a card from a resource path
	public Card(string path) {
		this.Equals(Resources.Load<Card>(path));
	} // Card(string) constructor
	
} // Card class

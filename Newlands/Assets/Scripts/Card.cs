// The template for the Card Scriptable Object.
// Members included here include all possible Card types.
// TODO: Make some bodies simpler, and possibly add another felid for in-depth descriptions
//	that can appear when using a future help/info tool.

using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject {

	// FIELDS #####################################################################################

	// The overall category of the Card (ex. Game Card, Price Card)
	public string category;

	// The Title of the Card, specifying its type in its category
	public string title;

	// The subtitle of the Card, further specifying the title subcategory
	public string subtitle;

	// The body text of a Card
	public string bodyText;

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

	// What subtypes of Card this Card can target within the category
	public string target;

	// Does this card get discarded if used? (Rather than being stacked under a Tile)
	public bool doesDiscard;

	// public CardData data;

} // Card class

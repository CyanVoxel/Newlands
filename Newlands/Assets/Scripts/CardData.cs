// A struct used to store any possible card data in a format that's able to be instantiated, used
// internally, or over the network.

public struct CardData
{
	// DATA FIELDS #################################################################################
	// NOTE: Currently, these should include all of the scriptable object fields.

	// The Card Object's Name
	public string objectName;
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
	public string footerColor;
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
	// public int ownerId;

	// CONSTRUCTORS ################################################################################

	public CardData(Card cardScript)
	{
		objectName = "Default"; // The Card Object's Name (Uninitialized)
		// ownerId = -1;
		title = cardScript.title;           // The Card's Title
		subtitle = cardScript.subtitle;     // The Card's Subtitle
		bodyText = cardScript.bodyText;     // The Card's Body Text
		footerText = cardScript.footerText; // The Card's Footer Text
		footerValue = cardScript.footerValue;
		percFlag = cardScript.percFlag;
		moneyFlag = cardScript.moneyFlag;
		footerOpr = cardScript.footerOpr;
		category = cardScript.category;     // The Card's Category (Used to determine misc visuals)
		resource = cardScript.resource;
		target = cardScript.target;
		doesDiscard = cardScript.doesDiscard;
		footerColor = cardScript.footerColor;
		onlyColorCorners = cardScript.onlyColorCorners;

	} // CardData(Card) constructor

	// public CardData(Card cardScript, int ownerId)
	// {
	// 	this = new CardData(cardScript);
	// 	this.ownerId = ownerId;

	// } // CardData(Card, int) constructor
} // struct CardData

// The template for the Card Scriptable Object.
// Members included here include all possible Card types.
// TODO: Make some bodies simpler, and possibly add another felid for in-depth descriptions
//	that can appear when using a future help/info tool.

public class Card
{
	// FIELDS ##########################################################################################################

	// The overall category of the Card (ex. Game Card, Price Card)
	private string category = "";
	// The Title of the Card, specifying its type in its category
	private string title = "";
	// The subtitle of the Card, further specifying the title subcategory
	private string subtitle = "";
	// The body text of a Card
	private string bodyText = "";
	// The footer text of the Card
	private string footerText = "";
	// The resource of the card, if it had one
	private string footerColor = "";
	// Flag indicating if only the corners of the footer should be colored
	private string resource = "";
	// What subtypes of Card this Card can target within the category
	private string target = "";
	// The operator of the footerValue
	private char footerOpr;
	// The raw number value that is presented in the Card footer
	private int footerValue;
	// The color of the footer border
	private bool colorCornerFlag;
	// Flag indicating if the footerValue is a percentage
	private bool percFlag;
	// Flag indicating if the footerValue is a monetary value
	private bool moneyFlag;
	// Does this card get discarded if used? (Rather than being stacked under a Tile)
	private bool discardFlag;

	// PROPERTIES ######################################################################################################

	public string Category { get { return category; } set { category = value; } }
	public string Title { get { return title; } set { title = value; } }
	public string Subtitle { get { return subtitle; } set { subtitle = value; } }
	public string BodyText { get { return bodyText; } set { bodyText = value; } }
	public string FooterText { get { return footerText; } set { footerText = value; } }
	public string FooterColor { get { return footerColor; } set { footerColor = value; } }
	public string Resource { get { return resource; } set { resource = value; } }
	public string Target { get { return target; } set { target = value; } }
	public char FooterOpr { get { return footerOpr; } set { footerOpr = value; } }
	public int FooterValue { get { return footerValue; } set { footerValue = value; } }
	public bool ColorCornerFlag { get { return colorCornerFlag; } set { colorCornerFlag = value; } }
	public bool PercFlag { get { return percFlag; } set { percFlag = value; } }
	public bool MoneyFlag { get { return moneyFlag; } set { moneyFlag = value; } }
	public bool DiscardFlag { get { return discardFlag; } set { discardFlag = value; } }

	// CONSTRUCTORS ####################################################################################################

	public Card() { }

	public Card(CardTemplate template)
	{
		this.category = template.category;
		this.title = template.title;
		this.subtitle = template.subtitle;
		this.bodyText = template.bodyText;
		this.footerText = template.footerText;
		this.resource = template.resource;
		this.footerValue = template.footerValue;
		this.footerColor = template.footerColor;
		this.colorCornerFlag = template.colorCornerFlag;
		this.percFlag = template.percFlag;
		this.moneyFlag = template.moneyFlag;
		this.footerOpr = template.footerOpr;
		this.target = template.target;
		this.discardFlag = template.discardFlag;
	}

	public Card(Card card)
	{
		this.category = card.category;
		this.title = card.title;
		this.subtitle = card.subtitle;
		this.bodyText = card.bodyText;
		this.footerText = card.footerText;
		this.resource = card.resource;
		this.footerValue = card.footerValue;
		this.footerColor = card.footerColor;
		this.colorCornerFlag = card.colorCornerFlag;
		this.percFlag = card.percFlag;
		this.moneyFlag = card.moneyFlag;
		this.footerOpr = card.footerOpr;
		this.target = card.target;
		this.discardFlag = card.discardFlag;
	}

} // Card class

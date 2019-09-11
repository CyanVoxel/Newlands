// A struct used to store any possible card data in a format that's able to be instantiated, used
// internally, or over the network.

public class CardData : Card
{
	// FIELDS ######################################################################################
	// private Card card = new Card();
	private string objectName = "Default";

	// PROPERTIES ##################################################################################
	// public Card Card { get { return card; } set { card = value; } }
	public string ObjectName { get { return objectName; } set { objectName = value; } }

	// CONSTRUCTORS ################################################################################

	public CardData() : base() { }

	public CardData(Card card) : base(card) { }

	public CardData(Card card, string objectName) : base(card)
	{
		this.objectName = objectName;
	}

}

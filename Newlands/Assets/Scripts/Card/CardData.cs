// A struct used to store any possible card data in a format that's able to be instantiated, used
// internally, or over the network.

using UnityEngine;

public class CardData : Card
{
	// FIELDS ######################################################################################
	// private Card card = new Card();
	private string objectName = "Default";
	private int ownerId = 0;
	private bool isBankrupt = false;

	private GameObject cardObject;

	// PROPERTIES ##################################################################################
	// public Card Card { get { return card; } set { card = value; } }
	public string ObjectName { get { return objectName; } set { objectName = value; } }
	public int OwnerId { get { return ownerId; } set { ownerId = value; } }
	public bool IsBankrupt { get { return isBankrupt; } set { isBankrupt = value; } }

	public GameObject CardObject{
		get {
			return cardObject;
		}
		set {
			cardObject = value;
			value.GetComponent<CardViewController>().Card = this;
		}
	}

	// CONSTRUCTORS ################################################################################

	public CardData() : base() { }

	public CardData(Card card) : base(card) { }

	public CardData(Card card, string objectName) : base(card)
	{
		this.objectName = objectName;
	}

}

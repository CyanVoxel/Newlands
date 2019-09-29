// A class used to store any possible card data in a format that's able to be instantiated, used
// internally, or over the network. Replacement for GridUnit.

using System.Collections.Generic;
using UnityEngine;

public class CardData : Card
{
	// FIELDS ######################################################################################
	// private Card card = new Card();
	private List<Card> cardStack = new List<Card>();
	private string objectName = "Default";
	private int ownerId = 0;
	private bool isBankrupt = false;

	private GameObject cardObject;

	// PROPERTIES ##################################################################################
	// public Card Card { get { return card; } set { card = value; } }
	public string ObjectName { get { return objectName; } set { objectName = value; } }
	public int OwnerId { get { return ownerId; } set { ownerId = value; } }
	public bool IsBankrupt { get { return isBankrupt; } set { isBankrupt = value; } }
	public List<Card> CardStack { get { return cardStack; } set { cardStack = value; } }

	public GameObject CardObject
	{
		get
		{
			return cardObject;
		}
		set
		{
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

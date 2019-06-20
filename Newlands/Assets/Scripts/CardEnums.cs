// A collection of enums used by the Card object.
// These are used to compare and verify that Tile objects have valid info.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEnums {

	// Categories of Tiles
	public enum Category {GameCard, PriceCard, LandTile}; 

	// The Title of the Card, specifying its type in its category
	public enum Title {TileMod, Resource, MarketMod, PriceCard,
					   Forest, Plains, Quarry};

	// The Title of the Card, specifying its type in its category
	public enum Subtitle {None, Investment, Sabotage, Resource,
						  Lumber, CashCrops, Oil, Iron, Gold, Silver, Gems, Platinum};

	// How the footer value on the Card is supposed to be applied
	public enum FooterOp {None, Add, Sub};

	// The color of the footer border
	public enum FooterColor {Black, Red, Blue, Green, Cyan, Magenta, Yellow};

	// Deck names
	public enum Deck {VanillaStandard, Corny};

	// All resources available
	public enum Resource {None, Lumber, CashCrops, Oil, Iron, Gold, Silver,
							 Gems, Platinum};

} // CardEnums class

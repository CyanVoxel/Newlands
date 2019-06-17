// This class acts as a bridge between the internal Card scriptable object data
// and the display components of the Card prefab.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour {

	// DATA FIELDS ------------------------------------------------------------

	// Textmesh Pro text objects --------------------------
	private TMP_Text title;
	private TMP_Text subtitle;
	private TMP_Text body;
	private TMP_Text footer;
	private Image footerBorder;

	// GameObject containers ------------------------------
	private GameObject titleObj;
	private GameObject subtitleObj;
	private GameObject bodyObj;
	private GameObject footerObj;
	private GameObject footerBorderObj;

	// Long directories stored as strings
	private string dirFooterBorder = "Front Canvas/Footer Mask/Footer Border Mask/Footer Border";

	// The local Card scriptable object
	private Card card;

	void Start() {

		// TEMP: Set the Card prefab to display one of the known Game Cards
		int landTilesLeft = GridManager.masterDeckMutable.landTileDeck.Count();
		int landTileCount = GridManager.masterDeck.landTileDeck.Count();

		// Draws a card from the mutable deck, then removes that card from the deck.
		// If all cards are drawn, draw randomly from the immutable deck.
		if (landTilesLeft > 0 ) {
			card = GridManager.masterDeckMutable.landTileDeck[Random.Range(0, landTilesLeft)];
			GridManager.masterDeckMutable.landTileDeck.Remove(card);
			// Debug.Log("<b>[CardDisplay]</b> " + 
			// 	landTilesLeft + 
			// 	" of " + 
			// 	landTileCount + 
			// 	" Master Deck cards left");
		} else {
			card = GridManager.masterDeck.landTileDeck[Random.Range(0, landTileCount)];
			Debug.LogWarning("<b>[CardDisplay]</b> Warning: " +
			 "All Land Tile cards were drawn! Now drawing from immutable deck...");
		}
		

		// Grab the display elements from this parent object
		GameObject titleObj = transform.Find("Front Canvas/Title").gameObject;
		GameObject subtitleObj = transform.Find("Front Canvas/Subtitle").gameObject;
		GameObject bodyObj = transform.Find("Front Canvas/Body").gameObject;
		GameObject footerObj = transform.Find("Front Canvas/Footer").gameObject;
		GameObject footerBorderObj = transform.Find(dirFooterBorder).gameObject;

		// Pick out the appropriate elements from the GameObjects that were grabbed
		TMP_Text title = titleObj.GetComponent<TMP_Text>();
		TMP_Text subtitle = subtitleObj.GetComponent<TMP_Text>();
		TMP_Text body = bodyObj.GetComponent<TMP_Text>();
		TMP_Text footer = footerObj.GetComponent<TMP_Text>();
		Image footerBorder = footerBorderObj.GetComponent<Image>();
		
		// Set the TMP subtitle text based on the card object's enum
		if (card.title == CardEnums.Title.MarketMod) {			// Market Mod
			title.text = "\u2013Market Mod\u2013";
		} else if (card.title == CardEnums.Title.PriceCard) {	// Price Card
			title.text = "\u2013Price Card\u2013";
		} else if (card.title == CardEnums.Title.Resource) {	// Resource
			title.text = "\u2013Resource\u2013";
		} else if (card.title == CardEnums.Title.TileMod) {		// Tile Mod
			title.text = "\u2013Tile Mod\u2013";
		} else if (card.title == CardEnums.Title.Forest) {		// Forest Tile
			title.text = "   Forest";
		} else if (card.title == CardEnums.Title.Plains) {		// Plains Tile
			title.text = "   Plains";
		} else if (card.title == CardEnums.Title.Quarry) {		// Quarry Tile
			title.text = "   Quarry";
		}

		// Set the TMP subtitle text based on the card object's enum
		if (card.subtitle == CardEnums.Subtitle.Investment) {		// Investment
			subtitle.text = "Investment";
		} else if (card.subtitle == CardEnums.Subtitle.Sabotage) {	// Sabotage
			subtitle.text = "Sabotage";
		} else if (card.subtitle == CardEnums.Subtitle.Resource) {	// Resource
			subtitle.text = "Resource";
		} else if (card.subtitle == CardEnums.Subtitle.Lumber) {	// Lumber
			subtitle.text = "Lumber";
		} else if (card.subtitle == CardEnums.Subtitle.Oil) {		// Oil
			subtitle.text = "Oil";
		} else if (card.subtitle == CardEnums.Subtitle.CashCrops) {	// Cash Crops
			subtitle.text = "Cash Crops";
		} else if (card.subtitle == CardEnums.Subtitle.Iron) {		// Iron
			subtitle.text = "Iron";
		} else if (card.subtitle == CardEnums.Subtitle.Silver) {	// Silver
			subtitle.text = "Silver";
		} else if (card.subtitle == CardEnums.Subtitle.Gold) {		// Gold
			subtitle.text = "Gold";
		} else if (card.subtitle == CardEnums.Subtitle.Gems) {		// Gems
			subtitle.text = "Gems";
		} else if (card.subtitle == CardEnums.Subtitle.Platinum) {	// Platinum
			subtitle.text = "Platinum";
		} else if (card.subtitle == CardEnums.Subtitle.None) {		// None
			subtitle.text = "";
		}

		// Color the footer border
		if (card.FooterColor == CardEnums.FooterColor.Black) { 			// Black
			footerBorder.color = ColorPalette.inkBlack;
		} else if (card.FooterColor == CardEnums.FooterColor.Red) {		// Red
			footerBorder.color = ColorPalette.inkRed;
		} else if (card.FooterColor == CardEnums.FooterColor.Green) {	// Green
			footerBorder.color = ColorPalette.inkGreen;
		} else if (card.FooterColor == CardEnums.FooterColor.Cyan) {	// Cyan
			footerBorder.color = ColorPalette.inkCyan;
		} else if (card.FooterColor == CardEnums.FooterColor.Yellow) {	// Yellow
			footerBorder.color = ColorPalette.inkYellow;
		} else if (card.FooterColor == CardEnums.FooterColor.Magenta) {	// Magenta
			footerBorder.color = ColorPalette.inkMagenta;
		} else if (card.FooterColor == CardEnums.FooterColor.Blue) {	// Blue
			footerBorder.color = ColorPalette.inkBlue;
		}

		// String members are assigned to the text labels after being formatted
		body.text = mdToTag(card.bodyText);
		footer.text = insertFooterValue(card.footerText, card.percFlag, 
											card.moneyFlag, card.footerOp);
		footer.text = mdToTag(footer.text);
	} // Start()
	


	// Converts a string with bold and italic markdown into html-like tags
	private string mdToTag(string inputText) {

		string outputText = inputText;	// String to output

		//While there's still BOLD markdown left in input string
		while (outputText.IndexOf("**") >= 0) {
			int index = outputText.IndexOf("**");								// Set known index
			outputText = outputText.Remove(startIndex: index, count: 2);		// Remove markdown
			outputText = outputText.Insert(startIndex: index, value: "<b>");	// Insert start tag

			//Making sure there's a place to insert an end tag
			if (outputText.IndexOf("**") >= 0) {
				index = outputText.IndexOf("**");								// Reset the index
				outputText = outputText.Remove(startIndex: index, count: 2);	// Remove markdown
			outputText =  outputText.Insert(startIndex: index, value: "</b>");	// Insert end tag
			} else {
				Debug.Log("Error parsing markdown: No closing statement found!");
			}

		} //while BOLD left

		//While there's still ITALIC markdown left in input string
		while (outputText.IndexOf('*') >= 0) {
			int index = outputText.IndexOf('*');								// Set known index
			outputText = outputText.Remove(startIndex: index, count: 1);		// Remove markdown
			outputText = outputText.Insert(startIndex: index, value: "<i>");	// Insert start tag

			//Making sure there's a place to insert an end tag
			if (outputText.IndexOf('*') >= 0) {
				index = outputText.IndexOf('*');								// Reset the index
				outputText = outputText.Remove(startIndex: index, count: 1);	// Remove markdown
			outputText =  outputText.Insert(startIndex: index, value: "</i>");	// Insert end tag
			} else {
				Debug.Log("Error parsing markdown: No closing statement found!");
			}

		} //while ITALIC left

		return outputText;

	} // mdToTag()

	// Inserts the footerValue into a string meant for the footer text
	private string insertFooterValue(string inputText, bool percFlag, 
									 bool moneyFlag, CardEnums.FooterOp op) {

		string outputText = inputText;								// String to output
		string footerValueStr = card.footerValue.ToString("n0");	// The formatted footer value

		// While there's still ITALIC markdown left in input string
		while (outputText.IndexOf("<x>") >= 0) {
			// Set known index
			int index = outputText.IndexOf("<x>");								
			// Remove markdown
			outputText = outputText.Remove(startIndex: index, count: 3);

			// If the value is a percentage, add a %
			if (percFlag) {
				footerValueStr = (footerValueStr + "%");
			}
			// If the value is a percentage, add a $
			if (moneyFlag) {
				footerValueStr = ("$" + footerValueStr);
			}

			// Add the appropriate operator to the string
			if (op == CardEnums.FooterOp.Add) {
				footerValueStr = ("+" + footerValueStr);
			} else if (op == CardEnums.FooterOp.Sub) {
				footerValueStr = ("\u2013" + footerValueStr);
			}
		
			// Insert start tag
			outputText = outputText.Insert(startIndex: index, value: footerValueStr);	

		} // while ITALIC left

		return outputText;

	} // insertFooterValue()

} // CardDisplay class

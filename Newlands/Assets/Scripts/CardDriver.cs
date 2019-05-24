// A driver class to test the functionality of the Card object.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDriver : MonoBehaviour {

	// Textmesh Pro text objects
	public TMP_Text title;
	public TMP_Text subtitle;
	public TMP_Text body;
	public TMP_Text footer;
	public Image footerBorder;

	// The local Card scriptable object
	private Card card;
	private MasterDeck masterDeck;

	float rot = -90f;
	float timer = 0f;
	int displayIndex = 0;

	void Start() {;

		masterDeck = new MasterDeck(CardEnums.Decks.VanillaStandard);
		card = masterDeck.priceCardDeck[0];

	} // Start()
	
	void Update() {

		// Simple rotation code, used for testings/looking friggin sweet
		rot += Time.deltaTime * 50;
		timer += Time.deltaTime;

		// Alternate PingPong code
		rot = 0;
		rot = Mathf.PingPong(Time.time * 12.5f, 30f);
		//Debug.Log(rot);
		if (rot >= 360f) {
			rot = 0f;
		}

		// Cycles though each card of the tileDeck
		card = masterDeck.priceCardDeck[displayIndex];
		if (timer >= 2f) {		// Seconds
			displayIndex++;

			if (displayIndex >= masterDeck.priceCardDeck.Count()) {
				displayIndex = 0;
			}
			timer = 0;
		}

		Quaternion newRotation = Quaternion.Euler(30, rot, -15);
		transform.rotation = newRotation;


		// Set the TMP subtitle text based on the card object's enum
		if (card.title == CardEnums.Title.MarketMod) {			// Market Mod
			title.text = "\u2013Market Mod\u2013";
		} else if (card.title == CardEnums.Title.PriceCard) {	// Price Card
			title.text = "\u2013Price Card\u2013";
		} else if (card.title == CardEnums.Title.Resource) {	// Resource
			title.text = "\u2013Resource\u2013";
		} else if (card.title == CardEnums.Title.TileMod) {		// Tile Mod
			title.text = "\u2013Tile Mod\u2013";
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

	} // Update()

	// Converts a string with bold and italic markdown into html-like tags
	private string mdToTag(string inputText) {

		string outputText = inputText;

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
	}

} // TileGameObject()

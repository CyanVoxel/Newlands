// This class acts as a bridge between the internal Card scriptable object data
// and the display components of the Card prefab.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour {

	// DATA FIELDS ------------------------------------------------------------

	// // Textmesh Pro text objects --------------------------
	// private TMP_Text title;
	// private TMP_Text subtitle;
	// private TMP_Text body;
	// private TMP_Text footer;
	// private Image footerBorder;

	// // GameObject containers ------------------------------
	// private GameObject titleObj;
	// private GameObject subtitleObj;
	// private GameObject bodyObj;
	// private GameObject footerObj;
	// private GameObject footerBorderObj;

	// Long directories stored as strings
	private string dirFtrBdr = "Front Canvas/Footer Mask/Footer Border Mask/Footer Border";
	private string dirFtrBdrL = "Front Canvas/Footer Mask Left/Footer Border Mask/Footer Border";
	private string dirFtrBdrR = "Front Canvas/Footer Mask Right/Footer Border Mask/Footer Border";

	// The local Card scriptable object
	// private Card card;

	// An object reference of the GameManager, used for method access
	//private GameManager gameMan;

	void Start() {

		//  card = GameManager.DrawCard(GameManager.masterDeckMutable.landTileDeck, GameManager.masterDeck.landTileDeck);
		//  DisplayCard(card);

		// TODO: Move display code to a standalone function that takes in a Card object
		//	and a card prefab.

		
	} // Start()
	


	// Converts a string with bold and italic markdown into html-like tags
	private string MdToTag(string inputText) {

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
	private string InsertFooterValue(Card card, string inputText, bool percFlag, 
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

	// Displays card scriptable object data onto a card prefab
	public void DisplayCard(Card card) {

		// Grab the display elements from this parent object
		GameObject titleObj = transform.Find("Front Canvas/Title").gameObject;
		
		GameObject bodyObj = transform.Find("Front Canvas/Body").gameObject;
		GameObject footerObj = transform.Find("Front Canvas/Footer").gameObject;
		GameObject footerBorderObj = transform.Find(dirFtrBdr).gameObject;

		// Pick out the appropriate elements from the GameObjects that were grabbed
		TMP_Text title = titleObj.GetComponent<TMP_Text>();
		
		TMP_Text body = bodyObj.GetComponent<TMP_Text>();
		TMP_Text footer = footerObj.GetComponent<TMP_Text>();
		Image footerBorder = footerBorderObj.GetComponent<Image>();

		// GAMECARD SPECIFICS -------------------------------------------------
		if (card.category == CardEnums.Category.GameCard) {

			GameObject subtitleObj = transform.Find("Front Canvas/Subtitle").gameObject;
			TMP_Text subtitle = subtitleObj.GetComponent<TMP_Text>();

			GameObject footerBorderObjL = transform.Find(dirFtrBdrL).gameObject;
			GameObject footerBorderObjR = transform.Find(dirFtrBdrR).gameObject;
			Image footerBorderL = footerBorderObjL.GetComponent<Image>();
			Image footerBorderR = footerBorderObjR.GetComponent<Image>();

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
			} // if-else

			// Color the footer border
			if (card.title == CardEnums.Title.MarketMod) {

				footerBorder.color = ColorPalette.inkBlack;

				if (card.FooterColor == CardEnums.FooterColor.Red) { 			// Red
					footerBorderL.color = ColorPalette.inkRed;
					footerBorderR.color = ColorPalette.inkRed;
				} else if (card.FooterColor == CardEnums.FooterColor.Green) { 	// Green
					footerBorderL.color = ColorPalette.inkGreen;
					footerBorderR.color = ColorPalette.inkGreen;
				}

			} else {

				footerBorderL.color = ColorPalette.alpha;
				footerBorderR.color = ColorPalette.alpha;

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
				} // if-else

			} // if Market Mod else

		} // GameCard specifics

		// LANDTILE SPECIFICS -------------------------------------------------
		if (card.category == CardEnums.Category.LandTile) {

			GameObject titleIconObj = transform.Find("Front Canvas/Icon").gameObject;
			Image iconImage = titleIconObj.GetComponent<Image>();

			if (card.title == CardEnums.Title.Forest) {
				iconImage.sprite = Resources.Load<Sprite>("Sprites/icon_forest");	// Forest
			} else if (card.title == CardEnums.Title.Plains) {
				iconImage.sprite = Resources.Load<Sprite>("Sprites/icon_plains");	// Plains
			} else if (card.title == CardEnums.Title.Quarry) {
				iconImage.sprite = Resources.Load<Sprite>("Sprites/icon_quarry");	// Quarry
			} // else-if

		} // LandTile specifics
		
		// Set the TMP title text based on the card object's enum
		if (card.title == CardEnums.Title.MarketMod) {			// Market Mod
			title.text = "\u2013Market Mod\u2013";
		} else if (card.title == CardEnums.Title.PriceCard) {	// Price Card
			title.text = "\u2013Price Card\u2013";
		} else if (card.title == CardEnums.Title.Resource) {	// Resource
			title.text = "\u2013Resource\u2013";
		} else if (card.title == CardEnums.Title.TileMod) {		// Tile Mod
			title.text = "\u2013Tile Mod\u2013";
		} else if (card.title == CardEnums.Title.Forest) {		// Forest Tile
			title.text = " Forest";
		} else if (card.title == CardEnums.Title.Plains) {		// Plains Tile
			title.text = " Plains";
		} else if (card.title == CardEnums.Title.Quarry) {		// Quarry Tile
			title.text = "    Quarry";
		} // if-else

		// String members are assigned to the text labels after being formatted
		body.text = MdToTag(card.bodyText);
		footer.text = InsertFooterValue(card, card.footerText, card.percFlag, 
											card.moneyFlag, card.footerOp);
		footer.text = MdToTag(footer.text);

	} // displayCard()

} // CardDisplay class

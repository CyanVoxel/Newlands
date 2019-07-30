// This class acts as a bridge between the internal Card scriptable object data
// and the display components of the Card prefab.

using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : NetworkBehaviour {

	// DATA FIELDS -------------------------------------------------------------

	// Long directories stored as strings
	private string dirFtrBdr = "Front Canvas/Footer Mask/Footer Border Mask/Footer Border";
	private string dirFtrBdrL = "Front Canvas/Footer Mask Left/Footer Border Mask/Footer Border";
	private string dirFtrBdrR = "Front Canvas/Footer Mask Right/Footer Border Mask/Footer Border";

	// Converts a string with bold and italic markdown into html-like tags
	private string MdToTag(string inputText) {

		string outputText = inputText; // String to output

		// While there's still BOLD markdown left in input string
		while (outputText.IndexOf("**") >= 0) {
			int index = outputText.IndexOf("**"); // Set known index
			outputText = outputText.Remove(startIndex: index, count: 2); // Remove markdown
			outputText = outputText.Insert(startIndex: index, value: "<b>"); // Insert start tag

			// Making sure there's a place to insert an end tag
			if (outputText.IndexOf("**") >= 0) {
				index = outputText.IndexOf("**"); // Reset the index
				outputText = outputText.Remove(startIndex: index, count: 2); // Remove markdown
				outputText = outputText.Insert(startIndex: index, value: "</b>"); // Insert end tag
			} else {
				Debug.Log("Error parsing markdown: No closing statement found!");
			}

		} // while BOLD left

		// While there's still ITALIC markdown left in input string
		while (outputText.IndexOf('*') >= 0) {
			int index = outputText.IndexOf('*'); // Set known index
			outputText = outputText.Remove(startIndex: index, count: 1); // Remove markdown
			outputText = outputText.Insert(startIndex: index, value: "<i>"); // Insert start tag

			// Making sure there's a place to insert an end tag
			if (outputText.IndexOf('*') >= 0) {
				index = outputText.IndexOf('*'); // Reset the index
				outputText = outputText.Remove(startIndex: index, count: 1); // Remove markdown
				outputText = outputText.Insert(startIndex: index, value: "</i>"); // Insert end tag
			} else {
				Debug.Log("Error parsing markdown: No closing statement found!");
			}

		} // while ITALIC left

		return outputText;

	} // mdToTag()

	// Converts a custom tag to Card Data
	// TODO: Expand the parser to dynamically generate most of the needed body text for cards,
	//	including frequent phrases and dynamically generated scope info text.
	private string TagToCardData(string inputText, Card card) {

		string outputText = inputText; // String to output

		// Processes an <r> tag
		while (outputText.IndexOf("<r>") >= 0) {
			int index = outputText.IndexOf("<r>"); // Set known index
			outputText = outputText.Remove(startIndex: index, count: 3); // Remove tag
			outputText = outputText.Insert(startIndex: index, value: card.resource);
		} // while <r> left

		// Processes an <c> tag
		while (outputText.IndexOf("<c>") >= 0) {
			int index = outputText.IndexOf("<c>"); // Set known index
			outputText = outputText.Remove(startIndex: index, count: 3); // Remove tag
			outputText = outputText.Insert(startIndex: index, value: card.category);
		} // while <c> left

		// Processes an <ts> tag
		while (outputText.IndexOf("<ts>") >= 0) {
			int index = outputText.IndexOf("<ts>"); // Set known index
			outputText = outputText.Remove(startIndex: index, count: 4); // Remove tag
			outputText = outputText.Insert(startIndex: index, value: card.target);
		} // while <ts> left

		// Processes an <tc> tag
		// while (outputText.IndexOf("<tc>") >= 0) {
		// 	int index = outputText.IndexOf("<tc>");								// Set known index
		// 	outputText = outputText.Remove(startIndex: index, count: 4);		// Remove tag
		// 	outputText = outputText.Insert(startIndex: index, value: card.targetCategory);
		// } // while <tc> left

		return outputText;

	} // TagToCardData(string inputText, Card card)

	// Inserts the footerValue into a string meant for the footer text
	private string InsertFooterValue(Card card, string inputText, bool percFlag,
		bool moneyFlag, char op) {

		string outputText = inputText; // String to output
		string footerValueStr = card.footerValue.ToString("n0"); // The formatted footer value

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
			if (op == '+') {
				footerValueStr = ("+" + footerValueStr);
			} else if (op == '-') {
				footerValueStr = ("\u2013" + footerValueStr);
			}

			// Insert start tag
			outputText = outputText.Insert(startIndex: index, value: footerValueStr);

		} // while ITALIC left

		return outputText;

	} // insertFooterValue()

	public void DisplayTitle(GameObject cardParent) {

		CardState card = cardParent.GetComponent<CardState>();

		GameObject titleObj = this.transform.Find("Front Canvas/Title").gameObject;
		TMP_Text title = titleObj.GetComponent<TMP_Text>();

		// GAMECARD SPECIFICS -------------------------------------------------
		if (card.category == "Game Card" || card.category == "Market") {

		}

		// TILE SPECIFICS -----------------------------------------------------
		if (card.category == "Tile") {

			GameObject titleIconObj = this.transform.Find("Front Canvas/Icon").gameObject;
			Image iconImage = titleIconObj.GetComponent<Image>();

			// Set the Title Text field
			// TODO: Optimize this under the new string system, as well as when Land Tile titles
			//	are finally auto-centered with along with their icons.
			switch (card.title) {
				case "Forest":
					title.text = " Forest";
					break;
				case "Plains":
					title.text = " Plains";
					break;
				case "Quarry":
					title.text = "    Quarry";
					break;
				case "Farmland":
					title.text = "       Farmland";
					break;
				default:
					break;
			}

			// Try to load the icon sprite
			if (iconImage.sprite = Resources.Load<Sprite>("Sprites/icon_" + card.title.ToLower())) {
				Debug.Log("[CardDisplay] Successfully loaded image sprite \""
					+ "Sprites/icon_"
					+ card.title.ToLower() + "\"");
			} else {
				Debug.LogError("[CardDisplay] ERROR: Could not load image sprite \""
					+ "Sprites/icon_"
					+ card.title.ToLower() + "\"");
			}

		} // Tile specifics

	} // DisplayTitle()

	// Displays card scriptable object data onto a card prefab
	// NOTE: This method is DEPRECATED! Use the new specific methods such as DisplayTitle()!
	public void DisplayCard(GameObject obj, Card card) {
		// Debug.Log("[CardDisplay] Trying to display the card "
		// 	+ card.title
		// 	+ " on object "
		// 	+ obj);

		// Grab the display elements from this parent object
		GameObject titleObj = obj.transform.Find("Front Canvas/Title").gameObject;

		GameObject bodyObj = obj.transform.Find("Front Canvas/Body").gameObject;
		GameObject footerObj = obj.transform.Find("Front Canvas/Footer").gameObject;
		GameObject footerBorderObj = obj.transform.Find(dirFtrBdr).gameObject;

		// Pick out the appropriate elements from the GameObjects that were grabbed
		TMP_Text title = titleObj.GetComponent<TMP_Text>();

		TMP_Text body = bodyObj.GetComponent<TMP_Text>();
		TMP_Text footer = footerObj.GetComponent<TMP_Text>();
		Image footerBorder = footerBorderObj.GetComponent<Image>();

		// GAMECARD SPECIFICS -------------------------------------------------
		if (card.category == "Game Card" || card.category == "Market") {

			GameObject subtitleObj = obj.transform.Find("Front Canvas/Subtitle").gameObject;
			TMP_Text subtitle = subtitleObj.GetComponent<TMP_Text>();

			GameObject footerBorderObjL = obj.transform.Find(dirFtrBdrL).gameObject;
			GameObject footerBorderObjR = obj.transform.Find(dirFtrBdrR).gameObject;
			Image footerBorderL = footerBorderObjL.GetComponent<Image>();
			Image footerBorderR = footerBorderObjR.GetComponent<Image>();

			// Set the TMP subtitle text based on the card object's enum
			subtitle.text = card.subtitle;

			// Color the footer border
			if (card.title == "Market Mod") {

				footerBorder.color = ColorPalette.cardDark;

				if (card.FooterColor == "Red") { // Red
					footerBorderL.color = ColorPalette.Red500;
					footerBorderR.color = ColorPalette.Red500;
				} else if (card.FooterColor == "Green") { // Green
					footerBorderL.color = ColorPalette.Green500;
					footerBorderR.color = ColorPalette.Green500;
				}

			} else {

				footerBorderL.color = ColorPalette.alpha;
				footerBorderR.color = ColorPalette.alpha;

				// Color the footer border
				if (card.FooterColor == "Black") { // Black
					footerBorder.color = ColorPalette.cardDark;
				} else if (card.FooterColor == "Red") { // Red
					footerBorder.color = ColorPalette.Red500;
				} else if (card.FooterColor == "Green") { // Green
					footerBorder.color = ColorPalette.Green500;
				} else if (card.FooterColor == "Light Blue") { // Light Blue
					footerBorder.color = ColorPalette.LightBlue500;
				} else if (card.FooterColor == "Yellow") { // Yellow
					footerBorder.color = ColorPalette.Yellow500;
				} else if (card.FooterColor == "Pink") { // Pink
					footerBorder.color = ColorPalette.Pink500;
				} else if (card.FooterColor == "Blue") { // Blue
					footerBorder.color = ColorPalette.LightBlue500;
				} else if (card.FooterColor == "Dark Blue") { // Dark Blue
					footerBorder.color = ColorPalette.Indigo500;
				} // if-else

			} // if Market Mod else

			subtitle.text = TagToCardData(MdToTag(subtitle.text), card);

		} // GameCard specifics

		if (card.category == "Market") {

			title.text = "\u2013Market Card\u2013";

			ResourceInfo.prices.TryGetValue(card.resource, out card.footerValue);
			footer.text = TagToCardData(MdToTag(footer.text), card);
		}

		// TILE SPECIFICS -----------------------------------------------------
		if (card.category == "Tile") {

			GameObject titleIconObj = obj.transform.Find("Front Canvas/Icon").gameObject;
			Image iconImage = titleIconObj.GetComponent<Image>();

			if (card.title == "Forest") {
				iconImage.sprite = Resources.Load<Sprite>("Sprites/icon_forest"); // Forest
			} else if (card.title == "Plains") {
				iconImage.sprite = Resources.Load<Sprite>("Sprites/icon_plains"); // Plains
			} else if (card.title == "Quarry") {
				iconImage.sprite = Resources.Load<Sprite>("Sprites/icon_quarry"); // Quarry
			} else if (card.title == "Farmland") {
				iconImage.sprite = Resources.Load<Sprite>("Sprites/icon_farmland"); // Farmland
			} // else-if

			// Color the footer border
			if (card.FooterColor == "Black") { // Black
				footerBorder.color = ColorPalette.cardDark;
			} else if (card.FooterColor == "Red") { // Red
				footerBorder.color = ColorPalette.Red500;
			} else if (card.FooterColor == "Green") { // Green
				footerBorder.color = ColorPalette.Green500;
			} else if (card.FooterColor == "Light Blue") { // Light Blue
				footerBorder.color = ColorPalette.LightBlue500;
			} else if (card.FooterColor == "Yellow") { // Yellow
				footerBorder.color = ColorPalette.Yellow500;
			} else if (card.FooterColor == "Pink") { // Pink
				footerBorder.color = ColorPalette.Pink500;
			} else if (card.FooterColor == "Blue") { // Blue
				footerBorder.color = ColorPalette.LightBlue500;
			} else if (card.FooterColor == "Dark Blue") { // Dark Blue
				footerBorder.color = ColorPalette.Indigo500;
			} // if-else

		} // LandTile specifics

		// Set the TMP title text.
		// TODO: Optimize this under the new string system, as well as when Land Tile titles
		//	are finally auto-centered with along with their icons.
		if (card.title == "Market Mod") { // Market Mod
			title.text = "\u2013Market Mod\u2013";
		} else if (card.title == "Market Card") { // Price Card
			title.text = "\u2013Market Card\u2013";
		} else if (card.title == "Resource") { // Resource
			title.text = "\u2013Resource\u2013";
		} else if (card.title == "Tile Mod") { // Tile Mod
			title.text = "\u2013Tile Mod\u2013";
		} else if (card.title == "Forest") { // Forest Tile
			title.text = " Forest";
		} else if (card.title == "Plains") { // Plains Tile
			title.text = " Plains";
		} else if (card.title == "Quarry") { // Quarry Tile
			title.text = "    Quarry";
		} else if (card.title == "Farmland") { // Farmland Tile
			title.text = "       Farmland";
		} // if-else

		// String members are assigned to the text labels after being formatted
		body.text = TagToCardData(MdToTag(card.bodyText), card);
		footer.text = InsertFooterValue(card, card.footerText, card.percFlag,
			card.moneyFlag, card.footerOpr);

		body.text = MdToTag(body.text);
		footer.text = TagToCardData(MdToTag(footer.text), card);

	} // displayCard()

	// Performas visual updates to a bankrupt card
	public static void BankruptVisuals(GameObject tile) {

		tile.GetComponentsInChildren<Renderer>() [0].material.color = ColorPalette.cardDark;
		tile.GetComponentsInChildren<Renderer>() [1].material.color = ColorPalette.cardDark;

		GameObject titleObj = tile.transform.Find("Front Canvas/Title").gameObject;
		TMP_Text title = titleObj.GetComponent<TMP_Text>();
		title.color = ColorPalette.Red500;

		GameObject titleIconObj = tile.transform.Find("Front Canvas/Icon").gameObject;
		Image iconImage = titleIconObj.GetComponent<Image>();
		iconImage.color = ColorPalette.Red500;

		// This picks up the title text for some reason?
		// GameObject footerObj = tile.transform.Find("Front Canvas/Footer").gameObject;
		// TMP_Text footer = titleObj.GetComponent<TMP_Text>();
		// footer.color = ColorPalette.red500;
		// footer.text = "";

	} //BankruptVisuals()

	public void UpdateFooter(GridUnit unit, double value) {

		GameObject footerObj = transform.Find("Front Canvas/Footer").gameObject;
		TMP_Text footer = footerObj.GetComponent<TMP_Text>();

		ResourceInfo.pricesMut.TryGetValue(unit.card.resource, out unit.card.footerValue);
		footer.text = InsertFooterValue(unit.card, unit.card.footerText, unit.card.percFlag,
			unit.card.moneyFlag, unit.card.footerOpr);
		footer.text = TagToCardData(MdToTag(footer.text), unit.card);
		GameManager.UpdatePlayersInfo();
		// GuiManager.CmdUpdateUI();

	} //UpdateFooter()

} // CardDisplay class

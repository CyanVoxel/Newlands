// This class acts as a bridge between the internal Card scriptable object data
// and the display components of the Card prefab.

using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : NetworkBehaviour
{
	// DATA FIELDS -------------------------------------------------------------
	private static DebugTag debug = new DebugTag("CardDisplay", "00BCD4");

	// Long directories stored as strings
	private string dirFtrBdr = "Front Canvas/Footer Mask/Footer Border Mask/Footer Border";
	private string dirFtrBdrL = "Front Canvas/Footer Mask Left/Footer Border Mask/Footer Border";
	private string dirFtrBdrR = "Front Canvas/Footer Mask Right/Footer Border Mask/Footer Border";

	// Converts a string with bold and italic markdown into html-like tags
	private string MdToTag(string inputText)
	{
		string outputText = inputText; // String to output

		// While there's still BOLD markdown left in input string
		while (outputText.IndexOf("**") >= 0)
		{
			int index = outputText.IndexOf("**"); // Set known index
			outputText = outputText.Remove(startIndex: index, count: 2); // Remove markdown
			outputText = outputText.Insert(startIndex: index, value: "<b>"); // Insert start tag

			// Making sure there's a place to insert an end tag
			if (outputText.IndexOf("**") >= 0)
			{
				index = outputText.IndexOf("**"); // Reset the index
				outputText = outputText.Remove(startIndex: index, count: 2); // Remove markdown
				outputText = outputText.Insert(startIndex: index, value: "</b>"); // Insert end tag
			}
			else
			{
				Debug.Log("Error parsing markdown: No closing statement found!");
			}
		} // while BOLD left

		// While there's still ITALIC markdown left in input string
		while (outputText.IndexOf('*') >= 0)
		{
			int index = outputText.IndexOf('*'); // Set known index
			outputText = outputText.Remove(startIndex: index, count: 1); // Remove markdown
			outputText = outputText.Insert(startIndex: index, value: "<i>"); // Insert start tag

			// Making sure there's a place to insert an end tag
			if (outputText.IndexOf('*') >= 0)
			{
				index = outputText.IndexOf('*'); // Reset the index
				outputText = outputText.Remove(startIndex: index, count: 1); // Remove markdown
				outputText = outputText.Insert(startIndex: index, value: "</i>"); // Insert end tag
			}
			else
			{
				Debug.Log("Error parsing markdown: No closing statement found!");
			}

		} // while ITALIC left

		return outputText;
	} // mdToTag()

	// Converts a custom tag to Card Data
	// TODO: Expand the parser to dynamically generate most of the needed body text for cards,
	//	including frequent phrases and dynamically generated scope info text.
	private string TagToCardData(string inputText, CardState cardState)
	{
		string outputText = inputText; // String to output

		// Processes an <r> tag
		while (outputText.IndexOf("<r>") >= 0)
		{
			int index = outputText.IndexOf("<r>"); // Set known index
			outputText = outputText.Remove(startIndex: index, count: 3); // Remove tag
			outputText = outputText.Insert(startIndex: index, value: cardState.resource);
		} // while <r> left

		// Processes an <c> tag
		while (outputText.IndexOf("<c>") >= 0)
		{
			int index = outputText.IndexOf("<c>"); // Set known index
			outputText = outputText.Remove(startIndex: index, count: 3); // Remove tag
			outputText = outputText.Insert(startIndex: index, value: cardState.category);
		} // while <c> left

		// Processes an <ts> tag
		while (outputText.IndexOf("<ts>") >= 0)
		{
			int index = outputText.IndexOf("<ts>"); // Set known index
			outputText = outputText.Remove(startIndex: index, count: 4); // Remove tag
			outputText = outputText.Insert(startIndex: index, value: cardState.target);
		} // while <ts> left

		// // Processes an <tc> tag
		// while (outputText.IndexOf("<tc>") >= 0)
		// {
		// 	int index = outputText.IndexOf("<tc>");                             // Set known index
		// 	outputText = outputText.Remove(startIndex: index, count: 4);        // Remove tag
		// 	outputText = outputText.Insert(startIndex: index, value: card.targetCategory);
		// } // while <tc> left

		return outputText;
	} // TagToCardData(string inputText, Card card)

	// Inserts the footerValue into a string meant for the footer text
	private string InsertFooterValue(CardState cardState, string inputText)
	{
		string outputText = inputText; // String to output
		string footerValueStr = cardState.footerValue.ToString("n0"); // The formatted footer value

		// While there's still ITALIC markdown left in input string
		while (outputText.IndexOf("<x>") >= 0)
		{
			// Set known index
			int index = outputText.IndexOf("<x>");
			// Remove markdown
			outputText = outputText.Remove(startIndex: index, count: 3);

			// If the value is a percentage, add a %
			if (cardState.percFlag)
			{
				footerValueStr = (footerValueStr + "%");
			}
			// If the value is a percentage, add a $
			if (cardState.moneyFlag)
			{
				footerValueStr = ("$" + footerValueStr);
			}

			// Add the appropriate operator to the string
			if (cardState.footerOpr == '+')
			{
				footerValueStr = ("+" + footerValueStr);
			}
			else if (cardState.footerOpr == '-')
			{
				footerValueStr = ("\u2013" + footerValueStr);
			}

			// Insert start tag
			outputText = outputText.Insert(startIndex: index, value: footerValueStr);

		} // while ITALIC left

		return outputText;

	} // insertFooterValue()

	// Formats card elements based on a new Title
	// TODO: Probably want to break this up into smaller private methods
	public void DisplayTitle(GameObject cardObj)
	{
		CardState cardState = cardObj.GetComponent<CardState>();
		GameObject titleObj = this.transform.Find("Front Canvas/Title").gameObject;
		TMP_Text title = titleObj.GetComponent<TMP_Text>();

		GameObject footerBorderObj = this.transform.Find(dirFtrBdr).gameObject;
		Image footerBorder = footerBorderObj.GetComponent<Image>();

		GameObject footerBorderObjL;
		Image footerBorderL;
		GameObject footerBorderObjR;
		Image footerBorderR;

		// Debug.Log(debug.head + "Category: " + cardState.category);
		switch (cardState.category)
		{
			case "Tile":
				// Title Text --------------------------------------------------
				// TODO: Create a dynamic text-centering system based on the physical size of
				// the TMP asset plus the icon width.
				switch (cardState.title)
				{
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
						title.text = cardState.title;
						break;
				}
				// Icon --------------------------------------------------------
				GameObject titleIconObj = this.transform.Find("Front Canvas/Icon").gameObject;
				Image iconImage = titleIconObj.GetComponent<Image>();
				if (iconImage.sprite = Resources.Load<Sprite>("Sprites/icon_" + cardState.title.ToLower()))
				{
					// Debug.Log("[CardDisplay] Successfully loaded image sprite \""
					// 	+ "Sprites/icon_"
					// 	+ cardState.title.ToLower() + "\"");
				}
				else
				{
					Debug.LogError("[CardDisplay] ERROR: Could not load image sprite \""
						+ "Sprites/icon_"
						+ cardState.title.ToLower() + "\"");
				} // if icon could be loaded
				break;

			case "Game Card":
				// Title Text --------------------------------------------------
				switch (cardState.title)
				{
					case "Market Mod":
						title.text = "\u2013Market Mod\u2013";
						break;
					case "Resource":
						title.text = "\u2013Resource\u2013";
						break;
					case "Tile Mod":
						title.text = "\u2013Tile Mod\u2013";
						break;
					default:
						title.text = cardState.title;
						break;
				} // switch (cardState.title)
				// Footer Border -----------------------------------------------
				footerBorderObjL = this.transform.Find(dirFtrBdrL).gameObject;
				footerBorderL = footerBorderObjL.GetComponent<Image>();
				footerBorderObjR = this.transform.Find(dirFtrBdrR).gameObject;
				footerBorderR = footerBorderObjR.GetComponent<Image>();
				footerBorderL.color = ColorPalette.alpha;
				footerBorderR.color = ColorPalette.alpha;

				switch (cardState.footerColor)
				{
					case "Black":
						if (cardState.onlyColorCorners)
						{
							footerBorderL.color = ColorPalette.cardDark;
							footerBorderR.color = ColorPalette.cardDark;
						}
						else
						{
							footerBorder.color = ColorPalette.cardDark;
						}
						break;
					case "Red":
						if (cardState.onlyColorCorners)
						{
							footerBorderL.color = ColorPalette.red500;
							footerBorderR.color = ColorPalette.red500;
						}
						else
						{
							footerBorder.color = ColorPalette.red500;
						}
						break;
					case "Green":
						if (cardState.onlyColorCorners)
						{
							footerBorderL.color = ColorPalette.green500;
							footerBorderR.color = ColorPalette.green500;
						}
						else
						{
							footerBorder.color = ColorPalette.green500;
						}
						break;
					case "Light Blue":
						if (cardState.onlyColorCorners)
						{
							footerBorderL.color = ColorPalette.lightBlue500;
							footerBorderR.color = ColorPalette.lightBlue500;
						}
						else
						{
							footerBorder.color = ColorPalette.lightBlue500;
						}
						break;
					case "Yellow":
						if (cardState.onlyColorCorners)
						{
							footerBorderL.color = ColorPalette.yellow500;
							footerBorderR.color = ColorPalette.yellow500;
						}
						else
						{
							footerBorder.color = ColorPalette.yellow500;
						}
						break;
					case "Pink":
						if (cardState.onlyColorCorners)
						{
							footerBorderL.color = ColorPalette.pink500;
							footerBorderR.color = ColorPalette.pink500;
						}
						else
						{
							footerBorder.color = ColorPalette.pink500;
						}
						break;
					case "Blue":
						if (cardState.onlyColorCorners)
						{
							footerBorderL.color = ColorPalette.lightBlue500;
							footerBorderR.color = ColorPalette.lightBlue500;
						}
						else
						{
							footerBorder.color = ColorPalette.lightBlue500;
						}
						break;
					case "Dark Blue":
						if (cardState.onlyColorCorners)
						{
							footerBorderL.color = ColorPalette.indigo500;
							footerBorderR.color = ColorPalette.indigo500;
						}
						else
						{
							footerBorder.color = ColorPalette.indigo500;
						}
						break;
					case "":
						if (cardState.onlyColorCorners)
						{
							footerBorderL.color = Color.magenta;
							footerBorderR.color = Color.magenta;
						}
						else
						{
							footerBorder.color = Color.magenta;
						}
						break;
					default:
						footerBorder.color = ColorPalette.cardDark;
						break;
				} // (cardState.footerColor)
				break;

			case "Market":
				// Title Text --------------------------------------------------
				title.text = "\u2013Market Card\u2013";
				// Footer Value ------------------------------------------------
				ResourceInfo.prices.TryGetValue(cardState.resource, out cardState.footerValue);
				// Footer Border -----------------------------------------------
				footerBorder.color = ColorPalette.cardDark;
				break;

			default:
				title.text = cardState.title;
				break;
		} // switch (cardState.category)

	} // DisplayTitle()

	public void DisplaySubtitle(GameObject cardObj)
	{
		CardState cardState = cardObj.GetComponent<CardState>();

		if (cardState.category != "Tile")
		{
			GameObject subtitleObj = this.transform.Find("Front Canvas/Subtitle").gameObject;
			TMP_Text subtitle = subtitleObj.GetComponent<TMP_Text>();
			subtitle.text = TagToCardData(MdToTag(cardState.subtitle), cardState);
		}
	} // DisplaySubtitle()

	public void DisplayBody(GameObject cardObj)
	{
		CardState cardState = cardObj.GetComponent<CardState>();
		GameObject bodyObj = this.transform.Find("Front Canvas/Body").gameObject;
		TMP_Text body = bodyObj.GetComponent<TMP_Text>();

		body.text = TagToCardData(MdToTag(cardState.bodyText), cardState);
	} // DisplayBody()

	public void DisplayFooter(GameObject cardObj)
	{
		CardState cardState = cardObj.GetComponent<CardState>();
		GameObject footerObj = this.transform.Find("Front Canvas/Footer").gameObject;
		TMP_Text footer = footerObj.GetComponent<TMP_Text>();

		string tempFooter = cardState.footerText;

		tempFooter = InsertFooterValue(cardState, cardState.footerText);
		footer.text = TagToCardData(MdToTag(tempFooter), cardState);
	} // DisplayFooter()

	// Performas visual updates to a bankrupt card
	public static void BankruptVisuals(GameObject tile)
	{
		tile.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.cardDark;
		tile.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.cardDark;

		GameObject titleObj = tile.transform.Find("Front Canvas/Title").gameObject;
		TMP_Text title = titleObj.GetComponent<TMP_Text>();
		title.color = ColorPalette.red500;

		GameObject titleIconObj = tile.transform.Find("Front Canvas/Icon").gameObject;
		Image iconImage = titleIconObj.GetComponent<Image>();
		iconImage.color = ColorPalette.red500;

		// This picks up the title text for some reason?
		// GameObject footerObj = tile.transform.Find("Front Canvas/Footer").gameObject;
		// TMP_Text footer = titleObj.GetComponent<TMP_Text>();
		// footer.color = ColorPalette.red500;
		// footer.text = "";
	} //BankruptVisuals()

	public void UpdateFooter(GridUnit unit, double value)
	{
		CardState cardState = unit.tileObj.GetComponent<CardState>();
		GameObject footerObj = this.transform.Find("Front Canvas/Footer").gameObject;
		TMP_Text footer = footerObj.GetComponent<TMP_Text>();

		cardState.footerValue = (int)value;
		string tempFooter = cardState.footerText;

		tempFooter = InsertFooterValue(cardState, cardState.footerText);
		footer.text = TagToCardData(MdToTag(tempFooter), cardState);
	} //UpdateFooter()
} // class CardDisplay

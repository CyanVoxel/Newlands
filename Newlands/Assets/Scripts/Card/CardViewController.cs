// Responsible for storing the state values of a Card, as well as updating it's GUI when changed.

using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardViewController : MonoBehaviour
{
	// FIELDS ##########################################################################################################

	// private Animator animator;

	private Image iconImage;
	private TMP_Text titleUiText;
	private TMP_Text subtitleUiText;
	private TMP_Text bodyUiText;
	private TMP_Text footerUiText;
	private Image footerBorder;
	private Image footerBorderL;
	private Image footerBorderR;

	// Long directories stored as strings
	private string dirFtrBdr = "Front Canvas/Footer Mask/Footer Border Mask/Footer Border";
	private string dirFtrBdrL = "Front Canvas/Footer Mask Left/Footer Border Mask/Footer Border";
	private string dirFtrBdrR = "Front Canvas/Footer Mask Right/Footer Border Mask/Footer Border";

	private Card card = new Card();

	private bool initialized = false;

	private static DebugTag debugTag = new DebugTag("CardView", "00BCD4");

	// PROPERTIES ######################################################################################################

	public Card Card
	{
		get
		{
			return card;
		}
		set
		{
			card = value;
			UpdateAll();
		}
	}

	public bool Initialized
	{
		get { return initialized; }
		set
		{
			initialized = value;
			UpdateAll();
		}
	}

	public string Category
	{
		get { return card.Category; }
		set
		{
			card.Category = value;
			if (initialized)
				UpdateFooter();
		}
	}

	public string Title
	{
		get { return card.Title; }
		set
		{
			card.Title = value;
			if (initialized)
				DisplayTitle();
		}
	}

	public string Subtitle
	{
		get { return card.Subtitle; }
		set
		{
			card.Subtitle = value;
			if (initialized)
				DisplaySubtitle();
		}
	}

	public string BodyText
	{
		get { return card.Body; }
		set
		{
			card.Body = value;
			if (initialized)
				DisplayBody();
		}
	}

	public string FooterText
	{
		get { return card.Footer; }
		set
		{
			card.Footer = value;
			if (initialized)
				DisplayFooter();
		}
	}

	public string FooterColor
	{
		get { return card.FooterColor; }
		set
		{
			card.FooterColor = value;
			if (initialized)
				DisplayFooter();
		}
	}

	public string Resource
	{
		get { return card.Resource; }
		set
		{
			card.Resource = value;

		}
	}

	public string Target
	{
		get { return card.Target; }
		set
		{
			card.Target = value;
		}
	}

	public char FooterOpr
	{
		get { return card.FooterOpr; }
		set
		{
			card.FooterOpr = value;
			if (initialized)
				DisplayFooter();
		}
	}

	public int FooterValue
	{
		get { return card.FooterValue; }
		set
		{
			card.FooterValue = value;
			if (initialized)
				DisplayFooter();
		}
	}

	public bool ColorCornerFlag
	{
		get { return card.ColorCornerFlag; }
		set
		{
			card.ColorCornerFlag = value;
			if (initialized)
				DisplayFooter();
		}
	}

	public bool PercFlag
	{
		get { return card.PercFlag; }
		set
		{
			card.PercFlag = value;
			if (initialized)
				DisplayFooter();
		}
	}

	public bool MoneyFlag
	{
		get { return card.MoneyFlag; }
		set
		{
			card.MoneyFlag = value;
			if (initialized)
				DisplayFooter();
		}
	}

	public bool DiscardFlag
	{
		get { return card.DiscardFlag; }
		set
		{
			card.DiscardFlag = value;
		}
	}

	// METHODS #################################################################################

	void Awake()
	{
		if (this.transform.Find("Front Canvas/Icon"))
			this.iconImage = this.transform.Find("Front Canvas/Icon").GetComponent<Image>();

		this.titleUiText = this.transform.Find("Front Canvas/Title").GetComponent<TMP_Text>();
		this.bodyUiText = this.transform.Find("Front Canvas/Body").GetComponent<TMP_Text>();
		this.footerUiText = this.transform.Find("Front Canvas/Footer").GetComponent<TMP_Text>();
		this.footerBorder = this.transform.Find(dirFtrBdr).GetComponent<Image>();

		if (this.transform.Find("Front Canvas/Subtitle") != null)
			this.subtitleUiText = this.transform.Find("Front Canvas/Subtitle").GetComponent<TMP_Text>();

		if (this.transform.Find(dirFtrBdrL) != null)
			this.footerBorderL = this.transform.Find(dirFtrBdrL).GetComponent<Image>();

		if (this.transform.Find(dirFtrBdrR) != null)
			this.footerBorderR = this.transform.Find(dirFtrBdrR).GetComponent<Image>();

		// this.animator = this.GetComponent<Animator>();

		// NOTE: Let MatchController do this after setting values for this card
		this.initialized = true;
		UpdateAll();
	}

	private void UpdateAll()
	{
		DisplayTitle();
		DisplaySubtitle();
		DisplayBody();
		DisplayFooter();
	}

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
	}

	// Converts a custom tag to Card Data
	// TODO: Expand the parser to dynamically generate most of the needed body text for cards,
	//	including frequent phrases and dynamically generated scope info text.
	private string TagToCardData(string inputText)
	{
		string outputText = inputText; // String to output

		// Processes an <r> tag
		while (outputText.IndexOf("<r>") >= 0)
		{
			int index = outputText.IndexOf("<r>"); // Set known index
			outputText = outputText.Remove(startIndex: index, count: 3); // Remove tag
			outputText = outputText.Insert(startIndex: index, value: card.Resource);
		} // while <r> left

		// Processes an <c> tag
		while (outputText.IndexOf("<c>") >= 0)
		{
			int index = outputText.IndexOf("<c>"); // Set known index
			outputText = outputText.Remove(startIndex: index, count: 3); // Remove tag
			outputText = outputText.Insert(startIndex: index, value: card.Category);
		} // while <c> left

		// Processes an <ts> tag
		while (outputText.IndexOf("<ts>") >= 0)
		{
			int index = outputText.IndexOf("<ts>"); // Set known index
			outputText = outputText.Remove(startIndex: index, count: 4); // Remove tag
			outputText = outputText.Insert(startIndex: index, value: card.Target);
		} // while <ts> left

		// // Processes an <tc> tag
		// while (outputText.IndexOf("<tc>") >= 0)
		// {
		// 	int index = outputText.IndexOf("<tc>");                             // Set known index
		// 	outputText = outputText.Remove(startIndex: index, count: 4);        // Remove tag
		// 	outputText = outputText.Insert(startIndex: index, value: card.targetCategory);
		// } // while <tc> left

		return outputText;
	}

	// Inserts the footerValue into a string meant for the footer text
	private string InsertFooterValue(string inputText)
	{
		string outputText = inputText; // String to output
		string footerValueStr = card.FooterValue.ToString("n0"); // The formatted footer value

		// While there's still ITALIC markdown left in input string
		while (outputText.IndexOf("<x>") >= 0)
		{
			// Set known index
			int index = outputText.IndexOf("<x>");
			// Remove markdown
			outputText = outputText.Remove(startIndex: index, count: 3);

			// If the value is a percentage, add a %
			if (card.PercFlag)
			{
				footerValueStr = (footerValueStr + "%");
			}
			// If the value is a percentage, add a $
			if (card.MoneyFlag)
			{
				footerValueStr = ("$" + footerValueStr);
			}

			// Add the appropriate operator to the string
			if (card.FooterOpr == '+')
			{
				footerValueStr = ("+" + footerValueStr);
			}
			else if (card.FooterOpr == '-')
			{
				footerValueStr = ("\u2013" + footerValueStr);
			}

			// Insert start tag
			outputText = outputText.Insert(startIndex: index, value: footerValueStr);

		} // while ITALIC left

		return outputText;

	}

	// Formats card elements based on a new Title
	// TODO: Probably want to break this up into smaller private methods
	public void DisplayTitle()
	{
		switch (card.Category)
		{
			case "Tile":
				// Title Text --------------------------------------------------
				// TODO: Create a dynamic text-centering system based on the physical size of
				// the TMP asset plus the icon width.
				switch (card.Title)
				{
					case "Forest":
						titleUiText.text = " Forest";
						break;
					case "Plains":
						titleUiText.text = " Plains";
						break;
					case "Quarry":
						titleUiText.text = "    Quarry";
						break;
					case "Farmland":
						titleUiText.text = "       Farmland";
						break;
					case "Mountain":
						titleUiText.text = "      Mountain";
						break;
					default:
						titleUiText.text = card.Title;
						break;
				}
				// Icon --------------------------------------------------------

				if (iconImage != null)
				{
					if (iconImage.sprite = Resources.Load<Sprite>("Sprites/icon_" + card.Title.ToLower()))
					{
						// Debug.Log("[CardDisplay] Successfully loaded image sprite \""
						// 	+ "Sprites/icon_"
						// 	+ cardState.title.ToLower() + "\"");
					}
					else
					{
						Debug.LogError("[CardDisplay] ERROR: Could not load image sprite \""
							+ "Sprites/icon_"
							+ card.Title.ToLower() + "\"");
					}
				}

				break;

			case "Game Card":
				// Title Text --------------------------------------------------
				switch (card.Title)
				{
					case "Market Mod":
						titleUiText.text = "\u2013Market Mod\u2013";
						break;
					case "Resource":
						titleUiText.text = "\u2013Resource\u2013";
						break;
					case "Tile Mod":
						titleUiText.text = "\u2013Tile Mod\u2013";
						break;
					default:
						titleUiText.text = card.Title;
						break;
				}
				// Footer Border -----------------------------------------------

				footerBorderL.color = ColorPalette.alpha;
				footerBorderR.color = ColorPalette.alpha;

				switch (card.FooterColor)
				{
					case "Black":
						if (card.ColorCornerFlag)
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
						if (card.ColorCornerFlag)
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
						if (card.ColorCornerFlag)
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
						if (card.ColorCornerFlag)
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
						if (card.ColorCornerFlag)
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
						if (card.ColorCornerFlag)
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
						if (card.ColorCornerFlag)
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
						if (card.ColorCornerFlag)
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
						if (card.ColorCornerFlag)
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
				}
				break;

			case "Market":
				// Title Text --------------------------------------------------
				titleUiText.text = "\u2013Market Card\u2013";
				// Footer Value ------------------------------------------------
				int tempFooterValue = card.FooterValue;
				ResourceInfo.prices.TryGetValue(card.Resource, out tempFooterValue);
				card.FooterValue = tempFooterValue;
				// Footer Border -----------------------------------------------
				footerBorder.color = ColorPalette.cardDark;
				break;

			default:
				titleUiText.text = card.Title;
				break;
		}
	}

	public void DisplaySubtitle()
	{
		if (card.Category != "Tile" && card.Category != "")
			subtitleUiText.text = TagToCardData(MdToTag(card.Subtitle));
	}

	public void DisplayBody()
	{
		bodyUiText.text = TagToCardData(MdToTag(card.Body));
	}

	public void DisplayFooter()
	{
		string tempFooter = card.Footer;
		tempFooter = InsertFooterValue(card.Footer);
		footerUiText.text = TagToCardData(MdToTag(tempFooter));
	}

	// Performas visual updates to a bankrupt card
	public void BankruptVisuals()
	{
		this.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.cardDark;
		this.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.cardDark;

		titleUiText.color = ColorPalette.red500;
		iconImage.color = ColorPalette.red500;

		// This picks up the title text for some reason?
		// GameObject footerObj = tile.transform.Find("Front Canvas/Footer").gameObject;
		// TMP_Text footer = titleObj.GetComponent<TMP_Text>();
		// footer.color = ColorPalette.red500;
		// footer.text = "";
	}

	public void UpdateFooter()
	{
		string tempFooter = card.Footer;
		tempFooter = InsertFooterValue(card.Footer);
		footerUiText.text = TagToCardData(MdToTag(tempFooter));
	}
}

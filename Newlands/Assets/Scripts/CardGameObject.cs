// A driver class to test the functionality of the Card object.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CardGameObject : MonoBehaviour {

	// Textmesh Pro text objects
	public TMP_Text title;
	public TMP_Text subtitle;
	public TMP_Text body;
	public TMP_Text footer;

	// The Card GameObject
	public CardScriptableObject card;

	void Start() {

		card.title = CardEnums.Title.MarketMod;
		card.subtitle = CardEnums.Subtitle.Investment;
		card.footerText = "+**<x>** in Price";
		card.bodyText = "Place this under any **Price Card.** This increases the **Base Price** of the **Resource.**";

		if (card.title == CardEnums.Title.MarketMod) {
			title.text = "\u2013Market Mod\u2013";
		}

		if (card.subtitle == CardEnums.Subtitle.Investment) {
			subtitle.text = "Investment";
		}

		// String members are assinged to the text labels after being formatted
		body.text = mdToTag(card.bodyText);
		card.footerText = insertFooterValue(card.footerText, card.percFlag, card.moneyFlag);
		footer.text = mdToTag(card.footerText);

	} // Start()
	
	void Update() {

	} // Update()

	// Converts a string with bold and italic markdown into html-like tags
	private string mdToTag(string inputText) {

		string outputText = inputText;

		//While there's still BOLD markdown left in input string
		while (outputText.IndexOf("**") >= 0) {
			int index = outputText.IndexOf("**");								// Set known index
			Debug.Log("Index: " + index);
			outputText = outputText.Remove(startIndex: index, count: 2);		// Remove markdown
			Debug.Log(outputText);
			outputText = outputText.Insert(startIndex: index, value: "<b>");	// Insert start tag
			Debug.Log(outputText);

			//Making sure there's a place to insert an end tag
			if (outputText.IndexOf("**") >= 0) {
				index = outputText.IndexOf("**");								// Reset the index
				outputText = outputText.Remove(startIndex: index, count: 2);	// Remove markdown
			outputText =  outputText.Insert(startIndex: index, value: "</b>");	// Insert end tag
			Debug.Log(outputText);
			} else {
				Debug.Log("Error parsing markdown: No closing statement found!");
			}

		} //while BOLD left

		//While there's still ITALIC markdown left in input string
		while (outputText.IndexOf('*') >= 0) {
			int index = outputText.IndexOf('*');								// Set known index
			Debug.Log("Index: " + index);
			outputText = outputText.Remove(startIndex: index, count: 1);		// Remove markdown
			Debug.Log(outputText);
			outputText = outputText.Insert(startIndex: index, value: "<i>");	// Insert start tag
			Debug.Log(outputText);

			//Making sure there's a place to insert an end tag
			if (outputText.IndexOf('*') >= 0) {
				index = outputText.IndexOf('*');								// Reset the index
				outputText = outputText.Remove(startIndex: index, count: 1);	// Remove markdown
			outputText =  outputText.Insert(startIndex: index, value: "</i>");	// Insert end tag
			Debug.Log(outputText);
			} else {
				Debug.Log("Error parsing markdown: No closing statement found!");
			}

		} //while ITALIC left

		return outputText;

	} // mdToTag()

	// Inserts the footerValue into a string meant for the footer text
	private string insertFooterValue(string inputText, bool percFlag, bool moneyFlag) {

		string outputText = inputText;							// String to output
		string footerValueStr = card.footerValue.ToString();	// The formatted footer value

		//While there's still ITALIC markdown left in input string
		while (outputText.IndexOf("<x>") >= 0) {
			// Set known index
			int index = outputText.IndexOf("<x>");								
			Debug.Log("Index: " + index);
			// Remove markdown
			outputText = outputText.Remove(startIndex: index, count: 3);
			Debug.Log(outputText);

			// If the value is a percentage, add a %
			if (percFlag) {
				footerValueStr = (footerValueStr + "%");
			}
			// If the value is a percentage, add a $
			if (moneyFlag) {
				footerValueStr = ("$" + footerValueStr);
			}
		
			// Insert start tag
			outputText = outputText.Insert(startIndex: index, value: footerValueStr);	
			Debug.Log(outputText);

		} //while ITALIC left

		return outputText;
	}

} // TileGameObject()

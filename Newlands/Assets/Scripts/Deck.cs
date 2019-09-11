// A class designed to hold and manage "decks" of Card objects.

// Cards/Vanilla/Tile
// Cards/Vanilla/GameCard
// Cards/Vanilla/MarketCard
// Cards/Vanilla/Undrawable/Tile
// Cards/Vanilla/Undrawable/GameCard

using System.Collections.Generic;
using UnityEngine;

public class Deck : List<Card>
{
	// FIELDS ##################################################################################
	// NOTE: These will no longer be needed after the transition to JSON
	// Resource Directories ================================================
	protected string dirGcMmI = "Cards/Game Cards/Market Mods/Investment";
	protected string dirGcMmS = "Cards/Game Cards/Market Mods/Sabotage";
	protected string dirGcTmI = "Cards/Game Cards/Tile Mods/Investment";
	protected string dirGcTmS = "Cards/Game Cards/Tile Mods/Sabotage";
	protected string dirGcTmR = "Cards/Game Cards/Tile Mods/Resource";
	protected string dirGcTmO = "Cards/Game Cards/Tile Mods/Other";
	protected string dirMc = "Cards/Market Cards";
	protected string dirTL = "Cards/Tiles/Land";
	protected string dirTC = "Cards/Tiles/Coast";

	private DebugTag debugTag = new DebugTag("Deck", "FFEB3B");

	// METHODS #################################################################################

	// Add a card to the deck, taking in a directory and an amount
	public void Add(string directory, int amount = 1)
	{
		TextAsset cardFile = Resources.Load<TextAsset>(directory);
		if (cardFile != null)
		{
			Card cardParsed = JsonUtility.FromJson<Card>(cardFile.text);
			for (int i = 0; i < amount; i++)
			{
				if (cardParsed != null)
				{
					this.Add(cardParsed);
					Debug.Log(debugTag + "Added: "
						+ cardParsed.Category + " - "
						+ cardParsed.Title + " - "
						+ cardParsed.Subtitle);
				}
				else
				{
					Debug.LogError(debugTag.error + "Malformed JSON file at: "
						+ directory);
				}
			}
		}
		else
		{
			Debug.LogError(debugTag.error + "No card file to load from at: " + directory);
		}
	}

	// CONSTRUCTORS ############################################################################

	// No-arg constructor
	public Deck() { }
}

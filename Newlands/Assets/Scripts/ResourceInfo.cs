// A class used to hold the resource prices in a dictionary.

using System.Collections.Generic;
using UnityEngine;

public class ResourceInfo
{
	// DATA FIELDS #################################################################################

	public static readonly List<string> resources = new List<string>();
	public static readonly Dictionary<string, int> prices = new Dictionary<string, int>();
	public static Dictionary<string, int> pricesMut = new Dictionary<string, int>();

	// METHODS #####################################################################################

	// CONSTRUCTORS ################################################################################

	// Default static constructor
	static ResourceInfo()
	{
		// Create the Standard Vanilla resource list, used by default.
		resources.Add("None");
		resources.Add("Lumber");
		resources.Add("Cash Crops");
		// resources.Add("Fish");
		resources.Add("Oil");
		resources.Add("Iron");
		resources.Add("Gems");
		resources.Add("Silver");
		resources.Add("Gold");
		resources.Add("Platinum");

		// Create the Standard Vanilla price list, used by default.
		prices.Add("None", 0);
		prices.Add("Lumber", 50);
		prices.Add("Cash Crops", 50);
		// prices.Add("Fish", 50);
		prices.Add("Oil", 100);
		prices.Add("Iron", 100);
		prices.Add("Gems", 500);
		prices.Add("Silver", 500);
		prices.Add("Gold", 1000);
		prices.Add("Platinum", 2000);

		// Error checking; verify that each resource corresponds to a price
		// If there's an equal number of resources and prices, continue
		if (resources.Count == prices.Count)
		{
			for (int i = 0; i < resources.Count; i++)
			{
				if (!prices.ContainsKey(resources[i]))
				{
					Debug.LogError("<b>[ResourceInfo]</b> Error: "
						+ "The resource \"" + resources[i] + "\""
						+ " could not be found in the price dictionary!");
				} // if prices does not contain key for a resource
				else{
					int value = -1;
					prices.TryGetValue(resources[i], out value);
					pricesMut.Add(resources[i], value);
				}
			} // for
		}
		else
		{
			Debug.LogError("<b>[ResourceInfo]</b> Error: "
				+ "The number of resources (" + resources.Count + ") "
				+ "does not match the number of prices (" + resources.Count + ")!");
		} // if equal resource/price count

	} // ResourceInfo() constructor

	public ResourceInfo(string deckType)
	{
		// NOTE: Only custom resources or price changes need to be put here.
	} // Prices(deckType) constructor
} // class ResourceInfo
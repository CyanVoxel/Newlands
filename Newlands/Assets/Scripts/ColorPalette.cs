// A class containing various colors stored as Color32 variables

using UnityEngine;
using ColorHexUtility;

public class ColorPalette
{
	// FIELDS ######################################################################################

	// General Colors ==============================================================================
	public static readonly Color32 alpha = new Color32(r: 0, g: 0, b: 0, a: 0);

	// Newlands Colors =============================================================================

	// Main  ---------------------------------------------------------------------------------------
	public static readonly Color32 tintCard = new ColorHex("#FCFAF5");
	public static readonly Color32 cardLight = new ColorHex("#CAC8C4");
	public static readonly Color32 cardDark = new ColorHex("#111111");

	// Tint  ---------------------------------------------------------------------------------------
	public static readonly Color32[] tintRed = new Color32[5]
	{
		new ColorHex("#f14234"),	// 500
		new ColorHex("#ee5a50"),	// 400
		new ColorHex("#ea6f68"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintOrangeDeep = new Color32[5]
	{
		new ColorHex("#fc6421"),	// 500
		new ColorHex("#f8744b"),	// 400
		new ColorHex("#f28366"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintOrange = new Color32[5]
	{
		new ColorHex("#fc9500"),	// 500
		new ColorHex("#f79c49"),	// 400
		new ColorHex("#f1a366"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintAmber = new Color32[5]
	{
		new ColorHex("#fcbd07"),	// 500
		new ColorHex("#f6bf50"),	// 400
		new ColorHex("#f0c06c"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintYellow = new Color32[5]
	{
		new ColorHex("#fce639"),	// 500
		new ColorHex("#f4e168"),	// 400
		new ColorHex("#eddc80"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintGreenLight = new Color32[5]
	{
		new ColorHex("#aed837"),	// 500
		new ColorHex("#b3d65f"),	// 400
		new ColorHex("#b8d37d"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintGreen = new Color32[5]
	{
		new ColorHex("#4bac4d"),	// 500
		new ColorHex("#62af62"),	// 400
		new ColorHex("#79b377"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintTeal = new Color32[5]
	{
		new ColorHex("#1aa292"),	// 500
		new ColorHex("#43a597"),	// 400
		new ColorHex("#5ca99c"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintCyan = new Color32[5]
	{
		new ColorHex("#00b8cc"),	// 500
		new ColorHex("#48bacb"),	// 400
		new ColorHex("#63bcca"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintBlueLight = new Color32[5]
	{
		new ColorHex("#03a6ea"),	// 500
		new ColorHex("#4cabe6"),	// 400
		new ColorHex("#61ade3"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintBlue = new Color32[5]
	{
		new ColorHex("#217de9"),	// 500
		new ColorHex("#4d87e6"),	// 400
		new ColorHex("#5e8de3"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintBlueDeep = new Color32[5]
	{
		new ColorHex("#4e54cd"),	// 500
		new ColorHex("#5c60cc"),	// 400
		new ColorHex("#6c6fcc"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintPurpleDeep = new Color32[5]
	{
		new ColorHex("#7b40dc"),	// 500
		new ColorHex("#8151db"),	// 400
		new ColorHex("#8a63d9"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintPurple = new Color32[5]
	{
		new ColorHex("#b039bf"),	// 500
		new ColorHex("#b24cbf"),	// 400
		new ColorHex("#b45dc0"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintPink = new Color32[5]
	{
		new ColorHex("#e94076"),	// 500
		new ColorHex("#e7567e"),	// 400
		new ColorHex("#e56385"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] tintGray = new Color32[5]
	{
		new ColorHex("#878684"),	// 500
		new ColorHex("#8f8e8b"),	// 400
		new ColorHex("#969592"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};

	// Full ----------------------------------------------------------------------------------------
	public static readonly Color32[] baseRed = new Color32[5]
	{
		new ColorHex("#c1352a"),	// 500
		new ColorHex("#c2524b"),	// 400
		new ColorHex("#c36a65"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] baseOrangeDeep = new Color32[5]
	{
		new ColorHex("#ca501a"),	// 500
		new ColorHex("#ca6548"),	// 400
		new ColorHex("#ca7865"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] baseOrange = new Color32[5]
	{
		new ColorHex("#ca7700"),	// 500
		new ColorHex("#ca8449"),	// 400
		new ColorHex("#ca9066"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] baseAmber = new Color32[5]
	{
		new ColorHex("#ca9705"),	// 500
		new ColorHex("#ca9f50"),	// 400
		new ColorHex("#caa66c"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] baseYellow = new Color32[5]
	{
		new ColorHex("#cab82d"),	// 500
		new ColorHex("#cabb63"),	// 400
		new ColorHex("#cabe7d"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] baseGreenLight = new Color32[5]
	{
		new ColorHex("#8bad2c"),	// 500
		new ColorHex("#97b15b"),	// 400
		new ColorHex("#a3b67a"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] baseGreen = new Color32[5]
	{
		new ColorHex("#3c893d"),	// 500
		new ColorHex("#599158"),	// 400
		new ColorHex("#739b70"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] baseTeal = new Color32[5]
	{
		new ColorHex("#158175"),	// 500
		new ColorHex("#41887e"),	// 400
		new ColorHex("#5c9087"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] baseCyan = new Color32[5]
	{
		new ColorHex("#0093a3"),	// 500
		new ColorHex("#489aa7"),	// 400
		new ColorHex("#63a0ab"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] baseBlueLight = new Color32[5]
	{
		new ColorHex("#0285bc"),	// 500
		new ColorHex("#4c8fbd"),	// 400
		new ColorHex("#6196be"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] baseBlue = new Color32[5]
	{
		new ColorHex("#1a64bb"),	// 500
		new ColorHex("#4b74bc"),	// 400
		new ColorHex("#5c7dbd"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] baseBlueDeep = new Color32[5]
	{
		new ColorHex("#3f43a4"),	// 500
		new ColorHex("#5153a6"),	// 400
		new ColorHex("#6365a9"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] basePurpleDeep = new Color32[5]
	{
		new ColorHex("#6233b0"),	// 500
		new ColorHex("#6c48b1"),	// 400
		new ColorHex("#785db3"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] basePurple = new Color32[5]
	{
		new ColorHex("#8d2d99"),	// 500
		new ColorHex("#92459c"),	// 400
		new ColorHex("#97589f"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] basePink = new Color32[5]
	{
		new ColorHex("#bb335f"),	// 500
		new ColorHex("#bc4e6c"),	// 400
		new ColorHex("#bd5d74"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};
	public static readonly Color32[] cardGray = new Color32[5]
	{
		new ColorHex("#6d6b69"),	// 500
		new ColorHex("#797775"),	// 400
		new ColorHex("#848280"),	// 300
		new ColorHex("#FF00FF"),	// 200
		new ColorHex("#FF00FF")		// 100
	};

	// METHODS #####################################################################################

	// Returns a color from the Newlands palette based on its name, strength (500, 400, etc.),
	// and whether or not it's a tint.
	public static Color32 GetNewlandsColor(string name, int strength, bool isTint)
	{
		Color32 color = tintCard;
		int strengthIndex;

		switch (strength)
		{
			case 500:
				strengthIndex = 0; break;
			case 400:
				strengthIndex = 1; break;
			case 300:
				strengthIndex = 2; break;
			case 200:
				strengthIndex = 3; break;
			case 100:
				strengthIndex = 4; break;
			default:
				strengthIndex = 0; break;
		}

		switch (name)
		{
			case "Red":
				if (isTint)
					color = tintRed[strengthIndex];
				else
					color = baseRed[strengthIndex]; break;
			case "Orange Deep":
			case "Deep Orange":
				if (isTint)
					color = tintOrangeDeep[strengthIndex];
				else
					color = baseOrangeDeep[strengthIndex]; break;
			case "Amber":
				if (isTint)
					color = tintAmber[strengthIndex];
				else
					color = baseAmber[strengthIndex]; break;
			case "Yellow":
				if (isTint)
					color = tintYellow[strengthIndex];
				else
					color = baseYellow[strengthIndex]; break;
			case "Green Light":
			case "Light Green":
				if (isTint)
					color = tintGreenLight[strengthIndex];
				else
					color = baseGreenLight[strengthIndex]; break;
			case "Green":
				if (isTint)
					color = tintGreen[strengthIndex];
				else
					color = baseGreen[strengthIndex]; break;
			case "Teal":
				if (isTint)
					color = tintTeal[strengthIndex];
				else
					color = baseTeal[strengthIndex]; break;
			case "Cyan":
				if (isTint)
					color = tintCyan[strengthIndex];
				else
					color = baseCyan[strengthIndex]; break;
			case "Blue Light":
			case "Light Blue":
				if (isTint)
					color = tintBlueLight[strengthIndex];
				else
					color = baseBlueLight[strengthIndex]; break;
			case "Blue":
				if (isTint)
					color = tintBlue[strengthIndex];
				else
					color = baseBlue[strengthIndex]; break;
			case "Blue Deep":
			case "Deep Blue":
				if (isTint)
					color = tintBlueDeep[strengthIndex];
				else
					color = baseBlueDeep[strengthIndex]; break;
			case "Purple Deep":
			case "Deep Purple":
				if (isTint)
					color = tintPurpleDeep[strengthIndex];
				else
					color = basePurpleDeep[strengthIndex]; break;
			case "Purple":
				if (isTint)
					color = tintPurple[strengthIndex];
				else
					color = basePurple[strengthIndex]; break;
			case "Pink":
				if (isTint)
					color = tintPink[strengthIndex];
				else
					color = basePink[strengthIndex]; break;
			case "Gray":
			case "Grey":
				if (isTint)
					color = tintGray[strengthIndex];
				else
					color = cardGray[strengthIndex]; break;
			case "Black":
				color = cardDark; break;
			case "Card":
			case "White":
				if (isTint)
					color = tintCard;
				else
					color = cardLight; break;
			default: break;
		}

		return color;
	}

	// Returns a color based on the Player's ID.
	// Used if players aren't allowed to choose their own colors.
	public static Color32 GetDefaultPlayerColor(int id, int strength, bool isTint)
	{
		Color32 color = tintCard;

		switch (id)
		{
			case 1: color = GetNewlandsColor("Red", strength, isTint); break;
			case 2: color = GetNewlandsColor("Blue", strength, isTint); break;
			case 3: color = GetNewlandsColor("Amber", strength, isTint); break;
			case 4: color = GetNewlandsColor("Green", strength, isTint); break;
			case 5: color = GetNewlandsColor("Orange", strength, isTint); break;
			case 6: color = GetNewlandsColor("Cyan", strength, isTint); break;
			case 7: color = GetNewlandsColor("Pink", strength, isTint); break;
			case 8: color = GetNewlandsColor("Deep Purple", strength, isTint); break;
			default: break;
		}

		return color;
	}

	// Outputs a hex color tag from a Color32
	public static string Color32ToTag(Color32 color)
	{
		string hex = "<color=#"
			+ color.r.ToString("X2")
			+ color.g.ToString("X2")
			+ color.b.ToString("X2")
			+ ">";
		return hex;
	}

}

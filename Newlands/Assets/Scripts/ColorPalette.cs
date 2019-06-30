// A class containing various colors stored as Color32 variables
// TODO: Add final colors to palette, replacing Material Design ones.

using UnityEngine;

public class ColorPalette {

	#region FIELDS
	// FIELDS #####################################################################################

	// Material Design ============================================================================
	// 500 --------------------------------------
	private static readonly Color32 red500 = new Color32(r: 244, g: 67, b: 54, a: 255);
	private static readonly Color32 deepOrange500 = new Color32(r: 255, g: 87, b: 34, a: 255);
	private static readonly Color32 orange500 = new Color32(r: 255, g: 152, b: 0, a: 255);
	private static readonly Color32 amber500 = new Color32(r: 255, g: 152, b: 0, a: 255);
	private static readonly Color32 yellow500 = new Color32(r: 255, g: 235, b: 59, a: 255);
	private static readonly Color32 lime500 = new Color32(r: 205, g: 220, b: 57, a: 255);
	private static readonly Color32 lightGreen500 = new Color32(r: 139, g: 195, b: 74, a: 255);
	private static readonly Color32 green500 = new Color32(r: 76, g: 175, b: 80, a: 255);
	private static readonly Color32 teal500 = new Color32(r: 0, g: 150, b: 136, a: 255);
	private static readonly Color32 cyan500 = new Color32(r: 0, g: 188, b: 212, a: 255);
	private static readonly Color32 lightBlue500 = new Color32(r: 3, g: 169, b: 244, a: 255);
	private static readonly Color32 blue500 = new Color32(r: 33, g: 150, b: 243, a: 255);
	private static readonly Color32 indigo500 = new Color32(r: 63, g: 81, b: 181, a: 255);
	private static readonly Color32 deepPurple500 = new Color32(r: 103, g: 58, b: 183, a: 255);
	private static readonly Color32 purple500 = new Color32(r: 156, g: 39, b: 176, a: 255);
	private static readonly Color32 pink500 = new Color32(r: 233, g: 30, b: 99, a: 255);

	// 400 --------------------------------------
	private static readonly Color32 red400 = new Color32(r: 239, g: 83, b: 80, a: 255);
	private static readonly Color32 amber400 = new Color32(r: 255, g: 202, b: 40, a: 255);
	private static readonly Color32 lightBlue400 = new Color32(r: 41, g: 182, b: 246, a: 255);
	private static readonly Color32 purple400 = new Color32(r: 171, g: 71, b: 188, a: 255);

	// 300 --------------------------------------
	private static readonly Color32 red300 = new Color32(r: 229, g: 115, b: 115, a: 255);
	private static readonly Color32 orange300 = new Color32(r: 255, g: 183, b: 77, a: 255);
	private static readonly Color32 amber300 = new Color32(r: 255, g: 213, b: 79, a: 255);
	private static readonly Color32 cyan300 = new Color32(r: 77, g: 205, b: 225, a: 255);
	private static readonly Color32 lightBlue300 = new Color32(r: 79, g: 195, b: 247, a: 255);
	private static readonly Color32 purple300 = new Color32(r: 186, g: 104, b: 200, a: 255);

	// 200 --------------------------------------
	private static readonly Color32 red200 = new Color32(r: 239, g: 154, b: 154, a: 255);
	private static readonly Color32 orange200 = new Color32(r: 255, g: 204, b: 128, a: 255);
	private static readonly Color32 amber200 = new Color32(r: 255, g: 224, b: 130, a: 255);
	private static readonly Color32 lightBlue200 = new Color32(r: 129, g: 212, b: 250, a: 255);
	private static readonly Color32 purple200 = new Color32(r: 206, g: 147, b: 216, a: 255);

	// 100 --------------------------------------
	private static readonly Color32 lightBlue100 = new Color32(r: 179, g: 229, b: 252, a: 255);

	// Misc =======================================================================================
	public static readonly Color32 alpha = new Color32(r: 0, g: 0, b: 0, a: 0);

	// Newlands ===================================================================================
	// Main Colors ------------------------------
	public static readonly Color32 cardLight = new Color32(r: 250, g: 245, b: 234, a: 255);
	public static readonly Color32 cardDark = new Color32(r: 17, g: 17, b: 17, a: 255);

	// 500 Tint Colors --------------------------
	public static readonly Color32 tintCard = new Color32(r: 252, g: 250, b: 245, a: 255);
	public static readonly Color32 tintRed500 = new Color32();
	public static readonly Color32 tintOrangeD500 = new Color32();
	public static readonly Color32 tintOrange500 = new Color32();
	public static readonly Color32 tintAmber500 = new Color32();
	public static readonly Color32 tintYellow500 = new Color32();
	public static readonly Color32 tintGreenL500 = new Color32();
	public static readonly Color32 tintGreen500 = new Color32();
	public static readonly Color32 tintTeal500 = new Color32();
	public static readonly Color32 tintCyan500 = new Color32();
	public static readonly Color32 tintBlueL500 = new Color32();
	public static readonly Color32 tintBlue500 = new Color32();
	public static readonly Color32 tintBlueD500 = new Color32();
	public static readonly Color32 tintPurpleD500 = new Color32();
	public static readonly Color32 tintPurple500 = new Color32();
	public static readonly Color32 tintPink500 = new Color32();
	public static readonly Color32 tintGray500 = new Color32();

	#endregion

	#region PROPERTIES
	// PROPERTIES #################################################################################

	// Material Design ============================================================================
	// 500 --------------------------------------
	public static Color32 Red500 => red500;
	public static Color32 DeepOrange500 => deepOrange500;
	public static Color32 Orange500 => orange500;
	public static Color32 Amber500 => amber500;
	public static Color32 Yellow500 => yellow500;
	public static Color32 Lime500 => lime500;
	public static Color32 LightGreen500 => lightGreen500;
	public static Color32 Green500 => green500;
	public static Color32 Teal500 => teal500;
	public static Color32 Cyan500 => cyan500;
	public static Color32 LightBlue500 => lightBlue500;
	public static Color32 Blue500 => blue500;
	public static Color32 Indigo500 => indigo500;
	public static Color32 DeepPurple500 => deepPurple500;
	public static Color32 Purple500 => purple500;
	public static Color32 Pink500 => pink500;

	// 400 --------------------------------------
	public static Color32 Red400 => red400;
	public static Color32 Amber400 => amber400;
	public static Color32 LightBlue400 => lightBlue400;
	public static Color32 Purple400 => purple400;

	// 300 --------------------------------------
	public static Color32 Red300 => red300;
	public static Color32 Orange300 => orange300;
	public static Color32 Amber300 => amber300;
	public static Color32 Cyan300 => cyan300;
	public static Color32 LightBlue300 => lightBlue300;
	public static Color32 Purple300 => purple300;

	// 200 --------------------------------------
	public static Color32 Red200 => red200;
	public static Color32 Orange200 => orange200;
	public static Color32 Amber200 => amber200;
	public static Color32 LightBlue200 => lightBlue200;
	public static Color32 Purple200 => purple200;

	// 100 --------------------------------------
	public static Color32 LightBlue100 => lightBlue100;

	#endregion

	#region METHODS
	// METHODS ####################################################################################

	// Outputs a hex color tag from a Color32
	public string color32ToHex(Color32 color) {
		string hex = "<color=#"
			+ color.r.ToString("X2")
			+ color.g.ToString("X2")
			+ color.b.ToString("X2")
			+ ">";
		return hex;
	} // color32ToHex()

	#endregion

} // ColorPalette class

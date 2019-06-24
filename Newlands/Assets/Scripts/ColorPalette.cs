// A class containing various colors stored as Color32 variables

using UnityEngine;

public class ColorPalette {

	// COLORS #####################################################################################

	// CMYK Ink Colors ============================================================================

	// Full -------------------------------------
	// public static readonly Color inkBlack = new Color32(r: 17, g: 17, b: 17, a: 255);
	// public static readonly Color inkRed = new Color32(r: 237, g: 28, b: 36, a: 255);
	// public static readonly Color inkBlue = new Color32(r: 46, g: 49, b: 146, a: 255);
	// public static readonly Color inkGreen = new Color32(r: 0, g: 166, b: 81, a: 255);
	// public static readonly Color inkCyan = new Color32(r: 0, g: 174, b: 235, a: 255);
	// public static readonly Color inkMagenta = new Color32(r: 236, g: 0, b: 140, a: 255);
	// public static readonly Color inkYellow = new Color32(r: 255, g: 242, b: 0, a: 255);

	// // 90p --------------------------------------
	// public static readonly Color inkRed90p = new Color32(r: 239, g: 93, b: 95, a: 255);
	// public static readonly Color inkCyan90p = new Color32(r: 88, g: 184, b: 241, a: 255);

	// // 70p --------------------------------------
	// public static readonly Color inkRed70p = new Color32(r: 243, g: 150, b: 151, a: 255);
	// public static readonly Color inkGreen70p = new Color32(r: 148, g: 198, b: 160, a: 255);
	// public static readonly Color inkCyan70p = new Color32(r: 148, g: 203, b: 244, a: 255);

	// Material Design Colors =====================================================================

	// 500 --------------------------------------
	public static readonly Color red500 = new Color32(r: 244, g: 67, b: 54, a: 255);
	public static readonly Color deepOrange500 = new Color32(r: 255, g: 87, b: 34, a: 255);
	public static readonly Color orange500 = new Color32(r: 255, g: 152, b: 0, a: 255);
	public static readonly Color amber500 = new Color32(r: 255, g: 152, b: 0, a: 255);
	public static readonly Color yellow500 = new Color32(r: 255, g: 235, b: 59, a: 255);
	public static readonly Color lime500 = new Color32(r: 205, g: 220, b: 57, a: 255);
	public static readonly Color lightGreen500 = new Color32(r: 139, g: 195, b: 74, a: 255);
	public static readonly Color green500 = new Color32(r: 76, g: 175, b: 80, a: 255);
	public static readonly Color teal500 = new Color32(r: 0, g: 150, b: 136, a: 255);
	public static readonly Color cyan500 = new Color32(r: 0, g: 188, b: 212, a: 255);
	public static readonly Color lightBlue500 = new Color32(r: 3, g: 169, b: 244, a: 255);
	public static readonly Color blue500 = new Color32(r: 33, g: 150, b: 243, a: 255);
	public static readonly Color indigo500 = new Color32(r: 63, g: 81, b: 181, a: 255);
	public static readonly Color deepPurple500 = new Color32(r: 103, g: 58, b: 183, a: 255);
	public static readonly Color purple500 = new Color32(r: 156, g: 39, b: 176, a: 255);
	public static readonly Color pink500 = new Color32(r: 233, g: 30, b: 99, a: 255);

	// 400 --------------------------------------
	public static readonly Color red400 = new Color32(r: 239, g: 83, b: 80, a: 255);
	public static readonly Color amber400 = new Color32(r: 255, g: 202, b: 40, a: 255);
	public static readonly Color lightBlue400 = new Color32(r: 41, g: 182, b: 246, a: 255);
	public static readonly Color purple400 = new Color32(r: 171, g: 71, b: 188, a: 255);

	// 300 --------------------------------------
	public static readonly Color red300 = new Color32(r: 229, g: 115, b: 115, a: 255);
	public static readonly Color orange300 = new Color32(r: 255, g: 183, b: 77, a: 255);
	public static readonly Color amber300 = new Color32(r: 255, g: 213, b: 79, a: 255);
	public static readonly Color cyan300 = new Color32(r: 77, g: 205, b: 225, a: 255);
	public static readonly Color lightBlue300 = new Color32(r: 79, g: 195, b: 247, a: 255);
	public static readonly Color purple300 = new Color32(r: 186, g: 104, b: 200, a: 255);

	// 200 --------------------------------------
	public static readonly Color red200 = new Color32(r: 239, g: 154, b: 154, a: 255);
	public static readonly Color orange200 = new Color32(r: 255, g: 204, b: 128, a: 255);
	public static readonly Color amber200 = new Color32(r: 255, g: 224, b: 130, a: 255);
	public static readonly Color lightBlue200 = new Color32(r: 129, g: 212, b: 250, a: 255);
	public static readonly Color purple200 = new Color32(r: 206, g: 147, b: 216, a: 255);

	// 100 --------------------------------------
	public static readonly Color lightBlue100 = new Color32(r: 179, g: 229, b: 252, a: 255);

	// Newlands ===================================================================================

	// Theme Colors -----------------------------
	public static readonly Color cardLight = new Color32(r: 250, g: 245, b: 234, a: 255);
	public static readonly Color cardDark = new Color32(r: 17, g: 17, b: 17, a: 255);

	// Cardstock Texture Tints ------------------
	public static readonly Color cardTintLight = new Color32(r: 252, g: 250, b: 245, a: 255);

	// Other ======================================================================================
	public static readonly Color alpha = new Color32(r: 0, g: 0, b: 0, a: 0);
	

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

} // ColorPalette class
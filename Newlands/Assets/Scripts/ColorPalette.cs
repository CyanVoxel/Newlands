// A class containing various colors stored as Color32 variables

using UnityEngine;

public class ColorPalette {

	// COLORS #####################################################################################

	// Ink Colors - Based on the original tabletop cards ------------------------------------------

	// Full -------------------------------------------------------------------
	public static readonly Color inkBlack = new Color32(r: 17, g: 17, b: 17, a: 255);
	public static readonly Color inkRed = new Color32(r: 237, g: 28, b: 36, a: 255);
	public static readonly Color inkBlue = new Color32(r: 46, g: 49, b: 146, a: 255);
	public static readonly Color inkGreen = new Color32(r: 0, g: 166, b: 81, a: 255);
	public static readonly Color inkCyan = new Color32(r: 0, g: 174, b: 235, a: 255);
	public static readonly Color inkMagenta = new Color32(r: 236, g: 0, b: 140, a: 255);
	public static readonly Color inkYellow = new Color32(r: 255, g: 242, b: 0, a: 255);

	// 90p ----------------------------
	public static readonly Color inkRed90p = new Color32(r: 239, g: 93, b: 95, a: 255);
	public static readonly Color inkCyan90p = new Color32(r: 88, g: 184, b: 241, a: 255);

	// 70p ----------------------------
	public static readonly Color inkRed70p = new Color32(r: 243, g: 150, b: 151, a: 255);
	public static readonly Color inkGreen70p = new Color32(r: 148, g: 198, b: 160, a: 255);
	public static readonly Color inkCyan70p = new Color32(r: 148, g: 203, b: 244, a: 255);

	// Material Design Colors ---------------------------------------------------------------------

	// 500 ----------------------------
	public static readonly Color amber500 = new Color32(r: 255, g: 152, b: 0, a: 255);
	public static readonly Color purple500 = new Color32(r: 156, g: 39, b: 176, a: 255);

	// 300 ----------------------------
	public static readonly Color red300 = new Color32(r: 229, g:115, b: 115, a: 255);
	public static readonly Color orange300 = new Color32(r: 255, g: 183, b: 77, a: 255);
	public static readonly Color amber300 = new Color32(r: 255, g: 213, b: 79, a: 255);
	public static readonly Color cyan300 = new Color32(r: 77, g:205, b: 225, a: 255);
	public static readonly Color purple300 = new Color32(r: 186, g:104, b: 200, a: 255);

	// 200 ----------------------------
	public static readonly Color amber200 = new Color32(r: 255, g: 224, b: 130, a: 255);
	public static readonly Color purple200 = new Color32(r: 206, g: 147, b: 216, a: 255);

	// Other --------------------------------------------------------------------------------------
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
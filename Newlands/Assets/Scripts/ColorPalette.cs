// A class containing various colors stored as Color32 variables

using UnityEngine;

public class ColorPalette {

	// Ink Colors - Based on the original tabletop cards
	public static Color inkBlack = new Color32(r: 17, g: 17, b: 17, a: 255);
	public static Color inkRed = new Color32(r: 237, g: 28, b: 36, a: 255);
	public static Color inkGreen = new Color32(r: 0, g: 166, b: 81, a: 255);
	public static Color inkBlue = new Color32(r: 0, g: 174, b: 235, a: 255);
	
	
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
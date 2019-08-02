// A class containing various colors stored as Color32 variables
// TODO: Add final colors to palette, replacing Material Design ones.

using UnityEngine;

public class ColorPalette {

	#region FIELDS
	// FIELDS ######################################################################################

	// Material Design =============================================================================
	// 500 --------------------------------------
	public static readonly Color32 red500 = new Color32(r: 244, g: 67, b: 54, a: 255);
	public static readonly Color32 deepOrange500 = new Color32(r: 255, g: 87, b: 34, a: 255);
	public static readonly Color32 orange500 = new Color32(r: 255, g: 152, b: 0, a: 255);
	public static readonly Color32 amber500 = new Color32(r: 255, g: 152, b: 0, a: 255);
	public static readonly Color32 yellow500 = new Color32(r: 255, g: 235, b: 59, a: 255);
	public static readonly Color32 lime500 = new Color32(r: 205, g: 220, b: 57, a: 255);
	public static readonly Color32 lightGreen500 = new Color32(r: 139, g: 195, b: 74, a: 255);
	public static readonly Color32 green500 = new Color32(r: 76, g: 175, b: 80, a: 255);
	public static readonly Color32 teal500 = new Color32(r: 0, g: 150, b: 136, a: 255);
	public static readonly Color32 cyan500 = new Color32(r: 0, g: 188, b: 212, a: 255);
	public static readonly Color32 lightBlue500 = new Color32(r: 3, g: 169, b: 244, a: 255);
	public static readonly Color32 blue500 = new Color32(r: 33, g: 150, b: 243, a: 255);
	public static readonly Color32 indigo500 = new Color32(r: 63, g: 81, b: 181, a: 255);
	public static readonly Color32 deepPurple500 = new Color32(r: 103, g: 58, b: 183, a: 255);
	public static readonly Color32 purple500 = new Color32(r: 156, g: 39, b: 176, a: 255);
	public static readonly Color32 pink500 = new Color32(r: 233, g: 30, b: 99, a: 255);
	public static readonly Color32 testColor = new ColorHex("#E64A19");

	// 400 --------------------------------------
	public static readonly Color32 red400 = new Color32(r: 239, g: 83, b: 80, a: 255);
	public static readonly Color32 amber400 = new Color32(r: 255, g: 202, b: 40, a: 255);
	public static readonly Color32 lightBlue400 = new Color32(r: 41, g: 182, b: 246, a: 255);
	public static readonly Color32 purple400 = new Color32(r: 171, g: 71, b: 188, a: 255);

	// 300 --------------------------------------
	public static readonly Color32 red300 = new Color32(r: 229, g: 115, b: 115, a: 255);
	public static readonly Color32 orange300 = new Color32(r: 255, g: 183, b: 77, a: 255);
	public static readonly Color32 amber300 = new Color32(r: 255, g: 213, b: 79, a: 255);
	public static readonly Color32 cyan300 = new Color32(r: 77, g: 205, b: 225, a: 255);
	public static readonly Color32 lightBlue300 = new Color32(r: 79, g: 195, b: 247, a: 255);
	public static readonly Color32 purple300 = new Color32(r: 186, g: 104, b: 200, a: 255);

	// 200 --------------------------------------
	public static readonly Color32 red200 = new Color32(r: 239, g: 154, b: 154, a: 255);
	public static readonly Color32 orange200 = new Color32(r: 255, g: 204, b: 128, a: 255);
	public static readonly Color32 amber200 = new Color32(r: 255, g: 224, b: 130, a: 255);
	public static readonly Color32 lightBlue200 = new Color32(r: 129, g: 212, b: 250, a: 255);
	public static readonly Color32 purple200 = new Color32(r: 206, g: 147, b: 216, a: 255);

	// 100 --------------------------------------
	public static readonly Color32 lightBlue100 = new Color32(r: 179, g: 229, b: 252, a: 255);

	// Misc ========================================================================================
	public static readonly Color32 alpha = new Color32(r: 0, g: 0, b: 0, a: 0);

	// Newlands ====================================================================================
	// Main Colors ------------------------------
	public static readonly Color32 tintCard = new Color32(r: 252, g: 250, b: 245, a: 255);
	public static readonly Color32 cardLight = new Color32(r: 250, g: 245, b: 234, a: 255);
	public static readonly Color32 cardDark = new Color32(r: 17, g: 17, b: 17, a: 255);

	// 500 Tint Colors --------------------------
	public static readonly Color32 tintRed500 = new Color32();
	public static readonly Color32 tintOrangeDeep500 = new Color32();
	public static readonly Color32 tintOrange500 = new Color32();
	public static readonly Color32 tintAmber500 = new Color32();
	public static readonly Color32 tintYellow500 = new Color32();
	public static readonly Color32 tintGreenLight500 = new Color32();
	public static readonly Color32 tintGreen500 = new Color32();
	public static readonly Color32 tintTeal500 = new Color32();
	public static readonly Color32 tintCyan500 = new Color32();
	public static readonly Color32 tintBlueLight500 = new Color32();
	public static readonly Color32 tintBlue500 = new Color32();
	public static readonly Color32 tintBlueDeep500 = new Color32();
	public static readonly Color32 tintPurpleDeep500 = new Color32();
	public static readonly Color32 tintPurple500 = new Color32();
	public static readonly Color32 tintPink500 = new Color32();
	public static readonly Color32 tintGray500 = new Color32();

	#endregion

	#region METHODS
	// METHODS #####################################################################################

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

public struct ColorHex {

	byte r;
	byte g;
	byte b;
	byte a;

	public ColorHex(byte r, byte g, byte b, byte a) {

		this.r = r;
		this.g = g;
		this.b = b;
		this.a = a;

	} // ColorHex(byte r, byte g, byte b, byte a)

	public ColorHex(string hex) {

		string h = hex;

		if (h.Contains("#")) {
			h = h.Remove(hex.IndexOf("#"), 1);
		}

		switch (h.Length) {

			case 6:
				this.r = byte.Parse(h.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
				this.g = byte.Parse(h.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
				this.b = byte.Parse(h.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
				this.a = 255;
				break;

			case 8:
				this.r = byte.Parse(h.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
				this.g = byte.Parse(h.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
				this.b = byte.Parse(h.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
				this.a = byte.Parse(h.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
				break;

			default:
				this.r = 0;
				this.g = 0;
				this.b = 0;
				this.a = 0;
				break;

		} // switch (hex.Length)

	} // ColorHex(byte r, byte g, byte b, byte a)

	public override bool Equals(object obj) {

		bool typeCheck = false;

		if (this.GetType().Equals(obj.GetType()) || obj is UnityEngine.Color32) {
			typeCheck = true;
		}

		if (obj == null || !typeCheck) {
			return false;
		} else {
			ColorHex c = (ColorHex)obj;
			return (r == c.r && g == c.g && b == c.b && a == c.a);
		}

	} // override Equals()

	public static bool operator ==(UnityEngine.Color32 left, ColorHex right) {

		if (left.r == right.r && left.r == right.r
			&& left.g == right.g && left.g == right.g
			&& left.b == right.b && left.b == right.b
			&& left.a == right.a && left.a == right.a) {
			return true;
		} else {
			return false;
		}

	} // operator ==

	public static bool operator ==(ColorHex left, ColorHex right) {

		if (left.r == right.r && left.r == right.r
			&& left.g == right.g && left.g == right.g
			&& left.b == right.b && left.b == right.b
			&& left.a == right.a && left.a == right.a) {
			return true;
		} else {
			return false;
		}

	} // operator ==

	public static bool operator !=(UnityEngine.Color32 left, ColorHex right) {

		if (left.r != right.r || left.r != right.r
			|| left.g != right.g || left.g != right.g
			|| left.b != right.b || left.b != right.b
			|| left.a != right.a || left.a != right.a) {
			return true;
		} else {
			return false;
		}

	} // operator !=

	public static bool operator !=(ColorHex left, ColorHex right) {

		if (left.r != right.r || left.r != right.r
			|| left.g != right.g || left.g != right.g
			|| left.b != right.b || left.b != right.b
			|| left.a != right.a || left.a != right.a) {
			return true;
		} else {
			return false;
		}

	} // operator !=

	public static implicit operator UnityEngine.Color32(ColorHex c) {
		return new UnityEngine.Color32(c.r, c.b, c.g, c.a);
	}

	public override int GetHashCode() {

		return r ^ g ^ b ^ a;

	} // override GetHashCode()

} // ColorHex struct

// A class that holds various strings for beautifying Debug.Log outputs.
// @Author: Travis Abendshien (https://github.com/CyanVoxel)

public class DebugTag
{
	public readonly string head;
	public readonly string warning;
	public readonly string error;

	// Constructor which takes in the Class Name with no color
	public DebugTag(string className)
	{
		this.head = "<b>[" + className + "] </b>";
		this.warning = "<b>[" + className + "] </b><color=#FFAB00FF><b>Warning: </b></color>";
		this.error = "<b>[" + className + "] </b><color=#D50000FF><b>Error: </b></color>";
	}

	// Constructor which takes in the Class Name and a desired Hex Color
	public DebugTag(string className, string color)
	{
		this.head = "<color=#" + color + "FF><b>[" + className + "] </b></color>";
		this.warning = "<color=#" + color + "FF><b>[" + className + "] </b></color><color=#FFAB00FF><b>Warning: </b></color>";
		this.error = "<color=#" + color + "FF><b>[" + className + "] </b></color><color=#D50000FF><b>Error: </b></color>";
	}

	public override string ToString() {
		return this.head;
	}
}

// USAGE:
// private static DebugTag debug = new DebugTag("GameManager", "FF6D00");
// Debug.Log(debug.head + "Hello, World!");

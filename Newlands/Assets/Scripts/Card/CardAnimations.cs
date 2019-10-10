// Script containing hard-coded animations for cards. Could be replaced by proper animations
// in the future.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimations : MonoBehaviour
{
	private static DebugTag debug = new DebugTag("CardAnimations", "F50057");

	public static void FlipCard(string cardType, int x, int y)
	{
		GameObject cardObj;
		string xZeroes = "0";
		string yZeroes = "0";

		// Determines the number of zeroes to add in the object name
		if (x >= 10)
			xZeroes = "";
		else
			xZeroes = "0";
		if (y >= 10)
			yZeroes = "";
		else
			yZeroes = "0";

		// Does different things depending on the card type
		switch (cardType)
		{
			case "Tile":
				cardObj = GameObject.Find("x" + xZeroes + x + "_"
					+ "y" + yZeroes + y + "_"
					+ cardType);
				if (cardObj != null)
				{
					cardObj.transform.Rotate(0, 180, 0);
				}
				else
				{
					Debug.Log(debug.head + "Null value found for GameObject "
						+ "x" + xZeroes + x + "_"
						+ "y" + yZeroes + y + "_"
						+ cardType);
				}
				break;

			case "GameCard":
				cardObj = GameObject.Find("x" + xZeroes + x + "_"
					+ "y" + yZeroes + y + "_"
					+ cardType);
				if (cardObj != null)
				{
					cardObj.transform.Rotate(0, 180, 0);
				}
				else
				{
					Debug.Log(debug.error + "Null value found for GameObject "
						+ "x" + xZeroes + x + "_"
						+ "y" + yZeroes + y + "_"
						+ cardType);
				}
				break;

			case "Market":
				break;

			default:
				Debug.LogError(debug.error + "No Card Type matches: " + cardType);
				break;
		}
	}

	public static void HighlightCards(List<Coordinate2> cards, int colorId = 0)
	{
		for (int i = 0; i < cards.Count; i++)
		{
			GameObject cardObj;
			string xZeroes = "0";
			string yZeroes = "0";

			// Determines the number of zeroes to add in the object name
			if (cards[i].x >= 10)
				xZeroes = "";
			else
				xZeroes = "0";
			if (cards[i].y >= 10)
				yZeroes = "";
			else
				yZeroes = "0";

			cardObj = GameObject.Find("x" + xZeroes + cards[i].x + "_"
				+ "y" + yZeroes + cards[i].y + "_"
				+ "Tile");
			if (cardObj != null)
			{
				cardObj.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.GetDefaultPlayerColor(colorId, 300, true);
				cardObj.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.GetDefaultPlayerColor(colorId, 300, true);
			}
			else
			{
				Debug.Log(debug.head + "Null value found for GameObject "
					+ "x" + xZeroes + cards[i].x + "_"
					+ "y" + yZeroes + cards[i].y + "_"
					+ "Tile");
			}
		}
	}

	public static IEnumerator MoveCardCoroutine(GameObject obj, GameObject end, Vector3 gap, float speed, bool moveUnder = true)
	{
		if (moveUnder)
		{
			Vector3 underEnd = new Vector3(end.transform.position.x,
				end.transform.position.y,
				end.transform.position.z + 1f) - gap;

			while (Vector3.Distance(obj.transform.position, underEnd) >.1f)
			{
				obj.transform.position = Vector3.Lerp(obj.transform.position, underEnd, speed * 1);
				yield return new WaitForSeconds(0.01f);
			}
		}

		while (Vector3.Distance(obj.transform.position, end.transform.position - gap) >.01f)
		{
			obj.transform.position = Vector3.Lerp(obj.transform.position, end.transform.position - gap, speed * 1);
			yield return new WaitForSeconds(0.001f);
		}

		obj.transform.position = end.transform.position - gap;
	}

	public static IEnumerator MoveCardCoroutine(GameObject obj, Vector3 end, float speed)
	{
		while (Vector3.Distance(obj.transform.position, end) >.01f)
		{
			obj.transform.position = Vector3.Lerp(obj.transform.position, end, speed * 2);
			yield return new WaitForSeconds(0.001f);
		}

		obj.transform.position = end;
	}

	public static IEnumerator MoveTileCoroutine(GameObject obj, Vector3 end, float speed)
	{
		Vector3 start = obj.transform.position;

		while (Vector3.Distance(obj.transform.position, (start + end)) >.01f)
		{
			obj.transform.position = Vector3.Lerp(obj.transform.position, (obj.transform.position + end), speed * .5f);
			yield return new WaitForSeconds(0.001f);
		}

		obj.transform.position = (start + end);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimations : MonoBehaviour {

    [SerializeField]
    private GameManager gameMan;
    private static DebugTag debug = new DebugTag("CardAnimations", "F50057");

    // Start is called before the first frame update
    void Start() {
        gameMan = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update() {

    }

    public static void FlipCard(string cardType, int x, int y) {

        GameObject cardObj;
        string xZeroes = "0";
        string yZeroes = "0";
        // Determines the number of zeroes to add in the object name
        if (x >= 10) {
            xZeroes = "";
        } else {
            xZeroes = "0";
        }
        if (y >= 10) {
            yZeroes = "";
        } else {
            yZeroes = "0";
        } // zeroes calc

        // Does different things depending on the card type
        switch (cardType) {

            case "Tile":
                // Debug.Log(debug.head + "Trying to flip " + "x"
                //     + xZeroes + x + "_"
                //     + "y" + yZeroes + y + "_"
                //     + cardType);
                cardObj = GameObject.Find("x" + xZeroes + x + "_"
                    + "y" + yZeroes + y + "_"
                    + cardType);
                if (cardObj != null) {
                    cardObj.transform.rotation = new Quaternion(cardObj.transform.rotation.x,
                        1 - cardObj.transform.rotation.y,
                        cardObj.transform.rotation.z, 0);
                } else {
                    Debug.Log(debug.head + "Null value found for GameObject "
                        + "x" + xZeroes + x + "_"
                        + "y" + yZeroes + y + "_"
                        + cardType);
                } // Null check
                break;

            case "GameCard":
                cardObj = GameObject.Find("x" + xZeroes + x + "_"
                    + "y" + yZeroes + y + "_"
                    + cardType);
                if (cardObj != null) {
                    cardObj.transform.rotation = new Quaternion(cardObj.transform.rotation.x,
                        1 - cardObj.transform.rotation.y,
                        cardObj.transform.rotation.z, 0);
                } else {
                    Debug.Log(debug.error + "Null value found for GameObject "
                        + "x" + xZeroes + x + "_"
                        + "y" + yZeroes + y + "_"
                        + cardType);
                } // Null check
                break;

            case "Market":
                break;

            default:
                Debug.LogError(debug.error + "No Card Type matches: " + cardType);
                break;

        } // switch

    } // FlipCard()

    public static void HighlightCards(List<Coordinate2> cards, int colorId = 0) {

        for (int i = 0; i < cards.Count; i++) {

            GameObject cardObj;
            string xZeroes = "0";
            string yZeroes = "0";
            // Determines the number of zeroes to add in the object name
            if (cards[i].x >= 10) {
                xZeroes = "";
            } else {
                xZeroes = "0";
            }
            if (cards[i].y >= 10) {
                yZeroes = "";
            } else {
                yZeroes = "0";
            } // zeroes calc

            // Debug.Log(debug.head + "Trying to Color " + "x"
            //     + xZeroes + cards[i].x + "_"
            //     + "y" + yZeroes + cards[i].y + "_"
            //     + "Tile");
            cardObj = GameObject.Find("x" + xZeroes + cards[i].x + "_"
                + "y" + yZeroes + cards[i].y + "_"
                + "Tile");
            if (cardObj != null) {

                switch (colorId) {
                    case 0: // Default Player ID, used for wiping selection
                        cardObj.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintCard;
                        cardObj.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintCard;
                        break;
                    case 1:
                        cardObj.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintRed300;
                        cardObj.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintRed300;
                        break;
                    case 2:
                        cardObj.GetComponentsInChildren<Renderer>()[0].material.color = ColorPalette.tintBlueLight300;
                        cardObj.GetComponentsInChildren<Renderer>()[1].material.color = ColorPalette.tintBlueLight300;
                        break;
                    default:
                        break;
                }

            } else {
                Debug.Log(debug.head + "Null value found for GameObject "
                    + "x" + xZeroes + cards[i].x + "_"
                    + "y" + yZeroes + cards[i].y + "_"
                    + "Tile");
            } // Null check

        }

    } // HighlightNeighbors()

}

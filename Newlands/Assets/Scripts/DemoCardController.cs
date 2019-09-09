using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoCardController : MonoBehaviour
{
	GameObject demoTile;
	// CardState cardState;
	LandTileDeck landTileDeck;

	// Start is called before the first frame update
	void Start()
	{
		landTileDeck = new LandTileDeck("Vanilla");

		demoTile = GameObject.Find("DemoTile");
		if (demoTile != null)
		{
			// cardState = demoTile.GetComponent<CardState>();
			// demoTile.gameObject.SetActive(true);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (demoTile == null)
		{
			Debug.Log("DemoTile was null, trying to find again...");
			demoTile = GameObject.Find("DemoTile");
			if (demoTile != null)
			{
				// cardState = demoTile.GetComponent<CardState>();
				// demoTile.gameObject.SetActive(true);
			}
		}

		if (demoTile != null)
		{
			demoTile.transform.Rotate(0, Time.deltaTime * 30f, 0, Space.World);
		}

	}

}

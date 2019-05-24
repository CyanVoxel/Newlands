// Used to create a grid of Tiles/Cards

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	// DATA FIELDS ------------------------------------------------------------
	private int width = 3;
	private int height = 3;

	//private GameObject card = Resources.Load<GameObject>("Prefabs/Card");
	public GameObject card;		//For easy testing

	// Use this for initialization
	void Start() {

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				float xOff = x * 11;
				float yOff = y * 8;

				GameObject cardObj = (GameObject)Instantiate(card, new Vector3(xOff, yOff, 50), Quaternion.identity);
				cardObj.name = ("Card_x" + x + "_y" + y + "_z0");
				cardObj.transform.SetParent(this.transform);

			} // y
		} // x

	}
	
	// Update is called once per frame
	void Update() {
		
	}
}

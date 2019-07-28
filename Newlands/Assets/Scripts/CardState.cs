// A test class used to test the functionality of scripts across the network

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardState : MonoBehaviour {

    [SerializeField]
    private string titleStr;
    private CardDisplay cardDis;

    // Start is called before the first frame update
    void Start() {

        cardDis = this.GetComponent<CardDisplay>();

        titleStr = "test";
        // titleStr = GridManager.grid[2,2].subScope;

        GameObject titleObj = this.transform.Find("Front Canvas/Title").gameObject;
        TMP_Text title = titleObj.GetComponent<TMP_Text>();
        title.text = titleStr;

    } // Start()

    // Update is called once per frame
    void Update() {

    }
}

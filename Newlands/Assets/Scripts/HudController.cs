﻿// Controls and updates the HUD based on changing values in GameManager.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudController : MonoBehaviour {

    public GameManager gameMan;

    private TMP_Text phaseNumberText;
    private TMP_Text roundNumberText;
    private TMP_Text turnNumberText;
    private int lastKnownTurn = -1;
    private int lastKnownRound = -1;
    private int lastKnownPhase = -1;

    // Start is called before the first frame update
    void Start() {

        InitPlayerText();

        this.lastKnownTurn = gameMan.turn;
        this.lastKnownRound = gameMan.round;
        this.lastKnownPhase = gameMan.phase;

        UpdateUI();

    } // Start()

    // Update is called once per frame
    void Update() {

        // On New Turn
        if (gameMan.turn != this.lastKnownTurn
            || gameMan.round != this.lastKnownRound
            || gameMan.phase != this.lastKnownPhase) {

            UpdateUI();

        }

    } // Update()

    private void InitPlayerText() {

        if (transform.Find("Hud/PhaseNumber") != null
            && (transform.Find("Hud/PhaseNumber").GetComponent<TMP_Text>() != null)) {
            phaseNumberText = transform.Find("Hud/PhaseNumber").gameObject.GetComponent<TMP_Text>();
        }

        if (transform.Find("Hud/RoundNumber") != null
            && (transform.Find("Hud/RoundNumber").GetComponent<TMP_Text>() != null)) {
            roundNumberText = transform.Find("Hud/RoundNumber").gameObject.GetComponent<TMP_Text>();
        }

        if (transform.Find("Hud/TurnNumber") != null
            && (transform.Find("Hud/TurnNumber").GetComponent<TMP_Text>() != null)) {
            turnNumberText = transform.Find("Hud/TurnNumber").gameObject.GetComponent<TMP_Text>();
        }

    } //InitPlayerText()

    private void UpdateUI() {

        this.lastKnownTurn = gameMan.turn;
        this.lastKnownRound = gameMan.round;
        this.lastKnownPhase = gameMan.phase;

        this.phaseNumberText.text = ("Phase " + this.lastKnownPhase);
        this.roundNumberText.text = ("Round " + this.lastKnownRound);
        this.turnNumberText.text = ("Player " + this.lastKnownTurn + "'s Turn");

    } // UpdateUI()

} // class HudController

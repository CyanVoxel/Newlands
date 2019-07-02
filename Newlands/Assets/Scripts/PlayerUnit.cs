// The unit controlled by the player.

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerUnit : NetworkBehaviour {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        if (!hasAuthority) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            this.transform.Translate(0, 1, 0);
        }

    } // Update()

} // PlayerUnit class

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorillaController : Movable {
    // Gorillas simply idle until player gets close enough
    // then leap towards them attacking

    // State handling
    public enum GorillaState {
        Idle,
        Crouching,
        Jumping,
    }

    public enum GorillaDirection {
        Right,
        Left
    }


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
        if(IsGrounded()){
            // standin anim
        } else {
            // anim to jumping
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : Movable {
    public float lifetimeSeconds = Mathf.Infinity;

    // Just a macro to despawn an object when it's out of frame
    private float radius = 0.1f;

    private float birthTime;

    void Start() {
    	birthTime = Time.time;
    }

	void Update(){

        if(isOffScreen() || isTimedOut()){
               Destroy(gameObject);
        }
	}

	bool isOffScreen() {
        Vector3 screenposition = Camera.main.WorldToViewportPoint(transform.position);
		return 1.0f < screenposition.y - radius || 0.0f > screenposition.y + radius || 1.0f < screenposition.x - radius || 0.0f > screenposition.x + radius;
	}

	bool isTimedOut() {
		return Time.time - birthTime >= lifetimeSeconds;
	}
}

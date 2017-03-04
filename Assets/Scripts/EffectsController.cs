using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : Movable {
    // Just a macro to despawn an object when it's out of frame
    private float radius = 0.1f;

	void Update(){
        Vector3 screenposition = Camera.main.WorldToViewportPoint(transform.position);

        if(1.0f < screenposition.y - radius || 0.0f > screenposition.y + radius ||
           1.0f < screenposition.x - radius || 0.0f > screenposition.x + radius){
               Destroy(gameObject);
        }
	}
}

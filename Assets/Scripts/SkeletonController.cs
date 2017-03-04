using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : Movable {
    // Skeletons patrol a given path, until the player is within a certain distance.
    // Then, the skeleton throws an axe at the player until the player runs away or is dead
    // Then returns to patrolling

    // State handling
    public enum SkeletonState {
        Patrolling,
        Attacking
    }

    public SkeletonState state;


    // Patrol settings
    public enum SkeletonDirection {
        Right,
        Left
    }

    public SkeletonDirection direction;
    public float patrolSize = 0.0f;
    private float leftBound, rightBound;
    public float moveSpeed = 0.0f;
    private SpriteRenderer sprite;

	void Start () {
        // Setup patrol bounds
        leftBound = transform.position.x - patrolSize;
        rightBound = transform.position.x + patrolSize;

        sprite = GetComponent<SpriteRenderer>();
	}

	void Update () {
        if(IsGrounded()){
            // update skeleton state here based on player distance

            if(state == SkeletonState.Attacking){
                // Attack player
            }
            if(state == SkeletonState.Patrolling){
                // If we're patrolling, face the way we're moving
                sprite.flipX = (direction == SkeletonDirection.Left);

                if(direction == SkeletonDirection.Right && transform.position.x < rightBound){
                    // Move right
                    Vector3 move = new Vector3(Time.deltaTime * moveSpeed, 0.0f, 0.0f);
                    transform.position += move;
                } else if(direction == SkeletonDirection.Left && transform.position.x > leftBound){
                    // Move left
                    Vector3 move = new Vector3(Time.deltaTime * moveSpeed, 0.0f, 0.0f);
                    transform.position -= move;
                } else {
                    // Reverse direction
                    direction = (direction == SkeletonDirection.Right) ? SkeletonDirection.Left : SkeletonDirection.Right;
                }
            }
        }
	}
}

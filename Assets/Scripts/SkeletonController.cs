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

    public enum SkeletonDirection {
        Right,
        Left
    }

    [Header("Skeleton State")]
    public SkeletonState state;
    public SkeletonDirection direction;

    [Header("Patrol Settings")]
    public float patrolSize = 0.0f;
    private float leftBound, rightBound;
    public float moveSpeed = 0.0f;

    [Header("Attack Settings")]
    public float attackDistance = 0.0f;
    public float attackCooldown = 0.0f;
    public float attackSpeed = 0.0f;

    // component references
    private SpriteRenderer sprite;

    // gameobject references
    private GameObject player;

	void Start () {
        // Setup patrol bounds
        leftBound = transform.position.x - patrolSize;
        rightBound = transform.position.x + patrolSize;

        sprite = GetComponent<SpriteRenderer>();

        player = GameObject.FindWithTag("Player");
        if(!player){
            Debug.LogError("Couldn't find player gameobject.");
        }
	}

	void Update () {
        if(IsGrounded()){
            state = (transform.position - player.transform.position).magnitude <= attackDistance ? SkeletonState.Attacking : SkeletonState.Patrolling;

            if(state == SkeletonState.Attacking){
                direction = transform.position.x > player.transform.position.x ? SkeletonDirection.Left : SkeletonDirection.Right;

                // create a new axe
                // shoot it at player
                // put throw on cooldown
            }
            if(state == SkeletonState.Patrolling){
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

        // flip the sprite depending on direction
        sprite.flipX = (direction == SkeletonDirection.Left);
	}
}

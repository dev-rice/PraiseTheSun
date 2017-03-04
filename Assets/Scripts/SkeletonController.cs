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
        Attacking,
        Dead
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
    private float currentCooldown = 999.0f;
    public float attackSpeed = 0.0f;
    public GameObject axe;

    [Header("Health Settings")]
    public int health;

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
        if(state == SkeletonState.Dead){
            return;
        }
        // increment current cooldown regardless of state
        currentCooldown += Time.deltaTime;

        if(IsGrounded()){
            state = (transform.position - player.transform.position).magnitude <= attackDistance ? SkeletonState.Attacking : SkeletonState.Patrolling;

            if(state == SkeletonState.Attacking){
                // face player
                direction = transform.position.x > player.transform.position.x ? SkeletonDirection.Left : SkeletonDirection.Right;

                if(currentCooldown >= attackCooldown){
                    // clear current cooldown
                    currentCooldown = 0.0f;

                    // create axe
                    GameObject newAxe = Instantiate(axe);
                    Rigidbody2D axeRigidbody = newAxe.GetComponent<Rigidbody2D>();
                    newAxe.transform.position = transform.position;

                    // throw axe in arc at player
                    float v_x = (player.transform.position.x - newAxe.transform.position.x) / attackSpeed;
                    float v_y = ((player.transform.position.y - newAxe.transform.position.y) / attackSpeed) + 0.5f * 9.8f * attackSpeed;

                    axeRigidbody.velocity = new Vector3(v_x, v_y, 0.0f);
                }
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

        if(health <= 0){
            state = SkeletonState.Dead;
        }

        // flip the sprite depending on direction
        sprite.flipX = (direction == SkeletonDirection.Left);
	}

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.tag == "Weapon" && currentCooldown > 0.1f){
            health -= collider.gameObject.GetComponent<WeaponDamage>().damage;
            Destroy(collider.gameObject);
        }
    }
}

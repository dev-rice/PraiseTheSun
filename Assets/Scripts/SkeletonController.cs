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
    private float horizontaldistance = 0.4f;

    [Header("Attack Settings")]
    public float attackDistance = 0.0f;
    public float attackCooldown = 0.0f;
    private float currentCooldown = 999.0f;
    public float attackSpeed = 0.0f;
    public GameObject axe;
    public GameObject bloodParticles;

    [Header("Health Settings")]
    public int health;

    [Header("Sound Effects")]
    public AudioClip hitSound;
    public AudioClip throwSound;

    // component references
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;

    // gameobject references
    private GameObject player;

	void Start () {
        // Setup patrol bounds
        leftBound = transform.position.x - patrolSize;
        rightBound = transform.position.x + patrolSize;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindWithTag("Player");
        if(!player){
            Debug.LogError("Couldn't find player gameobject.");
        }
	}

	void Update () {
        if(state == SkeletonState.Dead){
            if(!IsGrounded()){
                return;
            }

            // turn off physics
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<BoxCollider2D>().enabled = false;

            // don't let snapping happen
            CacheTransformAndPixelSnap();
            snap = PixelSnap.Never;

            return;
        }
        // increment current cooldown regardless of state
        currentCooldown += Time.deltaTime;

        // cache grounded state
        bool isGrounded = IsGrounded();

        if(isGrounded){
            state = (transform.position - player.transform.position).magnitude <= attackDistance ? SkeletonState.Attacking : SkeletonState.Patrolling;

            if(state == SkeletonState.Attacking){
                // face player
                direction = transform.position.x > player.transform.position.x ? SkeletonDirection.Left : SkeletonDirection.Right;

                if(currentCooldown >= attackCooldown){
                    // clear current cooldown
                    currentCooldown = 0.0f;

                    //  play throw animation and sounds
                    animator.SetTrigger("anim_throw");
                    audioSource.clip = throwSound;
                    audioSource.Play();

                    // create axe
                    GameObject newAxe = Instantiate(axe);
                    Rigidbody2D axeRigidbody = newAxe.GetComponent<Rigidbody2D>();
                    newAxe.transform.position = transform.position;
                    newAxe.GetComponent<WeaponDamage>().creator = gameObject;
                    newAxe.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;

                    // throw axe in arc at player
                    // V_0_x = (x - x_0) / t
                    // V_0_y = (y - y_0 + 1/2 g t^2) / t
                    float v_x = (player.transform.position.x - newAxe.transform.position.x) / attackSpeed;
                    float v_y = ((player.transform.position.y - newAxe.transform.position.y) / attackSpeed) + 0.5f * 9.8f * attackSpeed;

                    axeRigidbody.velocity = new Vector3(v_x, v_y, 0.0f);
                } else {
                    animator.SetTrigger("anim_idle");
                }
            }
            if(state == SkeletonState.Patrolling){
                animator.SetTrigger("anim_patrol");

                if(direction == SkeletonDirection.Right && transform.position.x < rightBound && RightGroundDistance() > horizontaldistance){
                    // Move right
                    Vector3 move = new Vector3(Time.deltaTime * moveSpeed, 0.0f, 0.0f);
                    transform.position += move;
                } else if(direction == SkeletonDirection.Left && transform.position.x > leftBound && LeftGroundDistance() > horizontaldistance){
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
            // set state and animation state
            state = SkeletonState.Dead;
            animator.SetTrigger("anim_death");
        }

        // flip the sprite depending on direction
        spriteRenderer.flipX = (direction == SkeletonDirection.Left);
	}

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Weapon" && currentCooldown > 0.1f && health > 0.0f){
            // remove health
            WeaponDamage weapon = other.gameObject.GetComponent<WeaponDamage>();

            if(!weapon.hurtCreator || weapon.creator == gameObject){
                health -= weapon.damage;
            } else {
                return;
            }

            if(weapon.destroyOnImpact){
                Destroy(other.gameObject);
            }

            // play hit animation and sound
            animator.SetTrigger("anim_hit");
            audioSource.clip = hitSound;
            audioSource.Play();

            // Create blood
            GameObject blood = Instantiate(bloodParticles);
            blood.transform.position = transform.position;
        }
    }
}

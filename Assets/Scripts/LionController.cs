using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LionController : Movable {
    // Lions/Panthers sleep until player gets near, then perks up.
    // if the player continues approaching the lion, it will jump up and charge blindly forward
    // either hitting the player, or hitting the end of the charge, where it will check for the
    // player and either charge again or go to sleep

    // State handling
    public enum LionState {
        Sleeping,
        Alert,
        Charging,
        Dead
    }

    public enum LionDirection {
        Right,
        Left
    }

    [Header("Lion State")]
    public LionState state;
    public LionDirection direction;

    [Header("Attack Settings")]
    public float chargeDistance;
    public float chargeSpeed;
    public float alertDistance;
    public float alertTime;
    private float currentAlertTime;
    public GameObject bloodParticles;
    public GameObject lionWeapon;
    private GameObject lionWeaponInstance;

    [Header("Health Settings")]
    public int health;

    [Header("Sound Effects")]
    public AudioClip growlSound;
    public AudioClip hitSound;

    // component references
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    // gameobject references
    private GameObject player;

	void Start () {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindWithTag("Player");
        if(!player){
            Debug.LogError("Couldn't find player gameobject.");
        }
	}

	void Update () {
        if(state == LionState.Dead){
            if(!IsGrounded()){
                return;
            }

            animator.SetTrigger("anim_sleep");

            // turn off physics
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<BoxCollider2D>().enabled = false;

            // don't let snapping happen
            CacheTransformAndPixelSnap();
            snap = PixelSnap.Never;

            return;
        }

        // In the jungle, the mighty jungle
        if(IsGrounded()){
            if(state == LionState.Sleeping){
                animator.SetTrigger("anim_sleep");

                if((transform.position - player.transform.position).magnitude < alertDistance){
                    state = LionState.Alert;
                    currentAlertTime = 0.0f;
                }
            } else if(state == LionState.Alert){
                animator.SetTrigger("anim_alert");

                currentAlertTime += Time.deltaTime;

                if(currentAlertTime > alertTime){
                    currentAlertTime = 0.0f;

                    state = (transform.position - player.transform.position).magnitude < alertDistance ? LionState.Charging : LionState.Sleeping;

                    if(state == LionState.Charging){
                        // If newly charging, setup state
                        direction = transform.position.x > player.transform.position.x ? LionDirection.Left : LionDirection.Right;
                        spriteRenderer.flipX = direction == LionDirection.Left;

                        // play sound
                        audioSource.clip = growlSound;
                        audioSource.Play();
                    }
                }
            } else if(state == LionState.Charging){
                animator.SetTrigger("anim_run");

                Destroy(lionWeaponInstance);
                lionWeaponInstance = Instantiate(lionWeapon);
                lionWeaponInstance.transform.position = transform.position;
                lionWeapon.transform.localScale = direction == LionDirection.Left ? new Vector3(-1.0f, 1.0f, 1.0f) : new Vector3(1.0f, 1.0f, 1.0f);
                lionWeaponInstance.transform.parent = transform;

                float newx = direction == LionDirection.Left ? transform.position.x - chargeSpeed * Time.deltaTime : transform.position.x + chargeSpeed * Time.deltaTime;
                transform.position = new Vector3(newx, transform.position.y, 0.0f);

                if((transform.position - player.transform.position).magnitude > alertDistance){
                    state = LionState.Alert;
                    Destroy(lionWeaponInstance);
                }
            }
        } else {
            float newx = direction == LionDirection.Left ? transform.position.x - chargeSpeed * Time.deltaTime : transform.position.x + chargeSpeed * Time.deltaTime;
            transform.position = new Vector3(newx, transform.position.y, 0.0f);

            animator.SetTrigger("anim_jump");
        }

        if(health <= 0){
            state = LionState.Dead;
        }
	}

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Weapon" && health > 0){
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

            // play sound
            audioSource.clip = hitSound;
            audioSource.Play();

            // Create blood
            GameObject blood = Instantiate(bloodParticles);
            blood.transform.position = transform.position;
        }
    }
}

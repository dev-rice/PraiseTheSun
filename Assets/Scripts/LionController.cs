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
    private float chargeStart;
    public float alertDistance;
    public float alertTime;
    private float currentAlertTime;
    public GameObject bloodParticles;

    [Header("Health Settings")]
    public int health;

    [Header("TEMPORARY sprites")]
    public Sprite sleepSprite;
    public Sprite alertSprite;
    public Sprite chargeSprite;
    private SpriteRenderer spriteRenderer;

    // component references

    // gameobject references
    private GameObject player;

	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindWithTag("Player");
        if(!player){
            Debug.LogError("Couldn't find player gameobject.");
        }
	}

	void Update () {
        if(state == LionState.Dead){
            return;
        }

        // In the jungle, the mighty jungle
        if(state == LionState.Sleeping){
            spriteRenderer.sprite = sleepSprite;

            if((transform.position - player.transform.position).magnitude < alertDistance){
                state = LionState.Alert;
                currentAlertTime = 0.0f;
            }
        } else if(state == LionState.Alert){
            spriteRenderer.sprite = alertSprite;

            currentAlertTime += Time.deltaTime;

            if(currentAlertTime > alertTime){
                state = (transform.position - player.transform.position).magnitude < alertDistance ? LionState.Charging : LionState.Sleeping;

                // If newly charging, setup state
                if(state == LionState.Charging){
                    spriteRenderer.sprite = chargeSprite;

                    direction = transform.position.x > player.transform.position.x ? LionDirection.Left : LionDirection.Right;
                    spriteRenderer.flipX = direction == LionDirection.Left;

                    chargeStart = transform.position.x;
                }
            }
        } else if(state == LionState.Charging){
            float newx = direction == LionDirection.Left ? transform.position.x - chargeSpeed * Time.deltaTime : transform.position.x + chargeSpeed * Time.deltaTime;
            transform.position = new Vector3(newx, transform.position.y, 0.0f);

            if(Mathf.Abs(transform.position.x - chargeStart) > chargeDistance){
                state = LionState.Alert;
                currentAlertTime = 0.0f;
            }
        }

        if(health <= 0){
            // set state
            state = LionState.Dead;

            // turn off physics
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<BoxCollider2D>().enabled = false;

            // don't let snapping happen
            CacheTransformAndPixelSnap();
            snap = PixelSnap.Never;
        }
	}

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Weapon" && health > 0){
            // remove health
            WeaponDamage weapon = other.gameObject.GetComponent<WeaponDamage>();
            health -= weapon.damage;

            if(weapon.destroyOnImpact){
                Destroy(other.gameObject);
            }

            // Create blood
            GameObject blood = Instantiate(bloodParticles);
            blood.transform.position = transform.position;
        }
    }
}

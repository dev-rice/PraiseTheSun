using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Movable {

    public float moveSpeed = 3.0f;
    public float jumpVelocity = 8.0f;
    public int health;
    private const int BASE_HEATLH = 1000;

    public Bonfire bonfire;

    public MessageBanner banner;

    public GameObject healtPickupPrefab;

    public LevelManager levelManager;

	public enum PlayerDirection {
        Right,
        Left
    }
    public PlayerDirection direction;

    public GameObject bloodParticles;
    public GameObject healthPickupParticles;

    [Header("Action Costs")]
    public int jumpStaminaCost;

	private float acceleration_time_airborne = 0.5f;
	private float acceleration_time_grounded = 0.1f;

    private Rigidbody2D rigidbody2d;

    private float velocity_x_smoothing;

    private bool isDead;
    private const string BONFIRE_TAG = "Bonfire";
    private const string WEAPON_TAG = "Weapon";
    private const string HEALTH_PICKUP_TAG = "HealthPickup";

    private int healthPickedUpSinceLastDeath = 0;

    private GameObject lastHealthPickupDropped;

	void Start () {
		health = BASE_HEATLH;

		rigidbody2d = GetComponent<Rigidbody2D>();
		direction = PlayerDirection.Right;
		isDead = false;

        if(bonfire){
		    bonfire.Light();
        }
	}

	// Update is called once per frame
	void Update () {
		Vector2 velocity = rigidbody2d.velocity;
		if (IsGrounded()) {
			if (Input.GetKeyDown(KeyCode.Space)) {
                health -= jumpStaminaCost;
				velocity.y = jumpVelocity;
			}
		}
		Vector2 input = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		// This smooths the velocity so it feels like you have less impact on your movement when you are in the air
		velocity.x = smoothVelocityX(velocity.x, input.x * moveSpeed);

		rigidbody2d.velocity = velocity;

		direction = getDirectionFromInput(input);
		flipBasedOnDirection();

		if (Input.GetKeyDown(KeyCode.K)) {
			isDead = true;
		}
		if (health <= 0) {
			isDead = true;
		}


		if (isDead) {
			die();
		}
	}

	void die() {
		if (banner != null) {
			banner.showMessage("YOU DIED");			
		}
		
		levelManager.playerDied();

		rigidbody2d.velocity = new Vector2(0, 0);
		

		if (lastHealthPickupDropped != null) {
			Destroy(lastHealthPickupDropped);
		}
		if (healthPickedUpSinceLastDeath > 0) {
			dropHealthPickup();			
		}
        
        if(bonfire){
            transform.position = bonfire.transform.position;
            direction = PlayerDirection.Right;
        }

        health = BASE_HEATLH;
        healthPickedUpSinceLastDeath = 0;
		isDead = false;
	}

	void dropHealthPickup() {
		GameObject g = Instantiate(healtPickupPrefab);
		g.transform.position = transform.position;
		HealthPickup h = (HealthPickup)g.GetComponent<HealthPickup>();
		h.amount = healthPickedUpSinceLastDeath;

		lastHealthPickupDropped = g;
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == BONFIRE_TAG) {

			Bonfire new_bonfire = (Bonfire)other.gameObject.GetComponent<Bonfire>();

			if (!new_bonfire.isLit()) {
				new_bonfire.Light();
				if (banner != null) {
					banner.showMessage("BONFIRE LIT");					
				}
			}

			this.bonfire = new_bonfire;
		}

        if(other.GetComponent<Collider2D>().tag == WEAPON_TAG && health > 0.0f){
            WeaponDamage weapon = other.gameObject.GetComponent<WeaponDamage>();
            health -= weapon.damage;

            if(weapon.destroyOnImpact){
                Destroy(other.gameObject);
            }

            // Create blood
            GameObject blood = Instantiate(bloodParticles);
            blood.transform.position = transform.position;
        }

        if (other.tag == HEALTH_PICKUP_TAG) {
        	HealthPickup h = (HealthPickup)other.gameObject.GetComponent<HealthPickup>();
        	Debug.Log("picking up " + h.amount + " health");

        	health += h.amount;
        	healthPickedUpSinceLastDeath += h.amount;
        	Destroy(h.gameObject);

        	GameObject particles = Instantiate(healthPickupParticles);
            particles.transform.position = transform.position;
            particles.GetComponent<ParticleImplodeController>().target = transform;

        }
	}

	private float smoothVelocityX(float current_x_velocity, float target_velocity_x) {
		float acceleration_time = getAccelerationTime();
        return Mathf.SmoothDamp(current_x_velocity, target_velocity_x, ref velocity_x_smoothing, acceleration_time);
	}

	private float getAccelerationTime() {
        if (IsGrounded()) {
            return acceleration_time_grounded;
        } else {
            return acceleration_time_airborne;
        }
	}

	private void flipBasedOnDirection() {
		Vector3 scale = transform.localScale;
		if (direction == PlayerDirection.Left) {
			transform.localScale = new Vector3(-1, scale.y, scale.z);
		} else if (direction == PlayerDirection.Right) {
			transform.localScale = new Vector3(1, scale.y, scale.z);
		}
	}

	private PlayerDirection getDirectionFromInput(Vector3 input) {
		if (input.x == 0) {
			return direction;
		} else if (input.x >= 0) {
			return PlayerDirection.Right;
		} else {
			return PlayerDirection.Left;
		}
	}
}

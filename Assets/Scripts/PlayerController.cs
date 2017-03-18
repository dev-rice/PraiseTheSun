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

    public enum PlayerState {
        Idle,
        Running,
        Jumping,
        Attacking,
        Blocking,
        Hit,
        Dead,
    }

    // timers
    private float blockTime = 0.4166f;
    private float blockTimeCurrent;
    private float attackTime = 0.4166f;
    private float attackWeaponTime = 0.166f;
    private float attackTimeCurrent;

    public PlayerState state;
    private PlayerState oldstate;
    public PlayerDirection direction;

    public GameObject bloodParticles;
    public GameObject healthPickupParticles;

    [Header("Action Costs")]
    public int jumpStaminaCost;
    public int blockStaminaCost;
    public int attackStaminaCost;

    [Header("Sounds")]
    public AudioClip hitSound;
    public AudioClip jumpSound;
    public AudioClip blockSound;
    public AudioClip pickupSound;

    private AudioSource audioSource;

	private float acceleration_time_airborne = 0.5f;
	private float acceleration_time_grounded = 0.1f;

    private Rigidbody2D rigidbody2d;

    private float velocity_x_smoothing;

    private const string BONFIRE_TAG = "Bonfire";
    private const string WEAPON_TAG = "Weapon";
    private const string HEALTH_PICKUP_TAG = "HealthPickup";

    private int healthPickedUpSinceLastDeath = 0;

    private GameObject lastHealthPickupDropped;
    private Animator animator;

    public GameObject playerWeaponInstance;
    private GameObject playerWeapon;

	void Start () {
		health = BASE_HEATLH;

		rigidbody2d = GetComponent<Rigidbody2D>();
		direction = PlayerDirection.Right;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

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

                audioSource.clip = jumpSound;
                audioSource.Play();
			}
		}
		Vector2 input = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		// This smooths the velocity so it feels like you have less impact on your movement when you are in the air
		velocity.x = smoothVelocityX(velocity.x, input.x * moveSpeed);

		rigidbody2d.velocity = velocity;

		direction = getDirectionFromInput(input);
		flipBasedOnDirection();

        // Hacky state handling because we didn't start with this
        oldstate = state;

        if(!IsGrounded()){
            state = PlayerState.Jumping;
        } else if(Mathf.Abs(velocity.x) > 0.1f){
            state = PlayerState.Running;
        } else {
            state = PlayerState.Idle;
        }

        if(Input.GetKeyDown(KeyCode.X) && oldstate != PlayerState.Blocking){
            state = PlayerState.Blocking;
            blockTimeCurrent = 0.0f;
            health -= blockStaminaCost;
        } else if(Input.GetKeyDown(KeyCode.Z) && oldstate != PlayerState.Attacking){
            state = PlayerState.Attacking;
            attackTimeCurrent = 0.0f;
            health -= attackStaminaCost;
        }

        // Death
		if (health <= 0 || Input.GetKeyDown(KeyCode.K)) {
			state = PlayerState.Dead;
		}

		if (state == PlayerState.Dead) {
			die();
		}

        Animate();
	}

    void Animate(){
        if(oldstate == PlayerState.Attacking){
            attackTimeCurrent += Time.deltaTime;

            if(attackTimeCurrent < attackTime){
                state = PlayerState.Attacking;

                if(attackTimeCurrent > attackWeaponTime && !playerWeapon){
                    playerWeapon = Instantiate(playerWeaponInstance);
                    playerWeapon.transform.position = transform.position;

                    // fixes bug where flipping direction while playing attack animation
                    if (direction == PlayerDirection.Left) {
            			playerWeapon.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            		} else if (direction == PlayerDirection.Right) {
            			playerWeapon.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            		}

                    playerWeapon.transform.parent = transform;
                }
            } else {
                Destroy(playerWeapon);
                state = PlayerState.Idle;
            }
        }

        if(oldstate == PlayerState.Blocking){
            blockTimeCurrent += Time.deltaTime;

            if(blockTimeCurrent < blockTime){
                state = PlayerState.Blocking;
            } else {
                state = PlayerState.Idle;
            }
        }

        // Do the rest only if the state's changed
        if(oldstate == state){
            return;
        }

        // If we're transitioning out of stuff that doesn't need a timer
        if(state == PlayerState.Idle){
            animator.SetTrigger("idle");
        } else if(state == PlayerState.Running){
            animator.SetTrigger("run");
        } else if(state == PlayerState.Jumping){
            animator.SetTrigger("jump");
        }else if(state == PlayerState.Attacking){
            animator.SetTrigger("attack");
        } else if(state == PlayerState.Blocking){
            animator.SetTrigger("block");
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
		state = PlayerState.Idle;
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
            if(state == PlayerState.Blocking){
                audioSource.clip = blockSound;
                audioSource.Play();
            } else {
                WeaponDamage weapon = other.gameObject.GetComponent<WeaponDamage>();
                health -= weapon.damage;

                if(weapon.destroyOnImpact){
                    Destroy(other.gameObject);
                }

                animator.SetTrigger("hit");
                audioSource.clip = hitSound;
                audioSource.Play();

                // Create blood
                GameObject blood = Instantiate(bloodParticles);
                blood.transform.position = transform.position;
            }
        }

        if (other.tag == HEALTH_PICKUP_TAG) {
        	HealthPickup h = (HealthPickup)other.gameObject.GetComponent<HealthPickup>();
        	Debug.Log("picking up " + h.amount + " health");

        	health += h.amount;
        	healthPickedUpSinceLastDeath += h.amount;
        	Destroy(h.gameObject);

            audioSource.clip = pickupSound;
            audioSource.Play();

        	GameObject particles = Instantiate(healthPickupParticles);
            particles.transform.position = transform.position;
            particles.GetComponent<ParticleImplodeController>().target = gameObject;

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

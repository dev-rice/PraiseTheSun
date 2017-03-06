using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Movable {

    public float moveSpeed = 3.0f;
    public float jumpVelocity = 8.0f;
    public int health;

    public Bonfire bonfire;

	private float acceleration_time_airborne = 0.5f;
	private float acceleration_time_grounded = 0.1f;

    private SpriteRenderer sprite;

    private Rigidbody2D rigidbody;

    private float velocity_x_smoothing;

    private bool isDead;
    private const string BONFIRE_TAG = "Bonfire";

    public enum PlayerDirection {
        Right,
        Left
    }

    public PlayerDirection direction;

	void Start () {
		rigidbody = GetComponent<Rigidbody2D>();
		sprite = GetComponent<SpriteRenderer>();
		direction = PlayerDirection.Right;
		isDead = false;
	}

	// Update is called once per frame
	void Update () {
		Vector2 velocity = rigidbody.velocity;
		if (IsGrounded()) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				velocity.y = jumpVelocity;
			}
		}
		Vector2 input = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		// This smooths the velocity so it feels like you have less impact on your movement when you are in the air
		velocity.x = smoothVelocityX(velocity.x, input.x * moveSpeed);

		rigidbody.velocity = velocity;

		direction = getDirectionFromInput(input);
		flipBasedOnDirection();

		if (Input.GetKeyDown(KeyCode.K)) {
			isDead = true;
		}

		if (isDead) {
			die();
		}
	}

	void die() {
		Debug.Log("You are died.");
		rigidbody.velocity = new Vector2(0, 0);
		transform.position = bonfire.transform.position;
		isDead = false;
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == BONFIRE_TAG) {
			Bonfire new_bonfire = (Bonfire)other.gameObject.GetComponent<Bonfire>();
			new_bonfire.Light();
			this.bonfire = new_bonfire;
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

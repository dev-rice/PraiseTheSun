using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	public float jump_height = 4;
	public float time_to_jump_apex = 0.4f;
	public float moveSpeed = 6;
	public float acceleration_time_airborne = 0.2f;
	public float acceleration_time_grounded = 0.1f;
    
    public float movement_stopped_epsilon = 0.001f;

    private float gravity;
	private float jumpVelocity;
	private Vector3 velocity;
	private float velocity_x_smoothing;

    private Controller2D controller;

    void Start() {
		controller = GetComponent<Controller2D>();

		gravity = -(2 * jump_height) / Mathf.Pow(time_to_jump_apex, 2);
		jumpVelocity = Mathf.Abs(gravity) * time_to_jump_apex;

		velocity = new Vector3(10, jumpVelocity, 0);
	}

	void Update() {
		// Zero out the y velocity if we are touching the ceiling or the ground
		if (controller.isTouchingCeiling() || controller.isTouchingGround()) {
			velocity.y = 0;
		}

		// Get User's input
		Vector2 input = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		if (Input.GetKeyDown(KeyCode.Space) && controller.isTouchingGround()) {
			velocity.y = jumpVelocity;
		}

		// Calculate target velocity
		float target_velocity_x = input.x * moveSpeed;

		float acceleration_time = getAccelerationTime();

		// Apply changes to velocity
        velocity.x = Mathf.SmoothDamp(velocity.x, target_velocity_x, ref velocity_x_smoothing, acceleration_time);
        velocity.y += gravity * Time.deltaTime;

        // Use the Controller2D to calculate the actual amount we will move
		Vector3 actual_amount_moved = controller.CalculateMovementAmount(velocity * Time.deltaTime);
		// Adjust based on the sprite flipping scale
		actual_amount_moved.x *= Mathf.Sign(transform.localScale.x);
		// Actually move that amount
		transform.Translate (actual_amount_moved);
    }

    private float getAccelerationTime() {
    	// Calculate acceleration time
        float acceleration_time;
        if (controller.isTouchingGround()) {
            acceleration_time = acceleration_time_grounded;
        } else {
            acceleration_time = acceleration_time_airborne;
        }
        return acceleration_time;
    }

    private bool isNotMoving(Vector3 position_delta) {
        return position_delta.magnitude <= movement_stopped_epsilon;
    }
}

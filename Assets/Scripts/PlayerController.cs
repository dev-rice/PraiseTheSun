using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Movable {

    public float moveSpeed = 3.0f;
    public float jump_height = 2.0f;
    public float time_to_jump_apex = 0.4f;

    private SpriteRenderer sprite;

    private float gravity;
    private float jumpVelocity;
    private Vector3 velocity;

    public enum PlayerDirection {
        Right,
        Left
    }

    public PlayerDirection direction;

	void Start () {
		sprite = GetComponent<SpriteRenderer>();
		direction = PlayerDirection.Right;

		gravity = -(2 * jump_height) / Mathf.Pow(time_to_jump_apex, 2);
		jumpVelocity = Mathf.Abs(gravity) * time_to_jump_apex;

		velocity = new Vector3(0, 0, 0);
	}

	// Update is called once per frame
	void Update () {
		if (IsGrounded()) {
			velocity.y = 0;

			Vector2 input = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			direction = getDirectionFromInput(input);

			velocity.x = moveSpeed * input.x;

			if (Input.GetKeyDown(KeyCode.Space)) {
				velocity.y = jumpVelocity;
			}
		}

		transform.Translate(velocity * Time.deltaTime);

		handleDirection();
	}

	private void handleDirection() {
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

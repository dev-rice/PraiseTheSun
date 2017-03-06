using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform target;

	public float kp, ki, kd;

	private Camera camera;

	private float bottomBound;
	private float topBound;
	
	private float leftBound;
	private float rightBound;

	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera>();

		bottomBound = 1.0f * camera.pixelHeight / 3.0f;
		topBound = 2.0f * camera.pixelHeight / 3.0f;
		
		leftBound = 1.0f * camera.pixelWidth / 3.0f;
		rightBound = 2.0f * camera.pixelWidth / 3.0f;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 targetScreenPos = camera.WorldToScreenPoint(target.position);

		Vector3 adjustment = new Vector3(0, 0, 0);

        if (targetScreenPos.y <= bottomBound) {
        	Vector3 bottomBoundPosWorld = camera.ScreenToWorldPoint(new Vector2(0, bottomBound));
        	float bottomBoundWorld = bottomBoundPosWorld.y;
        	adjustment.y = target.position.y - bottomBoundWorld;
        }
        if (targetScreenPos.y >= topBound) {
        	Vector3 topBoundPosWorld = camera.ScreenToWorldPoint(new Vector2(0, topBound));
        	float topBoundWorld = topBoundPosWorld.y;
        	adjustment.y = target.position.y - topBoundWorld;
        }

        if (targetScreenPos.x <= leftBound) {
        	Vector3 leftBoundPosWorld = camera.ScreenToWorldPoint(new Vector2(leftBound, 0));
        	float leftBoundWorld = leftBoundPosWorld.x;
        	adjustment.x = target.position.x - leftBoundWorld;
        }
        if (targetScreenPos.x >= rightBound) {
        	Vector3 rightBoundPosWorld = camera.ScreenToWorldPoint(new Vector2(rightBound, 0));
        	float rightBoundWorld = rightBoundPosWorld.x;
        	adjustment.x = target.position.x - rightBoundWorld;
        }

		transform.Translate(adjustment);
	}
}

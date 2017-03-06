using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform target;

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
		centerTargetOnScreenPoint(getDesiredScreenPoint());
	}

	private Vector2 getDesiredScreenPoint() {
		Vector3 targetScreenPos = camera.WorldToScreenPoint(target.position);
		Vector2 screenPoint = targetScreenPos;
        if (targetScreenPos.y <= bottomBound) {
        	screenPoint.y = bottomBound;
        }
        if (targetScreenPos.y >= topBound) {
        	screenPoint.y = topBound;
        }

        if (targetScreenPos.x <= leftBound) {
        	screenPoint.x = leftBound;
        }
        if (targetScreenPos.x >= rightBound) {
        	screenPoint.x = rightBound;
        }
        return screenPoint;
	}


	// Center target on a screen point
	private void centerTargetOnScreenPoint(Vector2 screenPoint) {
		// Screen point in world coordinates
		Vector3 screenPointWorld = camera.ScreenToWorldPoint(screenPoint);
		Vector3 adjustment = -1 * (screenPointWorld - target.position);
		adjustment.z = 0;

		transform.Translate(adjustment);
	}
}

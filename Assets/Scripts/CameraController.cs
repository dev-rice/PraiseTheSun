using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform target;

	public enum CameraMode {
		TrackOnFocusPoint,
		ConstrainTargetToBounds
	}
	public CameraMode cameraMode;

	public Vector2 focusPoint;

	public float cameraAdjustSpeed;

	private Camera camera_;

	private Vector2 bottomLeftBound;
	private Vector2 topRightBound;

	private float bottomBound;
	private float topBound;

	private float leftBound;
	private float rightBound;

	// Use this for initialization
	void Start () {
		camera_ = GetComponent<Camera>();

		bottomLeftBound = screenRatioToPixels(new Vector2(1.0f/3.0f, 1.0f/3.0f));
		topRightBound = screenRatioToPixels(new Vector2(2.0f/3.0f, 2.0f/3.0f));

		bottomBound = bottomLeftBound.y;
		leftBound = bottomLeftBound.x;
		
		topBound = topRightBound.y;		
		rightBound = topRightBound.x;

	}

	// Update is called once per frame
	void Update () {
		switch (cameraMode) {
			case CameraMode.TrackOnFocusPoint:
				trackOnFocusPoint();
				break;
			case CameraMode.ConstrainTargetToBounds:
				constrainTargetToBounds();
				break;
		}
	}

	private void trackOnFocusPoint() {
		centerTargetOnScreenPoint(screenRatioToPixels(focusPoint));
	}

	private void constrainTargetToBounds() {
		centerTargetOnScreenPoint(getDesiredScreenPoint());
	}

	private Vector2 getDesiredScreenPoint() {
		Vector3 targetScreenPos = camera_.WorldToScreenPoint(target.position);
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
		Vector3 screenPointWorld = camera_.ScreenToWorldPoint(screenPoint);
		Vector3 adjustment = -1 * (screenPointWorld - target.position);
		adjustment.z = 0;

		Vector3 adjusted = transform.position + adjustment;
		transform.position = Vector3.Lerp(transform.position, adjusted, cameraAdjustSpeed * Time.deltaTime);
	}

	private Vector2 screenRatioToPixels(Vector2 screenRatio) {
		return new Vector2(screenRatio.x * camera_.pixelWidth, screenRatio.y * camera_.pixelHeight);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public enum CameraMode {
		TrackOnFocusPoint,
		ConstrainTargetToBounds
	}

	[Header("Tracking Settings")]
	public Transform target;
	public float cameraAdjustSpeed;

	[Header("Camera Modes")]
	public CameraMode cameraMode;

	[Header("Focus Point")]
	public Vector2 focusPoint;

	[Header("Bounds")]
	public Vector2 bottomLeftBound;
	public Vector2 topRightBound;

	private Camera camera_;

	private float bottomBound;
	private float topBound;

	private float leftBound;
	private float rightBound;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;
		camera_ = GetComponent<Camera>();

		Vector2 bottomLeftBoundPx = screenRatioToPixels(bottomLeftBound);
		Vector2 topRightBoundPx = screenRatioToPixels(topRightBound);

		bottomBound = bottomLeftBoundPx.y;
		leftBound = bottomLeftBoundPx.x;

		topBound = topRightBoundPx.y;
		rightBound = topRightBoundPx.x;

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

	// TODO: Should the focus point be dependent on player direction? For example when the player is moving right we want focus.x to be 1/3, when the player is moving left should the focus be 2/3? Maybe there should be a left and right focus point defined in here.
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour {

	public CameraController cameraController;
	public CameraController.CameraMode cameraMode;
	public Vector2 focusPoint;

	private string PLAYER_TAG = "Player";

	private CameraController.CameraMode prevCameraMode;
	private Vector2 prevFocusPoint;
	
	// Use this for initialization
	void Start () {
		prevCameraMode = CameraController.CameraMode.ConstrainTargetToBounds;
		prevFocusPoint = new Vector2(0.5f, 0.5f);
	}
	
	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == PLAYER_TAG) {
			prevCameraMode = cameraController.cameraMode;
			cameraController.cameraMode = cameraMode;

			prevFocusPoint = cameraController.focusPoint;
			cameraController.focusPoint = focusPoint;
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.tag == PLAYER_TAG) {
			cameraController.cameraMode = cameraMode;
			cameraController.focusPoint = focusPoint;
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.tag == PLAYER_TAG) {
			cameraController.cameraMode = prevCameraMode;
			cameraController.focusPoint = prevFocusPoint;
		}
	}
}

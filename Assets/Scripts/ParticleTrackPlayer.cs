using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrackPlayer : MonoBehaviour {

	public GameObject target;

	private float distanceToKill = 0.2f;
	private float baseTrackSpeed = 6.0f;

	// Update is called once per frame
	void Update () {
		if (target == null) {
			return;
		}

		Vector2 vectorToPlayer = (Vector2)transform.position - (Vector2)target.transform.position;
		if (vectorToPlayer.magnitude <= distanceToKill) {
			Destroy(gameObject);
		}

		float trackSpeed = vectorToPlayer.magnitude * baseTrackSpeed;
		trackSpeed = Mathf.Max(baseTrackSpeed, trackSpeed);
		trackSpeed = trackSpeed * Random.Range(0.5f, 1.5f);

		transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * trackSpeed);
	}
}

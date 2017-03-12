using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public int healthDropped;
	public GameObject healtPickupPrefab;

	public void die() {
		dropHealthPickup();
	}

	void dropHealthPickup() {
		GameObject g = Instantiate(healtPickupPrefab);
		g.transform.position = transform.position;
		HealthPickup h = (HealthPickup)g.GetComponent<HealthPickup>();
		h.amount = healthDropped;
	}

}

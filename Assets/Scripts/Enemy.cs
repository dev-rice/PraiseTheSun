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
		// This ensures that the health pickups get deleted when the enemies get deleted
		g.transform.SetParent(transform.parent);
		
		HealthPickup h = (HealthPickup)g.GetComponent<HealthPickup>();
		h.amount = healthDropped;
	}

}

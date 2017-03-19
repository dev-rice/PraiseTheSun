using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public GameObject allEnemiesPrefab;
	public GameObject enemies;

	// Use this for initialization
	void Start () {
		// enemies = Instantiate(allEnemiesPrefab);
	}

	// Update is called once per frame
	void Update () {

	}

	public void playerDied() {
		Destroy(enemies);

        // iterate through all things with enemy tag, weapon tag
        GameObject[] enems = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enems) {
            Destroy(enemy);
        }

        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");

        foreach (GameObject weapon in weapons) {
            Destroy(weapon);
        }

		enemies = Instantiate(allEnemiesPrefab);
	}
}

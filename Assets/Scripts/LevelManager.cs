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
		enemies = Instantiate(allEnemiesPrefab);
	}
}

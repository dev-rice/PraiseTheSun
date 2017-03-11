using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : Movable {
    public GameObject camera_;

	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(camera_.transform.position.x * 0.8f, camera_.transform.position.y, 0.0f);
	}
}

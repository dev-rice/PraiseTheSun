using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyReticleController : Movable {
	void Update () {
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousepos.z = 0.0f;

        transform.position = mousepos;
	}
}

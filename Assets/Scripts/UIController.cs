using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    private float originalWidth = 1.0f / 16.0f;

    public PlayerController player;
    public GameObject leftBar;
    public GameObject rightBar;

	void Update () {
        float scalex = (float)player.health / 10.0f;
        scalex = Mathf.Max(1.0f, scalex);

        float halfwidth = scalex * originalWidth * 0.5f;

        transform.localScale = new Vector3(scalex, 1.0f, 1.0f);
        leftBar.transform.position = new Vector3(transform.position.x - halfwidth, transform.position.y, 0.0f);
        rightBar.transform.position = new Vector3(transform.position.x + halfwidth, transform.position.y, 0.0f);
	}
}

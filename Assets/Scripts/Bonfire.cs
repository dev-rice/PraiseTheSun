using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonfire : MonoBehaviour {

	private bool lit;
	private Animator animator;

	// Use this for initialization
	void Awake () {
		lit = false;
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		animator.SetBool("lit", lit);
	}

	public void Light() {
		lit = true;
	}

	public bool isLit() {
		return lit;
	}
}

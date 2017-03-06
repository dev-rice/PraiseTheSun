using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonfire : MonoBehaviour {

	private bool isLit;
	private SpriteRenderer sprite;
	private Animator animator;

	// Use this for initialization
	void Awake () {
		isLit = false;
		sprite = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		animator.SetBool("lit", isLit);
	}

	public void Light() {
		isLit = true;
	}
}

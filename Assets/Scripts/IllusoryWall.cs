using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusoryWall : MonoBehaviour {

	SpriteRenderer sprite;

	// Use this for initialization
	void Start () {
		sprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other) {
    	if (other.tag == "Weapon") {
			StartCoroutine(FadeTo(0.0f, 0.5f));
    	}
	}

	IEnumerator FadeTo(float aValue, float aTime) {
		float alpha = sprite.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
			Color c = sprite.color;
			c.a = Mathf.Lerp(alpha, aValue, t);
			sprite.color = c;
		 	yield return null;
		}
		
		Color newColor = sprite.color;	
		newColor.a = aValue;
		sprite.color = newColor;

    	Destroy(gameObject);
 	}
}

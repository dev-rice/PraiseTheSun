using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBanner : MonoBehaviour {

	public Text messageText;
	public float bannerFadeTime;

	private CanvasGroup canvasGroup;
	private bool shown;
	private float shownTime;

	// Use this for initialization
	void Start () {
		canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0;
		hide();
	}
	
	// Update is called once per frame
	void Update () {
		if (shown && (Time.time - shownTime >= bannerFadeTime)) {
			hide();
		}
	}

	private void show() {
		StartCoroutine(FadeTo(1.0f, 0.5f));
		shownTime = Time.time;
		shown = true;
	}

	private void hide() {
		StartCoroutine(FadeTo(0.0f, 0.5f));
		shown = false;
	}

	public void showMessage(string message) {
		messageText.text = message;
		show();
	}

	IEnumerator FadeTo(float aValue, float aTime) {
		float alpha = canvasGroup.alpha;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
			canvasGroup.alpha = Mathf.Lerp(alpha,aValue,t);
		 	yield return null;
		}
		canvasGroup.alpha = aValue;
 	}
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenController : MonoBehaviour {

    public float fadeTime;
    private float fadeTimeCurrent;

    public GameObject title;

    public enum TitleState {
        BlackFade,
        Title
    }

    public TitleState state;

    private SpriteRenderer renderer_;
    public Sprite titleSprite;

	void Start () {
        renderer_ = title.GetComponent<SpriteRenderer>();
	}

	void Update () {
        fadeTimeCurrent += Time.deltaTime;

        if(state == TitleState.BlackFade){
            float brightness = ((int)((fadeTimeCurrent / fadeTime) * 12.0f)) / 12.0f;
            renderer_.color = new Color(brightness, brightness, brightness);

            if(fadeTimeCurrent > fadeTime * 1.5){
                state = TitleState.Title;
            }
        } else if(state == TitleState.Title){
            renderer_.sprite = titleSprite;

            if(Input.anyKey){
                Destroy(title);
                this.enabled = false;
            }
        }
	}
}

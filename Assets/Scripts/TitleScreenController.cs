using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenController : MonoBehaviour {

    public float fadeTime;
    private float fadeTimeCurrent;

    public GameObject title;

    public enum TitleState {
        BlackFade,
        Title,
        Nope
    }

    public TitleState state;

    private SpriteRenderer renderer_;
    public Sprite titleSprite;
    private GameObject player;
    public GameObject tptransform;

	void Start () {
        renderer_ = title.GetComponent<SpriteRenderer>();

        player = GameObject.FindWithTag("Player");
        if(!player){
            Debug.LogError("Couldn't find player gameobject.");
        }
        player.GetComponent<PlayerController>().enabled = false;
        GetComponent<JukeboxController>().enabled = false;
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
            if(renderer_.sprite != titleSprite){
                renderer_.sprite = titleSprite;
            }

            if(Input.anyKey){
                // teleport player and camera to first bonfire
                // transform.position = tptransform.transform.position;
                player.transform.position = tptransform.transform.position;

                // enable player and jukebox
                player.GetComponent<PlayerController>().enabled = true;
                GetComponent<JukeboxController>().enabled = true;

                renderer_.enabled = false;
                state = TitleState.Nope;
            }
        }
	}
}

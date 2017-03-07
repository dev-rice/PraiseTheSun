using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTrapController : Movable {
    public Sprite closedSprite;
    public GameObject weaponInstance;
    private GameObject trappedObject;

    private bool triggered; // REEEEEEEEEEE
    public float trappedTime;
    private float currentTime;

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy"){
            // set sprite
            GetComponent<SpriteRenderer>().sprite = closedSprite;

            // create weapon
            GameObject weapon = Instantiate(weaponInstance);
            weapon.transform.position = transform.position;

            triggered = true;
            trappedObject = other.gameObject;
        }
    }

    void Update(){
        if(!triggered){
            return;
        }

        currentTime += Time.deltaTime;

        if(currentTime > trappedTime){
            Destroy(gameObject);
        }
    }

    void LateUpdate(){
        if(triggered){
            // Override trapped character position
            trappedObject.transform.position = transform.position + new Vector3(0.5f, 0.5f, 0.0f);
        }
    }
}

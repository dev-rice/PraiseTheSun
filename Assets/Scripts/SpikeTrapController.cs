using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapController : Movable {

    public Sprite bloodiedSprite;
    public int damage;
    private bool triggered;

    private GameObject target;
    private float targetx;

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.gameObject.tag == "Player" || collider.gameObject.tag == "Enemy"){
            if(collider.gameObject.GetComponent<Rigidbody2D>().velocity.y < 0.0f){
                triggered = true;
                GetComponent<SpriteRenderer>().sprite = bloodiedSprite;
                target = collider.gameObject;
                targetx = collider.gameObject.transform.position.x;
            }
        }
    }

    void LateUpdate(){
        if(triggered){
            target.transform.position = new Vector3(targetx, target.transform.position.y, 0.0f);
        }
    }
}

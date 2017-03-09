using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class WeaponDamage : MonoBehaviour {
    public bool destroyOnImpact;
    public int damage;
    private int frames = 15;

    void Start(){
        tag = "Weapon"; // Just in case
    }

    void Update(){
        frames--;

        if(frames == 0){
            Destroy(gameObject);
        }
    }
}

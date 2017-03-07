using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class WeaponDamage : MonoBehaviour {
    public bool destroyOnImpact;
    public int damage;

    void Start(){
        tag = "Weapon"; // Just in case
    }
}

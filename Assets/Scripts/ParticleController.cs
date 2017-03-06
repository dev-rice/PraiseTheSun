using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour {

    public int maxParticles;
    public float maxStartSpeed;

    public GameObject particlesInstance;

    void Start(){
        for(int i = 0; i < maxParticles; ++i){
            GameObject particle = Instantiate(particlesInstance);
            particle.transform.position = transform.position;
            Vector3 velocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 1.0f), 0.0f);
            velocity.Normalize();
            particle.GetComponent<Rigidbody2D>().velocity = velocity * Random.Range(0.0f, maxStartSpeed);
        }
        Destroy(gameObject);
    }
}

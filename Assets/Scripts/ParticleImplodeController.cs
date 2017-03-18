using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleImplodeController : MonoBehaviour {

	public int maxParticles;

    public GameObject particlesInstance;
    public GameObject target;

    private float minRadius = 1.0f;
    private float maxRadius = 4.0f;

    private float particleLifetimeSeconds = 0.5f;

    void Start(){
        for(int i = 0; i < maxParticles; ++i){
        	float radius = Random.Range(minRadius, maxRadius);
        	float radians = 2.0f * Mathf.PI * ((float)i / (float)maxParticles);
        	Vector2 point = new Vector2(radius * Mathf.Cos(radians), radius * Mathf.Sin(radians)) + (Vector2)transform.position;

            NewParticle(point, particleLifetimeSeconds);
        }
        Destroy(gameObject);
    }

    void NewParticle(Vector3 position, float seconds) {
        GameObject particle = Instantiate(particlesInstance);

        particle.transform.position = position;

        particle.GetComponent<Rigidbody2D>().gravityScale = 0;

        particle.GetComponent<EffectsController>().lifetimeSeconds = seconds;

        particle.GetComponent<ParticleTrackPlayer>().target = target;
    }
}

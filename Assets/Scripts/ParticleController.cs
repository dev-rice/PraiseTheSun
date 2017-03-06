using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleController : MonoBehaviour {

    public float lifetime;
    private float currentTime;

    ParticleSystem particleSystem_;
    ParticleSystem.Particle[] particles_;
    float[] cachedParticlePositions;

    void Start(){
        particleSystem_ = GetComponent<ParticleSystem>();
        particles_ = new ParticleSystem.Particle[particleSystem_.main.maxParticles];
        cachedParticlePositions = new float[particleSystem_.main.maxParticles * 2];
    }

    void Update(){
        currentTime += Time.deltaTime;
        if(currentTime > lifetime){
            Destroy(gameObject);
        }
    }

    /*private void OnWillRenderObject(){
        int liveParticleCount = particleSystem_.GetParticles(particles_);

        // Change only the particles that are alive
        for (int i = 0; i < liveParticleCount * 2; i++){
            // particles_[i];
            float x_ = particles_[i/2].position.x;
            float y_ = particles_[i/2].position.y;

            // get int position
            int xint = (int)x_;
            int yint = (int)y_;

            // get fractional part of position
            float xfrac = x_ - (float)xint;
            float yfrac = y_ - (float)yint;

            // get pixel position, rounded
            int xpx = (int)((xfrac * 16.0f) - 0.5f);
            int ypx = (int)((yfrac * 16.0f) - 0.5f);

            // turn back into float
            float xfinal = (float)xint + ((float)xpx / 16.0f);
            float yfinal = (float)yint + ((float)ypx / 16.0f);

            particles_[i/2].position = new Vector3(xfinal, yfinal, 0.0f);
            cachedParticlePositions[i] = x_;
            cachedParticlePositions[i + 1] = y_;
        }

        particleSystem_.SetParticles(particles_, liveParticleCount);
    }

    void OnRenderObject() {
        int liveParticleCount = particleSystem_.GetParticles(particles_);

        // restore the true position
        for (int i = 0; i < liveParticleCount * 2; i++){
            particles_[i/2].position = new Vector3(cachedParticlePositions[i], cachedParticlePositions[i + 1], 0.0f);
        }
    }*/
}

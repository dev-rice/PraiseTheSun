using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeboxController : MonoBehaviour {

    public AudioClip[] tracks;
    private int tracknumber;
    AudioSource audioSource;

	void Start(){
        audioSource = GetComponent<AudioSource>();
        tracknumber = 0;
        Shuffle();
	}

	void Update(){
        if (!audioSource.isPlaying) {
            if(tracknumber > tracks.Length - 1){
                tracknumber = 0;
                Shuffle();
            }

            audioSource.clip = tracks[tracknumber];
            audioSource.Play();
            tracknumber++;
        }
	}

    void Shuffle(){
        for(int i = 0; i < tracks.Length - 2; ++i){
            // get random value i <= j < remaining elements
            int j = (int)((Random.value * tracks.Length - i) + i);

            // if we're the first element, don't take the last element
            if(i == 0 && j == tracks.Length - 1){
                j--;
            }

            AudioClip temp = tracks[i];
            tracks[i] = tracks[j];
            tracks[j] = temp;
        }
    }
}

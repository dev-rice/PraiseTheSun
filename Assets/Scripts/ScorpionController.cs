using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorpionController : Movable {
    // Scorpion: retreats or advances keeping player at a distance, then lunges
    // occasionally to try and jab player with claws or stab with tail

    // State handling
    public enum ScorpionState {
        Idle,
        ClawingOut,
        ClawingIn,
        TailJabOut,
        TailJabIn,
        Dead
    }

    [Header("Scorpion State")]
    public ScorpionState state;
    public float speed;
    public float playerNearDistance;
    public float playerFarDistance;
    private GameObject player;
    public float idleTime;
    private float idleCurrentTime;
    private bool clawNext;

    [Header("Health")]
    public int health;

    [Header("Tail Controls")]
    // There is no god
    public Vector3 tailJoint1;
    public Vector3 tailJoint2;
    private Vector3 tailJoint2Target = new Vector3(-0.5f, 2.0f, 0.0f);
    public Vector3 tailJoint3;
    private Vector3 tailJoint3Target = new Vector3(-2.5f, 0.25f, 0.0f);
    private Vector3 realJoint1;
    private Vector3 realJoint2Target;
    private Vector3 realJoint2;
    private Vector3 realJoint3Target;
    private Vector3 realJoint3;
    public int tailSegments;
    private GameObject[] tailSprites;
    public GameObject tailSprite;
    public GameObject stingerSprite;
    public float tailJabTime;
    private float currentTailJabTime;

    
    public GameObject bloodParticles;

    // enemy reference (for dying/respawning)
    private Enemy enemy;

    public MessageBanner banner;
    private float playerWonTime;
    private bool playerWon = false;

	void Start(){
        currentTailJabTime = 0.0f;

        stingerSprite = Instantiate(stingerSprite);

        tailSprites = new GameObject[tailSegments - 2];

        for(int i = 0; i < tailSegments - 2; ++i){
            tailSprites[i] = Instantiate(tailSprite);
        }

        player = GameObject.FindWithTag("Player");
        if(!player){
            Debug.LogError("Couldn't find player gameobject.");
        }

        enemy = GetComponent<Enemy>();

	}

	void Update(){
        if(state != ScorpionState.Dead){
            if(state == ScorpionState.Idle){
                float direction = 0.0f;
                float distance = (transform.position - player.transform.position).magnitude;

                if(distance < playerFarDistance){
                    direction = -1.0f;
                }

                if(distance < playerNearDistance - 0.1f){
                    direction = 1.0f;
                }

                // stop oscillating
                if(distance < playerNearDistance + 0.1f && distance > playerNearDistance - 0.1f){
                    direction = 0.0f;
                }

                // must be on same y level
                if(Mathf.Abs(transform.position.y - player.transform.position.y) > 1.5f){
                    direction = 0.0f;
                }

                transform.position += direction * Vector3.right * speed * Time.deltaTime;

                idleCurrentTime += Time.deltaTime;

                if(idleCurrentTime > idleTime){
                    state = clawNext ? ScorpionState.ClawingOut : ScorpionState.TailJabOut;
                    clawNext = !clawNext;
                    clawNext = !clawNext;
                }
            } else if(state == ScorpionState.ClawingOut){

            } else if(state == ScorpionState.ClawingIn){

            } else if(state == ScorpionState.TailJabOut){
                currentTailJabTime += Time.deltaTime;

                if(currentTailJabTime > tailJabTime){
                    state = ScorpionState.TailJabIn;
                    currentTailJabTime = tailJabTime;
                }
            } else if(state == ScorpionState.TailJabIn){
                currentTailJabTime -= Time.deltaTime;

                if(currentTailJabTime < 0.0f){
                    state = ScorpionState.Idle;
                    currentTailJabTime = 0.0f;
                    idleCurrentTime = 0.0f;
                }
            }

            // Update this last
            UpdateTail();
        }

        if(health <= 0){
            // set state and animation state
            state = ScorpionState.Dead;

            // Create blood
            GameObject blood = Instantiate(bloodParticles);
            blood.transform.position = transform.position;

            // Create blood
            GameObject blood2 = Instantiate(bloodParticles);
            blood2.transform.position = transform.position + new Vector3(1, 0, 0);

            // Create blood
            GameObject blood3 = Instantiate(bloodParticles);
            blood3.transform.position = transform.position + new Vector3(-1, 0, 0);

            enemy.die();

            banner.showMessage("YOU WIN!");
            playerWon = true;
            playerWonTime = Time.time;

        }

        if(playerWon && Input.anyKey && playerWonTime - Time.time > 1.0f){
            Application.LoadLevel(Application.loadedLevel);
        }
	}

    void UpdateTail(){
        // move to start() when done with manual stuff
        realJoint1 = transform.position + tailJoint1;
        realJoint2 = transform.position + tailJoint2;
        realJoint3 = transform.position + tailJoint3;
        realJoint2Target = transform.position + tailJoint2Target;
        realJoint3Target = transform.position + tailJoint3Target;
        //
        float jabt = currentTailJabTime / tailJabTime;
        jabt = Mathf.Clamp(jabt, 0.0f, 1.0f);

        Vector3 finalJoint2 = Vector3.Lerp(realJoint2, realJoint2Target, jabt);
        Vector3 finalJoint3 = Vector3.Lerp(realJoint3, realJoint3Target, jabt);

        Debug.DrawLine(realJoint1, finalJoint2, Color.blue);
        Debug.DrawLine(finalJoint2, finalJoint3, Color.blue);

        // Bezier curve
        Vector3 lastp = realJoint1;
        for(int i = 0; i < tailSegments; ++i){
            float t = (float)i/(float)(tailSegments - 1);
            if(i != 0){
                Vector3 a = Vector3.Lerp(realJoint1, finalJoint2, t);
                Vector3 b = Vector3.Lerp(finalJoint2, finalJoint3, t);
                Vector3 p = Vector3.Lerp(a, b, t);
                Debug.DrawLine(lastp, p);
                lastp = p;
                // don't draw on first or last points
                if(i < tailSegments - 1){
                    tailSprites[i - 1].transform.position = p;
                }
            }
        }

        stingerSprite.transform.position = finalJoint3;
    }

     void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Weapon" && health > 0.0f){
            // remove health
            WeaponDamage weapon = other.gameObject.GetComponent<WeaponDamage>();

            if(!weapon.hurtCreator || weapon.creator == gameObject){
                health -= weapon.damage;
            } else {
                return;
            }

            if(weapon.destroyOnImpact){
                Destroy(other.gameObject);
            }

            // // play hit animation and sound
            // audioSource.clip = hitSound;
            // audioSource.Play();

            // Create blood
            GameObject blood = Instantiate(bloodParticles);
            blood.transform.position = transform.position;
        }
    }
}

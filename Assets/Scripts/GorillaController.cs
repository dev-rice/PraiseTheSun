using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorillaController : Movable {
    // Gorillas simply idle until player gets close enough
    // then leap towards them attacking

    // State handling
    public enum GorillaState {
        Idle,
        Crouching1,
        Crouching2,
        Jumping,
        Dead,
    }

    public enum GorillaDirection {
        Right,
        Left
    }

    [Header("Lion State")]
    public GorillaState state;
    public GorillaDirection direction;

    [Header("Attack Settings")]
    public float aggroDistance;
    public float attackSpeed;
    public float jumpDelay;
    private float jumpElapsed;
    private int crouchTimes = 3;
    private int crouchTimesCurrent;
    public GameObject bloodParticles;

    [Header("Health Settings")]
    public int health;

    [Header("Health Settings")]
    public Sprite idleSprite;
    public Sprite crouchingSprite;
    public Sprite jumpingSprite;
    public Sprite deathSprite;

    [Header("Sound Effects")]
    public AudioClip hitSound;
    public AudioClip growlSound;

    // component references
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    // gameobject references
    private GameObject player;

	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindWithTag("Player");
        if(!player){
            Debug.LogError("Couldn't find player gameobject.");
        }
	}

	void Update () {
        if(state == GorillaState.Dead){
            if(!IsGrounded()){
                return;
            }

            // turn off physics
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<BoxCollider2D>().enabled = false;

            // don't let snapping happen
            // CacheTransformAndPixelSnap();
            // snap = PixelSnap.Never;

            return;
        }

        if(IsGrounded()){
            if(state == GorillaState.Idle){
                spriteRenderer.sprite = idleSprite;

                crouchTimesCurrent = 0;

                if((transform.position - player.transform.position).magnitude < aggroDistance){
                    direction = transform.position.x > player.transform.position.x ? GorillaDirection.Left : GorillaDirection.Right;
                    spriteRenderer.flipX = direction == GorillaDirection.Left;

                    state = GorillaState.Crouching1;

                    audioSource.clip = growlSound;
                    audioSource.Play();
                }
            } else if(state == GorillaState.Crouching1){
                if(jumpElapsed > jumpDelay){
                    jumpElapsed = 0.0f;
                    state = GorillaState.Crouching2;
                    spriteRenderer.sprite = idleSprite;
                }

                jumpElapsed += Time.deltaTime;

            } else if(state == GorillaState.Crouching2){
                if(jumpElapsed > jumpDelay){
                    jumpElapsed = 0.0f;
                    state = GorillaState.Crouching1;
                    spriteRenderer.sprite = crouchingSprite;
                    crouchTimesCurrent++;
                }

                jumpElapsed += Time.deltaTime;

                if(crouchTimesCurrent > crouchTimes){
                    state = GorillaState.Jumping;
                }
            } else if(state == GorillaState.Jumping){
                // set direction before jumping
                direction = transform.position.x > player.transform.position.x ? GorillaDirection.Left : GorillaDirection.Right;
                spriteRenderer.flipX = direction == GorillaDirection.Left;

                // set velocity
                float v_x = (player.transform.position.x - transform.position.x) / attackSpeed;
                float v_y = ((player.transform.position.y - transform.position.y) / attackSpeed) + 0.5f * 9.8f * attackSpeed;
                GetComponent<Rigidbody2D>().velocity = new Vector3(v_x, v_y, 0.0f);

                state = GorillaState.Idle;
            }
        } else {
            spriteRenderer.sprite = jumpingSprite;
        }

        if(health <= 0){
            // set state and animation state
            state = GorillaState.Dead;
            spriteRenderer.sprite = deathSprite;
        }
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

            audioSource.clip = hitSound;
            audioSource.Play();

            // Create blood
            GameObject blood = Instantiate(bloodParticles);
            blood.transform.position = transform.position;
        }
    }
}

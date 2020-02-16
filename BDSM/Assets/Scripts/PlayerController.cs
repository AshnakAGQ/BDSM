using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

public class PlayerController : MonoBehaviour, IDamageable, IHealable, IMassive
{
    new Rigidbody2D rigidbody2D;
    new Collider2D collider2D;
    Animator animator;
    SpriteRenderer spriteRenderer;
    private AudioPlayer audioPlayer;
    [SerializeField] Transform aimingCircle = null;

    public Vector2 direction;
    public Vector2 lookDirection;

    public GameObject healthBar;

    static private int numPlayersDead;
    UnityEvent playerDies = new UnityEvent();
    UnityEvent playerRevives = new UnityEvent();

    [Header("Sound")]
    [SerializeField] public AudioContainer footStepClip;
    
    [Header("Stats")]
    [SerializeField] float speed = 5f;
    float stun = 0;
    [SerializeField] float maxHealth = 200f;
    [SerializeField] float health = 100f;
    bool alive = true;
    [System.NonSerialized] public bool falling = false;

    enum Directions { Up, Right, Down, Left }
    
    List<IInteractable> interactables;
    List<PlayerController> playersTouching;

    public static GameObjectUnityEvent PitEvent = new GameObjectUnityEvent();

    public float Speed { get => speed; set => speed = value; }
    public float Stun { get => stun; }
    public bool Alive { get => alive; set => alive = value; }
    public Vector3 LastPosition { get => lastPosition; set => lastPosition = value; }
    public bool CanLook { get => canLook; set => canLook = value; }

    bool reviving = false;
    float progress = 0;
    float goal = 2;
    
    ParticleSystem BloodParticles = null;
    float bleedCooldown = 0.3f;
    float bleedTimer = 0;
    bool bleed = false;
    Vector3 lastPosition;
    bool canLook = true;

    private void Awake()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        collider2D = this.GetComponent<Collider2D>();
        animator = this.GetComponent<Animator>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        audioPlayer = this.GetComponent<AudioPlayer>();
        interactables = new List<IInteractable>();
        playersTouching = new List<PlayerController>();
        BloodParticles = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        playerDies.AddListener(AddNumDeadPlayers);
        playerRevives.AddListener(SubtractNumDeadPlayers);
        numPlayersDead = 0;
        bleedCooldown = BloodParticles.main.duration;
    }
   

    private void Update()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

        if (Time.timeScale == 1)
        {
            if (Alive)
            {
                if (bleedTimer > 0) bleedTimer -= Time.deltaTime;
                else if (bleed)
                {
                    bleed = false;
                    BloodParticles.Play();
                    bleedTimer = bleedCooldown;
                }
                if (stun > 0) stun -= Time.deltaTime;
                else 
                {
                    if (!falling)
                    {
                        // Movement
                        rigidbody2D.velocity = direction * speed;

                        if (direction != Vector2.zero)
                        {
                            animator.SetBool("moving", true);

                            if (!audioPlayer.ActiveSounds.ContainsKey(footStepClip))
                            {
                                audioPlayer.PlaySFX(footStepClip);
                            }
                        }
                        else
                            animator.SetBool("moving", false);
                    }
                       
                    // Aiming
                    if (lookDirection != Vector2.zero)
                    {
                        aimingCircle.right = lookDirection;

                        if (System.Math.Abs(lookDirection.y) > System.Math.Abs(lookDirection.x))
                        {
                            if (lookDirection.y > 0)
                                animator.SetInteger("direction", (int)Directions.Up);
                            else
                                animator.SetInteger("direction", (int)Directions.Down);
                        }
                        else
                        {
                            if (lookDirection.x > 0)
                                animator.SetInteger("direction", (int)Directions.Right);
                            else
                                animator.SetInteger("direction", (int)Directions.Left);
                        }
                    }
                }
            }
            else if (reviving)
            {
                progress += Time.deltaTime;
                GetComponent<SpriteRenderer>().color = Color.Lerp(Color.yellow, Color.green, progress / goal);
                if (progress >= goal)
                {
                    RevivePlayer();
                    StopRevive();
                }
            }
            else
            {
                if (bleedTimer > 0) bleedTimer -= Time.deltaTime;
                else if (lastPosition != transform.position)
                {
                    bleed = false;
                    BloodParticles.Play();
                    bleedTimer = bleedCooldown;
                }
            }
            lastPosition = transform.position;
        }
    }

    void OnMove(InputValue value)
    {
        direction = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        if (canLook) lookDirection = value.Get<Vector2>();
    }

    void OnLookMouse(InputValue value)
    {
        if (canLook) lookDirection = (Vector2)(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position);
    }

    public void RevivePlayer()
    {
        health = maxHealth / 2;
        alive = true;
        transform.right = transform.up;
        playerRevives.Invoke();
        GetComponent<SpriteRenderer>().color = Color.white;
        healthBar.GetComponent<Image>().fillAmount = health / maxHealth;
        rigidbody2D.drag = 0;
    }

    void OnInteract(InputValue value)
    {
        if (Time.timeScale == 1 && Alive && stun <= 0 && value.isPressed)
        {
            foreach (IInteractable obj in interactables)
                obj.Interact();
            foreach (PlayerController obj in playersTouching)
                obj.StartRevive();
        }
        else
        {
            foreach (PlayerController obj in playersTouching)
                obj.StopRevive();
        }
    }

    private void StopRevive()
    {
        reviving = false;
        progress = 0;
    }

    private void StartRevive()
    {
        if (!alive)
        {
            reviving = true;
            progress = 0;
        }
    }

    void OnPause(InputValue value)
    {
        if (UI_Manager.instance != null)
        {
            UI_Manager.instance.Pause(Time.timeScale == 1);
        }

    }

    void OnContinue(InputValue value)
    {
        if(UI_Manager.instance.gameOverMenu != null && UI_Manager.instance.gameOverMenu.activeSelf)
        {
            if (UI_Manager.instance.gameWon)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        IPickupable itemComponent = collider.GetComponent<IPickupable>();
        if (itemComponent != null)
        {
            itemComponent.pickUp();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (!collision.collider.GetComponent<PlayerController>().Alive) collision.collider.GetComponent<SpriteRenderer>().color = Color.yellow;
            playersTouching.Add(collision.collider.GetComponent<PlayerController>());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<SpriteRenderer>().color = Color.white;
            playersTouching.Remove(collision.collider.GetComponent<PlayerController>());
            collision.collider.GetComponent<PlayerController>().StopRevive();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable obj = collision.GetComponent<IInteractable>();
        if (obj != null)
        {
            collision.GetComponent<SpriteRenderer>().color = Color.yellow;
            interactables.Add(obj);
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable obj = collision.GetComponent<IInteractable>();
        if (obj != null)
        {
            collision.GetComponent<SpriteRenderer>().color = Color.white;
            interactables.Remove(obj);
        }
        
    }

    public void Damage(float damage, float stun, Vector2 knockback)
    {
        WarriorController warriorCheck = GetComponent<WarriorController>();
        if (warriorCheck && warriorCheck.Blocking && Vector2.Angle(lookDirection, -knockback) < warriorCheck.BlockAngle / 2)
            return;
        if (health > 0)
        {
            rigidbody2D.velocity = Vector2.zero;
            if (damage > 0) bleed = true;
            health -= damage;
            healthBar.GetComponent<Image>().fillAmount = health / maxHealth;
            rigidbody2D.AddForce(knockback);
            this.stun = stun;
        }
        if (Alive && health <= 0)
        {
            health = 0;
            healthBar.GetComponent<Image>().fillAmount = 0;
            Alive = false;
            transform.right = -transform.up;
            playerDies.Invoke();
            rigidbody2D.drag = 10f;
        }
    }

    public void Heal(float healthDelta)
    {
        float newHealthValue = health + healthDelta;
        if (newHealthValue > maxHealth)
            health = maxHealth;
        else
            health = newHealthValue;
        
        healthBar.GetComponent<Image>().fillAmount = health / maxHealth;
        //SetHealthUI();

        //m_audioPlayer.playSFX(healFileName, healVolume, healPitchMin, healPitchMax);
    }

    private void AddNumDeadPlayers()
    {
        numPlayersDead += 1;
        if (numPlayersDead >= 2)
        {
            UI_Manager.GameOver();
        }
    }

    private void SubtractNumDeadPlayers()
    {
        numPlayersDead -= 1;
    }

    public void Fall(float fallingRate)
    {
        if (!falling)
        {
            rigidbody2D.velocity = Vector2.zero; // Stop moving around, you're falling in a pit!
            falling = true;
            PitEvent.Invoke(this.gameObject); // Tell the rope to start shrinking and tugging on the other players
            StartCoroutine(fallCoroutine(fallingRate));
        }
    }

    private IEnumerator fallCoroutine(float fallingRate)
    {
        Vector3 fallingModifier = new Vector3(-0.1f, -0.1f, -0.1f);
        while (this.transform.localScale.x > 0 && this.transform.localScale.y > 0 && this.transform.localScale.z > 0)
        {
            this.transform.localScale += (fallingRate * fallingModifier); // slowly shrink the scale of the object until it disappears
            yield return null;
        }
        spriteRenderer.enabled = false;
        alive = false;
        playerDies.Invoke();
    }
}

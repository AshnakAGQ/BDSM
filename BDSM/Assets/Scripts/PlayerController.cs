using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

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

    public GameObject pauseMenu;
    public GameObject gameOverMenu;

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
    
    List<InteractableObject> interactables;

    public static GameObjectUnityEvent PitEvent = new GameObjectUnityEvent();

    public float Speed { get => speed; set => speed = value; }
    public float Stun { get => stun; }
    public bool Alive { get => alive; set => alive = value; }

    private void Awake()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        collider2D = this.GetComponent<Collider2D>();
        animator = this.GetComponent<Animator>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        audioPlayer = this.GetComponent<AudioPlayer>();
        interactables = new List<InteractableObject>();
    }

    private void Start()
    {
        playerDies.AddListener(AddNumDeadPlayers);
        playerRevives.AddListener(SubtractNumDeadPlayers);
        numPlayersDead = 0;
    }
   

    private void Update()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        

        if (Time.timeScale == 1 && Alive)
        {
            if (stun > 0) stun -= Time.deltaTime;
            else if (!falling)
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
            if (!(stun > 0))
            {
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
    }

    void OnMove(InputValue value)
    {
        direction = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        lookDirection = value.Get<Vector2>();
    }

    void OnLookMouse(InputValue value)
    {
        lookDirection = (Vector2)(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position);
    }

    void OnInteract(InputValue value)
    {
        if (Time.timeScale == 1 && Alive && stun <= 0)
        {
            foreach (InteractableObject obj in interactables)
                obj.Interact();
        }
    }

    void OnPause(InputValue value)
    {
        if (Pause_menu.instance != null)
        {
            Pause_menu.instance.Pause(Time.timeScale == 1);
        }

    }

    void OnContinue(InputValue value)
    {
        if(gameOverMenu != null && gameOverMenu.activeSelf)
        {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            InteractableObject obj = collision.GetComponent<InteractableObject>();
            if (obj != null)
            {
                obj.GetComponent<SpriteRenderer>().color = Color.yellow;
                interactables.Add(obj);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            InteractableObject obj = collision.GetComponent<InteractableObject>();
            if (obj != null)
            {
                obj.GetComponent<SpriteRenderer>().color = Color.white;
                interactables.Remove(obj);
            }
        }
    }

    public void Damage(float damage, float stun, Vector2 knockback)
    {
        if (health > 0)
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.AddForce(knockback);
            health -= damage;
            this.stun = stun;
        }
        if (Alive && health <= 0)
        {
            health = 0;
            Alive = false;
            rigidbody2D.bodyType = RigidbodyType2D.Static;
            transform.right = -transform.up;
            playerDies.Invoke();
        }
    }

    public void Heal(float healthDelta)
    {
        float newHealthValue = health + healthDelta;
        if (newHealthValue > maxHealth)
            health = maxHealth;
        else
            health = newHealthValue;
        //SetHealthUI();

        //m_audioPlayer.playSFX(healFileName, healVolume, healPitchMin, healPitchMax);
    }

    private void AddNumDeadPlayers()
    {
        numPlayersDead += 1;
        Debug.Log(numPlayersDead);
        if (numPlayersDead >= 2)
        {
            Debug.Log("Game Over");
            gameOverMenu.SetActive(true);
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
    }
}

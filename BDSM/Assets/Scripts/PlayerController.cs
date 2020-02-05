using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    new Rigidbody2D rigidbody2D;
    new Collider2D collider2D;
    Animator animator;
    SpriteRenderer spriteRenderer;
    private AudioPlayer audioPlayer;
    [SerializeField] Transform aimingCircle = null;

    Vector2 direction;
    public Vector2 lookDirection;

    [Header("Sound")]
    [SerializeField] public AudioContainer footStepClip;

    [Header("Stats")]
    [SerializeField] float speed = 5f;
    float stun = 0;
    [SerializeField] float maxHealth = 200f;
    [SerializeField] float health = 100f;
    bool alive = true;

    enum Directions { Up, Right, Down, Left }
    
    List<InteractableObject> interactables;

    private void Awake()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        collider2D = this.GetComponent<Collider2D>();
        animator = this.GetComponent<Animator>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        audioPlayer = this.GetComponent<AudioPlayer>();
        interactables = new List<InteractableObject>();
    }

    private void Update()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

        if (Time.timeScale == 1 && alive)
        {
            if (stun > 0) stun -= Time.deltaTime;
            else
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
        if (Time.timeScale == 1 && alive && stun <= 0)
        {
            foreach (InteractableObject obj in interactables)
                obj.Interact();
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
        if (alive && health <= 0)
        {
            health = 0;
            alive = false;
            rigidbody2D.bodyType = RigidbodyType2D.Static;
            transform.right = -transform.up;
        }
    }
}

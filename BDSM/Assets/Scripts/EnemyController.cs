using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable, IMassive
{
    [Header("Components")]
    new Rigidbody2D rigidbody2D;
    List<PlayerController> players;
    Animator animator = null;

    [Header("Attributes")]
    [SerializeField] float range = 10f;
    [SerializeField] float viewAngle = 45f;
    [SerializeField] float speed = 5f;
    [SerializeField] float health = 100f;
    [SerializeField] float attackDamage = 15.0f;
    [SerializeField] float attackStun = 0.5f;
    [SerializeField] int knockbackMultiplier = 100;
    float stun = 0;

    [Header("AITweaks")]
    [SerializeField] float AttackRange = 0.5f;
    [SerializeField] float movementChance = .5f;
    [SerializeField] float movementTime = 10;
    [SerializeField] float idleTime = 1.5f;
    [SerializeField] float blockedReverseAngle = 90;
    
    float timeLeft;
    bool moving;
    bool canBounce = false;
    bool followingPlayer = false;
    Vector2 lastPosition;
    int touchingPlayer = 0;
    Vector2 previousDirection;
    PlayerController target = null;
    float distanceToTarget = float.PositiveInfinity;
    [System.NonSerialized] public bool falling = false;

    //Animation Variables
    enum Directions { Up, Right, Down, Left }
    public Vector2 lookDirection;

    void Awake()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
        players = new List<PlayerController>(FindObjectsOfType<PlayerController>());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (stun > 0) stun -= Time.deltaTime;
        else if (Time.timeScale == 1 && !falling)
        {
            target = null;
            distanceToTarget = float.PositiveInfinity;

            // Find closest player
            foreach (PlayerController player in players)
            {
                Vector2 vectorToPlayer = player.transform.position - transform.position;
                float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, vectorToPlayer);


                // Sight Cone
                if (player.Alive && distanceToPlayer < range && distanceToPlayer < distanceToTarget &&
                    Vector2.Angle(vectorToPlayer, lookDirection) < viewAngle &&
                    hit && hit.collider.CompareTag("Player"))
                {
                    target = player;
                    distanceToTarget = distanceToPlayer;
                    lookDirection = vectorToPlayer.normalized;
                }
            }

            if (target)
            {
                followingPlayer = true;

                // Attack Range
                RaycastHit2D hit = Physics2D.Raycast(this.transform.position, lookDirection, AttackRange);
                if (hit && hit.collider.CompareTag("Player") && hit.collider.gameObject == target.gameObject)
                    Attack(target);
                else
                    rigidbody2D.velocity = lookDirection.normalized * speed;
            }
            else
            {
                if (followingPlayer)
                {
                    followingPlayer = false;
                    timeLeft = idleTime;
                    rigidbody2D.velocity = Vector2.zero;
                }
                else if (timeLeft > 0)
                {
                    timeLeft -= Time.deltaTime;
                    if (moving) rigidbody2D.velocity = lookDirection.normalized * speed;
                }
                else
                { 
                    float action = UnityEngine.Random.value;

                    lookDirection = UnityEngine.Random.insideUnitCircle;
                    previousDirection = lookDirection;
                    if (action < movementChance)
                    {
                        timeLeft = movementTime;
                        moving = true;
                        canBounce = true;
                    }
                    else
                    {
                        timeLeft = idleTime;
                        rigidbody2D.velocity = Vector2.zero;
                        moving = false;
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (stun <= 0 && Time.timeScale == 1 && !falling )
        {
            if (rigidbody2D.velocity != Vector2.zero)
            {
                animator.SetBool("moving", true);

                //if (!audioPlayer.ActiveSounds.ContainsKey(footStepClip))
                //{
                //    audioPlayer.PlaySFX(footStepClip);
                //}
            }
            else
                animator.SetBool("moving", false);

            // Aiming
            if (lookDirection != Vector2.zero)
            {
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

    private void Attack(PlayerController target)
    {
        // ANIMATE ATTACK HERE
        IDamageable damageComponent = target.GetComponent<IDamageable>();
        stun += 0.3f;

        // The colliding object will begin falling if the point on the bottom of the object's collider is contained inside of the collider on this pit
        if (damageComponent != null)
        {
            damageComponent.Damage(attackDamage, attackStun, (lookDirection * knockbackMultiplier));
        }
        rigidbody2D.velocity = Vector2.zero;
    }

    public void Damage(float damage, float stun, Vector2 knockback)
    {
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(knockback);

        // Face Attacker
        lookDirection = -knockback;

        health -= damage;
        this.stun = stun;
        timeLeft = 0;
        followingPlayer = true;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Vector2 vectorToPlayer = collision.gameObject.transform.position - this.transform.position;

            rigidbody2D.velocity = Vector2.zero;

            touchingPlayer++;
        }

        else if (collision.collider.CompareTag("Enemy"))
        {
            Vector2 normal = collision.GetContact(0).normal;
            lookDirection = Vector2.Reflect(lookDirection, normal);
            previousDirection = -previousDirection;
        }

        else if (!followingPlayer && moving)
        {
            Vector2 normal = collision.GetContact(0).normal;

            float angle = Vector2.SignedAngle(lookDirection, -normal);

            if (Math.Abs(angle) < 25f)
            {
                lookDirection = Vector2.Reflect(lookDirection, normal);
                previousDirection = -previousDirection;
                if (canBounce) canBounce = false;
                else timeLeft = 0;
            }
            else
            {
                if (angle < 0) // Transform right
                    lookDirection = -Vector2.Perpendicular(normal);
                else           // Transform left
                    lookDirection = Vector2.Perpendicular(normal);
            }
        }

        lastPosition = transform.position;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!followingPlayer && moving && !collision.collider.CompareTag("Player") && !collision.collider.CompareTag("Enemy"))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDirection, .75f);
            if (hit.collider != null)
            {
                //print(this.name + " blocked by " + hit.collider.name);
                List<ContactPoint2D> contacts = new List<ContactPoint2D>();
                collision.GetContacts(contacts);
                foreach (ContactPoint2D contact in contacts)
                {
                    Vector2 normal = contact.normal;

                    float angle = Vector2.SignedAngle(lookDirection, -normal);

                    if (Math.Abs(angle) < 25f)
                    {
                        lookDirection = Vector2.Reflect(lookDirection, normal);
                        previousDirection = -previousDirection;
                        if (canBounce) canBounce = false;
                        else timeLeft = 0;
                    }
                    else
                    {
                        if (angle < 0) // Transform right
                            lookDirection = -Vector2.Perpendicular(normal);
                        else           // Transform left
                            lookDirection = Vector2.Perpendicular(normal);
                    }
                    hit = Physics2D.Raycast(transform.position, lookDirection, 0.1f);
                    if (hit.collider != null) break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) touchingPlayer--;
        lookDirection = previousDirection;
    }

    public void Fall(float fallingRate)
    {
        rigidbody2D.velocity = Vector2.zero; // Stop moving around, you're falling in a pit!
        StartCoroutine(fallCoroutine(fallingRate));
    }

    private IEnumerator fallCoroutine(float fallingRate)
    {
        Vector3 fallingModifier = new Vector3(-0.1f, -0.1f, -0.1f);
        while (this.transform.localScale.x > 0 && this.transform.localScale.y > 0 && this.transform.localScale.z > 0)
        {
            this.transform.localScale += (fallingRate * fallingModifier); // slowly shrink the scale of the object until it disappears
            yield return null;
        }
        Destroy(this.gameObject);
    }
}

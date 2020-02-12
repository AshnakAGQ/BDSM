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
    bool acting = false;
    bool followingPlayer = false;
    bool touchingPlayer = false;
    PlayerController target = null;
    float distanceToTarget = float.PositiveInfinity;

    //Animation Variables
    enum Directions { Up, Right, Down, Left }
    public Vector2 direction;
    public Vector2 lookDirection;

    void Awake()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
        players = new List<PlayerController>(FindObjectsOfType<PlayerController>());

        foreach (PlayerController player in players)
        {
            Debug.Log(player.gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.timeScale == 1)
        {
            if (animator)
            {
                direction = rigidbody2D.velocity;
                if(rigidbody2D.velocity != Vector2.zero) lookDirection = rigidbody2D.velocity;
            }

            bool foundTarget = false;
            foreach(PlayerController player in players)
            {
                Vector2 vectorToPlayer = player.transform.position - transform.position;
                float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, vectorToPlayer);


                // Sight Cone
                if (player.Alive && distanceToPlayer < range && distanceToPlayer < distanceToTarget &&
                    Vector3.Angle(vectorToPlayer, lookDirection) < viewAngle &&
                    hit && hit.collider.CompareTag("Player"))
                {
                    foundTarget = true;
                    target = player;
                    distanceToTarget = distanceToPlayer;
                }
            }
            if (!foundTarget)
            {
                target = null;
                distanceToTarget = float.PositiveInfinity;
            }

            if (target)
            {
                acting = false;
                followingPlayer = true;

                Vector2 vectorToPlayer = target.transform.position - transform.position;

                if (stun > 0)
                {
                    stun -= Time.deltaTime;
                }
                else
                {
                    //animation
                    if (animator)
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
                    
                    // Attack Range
                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, vectorToPlayer, AttackRange);
                    if (hit && hit.collider.CompareTag("Player") && hit.collider.gameObject == target.gameObject)
                    {
                        // ANIMATE ATTACK HERE
                        IDamageable damageComponent = target.GetComponent<IDamageable>();
                        stun += 0.3f;

                        // The colliding object will begin falling if the point on the bottom of the object's collider is contained inside of the collider on this pit
                        if (damageComponent != null)
                        {
                            damageComponent.Damage(attackDamage, attackStun, (vectorToPlayer.normalized * knockbackMultiplier));
                        }
                        rigidbody2D.velocity = Vector2.zero;
                    }

                    if (!touchingPlayer && Vector3.Distance(target.transform.position, transform.position) > AttackRange)
                    {
                        rigidbody2D.velocity = vectorToPlayer.normalized * speed;
                    }
                }
                
            }
            else
            {
                if (stun > 0)
                {
                    stun -= Time.deltaTime;
                }
                else if (followingPlayer)
                {
                    acting = true;
                    followingPlayer = false;
                    timeLeft = idleTime;
                    rigidbody2D.velocity = Vector2.zero;
                }
                else if (!acting)
                {
                    float action = Random.value;
                    acting = true;
                    if (action < movementChance)
                    {
                        timeLeft = movementTime;
                        rigidbody2D.velocity = Random.insideUnitCircle * speed;
                    }
                    else
                    {
                        timeLeft = idleTime;
                        rigidbody2D.velocity = Vector2.zero;
                    }
                }
                else if (timeLeft > 0)
                {
                    timeLeft -= Time.deltaTime;
                }
                else
                {
                    acting = false;
                }
            }
        }
    }

    public void Damage(float damage, float stun, Vector2 knockback)
    {
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(knockback);

        // Face Attacker
        lookDirection = -knockback;

        health -= damage;
        this.stun = stun;
        acting = false;
        followingPlayer = true;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (acting && !followingPlayer && rigidbody2D.velocity != Vector2.zero)
        {
            rigidbody2D.velocity = lookDirection.normalized * speed;
        }

        if (collision.collider.CompareTag("Player"))
        {
            Vector2 vectorToPlayer = collision.gameObject.transform.position - this.transform.position;

            rigidbody2D.velocity = Vector2.zero;

            touchingPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            touchingPlayer = false;
        }
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

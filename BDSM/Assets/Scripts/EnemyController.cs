using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
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
    void Update()
    {
        if (Time.timeScale == 1)
        {
            if (animator)
            {
                animator.SetFloat("Speed", Mathf.Abs(this.rigidbody2D.velocity.x) + Mathf.Abs(this.rigidbody2D.velocity.y));
            }

            bool foundTarget = false;
            foreach(PlayerController player in players)
            {
                Vector2 vectorToPlayer = player.transform.position - transform.position;
                float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, vectorToPlayer);


                // Sight Cone
                if (distanceToPlayer < range && distanceToPlayer < distanceToTarget &&
                    Vector3.Angle(vectorToPlayer, this.transform.right) < viewAngle &&
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
                this.transform.right = (Vector2)(vectorToPlayer);

                if (stun > 0)
                {
                    stun -= Time.deltaTime;
                }
                else
                {
                    // Attack Range
                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, vectorToPlayer, AttackRange);
                    if (hit && hit.collider.CompareTag("Player"))
                    {
                        //weapon.Attack(); *************************
                        rigidbody2D.velocity = Vector2.zero;
                    }

                    if (!touchingPlayer && Vector3.Distance(target.transform.position, transform.position) > AttackRange)
                    {
                        rigidbody2D.velocity = this.transform.right.normalized * speed;
                        this.transform.right = rigidbody2D.velocity;
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
                        this.transform.right = Random.insideUnitCircle;
                        rigidbody2D.velocity = this.transform.right.normalized * speed;
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
        this.transform.right = -knockback;

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
            this.transform.right = Quaternion.Euler(0, 0, Random.Range(-blockedReverseAngle, blockedReverseAngle) / 2) * -this.transform.right;
            rigidbody2D.velocity = this.transform.right.normalized * speed;
        }

        if (collision.collider.CompareTag("Player"))
        {
            Vector2 vectorToPlayer = collision.gameObject.transform.position - this.transform.position;

            this.transform.right = vectorToPlayer;
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
}

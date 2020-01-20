using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Components")]
    [SerializeField] GameObject EnemyTorso = null;
    [SerializeField] Weapon weapon = null;
    [SerializeField] GameObject EnemyLegs = null;
    [SerializeField] GameObject EnemyArmLeft = null;
    [SerializeField] GameObject EnemyArmRight = null;
    [SerializeField] GameObject Blood = null;
    [SerializeField] ParticleSystem BloodParticles = null;
    new Rigidbody2D rigidbody2D;
    new Collider2D collider2D;

    [Header("Attributes")]
    [SerializeField] float range = 10f;
    [SerializeField] float viewAngle = 45f;
    [SerializeField] float speed = 5f;
    [SerializeField] bool faceTarget = true;
    [SerializeField] float health = 100f;
    [SerializeField] GameObject loot = null;
    float stun = 0;

    [SerializeField] Animator animator = null;

    [Header("HealthUI")]
    [SerializeField] Slider slider = null;
    [SerializeField] Image fillImage = null;
    [SerializeField] Color FullHealthColor = Color.green;
    [SerializeField] Color ZeroHealthColor = Color.clear;

    [Header("AITweaks")]
    [SerializeField] float rangeModifier = .8f;
    [SerializeField] float movementChance = .5f;
    [SerializeField] float movementTime = 10;
    [SerializeField] float idleTime = 3;
    [SerializeField] bool attacksOthers = true;
    [SerializeField] float blockedReverseAngle = 90;
    float timeLeft;
    bool acting = false;
    bool followingPlayer = false;
    bool touchingPlayer = false;

    [Header("Sound")]

    [SerializeField] string footStepFileName = null;
    [SerializeField] float footStepVolume = 0.30f;
    [SerializeField] float footStepPitchMin = 0.9f;
    [SerializeField] float footStepPitchMax = 1.1f;

    [SerializeField] string enemyDamagedFileName = null;
    [SerializeField] float damagedSoundVolume = 0.65f;
    [SerializeField] float damagedPitchMinimum = 0.85f;
    [SerializeField] float damagedPitchMaximum = 1.15f;
    
    AudioPlayer m_audioPlayer;

    void Awake()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        collider2D = this.GetComponent<Collider2D>();
        m_audioPlayer = GetComponentInChildren<AudioPlayer>();
        m_audioPlayer.addSFX(enemyDamagedFileName);
        m_audioPlayer.addSFX(footStepFileName);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (weapon.name != "Arms")
            Physics2D.IgnoreCollision(collider2D, weapon.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(collider2D, EnemyArmLeft.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(collider2D, EnemyArmRight.GetComponent<Collider2D>());
        SetHealthUI();
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

            Vector2 vectorToPlayer = PlayerController.instance.transform.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, vectorToPlayer);
            

             // Sight Cone
            if (Vector3.Distance(PlayerController.instance.transform.position, transform.position) < range &&
                Vector3.Angle(vectorToPlayer, EnemyTorso.transform.right) < viewAngle &&
                hit && hit.collider.CompareTag("Player"))
            {
                acting = false;
                followingPlayer = true;
                EnemyTorso.transform.right = (Vector2)(vectorToPlayer);

                if (stun > 0)
                {
                    stun -= Time.deltaTime;
                }
                else
                {
                    // Attack Range
                    hit = Physics2D.Raycast(transform.position, vectorToPlayer, weapon.WeaponRange * rangeModifier); //weapon.GetComponent<SpriteRenderer>().bounds.size.y);
                    if (hit && hit.collider.CompareTag("Player"))
                    {
                        weapon.Attack();
                        rigidbody2D.velocity = Vector2.zero;
                    }

                    if (!touchingPlayer && Vector3.Distance(PlayerController.instance.transform.position, transform.position) > weapon.WeaponRange * rangeModifier)
                    {
                        rigidbody2D.velocity = EnemyTorso.transform.right.normalized * speed;
                        EnemyLegs.transform.right = rigidbody2D.velocity;
                        if (faceTarget && Quaternion.Angle(EnemyTorso.transform.rotation, EnemyLegs.transform.rotation) > 90) EnemyLegs.transform.right = -1 * EnemyLegs.transform.right; // Keeps body facing mouse
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
                        EnemyLegs.transform.right = EnemyTorso.transform.right = Random.insideUnitCircle;
                        rigidbody2D.velocity = EnemyTorso.transform.right.normalized * speed;
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
        //DEBUG
        //Debug.Log("Distance is " + Vector3.Distance(PlayerController.instance.transform.position, transform.position));
        //Debug.Log("Angle is " + Vector3.Angle(PlayerController.instance.transform.position - transform.position, EnemyTorso.transform.up));
        Debug.DrawRay(transform.position, (Quaternion.Euler(0, 0, viewAngle) * EnemyTorso.transform.right).normalized * range, Color.yellow, .01f);
        Debug.DrawRay(transform.position, (Quaternion.Euler(0, 0, -viewAngle) * EnemyTorso.transform.right).normalized * range, Color.yellow, .01f);
        Debug.DrawRay(transform.position, EnemyTorso.transform.right.normalized * weapon.WeaponRange, Color.red, .01f);
        //Debug.DrawRay(transform.position, EnemyLegs.transform.right, Color.green, .01f);
    }

    public void Damage(float damage, float stun, Vector2 knockback)
    {
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(knockback);

        // Face Attacker
        EnemyLegs.transform.right = -knockback;
        EnemyTorso.transform.right = -knockback;

        health -= damage;
        SetHealthUI();
        this.stun = stun;
        acting = false;
        followingPlayer = true;

        BloodParticles.Play();

        if (health <= 0)
        {
            Instantiate(Blood, EnemyTorso.transform.position, Quaternion.identity).transform.localScale = new Vector3(transform.lossyScale.x, transform.lossyScale.y, 1);
            dropLoot();
            Destroy(gameObject);
        }

        m_audioPlayer.playSFX(enemyDamagedFileName, damagedSoundVolume, damagedPitchMinimum, damagedPitchMaximum);
    }

    private void SetHealthUI()
    {
        slider.value = health;

        fillImage.color = Color.Lerp(ZeroHealthColor, FullHealthColor, health / 100); // 100 is the hardcoded starting health, might need to change later
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (attacksOthers && collision.collider.CompareTag("Enemy") && !followingPlayer)
        {
            Vector2 vectorToEnemy = collision.transform.position - transform.position;

            EnemyLegs.transform.right = vectorToEnemy;
            EnemyTorso.transform.right = vectorToEnemy;

            rigidbody2D.velocity = Vector2.zero;
            weapon.Attack();
        }

        else if (acting && !followingPlayer && rigidbody2D.velocity != Vector2.zero)
        {
            EnemyLegs.transform.right = EnemyTorso.transform.right = Quaternion.Euler(0, 0, Random.Range(-blockedReverseAngle, blockedReverseAngle) / 2) * -EnemyTorso.transform.right;
            rigidbody2D.velocity = EnemyTorso.transform.right.normalized * speed;
        }

        //if (!followingPlayer && collision.collider.CompareTag("Wall"))
        //{
        //    timeLeft = idleTime;
        //    rigidbody2D.velocity = Vector2.zero;
        //    transform.right *= -1;
        //}

        if (collision.collider.CompareTag("Player"))
        {
            Vector2 vectorToPlayer = PlayerController.instance.transform.position - transform.position;

            EnemyLegs.transform.right = vectorToPlayer;
            EnemyTorso.transform.right = vectorToPlayer;
            rigidbody2D.velocity = Vector2.zero;

            touchingPlayer = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("WeaponGround"))
        {
            IPickupable itemComponent = collision.GetComponent<IPickupable>();
            if (itemComponent != null && loot == null)
            {
                Physics2D.IgnoreCollision(collider2D, collision);
                loot = collision.gameObject;
                GameObject newItem = Instantiate(collision.gameObject, EnemyTorso.transform);

                newItem.GetComponent<Collider2D>().enabled = false;
                newItem.GetComponent<MonoBehaviour>().enabled = false;
                newItem.transform.localPosition = Vector3.left * .60f;
                newItem.transform.right = newItem.transform.up;
                newItem.transform.localScale /= 4;

                collision.gameObject.SetActive(false);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            touchingPlayer = false;
        }
    }

    private void playFootStepSFX()
    {
        m_audioPlayer.playSFX(footStepFileName, footStepVolume, footStepPitchMin, footStepPitchMax);
    }

    void dropLoot()
    {
        if (loot != null)
        {
            loot.transform.position = transform.position;
            loot.SetActive(true);
        }
        if (weapon.PickupVersion != null) Instantiate(weapon.PickupVersion, transform.position, Quaternion.identity);
    }
}

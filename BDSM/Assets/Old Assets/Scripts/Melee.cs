using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon
{
    private bool broken = false;

    [Header("Battle Values")]
    [SerializeField] float attackTime = .5f;
    [SerializeField] float cooldown = .5f;
    [SerializeField] float knockback = 100;
    [SerializeField] float damage = 25;
    [SerializeField] float stun = .5f;
    bool attackReset = true;

    [Header("Animation")]
    [SerializeField] float startArc = -75;
    [SerializeField] float endArc = 75;

    [Header("Sound")]
    [SerializeField] string pickupFileName = null;
    [SerializeField] float pickupVolume = 0.65f;
    [SerializeField] float pickupPitchMinimum = 0.95f;
    [SerializeField] float pickupPitchMaximum = 1.05f;

    [SerializeField] string whooshFileName = null;
    [SerializeField] string whooshFileName_2 = null;
    [SerializeField] float whooshVolume = 0.65f;
    [SerializeField] float whooshPitchMinimum = 0.90f;
    [SerializeField] float whooshPitchMaximum = 1.10f;

    [SerializeField] string collideFileName = null;
    [SerializeField] float collideSoundVolume = 0.65f;
    [SerializeField] float collidePitchMinimum = 0.95f;
    [SerializeField] float collidePitchMaximum = 1.05f;

    AudioPlayer m_audioPlayer;
    float rotateSpeed;
    new Collider2D collider2D = null;
    new Rigidbody2D rigidbody2D;
    float time = 0;
    float attackTimer = 0.01f;

    public float Cooldown { get => cooldown; }

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        m_audioPlayer = GetComponentInChildren<AudioPlayer>();
        m_audioPlayer.addSFX(whooshFileName);
        m_audioPlayer.addSFX(whooshFileName_2);
        m_audioPlayer.addSFX(collideFileName);
        m_audioPlayer.addSFX(pickupFileName);
    }

    protected void Start()
    {
        transform.localRotation = Quaternion.Euler(0, 0, startArc);
        if (attackTime != 0) rotateSpeed = (endArc - startArc) / attackTime;
        else rotateSpeed = 0;
        collider2D.enabled = false;
        weaponRange = GetComponent<SpriteRenderer>().bounds.size.y;

        if (this.transform.parent.CompareTag("PlayerTorso"))
        {
            m_audioPlayer.setSpatialBlend(0.0f);
        }

        m_audioPlayer.playSFX(pickupFileName, pickupVolume, pickupPitchMinimum, pickupPitchMaximum);
    }

    private void Update()
    {
        if (collider2D && collider2D.enabled)
        {
            time += Time.deltaTime;
            if (time < attackTime)
            {
                transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
            }
            else if (time >= attackTime + cooldown)
            {
                collider2D.enabled = false;
                transform.localRotation = Quaternion.Euler(0, 0, startArc);
                time = 0;
                attackReset = true;
            }
            else transform.localRotation = Quaternion.Euler(0, 0, startArc);

        }
    }

    override public void Attack()
    {
        if (collider2D && collider2D.enabled == false)
        {
            collider2D.enabled = true;

            StartCoroutine(startAttackTimer(attackTime));

            int random = Random.Range(0, 2);
            if (random == 0)
            {
                m_audioPlayer.playSFX(whooshFileName, whooshVolume, whooshPitchMinimum, whooshPitchMaximum);
            }
            else
            {
                m_audioPlayer.playSFX(whooshFileName_2, whooshVolume, whooshPitchMinimum, whooshPitchMaximum);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (attackReset)
        {
            IDamageable damageableComponent = other.GetComponent<IDamageable>();

            if (damageableComponent != null) // registers a hit and plays sounds only when hitting enemies
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, other.transform.position - transform.position);
                //if (hit) Debug.Log(hit.collider.name);
                if (!hit || hit.collider == other)
                {
                    damageableComponent.Damage(damage, stun, knockback * (Vector2)(other.transform.position - transform.position));
                    Durability--;
                    if (Durability == 0) StartCoroutine ( waitAndDestroy(attackTime - attackTimer) );
                    attackReset = false;
                    m_audioPlayer.playSFX(collideFileName, collideSoundVolume, collidePitchMinimum, collidePitchMaximum);
                }
            }
        }
    }
    
    public override bool checkDestroy()
    {
        return broken;
    }


    private IEnumerator startAttackTimer(float time)
    {
        float memory = attackTimer;
        while (attackTimer < time)
        {
            attackTimer += Time.deltaTime;
            yield return null;
        }

        attackTimer = memory;
    }


    private IEnumerator waitAndDestroy(float time)
    {
        float timer = 0.0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        broken = true;
    }
}

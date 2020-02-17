using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] int animTime = 30;

    [Header("Stats")]
    [SerializeField] int damage = 0;
    [SerializeField] float stun = 1.0f;
    [SerializeField] int knockback = 350;
    [SerializeField] int areaOfEffect = 3;

    List<IDamageable> damaged;

    int currentTime = 0;
    new Collider2D collider2D;
    bool activated = false;
    SpriteRenderer spriteRenderer;

    [SerializeField] public AudioContainer windSound;
    AudioPlayer m_AudioPlayer;

    private void Awake()
    {
        m_AudioPlayer = this.GetComponent<AudioPlayer>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        collider2D = GetComponent<Collider2D>();
        damaged = new List<IDamageable>();
        
        collider2D.isTrigger = true;
    }

    private void Start()
    {

        if (GameManager.instance)
        {
            GameManager.instance.GetComponent<AudioPlayer>().PlaySFX(windSound);
        }
        activated = true;
    }

    private void Update()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

        if (activated)
        {
            transform.localScale = Vector2.Lerp(Vector2.one, new Vector2(areaOfEffect, areaOfEffect), (float)currentTime / animTime);
            if (currentTime == animTime)
            {
                Destroy(this.gameObject);
            }
            currentTime++;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageableComponent = collision.GetComponent<IDamageable>();

        if (damageableComponent != null && !damaged.Contains(damageableComponent))
        {
            damageableComponent.Damage(damage, stun, knockback * (Vector2)(collision.transform.position - transform.position));
            damaged.Add(damageableComponent);
        }
    }
}

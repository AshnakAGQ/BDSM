using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fireball : MonoBehaviour
{
    [SerializeField] Vector2 target;
    [SerializeField] int animTime = 30;

    [Header("Stats")]
    [SerializeField] int speed = 5;
    [SerializeField] int damage = 30;
    [SerializeField] float stun = .5f;
    [SerializeField] int knockback = 100;
    [SerializeField] int areaOfEffect = 3;

    List<IDamageable> damaged;

    int currentTime = 0;
    Animator animator;
    new Rigidbody2D rigidbody2D;
    new Collider2D collider2D;
    bool activated = false;
    SpriteRenderer spriteRenderer;

    public Vector2 Target { get => target; set => target = value; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        damaged = new List<IDamageable>();
    }
    
    private void Update()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

        if (activated)
        {
            animator.SetTrigger("explode");
            transform.localScale = Vector2.Lerp(Vector2.one, new Vector2(areaOfEffect, areaOfEffect), (float) currentTime / animTime);
            if (currentTime == animTime)
            {
                Destroy(this.gameObject);
            }
            currentTime++;
        }
        else
        {
            rigidbody2D.velocity = (target - (Vector2)transform.position).normalized * speed;
        }
    }

    private void FixedUpdate()
    {
        if (!activated && Vector2.Distance(transform.position, Target) <= 0.05f)
        {
            activated = true;
            rigidbody2D.bodyType = RigidbodyType2D.Static;
            collider2D.isTrigger = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.collider.name);
        activated = true;
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        collider2D.isTrigger = true;
        if (collision.collider.CompareTag("Wall"))
        {
            spriteRenderer.flipX = false;
        }

        IDamageable damageableComponent = collision.collider.GetComponent<IDamageable>();

        if (damageableComponent != null && !damaged.Contains(damageableComponent))
        {
            damageableComponent.Damage(damage, stun, knockback * (Vector2)(collision.transform.position - transform.position));
            damaged.Add(damageableComponent);
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

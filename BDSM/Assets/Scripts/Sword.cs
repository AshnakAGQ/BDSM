﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    bool swinging = false;
    bool canCombo = false;
    Animator animator = null;
    PlayerController player;
    SpriteRenderer spriteRenderer;
    List<IDamageable> damaged;


    [Header("Stats")]
    [SerializeField] int damage = 30;
    [SerializeField] float stun = .5f;
    [SerializeField] int knockback = 100;
    [SerializeField] int lunge = 100;
    [SerializeField] float cooldown = 0.25f;
    float timer = 0;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        damaged = new List<IDamageable>();
    }

    private void Update()
    {
        if (spriteRenderer.enabled) spriteRenderer.sortingOrder = Mathf.RoundToInt((transform.position.y + 0.15f) * 100f) * -1;

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void CanCombo()
    {
        canCombo = true;
    }

    public void ResetSwing()
    {
        swinging = false;
        canCombo = false;
        animator.ResetTrigger("Swing");
        animator.ResetTrigger("Swing2");
        timer = cooldown;
    }

    public void Swing()
    {
        if(canCombo)
        {
            damaged = new List<IDamageable>();
            canCombo = false;
            animator.SetTrigger("Swing2");
            player.Damage(0, .25f, transform.parent.transform.right * lunge);
        }
        else if (!swinging && timer <= 0)
        {
            damaged = new List<IDamageable>();
            swinging = true;
            animator.SetTrigger("Swing");
            player.Damage(0, .25f, transform.parent.transform.right * lunge);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Shield"))
        {
            IDamageable warrior = collision.GetComponentInParent<IDamageable>();
            if (warrior != null)
            {
                damaged.Add(warrior);
            }
        }

        IDamageable damageableComponent = collision.GetComponent<IDamageable>();

        if (damageableComponent != null && !damaged.Contains(damageableComponent))
        {
            damageableComponent.Damage(damage, stun, knockback * (Vector2)(collision.transform.position - transform.position));
            damaged.Add(damageableComponent);
        }
    }
}

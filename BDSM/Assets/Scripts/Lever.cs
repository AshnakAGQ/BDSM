using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : InteractableObject
{
    [SerializeField] ActivatableObject target = null;
    Animator animator;
    bool activated = false;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
    }

    public override void Interact()
    {
        if (!activated)
        {
            activated = true;
            if (animator != null) animator.SetBool("activated", true);
            if (target != null) target.Activate();
        }
        else
        {
            activated = false;
            if (animator != null) animator.SetBool("activated", false);
            if (target != null) target.Deactivate();
        }
    }
}

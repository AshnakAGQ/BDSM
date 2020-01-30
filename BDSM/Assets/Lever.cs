using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : InteractableObject
{
    [SerializeField] ActivatableObject target;
    Animator animator;
    bool activated = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : ActivatableObject
{
    Animator animator;
    bool activated = false;

    public override void Activate()
    {
        if (!activated)
        {

            activated = true;
            if (animator != null) animator.SetBool("activated", true);
        }
    }

    public override void Deactivate()
    {
        if (activated)
        {
            activated = false;
            if (animator != null) animator.SetBool("activated", false);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
}

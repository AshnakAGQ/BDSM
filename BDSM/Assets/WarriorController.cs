using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarriorController : MonoBehaviour
{
    [SerializeField] Transform aimingCircle = null;
    Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
    }

    void OnPrimaryAction(InputValue value)
    {
        
    }

    void OnSecondaryAction(InputValue value)
    {
        animator.SetBool("blocking", (value.isPressed ? true : false));
    }

    void OnTertiaryAction(InputValue value)
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarriorController : MonoBehaviour
{
    [SerializeField] Transform aimingCircle = null;
    [SerializeField] SpriteRenderer aimingCircleRenderer = null;
    [SerializeField] float speedMult = 0.5f;
    [SerializeField] GameObject shield;
    Animator animator;
    PlayerController player;
    float maxSpeed;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }

    private void Start()
    {
        maxSpeed = player.Speed;
    }

    void OnPrimaryAction(InputValue value)
    {
        
    }

    void OnSecondaryAction(InputValue value)
    {
        if (Time.timeScale == 1)
        {
            animator.SetBool("blocking", (value.isPressed ? true : false));
            player.Speed = (value.isPressed ? maxSpeed * speedMult : maxSpeed);
            shield.SetActive(value.isPressed);
            aimingCircleRenderer.enabled = !value.isPressed;
        }
    }

    void OnTertiaryAction(InputValue value)
    {

    }
}

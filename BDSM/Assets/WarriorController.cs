using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarriorController : MonoBehaviour
{
    [SerializeField] Transform aimingCircle = null;
    [SerializeField] SpriteRenderer aimingCircleRenderer = null;
    [SerializeField] float speedMult = 0.5f;
    GameObject shield;
    Sword sword;
    Animator animator;
    PlayerController player;
    float maxSpeed;
    bool blocking;
    bool unblock = false;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
        sword = GetComponentInChildren<Sword>();
        shield = GetComponentInChildren<Shield>().gameObject;
    }

    private void Start()
    {
        maxSpeed = player.Speed;
        shield.SetActive(false);
    }

    private void Update()
    {
        if (unblock && Time.timeScale == 1 && player.Stun <= 0)
        {
            blocking = false;
            animator.SetBool("blocking", false);
            player.Speed = maxSpeed;
            shield.SetActive(false);
            aimingCircleRenderer.enabled = true;
            unblock = false;
        }
    }

    void OnPrimaryAction(InputValue value)
    {
        if (!blocking && Time.timeScale == 1 && player.Stun <= 0)
            sword.Swing();
    }

    void OnSecondaryAction(InputValue value)
    {
        if (value.isPressed && Time.timeScale == 1 && player.Stun <= 0)
        {
            blocking = true;
            animator.SetBool("blocking", true);
            player.Speed = maxSpeed * speedMult;
            shield.SetActive(true);
            aimingCircleRenderer.enabled = false;
        }
        else
            unblock = true;
    }

    void OnTertiaryAction(InputValue value)
    {

    }
}

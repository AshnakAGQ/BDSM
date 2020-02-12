using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarriorController : MonoBehaviour
{
    [SerializeField] Transform aimingCircle = null;
    Animator animator;

    [Header("Sound")]
    [SerializeField] public List<AudioContainer> SwordHit;

    AudioPlayer m_AudioPlayer;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        m_AudioPlayer = this.GetComponent<AudioPlayer>();
    }

    private void Start()
    {
    }

    void OnPrimaryAction(InputValue value)
    {
        
    }

    void OnSecondaryAction(InputValue value)
    {
        if (Time.timeScale == 1)
        {
            animator.SetBool("blocking", (value.isPressed ? true : false));
        }
    }

    void OnTertiaryAction(InputValue value)
    {

    }
}

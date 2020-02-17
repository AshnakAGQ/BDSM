using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField] ActivatableObject target = null;

    Animator animator;
    bool activated = false;
    SpriteRenderer spriteRenderer;

    [SerializeField] AudioContainer sound = null;
    AudioPlayer m_AudioPlayer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        m_AudioPlayer = GetComponent<AudioPlayer>();
    }

    private void Start()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
    }

    public void Interact()
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
        m_AudioPlayer.PlaySFX(sound);
    }
}

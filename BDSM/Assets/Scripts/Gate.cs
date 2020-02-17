using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : ActivatableObject
{
    Animator animator;
    bool activated = false;
    [SerializeField] AudioContainer activateSfx = null;
    [SerializeField] AudioContainer deactivateSfx = null;
    AudioPlayer m_AudioPlayer;

    public override void Activate()
    {
        if (!activated)
        {
            activated = true;
            if (animator != null) animator.SetBool("activated", true);
            m_AudioPlayer.PlaySFX(activateSfx);
        }
    }

    public override void Deactivate()
    {
        if (activated)
        {
            activated = false;
            if (animator != null) animator.SetBool("activated", false);
            AudioSource[] sounds  = GetComponents<AudioSource>();
            foreach (AudioSource sound in sounds) sound.Pause();
            m_AudioPlayer.PlaySFX(deactivateSfx);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        m_AudioPlayer = GetComponent<AudioPlayer>();
    }
    
}

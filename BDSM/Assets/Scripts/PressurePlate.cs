using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] ActivatableObject target = null;
    SpriteRenderer spriteRenderer;
    int triggerCount = 0;
    [SerializeField] AudioContainer activateSfx = null;
    [SerializeField] AudioContainer deactivateSfx = null;
    AudioPlayer m_AudioPlayer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        m_AudioPlayer = GetComponent<AudioPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Projectile"))
        {
            if (triggerCount == 0)
            {

                if (target != null) target.Activate();

                if (spriteRenderer != null) spriteRenderer.color = Color.gray;
                m_AudioPlayer.PlaySFX(activateSfx);
            }

            ++triggerCount;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Projectile"))
        {
            if (triggerCount == 1)
            {

                if (target != null) target.Deactivate();

                if (spriteRenderer != null) spriteRenderer.color = Color.white;
                m_AudioPlayer.PlaySFX(deactivateSfx);
            }

            --triggerCount;
        }
    }
}

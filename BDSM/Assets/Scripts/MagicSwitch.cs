using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSwitch : MonoBehaviour
{
    [SerializeField] ActivatableObject target = null;
    [SerializeField] List<MagicSwitch> coop = null;
    Animator animator;
    SpriteRenderer spriteRenderer;
    bool activated = false;

    public bool Activated { get => activated; }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Projectile"))
        {
            activated = true;
            bool activate = true;
            if (coop != null)
            {
                foreach(MagicSwitch ms in coop)
                {
                    if (!ms.Activated) activate = false;
                }
            }
            if (animator != null) animator.SetTrigger("activated");
            if (target != null && activate) target.Activate();
            m_AudioPlayer.PlaySFX(sound);
        }
    }
}

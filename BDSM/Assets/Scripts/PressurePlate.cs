using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] ActivatableObject target = null;
    SpriteRenderer spriteRenderer;
    int triggerCount = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnTriggerEnter2D()
    {
        if (triggerCount == 0)
        {

            if (target != null) target.Activate();

            if (spriteRenderer != null) spriteRenderer.color = Color.gray;
        }

        ++triggerCount;
    }

    public void OnTriggerExit2D()
    {
        if (triggerCount == 1)
        {

            if (target != null) target.Deactivate();

            if (spriteRenderer != null) spriteRenderer.color = Color.white;
        }

        --triggerCount;
    }
}

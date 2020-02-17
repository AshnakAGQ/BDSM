using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    PlayerController player;
    
    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt((transform.position.y+.15f) * 100f) * -1;
        
        if (player != null && player.lookDirection != Vector2.zero)
        {
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(player.lookDirection.normalized.y)/2 + 1, transform.localScale.z);
        }
    }
}

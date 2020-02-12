using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    [SerializeField] GameObject gameOverMenu;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CompareTag("Player")) gameOverMenu.SetActive(true);
    }
}

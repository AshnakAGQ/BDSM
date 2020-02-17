using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    [Tooltip("Indicates the rate at which things fall into the stairs.\nA lower value is slower, and vice - versa")]
    [SerializeField] public float fallingRate;

    private Collider2D m_Collider2D;

    private void Awake()
    {
        m_Collider2D = this.GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) UI_Manager.WinGame();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        IMassive massComponent = collision.GetComponent<IMassive>();

        // The colliding object will begin falling if the point on the bottom of the object's collider is contained inside of the collider on this pit
        if (massComponent != null && !massComponent.isFalling() && m_Collider2D.bounds.Contains(collision.bounds.center - new Vector3(0, collision.bounds.size.y / 2, collision.bounds.center.z)))
        {
            if (collision.CompareTag("Player"))
            {
                massComponent.Fall(fallingRate);
            }
        }
    }
}

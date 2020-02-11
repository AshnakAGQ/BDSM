using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A type of object that makes certain other objects begin to fall when they enter it
/// </summary>

public class Pit : MonoBehaviour
{
    [Tooltip("Indicates the rate at which things fall into the pit.\nA lower value is slower, and vice - versa")]
    [SerializeField] public float fallingRate;

    private Collider2D m_Collider2D;

    private void Awake()
    {
        m_Collider2D = this.GetComponent<Collider2D>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        IMassive massComponent = collision.GetComponent<IMassive>();

        // The colliding object will begin falling if the point on the bottom of the object's collider is contained inside of the collider on this pit
        if (massComponent != null && m_Collider2D.bounds.Contains(collision.bounds.center - new Vector3(0, collision.bounds.size.y/2, 0)))
        {
            massComponent.Fall(fallingRate);
        }
    }
}

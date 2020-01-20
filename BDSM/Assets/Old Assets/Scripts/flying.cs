using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flying : MonoBehaviour
{
    [SerializeField] float speed = 0.0f;
    Rigidbody2D m_rigidbody2D;

    private void Awake()
    {
        m_rigidbody2D = this.GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Gun").GetComponent<Collider2D>());
    }

    private void Update()
    {
        m_rigidbody2D.velocity = (transform.right * speed);
    }

}

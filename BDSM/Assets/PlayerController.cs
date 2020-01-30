using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    new Rigidbody2D rigidbody2D;
    new Collider2D collider2D;
    [SerializeField] Transform aimingCircle;


    Vector2 direction;
    Vector2 lookDirection;

    float speed = 5f;

    enum Directions { Up, Right, Down, Left }
    Directions faceDirection;

    private void Awake()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        collider2D = this.GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Time.timeScale == 1)
        {
            rigidbody2D.velocity = direction * speed;

            aimingCircle.right = lookDirection;

            if (lookDirection != Vector2.zero)
            {
                if (System.Math.Abs(lookDirection.y) > System.Math.Abs(lookDirection.x))
                {
                    faceDirection = lookDirection.y > 0 ? Directions.Up : Directions.Down;
                }
                else
                {
                    faceDirection = lookDirection.x > 0 ? Directions.Right : Directions.Left;
                }
            }
        }
    }

    void OnMove(InputValue value)
    {
        direction = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        lookDirection = value.Get<Vector2>();
    }

    void OnLookMouse(InputValue value)
    {
        lookDirection = (Vector2)(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position);
    }

    void OnPrimaryAction(InputValue value)
    {
        
    }

    void OnSecondaryAction(InputValue value)
    {
        
    }

    void OnTertiaryAction(InputValue value)
    {
        
    }

    void OnInteract(InputValue value)
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    new Rigidbody2D rigidbody2D;
    new Collider2D collider2D;
    Animator animator;
    [SerializeField] Transform aimingCircle;


    Vector2 direction;
    Vector2 lookDirection;

    float speed = 5f;

    enum Directions { Up, Right, Down, Left }

    private void Awake()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        collider2D = this.GetComponent<Collider2D>();
        animator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Time.timeScale == 1)
        {
            // Movement
            rigidbody2D.velocity = direction * speed;

            if (direction != Vector2.zero)
                animator.SetBool("moving", true);
            else
                animator.SetBool("moving", false);

            // Aiming
            if (lookDirection != Vector2.zero)
            {
                aimingCircle.right = lookDirection;

                if (System.Math.Abs(lookDirection.y) > System.Math.Abs(lookDirection.x))
                {
                    if ( lookDirection.y > 0)
                        animator.SetInteger("direction", (int) Directions.Up);
                    else
                        animator.SetInteger("direction", (int) Directions.Down);
                }
                else
                {
                    if (lookDirection.x > 0)
                        animator.SetInteger("direction", (int) Directions.Right);
                    else
                        animator.SetInteger("direction", (int) Directions.Left);
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

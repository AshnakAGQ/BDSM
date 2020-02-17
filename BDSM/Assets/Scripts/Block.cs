using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour, IMassive
{
    new Rigidbody2D rigidbody;
    SpriteRenderer spriteRenderer;
    WarriorController warrior = null;
    Transform originalParent;
    bool canMove = false;

    [SerializeField] AudioContainer pushSound = null;
    [SerializeField] AudioContainer clickSound = null;
    AudioPlayer m_AudioPlayer;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        m_AudioPlayer = GetComponent<AudioPlayer>();
    }

    private void Start()
    {
        originalParent = transform.parent;
    }

    private void FixedUpdate()
    {
        if (warrior && warrior.Grabbing && Vector2.Angle(warrior.Player.lookDirection, transform.position - warrior.transform.position) < warrior.BlockAngle / 2)
        {
            canMove = true;
        }
        else
        {
            canMove = false;
            rigidbody.velocity = Vector2.zero;
        }
        if (canMove)
        {
            if (warrior.GetComponent<Rigidbody2D>().velocity != Vector2.zero)
            {
                m_AudioPlayer.PlaySFX(pushSound);
                rigidbody.velocity = warrior.GetComponent<Rigidbody2D>().velocity;
            }
        }
    }

    private void Update()
    {
        if (canMove)
            spriteRenderer.color = Color.red;
        else if (warrior)
            spriteRenderer.color = Color.yellow;
        else
            spriteRenderer.color = Color.white;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        WarriorController warriorController = collision.collider.GetComponent<WarriorController>();
        if (warriorController)
        {
            warrior = warriorController;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        WarriorController warriorController = collision.collider.GetComponent<WarriorController>();
        if (warriorController)
        {
            warrior = null;
        }
    }

    public void Fall(float fallingRate)
    {
        print("triggered");
        if (GameManager.instance)
        {
            GameManager.instance.GetComponent<AudioPlayer>().PlaySFX(pushSound);
            GameManager.instance.GetComponent<AudioPlayer>().PlaySFX(clickSound);
        }
        Destroy(gameObject);
    }
}

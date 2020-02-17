using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarriorController : MonoBehaviour
{
    [SerializeField] Transform aimingCircle = null;
    [SerializeField] SpriteRenderer aimingCircleRenderer = null;
    [SerializeField] float speedMult = 0.5f;
    GameObject shield;
    Sword sword;
    Animator animator;
    PlayerController player;
    float maxSpeed;
    bool blocking;
    float blockAngle = 45f;
    bool unblock = false;
    List<Block> objects;

    [Header("Sound")]
    [SerializeField] public List<AudioContainer> SwordHit;

    AudioPlayer m_AudioPlayer;
    private bool grabbing;

    public GameObject Shield { get => shield; set => shield = value; }
    public bool Blocking { get => blocking; set => blocking = value; }
    public float BlockAngle { get => blockAngle; set => blockAngle = value; }
    public PlayerController Player { get => player; set => player = value; }
    public bool Grabbing { get => grabbing; set => grabbing = value; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
        sword = GetComponentInChildren<Sword>();
        shield = GetComponentInChildren<Shield>().gameObject;
        m_AudioPlayer = this.GetComponent<AudioPlayer>();
        objects = new List<Block>();
    }

    private void Start()
    {
        maxSpeed = player.Speed;
        shield.SetActive(false);
    }

    private void Update()
    {
        if (unblock && Time.timeScale == 1 && player.Stun <= 0)
        {
            blocking = false;
            grabbing = false;
            player.CanLook = true;
            animator.SetBool("blocking", false);
            player.Speed = maxSpeed;
            shield.SetActive(false);
            aimingCircleRenderer.enabled = true;
            unblock = false;
        }
    }

    void OnPrimaryAction(InputValue value)
    {
        if (player.Alive && !blocking && !grabbing && !player.isFalling() && Time.timeScale == 1 && player.Stun <= 0)
            sword.Swing();
    }

    void OnSecondaryAction(InputValue value)
    {
        if (player.Alive && value.isPressed && !player.isFalling() && Time.timeScale == 1 && player.Stun <= 0 )
        {
            if (objects.Count > 0)
            {
                grabbing = true;
                player.CanLook = false;
            }
            else
            {
                blocking = true;
                shield.SetActive(true);
            }
            animator.SetBool("blocking", true);
            player.Speed = maxSpeed * speedMult;
            aimingCircleRenderer.enabled = false;
        }
        else
            unblock = true;
        
    }

    void OnTertiaryAction(InputValue value)
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Block"))
        {
            objects.Add(collision.collider.GetComponent<Block>());
            collision.collider.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Block"))
        {
            objects.Remove(collision.collider.GetComponent<Block>());
            collision.collider.GetComponent<SpriteRenderer>().color = Color.white;
            unblock = true;
        }
    }
}

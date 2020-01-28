using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable, IHealable
{
    public static PlayerController instance;

    new Rigidbody2D rigidbody2D;
    new Collider2D collider2D;
    public Animator animator = null;
    public string message = "";

    [SerializeField] GameObject playerTorso = null;
    [SerializeField] GameObject playerLegs = null;
    [SerializeField] GameObject playerArmLeft = null;
    [SerializeField] GameObject playerArmRight = null;
    [SerializeField] ParticleSystem BloodParticles = null;

    [SerializeField] Weapon defaultWeapon = null;
    public Weapon weapon;

    Vector2 Direction;
    Vector2 LookDirection;
    [SerializeField] float speed = 5f;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] float stun = 0;
    [SerializeField] bool faceTarget = true;

    [Header("HealthUI")]
    [SerializeField] float maxHealth = 200f;
    [SerializeField] float health = 100f;
    public Slider slider;
    public Image fillImage;
    public Color FullHealthColor;
    public Color ZeroHealthColor;

    [Header("Sound")]
    [SerializeField] string footStepFileName = null;
    [SerializeField] float footStepVolume = 0.30f;
    [SerializeField] float footStepPitchMin = 0.9f;
    [SerializeField] float footStepPitchMax = 1.1f;

    [SerializeField] string damagedFileName = null;
    [SerializeField] float damagedVolume = 0.30f;
    [SerializeField] float damagedPitchMin = 0.9f;
    [SerializeField] float damagedPitchMax = 1.1f;

    [SerializeField] string healFileName = null;
    [SerializeField] float healVolume = 0.30f;
    [SerializeField] float healPitchMin = 0.9f;
    [SerializeField] float healPitchMax = 1.1f;

    [SerializeField] string deathFileName = null;
    [SerializeField] float deathVolume = 0.30f;
    [SerializeField] float deathPitchMin = 0.9f;
    [SerializeField] float deathPitchMax = 1.1f;

    AudioPlayer m_audioPlayer;

    void Awake()
    {
        instance = this;
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        collider2D = this.GetComponent<Collider2D>();
        m_audioPlayer = this.GetComponentInChildren<AudioPlayer>();
        m_audioPlayer.addSFX(footStepFileName);
        m_audioPlayer.addSFX(damagedFileName);
        m_audioPlayer.addSFX(healFileName);
        m_audioPlayer.addSFX(deathFileName);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_audioPlayer.setSpatialBlend(0.0f);
        if (weapon.name != "Arms")
            Physics2D.IgnoreCollision(collider2D, weapon.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(collider2D, playerArmLeft.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(collider2D, playerArmRight.GetComponent<Collider2D>());
        SetHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1)
        {
            if (animator)
            {
                animator.SetFloat("Speed", Mathf.Abs(this.rigidbody2D.velocity.x) + Mathf.Abs(this.rigidbody2D.velocity.y));
            }

            if (weapon.checkDestroy())
            {
                Destroy(weapon.gameObject);
                SetDefaultWeapon();
            }

            //if (Input.GetKeyDown(KeyCode.Q)) DropWeapon();

            if (stun > 0) stun -= Time.deltaTime;

            else
            {
                playerTorso.transform.right = LookDirection;

                rigidbody2D.velocity = Direction * speed;

                if (Direction != Vector2.zero)
                {
                    playerLegs.transform.right = Direction;
                    if (faceTarget && Quaternion.Angle(playerTorso.transform.rotation, playerLegs.transform.rotation) > 90) playerLegs.transform.right = -1 * playerLegs.transform.right; // Keeps body facing mouse
                }
            }
        }
        //DEBUG
        //Debug.DrawRay(transform.position, playerTorso.transform.right.normalized, Color.red, .01f);
        //Debug.DrawRay(transform.position, playerLegs.transform.up, Color.green, .01f); 
    }

    void OnMove(InputValue value)
    {
        if (stun <= 0)
        {
            Direction = value.Get<Vector2>();
        }
        
    }

    void OnLook(InputValue value)
    {
        if (stun <= 0)
        {
            LookDirection = value.Get<Vector2>();
        }
    }

    void OnLookMouse(InputValue value)
    {
        if (stun <= 0)
        {
            var mouse = Mouse.current;
            LookDirection = (Vector2)(Camera.main.ScreenToWorldPoint(mouse.position.ReadValue()) - transform.position);
        }
    }

    void OnAttack()
    {
        if (stun <= 0 && weapon.checkDestroy())
        {
            Destroy(weapon.gameObject);
            SetDefaultWeapon();
        }
        weapon.Attack();
    }

    bool IsMoving => Direction != Vector2.zero;

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Checkout"))
        {
            message = "Press E or Mouse 2 to Checkout";
            if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.E))
            {
                UIManager.instance.EndGame(true, collider.gameObject.name);
                message = "";
            }
        }

        else
        {
            if (collider.CompareTag("WeaponGround"))
            {
                message = "Press E or Mouse 2 to Equip";
                if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.E))
                {
                    IPickupable itemComponent = collider.GetComponent<IPickupable>();
                    if (itemComponent != null)
                    {
                        DropWeapon();
                        itemComponent.pickUp();
                        defaultWeapon.gameObject.SetActive(false);
                        message = "";
                    }
                }
            }
            else
            {
                IPickupable itemComponent = collider.GetComponent<IPickupable>();
                if (itemComponent != null)
                {
                    itemComponent.pickUp();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("WeaponGround") || collision.gameObject.CompareTag("Checkout"))
        {
            message = "";
        }
    }

    public void Damage(float damage, float stun, Vector2 knockback)
    {
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(knockback);
        health -= damage;
        SetHealthUI();
        this.stun = stun;
        BloodParticles.Play();
        //StartCoroutine(screenShake.Shake(.02f, .05f));

        m_audioPlayer.playSFX(damagedFileName, damagedVolume, damagedPitchMin, damagedPitchMax);

        if (health <= 0)
        {
            m_audioPlayer.playSFX(deathFileName, deathVolume, deathPitchMin, deathPitchMax);
            UIManager.instance.EndGame(false, "Death");
        }
    }

    public void Heal(float healthDelta)
    {
        float newHealthValue = health + healthDelta;
        if (newHealthValue > maxHealth)
            health = maxHealth;
        else
            health = newHealthValue;
        SetHealthUI();

        m_audioPlayer.playSFX(healFileName, healVolume, healPitchMin, healPitchMax);
    }
    
    private void DropWeapon()
    {

        if (weapon.PickupVersion != null)
        {
            Debug.Log("Dropped" + weapon);
            WeaponOnGround groundWep = Instantiate(weapon.PickupVersion, transform.position, Quaternion.identity);
            groundWep.durability = weapon.Durability;
            Destroy(weapon.gameObject);
            SetDefaultWeapon();
        }
        else
        {
            Debug.Log(weapon.PickupVersion);
        }
    }

    private void SetDefaultWeapon()
    {
        defaultWeapon.gameObject.SetActive(true);
        weapon = defaultWeapon;
    }

    private void SetHealthUI()
    {
        slider.value = health;

        fillImage.color = Color.Lerp(ZeroHealthColor, FullHealthColor, health / 100); // 100 is the hardcoded starting health, might need to change later
    }

    private void playFootStepSFX()
    {
        m_audioPlayer.playSFX(footStepFileName, footStepVolume, footStepPitchMin, footStepPitchMax);
    }
}

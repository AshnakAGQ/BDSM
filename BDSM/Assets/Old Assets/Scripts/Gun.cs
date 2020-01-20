using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{

    //OTHER VARIABLES
    [Header("STATS")]
    [SerializeField] GameObject bulletSpawn = null;
    [SerializeField] GameObject bullet = null;
    [SerializeField] float rateOfFire = 1f;
    

    [Header("Audio")]
    [SerializeField] string pickupFileName = null;
    [SerializeField] float pickupVolume = 0.65f;
    [SerializeField] float pickupPitchMinimum = 0.95f;
    [SerializeField] float pickupPitchMaximum = 1.05f;

    [SerializeField] string fireFileName = null;
    [SerializeField] float fireVolume = 0.65f;
    [SerializeField] float firePitchMinimum = 0.90f;
    [SerializeField] float firePitchMaximum = 1.10f;

    bool isBroken = false;
    float attackTimer = 0.01f;
    AudioPlayer m_audioPlayer;
    float attackTime = 1f;
    PlayerController player;

    private void Awake()
    {
        player = PlayerController.instance;
        m_audioPlayer = GetComponentInChildren<AudioPlayer>();
        m_audioPlayer.addSFX(pickupFileName);
        m_audioPlayer.addSFX(fireFileName);
    }

    protected void Start()
    {
        if (this.transform.parent.CompareTag("PlayerTorso"))
        {
            m_audioPlayer.setSpatialBlend(0.0f);
        }

        m_audioPlayer.playSFX(pickupFileName, pickupVolume, pickupPitchMinimum, pickupPitchMaximum);
    }

    private void Update()
    {
        attackTime += Time.deltaTime;
    }

    public override void Attack()
    {
        if (attackTime >= rateOfFire)
        {
            attackTime = 0;
            StartCoroutine(startAttackTimer(attackTimer));
            GameObject newBullet = Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            Durability--;
            if (Durability == 0) StartCoroutine(waitAndDestroy(rateOfFire/2 - attackTimer));
            m_audioPlayer.playSFX(fireFileName, fireVolume, firePitchMinimum, firePitchMaximum);
        }
    }
    
    public override bool checkDestroy()
    {
        return isBroken;
    }

    private IEnumerator startAttackTimer(float time)
    {
        float memory = attackTimer;
        while (attackTimer < time)
        {
            attackTimer += Time.deltaTime;
            yield return null;
        }

        attackTimer = memory;
    }


    private IEnumerator waitAndDestroy(float time)
    {
        float timer = 0.0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        isBroken = true;
    }
}

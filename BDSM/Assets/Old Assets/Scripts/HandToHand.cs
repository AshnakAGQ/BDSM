using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandToHand : Weapon
{
    [Header("Battle Stats")]
    [SerializeField] float attackDuration = .4f;
    [SerializeField] public bool attacking = false;
    [SerializeField] public bool attackReset = false;
    float attackTime = 0;

    [Header("Sound")]
    [SerializeField] float whooshVolume = 0.65f;
    [SerializeField] float whooshPitchMinimum = 0.90f;
    [SerializeField] float whooshPitchMaximum = 1.10f;
    [SerializeField] string punchWhooshSoundEffect = null;
    [SerializeField] string punchWhooshSoundEffect2 = null;

    AudioPlayer m_audioPlayer;
    public Animator m_animator = null;
    List<string> whooshes = new List<string>();

    void Awake()
    {
        m_audioPlayer = GetComponentInChildren<AudioPlayer>();
        m_audioPlayer.addSFX(punchWhooshSoundEffect);
        m_audioPlayer.addSFX(punchWhooshSoundEffect2);
        whooshes.Add(punchWhooshSoundEffect);
        whooshes.Add(punchWhooshSoundEffect2);
        m_animator = this.GetComponent<Animator>();
    }

    private void Start()
    {
        if (this.transform.parent.CompareTag("PlayerTorso"))
        {
            m_audioPlayer.setSpatialBlend(0.0f);
        }

        weaponRange = .75f * transform.lossyScale.x;
    }

    void Update()
    {
        if (attacking)
        {
            attackTime += Time.deltaTime;
            if (attackTime >= attackDuration)
            {
                attacking = false;
                attackReset = true;
                attackTime = 0;
            }
        }
    }

    override public void Attack()
    {
        
        if (!attacking)
        {
            attacking = true;
            attackReset = false;
            attackTime = 0;
            int randNum = Random.Range(0, 2);
            if (randNum == 1)
            {
                m_animator.Play("PunchingLeftArm", 0, 0.0f);
            }

            else
            {
                m_animator.Play("PunchingRightArm", 0, 0.0f);
            }

            int randomIndex = Random.Range(0, whooshes.Count);
            m_audioPlayer.playSFX(whooshes[randomIndex], whooshVolume, whooshPitchMinimum, whooshPitchMaximum);
        }
    }

    
}

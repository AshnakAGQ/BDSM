using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dartTrap : ActivatableObject
{

    [SerializeField] bool auto = true;
    [SerializeField] float startDelay = 0;
    [SerializeField] float fireRate = 2;
    [SerializeField] GameObject dart = null;
    [SerializeField] Vector3 dartPosition;
    private float stopWatch = 0;
    private float shotTimer = 0;

    [SerializeField] AudioContainer sound = null;
    AudioPlayer m_AudioPlayer;

    private void Awake()
    {
        m_AudioPlayer = GetComponent<AudioPlayer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        dartPosition += this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (auto)
        {
            UpdateTimer();
            Shoot();
        }
    }

    void UpdateTimer()
    {
        stopWatch += Time.deltaTime;
        shotTimer += Time.deltaTime;
    }

    void Shoot()
    {
        if (stopWatch >= startDelay && shotTimer >= fireRate)
        {
            shotTimer = 0;
            Activate();
        }
    }

    public override void Activate()
    {
        Instantiate(dart, dartPosition, Quaternion.identity);
        m_AudioPlayer.PlaySFX(sound);
    }

    public override void Deactivate()
    {
        
    }
}

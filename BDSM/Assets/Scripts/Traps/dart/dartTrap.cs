using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dartTrap : MonoBehaviour
{

    [SerializeField] float startDelay = 0;
    [SerializeField] float fireRate = 2;
    [SerializeField] GameObject dart = null;
    [SerializeField] Vector3 dartPosition;
    private float stopWatch = 0;
    private float shotTimer = 0;


    // Start is called before the first frame update
    void Start()
    {
        dartPosition += this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
        Shoot();
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
            Instantiate(dart, dartPosition, Quaternion.identity);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Weapon : MonoBehaviour
{

    [Header("Weapon Fields")]
    [SerializeField] protected float weaponRange = 0;
    [SerializeField] protected int durability = 15;
    [SerializeField] protected int maxDurability = 15;
    [SerializeField] protected Sprite sprite;
    [SerializeField] protected WeaponOnGround pickupVersion;

    public Sprite Sprite { get => sprite; }
    public int Durability { get => durability; set => durability = value; }
    public int MaxDurability { get => maxDurability; }
    public float WeaponRange { get => weaponRange; }
    public WeaponOnGround PickupVersion { get => pickupVersion; }

    virtual public void Attack()
    {
        Debug.Log("Wrong Function");
    }

    virtual protected void OnEnable()
    {
       Durability = maxDurability;
    }

    virtual public bool checkDestroy()
    {
        return false;
    }
}

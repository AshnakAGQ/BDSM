using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dartScript : MonoBehaviour
{
    //Speed of the bullet projectile.
    [SerializeField] public float speed = 30f;
    //need a reference to a rigidbody
    public Rigidbody2D rigid;
    //how much damage the bullet can do
    [SerializeField] public int dartDamage = 20;
    //this is a timer so in case the bullet somehow goes out of bounds
    //it will despawn itself. 
    public float timer;
    [SerializeField] private string opponent = null;
    [SerializeField] float stun = 0f;
    [SerializeField] int knockback = 10;


    List<IDamageable> damaged;


    private void Awake()
    {
        damaged = new List<IDamageable>();
 
    }

   
    // Update is called once per frame
    void Update()
    {
        //up because that's my default position.
        //firepoint will tell the bullet where to go.
        
        rigid.velocity = transform.right * speed;

        //In case the bullet somehow ends up outside I am destroying it.
        timer += 0.5F * Time.deltaTime;
        if (timer >= 3)
        {
            Destroy(gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageableComponent = collision.gameObject.GetComponent<IDamageable>();

        if (damageableComponent != null && !damaged.Contains(damageableComponent))
        {
            damageableComponent.Damage(dartDamage, stun, knockback * (Vector2)(collision.transform.position - transform.position));
            damaged.Add(damageableComponent);
        }
        Destroy(this.gameObject);
    }

    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Combat Data")]
    [SerializeField] float knockback = 100;
    [SerializeField] float damage = 40;
    [SerializeField] float stun = 0.1f;

    [Header("Audio")]
    [SerializeField] GameObject bulletHitObject = null;
    [SerializeField] GameObject bulletMissObject = null;
    float lifetime = 0.0f;

    private void Update()
    {
        if (lifetime >= 5.0f)
            Destroy(this.gameObject);
        lifetime += Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Player"))
        {
            IDamageable damageableComponent = collision.collider.GetComponent<IDamageable>();
            if (damageableComponent != null)
            {
                damageableComponent.Damage(damage, stun, knockback * (Vector2)(collision.transform.position - transform.position));
                Instantiate(bulletHitObject, this.transform.position, this.transform.rotation);
            }
        }
        else
        {
            Instantiate(bulletMissObject, this.transform.position, this.transform.rotation);
        }

        Destroy(this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] float healthRestore = 0.0f;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        IHealable healComponent = collider.GetComponent<IHealable>();
        if (healComponent != null)
        {
            healComponent.Heal(healthRestore);
            Destroy(this.gameObject);
        }
    }
}

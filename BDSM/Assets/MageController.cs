using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MageController : MonoBehaviour
{
    [SerializeField] Transform aimingCircle = null;
    [SerializeField] Transform targetingReticle = null;

    [Header("Primary")]
    [SerializeField] GameObject fireball = null;
    [SerializeField] GameObject wind = null;
    [SerializeField] int range = 5;
    [SerializeField] float primaryCooldown = 1;
    [SerializeField] float primaryTimer = 0;
    [SerializeField] float secondaryCooldown = 7.0f;
    [SerializeField] float secondaryTimer = 0;
    PlayerController playerController = null;
    PlayerInput playerInput = null;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
    }

    void OnPrimaryAction(InputValue value)
    {
        if (Time.timeScale == 1 && fireball != null && primaryTimer <= 0)
        {
            Fireball ball = Instantiate(fireball, transform.position + aimingCircle.right, aimingCircle.rotation).GetComponent<Fireball>();
            ball.Target = targetingReticle.position;
            primaryTimer = primaryCooldown;
        }
    }

    void OnSecondaryAction(InputValue value)
    {
        if (Time.timeScale == 1 && wind != null && secondaryTimer <= 0)
        {
            Vector3 buffer = ((targetingReticle.position - this.transform.position).normalized * 0.4f);
            Wind newWind = Instantiate(wind, transform.position + aimingCircle.right + buffer, aimingCircle.rotation).GetComponent<Wind>();
            newWind.Target = targetingReticle.position;
            secondaryTimer = secondaryCooldown;
        }
    }

    void OnTertiaryAction(InputValue value)
    {

    }

    private void Update()
    {
        if (Time.timeScale == 1)
        {
            Vector2 target = playerController.lookDirection * (playerInput.currentControlScheme == "Gamepad" ? range : 1);

            if (target.magnitude < 1)
                targetingReticle.localPosition = aimingCircle.right * 1000;
            else
                targetingReticle.localPosition = target;

            if (primaryTimer > 0)
                primaryTimer -= Time.deltaTime;

            if (secondaryTimer > 0)
                secondaryTimer -= Time.deltaTime;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsPage : MonoBehaviour
{
    [SerializeField] GameObject creditsPage = null;
    private bool isActive = false;

    public void ToggleCredits()
    {
        isActive = !isActive;
        creditsPage.SetActive(isActive);
    }

}

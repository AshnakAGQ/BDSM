﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Pause_menu : MonoBehaviour
{
    public static Pause_menu instance = null;
    public GameObject pause_menu;
    private bool paused = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }


    void Start()
    {

    }

    void Update()
    { 
        
    }

    public void Pause(bool enable)
    {
        if (enable)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        pause_menu.SetActive(enable);
        paused = enable;
    }

    void Temp()
    {
        Debug.Log("hey");
    }

    public void Resume()
    {
        if(paused)
        {
            Time.timeScale = 1;
            pause_menu.SetActive(false);
            paused = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Pause_menu : MonoBehaviour
{
    public static Pause_menu instance = null;
    public GameObject pause_menu;
    public Button resume_button;
    public GameObject menu_button;
    public GameObject exit_button;
    private bool paused;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        paused = false;
    }

    // Update is called once per frame
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


    public void checkResumeButton(InputValue value)
    {
        
        if(paused)
        {
            Time.timeScale = 1;
            pause_menu.SetActive(false);
            paused = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance = null;
    public GameObject pauseMenu = null;
    public GameObject gameOverMenu = null;
    public GameObject warriorHP = null;
    public GameObject mageHP = null;
    public bool gameWon = false;
    int levelGold;

    private bool paused = false;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        levelGold = GameManager.instance.Score;
    }

    public static void GameOver()
    {
        instance.gameOverMenu.SetActive(true);
        if (!instance.gameWon) GameManager.instance.Score = instance.levelGold;
    }

    public static void WinGame()
    {
        instance.gameOverMenu.GetComponent<TextMeshProUGUI>().text = "YOU WIN\n<size=20> Left Click or Press \"Options\"\nTo Continue";
        instance.gameOverMenu.SetActive(true);
        instance.gameWon = true;
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
        pauseMenu.SetActive(enable);
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
            pauseMenu.SetActive(false);
            paused = false;
        }
    }
}

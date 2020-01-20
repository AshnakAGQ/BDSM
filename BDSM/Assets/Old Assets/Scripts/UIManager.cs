using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [SerializeField] GameObject UI = null;
    [SerializeField] TextMeshProUGUI scoreText = null;
    [SerializeField] Image weapon = null;
    [SerializeField] Sprite defaultWeapon = null;
    [SerializeField] TextMeshProUGUI weaponDurability = null;
    [SerializeField] TextMeshProUGUI prompt = null;

    [SerializeField] TextMeshProUGUI timerText = null;
    public float timeLeft = 300f;
    private float timeMin = 0f;
    private float timeSec = 0f;

    [SerializeField] GameObject pauseMenu = null;

    [SerializeField] GameObject endScreen = null;
    [SerializeField] TextMeshProUGUI message = null;
    [SerializeField] TextMeshProUGUI endScore = null;
    [SerializeField] TextMeshProUGUI endPrice = null;

    bool isPaused = false;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        UpdateTimer();

        scoreText.text = "$" + GameManager.instance.score;
        prompt.text = PlayerController.instance.message;

        if (PlayerController.instance.weapon is HandToHand)
        {
            weapon.sprite = defaultWeapon;
            weaponDurability.text = "";
        }
        else
        {
            weapon.sprite = PlayerController.instance.weapon.Sprite;
            weaponDurability.text = PlayerController.instance.weapon.Durability + "/" + PlayerController.instance.weapon.MaxDurability
                + "\nPress Q to Drop";
        }

        scoreText.text = "$" + GameManager.instance.score;

        if (timeLeft == 0) EndGame(false, "Time");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseControl();
        }
    }

    void UpdateTimer()
    {
        if (timeLeft > 0)
            timeLeft -= Time.deltaTime;
        else
            timeLeft = 0;
        timeMin = timeLeft / 60;
        timeSec = timeLeft % 60;
        if (timeSec <= 10)
        {
            timerText.text = string.Format("{0}:0{1}", (int)timeMin, (int)timeSec);
        }
        else
        {
            timerText.text = string.Format("{0}:{1}", (int)timeMin, (int)timeSec);
        }
    }

    public void PauseControl()
    {
        isPaused = !isPaused;

        if( isPaused )
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }

        if (isPaused)
            UI.SetActive(false);
        else
            UI.SetActive(true);
    }
  
    public void EndGame(bool win, string condition)
    {
        Time.timeScale = 0;
        int score = GameManager.instance.score;
        GameObject.Find("Score").SetActive(false);
        endScreen.SetActive(true);
        
        if (win)
        {
            if (condition == "RegisterCheckout")
            {
                message.text = "YOU SURVIVED";
                endScore.text = "Total Value of Your Cart: $" + score;
                endPrice.text = "You Paid: $" + Mathf.Round(score * .25f * 100f) / 100f;
                message.color = Color.green;

            }
            else
            {
                message.text = "YOU ESCAPED";
                endScore.text = "You Stole $" + score + " Worth of Items";
                endPrice.text = "";
                message.color = Color.blue;
            }
            


            
        }
        else if (condition == "Time")
        {
            message.text = "TIME'S UP";
            message.color = Color.red;
            endScore.text = "Total Value of Your Cart: $" + score;
            endPrice.text = "";
        }
        else
        {
            message.text = "YOU DIED";
            message.color = Color.red;
            endScore.text = "Total Value of Your Cart: $" + score;
            endPrice.text = "";

        }

        Destroy(this);
    }
}

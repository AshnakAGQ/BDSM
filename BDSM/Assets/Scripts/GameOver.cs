using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textbox;

    private void Update()
    {
        textbox.text = "Score: " + GameManager.instance.Score + "\nPress any key to restart";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Keyboard.current.anyKey.isPressed || Mouse.current.leftButton.isPressed|| Gamepad.current != null && (Gamepad.current.startButton.isPressed || Gamepad.current.aButton.isPressed))
        {
            SceneManager.LoadScene(0);
            Destroy(GameManager.instance.gameObject);
        }
    }
}

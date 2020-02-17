using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class StartMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textbox;
    readonly string[] B = { "Brave ", "Buddy ", "Buff ", "Bound " };
    readonly string[] D = { "Dungeon ", "Dank ", "Dangerous " };
    readonly string[] S = { "Spelunking ", "Slaying ", "Survival "};
    readonly string[] M = { "Mayhem", "Monsters", "Madness", "Mission" };

    private void Start()
    {
        string b = B[Random.Range(0, B.Length)];
        string d = D[Random.Range(0, D.Length)];
        string s = S[Random.Range(0, S.Length)];
        string m = M[Random.Range(0, M.Length)];
        textbox.text = b + d + s + m;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Keyboard.current.anyKey.isPressed || Mouse.current.leftButton.isPressed || Gamepad.current != null && (Gamepad.current.startButton.isPressed || Gamepad.current.aButton.isPressed))
        {
            SceneManager.LoadScene(1);
        }
    }
}

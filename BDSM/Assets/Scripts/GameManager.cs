using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public int score { get; private set; } = 0;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int score)
    {
        this.score += score;
        //m_audioPlayer.playSFX(pointsPlusFileName, pointsPlusVolume, pointsPlusPitchMinimum, pointsPlusPitchMaximum);
    }
}

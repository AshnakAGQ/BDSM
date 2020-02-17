using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public int Score { get => score; set => score = value; }

    [SerializeField] private int score;
    public static GameObject [] players;

    [Header("Sound")]
    [SerializeField] public AudioContainer goldPickup;
    AudioPlayer m_AudioPlayer;


    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        if (instance)
        {
            score = instance.score;
            Destroy(instance.gameObject);
        }
        instance = this;
        m_AudioPlayer = this.GetComponent<AudioPlayer>();
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Cursor.visible = false;
}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int score)
    {
        this.score += score;
        m_AudioPlayer.PlaySFX(goldPickup);
        //m_audioPlayer.playSFX(pointsPlusFileName, pointsPlusVolume, pointsPlusPitchMinimum, pointsPlusPitchMaximum);
    }
}

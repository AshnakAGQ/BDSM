using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public int score { get; private set; } = 0;

    [Header("Points")]
    [SerializeField] string pointsPlusFileName = null;
    [SerializeField] float pointsPlusVolume = 0.65f;
    [SerializeField] float pointsPlusPitchMinimum = 0.95f;
    [SerializeField] float pointsPlusPitchMaximum = 1.05f;

    AudioPlayer m_audioPlayer;

    [Header("Spawning")]
    [SerializeField] Camera mainCamera = null; 
    [SerializeField] bool spawningEnemies = false;
    [SerializeField] int spawnTries = 0;
    [SerializeField] int maxEnemies = 0;
    [SerializeField] bool autoCalculateMaxEnemies = false;
    [SerializeField] List<GameObject> spawnList = null;
    [SerializeField] List<float> spawnChance = null;
    [SerializeField] RectTransform spawnArea = null;
    int enemies = 0;


    void Awake()
    {
        instance = this;
        m_audioPlayer = GetComponentInChildren<AudioPlayer>();
        m_audioPlayer.addSFX(pointsPlusFileName);
    }

    private void Start()
    {
        Time.timeScale = 1;

        m_audioPlayer.setSpatialBlend(0.0f);

        if (autoCalculateMaxEnemies) maxEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        enemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    private void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (spawningEnemies && enemies < maxEnemies)
        {
            GameObject enemy = ChooseEnemy();
            Vector2 spawn = FindSpawn(enemy);
            if (spawn != Vector2.zero) Instantiate(enemy, spawn, Quaternion.identity);
        }
    }


    public void AddScore(int score)
    {
        this.score += score;
        m_audioPlayer.playSFX(pointsPlusFileName, pointsPlusVolume, pointsPlusPitchMinimum, pointsPlusPitchMaximum);
    }

    GameObject ChooseEnemy()
    {
        float roll = Random.value;

        for (int i = 0; i < spawnList.Count && i < spawnChance.Count; i++)
        {
            if (roll < spawnChance[i]) return spawnList[i];
            else roll -= spawnChance[i];
        }

        return null;
    }

    Vector2 FindSpawn(GameObject enemy)
    {
        Vector2 spawnPoint;

        for (int i = 0; i < spawnTries; i++)
        {
            float x = Random.Range(spawnArea.rect.xMin, spawnArea.rect.xMax) + spawnArea.position.x;
            float y = Random.Range(spawnArea.rect.yMin, spawnArea.rect.yMax) + spawnArea.position.y;
            spawnPoint = new Vector2(x, y);
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(spawnPoint);
            
            if ((screenPoint.x < 0 || screenPoint.x > mainCamera.pixelWidth) && (screenPoint.y < 0 || screenPoint.y > mainCamera.pixelHeight))
            {
                Collider2D spawnConflict = Physics2D.OverlapCircle(spawnPoint, Mathf.Max(enemy.GetComponent<Collider2D>().bounds.extents.x,
                                                                                            enemy.GetComponent<Collider2D>().bounds.extents.y));
                if (spawnConflict == null)
                    return spawnPoint;
                else Debug.Log("Spawn conflict with " + spawnConflict.name);
            }
        }
        return Vector2.zero;
    }
}

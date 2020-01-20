using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    [SerializeField] float timeAlive = 0.5f;

    [Header("Audio")]
    [SerializeField] string collisionFileName = null;
    [SerializeField] float collisionVolume = 0.30f;
    [SerializeField] float collisionPitchMinimum = 0.95f;
    [SerializeField] float collisionPitchMaximum = 1.05f;

    AudioPlayer m_audioPlayer;

    private void Awake()
    {
        m_audioPlayer = GetComponentInChildren<AudioPlayer>();
        m_audioPlayer.setSpatialBlend(1.0f);
        m_audioPlayer.addSFX(collisionFileName);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(playandDestroy(timeAlive));
    }

    private IEnumerator playandDestroy(float time)
    {
        m_audioPlayer.playSFX(collisionFileName, collisionVolume, collisionPitchMinimum, collisionPitchMaximum);

        float timePassed = 0.0f;
        while (timePassed < time)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject);
    }
}

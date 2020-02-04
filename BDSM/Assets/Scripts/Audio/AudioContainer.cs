using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AudioContainer : ScriptableObject
{
    [Tooltip("The file of the sound to be played")]
    [SerializeField] public AudioClip clip;

    [Tooltip("Proportion of the volume of the sound played")]
    [Range(0.0f, 1.0f)]
    [SerializeField] public float volume;

    [Tooltip("The minimum pitch of the sound when it is played\n\nClips will play at a random pitch between the minimum and maximum values.")]
    [Range(-3.0f, 3.0f)]
    [SerializeField] public float pitchMin;

    [Tooltip("The maximum pitch of the sound when it is played\n\nClips will play at a random pitch between the minimum and maximum values.")]
    [Range(-3.0f, 3.0f)]
    [SerializeField] public float pitchMax;

    [Tooltip("Proportion of the dimensionality of the sound played\n\nA value of 0 represents a 2D sound with no directional properties\n\nA value of 1 represents a 3D sound with full directional properties")]
    [Range(0.0f, 1.0f)]
    [SerializeField] public float spatialBlend;

    [Tooltip("The maximum distance between the AudioSource Component that plays this clip and an AudioListener Component that can hear the clip")]
    [SerializeField] public float maxDistance;
}

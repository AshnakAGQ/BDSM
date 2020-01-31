using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AudioContainer : ScriptableObject
{
    [SerializeField] public string fileName;

    [SerializeField] public float volume;

    [SerializeField] public float pitchMin;

    [SerializeField] public float pitchMax;

    [SerializeField] public float spatialBlend;

    [SerializeField] public float maxDistance;
}

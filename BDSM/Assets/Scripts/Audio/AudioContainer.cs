using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AudioContainer : ScriptableObject
{
    [SerializeField] string fileName;
    [SerializeField] float volume;
    [SerializeField] float pitchMin;
    [SerializeField] float pitchMax;
}

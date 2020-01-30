using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Definitions for all UnityEvents.
/// Any script can call any particular UnityEvent by referencing this script.
/// 
/// Call a particular UnityEvent with:
/// UnityEvents.EVENT_NAME.Invoke(arguments);
/// </summary>
public static class UnityEvents
{
    public class AudioClipUnityEvent : UnityEvent<AudioClip> { }
    public static AudioClipUnityEvent PlaySoundEvent;
}

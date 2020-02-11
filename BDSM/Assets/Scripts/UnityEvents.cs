using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Definitions for all UnityEvents.
/// Any script can call any particular UnityEvent by referencing this script.
/// 
/// Call a particular UnityEvent with:
/// **referenceToTheEvent**.Invoke(arguments);
/// </summary>



public class GameObjectUnityEvent : UnityEvent<GameObject> { }

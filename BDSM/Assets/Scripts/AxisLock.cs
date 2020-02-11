using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attaching this script to an object will lock it along certain set of axes
/// The object will not be able to change its transform.position along the specified axes
/// </summary>

public class AxisLock : MonoBehaviour
{
    [SerializeField] public bool lockX;
    [SerializeField] public bool lockY;
    [SerializeField] public bool lockZ;

    [System.NonSerialized] private Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = this.transform.position;

        if (lockX)
        {
            newPosition.x = initialPosition.x;
        }
        if (lockY)
        {
            newPosition.y = initialPosition.y;
        }
        if (lockZ)
        {
            newPosition.z = initialPosition.z;
        }

        this.transform.position = newPosition;
    }
}

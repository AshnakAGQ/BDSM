using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonHandler : MonoBehaviour
{
    public UnityEvent buttonClick;

    void Awake()
    {
        if(buttonClick == null)
        {
            buttonClick = new UnityEvent();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnMouseUp()
    {
        print("up");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

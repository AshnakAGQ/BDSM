using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{

    public Slider slider;



    public void SetMaxHealthBar(float health)
    {
        //when a player is created it should pass the initial health value in
        slider.maxValue = health;
        slider.value = health;
    }


    public void SetHealthBar(float health)
    {
        //whenever we call the setHealth function then it's going to 
        //set the current health of the bar
        slider.value = health;
    }



}

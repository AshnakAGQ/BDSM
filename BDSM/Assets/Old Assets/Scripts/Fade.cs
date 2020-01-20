using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public float FadeSpeed;

    public bool doneFading;
    private Image image;// = GameObject.Find("Fader").GetComponent<Image>();
    private float targetAlpha;

    // Use this for initialization
    void Start()
    {
        this.image = this.GetComponent<Image>();
        this.targetAlpha = this.image.color.a;
    }

    // Update is called once per frame
    void Update()
    {
/*
        Color curColor = this.image.color;
        float alphaDiff = Mathf.Abs(curColor.a - this.targetAlpha);
        if (alphaDiff > 0.0001f)
        {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, this.FadeSpeed * Time.deltaTime);
            this.image.color = curColor;
        }

        if (alphaDiff == 0f)
            doneFading = true;*/
    }

    public void FadeOut()
    {
        doneFading = false;
        this.targetAlpha = 0.0f;
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }
    public IEnumerator FadeInRoutine()
    {
        Debug.Log("trying to fade");

        this.targetAlpha = 1.0f;
        Color curColor = this.image.color;
        float alphaDiff = Mathf.Abs(curColor.a - this.targetAlpha); ;
        while (alphaDiff > 0.0001f)
        {
            alphaDiff = Mathf.Abs(curColor.a - this.targetAlpha);
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, this.FadeSpeed * Time.deltaTime);
            this.image.color = curColor;
            yield return null;
        }
    }
}
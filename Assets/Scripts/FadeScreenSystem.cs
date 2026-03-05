using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreenSystem : MonoBehaviour
{
    public Image fadeScreenImage;
    private bool bFading, bFadeOut;
    private float rate;

    public static Action FadeInStart = delegate { };
    public static Action FadeOutStart = delegate { };
    public static Action FadeOutEnd = delegate { };
    public static Action FadeInEnd = delegate { };

    private void Start()
    {
        FadeScreenGA.FadeOut += FadeOut;
        FadeScreenGA.FadeIn += FadeIn;
    }
    private void OnDestroy()
    {
        FadeScreenGA.FadeOut -= FadeOut;
        FadeScreenGA.FadeIn -= FadeIn;
    }
    private void FadeIn()
    {
        Debug.Log("fade in");
        bFadeOut = false;
        if(bFading)
        {
            rate = 1 - rate;
            return;
        }
        FadeInStart();
        StartCoroutine(nameof(FadeScreen));
    }
    private void FadeOut()
    {
        bFadeOut = true;
        if (bFading)
        {
            rate = 1 - rate;
            return;
        }
        FadeOutStart();
        StartCoroutine(nameof(FadeScreen));
    }
    IEnumerator FadeScreen()
    {
        rate = 0;
        bFading = true;
        while(rate < 1)
        {
            rate += Time.deltaTime * 2;
            if (bFadeOut)
            {
                fadeScreenImage.color = Color.Lerp(Color.clear, Color.white, rate);
                if(rate >= 1)
                    FadeOutEnd();
            }
            else
            {
                fadeScreenImage.color = Color.Lerp(Color.white, Color.clear, rate);
                if(rate >= 1) 
                    FadeInEnd();
            }

            yield return null;
        }
       
        bFading = false;
    }
}

using System;
using UnityEngine;
[Serializable]
public class FadeScreenGA : GameAction
{
    public bool bFadeOut;

    public static Action FadeIn = delegate { };
    public static Action FadeOut = delegate { };

    public FadeScreenGA()
    {
        actionName = "Fades screen in or out";
    }
    public override void Action()
    {
        if(bFadeOut) 
            FadeOut();
        else
            FadeIn();
    }
}

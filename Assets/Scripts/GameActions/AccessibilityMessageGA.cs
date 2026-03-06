using System;
using UnityEngine;
[Serializable]
public class AccessibilityMessageGA : GameAction
{
    public AudioSource aSource;
    public AudioClip accessibilityOn,accessiblityOff;

    public AccessibilityMessageGA()
    {
        actionName = "Informs user of accessibility mode";
    }
    public override void Action()
    {
        if(GameMaster.bTextToSpeech)
            aSource.PlayOneShot(accessibilityOn);
        else
            aSource.PlayOneShot(accessiblityOff);
    }
}

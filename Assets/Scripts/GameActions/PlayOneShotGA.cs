using System;
using UnityEngine;
[Serializable]
public class PlayOneShotGA : GameAction
{
    public bool bAccessibility;
    public AudioClip aClip;
    public AudioSource aSource;
    public PlayOneShotGA()
    {
        actionName = "Plays one shot audio";
    }
    public override void Action()
    {
        if(bAccessibility && !GameMaster.bTextToSpeech) return;
        
        aSource.PlayOneShot(aClip);
    }
}

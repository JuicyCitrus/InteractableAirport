using System;
using UnityEngine;
[Serializable]
public class ToggleTextToSpeechGA : GameAction
{
   public ToggleTextToSpeechGA()
    {
        actionName = "Toggles text to speech";
    }
    public override void Action()
    {
        GameMaster.bTextToSpeech = !GameMaster.bTextToSpeech;
    }
}

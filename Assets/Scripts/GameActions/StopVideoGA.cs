using System;
using UnityEngine;
using UnityEngine.Video;
[Serializable]
public class StopVideoGA : GameAction
{
    public VideoPlayer vPlayer;
    public StopVideoGA()
    {
        actionName = "Stops video play back";
    }
    public override void Action()
    {
        vPlayer.Stop();
        vPlayer.enabled = false;
    }
}

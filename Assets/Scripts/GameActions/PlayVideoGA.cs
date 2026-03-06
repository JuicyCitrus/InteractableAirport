using System;
using UnityEngine;
using UnityEngine.Video;
[Serializable]
public class PlayVideoGA : GameAction
{
    public bool bLoop;
    public AudioSource aSource;
    public VideoPlayer vPlayer;
    public VideoClip clip;
    public PlayVideoGA()
    {
        actionName = "Plays video clip";
    }
    public override void Action()
    {
        vPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        vPlayer.EnableAudioTrack(0,true);
        vPlayer.SetTargetAudioSource(0,aSource);
        vPlayer.clip = clip;
        vPlayer.Play();
        vPlayer.isLooping = bLoop;       
    }
}

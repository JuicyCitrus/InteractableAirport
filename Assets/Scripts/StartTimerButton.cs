using UnityEngine;

public class StartTimerButton : WorldSpaceButton
{
    public override void Push()
    {
        base.Push();

        Timer.Instance.StartTimer();
    }
}

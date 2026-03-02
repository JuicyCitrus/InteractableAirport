using UnityEngine;

public class ApprovedButton : WorldSpaceButton
{
    public override void Push()
    {
        if (XRaySystem.Instance.currentLuggage == null)
            return;

        base.Push();

        XRaySystem.Instance.currentLuggage.ContinueMoving();
    }
}

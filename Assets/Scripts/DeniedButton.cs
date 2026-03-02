using UnityEngine;

public class DeniedButton : WorldSpaceButton
{
    public override void Push()
    {
        if (XRaySystem.Instance.currentLuggage == null)
            return;

        base.Push();

        XRaySystem.Instance.currentLuggage.markedAsContraband = true;
        XRaySystem.Instance.currentLuggage.ContinueMoving();
    }
}

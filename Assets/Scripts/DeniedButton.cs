using UnityEngine;

public class DeniedButton : WorldSpaceButton
{
    public override void Push()
    {
        // Do not allow pushing if nothing is in the x-ray machine
        if (XRaySystem.Instance.currentLuggage == null)
            return;

        base.Push();

        // Move the bag along the conveyor belt again and mark it as contraband
        XRaySystem.Instance.currentLuggage.ContinueMoving();
        XRaySystem.Instance.currentLuggage.markedAsContraband = true;

        // Score based on whether or not the luggage had contraband
        if (XRaySystem.Instance.currentLuggage.hasContraband)
        {
            SecurityScoring.Instance.successfulIdentifications++;
        }
        else
        {
            SecurityScoring.Instance.failedIdentifications++;
        }

        // Set the x-ray machine to be empty again
        XRaySystem.Instance.currentLuggage = null;

        // Spawn the next bag
        LuggageSpawner.Instance.SpawnLuggage();
    }
}

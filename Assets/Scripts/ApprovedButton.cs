using UnityEngine;

public class ApprovedButton : WorldSpaceButton
{
    public override void Push()
    {
        base.Push();
        // Do not allow pushing if nothing is in the x-ray machine
        if (XRaySystem.Instance.currentLuggage == null)
        {
            Debug.Log("No luggage in x-ray machine!");
            return;
        }

        // base.Push();

        // Move the bag along the conveyor belt again
        XRaySystem.Instance.currentLuggage.ContinueMoving();
        
        // Score based on whether or not the luggage had contraband
        if(XRaySystem.Instance.currentLuggage.hasContraband)
        {
            SecurityScoring.Instance.failedIdentifications++;
        }
        else
        {
            SecurityScoring.Instance.successfulIdentifications++;
        }

        // Set the x-ray machine to be empty again
        XRaySystem.Instance.currentLuggage = null;

        // Spawn the next bag
        LuggageSpawner.Instance.SpawnLuggage();

        // Increment luggages cleared
        SecurityScoring.Instance.luggagesCleared++;
    }
}

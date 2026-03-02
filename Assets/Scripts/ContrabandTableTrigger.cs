using UnityEngine;

public class ContrabandTableTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SecurityLuggage>(out SecurityLuggage luggage) && luggage.markedAsContraband)
        {
            SecurityScoring.Instance.luggagesCleared++;
        }

        // End round if it's the last bag
        if (SecurityScoring.Instance.luggagesCleared >= SecurityScoring.Instance.luggageInRound)
        {
            SecurityScoring.Instance.RoundOver();
        }
    }
}

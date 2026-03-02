using UnityEngine;

public class ApprovedTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SecurityLuggage>(out SecurityLuggage luggage))
        {
            luggage.StopAllCoroutines();
        }

        // End round if it's the last bag
        if(SecurityScoring.Instance.luggagesCleared >= SecurityScoring.Instance.luggageInRound)
        {
            SecurityScoring.Instance.RoundOver();
        }
    }
}

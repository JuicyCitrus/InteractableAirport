using UnityEngine;

public class ContrabandTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SecurityLuggage>(out SecurityLuggage luggage) && luggage.markedAsContraband)
        {
            luggage.StopAllCoroutines();
        }
    }
}

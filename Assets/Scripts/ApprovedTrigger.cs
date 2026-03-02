using UnityEngine;

public class ApprovedTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SecurityLuggage>(out SecurityLuggage luggage))
        {
            luggage.StopAllCoroutines();
        }
    }
}

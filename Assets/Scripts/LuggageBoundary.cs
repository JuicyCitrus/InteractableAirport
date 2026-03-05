using UnityEngine;

public class LuggageBoundary : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<SecurityLuggage>(out SecurityLuggage luggage))
        {
            luggage.StopAllCoroutines();
        }
    }
}

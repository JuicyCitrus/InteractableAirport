using UnityEngine;
using UnityEngine.UI;

public class XRayTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SecurityLuggage>(out SecurityLuggage luggage) && luggage != XRaySystem.Instance.currentLuggage)
        {
            luggage.isInXRayMachine = true;
            XRaySystem.Instance.currentLuggage = luggage;
            luggage.StopAllCoroutines();
            XRaySystem.Instance.SpawnImagesOnUI();
        }
    }
}

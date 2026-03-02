using UnityEngine;

public class XRayTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Luggage entered X-Ray machine.");

        if (other.TryGetComponent<SecurityLuggage>(out SecurityLuggage luggage) && luggage != XRaySystem.Instance.currentLuggage)
        {
            luggage.isInXRayMachine = true;
            XRaySystem.Instance.currentLuggage = luggage;
            XRaySystem.Instance.UpdateXRayImage(luggage.xRayImage);
            luggage.StopAllCoroutines();
        }
    }
}

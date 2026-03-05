using System.Collections;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    //custom gizmos for interaction trigger
    #if UNITY_EDITOR
    private void OnValidate()
    {
        
    }
    private void OnDrawGizmos()
    {
        
    }
#endif
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered interaction trigger.");
            // Add interaction logic here
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited interaction trigger.");
            // Add exit logic here
        }
    }
    private void ResetTrigger()
    {
        // Logic to reset the trigger state
    }
    IEnumerator ExecuteActions()
    {
        yield return new WaitForSeconds(0.5f);
    }
}

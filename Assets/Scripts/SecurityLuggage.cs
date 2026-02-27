using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SecurityLuggage : MonoBehaviour
{
    public Sprite xRayImage;
    public bool hasContraband;
    public bool isInXRayMachine = false;
    public Rigidbody rb;

    private void OnEnable()
    {
        StartCoroutine(MoveForward());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator MoveForward()
    {
        while(!isInXRayMachine)
        {
            rb.MovePosition(transform.position + transform.forward * Time.deltaTime);
            yield return null;
        }
    }
}

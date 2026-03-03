using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SecurityLuggage : MonoBehaviour
{
    [Header("Luggage Info")]
    public Sprite xRayImage;
    public bool hasContraband;
    public int luggageID;

    [Header("State")]
    public bool markedAsContraband = false;
    public bool isInXRayMachine = false;

    [Header("Movement")]
    public Rigidbody rb;
    public float movementSpeed = 5f;
    public float distanceForSecondMove = 10;

    private void OnEnable()
    {
        StartCoroutine(MoveForward());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void ContinueMoving()
    {
        isInXRayMachine = false;
        StartCoroutine(MoveForward());
    }

    public IEnumerator MoveForward()
    {
        while(!isInXRayMachine)
        {
            rb.MovePosition(transform.position + transform.forward * movementSpeed * Time.deltaTime);
            yield return null;
        }
    }
}

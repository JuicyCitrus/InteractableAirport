using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldSpaceButton : MonoBehaviour
{
    public Vector3 pushedPosition;
    public float pushDuration = 0.2f;

    private Vector3 originalPosition;
    private bool isPushing = false;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public virtual void Push()
    {
        if(isPushing) 
            return;

        StartCoroutine(PushCoroutine());
    }

    private IEnumerator PushCoroutine()
    {
        isPushing = true;
        float elapsedTime = 0f;

        while (elapsedTime < pushDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, pushedPosition, elapsedTime / pushDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < pushDuration)
        {
            transform.localPosition = Vector3.Lerp(pushedPosition, originalPosition, elapsedTime / pushDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isPushing = false;
    }
}

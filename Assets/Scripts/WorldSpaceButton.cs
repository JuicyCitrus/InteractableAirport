using System.Collections;
using UnityEngine;

public class WorldSpaceButton : MonoBehaviour
{
    public Vector3 pushedPosition;

    public virtual void Push()
    {
        StartCoroutine(PushCoroutine());
    }

    private IEnumerator PushCoroutine()
    {
        // Get the original position for returning after the push
        Vector3 originalPosition = transform.position;

        // Move towards the pushed position
        while (Vector3.Distance(transform.position, pushedPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, pushedPosition, Time.deltaTime * 5f);
            yield return null;
        }

        // Move back to the original position
        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, Time.deltaTime * 5f);
            yield return null;
        }
    }
}

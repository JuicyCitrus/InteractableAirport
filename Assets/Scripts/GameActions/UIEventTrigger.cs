using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventTrigger : MonoBehaviour
{
    [SerializeReference, SubclassSelector]
    public List<GameAction> actions;
    private bool bActive;
    public void TriggerActions()
    {
        if (bActive) return;
        StartCoroutine(nameof(DelayedActions));
    }
    IEnumerator DelayedActions()
    {
        bActive = true;
        foreach (GameAction action in actions)
        {
            yield return new WaitForSeconds(action.delay);
            action.Action();
        }
        bActive = false;
    }
}

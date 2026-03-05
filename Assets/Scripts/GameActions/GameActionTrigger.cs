using System.Collections;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class GameActionTrigger : MonoBehaviour
{
    public bool bTriggerOnce = false;

    [SerializeReference,SubclassSelector]
    public List<GameAction> enterActions, exitActions;
    private bool bActive,bEnterTriggered,bExitTriggered;
    private float localTimer = 0f;
    private void OnValidate()
    {
#if UNITY_EDITOR
        Undo.RegisterCompleteObjectUndo(this,"Run Actions setup");
#endif
        if(enterActions != null)
        {
            foreach(GameAction ga in enterActions)
            {
                if(ga != null)
                ga.Setup();
            }
        }
        if(exitActions != null)
        {
            foreach(GameAction ga in exitActions)
            {
                if(ga != null)
                ga.Setup(); 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {        
        if(bActive) //don't allow re-entry while actions are running
            return;

        if(bTriggerOnce && bEnterTriggered) //don't retrigger if set to only trigger once
            return;

        if(other.CompareTag("Player"))
        {  
            bEnterTriggered = true; 
            StartCoroutine(GameActionSequence(enterActions));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(bActive)
            return;

        if(bTriggerOnce && bExitTriggered)
            return;
        if(other.CompareTag("Player"))
        {
            bExitTriggered = true;
            StartCoroutine(GameActionSequence(exitActions));
        }
    }
    private void ResetSystem()
    {
        StopAllCoroutines();
        bEnterTriggered = false;
        bExitTriggered = false;
    }
     IEnumerator GameActionSequence(List<GameAction> actions)
    {        
        bActive = true;

        int counter = 0;;
        localTimer = 0;
        while(counter < actions.Count)
        {
            yield return null;
            localTimer += Time.deltaTime;
            
            if(localTimer >= actions[counter].delay)
            {
                actions[counter].Action();
                counter++;
                localTimer = 0;
            }
        }
        bActive = false;
    }
}

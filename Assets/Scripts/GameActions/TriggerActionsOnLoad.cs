using System.Collections.Generic;
using UnityEngine;

public class TriggerActionsOnLoad : MonoBehaviour
{
    [SerializeReference,SubclassSelector]
    public List<GameAction> actions;

    private void Start()
    {
        foreach(GameAction action in actions)
            action.Action();
    }
}

using System;
using UnityEngine;
[Serializable]
public class AnimationTriggerGA : GameAction
{
    public Animator animator;
    public string parameterName;
    public AnimationTriggerGA()
    {
        actionName = "Triggers animation using name provided";
    }
    public override void Action()
    {
        animator.SetTrigger(parameterName);
    }
}

using System;
using UnityEngine;
[Serializable]
public class GameAction
{
    public string actionName;
    public float delay = 0f;
    public virtual void Action(){}
    public virtual void ResetAction(){}
    public virtual void InitializeAction(){}
    public virtual void Setup(){} //used for editor setup
}

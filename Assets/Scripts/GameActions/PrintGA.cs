using System;
using UnityEngine;
[Serializable]
public class PrintGA : GameAction
{
    public string message = "Hello World";
    public PrintGA()
    {
        actionName = "Prints to Console";
    }
    public override void Action()
    {
        Debug.Log(message);
    }
}

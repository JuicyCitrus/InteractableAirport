using System;
using UnityEngine;
[Serializable]
public class QuitGA : GameAction
{
    public QuitGA()
    {
        actionName = "Quit Application";
    }
    public override void Action()
    {
        Application.Quit();
    }
}

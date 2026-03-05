using System.Collections.Generic;
using System;
using UnityEngine;
[Serializable]
public class GameObjectToggleGA : GameAction
{
    public bool bEnable;
    public List<GameObject> gameObjects;
    public GameObjectToggleGA()
    {
        actionName = "This enables/disables object";
    }
    public override void Action()
    {
        //do something in here
        foreach (GameObject go in gameObjects)
        {
            if(bEnable)
                go.SetActive(true);
            else
                go.SetActive(false);
        }
    }
}

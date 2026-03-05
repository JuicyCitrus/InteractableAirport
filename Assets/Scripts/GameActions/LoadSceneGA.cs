using System;
using UnityEngine;
using UnityEngine.SceneManagement;
[Serializable]
public class LoadSceneGA : GameAction
{
    public int sceneIndex;
    public string sceneName;
    public LoadSceneGA()
    {
        actionName = "Loads scene";
    }
    public override void Action()
    {
        if(sceneIndex > -1)
            SceneManager.LoadScene(sceneIndex);
        else
            SceneManager.LoadScene(sceneName);
    }
}

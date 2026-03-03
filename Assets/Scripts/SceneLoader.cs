using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class SceneLoader : MonoBehaviour
{
    public string SceneName;
    
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}

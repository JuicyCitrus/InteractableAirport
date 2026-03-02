using UnityEngine;

public class ButtonUI_Restart : ButtonUI
{
    public override void ButtonFunction()
    {
        // Restart the game by reloading the current scene
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}

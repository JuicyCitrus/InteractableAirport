using UnityEngine;

public class ButtonUI_ReturnToLobby : ButtonUI
{
    public string lobbySceneName = "Lobby";

    public override void ButtonFunction()
    {
        // Load the lobby scene
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(lobbySceneName);
    }
}

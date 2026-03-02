using TMPro;
using UnityEngine;

public class SecurityScoring : MonoBehaviour
{
    public static SecurityScoring Instance { get; private set; }

    [Header("Scoring Settings")]
    public int luggageInRound = 10;
    public int successfulIdentifications;
    public int failedIdentifications;

    [Header("UI Elements")]
    public GameObject roundOverScreen;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void RoundOver()
    {
        // Turn on UI and make the mouse usable again
        roundOverScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

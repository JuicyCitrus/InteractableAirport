using UnityEngine;
using TMPro;

public class CargoWinLoseController : MonoBehaviour
{
    public enum EndState { None, Win, Lose }

    [Header("Win Condition")]
    [Tooltip("How many suitcases must be placed to win (usually equals initialQueueCount).")]
    [SerializeField] private int targetPlacements = 6;

    [Header("UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject GiveUpPanel;
    [SerializeField] private TMP_Text placedCounterText;

    [Header("References")]
    [SerializeField] private SimpleFPSController fpsController;
    [SerializeField] private SuitcasePlacer suitcasePlacer;

    private int placedCount = 0;
    public EndState State { get; private set; } = EndState.None;

    private void Awake()
    {
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) winPanel.SetActive(false);
        if (GiveUpPanel) GiveUpPanel.SetActive(false);

        RefreshCounterUI();
    }

    public void SetTargetPlacements(int count)
    {
        targetPlacements = Mathf.Max(1, count);
        RefreshCounterUI();
    }

    /// <summary>Call this when a suitcase is successfully placed.</summary>
    public void NotifySuitcasePlaced()
    {
        if (State != EndState.None) return;

        placedCount++;
        RefreshCounterUI();

        if (placedCount >= targetPlacements)
            Win();
    }

    public void GiveUp()
    {
        if (State != EndState.None) return;
        Lose();
    }

    private void RefreshCounterUI()
    {
        if (!placedCounterText) return;
        placedCounterText.text = $"Placed Count : {placedCount} / {targetPlacements}";
    }

    private void Win()
    {
        State = EndState.Win;

        if (winPanel) winPanel.SetActive(true);
        if (losePanel) losePanel.SetActive(false);

        if (fpsController) fpsController.SetUIOpen(true);
        if (suitcasePlacer) suitcasePlacer.enabled = false;
    }

    private void Lose()
    {
        State = EndState.Lose;

        if (losePanel) losePanel.SetActive(true);
        if (winPanel) winPanel.SetActive(false);

        if (fpsController) fpsController.SetUIOpen(true);
        if (suitcasePlacer) suitcasePlacer.enabled = false;
    }

    public void TriggerLose_NoValidMoves()
    {
        if (GiveUpPanel) losePanel.SetActive(true);

        if (fpsController) fpsController.SetUIOpen(true);
        if (suitcasePlacer) suitcasePlacer.enabled = false;
    }
}
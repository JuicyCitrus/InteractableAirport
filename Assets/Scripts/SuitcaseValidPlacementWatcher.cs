using UnityEngine;

/// <summary>
/// It listens to SuitcasePlacer.OnNoValidMove and/or can be called directly.
///
/// This is intentionally ONLY about the "valid placement" reaction layer
/// </summary>
public class SuitcaseValidPlacementWatcher : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SuitcasePlacer placer;

    [Tooltip("Optional: call into your win/lose controller here if you want auto-loss.")]
    [SerializeField] private CargoWinLoseController winLose;

    private void OnEnable()
    {
        if (placer != null)
            placer.OnNoValidMove += HandleNoValidMove;
    }

    private void OnDisable()
    {
        if (placer != null)
            placer.OnNoValidMove -= HandleNoValidMove;
    }

    private void HandleNoValidMove()
    {
        winLose.TriggerLose_NoValidMoves();
    }
}
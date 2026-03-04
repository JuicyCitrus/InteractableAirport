using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }

    public float timeInRound = 60f;
    public float timeElapsed = 0f;

    public TextMeshProUGUI timerText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        timeElapsed = 0f;
    }

    public void StartTimer()
    {
        LuggageSpawner.Instance.SpawnLuggage();
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        while (timeElapsed < timeInRound)
        {
            yield return new WaitForEndOfFrame();
            timeElapsed += Time.deltaTime;

            // Update UI Timer
            float minutesLeft = Mathf.Floor((timeInRound - timeElapsed) / 60);
            float secondsLeft = Mathf.Floor((timeInRound - timeElapsed) % 60);
            float millisecondsLeft = Mathf.Floor(((timeInRound - timeElapsed) * 100) % 100);
            timerText.text = $"{minutesLeft:00}:{secondsLeft:00}:{millisecondsLeft:00}";
        }

        timerText.text = "00:00:00";
        SecurityScoring.Instance.RoundOver();
    }
}

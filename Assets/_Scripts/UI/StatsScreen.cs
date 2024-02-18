using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StatsScreen : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] string upgradeSceneName;
    [SerializeField] string lossSceneName;

    [Header("Stats Menu")]
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI ordersFulfilledText;
    [SerializeField] TextMeshProUGUI accuracyText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI rentText;
    [SerializeField] TextMeshProUGUI earningsText;
    [SerializeField] TextMeshProUGUI totalText;

    private void Start()
    {
        // Display our last day's stats
        DayStats stats = GameManager.Instance.GetLastDayStats();
        dayText.text             = "Day " + GameManager.Instance.GetDayNumber().ToString();
        ordersFulfilledText.text = stats.OrdersFulfilled.ToString();
        accuracyText.text        = Mathf.Ceil(stats.Accuracy * 100).ToString() + " %";
        timeText.text            = TimeSpan.FromSeconds(stats.Time).ToString(@"mm\:ss");
        rentText.text            = stats.Rent.ToString() + " $";
        earningsText.text        = stats.Score.ToString() + " $";
        totalText.text           = GameManager.Instance.TotalMoney + " $";
    }

    public void Continue()
    {
        if (GameManager.Instance.TotalMoney <= 0)
        {
            SceneLoader.Instance.LoadScene(lossSceneName);
            return;
        }
        SceneLoader.Instance.LoadScene(upgradeSceneName);
    }
}

public struct DayStats
{
    public int OrdersFulfilled;
    public float Accuracy;
    public float Time;
    public int Score;
    public int Rent;

    public DayStats(int ordersFulfilled, float accuracy, float time, int score, int rent)
    {
        this.OrdersFulfilled = ordersFulfilled;
        this.Accuracy = accuracy;
        this.Time = time;
        this.Score = score;
        this.Rent = rent;
    }
}
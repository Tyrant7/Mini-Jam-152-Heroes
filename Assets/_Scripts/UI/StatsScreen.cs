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

    [Header("Stats Menu")]
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI ordersFulfilledText;
    [SerializeField] TextMeshProUGUI accuracyText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI gradeText;

    private void Start()
    {
        // Display our last day's stats
        DayStats stats = GameManager.Instance.GetLastDayStats();
        dayText.text             = "Day " + GameManager.Instance.GetDayNumber().ToString();
        ordersFulfilledText.text = stats.OrdersFulfilled.ToString();
        accuracyText.text        = Mathf.Ceil(stats.Accuracy * 100).ToString() + " %";
        timeText.text            = TimeSpan.FromSeconds(stats.Time).ToString(@"mm\:ss");
        scoreText.text           = stats.Score.ToString() + " $";
        gradeText.text           = GetGrade(stats);
    }

    private string GetGrade(DayStats stats)
    {
        return "C";
    }

    public void Continue()
    {
        SceneLoader.Instance.LoadScene(upgradeSceneName);
    }
}

public struct DayStats
{
    public int OrdersFulfilled;
    public float Accuracy;
    public float Time;
    public int Score;

    public DayStats(int ordersFulfilled, float accuracy, float time, int score)
    {
        this.OrdersFulfilled = ordersFulfilled;
        this.Accuracy = accuracy;
        this.Time = time;
        this.Score = score;
    }
}
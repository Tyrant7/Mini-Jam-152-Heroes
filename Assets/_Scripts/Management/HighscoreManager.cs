using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighscoreManager : MonoBehaviour
{
    public static void TrySetHighscore(GameStats stats)
    {
        GameStats highscore = GetHighscore();
        if (stats.MoneyMade > highscore.MoneyMade &&
            stats.DaysInBusiness >= highscore.DaysInBusiness)
        {
            SetHighscore(stats);
        }
    }

    private static void SetHighscore(GameStats stats)
    {
        PlayerPrefs.SetInt("Days", stats.DaysInBusiness);
        PlayerPrefs.SetInt("Money", stats.MoneyMade);
        PlayerPrefs.Save();
    }

    public static GameStats GetHighscore()
    {
        return new GameStats(PlayerPrefs.GetInt("Days", 0), 0, PlayerPrefs.GetInt("Money", 0));
    }
}

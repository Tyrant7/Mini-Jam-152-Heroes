using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighscoreDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI daysText;
    [SerializeField] TextMeshProUGUI moneyText;

    private void Start()
    {
        DisplayHighscore(HighscoreManager.GetHighscore());
    }

    public void DisplayHighscore(GameStats stats)
    {
        daysText.text = stats.DaysInBusiness.ToString();
        moneyText.text = stats.MoneyMade.ToString() + " $";
    }
}

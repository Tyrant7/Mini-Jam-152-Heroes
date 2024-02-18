using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameEndScreen : MonoBehaviour
{
    [Header("Stats Menu")]
    [SerializeField] TextMeshProUGUI daysSurvivedText;
    [SerializeField] TextMeshProUGUI ordersFulfilledText;
    [SerializeField] TextMeshProUGUI totalMoneyMadeText;

    private void Start()
    {
        GameStats stats = GameManager.Instance.GetGameStats();
        daysSurvivedText.text = stats.DaysInBusiness.ToString();
        ordersFulfilledText.text = stats.OrdersFulfilled.ToString();
        totalMoneyMadeText.text = stats.MoneyMade.ToString() + " $";
    }

    public void Continue()
    {
        SceneLoader.Instance.LoadMainMenu();
    }
}

public struct GameStats
{
    public int DaysInBusiness;
    public int OrdersFulfilled;
    public int MoneyMade;

    public GameStats(int daysInBusiness, int ordersFulfilled, int moneyMade)
    {
        this.DaysInBusiness = daysInBusiness;
        this.OrdersFulfilled = ordersFulfilled;
        this.MoneyMade = moneyMade;
    }
}
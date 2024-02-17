using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UpgradeManager : MonoBehaviour
{
    #region Singleton

    public static UpgradeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    [SerializeField] Upgrade[] allUpgrades;

    private const int StoreSlots = 3;

    List<Upgrade> lockedUpgrades = new List<Upgrade>();
    List<Upgrade> unlockedUpgrades = new List<Upgrade>();

    private void Start()
    {
        lockedUpgrades = new List<Upgrade>(allUpgrades);
    }

    public Upgrade[] GetUpgrades()
    {
        List<Upgrade> validChoices = new List<Upgrade>(lockedUpgrades);
        int numberToTake = Mathf.Min(validChoices.Count, StoreSlots);
        Upgrade[] upgrades = new Upgrade[numberToTake];
        for (int i = 0; i < numberToTake; i++)
        {
            upgrades[i] = validChoices[UnityEngine.Random.Range(0, validChoices.Count)];
            validChoices.Remove(upgrades[i]);
        }
        return upgrades;
    }

    public bool PurchaseUpgrade(Upgrade upgrade)
    {
        if (GameManager.Instance.TotalMoney >= upgrade.Cost)
        {
            GameManager.Instance.TotalMoney -= upgrade.Cost;
            lockedUpgrades.Remove(upgrade);
            unlockedUpgrades.Add(upgrade);
            return true;
        }
        return false;
    }

    public bool HasUnlocked(string upgradeName)
    {
        foreach (Upgrade upgrade in unlockedUpgrades)
        {
            if (upgrade.Name == upgradeName)
            {
                return true;
            }
        }
        return false;
    }

    public UpgradeBonuses GetUpgradeBonuses()
    {
        List<FoodItem> foods = new List<FoodItem>();
        int customers = 0;
        foreach (Upgrade upgrade in unlockedUpgrades)
        {
            if (upgrade.Unlocks != null)
            {
                foods.Add(upgrade.Unlocks);
            }
            customers += upgrade.CustomerBonus;
        }
        return new UpgradeBonuses(foods.ToArray(), customers);
    }
}

public struct UpgradeBonuses
{
    public readonly FoodItem[] Unlocks;
    public readonly int CustomerBonus;

    public UpgradeBonuses(FoodItem[] unlocks, int customerBonus)
    {
        this.Unlocks = unlocks;
        this.CustomerBonus = customerBonus;
    }
}
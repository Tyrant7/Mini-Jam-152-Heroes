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
    [SerializeField] AudioClip purchaseSFX;

    private const int StoreSlots = 3;

    List<Upgrade> lockedUpgrades = new List<Upgrade>();
    List<Upgrade> unlockedUpgrades = new List<Upgrade>();

    private void Start()
    {
        ResetUpgrades();
    }

    public void ResetUpgrades()
    {
        lockedUpgrades = new List<Upgrade>(allUpgrades);
        unlockedUpgrades.Clear();
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

            // Show visual for new upgrade
            ActivateAllUnlocked activate = FindObjectOfType<ActivateAllUnlocked>();
            if (activate != null)
            {
                activate.ActivateAll(true);
            }
            AudioManager.PlayAudioClip(purchaseSFX);

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
        int dayLength = 0;
        int sandwichSize = 0;
        foreach (Upgrade upgrade in unlockedUpgrades)
        {
            if (upgrade.Unlocks != null)
            {
                foods.Add(upgrade.Unlocks);
            }
            customers += upgrade.CustomerBonus;
            dayLength += upgrade.DayLengthBonus;
            sandwichSize += upgrade.SandwichSizeBonus;
        }
        return new UpgradeBonuses(foods.ToArray(), customers, dayLength, sandwichSize);
    }
}

public struct UpgradeBonuses
{
    public readonly FoodItem[] Unlocks;
    public readonly int CustomerBonus;
    public readonly int DayLength;
    public readonly int SandwichSize;

    public UpgradeBonuses(FoodItem[] unlocks, int customerBonus, int dayLength, int sandwichSize)
    {
        this.Unlocks = unlocks;
        this.CustomerBonus = customerBonus;
        this.DayLength = dayLength;
        this.SandwichSize = sandwichSize;
    }
}